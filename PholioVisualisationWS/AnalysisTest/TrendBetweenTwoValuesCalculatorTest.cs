using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class TrendBetweenTwoValuesCalculatorTest
    {
        [TestMethod]
        public void TestComparingLowerRecentWithHigherPrevious()
        {
            var trendMarkerResult = new TrendMarkerResult();
            new TrendBetweenTwoValuesCalculator().SetTrendMarker(
                new CoreDataSet { Value = 1, LowerCI = 0.9, UpperCI = 1.1 },
                new CoreDataSet { Value = 2, LowerCI = 1.9, UpperCI = 2.1 },
                trendMarkerResult);

            Assert.AreEqual(TrendMarker.Decreasing,
                trendMarkerResult.MarkerForMostRecentValueComparedWithPreviousValue);
        }

        [TestMethod]
        public void TestComparingOverlappingRecentAndPrevious()
        {
            var trendMarkerResult = new TrendMarkerResult();
            new TrendBetweenTwoValuesCalculator().SetTrendMarker(
                new CoreDataSet { Value = 1, LowerCI = 0.9, UpperCI = 1.6 },
                new CoreDataSet { Value = 2, LowerCI = 1.5, UpperCI = 2.1 },
                trendMarkerResult);

            Assert.AreEqual(TrendMarker.NoChange,
                trendMarkerResult.MarkerForMostRecentValueComparedWithPreviousValue);
        }

        [TestMethod]
        public void TestNotCalculatedWithNulls()
        {
            var trendMarkerResult = new TrendMarkerResult();
            new TrendBetweenTwoValuesCalculator().SetTrendMarker(null, null,
                trendMarkerResult);

            Assert.AreEqual(TrendMarker.CannotBeCalculated,
                trendMarkerResult.MarkerForMostRecentValueComparedWithPreviousValue);
        }

        [TestMethod]
        public void TestNotCalculatedIfMissingCI()
        {
            var trendMarkerResult = new TrendMarkerResult();
            new TrendBetweenTwoValuesCalculator().SetTrendMarker(
                new CoreDataSet { Value = 1, LowerCI =ValueData.NullValue, UpperCI = ValueData.NullValue },
                new CoreDataSet { Value = 2, LowerCI = 1.5, UpperCI = 2.1 },
                trendMarkerResult);

            Assert.AreEqual(TrendMarker.CannotBeCalculated,
                trendMarkerResult.MarkerForMostRecentValueComparedWithPreviousValue);
        }

        [TestMethod]
        public void TestDataListIsSorted()
        {
            var dataList = new List<CoreDataSet>
            {
                new CoreDataSet { Value = 2, LowerCI = 1.9, UpperCI = 2.1, Year = 2000 },
                new CoreDataSet { Value = 2, LowerCI = 1.9, UpperCI = 2.1, Year = 2002 },
                new CoreDataSet { Value = 1, LowerCI = 0.9, UpperCI = 1.1, Year = 2003 },
                new CoreDataSet { Value = 2, LowerCI = 1.5, UpperCI = 2.1, Year = 2001 }
            };

            var trendMarkerResult = new TrendMarkerResult();
            new TrendBetweenTwoValuesCalculator().SetTrendMarkerFromDataList(dataList, trendMarkerResult);

            Assert.AreEqual(TrendMarker.Decreasing,
                trendMarkerResult.MarkerForMostRecentValueComparedWithPreviousValue);
        }
    }
}
