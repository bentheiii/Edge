using System;
using System.Linq;
using Edge.Arrays;
using Edge.Looping;
using Edge.RecursiveQuerier;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class LazyArrayTests
    {
        [TestMethod] public void Simple()
        {
            var val = new LazyArray<int>((a,f) =>
            {
                if (a == 0)
                    return 1;
                var x = f[a / 2];
                x = x * x;
                if (a % 2 == 1)
                    x *= 2;
                return x;
            });
            AreEqual(val[10],1024);
            AreEqual(val[30],1073741824);
            IsTrue(val.Take(10).SequenceEqual(Loops.YieldAggregate(a=>2*a,1).Take(10)));
        }
    }
    [TestClass] public class QuerierTests
    {
        [TestMethod] public void Simple()
        {
            var pow = new PowQuerier<int>(2);
            var prod = new ProdQuerier<int>(2);
            foreach (int i in Loops.Range(20))
            {
                AreEqual(pow[i],1 << i);
                AreEqual(prod[i],2*i);
            }
        }
    }
}
