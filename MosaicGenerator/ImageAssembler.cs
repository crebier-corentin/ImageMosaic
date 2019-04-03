using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using SharedClasses;

namespace MosaicGenerator
{
    public class ImageAssembler
    {
        private ConcurrentDictionary<Point, string> _images = new ConcurrentDictionary<Point, string>();
        private InputArchive _archive;
        private IMagickImage _template;

        private ImageAssembler(IMagickImage template, InputArchive archive)
        {
            _template = template;
            _archive = archive;

            PickImages();
        }

        private void PickImages()
        {
            using (var pixels = _template.GetPixels())
            {
                var counter = 0;
                var total = pixels.Count();

                //Picker
                var allowDuplicates = _archive.GetImageInfos().Count < total;

                var picker = new ImagePicker(_archive, allowDuplicates);

                Parallel.ForEach(pixels, pixel =>
                {
                    Interlocked.Increment(ref counter);

                    var point = new Point(pixel.X, pixel.Y);

                    Console.WriteLine($"Picking image for pixel {point} - {counter}/{total}");

                    var filename = picker.MostFittingImageFilename(pixel.ToColor());
                    _images.TryAdd(point, filename);
                });
            }
        }

        private void CreateMosaic(string outputPath)
        {
            var pixelSize = _archive.GetImageInfos().Size;

            var outputWidth = _template.Width * pixelSize;
            var outputHeight = _template.Height * pixelSize;

            using (var output = new MagickImage(new MagickColor("#FFFFFF"), outputWidth, outputHeight)
                {Format = MagickFormat.Png})
            {
                using (var pixelCollection = _template.GetPixels())
                {
                    var pixelCountTotal = pixelCollection.Count();
                    var pixelCount = 0;

                    foreach (var pixel in pixelCollection)
                    {
                        Console.WriteLine($"Current pixel : {++pixelCount}/{pixelCountTotal}");

                        var outputX = pixel.X * pixelSize;
                        var outputY = pixel.Y * pixelSize;

                        using (var image = _archive.GetImage(_images[new Point(pixel.X, pixel.Y)]))
                        {
                            output.Composite(image, outputX, outputY);
                        }
                    }
                }

                output.Write(outputPath);
            }
        }

        public static void AssembleImage(IMagickImage template, InputArchive archive, string outputPath)
        {
            /*var pixelSize = archive.GetImageInfos().Size;

            var outputWidth = template.Width * pixelSize;
            var outputHeight = template.Height * pixelSize;

            var output = new MagickImage(new MagickColor("#FFFFFF"), outputWidth, outputHeight)
            {
                Format = MagickFormat.Png
            };

            using (var pixelCollection = template.GetPixels())
            {
                var pixelCountTotal = pixelCollection.Count();
                var pixelCount = 0;

                //Picker
                var allowDuplicates = archive.GetImageInfos().Count < pixelCountTotal;

                var picker = new ImagePicker(archive, allowDuplicates);

                foreach (var pixel in pixelCollection)
                {
                    Console.WriteLine($"Current pixel : {++pixelCount}/{pixelCountTotal}");

                    var color = pixel.ToColor();

                    var outputX = pixel.X * pixelSize;
                    var outputY = pixel.Y * pixelSize;

                    using (var image = picker.MostFittingImage(color))
                    {
                        output.Composite(image, outputX, outputY);
                    }
                }
            }


            output.Write(outputPath);

            output.Dispose();*/

            var assembler = new ImageAssembler(template, archive);
            assembler.CreateMosaic(outputPath);
        }
    }
}