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
    public class WeightedAverageCalculatorTest
    {
        [TestMethod]
        public void Test_Average_CoreDataSet_Has_Default_Values()
        {
            var dataList = GetValidDataList();

            var coreDataSetFilter = CoreDataSetFilter(dataList);
            var average = new StandardAverageCalculator(coreDataSetFilter, Unit(1)).Average;

            // Assert: check default values
            Assert.AreEqual(ValueData.NullValue, average.Denominator2);
            Assert.IsNull(average.LowerCI95);
            Assert.IsNull(average.UpperCI95);
            Assert.IsNull(average.LowerCI99_8);
            Assert.IsNull(average.UpperCI99_8);
        }

        [TestMethod]
        public void Test_Average_Can_Be_Calculated()
        {
            var dataList = GetValidDataList();

            var coreDataSetFilter = CoreDataSetFilter(dataList);
            var calculator = new StandardAverageCalculator(coreDataSetFilter, Unit(1));

            // Assert: check calculated values
            var average = calculator.Average;
            Assert.AreEqual(Round(3.3), Round(average.Value));
            Assert.AreEqual(40, average.Count);
            Assert.AreEqual(12, average.Denominator);
        }

        [TestMethod]
        public void TestAverageUsesUnit()
        {
            var dataList = GetValidDataList();

            var coreDataSetFilter = CoreDataSetFilter(dataList);
            var calculator = new StandardAverageCalculator(coreDataSetFilter, Unit(10));

            // Assert: unit used
            var average = calculator.Average;
            Assert.AreEqual(Round(33.3), Round(average.Value));
        }

        [TestMethod]
        public void Test_Average_Assessment_Of_Whether_Average_Is_Valid_On_First_CoreDataSet_Uses_Unit()
        {
            var dataList = GetValidDataList();

            var coreDataSetFilter = CoreDataSetFilter(dataList);
            var calculator = new StandardAverageCalculator(coreDataSetFilter, Unit(10));
            Assert.IsNotNull(calculator.Average);
        }

        [TestMethod]
        public void Test_Average_Not_Calculated_For_Empty_Data_List()
        {
            var coreDataSetFilter = CoreDataSetFilter(new List<CoreDataSet> { });
            var calculator = new StandardAverageCalculator(coreDataSetFilter, Unit(1));
            Assert.IsNull(calculator.Average);
        }

        [TestMethod]
        public void Test_Average_Not_Calculated_When_Only_One_Value()
        {
            var dataList = new List<CoreDataSet> {
                new CoreDataSet{Value = 2, Count = 1, Denominator = 1}
            };
            var coreDataSetFilter = CoreDataSetFilter(dataList);
            var calculator = new StandardAverageCalculator(coreDataSetFilter, Unit(1));
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


        private static List<CoreDataSet> GetValidDataList()
        {
            var dataList = new List<CoreDataSet>
            {
                new CoreDataSet {Value = 2, Count = 8, Denominator = 4},
                new CoreDataSet {Value = 4, Count = 32, Denominator = 8}
            };
            return dataList;
        }

        private static double Round(double d)
        {
            return Math.Round(d, 1);
        }
    }
}
