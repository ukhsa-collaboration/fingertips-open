using Fpm.MainUI.Helpers;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace Fpm.MainUITest.Helpers
{
    [TestClass]
    public class GroupingRemoverTest
    {
        private Mock<IProfilesReader> _profilesReader;
        private Mock<IProfileRepository> _profileRepository;

        private const string UserName = "phe\\doris.hain";

        [TestInitialize]
        public void TestInitialize()
        {
            _profilesReader = new Mock<IProfilesReader>(MockBehavior.Strict);
            _profileRepository = new Mock<IProfileRepository>(MockBehavior.Strict);
        }

        [TestMethod]
        public void Test_Remove_Groupings_Non_Owner_Profile()
        {
            // Arrange
            var profileId = 1;
            var otherProfileId = 91;
            var groupId = 2;
            var indicatorId = 3;
            var areaTypeId = 4;
            var sexId = 5;
            var ageId = 6;

        var indicatorMetadata = new IndicatorMetadata()
            {
                IndicatorId =  indicatorId,
                OwnerProfileId = otherProfileId
            };
            _profilesReader.Setup(x => x.GetIndicatorMetadata(indicatorId)).Returns(indicatorMetadata);

            _profileRepository.Setup(x =>
                x.DeleteIndicatorFromGrouping(groupId, indicatorId, areaTypeId, sexId, ageId));

            _profileRepository.Setup(x => x.DeleteOverriddenMetadataTextValues(indicatorId, profileId));

            var auditMessage =
                string.Format(
                    "Indicator {0} (ProfileId: {1}, GroupId: {2}, AreaTypeId: {3}, SexId: {4}, AgeId: {5} has been deleted.",
                    indicatorId, profileId, groupId, areaTypeId, sexId, ageId);


            _profileRepository.Setup(x => x.LogAuditChange(auditMessage, indicatorId, groupId, UserName, It.IsAny<DateTime>(), 
                CommonUtilities.AuditType.Delete.ToString())).Returns(true);


            // Act
            new GroupingRemover(_profilesReader.Object, _profileRepository.Object).RemoveGroupings(profileId, groupId, indicatorId, areaTypeId, sexId, ageId);

            // Assert
            VerifyAll();
        }

        [TestMethod]
        public void Test_Remove_Groupings_Owner_Profile()
        {
            // Arrange
            var profileId = ProfileIds.Phof;
            var groupId = GroupIds.PhofOverarchingIndicators;
            var indicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth;
            var areaTypeId = AreaTypeIds.GoRegion;
            var sexId = SexIds.Male;
            var ageId = AgeIds.AllAges;

            var indicatorMetadata = new IndicatorMetadata()
            {
                IndicatorId = indicatorId,
                OwnerProfileId = profileId
            };

            _profilesReader.Setup(x => x.GetIndicatorMetadata(indicatorId)).Returns(indicatorMetadata);

            _profileRepository.Setup(x =>
                x.DeleteIndicatorFromGrouping(groupId, indicatorId, areaTypeId, sexId, ageId));

            _profileRepository.Setup(x => x.DeleteOverriddenMetadataTextValues(indicatorId, profileId));

            var auditMessage =
                string.Format(
                    "Indicator {0} (ProfileId: {1}, GroupId: {2}, AreaTypeId: {3}, SexId: {4}, AgeId: {5} has been deleted.",
                    indicatorId, profileId, groupId, areaTypeId, sexId, ageId);

            _profileRepository.Setup(x => x.LogAuditChange(auditMessage, indicatorId, groupId, UserName,
                It.IsAny<DateTime>(),
                CommonUtilities.AuditType.Delete.ToString())).Returns(true);


            // Act
            new GroupingRemover(_profilesReader.Object, _profileRepository.Object).RemoveGroupings(profileId, groupId, indicatorId, areaTypeId, sexId, ageId);

            // Assert
            VerifyAll();
        }

        private void VerifyAll()
        {
            _profilesReader.VerifyAll();
            _profileRepository.VerifyAll();
        }
    }
}