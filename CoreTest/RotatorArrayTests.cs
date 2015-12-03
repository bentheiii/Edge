using System;
using Edge.Arrays.GroupSwitches;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class Access
    {
        [TestMethod] public void Simple()
        {
            RotatorArray<int> val = new RotatorArray<int>(1, 2, 3, 4, 5);
            AreEqual(val[3], 4);
        }
        [TestMethod] public void NegAccess()
        {
            RotatorArray<int> val = new RotatorArray<int>(1, 2, 3, 4, 5);
            AreEqual(val[3 - 5], 4);
        }
        [TestMethod] public void Overflow()
        {
            RotatorArray<int> val = new RotatorArray<int>(1, 2, 3, 4, 5);
            AreEqual(val[3 + 5], 4);
        }
        [TestMethod] public void LengthConstructor()
        {
            RotatorArray<int> val = new RotatorArray<int>(5);
            val[6] = 3;
            AreEqual(val[6],3);
            AreEqual(val[0],0);
        }
        [TestMethod, ExpectedException(typeof(Exception), AllowDerivedTypes = true)]  public void ZeroLength()
        {
            RotatorArray<int> val = new RotatorArray<int>(0);
            Fail();
        }
    }
    [TestClass]
    public class Rotation
    {
        [TestMethod] public void Simple()
        {
            RotatorArray<int> val = new RotatorArray<int>(1, 2, 3, 4, 5);
            val.Rotate(3);
            AreEqual(val[3], 2);
        }
        [TestMethod] public void NegRotate()
        {
            RotatorArray<int> val = new RotatorArray<int>(1, 2, 3, 4, 5);
            val.Rotate(-2);
            AreEqual(val[3], 2);
        }
        [TestMethod] public void NegOverRotate()
        {
            RotatorArray<int> val = new RotatorArray<int>(1, 2, 3, 4, 5);
            val.Rotate(-102);
            AreEqual(val[3], 2);
        }
        [TestMethod] public void OverRotate()
        {
            RotatorArray<int> val = new RotatorArray<int>(1, 2, 3, 4, 5);
            val.Rotate(10003);
            AreEqual(val[3], 2);
        }
        [TestMethod] public void ZeroRotate()
        {
            RotatorArray<int> val = new RotatorArray<int>(1, 2, 3, 4, 5);
            val.Rotate(0);
            AreEqual(val[3], 4);
        }
    }
    [TestClass]
    public class Increment
    {
        [TestMethod] public void Simple()
        {
            RotatorArray<int> val = new RotatorArray<int>(1, 2, 3, 4, 5);
            val++;
            AreEqual(val[3], 5);
        }
        [TestMethod] public void NegRotate()
        {
            RotatorArray<int> val = new RotatorArray<int>(1, 2, 3, 4, 5);
            val--;
            AreEqual(val[3], 3);
        }
    }
}