using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using SharedClasses;
using ShellProgressBar;

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
                var total = pixels.Count();

                //Picker
                var allowDuplicates = _archive.GetImageInfos().Count < total;

                var picker = new ImagePicker(_archive, allowDuplicates);

                //Bar
                using (var bar = new ProgressBar(total, "Picking images"))
                {
                    Parallel.ForEach(pixels, pixel =>
                    {
                        var point = new Point(pixel.X, pixel.Y);

                        var filename = picker.MostFittingImageFilename(pixel.ToColor());
                        _images.TryAdd(point, filename);

                        bar.Tick($"Picking image for pixel {point}");
                    });
                }
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

                    //Bar
                    using (var bar = new ProgressBar(pixelCountTotal, "Creating mosaic"))
                    {
                        foreach (var pixel in pixelCollection)
                        {
                            var outputX = pixel.X * pixelSize;
                            var outputY = pixel.Y * pixelSize;

                            using (var image = _archive.GetImage(_images[new Point(pixel.X, pixel.Y)]))
                            {
                                output.Composite(image, outputX, outputY);
                            }

                            bar.Tick();
                        }
                    }
                }

                output.Write(outputPath);
            }
        }

        public static void AssembleImage(IMagickImage template, InputArchive archive, string outputPath)
        {
            var assembler = new ImageAssembler(template, archive);
            assembler.CreateMosaic(outputPath);
        }
    }
}