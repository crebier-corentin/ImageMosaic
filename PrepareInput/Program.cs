using CommandLine;
using SharedClasses;
using System;
using System.IO;

namespace PrepareInput
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    if (!Directory.Exists(options.InputDirectoryPath))
                    {
                        throw new DirectoryNotFoundException($"Cannot find directory : {options.InputDirectoryPath}");
                    }

                    if (options.Size <= 0)
                    {
                        throw new Exception("Size must be superior to 0");
                    }

                    using (new MagickTmpDir())
                    {
                        var inputDir = new DirectoryInfo(options.InputDirectoryPath);

                        PrepareInput.CreateInputArchive(inputDir, options.OutputPath, options.Size, options.Limit);

                        Console.WriteLine($"Done output file : {options.OutputPath}");
                    }
                });
        }
    }
}