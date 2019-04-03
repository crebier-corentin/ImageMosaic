using System;
using System.IO;
using CommandLine;
using ImageMagick;
using SharedClasses;

namespace MosaicGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    if (!File.Exists(options.InputPath))
                    {
                        throw new FileNotFoundException($"Cannot find file : {options.InputPath}");
                    }

                    using (new MagickTmpDir())
                    using (var archive = InputArchive.FromFile(options.InputPath))
                    using (var templateImage = new MagickImage(options.TemplatePath))
                    {
                        ImageAssembler.AssembleImage(templateImage, archive, options.OutputPath);

                        Console.WriteLine($"Done output file : {options.OutputPath}");
                    }
                });

          /*  using (var image = new MagickImage("aurora-borealis.png"))
            {
                image.Resize(new Percentage(20));
                image.Write($"small-aurora-borealis.png");
            }*/
        }
    }
}