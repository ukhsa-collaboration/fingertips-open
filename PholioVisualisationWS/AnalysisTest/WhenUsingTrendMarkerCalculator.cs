using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.AnalysisTest
{
    [TestClass]
    public class WhenUsingTrendMarkerCalculator
    {

        private TrendMarkerCalculator _trendMarkerCalculator;

        [TestInitialize]
        public void Init()
        {
            _trendMarkerCalculator = new TrendMarkerCalculator();
        }

        [TestMethod]
        public void GetResults_Return_No_SignificanceValue_For_NullTrendData()
        {
            // Arrange
            var trendRequest = GetTrendRequest();

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // AssertWriteResult(markerResult);
            Assert.IsTrue(result.IsSignificant == false
                          && result.Marker == TrendMarker.CannotBeCalculated);
        }

        [TestMethod]
        public void GetResults_Return_No_SignificanceValue_For_InsufficientTrendData()
        {
            // Arrange
            const int totalRecords = 4;
            var trendData = GetTrendData_1().Take(totalRecords);
            var trendRequest = GetTrendRequest(trendData);
            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsTrue(result.IsSignificant == false
                          && result.Marker == TrendMarker.CannotBeCalculated
                          && result.NumberOfPointsUsedInCalculation == 0
                          );
        }

        [TestMethod]
        public void GetResults_Return_Correct_SignificanceValue_For_TrendData_1()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_1(), ValueTypeIds.Proportion, 100);

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsTrue(result.IsSignificant);
        }


        [TestMethod]
        public void GetResults_Return_DecreasingTrend_For_Proportion_And_TrendData_1()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_1(), ValueTypeIds.Proportion, 100);

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsTrue(result.Slope.HasValue);
            Assert.IsTrue(result.Marker == TrendMarker.Decreasing);
        }

        [TestMethod]
        public void GetResults_Return_DecreasingTrend_For_Proportion_And_Shuffled_TrendData_1()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_1_Shuffled(), ValueTypeIds.Proportion, 100);

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsTrue(result.Slope.HasValue);
            Assert.AreEqual(TrendMarker.Decreasing, result.Marker);
        }


        [TestMethod]
        public void GetResults_Return_IncreasingTrend_For_Proportion_And_TrendData_2()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_2());

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsTrue(result.Slope.HasValue
                    && result.Marker == TrendMarker.Increasing
                );
        }

        [TestMethod]
        public void GetResults_Return_DecreasingTrend_For_CrudeRate_And_TrendData_1()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_1(), ValueTypeIds.CrudeRate);

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsTrue(result.Slope.HasValue);
            Assert.AreEqual(TrendMarker.Decreasing, result.Marker);
        }

        [TestMethod]
        public void GetResults_Return_IncreasingTrend_For_CrudeRate_And_TrendData_2()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_2(), ValueTypeIds.CrudeRate);

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsTrue(result.Slope.HasValue);
            Assert.IsTrue(result.Marker == TrendMarker.Increasing);
        }


        [TestMethod]
        public void GetResults_Return_IncreasingTrend_For_Proportion_And_TrendData_3()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_3(), ValueTypeIds.Proportion, 100);

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsTrue(result.IsSignificant);
            Assert.AreEqual(TrendMarker.Increasing, result.Marker);
        }

        [TestMethod]
        public void GetResults_Return_IncreasingTrend_For_Proportion_And_TrendData_4()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_4(), ValueTypeIds.Proportion, 100);

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsTrue(result.IsSignificant);
            Assert.IsTrue(result.Marker == TrendMarker.Decreasing);
        }

        [TestMethod]
        public void GetResults_Return_No_Change_For_Proportion_And_TrendData_5()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_5(), ValueTypeIds.Proportion, 100D);

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);

            Assert.IsTrue(result.NumberOfPointsUsedInCalculation == 5);
            Assert.AreEqual(TrendMarker.NoChange, result.Marker);
        }

        [TestMethod]
        public void GetResults_Return_No_Change_For_Proportion_And_TrendData_6()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_6());

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);

            Assert.IsTrue(result.NumberOfPointsUsedInCalculation == 6);
            Assert.AreEqual(TrendMarker.NoChange, result.Marker);
        }


        [TestMethod]
        public void GetResults_Return_No_Change_For_Proportion_And_TrendData_7()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_7());

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);

            Assert.IsTrue(result.NumberOfPointsUsedInCalculation == 7);
            Assert.AreEqual(TrendMarker.NoChange, result.Marker);
        }


        [TestMethod]
        public void GetResults_Return_DecreasingTrend_For_Proportion_And_TrendData_5a()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_5a(), ValueTypeIds.Proportion, 100D);

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);

            Assert.IsTrue(result.IsSignificant);
            Assert.IsTrue(result.NumberOfPointsUsedInCalculation == 5);
            Assert.AreEqual(TrendMarker.Decreasing, result.Marker);
        }

        [TestMethod]
        public void GetResults_Return_IncreasingTrend_For_Proportion_And_TrendData_7a()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_7a());

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);

            Assert.IsTrue(result.IsSignificant);
            Assert.AreEqual(TrendMarker.Decreasing, result.Marker);
        }

        [TestMethod]
        public void GetResults_Return_IncreasingTrend_For_Proportion_And_TrendData_8()
        {
            // Arrange
            var values = GetTrendData_8();
            var unitValue = 100000;
            var trendRequest = GetTrendRequest(values, ValueTypeIds.DirectlyStandardisedRate, unitValue);

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsFalse(result.IsSignificant);
            Assert.AreEqual(1.4948, Math.Round(result.ChiSquare, 4));
            Assert.AreEqual(TrendMarker.NoChange, result.Marker);
        }

        [TestMethod]
        public void GetResults_Return_Not_Calculated_For_Invalid_Values()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_9());

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsFalse(result.IsSignificant);
            Assert.AreEqual(TrendMarker.CannotBeCalculated, result.Marker);
        }

        private void WriteResult(TrendMarkerResult markerResult)
        {
            Console.WriteLine("Is Significant? " + markerResult.IsSignificant);

            Console.WriteLine("ChiSquare: " + markerResult.ChiSquare);

            Console.WriteLine(markerResult.Slope.HasValue
                ? "Slope: " + markerResult.Slope.Value
                : "No Slope");

            Console.WriteLine(markerResult.Intercept.HasValue
                ? "Intercept: " + markerResult.Intercept.Value
                : "No Intercept");


            Console.WriteLine("Trend Marker: {0}", markerResult.Marker);

            Console.WriteLine("Number of Points used: {0}", markerResult.NumberOfPointsUsedInCalculation);
        }

        private TrendRequest GetTrendRequest(IEnumerable<CoreDataSet> trendData, int valueType = ValueTypeIds.Proportion,
             double unitValue = 1)
        {

            return new TrendRequest()
            {
                UnitValue = unitValue,
                ValueTypeId = valueType,
                Data = trendData,
                YearRange = 1
            };
        }

        private IEnumerable<CoreDataSet> GetTrendData_9()
        {
            return new List<CoreDataSet>()
            {
                new CoreDataSet {Count = ValueData.NullValue, Denominator = ValueData.NullValue, Value = ValueData.NullValue, Year = 1},
                new CoreDataSet {Count = ValueData.NullValue, Denominator = ValueData.NullValue, Value = ValueData.NullValue, Year = 2},
                new CoreDataSet {Count = 11024.38, Denominator = 2315677, Value = 48313, Year = 3},
                new CoreDataSet {Count = 11681.04, Denominator = 2330591, Value = 50802, Year = 4},
                new CoreDataSet {Count = 11738.96, Denominator = 2349351, Value = 50623, Year = 5}
            };
        }

        private IEnumerable<CoreDataSet> GetTrendData_8()
        {
            return new List<CoreDataSet>()
            {
                new CoreDataSet {Count = 10936.06, Denominator = 2252383, Value = 49372, Year = 2008},
                new CoreDataSet {Count = 11274.7, Denominator = 2268354, Value = 50525, Year = 2009},
                new CoreDataSet {Count = 11369.44, Denominator = 2285373, Value = 50551, Year = 2010},
                new CoreDataSet {Count = 11106.21, Denominator = 2300469, Value = 49016, Year = 2011},
                new CoreDataSet {Count = 11024.38, Denominator = 2315677, Value = 48313, Year = 2012},
                new CoreDataSet {Count = 11681.04, Denominator = 2330591, Value = 50802, Year = 2013},
                new CoreDataSet {Count = 11738.96, Denominator = 2349351, Value = 50623, Year = 2014}
            };
        }

        private IEnumerable<CoreDataSet> GetTrendData_1()
        {
            return new List<CoreDataSet>()
            {
                new CoreDataSet() {Count = 5, Denominator = 22, Value = 0.22727*100, Year = 2006},
                new CoreDataSet() {Count = 7, Denominator = 35, Value = 0.20000*100, Year = 2007},
                new CoreDataSet() {Count = 6, Denominator = 42, Value = 0.14286*100, Year = 2008},
                new CoreDataSet {Count = 7, Denominator = 48, Value = 0.14583*100, Year = 2009},
                new CoreDataSet {Count = 8, Denominator = 54, Value = 0.14815*100, Year = 2010},
                new CoreDataSet {Count = 10, Denominator = 150, Value = 0.06667*100, Year = 2011},
            };
        }

        private IEnumerable<CoreDataSet> GetTrendData_1_Shuffled()
        {
            return new List<CoreDataSet>()
            {
                new CoreDataSet {Count = 7, Denominator = 48, Value = 0.14583*100, Year = 2009},
                new CoreDataSet {Count = 5, Denominator = 22, Value = 0.22727*100, Year = 2006},
                new CoreDataSet {Count = 10, Denominator = 150, Value = 0.06667*100, Year = 2011},
                new CoreDataSet {Count = 6, Denominator = 42, Value = 0.14286*100, Year = 2008},
                new CoreDataSet {Count = 7, Denominator = 35, Value = 0.20000*100, Year = 2007},
                new CoreDataSet {Count = 8, Denominator = 54, Value = 0.14815*100, Year = 2010},

            };
        }

        private IEnumerable<CoreDataSet> GetTrendData_2()
        {
            return new List<CoreDataSet>()
                {
                    new CoreDataSet {Count = 10, Denominator = 150, Value = 0.06667 * 100, Year= 2006 },
                    new CoreDataSet {Count = 8, Denominator = 54, Value = 0.14815 * 100, Year = 2007 },
                    new CoreDataSet {Count = 7, Denominator = 48, Value = 0.14583 * 100, Year = 2008 },
                    new CoreDataSet {Count = 6, Denominator = 42, Value = 0.14286 * 100, Year = 2009 },
                    new CoreDataSet {Count = 7, Denominator = 35, Value = 0.20000 * 100, Year = 2010},
                    new CoreDataSet {Count = 5, Denominator = 22, Value = 0.22727 * 100, Year= 2011 },
                };
        }

        private IEnumerable<CoreDataSet> GetTrendData_3()
        {
            // Indicator ID 1172
            return new List<CoreDataSet>()
            {
                new CoreDataSet {Count = 8116, Denominator = 91530, Value = 8.86704, Year = 2009},
                new CoreDataSet {Count = 8145, Denominator = 91773, Value = 8.87516, Year = 2010},
                new CoreDataSet {Count = 8222, Denominator = 92088, Value = 8.92842, Year = 2011},
                new CoreDataSet {Count = 8506, Denominator = 92238, Value = 9.2218, Year = 2012},
                new CoreDataSet {Count = 8728, Denominator = 92665.890, Value = 9.41887, Year = 2013}
            };

        }

        // decreasing trend
        private IEnumerable<CoreDataSet> GetTrendData_4()
        {
            return ToProportionCoreDataList(new List<CoreDataSet>()
            {
                new CoreDataSet {Count = 5, Denominator = 22, Year = 1},
                new CoreDataSet {Count = 7, Denominator = 35, Year = 2},
                new CoreDataSet {Count = 6, Denominator = 42, Year = 3},
                new CoreDataSet {Count = 8, Denominator = 50, Year = 4},
                new CoreDataSet {Count = 8, Denominator = 100, Year = 5},
                new CoreDataSet {Count = 9, Denominator = 200, Year = 6},
            });
            // Values
            //0.23
            //0.20
            //0.14
            //0.16
            //0.08
            //0.05
        }

        private IEnumerable<CoreDataSet> GetTrendData_5()
        {
            // Indicator ID 10101
            return ToProportionCoreDataList(new List<CoreDataSet>()
            {
                new CoreDataSet {Count = 5405, Denominator = 18095, Year = 2008},
                new CoreDataSet {Count = 5500, Denominator = 18170, Year = 2009},
                new CoreDataSet {Count = 5435, Denominator = 18005, Year = 2010},
                new CoreDataSet {Count = 5480, Denominator = 17890, Year = 2011},
                new CoreDataSet {Count = 5315, Denominator = 17860, Year = 2012},
            });

            //Values
            //29.8701298701299
            //30.2696752889378
            //30.1860594279367
            //30.6316377864729
            //29.7592385218365
        }


        private IEnumerable<CoreDataSet> GetTrendData_6()
        {
            var dataList = GetTrendData_5().ToList();
            dataList.Insert(0, new CoreDataSet { Count = 5645, Denominator = 18285, Year = 2007 });
            return ToProportionCoreDataList(dataList);
        }

        private IEnumerable<CoreDataSet> GetTrendData_7()
        {
            var dataList = GetTrendData_6().ToList();
            dataList.Insert(0, new CoreDataSet { Count = 5455, Denominator = 18590, Year = 2006 });
            return ToProportionCoreDataList(dataList);
        }

        /// <summary>
        /// Decreasing trend
        /// </summary>
        private IEnumerable<CoreDataSet> GetTrendData_5a()
        {
            // Indicator ID 10101
            return ToProportionCoreDataList(new List<CoreDataSet>()
            {
                new CoreDataSet {Count = 22435, Denominator = 65880, Year = 2008},
                new CoreDataSet {Count = 22445, Denominator = 67900, Year = 2009},
                new CoreDataSet {Count = 21365, Denominator = 68705, Year = 2010},
                new CoreDataSet {Count = 20955, Denominator = 69835, Year = 2011},
                new CoreDataSet {Count = 19055, Denominator = 70910, Year = 2012},
            });
            //Values
            //34.0543412264724
            //33.0559646539028
            //31.0967178516847
            //30.0064437602921
            //26.8720913834438
        }

        private IEnumerable<CoreDataSet> GetTrendData_7a()
        {
            var dataList = GetTrendData_5a().ToList();

            dataList.Insert(0, new CoreDataSet { Count = 23490, Denominator = 64830, Year = 2007 });
            dataList.Insert(0, new CoreDataSet { Count = 22460, Denominator = 64600, Year = 2006 });

            return ToProportionCoreDataList(dataList);
        }

        private static IEnumerable<CoreDataSet> ToProportionCoreDataList(IEnumerable<CoreDataSet> data)
        {
            return
                data.Select(
                    d =>
                        new CoreDataSet
                        {
                            Count = d.Count,
                            Denominator = d.Denominator,
                            Year = d.Year,
                            Value = Math.Abs(Convert.ToDouble(d.Count) / Convert.ToDouble(d.Denominator) * Convert.ToDouble(100.00))
                        });
        }


        private TrendRequest GetTrendRequest(int valueType = ValueTypeIds.Proportion)
        {
            return new TrendRequest()
            {
                UnitValue = 1,
                ValueTypeId = valueType,
                YearRange = 1
            };

        }
    }



}
