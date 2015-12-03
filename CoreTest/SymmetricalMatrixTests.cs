using System;
using Edge.Arrays.Arr2D;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class SymmetricalReflexiveAccessTest
    {
        [TestMethod] public void Simple()
        {
            var val = new SymmetricMatrix<int>(10);
            val[2, 1] = 3;
            AreEqual(val[2, 1], 3);
            AreEqual(val[1, 2], 3);
            AreEqual(val[4, 4], 0);
        }
        [TestMethod] public void Reflex()
        {
            var val = new SymmetricMatrix<int>(10);
            val[2, 2] = 3;
            AreEqual(val[2, 2], 3);
            AreEqual(val[4, 4], 0);
        }
        [TestMethod, ExpectedException(typeof(IndexOutOfRangeException))] public void Neg()
        {
            var val = new SymmetricMatrix<int>(10);
            val[-1, 3] = 3;
            Fail();
        }
        [TestMethod, ExpectedException(typeof(IndexOutOfRangeException))] public void OutOfRange()
        {
            var val = new SymmetricMatrix<int>(10);
            val[11, 3] = 3;
            Fail();
        }
    }
    [TestClass]
    public class SymmetricalNonReflexiveAccessTest
    {
        [TestMethod] public void Simple()
        {
            var val = new SymmetricMatrix<int>(10, false);
            val[2, 1] = 3;
            AreEqual(val[2, 1], 3);
            AreEqual(val[1, 2], 3);
        }
        [TestMethod, ExpectedException(typeof(IndexOutOfRangeException))] public void Reflex()
        {
            var val = new SymmetricMatrix<int>(10, false);
            val[2, 2] = 3;
            Fail();
        }
        [TestMethod, ExpectedException(typeof(IndexOutOfRangeException))] public void Neg()
        {
            var val = new SymmetricMatrix<int>(10, false);
            val[-1, 3] = 3;
            Fail();
        }
        [TestMethod, ExpectedException(typeof(IndexOutOfRangeException))] public void OutOfRange()
        {
            var val = new SymmetricMatrix<int>(10, false);
            val[11, 3] = 3;
            Fail();
        }
    }
}