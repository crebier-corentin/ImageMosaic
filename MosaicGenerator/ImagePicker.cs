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

        public ImagePicker(InputArchive archive, bool allowDuplicate, int total)
        {
            _archive = archive;
            _allowDuplicate = allowDuplicate;
            _imageInfos = new LinkedList<ImageInfo>(archive.GetImageInfos().ImageInfos);

            if (_archive.GetImageInfos().Count < total)
            {
                //Get the minimum amount of duplications to surpass the total
                var dupAmount = Math.Ceiling(total / (double) _archive.GetImageInfos().Count);
                Console.WriteLine($"Not enough images, duplicating the list {dupAmount} times.");
                DuplicateImages((int) dupAmount);
            }
        }

        private void DuplicateImages(int times)
        {
            IEnumerable<ImageInfo> tmpList = new ImageInfo[0];
            for (var i = 0; i < times; i++)
            {
                var clone = new LinkedList<ImageInfo>(_imageInfos);
                tmpList = tmpList.Concat(clone);
            }

            _imageInfos = new LinkedList<ImageInfo>(tmpList);
        }

        public MagickImage MostFittingImage(MagickColor color)
        {
            return _archive.GetImage(MostFittingImageFilename(color));
        }

        public string MostFittingImageFilename(MagickColor color)
        {
            var node = _imageInfos.EnumerateNodes().MinBy(i => ColorEuclideanDistance(i.Value.AverageColor, color))
                .First();

            //Duplicate
            if (!_allowDuplicate)
            {
                _imageInfos.Remove(node);
            }

            return node.Value.FileName;
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