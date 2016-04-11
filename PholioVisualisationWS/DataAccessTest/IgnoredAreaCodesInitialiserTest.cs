using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class IgnoredAreaCodesInitialiserTest
    {
        [TestMethod]
        public void TestAreaCodesIgnoredForSpineChartContainsValuesInAreaCodesIgnoredEverywhereStringToo()
        {
            var config = new ProfileConfig()
            {
                AreaCodesIgnoredEverywhereString = "a",
                AreaCodesIgnoredForSpineChartString = "b"
            };

            var initialised = new IgnoredAreaCodesInitialiser(config).Initialised;

            Assert.AreEqual(1, initialised.AreaCodesIgnoredEverywhere.Count);
            Assert.AreEqual(2, initialised.AreaCodesIgnoredForSpineChart.Count);
        }

        [TestMethod]
        public void TestNullProfileConfigHandled()
        {
            var initialised = new IgnoredAreaCodesInitialiser(null).Initialised;

            Assert.AreEqual(0, initialised.AreaCodesIgnoredEverywhere.Count);
            Assert.AreEqual(0, initialised.AreaCodesIgnoredForSpineChart.Count);
        }

        [TestMethod]
        public void TestNullAreaCodesStringValuesAreHandled()
        {
            var initialised = new IgnoredAreaCodesInitialiser(new ProfileConfig()).Initialised;

            Assert.AreEqual(0, initialised.AreaCodesIgnoredEverywhere.Count);
            Assert.AreEqual(0, initialised.AreaCodesIgnoredForSpineChart.Count);
        }

        [TestMethod]
        public void TestEmptyAreaCodesStringValuesAreHandled()
        {
            var ignoredAreaCodes = new ProfileConfig
            {
                AreaCodesIgnoredEverywhereString = "",
                AreaCodesIgnoredForSpineChartString = ""
            };

            var initialised = new IgnoredAreaCodesInitialiser(ignoredAreaCodes).Initialised;

            Assert.AreEqual(0, initialised.AreaCodesIgnoredEverywhere.Count);
            Assert.AreEqual(0, initialised.AreaCodesIgnoredForSpineChart.Count);
        }
    }
}
