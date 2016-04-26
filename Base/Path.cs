using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Edge.Arrays;
using Microsoft.WindowsAPICodePack.Shell;

namespace Edge.Path
{
    public static class LoadFiles
    {
        public static byte[] loadAsBytes(FileStream stream, int bufferSize = 4096)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var b = new ResizingArray<byte>();
            byte[] buffer = new byte[bufferSize];
            while (true)
            {
                var grabbed = stream.Read(buffer, 0, bufferSize);
                if (grabbed == 0)
                    break;
                b.AddRange(buffer.Take(grabbed));
            }
            return b.ToArray();
        }
    }
    namespace thumbnails
    {
        public static class FileThumbnails
        {
            public enum ThumbnailSize {none,Small, Medium, Large, ExtraLarge}
            public static Bitmap getthumbnailforfile(string filepath, ThumbnailSize s = ThumbnailSize.Medium)
            {
                ShellFile sf = ShellFile.FromFilePath(filepath);
                ShellThumbnail t = sf.Thumbnail;
                return getimagefromthumbnail(t, s);
            }
            public static Bitmap getthumbnailforfolder(string filepath, ThumbnailSize s = ThumbnailSize.Medium)
            {
                ShellObject sf = ShellObject.FromParsingName(filepath);
                ShellThumbnail t = sf.Thumbnail;
                return getimagefromthumbnail(t, s);
            }
            private static Bitmap getimagefromthumbnail(ShellThumbnail t, ThumbnailSize s = ThumbnailSize.none)
            {
                switch (s)
                {
                    case ThumbnailSize.Small:
                        return t.SmallBitmap;
                    case ThumbnailSize.Medium:
                        return t.MediumBitmap;
                    case ThumbnailSize.Large:
                        return t.LargeBitmap;
                    case ThumbnailSize.ExtraLarge:
                        return t.ExtraLargeBitmap;
                    default:
                        return t.Bitmap;
                }
            }
        }
    }
    public static class FilePath
    {
        public static string currentexeroot
        {
            get
            {
                return Assembly.GetCallingAssembly().GetName().CodeBase;
            }
        }
        public static bool IsFileAccessible(string filename,FileAccess access = FileAccess.ReadWrite)
        {
            try
            {
                FileStream fs = File.Open(filename, FileMode.Open, access, FileShare.None);
                fs.Close();
                return true;
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (IOException ex)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                return false;
            }
        }
        public static string MutateFileName(string filepath, Func<string, string> mutation)
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filepath), mutation(System.IO.Path.GetFileName(filepath)));
        }
    }
}
