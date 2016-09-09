using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;

namespace PholioVisualisation.AnalysisTest
{
    /// <summary>
    /// Test helper methods
    /// </summary>
    [TestClass]
    public class TrendRequestTest
    {
        [TestMethod]
        public void TestTrendRequest_IsValid_True()
        {
            var trendRequest = new TrendRequest()
            {
                UnitValue = 1,
                ValueTypeId = ValueTypeIds.Proportion,
                YearRange = 1,
                Data = DataList()
            };

            string validationMessage = null;
            var isValid = trendRequest.IsValid(ref validationMessage);
            Assert.IsTrue(isValid, validationMessage);
        }

        [TestMethod]
        public void TestTrendRequest_IsValid_True_If_Enough_Recent_Values()
        {
            var trendRequest = new TrendRequest()
            {
                UnitValue = 1,
                ValueTypeId = ValueTypeIds.Proportion,
                YearRange = 1,
                Data = DataList()
            };

            string validationMessage = null;
            var isValid = trendRequest.IsValid(ref validationMessage);
            Assert.IsTrue(isValid, validationMessage);
        }

        [TestMethod]
        public void TestTrendRequest_IsValid_True_If_Enough_Valid_Values()
        {
            var dataList = DataList();

            // Add invalid data point after 5 more recent points
            dataList.Add(new CoreDataSet
            {
                Value = ValueData.NullValue,
                Denominator = 4,
                Count = 8,
                Year = 2000
            });

            var trendRequest = new TrendRequest()
            {
                UnitValue = 1,
                ValueTypeId = ValueTypeIds.Proportion,
                YearRange = 1,
                Data = dataList
            };

            string validationMessage = null;
            var isValid = trendRequest.IsValid(ref validationMessage);
            Assert.IsTrue(isValid, validationMessage);
        }

        [TestMethod]
        public void TestTrendRequest_IsValid_False_If_Value_Invalid()
        {
            var dataList = DataList();
            dataList[0].Value = ValueData.NullValue;

            var trendRequest = new TrendRequest()
            {
                UnitValue = 1,
                ValueTypeId = ValueTypeIds.Proportion,
                YearRange = 1,
                Data = dataList
            };

            string validationMessage = null;
            trendRequest.IsValid(ref validationMessage);
            Assert.AreEqual("Not enough data points with valid values to calculate recent trend", validationMessage);
        }

        [TestMethod]
        public void TestTrendRequestIsInvalidForYearRangeGreaterThan1()
        {
            var trendRequest = new TrendRequest()
            {
                UnitValue = 1,
                ValueTypeId = ValueTypeIds.Proportion,
                YearRange = 3
            };

            string validationMessage = null;
            trendRequest.IsValid(ref validationMessage);
            Assert.IsTrue(validationMessage == "The recent trend cannot be calculated for this year range");
        }

        [TestMethod]
        public void TestTrendRequestIsInvalidForNonRelevantValueType()
        {
            var trendRequest = new TrendRequest()
            {
                UnitValue = 1,
                ValueTypeId = ValueTypeIds.Count,
                YearRange = 1
            };

            string validationMessage = null;
            trendRequest.IsValid(ref validationMessage);
            Assert.IsTrue(validationMessage == "The recent trend cannot be calculated for this value type");
        }

        private IList<CoreDataSet> DataList()
        {
            return new List<CoreDataSet>
            {
                new CoreDataSet { Value = 2, Denominator = 4, Count = 8, Year = 2001},
                new CoreDataSet { Value = 2, Denominator = 4, Count = 8, Year = 2002},
                new CoreDataSet { Value = 2, Denominator = 4, Count = 8, Year = 2003},
                new CoreDataSet { Value = 2, Denominator = 4, Count = 8, Year = 2004},
                new CoreDataSet { Value = 2, Denominator = 4, Count = 8, Year = 2005}
            };
        }
    }
}
