using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.User;

namespace Fpm.ProfileDataTest.Respositories
{
    [TestClass]
    public class WhenUsingUserRepository
    {

        private UserRepository _userRepository;

        [TestInitialize]
        public void Init()
        {
            _userRepository = new UserRepository();
        }

        [TestCleanup]
        public void CleanUp()
        {
            _userRepository.Dispose();
        }

        [TestMethod]
        public void TestGetUserAudit_MostRecentFirst()
        {
            // Act: get user audit
            var audits = _userRepository.GetUserAudit(new List<int> { FpmUserIds.TimWinters })
                .ToList();

            // Assert: most recent is first
            MostRecentIsFirst(audits.First().Timestamp, audits.Last().Timestamp);
        }

        [TestMethod]
        public void TestSaveUserGroupPermissions()
        {
            // Arrange: Ensure clean database
            _userRepository.DeleteUserGroupPermissions(ProfileIds.ProfileId, FpmUserIds.UserId);
            Assert.IsNull(_userRepository.GetUserGroupPermissions(ProfileIds.ProfileId, FpmUserIds.UserId));

            // Act: Save new permission
            UserGroupPermissions permissions = new UserGroupPermissions { UserId = FpmUserIds.UserId, ProfileId = ProfileIds.ProfileId }; // ProfileIds. FpmUserIds
            _userRepository.SaveUserGroupPermissions(permissions);

            // Assert: Load permission to verify save
            var permissionsFromDb = _userRepository.GetUserGroupPermissions(ProfileIds.ProfileId,  FpmUserIds.UserId);
            Assert.AreEqual(146, permissionsFromDb.UserId);
            Assert.AreEqual(115, permissionsFromDb.ProfileId);


            //var user = new FpmUser();
            //user.UserName = UserNames.Doris;
        }

        [TestMethod]
        public void TestDeleteUserGroupPermissions()
        {
            // Arrange: Ensure permissions in database


            // Act: Delete 
            _userRepository.DeleteUserGroupPermissions(ProfileIds.ProfileId, FpmUserIds.UserId);

            // Assert: Check permission not in database
            Assert.IsNull(_userRepository.GetUserGroupPermissions(ProfileIds.ProfileId, FpmUserIds.UserId));
        }

        public static void MostRecentIsFirst(DateTime first, DateTime last)
        {
            Assert.AreEqual(1, first.CompareTo(last));
        }
    }
}
