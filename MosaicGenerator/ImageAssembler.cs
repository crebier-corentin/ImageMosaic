using ImageMagick;
using SharedClasses;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace MosaicGenerator
{
    public class ImageAssembler
    {
        private readonly Dictionary<Point, string> _images = new Dictionary<Point, string>();
        private readonly InputArchive _archive;
        private readonly IMagickImage _template;

        private ImageAssembler(IMagickImage template, InputArchive archive, bool duplicates)
        {
            _template = template;
            _archive = archive;

            PickImages(duplicates);
        }

        private void PickImages(bool duplicates)
        {
            using var pixels = _template.GetPixels();
            var total = pixels.Count();

            //Picker
            var picker = new ImagePicker(_archive, duplicates, total);

            //Bar
            var bar = new ProgressBar(total, "Picking images");

            foreach (var pixel in pixels)
            {
                var point = new Point(pixel.X, pixel.Y);

                var filename = picker.MostFittingImageFilename(pixel.ToColor());
                _images.Add(point, filename);

                bar.Tick($"Picking image for pixel {point}");
            }
        }

        private void CreateMosaic(string outputPath)
        {
            var pixelSize = _archive.GetImageInfos().Size;

            var outputWidth = _template.Width * pixelSize;
            var outputHeight = _template.Height * pixelSize;

            using var output = new MagickImage(new MagickColor("#FFFFFF"), outputWidth, outputHeight)
                {Format = MagickFormat.Png};
            using var pixelCollection = _template.GetPixels();

            var pixelCountTotal = pixelCollection.Count();

            //Bar
            var bar = new ProgressBar(pixelCountTotal, "Assembling mosaic");

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


            output.Write(outputPath);
        }

        public static void AssembleImage(IMagickImage template, InputArchive archive, string outputPath, bool duplicates)
        {
            var assembler = new ImageAssembler(template, archive, duplicates);
            assembler.CreateMosaic(outputPath);
        }
    }
}