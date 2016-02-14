using System;
using System.Collections.Generic;
using System.Linq;
using Edge.Arrays;
using Edge.Comparison;
using Edge.Looping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class OccuranceTestNoComp
    {
        [TestMethod] public void Simple()
        {
            var val = new int[] {1, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            var expected = new Dictionary<int, ulong>() {{0, 1}, {1, 4}, {2, 3}, {3, 3}, {5, 2}, {8, 1}};
            foreach (KeyValuePair<int, ulong> keyValuePair in val.ToOccurances())
                AreEqual(expected[keyValuePair.Key], keyValuePair.Value);
        }
    }
    [TestClass]
    public class OccuranceTestComp
    {
        [TestMethod] public void Simple()
        {
            var val = new int[] {1, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            var expected = new Dictionary<int, ulong>() {{0, 4}, {1, 4}, {2, 6}};
            foreach (KeyValuePair<int, ulong> keyValuePair in val.ToOccurances(new EqualityFunctionComparer<int, int>(a => a % 3)))
                AreEqual(expected[keyValuePair.Key], keyValuePair.Value);
        }
    }
    [TestClass]
    public class UniquesNoComp
    {
        [TestMethod] public void Simple()
        {
            var val = new int[] {1, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            IsTrue(val.Uniques().OrderBy().SequenceEqual(new int[] {0, 8}));
        }
        [TestMethod] public void None()
        {
            var val = new int[] {1, 0, 8, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            IsTrue(val.Uniques().OrderBy().SequenceEqual(new int[] {}));
        }
        [TestMethod] public void Limit3()
        {
            var val = new int[] {1, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            IsTrue(val.Uniques(3).OrderBy().SequenceEqual(new int[] {0, 2, 3, 5, 8}));
        }
        [TestMethod] public void Limit0()
        {
            var val = new int[] {1, 0, 8, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            IsTrue(val.Uniques(0).OrderBy().SequenceEqual(new int[] {}));
        }
    }
    [TestClass]
    public class UniquesComp
    {
        private static readonly IEqualityComparer<int> _comp = new EqualityFunctionComparer<int, int>(a => a % 7);
        [TestMethod] public void Simple()
        {
            var val = new int[] {1, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            IsTrue(val.Uniques(_comp).OrderBy().SequenceEqual(new int[] {0}));
        }
        [TestMethod] public void None()
        {
            var val = new int[] {1, 0, 8, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            IsTrue(val.Uniques(_comp).OrderBy().SequenceEqual(new int[] {}));
        }
        [TestMethod] public void Limit3()
        {
            var val = new int[] {1, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            IsTrue(val.Uniques(_comp, 3).OrderBy().SequenceEqual(new int[] {0, 2, 3, 5}));
        }
        [TestMethod] public void Limit0()
        {
            var val = new int[] {1, 0, 8, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            IsTrue(val.Uniques(_comp, 0).OrderBy().SequenceEqual(new int[] {}));
        }
    }
    [TestClass]
    public class DuplicatesNoComp
    {
        [TestMethod] public void Simple()
        {
            var val = new int[] {1, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            IsTrue(val.Duplicates().OrderBy().SequenceEqual(new int[] {1, 2, 3, 5}));
        }
        [TestMethod] public void None()
        {
            var val = new int[] {1, 0, 8, 2};
            IsTrue(val.Duplicates().OrderBy().SequenceEqual(new int[] {}));
        }
        [TestMethod] public void Limit3()
        {
            var val = new int[] {1, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            IsTrue(val.Duplicates(3).OrderBy().SequenceEqual(new int[] {1, 2, 3}));
        }
        [TestMethod] public void Limit0()
        {
            var val = new int[] {1, 0, 8, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            IsTrue(val.Duplicates(0).OrderBy().SequenceEqual(new int[] {0, 1, 2, 3, 5, 8}));
        }
    }
    [TestClass]
    public class DuplicatesComp
    {
        private static readonly IEqualityComparer<int> _comp = new EqualityFunctionComparer<int, int>(a => a % 7);
        [TestMethod] public void Simple()
        {
            var val = new int[] {1, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            IsTrue(val.Duplicates(_comp).OrderBy().SequenceEqual(new int[] {1, 2, 3, 5}));
        }
        [TestMethod] public void None()
        {
            var val = new int[] {1, 0, 5, 2};
            IsTrue(val.Duplicates(_comp).OrderBy().SequenceEqual(new int[] {}));
        }
        [TestMethod] public void Limit3()
        {
            var val = new int[] {1, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            IsTrue(val.Duplicates(_comp, 3).OrderBy().SequenceEqual(new int[] {1, 2, 3}));
        }
        [TestMethod] public void Limit0()
        {
            var val = new int[] {1, 0, 8, 0, 1, 1, 2, 3, 5, 2, 2, 1, 3, 3, 8, 5};
            var s = val.Duplicates(_comp, 0);
            IsTrue(s.OrderBy().SequenceEqual(new int[] {0, 1, 2, 3, 5}));
        }
    }
    [TestClass]
    public class FromOccurance
    {
        [TestMethod] public void Simple()
        {
            var val = new Dictionary<int, ulong>() { { 0, 1 }, { 1, 4 }, { 2, 3 }, { 3, 3 }, { 5, 2 }, { 8, 1 } };
            var expected = new int[] {0, 1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 5, 5, 8};
            IsTrue(val.FromOccurances().OrderBy().SequenceEqual(expected.OrderBy()));
        }
        [TestMethod]
        public void ZeroArg()
        {
            var val = new Dictionary<int, ulong>() { { 0, 1 }, { 1, 4 }, { 2, 3 }, { 3, 3 }, { 5, 2 }, { 8, 1 }, {9,0} };
            var expected = new int[] { 0, 1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 5, 5, 8 };
            IsTrue(val.FromOccurances().OrderBy().SequenceEqual(expected.OrderBy()));
        }
        [TestMethod]
        public void Empty()
        {
            var val = new Dictionary<int, ulong>();
            var expected = new int[] {  };
            IsTrue(val.FromOccurances().OrderBy().SequenceEqual(expected.OrderBy()));
        }
    }
}