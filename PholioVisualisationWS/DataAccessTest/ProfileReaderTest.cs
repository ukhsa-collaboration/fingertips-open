using System;
using System.Collections.Generic;
using System.Linq;
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
            var pdfs = Reader().GetProfilePdfs(ProfileIds.ChildAndMaternalHealth);

            Assert.IsNotNull(pdfs.FirstOrDefault(x => x.AreaTypeId == AreaTypeIds.CountyAndUnitaryAuthority));
            Assert.IsNull(pdfs.FirstOrDefault(x => x.AreaTypeId == AreaTypeIds.Subregion));
        }

        [TestMethod]
        public void TestGetAreaCodesToIgnoreEverywhere()
        {
            IList<string> codes = Reader()
                .GetAreaCodesToIgnore(ProfileIds.HealthProfiles)
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
        public void TestGetLongerLivesProfileIds()
        {
            var ids = Reader().GetLongerLivesProfileIds();
            Assert.IsTrue(ids.Any());
        }

        [TestMethod]
        public void TestGetProfilesForIndicatorsWithOneIndicator()
        {
            var indicators = new List<int> {IndicatorIds.LifeExpectancyAtBirth};
            var profilesForIndicators = Reader().GetProfilesForIndicators(indicators, AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019);
            Assert.IsTrue(profilesForIndicators.Count > 0);
            Assert.AreEqual(profilesForIndicators[0].IndicatorId, IndicatorIds.LifeExpectancyAtBirth);
        }

        [TestMethod]
        public void TestGetProfilesForIndicatorsWithMultipleIndicators()
        {
            const int indicator1 = IndicatorIds.DeprivationScoreIMD2015;
            const int indicator2 = IndicatorIds.LifeExpectancyAtBirth;

            var indicators = new List<int> { indicator1, indicator2};
            var profilesForIndicators = Reader().GetProfilesForIndicators(indicators, AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019);

            // Assert
            Assert.IsTrue(profilesForIndicators.Count > 0);
            Assert.IsNotNull(profilesForIndicators.FirstOrDefault(x => x.IndicatorId == indicator2));
            Assert.IsNotNull(profilesForIndicators.FirstOrDefault(x=> x.IndicatorId == indicator1));
        }

        private static IProfileReader Reader()
        {
            var reader = ReaderFactory.GetProfileReader();
            return reader;
        }
    }
}
