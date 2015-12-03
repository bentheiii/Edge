using System;
using Edge.Arrays.Arr2D;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class SparceAccessTest
    {
        [TestMethod] public void Simple()
        {
            var val = new SparseArray<int>(2);
            val[4, 3] = 5;
            AreEqual(val[4, 3], 5);
            AreEqual(val[1, 1], 0);
            AreEqual(val[100, 100], 0);
        }
        [TestMethod] public void Negativecoor()
        {
            var val = new SparseArray<int>(3);
            val[4, -1, 6] = 5;
            AreEqual(val[4, -1, 6], 5);
            AreEqual(val[4, 3, -7], 0);
        }
        [TestMethod] public void WierdDefault()
        {
            var val = new SparseArray<int>(2, 9);
            val[4, 3] = 5;
            AreEqual(val[4, 3], 5);
            AreEqual(val[1, 1], 9);
            AreEqual(val[100, 100], 9);
        }
        [ExpectedException(typeof(ArgumentException)), TestMethod] public void ZeroDems()
        {
            var val = new SparseArray<int>(0);
            Fail();
        }
    }
    [TestClass]
    public class SparceLengthsTest
    {
        [TestMethod] public void Simple()
        {
            var val = new SparseArray<int>(2);
            val[4, 3] = 5;
            AreEqual(val.GetLength(0), 5);
            AreEqual(val.GetLength(1), 4);
        }
        [TestMethod] public void Default()
        {
            var val = new SparseArray<int>(2);
            AreEqual(val.GetLength(0), 0);
            AreEqual(val.GetLength(1), 0);
        }
        [TestMethod] public void NegAccess()
        {
            var val = new SparseArray<int>(2);
            val[-7, -5] = 5;
            AreEqual(val.GetLength(0), 0);
            AreEqual(val.GetLength(1), 0);
        }
    }
}