using CommandLine;
using InTheHand.Bluetooth;

namespace letra200bsharp.Console
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<Options>(args);

            await parserResult.MapResult(async o =>
            {
                var bluetoothDevice = await BluetoothDevice.FromIdAsync(o.Address);

                if (bluetoothDevice != null)
                {
                    var services = await bluetoothDevice.Gatt.GetPrimaryServicesAsync();
                    var uuid = services.FirstOrDefault(s => s.Uuid.ToString().Length == 36)?.Uuid;
                    if (uuid.HasValue)
                    {
                        var serv = await bluetoothDevice.Gatt.GetPrimaryServiceAsync(uuid.Value);

                        var characteristics = await serv.GetCharacteristicsAsync();

                        if (characteristics != null && characteristics.Count > 0)
                        {
                            var imageBytes = await File.ReadAllBytesAsync(o.Image);
                            var job = LetraHelper.CreateJob(imageBytes);

                            foreach (var jobPart in job)
                            {
                                await characteristics[0].WriteValueWithoutResponseAsync(jobPart);
                            }
                        }
                        else
                        {
                            System.Console.WriteLine("Error: Unable to determine device characteristics.");
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Error: Unable to determine UUID.");
                    }
                }
                else
                {
                    System.Console.WriteLine("Error: Unable to connect to selected bluetooth device.");
                }
            }, errs =>
            {
                foreach (var err in errs)
                {
                    System.Console.WriteLine(err);
                }
                return Task.CompletedTask;
            });
        }
    }
}
