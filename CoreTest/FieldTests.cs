using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Edge.Complex;
using Edge.Fielding;
using Edge.Looping;
using Edge.NumbersMagic;
using Edge.SystemExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static Edge.Assert.AssertExtentions;

namespace CoreTest
{
    public static class AssertFieldsTest
    {
        public static T getDelta<T>(this Field<T> f)
        {
            try
            {
                return f.fromFraction(1, 10001);
            }
            catch (NotSupportedException)
            {
                return f.zero;
            }
        }
        public static void CheckZero<T>(this Field<T> f, T z)
        {
            AreEqual(f.zero, z);
        }
        public static void CheckOne<T>(this Field<T> f, T o)
        {
            AreEqual(f.one, o);
        }
        public static void CheckBase<T>(this Field<T> f, T b)
        {
            AreEqual(f.naturalbase, b);
        }
        public static void CheckNegativeOne<T>(this Field<T> f, T n)
        {
            AreEqual(f.negativeone, n);
        }
        public static void CheckNegate<T>(this Field<T> f, IEnumerable<Tuple<T, T>> posandnegs)
        {
            foreach (Tuple<T, T> t in posandnegs)
            {
                AreEqual(f.Negate(t.Item1), t.Item2, f.getDelta());
                AreEqual(f.Negate(t.Item2), t.Item1, f.getDelta());
            }
        }
        public static void CheckInvert<T>(this Field<T> f, IEnumerable<Tuple<T, T>> posandnegs)
        {
            foreach (Tuple<T, T> t in posandnegs)
            {
                AreEqual(f.Invert(t.Item1), t.Item2, f.getDelta());
                AreEqual(f.Invert(t.Item2), t.Item1, f.getDelta());
            }
        }
        public static void CheckProduct<T>(this Field<T> f, IEnumerable<Tuple<T, T, T>> operandsandproducts)
        {
            CheckProduct(f, true, operandsandproducts);
        }
        public static void CheckProduct<T>(this Field<T> f, bool symmetric, IEnumerable<Tuple<T, T, T>> operandsandproducts)
        {
            foreach (var t in operandsandproducts)
            {
                var k = f.multiply(t.Item1, t.Item2);
                AreEqual(k, t.Item3, f.getDelta());
                if (symmetric)
                    AreEqual(f.multiply(t.Item2, t.Item1), t.Item3, f.getDelta());
            }
        }
        public static void CheckDivide<T>(this Field<T> f, IEnumerable<Tuple<T, T, T>> operandsandproducts)
        {
            foreach (var t in operandsandproducts)
                AreEqual(f.divide(t.Item1, t.Item2), t.Item3, f.getDelta());
        }
        public static void CheckMod<T>(this Field<T> f, IEnumerable<Tuple<T, T, T>> operandsandproducts)
        {
            foreach (var t in operandsandproducts)
                AreEqual(f.mod(t.Item1, t.Item2), t.Item3, f.getDelta());
        }
        public static void CheckSum<T>(this Field<T> f, IEnumerable<Tuple<T, T, T>> operandsandproducts)
        {
            CheckSum(f, true, operandsandproducts);
        }
        public static void CheckSum<T>(this Field<T> f, bool symmetric, IEnumerable<Tuple<T, T, T>> operandsandproducts)
        {
            foreach (var t in operandsandproducts)
            {
                AreEqual(f.add(t.Item1, t.Item2), t.Item3, f.getDelta());
                if (symmetric)
                    AreEqual(f.add(t.Item2, t.Item1), t.Item3, f.getDelta());
            }
        }
        public static void CheckDifference<T>(this Field<T> f, IEnumerable<Tuple<T, T, T>> operandsandproducts)
        {
            foreach (var t in operandsandproducts)
                AreEqual(f.subtract(t.Item1, t.Item2), t.Item3, f.getDelta());
        }
        public static void CheckConjugate<T>(this Field<T> f, IEnumerable<Tuple<T, T>> posandnegs)
        {
            foreach (Tuple<T, T> t in posandnegs)
            {
                AreEqual(f.Conjugate(t.Item1), t.Item2, f.getDelta());
                AreEqual(f.Conjugate(t.Item2), t.Item1, f.getDelta());
            }
        }
        public static void Checkpow<T>(this Field<T> f, IEnumerable<Tuple<T, T, T>> operandsandproducts)
        {
            foreach (var t in operandsandproducts)
                AreEqual(f.pow(t.Item1, t.Item2), t.Item3, f.getDelta());
        }
        public static void CheckLog<T>(this Field<T> f, IEnumerable<Tuple<T, T>> posandnegs)
        {
            foreach (Tuple<T, T> t in posandnegs)
                AreEqual(f.log(t.Item1), t.Item2, f.getDelta());
        }
        public static void CheckDouble<T>(this Field<T> f, IEnumerable<Tuple<T, double?>> posandnegs)
        {
            foreach (Tuple<T, double?> tuple in posandnegs)
            {
                var k = f.toDouble(tuple.Item1);
                if (k.HasValue)
                {
                    if (tuple.Item2.HasValue)
                        AreEqual(k.Value, tuple.Item2.Value, 0.00001);
                    else
                        Fail();
                }
                else
                    AreEqual(null, tuple.Item2);
            }
        }
        public static void CheckString<T>(this Field<T> f, IEnumerable<Tuple<T, string>> posandnegs)
        {
            foreach (var tuple in posandnegs)
                AreEqual(f.String(tuple.Item1), tuple.Item2);
        }
        public static void CheckInvertible<T>(this Field<T> f, bool tocheck)
        {
            AreEqual(f.Invertible, tocheck);
        }
        public static void CheckParsable<T>(this Field<T> f, bool tocheck)
        {
            AreEqual(f.Parsable, tocheck);
        }
        public static void CheckModuloable<T>(this Field<T> f, bool tocheck)
        {
            AreEqual(f.ModduloAble, tocheck);
        }
        public static void CheckNegatable<T>(this Field<T> f, bool tocheck)
        {
            AreEqual(f.Negatable, tocheck);
        }
        public static void CheckIsNegative<T>(this Field<T> f, IEnumerable<Tuple<T, bool>> posandnegs)
        {
            foreach (var tuple in posandnegs)
                AreEqual(f.isNegative(tuple.Item1), tuple.Item2);
        }
        public static void CheckOrder<T>(this Field<T> f, OrderType tocheck)
        {
            AreEqual(f.Order, tocheck);
        }
        public static void CheckComparer<T>(this Field<T> f, params T[] ordered)
        {
            AssertComparerTest.IsWorking((IComparer<T>)f, ordered);
            AssertComparerTest.IsWorking((IEqualityComparer<T>)f, ordered);
        }
        public static void CheckAbs<T>(this Field<T> f, IEnumerable<Tuple<T, T>> posandnegs)
        {
            foreach (Tuple<T, T> t in posandnegs)
                AreEqual(f.abs(t.Item1), t.Item2, f.getDelta());
        }
        public static void CheckFromInt<T>(this Field<T> f, IEnumerable<Tuple<int, T>> posandnegs)
        {
            foreach (var tuple in posandnegs)
                AreEqual(f.fromInt(tuple.Item1), tuple.Item2, f.getDelta());
        }
        public static void CheckFromFraction<T>(this Field<T> f, IEnumerable<Tuple<int, int, T>> posandnegs)
        {
            foreach (var tuple in posandnegs)
                AreEqual(f.fromFraction(tuple.Item1, tuple.Item2), tuple.Item3);
        }
        public static void CheckPow<T>(this Field<T> f, IEnumerable<Tuple<T, int, T>> posandnegs)
        {
            foreach (var tuple in posandnegs)
                AreEqual(f.Pow(tuple.Item1, tuple.Item2), tuple.Item3, f.getDelta());
        }
        public static void CheckFactorial<T>(this Field<T> f, IEnumerable<Tuple<int, T>> posandnegs)
        {
            foreach (var tuple in posandnegs)
                AreEqual(f.Factorial(tuple.Item1), tuple.Item2, f.getDelta());
        }
        public static void CheckParse<T>(this Field<T> f, IEnumerable<Tuple<string, T>> posandnegs)
        {
            foreach (var tuple in posandnegs)
                AreEqual(f.Parse(tuple.Item1), tuple.Item2, f.getDelta());
        }
    }
    [TestClass]
    public class FloatingFieldTest
    {
        [TestMethod] public void DoubleTest()
        {
            var val = Fields.getField<double>();
            val.CheckZero(0);
            val.CheckOne(1);
            val.CheckBase(Math.E);
            val.CheckNegativeOne(-1);
            val.CheckNegate(new double[] {1, -1, 5, -5, 0.3, -0.3, -2.817, 2.817, 0, 0, Math.PI, -Math.PI}.Attach(a => -a));
            val.CheckInvert(new double[] {1, 1, 0.25, 4, 9, 1.0 / 9, -5, -0.2}.Attach(a => 1.0 / a));
            val.CheckProduct(new double[] {0, 2, 0, 3, 6, 18, 0.25, 4, 1, 9, 0.23, 2.07, 1.25, 6.32, 7.9}.Group2().Attach((a, b) => a * b));
            val.CheckDivide(
                new double[] {0, 2, 0, 3, 6, 0.5, 0.25, 4, 0.0625, 9, 0.24, 37.5, 1.25, 6.32, 0.19778481012658227848101265822785}.Group2().Attach(
                    (a, b) => a / b));
            val.CheckMod(new double[] {5, 3, 2, 12, 2.2, 1, 5.3, 1.1, 0.9}.Group2().Attach((a, b) => a % b));
            val.CheckSum(new double[] {0, 1, 1, 6.2, 8.3, 14.5, -2, 3, 1, 85, 100.32, 185.32}.Group2().Attach((a, b) => a + b));
            val.CheckDifference(new double[] {2, 1, 1, 3, 9, -6, 1.2, 1, 0.2}.Group2().Attach((a, b) => a - b));
            val.CheckConjugate(new double[] {1, 1, 0, 0, -3, -3, 5, 5, 0.69, 0.69}.Attach(a => a));
            val.Checkpow(new double[] {1, 6, 112, 2, 12544, 6, 0.5, 2.4494897427831780981972840747059, 0.5, 0.7, 0.61557220667245814224969653458387}
                .Group2().Attach((a, b) => a.pow(b)));
            val.CheckLog(new double[] {7.8, 2.0541237336955460528479733452617, Math.E, 1, 1000, 6.9077552789821370520539743640531}.Attach(a => a.log()));
            val.CheckDouble(new double[] {1.2, 6, 0, -0.0002}.Attach(a => (double?)a));
            val.CheckString(new double[] {1.2, 6, 0, -0.0002}.Attach(a => a.ToString()));
            val.CheckInvertible(true);
            val.CheckParsable(true);
            val.CheckModuloable(true);
            val.CheckNegatable(true);
            val.CheckIsNegative(new double[] {1.5, 0.0, 0.2}.Attach(a => a < 0));
            val.CheckOrder(OrderType.TotalOrder);
            val.CheckComparer(-123.01, -123, -20, -9, 0, 1, 10, 52, 10000);
            val.CheckAbs(new double[] {0, 0, -2, 2, 3, 3, -3, 3, -1000, 1000, 1000, 1000}.Attach(a => a.abs()));
            val.CheckFromInt(new int[] {0, 3, -2, 4, 9, 4}.Attach(a => (double)a));
            val.CheckFromFraction(new Tuple<int, int>[] {Tuple.Create(0, 9), Tuple.Create(9, 8), Tuple.Create(-8, 1)}.Attach((a, b) => a / (double)b));
            val.CheckPow(new Tuple<double, int>[] {Tuple.Create(0.0, 9), Tuple.Create(9.0, 3), Tuple.Create(-8.0, 1)}.Attach((a, b) => a.pow(b)));
            val.CheckFactorial(new int[] {0, 9, 3}.Attach(a => (double)a.factorial()));
            val.CheckParse(new string[] {"1.5", "0.0", "-0.0002"}.Attach(double.Parse));
        }
        [TestMethod] public void ComplexNumberTest()
        {
            var val = Fields.getField<ComplexNumber>();
            val.CheckZero(0);
            val.CheckOne(1);
            val.CheckBase(Math.E);
            val.CheckNegativeOne(-1);
            val.CheckNegate(
                new ComplexNumber[] {1, -1, 5, -5, 0.3, (-0.3), -2.817, 2.817, 0, 0, Math.PI, -Math.PI}.Attach(a => -a));
            val.CheckInvert(new ComplexNumber[] {1, 1, 0.25, 4, 9, 1.0 / 9, -5, -0.2}.Attach(a => 1.0 / a));
            val.CheckProduct(
                new ComplexNumber[] {0, 2, 0, 3, 6, 18, 0.25, 4, 1, 9, 0.23, 2.07, 1.25, 6.32, 7.9, ComplexNumber.ImaginaryUnit}.Group2().Attach(
                    (a, b) => a * b));
            val.CheckDivide(
                new ComplexNumber[]
                {
                    0, 2, 0, 3, 6, 0.5, 0.25, 4, 0.0625, 9, 0.24, 37.5, 1.25, 6.32,
                    0.19778481012658227848101265822785, ComplexNumber.ImaginaryUnit
                }.Group2().Attach((a, b) => a / b));
            val.CheckMod(new ComplexNumber[] {5, 3, 2, 12, 2.2, 1, 5.3, 1.1, 0.9, ComplexNumber.ImaginaryUnit }.Group2().Attach((a, b) => a % b));
            val.CheckSum(
                new ComplexNumber[] {0, 1, 1, 6.2, 8.3, 14.5, -2, 3, 1, 85, 100.32, 185.32}.Group2().Attach((a, b) => a + b));
            val.CheckDifference(new ComplexNumber[] {2, 1, 1, 3, 9, -6, 1.2, 1, 0.2, ComplexNumber.ImaginaryUnit }.Group2().Attach((a, b) => a - b));
            val.CheckConjugate(new ComplexNumber[] {1, 1, 0, 0, -3, -3, 5, 5, 0.69, 0.69, ComplexNumber.ImaginaryUnit }.Attach(a => a.Conjugate()));
            val.Checkpow(
                new ComplexNumber[]
                {
                    1, 6, 112, 2, 12544, 6, 0.5, 2.4494897427831780981972840747059, 0.5, 0.7,
                    0.61557220667245814224969653458387, ComplexNumber.ImaginaryUnit
                }.Group2().Attach((a, b) => a.pow(b)));
            val.CheckLog(
                new ComplexNumber[] {7.8, 2.0541237336955460528479733452617, Math.E, 1, 1000, 6.9077552789821370520539743640531}
                    .Attach(a => (a).log()));
            val.CheckDouble(new ComplexNumber[] {1.2, 6, 0, -0.0002}.Attach(a => (double?)a.RealPart));
            val.CheckString(new ComplexNumber[] {1.2, 6, 0, -0.0002}.Attach(a => a.ToString()));
            val.CheckInvertible(true);
            val.CheckParsable(true);
            val.CheckModuloable(true);
            val.CheckNegatable(true);
            val.CheckIsNegative(new ComplexNumber[] {1.5, 0.0, 0.2}.Attach(a => a.CompareTo(0) < 0));
            val.CheckOrder(OrderType.PartialOrder);
            val.CheckComparer(0, 1, 10, 52, 10000);
            val.CheckAbs(new ComplexNumber[] {0, 0, -2, 2, 3, 3, -3, 3, -1000, 1000, 1000, 1000}.Attach(a => (ComplexNumber)a.abs()));
            val.CheckFromInt(new int[] {0, 3, -2, 4, 9, 4}.Attach(a => (ComplexNumber)a));
            val.CheckFromFraction(new Tuple<int, int>[] {Tuple.Create(0, 9), Tuple.Create(9, 8), Tuple.Create(-8, 1)}.Attach((a, b) => (ComplexNumber)a / b));
            val.CheckPow(
                new Tuple<ComplexNumber, int>[] {Tuple.Create((ComplexNumber)0.0, 9), Tuple.Create((ComplexNumber)9.0, 3), Tuple.Create((ComplexNumber)(-8.0), 1)}.Attach(
                    (a, b) => a.pow(b)));
            val.CheckFactorial(new int[] {0, 9, 3}.Attach(a => (ComplexNumber)a.factorial()));
            val.CheckParse(new string[] {"1.5", "0.0", "-0.0002"}.Attach(ComplexNumber.Parse));
        }
        [TestMethod] public void DecimalTest()
        {
            var val = Fields.getField<decimal>();
            val.CheckZero(0);
            val.CheckOne(1);
            val.CheckBase((decimal)Math.E);
            val.CheckNegativeOne(-1);
            val.CheckNegate(
                new decimal[] {1, -1, 5, -5, (decimal)0.3, (decimal)-0.3, (decimal)-2.817, (decimal)2.817, 0, 0, (decimal)Math.PI, (decimal)-Math.PI}
                    .Attach(a => -a));
            val.CheckInvert(new decimal[] {1, 1, (decimal)0.25, 4, 9, (decimal)1.0 / 9, -5, (decimal)-0.2}.Attach(a => (decimal)1.0 / a));
            val.CheckProduct(
                new decimal[] {0, 2, 0, 3, 6, 18, (decimal)0.25, 4, 1, 9, (decimal)0.23, (decimal)2.07, (decimal)1.25, (decimal)6.32, (decimal)7.9}
                    .Group2().Attach((a, b) => a * b));
            val.CheckDivide(
                new decimal[]
                {
                    0, 2, 3, 6, (decimal)0.5, (decimal)0.25, 4, (decimal)0.0625, 9, (decimal)0.24, (decimal)37.5, (decimal)1.25, (decimal)6.32,
                    (decimal)0.19778481012658227848101265822785
                }.Group2().Attach((a, b) => a / b));
            val.CheckMod(
                new decimal[] {5, 3, 2, 12, (decimal)2.2, 1, (decimal)5.3, (decimal)1.1, (decimal)0.9, 6, 0, 2}.Group2().Attach((a, b) => a % b));
            val.CheckSum(
                new decimal[] {0, 1, 1, (decimal)6.2, (decimal)8.3, (decimal)14.5, -2, 3, 1, 85, (decimal)100.32, (decimal)185.32}.Group2().Attach(
                    (a, b) => a + b));
            val.CheckDifference(new decimal[] {2, 1, 1, 3, 9, -6, (decimal)1.2, 1, (decimal)0.2}.Group2().Attach((a, b) => a - b));
            val.CheckConjugate(new decimal[] {1, 1, 0, 0, -3, -3, 5, 5, (decimal)0.69, (decimal)0.69}.Attach(a => a));
            val.Checkpow(
                new decimal[]
                {
                    1, 6, 112, 2, 12544, 6, (decimal)0.5, (decimal)2.4494897427831780981972840747059, (decimal)0.5, (decimal)0.7,
                    (decimal)0.61557220667245814224969653458387
                }.Group2().Attach((a, b) => a.pow(b)));
            val.CheckLog(
                new decimal[]
                {(decimal)7.8, (decimal)2.0541237336955460528479733452617, (decimal)Math.E, 1, 1000, (decimal)6.9077552789821370520539743640531}
                    .Attach(a => (decimal)((double)a).log()));
            val.CheckDouble(new decimal[] {(decimal)1.2, 6, 0, (decimal)-0.0002}.Attach(a => (double?)a));
            val.CheckString(new decimal[] {(decimal)1.2, 6, 0, (decimal)-0.0002}.Attach(a => a.ToString()));
            val.CheckInvertible(true);
            val.CheckParsable(true);
            val.CheckModuloable(true);
            val.CheckNegatable(true);
            val.CheckIsNegative(new decimal[] {(decimal)1.5, (decimal)0.0, (decimal)0.2}.Attach(a => a < 0));
            val.CheckOrder(OrderType.TotalOrder);
            val.CheckComparer((decimal)-123.01, -123, -20, -9, 0, 1, 10, 52, 10000);
            val.CheckAbs(new decimal[] {0, 0, -2, 2, 3, 3, -3, 3, -1000, 1000, 1000, 1000}.Attach(a => a.abs()));
            val.CheckFromInt(new int[] {0, 3, -2, 4, 9, 4}.Attach(a => (decimal)a));
            val.CheckFromFraction(new Tuple<int, int>[] {Tuple.Create(0, 9), Tuple.Create(9, 8), Tuple.Create(-8, 1)}.Attach((a, b) => a / (decimal)b));
            val.CheckPow(
                new Tuple<decimal, int>[] {Tuple.Create((decimal)0.0, 9), Tuple.Create((decimal)9.0, 3), Tuple.Create((decimal)-8.0, 1)}.Attach(
                    (a, b) => a.pow(b)));
            val.CheckFactorial(new int[] {0, 9, 3}.Attach(a => (decimal)a.factorial()));
            val.CheckParse(new string[] {"1.5", "0.0", "-0.0002"}.Attach(decimal.Parse));
        }
        [TestMethod] public void RationalTest()
        {
            var val = Fields.getField<BigRational>();
            val.CheckZero(0);
            val.CheckOne(1);
            val.CheckBase(Math.E);
            val.CheckNegativeOne(-1);
            val.CheckNegate(new BigRational[] {1, -1, 5, -5, 0.3, -0.3, -2.817, 2.817, 0, 0, Math.PI, -Math.PI}
                .Attach(a => -a));
            val.CheckInvert(new BigRational[] {1, 1, 0.25, 4, 9, 1.0 / 9, -5, -0.2}.Attach(a => 1.0 / a));
            val.CheckProduct(
                new BigRational[] {0, 2, 0, 3, 6, 18, 0.25, 4, 1, 9, 0.23, 2.07, 1.25, 6.32, 7.9, 8.1}
                    .Group2().Attach((a, b) => a * b));
            val.CheckDivide(
                new BigRational[]
                {
                    0, 2, 3, 6, 0.5, 0.25, 4, 0.0625, 9, 0.24, 37.5, 1.25, 6.32,
                    0.19778481012658227848101265822785
                }.Group2().Attach((a, b) => a / b));
            val.CheckMod(
                new BigRational[] {5, 3, 2, 12, 2.2, 1, 5.3, 1.1, 0.9, 6, 0, 2}.Group2().Attach((a, b) => a % b));
            val.CheckSum(
                new BigRational[] {0, 1, 1, 6.2, 8.3, 14.5, -2, 3, 1, 85, 100.32, 185.32}.Group2().Attach(
                    (a, b) => a + b));
            val.CheckDifference(new BigRational[] {2, 1, 1, 3, 9, -6, 1.2, 1, 0.2, 21}.Group2().Attach((a, b) => a - b));
            val.CheckConjugate(new BigRational[] {1, 1, 0, 0, -3, -3, 5, 5, 0.69, 0.69}.Attach(a => a));
            val.Checkpow(
                new BigRational[]
                {
                    new BigRational(1, 3), new BigRational(2, 1), new BigRational(7, 8), new BigRational(2, 3)
                }.Group2().Attach((a, b) => a.pow(b, new BigRational(1, a.Denominator))));
            val.CheckLog(
                new BigRational[]
                {7.8, 2.0541237336955460528479733452617, Math.E, 1, 1000, 6.9077552789821370520539743640531}
                    .Attach(a => (BigRational)Math.Log((double)a)));
            val.CheckDouble(new BigRational[] {1.2, 6, 0, -0.0002}.Attach(a => (double?)a));
            val.CheckString(new BigRational[] {1.2, 6, 0, -0.0002}.Attach(a => a.ToString()));
            val.CheckInvertible(true);
            val.CheckParsable(true);
            val.CheckModuloable(true);
            val.CheckNegatable(true);
            val.CheckIsNegative(new BigRational[] {1.5, 0.0, 0.2}.Attach(a => a < 0));
            val.CheckOrder(OrderType.TotalOrder);
            val.CheckComparer(-123.01, -123, -20, -9, 0, 1, 10, 52, 10000);
            val.CheckAbs(new BigRational[] {0, 0, -2, 2, 3, 3, -3, 3, -1000, 1000, 1000, 1000}.Attach(a => a.abs()));
            val.CheckFromInt(new int[] {0, 3, -2, 4, 9, 4}.Attach(a => (BigRational)a));
            val.CheckFromFraction(
                new Tuple<int, int>[] {Tuple.Create(0, 9), Tuple.Create(9, 8), Tuple.Create(-8, 1)}.Attach((a, b) => a / (BigRational)b));
            var parr = new Tuple<BigRational, int>[]
            {Tuple.Create((BigRational)0.0, 9), Tuple.Create((BigRational)9.0, 3), Tuple.Create((BigRational)(-8.0), 1)}.Attach((a, b) =>
            {
                var ret= a.pow(b);
                return ret;
            }).ToArray();
            val.CheckPow(
                parr);
            val.CheckFactorial(new int[] {0, 9, 3}.Attach(a => (BigRational)a.factorial()));
            val.CheckParse(new string[] {"1.5", "0.0", "-0.0002"}.Attach(a => new BigRational(double.Parse(a))));
        }
        [TestMethod] public void FloatTest()
        {
            var val = Fields.getField<float>();
            val.CheckZero(0);
            val.CheckOne(1);
            val.CheckBase((float)Math.E);
            val.CheckNegativeOne(-1);
            val.CheckNegate(
                new float[] { 1, -1, 5, -5, (float)0.3, (float)-0.3, (float)-2.817, (float)2.817, 0, 0, (float)Math.PI, (float)-Math.PI }.Attach(a => -a));
            val.CheckInvert(new float[] { 1, 1, (float)0.25, 4, 9, (float)1.0 / 9, -5, (float)-0.2 }.Attach(a => (float)1.0 / a));
            val.CheckProduct(
                new float[] { 0, 2, 0, 3, 6, 18, (float)0.25, 4, 1, 9, (float)0.23, (float)2.07, (float)1.25, (float)6.32, (float)7.9 }.Group2().Attach(
                    (a, b) => a * b));
            val.CheckDivide(
                new float[]
                {
                    0, 2, 0, 3, 6, (float)0.5, (float)0.25, 4, (float)0.0625, 9, (float)0.24, (float)37.5, (float)1.25, (float)6.32,
                    (float)0.19778481012658227848101265822785
                }.Group2().Attach((a, b) => a / b));
            val.CheckMod(new float[] { 5, 3, 2, 12, (float)2.2, 1, (float)5.3, (float)1.1, (float)0.9 }.Group2().Attach((a, b) => a % b));
            val.CheckSum(
                new float[] { 0, 1, 1, (float)6.2, (float)8.3, (float)14.5, -2, 3, 1, 85, (float)100.32, (float)185.32 }.Group2().Attach((a, b) => a + b));
            val.CheckDifference(new float[] { 2, 1, 1, 3, 9, -6, (float)1.2, 1, (float)0.2 }.Group2().Attach((a, b) => a - b));
            val.CheckConjugate(new float[] { 1, 1, 0, 0, -3, -3, 5, 5, (float)0.69, (float)0.69 }.Attach(a => a));
            val.Checkpow(
                new float[]
                {
                    1, 6, 112, 2, 12544, 6, (float)0.5, (float)2.4494897427831780981972840747059, (float)0.5, (float)0.7,
                    (float)0.61557220667245814224969653458387
                }.Group2().Attach((a, b) => a.pow(b)));
            val.CheckLog(
                new float[] { (float)7.8, (float)2.0541237336955460528479733452617, (float)Math.E, 1, 1000, (float)6.9077552789821370520539743640531 }
                    .Attach(a => (float)((double)a).log()));
            val.CheckDouble(new float[] { (float)1.2, 6, 0, (float)-0.0002 }.Attach(a => (double?)a));
            val.CheckString(new float[] { (float)1.2, 6, 0, (float)-0.0002 }.Attach(a => a.ToString()));
            val.CheckInvertible(true);
            val.CheckParsable(true);
            val.CheckModuloable(true);
            val.CheckNegatable(true);
            val.CheckIsNegative(new float[] { (float)1.5, (float)0.0, (float)0.2 }.Attach(a => a < 0));
            val.CheckOrder(OrderType.TotalOrder);
            val.CheckComparer((float)-123.01, -123, -20, -9, 0, 1, 10, 52, 10000);
            val.CheckAbs(new float[] { 0, 0, -2, 2, 3, 3, -3, 3, -1000, 1000, 1000, 1000 }.Attach(a => a.abs()));
            val.CheckFromInt(new int[] { 0, 3, -2, 4, 9, 4 }.Attach(a => (float)a));
            val.CheckFromFraction(new Tuple<int, int>[] { Tuple.Create(0, 9), Tuple.Create(9, 8), Tuple.Create(-8, 1) }.Attach((a, b) => a / (float)b));
            val.CheckPow(
                new Tuple<float, int>[] { Tuple.Create((float)0.0, 9), Tuple.Create((float)9.0, 3), Tuple.Create((float)-8.0, 1) }.Attach(
                    (a, b) => a.pow(b)));
            val.CheckFactorial(new int[] { 0, 9, 3 }.Attach(a => (float)a.factorial()));
            val.CheckParse(new string[] { "1.5", "0.0", "-0.0002" }.Attach(float.Parse));
        }
    }
    [TestClass]
    public class SignedIntegerFieldTest
    {
        [TestMethod] public void IntTest()
        {
            var val = Fields.getField<int>();
            val.CheckZero(0);
            val.CheckOne(1);
            val.CheckBase(2);
            val.CheckNegativeOne(-1);
            val.CheckNegate(new int[] {1, 5, 2, 6, 8, 4, -4, 2, 3, -7, 5, -1, 2}.Attach(a => -a));
            val.CheckProduct(new int[] {0, 2, 0, 3, 6, 18, 3, 4, 1, 9, 6, 9, 8, 1, 6}.Group2().Attach((a, b) => a * b));
            val.CheckMod(new int[] {5, 3, 2, 12, 4, 102, 8, 2, 91, 6}.Group2().Attach((a, b) => a % b));
            val.CheckSum(new int[] {0, 1, 1, 62, 83, 145, -2, 3, 1, 85, 10032, 18532}.Group2().Attach((a, b) => a + b));
            val.CheckDifference(new int[] {2, 1, 1, 3, 9, -6, 12, 1, 2}.Group2().Attach((a, b) => a - b));
            val.CheckConjugate(new int[] {1, 1, 20, 0, -3, -13, 5, 15, 69, 169}.Attach(a => a));
            val.Checkpow(new int[] {1, 6, 1, 2, 1, 6, 5, 4, 5, 7, 6}.Group2().Attach((a, b) => a.pow(b)));
            val.CheckDouble(new int[] {1, 0, -3, -9, 10}.Attach(a => (double?)a));
            val.CheckString(new int[] {1, 0, 8, 12, -95, -87, -100, 100}.Attach(a => a.ToString()));
            val.CheckInvertible(false);
            val.CheckParsable(true);
            val.CheckModuloable(true);
            val.CheckNegatable(true);
            val.CheckIsNegative(new int[] {2, 0, -8, -1}.Attach(a => a < 0));
            val.CheckOrder(OrderType.TotalOrder);
            val.CheckComparer(-123, -20, -9, 0, 1, 10, 52, 10000);
            val.CheckAbs(new int[] {0, 0, -2, 2, 3, 3, -3, 3, -1000, 1000, 1000, 1000}.Attach(a => a.abs()));
            val.CheckFromInt(new int[] {0, 3, -2, 4, 9, 4}.Attach(a => a));
            val.CheckFromFraction(new Tuple<int, int>[] {Tuple.Create(0, 9), Tuple.Create(9, 8), Tuple.Create(-8, 1)}.Attach((a, b) => a / b));
            val.CheckPow(new int[] {0, 1, 8, 3, 9, 0, 4, 5, 2, 8, 1}.Group2().Attach((a, b) => a.pow(b)));
            val.CheckFactorial(new int[] {0, 9, 3}.Attach(a => (int)a.factorial()));
            val.CheckParse(new string[] {"1", "0", "-98", "785"}.Attach(int.Parse));
        }
        [TestMethod] public void LongTest()
        {
            var val = Fields.getField<long>();
            val.CheckZero(0);
            val.CheckOne(1);
            val.CheckBase(2);
            val.CheckNegativeOne(-1);
            val.CheckNegate(new long[] {1, 5, 2, 6, 8, 4, -4, 2, 3, -7, 5, -1, 2}.Attach(a => -a));
            val.CheckProduct(new long[] {0, 2, 0, 3, 6, 18, 3, 4, 1, 9, 6, 9, 8, 1, 6}.Group2().Attach((a, b) => a * b));
            val.CheckMod(new long[] {5, 3, 2, 12, 4, 102, 8, 2, 91, 6}.Group2().Attach((a, b) => a % b));
            val.CheckSum(new long[] {0, 1, 1, 62, 83, 145, -2, 3, 1, 85, 10032, 18532}.Group2().Attach((a, b) => a + b));
            val.CheckDifference(new long[] {2, 1, 1, 3, 9, -6, 12, 1, 2}.Group2().Attach((a, b) => a - b));
            val.CheckConjugate(new long[] {1, 1, 20, 0, -3, -13, 5, 15, 69, 169}.Attach(a => a));
            val.Checkpow(new long[] {1, 6, 1, 2, 1, 6, 5, 4, 5, 7, 6}.Group2().Attach((a, b) => a.pow(b)));
            val.CheckDouble(new long[] {1, 0, -3, -9, 10}.Attach(a => (double?)a));
            val.CheckString(new long[] {1, 0, 8, 12, -95, -87, -100, 100}.Attach(a => a.ToString()));
            val.CheckInvertible(false);
            val.CheckParsable(true);
            val.CheckModuloable(true);
            val.CheckNegatable(true);
            val.CheckIsNegative(new long[] {2, 0, -8, -1}.Attach(a => a < 0));
            val.CheckOrder(OrderType.TotalOrder);
            val.CheckComparer(-123, -20, -9, 0, 1, 10, 52, 10000);
            val.CheckAbs(new long[] {0, 0, -2, 2, 3, 3, -3, 3, -1000, 1000, 1000, 1000}.Attach(a => a.abs()));
            val.CheckFromInt(new int[] {0, 3, -2, 4, 9, 4}.Attach(a => (long)a));
            val.CheckFromFraction(new Tuple<int, int>[] {Tuple.Create(0, 9), Tuple.Create(9, 8), Tuple.Create(-8, 1)}.Attach((a, b) => a / (long)b));
            val.CheckPow(
                new Tuple<long, int>[]
                {Tuple.Create<long, int>(12, 3), Tuple.Create<long, int>(1, 3), Tuple.Create<long, int>(0, 3), Tuple.Create<long, int>(12, 0)}.Attach(
                    (a, b) => a.pow(b)));
            val.CheckFactorial(new int[] {0, 9, 3}.Attach(a => (long)a.factorial()));
            val.CheckParse(new string[] {"1", "0", "-98", "785"}.Attach(long.Parse));
        }
        [TestMethod] public void ShortTest()
        {
            var val = Fields.getField<short>();
            val.CheckZero<short>(0);
            val.CheckOne<short>(1);
            val.CheckBase<short>(2);
            val.CheckNegativeOne<short>(-1);
            val.CheckNegate(new short[] {1, 5, 2, 6, 8, 4, -4, 2, 3, -7, 5, -1, 2}.Attach(a => (short)-a));
            val.CheckProduct(new short[] {0, 2, 0, 3, 6, 18, 3, 4, 1, 9, 6, 9, 8, 1, 6}.Group2().Attach((a, b) => (short)(a * b)));
            val.CheckMod(new short[] {5, 3, 2, 12, 4, 102, 8, 2, 91, 6}.Group2().Attach((a, b) => (short)(a % b)));
            val.CheckSum(new short[] {0, 1, 1, 62, 83, 145, -2, 3, 1, 85, 10032, 18532}.Group2().Attach((a, b) => (short)(a + b)));
            val.CheckDifference(new short[] {2, 1, 1, 3, 9, -6, 12, 1, 2}.Group2().Attach((a, b) => (short)(a - b)));
            val.CheckConjugate(new short[] {1, 1, 20, 0, -3, -13, 5, 15, 69, 169}.Attach(a => a));
            val.Checkpow(new short[] {1, 6, 1, 2, 1, 6, 5, 4, 5, 7, 6}.Group2().Attach((a, b) => a.pow(b)));
            val.CheckDouble(new short[] {1, 0, -3, -9, 10}.Attach(a => (double?)a));
            val.CheckString(new short[] {1, 0, 8, 12, -95, -87, -100, 100}.Attach(a => a.ToString()));
            val.CheckInvertible(false);
            val.CheckParsable(true);
            val.CheckModuloable(true);
            val.CheckNegatable(true);
            val.CheckIsNegative(new short[] {2, 0, -8, -1}.Attach(a => a < 0));
            val.CheckOrder(OrderType.TotalOrder);
            val.CheckComparer((short)-123, (short)-20, (short)-9, (short)0, (short)1, (short)10, (short)52, (short)10000);
            val.CheckAbs(new short[] {0, 0, -2, 2, 3, 3, -3, 3, -1000, 1000, 1000, 1000}.Attach(a => a.abs()));
            val.CheckFromInt(new int[] {0, 3, -2, 4, 9, 4}.Attach(a => (short)a));
            val.CheckFromFraction(
                new Tuple<int, int>[] {Tuple.Create(0, 9), Tuple.Create(9, 8), Tuple.Create(-8, 1)}.Attach((a, b) => (short)(a / (short)b)));
            val.CheckPow(
                new Tuple<short, int>[]
                {Tuple.Create<short, int>(12, 3), Tuple.Create<short, int>(1, 3), Tuple.Create<short, int>(0, 3), Tuple.Create<short, int>(12, 0)}
                    .Attach((a, b) => a.pow((short)b)));
            val.CheckFactorial(new int[] {0, 9, 3}.Attach(a => (short)a.factorial()));
            val.CheckParse(new string[] {"1", "0", "-98", "785"}.Attach(short.Parse));
        }
        [TestMethod] public void SbyteTest()
        {
            var val = Fields.getField<sbyte>();
            val.CheckZero<sbyte>(0);
            val.CheckOne<sbyte>(1);
            val.CheckBase<sbyte>(2);
            val.CheckNegativeOne<sbyte>(-1);
            val.CheckNegate(new sbyte[] {1, 5, 2, 6, 8, 4, -4, 2, 3, -7, 5, -1, 2}.Attach(a => (sbyte)-a));
            val.CheckProduct(new sbyte[] {0, 2, 0, 3, 6, 18, 3, 4, 1, 9, 6, 9, 8, 1, 6}.Group2().Attach((a, b) => (sbyte)(a * b)));
            val.CheckMod(new sbyte[] {5, 3, 2, 12, 4, 102, 8, 2, 91, 6}.Group2().Attach((a, b) => (sbyte)(a % b)));
            val.CheckSum(new sbyte[] {0, 1, 1, 62, 83, 12, -2, 3, 1, 85, 32, 52}.Group2().Attach((a, b) => (sbyte)(a + b)));
            val.CheckDifference(new sbyte[] {2, 1, 1, 3, 9, -6, 12, 1, 2}.Group2().Attach((a, b) => (sbyte)(a - b)));
            val.CheckConjugate(new sbyte[] {1, 1, 20, 0, -3, -13, 5, 15, 69, 95}.Attach(a => a));
            val.Checkpow(new sbyte[] {1, 6, 1, 2, 1, 6, 5, 4, 5, 7, 6}.Group2().Attach((a, b) => a.pow(b)));
            val.CheckDouble(new sbyte[] {1, 0, -3, -9, 10}.Attach(a => (double?)a));
            val.CheckString(new sbyte[] {1, 0, 8, 12, -95, -87, -100, 100}.Attach(a => a.ToString()));
            val.CheckInvertible(false);
            val.CheckParsable(true);
            val.CheckModuloable(true);
            val.CheckNegatable(true);
            val.CheckIsNegative(new sbyte[] {2, 0, -8, -1}.Attach(a => a < 0));
            val.CheckOrder(OrderType.TotalOrder);
            val.CheckComparer((sbyte)-123, (sbyte)-20, (sbyte)-9, (sbyte)0, (sbyte)1, (sbyte)10, (sbyte)52, (sbyte)100);
            val.CheckAbs(new sbyte[] {0, 0, -2, 2, 3, 3, -3, 3, -100}.Attach(a => a.abs()));
            val.CheckFromInt(new int[] {0, 3, -2, 4, 9, 4}.Attach(a => (sbyte)a));
            val.CheckFromFraction(
                new Tuple<int, int>[] {Tuple.Create(0, 9), Tuple.Create(9, 8), Tuple.Create(-8, 1)}.Attach((a, b) => (sbyte)(a / (sbyte)b)));
            val.CheckPow(
                new Tuple<sbyte, int>[]
                {Tuple.Create<sbyte, int>(12, 3), Tuple.Create<sbyte, int>(1, 3), Tuple.Create<sbyte, int>(0, 3), Tuple.Create<sbyte, int>(12, 0)}
                    .Attach((a, b) => a.pow((sbyte)b)));
            val.CheckFactorial(new int[] {0, 9, 3}.Attach(a => (sbyte)a.factorial()));
            val.CheckParse(new string[] {"1", "0", "-98", "85"}.Attach(sbyte.Parse));
        }
        [TestMethod] public void BigintegerTest()
        {
            var val = Fields.getField<BigInteger>();
            val.CheckZero(0);
            val.CheckOne(1);
            val.CheckBase(2);
            val.CheckNegativeOne(-1);
            val.CheckNegate(new BigInteger[] {1, 5, 2, 6, 8, 4, -4, 2, 3, -7, 5, -1, 2}.Attach(a => -a));
            val.CheckProduct(new BigInteger[] {0, 2, 0, 3, 6, 18, 3, 4, 1, 9, 6, 9, 8, 1, 6}.Group2().Attach((a, b) => a * b));
            val.CheckMod(new BigInteger[] {5, 3, 2, 12, 4, 102, 8, 2, 91, 6}.Group2().Attach((a, b) => a % b));
            val.CheckSum(new BigInteger[] {0, 1, 1, 62, 83, 145, -2, 3, 1, 85, 10032, 18532}.Group2().Attach((a, b) => a + b));
            val.CheckDifference(new BigInteger[] {2, 1, 1, 3, 9, -6, 12, 1, 2}.Group2().Attach((a, b) => a - b));
            val.CheckConjugate(new BigInteger[] {1, 1, 20, 0, -3, -13, 5, 15, 69, 169}.Attach(a => a));
            val.Checkpow(new BigInteger[] {1, 6, 1, 2, 1, 6, 5, 4, 5, 7, 6}.Group2().Attach((a, b) => a.pow(b)));
            val.CheckDouble(new BigInteger[] {1, 0, -3, -9, 10}.Attach(a => (double?)a));
            val.CheckString(new BigInteger[] {1, 0, 8, 12, -95, -87, -100, 100}.Attach(a => a.ToString()));
            val.CheckInvertible(false);
            val.CheckParsable(true);
            val.CheckModuloable(true);
            val.CheckNegatable(true);
            val.CheckIsNegative(new BigInteger[] {2, 0, -8, -1}.Attach(a => a < 0));
            val.CheckOrder(OrderType.TotalOrder);
            val.CheckComparer(-123, -20, -9, 0, 1, 10, 52, 10000);
            val.CheckAbs(new BigInteger[] {0, 0, -2, 2, 3, 3, -3, 3, -1000, 1000, 1000, 1000}.Attach(BigInteger.Abs));
            val.CheckFromInt(new int[] {0, 3, -2, 4, 9, 4}.Attach(a => (BigInteger)a));
            val.CheckFromFraction(
                new Tuple<int, int>[] {Tuple.Create(0, 9), Tuple.Create(9, 8), Tuple.Create(-8, 1)}.Attach((a, b) => a / (BigInteger)b));
            val.CheckPow(
                new Tuple<BigInteger, int>[]
                {
                    Tuple.Create<BigInteger, int>(12, 3), Tuple.Create<BigInteger, int>(1, 3), Tuple.Create<BigInteger, int>(0, 3),
                    Tuple.Create<BigInteger, int>(12, 0)
                }.Attach((a, b) => a.pow(b)));
            val.CheckFactorial(new int[] {0, 9, 3}.Attach(a => (BigInteger)a.factorial()));
            val.CheckParse(new string[] {"1", "0", "-98", "785"}.Attach(BigInteger.Parse));
        }
    }
    [TestClass]
    public class UnignedIntegerFieldTest
    {
        [TestMethod] public void UintTest()
        {
            var val = Fields.getField<uint>();
            val.CheckZero<uint>(0);
            val.CheckOne<uint>(1);
            val.CheckBase<uint>(2);
            val.CheckProduct(new uint[] {0, 2, 0, 3, 6, 18, 3, 4, 1, 9, 6, 9, 8, 1, 6}.Group2().Attach((a, b) => a * b));
            val.CheckMod(new uint[] {5, 3, 2, 12, 4, 102, 8, 2, 91, 6}.Group2().Attach((a, b) => a % b));
            val.CheckSum(new uint[] {0, 1, 1, 62, 83, 145, 2, 3, 1, 85, 10032, 18532}.Group2().Attach((a, b) => a + b));
            val.CheckDifference(new uint[] {2, 1, 1, 3, 9, 6, 12, 1, 2}.Group2().Attach((a, b) => a - b));
            val.CheckConjugate(new uint[] {1, 1, 20, 0, 3, 13, 5, 15, 69, 169}.Attach(a => a));
            val.Checkpow(new uint[] {1, 6, 1, 2, 1, 6, 5, 4, 5, 7, 6}.Group2().Attach((a, b) => a.pow(b)));
            val.CheckDouble(new uint[] {1, 0, 3, 9, 10}.Attach(a => (double?)a));
            val.CheckString(new uint[] {1, 0, 8, 12, 95, 87, 100}.Attach(a => a.ToString()));
            val.CheckInvertible(false);
            val.CheckParsable(true);
            val.CheckModuloable(true);
            val.CheckNegatable(false);
            val.CheckIsNegative(new uint[] {2, 0, 8, 1}.Attach(a => false));
            val.CheckOrder(OrderType.TotalOrder);
            val.CheckComparer<uint>(0, 1, 10, 52, 10000);
            val.CheckAbs(new uint[] {0, 0, 2, 3, 1000}.Attach(a => a));
            val.CheckFromInt(new int[] {0, 3, 2, 4, 9, 4}.Attach(a => (uint)a));
            val.CheckFromFraction(
                new Tuple<int, int>[] {Tuple.Create(0, 9), Tuple.Create(9, 8), Tuple.Create(8, 1)}.Attach((a, b) => (uint)a / (uint)b));
            val.CheckPow(
                new Tuple<uint, int>[]
                {Tuple.Create<uint, int>(12, 3), Tuple.Create<uint, int>(1, 3), Tuple.Create<uint, int>(0, 3), Tuple.Create<uint, int>(12, 0)}.Attach(
                    (a, b) => a.pow((uint)b)));
            val.CheckFactorial(new int[] {0, 9, 3}.Attach(a => (uint)a.factorial()));
            val.CheckParse(new string[] {"1", "0", "98", "785"}.Attach(uint.Parse));
        }
        [TestMethod] public void UlongTest()
        {
            var val = Fields.getField<ulong>();
            val.CheckZero<ulong>(0);
            val.CheckOne<ulong>(1);
            val.CheckBase<ulong>(2);
            val.CheckProduct(new ulong[] {0, 2, 0, 3, 6, 18, 3, 4, 1, 9, 6, 9, 8, 1, 6}.Group2().Attach((a, b) => a * b));
            val.CheckMod(new ulong[] {5, 3, 2, 12, 4, 102, 8, 2, 91, 6}.Group2().Attach((a, b) => a % b));
            val.CheckSum(new ulong[] {0, 1, 1, 62, 83, 145, 2, 3, 1, 85, 10032, 18532}.Group2().Attach((a, b) => a + b));
            val.CheckDifference(new ulong[] {2, 1, 1, 3, 9, 6, 12, 1, 2}.Group2().Attach((a, b) => a - b));
            val.CheckConjugate(new ulong[] {1, 1, 20, 0, 3, 13, 5, 15, 69, 169}.Attach(a => a));
            val.Checkpow(new ulong[] {1, 6, 1, 2, 1, 6, 5, 4, 5, 7, 6}.Group2().Attach((a, b) => a.pow(b)));
            val.CheckDouble(new ulong[] {1, 0, 3, 9, 10}.Attach(a => (double?)a));
            val.CheckString(new ulong[] {1, 0, 8, 12, 95, 87, 100}.Attach(a => a.ToString()));
            val.CheckInvertible(false);
            val.CheckParsable(true);
            val.CheckModuloable(true);
            val.CheckNegatable(false);
            val.CheckIsNegative(new ulong[] {2, 0, 8, 1}.Attach(a => false));
            val.CheckOrder(OrderType.TotalOrder);
            val.CheckComparer<ulong>(0, 1, 10, 52, 10000);
            val.CheckAbs(new ulong[] {0, 0, 2, 3, 1000}.Attach(a => a));
            val.CheckFromInt(new int[] {0, 3, 2, 4, 9, 4}.Attach(a => (ulong)a));
            val.CheckFromFraction(
                new Tuple<int, int>[] {Tuple.Create(0, 9), Tuple.Create(9, 8), Tuple.Create(8, 1)}.Attach((a, b) => (ulong)a / (ulong)b));
            val.CheckPow(
                new Tuple<ulong, int>[]
                {Tuple.Create<ulong, int>(12, 3), Tuple.Create<ulong, int>(1, 3), Tuple.Create<ulong, int>(0, 3), Tuple.Create<ulong, int>(12, 0)}
                    .Attach(
                        (a, b) => a.pow((ulong)b)));
            val.CheckFactorial(new int[] {0, 9, 3}.Attach(a => a.factorial()));
            val.CheckParse(new string[] {"1", "0", "98", "785"}.Attach(ulong.Parse));
        }
        [TestMethod] public void UshortTest()
        {
            var val = Fields.getField<ushort>();
            val.CheckZero<ushort>(0);
            val.CheckOne<ushort>(1);
            val.CheckBase<ushort>(2);
            val.CheckProduct(new ushort[] {0, 2, 0, 3, 6, 18, 3, 4, 1, 9, 6, 9, 8, 1, 6}.Group2().Attach((a, b) => (ushort)(a * b)));
            val.CheckMod(new ushort[] {5, 3, 2, 12, 4, 102, 8, 2, 91, 6}.Group2().Attach((a, b) => (ushort)(a % b)));
            val.CheckSum(new ushort[] {0, 1, 1, 62, 83, 145, 2, 3, 1, 85, 10032, 18532}.Group2().Attach((a, b) => (ushort)(a + b)));
            val.CheckDifference(new ushort[] {2, 1, 1, 3, 9, 6, 12, 1, 2}.Group2().Attach((a, b) => (ushort)(a - b)));
            val.CheckConjugate(new ushort[] {1, 1, 20, 0, 3, 13, 5, 15, 69, 169}.Attach(a => a));
            val.Checkpow(new ushort[] {1, 6, 1, 2, 1, 6, 5, 4, 5, 7, 6}.Group2().Attach((a, b) => a.pow(b)));
            val.CheckDouble(new ushort[] {1, 0, 3, 9, 10}.Attach(a => (double?)a));
            val.CheckString(new ushort[] {1, 0, 8, 12, 95, 87, 100}.Attach(a => a.ToString()));
            val.CheckInvertible(false);
            val.CheckParsable(true);
            val.CheckModuloable(true);
            val.CheckNegatable(false);
            val.CheckIsNegative(new ushort[] {2, 0, 8, 1}.Attach(a => false));
            val.CheckOrder(OrderType.TotalOrder);
            val.CheckComparer<ushort>(0, 1, 10, 52, 10000);
            val.CheckAbs(new ushort[] {0, 0, 2, 3, 1000}.Attach(a => a));
            val.CheckFromInt(new int[] {0, 3, 2, 4, 9, 4}.Attach(a => (ushort)a));
            val.CheckFromFraction(
                new Tuple<int, int>[] {Tuple.Create(0, 9), Tuple.Create(9, 8), Tuple.Create(8, 1)}.Attach((a, b) => (ushort)((ushort)a / (ushort)b)));
            val.CheckPow(
                new Tuple<ushort, int>[]
                {Tuple.Create<ushort, int>(12, 3), Tuple.Create<ushort, int>(1, 3), Tuple.Create<ushort, int>(0, 3), Tuple.Create<ushort, int>(12, 0)}
                    .Attach(
                        (a, b) => a.pow((ushort)b)));
            val.CheckFactorial(new int[] {0, 9, 3}.Attach(a => (ushort)a.factorial()));
            val.CheckParse(new string[] {"1", "0", "98", "785"}.Attach(ushort.Parse));
        }
        [TestMethod] public void byteTest()
        {
            var val = Fields.getField<byte>();
            val.CheckZero<byte>(0);
            val.CheckOne<byte>(1);
            val.CheckBase<byte>(2);
            val.CheckProduct(new byte[] {0, 2, 0, 3, 6, 18, 3, 4, 1, 9, 6, 9, 8, 1, 6}.Group2().Attach((a, b) => (byte)(a * b)));
            val.CheckMod(new byte[] {5, 3, 2, 12, 4, 102, 8, 2, 91, 6}.Group2().Attach((a, b) => (byte)(a % b)));
            val.CheckSum(new byte[] {0, 1, 1, 62, 83, 145, 2, 3, 1, 85, 100, 32}.Group2().Attach((a, b) => (byte)(a + b)));
            val.CheckDifference(new byte[] {2, 1, 1, 3, 9, 6, 12, 1, 2}.Group2().Attach((a, b) => (byte)(a - b)));
            val.CheckConjugate(new byte[] {1, 1, 20, 0, 3, 13, 5, 15, 69, 169}.Attach(a => a));
            val.Checkpow(new byte[] {1, 6, 1, 2, 1, 6, 5, 4, 5, 7, 6}.Group2().Attach((a, b) => a.pow(b)));
            val.CheckDouble(new byte[] {1, 0, 3, 9, 10}.Attach(a => (double?)a));
            val.CheckString(new byte[] {1, 0, 8, 12, 95, 87, 100}.Attach(a => a.ToString()));
            val.CheckInvertible(false);
            val.CheckParsable(true);
            val.CheckModuloable(true);
            val.CheckNegatable(false);
            val.CheckIsNegative(new byte[] {2, 0, 8, 1}.Attach(a => false));
            val.CheckOrder(OrderType.TotalOrder);
            val.CheckComparer<byte>(0, 1, 10, 52, 100);
            val.CheckAbs(new byte[] {0, 0, 2, 3, 10}.Attach(a => a));
            val.CheckFromInt(new int[] {0, 3, 2, 4, 9, 4}.Attach(a => (byte)a));
            val.CheckFromFraction(
                new Tuple<int, int>[] {Tuple.Create(0, 9), Tuple.Create(9, 8), Tuple.Create(8, 1)}.Attach((a, b) => (byte)((byte)a / (byte)b)));
            val.CheckPow(
                new Tuple<byte, int>[]
                {Tuple.Create<byte, int>(12, 3), Tuple.Create<byte, int>(1, 3), Tuple.Create<byte, int>(0, 3), Tuple.Create<byte, int>(12, 0)}.Attach(
                    (a, b) => a.pow((byte)b)));
            val.CheckFactorial(new int[] {0, 9, 3}.Attach(a => (byte)a.factorial()));
            val.CheckParse(new string[] {"1", "0", "98", "85"}.Attach(byte.Parse));
        }
    }
    [TestClass]
    public class MiscFieldTest
    {
        [TestMethod] public void StringTest()
        {
            var val = Fields.getField<string>();
            val.CheckZero("");
            val.CheckOne("");
            val.CheckBase("");
            val.CheckSum(false, new string[] {"a", "cas", "", "strion", "hello world", "sdad"}.Group2().Attach((a, b) => a + b));
            val.CheckComparer("", "aaaaa", "bbbsd", "sretr", "t", "tttt", "z");
            val.CheckFromInt(new int[] {0, 5, 1, 3, 9, 10}.Attach(a => new string(' ', a)));
            val.CheckAbs(new string[] {"asd", "fddf", "tf", "oopopo"}.Attach(a => a));
            val.CheckConjugate(new string[] {"asd", "fddf", "tf", "oopopo"}.Attach(a => a));
            val.CheckIsNegative(new string[] {"asd", "fddf", "tf", "oopopo"}.Attach(a => false));
            val.CheckDouble(
                new string[] {"12.6", "9", "87.12", "-3.6"}.Attach(a => (double?)double.Parse(a)).Concat(
                    new string[] {"", "ab", "-ab"}.Attach(a => (double?)null)));
            val.CheckInvertible(false);
            val.CheckNegatable(false);
            val.CheckModuloable(false);
            val.CheckString(new string[] {"asd", "fddf", "tf", "oopopo"}.Attach(a => a));
            val.CheckParsable(true);
            val.CheckParse(new string[] {"asd", "fddf", "tf", "oopopo"}.Attach(a => a));
            val.CheckOrder(OrderType.TotalOrder);
        }
        [TestMethod] public void BoolTest()
        {
            var val = Fields.getField<bool>();
            val.CheckZero(false);
            val.CheckOne(true);
            val.CheckBase(true);
            val.CheckNegativeOne<bool>(true);
            val.CheckNegate(new bool[] {true, false}.Attach(a => a));
            val.CheckProduct(new bool[] {true, true, false, true, false, false}.Group2().Attach((a, b) => (a && b)));
            val.CheckMod(new bool[] {true, true, false, true}.Group2().Attach((a, b) => false));
            val.CheckSum(new bool[] {true, true, false, true, false, false}.Group2().Attach((a, b) => (a ^ b)));
            val.CheckDifference(new bool[] {true, true, false, true, false, false}.Group2().Attach((a, b) => (a ^ !b)));
            val.CheckConjugate(new bool[] {true, true, false, true, false, false}.Attach(a => a));
            val.Checkpow(new bool[] {true, true, false, true}.Group2().Attach((a, b) => !b || a));
            val.CheckDouble(new bool[] {true, false}.Attach<bool, double?>(a => a ? 1.0 : 0.0));
            val.CheckString(new bool[] {true, false}.Attach(a => a.ToString()));
            val.CheckInvert(true.Enumerate().Attach(a => true));
            val.CheckInvertible(true);
            val.CheckParsable(true);
            val.CheckModuloable(true);
            val.CheckNegatable(true);
            val.CheckIsNegative(new bool[] {true, false}.Attach(a => false));
            val.CheckOrder(OrderType.ReflexiveZero);
            val.CheckComparer(false, true);
            val.CheckAbs(new bool[] {true, false}.Attach(a => a));
            val.CheckFromInt(new int[] {0, 3, -2, 4, 9, 4}.Attach(a => a.TrueMod(2) == 1));
            val.CheckFromFraction(
                new Tuple<int, int>[] {Tuple.Create(0, 9), Tuple.Create(9, 1), Tuple.Create(-8, 1)}.Attach((a, b) => a.TrueMod(2) == 1));
            val.CheckPow(
                new Tuple<bool, int>[]
                {
                    Tuple.Create<bool, int>(true, 3), Tuple.Create<bool, int>(false, 3), Tuple.Create<bool, int>(false, 0),
                    Tuple.Create<bool, int>(true, 0)
                }
                    .Attach((a, b) => a && b % 2 == 1));
            val.CheckFactorial(new int[] {0, 9, 3}.Attach(a => a <= 1));
            val.CheckParse(new string[] {"True", "False"}.Attach(bool.Parse));
        }
    }
}