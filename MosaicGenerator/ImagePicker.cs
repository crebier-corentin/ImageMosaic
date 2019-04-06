using ImageMagick;
using MoreLinq;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MosaicGenerator
{
    public class ImagePicker
    {
        private LinkedList<ImageInfo> _imageInfos;
        private InputArchive _archive;
        private readonly bool _allowDuplicate;

        public ImagePicker(InputArchive archive, bool allowDuplicate = true)
        {
            _archive = archive;
            _allowDuplicate = allowDuplicate;
            _imageInfos = new LinkedList<ImageInfo>(archive.GetImageInfos().ImageInfos);
        }

        public MagickImage MostFittingImage(MagickColor color)
        {
            return _archive.GetImage(MostFittingImageFilename(color));
        }

        public string MostFittingImageFilename(MagickColor color)
        {
            var image = _imageInfos.Where(i => i.Available).MinBy(i => ColorEuclideanDistance(i.AverageColor, color))
                .First();

            //Duplicate
            if (!_allowDuplicate)
            {
                image.Available = false;
            }

            return image.FileName;
        }

        private static double ColorEuclideanDistance(MagickColor color1, MagickColor color2)
        {
            var r = Math.Pow(color1.R - color2.R, 2);
            var g = Math.Pow(color1.G - color2.G, 2);
            var b = Math.Pow(color1.B - color2.B, 2);

            return Math.Sqrt(r + g + b);
        }
    }
}