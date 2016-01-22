using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Profile;
using FingertipsUploadService.ProfileData.Entities.User;
using FingertipsUploadService.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProfileDataTest.Respositories
{
    [TestClass]
    public class WhenUsingProfileRepository
    {
        private ProfileRepository _profileRepository;

        [TestInitialize]
        public void Init()
        {
            _profileRepository = new ProfileRepository();
        }

        [TestCleanup]
        public void CleanUp()
        {
            _profileRepository.Dispose();
        }

        [TestMethod]
        public void GetProfiles_Returns_NonZeroList()
        {
            var result = _profileRepository.GetProfiles();

            Assert.IsTrue(result.Count > 0);

            Assert.IsTrue(result.Count(p => p.Id == ProfileIds.Search) == 0);
        }

        [TestMethod]
        public void TestCreateProfile()
        {
            var name = GuidSortedLast();
            var reader = ReaderFactory.GetProfilesWriter();
            var user1 = reader.GetUserByUserId(FpmUserIds.Doris);
            var user2 = reader.GetUserByUserId(FpmUserIds.FarrukhAyub);

            var userPermissions = new List<UserGroupPermissions>()
            {
                new UserGroupPermissions {FpmUser = user1, UserId = user1.Id},
                new UserGroupPermissions {FpmUser = user2, UserId = user2.Id}
            };

            var profileId = _profileRepository.CreateProfile(new ProfileDetails
            {
                Name = name,
                UrlKey = name,
                SkinId = SkinIds.Core,
                ExtraJsFiles = string.Empty,
                ExtraCssFiles = string.Empty,
                AreasIgnoredEverywhere = string.Empty,
                AreasIgnoredForSpineChart = string.Empty,
                UserPermissions = userPermissions
            });

            var details = reader.GetProfileDetailsByProfileId(profileId);

            Assert.AreEqual(name, details.Name);
        }

        [TestMethod]
        public void TestUpdateProfile()
        {
            var reader = ReaderFactory.GetProfilesReader();

            var details = reader.GetProfileDetails(
                UrlKeys.SevereMentalIllness
                /*a profile we can change which won't interfere with tests in other solutions*/
                );

            string name = GuidSortedLast();
            details.Name = name;

            string areasIgnoredEverywhere = Guid.NewGuid().ToString();
            details.AreasIgnoredEverywhere = areasIgnoredEverywhere;

            string areasIgnoredForSpineChart = Guid.NewGuid().ToString();
            details.AreasIgnoredForSpineChart = areasIgnoredForSpineChart;

            string accessControlGroup = Guid.NewGuid().ToString();
            details.AccessControlGroup = accessControlGroup;

            bool isLive = !details.IsLive;
            details.IsLive = isLive;

            bool arePdfs = !details.ArePdfs;
            details.ArePdfs = arePdfs;

            bool showDataQuality = !details.ShowDataQuality;
            details.ShowDataQuality = showDataQuality;

            bool isNational = !details.IsNational;
            details.IsNational = isNational;

            bool hasOwnFrontPage = !details.HasOwnFrontPage;
            details.HasOwnFrontPage = hasOwnFrontPage;

            bool areIndicatorsExcludedFromSearch = !details.AreIndicatorsExcludedFromSearch;
            details.AreIndicatorsExcludedFromSearch = areIndicatorsExcludedFromSearch;

            int keyColourId = details.KeyColourId == 0 ? 1 : 0;
            details.KeyColourId = keyColourId;

            int defaultAreaTypeId = details.DefaultAreaTypeId == 0 ? 1 : 0;
            details.DefaultAreaTypeId = defaultAreaTypeId;

            bool startZeroYAxis = !details.StartZeroYAxis;
            details.StartZeroYAxis = startZeroYAxis;

            int defaultFingertipsTabId = details.DefaultFingertipsTabId;
            details.DefaultFingertipsTabId = defaultFingertipsTabId;

            _profileRepository.UpdateProfile(details);
            reader.ClearCache();

            // Check changes have been persisted
            details = reader.GetProfileDetails(UrlKeys.SevereMentalIllness);
            Assert.AreEqual(name, details.Name);
            Assert.AreEqual(isLive, details.IsLive);
            Assert.AreEqual(arePdfs, details.ArePdfs);
            Assert.AreEqual(showDataQuality, details.ShowDataQuality);
            Assert.AreEqual(isNational, details.IsNational);
            Assert.AreEqual(hasOwnFrontPage, details.HasOwnFrontPage);
            Assert.AreEqual(areIndicatorsExcludedFromSearch, details.AreIndicatorsExcludedFromSearch);
            Assert.AreEqual(keyColourId, details.KeyColourId);
            Assert.AreEqual(defaultAreaTypeId, details.DefaultAreaTypeId);
            Assert.AreEqual(areasIgnoredForSpineChart, details.AreasIgnoredForSpineChart);
            Assert.AreEqual(areasIgnoredEverywhere, details.AreasIgnoredEverywhere);
            Assert.AreEqual(accessControlGroup, details.AccessControlGroup);
            Assert.AreEqual(startZeroYAxis, details.StartZeroYAxis);
        }

        [TestMethod]
        public void TestUpdateProfile_ExtraFilesAreUpdated()
        {
            var reader = ReaderFactory.GetProfilesReader();
            var details = reader.GetProfileDetails(UrlKeys.HealthProfiles);

            string extraCssFiles = Guid.NewGuid().ToString();
            details.ExtraCssFiles = extraCssFiles;

            string extraJsFiles = Guid.NewGuid().ToString();
            details.ExtraJsFiles = extraJsFiles;

            _profileRepository.UpdateProfile(details);
            reader.ClearCache();

            // Check changes have been persisted
            details = reader.GetProfileDetails(UrlKeys.HealthProfiles);
            Assert.AreEqual(extraJsFiles, details.ExtraJsFiles);
            Assert.AreEqual(extraCssFiles, details.ExtraCssFiles);
        }

        [TestMethod]
        public void TestUpdateProfile_ExtraFilesCanBeUpdatedWithNulls()
        {
            var reader = ReaderFactory.GetProfilesReader();
            var details = reader.GetProfileDetails(UrlKeys.HealthProfiles);

            details.ExtraCssFiles = null;
            details.ExtraJsFiles = null;

            _profileRepository.UpdateProfile(details);
            reader.ClearCache();

            // Check changes have been persisted
            details = reader.GetProfileDetails(UrlKeys.HealthProfiles);
            Assert.IsNull(details.ExtraJsFiles);
            Assert.IsNull(details.ExtraCssFiles);
        }

        [TestMethod]
        public void TestLogAuditChange()
        {
            //            var reader = ReaderFactory.GetProfilesReader();
            //            const int indicatorId = 999999999;
            //
            //            //Insert the change audit record
            //            _profileRepository.LogAuditChange("Test - Move Audit Message", indicatorId, 1, "TestLogAuditChange",
            //                DateTime.Now, CommonUtilities.AuditType.Move.ToString());
            //
            //            //Read the change audit record back
            //            Assert.IsTrue(reader.GetIndicatorAudit(new List<int> { indicatorId }).Any());
            //
            //            //Delete the new change audit record
            //            _profileRepository.DeleteChangeAudit(indicatorId);
            //
            //            //Read the change audit record back
            //            Assert.IsFalse(reader.GetIndicatorAudit(new List<int> { indicatorId }).Any());
        }

        [TestMethod]
        public void CreateIndicator_Creates_New_Indicator()
        {
            // Arrange
            var reader = ReaderFactory.GetProfilesReader();

            var indicatorProperties = reader.GetIndicatorMetadataTextProperties();

            var nextIndicatorId = _profileRepository.GetNextIndicatorId();

            foreach (var indicatorMetadataTextProperty in indicatorProperties)
            {
                indicatorMetadataTextProperty.Text = "a";
            }

            // Act
            _profileRepository.CreateIndicator(indicatorProperties, nextIndicatorId);

            // Assert
            var updated = reader.GetIndicatorMetadataTextProperties();

            for (var i = 0; i < updated.Count; i++)
            {
                Assert.AreEqual(updated[i].Text, indicatorProperties[i].Text);
            }
        }

        [TestMethod]
        public void CreateProfileCollection_Creates_a_new_Collection()
        {
            // Arrange
            const string assignedProfiles = "8,53";
            var randomNumber = new Random().Next();
            var profileCollection = new ProfileCollection()
            {
                CollectionName = "Test Collection " + randomNumber,
                CollectionSkinTitle = "Test Skin title" + randomNumber
            };

            // Act
            var result = _profileRepository.CreateProfileCollection(profileCollection, assignedProfiles);

            // Assert
            Assert.IsTrue(result > 1);
        }

        [TestMethod]
        public void CreateProfileCollection_Creates_a_new_Collection_For_a_SingleProfile()
        {
            // Arrange
            const string assignedProfiles = "53";
            var randomNumber = new Random().Next();
            var profileCollection = new ProfileCollection()
            {
                CollectionName = "Test Collection " + randomNumber,
                CollectionSkinTitle = "Test Skin title" + randomNumber
            };

            // Act
            var result = _profileRepository.CreateProfileCollection(profileCollection, assignedProfiles);

            // Assert
            Assert.IsTrue(result > 1);
        }

        [TestMethod]
        public void CreateProfileCollection_Creates_EmptyCollection_For_NoAssignedProfiles_()
        {
            // Arrange
            const string assignedProfiles = "";
            var randomNumber = new Random().Next();
            var profileCollection = new ProfileCollection()
            {
                CollectionName = "Test Collection " + randomNumber,
                CollectionSkinTitle = "Test Skin title" + randomNumber
            };

            // Act
            var result = _profileRepository.CreateProfileCollection(profileCollection, assignedProfiles);

            // Assert
            Assert.IsTrue(result > 1);
        }

        [TestMethod]
        public void UpdateProfileCollection_Update_a_Collection()
        {
            // Arrange
            const string assignedProfilesToInsert = @"53,39";
            const string assignedProfilesToUpdate = @"53~true,39~false,72~false";

            var randomNumber = new Random().Next();

            var profileCollectionToInsert = new ProfileCollection()
            {
                CollectionName = "Test Collection " + randomNumber,
                CollectionSkinTitle = "Test Skin" + randomNumber
            };


            var collectionNameToUpdate = "Test Update Collection " + randomNumber;
            var collectionSkinTitleToUpdate = "Test Update Skin" + randomNumber;

            // Act
            var profileCollectionid = _profileRepository.CreateProfileCollection(profileCollectionToInsert,
              assignedProfilesToInsert);
            _profileRepository.Dispose();

            _profileRepository.OpenSession();


            var result = _profileRepository.UpdateProfileCollection(profileCollectionid, assignedProfilesToUpdate,
                collectionNameToUpdate, collectionSkinTitleToUpdate);

            // Assert
            Assert.IsTrue(result == true);
        }

        [TestMethod]
        public void ChangeOwner_Transfers_Ownership_Successfully()
        {
            // Arrange
            var reader = ReaderFactory.GetProfilesReader();

            const int indicatorId = IndicatorIds.IDAOPI;
            const int newOwnerProfileId = ProfileIds.Diabetes;

            var currentOwnerProfile = reader.GetIndicatorMetadata(indicatorId).OwnerProfileId;
            var properties = reader.GetIndicatorMetadataTextProperties();

            var newOwnerIMDTVs = reader.GetOverriddenIndicatorTextValuesForSpecificProfileId(indicatorId, properties, newOwnerProfileId);

            //Get IndicatorMetadataTextValues where indicatorId = Null (currentOwner)
            var currentOwnerIMDTVs = reader.GetIndicatorTextValues(indicatorId, properties, currentOwnerProfile);

            // Act
            _profileRepository.ChangeOwner(indicatorId, newOwnerProfileId,
                newOwnerIMDTVs, currentOwnerIMDTVs);

            // Assert
            var indicatorMetadataChanged = reader.GetIndicatorMetadata(indicatorId);
            Assert.AreEqual(newOwnerProfileId, indicatorMetadataChanged.OwnerProfileId);
        }

        [TestMethod]
        public void GetNextIndicatorId_Returns_NextId()
        {
            var result = _profileRepository.GetNextIndicatorId();

            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void LogPropertyChange_Creates_Logs()
        {
            var result = _profileRepository.LogPropertyChange(10, "Test", 10, 10, "Test user", DateTime.Now);

            Assert.IsTrue(result == true);
        }

        [TestMethod]
        public void LogAuditChange_Creates_Logs()
        {
            var result = _profileRepository.LogAuditChange("Test message", 10, 10, "test user", DateTime.Now, "test audit type");

            Assert.IsTrue(result == true);
        }

        private static string GuidSortedLast()
        {
            return "z" + Guid.NewGuid();
        }
    }
}
