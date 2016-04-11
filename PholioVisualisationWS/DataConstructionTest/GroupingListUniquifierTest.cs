using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class GroupingListUniquifierTest
    {
        private const int UniqueValue = 99;

        [TestMethod]
        public void TestEmptyListInEmptyListOut()
        {
            var uniqueGroupings = UniqueGroupings();
            Assert.AreEqual(0, uniqueGroupings.Count);
        }

        [TestMethod]
        public void TestTwoIdenticalGroupingsReducedToOne()
        {
            var uniqueGroupings = UniqueGroupings(Grouping(), Grouping());
            Assert.AreEqual(1, uniqueGroupings.Count);
        }

        [TestMethod]
        public void TestUniquifyOnIndicatorId()
        {
            var grouping = Grouping();
            grouping.IndicatorId = UniqueValue;
            var uniqueGroupings = UniqueGroupings(Grouping(), grouping);
            Assert.AreEqual(2, uniqueGroupings.Count);
        }

        [TestMethod]
        public void TestUniquifyOnSexId()
        {
            var grouping = Grouping();
            grouping.SexId = UniqueValue;
            var uniqueGroupings = UniqueGroupings(Grouping(), grouping);
            Assert.AreEqual(2, uniqueGroupings.Count);
        }

        [TestMethod]
        public void TestUniquifyOnAreaTypeId()
        {
            var grouping = Grouping();
            grouping.AreaTypeId = UniqueValue;
            var uniqueGroupings = UniqueGroupings(Grouping(), grouping);
            Assert.AreEqual(2, uniqueGroupings.Count);
        }

        [TestMethod]
        public void TestUniquifyOnComparatorId()
        {
            var grouping = Grouping();
            grouping.ComparatorId = UniqueValue;
            var uniqueGroupings = UniqueGroupings(Grouping(), grouping);
            Assert.AreEqual(2, uniqueGroupings.Count);
        }

        [TestMethod]
        public void TestUniquifyOnAgeId()
        {
            var grouping = Grouping();
            grouping.AgeId = UniqueValue;
            var uniqueGroupings = UniqueGroupings(Grouping(), grouping);
            Assert.AreEqual(2, uniqueGroupings.Count);
        }

        private static Grouping Grouping()
        {
            return new Grouping
                {
                    IndicatorId = 1,
                    AreaTypeId = 2,
                    SexId = 3,
                    ComparatorId = 4,
                    AgeId = 5
                };
        }

        private static IList<Grouping> UniqueGroupings(params Grouping[] groupings)
        {
            var list = new List<Grouping>(groupings);
            return new GroupingListUniquifier(list).GetUniqueGroupings();
        }
    }
}
