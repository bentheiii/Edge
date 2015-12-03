using System;
using Edge.Arrays;
using Edge.Looping;
using Edge.NumbersMagic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class FillValuesTest
    {
        [TestMethod] public void Simple()
        {
            int[] val = new int[6];
            val.Fill(10);
            val.Do(a => AreEqual(a, 10));
        }
        [TestMethod] public void SimpleTwo()
        {
            int[] val = new int[6];
            val.Fill(10, 12);
            for (int i = 0; i < val.Length; i += 2)
            {
                AreEqual(val[i], 10);
                if (i + 1 < val.Length)
                    AreEqual(val[i + 1], 12);
            }
        }
        [TestMethod] public void SimpleTwoOdd()
        {
            int[] val = new int[7];
            val.Fill(10, 12);
            for (int i = 0; i < val.Length; i += 2)
            {
                AreEqual(val[i], 10);
                if (i + 1 < val.Length)
                    AreEqual(val[i + 1], 12);
            }
        }
        [TestMethod] public void Overflow()
        {
            int[] val = new int[7];
            val.Fill(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            for (int i = 0; i < val.Length; i ++)
                AreEqual(val[i], i);
        }
        [TestMethod] public void Empty()
        {
            int[] val = new int[0];
            val.Fill(10);
            val.Do(a => AreEqual(a, 10));
        }
        [TestMethod] public void EmptyTwo()
        {
            int[] val = new int[0];
            val.Fill(10, 12);
            for (int i = 0; i < val.Length; i += 2)
            {
                AreEqual(val[i], 10);
                if (i + 1 < val.Length)
                    AreEqual(val[i + 1], 12);
            }
        }
        [TestMethod, ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)] public void NoArg()
        {
            int[] val = new int[6];
            val.Fill();
            val.Do(a => AreEqual(a, 0));
        }
    }
    [TestClass]
    public class FillPartialValuesTest
    {
        [TestMethod] public void Simple()
        {
            int[] val = new int[6];
            val.Fill(new[] {10}, 3, 2);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(3, 3 + 2) ? 10 : 0);
        }
        [TestMethod, ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)] public void NoArg()
        {
            int[] val = new int[6];
            val.Fill(new int[0], 3, 2);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(3, 3 + 2) ? 10 : 0);
        }
        [TestMethod] public void NoLength()
        {
            int[] val = new int[6];
            val.Fill(new[] {10}, 3, 0);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(3, 3 + 0) ? 10 : 0);
        }
        [TestMethod] public void TooLong()
        {
            int[] val = new int[6];
            val.Fill(new[] {10}, 3, 100);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(3, 3 + 100) ? 10 : 0);
        }
        [TestMethod] public void TooFar()
        {
            int[] val = new int[6];
            val.Fill(new[] {10}, 10, 2);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(10, 10 + 2) ? 10 : 0);
        }
        [TestMethod] public void TooFarBehind()
        {
            int[] val = new int[6];
            val.Fill(new[] {10}, -3, 5);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(-3, -3 + 5) ? 10 : 0);
        }
    }
    [TestClass]
    public class FillPartialFuncTest
    {
        [TestMethod] public void Simple()
        {
            int[] val = new int[6];
            val.Fill(a => 10, 3, 2);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(3, 3 + 2) ? 10 : 0);
        }
        [TestMethod] public void NegLength()
        {
            int[] val = new int[6];
            val.Fill(a => 10, 3, -2);
            foreach (int t in val)
                AreEqual(t, 0);
        }
        [TestMethod] public void NoLength()
        {
            int[] val = new int[6];
            val.Fill(a => 10, 3, 0);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(3, 3 + 0) ? 10 : 0);
        }
        [TestMethod] public void TooLong()
        {
            int[] val = new int[6];
            val.Fill(a => 10, 3, 100);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(3, 3 + 100) ? 10 : 0);
        }
        [TestMethod] public void TooFar()
        {
            int[] val = new int[6];
            val.Fill(a => 10, 10, 2);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(10, 10 + 2) ? 10 : 0);
        }
        [TestMethod] public void TooFarBehind()
        {
            int[] val = new int[6];
            val.Fill(a => 10, -3, 5);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i.iswithinPartialExclusive(-3, -3 + 5) ? 10 : 0);
        }
    }
    [TestClass]
    public class FillFuncTest
    {
        [TestMethod] public void Simple()
        {
            int[] val = new int[6];
            val.Fill(a => 10);
            foreach (int t in val)
                AreEqual(t, 10);
        }
        [TestMethod] public void Empty()
        {
            int[] val = new int[0];
            val.Fill(a => 10);
            val.Do(a => AreEqual(a, 10));
        }
        [TestMethod] public void modulo()
        {
            int[] val = new int[10];
            val.Fill(a => a % 3);
            for (int i = 0; i < val.Length; i++)
                AreEqual(val[i], i % 3);
        }
    }
}