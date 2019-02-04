using Fpm.MainUI.Controllers;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.MainUITest.Controllers
{
    [TestClass]
    public class WhenUsingAdminController
    {
        [TestMethod]
        public void TestGetIndicatorOwner()
        {
            var c = new AdminController();
            var result = c.GetIndicatorOwner(IndicatorIds.ChildrenInPoverty);
            var profile = (ProfileDetails) result.Data;
            Assert.IsNotNull(profile);
        }

        [TestMethod]
        public void TestGetIndicatorOwnerInvalidIndicatorId()
        {
            var c = new AdminController();
            var result = c.GetIndicatorOwner(IndicatorIds.IndicatorThatDoesNotExist);
            var profile = (ProfileDetails)result.Data;
            Assert.IsNull(profile);
        }
    }
}
