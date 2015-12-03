using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Edge.SpecialNumerics;

namespace Edge.Colors
{
// ReSharper disable once InconsistentNaming
    public static class ColorExtensions
    {
        private static readonly ISpecialValTemplate<int> _colorConstraint = new BoundTemplate<int>(byte.MaxValue,byte.MinValue); 
        public static Color Brighten(this Color c, int value = 64)
        {
            return Color.FromArgb(_colorConstraint.quick(c.R + value), _colorConstraint.quick(c.G + value), _colorConstraint.quick(c.B + value));
        }
        public static Color Darken(this Color c ,int value = 64)
        {
            return c.Brighten(-value);
        }
        public static Color Multiply(this Color @this, double factor)
        {
            return Color.FromArgb(_colorConstraint.quick((int)(@this.R * factor)), _colorConstraint.quick((int)(@this.G * factor)),
                _colorConstraint.quick((int)(@this.B * factor)));
        }
        /// <summary>
        /// returns between 0(same color) and 1(opposites)
        /// </summary>
        public static double Difference(this Color a, Color b)
        {
            double r = Math.Abs(a.R - b.R);
            double g = Math.Abs(a.G - b.G);
            double blue = Math.Abs(a.B - b.B);
            return (r + g + blue) / (255 * 3);
        }
        public static void Fill(this PictureBox p, Color c)
        {
            p.Image = c.Fill(new Bitmap(p.Width,p.Height));
        }
        public static Bitmap Fill(this Color c, Image original)
        {
            return c.Fill(original.Size);
        }
        public static Bitmap Fill(this Color c,int x)
        {
            return c.Fill(x, x);
        }
        public static Bitmap Fill(this Color c , int width, int height)
        {
            Bitmap b = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(b);
            g.FillRectangle(new SolidBrush(c), 0,0,width,height);
            g.Save();
            return b;
        }
        public static Bitmap Fill(this Color c, Size s)
        {
            return c.Fill(s.Width, s.Height);
        }
        public static Color ToSystemColor(this System.Windows.Media.Color b)
        {
            return Color.FromArgb(b.A,b.R,b.G,b.B);
        }
        public static System.Windows.Media.Color ToMediaColor(this Color @this)
        {
            return System.Windows.Media.Color.FromArgb(@this.A, @this.R, @this.G, @this.B);
        }
        public static Color Invert(this Color c)
        {
            return Color.FromArgb((byte)(255 - c.R), (byte)(255 - c.G), (byte)(255 - c.B));
        }
        public static Color Readablecounterpart(this Color c)
        {
            Color ret = c.Invert();
            if (c.R < c.B && c.R < c.G)
                ret= Color.FromArgb((byte)(255 - c.R), (byte)(255 - c.B), (byte)(255 - c.G));
            if (c.G < c.R && c.G < c.B)
                ret= Color.FromArgb((byte)(255 - c.B), (byte)(255 - c.G), (byte)(255 - c.R));
            if (c.B < c.G && c.B < c.R)
                ret= Color.FromArgb((byte)(255 - c.G), (byte)(255 - c.R), (byte)(255 - c.B));
            return c.GetBrightness() < 0.66 ? ret.Brighten() : ret.Darken();
        }
        public static Color Saturate(this Color c)
        {
            int average = ((c.R + c.B + c.G) / 3);
            return Color.FromArgb(average, average, average);
        }
        public static Color Saturate(this Color c ,int value)
        {
            return c.Bringclosertocolor(Color.Gray, value);
        }
        public static Color Bringclosertocolor(this Color c, Color target, int maxsteps = 64)
        {
            int r = c.R > target.R ? Math.Max(target.R, c.R - maxsteps) : Math.Min(target.R, c.R + maxsteps);
            int g = c.G > target.G ? Math.Max(target.G, c.G - maxsteps) : Math.Min(target.G, c.G + maxsteps);
            int b = c.B > target.B ? Math.Max(target.B, c.B - maxsteps) : Math.Min(target.B, c.B + maxsteps);
            return Color.FromArgb(r, g, b);
        }
        public static Color GetAverageColor(this IEnumerable<Color> @this)
        {
            ulong red = 0, blue = 0, green = 0, alpha = 0, count = 0;
            foreach (Color c in @this)
            {
                count++;
                red += c.R;
                blue += c.B;
                green += c.G;
                alpha += c.A;
            }
            red /= count;
            blue /= count;
            green /= count;
            alpha /= count;
            return Color.FromArgb((int)alpha, (int)red, (int)green, (int)blue);
        }
        public static Color ToUnnamedColor(this Color @this)
        {
            return Color.FromArgb(@this.ToArgb());
        }
    }
    public static class ColorCollectionss
    {
        public static IReadOnlyList<Color> SimpleColors =
            // ReSharper disable RedundantExplicitArraySize
            new ArraySegment<Color>(new Color[21]
            {
                Color.White, Color.Gray, Color.Black, Color.FromArgb(255, 128, 128), Color.Red, Color.Crimson, Color.LightSalmon, Color.Orange,
                Color.Brown, Color.Ivory, Color.Yellow, Color.Gold, Color.LightGreen, Color.Green, Color.DarkGreen, Color.LightBlue, Color.Blue,
                Color.DarkBlue, Color.Pink, Color.Purple, Color.FromArgb(64, 0, 64)
            }),
            PrimaryColors = new ArraySegment<Color>(new Color[3] {Color.Red, Color.Lime, Color.Blue});
                // ReSharper restore RedundantExplicitArraySize

    }
}
