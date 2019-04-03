using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using Newtonsoft.Json;
using SharedClasses;

namespace PrepareInput
{
    public class PrepareInput
    {
        private readonly InfoFile _infoFile = new InfoFile();
        private readonly DirectoryInfo _inputDir;
        private TmpDir _outputDir;
        private readonly int _limit;


        public PrepareInput(DirectoryInfo dir, int size = 20, int limit = 5000)
        {
            _inputDir = dir;
            _infoFile.Size = size;
            _limit = limit;
        }

        private void CreateInputArchive(string archiveName)
        {
            //Create Archive
            using (_outputDir = new TmpDir("output_tmp"))
            {
                CreateImageInfosAndResize();

                //Add info file
                using (var writer = new StreamWriter(_outputDir.GetCreateStream("info.json")))
                {
                    writer.Write(JsonConvert.SerializeObject(_infoFile));
                }

                _outputDir.CreateArchive(archiveName);
            }
        }

        public static void CreateInputArchive(DirectoryInfo dir, string archiveName, int size = 20, int limit = 5000)
        {
            var prepareInput = new PrepareInput(dir, size, limit);
            prepareInput.CreateInputArchive(archiveName);
        }

        private void CreateImageInfosAndResize()
        {
            //Get file array 
            var files = ImageFinder.GetImages(_inputDir, _limit).ToArray();

            var counter = 0;
            var total = files.Length;

            Parallel.ForEach(files, (file, state) =>
            {
                Interlocked.Increment(ref counter);

                Console.WriteLine($"Preparing image : {file.Name} - {counter}/{total}");

                ResizeImage(file);

                var imageInfo = new ImageInfo {FileName = file.Name, AverageColor = GetAverageColor(file)};

                lock (_infoFile)
                {
                    _infoFile.ImageInfos.Add(imageInfo);
                }
            });
        }

        private void ResizeImage(FileInfo fileInfo)
        {
            using (var magicImage = new MagickImage(fileInfo))
            {
                var size = new MagickGeometry(_infoFile.Size, _infoFile.Size) {IgnoreAspectRatio = true};

                magicImage.Resize(size);

                //Save image to dir
                magicImage.Write(Path.Combine(_outputDir.Dir.FullName, fileInfo.Name), MagickFormat.Jpg);
            }
        }

        private static MagickColor GetAverageColor(FileInfo fileInfo)
        {
            using (var image = new MagickImage(fileInfo))
            using (var pixels = image.GetPixels())
            {
                var averageColor = new MagickColor();

                //Get Average of Red Green And Blue
                averageColor.R = (byte) pixels.Average(pixel => pixel.ToColor().R);
                averageColor.G = (byte) pixels.Average(pixel => pixel.ToColor().G);
                averageColor.B = (byte) pixels.Average(pixel => pixel.ToColor().B);

                return averageColor;
            }
        }
    }
}