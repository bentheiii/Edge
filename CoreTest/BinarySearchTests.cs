using System.Collections.Generic;
using System.Linq;
using Edge.Arrays;
using Edge.NumbersMagic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class AggregateDefTest
    {
        [TestMethod] public void Simple()
        {
            IDictionary<char, int> d = new Dictionary<char, int>();
            d['a'] = 3;
            d['b'] = 2;
            IsTrue(d.AggregateDefinition('b', 4, (i, i1) => i * i1));
            AreEqual(d['a'], 3);
            AreEqual(d['b'], 2 * 4);
            IsTrue(d.AggregateDefinition('b', 4, (i, i1) => i + i1));
            AreEqual(d['a'], 3);
            AreEqual(d['b'], (2 * 4) + 4);
            IsTrue(d.SumDefinition('b', 3));
            AreEqual(d['a'], 3);
            AreEqual(d['b'], (2 * 4) + 4 + 3);
            IsTrue(d.ProductDefinition('b', 3));
            AreEqual(d['a'], 3);
            AreEqual(d['b'], ((2 * 4) + 4 + 3) * 3);
        }
        [TestMethod] public void MissingDefiniton()
        {
            IDictionary<char, int> d = new Dictionary<char, int>();
            d['a'] = 3;
            IsFalse(d.AggregateDefinition('b', 4, (i, i1) => i * i1));
            AreEqual(d['a'], 3);
            AreEqual(d['b'], 4);
            IsTrue(d.SumDefinition('b', 3));
            AreEqual(d['a'], 3);
            AreEqual(d['b'], 4 + 3);
        }
        [TestMethod] public void Fields()
        {
            IDictionary<char, string> d = new Dictionary<char, string>();
            d['b'] = "ab";
            IsTrue(d.SumDefinition('b', "cd"));
            AreEqual(d['b'], "cdab");
        }
        [TestMethod] public void EnsureDefiniton()
        {
            IDictionary<char, string> d = new Dictionary<char, string>();
            d['b'] = "ab";
            IsTrue(d.EnsureDefinition('b', "cd"));
            IsFalse(d.EnsureDefinition('a', "dc"));
            AreEqual(d['b'], "ab");
            AreEqual(d['a'], "dc");
        }
    }
    [TestClass]
    public class BinarySearchUnboundBiDirectionalTest
    {
        [TestMethod] public void Simple()
        {
            AreEqual(arrayExtensions.binSearch(a => 12.CompareTo(a), 10, 19), 12);
        }
        [TestMethod] public void AtMin()
        {
            AreEqual(arrayExtensions.binSearch(a => 12.CompareTo(a), 12, 19), 12);
        }
        [TestMethod] public void AtMax()
        {
            AreEqual(arrayExtensions.binSearch(a => 12.CompareTo(a), 10, 13), 12);
        }
        [TestMethod] public void MinAndMax()
        {
            AreEqual(arrayExtensions.binSearch(a => 12.CompareTo(a), 12, 13), 12);
        }
        [TestMethod] public void BelowTarget()
        {
            AreEqual(arrayExtensions.binSearch(a => 12.CompareTo(a), 10, 12), -1);
        }
        [TestMethod] public void OnTarget()
        {
            AreEqual(arrayExtensions.binSearch(a => 12.CompareTo(a), 12, 12), -1);
        }
        [TestMethod] public void AboveTarget()
        {
            AreEqual(arrayExtensions.binSearch(a => 12.CompareTo(a), 15, 19), -1);
        }
        [TestMethod] public void BelowNear()
        {
            AreEqual(arrayExtensions.binSearch(a => 12.CompareTo(a), 10, 11), -1);
        }
        [TestMethod] public void AlwaysUp()
        {
            AreEqual(arrayExtensions.binSearch(a => 1, 10, 20), -1);
        }
        [TestMethod] public void AlwaysDown()
        {
            AreEqual(arrayExtensions.binSearch(a => -1, 10, 20), -1);
        }
    }
    [TestClass]
    public class BinarySearchUnboundUniDirectionalTest
    {
        [TestMethod] public void Simple()
        {
            AreEqual(arrayExtensions.binSearch(a => a <= 12, 10, 19), 12);
        }
        [TestMethod] public void AtMin()
        {
            AreEqual(arrayExtensions.binSearch(a => a <= 12, 12, 19), 12);
        }
        [TestMethod] public void AtMax()
        {
            AreEqual(arrayExtensions.binSearch(a => a <= 12, 10, 13), 12);
        }
        [TestMethod] public void MinAndMax()
        {
            AreEqual(arrayExtensions.binSearch(a => a <= 12, 12, 13), 12);
        }
        [TestMethod] public void BelowTarget()
        {
            AreEqual(arrayExtensions.binSearch(a => a <= 12, 10, 12), 11);
        }
        [TestMethod] public void OnTarget()
        {
            AreEqual(arrayExtensions.binSearch(a => a <= 12, 12, 12), -1);
        }
        [TestMethod] public void AboveTarget()
        {
            AreEqual(arrayExtensions.binSearch(a => a <= 12, 15, 19), -1);
        }
        [TestMethod] public void BelowNear()
        {
            AreEqual(arrayExtensions.binSearch(a => a <= 12, 10, 11), 10);
        }
        [TestMethod] public void ConstFalse()
        {
            AreEqual(arrayExtensions.binSearch(a => false, 10, 20), -1);
        }
        [TestMethod] public void ConstTrue()
        {
            AreEqual(arrayExtensions.binSearch(a => true, 18, 20), 19);
        }
    }
    [TestClass]
    public class BinarySearchBoundUniDirectionalTest
    {
        private static int[] _primes;
        private const int ARRSIZE = 20;
        [ClassInitialize] public static void ClassInit(TestContext context)
        {
            _primes = NumberMagic.listprimes().Take(ARRSIZE).ToArray();
        }
        [TestMethod] public void Simple()
        {
            AreEqual(_primes.binSearch(a => a <= 7), 3);
        }
        [TestMethod] public void Even()
        {
            AreEqual(_primes.binSearch(a => a % 2 == 0), 0);
        }
        [TestMethod] public void Last()
        {
            AreEqual(_primes.binSearch(a => true), ARRSIZE - 1);
        }
    }
    [TestClass]
    public class BinarySearchBoundBiDirectionalTest
    {
        private static int[] _primes;
        private const int ARRSIZE = 20;
        [ClassInitialize] public static void ClassInit(TestContext context)
        {
            _primes = NumberMagic.listprimes().Take(ARRSIZE).ToArray(ARRSIZE);
        }
        [TestMethod] public void Simple()
        {
            AreEqual(_primes.binSearch(a => 7.CompareTo(a)), 3);
        }
        [TestMethod] public void Even()
        {
            AreEqual(_primes.binSearch(a => a % 2 == 0 ? 0 : -1), 0);
        }
        [TestMethod] public void Missing()
        {
            AreEqual(_primes.binSearch(a => 1), -1);
        }
        [TestMethod] public void MissingCenter()
        {
            AreEqual(_primes.binSearch(a => 4.CompareTo(a)), -1);
        }
    }
    [TestClass]
    public class BinarySearchBoundSearcherTest
    {
        private static int[] _primes;
        private const int ARRSIZE = 100;
        [ClassInitialize] public static void ClassInit(TestContext context)
        {
            _primes = NumberMagic.listprimes().Take(ARRSIZE).ToArray();
        }
        [TestMethod] public void Simple()
        {
            AreEqual(_primes.binSearch(7), 3);
        }
        [TestMethod] public void Even()
        {
            AreEqual(_primes.binSearch(2), 0);
        }
        [TestMethod] public void Missing()
        {
            AreEqual(_primes.binSearch(4), -1);
        }
        [TestMethod] public void MissingOutOfRange()
        {
            AreEqual(_primes.binSearch(1001), -1);
        }
    }
}