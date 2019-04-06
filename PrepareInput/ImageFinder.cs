using ImageMagick;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PrepareInput
{
    public static class ImageFinder
    {
        private static ProgressBar _bar = null;

        public class Count
        {
            public int Value;
        }

        public static IEnumerable<FileInfo> GetImages(DirectoryInfo rootDir, int limit, Count count = null)
        {
            if (count == null)
            {
                count = new Count();
            }

            if (_bar == null)
            {
                _bar = new ProgressBar(limit);
            }

            foreach (var file in rootDir.EnumerateFiles())
            {
                //Limit
                if (limit > 0 && count.Value >= limit)
                {
                    yield break;
                }

                //Image
                if (CanOpenImage(file))
                {
                    _bar.Tick($"Finding image : {file.Name}");

                    count.Value++;

                    yield return file;
                }
            }

            foreach (var dir in rootDir.GetDirectories())
            {
                foreach (var image in GetImages(dir, limit, count))
                {
                    yield return image;
                }
            }
        }

        private static bool CanOpenImage(FileInfo file)
        {
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".svg", ".tiff" };

            if (!allowedExtensions.Contains(file.Extension.ToLower()))
            {
                return false;
            }

            try
            {
                using (new MagickImage(file))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}