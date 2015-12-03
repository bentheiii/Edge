
using System;
using System.Text;
using System.Collections.Generic;
using Edge.Fielding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace CoreTest
{
    /// <summary>
    /// Summary description for Init
    /// </summary>
    [TestClass]
    public class Init
    {
        public static bool AllowLongTests { get; private set; }
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        [AssemblyInitialize] public static void Ainit(TestContext t)
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Fields).TypeHandle);
            AllowLongTests = false;
        }
        #endregion
    }
}
