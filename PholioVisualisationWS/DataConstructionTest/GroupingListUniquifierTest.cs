using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [TestMethod]
        public void Test_Widest_Time_Period_Range_Is_Used_For_Grouping()
        {
            // Arrange: data with a variety of years
            var grouping1 = Grouping();
            grouping1.BaselineYear = 2001;
            grouping1.DataPointYear = 2009;
            var grouping2 = Grouping();
            grouping2.BaselineYear = 2000;
            grouping2.DataPointYear = 2009;
            var grouping3 = Grouping();
            grouping3.BaselineYear = 2001;
            grouping3.DataPointYear = 2010;

            var uniqueGroupings = UniqueGroupings(grouping1, grouping2, grouping3);

            // Assert: earliest baseline and latest datapoint used
            Assert.AreEqual(1, uniqueGroupings.Count);
            Assert.AreEqual(2000, uniqueGroupings.First().BaselineYear);
            Assert.AreEqual(2010, uniqueGroupings.First().DataPointYear);
        }

        [TestMethod]
        public void Test_Commonest_Polarity_Is_Used_For_Grouping()
        {
            // Arrange: data with a variety of years
            var grouping1 = Grouping();
            grouping1.PolarityId = PolarityIds.NotApplicable;
            var grouping2 = Grouping();
            grouping2.PolarityId = PolarityIds.BlueOrangeBlue;
            var grouping3 = Grouping();
            grouping3.PolarityId = PolarityIds.BlueOrangeBlue;

            var uniqueGroupings = UniqueGroupings(grouping1, grouping2, grouping3);

            // Assert: commonest polarity id used
            Assert.AreEqual(1, uniqueGroupings.Count);
            Assert.AreEqual(PolarityIds.BlueOrangeBlue, uniqueGroupings.First().PolarityId);
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
