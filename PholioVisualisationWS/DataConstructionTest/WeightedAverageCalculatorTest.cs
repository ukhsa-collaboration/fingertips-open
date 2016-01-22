using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class WeightedAverageCalculatorTest
    {
        [TestMethod]
        public void TestAverageReturnCoreDataSetHasDefaultValues()
        {
            var denominator = 4;
            var count = 8;

            var dataList = new List<CoreDataSet> {
                new CoreDataSet{Value = 2, Count = count, Denominator = denominator}
            };

            var coreDataSetFilter = CoreDataSetFilter(dataList);
            var average = new WeightedAverageCalculator(coreDataSetFilter, Unit(1)).Average;
            Assert.AreEqual(count, average.Count);
            Assert.AreEqual(denominator, average.Denominator);
            Assert.AreEqual(ValueData.NullValue, average.Denominator2);
            Assert.AreEqual(ValueData.NullValue, average.LowerCI);
            Assert.AreEqual(ValueData.NullValue, average.UpperCI);
        }

        [TestMethod]
        public void TestAverage()
        {
            var dataList = new List<CoreDataSet> {
                new CoreDataSet{Value = 2, Count = 8, Denominator = 4},
                new CoreDataSet{Value = 4, Count = 32, Denominator = 8}
            };

            var coreDataSetFilter = CoreDataSetFilter(dataList);
            var calculator = new WeightedAverageCalculator(coreDataSetFilter, Unit(1));
            Assert.AreEqual(Round(3.33333), Round(calculator.Average.Value));
        }

        [TestMethod]
        public void TestAverageUsesUnit()
        {
            var dataList = new List<CoreDataSet> {
                new CoreDataSet{Value = 20, Count = 8, Denominator = 4}
            };

            var coreDataSetFilter = CoreDataSetFilter(dataList);
            var calculator = new WeightedAverageCalculator(coreDataSetFilter, Unit(10));
            Assert.AreEqual(Round(20), Round(calculator.Average.Value));
        }

        [TestMethod]
        public void TestAverageMustBeValidOnFirstCoreDataSetObject()
        {
            var dataList = new List<CoreDataSet> {
                new CoreDataSet{Value = -1, Count = 8, Denominator = 4},
                new CoreDataSet{Value = 4, Count = 32, Denominator = 8}
            };

            var coreDataSetFilter = CoreDataSetFilter(dataList);
            var calculator = new WeightedAverageCalculator(coreDataSetFilter, Unit(1));
            Assert.IsNull(calculator.Average);
        }

        [TestMethod]
        public void TestAverageAssessmentOfWhetherAverageIsValidOnFirstCoreDataSetObjectUsesUnit()
        {
            var dataList = new List<CoreDataSet> {
                new CoreDataSet{Value = 20, Count = 8, Denominator = 4}
            };

            var coreDataSetFilter = CoreDataSetFilter(dataList);
            var calculator = new WeightedAverageCalculator(coreDataSetFilter, Unit(10));
            Assert.IsNotNull(calculator.Average);
        }

        [TestMethod]
        public void TestAverageReturnsNullForEmptyDataList()
        {
            var coreDataSetFilter = CoreDataSetFilter(new List<CoreDataSet> { });
            var calculator = new WeightedAverageCalculator(coreDataSetFilter, Unit(1));
            Assert.IsNull(calculator.Average);
        }

        private static Unit Unit(int id)
        {
            return new Unit { Value = id };
        }

        private static CoreDataSetFilter CoreDataSetFilter(List<CoreDataSet> dataList)
        {
            var coreDataSetFilter = new Mock<CoreDataSetFilter>(MockBehavior.Strict);
            coreDataSetFilter.Protected();
            coreDataSetFilter
                .Setup(x => x.SelectWhereCountAndDenominatorAreValid())
                .Returns(dataList);
            return coreDataSetFilter.Object;
        }


        private static double Round(double d)
        {
            return Math.Round(d, 4);
        }
    }
}
