using System;
using System.Collections.Generic;
using System.Linq;
using Edge.Fielding;
using Edge.Formulas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static CoreTest.AssertFieldsTest;
using static Edge.Assert.AssertExtentions;

namespace CoreTest
{
    public static class AssertFormula
    {
        public static void CheckVitals<T>(this Formula<T> @this, T val, params T[] coor)
        {
            CheckVitals(@this, val, (IEnumerable<T>)coor);
        }
        public static void CheckVitals<T>(this Formula<T> @this, T val, IEnumerable<T> coor)
        {
            IsTrue(@this.hasValue(coor.ToArray()));
            AreEqual(@this[coor.ToArray()], val, Fields.getField<T>().getDelta());
        }
        public static void CheckVitals<T>(this Formula<T> @this, bool hasval, IEnumerable<T> coor)
        {
            AreEqual(@this.hasValue(coor.ToArray()), hasval);
        }
        private static void CheckEquals<T>(Formula<T> first, Formula<T> second, params T[] coor)
        {
            CheckEquals(first, second, (IEnumerable<T>)coor);
        }
        private static void CheckEquals<T>(Formula<T> first, Formula<T> second, IEnumerable<T> coor)
        {
            if (first.hasValue(coor.ToArray()) && second.hasValue(coor.ToArray()))
                first.CheckVitals(second[coor.ToArray()], coor);
            else
                first.CheckVitals(second.hasValue(coor.ToArray()),coor);
        }
        public static void Equate(this Formula<double> @this, Formula<double> other)
        {
            double[] coors = {1.2, 0, 3.6, -9.1, 3, 1, Math.PI, Math.E};
            foreach (var coor in coors)
            {
                CheckEquals(@this,other,coor);
            }
        }
    }
    [TestClass]
    public class FormulaConstructionAndAccessTest
    {
        [TestMethod] public void Const()
        {
            Formula<double> val = new ConstantFormula<double>(3);
            val.CheckVitals(3, 9);
            val.CheckVitals(3, 10);
            val.CheckVitals(3, -8);
            val.CheckVitals(3, 0);
        }
        [TestMethod] public void Ident0()
        {
            Formula<double> val = new IdentFormula<double>(0);
            val.CheckVitals(9, 9);
            val.CheckVitals(10, 10);
            val.CheckVitals(-8, -8);
            val.CheckVitals(0, 0);
        }
        [TestMethod] public void Ident1()
        {
            Formula<double> val = new IdentFormula<double>(1);
            val.CheckVitals(9, 3, 9);
            val.CheckVitals(10, 0, 10);
            val.CheckVitals(-8, 8, -8);
            val.CheckVitals(0, 10, 0);
        }
        [TestMethod] public void Sum0()
        {
            Formula<double> val = new SumFormula<double>(Formula<double>.x, 3);
            val.CheckVitals(6, 3);
            val.CheckVitals(0, -3);
        }
        [TestMethod] public void Sum1()
        {
            Formula<double> val = new SumFormula<double>(Formula<double>.x, Formula<double>.y);
            val.CheckVitals(6, 3, 3);
            val.CheckVitals(1, -3, 4);
        }
        [TestMethod] public void Difference0()
        {
            Formula<double> val = new DifferenceFormula<double>(Formula<double>.x, 3);
            val.CheckVitals(0, 3);
            val.CheckVitals(60, 63);
        }
        [TestMethod] public void Difference1()
        {
            Formula<double> val = new DifferenceFormula<double>(Formula<double>.x, Formula<double>.y);
            val.CheckVitals(4, 8, 4);
            val.CheckVitals(-3, 60, 63);
        }
        [TestMethod] public void Product0()
        {
            Formula<double> val = new ProductFormula<double>(Formula<double>.x, 3);
            val.CheckVitals(9, 3);
            val.CheckVitals(-9, -3);
        }
        [TestMethod] public void Product1()
        {
            Formula<double> val = new ProductFormula<double>(Formula<double>.x, Formula<double>.y);
            val.CheckVitals(9, 3, 3);
            val.CheckVitals(-12, -3, 4);
        }
        [TestMethod] public void Quotient0()
        {
            Formula<double> val = new QuotientFormula<double>(Formula<double>.x, 3);
            val.CheckVitals(1, 3);
            val.CheckVitals(-2.0 / 3, -2);
        }
        [TestMethod] public void Quotient1()
        {
            Formula<double> val = new QuotientFormula<double>(Formula<double>.x, Formula<double>.y);
            val.CheckVitals(1, 3, 3);
            val.CheckVitals(-3.0 / 4, -3, 4);
        }
        [TestMethod] public void Log()
        {
            Formula<double> val = new LogFormula<double>(Formula<double>.x);
            val.CheckVitals(Math.Log(2.1),2.1);
        }
        [TestMethod] public void Exponent0()
        {
            Formula<double> val = new ExponentFormula<double>(Formula<double>.x, 3);
            val.CheckVitals(27, 3);
            val.CheckVitals(-27, -3);
        }
        [TestMethod] public void Exponent1()
        {
            Formula<double> val = new ExponentFormula<double>(Formula<double>.x, Formula<double>.y);
            val.CheckVitals(9, 3, 2);
            val.CheckVitals(81, -3, 4);
        }
        [TestMethod] public void Sin()
        {
            Formula<double> val = new SineFormula(Formula<double>.x);
            val.CheckVitals(Math.Sin(2.1), 2.1);
        }
        [TestMethod] public void MultiVal()
        {
            var val = Formula<double>.x + Formula<double>.y;
            val.CheckVitals(9,6,3);
            val.CheckVitals(6,6);
            val.CheckVitals(10,6,4,1);
        }
    }
    [TestClass]
    public class FormulaDeriveAndOptimiseTest
    {
        [TestMethod] public void Simple()
        {
            var val = 3 * (Formula<double>.x ^ 3) - Formula<double>.x.log() / Formula<double>.x;
            val.derive().Optimise().Equate(9*(Formula<double>.x^2) - (1/ (Formula<double>.x ^ 2)) + (Formula<double>.x.log()/(Formula<double>.x^2)));
        }
    }
}