using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.MainUI.Helpers;

namespace IndicatorsUI.MainUITest.Helpers
{
    [TestClass]
    public class ContentHelperTest
    {
        [TestMethod]
        public void TestRemoveHtmlTags()
        {
            Assert.AreEqual("a", ContentHelper.RemoveHtmlTags("<p>a</p>"));
        }
    }
}
