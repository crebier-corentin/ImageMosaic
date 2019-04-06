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

        private static IEnumerable<FileInfo> GetFiles(DirectoryInfo rootDir, bool recursiveSearch)
        {
            foreach (var file in rootDir.EnumerateFiles())
            {
                yield return file;
            }

            //Recursive Search
            if (!recursiveSearch) yield break;

            foreach (var dir in rootDir.GetDirectories())
            {
                foreach (var image in GetFiles(dir, true))
                {
                    yield return image;
                }
            }
        }

        public static IEnumerable<FileInfo> GetImages(DirectoryInfo rootDir, bool recursiveSearch, int limit)
        {
            var count = 0;

            if (_bar == null)
            {
                _bar = new ProgressBar(limit);
            }

            foreach (var file in GetFiles(rootDir, recursiveSearch))
            {
                //Limit
                if (limit > 0 && count >= limit)
                {
                    yield break;
                }

                //Image
                if (CanOpenImage(file))
                {
                    _bar.Tick($"Searching image : {file.Name}");

                    count++;

                    yield return file;
                }
            }
        }

        private static bool CanOpenImage(FileInfo file)
        {
            var allowedExtensions = new[] {".png", ".jpg", ".jpeg", ".gif", ".bmp", ".svg", ".tiff"};

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