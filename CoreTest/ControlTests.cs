using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Edge.Controls;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class BindEnumTest
    {
        private enum TestEnum
        {
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Five = 5
        };
        [TestMethod]
        public void Simple()
        {
            var val = new ComboBox();
            val.BindEnum<TestEnum>();
            var c = ((IEnumerable<TestEnum>)val.DataSource).ToArray();
            IsTrue(c.SequenceEqual(new[] {TestEnum.Zero,TestEnum.One, TestEnum.Two, TestEnum.Three,TestEnum.Five, }));
        }
        public void SimpleSelect()
        {
            var val = new ComboBox();
            val.BindEnum<TestEnum>(TestEnum.Two);
            var c = ((IEnumerable<TestEnum>)val.DataSource).ToArray();
            IsTrue(c.SequenceEqual(new[] { TestEnum.Zero, TestEnum.One, TestEnum.Two, TestEnum.Three, TestEnum.Five, }));
            AreEqual(val.SelectedItem, TestEnum.Two);
        }
    }
}
