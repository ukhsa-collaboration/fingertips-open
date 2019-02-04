using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class SingleGroupingProviderTest
    {
        private int indicatorId = IndicatorIds.ChildrenInLowIncomeFamilies;
        private int sexId = SexIds.Persons;
        private int areaTypeId = AreaTypeIds.CountyAndUnitaryAuthority;
        private int ageId = AgeIds.Under16;

        private Mock<IGroupDataReader> _groupDataReader;

        [TestInitialize]
        public void TestInitialize()
        {
            _groupDataReader = new Mock<IGroupDataReader>(MockBehavior.Strict);
        }

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

            var grouping = provider.GetGroupingByProfileIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(ProfileIds.Phof,
                AreaTypeIds.CountyAndUnitaryAuthority,
                indicatorId, sexId, ageId);

            Assert.AreEqual(groupId, GroupIds.Phof_WiderDeterminantsOfHealth);
            Assert.AreEqual(indicatorId, grouping.IndicatorId);
            Assert.AreEqual(sexId, grouping.SexId);
            Assert.AreEqual(ageId, grouping.AgeId);

            VerifyAll();
        }

        [TestMethod]
        public void TestGetGroupingWhereGroupIdAppearsInAProfile()
        {
            var groupId = GroupIds.Phof_WiderDeterminantsOfHealth;

            var provider = new SingleGroupingProvider(ReaderFactory.GetGroupDataReader(),
                GroupIdProviderThatWontBeUsed());
            var grouping = provider.GetGroupingByProfileIdAndGroupIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(ProfileIds.Phof, groupId, areaTypeId, indicatorId, sexId, ageId);

            Assert.AreEqual(groupId, grouping.GroupId);
            AssertIdsAreSameAsRequested(grouping);

            VerifyAll();
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
            var grouping = provider.GetGroupingByProfileIdAndGroupIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(ProfileIds.Phof, groupId, areaTypeId, indicatorId, sexId, ageId);

            Assert.AreEqual(expectedGroupId, grouping.GroupId);
            AssertIdsAreSameAsRequested(grouping);

            VerifyAll();
        }

        /// <summary>
        /// When the search results are not restricted to a specific profile.
        /// </summary>
        [TestMethod]
        public void TestGetGroupingWhereGroupIdIsSearchAndProfileIdIsUndefined()
        {
            var provider = new SingleGroupingProvider(ReaderFactory.GetGroupDataReader(),
                new GroupIdProvider(ReaderFactory.GetProfileReader()));
            var grouping = provider.GetGroupingByProfileIdAndGroupIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(ProfileIds.Undefined,
                GroupIds.Search, areaTypeId, indicatorId, sexId, ageId);

            AssertIdsAreSameAsRequested(grouping);

            VerifyAll();
        }

        [TestMethod]
        public void TestGetGroupingByAreaTypeIdAndIndicatorIdAndSexIdAndAgeId()
        {
            var provider = new SingleGroupingProvider(ReaderFactory.GetGroupDataReader(),
                new GroupIdProvider(ReaderFactory.GetProfileReader()));
            var grouping = provider.GetGroupingByAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(areaTypeId, indicatorId, sexId, ageId);

            AssertIdsAreSameAsRequested(grouping);

            VerifyAll();
        }

        [TestMethod]
        public void TestWhenNoGroupingInPholioMatchesRequestedParameters()
        {
            var provider = new SingleGroupingProvider(ReaderFactory.GetGroupDataReader(),
                new GroupIdProvider(ReaderFactory.GetProfileReader()));
            var grouping = provider.GetGroupingByProfileIdAndGroupIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(ProfileIds.Undefined, 1, 2, 3, 4, 5);
            Assert.IsNull(grouping);

            VerifyAll();
        }

        [TestMethod]
        public void TestGetGroupingWithLatestDataPoint()
        {
            // Arrange
            _groupDataReader.Setup(x => x.GetGroupingsByGroupIdsAndIndicatorIds(It.IsAny<IList<int>>(),
                It.IsAny<IList<int>>())).Returns(GetGroupings());

            SetUpGetCommonestPolarityForIndicator();

            // Act: Get the latest grouping
            var grouping = new SingleGroupingProvider(_groupDataReader.Object, null)
                .GetGroupingWithLatestDataPoint(new List<int> { 1 }, 2, 3, 4);

            // Assert: most recent year
            Assert.AreEqual(2002, grouping.DataPointYear);

            VerifyAll();
        }

        private void SetUpGetCommonestPolarityForIndicator()
        {
            _groupDataReader.Setup(x => x.GetCommonestPolarityForIndicator(It.Is<int>(i => i == indicatorId)))
                .Returns(PolarityIds.RagHighIsGood);
        }

        [TestMethod]
        public void Test_GetGroupingByAreaTypeIdAndIndicatorIdAndSexIdAndAgeId_WithLatestDataPoint()
        {
            // Arrange
            _groupDataReader.Setup(x => x.GetGroupingByAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(GetGroupings());

            SetUpGetCommonestPolarityForIndicator();

            // Act: Get the latest grouping
            var grouping = new SingleGroupingProvider(_groupDataReader.Object, null)
                .GetGroupingByAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(1, 2, 3, 4);

            // Assert: most recent year
            Assert.AreEqual(2002, grouping.DataPointYear);

            VerifyAll();
        }

        [TestMethod]
        public void TestGetGroupingWithLatestDataPointForAnyProfile()
        {
            // Arrange
            const int childAreaTypeId = AreaTypeIds.AcuteTrust;
            const int unmatchedId = 1;

            var groupingDifferentiator = new GroupingDifferentiator
            {
                IndicatorId = indicatorId,
                AgeId = 3,
                SexId = 4
            };

            _groupDataReader.Setup(x => x.GetGroupingsByIndicatorId(groupingDifferentiator.IndicatorId)).Returns(new List<Grouping>
                {
                    new Grouping {IndicatorId = indicatorId, AreaTypeId = childAreaTypeId, SexId = groupingDifferentiator.SexId, AgeId = groupingDifferentiator.AgeId},
                    new Grouping {IndicatorId = indicatorId, AreaTypeId = unmatchedId, SexId = groupingDifferentiator.SexId, AgeId = groupingDifferentiator.AgeId},
                    new Grouping {IndicatorId = indicatorId, AreaTypeId = childAreaTypeId, SexId = unmatchedId, AgeId = groupingDifferentiator.AgeId},
                    new Grouping {IndicatorId = indicatorId, AreaTypeId = childAreaTypeId, SexId = groupingDifferentiator.SexId, AgeId = unmatchedId}
                });

            SetUpGetCommonestPolarityForIndicator();

            // Act: Get the latest grouping
            var grouping = new SingleGroupingProvider(_groupDataReader.Object, null)
                .GetGroupingWithLatestDataPointForAnyProfile(groupingDifferentiator, AreaTypeIds.AcuteTrust);

            // Assert: most recent year
            Assert.AreEqual(childAreaTypeId, grouping.AreaTypeId);
            Assert.AreEqual(groupingDifferentiator.AgeId, grouping.AgeId);
            Assert.AreEqual(groupingDifferentiator.SexId, grouping.SexId);

            VerifyAll();
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

        private void VerifyAll()
        {
            _groupDataReader.VerifyAll();
        }

        private IList<Grouping> GetGroupings()
        {
            return new List<Grouping>
            {
                new Grouping {IndicatorId = indicatorId, DataPointYear = 2000, AreaTypeId = 3},
                new Grouping {IndicatorId = indicatorId,DataPointYear = 2002, AreaTypeId = 3},
                new Grouping {IndicatorId = indicatorId,DataPointYear = 2001, AreaTypeId = 3}
            };
        }
    }
}
