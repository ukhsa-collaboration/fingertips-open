using FingertipsUploadService.ProfileData.Entities.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProfileDataTest
{
    [TestClass]
    public class FpmUserTest
    {
        [TestMethod]
        public void TestEmailAddress()
        {
            var user = new FpmUser();
            user.UserName = @"phe\doris.hain";
            Assert.AreEqual("doris.hain@phe.gov.uk", user.EmailAddress);
        }
    }
}
