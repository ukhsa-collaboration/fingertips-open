using System;
using System.Linq;
using Fpm.ProfileData;
using Fpm.Search;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SearchTest
{
    [TestClass]
    public class IndicatorSearchTest
    {
        [TestMethod]
        public void TestSearchByText()
        {
            var word = "hip";
            var results = new IndicatorSearch().SearchByText(word);
            Assert.IsTrue(results.Any());
            foreach (var indicatorMetadataTextValue in results)
            {
                var name = indicatorMetadataTextValue.Name;
                Assert.IsTrue(name.ToLower().Contains(word),
                    word + " not found in " + name);
            }
        }

        [TestMethod]
        public void TestSearchByIndicatorId()
        {
            var results = new IndicatorSearch().SearchByIndicatorId(IndicatorIds.ObesityYear6);
            Assert.AreEqual(1, results.Count());
            Assert.IsTrue(results.First().Name.ToLower().Contains("obes"));
        }
    }
}
