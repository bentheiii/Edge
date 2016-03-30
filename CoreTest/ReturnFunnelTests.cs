using System;
using System.Linq;
using Edge.Funnels;
using Edge.WordsPlay;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class StandardFunnelTest
    {
        [TestMethod] public void Simple()
        {
            var val = new Funnel<double,double>();
            val.Add((double processed, out double returnval) =>
            {
                returnval = -processed;
                return processed < 0;
            });
            val.Add((double processed, out double returnval) =>
            {
                returnval = processed;
                return processed > 0;
            });
            val.Add((double processed, out double returnval) =>
            {
                returnval = -1;
                return processed == 0;
            });
            AreEqual(val.Process(-7),7);
            AreEqual(val.Process(9),9);
            AreEqual(val.Process(0),-1);
        }
        [TestMethod, ExpectedException(typeof(NoValidProcessorException))] public void NoProcessor()
        {
            var val = new Funnel<double, double>();
            val.Add((double processed, out double returnval) =>
            {
                returnval = -processed;
                return processed < 0;
            });
            val.Add((double processed, out double returnval) =>
            {
                returnval = processed;
                return processed > 0;
            });
            AreEqual(val.Process(-7), 7);
            AreEqual(val.Process(9), 9);
            AreEqual(val.Process(0), -1);
            Fail();
        }
    }
    [TestClass]
    public class ConditionalFunnelTest
    {
        [TestMethod] public void Simple()
        {
            var val = new ConditionFunnel<double,double>();
            val.Add(a=>a<0,a=>-a);
            val.Add(a => a > 0, a => a);
            val.Add(a => a ==0, a => -1);
            AreEqual(val.Process(-7), 7);
            AreEqual(val.Process(9), 9);
            AreEqual(val.Process(0), -1);
        }
    }
    [TestClass]
    public class QualifierFunnelTest
    {
        [TestMethod]
        public void Simple()
        {
            var val = new QualifierFunnel<double, double, double>((d, d1) => d <= d1);
            val.Add(0, a => a + 10);
            val.Add(2, a => a + 8);
            val.Add(4, a => a + 6);
            val.Add(6, a => a + 4);
            val.Add(8, a => a + 2);
            val.Add(10, a => a + 0);
            val.Add(int.MaxValue, a => 0);
            AreEqual(val.Process(0), 10);
            AreEqual(val.Process(3), 9);
            AreEqual(val.Process(20), 0);
        }
    }
    [TestClass]
    public class TypeFunnelTest
    {
        class Base { }
        class A : Base { }
        class B : Base { }
        class C : Base { }
        [TestMethod]
        public void Simple()
        {
            var val = new TypeFunnel<Base, char>();
            val.Add((A a)=>'a');
            val.Add((B a) => 'b');
            val.Add((C a) => 'c');
            val.Add((Base a) => '\0');
            AreEqual(val.Process(new A()), 'a');
            AreEqual(val.Process(new B()), 'b');
            AreEqual(val.Process(new C()), 'c');
            AreEqual(val.Process(new Base()), '\0');
        }
    }
    [TestClass]
    public class PrefixFunnelTest
    {
        [TestMethod]
        public void Simple()
        {
            var val = new PrefixFunnel<char,string>();
            val.Add("U", a=>a.convertToString().ToUpper());
            val.Add("L",a=>a.convertToString().ToLower());
            val.Add("R",a=>a.Reverse().convertToString());
            val.Add(a=>a.convertToString());
            AreEqual(val.Process("LA"), "a");
            AreEqual(val.Process("Ub"), "B");
            AreEqual(val.Process("Rab"), "ba");
            AreEqual(val.Process("Special"), "Special");
        }
    }
}
