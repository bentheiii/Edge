using System;
using Edge.Guard;
using Edge.SystemExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class MutateTest
    {
        [TestMethod]
        public void Simple()
        {
            var val = new EventGuard<int>(3);
            AreEqual(val.Mutate(a=>a.value/=3).value,1);
        }
    }
}
