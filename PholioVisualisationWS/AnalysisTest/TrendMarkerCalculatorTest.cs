using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis.TrendMarkers;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class TrendMarkerCalculatorTest
    {
        private TrendMarkerCalculator _trendMarkerCalculator;

        [TestInitialize]
        public void Init()
        {
            _trendMarkerCalculator = new TrendMarkerCalculator();
        }

        [TestMethod]
        public void TestCalculateTrendInvalidTrendRequest()
        {
            // Arrange
            var trendRequest = GetTrendRequest1();
            trendRequest.YearRange = 3;

            // Act
            var trendMarkerResult = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            Assert.IsTrue(trendMarkerResult.Message.Length > 0);
            Assert.IsFalse(trendMarkerResult.IsSignificant);
        }

        [TestMethod]
        public void TestIsSignificantReturnsTrue()
        {
            // Arrange
            var trendRequest = GetTrendRequest1();

            // Act
            var trendMarkerResult = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            Assert.IsTrue(trendMarkerResult.IsSignificant);
            Assert.IsTrue(Math.Round(trendMarkerResult.ChiSquare, 2) == 46.77);
        }

        [TestMethod]
        public void TestIsSignificantReturnsFalse()
        {
            // Arrange
            var trendRequest = GetTrendRequest2();

            // Act
            var trendMarkerResult = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            Assert.IsFalse(trendMarkerResult.IsSignificant);
        }

        [TestMethod]
        public void TestIsSignificantForProportionsReturnsTrue()
        {
            // Arrange
            var trendRequest = GetTrendRequestForProportions1();

            // Act
            var trendMarkerResult = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            Assert.IsTrue(trendMarkerResult.IsSignificant);
            Assert.IsTrue(Math.Round(trendMarkerResult.ChiSquare, 2) == 47.9);
        }

        [TestMethod]
        public void TestIsSignificantForProportionsCannotBeCalculated()
        {
            // Arrange
            var trendRequest = GetTrendRequestForProportions2();

            // Act
            var trendMarkerResult = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            Assert.IsFalse(trendMarkerResult.IsSignificant);
            Assert.IsTrue(trendMarkerResult.ChiSquare.CompareTo(0.0) == 0);
            Assert.IsTrue(trendMarkerResult.Marker == TrendMarker.CannotBeCalculated);
        }

        private TrendRequest GetTrendRequest1(int valueType = ValueTypeIds.Proportion)
        {
            return new TrendRequest()
            {
                UnitValue = 1,
                ValueTypeId = valueType,
                Data = GetTrendData_Set1(),
                YearRange = 1,
                Grouping = new Grouping()
            };
        }

        private TrendRequest GetTrendRequest2(int valueType = ValueTypeIds.Proportion)
        {
            return new TrendRequest()
            {
                UnitValue = 1,
                ValueTypeId = valueType,
                Data = GetTrendData_Set2(),
                YearRange = 1,
                Grouping = new Grouping()
            };
        }

        private TrendRequest GetTrendRequestForProportions1()
        {
            var grouping = new Grouping()
            {
                ComparatorMethodId = ComparatorMethodIds.SpcForProportions
            };

            var request = new TrendRequest()
            {
                UnitValue = 1,
                ValueTypeId = ValueTypeIds.Proportion,
                Data = GetTrendDataForProportion_Set1(),
                YearRange = 1,
                Grouping = grouping
            };

            return request;
        }

        private TrendRequest GetTrendRequestForProportions2()
        {
            var grouping = new Grouping()
            {
                ComparatorMethodId = ComparatorMethodIds.SpcForProportions
            };

            var request = new TrendRequest()
            {
                UnitValue = 1,
                ValueTypeId = ValueTypeIds.Proportion,
                Data = GetTrendDataForProportion_Set2(),
                YearRange = 1,
                Grouping = grouping
            };

            return request;
        }

        private IEnumerable<CoreDataSet> GetTrendData_Set1()
        {
            return new List<CoreDataSet>()
            {
                new CoreDataSet { Count = 1367, Denominator = 44701, Value = 30.6, LowerCI95 = 28.98, UpperCI95 = 32.25, Year = 2018 },
                new CoreDataSet { Count = 1332, Denominator = 44064, Value = 30.2, LowerCI95 = 28.63, UpperCI95 = 31.9, Year = 2017 },
                new CoreDataSet { Count = 1199, Denominator = 42811, Value = 28, LowerCI95 = 26.44, UpperCI95 = 29.64, Year = 2016 },
                new CoreDataSet { Count = 1023, Denominator = 41527, Value = 24.6, LowerCI95 = 23.15, UpperCI95 = 26.19, Year = 2015 },
                new CoreDataSet { Count = 994, Denominator = 40299, Value = 24.67, LowerCI95 = 23.16, UpperCI95 = 26.25, Year = 2014 }
            };
        }

        private IEnumerable<CoreDataSet> GetTrendData_Set2()
        {
            return new List<CoreDataSet>()
            {
                new CoreDataSet { Count = 209, Denominator = 1017, Value = 20.5506, LowerCI95 = 18.1803, UpperCI95 = 23.1427, Year = 2014 },
                new CoreDataSet { Count = 213, Denominator = 1087, Value = 19.5952, LowerCI95 = 17.3444, UpperCI95 = 22.0602, Year = 2015 },
                new CoreDataSet { Count = 213, Denominator = 1119, Value = 19.0349, LowerCI95 = 16.8421, UpperCI95 = 21.4394, Year = 2016 },
                new CoreDataSet { Count = 239, Denominator = 1117, Value = 21.3966, LowerCI95 = 19.0918, UpperCI95 = 23.8975, Year = 2017 },
                new CoreDataSet { Count = 210, Denominator = 1139, Value = 18.4372, LowerCI95 = 16.2925, UpperCI95 = 20.7941, Year = 2018 }
            };
        }

        private IEnumerable<CoreDataSet> GetTrendDataForProportion_Set1()
        {
            return new List<CoreDataSet>()
            {
                new CoreDataSet { Count = 1367, Denominator = 44701, Value = 30.6, LowerCI95 = 28.98, UpperCI95 = 32.25, Year = 2018 },
                new CoreDataSet { Count = 1332, Denominator = 44064, Value = 30.2, LowerCI95 = 28.63, UpperCI95 = 31.9, Year = 2017 },
                new CoreDataSet { Count = 1199, Denominator = 42811, Value = 28, LowerCI95 = 26.44, UpperCI95 = 29.64, Year = 2016 },
                new CoreDataSet { Count = 1023, Denominator = 41527, Value = 24.6, LowerCI95 = 23.15, UpperCI95 = 26.19, Year = 2015 },
                new CoreDataSet { Count = 994, Denominator = 40299, Value = 24.67, LowerCI95 = 23.16, UpperCI95 = 26.25, Year = 2014 }
            };
        }

        private IEnumerable<CoreDataSet> GetTrendDataForProportion_Set2()
        {
            return new List<CoreDataSet>()
            {
                new CoreDataSet { Count = 5, Denominator = 5, Value = 100, LowerCI95 = 0, UpperCI95 = 0, Year = 2012 },
                new CoreDataSet { Count = 3, Denominator = 5, Value = 60, LowerCI95 = 0, UpperCI95 = 0, Year = 2013 },
                new CoreDataSet { Count = 1, Denominator = 5, Value = 20, LowerCI95 = 0, UpperCI95 = 0, Year = 2014 },
                new CoreDataSet { Count = 1, Denominator = 4, Value = 25, LowerCI95 = 0, UpperCI95 = 0, Year = 2015 },
                new CoreDataSet { Count = 2, Denominator = 4, Value = 50, LowerCI95 = 0, UpperCI95 = 0, Year = 2016 }
            };
        }
    }
}