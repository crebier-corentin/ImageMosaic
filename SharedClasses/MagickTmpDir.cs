using System;
using System.IO;
using ImageMagick;

namespace SharedClasses
{
    public class MagickTmpDir : TmpDir
    {
        public MagickTmpDir(string name = "tmp") : base(name)
        {
            MagickNET.SetTempDirectory(Dir.FullName);
        }
    }
}