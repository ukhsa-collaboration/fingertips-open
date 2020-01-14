using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class ProfilePerIndicatorBuilderTest
    {
        public const int Indicator1 = IndicatorIds.DeprivationScoreIMD2015;

        [TestMethod]
        public void TestEmptyIndicatorList()
        {
            var builder = new ProfilePerIndicatorBuilder(true);
            var result = builder.Build(new List<int>(), AreaTypeIds.DistrictAndUnitaryAuthorityPreApr2019);

            // Assert: result is returned
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Keys.Count);
        }

        [TestMethod]
        public void BuildTest()
        {
            var result = BuildProfilesPerIndicator(false);
            Assert.IsNotNull(result);
            // Should have the same number of indicators as keys of Dictionary
            Assert.IsTrue(result.Keys.Count == 2);
            Assert.IsTrue(result.Values.Count > 0);
        }

        [TestMethod]
        public void UrlWithStagingTest()
        {
            var result = BuildProfilesPerIndicator(false);
            var profiles = result[Indicator1];
            Assert.IsTrue(profiles[0].Url.Contains("https://"));
        }

        [TestMethod]
        public void UrlWithProdTest()
        {
            var result = BuildProfilesPerIndicator(true);
            var profiles = result[Indicator1];
            Assert.IsTrue(profiles[0].Url.Contains("http://"));
        }

        private Dictionary<int, List<ProfilePerIndicator>> BuildProfilesPerIndicator(bool isEnvironmentLive)
        {
            var indicators = new List<int>
            {
                Indicator1,
                IndicatorIds.ChildrenInLowIncomeFamilies
            };
            var builder = new ProfilePerIndicatorBuilder(isEnvironmentLive);
            return builder.Build(indicators, AreaTypeIds.DistrictAndUnitaryAuthorityPreApr2019);
        }
    }
}
