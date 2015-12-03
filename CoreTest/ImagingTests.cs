using System;
using System.Drawing;
using System.Linq;
using Edge.Imaging;
using Edge.Looping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class MiscImnagingTests
    {
        [TestMethod] public void GetUnusedColor()
        {
            const int size = 9;
            Bitmap b = new Bitmap(size, size);
            LockBitmap l = new LockBitmap(b);
            l.LockBits();
            int k = 0;
            foreach (var c in Loops.Range(l.Width).Join(Loops.Range(l.Height)))
            {
                l[c.Item1, c.Item2] = Color.FromArgb(k);
                k++;
            }
            l.UnlockBits();
            AreEqual(Color.FromArgb(size * size), b.getUnusedColor());
        }
        [TestMethod] public void isWithin()
        {
            const int size = 9;
            Bitmap b = new Bitmap(size, size);
            IsTrue(b.isWithin(new Point(5, 8)));
            IsTrue(b.isWithin(new Point(1, 0)));
            IsTrue(b.isWithin(new Point(8, 8)));
            IsTrue(b.isWithin(new Point(8, 2)));
            IsFalse(b.isWithin(new Point(-1, 5)));
            IsFalse(b.isWithin(new Point(-1, -1)));
            IsFalse(b.isWithin(new Point(-1, 10)));
            IsFalse(b.isWithin(new Point(10, 10)));
        }
        [TestMethod] public void EnumeratePixelsImage()
        {
            const int size = 9;
            Bitmap b = new Bitmap(size, size);
            LockBitmap l = new LockBitmap(b);
            l.LockBits();
            int k = 0;
            foreach (var c in Loops.Range(l.Width).Join(Loops.Range(l.Height)))
            {
                l[c.Item1, c.Item2] = Color.FromArgb(k);
                k++;
            }
            l.UnlockBits();
            IsTrue(b.EnumeratePixels().Select(a => a.ToArgb()).OrderBy().SequenceEqual(Loops.Range(size * size)));
        }
        [TestMethod] public void EnumeratePixelsLock()
        {
            const int size = 9;
            Bitmap b = new Bitmap(size, size);
            LockBitmap l = new LockBitmap(b);
            l.LockBits();
            int k = 0;
            foreach (var c in Loops.Range(l.Width).Join(Loops.Range(l.Height)))
            {
                l[c.Item1, c.Item2] = Color.FromArgb(k);
                k++;
            }
            l.UnlockBits();
            l.LockBits();
            IsTrue(l.EnumeratePixels().Select(a => a.ToArgb()).OrderBy().SequenceEqual(Loops.Range(size * size)));
            l.UnlockBits();
        }
        [TestMethod] public void EnumerateBorderImage()
        {
            const int size = 3;
            Bitmap b = new Bitmap(size, size);
            LockBitmap l = new LockBitmap(b);
            l.LockBits();
            int k = 0;
            foreach (var c in Loops.Range(l.Width).Join(Loops.Range(l.Height)))
            {
                l[c.Item2, c.Item1] = Color.FromArgb(k);
                k++;
            }
            l.UnlockBits();
            IsTrue(b.EnumerateBorders(ImageExtentions.BorderType.Bottom).Select(a => a.ToArgb()).OrderBy().SequenceEqual(new int[] {6, 7, 8}));
            IsTrue(b.EnumerateBorders(ImageExtentions.BorderType.Left).Select(a => a.ToArgb()).OrderBy().SequenceEqual(new int[] {0, 3, 6}));
            IsTrue(b.EnumerateBorders(ImageExtentions.BorderType.Right).Select(a => a.ToArgb()).OrderBy().SequenceEqual(new int[] {2, 5, 8}));
            IsTrue(b.EnumerateBorders(ImageExtentions.BorderType.Top).Select(a => a.ToArgb()).OrderBy().SequenceEqual(new int[] {0, 1, 2}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.Bottom | ImageExtentions.BorderType.Left).Select(a => a.ToArgb()).OrderBy()
                 .SequenceEqual(new int[] {0, 3, 6, 7, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.Bottom | ImageExtentions.BorderType.Right).Select(a => a.ToArgb()).OrderBy()
                 .SequenceEqual(new int[] {2, 5, 6, 7, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.Bottom | ImageExtentions.BorderType.Top).Select(a => a.ToArgb()).OrderBy().SequenceEqual
                    (new int[] {0, 1, 2, 6, 7, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.Left | ImageExtentions.BorderType.Right).Select(a => a.ToArgb()).OrderBy().SequenceEqual
                    (new int[] {0, 2, 3, 5, 6, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.Left | ImageExtentions.BorderType.Top).Select(a => a.ToArgb()).OrderBy().SequenceEqual(
                    new int[] {0, 1, 2, 3, 6}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.Right | ImageExtentions.BorderType.Top).Select(a => a.ToArgb()).OrderBy().SequenceEqual(
                    new int[] {0, 1, 2, 5, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.All & ~ImageExtentions.BorderType.Bottom).Select(a => a.ToArgb()).OrderBy()
                 .SequenceEqual(new int[] {0, 1, 2, 3, 5, 6, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.All & ~ImageExtentions.BorderType.Left).Select(a => a.ToArgb()).OrderBy().SequenceEqual(
                    new int[] {0, 1, 2, 5, 6, 7, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.All & ~ImageExtentions.BorderType.Right).Select(a => a.ToArgb()).OrderBy().SequenceEqual
                    (new int[] {0, 1, 2, 3, 6, 7, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.All & ~ImageExtentions.BorderType.Bottom).Select(a => a.ToArgb()).OrderBy()
                 .SequenceEqual(new int[] {0, 1, 2, 3, 5, 6, 8}));
            IsTrue(b.EnumerateBorders().Select(a => a.ToArgb()).OrderBy().SequenceEqual(new int[] {0, 1, 2, 3, 5, 6, 7, 8}));
        }
        [TestMethod] public void EnumerateBorderLock()
        {
            const int size = 3;
            Bitmap l = new Bitmap(size, size);
            LockBitmap b = new LockBitmap(l);
            b.LockBits();
            int k = 0;
            foreach (var c in Loops.Range(l.Width).Join(Loops.Range(l.Height)))
            {
                b[c.Item2, c.Item1] = Color.FromArgb(k);
                k++;
            }
            b.UnlockBits();
            IsTrue(b.EnumerateBorders(ImageExtentions.BorderType.Bottom).Select(a => a.ToArgb()).OrderBy().SequenceEqual(new int[] {6, 7, 8}));
            IsTrue(b.EnumerateBorders(ImageExtentions.BorderType.Left).Select(a => a.ToArgb()).OrderBy().SequenceEqual(new int[] {0, 3, 6}));
            IsTrue(b.EnumerateBorders(ImageExtentions.BorderType.Right).Select(a => a.ToArgb()).OrderBy().SequenceEqual(new int[] {2, 5, 8}));
            IsTrue(b.EnumerateBorders(ImageExtentions.BorderType.Top).Select(a => a.ToArgb()).OrderBy().SequenceEqual(new int[] {0, 1, 2}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.Bottom | ImageExtentions.BorderType.Left).Select(a => a.ToArgb()).OrderBy()
                 .SequenceEqual(new int[] {0, 3, 6, 7, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.Bottom | ImageExtentions.BorderType.Right).Select(a => a.ToArgb()).OrderBy()
                 .SequenceEqual(new int[] {2, 5, 6, 7, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.Bottom | ImageExtentions.BorderType.Top).Select(a => a.ToArgb()).OrderBy().SequenceEqual
                    (new int[] {0, 1, 2, 6, 7, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.Left | ImageExtentions.BorderType.Right).Select(a => a.ToArgb()).OrderBy().SequenceEqual
                    (new int[] {0, 2, 3, 5, 6, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.Left | ImageExtentions.BorderType.Top).Select(a => a.ToArgb()).OrderBy().SequenceEqual(
                    new int[] {0, 1, 2, 3, 6}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.Right | ImageExtentions.BorderType.Top).Select(a => a.ToArgb()).OrderBy().SequenceEqual(
                    new int[] {0, 1, 2, 5, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.All & ~ImageExtentions.BorderType.Bottom).Select(a => a.ToArgb()).OrderBy()
                 .SequenceEqual(new int[] {0, 1, 2, 3, 5, 6, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.All & ~ImageExtentions.BorderType.Left).Select(a => a.ToArgb()).OrderBy().SequenceEqual(
                    new int[] {0, 1, 2, 5, 6, 7, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.All & ~ImageExtentions.BorderType.Right).Select(a => a.ToArgb()).OrderBy().SequenceEqual
                    (new int[] {0, 1, 2, 3, 6, 7, 8}));
            IsTrue(
                b.EnumerateBorders(ImageExtentions.BorderType.All & ~ImageExtentions.BorderType.Bottom).Select(a => a.ToArgb()).OrderBy()
                 .SequenceEqual(new int[] {0, 1, 2, 3, 5, 6, 8}));
            IsTrue(b.EnumerateBorders().Select(a => a.ToArgb()).OrderBy().SequenceEqual(new int[] {0, 1, 2, 3, 5, 6, 7, 8}));
        }
    }
    [TestClass]
    public class LockBitmapTest
    {
        [TestMethod] public void Simple()
        {
            const int size = 9;
            Bitmap b = new Bitmap(size, size);
            int k = 0;
            foreach (var c in Loops.Range(b.Width).Join(Loops.Range(b.Height)))
            {
                b.SetPixel(c.Item1, c.Item2,Color.FromArgb(k));
                k++;
            }
            LockBitmap l = new LockBitmap(b);
            k = 0;
            l.LockBits();
            foreach (var c in Loops.Range(b.Width).Join(Loops.Range(b.Height)))
            {
                AreEqual(l[c.Item1,c.Item2].ToArgb(),k);
                k++;
                l[c.Item1, c.Item2] = Color.FromArgb(k);
            }
            l.UnlockBits();
            k = 1;
            foreach (var c in Loops.Range(b.Width).Join(Loops.Range(b.Height)))
            {
                AreEqual(b.GetPixel(c.Item1,c.Item2).ToArgb(), k);
                k++;
            }
        }
    }
}