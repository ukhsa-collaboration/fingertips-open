using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.MainUI.Helpers;
using Fpm.ProfileData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsUITest.Helpers
{
    [TestClass]
    public class UserDetailsTest
    {
        [TestMethod]
        public void TestFpmUser()
        {
            var user = UserWithRights().FpmUser;
            Assert.IsNotNull(user, "No user object found");
            Assert.AreEqual(FpmUserIds.Doris, user.Id, "UserDetails ID not as expected");
        }

        [TestMethod]
        public void TestNewUserFromUserId()
        {
            var user = UserWithRights().FpmUser;
            Assert.IsNotNull(user, "No user object found");
            Assert.AreEqual(FpmUserIds.Doris, user.Id, "UserDetails ID not as expected");
        }

        [TestMethod]
        public void TestHasWritePermissionsToProfile()
        {
            Assert.IsTrue(UserWithRights()
                .HasWritePermissionsToProfile(ProfileIds.Phof));

            Assert.IsFalse(UserWithNoRights()
                .HasWritePermissionsToProfile(ProfileIds.Phof));
        }

        [TestMethod]
        public void TestGetProfilesUserHasPermissionsTo()
        {
            Assert.IsTrue(UserWithRights().GetProfilesUserHasPermissionsTo().Count() > 10);

            Assert.AreEqual(0, UserWithNoRights().GetProfilesUserHasPermissionsTo().Count());
        }

        [TestMethod]
        public void TestGetProfilesUserHasPermissionsTo2()
        {
            Assert.IsTrue(UserWithRights().GetProfilesUserHasPermissionsTo().Count() > 10);
            var user = UserDetails.NewUserFromUserId(FpmUserIds.FarrukhAyub);
            var userProfiles = user.GetProfilesUserHasPermissionsTo();
            foreach (var userProfile in userProfiles)
            {
                Assert.IsNotNull(userProfile);
            }
        }


        private static UserDetails UserWithNoRights()
        {
            return UserDetails.NewUserFromUserId(FpmUserIds.UserWithNoRightsToAnything);
        }

        private static UserDetails UserWithRights()
        {
            return UserDetails.NewUserFromUserId(FpmUserIds.Doris);
        }
    }
}
