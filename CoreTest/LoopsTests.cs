using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CCDefault.Annotations;
using Edge.Arrays;
using Edge.Comparison;
using Edge.Complex;
using Edge.Guard;
using Edge.Looping;
using Edge.NumbersMagic;
using Edge.SystemExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class GenRangeTest
    {
        [TestMethod] public void SimpleMax()
        {
            IsTrue(Loops.Range((short)5).SequenceEqual(new short[] {0, 1, 2, 3, 4}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void NegMax()
        {
            IsTrue(Loops.Range((short)-1).SequenceEqual(new short[] {0, 1, 2, 3, 4}));
            Fail();
        }
        [TestMethod] public void SimpleStart()
        {
            IsTrue(Loops.Range((short)1, (short)5).SequenceEqual(new short[] {1, 2, 3, 4}));
        }
        [TestMethod] public void NegToPosStart()
        {
            IsTrue(Loops.Range((short)-1, (short)6).SequenceEqual(new short[] {-1, 0, 1, 2, 3, 4, 5}));
        }
        [TestMethod] public void NegToNegStart()
        {
            IsTrue(Loops.Range((short)-5, (short)-1).SequenceEqual(new short[] {-5, -4, -3, -2}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void RevStart()
        {
            IsTrue(Loops.Range((short)3, (short)-1).SequenceEqual(new short[] {0, 1, 2, 3, 4}));
            Fail();
        }
        [TestMethod] public void SimpleStep()
        {
            IsTrue(Loops.Range((short)1, (short)10, (short)2).SequenceEqual(new short[] {1, 3, 5, 7, 9}));
        }
        [TestMethod] public void NegStep()
        {
            IsTrue(Loops.Range((short)8, (short)1, (short)-1).SequenceEqual(new short[] {8, 7, 6, 5, 4, 3, 2}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void BadNegStep()
        {
            IsTrue(Loops.Range((short)0, (short)10, (short)-1).SequenceEqual(new short[] {8, 7, 6, 5, 4, 3, 2}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void BadPosStep()
        {
            IsTrue(Loops.Range((short)0, (short)-8, (short)1).SequenceEqual(new short[] {8, 7, 6, 5, 4, 3, 2}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void ZeroStep()
        {
            IsTrue(Loops.Range((short)0, (short)10, (short)0).SequenceEqual(new short[] {8, 7, 6, 5, 4, 3, 2}));
        }
    }
    [TestClass]
    public class DoubleRangeTest
    {
        [TestMethod] public void SimpleMax()
        {
            IsTrue(Loops.Range((double)5).SequenceEqual(new double[] {0, 1, 2, 3, 4}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void NegMax()
        {
            IsTrue(Loops.Range((double)-1).SequenceEqual(new double[] {0, 1, 2, 3, 4}));
            Fail();
        }
        [TestMethod] public void SimpleStart()
        {
            IsTrue(Loops.Range(1, (double)5).SequenceEqual(new double[] {1, 2, 3, 4}));
        }
        [TestMethod] public void NegToPosStart()
        {
            IsTrue(Loops.Range(-1, (double)6).SequenceEqual(new double[] {-1, 0, 1, 2, 3, 4, 5}));
        }
        [TestMethod] public void NegToNegStart()
        {
            IsTrue(Loops.Range(-5, (double)-1).SequenceEqual(new double[] {-5, -4, -3, -2}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void RevStart()
        {
            IsTrue(Loops.Range(3, (double)-1).SequenceEqual(new double[] {0, 1, 2, 3, 4}));
            Fail();
        }
        [TestMethod] public void SimpleStep()
        {
            IsTrue(Loops.Range(1, 10, (double)2).SequenceEqual(new double[] {1, 3, 5, 7, 9}));
        }
        [TestMethod] public void NegStep()
        {
            IsTrue(Loops.Range(8, 1, (double)-1).SequenceEqual(new double[] {8, 7, 6, 5, 4, 3, 2}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void BadNegStep()
        {
            IsTrue(Loops.Range(0, 10, (double)-1).SequenceEqual(new double[] {8, 7, 6, 5, 4, 3, 2}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void BadPosStep()
        {
            IsTrue(Loops.Range(0, -8, (double)1).SequenceEqual(new double[] {8, 7, 6, 5, 4, 3, 2}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void ZeroStep()
        {
            IsTrue(Loops.Range(0, 10, (double)0).SequenceEqual(new double[] {8, 7, 6, 5, 4, 3, 2}));
        }
    }
    [TestClass]
    public class UlongRangeTest
    {
        [TestMethod] public void SimpleMax()
        {
            IsTrue(Loops.Range((ulong)5).SequenceEqual(new ulong[] {0, 1, 2, 3, 4}));
        }
        [TestMethod] public void SimpleStart()
        {
            IsTrue(Loops.Range(1, (ulong)5).SequenceEqual(new ulong[] {1, 2, 3, 4}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void RevStart()
        {
            IsTrue(Loops.Range(3, (ulong)1).SequenceEqual(new ulong[] {0, 1, 2, 3, 4}));
            Fail();
        }
        [TestMethod] public void SimpleStep()
        {
            IsTrue(Loops.Range(1, 10, (ulong)2).SequenceEqual(new ulong[] {1, 3, 5, 7, 9}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void BadPosStep()
        {
            IsTrue(Loops.Range(8, 0, (ulong)1).SequenceEqual(new ulong[] {8, 7, 6, 5, 4, 3, 2}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void ZeroStep()
        {
            IsTrue(Loops.Range(0, 10, (ulong)0).SequenceEqual(new ulong[] {8, 7, 6, 5, 4, 3, 2}));
        }
    }
    [TestClass]
    public class IntRangeTest
    {
        [TestMethod] public void SimpleMax()
        {
            IsTrue(Loops.Range(5).SequenceEqual(new int[] {0, 1, 2, 3, 4}));
        }
        [TestMethod] public void SimpleStart()
        {
            IsTrue(Loops.Range(1, 5).SequenceEqual(new int[] {1, 2, 3, 4}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void RevStart()
        {
            IsTrue(Loops.Range(3, 1).SequenceEqual(new int[] {0, 1, 2, 3, 4}));
            Fail();
        }
        [TestMethod] public void SimpleStep()
        {
            IsTrue(Loops.Range(1, 10, 2).SequenceEqual(new int[] {1, 3, 5, 7, 9}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void BadPosStep()
        {
            IsTrue(Loops.Range(8, 0, 1).SequenceEqual(new int[] {8, 7, 6, 5, 4, 3, 2}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void ZeroStep()
        {
            IsTrue(Loops.Range(0, 10, 0).SequenceEqual(new int[] {8, 7, 6, 5, 4, 3, 2}));
        }
        [TestMethod]
        public void NegStep()
        {
            IsTrue(Loops.Range(8, 1, -1).SequenceEqual(new int[] { 8, 7, 6, 5, 4, 3, 2 }));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void BadNegStep()
        {
            IsTrue(Loops.Range(0, 10, -1).SequenceEqual(new int[] { 8, 7, 6, 5, 4, 3, 2 }));
        }
    }
    [TestClass]
    public class GenXRangeTest
    {
        [TestMethod] public void SimpleMax()
        {
            IsTrue(Loops.IRange((short)5).SequenceEqual(new short[] {0, 1, 2, 3, 4, 5}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void NegMax()
        {
            IsTrue(Loops.Range((short)-1).SequenceEqual(new short[] {0, 1, 2, 3, 4}));
            Fail();
        }
        [TestMethod] public void SimpleStart()
        {
            IsTrue(Loops.IRange((short)1, (short)5).SequenceEqual(new short[] {1, 2, 3, 4, 5}));
        }
        [TestMethod] public void NegToPosStart()
        {
            IsTrue(Loops.IRange((short)-1, (short)6).SequenceEqual(new short[] {-1, 0, 1, 2, 3, 4, 5, 6}));
        }
        [TestMethod] public void NegToNegStart()
        {
            IsTrue(Loops.IRange((short)-5, (short)-1).SequenceEqual(new short[] {-5, -4, -3, -2, -1}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void RevStart()
        {
            IsTrue(Loops.IRange((short)3, (short)-1).SequenceEqual(new short[] {0, 1, 2, 3, 4}));
            Fail();
        }
        [TestMethod] public void SimpleStep()
        {
            IsTrue(Loops.IRange((short)1, (short)11, (short)2).SequenceEqual(new short[] {1, 3, 5, 7, 9, 11}));
        }
        [TestMethod] public void NegStep()
        {
            IsTrue(Loops.IRange((short)8, (short)1, (short)-1).SequenceEqual(new short[] {8, 7, 6, 5, 4, 3, 2, 1}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void BadNegStep()
        {
            IsTrue(Loops.IRange((short)0, (short)10, (short)-1).SequenceEqual(new short[] {8, 7, 6, 5, 4, 3, 2, 1}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void BadPosStep()
        {
            IsTrue(Loops.IRange((short)0, (short)-8, (short)1).SequenceEqual(new short[] {8, 7, 6, 5, 4, 3, 2}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void ZeroStep()
        {
            IsTrue(Loops.IRange((short)0, (short)10, (short)0).SequenceEqual(new short[] {8, 7, 6, 5, 4, 3, 2}));
        }
    }
    [TestClass]
    public class DoubleXRangeTest
    {
        [TestMethod] public void SimpleMax()
        {
            IsTrue(Loops.IRange((double)5).SequenceEqual(new double[] {0, 1, 2, 3, 4, 5}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void NegMax()
        {
            IsTrue(Loops.IRange((double)-1).SequenceEqual(new double[] {0, 1, 2, 3, 4}));
            Fail();
        }
        [TestMethod] public void SimpleStart()
        {
            IsTrue(Loops.IRange(1, (double)5).SequenceEqual(new double[] {1, 2, 3, 4, 5}));
        }
        [TestMethod] public void NegToPosStart()
        {
            IsTrue(Loops.IRange(-1, (double)6).SequenceEqual(new double[] {-1, 0, 1, 2, 3, 4, 5, 6}));
        }
        [TestMethod] public void NegToNegStart()
        {
            IsTrue(Loops.IRange(-5, (double)-1).SequenceEqual(new double[] {-5, -4, -3, -2, -1}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void RevStart()
        {
            IsTrue(Loops.IRange(3, (double)-1).SequenceEqual(new double[] {0, 1, 2, 3, 4}));
            Fail();
        }
        [TestMethod] public void SimpleStep()
        {
            IsTrue(Loops.IRange(1, 11, (double)2).SequenceEqual(new double[] {1, 3, 5, 7, 9, 11}));
        }
        [TestMethod] public void NegStep()
        {
            IsTrue(Loops.IRange(8, 1, (double)-1).SequenceEqual(new double[] {8, 7, 6, 5, 4, 3, 2, 1}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void BadNegStep()
        {
            IsTrue(Loops.IRange(0, 10, (double)-1).SequenceEqual(new double[] {8, 7, 6, 5, 4, 3, 2}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void BadPosStep()
        {
            IsTrue(Loops.IRange(0, -8, (double)1).SequenceEqual(new double[] {8, 7, 6, 5, 4, 3, 2}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void ZeroStep()
        {
            IsTrue(Loops.IRange(0, 10, (double)0).SequenceEqual(new double[] {8, 7, 6, 5, 4, 3, 2}));
        }
    }
    [TestClass]
    public class UlongXRangeTest
    {
        [TestMethod] public void SimpleMax()
        {
            IsTrue(Loops.IRange((ulong)5).SequenceEqual(new ulong[] {0, 1, 2, 3, 4, 5}));
        }
        [TestMethod] public void SimpleStart()
        {
            IsTrue(Loops.IRange(1, (ulong)5).SequenceEqual(new ulong[] {1, 2, 3, 4, 5}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void RevStart()
        {
            IsTrue(Loops.IRange(3, (ulong)1).SequenceEqual(new ulong[] {0, 1, 2, 3, 4}));
            Fail();
        }
        [TestMethod] public void SimpleStep()
        {
            IsTrue(Loops.IRange(1, 11, (ulong)2).SequenceEqual(new ulong[] {1, 3, 5, 7, 9, 11}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void BadPosStep()
        {
            IsTrue(Loops.IRange(8, 0, (ulong)1).SequenceEqual(new ulong[] {8, 7, 6, 5, 4, 3, 2}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void ZeroStep()
        {
            IsTrue(Loops.IRange(0, 10, (ulong)0).SequenceEqual(new ulong[] {8, 7, 6, 5, 4, 3, 2}));
        }
    }
    [TestClass]
    public class IntXRangeTest
    {
        [TestMethod] public void SimpleMax()
        {
            IsTrue(Loops.IRange(5).SequenceEqual(new int[] {0, 1, 2, 3, 4, 5}));
        }
        [TestMethod] public void SimpleStart()
        {
            IsTrue(Loops.IRange(1, 5).SequenceEqual(new int[] {1, 2, 3, 4, 5}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void RevStart()
        {
            IsTrue(Loops.IRange(3, 1).SequenceEqual(new int[] {0, 1, 2, 3, 4, 5}));
            Fail();
        }
        [TestMethod] public void SimpleStep()
        {
            IsTrue(Loops.IRange(1, 11, 2).SequenceEqual(new int[] {1, 3, 5, 7, 9, 11}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void BadPosStep()
        {
            IsTrue(Loops.IRange(8, 0, 1).SequenceEqual(new int[] {8, 7, 6, 5, 4, 3, 2}));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void ZeroStep()
        {
            IsTrue(Loops.IRange(0, 10, 0).SequenceEqual(new int[] {8, 7, 6, 5, 4, 3, 2}));
        }
        [TestMethod]
        public void NegStep()
        {
            var val = Loops.IRange(8, 1, -1);
            IsTrue(val.SequenceEqual(new int[] { 8, 7, 6, 5, 4, 3, 2, 1 }));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void BadNegStep()
        {
            IsTrue(Loops.IRange(0, 10, -1).SequenceEqual(new int[] { 8, 7, 6, 5, 4, 3, 2, 0 }));
        }
    }
    [TestClass]
    public class RepeatTest
    {
        [TestMethod] public void Simple()
        {
            Guard<int> val = new Guard<int>(1);
            3.Repeat(() => val.value *= 2);
            AreEqual(val.value, 8);
        }
    }
    [TestClass]
    public class DetachTest
    {
        [TestMethod] public void SimpleOne()
        {
            var val = new Guard<double>();
            var n = 0;
            foreach (var t in Loops.Range(10).Attach(a => 1.0 / a).Detach(val))
            {
                AreEqual(t, n++);
                AreEqual(val.value, 1.0 / t);
            }
        }
        [TestMethod] public void SimpleTwo()
        {
            var val = new Guard<double>();
            var val2 = new Guard<int>();
            var n = 0;
            foreach (var t in Loops.Range(10).Attach(a => 1.0 / a).Attach((i, j) => i * i).Detach(val, val2))
            {
                AreEqual(t, n++);
                AreEqual(val.value, 1.0 / t);
                AreEqual(val2.value, t * t);
            }
        }
        [TestMethod] public void TwoTwo()
        {
            var val2 = new Guard<int>();
            var n = 0;
            foreach (var t in Loops.Range(10).Attach(a => 1.0 / a).Attach((i, j) => i * i).Detach(val2))
            {
                AreEqual(t.Item1, n++);
                AreEqual(val2.value, t.Item1 * t.Item1);
            }
        }
        [TestMethod] public void SimpleThree()
        {
            var val = new Guard<double>();
            var val2 = new Guard<int>();
            var val3 = new Guard<int>();
            var n = 0;
            foreach (var t in Loops.Range(10).Attach(a => 1.0 / a).Attach((i, j) => i * i).Attach((i, j, k) => -i).Detach(val, val2, val3))
            {
                AreEqual(t, n++);
                AreEqual(val.value, 1.0 / t);
                AreEqual(val2.value, t * t);
                AreEqual(val3.value, -t);
            }
        }
        [TestMethod] public void TwoThree()
        {
            var val2 = new Guard<int>();
            var val3 = new Guard<int>();
            var n = 0;
            foreach (var t in Loops.Range(10).Attach(a => 1.0 / a).Attach((i, j) => i * i).Attach((i, j, k) => -i).Detach(val2, val3))
            {
                AreEqual(t.Item1, n++);
                AreEqual(val2.value, t.Item1 * t.Item1);
                AreEqual(val3.value, -t.Item1);
            }
        }
        [TestMethod] public void ThreeThree()
        {
            var val3 = new Guard<int>();
            var n = 0;
            foreach (var t in Loops.Range(10).Attach(a => 1.0 / a).Attach((i, j) => i * i).Attach((i, j, k) => -i).Detach(val3))
            {
                AreEqual(t.Item1, n++);
                AreEqual(val3.value, -t.Item1);
            }
        }
    }
    [TestClass]
    public class AttachTest
    {
        [TestMethod] public void SimpleOne()
        {
            foreach (var i in Loops.Range(10).Attach(a => a * a))
                AreEqual(i.Item1 * i.Item1, i.Item2);
        }
        [TestMethod] public void SimpleTwo()
        {
            foreach (var i in Loops.Range(10).Attach(a => a * a).Attach((i, j) => i == 0 ? 0 : j / i))
            {
                AreEqual(i.Item1 * i.Item1, i.Item2);
                AreEqual(i.Item1, i.Item3);
            }
        }
        [TestMethod] public void SimpleThree()
        {
            foreach (var i in Loops.Range(10).Attach(a => a * a).Attach((i, j) => i == 0 ? 0 : j / i).Attach((i, j, k) => i * j * k))
            {
                AreEqual(i.Item1 * i.Item1, i.Item2);
                AreEqual(i.Item1, i.Item3);
                AreEqual(i.Item1.pow(4), i.Item4);
            }
        }
        [TestMethod] public void SimpleFour()
        {
            foreach (
                var i in
                    Loops.Range(10).Attach(a => a * a).Attach((i, j) => i == 0 ? 0 : j / i).Attach((i, j, k) => i * j * k).Attach(
                        (i, i1, arg3, arg4) => Math.Sqrt(i)))
            {
                AreEqual(i.Item1 * i.Item1, i.Item2);
                AreEqual(i.Item1, i.Item3);
                AreEqual(i.Item1.pow(4), i.Item4);
                AreEqual(i.Item5, Math.Sqrt(i.Item1));
            }
        }
    }
    [TestClass]
    public class ZipTest
    {
        [TestMethod] public void SimpleTwo()
        {
            foreach (var t in Loops.Range(10).Zip(Loops.Count(3)))
                AreEqual(t.Item1 + 3, t.Item2);
        }
        [TestMethod] public void SimpleThree()
        {
            foreach (var t in Loops.Range(10).Zip(Loops.Count(3), Loops.Range(3, 10)))
            {
                AreEqual(t.Item1 + 3, t.Item2);
                AreEqual(t.Item2, t.Item3);
            }
        }
        [TestMethod] public void SimpleFour()
        {
            foreach (var t in Loops.Range(10).Zip(Loops.Count(3), Loops.Range(3, 10), Loops.Range(10, 0, -1)))
            {
                AreEqual(t.Item1 + 3, t.Item2);
                AreEqual(t.Item2, t.Item3);
                AreEqual(t.Item1 + t.Item4, 10);
            }
        }
        [TestMethod] public void SimpleFive()
        {
            foreach (var t in Loops.Range(10).Zip(Loops.Count(3), Loops.Range(3, 10), Loops.Range(10, 0, -1), Loops.Count(0, 2)))
            {
                AreEqual(t.Item1 + 3, t.Item2);
                AreEqual(t.Item2, t.Item3);
                AreEqual(t.Item1 + t.Item4, 10);
                AreEqual(t.Item5, t.Item1 * 2);
            }
        }
    }
    [TestClass]
    public class ZipUnboundTest
    {
        [TestMethod] public void SimpleTwo()
        {
            IsTrue(
                Loops.Range(6).ZipUnbound(new int[] {3, 2, 6, 9}).SequenceEqual(new[]
                {Tuple.Create(0, 3), Tuple.Create(1, 2), Tuple.Create(2, 6), Tuple.Create(3, 9), Tuple.Create(4, 0), Tuple.Create(5, 0)}));
        }
        [TestMethod] public void SimpleThree()
        {
            IsTrue(
                Loops.Range(6).ZipUnbound(new int[] {3, 2, 6, 9}, new[] {9, 7, 1, 2, 6}).SequenceEqual(new[]
                {
                    Tuple.Create(0, 3, 9), Tuple.Create(1, 2, 7), Tuple.Create(2, 6, 1), Tuple.Create(3, 9, 2), Tuple.Create(4, 0, 6),
                    Tuple.Create(5, 0, 0)
                }));
        }
        [TestMethod] public void SimpleFour()
        {
            IsTrue(
                Loops.Range(6).ZipUnbound(new int[] {3, 2, 6, 9}, new[] {9, 7, 1, 2, 6}, new[] {1, 7, 9, 3}).SequenceEqual(new[]
                {
                    Tuple.Create(0, 3, 9, 1), Tuple.Create(1, 2, 7, 7), Tuple.Create(2, 6, 1, 9), Tuple.Create(3, 9, 2, 3), Tuple.Create(4, 0, 6, 0),
                    Tuple.Create(5, 0, 0, 0)
                }));
        }
    }
    [TestClass]
    public class MiscTest
    {
        [TestMethod] public void Enumerate()
        {
            IsTrue(3.Enumerate().SequenceEqual(new int[] {3}));
        }
        [TestMethod] public void Concat()
        {
            var val = new int[][] {new int[] {0, 1, 2, 3, 4}, new int[] {5, 6, 7}, new int[] {8, 9}, new int[] {10}}.Concat();
            IsTrue(val.SequenceEqual(Loops.Range(11)));
        }
        [TestMethod] public void Choose()
        {
            var val = new int[][] {new[] {1, 7, 8, 9}, new[] {2, 3, 4, 6}, new[] {5, 8, 11}, new[] {0, 5, 5}};
            var ch = val.Choose(a =>
            {
                int ret;
                a.getMin(out ret);
                return ret;
            }).ToArray();
            IsTrue(ch.SequenceEqual(new int[] {0, 1, 2, 3, 4, 5, 5, 5, 6, 7, 8, 8, 9, 11}));
        }
        [TestMethod] public void ChooseComp()
        {
            IsTrue(new[] {1, 7, 8, 9}.Choose(new[] {2, 3, 4, 6}, Comparer<int>.Default).SequenceEqual(new int[] {1, 2, 3, 4, 6, 7, 8, 9}));
        }
        [TestMethod] public void Switch()
        {
            var val = new int[][] {new[] {1, 7, 8, 9}, new[] {2, 3, 4, 6}, new[] {5, 8, 11}, new[] {0, 5, 5}};
            IsTrue(val.Switch().SequenceEqual(new int[] {1, 2, 5, 0, 7, 3, 8, 5, 8, 4, 11, 5, 9, 6}));
        }
        [TestMethod] public void SwitchUnBound()
        {
            var val = new int[][] {new[] {1, 7, 8, 9}, new[] {2, 3, 4, 6}, new[] {5, 8, 11}, new[] {0, 5, 5}};
            var v = val.SwitchUnbound().ToArray();
            IsTrue(v.SequenceEqual(new int[] {1, 2, 5, 0, 7, 3, 8, 5, 8, 4, 11, 5, 9, 6, 0, 0}));
        }
        [TestMethod] public void Cycle()
        {
            IsTrue(Loops.Range(3).Cycle().Take(8).SequenceEqual(new int[] {0, 1, 2, 0, 1, 2, 0, 1}));
        }
        [TestMethod] public void Where()
        {
            IsTrue(Loops.Range(3).Cycle().Take(8).Where(0, 2).SequenceEqual(new int[] {0, 2, 0, 2, 0}));
        }
        [TestMethod] public void Except()
        {
            IsTrue(Loops.Range(3).Cycle().Take(8).Except(1).SequenceEqual(new int[] {0, 2, 0, 2, 0}));
        }
        [TestMethod] public void PositionBind()
        {
            IsTrue(
                Loops.Range(4).PositionBind().Select(a => a.Item2).SequenceEqual(new Loops.Position[]
                {
                    Loops.Position.First | Loops.Position.Middle, Loops.Position.Middle, Loops.Position.Middle,
                    Loops.Position.Middle | Loops.Position.Last
                }));
        }
        [TestMethod] public void Match()
        {
            var val = Loops.Range(9).Match(new EqualityFunctionComparer<int>(a => a % 3));
            foreach (var i in val.CountBind())
            {
                IsTrue(i.Item1.Select(a => a % 3).All(a => a == i.Item2));
                IsTrue(i.Item1.Count() == 3);
            }
        }
        private enum Sample
        {
            f = 0,
            s = 1,
            t = 2,
            e = 8
        }
        [Flags]
        private enum SampleFlags
        {
            N = 0,
            f = 1,
            s = 2,
            t = 4,
            e = 256,
            fs = f | s,
            fst = f | s | t,
            et = e | t
        }
        [TestMethod] public void Enum()
        {
            IsTrue(Loops.Enum<Sample>().SequenceEqual(new Sample[] {Sample.f, Sample.s, Sample.t, Sample.e}));
        }
        [TestMethod] public void EnumFlags()
        {
            var val = Loops.EnumFlags<SampleFlags>().ToArray();
            IsTrue(val.SequenceEqual(new SampleFlags[] {SampleFlags.f, SampleFlags.s, SampleFlags.t, SampleFlags.e}));
        }
        [TestMethod] public void YieldAggregate()
        {
            IsTrue(Loops.YieldAggregate<Tuple<int,int>>(a=>Tuple.Create(a.Item2, a.Item1 + a.Item2), Tuple.Create(1,0)).Take(13).Select(a=>a.Item2).SequenceEqual(new int[] {0,1,1,2,3,5,8,13,21,34,55,89,144}));
        }
        [TestMethod]
        public void YieldAggregateTwo()
        {
            var val = new int[] {0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144};
            IsTrue(val.YieldAggregate((a,b)=>a+b,0).SequenceEqual(new int[] {0,1,2,4,7,12,20,33,54,88,143,232,376}));
        }
    }
    [TestClass]
    public class JoinTest
    {
        [TestMethod] public void SingleAll()
        {
            IsTrue(Loops.Range(3).Join().Select(a => a.Item1 * 10 + a.Item2).SequenceEqual(new int[] {0, 1, 2, 10, 11, 12, 20, 21, 22}));
        }
        [TestMethod] public void SingleNoS()
        {
            IsTrue(
                Loops.Range(3).Join(Loops.CartesianType.NoSymmatry).Select(a => a.Item1 * 10 + a.Item2).SequenceEqual(new int[]
                {00, 10, 11, 20, 21, 22}));
        }
        [TestMethod] public void SingleNoR()
        {
            IsTrue(
                Loops.Range(3).Join(Loops.CartesianType.NoReflexive).Select(a => a.Item1 * 10 + a.Item2).SequenceEqual(new int[]
                {1, 2, 10, 12, 20, 21}));
        }
        [TestMethod] public void SingleNoSoR()
        {
            IsTrue(
                Loops.Range(3).Join(Loops.CartesianType.NoReflexive | Loops.CartesianType.NoSymmatry).Select(a => a.Item1 * 10 + a.Item2)
                     .SequenceEqual(new int[] {10, 20, 21}));
        }
        [TestMethod] public void DoubleAll()
        {
            IsTrue(
                Loops.Range(3).Join(Loops.Range(1, 4)).Select(a => a.Item1 * 10 + a.Item2).SequenceEqual(new int[]
                {01, 02, 03, 11, 12, 13, 21, 22, 23}));
        }
        [TestMethod] public void TripleAll()
        {
            IsTrue(
                Loops.Range(3).Join(Loops.Range(1, 4), Loops.Range(2)).Select(a => a.Item1 * 100 + a.Item2 * 10 + a.Item3).SequenceEqual(new int[]
                {010, 011, 020, 021, 030, 031, 110, 111, 120, 121, 130, 131, 210, 211, 220, 221, 230, 231}));
        }
        [TestMethod] public void CartesSimple()
        {
            var val = Loops.Range(10).ToArray().Join(3).Select(a => NumberMagic.convertfrombase(a, 10));
            IsTrue(val.SequenceEqual(Loops.Range((long)1000)));
        }
    }
    [TestClass]
    public class GroupTest
    {
        [TestMethod] public void Simple2()
        {
            IsTrue(Loops.Range(8).Group2().SequenceEqual(Loops.Range(4).Select(a => Tuple.Create(2 * a, 2 * a + 1))));
        }
        [TestMethod] public void Simple3()
        {
            IsTrue(Loops.Range(8).Group3(8).SequenceEqual(Loops.Range(3).Select(a => Tuple.Create(3 * a, 3 * a + 1, 3 * a + 2))));
        }
        [TestMethod] public void Simple4()
        {
            IsTrue(Loops.Range(8).Group4().SequenceEqual(Loops.Range(2).Select(a => Tuple.Create(4 * a, 4 * a + 1, 4 * a + 2, 4 * a + 3))));
        }
        [TestMethod] public void Simple5()
        {
            IsTrue(
                Loops.Range(10).Group5(10).SequenceEqual(Loops.Range(2).Select(a => Tuple.Create(5 * a, 5 * a + 1, 5 * a + 2, 5 * a + 3, 5 * a + 4))));
        }
    }
    [TestClass]
    public class TrailTest
    {
        [TestMethod] public void Simple2()
        {
            IsTrue(Loops.Range(8).Trail2().SequenceEqual(Loops.Range(7).Select(a => Tuple.Create(a, a + 1))));
        }
        [TestMethod] public void Simple3()
        {
            IsTrue(Loops.Range(8).Trail3().SequenceEqual(Loops.Range(8 - 3 + 1).Select(a => Tuple.Create(a, a + 1, a + 2))));
        }
        [TestMethod] public void Simple4()
        {
            IsTrue(Loops.Range(8).Trail4().SequenceEqual(Loops.Range(8 - 4 + 1).Select(a => Tuple.Create(a, a + 1, a + 2, a + 3))));
        }
        [TestMethod] public void Simple5()
        {
            IsTrue(Loops.Range(8).Trail5().SequenceEqual(Loops.Range(8 - 5 + 1).Select(a => Tuple.Create(a, a + 1, a + 2, a + 3, a + 4))));
        }
        [TestMethod] public void Wrap2()
        {
            var val = Loops.Range(8).Trail2(true).ToArray();
            var val2 = Loops.Range(8).Select(a => Tuple.Create(a, (a + 1) % 8)).ToArray();
            IsTrue(Loops.Range(8).Trail2(true).SequenceEqual(Loops.Range(8).Select(a => Tuple.Create(a, (a + 1) % 8))));
        }
        [TestMethod] public void Wrap3()
        {
            IsTrue(Loops.Range(8).Trail3(true).SequenceEqual(Loops.Range(8).Select(a => Tuple.Create(a, (a + 1) % 8, (a + 2) % 8))));
        }
        [TestMethod] public void Wrap4()
        {
            IsTrue(Loops.Range(8).Trail4(true).SequenceEqual(Loops.Range(8).Select(a => Tuple.Create(a, (a + 1) % 8, (a + 2) % 8, (a + 3) % 8))));
        }
        [TestMethod] public void Wrap5()
        {
            IsTrue(
                Loops.Range(8).Trail5(true).SequenceEqual(
                    Loops.Range(8).Select(a => Tuple.Create(a, (a + 1) % 8, (a + 2) % 8, (a + 3) % 8, (a + 4) % 8))));
        }
    }
}