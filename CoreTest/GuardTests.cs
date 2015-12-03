using System.Text;
using Edge.Guard;
using Edge.SystemExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class GuardConstructionTest
    {
        [TestMethod]
        public void Simple()
        {
            var val = new EventGuard<int>();
            AreEqual(val.value,0);
        }
        [TestMethod] public void initialvalue()
        {
            var val = new EventGuard<int>(3);
            AreEqual(val.value, 3);
        }
    }
    [TestClass]
    public class GuardMutabilityTest
    {
        [TestMethod] public void Simple()
        {
            var val = new EventGuard<int>();
            val.Mutate(a => a.value++);
            AreEqual(val.value, 1);
        }
        [TestMethod]
        public void Simple2()
        {
            var val = new EventGuard<int>();
            val.value++;
            AreEqual(val.value, 1);
        }
    }
    [TestClass]
    public class GuardCloningTest
    {
        [TestMethod]
        public void Simple()
        {
            StringBuilder b = new StringBuilder();
            EventGuard<int>.GuardAccessedHandler a = (sender, type) => GuardEventsTest.Access(b);
            EventGuard<int>.GuardDrawHandler d = (sender, value) => GuardEventsTest.Get(b);
            EventGuard<int>.GuardChangedHandler c = (sender, e) => GuardEventsTest.Set(b);
            var val = new EventGuard<int>(3);
            val.accessed += a;
            val.drawn += d;
            val.changed += c;
            var co = val.Copy();
            co.accessed -= a;
            co.drawn -= d;
            co.changed -= c;
            co.EventValue = 2;
            AreEqual(b.ToString(),"");
            AreEqual(co.EventValue, 2);
            AreEqual(val.EventValue, 3);
            val.EventValue = 4;
            AreEqual(b.ToString(),"agas");
        }
    }
    [TestClass]
    public class GuardEventsTest
    {
        public static void Access(StringBuilder b)
        {
            b.Append('a');
        }
        public static void Get(StringBuilder b)
        {
            b.Append('g');
        }
        public static void Set(StringBuilder b)
        {
            b.Append('s');
        }
        [TestMethod]
        public void Simple()
        {
            var val = new EventGuard<int>();
            StringBuilder b = new StringBuilder();
            val.accessed += (sender, type) => Access(b);
            val.drawn += (sender, value) => Get(b);
            val.changed += (sender, e) => Set(b);
            var temp = val.EventValue;
            val.EventValue = temp;
            AreEqual(b.ToString(),"agas");
        }
    }
}
