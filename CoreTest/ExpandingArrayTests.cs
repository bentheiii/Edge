using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Edge.Arrays;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
namespace CoreTest
{
    [TestClass]
    public class ExpandingArrayTests
    {
        [TestMethod] public void Simple()
        {
            var val = new ExpandingArray<int>();
            val[4] = 1;
            val[8] = 1;
            val[2] = 1;
            var seq = val.Take(9);
            IsTrue(seq.SequenceEqual(new int[] {0, 0, 1, 0, 1, 0, 0, 0, 1}));
        }
    }
}
