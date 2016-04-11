using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.MainUI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.MainUITest.Helpers
{
    [TestClass]
    public class IndicatorSpecifierParserTest
    {
        [TestMethod]
        public void Test()
        {
            var detailsList = IndicatorSpecifierParser.Parse(new[] { "1~2~3" });

            Assert.AreEqual(1, detailsList.Count);

            var details = detailsList.First();
            Assert.AreEqual(1, details.IndicatorId);
            Assert.AreEqual(2, details.SexId);
            Assert.AreEqual(3, details.AgeId);
        }

        [TestMethod]
        public void TestTolerateAgeIdNotBeingPresent()
        {
            var detailsList = IndicatorSpecifierParser.Parse(new[] { "1~2" });

            Assert.AreEqual(1, detailsList.Count);

            var details = detailsList.First();
            Assert.AreEqual(1, details.IndicatorId);
            Assert.AreEqual(2, details.SexId);
        }

        [TestMethod]
        public void TestEmptyStringIgnored()
        {
            var detailsList = IndicatorSpecifierParser.Parse(new[] { "" });
            Assert.IsFalse(detailsList.Any());
        }

        [TestMethod]
        public void TestEmptyListIgnored()
        {
            var detailsList = IndicatorSpecifierParser.Parse(new string[] { });
            Assert.IsFalse(detailsList.Any());
        }

        [TestMethod]
        public void TestNullListIgnored()
        {
            var detailsList = IndicatorSpecifierParser.Parse(null);
            Assert.IsFalse(detailsList.Any());
        }

        [TestMethod]
        public void TestNullItemsIgnored()
        {
            var detailsList = IndicatorSpecifierParser.Parse(new string[] { null });
            Assert.IsFalse(detailsList.Any());
        }
    }
}
