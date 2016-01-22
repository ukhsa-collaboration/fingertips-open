using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace DataConstructionTest
{
    [TestClass]
    public class CcgAverageCalculatorTest
    {
        //NOTE value = count / denominator

        [TestMethod]
        public void TestNullAverageWhenRatioValueType()
        {
            var data = new[] {
                new CoreDataSet { Value = 2, Count = 4, Denominator = 2, AreaCode = "a" },
                new CoreDataSet { Value = 2, Count = 4, Denominator = 2, AreaCode = "b" }
            };

            var metadata = new IndicatorMetadata
            {
                ValueTypeId = ValueTypeId.Ratio,
                Unit = new Unit { Value = 1 }
            };

            Assert.IsNull(new CcgAverageCalculator(data, GetCcgPopulation(), metadata).Average);
        }

        [TestMethod]
        public void TestStandardAverageSimple()
        {
            var data = new[] {
                new CoreDataSet { Value = 2, Count = 4, Denominator = 2, AreaCode = "a" },
                new CoreDataSet { Value = 2, Count = 4, Denominator = 2, AreaCode = "b" }
            };

            Assert.AreEqual(2, new CcgAverageCalculator(data, GetCcgPopulation(), Metadata(1)).Average.Value);
        }

        [TestMethod]
        public void TestStandardAverage()
        {
            var data = new[] {
                new CoreDataSet { Value = 1, Count = 1, Denominator = 1, AreaCode = "a" },
                new CoreDataSet { Value = 4, Count = 16, Denominator = 4, AreaCode = "b" }
            };

            Assert.AreEqual(3.4, new CcgAverageCalculator(data, GetCcgPopulation(), Metadata(1)).Average.Value);
        }

        [TestMethod]
        public void TestStandardIfNoDenominatorForPopulationBelowThreshhold()
        {
            var data = new[] {
                new CoreDataSet { Value = 1, Count = 1, Denominator = -1, AreaCode = "a" },
                new CoreDataSet { Value = 4, Count = 16, Denominator = 4, AreaCode = "b" }
            };

            Assert.AreEqual(4, Math.Round(new CcgAverageCalculator(data, GetCcgPopulation(), Metadata(1)).Average.Value, 3));
        }

        [TestMethod]
        public void TestStandardIfNoCountForPopulationBelowThreshhold()
        {
            var data = new[] {
                new CoreDataSet { Value = 1, Count = -1, Denominator = 1, AreaCode = "a" },
                new CoreDataSet { Value = 4, Count = 16, Denominator = 4, AreaCode = "b" }
            };

            Assert.AreEqual(4, Math.Round(new CcgAverageCalculator(data, GetCcgPopulation(), Metadata(1)).Average.Value, 3));
        }

        [TestMethod]
        public void TestWeightedIfNoDenominatorForPopulationAboveThreshhold()
        {
            var data = new[] {
                new CoreDataSet { Value = 1, Count = 1, Denominator = 1, AreaCode = "a" },
                new CoreDataSet { Value = 4, Count = 16, Denominator = -1, AreaCode = "b" }
            };

            Assert.IsNull(new CcgAverageCalculator(data, GetCcgPopulation(), Metadata(1)).Average);
        }

        [TestMethod]
        public void TestWeightedIfNoCountForPopulationAboveThreshhold()
        {
            var data = new[] {
                new CoreDataSet { Value = 1, Count = 1, Denominator = 1, AreaCode = "a" },
                new CoreDataSet { Value = 4, Count = -1, Denominator = 4, AreaCode = "b" }
            };

            Assert.IsNull(new CcgAverageCalculator(data, GetCcgPopulation(), Metadata(1)).Average);
        }

        [TestMethod]
        public void TestStandardAverageWithUnit()
        {
            var data = new[] {
                new CoreDataSet { Value = 1, Count = 1, Denominator = 1, AreaCode = "a" },
                new CoreDataSet { Value = 4, Count = 16, Denominator = 4, AreaCode = "b" }
            };

            Assert.AreEqual(340, new CcgAverageCalculator(data, GetCcgPopulation(), Metadata(100)).Average.Value);
        }

        private static IndicatorMetadata Metadata(int unitValue)
        {
            return new IndicatorMetadata
            {
                ValueTypeId = ValueTypeId.Proportion,
                Unit = new Unit { Value = unitValue }
            };
        }

        private CcgPopulation GetCcgPopulation()
        {
            return new CcgPopulation
            {
                TotalPopulation = 11,
                PracticeCodeToPopulation = new Dictionary<string, double> { { "a", 1 }, { "b", 10 } },
                AreaCode = "parent"
            };
        }


    }
}
