using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class GroupingListProviderTest
    {
        private int areaTypeId = 1;
        private List<int> profileIds = new List<int> { 2, 3 };
        private List<int> indicatorIds = new List<int> { 4, 5 };
        private List<int> groupIds = new List<int> { 6, 7 };

        [TestMethod]
        public void TestGetGroupings()
        {
            var groupDataReader = GroupDataReader(new List<Grouping>());
            var profileReader = ProfileReader();

            var groupings = new GroupingListProvider(groupDataReader.Object, profileReader.Object)
                 .GetGroupings(profileIds, indicatorIds, areaTypeId);

            // Only 1 grouping ensures Groupings have been uniquified
            Assert.IsNotNull(groupings);

            groupDataReader.Verify(x => x.GetGroupingsByGroupIdsAndIndicatorIdsAndAreaType(
                groupIds, indicatorIds, areaTypeId));

            profileReader.Verify((x => x.GetGroupIdsFromSpecificProfiles(profileIds)));
        }

        [TestMethod]
        public void TestGetGroupingsUniquifiesGroupings()
        {
            var groupingsFromReader = new List<Grouping>
            {
                new Grouping {IndicatorId = 4},
                new Grouping {IndicatorId = 4}
            };

            var groupings = new GroupingListProvider(GroupDataReader(groupingsFromReader).Object, ProfileReader().Object)
                 .GetGroupings(profileIds, indicatorIds, areaTypeId);

            Assert.AreEqual(1, groupings.Count);
        }

        private Mock<ProfileReader> ProfileReader()
        {
            var profileReader = new Mock<ProfileReader>();
            profileReader.Setup(x => x.GetGroupIdsFromSpecificProfiles(profileIds))
                .Returns(groupIds);
            return profileReader;
        }

        private Mock<GroupDataReader> GroupDataReader(IList<Grouping> groupings)
        {
            var groupDataReader = new Mock<GroupDataReader>();
            groupDataReader.Setup(x => x.GetGroupingsByGroupIdsAndIndicatorIdsAndAreaType(
                groupIds, indicatorIds, areaTypeId))
                .Returns(groupings);
            return groupDataReader;
        }
    }
}
