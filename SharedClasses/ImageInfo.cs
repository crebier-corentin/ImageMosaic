using ImageMagick;
using System.Collections.Generic;

namespace SharedClasses
{
    public class ImageInfo
    {
        public string FileName;
        public MagickColor AverageColor;
        public bool Available = true;
    }

    public class InfoFile
    {
        public readonly List<ImageInfo> ImageInfos;
        public int Size;

        public int Count => ImageInfos.Count;

        public InfoFile()
        {
            ImageInfos = new List<ImageInfo>();
        }
    }
}