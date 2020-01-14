using System;
using System.Collections.Generic;
using Fpm.MainUI.Helpers;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fpm.MainUITest.Helpers
{
    [TestClass]
    public class RemoveIndicatorCheckerTest
    {
        private Mock<IProfilesReader> _profilesReader;

        // Ids of grouping row to be deleted
        private const int OwnerProfileId = 1;
        private const int AreaTypeId = 2;
        private const int GroupId = 3;
        private const int IndicatorId = 4;
        private const int SexId = 5;
        private const int AgeId = 6;

        // Alternative IDs
        private const int OtherProfileId = 91;
        private const int OtherAreaTypeId = 92;
        private const int OtherGroupId = 93;

        [TestInitialize]
        public void TestInitialize()
        {
            _profilesReader = new Mock<IProfilesReader>(MockBehavior.Strict);
        }

        [TestMethod]
        public void Test_Indicator_Can_Be_Removed_From_Profile_That_Does_Not_Own_The_Indicator()
        {
            Assert.IsTrue(CanIndicatorBeRemoved(OtherProfileId));
            VerifyAll();
        }

        [TestMethod]
        public void Test_Indicator_Can_Be_Removed_If_Would_Still_Be_In_Another_Domain()
        {
            // Arrange: GetGroupingByIndicatorIdAndSexIdAndAgeId to return groupings from two different domains
            var groupings = new List<Grouping>();
            var grouping1 = GetGrouping();
            groupings.Add(grouping1);
            var grouping2 = GetGrouping();
            grouping2.GroupId = OtherGroupId;
            groupings.Add(grouping2);

            _profilesReader.Setup(x => x.GetGroupingByIndicatorIdAndSexIdAndAgeId(IndicatorId, SexId, AgeId))
                .Returns(groupings);

            // Arrange: GetGroupingMetadataList to return two different domains
            var metadataList = new List<GroupingMetadata>
            {
                new GroupingMetadata{GroupId = GroupId, ProfileId = OwnerProfileId},
                new GroupingMetadata{GroupId = OtherGroupId, ProfileId = OwnerProfileId}
            };
            
            _profilesReader.Setup(x => x.GetGroupingMetadataList(It.IsAny<List<int>>()))
                .Returns(metadataList);
            
            // Assert
            Assert.IsTrue(CanIndicatorBeRemoved(OwnerProfileId));
            VerifyAll();
        }

        [TestMethod]
        public void Test_Indicator_Can_Be_Removed_If_Would_Still_Be_In_Another_Area_Type_Of_Same_Domain()
        {
            // Arrange: GetGroupingByIndicatorIdAndSexIdAndAgeId to return groupings from same domain but with different area type IDs
            var groupings = new List<Grouping>();
            var grouping1 = GetGrouping();
            groupings.Add(grouping1);
            var grouping2 = GetGrouping();
            grouping2.AreaTypeId = OtherAreaTypeId;
            groupings.Add(grouping2);

            _profilesReader.Setup(x => x.GetGroupingByIndicatorIdAndSexIdAndAgeId(IndicatorId, SexId, AgeId))
                .Returns(groupings);

            // Arrange: GetGroupingMetadataList to return one domain
            var metadataList = new List<GroupingMetadata>
            {
                new GroupingMetadata{GroupId = GroupId, ProfileId = OwnerProfileId}
            };

            _profilesReader.Setup(x => x.GetGroupingMetadataList(It.IsAny<List<int>>()))
                .Returns(metadataList);

            // Assert
            Assert.IsTrue(CanIndicatorBeRemoved(OwnerProfileId));
            VerifyAll();
        }

        [TestMethod]
        public void Test_Indicator_Cannot_Be_Removed_If_Is_Last_Grouping_In_Owner_Profile_And_It_Is_Used_In_Another_Profile()
        {
            // Arrange: GetGroupingByIndicatorIdAndSexIdAndAgeId to return groupings from different domains
            var groupings = new List<Grouping>{};
            var grouping1 = GetGrouping();
            groupings.Add(grouping1);
            var grouping2 = GetGrouping();
            grouping2.GroupId = OtherGroupId;
            groupings.Add(grouping2);

            _profilesReader.Setup(x => x.GetGroupingByIndicatorIdAndSexIdAndAgeId(IndicatorId, SexId, AgeId))
                .Returns(groupings);

            // Arrange: GetGroupingMetadataList to return two domains
            var metadataList = new List<GroupingMetadata>
            {
                new GroupingMetadata{GroupId = GroupId, ProfileId = OwnerProfileId},
                new GroupingMetadata{GroupId = OtherGroupId, ProfileId = OtherProfileId}
            };

            _profilesReader.Setup(x => x.GetGroupingMetadataList(It.IsAny<List<int>>()))
                .Returns(metadataList);

            // Assert
            Assert.IsFalse(CanIndicatorBeRemoved(OwnerProfileId));
            VerifyAll();
        }

        private static Grouping GetGrouping()
        {
            return new Grouping
            {
                IndicatorId = IndicatorId,
                SexId = SexId,
                AgeId = AgeId,
                AreaTypeId = AreaTypeId,
                GroupId = GroupId
            };
        }

        private bool CanIndicatorBeRemoved(int profileId)
        {
            var indicatorMetadata = new IndicatorMetadata {OwnerProfileId = OwnerProfileId};

            var groupingPlusName = new GroupingPlusName
            {
                IndicatorId = IndicatorId,
                SexId = SexId,
                AgeId = AgeId
            };

            var canRemove =
                new RemoveIndicatorChecker(_profilesReader.Object).CanIndicatorBeRemoved(profileId, indicatorMetadata,
                    groupingPlusName);

            return canRemove;
        }

        private void VerifyAll()
        {
            _profilesReader.VerifyAll();
        }
    }
}
