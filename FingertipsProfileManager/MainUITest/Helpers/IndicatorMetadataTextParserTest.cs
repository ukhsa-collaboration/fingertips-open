using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.MainUI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsUITest.Helpers
{
    [TestClass]
    public class IndicatorMetadataTextParserTest
    {
        [TestMethod]
        public void TestParseOneSimpleProperty()
        {
            var items = new IndicatorMetadataTextParser().Parse(Join("2", "a"));
            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(2, items.First().PropertyId);
            Assert.AreEqual("a", items.First().Text);
        }

        [TestMethod]
        public void TestParseTwoSimpleProperties()
        {
            var items = new IndicatorMetadataTextParser().Parse(Join("2", "a", "3", "b"));
            Assert.AreEqual(2, items.Count);

            var item = items[0];
            Assert.AreEqual(2, item.PropertyId);
            Assert.AreEqual("a", item.Text);

            item = items[1];
            Assert.AreEqual(3, item.PropertyId);
            Assert.AreEqual("b", item.Text);
        }

        [TestMethod]
        public void TestIsOverriddenParsedCorrectly()
        {
            var items = new IndicatorMetadataTextParser().Parse(Join("2", "a", "3o", "b"));
            Assert.IsFalse(items[0].IsOverridden);
            Assert.IsTrue(items[1].IsOverridden);
        }

        private static string Join(params string[] items)
        {
            return string.Join(IndicatorMetadataTextParser.Separator.ToString(), items);
        }
    }
}
