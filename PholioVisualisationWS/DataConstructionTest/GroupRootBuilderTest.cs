using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class GroupRootBuilderTest
    {
        [TestMethod]
        public void TestPolarityAssignedFromGrouping()
        {
            // Assign
            var polarity = PolarityIds.RagHighIsGood;
            List<Grouping> grouping = new List<Grouping>
            {
                new Grouping { IndicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth, PolarityId = polarity }
            };

            // Action
            var root = new GroupRootBuilder(ReaderFactory.GetGroupDataReader()).BuildGroupRoots(grouping).First();

            // Assert
            Assert.AreEqual(polarity, root.PolarityId);
        }

        [TestMethod]
        public void TestAgeIdAssignedFromGrouping()
        {
            // Assign
            var ageId = AgeIds.From4To5;
            var indicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth;

            List<Grouping> grouping = new List<Grouping>
            {
                new Grouping { AgeId = ageId, IndicatorId = indicatorId }
            };

            // Action
            var root = new GroupRootBuilder(ReaderFactory.GetGroupDataReader()).BuildGroupRoots(grouping).First();

            // Assert
            Assert.AreEqual(ageId, root.AgeId);
        }

        [TestMethod]
        public void TestSexIdAssignedFromGrouping()
        {
            // Assign
            List<Grouping> grouping = new List<Grouping>
            {
                new Grouping { IndicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth, SexId = SexIds.Male },
                new Grouping { IndicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth, SexId = SexIds.Female }
            };

            // Action
            var roots = new GroupRootBuilder(ReaderFactory.GetGroupDataReader()).BuildGroupRoots(grouping);

            // Assert
            Assert.AreEqual(SexIds.Male, roots[0].SexId);
            Assert.AreEqual(SexIds.Female, roots[1].SexId);
        }

        [TestMethod]
        public void TestTwoGroupings()
        {
            // Assign
            List<Grouping> grouping = new List<Grouping>
            {
                new Grouping { IndicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth, SexId = SexIds.Male },
                new Grouping { IndicatorId = IndicatorIds.DeprivationScoreIMD2015, SexId = SexIds.Persons }
            };

            // Action
            var roots = new GroupRootBuilder(ReaderFactory.GetGroupDataReader()).BuildGroupRoots(grouping);

            // Assert
            Assert.AreEqual(2, roots.Count);
        }

        [TestMethod]
        public void TestGroupingsAnnualAndQuarterlyGroupingsGiveTwoGroupRoots()
        {
            // Assign
            List<Grouping> grouping = new List<Grouping>
            {
                new Grouping
                {
                    IndicatorId = IndicatorIds.ChildrenInLowIncomeFamilies,
                    SexId = SexIds.Male,
                    BaselineYear = 2001,
                    BaselineQuarter = -1,
                    DataPointYear = 2001,
                    DataPointQuarter = -1
                },
                new Grouping
                {
                    IndicatorId = IndicatorIds.ChildrenInLowIncomeFamilies,
                    SexId = SexIds.Male,
                    BaselineYear = 2001,
                    BaselineQuarter = 1,
                    DataPointYear = 2001,
                    DataPointQuarter = 1
                }
            };

            // Action
            var roots = new GroupRootBuilder(ReaderFactory.GetGroupDataReader()).BuildGroupRoots(grouping);

            // Assert
            Assert.AreEqual(2, roots.Count);
        }

        [TestMethod]
        public void TestGroupingsAnnualAndMonthlyGroupingsGiveTwoGroupRoots()
        {
            // Assign
            List<Grouping> grouping = new List<Grouping>
            {
                new Grouping
                {
                    IndicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth,
                    SexId = SexIds.Male,
                    BaselineYear = 2009,
                    BaselineQuarter = -1,
                    DataPointYear = 2015,
                    DataPointQuarter = -1
                },
                new Grouping
                {
                    IndicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth,
                    SexId = SexIds.Male,
                    BaselineYear = 2001,
                    BaselineQuarter = -1,
                    BaselineMonth = 1,
                    DataPointYear = 2001,
                    DataPointQuarter = -1,
                    DataPointMonth = 1
                }
            };

            // Action
            var roots = new GroupRootBuilder(ReaderFactory.GetGroupDataReader()).BuildGroupRoots(grouping);

            // Assert
            Assert.AreEqual(2, roots.Count);
        }

        [TestMethod]
        public void TestStateSexSetCorrectly()
        {
            // Assign
            List<Grouping> grouping = new List<Grouping>()
            {
                new Grouping { IndicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth, SexId = SexIds.Male },
                new Grouping { IndicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth, SexId = SexIds.Female },
                new Grouping { IndicatorId = IndicatorIds.ChildrenInLowIncomeFamilies, SexId = SexIds.Persons }
            };

            // Action
            var roots = new GroupRootBuilder(ReaderFactory.GetGroupDataReader()).BuildGroupRoots(grouping);

            // Assert
            Assert.AreEqual(3, roots.Count);
            Assert.IsTrue(roots[0].StateSex);
            Assert.IsTrue(roots[1].StateSex);
            Assert.IsFalse(roots[2].StateSex);
        }

        [TestMethod]
        public void TestAgeLabelSetCorrectly_True()
        {
            // Assign: two groupings with same indicator id but different ages
            List<Grouping> grouping = new List<Grouping>
            {
                new Grouping { IndicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth, SexId = SexIds.Male, AgeId = AgeIds.From10To11 },
                new Grouping { IndicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth, SexId = SexIds.Male, AgeId = AgeIds.From0To4 }
            };

            // Action
            var roots = new GroupRootBuilder(ReaderFactory.GetGroupDataReader()).BuildGroupRoots(grouping);

            // Assert
            Assert.AreEqual(2, roots.Count);
            Assert.AreEqual(2, roots.Select(x => x.StateAge == true).Count());
        }

        [TestMethod]
        public void TestAgeLabelSetCorrectly_False()
        {
            // Assign: two groupings with different indicator IDs
            List<Grouping> grouping = new List<Grouping>
            {
                new Grouping { IndicatorId = IndicatorIds.ChildrenInLowIncomeFamilies, SexId = SexIds.Male, AgeId = AgeIds.From0To4 },
                new Grouping { IndicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth, SexId = SexIds.Male, AgeId = AgeIds.From10To11 }
            };

            // Action
            var roots = new GroupRootBuilder(ReaderFactory.GetGroupDataReader()).BuildGroupRoots(grouping);

            // Assert
            Assert.AreEqual(2, roots.Count);
            Assert.IsFalse(roots[0].StateAge);
            Assert.IsFalse(roots[1].StateAge);
        }

        [TestMethod]
        public void TestOrderOfRootsIsSameAsGroupings()
        {
            // Assign
            List<Grouping> grouping = new List<Grouping>
            {
                new Grouping {IndicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth, Sequence = 1},
                new Grouping {IndicatorId = IndicatorIds.ChildrenInLowIncomeFamilies, Sequence = 2},
                new Grouping {IndicatorId = IndicatorIds.DeprivationScoreIMD2015, Sequence = 3}
            };

            // Action
            var roots = new GroupRootBuilder(ReaderFactory.GetGroupDataReader()).BuildGroupRoots(grouping);

            // Assert
            // Do not want any reordering, e.g. by indicator ID
            Assert.AreEqual(IndicatorIds.HealthyLifeExpectancyAtBirth, roots[0].IndicatorId);
            Assert.AreEqual(IndicatorIds.ChildrenInLowIncomeFamilies, roots[1].IndicatorId);
            Assert.AreEqual(IndicatorIds.DeprivationScoreIMD2015, roots[2].IndicatorId);

            // Sequences are set
            Assert.AreEqual(1, roots[0].Sequence);
            Assert.AreEqual(2, roots[1].Sequence);
            Assert.AreEqual(3, roots[2].Sequence);
        }

        [TestMethod]
        public void TestNoGroupingsNoIndicatorIds()
        {
            List<Grouping> grouping = new List<Grouping>();
            GroupRootBuilder builder = new GroupRootBuilder(new GroupDataReader());
            IList<GroupRoot> roots = builder.BuildGroupRoots(grouping);

            Assert.AreEqual(0, roots.Count);
        }

        [TestMethod]
        public void TestNoGroupingsSomeIndicatorIds()
        {
            List<Grouping> grouping = new List<Grouping>();
            GroupRootBuilder builder = new GroupRootBuilder(new GroupDataReader());
            IList<GroupRoot> roots = builder.BuildGroupRoots(grouping);

            Assert.AreEqual(0, roots.Count);
        }

        [TestMethod]
        public void TestPolarityIdSetCorrectlyWhenIsSearchProfile()
        {
            // Assign
            const int profileId = 13;
            const int polarity = PolarityIds.RagLowIsGood;
            const int expectedPolarity = PolarityIds.RagHighIsGood;
            var grouping = new List<Grouping>
            {
                new Grouping { IndicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth, PolarityId = polarity }
            };

            // Action
            var root = new GroupRootBuilder(profileId, ReaderFactory.GetGroupDataReader()).BuildGroupRoots(grouping).First();

            // Assert
            Assert.AreEqual(expectedPolarity, root.PolarityId);
        }

        [TestMethod]
        public void TestPolarityIdSetCorrectlyWhenIsNotSearchProfile()
        {
            // Assign
            const int polarity = PolarityIds.RagHighIsGood;
            const int expectedPolarity = PolarityIds.RagHighIsGood;
            var grouping = new List<Grouping>
            {
                new Grouping { IndicatorId = IndicatorIds.HealthyLifeExpectancyAtBirth, PolarityId = polarity }
            };

            // Action
            var root = new GroupRootBuilder(ReaderFactory.GetGroupDataReader()).BuildGroupRoots(grouping).First();

            // Assert
            Assert.AreEqual(expectedPolarity, root.PolarityId);
        }
    }
}
