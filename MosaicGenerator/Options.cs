using CommandLine;

namespace MosaicGenerator
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input filename (zip file)")]
        public string InputPath { get; set; }

        [Option('t', "template", Required = true, HelpText = "Template image for mosaic")]
        public string TemplatePath { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output filename")]
        public string OutputPath { get; set; }
    }
}