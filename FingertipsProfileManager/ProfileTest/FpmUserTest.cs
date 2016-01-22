using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.User;
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
