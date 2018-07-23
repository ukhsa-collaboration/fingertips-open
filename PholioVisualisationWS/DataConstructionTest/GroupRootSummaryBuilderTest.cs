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
                .BuildForProfileAndAreaType(ProfileIds.Phof, AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.IsNotNull(summaries);
            Assert.IsTrue(summaries.Any());
        }

        [TestMethod]
        public void TestBuildByIndicatorId()
        {
            IList<GroupRootSummary> summaries = new GroupRootSummaryBuilder(_groupDataReader)
                .BuildForIndicatorIds(new List<int> { IndicatorIds.ExcessWinterDeaths }, ProfileIds.Phof);
            Assert.IsNotNull(summaries);
            Assert.IsTrue(summaries.Any());
        }

        [TestMethod]
        public void TestBuildByIndicatorIdDistinctByAgeSexIndictorIds()
        {
            // Arrange
            const int indicatorId = IndicatorIds.ExcessWinterDeaths;
            var groupings = new List<Grouping>
            {
                GetGrouping(SexIds.Persons,6,indicatorId),
                GetGrouping(SexIds.Persons,7,indicatorId),
                GetGrouping(SexIds.Persons,8,indicatorId),
                GetGrouping(SexIds.Persons,8,indicatorId),
                GetGrouping(SexIds.Female,6,indicatorId),
                GetGrouping(SexIds.Female,6,indicatorId)
            };

            var mock = new Mock<IGroupDataReader>(MockBehavior.Strict);
            mock.Setup(x => x.GetGroupingsByIndicatorId(indicatorId)).Returns(groupings);

            // Act
            IList<GroupRootSummary> summaries = new GroupRootSummaryBuilder(mock.Object)
                .BuildForIndicatorIds(new List<int> { indicatorId }, null);

            // Assert: expected number of distinct summaries
            Assert.AreEqual(4, summaries.Count);
            Assert.AreEqual(3, summaries.Count(x => x.Sex.Id == SexIds.Persons));
            Assert.AreEqual(2, summaries.Count(x => x.Age.Id == 6));
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