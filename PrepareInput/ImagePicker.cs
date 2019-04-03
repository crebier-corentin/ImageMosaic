using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageMagick;

namespace PrepareInput
{
    public class ImagePicker
    {
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
                    Console.WriteLine($"Finding image : {file.Name} - {count.Value}/{limit}");

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