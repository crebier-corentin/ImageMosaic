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

        public static void CreateInputArchive(DirectoryInfo dir, string archiveName, int size = 20, int limit = 5000)
        {
            var prepareInput = new PrepareInput(dir, size, limit);
            prepareInput.CreateInputArchive(archiveName);
        }

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

        private void CreateImageInfosAndResize()
        {
            //Get file array 
            var files = ImageFinder.GetImages(_inputDir, _limit).ToArray();

            var total = files.Length;

            var bar = new ProgressBar(total, "Preparing Image");

            Parallel.ForEach(files, (file, state) =>
            {
                ResizeImage(file);

                var imageInfo = new ImageInfo {FileName = file.Name, AverageColor = GetAverageColor(file)};

                lock (_infoFile)
                {
                    _infoFile.ImageInfos.Add(imageInfo);
                }

                bar.Tick($"Preparing Image : {file.Name}");
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