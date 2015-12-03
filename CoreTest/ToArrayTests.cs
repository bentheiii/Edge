using System;
using System.Linq;
using Edge.Arrays;
using Edge.Guard;
using Edge.Looping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class SelectToArray
    {
        [TestMethod] public void Simple()
        {
            var val = Loops.Range(10).SelectToArray(a => -a);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], -i);
            AreEqual(val.Length, 10);
        }
        [TestMethod] public void Empty()
        {
            var val = Enumerable.Empty<int>().SelectToArray(a => -a);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], -i);
            AreEqual(val.Length, 0);
        }
        [TestMethod, ExpectedException(typeof(OutOfMemoryException), AllowDerivedTypes = true)] public void Infinite()
        {
            if (!Init.AllowLongTests)
                throw new OutOfMemoryException();
            var val = Loops.Infinite().SelectToArray(a => a.HasFlag(Loops.Position.First));
            Fail();
        }
        [TestMethod] public void EnsurePreCalculation()
        {
            EventGuard<int> g = new EventGuard<int>(0);
            var val = Loops.Range(10).SelectToArray(a =>
            {
                g.value++;
                return -a;
            });
            AreEqual(g.value, 10);
            AreEqual(val.Length, 10);
        }
    }
    [TestClass]
    public class LengthToArray
    {
        [TestMethod] public void Simple()
        {
            const int length = 10;
            var val = Loops.Range(length).ToArray(length);
            for (int i = 0; i < length; i++)
                AreEqual(val[i], i);
            AreEqual(val.Length, length);
        }
        [TestMethod] public void TooLong()
        {
            const int length = 10;
            var val = Loops.Range(length).ToArray(length + 6);
            for (int i = 0; i < length; i++)
                AreEqual(val[i], i);
            AreEqual(val.Length, length);
        }
        [TestMethod] public void TooShort()
        {
            const int length = 10;
            var val = Loops.Range(length).ToArray(length - 6);
            for (int i = 0; i < length; i++)
                AreEqual(val[i], i);
            AreEqual(val.Length, length);
        }
        [TestMethod] public void Empty()
        {
            const int length = 10;
            var val = Enumerable.Empty<int>().ToArray(length);
            AreEqual(val.Length, 0);
        }
        [TestMethod] public void NegLength()
        {
            const int length = 10;
            var val = Loops.Range(length).ToArray(-6);
            for (int i = 0; i < length; i++)
                AreEqual(val[i], i);
            AreEqual(val.Length, length);
        }
        [TestMethod] public void ZeroLength()
        {
            const int length = 10;
            var val = Loops.Range(length).ToArray(0);
            for (int i = 0; i < length; i++)
                AreEqual(val[i], i);
            AreEqual(val.Length, length);
        }
    }
    [TestClass]
    public class ReporterToArray
    {
        [TestMethod] public void Simple()
        {
            const int length = 10;
            var g = new EventGuard<int>(0);
            var val = Loops.Range(length).ToArray((i, i1) => g.value++);
            for (int i = 0; i < length; i++)
                AreEqual(val[i], i);
            AreEqual(val.Length, length);
            AreEqual(g.value,length);
        }
        [TestMethod] public void EnsurePreCalculation()
        {
            const int length = 5;
            var g = new EventGuard<int>(0);
            var g2 = new EventGuard<int>(0);
            var val = Loops.Range(length).Select(a => { g2.value = a; return a; }).ToArray((i, i1) => g.value+=g2.value);
            for (int i = 0; i < length; i++)
                AreEqual(val[i], i);
            AreEqual(val.Length, length);
            AreEqual(g.value, (length*(length-1))/2);
        }
    }
}