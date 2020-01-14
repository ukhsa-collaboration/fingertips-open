using Fpm.MainUI.Controllers;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.MainUITest.Controllers
{
    [TestClass]
    public class WhenUsingAdminController
    {
        private ProfilesReader _reader;
        private ProfilesWriter _writer;
        private ProfileRepository _profileRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _reader = ReaderFactory.GetProfilesReader();
            _writer = ReaderFactory.GetProfilesWriter();
            _profileRepository = new ProfileRepository();
        }

        [TestMethod]
        public void TestGetIndicatorOwner()
        {
            var controller = GetController();
            var result = controller.GetIndicatorOwner(IndicatorIds.ChildrenInPoverty);
            var profile = (ProfileDetails) result.Data;
            Assert.IsNotNull(profile);
        }

        [TestMethod]
        public void TestGetIndicatorOwnerInvalidIndicatorId()
        {
            var controller = GetController();
            var result = controller.GetIndicatorOwner(IndicatorIds.IndicatorThatDoesNotExist);
            var profile = (ProfileDetails)result.Data;
            Assert.IsNull(profile);
        }

        private AdminController GetController()
        {
            return new AdminController(_reader, _writer, _profileRepository);
        }
    }
}
