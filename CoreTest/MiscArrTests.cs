﻿using System;
using System.Collections.Generic;
using Edge.Arrays;
using Edge.Comparison;
using Edge.SystemExtensions;
using Edge.Tuples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class SymmetryTestNoComp
    {
        [TestMethod] public void TrueOdd()
        {
            var val = new[] {0, 1, 2, 1, 0};
            IsTrue(val.isSymmetrical());
            IsTrue(((IEnumerable<int>)val).isSymmetrical());
        }
        [TestMethod] public void TrueEven()
        {
            var val = new[] {0, 1, 1, 0};
            IsTrue(val.isSymmetrical());
            IsTrue(((IEnumerable<int>)val).isSymmetrical());
        }
        [TestMethod] public void FalseOddFar()
        {
            var val = new[] {0, 1, 2, 1, 1};
            IsFalse(val.isSymmetrical());
            IsFalse(((IEnumerable<int>)val).isSymmetrical());
        }
        [TestMethod] public void FalseOddNear()
        {
            var val = new[] {0, 1, 2, 0, 0};
            IsFalse(val.isSymmetrical());
            IsFalse(((IEnumerable<int>)val).isSymmetrical());
        }
        [TestMethod] public void FalseEvenFar()
        {
            var val = new[] {0, 1, 1, 1};
            IsFalse(val.isSymmetrical());
            IsFalse(((IEnumerable<int>)val).isSymmetrical());
        }
        [TestMethod] public void FalseEvenNear()
        {
            var val = new[] {0, 1, 0, 0};
            IsFalse(val.isSymmetrical());
            IsFalse(((IEnumerable<int>)val).isSymmetrical());
        }
    }
    [TestClass]
    public class SymmetryTestComp
    {
        private static readonly IEqualityComparer<int> _comp = new EqualityFunctionComparer<int,int>(a => a % 3);
        [TestMethod] public void TrueOdd()
        {
            var val = new[] {0, 1, 2, 4, 3};
            IsTrue(val.isSymmetrical(_comp));
            IsTrue(((IEnumerable<int>)val).isSymmetrical(_comp));
        }
        [TestMethod] public void TrueEven()
        {
            var val = new[] {0, 1, 4, 3};
            IsTrue(val.isSymmetrical(_comp));
            IsTrue(((IEnumerable<int>)val).isSymmetrical(_comp));
        }
        [TestMethod] public void FalseOddFar()
        {
            var val = new[] {0, 1, 2, 4, 7};
            IsFalse(val.isSymmetrical(_comp));
            IsFalse(((IEnumerable<int>)val).isSymmetrical(_comp));
        }
        [TestMethod] public void FalseOddNear()
        {
            var val = new[] {0, 1, 2, 3, 6};
            IsFalse(val.isSymmetrical(_comp));
            IsFalse(((IEnumerable<int>)val).isSymmetrical(_comp));
        }
        [TestMethod] public void FalseEvenFar()
        {
            var val = new[] {0, 1, 4, 7};
            IsFalse(val.isSymmetrical(_comp));
            IsFalse(((IEnumerable<int>)val).isSymmetrical(_comp));
        }
        [TestMethod] public void FalseEvenNear()
        {
            var val = new[] {0, 1, 3, 6};
            IsFalse(val.isSymmetrical(_comp));
            IsFalse(((IEnumerable<int>)val).isSymmetrical(_comp));
        }
    }
    [TestClass]
    public class ContainsAllTest
    {
        [TestMethod] public void Simple()
        {
            IsTrue((new[] {0, 1, 2, 3}.ContainsAll(new[] {0, 2})));
        }
        [TestMethod] public void SimpleFalse()
        {
            IsFalse((new[] {0, 1, 2, 3}.ContainsAll(new[] {0, 2, 5})));
        }
        [TestMethod] public void EmptyArg()
        {
            IsTrue((new[] {0, 1, 2, 3}.ContainsAll(new int[] {})));
        }
        [TestMethod] public void Empty()
        {
            IsFalse((new int[] {}.ContainsAll(new[] {0, 1, 2})));
        }
        [TestMethod] public void EmptyBoth()
        {
            IsTrue((new int[] {}.ContainsAll(new int[] {})));
        }
    }
    [TestClass]
    public class ContainsAnyTest
    {
        [TestMethod] public void Simple()
        {
            IsTrue((new[] {0, 1, 2, 3}.ContainsAny(new[] {0, 2})));
        }
        [TestMethod] public void SimpleFalse()
        {
            IsFalse((new[] {0, 1, 2, 3}.ContainsAny(new[] {5})));
        }
        [TestMethod] public void EmptyArg()
        {
            IsFalse((new[] {0, 1, 2, 3}.ContainsAny(new int[] {})));
        }
        [TestMethod] public void Empty()
        {
            IsFalse((new int[] {}.ContainsAny(new[] {0, 1, 2})));
        }
        [TestMethod] public void EmptyBoth()
        {
            IsFalse((new int[] {}.ContainsAny(new int[] {})));
        }
    }
    [TestClass]
    public class ToPrintableTest
    {
        [TestMethod] public void Simple()
        {
            AreEqual((new[] {0, 1, 2}).ToPrintable(","), "[0,1,2]");
        }
        [TestMethod] public void Empty()
        {
            AreEqual((new int[] {}).ToPrintable(), "[]");
        }
        [TestMethod] public void Nill()
        {
            AreEqual((new int[] {}).ToPrintable("", "", ""), "");
        }
    }
    [TestClass]
    public class ToPrintableDictionaryTest
    {
        [TestMethod]
        public void Simple()
        {
            AreEqual((new Dictionary<string,int>()  { ["0"]=0, ["1"]=1, ["2"]=2 }).ToPrintable(seperator:","), "{0:0,1:1,2:2}");
        }
        [TestMethod]
        public void Empty()
        {
            AreEqual((new Dictionary<string, int>()).ToPrintable(), "{}");
        }
        [TestMethod]
        public void Nill()
        {
            AreEqual((new Dictionary<string, int>()).ToPrintable("", "", "",""), "");
        }
    }
    [TestClass]
    public class AverageTest
    {
        [TestMethod] public void Simple()
        {
            var val = new[] {0, 1, 1, 2, 0, 1, 0, 0, 5, 0};
            AreEqual(val.getAverage(), 1);
        }
    }
    [TestClass]
    public class GeomMetricAverageTest
    {
        [TestMethod] public void Simple()
        {
            var val = new double[] {1, 1, 2, 1, 5};
            IsTrue((val.getGeometricAverage() - 1.58489).abs() < 0.001);
        }
    }
    [TestClass]
    public class ModeTest
    {
        [TestMethod] public void Simple()
        {
            var val = new double[] {1, 1, 2, 1, 5};
            AreEqual(val.getMode(), 1);
        }
    }
    [TestClass]
    public class AllEqualNoComp
    {
        [TestMethod] public void True()
        {
            var val = new[] {1, 1, 1, 1, 1};
            IsTrue(val.AllEqual());
        }
        [TestMethod] public void FalseCenter()
        {
            var val = new[] {1, 1, 2, 1, 1};
            IsFalse(val.AllEqual());
        }
        [TestMethod] public void FalseEnd()
        {
            var val = new[] {1, 1, 1, 1, 2};
            IsFalse(val.AllEqual());
        }
        [TestMethod] public void FalseStart()
        {
            var val = new[] {2, 1, 1, 1, 1};
            IsFalse(val.AllEqual());
        }
        [TestMethod] public void Empty()
        {
            var val = new int[] {};
            IsTrue(val.AllEqual());
        }
    }
    [TestClass]
    public class AllEqualComp
    {
        private static readonly IEqualityComparer<int> _comp = new EqualityFunctionComparer<int, int>(a => a % 3);
        [TestMethod] public void True()
        {
            var val = new[] {1, 1, 7, 1, 4};
            IsTrue(val.AllEqual(_comp));
        }
        [TestMethod] public void FalseCenter()
        {
            var val = new[] {1, 3, 2, 1, 7};
            IsFalse(val.AllEqual(_comp));
        }
        [TestMethod] public void FalseEnd()
        {
            var val = new[] {1, 7, 1, 3, 2};
            IsFalse(val.AllEqual(_comp));
        }
        [TestMethod] public void FalseStart()
        {
            var val = new[] {2, 1, 7, 1, 1};
            IsFalse(val.AllEqual(_comp));
        }
        [TestMethod] public void Empty()
        {
            var val = new int[] {};
            IsTrue(val.AllEqual(_comp));
        }
    }
    [TestClass]
    public class TupleFlipTest
    {
        [TestMethod] public void Simple()
        {
            var val = Tuple.Create(1, true);
            AreEqual(val.FlipTuple(),Tuple.Create(true,1));
        }
    }

}