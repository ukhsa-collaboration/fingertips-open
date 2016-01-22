using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DomainObjects;
using Profiles.MainUI.Common;

namespace IndicatorsUITest
{
    [TestClass]
    public class UserDetailsTest
    {
        [TestMethod]
        public void TestDoesUserHaveAccessToSkinIsTrueWhenUserIsInAccessControlGroup()
        {
            Assert.IsTrue(UserDetails.NewUserFromName(UserNames.Doris)
                 .IsUserMemberOfAccessControlGroup(ActiveDirectoryGroups.FpmUsers));
        }

        [TestMethod]
        public void TestDoesUserHaveAccessToSkinIsFalseWhenUserIsNotInAccessControlGroup()
        {
            Assert.IsFalse(UserDetails.NewUserFromName(UserNames.Doris)
                 .IsUserMemberOfAccessControlGroup(ActiveDirectoryGroups.LibraryServices));
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException), "Could not find user: test.user.that.does.not.exist")]
        public void TestExceptionWhenUserNameIsNotKnown()
        {
            UserDetails.NewUserFromName("test.user.that.does.not.exist")
                .IsUserMemberOfAccessControlGroup(ActiveDirectoryGroups.FpmUsers);
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException), "Could not find group: test.user.that.does.not.exist")]
        public void TestExceptionWhenActiveDirectoryGroupNameIsNotKnown()
        {
            UserDetails.NewUserFromName(UserNames.Doris)
                .IsUserMemberOfAccessControlGroup("test.group.that.does.not.exist");
        }

        [TestMethod]
        public void TestDoesUserHaveAccessToSkinIsTrueWhenSkinHasNoAccessControlGroup()
        {
            var userDetails = UserDetails.NewUserFromName(UserNames.Doris);

            Assert.IsTrue(userDetails.IsUserMemberOfAccessControlGroup(null));
            Assert.IsTrue(userDetails.IsUserMemberOfAccessControlGroup(""));
            Assert.IsTrue(userDetails.IsUserMemberOfAccessControlGroup(" "));
        }
    }
}
