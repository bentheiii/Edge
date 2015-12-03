using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Edge.Assert.AssertExtentions;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class AssertExtentionsTest
    {
        [TestMethod]
        public void SimpleTrue()
        {
            AreEqual(3,9,7);
            AreEqual(10, 9, 2);
            AreEqual(10, 10, 0);
        }
        [TestMethod, ExpectedException(typeof(AssertFailedException))]
        public void SimpleFalse()
        {
            AreEqual(3, 9, 2);
            Fail();
        }
    }
}
