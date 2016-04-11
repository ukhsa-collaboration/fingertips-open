using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestShouldSerializeShowLabelOnLeftOfValue()
        {
            Assert.IsTrue(new Unit{ShowLabelOnLeftOfValue = true}.ShowLabelOnLeftOfValue);
            Assert.IsFalse(new Unit { ShowLabelOnLeftOfValue = false }.ShowLabelOnLeftOfValue);
        }
    }
}
