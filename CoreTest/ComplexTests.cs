using System;
using System.Linq;
using Edge.Complex;
using Edge.Looping;
using Edge.Modular;
using Edge.Units.Angle;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Edge.Complex.ComplexNumber;
using static CoreTest.AssertComplexTest;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    public static class AssertComplexTest
    {
        public static void CheckVital(ComplexNumber c, double real, double imag, double radius, double angle)
        {
            AreEqual(c.RealPart, real, 0.0001);
            AreEqual(c.ImaginaryPart, imag, 0.0001);
            AreEqual(c.Radius, radius, 0.0001);
            AreEqual(c.Angle.Radians, angle, 0.0001);
        }
        public static void CheckVital(ComplexNumber c, ComplexNumber other)
        {
            CheckVital(c, other.RealPart, other.ImaginaryPart, other.Radius, other.Angle.Radians);
        }
    }
    [TestClass]
    public class ConstructionAndBasicsTest
    {
        [TestMethod] public void Simple()
        {
            var val = new ComplexNumber(3, 4);
            CheckVital(val, 3, 4, 5, Math.Atan(4 / 3.0));
        }
        [TestMethod] public void ZeroRect()
        {
            var val = new ComplexNumber(0, 0);
            CheckVital(val, 0, 0, 0, 0);
        }
        [TestMethod] public void ZeroPol()
        {
            var val = new ComplexNumber(2, 0, ComplexRepresentations.Polar);
            CheckVital(val, 0, 0, 0, 0);
        }
        [TestMethod] public void ImaginaryRect()
        {
            var val = new ComplexNumber(0, 3);
            CheckVital(val, 0, 3, 3, Math.PI / 2);
        }
        [TestMethod] public void ImaginaryPol()
        {
            var val = new ComplexNumber(Math.PI / 2.0, 3, ComplexRepresentations.Polar);
            CheckVital(val, 0, 3, 3, Math.PI / 2);
        }
        [TestMethod] public void RealRect()
        {
            var val = new ComplexNumber(3, 0);
            CheckVital(val, 3, 0, 3, 0);
        }
        [TestMethod] public void RealPol()
        {
            var val = new ComplexNumber(0, 3, ComplexRepresentations.Polar);
            CheckVital(val, 3, 0, 3, 0);
        }
        [TestMethod] public void MiscContructions()
        {
            var val = new ComplexNumber(-1, 4);
            CheckVital(val, -1, 4, 4.12311, 1.8157);
            val = new ComplexNumber(-1, -2);
            CheckVital(val, -1, -2, 2.236, 4.24874137);
            val = new ComplexNumber(5, -2);
            CheckVital(val, 5, -2, 5.38516, 5.90267893);
        }
    }
    [TestClass]
    public class ComplexOperatorsTest
    {
        [TestMethod] public void SimpleAddition()
        {
            var val1 = new ComplexNumber(1, 2);
            var val2 = new ComplexNumber(3, 4);
            CheckVital(val1 + val2, new ComplexNumber(4, 6));
            CheckVital(val2 + val1, new ComplexNumber(4, 6));
        }
        [TestMethod] public void ZeroAddition()
        {
            var val1 = new ComplexNumber(1, 2);
            var val2 = new ComplexNumber(0, 0);
            CheckVital(val1 + val2, new ComplexNumber(1, 2));
            CheckVital(val2 + val1, new ComplexNumber(1, 2));
        }
        [TestMethod] public void SelfAddition()
        {
            var val1 = new ComplexNumber(1, 2);
            CheckVital(val1 + val1, new ComplexNumber(2, 4));
        }
        [TestMethod] public void SimpleSubtraction()
        {
            var val1 = new ComplexNumber(1, 2);
            var val2 = new ComplexNumber(3, 4);
            CheckVital(val1 - val2, new ComplexNumber(-2, -2));
            CheckVital(val2 - val1, new ComplexNumber(2, 2));
        }
        [TestMethod] public void ZeroSubtraction()
        {
            var val1 = new ComplexNumber(1, 2);
            var val2 = new ComplexNumber(0, 0);
            CheckVital(val1 - val2, new ComplexNumber(1, 2));
            CheckVital(val2 - val1, new ComplexNumber(-1, -2));
        }
        [TestMethod] public void SelfSubtraction()
        {
            var val1 = new ComplexNumber(1, 2);
            CheckVital(val1 - val1, new ComplexNumber(0, 0));
        }
        [TestMethod] public void SimpleMultiplication()
        {
            var val1 = FromPolar(1, 2);
            var val2 = FromPolar(3, 4);
            CheckVital(val1 * val2, FromPolar(4, 8));
            CheckVital(val2 * val1, FromPolar(4, 8));
        }
        [TestMethod] public void ZeroMultiplication()
        {
            var val1 = FromPolar(1, 2);
            var val2 = new ComplexNumber(0, 0);
            CheckVital(val1 * val2, new ComplexNumber(0, 0));
            CheckVital(val2 * val1, new ComplexNumber(0, 0));
        }
        [TestMethod] public void OneMultiplication()
        {
            var val1 = FromPolar(1, 2);
            var val2 = new ComplexNumber(1, 0);
            CheckVital(val1 * val2, FromPolar(1, 2));
            CheckVital(val2 * val1, FromPolar(1, 2));
        }
        [TestMethod] public void SelfMultiplication()
        {
            var val1 = FromPolar(1, 2);
            CheckVital(val1 * val1, FromPolar(2, 4));
        }
        [TestMethod] public void SimpleDivision()
        {
            var val1 = FromPolar(1, 2);
            var val2 = FromPolar(3, 4);
            CheckVital(val1 / val2, FromPolar(-2, 0.5));
            CheckVital(val2 / val1, FromPolar(2, 2));
        }
        [TestMethod, ExpectedException(typeof(DivideByZeroException))] public void ZeroDivision()
        {
            var val1 = FromPolar(1, 2);
            var val2 = new ComplexNumber(0, 0);
            CheckVital(val2 / val1, new ComplexNumber(0, 0));
            val1 = val1 / val2;
            Fail();
        }
        [TestMethod] public void OneDivision()
        {
            var val1 = FromPolar(1, 2);
            var val2 = new ComplexNumber(1, 0);
            CheckVital(val1 / val2, FromPolar(1, 2));
            CheckVital(val2 / val1, FromPolar(-1, 0.5));
        }
        [TestMethod] public void SelfDivision()
        {
            var val1 = FromPolar(1, 2);
            CheckVital(val1 / val1, One);
        }
        [TestMethod] public void SimpleModulo()
        {
            var val1 = FromRectangular(1, 2);
            var val2 = FromRectangular(3, 4);
            CheckVital(val1 % val2, FromRectangular(1, 2));
            CheckVital(val2 % val1, FromRectangular(1, 0));
        }
        [TestMethod, ExpectedException(typeof(DivideByZeroException))] public void ZeroModulo()
        {
            var val1 = FromRectangular(1, 2);
            var val2 = new ComplexNumber(0, 0);
            CheckVital(val2 % val1, new ComplexNumber(0, 0));
            val1 = val1 % val2;
            Fail();
        }
        [TestMethod] public void OneModulo()
        {
            var val1 = FromRectangular(1, 2);
            var val2 = new ComplexNumber(1, 0);
            CheckVital(val1 % val2, FromRectangular(0, 0));
            CheckVital(val2 % val1, FromRectangular(1, 0));
        }
        [TestMethod] public void SelfModulo()
        {
            var val1 = FromRectangular(1, 2);
            CheckVital(val1 % val1, ComplexNumber.Zero);
        }
    }
    [TestClass]
    public class DoubleComplexOperatorsTest
    {
        [TestMethod] public void SimpleAddition()
        {
            var val1 = new ComplexNumber(1, 2);
            var val2 = 2.1;
            CheckVital(val1 + val2, new ComplexNumber(3.1, 2));
            CheckVital(val2 + val1, new ComplexNumber(3.1, 2));
        }
        [TestMethod] public void ZeroAddition()
        {
            var val1 = new ComplexNumber(1, 2);
            var val2 = 0;
            CheckVital(val1 + val2, new ComplexNumber(1, 2));
            CheckVital(val2 + val1, new ComplexNumber(1, 2));
        }
        [TestMethod] public void ZeroAddition2()
        {
            var val1 = new ComplexNumber(0, 0);
            var val2 = 2.5;
            CheckVital(val1 + val2, new ComplexNumber(2.5, 0));
            CheckVital(val2 + val1, new ComplexNumber(2.5, 0));
        }
        [TestMethod] public void SimpleSubtraction()
        {
            var val1 = new ComplexNumber(1, 2);
            var val2 = -3.5;
            CheckVital(val1 - val2, new ComplexNumber(4.5, 2));
            CheckVital(val2 - val1, new ComplexNumber(-4.5, -2));
        }
        [TestMethod] public void ZeroSubtraction()
        {
            var val1 = new ComplexNumber(1, 2);
            var val2 = 0;
            CheckVital(val1 - val2, new ComplexNumber(1, 2));
            CheckVital(val2 - val1, new ComplexNumber(-1, -2));
        }
        [TestMethod] public void ZeroSubtraction2()
        {
            var val1 = new ComplexNumber(0, 0);
            var val2 = -3.5;
            CheckVital(val1 - val2, new ComplexNumber(3.5, 0));
            CheckVital(val2 - val1, new ComplexNumber(-3.5, 0));
        }
        [TestMethod] public void SimpleMultiplication()
        {
            var val1 = FromRectangular(1, 2);
            var val2 = 4.1;
            CheckVital(val1 * val2, FromRectangular(4.1, 8.2));
            CheckVital(val2 * val1, FromRectangular(4.1, 8.2));
        }
        [TestMethod] public void ZeroMultiplication()
        {
            var val1 = FromPolar(1, 2);
            var val2 = 0;
            CheckVital(val1 * val2, new ComplexNumber(0, 0));
            CheckVital(val2 * val1, new ComplexNumber(0, 0));
        }
        [TestMethod] public void ZeroMultiplication2()
        {
            var val1 = FromPolar(0, 0);
            var val2 = 3.6;
            CheckVital(val1 * val2, new ComplexNumber(0, 0));
            CheckVital(val2 * val1, new ComplexNumber(0, 0));
        }
        [TestMethod] public void OneMultiplication()
        {
            var val1 = FromPolar(1, 2);
            var val2 = 1;
            CheckVital(val1 * val2, FromPolar(1, 2));
            CheckVital(val2 * val1, FromPolar(1, 2));
        }
        [TestMethod] public void OneMultiplication2()
        {
            var val1 = FromPolar(1, 0);
            var val2 = 1.9;
            CheckVital(val1 * val2, FromPolar(1.9, 0));
            CheckVital(val2 * val1, FromPolar(1.9, 0));
        }
        [TestMethod] public void SimpleDivision()
        {
            var val1 = FromPolar(1, 2);
            var val2 = 3;
            CheckVital(val1 / val2, FromPolar(1, 2.0 / 3));
            CheckVital(val2 / val1, FromPolar(-1, 3.0 / 2));
        }
        [TestMethod, ExpectedException(typeof(DivideByZeroException))] public void ZeroDivision()
        {
            var val1 = FromPolar(1, 2);
            var val2 = 0;
            CheckVital(val2 / val1, new ComplexNumber(0, 0));
            val1 = val1 / val2;
            Fail();
        }
        [TestMethod, ExpectedException(typeof(DivideByZeroException))] public void ZeroDivision2()
        {
            var val2 = new ComplexNumber(0, 0);
            var val1 = 3;
            CheckVital(val2 / val1, new ComplexNumber(0, 0));
            val2 = val1 / val2;
            Fail();
        }
        [TestMethod] public void OneDivision()
        {
            var val1 = FromPolar(1, 2);
            var val2 = 1;
            CheckVital(val1 / val2, FromPolar(1, 2));
            CheckVital(val2 / val1, FromPolar(-1, 0.5));
        }
        [TestMethod] public void OneDivision2()
        {
            var val1 = FromRectangular(1, 0);
            var val2 = 9.2;
            CheckVital(val1 / val2, FromRectangular(1 / 9.2, 0));
            CheckVital(val2 / val1, FromRectangular(9.2, 0));
        }
        [TestMethod] public void SimpleModulo()
        {
            var val1 = FromRectangular(1, 2);
            var val2 = 2.1;
            CheckVital(val1 % val2, FromRectangular(1, 2));
            CheckVital(val2 % val1, FromRectangular(2.1, 0));
        }
        [TestMethod, ExpectedException(typeof(DivideByZeroException))] public void ZeroModulo()
        {
            var val1 = FromRectangular(1, 2);
            var val2 = 0;
            CheckVital(val2 % val1, new ComplexNumber(0, 0));
            val1 = val1 % val2;
            Fail();
        }
        [TestMethod, ExpectedException(typeof(DivideByZeroException))] public void ZeroModulo2()
        {
            var val2 = FromRectangular(0, 0);
            var val1 = 0;
            CheckVital(val2 % val1, new ComplexNumber(0, 0));
            val2 = val1 % val2;
            Fail();
        }
        [TestMethod] public void OneModulo()
        {
            var val1 = FromRectangular(1, 2);
            var val2 = 1;
            CheckVital(val1 % val2, FromRectangular(0, 0));
            CheckVital(val2 % val1, FromRectangular(1, 0));
        }
        [TestMethod] public void OneModulo2()
        {
            var val2 = FromRectangular(1, 0);
            var val1 = 1.6;
            CheckVital(val1 % val2, FromRectangular(0.6, 0));
            CheckVital(val2 % val1, FromRectangular(1, 0));
        }
    }
    [TestClass]
    public class UnaryComplexOperatorsTest
    {
        [TestMethod] public void SimpleIncrement()
        {
            var val = FromRectangular(2, 5);
            val++;
            CheckVital(val, FromRectangular(3, 5));
        }
        [TestMethod] public void ZeroIncrement()
        {
            var val = FromRectangular(0, 5);
            val++;
            CheckVital(val, FromRectangular(1, 5));
        }
        [TestMethod] public void NegIncrement()
        {
            var val = FromRectangular(-9, 5);
            val++;
            CheckVital(val, FromRectangular(-8, 5));
        }
        [TestMethod] public void SimpleDecrement()
        {
            var val = FromRectangular(2, 5);
            val--;
            CheckVital(val, FromRectangular(1, 5));
        }
        [TestMethod] public void ZeroDecrement()
        {
            var val = FromRectangular(0, 5);
            val--;
            CheckVital(val, FromRectangular(-1, 5));
        }
        [TestMethod] public void NegDecrement()
        {
            var val = FromRectangular(-9, 5);
            val--;
            CheckVital(val, FromRectangular(-10, 5));
        }
        [TestMethod] public void SimpleNeg()
        {
            var val = FromRectangular(3, -5);
            CheckVital(-val, FromRectangular(-3, 5));
        }
        [TestMethod] public void ZeroNeg()
        {
            var val = FromRectangular(0, 0);
            CheckVital(-val, FromRectangular(0, 0));
        }
        [TestMethod] public void SimplePos()
        {
            var val = FromRectangular(3, -5);
            CheckVital(+val, FromRectangular(3, -5));
        }
        [TestMethod] public void ZeroPos()
        {
            var val = FromRectangular(0, 0);
            CheckVital(+val, FromRectangular(0, 0));
        }
    }
    [TestClass]
    public class ComplexEqualsTest
    {
        [TestMethod] public void SimpleComTrue()
        {
            var val1 = new ComplexNumber(2, 1);
            var val2 = new ComplexNumber(2, 1);
            IsTrue(val1.Equals(val2));
        }
        [TestMethod] public void SimpleComFalse()
        {
            var val1 = new ComplexNumber(2, 1);
            var val2 = new ComplexNumber(2, 2);
            IsFalse(val1.Equals(val2));
        }
        [TestMethod] public void SimpleDubTrue()
        {
            var val1 = new ComplexNumber(2, 0);
            var val2 = 2.0;
            IsTrue(val1.Equals(val2));
        }
        [TestMethod] public void SimpleDubFalse()
        {
            var val1 = new ComplexNumber(2, 1);
            var val2 = 2.0;
            IsFalse(val1.Equals(val2));
        }
        [TestMethod] public void SimpleIntTrue()
        {
            var val1 = new ComplexNumber(2, 0);
            var val2 = 2;
            IsTrue(val1.Equals(val2));
        }
        [TestMethod] public void SimpleIntFalse()
        {
            var val1 = new ComplexNumber(2, 1);
            var val2 = 2;
            IsFalse(val1.Equals(val2));
        }
        [TestMethod] public void SimpleDumbFalse()
        {
            var val1 = new ComplexNumber(2, 1);
            var val2 = "dum dee dum";
            IsFalse(val1.Equals(val2));
        }
    }
    [TestClass]
    public class ComplexPowDubTest
    {
        [TestMethod] public void Simple()
        {
            var val = FromPolar(3, 2);
            CheckVital(val.pow(3), FromPolar(9, 8));
        }
        [TestMethod] public void Invpower()
        {
            var val = FromPolar(3, 2);
            CheckVital(val.pow(-1), FromPolar(-3, 0.5));
        }
        [TestMethod] public void ZeroPow()
        {
            var val = FromPolar(3, 2);
            CheckVital(val.pow(0), FromPolar(0));
        }
        [TestMethod] public void OnePow()
        {
            var val = FromPolar(3, 2);
            CheckVital(val.pow(1), FromPolar(3, 2));
        }
        [TestMethod] public void ZeroBase()
        {
            var val = FromPolar(0, 0);
            CheckVital(val.pow(3), FromPolar(0, 0));
        }
        [TestMethod] public void ZeroByZero()
        {
            var val = FromPolar(0, 0);
            CheckVital(val.pow(0), FromPolar(0));
        }
        [TestMethod] public void Root()
        {
            var val = FromPolar(Math.PI);
            CheckVital(val.pow(0.5), FromPolar(Math.PI / 2));
        }
        [TestMethod] public void PowOverflow()
        {
            var val = FromPolar(Math.PI / 2);
            CheckVital(val.pow(8), FromPolar(0));
        }
    }
    [TestClass]
    public class DoublePowComTest
    {
        [TestMethod] public void Simple()
        {
            var val = Math.E;
            var val2 = FromRectangular(-1, 5);
            CheckVital(val.pow(val2), FromRectangular(0.10435, -0.35276));
        }
        [TestMethod] public void Inv()
        {
            var val = Math.E;
            var val2 = FromRectangular(-1, 0);
            CheckVital(val.pow(val2), FromRectangular(1 / Math.E, 0));
        }
        [TestMethod] public void PureImag()
        {
            var val = Math.E;
            var val2 = FromRectangular(0, Math.PI / 2);
            CheckVital(val.pow(val2), FromRectangular(0, 1));
        }
        [TestMethod] public void CompRoot()
        {
            var val = -1;
            var val2 = FromRectangular(0.5, 0);
            CheckVital(val.pow(val2), FromRectangular(0, 1));
        }
        [TestMethod] public void CompTriRoot()
        {
            var val = -1;
            var val2 = FromRectangular(1.0 / 3, 0);
            CheckVital(val.pow(val2), FromPolar((1.0 / 3) * Math.PI));
        }
    }
    [TestClass]
    public class ComplexPowComTest
    {
        [TestMethod] public void Simple()
        {
            var val = FromPolar(0.5, 2);
            var val2 = FromRectangular(1, -4);
            CheckVital(val.pow(val2), FromRectangular(-9.5405, -11.2858));
        }
        [TestMethod] public void Simple2()
        {
            var val = FromPolar(1, 1.5);
            var val2 = FromRectangular(2, 2);
            CheckVital(val.pow(val2), FromRectangular(-0.2880, 0.0988633));
        }
        [TestMethod] public void Invpower()
        {
            var val = FromPolar(3, 2);
            CheckVital(val.pow(FromRectangular(-1, 0)), FromPolar(-3, 0.5));
        }
        [TestMethod] public void ZeroPow()
        {
            var val = FromPolar(3, 2);
            CheckVital(val.pow(ComplexNumber.Zero), FromPolar(0));
        }
        [TestMethod] public void OnePow()
        {
            var val = FromPolar(3, 2);
            CheckVital(val.pow(One), FromPolar(3, 2));
        }
        [TestMethod] public void ZeroBase()
        {
            var val = FromPolar(0, 0);
            CheckVital(val.pow(FromRectangular(3, 0)), FromPolar(0, 0));
        }
        [TestMethod] public void ZeroByZero()
        {
            var val = FromPolar(0, 0);
            CheckVital(val.pow(ComplexNumber.Zero), FromPolar(0));
        }
        [TestMethod] public void Root()
        {
            var val = FromPolar(Math.PI);
            CheckVital(val.pow(FromRectangular(0.5, 0)), FromPolar(Math.PI / 2));
        }
        [TestMethod] public void PowOverflow()
        {
            var val = FromPolar(Math.PI / 2);
            CheckVital(val.pow(FromRectangular(8, 0)), FromPolar(0));
        }
    }
    [TestClass]
    public class ComplexLogarithmDoubleBaseTest
    {
        [TestMethod] public void Simple()
        {
            var val = FromRectangular(-1, 5);
            var val2 = Math.E;
            CheckVital(val.log(val2), FromRectangular(1.62904, 1.768191));
        }
        [TestMethod] public void Simple2()
        {
            var val = FromRectangular(-39.0 / 53, -4.0 / 53);
            var val2 = Math.E;
            CheckVital(val.log(val2), FromRectangular(-0.3014980417, -3.03938 + Math.PI * 2));
        }
        [TestMethod] public void One()
        {
            var val = FromRectangular(1, 0);
            var val2 = Math.E;
            CheckVital(val.log(val2), FromRectangular(0, 0));
        }
        [TestMethod, ExpectedException(typeof(ArithmeticException))] public void Zero()
        {
            var val = FromRectangular(0, 0);
            var val2 = Math.E;
            val.log(val2);
            Fail();
        }
        [TestMethod] public void SpecialBase()
        {
            var val = FromRectangular(64, 0);
            var val2 = 2.0;
            CheckVital(val.log(val2), FromRectangular(6, 0));
        }
        [TestMethod] public void SpecialBase2()
        {
            var val = FromRectangular(5, 3);
            var val2 = 8.0;
            CheckVital(val.log(val2), FromRectangular(0.8479104, 0.2598868));
        }
    }
    [TestClass]
    public class ComplexLogarithmComplexBaseTest
    {
        [TestMethod] public void Simple()
        {
            var val = FromRectangular(-1, 5);
            var val2 = FromRectangular(2, 3);
            CheckVital(val.log(val2), FromRectangular(1.465925, 0.25535));
        }
    }
    [TestClass]
    public class ComplexTrigonomoetricTest
    {
        [TestMethod] public void RealSin()
        {
            var val = FromRectangular(Math.PI / 6, 0);
            CheckVital(val.Sin(), FromRectangular(0.5, 0));
        }
        [TestMethod] public void SimpleSin()
        {
            var val = FromRectangular(2, -6);
            CheckVital(val.Sin(), FromRectangular(183.4195088, 83.942292));
        }
        [TestMethod] public void ImaginarySin()
        {
            var val = FromRectangular(0, 1);
            CheckVital(val.Sin(), FromRectangular(0, 1.17520119364));
        }
        [TestMethod] public void RealCos()
        {
            var val = FromRectangular(Math.PI / 6, 0);
            CheckVital(val.Cos(), FromRectangular(Math.Sqrt(3) / 2, 0));
        }
        [TestMethod] public void SimpleCos()
        {
            var val = FromRectangular(-2, 3.2);
            CheckVital(val.Cos(), FromRectangular(-5.11305, 11.1352));
        }
        [TestMethod] public void ImaginaryCos()
        {
            var val = FromRectangular(0, 2);
            CheckVital(val.Cos(), FromRectangular(3.7621956, 0));
        }
        [TestMethod] public void RealTan()
        {
            var val = FromRectangular(Math.PI / 6, 0);
            CheckVital(val.Tan(), FromRectangular(1.0 / Math.Sqrt(3), 0));
        }
        [TestMethod] public void SimpleTan()
        {
            var val = FromRectangular(-8, 2);
            CheckVital(val.Tan(), FromRectangular(0.010925, 1.035647));
        }
        [TestMethod] public void ImaginaryTan()
        {
            var val = FromRectangular(0, 1);
            CheckVital(val.Tan(), FromRectangular(0, 0.76159415));
        }
    }
    [TestClass]
    public class ComplexInverseTrigonometrictest
    {
        [TestMethod] public void SimpleASin()
        {
            var val = FromRectangular(2, -6);
            CheckVital(val.Asin(), FromRectangular(0.318056, -2.54257022));
        }
        [TestMethod] public void SimpleACos()
        {
            var val = FromRectangular(2, -6);
            CheckVital(val.Acos(), FromRectangular(1.25274, 2.5425702));
        }
        [TestMethod] public void SimpleATan()
        {
            var val = FromRectangular(2, -6);
            CheckVital(val.Atan(), FromRectangular(-1.621899685, -0.150749020891412));
        }
    }
    [TestClass]
    public class ComplexHyperbolicTrigonometrictest
    {
        [TestMethod] public void SimpleSinh()
        {
            var val = FromRectangular(2, -6);
            CheckVital(val.Sinh(), FromRectangular(3.48240359744, 1.051215783));
        }
        [TestMethod] public void SimpleCosh()
        {
            var val = FromRectangular(2, -6);
            CheckVital(val.Cosh(), FromRectangular(3.61234851, 1.01340100));
        }
        [TestMethod] public void SimpleTanh()
        {
            var val = FromRectangular(2, -6);
            CheckVital(val.Tanh(), FromRectangular(0.9693745, 0.01905979));
        }
    }
    [TestClass]
    public class MiscComplexTest
    {
        [TestMethod] public void SimpleRoots()
        {
            const int mag = 5;
            var val = FromRectangular(2, 5).roots(mag);
            var primary = FromRectangular(1.36087, 0.33023);
            AreEqual(val.Length, mag);
            foreach (int i in Loops.Range(val.Length))
                CheckVital(val[i], primary * new ComplexNumber(new Angle(i / (double)mag, Edge.Units.Angle.Angle.Turn)));
        }
        [TestMethod] public void SimpleConjugate()
        {
            int[] reals = {2, -3, 5, -8, 1, -4, 6, 8, -9, 7, -5, 0};
            var sample = reals.Concat(reals.Select(a => -a)).Join();
            sample.Do(a => CheckVital(FromRectangular(a.Item1, a.Item2).Conjugate(), FromRectangular(a.Item1, -a.Item2)));
        }
        [TestMethod] public void IsGaussianSimpleTrue()
        {
            var val = FromRectangular(3, 4);
            IsTrue(val.IsGaussian());
        }
        [TestMethod] public void IsGaussianSimpleFalse()
        {
            var val = FromRectangular(3, 4.999);
            IsFalse(val.IsGaussian());
        }
        [TestMethod] public void IsGaussianPrimeSimpleTrue()
        {
            var val = FromRectangular(5, 2);
            IsTrue(val.isGaussianPrime());
        }
        [TestMethod] public void IsGaussianPrimeSimpleFalse()
        {
            var val = FromRectangular(4, 2);
            IsFalse(val.isGaussianPrime());
        }
        [TestMethod] public void ToMatrixSimple()
        {
            var val = FromRectangular(2, 3);
            var m = val.ToMatrix();
            AreEqual(m.rows, 2);
            AreEqual(m.collumns, 2);
            IsTrue(m.SequenceEqual(new double[] {2, -3, 3, 2}));
        }
    }
    [TestClass]
    public class ComplexRealCompareTest
    {
        [TestMethod] public void SimpleForward()
        {
            var val1 = FromRectangular(3, 0);
            var val2 = 2;
            IsTrue(val1.CompareTo(val2) > 0);
        }
        [TestMethod] public void SimpleBackwards()
        {
            var val1 = FromRectangular(1, 0);
            var val2 = 2;
            IsTrue(val1.CompareTo(val2) < 0);
        }
        [TestMethod] public void SimpleEquals()
        {
            var val1 = FromRectangular(-3.35, 0);
            var val2 = -3.35;
            IsTrue(val1.CompareTo(val2) == 0);
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void SimpleBad()
        {
            var val1 = FromRectangular(-3.35, 1);
            var val2 = -3.35;
            val1.CompareTo(val2);
            Fail();
        }
    }
    [TestClass]
    public class ComplexComplexCompareTest
    {
        [TestMethod] public void SimpleForward()
        {
            var val1 = FromPolar(1, 2);
            var val2 = FromPolar(1);
            IsTrue(val1.CompareTo(val2) > 0);
        }
        [TestMethod] public void SimpleBackwards()
        {
            var val1 = FromPolar(1, 2);
            var val2 = FromPolar(1 + Math.PI, 0);
            IsTrue(val1.CompareTo(val2) > 0);
        }
        [TestMethod] public void SimpleEquals()
        {
            var val1 = FromPolar(1, 2);
            var val2 = FromPolar(1, 2);
            IsTrue(val1.CompareTo(val2) == 0);
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void SimpleBad()
        {
            var val1 = FromPolar(1, 2);
            var val2 = FromPolar(2, 2);
            val1.CompareTo(val2);
            Fail();
        }
    }
    [TestClass]
    public class ComplexToRectStingTest
    {
        [TestMethod] public void Simple()
        {
            var val = FromRectangular(2, 3);
            AreEqual(val.ToString(), "2+3i");
        }
        [TestMethod] public void ComNegIm()
        {
            var val = FromRectangular(2, -3);
            AreEqual(val.ToString("R_"), "2-3i");
        }
        [TestMethod] public void Real()
        {
            var val = FromRectangular(2, 0);
            AreEqual(val.ToString("R_"), "2");
        }
        [TestMethod] public void Imag()
        {
            var val = FromRectangular(0, 2);
            AreEqual(val.ToString("R_"), "2i");
        }
        [TestMethod] public void NegImag()
        {
            var val = FromRectangular(0, -2);
            AreEqual(val.ToString("R_"), "-2i");
        }
        [TestMethod] public void Zero()
        {
            var val = FromRectangular(0, 0);
            AreEqual(val.ToString("R_"), "0");
        }
    }
    //TODO: Polar tostring testing
    [TestClass]
    public class ComplexCastingTest
    {
        [TestMethod] public void SimpleInt()
        {
            var val = 2;
            IsTrue(((ComplexNumber)val).Equals(val));
        }
        [TestMethod] public void NegInt()
        {
            var val = -2;
            IsTrue(((ComplexNumber)val).Equals(val));
        }
        [TestMethod] public void ZeroInt()
        {
            var val = 0;
            IsTrue(((ComplexNumber)val).Equals(val));
        }
        [TestMethod] public void SimpleDouble()
        {
            var val = 2.3;
            IsTrue(((ComplexNumber)val).Equals(val));
        }
        [TestMethod] public void NegDouble()
        {
            var val = -2.1;
            IsTrue(((ComplexNumber)val).Equals(val));
        }
        [TestMethod] public void ZeroDouble()
        {
            var val = 0.0;
            IsTrue(((ComplexNumber)val).Equals(val));
        }
    }
    [TestClass]
    public class ComplexRectParseTest
    {
        [TestMethod] public void SimplePureComplex()
        {
            CheckVital(Parse("12+3.0i"), FromRectangular(12, 3));
            CheckVital(Parse("-2.5+3i"), FromRectangular(-2.5, 3));
            CheckVital(Parse("+1-3.1i"), FromRectangular(1, -3.1));
            CheckVital(Parse("-2-0i"), FromRectangular(-2, 0));
        }
        [TestMethod] public void SimpleReal()
        {
            CheckVital(Parse("12.0"), FromRectangular(12, 0));
            CheckVital(Parse("2"), FromRectangular(2, 0));
            CheckVital(Parse("+1.1"), FromRectangular(1.1, 0));
            CheckVital(Parse("-2"), FromRectangular(-2, 0));
        }
        [TestMethod] public void SimpleImag()
        {
            CheckVital(Parse("12.0i"), FromRectangular(0, 12));
            CheckVital(Parse("2i"), FromRectangular(0, 2));
            CheckVital(Parse("+1.1i"), FromRectangular(0, 1.1));
            CheckVital(Parse("-2i"), FromRectangular(0, -2));
        }
    }
    [TestClass]
    public class ComplexPolParseTest
    {
        [TestMethod] public void SimpleRadAngle()
        {
            CheckVital(Parse("2.2cis(3)"), FromPolar(3, 2.2));
            CheckVital(Parse("-8cis(10)"), FromPolar(10, -8));
            CheckVital(Parse("0.001cis(-1)"), FromPolar(-1, 0.001));
            CheckVital(Parse("0cis(3)"), FromPolar(0, 0));
        }
        [TestMethod] public void SimpleAngle()
        {
            CheckVital(Parse("cis(+3)"), FromPolar(3));
            CheckVital(Parse("cis(10)"), FromPolar(10));
            CheckVital(Parse("cis(-1.5)"), FromPolar(-1.5));
            CheckVital(Parse("cis(3.3)"), FromPolar(3.3));
        }
    }
    [TestClass]
    public class TotalOrderComparerTest
    {
        [TestMethod] public void Simple()
        {
            AssertComparerTest.IsWorking(ComplexComparers.TotalOrder, FromPolar(0, 0), FromPolar(13, 1), FromPolar(32, 1), FromPolar(7, 2),
                FromPolar(1, 2), FromPolar(200, 10));
        }
    }
}