using System;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using Edge.Colors;
using Edge.Imaging;
using Edge.Looping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Brush = System.Windows.Media.Brush;
using static System.Drawing.Color;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static Edge.Imaging.ImageExtentions;

namespace CoreTest
{
    [TestClass]
    public class BrightenTest
    {
        [TestMethod] public void Simple()
        {
            var val = FromArgb(10, 20, 30);
            val = val.Brighten(10);
            AreEqual(val, FromArgb(20, 30, 40));
        }
        [TestMethod] public void OverFlow()
        {
            var val = FromArgb(10, 20, 30);
            val = val.Brighten(300);
            AreEqual(val, FromArgb(255, 255, 255));
        }
        [TestMethod] public void PartialOverFlow()
        {
            var val = FromArgb(10, 20, 200);
            val = val.Brighten(60);
            AreEqual(val, FromArgb(70, 80, 255));
        }
        [TestMethod] public void UnderFlow()
        {
            var val = FromArgb(10, 20, 30);
            val = val.Brighten(-300);
            AreEqual(val, FromArgb(0, 0, 0));
        }
        [TestMethod] public void PartialUnderFlow()
        {
            var val = FromArgb(10, 20, 200);
            val = val.Brighten(-100);
            AreEqual(val, FromArgb(0, 0, 100));
        }
    }
    [TestClass]
    public class DarkenTest
    {
        [TestMethod] public void Simple()
        {
            var val = FromArgb(10, 20, 30);
            val = val.Darken(10);
            AreEqual(val, FromArgb(0, 10, 20));
        }
        [TestMethod] public void OverFlow()
        {
            var val = FromArgb(10, 20, 30);
            val = val.Darken(300);
            AreEqual(val, FromArgb(0, 0, 0));
        }
        [TestMethod] public void PartialOverFlow()
        {
            var val = FromArgb(10, 20, 200);
            val = val.Darken(60);
            AreEqual(val, FromArgb(0, 0, 140));
        }
        [TestMethod] public void UnderFlow()
        {
            var val = FromArgb(10, 20, 30);
            val = val.Darken(-300);
            AreEqual(val, FromArgb(255, 255, 255));
        }
        [TestMethod] public void PartialUnderFlow()
        {
            var val = FromArgb(10, 20, 200);
            val = val.Darken(-100);
            AreEqual(val, FromArgb(110, 120, 255));
        }
    }
    [TestClass]
    public class MultiplyColorTest
    {
        [TestMethod] public void Simple()
        {
            var val = FromArgb(10, 20, 30);
            AreEqual(val.Multiply(2), FromArgb(20, 40, 60));
        }
        [TestMethod] public void LessThanOne()
        {
            var val = FromArgb(10, 20, 30);
            AreEqual(val.Multiply(0.1), FromArgb(1, 2, 3));
        }
        [TestMethod] public void Neg()
        {
            var val = FromArgb(10, 20, 30);
            AreEqual(val.Multiply(-0.3), FromArgb(0, 0, 0));
        }
        [TestMethod] public void PartialOverflow()
        {
            var val = FromArgb(10, 20, 30);
            AreEqual(val.Multiply(10), FromArgb(100, 200, 255));
        }
        [TestMethod] public void Overflow()
        {
            var val = FromArgb(10, 20, 30);
            AreEqual(val.Multiply(30), FromArgb(255, 255, 255));
        }
    }
    [TestClass]
    public class DifferenceColorTest
    {
        [TestMethod] public void Simple()
        {
            AreEqual(Blue.Difference(Red), 2.0 / 3);
        }
        [TestMethod] public void Same()
        {
            AreEqual(Blue.Difference(Blue), 0);
        }
        [TestMethod] public void Opposite()
        {
            AreEqual(Blue.Difference(Yellow), 1);
        }
    }
    [TestClass]
    public class FillColorTest
    {
        [TestMethod] public void Simple()
        {
            var val = Purple.Fill(100, 200);
            AreEqual(val.Width, 100);
            AreEqual(val.Height, 200);
            IsTrue(val.EnumeratePixels().All(a => a.Equals(Purple.ToUnnamedColor())));
        }
    }
    [TestClass]
    public class MediaToSystemColorTest
    {
        [TestMethod] public void Simple()
        {
            System.Windows.Media.Color val = Purple.ToMediaColor();
            AreEqual(val.ToSystemColor(), Purple.ToUnnamedColor());
        }
    }
    [TestClass]
    public class InvertColorTest
    {
        [TestMethod] public void Simple()
        {
            var val = Purple;
            AreEqual(Purple.Invert(), FromArgb(255, 127, 255, 127));
        }
    }
    [TestClass]
    public class SaturateGenColorTest
    {
        [TestMethod] public void Simple()
        {
            const int a = 25, b = 185, c = 200;
            const int avr = (a + b + c) / 3;
            var val = FromArgb(a, b, c);
            val = val.Saturate();
            AreEqual(val.R, avr);
            AreEqual(val.G, avr);
            AreEqual(val.B, avr);
        }
    }
    [TestClass]
    public class SaturateSpecColorTest
    {
        [TestMethod] public void Simple()
        {
            const int a = 25, b = 185, c = 200;
            var val = FromArgb(a, b, c);
            val = val.Saturate(30);
            AreEqual(val.R, 55);
            AreEqual(val.G, 155);
            AreEqual(val.B, 170);
        }
        [TestMethod] public void Limit()
        {
            const int a = 25, b = 120, c = 130;
            var val = FromArgb(a, b, c);
            val = val.Saturate(30);
            AreEqual(val.R, 55);
            AreEqual(val.G, 128);
            AreEqual(val.B, 128);
        }
    }
    [TestClass]
    public class BringCloserToColorTest
    {
        [TestMethod] public void Simple()
        {
            var val = Aqua;
            AreEqual(val.Bringclosertocolor(Purple, 128), FromArgb(128, 127, 128));
        }
    }
    [TestClass]
    public class AverageColorTest
    {
        [TestMethod] public void Simple()
        {
            var val = Loops.Range(0, 256).Select(a => FromArgb(a, a, a)).GetAverageColor();
            AreEqual(val, FromArgb(127, 127, 127));
        }
    }
    [TestClass]
    public class ToUnknownColorTest
    {
        [TestMethod] public void Simple()
        {
            var val = Purple;
            IsTrue(val.IsNamedColor);
            IsFalse(val.ToUnnamedColor().IsNamedColor);
        }
    }
}