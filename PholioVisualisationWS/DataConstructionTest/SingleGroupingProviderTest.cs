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
        private const int IndicatorId = IndicatorIds.ChildrenInLowIncomeFamilies;
        private const int SexId = SexIds.Persons;
        private const int AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019;
        private const int AgeId = AgeIds.Under16;

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
                AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019,
                IndicatorId, SexId, AgeId);

            Assert.AreEqual(groupId, GroupIds.Phof_WiderDeterminantsOfHealth);
            Assert.AreEqual(IndicatorId, grouping.IndicatorId);
            Assert.AreEqual(SexId, grouping.SexId);
            Assert.AreEqual(AgeId, grouping.AgeId);

            VerifyAll();
        }

        [TestMethod]
        public void TestGetGroupingWhereGroupIdAppearsInAProfile()
        {
            var groupId = GroupIds.Phof_WiderDeterminantsOfHealth;

            var provider = new SingleGroupingProvider(ReaderFactory.GetGroupDataReader(),
                GroupIdProviderThatWontBeUsed());
            var grouping = provider.GetGroupingByProfileIdAndGroupIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(ProfileIds.Phof, 
                groupId, AreaTypeId, IndicatorId, SexId, AgeId);

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
            var grouping = provider.GetGroupingByProfileIdAndGroupIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(ProfileIds.Phof, 
                groupId, AreaTypeId, IndicatorId, SexId, AgeId);

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
                GroupIds.Search, AreaTypeId, IndicatorId, SexId, AgeId);

            AssertIdsAreSameAsRequested(grouping);

            VerifyAll();
        }

        [TestMethod]
        public void TestGetGroupingByAreaTypeIdAndIndicatorIdAndSexIdAndAgeId()
        {
            var provider = new SingleGroupingProvider(ReaderFactory.GetGroupDataReader(),
                new GroupIdProvider(ReaderFactory.GetProfileReader()));
            var grouping = provider.GetGroupingByAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(AreaTypeId, IndicatorId, SexId, AgeId);

            AssertIdsAreSameAsRequested(grouping);

            VerifyAll();
        }

        [TestMethod]
        public void TestWhenNoGroupingInPholioMatchesRequestedParameters()
        {
            var provider = new SingleGroupingProvider(ReaderFactory.GetGroupDataReader(),
                new GroupIdProvider(ReaderFactory.GetProfileReader()));

            var grouping = provider.GetGroupingByProfileIdAndGroupIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(ProfileIds.Phof, 1,
                AreaTypeIds.AcuteTrust, IndicatorId, SexId, AgeIds.Over18);

            // Assert: no grouping found
            Assert.IsNull(grouping);

            VerifyAll();
        }

        [TestMethod]
        public void TestGetGroupingWithLatestDataPoint_Uses_Commonest_Polarity_For_Undefined_Profile()
        {
            // Arrange
            _groupDataReader.Setup(x => x.GetGroupingsByGroupIdsAndIndicatorIds(It.IsAny<IList<int>>(),
                It.IsAny<IList<int>>())).Returns(GetGroupings());

            SetUpGetCommonestPolarityForIndicator();

            // Act: Get the latest grouping
            var grouping = new SingleGroupingProvider(_groupDataReader.Object, null)
                .GetGroupingWithLatestDataPoint(new List<int> { 1 }, IndicatorId, AreaTypeId, ProfileIds.Undefined);

            // Assert: most recent year
            Assert.AreEqual(2002, grouping.DataPointYear);

            VerifyAll();
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
                IndicatorId = IndicatorId,
                AgeId = 3,
                SexId = 4
            };

            _groupDataReader.Setup(x => x.GetGroupingsByIndicatorId(groupingDifferentiator.IndicatorId)).Returns(new List<Grouping>
                {
                    new Grouping {IndicatorId = IndicatorId, AreaTypeId = childAreaTypeId, SexId = groupingDifferentiator.SexId, AgeId = groupingDifferentiator.AgeId},
                    new Grouping {IndicatorId = IndicatorId, AreaTypeId = unmatchedId, SexId = groupingDifferentiator.SexId, AgeId = groupingDifferentiator.AgeId},
                    new Grouping {IndicatorId = IndicatorId, AreaTypeId = childAreaTypeId, SexId = unmatchedId, AgeId = groupingDifferentiator.AgeId},
                    new Grouping {IndicatorId = IndicatorId, AreaTypeId = childAreaTypeId, SexId = groupingDifferentiator.SexId, AgeId = unmatchedId}
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
            Assert.AreEqual(IndicatorId, grouping.IndicatorId);
            Assert.AreEqual(SexId, grouping.SexId);
            Assert.AreEqual(AreaTypeId, grouping.AreaTypeId);
            Assert.AreEqual(AgeId, grouping.AgeId);
        }

        private static GroupIdProvider GroupIdProviderThatWontBeUsed()
        {
            var groupIdProvider = new Mock<GroupIdProvider>(MockBehavior.Strict);
            groupIdProvider.Protected();
            var o = groupIdProvider.Object;
            return o;
        }

        [TestMethod]
        public void TestGetGroupingByProfileIdAndAreaTypeIdAndIndicatorIdAndSexIdInSearch()
        {
            var groupId = GroupIds.Phof_WiderDeterminantsOfHealth;

            var groupIdProvider = MockGroupIdProvider();
            groupIdProvider
                .Setup(x => x.GetGroupIds(ProfileIds.Search))
                .Returns(new List<int> { groupId });

            SetUpGetGroupingsByGroupIdIndicatorIdSexId(groupId);

            SetUpGetCommonestPolarityForIndicator();

            var provider = new SingleGroupingProvider(_groupDataReader.Object, groupIdProvider.Object);

            var grouping = provider.GetGroupingByProfileIdAndAreaTypeIdAndIndicatorIdAndSexId(ProfileIds.Search,
                AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019,IndicatorId, SexId);

            Assert.AreEqual(PolarityIds.RagHighIsGood, grouping.PolarityId);

            VerifyAll();
        }

        [TestMethod]
        public void TestGetGroupingByProfileIdAndAreaTypeIdAndIndicatorIdAndAgeIdInSearch()
        {
            var groupId = GroupIds.Phof_WiderDeterminantsOfHealth;

            var groupIdProvider = MockGroupIdProvider();
            groupIdProvider
                .Setup(x => x.GetGroupIds(ProfileIds.Search))
                .Returns(new List<int> { groupId });

            SetUpGetGroupingsByGroupIdIndicatorIdAgeId(groupId);

            SetUpGetCommonestPolarityForIndicator();

            var provider = new SingleGroupingProvider(_groupDataReader.Object, groupIdProvider.Object);

            var grouping = provider.GetGroupingByProfileIdAndAreaTypeIdAndIndicatorIdAndAgeId(ProfileIds.Search,
                AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019, IndicatorId, AgeId);

            Assert.AreEqual(PolarityIds.RagHighIsGood, grouping.PolarityId);

            VerifyAll();
        }

        private void SetUpGetCommonestPolarityForIndicator()
        {
            _groupDataReader.Setup(x => x.GetCommonestPolarityForIndicator(It.Is<int>(i => i == IndicatorId)))
                .Returns(PolarityIds.RagHighIsGood);
        }

        private void SetUpGetGroupingsByGroupIdIndicatorIdSexId(int groupId)
        {
            _groupDataReader.Setup(x => x.GetGroupingsByGroupIdIndicatorIdSexId(groupId, AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019,
                IndicatorId, SexId)).Returns(GetGroupings());
        }

        private void SetUpGetGroupingsByGroupIdIndicatorIdAgeId(int groupId)
        {
            _groupDataReader.Setup(x => x.GetGroupingsByGroupIdIndicatorIdAgeId(groupId, AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019,
                IndicatorId, AgeId)).Returns(GetGroupings());
        }

        private void VerifyAll()
        {
            _groupDataReader.VerifyAll();
        }

        private IList<Grouping> GetGroupings()
        {
            return new List<Grouping>
            {
                new Grouping {IndicatorId = IndicatorId, DataPointYear = 2000, AreaTypeId = AreaTypeId},
                new Grouping {IndicatorId = IndicatorId ,DataPointYear = 2002, AreaTypeId = AreaTypeId},
                new Grouping {IndicatorId = IndicatorId, DataPointYear = 2001, AreaTypeId = AreaTypeId}
            };
        }
    }
}
