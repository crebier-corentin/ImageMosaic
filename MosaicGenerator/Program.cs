using CommandLine;
using ImageMagick;
using SharedClasses;
using System;
using System.IO;

namespace MosaicGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    if (!File.Exists(options.InputPath))
                    {
                        throw new FileNotFoundException($"Cannot find file : {options.InputPath}");
                    }

                    using (new MagickTmpDir())
                    {
                        using var archive = InputArchive.FromFile(options.InputPath);
                        using var templateImage = new MagickImage(options.TemplatePath);
                        ImageAssembler.AssembleImage(templateImage, archive, options.OutputPath, options.Duplicates);

                        Console.WriteLine($"Done output file : {options.OutputPath}");
                    }
                });
        }
    }
}