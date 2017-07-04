using System;
using System.Collections.Generic;
using System.Linq;
using DIResolver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class ProfileReaderTest
    {
        [TestMethod]
        public void TestGetGroupIdsFromAllProfiles()
        {
            var groupIds = Reader().GetGroupIdsFromAllProfiles();
            Assert.IsTrue(groupIds.Count > 0);
            Assert.IsTrue(groupIds.Contains(GroupIds.Phof_WiderDeterminantsOfHealth));
        }

        [TestMethod]
        public void TestGetGroupIdsFromSpecificProfiles()
        {
            var groupIds = Reader()
                .GetGroupIdsFromSpecificProfiles(new List<int>
                {
                    ProfileIds.Phof, // 5 domains
                    ProfileIds.HealthProfiles // 7 domains
                });

            // Assert: expected number of domains
            var count = groupIds.Count;
            Assert.IsTrue(count > 10 && count < 20);

            // Assert: expected domain is included
            Assert.IsTrue(groupIds.Contains(GroupIds.Phof_WiderDeterminantsOfHealth));
        }

        [TestMethod]
        public void TestGetProfile()
        {
            Profile profile = Reader().GetProfile(ProfileIds.Phof);

            Assert.AreEqual(ProfileIds.Phof, profile.Id);
            Assert.IsTrue(profile.Name.ToLower().Contains("outcomes "));
            Assert.IsTrue(profile.UrlKey.ToLower().Contains("outcomes-"));
            Assert.IsTrue(profile.GroupIds.Contains(1000041));
        }

        [TestMethod]
        public void TestGetProfilePdfs()
        {
            var areaTypes = Reader().GetProfilePdfs(ProfileIds.Phof);

            Assert.IsNotNull(areaTypes.FirstOrDefault(x => x.AreaTypeId == AreaTypeIds.CountyAndUnitaryAuthority));
            Assert.IsNull(areaTypes.FirstOrDefault(x => x.AreaTypeId == AreaTypeIds.DistrictAndUnitaryAuthority));
        }

        [TestMethod]
        public void TestGetAreaCodesToIgnoreEverywhere()
        {
            IList<string> codes = Reader()
                .GetAreaCodesToIgnore(ProfileIds.LongerLives)
                .AreaCodesIgnoredEverywhere;
            Assert.AreEqual(3, codes.Count);
            Assert.IsTrue(codes.Contains(AreaCodes.CountyUa_Bedfordshire));
            Assert.IsTrue(codes.Contains(AreaCodes.CountyUa_IslesOfScilly));
            Assert.IsTrue(codes.Contains(AreaCodes.CountyUa_CityOfLondon));
        }

        [TestMethod]
        public void TestAreaCodesIgnoredForSpineChartAlsoContainThoseIgnoredEverywhere()
        {
            var codes = Reader()
                .GetAreaCodesToIgnore(ProfileIds.Phof)
                .AreaCodesIgnoredForSpineChart;
            Assert.AreEqual(3, codes.Count);
            Assert.IsTrue(codes.Contains(AreaCodes.CountyUa_Bedfordshire));
            Assert.IsTrue(codes.Contains(AreaCodes.CountyUa_IslesOfScilly));
            Assert.IsTrue(codes.Contains(AreaCodes.CountyUa_CityOfLondon));
        }

        [TestMethod]
        public void TestGetAllProfiles()
        {
            var profiles = Reader().GetAllProfiles();
            Assert.IsTrue(profiles.Any());
        }

        [TestMethod]
        public void TestGetAllProfileIds()
        {
            var ids = Reader().GetAllProfileIds();
            Assert.IsTrue(ids.Any());
        }

        [TestMethod]
        public void TestGetProfilesForIndicatorsWithOneIndicator()
        {
            var indicators = new List<int> {IndicatorIds.LifeExpectancyAtBirth};
            var profilesForIndicators = Reader().GetProfilesForIndicators(indicators, AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.IsTrue(profilesForIndicators.Count > 0);
            Assert.AreEqual(profilesForIndicators[0].IndicatorId, IndicatorIds.LifeExpectancyAtBirth);
        }

        [TestMethod]
        public void TestGetProfilesForIndicatorsWithMultipleIndicators()
        {
            var indicators = new List<int> { IndicatorIds.LifeExpectancyAtBirth, IndicatorIds.DeprivationScoreIMD2010 };
            var profilesForIndicators = Reader().GetProfilesForIndicators(indicators, AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.IsTrue(profilesForIndicators.Count > 0);
            Assert.IsTrue(profilesForIndicators.FirstOrDefault(x => x.IndicatorId == IndicatorIds.LifeExpectancyAtBirth) != null);
            Assert.IsTrue(profilesForIndicators.FirstOrDefault(x=> x.IndicatorId == IndicatorIds.DeprivationScoreIMD2010) != null);
        }

        private static IProfileReader Reader()
        {
            var reader = ReaderFactory.GetProfileReader();
            return reader;
        }
    }
}
