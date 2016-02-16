using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using Edge.Arrays;
using Edge.Colors;
using Edge.Looping;
using Edge.NumbersMagic;
using Edge.SystemExtensions;

namespace Edge.Imaging
{
    public static class ImageExtentions
    {
        public static Color getUnusedColor(this Bitmap i)
        {
            var l = new LockBitmap(i);
            l.LockBits();
            var b = l.EnumeratePixels();
            int ret = 0;
            while (b.Any(a => a.ToArgb().Equals(ret)))
            {
                ret++;
            }
            l.UnlockBits();
            return Color.FromArgb(ret);
        }
        public static bool isWithin(this Image @this, Point p)
        {
            return p.X.iswithinPartialExclusive(0, @this.Width) && p.Y.iswithinPartialExclusive(0, @this.Height);
        }
        public static Color GetPixel(this Bitmap @this, Point p)
        {
            return @this.GetPixel(p.X, p.Y);
        }
        public static void SetPixel(this Bitmap @this, Point p, Color c)
        {
            @this.SetPixel(p.X, p.Y,c);
        }
        public static Color[] EnumeratePixels(this Bitmap @this)
        {
            var l = new LockBitmap(@this);
            l.LockBits();
            var ret = l.EnumeratePixels().ToArray(@this.Height * @this.Width);
            l.UnlockBits();
            return ret;
        }
        public static IEnumerable<Color> EnumeratePixels(this LockBitmap @this)
        {
            return Loops.Range(@this.Width).Join(Loops.Range(@this.Height)).Select(a =>
            {
                var c = @this.GetPixel(a.Item1, a.Item2);
                return c;
            });
        }
        public static Color GetAverageColor(this Bitmap @this)
        {
            return @this.EnumeratePixels().GetAverageColor();
        }
        [Flags]
        public enum BorderType
        {
            None = 0,
            Top = 1,
            Left = 2,
            Bottom = 4,
            Right = 8,
            Horizontal = Right|Left,
            Vertical = Top | Bottom,
            All = Horizontal | Vertical
        };
        public static Color[] EnumerateBorders(this Bitmap @this, BorderType b = BorderType.All)
        {
            var l = new LockBitmap(@this);
            l.LockBits();
            var ret = l.EnumerateBorders(b).ToArray((b.HasFlag(BorderType.Top).Indicator() + b.HasFlag(BorderType.Bottom).Indicator()) * @this.Width +
                                                    (b.HasFlag(BorderType.Left).Indicator() + b.HasFlag(BorderType.Right).Indicator()) * @this.Height);
            l.UnlockBits();
            return ret;
        }
        public static IEnumerable<Color> EnumerateBorders(this LockBitmap l, BorderType b = BorderType.All)
        {
            var ret = Enumerable.Empty<Color>();
            // ReSharper disable ImplicitlyCapturedClosure
            if (b.HasFlag(BorderType.Top))
                ret = ret.Concat(Loops.Range(l.Width).Select(a => l.GetPixel(a, 0)));
            if (b.HasFlag(BorderType.Left))
                ret = ret.Concat(Loops.Range(b.HasFlag(BorderType.Top) ? 1 :0, l.Height).Select(a => l.GetPixel(0, a)));
            if (b.HasFlag(BorderType.Bottom))
                ret = ret.Concat(Loops.Range(b.HasFlag(BorderType.Left) ? 1 : 0, l.Width).Select(a => l.GetPixel(a, l.Height - 1)));
            if (b.HasFlag(BorderType.Right))
                ret = ret.Concat(Loops.Range(b.HasFlag(BorderType.Top) ? 1 : 0, l.Height - (b.HasFlag(BorderType.Bottom) ? 1 : 0)).Select(a => l.GetPixel(l.Width - 1, a)));
            // ReSharper restore ImplicitlyCapturedClosure
            return ret;
        }
    }
    /// <summary>
    /// source: http://www.codeproject.com/Tips/240428/Work-with-bitmap-faster-with-Csharp
    /// </summary>
    public class LockBitmap
    {
        readonly Bitmap _source = null;
        IntPtr _iptr = IntPtr.Zero;
        BitmapData _bitmapData = null;

        public byte[] Pixels { get; set; }
        public int Depth { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public LockBitmap(Bitmap source)
        {
            this._source = source;
        }

        public Color this[int x, int y]
        {
            get
            {
                return GetPixel(x, y);
            }
            set
            {
                SetPixel(x,y,value);
            }
        }

        /// <summary>
        /// Lock bitmap data
        /// </summary>
        public void LockBits()
        {
            // Get width and height of bitmap
            this.Width = this._source.Width;
            this.Height = this._source.Height;

            // get total locked pixels count
            int pixelCount = this.Width * this.Height;

            // Create rectangle to lock
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);

            // get source bitmap pixel format size
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            this.Depth = Bitmap.GetPixelFormatSize(this._source.PixelFormat);

            // Check if bpp (Bits Per Pixel) is 8, 24, or 32
            if (this.Depth != 8 && this.Depth != 24 && this.Depth != 32)
            {
                throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
            }

            // Lock bitmap and return bitmap data
            this._bitmapData = this._source.LockBits(rect, ImageLockMode.ReadWrite,
                this._source.PixelFormat);

            // create byte array to copy pixel values
            int step = this.Depth / 8;
            this.Pixels = new byte[pixelCount * step];
            this._iptr = this._bitmapData.Scan0;

            // Copy data from pointer to array
            Marshal.Copy(this._iptr, this.Pixels, 0, this.Pixels.Length);
        }

        /// <summary>
        /// Unlock bitmap data
        /// </summary>
        public void UnlockBits()
        {
            // Copy data from byte array to pointer
            Marshal.Copy(this.Pixels, 0, this._iptr, this.Pixels.Length);

            // Unlock bitmap data
            this._source.UnlockBits(this._bitmapData);
        }

        /// <summary>
        /// Get the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color GetPixel(int x, int y)
        {
            Color clr = Color.Empty;

            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (i > Pixels.Length - cCount)
                throw new IndexOutOfRangeException();

            if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                byte a = Pixels[i + 3]; // a
                clr = Color.FromArgb(a, r, g, b);
            }
            if (Depth == 24) // For 24 bpp get Red, Green and Blue
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                clr = Color.FromArgb(r, g, b);
            }
            if (Depth == 8)
            // For 8 bpp get color value (Red, Green and Blue values are the same)
            {
                byte c = Pixels[i];
                clr = Color.FromArgb(c, c, c);
            }
            return clr;
        }

        /// <summary>
        /// Set the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, Color color)
        {
            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
                Pixels[i + 3] = color.A;
            }
            if (Depth == 24) // For 24 bpp set Red, Green and Blue
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
            }
            if (Depth == 8)
            // For 8 bpp set color value (Red, Green and Blue values are the same)
            {
                Pixels[i] = color.B;
            }
        }
        public void SetPixel(Point point, Color color)
        {
            this.SetPixel(point.X,point.Y,color);
        }
    }
}
