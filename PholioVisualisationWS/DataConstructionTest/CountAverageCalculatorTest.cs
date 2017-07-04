using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class CountAverageCalculatorTest
    {
        [TestMethod]
        public void TestAverage()
        {
            var dataList = new List<CoreDataSet> {
                new CoreDataSet{Count = 2},
                new CoreDataSet{Count = 4}
            };

            var calculator = new CountAverageCalculator(CoreDataSetFilter(dataList));
            var average = calculator.Average;

            // Assert: count and value are the same
            Assert.AreEqual(6, average.Value);
            Assert.AreEqual(6, average.Count);
        }

        [TestMethod]
        public void TestAverageReturnCoreDataSetHasDefaultValues()
        {
            var dataList = new List<CoreDataSet> {
                new CoreDataSet{Count = 12}
            };

            var average = new CountAverageCalculator(CoreDataSetFilter(dataList)).Average;
            Assert.AreEqual(12, average.Count);
            Assert.AreEqual(ValueData.NullValue, average.Denominator);
            Assert.AreEqual(ValueData.NullValue, average.Denominator2);
            Assert.AreEqual(ValueData.NullValue, average.LowerCI);
            Assert.AreEqual(ValueData.NullValue, average.UpperCI);
        }

        [TestMethod]
        public void TestAverageReturnsNullForEmptyDataList()
        {
            var coreDataSetFilter = CoreDataSetFilter(new List<CoreDataSet> { });
            var calculator = new CountAverageCalculator(coreDataSetFilter);
            Assert.IsNull(calculator.Average);
        }

        private static CoreDataSetFilter CoreDataSetFilter(List<CoreDataSet> dataList)
        {
            var coreDataSetFilter = new Mock<CoreDataSetFilter>(MockBehavior.Strict);
            coreDataSetFilter.Protected();
            coreDataSetFilter
                .Setup(x => x.SelectWhereCountIsValid())
                .Returns(dataList);
            return coreDataSetFilter.Object;
        }
    }
}
