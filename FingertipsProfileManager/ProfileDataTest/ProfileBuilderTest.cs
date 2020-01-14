using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.ProfileDataTest
{
    [TestClass]
    public class ProfileBuilderTest
    {
        private const string UrlKey = UrlKeys.HealthProfiles;
        private ProfileRepository _profileRepository;
        private ProfilesReader _reader;

        [TestInitialize]
        public void Init()
        {
            _profileRepository = new ProfileRepository();
            _reader = ReaderFactory.GetProfilesReader();
        }

        [TestCleanup]
        public void CleanUp()
        {
            _profileRepository.Dispose();
        }

        [TestMethod]
        public void TestGetSelectedGroupingMetadata()
        {
            Profile profile = new ProfileBuilder(_reader, _profileRepository).Build(UrlKey, 1, AreaTypeIds.CountyAndUnitaryAuthorityPre2019);
            Assert.AreEqual(profile.GetSelectedGroupingMetadata(1).GroupId, profile.GroupingMetadatas[0].GroupId);

            profile = new ProfileBuilder(_reader, _profileRepository).Build(UrlKey, 3, AreaTypeIds.CountyAndUnitaryAuthorityPre2019);
            Assert.AreEqual(profile.GetSelectedGroupingMetadata(3).GroupId, profile.GroupingMetadatas[2].GroupId);

            // First is selected by default
            profile = new ProfileBuilder(_reader, _profileRepository).Build(UrlKey);
            Assert.AreEqual(profile.GetSelectedGroupingMetadata(1).GroupId, profile.GroupingMetadatas[0].GroupId);
        }

        [TestMethod]
        public void TestGroupingMetadatas()
        {
            Profile profile = new ProfileBuilder(_reader, _profileRepository).Build(UrlKey, 1, AreaTypeIds.CountyAndUnitaryAuthorityPre2019);

            // Assert: number of domains
            var count = profile.GroupingMetadatas.Count;
            Assert.IsTrue(count > 5 && count < 10);
        }

        [TestMethod]
        public void TestIndicatorNamesDefinedIfOnlyOneGrouping()
        {
            Profile profile = new ProfileBuilder(_reader, _profileRepository).Build(
                UrlKeys.Tobacco, 1, AreaTypeIds.CountyAndUnitaryAuthorityPre2019);
            Assert.IsTrue(profile.IndicatorNames.Count > 0);
        }
    }
}
