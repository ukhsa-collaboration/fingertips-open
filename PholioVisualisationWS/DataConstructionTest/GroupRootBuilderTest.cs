
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var polarity = PolarityIds.BlueOrangeBlue;
            List<Grouping> grouping = new List<Grouping>();
            grouping.Add(new Grouping { PolarityId = polarity });

            var root = new GroupRootBuilder().BuildGroupRoots(grouping).First();

            Assert.AreEqual(polarity, root.PolarityId);
        }

        [TestMethod]
        public void TestAgeIdAssignedFromGrouping()
        {
            var ageId = 200;
            List<Grouping> grouping = new List<Grouping>();
            grouping.Add(new Grouping { AgeId = ageId });

            var root = new GroupRootBuilder().BuildGroupRoots(grouping).First();

            Assert.AreEqual(ageId, root.AgeId);
        }

        [TestMethod]
        public void TestSexIdAssignedFromGrouping()
        {
            List<Grouping> grouping = new List<Grouping>();
            grouping.Add(new Grouping { SexId = SexIds.Male });
            grouping.Add(new Grouping { SexId = SexIds.Female });

            var roots = new GroupRootBuilder().BuildGroupRoots(grouping);

            Assert.AreEqual(SexIds.Male, roots[0].SexId);
            Assert.AreEqual(SexIds.Female, roots[1].SexId);
        }

        [TestMethod]
        public void TestTwoGroupings()
        {
            List<Grouping> grouping = new List<Grouping>();

            grouping.Add(new Grouping { IndicatorId = 1, SexId = 1 });
            grouping.Add(new Grouping { IndicatorId = 2, SexId = 1 });

            GroupRootBuilder builder = new GroupRootBuilder();
            IList<GroupRoot> roots = builder.BuildGroupRoots(grouping);

            Assert.AreEqual(2, roots.Count);
        }

        [TestMethod]
        public void TestGroupingsAnnualAndQuarterlyGroupingsGiveTwoGroupRoots()
        {
            List<Grouping> grouping = new List<Grouping>();

            grouping.Add(new Grouping
            {
                IndicatorId = 1,
                SexId = 1,
                BaselineYear = 2001,
                BaselineQuarter = -1,
                DataPointYear = 2001,
                DataPointQuarter = -1
            });

            grouping.Add(new Grouping
            {
                IndicatorId = 1,
                SexId = 1,
                BaselineYear = 2001,
                BaselineQuarter = 1,
                DataPointYear = 2001,
                DataPointQuarter = 1
            });

            GroupRootBuilder builder = new GroupRootBuilder();
            IList<GroupRoot> roots = builder.BuildGroupRoots(grouping);

            Assert.AreEqual(2, roots.Count);
        }

        [TestMethod]
        public void TestGroupingsAnnualAndMonthlyGroupingsGiveTwoGroupRoots()
        {
            List<Grouping> grouping = new List<Grouping>();

            grouping.Add(new Grouping
            {
                IndicatorId = 1,
                SexId = 1,
                BaselineYear = 2001,
                BaselineQuarter = -1,
                DataPointYear = 2001,
                DataPointQuarter = -1
            });

            grouping.Add(new Grouping
            {
                IndicatorId = 1,
                SexId = 1,
                BaselineYear = 2001,
                BaselineQuarter = -1,
                BaselineMonth = 1,
                DataPointYear = 2001,
                DataPointQuarter = -1,
                DataPointMonth = 1
            });

            GroupRootBuilder builder = new GroupRootBuilder();
            IList<GroupRoot> roots = builder.BuildGroupRoots(grouping);

            Assert.AreEqual(2, roots.Count);
        }

        [TestMethod]
        public void TestStateSexSetCorrectly()
        {
            List<Grouping> grouping = new List<Grouping>();

            grouping.Add(new Grouping { IndicatorId = 1, SexId = 1 });
            grouping.Add(new Grouping { IndicatorId = 2, SexId = 1 });
            grouping.Add(new Grouping { IndicatorId = 2, SexId = 4 });

            GroupRootBuilder builder = new GroupRootBuilder();
            IList<GroupRoot> roots = builder.BuildGroupRoots(grouping);

            Assert.AreEqual(3, roots.Count);
            Assert.IsFalse(roots[0].StateSex);
            Assert.IsTrue(roots[1].StateSex);
            Assert.IsTrue(roots[2].StateSex);
        }

        [TestMethod]
        public void TestAgeLabelSetCorrectly()
        {
            List<Grouping> grouping = new List<Grouping>();

            grouping.Add(new Grouping { IndicatorId = 1, SexId = 1 });
            grouping.Add(new Grouping { IndicatorId = 2, SexId = 1, AgeId = AgeIds.AllAges });
            grouping.Add(new Grouping { IndicatorId = 2, SexId = 1, AgeId = AgeIds.From0To4 });

            GroupRootBuilder builder = new GroupRootBuilder();
            IList<GroupRoot> roots = builder.BuildGroupRoots(grouping);

            Assert.AreEqual(3, roots.Count);
            Assert.IsFalse(roots[0].StateAge);
            Assert.IsTrue(roots[1].StateAge);
            Assert.IsTrue(roots[2].StateAge);
        }

        [TestMethod]
        public void TestOrderOfRootsIsSameAsGroupings()
        {
            List<Grouping> grouping = new List<Grouping>();

            grouping.Add(new Grouping { IndicatorId = 43, Sequence = 1 });
            grouping.Add(new Grouping { IndicatorId = 2, Sequence = 2 });
            grouping.Add(new Grouping { IndicatorId = 9, Sequence = 3 });

            GroupRootBuilder builder = new GroupRootBuilder();
            IList<GroupRoot> roots = builder.BuildGroupRoots(grouping);

            // Do not want any reordering, e.g. by indicator ID
            Assert.AreEqual(43, roots[0].IndicatorId);
            Assert.AreEqual(2, roots[1].IndicatorId);
            Assert.AreEqual(9, roots[2].IndicatorId);
        }

        [TestMethod]
        public void TestNoGroupingsNoIndicatorIds()
        {
            List<Grouping> grouping = new List<Grouping>();
            GroupRootBuilder builder = new GroupRootBuilder();
            IList<GroupRoot> roots = builder.BuildGroupRoots(grouping);

            Assert.AreEqual(0, roots.Count);
        }

        [TestMethod]
        public void TestNoGroupingsSomeIndicatorIds()
        {
            List<Grouping> grouping = new List<Grouping>();
            GroupRootBuilder builder = new GroupRootBuilder();
            IList<GroupRoot> roots = builder.BuildGroupRoots(grouping);

            Assert.AreEqual(0, roots.Count);
        }
    }
}
