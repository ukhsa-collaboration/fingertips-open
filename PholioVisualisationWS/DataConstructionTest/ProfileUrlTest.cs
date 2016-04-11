using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class ProfileUrlTest
    {
        private const int GroupId = GroupIds.HealthProfiles_OurCommunities;
        private const int AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority;
        private ProfilePerIndicator profile;

        [TestInitialize]
        public void TestInitialize()
        {
            profile = new ProfilePerIndicator
            {
                LiveHostUrl = "live-host",
                TestHostUrl = "test-host",
                ProfileUrl = "key",
                GroupId = GroupId,
                AreaTypeId = AreaTypeId
            };
        }

        [TestMethod]
        public void TestLiveDataUrl()
        {
            var profileUrl = new ProfileUrl(profile, true);

            Assert.AreEqual("http://live-host/key#gid/" + GroupId +
                "/ati/" + AreaTypeId, profileUrl.DataUrl);
        }

        [TestMethod]
        public void TestStagingDataUrl()
        {
            var profileUrl = new ProfileUrl(profile, false);

            Assert.AreEqual("https://test-host/key#gid/" + GroupId +
                "/ati/" + AreaTypeId, profileUrl.DataUrl);
        }

        [TestMethod]
        public void TestLiveDataUrlForPracticeProfiles()
        {
            profile.ProfileId = ProfileIds.PracticeProfiles;

            var profileUrl = new ProfileUrl(profile, true);

            Assert.AreEqual("http://live-host/profile/general-practice/data", profileUrl.DataUrl);
        }

    }
}
