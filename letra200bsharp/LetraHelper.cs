using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace letra200bsharp
{
    public class LetraHelper
    {
        /// <summary>
        /// Byte array represeinting the form feed command
        /// </summary>
        private static readonly byte[] FormFeed = new byte[2] { 0x1B, 0x45 };

        /// <summary>
        /// Byte array representing the status command
        /// </summary>
        private static readonly byte[] Status = new byte[2] { 0x1B, 0x41 };

        /// <summary>
        /// Byte array indicating the end of data
        /// </summary>
        private static readonly byte[] End = new byte[2] { 0x1B, 0x51 };

        /// <summary>
        /// Byte array representing the start of the job
        /// </summary>
        private static readonly byte[] StartJob = new byte[6] { 0x1B, 0x73, 0x9A, 0x02, 0x00, 0x00 };

        private static int CalculateChecksum(byte[] data)
        {
            var checksum = 0;
            foreach (byte b in data)
            {
                checksum += b;
            }
            return checksum & 0xFF;
        }

        /// <summary>
        /// Split data in chunks
        /// </summary>
        /// <param name="data"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        private static List<byte[]> SplitChunks(byte[] data, int chunkSize = 100)
        {
            var chunks = new List<byte[]>();

            for (int i = 0; i < data.Length; i += chunkSize)
            {
                int end = Math.Min(i + chunkSize, data.Length);
                byte[] chunk;

                // Determine if we need to append extra bytes
                if (end == data.Length) // Last chunk
                {
                    chunk = new byte[end - i + 3]; // 2 extra bytes
                }
                else
                {
                    chunk = new byte[end - i + 1]; // No extra bytes
                }

                chunk[0] = (byte)(i / chunkSize);

                // Copy the relevant bytes to the chunk
                for (int j = 0; j < end - i; j++)
                {
                    chunk[j + 1] = data[i + j];
                }

                // Append extra bytes to the last chunk
                if (end == data.Length)
                {
                    chunk[chunk.Length - 2] = 0x12;
                    chunk[chunk.Length - 1] = 0x34;
                }

                chunks.Add(chunk);
            }

            return chunks;
        }

        /// <summary>
        /// Prepare image in three steps:
        /// - Convert to 1-bit monochrome
        /// - Rotate the image
        /// - Convert the image to an array of 1s and 0s
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <returns></returns>
        private static ImageInfo PrepareImage(byte[] imageBytes)
        {
            // Load image from byte array
            using (var stream = new System.IO.MemoryStream(imageBytes))
            {
                using (var skiaImage = SKBitmap.Decode(stream))
                {
                    // Convert to 1-bit monochrome
                    var monoBitmap = new SKBitmap(skiaImage.Width, skiaImage.Height);
                    for (int x = 0; x < skiaImage.Width; x++)
                    {
                        for (int y = 0; y < skiaImage.Height; y++)
                        {
                            var color = skiaImage.GetPixel(x, y);
                            // Set pixel black or white according to the brightness
                            monoBitmap.SetPixel(x, y, color.Red < 128 ? SKColors.Black : SKColors.White);
                        }
                    }

                    // Rotate the image (the printer expects a portrait image)
                    monoBitmap = RotateBitmap(monoBitmap);

                    // Resize the image to a width of 32 pixels
                    int width = 32;
                    int height = (int)(64 * ((float)monoBitmap.Height / monoBitmap.Width));
                    var resizedBitmap = monoBitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.High);

                    // Convert the image to an array of 1s and 0s
                    byte[] data = new byte[resizedBitmap.Width * resizedBitmap.Height];
                    for (int i = 0; i < resizedBitmap.Width; i++)
                    {
                        for (int j = 0; j < resizedBitmap.Height; j++)
                        {
                            var pixel = resizedBitmap.GetPixel(i, j);
                            data[i + j * resizedBitmap.Width] = pixel.Red < 128 ? (byte)1 : (byte)0;
                        }
                    }

                    return new ImageInfo { Width = height, Height = width, Data = data };
                }
            }
        }

        /// <summary>
        /// Rotates a SkiaSharp SKBitmap object
        /// </summary>
        /// <param name="bitmap">Rotated version of the SKBitmap object</param>
        /// <returns></returns>
        private static SKBitmap RotateBitmap(SKBitmap bitmap)
        {
            double radians = Math.PI * 270 / 180;
            float sine = (float)Math.Abs(Math.Sin(radians));
            float cosine = (float)Math.Abs(Math.Cos(radians));
            int originalWidth = bitmap.Width;
            int originalHeight = bitmap.Height;
            int rotatedWidth = (int)(cosine * originalWidth + sine * originalHeight);
            int rotatedHeight = (int)(cosine * originalHeight + sine * originalWidth);

            var rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);

            using (var surface = new SKCanvas(rotatedBitmap))
            {
                surface.Clear();
                surface.Translate(rotatedWidth / 2, rotatedHeight / 2);
                surface.RotateDegrees((float)270);
                surface.Translate(-originalWidth / 2, -originalHeight / 2);
                surface.DrawBitmap(bitmap, new SKPoint());
            }

            return rotatedBitmap;
        }

        private static byte[] GetHeaderBytes(int length)
        {
            var lengthBytes = BitConverter.GetBytes(length);
            var header = new byte[5 + lengthBytes.Length];
            header[0] = 0xFF; // preamble
            header[1] = 0xF0; // flags
            header[2] = 0x12; // magic
            header[3] = 0x34; // magic
            Array.Copy(lengthBytes, 0, header, 4, lengthBytes.Length);
            header[header.Length - 1] = (byte)CalculateChecksum(header.Take(header.Length - 1).ToArray());
            return header;
        }

        private static byte[] GetPrintData(byte[] data, int width, int height)
        {
            if (width * height != data.Length * 8)
            {
                throw new ArgumentException($"Data does not match dimensions ({width}*{height}!={data.Length * 8})");
            }

            var printData = new List<byte> { 0x1B, 0x44, 0x01, 0x02 };
            printData.AddRange(BitConverter.GetBytes(width));
            printData.AddRange(BitConverter.GetBytes(height));
            printData.AddRange(data);
            return printData.ToArray();
        }

        /// <summary>
        /// A job always consists of the following parts:
        // - header bytes
        // - chunked body consisting of:
        //   - start job
        //   - print data
        //   - form feed
        //   - status
        //   - end
        /// </summary>
        /// <param name="image"></param>
        /// <returns>List of byte arrays containing the data to be sent to the Dymo Letra 200b</returns>
        public static List<byte[]> CreateJob(byte[] imageBytes)
        {
            var imageInfo = PrepareImage(imageBytes);
            byte[] packedData = new byte[(int)Math.Ceiling(imageInfo.Data.Length / 8.0)];
            for (int i = 0; i < imageInfo.Data.Length; i++)
            {
                if (imageInfo.Data[i] == 1)
                    packedData[i / 8] |= (byte)(1 << (i % 8));
            }

            var body = new List<byte>();
            body.AddRange(StartJob);
            body.AddRange(GetPrintData(packedData, imageInfo.Width, imageInfo.Height));
            body.AddRange(FormFeed);
            body.AddRange(Status);
            body.AddRange(End);

            byte[] header = GetHeaderBytes(body.Count);
            var chunks = SplitChunks(body.ToArray());

            var result = new List<byte[]> { header };
            result.AddRange(chunks);
            return result;
        }
    }
}
