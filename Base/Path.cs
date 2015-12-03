using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using Edge.Arrays;
using Edge.Streams;
using Microsoft.WindowsAPICodePack.Shell;

namespace Edge.Path
{
    namespace texts
    {
        public static class SaveLoadText
        {
            public static string loadasstring(string path, bool addnewlines = true)
            {
                StreamReader sr = new StreamReader(path);
                return sr.Loop().ToPrintable(addnewlines ? Environment.NewLine : "", "", "");
            }

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
    }
}
