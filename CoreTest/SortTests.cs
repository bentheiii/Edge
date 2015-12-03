using System;
using System.Collections.Generic;
using System.Linq;
using Edge.Arrays;
using Edge.Comparison;
using Edge.SystemExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class SortTestsNoComp
    {
        [TestMethod] public void Simple()
        {
            var val = new int[] {0, 1, 1, 2, 0, 1, 0, 0, 5, 0};
            IsTrue(val.Sort().SequenceEqual(new int[] {0, 0, 0, 0, 0, 1, 1, 1, 2, 5}));
        }
        [TestMethod] public void Empty()
        {
            var val = new int[] {};
            IsTrue(val.Sort().SequenceEqual(new int[] {}));
        }
        [TestMethod] public void Chars()
        {
            var val = new char[] {'a', 'z', 'd', 'k'};
            IsTrue(val.Sort().SequenceEqual(new char[] {'a', 'd', 'k', 'z'}));
        }
        [TestMethod] public void Negatives()
        {
            var val = new int[] {0, 1, 1, 2, 0, 1, 0, 0, 5, 0, -5, -10, -7, -1};
            IsTrue(val.Sort().SequenceEqual(new int[] {-10, -7, -5, -1, 0, 0, 0, 0, 0, 1, 1, 1, 2, 5}));
        }
    }
    [TestClass]
    public class SortTestsWithComp
    {
        private static readonly IComparer<int> _comp = new FunctionComparer<int>(a => a.abs());
        [TestMethod] public void Simple()
        {
            var val = new int[] {0, 1, 1, 2, 0, 1, 0, 0, 5, 0};
            IsTrue(val.Sort(_comp).SequenceEqual(new int[] {0, 0, 0, 0, 0, 1, 1, 1, 2, 5}));
        }
        [TestMethod] public void Empty()
        {
            var val = new int[] {};
            IsTrue(val.Sort(_comp).SequenceEqual(new int[] {}));
        }
        [TestMethod] public void Negatives()
        {
            var val = new int[] {0, 1, 1, 2, 0, 1, 0, 0, 5, 0, -10, -7};
            IsTrue(val.Sort(_comp).SequenceEqual(new int[] {0, 0, 0, 0, 0, 1, 1, 1, 2, 5, -7, -10}));
        }
    }
    [TestClass]
    public class MedianTest
    {
        [TestMethod] public void Simple()
        {
            var val = new int[] {0, 1, 1, 2, 0, 1, 0, 0, 5, 0};
            AreEqual(val.getMedian(), 1);
        }
        [TestMethod] public void WithIndexes()
        {
            var val = new int[] {0, 3, 1, 2, 4};
            int ind;
            AreEqual(val.getMedian(out ind), 2);
            AreEqual(ind, 3);
        }
        [TestMethod] public void WithComp()
        {
            var val = new int[] {0, -2, 3, -1, 5, -4};
            int ind;
            AreEqual(val.getMedian(new FunctionComparer<int>(a => a.abs()), out ind), 3);
            AreEqual(ind, 2);
        }
    }
    [TestClass]
    public class MinTest
    {
        [TestMethod] public void Simple()
        {
            var val = new int[] {0, 1, 1, 2, 0, 1, 0, 0, 5, 0};
            AreEqual(val.getMin(), 0);
        }
        [TestMethod] public void WithIndexes()
        {
            var val = new int[] {0, 3, 1, 2, 4};
            int ind;
            AreEqual(val.getMin(out ind), 0);
            AreEqual(ind, 0);
        }
        [TestMethod] public void WithComp()
        {
            var val = new int[] {0, -2, 3, -1, 5, -4};
            int ind;
            AreEqual(val.getMin(new FunctionComparer<int>(a => a.abs()), out ind), 0);
            AreEqual(ind, 0);
        }
    }
    [TestClass]
    public class MaxTest
    {
        [TestMethod] public void Simple()
        {
            var val = new int[] {0, 1, 1, 2, 0, 1, 0, 0, 5, 0};
            AreEqual(val.getMax(), 5);
        }
        [TestMethod] public void WithIndexes()
        {
            var val = new int[] {0, 3, 1, 2, 4};
            int ind;
            AreEqual(val.getMax(out ind), 4);
            AreEqual(ind, 4);
        }
        [TestMethod] public void WithComp()
        {
            var val = new int[] {0, -2, 3, -1, 5, -4};
            int ind;
            AreEqual(val.getMax(new FunctionComparer<int>(a => a.abs()), out ind), 5);
            AreEqual(ind, 4);
        }
    }
    [TestClass]
    public class SumTest
    {
        [TestMethod]
        public void Simple()
        {
            var val = new int[] { 0, 1, 1, 2, 0, 1, 0, 0, 5, 0 };
            AreEqual(val.getSum(), 10);
        }
        [TestMethod]
        public void NoField()
        {
            var val = new int[] { 0, 1, 1, 2, 0, 1, 0, 0, 5, 0 };
            AreEqual(val.getSum((a,b) => a+b), 10);
        }
        [TestMethod]
        public void SpecialOp()
        {
            var val = new int[] { 0, 1, 1, 2, 0, 1, 0, 0, 5, 0 };
            AreEqual(val.getSum((a, b) => a^b), 1^2^5);
        }
        [TestMethod]
        public void OrOp()
        {
            var val = new int[] { 0, 1, 1, 2, 0, 1, 0, 0, 5, 0 };
            AreEqual(val.getSum((a, b) => a|b), 1|2|5);
        }
    }
    [TestClass]
    public class ProductTest
    {
        [TestMethod]
        public void Simple()
        {
            var val = new int[] {1, 1, 2, 1, 5 };
            AreEqual(val.getProduct(), 10);
        }
        [TestMethod]
        public void NoField()
        {
            var val = new int[] { 1, 1, 2, 1, 5 };
            AreEqual(val.getProduct((a, b) => a*b), 10);
        }
        [TestMethod]
        public void SpecialOp()
        {
            var val = new int[] { 1, 1, 2, 1, 5 };
            AreEqual(val.getProduct((a, b) => a & b), 0);
        }
        [TestMethod]
        public void OrOp()
        {
            var val = new int[] { 1, 1, 2, 1, 5 };
            AreEqual(val.getProduct((a, b) => a ^ b), 2 ^ 5);
        }
    }
}