using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace DataAccessTest
{
    [TestClass]
    public class ProfileInitialiserTest
    {
        private int profileId = 1;
        private string name = "a";
        private string urlKey = "url";

        [TestMethod]
        public void TestProfileIdAssigned()
        {
            Assert.AreEqual(profileId, InitialisedProfile().Id);
        }

        [TestMethod]
        public void TestProfileNameAssigned()
        {
            Assert.AreEqual(name, InitialisedProfile().Name);
        }

        [TestMethod]
        public void TestUrlKeyAssigned()
        {
            Assert.AreEqual(urlKey, InitialisedProfile().UrlKey);
        }

        [TestMethod]
        public void TestIsNationalAssigned()
        {
            Assert.IsTrue(InitialisedProfile().IsNational);
        }

        [TestMethod]
        public void TestNullGroupIdsHandled()
        {
            var profileConfig = GetProfileConfig();
            var ids = new ProfileInitialiser(profileConfig).InitialisedProfile.GroupIds;

            Assert.AreEqual(0, ids.Count);
        }

        [TestMethod]
        public void TestEmptyGroupIdsHandled()
        {
            var profileConfig = GetProfileConfig();
            var ids = new ProfileInitialiser(profileConfig).InitialisedProfile.GroupIds;

            Assert.AreEqual(0, ids.Count);
        }

        private Profile InitialisedProfile()
        {
            return new ProfileInitialiser(GetProfileConfig()).InitialisedProfile;
        }

        private ProfileConfig GetProfileConfig()
        {
            var profileConfig = new ProfileConfig
                {
                    ProfileId = profileId,
                    Name = name,
                    UrlKey = urlKey,
                    IsNational = true
                };
            return profileConfig;
        }
    }
}
