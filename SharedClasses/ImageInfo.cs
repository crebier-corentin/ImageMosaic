using ImageMagick;
using System.Collections.Generic;

namespace SharedClasses
{
    public struct ImageInfo
    {
        public string FileName;
        public MagickColor AverageColor;
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