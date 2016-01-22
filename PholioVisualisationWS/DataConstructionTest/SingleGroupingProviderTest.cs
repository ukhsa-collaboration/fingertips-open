using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class SingleGroupingProviderTest
    {
        private int indicatorId = IndicatorIds.ChildrenInPoverty;
        private int sexId = SexIds.Persons;
        private int areaTypeId = AreaTypeIds.CountyAndUnitaryAuthority;
        private int ageId = AgeIds.Under16;

        [TestMethod]
        public void TestGetGroupingWhereGroupIdAppearsInAProfileNotUsingAreaTypeId()
        {
            var groupId = GroupIds.Phof_WiderDeterminantsOfHealth;

            var groupIdProvider = MockGroupIdProvider();
            groupIdProvider
                .Setup(x => x.GetGroupIds(ProfileIds.Phof))
                .Returns(new List<int> { groupId });

            var provider = new SingleGroupingProvider(ReaderFactory.GetGroupDataReader(),
                groupIdProvider.Object);

            var grouping = provider.GetGrouping(ProfileIds.Phof, 
                AreaTypeIds.CountyAndUnitaryAuthority,
                indicatorId, sexId, ageId);

            Assert.AreEqual(groupId, GroupIds.Phof_WiderDeterminantsOfHealth);
            Assert.AreEqual(indicatorId, grouping.IndicatorId);
            Assert.AreEqual(sexId, grouping.SexId);
            Assert.AreEqual(ageId, grouping.AgeId);
        }

        [TestMethod]
        public void TestGetGroupingWhereGroupIdAppearsInAProfile()
        {
            var groupId = GroupIds.Phof_WiderDeterminantsOfHealth;

            var provider = new SingleGroupingProvider(ReaderFactory.GetGroupDataReader(),
                GroupIdProviderThatWontBeUsed());
            var grouping = provider.GetGrouping(ProfileIds.Phof, groupId, areaTypeId, indicatorId, sexId, ageId);

            Assert.AreEqual(groupId, grouping.GroupId);
            AssertIdsAreSameAsRequested(grouping);
        }

        /// <summary>
        /// When the search results should be restricted to a specific profile
        /// </summary>
        [TestMethod]
        public void TestGetGroupingWhereGroupIdIsSearchAndProfileIdIsOfASpecificProfile()
        {
            var groupId = GroupIds.Search;
            var expectedGroupId = GroupIds.Phof_WiderDeterminantsOfHealth;

            var groupIdProvider = MockGroupIdProvider();
            groupIdProvider
                .Setup(x => x.GetGroupIds(ProfileIds.Phof))
                .Returns(new List<int> { expectedGroupId });

            var provider = new SingleGroupingProvider(ReaderFactory.GetGroupDataReader(),
                groupIdProvider.Object);
            var grouping = provider.GetGrouping(ProfileIds.Phof, groupId, areaTypeId, indicatorId, sexId, ageId);

            Assert.AreEqual(expectedGroupId, grouping.GroupId);
            AssertIdsAreSameAsRequested(grouping);
        }

        /// <summary>
        /// When the search results are not restricted to a specific profile.
        /// </summary>
        [TestMethod]
        public void TestGetGroupingWhereGroupIdIsSearchAndProfileIdIsUndefined()
        {
            var provider = new SingleGroupingProvider(ReaderFactory.GetGroupDataReader(), 
                new GroupIdProvider(ReaderFactory.GetProfileReader()));
            var grouping = provider.GetGrouping(ProfileIds.Undefined,
                GroupIds.Search, areaTypeId, indicatorId, sexId, ageId);

            AssertIdsAreSameAsRequested(grouping);
        }

        [TestMethod]
        public void TestWhenNoGroupingInPholioMatchesRequestedParameters()
        {
            var provider = new SingleGroupingProvider(ReaderFactory.GetGroupDataReader(), 
                new GroupIdProvider(ReaderFactory.GetProfileReader()));
            var grouping = provider.GetGrouping(ProfileIds.Undefined, 1, 2, 3, 4, 5);
            Assert.IsNull(grouping);
        }

        private static Mock<GroupIdProvider> MockGroupIdProvider()
        {
            var groupIdProvider = new Mock<GroupIdProvider>(MockBehavior.Strict);
            groupIdProvider.Protected();
            return groupIdProvider;
        }

        private void AssertIdsAreSameAsRequested(Grouping grouping)
        {
            Assert.AreEqual(indicatorId, grouping.IndicatorId);
            Assert.AreEqual(sexId, grouping.SexId);
            Assert.AreEqual(areaTypeId, grouping.AreaTypeId);
            Assert.AreEqual(ageId, grouping.AgeId);
        }

        private static GroupIdProvider GroupIdProviderThatWontBeUsed()
        {
            var groupIdProvider = new Mock<GroupIdProvider>(MockBehavior.Strict);
            groupIdProvider.Protected();
            var o = groupIdProvider.Object;
            return o;
        }
    }
}
