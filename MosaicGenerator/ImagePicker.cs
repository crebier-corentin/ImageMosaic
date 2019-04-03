using System;
using System.Collections.Generic;
using System.Linq;
using ImageMagick;
using SharedClasses;
using MoreLinq;

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
            /*var mostFittingNode = _imageInfos.First;
            var mostFittingValue = ColorEuclideanDistance(mostFittingNode.Value.AverageColor, color);

            //Find the most fitting image
            for (var node = mostFittingNode.Next; node != null; node = node.Next)
            {
                var imageInfo = node.Value;
                var val = ColorEuclideanDistance(imageInfo.AverageColor, color);

                if (!(val < mostFittingValue)) continue;

                mostFittingNode = node;
                mostFittingValue = val;
            }*/

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