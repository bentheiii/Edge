using System.Linq;
using Edge.Funnels;
using Edge.WordsPlay;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class VoidStandardFunnelTest
    {
        [TestMethod] public void Simple()
        {
            var val = new Funnel<double>();
            var t = 0;
            val.Add(a =>
            {
                t = (int)a;
                return a > 0;
            });
            val.Add(a =>
            {
                t = -(int)a;
                return a < 0;
            });
            val.Process(1.5);
            AreEqual(1, t);
            val.Process(-3);
            AreEqual(3, t);
        }
    }
    [TestClass]
    public class NillStandardFunnelTest
    {
        [TestMethod] public void Simple()
        {
            var val = new Funnel();
            var t = 0;
            var n = 0;
            val.Add(() =>
            {
                t = n;
                return n > 0;
            });
            val.Add(() =>
            {
                t = -n;
                return n < 0;
            });
            n = 1;
            val.Process();
            AreEqual(1, t);
            n = -3;
            val.Process();
            AreEqual(3, t);
        }
    }
    [TestClass]
    public class VoidConditionalFunnelTest
    {
        [TestMethod] public void Simple()
        {
            var val = new ConditionFunnel<double>();
            var t = 0;
            val.Add(a => a > 0, a => { t = (int)a; });
            val.Add(a => a <= 0, a => { t = -(int)a; });
            val.Process(1.5);
            AreEqual(1, t);
            val.Process(-3);
            AreEqual(3, t);
        }
    }
    [TestClass]
    public class NillConditionalFunnelTest
    {
        [TestMethod] public void Simple()
        {
            var val = new ConditionFunnel();
            var t = 0;
            var n = 0;
            val.Add(() => n > 0, () => { t = n; });
            val.Add(() => n <= 0, () => { t = -n; });
            n = 1;
            val.Process();
            AreEqual(1, t);
            n = -3;
            val.Process();
            AreEqual(3, t);
        }
    }
    [TestClass]
    public class VoidQualifierFunnelTest
    {
        [TestMethod] public void Simple()
        {
            var val = new QualifierFunnel<double, double>((d, d1) => d >= d1);
            var t = 0;
            val.Add(20, a => t = 20);
            val.Add(10, a => t = 10);
            val.Add(5, a => t = 5);
            val.Add(0, a => t = 0);
            val.Process(23);
            AreEqual(t, 20);
            val.Process(15);
            AreEqual(t, 10);
            val.Process(0);
            AreEqual(t, 0);
        }
    }
    [TestClass]
    public class NillQualifierFunnelTest
    {
        [TestMethod] public void Simple()
        {
            var t = 0;
            var n = 0;
            var val = new QualifierFunnel<double>((d) => n >= d);
            val.Add(20, () => t = 20);
            val.Add(10, () => t = 10);
            val.Add(5, () => t = 5);
            val.Add(0, () => t = 0);
            n = 23;
            val.Process();
            AreEqual(t, 20);
            n = 15;
            val.Process();
            AreEqual(t, 10);
            n = 0;
            val.Process();
            AreEqual(t, 0);
        }
    }
    [TestClass]
    public class VoidTypeFunnelTest
    {
        private class Base { }
        private class A : Base { }
        private class B : Base { }
        private class C : Base { }
        [TestMethod] public void Simple()
        {
            var val = new TypeFunnel<Base>();
            var t = '0';
            val.Add((A a) => t = 'a');
            val.Add((B a) => t = 'b');
            val.Add((C a) => t = 'c');
            val.Add((Base a) => t = '\0');
            val.Process(new A());
            AreEqual(t, 'a');
            val.Process(new B());
            AreEqual(t, 'b');
            val.Process(new C());
            AreEqual(t, 'c');
            val.Process(new Base());
            AreEqual(t, '\0');
        }
    }
    [TestClass]
    public class VoidPrefixFunnelTest
    {
        [TestMethod] public void Simple()
        {
            var val = new PrefixFunnel<char>();
            var t = "";
            val.Add("U", a => t = a.convertToString().ToUpper());
            val.Add("L", a => t = a.convertToString().ToLower());
            val.Add("R", a => t = a.Reverse().convertToString());
            val.Add(a => t = a.convertToString());
            val.Process("LA");
            AreEqual(t, "a");
            val.Process("Ub");
            AreEqual(t, "B");
            val.Process("Rab");
            AreEqual(t, "ba");
            val.Process("Special");
            AreEqual(t, "Special");
        }
    }
}