using System;
using System.Collections.Generic;
using System.Linq;
using Edge.Arrays;
using Edge.Comparison;
using Edge.Looping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static CoreTest.AssertComparerTest;

namespace CoreTest
{
    public static class AssertComparerTest
    {
        private static int Normalise(this int @this)
        {
            return @this < 0 ? -1 : (@this > 0 ? 1 : 0);
        }
        public static void IsWorking<T>(IComparer<T> comp, params T[] ordered)
        {
            ordered.CountBind().Join().Do(
                a =>
                {
                    AreEqual(comp.Compare(a.Item1.Item1, a.Item2.Item1).Normalise(), a.Item1.Item2.CompareTo(a.Item2.Item2).Normalise());
                });
        }
        public static void IsWorking<T>(IEqualityComparer<T> comp, params T[] ordered)
        {
            ordered.CountBind().Join().Do(
                a => { AreEqual(comp.Equals(a.Item1.Item1, a.Item2.Item1), a.Item1.Item2.Equals(a.Item2.Item2)); });
            AreEqual(ordered.Select(comp.GetHashCode).Duplicates().Count(), 0);
        }
    }
    [TestClass]
    public class IsworkingTest
    {
        [TestMethod] public void Simple()
        {
            IsWorking(Comparer<int>.Default, -1, 0, 1, 2);
        }
        [TestMethod]
        public void SimpleEqual()
        {
            IsWorking(EqualityComparer<int>.Default, -1, 0, 1, 2);
        }
    }
    [TestClass]
    public class DynamicComparerTest
    {
        [TestMethod] public void Simple()
        {
            var val = new DynamicComparer<double>();
            IsWorking(val, -100, -1, -0.5, 0, 0.0001, 0.1, 0.9, 1, 10, 15);
        }
    }
    [TestClass]
    public class FunctionComparerTest
    {
        [TestMethod] public void Simple()
        {
            var val = new FunctionComparer<int>((a, b) => (a % 3).CompareTo(b % 3));
            IsWorking(val, 12, 4, 2);
        }
        [TestMethod] public void Map()
        {
            var val = new FunctionComparer<int>(a => a % 3);
            IsWorking(val, 12, 4, 2);
        }
        [TestMethod] public void MapComp()
        {
            var val = new FunctionComparer<int>(a => a % 3, Comparer<int>.Default.Reverse().ToNonGeneric());
            IsWorking(val, 5, 31, -300);
        }
    }
    [TestClass]
    public class EqualityFunctionComparerTest
    {
        [TestMethod] public void Simple()
        {
            var val = new EqualityFunctionComparer<int>((a, b) => (a % 3) == (b % 3), a => a % 3);
            IsWorking(val, 12, 4, 2);
        }
        [TestMethod] public void Map()
        {
            var val = new EqualityFunctionComparer<int,int>(a => a % 3);
            IsWorking(val, 12, 4, 2);
        }
        [TestMethod] public void MapComp()
        {
            var val = new EqualityFunctionComparer<int,int>(a => a % 3, EqualityComparer<int>.Default);
            IsWorking(val, 5, 31, -300);
        }
    }
    [TestClass]
    public class PriorityComparerTest
    {
        [TestMethod] public void Simple()
        {
            var val = new PriorityComparer<int>(a => a % 3, a => a % 4);
            IsWorking(val, 12, 9, 6, 3, 4, 1, 10, 7, 8, 5, 2, 11);
        }
    }
    [TestClass]
    public class ReverseComparerTest
    {
        [TestMethod] public void Simple()
        {
            var val = Comparer<int>.Default.Reverse();
            IsWorking(val, 80,12,0,-1,-15);
        }
    }
    [TestClass]
    public class ComparertoEquatorTest
    {
        [TestMethod] public void Simple()
        {
            var val = Comparer<int>.Default.ToEqualityComparer();
            IsWorking(val, -100, -99, -85, -1, 0, 1, 25, 100, 136, 850);
        }
    }
    [TestClass]
    public class EnumerableComparerTest
    {
        [TestMethod] public void Simple()
        {
            var val = new EnumerableEqualityCompararer<int>();
            IsWorking(val, new int[] {0, 1, 2}, new int[] {1, 2, 0}, new int[] {10, 20, 30});
        }
        [TestMethod] public void SpecialComparer()
        {
            var val = new EnumerableEqualityCompararer<int>(new EqualityFunctionComparer<int,int>(a => a % 3));
            IsWorking(val, new int[] {0, 1, 2}, new int[] {1, 2, 0}, new int[] {10, 3, 30});
        }
    }
}