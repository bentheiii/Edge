using System;
using Edge.Fielding;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Edge.Assert
{
    public static class AssertExtentions
    {
        public static void AreEqual<T>(T expected, T actual, T delta)
        {
            try
            {
                IsTrue((expected.ToFieldWrapper() - actual).abs() <= delta);
            }
            catch (NotSupportedException)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected,actual);
            }
        }
    }
}