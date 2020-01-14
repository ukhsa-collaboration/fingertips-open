using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class GroupRootSummaryBuilderTest
    {
        private IGroupDataReader _groupDataReader = ReaderFactory.GetGroupDataReader();

        [TestMethod]
        public void TestBuildByProfileId()
        {
            IList<GroupRootSummary> summaries = new GroupRootSummaryBuilder(_groupDataReader)
                .BuildForProfileAndAreaType(ProfileIds.Phof, AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019);
            Assert.IsNotNull(summaries);
            Assert.IsTrue(summaries.Any());
        }

        [TestMethod]
        public void TestBuildByIndicatorId()
        {
            IList<GroupRootSummary> summaries = new GroupRootSummaryBuilder(_groupDataReader)
                .BuildForIndicatorIds(new List<int> { IndicatorIds.AdultExcessWeight }, ProfileIds.Phof);
            Assert.IsNotNull(summaries);
            Assert.IsTrue(summaries.Any());
        }

        [TestMethod]
        public void TestBuildByIndicatorIdDistinctByAgeSexIndicatorIds()
        {
            // Arrange
            const int indicatorId = IndicatorIds.DeprivationScoreIMD2015;
            var groupings = new List<Grouping>
            {
                GetGrouping(SexIds.Persons, AgeIds.From15To19, indicatorId),
                GetGrouping(SexIds.Persons, AgeIds.From20To24, indicatorId),
                GetGrouping(SexIds.Persons, AgeIds.From25To29, indicatorId),
                GetGrouping(SexIds.Persons, AgeIds.From25To29, indicatorId),
                GetGrouping(SexIds.Female, AgeIds.From15To19, indicatorId),
                GetGrouping(SexIds.Female, AgeIds.From15To19, indicatorId)
            };

            var mock = new Mock<IGroupDataReader>(MockBehavior.Strict);
            mock.Setup(x => x.GetGroupingsByIndicatorId(indicatorId)).Returns(groupings);

            // Act
            IList<GroupRootSummary> summaries = new GroupRootSummaryBuilder(ReaderFactory.GetGroupDataReader())
                .BuildForIndicatorIds(new List<int> { indicatorId }, ProfileIds.Phof);

            // Assert: expected number of distinct summaries
            Assert.AreEqual(1, summaries.Count);
            Assert.AreEqual(1, summaries.Count(x => x.Sex.Id == SexIds.Persons));
            Assert.AreEqual(1, summaries.Count(x => x.Age.Id == AgeIds.AllAges));
        }

        private static Grouping GetGrouping(int sexId, int ageId, int indicatorId)
        {
            return new Grouping
            {
                SexId = sexId,
                Sex = new Sex { Id = sexId },
                AgeId = ageId,
                Age = new Age { Id = ageId },
                IndicatorId = indicatorId
            };
        }
    }
}