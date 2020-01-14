using Fpm.MainUI.Helpers;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fpm.ProfileDataTest.Repositories
{
    [TestClass]
    public class WhenUsingProfileRepository
    {
        private ProfilesReader _reader;
        private ProfileRepository _profileRepository;
        private string _subheadingGuid;

        /*a profile we can change which won't interfere with tests in other solutions*/
        private const string ProfileUrlKey = UrlKeys.SevereMentalIllness;

        [TestInitialize]
        public void Init()
        {
            _reader = ReaderFactory.GetProfilesReader();
            _profileRepository = new ProfileRepository();
            _subheadingGuid = Guid.NewGuid().ToString();
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

            var contactUserIds = String.Format("{0},{1}", user1.Id.ToString(), user2.Id.ToString());

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
                UserPermissions = userPermissions,
                ContactUserIds = contactUserIds,
                ShowOfficialStatistic = true
            });

            var details = reader.GetProfileDetailsByProfileId(profileId);

            Assert.AreEqual(name, details.Name);
        }

        [TestMethod]
        public void TestUpdateProfile()
        {
            var reader = ReaderFactory.GetProfilesReader();

            var details = reader.GetProfileDetails(ProfileUrlKey);

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

            bool newShowDataOfficialStatisticValue = !details.ShowOfficialStatistic;
            details.ShowOfficialStatistic = newShowDataOfficialStatisticValue;

            bool isNational = !details.IsNational;
            details.IsNational = isNational;

            bool hasOwnFrontPage = !details.HasOwnFrontPage;
            details.HasOwnFrontPage = hasOwnFrontPage;

            bool areIndicatorsExcludedFromSearch = !details.AreIndicatorsExcludedFromSearch;
            details.AreIndicatorsExcludedFromSearch = areIndicatorsExcludedFromSearch;

            int defaultAreaTypeId = details.DefaultAreaTypeId == 0 ? 1 : 0;
            details.DefaultAreaTypeId = defaultAreaTypeId;

            bool startZeroYAxis = !details.StartZeroYAxis;
            details.StartZeroYAxis = startZeroYAxis;

            int defaultFingertipsTabId = details.DefaultFingertipsTabId;
            details.DefaultFingertipsTabId = defaultFingertipsTabId;

            _profileRepository.UpdateProfile(details);
            reader.ClearCache();

            // Check changes have been persisted
            details = reader.GetProfileDetails(ProfileUrlKey);
            Assert.AreEqual(name, details.Name);
            Assert.AreEqual(isLive, details.IsLive);
            Assert.AreEqual(arePdfs, details.ArePdfs);
            Assert.AreEqual(showDataQuality, details.ShowDataQuality);
            Assert.AreEqual(isNational, details.IsNational);
            Assert.AreEqual(hasOwnFrontPage, details.HasOwnFrontPage);
            Assert.AreEqual(areIndicatorsExcludedFromSearch, details.AreIndicatorsExcludedFromSearch);
            Assert.AreEqual(defaultAreaTypeId, details.DefaultAreaTypeId);
            Assert.AreEqual(areasIgnoredForSpineChart, details.AreasIgnoredForSpineChart);
            Assert.AreEqual(areasIgnoredEverywhere, details.AreasIgnoredEverywhere);
            Assert.AreEqual(accessControlGroup, details.AccessControlGroup);
            Assert.AreEqual(startZeroYAxis, details.StartZeroYAxis);
            Assert.AreEqual(newShowDataOfficialStatisticValue, details.ShowOfficialStatistic);
        }

        [TestMethod]
        public void TestUpdateProfile_ExtraFilesAreUpdated()
        {
            var reader = ReaderFactory.GetProfilesReader();
            var details = reader.GetProfileDetails(ProfileUrlKey);

            string extraCssFiles = Guid.NewGuid().ToString();
            details.ExtraCssFiles = extraCssFiles;

            string extraJsFiles = Guid.NewGuid().ToString();
            details.ExtraJsFiles = extraJsFiles;

            _profileRepository.UpdateProfile(details);
            reader.ClearCache();

            // Check changes have been persisted
            details = reader.GetProfileDetails(ProfileUrlKey);
            Assert.AreEqual(extraJsFiles, details.ExtraJsFiles);
            Assert.AreEqual(extraCssFiles, details.ExtraCssFiles);
        }

        [TestMethod]
        public void TestUpdateProfile_ExtraFilesCanBeUpdatedWithNulls()
        {
            var reader = ReaderFactory.GetProfilesReader();
            var details = reader.GetProfileDetails(ProfileUrlKey);

            details.ExtraCssFiles = null;
            details.ExtraJsFiles = null;

            _profileRepository.UpdateProfile(details);
            reader.ClearCache();

            // Check changes have been persisted
            details = reader.GetProfileDetails(ProfileUrlKey);
            Assert.IsNull(details.ExtraJsFiles);
            Assert.IsNull(details.ExtraCssFiles);
        }

        [TestMethod]
        public void TestLogAuditChange()
        {
            var reader = ReaderFactory.GetProfilesReader();
            const int indicatorId = 999999999;

            //Insert the change audit record
            _profileRepository.LogAuditChange("Test - Move Audit Message", indicatorId, 1, "TestLogAuditChange",
                DateTime.Now, CommonUtilities.AuditType.Move.ToString());

            //Read the change audit record back
            Assert.IsTrue(reader.GetIndicatorAudit(new List<int> { indicatorId }).Any());

            //Delete the new change audit record
            _profileRepository.DeleteChangeAudit(indicatorId);

            //Read the change audit record back
            Assert.IsFalse(reader.GetIndicatorAudit(new List<int> { indicatorId }).Any());
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
            var assignedProfiles = ProfileIds.DevelopmentProfileForTesting + "," + ProfileIds.Phof;
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
            var assignedProfiles = ProfileIds.DevelopmentProfileForTesting.ToString();
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
            var assignedProfilesToInsert = ProfileIds.Phof + "," + ProfileIds.SexualHealth;
            var assignedProfilesToUpdate = ProfileIds.Phof + "~true," + ProfileIds.SexualHealth + "~false";
            var randomNumber = new Random().Next();

            // Act: create the profile collection
            var profileCollectionToInsert = new ProfileCollection()
            {
                CollectionName = "Test Collection " + randomNumber,
                CollectionSkinTitle = "Test Skin" + randomNumber
            };
            var profileCollectionId = _profileRepository.CreateProfileCollection(profileCollectionToInsert,
              assignedProfilesToInsert);

            // Refresh repo to be sure that the insert has been done
            _profileRepository.Dispose();
            _profileRepository.OpenSession();

            // Act: update the profile collection
            var collectionNameToUpdate = "Test Update Collection " + randomNumber;
            var collectionSkinTitleToUpdate = "Test Update Skin" + randomNumber;
            var result = _profileRepository.UpdateProfileCollection(profileCollectionId, assignedProfilesToUpdate,
                collectionNameToUpdate, collectionSkinTitleToUpdate);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ChangeOwner_Transfers_Ownership_Successfully()
        {
            // Arrange
            var reader = ReaderFactory.GetProfilesReader();

            const int indicatorId = IndicatorIds.ChildrenInPoverty;
            var ownerProfileId = reader.GetIndicatorMetadata(indicatorId).OwnerProfileId;

            // Select a different profile ID to change to
            var newOwnerProfileId = reader.GetProfiles().Select(x => x.Id)
                .First(x => x != ownerProfileId);

            var indicatorMetadataTextProperties = reader.GetIndicatorMetadataTextProperties();
            var newOwnerIMDTVs = reader.GetOverriddenIndicatorTextValuesForSpecificProfileId(
                indicatorId, indicatorMetadataTextProperties, newOwnerProfileId);

            // Get IndicatorMetadataTextValues where indicatorId = Null (currentOwner)
            var currentOwnerIMDTVs = reader.GetIndicatorTextValues(indicatorId,
                indicatorMetadataTextProperties, ownerProfileId);

            // Act: Change owner
            _profileRepository.ChangeOwner(indicatorId, newOwnerProfileId,
                newOwnerIMDTVs, currentOwnerIMDTVs);

            // Assert: Check owner has been changed (must use new reader to avoid getting stale object)
            var indicatorMetadataChanged = ReaderFactory.GetProfilesReader().GetIndicatorMetadata(indicatorId);
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
            var result = _profileRepository.LogIndicatorMetadataTextPropertyChange(10, "Test", 10, 10, "Test user", DateTime.Now);

            Assert.IsTrue(result == true);
        }

        [TestMethod]
        public void LogAuditChange_Creates_Logs()
        {
            var result = _profileRepository.LogAuditChange("Test message", 10, 10, "test user", DateTime.Now, "test audit type");

            Assert.IsTrue(result == true);
        }

        [TestMethod]
        public void Get_Domain_Name()
        {
            var domainName = _profileRepository.GetDomainName(GroupIds.DevelopmentProfileForTesting_Domain1, 1);
            Assert.IsTrue(domainName.ToLower().StartsWith("domain 1"));
        }

        [TestMethod]
        public void Get_Grouping_Plus_Names()
        {
            Profile profile = new ProfileBuilder(_reader, _profileRepository).Build(UrlKeys.DevelopmentProfileForTesting, 1,
                AreaTypeIds.DistrictAndUnitaryAuthorityPre2019);
            Assert.IsTrue(profile.IndicatorNames.Any());
        }

        [TestMethod]
        public void Get_Grouping_Subheadings()
        {
            Assert.IsTrue(_profileRepository.GetGroupingSubheadings(
                AreaTypeIds.DistrictAndUnitaryAuthorityPre2019, GroupIds.DevelopmentProfileForTesting_Domain1).Any());
        }

        [TestMethod]
        public void Get_All_Grouping_Subheadings()
        {
            Assert.IsNotNull(_profileRepository.GetGroupingSubheadingsByGroupIds(new List<int>
            {
                GroupIds.DevelopmentProfileForTesting_Domain1
            }));
        }

        [TestMethod]
        public void Add_Grouping_Subheadings()
        {
            var groupingSubheading = new GroupingSubheading
            {
                GroupId = GroupIds.DevelopmentProfileForTesting_Domain1,
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPre2019,
                Sequence = 1,
                Subheading = _subheadingGuid
            };

            _profileRepository.SaveGroupingSubheading(groupingSubheading);

            var groupingSubheadings = _profileRepository.GetGroupingSubheadings(AreaTypeIds.CountyAndUnitaryAuthorityPre2019,
                GroupIds.DevelopmentProfileForTesting_Domain1);

            Assert.AreEqual(_subheadingGuid, groupingSubheadings.FirstOrDefault(x => x.Subheading == _subheadingGuid).Subheading);
        }

        [TestMethod]
        public void Edit_Grouping_Subheadings()
        {
            var groupingSubheading = new GroupingSubheading
            {
                GroupId = GroupIds.DevelopmentProfileForTesting_Domain1,
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPre2019,
                Sequence = 1,
                Subheading = _subheadingGuid
            };

            _profileRepository.SaveGroupingSubheading(groupingSubheading);

            var newSubheadingGuid = Guid.NewGuid().ToString();

            groupingSubheading.SubheadingId = _profileRepository
                .GetGroupingSubheadings(AreaTypeIds.CountyAndUnitaryAuthorityPre2019, GroupIds.DevelopmentProfileForTesting_Domain1)
                .FirstOrDefault(x => x.Subheading == groupingSubheading.Subheading).SubheadingId;
            groupingSubheading.Subheading = newSubheadingGuid;

            _profileRepository.UpdateGroupingSubheading(groupingSubheading);

            _profileRepository.RefreshObject(groupingSubheading);

            Assert.AreEqual(newSubheadingGuid,
                GetGroupingSubheadings(AreaTypeIds.CountyAndUnitaryAuthorityPre2019, GroupIds.DevelopmentProfileForTesting_Domain1)
                    .FirstOrDefault(x => x.Subheading == newSubheadingGuid).Subheading);
        }

        [TestMethod]
        public void Move_Indicators()
        {
            var indicatorId = IndicatorIds.BackPainPrevalence;
            var fromGroupId = GroupIds.KeyIndicators;
            var toGroupId = GroupIds.UnderReview;
            var areaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPre2019;
            var sexId = SexIds.Persons;
            var ageId = AgeIds.AllAges;

            // Move to under review status
            _profileRepository.MoveIndicators(indicatorId, fromGroupId, toGroupId, areaTypeId, sexId, ageId, IndicatorStatus.UnderReview);

            var profilesReader = ReaderFactory.GetProfilesReader();
            var groupings = profilesReader.GetGroupings(new List<int> { toGroupId }, areaTypeId, indicatorId, sexId, ageId, 1);

            // Test
            Assert.IsTrue(groupings.Any());

            // Awaiting revision status
            _profileRepository.MoveIndicators(indicatorId, toGroupId, fromGroupId, areaTypeId, sexId, ageId, IndicatorStatus.ChangesRequested);
        }

        private static string GuidSortedLast()
        {
            return "z" + Guid.NewGuid();
        }

        private IList<GroupingSubheading> GetGroupingSubheadings(int areaTypeId, int groupId)
        {
            return _profileRepository.GetGroupingSubheadings(areaTypeId, groupId);
        }
    }
}