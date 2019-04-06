using ImageMagick;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;

namespace SharedClasses
{
    public class InputArchive : IDisposable
    {
        private ZipArchive _archive;
        private string _archiveName;
        private FileStream _fileStream;

        private InputArchive()
        {
        }

        public static InputArchive FromFile(string filename)
        {
            return new InputArchive { _archive = ZipFile.Open(filename, ZipArchiveMode.Read), _archiveName = filename };
        }

        public static InputArchive Create(string filename)
        {
            var archive = new InputArchive { _fileStream = File.Create(filename), _archiveName = filename };
            archive._archive = new ZipArchive(archive._fileStream, ZipArchiveMode.Update);

            return archive;
        }

        /*  public void AddFile(string name, Stream stream)
          {
              var entry = _archive.CreateEntry(name);

              using (var entryStream = entry.Open())
              {
                  stream.CopyTo(entryStream);
              }
          }

          public void AddFile(string name, string content)
          {
              var entry = _archive.CreateEntry(name);

              using (var entryStream = new StreamWriter(entry.Open()))
              {
                  entryStream.Write(content);
              }
          }

        public Stream GetCreateStream(string name)
        {
            return _archive.CreateEntry(name).Open();
        }

        public void CreateInfoFile(InfoFile list)
        {
            var entry = _archive.CreateEntry("info.json");
            using (var stream = new StreamWriter(entry.Open()))
            {
                stream.Write(JsonConvert.SerializeObject(list));
            }
        }*/

        public InfoFile GetImageInfos()
        {
            var entry = _archive.GetEntry("info.json");

            using (var stream = new StreamReader(entry.Open()))
            {
                return JsonConvert.DeserializeObject<InfoFile>(stream.ReadToEnd());
            }
        }

        public MagickImage GetImage(string name)
        {
            var entry = _archive.GetEntry(name);

            using (var stream = entry.Open())
            {
                return new MagickImage(stream);
            }
        }

        public void SaveAndReload()
        {
            _archive.Dispose();
            _archive = ZipFile.Open(_archiveName, ZipArchiveMode.Update);
        }

        public void Dispose()
        {
            _archive?.Dispose();
            _fileStream?.Dispose();
        }
    }
}