using System;
using Edge.Looping;
using Edge.NumbersMagic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static Edge.Arrays.ArrayExtensions;

namespace CoreTest
{
    [TestClass]
    public class LengthFillValuesTest
    {
        [TestMethod] public void Simple()
        {
            var val = Fill(6, 10);
            val.Do(a => AreEqual(a, 10));
            AreEqual(val.Length, 6);
        }
        [TestMethod] public void SimpleTwo()
        {
            var val = Fill(6, 10, 12);
            for (int i = 0; i < val.Length; i += 2)
            {
                AreEqual(val[i], 10);
                if (i + 1 < val.Length)
                    AreEqual(val[i + 1], 12);
            }
            AreEqual(val.Length, 6);
        }
        [TestMethod] public void SimpleTwoOdd()
        {
            var val = Fill(7, 10, 12);
            for (int i = 0; i < val.Length; i += 2)
            {
                AreEqual(val[i], 10);
                if (i + 1 < val.Length)
                    AreEqual(val[i + 1], 12);
            }
            AreEqual(val.Length, 7);
        }
        [TestMethod] public void Overflow()
        {
            var val = Fill(7, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i);
            AreEqual(val.Length, 7);
        }
        [TestMethod] public void Empty()
        {
            var val = Fill(0, 10);
            val.Do(a => AreEqual(a, 10));
            AreEqual(val.Length, 0);
        }
        [TestMethod] public void EmptyTwo()
        {
            var val = Fill(0, 10, 12);
            for (int i = 0; i < val.Length; i += 2)
            {
                AreEqual(val[i], 10);
                if (i + 1 < val.Length)
                    AreEqual(val[i + 1], 12);
            }
            AreEqual(val.Length, 0);
        }
        [TestMethod, ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)] public void NoArg()
        {
            var val = Fill(6, new int[] {});
            val.Do(a => AreEqual(a, 0));
            AreEqual(val.Length, 0);
        }
    }
    [TestClass]
    public class LengthFillPartialValuesTest
    {
        [TestMethod] public void Simple()
        {
            var val = Fill(6, new[] {10}, 3, 2);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(3, 3 + 2) ? 10 : 0);
            AreEqual(val.Length, 6);
        }
        [TestMethod, ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)] public void NoArg()
        {
            var val = Fill(6, new int[0], 3, 2);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(3, 3 + 2) ? 10 : 0);
            AreEqual(val.Length, 6);
        }
        [TestMethod] public void NoLength()
        {
            var val = Fill(6, new[] {10}, 3, 0);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(3, 3 + 0) ? 10 : 0);
            AreEqual(val.Length, 6);
        }
        [TestMethod] public void TooLong()
        {
            var val = Fill(6, new[] {10}, 3, 100);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(3, 3 + 100) ? 10 : 0);
            AreEqual(val.Length, 6);
        }
        [TestMethod] public void TooFar()
        {
            var val = Fill(6, new[] {10}, 10, 2);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(10, 10 + 2) ? 10 : 0);
            AreEqual(val.Length, 6);
        }
        [TestMethod] public void TooFarBehind()
        {
            var val = Fill(6, new[] {10}, -3, 5);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(-3, -3 + 5) ? 10 : 0);
            AreEqual(val.Length, 6);
        }
    }
    [TestClass]
    public class LengthFillPartialFuncTest
    {
        [TestMethod] public void Simple()
        {
            var val = Fill(6, a => 10, 3, 2);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(3, 3 + 2) ? 10 : 0);
            AreEqual(val.Length, 6);
        }
        [TestMethod] public void NegLength()
        {
            var val = Fill(6, a => 10, 3, -2);
            foreach (int t in val)
                AreEqual(t, 0);
            AreEqual(val.Length, 6);
        }
        [TestMethod] public void NoLength()
        {
            var val = Fill(6, a => 10, 3, 0);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(3, 3 + 0) ? 10 : 0);
            AreEqual(val.Length, 6);
        }
        [TestMethod] public void TooLong()
        {
            var val = Fill(6, a => 10, 3, 100);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(3, 3 + 100) ? 10 : 0);
            AreEqual(val.Length, 6);
        }
        [TestMethod] public void TooFar()
        {
            var val = Fill(6, a => 10, 10, 2);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(10, 10 + 2) ? 10 : 0);
            AreEqual(val.Length, 6);
        }
        [TestMethod] public void TooFarBehind()
        {
            var val = Fill(6, a => 10, -3, 5);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(-3, -3 + 5) ? 10 : 0);
            AreEqual(val.Length, 6);
        }
    }
    [TestClass]
    public class LengthFillFuncTest
    {
        [TestMethod] public void Simple()
        {
            var val = Fill(6, a => 10);
            foreach (int t in val)
                AreEqual(t, 10);
            AreEqual(val.Length, 6);
        }
        [TestMethod] public void Empty()
        {
            var val = Fill(6, a => 10);
            val.Do(a => AreEqual(a, 10));
            AreEqual(val.Length, 6);
        }
        [TestMethod] public void modulo()
        {
            var val = Fill(10, a => a % 3);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i % 3);
            AreEqual(val.Length, 10);
        }
    }
}