using CommandLine;

namespace letra200bsharp.Console
{
    internal class Options
    {
        [Option("image", Required = true, HelpText = "Path to image file.")]
        public required string Image { get; set; }

        [Option("address", Required = true, HelpText = "Address of bluetooth device.")]
        public required string Address { get; set; }
    }
}
