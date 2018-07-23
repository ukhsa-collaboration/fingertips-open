using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.MainUI.Helpers;

namespace IndicatorsUI.MainUITest.Helpers
{
    [TestClass]
    public class UrlHelperTest
    {
        [TestMethod]
        public void TestCombineUrl()
        {
            Assert.AreEqual("http://localhost/a/b/c", 
                UrlHelper.CombineUrl("http://localhost", "/a/", "/b", "c/"));
        }

        [TestMethod]
        public void TestCombineUrl_When_Last_Parameter_Is_Empty_Then_No_Trailing_Slash()
        {
            Assert.AreEqual("http://localhost/a",
                UrlHelper.CombineUrl("http://localhost", "/a/", ""));
        }
    }
}
