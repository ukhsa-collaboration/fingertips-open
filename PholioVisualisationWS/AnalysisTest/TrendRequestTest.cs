using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    /// <summary>
    /// Test helper methods
    /// </summary>
    [TestClass]
    public class TrendRequestTest
    {
        [TestMethod]
        public void TestTrendRequestIsInvalidForYearRangeGreaterThan1()
        {
            var trendRequest = new TrendRequest()
            {
                ValueTypeId = ValueTypeIds.Proportion,
                YearRange = 3
            };

            string validationMessage = null;
            trendRequest.IsValid(ref validationMessage);
            Assert.IsTrue(validationMessage == "Year Range 3 is not relevant for the calculation");
        }

        [TestMethod]
        public void TestTrendRequestIsInvalidForNonRelevantValueType()
        {
            var trendRequest = new TrendRequest()
            {
                ValueTypeId = ValueTypeIds.Count,
                YearRange = 1
            };

            string validationMessage = null;
            trendRequest.IsValid(ref validationMessage);
            Assert.IsTrue(validationMessage == "Value Type 7 is not relevant for the calculation");
        }

    }
}
