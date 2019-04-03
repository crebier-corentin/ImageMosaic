using System;
using System.IO;
using System.IO.Compression;
using ImageMagick;

namespace SharedClasses
{
    public class TmpDir : IDisposable
    {
        public readonly DirectoryInfo Dir;

        public TmpDir(string name = "tmp")
        {
            //Delete previous if exist
            if (Directory.Exists(name))
            {
                DeleteDirectory(name);
            }

            //Create Tmp dir
            Dir = Directory.CreateDirectory(name);

            Console.WriteLine($"Created tmp dir : {Dir.FullName}");
        }

        public void CreateArchive(string name)
        {
            //Delete previous if exist
            if (File.Exists(name))
            {
                File.Delete(name);
            }
            
            ZipFile.CreateFromDirectory(Dir.FullName, name);
        }

        public FileStream GetCreateStream(string filename)
        {
            var filePath = Path.Combine(Dir.FullName, filename);

            //Delete previous if exist
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return File.Create(filePath);
        }

        public void Dispose()
        {
            //Delete tmp dir
            DeleteDirectory(Dir.FullName);

            Console.WriteLine($"Deleted tmp dir : {Dir.FullName}");
        }

        public static void DeleteDirectory(string path)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }
    }
}