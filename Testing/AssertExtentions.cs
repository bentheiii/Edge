﻿using Edge.Fielding;

namespace Edge.Assert
{
    public static class AssertExtentions
    {
        public static void AreEqual<T>(T expected, T actual, T delta)
        {
            if (!expected.Equals(actual) && (expected.ToFieldWrapper() - actual).abs() > delta)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, actual);
            }
        }
    }
}