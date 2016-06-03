using Edge.Looping;
using Edge.NumbersMagic;
using Edge.SystemExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class DivisibilityQuerierTest
    {
        [TestMethod]
        public void Simple()
        {
            var val = new DivisibilityQuerier(2);
            foreach (int i in Loops.Range(1,100))
            {
                int s;
                var div = val.Divisibility(i, out s);
                AreEqual(2.pow(div) * s , i);
            }
        }
    }
}
