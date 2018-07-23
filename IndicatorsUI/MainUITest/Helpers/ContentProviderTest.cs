using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUI.Helpers;

namespace IndicatorsUI.MainUITest.Helpers
{
    [TestClass]
    public class ContentProviderTest
    {
        [TestMethod]
        public void TestEmptyStringIfContentDoesNotExist()
        {
            var content = ContentProvider.GetContent("", ProfileIds.Undefined);
            Assert.AreEqual("", content.ToString());
        }

        [TestMethod]
        public void TestContentReturned()
        {
            var content = ContentProvider.GetContent(ContentKeys.Introduction, ProfileIds.Phof);
            Assert.AreNotEqual("", content.ToString());
        }

        [TestMethod]
        public void TestGetRecentUpdates()
        {
            var content = ContentProvider.GetRecentUpdates(ProfileIds.Phof);
            Assert.AreNotEqual("", content.ToString());
        }
    }
}
