using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.MainUI.Helpers;
using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MainUITest.Helpers
{
    [TestClass]
    public class IndicatorOwnerChangerTest
    {
        private ProfileRepository _profileRepository;
        private ProfilesReader _profilesReader;

        [TestInitialize]
        public void TestInitialize()
        {
            _profileRepository = new ProfileRepository();
            _profilesReader = ReaderFactory.GetProfilesReader();
        }

        [TestMethod]
        public void TestAssignIndicatorToProfile()
        {
            const int indicatorId = IndicatorIds.IDAOPI;

            var changer = new IndicatorOwnerChanger(_profilesReader, _profileRepository);

            // Set first owner
            const int profileId = ProfileIds.Phof;
            changer.AssignIndicatorToProfile(indicatorId, profileId);
            Assert.AreEqual(profileId, GetOwnerProfileId(indicatorId));

            // Confirm owner can be changed
            const int profileId2 = ProfileIds.HealthProfiles;
            changer.AssignIndicatorToProfile(indicatorId, profileId2);
            Assert.AreEqual(profileId2, GetOwnerProfileId(indicatorId));
        }

        [TestMethod]
        public void TestAssignIndicatorToProfileWhereOverriddenMetadata()
        {
            var changer = new IndicatorOwnerChanger(_profilesReader, _profileRepository);
            var indicatorId = IndicatorIds.Under18Conceptions;

            // Set first owner
            const int profileId = ProfileIds.SexualHealth;
            changer.AssignIndicatorToProfile(indicatorId, profileId);
            Assert.AreEqual(profileId, GetOwnerProfileId(indicatorId));

            // Confirm owner can be changed
            const int profileId2 = ProfileIds.HealthProfiles;
            changer.AssignIndicatorToProfile(indicatorId, profileId2);
            Assert.AreEqual(profileId2, GetOwnerProfileId(indicatorId));
        }

        private int GetOwnerProfileId(int indicatorId)
        {
            return _profileRepository.GetIndicatorMetadata(indicatorId).OwnerProfileId;
        }
    }
}
