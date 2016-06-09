using System;
using System.Linq;
using Edge.Looping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Edge.Arrays.Arr2D.Array2D;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class FillNoFuncTest
    {
        [TestMethod] public void Simple()
        {
            var val = Fill(10, 10, 3);
            foreach (int i in val)
                AreEqual(i, 3);
        }
        [TestMethod] public void NoVal()
        {
            var val = Fill<int>(10, 10);
            foreach (int i in val)
                AreEqual(i, default(int));
        }
        [TestMethod] public void ZeroArg0()
        {
            var val = Fill(0, 10, 3);
            foreach (int i in val)
                AreEqual(i, 3);
        }
        [TestMethod] public void ZeroArg1()
        {
            var val = Fill(10, 0, 3);
            foreach (int i in val)
                AreEqual(i, 3);
        }
        [TestMethod] public void ZeroArgs()
        {
            var val = Fill(0, 0, 3);
            foreach (int i in val)
                AreEqual(i, 3);
        }
        [TestMethod, ExpectedException(typeof(OverflowException))] public void NegArg0()
        {
            var val = Fill(-1, 10, 3);
            Fail();
        }
        [TestMethod, ExpectedException(typeof(OverflowException))] public void NegArg1()
        {
            var val = Fill(10, -1, 3);
            Fail();
        }
        [TestMethod, ExpectedException(typeof(OverflowException))] public void NegArgs()
        {
            var val = Fill(-1, -1, 3);
            Fail();
        }
    }
    [TestClass]
    public class FillWithFuncTest
    {
        [TestMethod] public void Simple()
        {
            var val = Fill(10, 10, (a, b) => a * b);
            foreach (var i in val.CoordinateBind())
                AreEqual(i.Item1, i.Item2 * i.Item3);
        }
        [TestMethod] public void ZeroArg0()
        {
            var val = Fill(0, 10, (a, b) => a * b);
            foreach (var i in val.CoordinateBind())
                AreEqual(i.Item1, i.Item2 * i.Item3);
        }
        [TestMethod] public void ZeroArg1()
        {
            var val = Fill(10, 0, (a, b) => a * b);
            foreach (var i in val.CoordinateBind())
                AreEqual(i.Item1, i.Item2 * i.Item3);
        }
        [TestMethod] public void ZeroArgs()
        {
            var val = Fill(0, 0, (a, b) => a * b);
            foreach (var i in val.CoordinateBind())
                AreEqual(i.Item1, i.Item2 * i.Item3);
        }
        [TestMethod, ExpectedException(typeof(OverflowException))] public void NegArg0()
        {
            var val = Fill(-1, 10, (a, b) => a * b);
            Fail();
        }
        [TestMethod, ExpectedException(typeof(OverflowException))] public void NegArg1()
        {
            var val = Fill(10, -1, (a, b) => a * b);
            Fail();
        }
        [TestMethod, ExpectedException(typeof(OverflowException))] public void NegArgs()
        {
            var val = Fill(-1, -1, (a, b) => a * b);
            Fail();
        }
    }
    [TestClass]
    public class GetSizeTest
    {
        [TestMethod] public void Simple()
        {
            IsTrue(new int[3, 4].getSize().SequenceEqual(new[] {3, 4}));
        }
        [TestMethod] public void oneD()
        {
            IsTrue(new int[3].getSize().SequenceEqual(new[] {3}));
        }
        [TestMethod] public void multiD()
        {
            IsTrue(new int[1, 2, 3, 4, 5, 0, 1, 2, 3, 4].getSize().SequenceEqual(new[] {1, 2, 3, 4, 5, 0, 1, 2, 3, 4}));
        }
    }
    [TestClass]
    public class To2DArrTest
    {
        [TestMethod] public void Simple()
        {
            var orig = new[] {0, 2, 4, 1, 3, 5};
            IsTrue(orig.to2DArr(3).getSize().SequenceEqual(new[] {3, 2}));
        }
    }
    [TestClass]
    public class IsWithinBoundsTest
    {
        [TestMethod] public void SimpleTrue()
        {
            var val = new int[10, 10];
            IsTrue(val.isWithinBounds(9, 9));
        }
        [TestMethod] public void SimpleFalse()
        {
            var val = new int[10, 10];
            IsFalse(val.isWithinBounds(15, 15));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)] public void BadArgs()
        {
            var val = new int[10, 10];
            val.isWithinBounds(15);
            Fail();
        }
        [TestMethod] public void NegCoor()
        {
            var val = new int[10];
            IsFalse(val.isWithinBounds(-1));
        }
        [TestMethod] public void ZeroAccess()
        {
            var val = new int[10];
            IsTrue(val.isWithinBounds(0));
        }
        [TestMethod] public void ZerLength()
        {
            var val = new int[0];
            IsFalse(val.isWithinBounds(0));
        }
    }
    [TestClass]
    public class To2DPrintableTest
    {
        [TestMethod] public void Simple()
        {
            AreEqual((new[,] {{0, 1, 2}, {3, 2, 1}}).ToTablePrintable(), @"/0 1 2\" + Environment.NewLine + @"\3 2 1/" + Environment.NewLine);
        }
        [TestMethod] public void Empty()
        {
            AreEqual((new int[,] {}).ToTablePrintable(), "");
        }
    }
    [TestClass]
    public class ConcatTest
    {
        [TestMethod] public void Simple0()
        {
            var val0 = new[,] {{0, 1}, {4, 5}};
            var val1 = new[,] {{2, 3}, {6, 7}, {8, 7}};
            var con = val0.Concat(val1, 0).Cast<int>().ToArray();
            var exp = new[,] {{0, 1}, {4, 5}, {2, 3}, {6, 7}, {8, 7}}.Cast<int>().ToArray();
            IsTrue(con.SequenceEqual(exp));
        }
        [TestMethod] public void Simple1()
        {
            var val0 = new[,] {{0, 1}, {4, 5}};
            var val1 = new[,] {{2, 3, 0}, {6, 7, 0}};
            var con = val0.Concat(val1, 1).Cast<int>().ToArray();
            var exp = new[,] {{0, 1, 2, 3, 0}, {4, 5, 6, 7, 0}}.Cast<int>().ToArray();
            IsTrue(con.SequenceEqual(exp));
        }
    }
}