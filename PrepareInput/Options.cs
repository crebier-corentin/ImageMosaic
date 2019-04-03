using CommandLine;

namespace PrepareInput
{
    public class Options
    {
        [Value(0, Required = true, HelpText = "Input Directory")]
        public string InputDirectoryPath { get; set; }

        [Option('s', "size", Default = 20, HelpText = "Images size")]
        public int Size { get; set; }

        [Option('l', "limit", Default = 0,
            HelpText = "Maximum number of images taken from input directory (0 for no limit)")]
        public int Limit { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output filename")]
        public string OutputPath { get; set; }
    }
}