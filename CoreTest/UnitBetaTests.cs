using System;
using Edge.Units;
using Edge.Units.Angles;
using Edge.Units.DataSizes;
using Edge.Units.Lengths;
using Edge.Units.Masses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class AngleTest
    {
        [TestMethod] public void Simple()
        {
            Angle a = new Angle(60, Angle.Degree);
            AreEqual(a.InUnits(Angle.Degree),60, 0.001);
            AreEqual(a.InUnits(Angle.Radian), Math.PI/3, 0.001);
        }
    }
    [TestClass]
    public class LengthTest
    {
        [TestMethod]
        public void Simple()
        {
            Length a = new Length(60, Length.Foot);
            AreEqual(a.InUnits(Length.Foot), 60, 0.001);
            AreEqual(a.InUnits(Length.Meter), 18.29, 0.01);
        }
    }
    [TestClass]
    public class MassTest
    {
        [TestMethod]
        public void Simple()
        {
            Mass a = new Mass(60, Mass.KiloGram);
            AreEqual(a.InUnits(Mass.KiloGram), 60, 0.001);
            AreEqual(a.InUnits(Mass.Pound), 132.277, 0.01);
        }
    }
    [TestClass]
    public class DataSizeTest
    {
        [TestMethod]
        public void Simple()
        {
            DataSize a = new DataSize(60, DataSize.Byte);
            AreEqual(a.InUnits(DataSize.Byte), 60, 0.001);
            AreEqual(a.InUnits(DataSize.Kilobyte), 60/1024.0, 0.01);
        }
    }
}
