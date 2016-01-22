using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.MainUI.Common;

namespace IndicatorsUITest
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
