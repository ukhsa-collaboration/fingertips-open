using Fpm.MainUI;
using Fpm.MainUI.Mappers;
using Fpm.MainUI.ViewModels;
using Fpm.ProfileData.Entities.Profile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Fpm.MainUITest
{
    [TestClass]
    public class WhenUsingMapper
    {
        [TestInitialize]
        public void Init()
        {
            AutoMapperConfig.RegisterMappings();
        }

        [TestMethod]
        public void ToProfileViewModel_Returns_Model_WithSameValues()
        {
            // Arrange
            var profileDetail = new ProfileDetails()
            {
                Id = 5,
                Name = "Test Profile",
                ContactUserId = 10
            };

            // Act
            var result = profileDetail.ToProfileViewModel();

            // Assert
            Assert.IsTrue(result.ContactUserId == profileDetail.ContactUserId &&
                          result.Id == profileDetail.Id &&
                          result.Name == profileDetail.Name);
        }

        [TestMethod]
        public void ToProfileDetails_Returns_Model_WithSameValues()
        {
            // Arrange
            var profileViewModel = new ProfileViewModel()
            {
                Id = 5,
                Name = "Test Profile",
                ContactUserId = 10,
            };

            // Act
            var result = profileViewModel.ToProfileDetails();

            // Assert
            Assert.IsTrue(result.ContactUserId == profileViewModel.ContactUserId &&
                          result.Id == profileViewModel.Id &&
                          result.Name == profileViewModel.Name);
        }

        [TestMethod]
        public void ToProfileDetails_Returns_Model_With_Correct_ArePDfs_Flag()
        {
            // Arrange
            var profileViewModel = new ProfileViewModel()
            {
                Id = 5,
                Name = "Test Profile",
                ContactUserId = 10,
                SelectedPdfAreaTypes = new List<ProfileAreaType>()
                {
                    new ProfileAreaType() {Id = 10, Value ="Test Area 1" },
                    new ProfileAreaType() {Id = 11, Value ="Test Area 2" },
                    new ProfileAreaType() {Id = 12, Value ="Test Area 3" }
                }
            };

            // Act
            var result = profileViewModel.ToProfileDetails();

            // Assert
            Assert.IsTrue(result.ArePdfs == true);
        }

        [TestMethod]
        public void ToProfileDetails_Returns_Model_With_UserPermissions()
        {
            // Arrange
            var profileViewModel = new ProfileViewModel()
            {
                Id = 5,
                Name = "Test Profile",
                ContactUserId = 10,
                ProfileUsers = new List<ProfileUser>()
                {
                    new ProfileUser() {Id = 10, Name ="Test Name 1" },
                    new ProfileUser() {Id = 11, Name ="Test Area 2" },
                    new ProfileUser() {Id = 12, Name ="Test Area 3" }
                }
            };

            // Act
            var result = profileViewModel.ToProfileDetails();

            // Assert
            Assert.IsTrue(result.UserPermissions != null &&
                          result.UserPermissions.Any() &&
                          result.UserPermissions.ToList()[0].UserId == profileViewModel.ProfileUsers.ToList()[0].Id);

        }

    }
}
