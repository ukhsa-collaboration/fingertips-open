using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class AverageCalculatorFactoryTest
    {
        [TestMethod]
        public void TestNullAverageCalculatorForSlopeIndexOfInequality()
        {
            Assert.IsInstanceOfType(Calculator(ValueTypeId.SlopeIndexOfInequality),
                typeof(NullAverageCalculator));
        }

        [TestMethod]
        public void TestCountAverageCalculatorForCount()
        {
            Assert.IsInstanceOfType(Calculator(ValueTypeId.Count),
                typeof(CountAverageCalculator));
        }

        [TestMethod]
        public void TestWeightedAverageCalculatorForProportion()
        {
            Assert.IsInstanceOfType(Calculator(ValueTypeId.Proportion),
                typeof(WeightedAverageCalculator));
        }

        public static AverageCalculator Calculator(int id)
        {
            var indicatorMetadata = new IndicatorMetadata
            {
                ValueTypeId = id
            };

            return AverageCalculatorFactory.New(new List<CoreDataSet>(), indicatorMetadata);
        }
    }
}
