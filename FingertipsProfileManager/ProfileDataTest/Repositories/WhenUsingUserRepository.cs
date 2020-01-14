using Fpm.ProfileData;
using Fpm.ProfileData.Entities.User;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fpm.ProfileDataTest.Repositories
{
    [TestClass]
    public class WhenUsingUserRepository
    {
        private int _profileId = ProfileIds.DevelopmentProfileForTesting;
        private int _userId = FpmUserIds.AlanJohnson;

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
            _userRepository.DeleteUserGroupPermissions(_profileId, _userId);
            Assert.IsNull(_userRepository.GetUserGroupPermissions(_profileId, _userId));

            // Act: Save new permission
            UserGroupPermissions permissions = new UserGroupPermissions
            {
                UserId = FpmUserIds.AlanJohnson,
                ProfileId = ProfileIds.DevelopmentProfileForTesting
            };

            _userRepository.SaveUserGroupPermissions(permissions);

            // Assert: Load permission to verify save
            var permissionsFromDb = _userRepository.GetUserGroupPermissions(_profileId, _userId);
            Assert.AreEqual(_userId, permissionsFromDb.UserId);
            Assert.AreEqual(_profileId, permissionsFromDb.ProfileId);
        }

        [TestMethod]
        public void TestDeleteUserGroupPermissions()
        {
            // Act: Delete 
            _userRepository.DeleteUserGroupPermissions(_profileId, _userId);

            // Assert: Check permission not in database
            Assert.IsNull(_userRepository.GetUserGroupPermissions(_profileId, _userId));
        }

        public static void MostRecentIsFirst(DateTime first, DateTime last)
        {
            Assert.AreEqual(1, first.CompareTo(last));
        }
    }
}