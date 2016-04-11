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

        private const double FaultTolerance = 0.00005;

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

            // AssertWriteResult(result);
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
        public void GetResults_Return_SignificanceValue_For_SufficientTrendData()
        {
            // Arrange
            const int totalRecords = 5;

            var trendData = GetTrendData_1().Take(totalRecords);
            var trendRequest = GetTrendRequest(trendData);

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsTrue(result.Marker == TrendMarker.NoChange
                          && result.NumberOfPointsUsedInCalculation == totalRecords
                          );
        }


        [TestMethod]
        public void GetResults_Return_Correct_SignificanceValue_For_TrendData_1()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_1());

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsTrue(result.IsSignificant
                    && Math.Abs(Math.Round(result.ChiSquare, 4) - 8.0237) < FaultTolerance
                );
        }


        [TestMethod]
        public void GetResults_Return_DecreasingTrend_For_Proportion_And_TrendData_1()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_1());

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsTrue(result.Slope.HasValue
                    && Math.Abs(Math.Round(result.Slope.Value, 4) - (-0.2326)) < FaultTolerance
                    && result.Marker == TrendMarker.Decreasing
                );
        }

        [TestMethod]
        public void GetResults_Return_DecreasingTrend_For_Proportion_And_Shuffled_TrendData_1()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_1_Shuffled());

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsTrue(result.Slope.HasValue
                    && Math.Abs(Math.Round(result.Slope.Value, 4) - (-0.2326)) < FaultTolerance
                    && result.Marker == TrendMarker.Decreasing
                );
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
            Assert.IsTrue(result.Slope.HasValue
                    && Math.Abs(Math.Round(result.Slope.Value, 4) - (-0.2003)) < FaultTolerance
                    && result.Marker == TrendMarker.Decreasing
                );
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
            Assert.IsTrue(result.Slope.HasValue
                    && result.Marker == TrendMarker.Increasing
                );
        }


        [TestMethod]
        public void GetResults_Return_IncreasingTrend_For_Proportion_And_TrendData_3()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_3());

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsTrue(result.IsSignificant
                    && result.Marker == TrendMarker.Increasing
                );
        }

        [TestMethod]
        public void GetResults_Return_IncreasingTrend_For_Proportion_And_TrendData_4()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_4());

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);
            Assert.IsTrue(result.IsSignificant
                    && result.Marker == TrendMarker.Increasing
                );
        }

        [TestMethod]
        public void GetResults_Return_IncreasingTrend_For_Proportion_And_TrendData_5()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_5());

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);

            Assert.IsTrue(Math.Abs(Math.Round(result.ChiSquare, 4) - 0.018) < FaultTolerance
                    && result.NumberOfPointsUsedInCalculation == 5
                );
        }

        [TestMethod]
        public void GetResults_Return_IncreasingTrend_For_Proportion_And_TrendData_6()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_6());

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);

            Assert.IsTrue(Math.Abs(Math.Round(result.ChiSquare, 4) - 1.3987) < FaultTolerance
                          && result.NumberOfPointsUsedInCalculation == 6
                          );
        }


        [TestMethod]
        public void GetResults_Return_IncreasingTrend_For_Proportion_And_TrendData_7()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_7());

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);

            Assert.IsTrue(Math.Abs(Math.Round(result.ChiSquare, 4) - 0.3925) < FaultTolerance
                            && result.NumberOfPointsUsedInCalculation == 7
                            );
        }


        [TestMethod]
        public void GetResults_Return_IncreasingTrend_For_Proportion_And_TrendData_5a()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_5a());

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);

            Assert.IsTrue(result.IsSignificant == true 
                    && Math.Abs(Math.Round(result.ChiSquare, 4) - 978.2845) < FaultTolerance
                    && result.NumberOfPointsUsedInCalculation == 5
                );
        }


        [TestMethod]
        public void GetResults_Return_IncreasingTrend_For_Proportion_And_TrendData_6a()
        {
            // Arrange
            var trendRequest = GetTrendRequest(GetTrendData_6a());

            // Act
            var result = _trendMarkerCalculator.GetResults(trendRequest);

            // Assert
            WriteResult(result);

            Assert.IsTrue(result.IsSignificant == true
                  );
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

            Assert.IsTrue(result.IsSignificant == true
                  );
        }


        private void WriteResult(TrendResponse result)
        {
            Console.WriteLine("Is Significant? " + result.IsSignificant);

            Console.WriteLine("ChiSquare: " + result.ChiSquare);

            Console.WriteLine(result.Slope.HasValue
                ? "Slope: " + result.Slope.Value
                : "No Slope");

            Console.WriteLine(result.Intercept.HasValue
                ? "Intercept: " + result.Intercept.Value
                : "No Intercept");


            Console.WriteLine("Trend Marker: {0}", result.Marker);

            Console.WriteLine("Number of Points used: {0}", result.NumberOfPointsUsedInCalculation);
        }

        private TrendRequest GetTrendRequest(IEnumerable<CoreDataSet > trendData, int valueType = ValueTypeIds.Proportion, double comparatorConfidence = 95.0)
        {

            return new TrendRequest()
            {
                ValueTypeId = valueType,
                ComparatorConfidence = comparatorConfidence,
                Data = trendData,
                YearRange = 1
            };
        }

        private IEnumerable<CoreDataSet > GetTrendData_1()
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
            return new List<CoreDataSet>()
            {
                new CoreDataSet {Count = 8267, Denominator = 90152.00, Value = 9.17007, Year = 2001},
                new CoreDataSet {Count = 8351, Denominator = 89993.00, Value = 9.27961, Year = 2002},
                new CoreDataSet {Count = 8444, Denominator = 90134.19, Value = 9.36827, Year = 2003},
                new CoreDataSet {Count = 8586, Denominator = 90317.126, Value = 9.50652, Year = 2004},
                new CoreDataSet {Count = 8519, Denominator = 90457.00, Value = 9.41773, Year = 2005},
                new CoreDataSet {Count = 8344, Denominator = 90781, Value = 9.19135, Year = 2006},
                new CoreDataSet {Count = 8250, Denominator = 90969, Value = 9.06902, Year = 2007},
                new CoreDataSet {Count = 8109, Denominator = 91379, Value = 8.87403, Year = 2008},
                new CoreDataSet {Count = 8116, Denominator = 91530, Value = 8.86704, Year = 2009},
                new CoreDataSet {Count = 8145, Denominator = 91773, Value = 8.87516, Year = 2010},
                new CoreDataSet {Count = 8222, Denominator = 92088, Value = 8.92842, Year = 2011},
                new CoreDataSet {Count = 8506, Denominator = 92238, Value = 9.2218, Year = 2012},
                new CoreDataSet {Count = 8728, Denominator = 92665.890, Value = 9.41887, Year = 2013}
            };

        }

        // increasing trend
        private IEnumerable<CoreDataSet> GetTrendData_4()
        {
            return ToCoreDataList(new List<CoreDataSet>()
            {
                new CoreDataSet {Count = 5, Denominator = 22, Year = 2012},
                new CoreDataSet {Count = 7, Denominator = 35, Year = 2011},
                new CoreDataSet {Count = 6, Denominator = 42, Year = 2010},
                new CoreDataSet {Count = 8, Denominator = 50, Year = 2009},
                new CoreDataSet {Count = 8, Denominator = 100, Year = 2008},
                new CoreDataSet {Count = 9, Denominator = 200, Year = 2007},
            });
        }



        // increasing trend
        private IEnumerable<CoreDataSet> GetTrendData_5()
        {
            return ToCoreDataList(new List<CoreDataSet>()
            {
                new CoreDataSet {Count = 5405, Denominator = 18095, Year = 2008},
                new CoreDataSet {Count = 5500, Denominator = 18170, Year = 2009},
                new CoreDataSet {Count = 5435, Denominator = 18005, Year = 2010},
                new CoreDataSet {Count = 5480, Denominator = 17890, Year = 2011},
                new CoreDataSet {Count = 5315, Denominator = 17860, Year = 2012},
            });
        }


        private IEnumerable<CoreDataSet> GetTrendData_6()
        {
            return ToCoreDataList(new List<CoreDataSet>()
            {
                new CoreDataSet {Count = 5645, Denominator = 18285, Year = 2007},
                new CoreDataSet {Count = 5405, Denominator = 18095, Year = 2008},
                new CoreDataSet {Count = 5500, Denominator = 18170, Year = 2009},
                new CoreDataSet {Count = 5435, Denominator = 18005, Year = 2010},
                new CoreDataSet {Count = 5480, Denominator = 17890, Year = 2011},
                new CoreDataSet {Count = 5315, Denominator = 17860, Year = 2012},
            });
         }
        
        // increasing trend
        private IEnumerable<CoreDataSet> GetTrendData_7()
        {
            return ToCoreDataList(new List<CoreDataSet>()
            {
                new CoreDataSet {Count = 5455, Denominator = 18590, Year = 2006},
                new CoreDataSet {Count = 5645, Denominator = 18285, Year = 2007},
                new CoreDataSet {Count = 5405, Denominator = 18095, Year = 2008},
                new CoreDataSet {Count = 5500, Denominator = 18170, Year = 2009},
                new CoreDataSet {Count = 5435, Denominator = 18005, Year = 2010},
                new CoreDataSet {Count = 5480, Denominator = 17890, Year = 2011},
                new CoreDataSet {Count = 5315, Denominator = 17860, Year = 2012},
            });
        }


      
        private IEnumerable<CoreDataSet> GetTrendData_5a()
        {
            return ToCoreDataList(new List<CoreDataSet>()
            {
                new CoreDataSet {Count = 22435, Denominator = 65880, Year = 2008},
                new CoreDataSet {Count = 22445, Denominator = 67900, Year = 2009},
                new CoreDataSet {Count = 21365, Denominator = 68705, Year = 2010},
                new CoreDataSet {Count = 20955, Denominator = 69835, Year = 2011},
                new CoreDataSet {Count = 19055, Denominator = 70910, Year = 2012},
            });
        }

        private IEnumerable<CoreDataSet> GetTrendData_6a()
        {
            return ToCoreDataList(new List<CoreDataSet>()
            {
                new CoreDataSet {Count = 23490, Denominator = 64830, Year = 2007},
                new CoreDataSet {Count = 22435, Denominator = 65880, Year = 2008},
                new CoreDataSet {Count = 22445, Denominator = 67900, Year = 2009},
                new CoreDataSet {Count = 21365, Denominator = 68705, Year = 2010},
                new CoreDataSet {Count = 20955, Denominator = 69835, Year = 2011},
                new CoreDataSet {Count = 19055, Denominator = 70910, Year = 2012},
            });
        }


        private IEnumerable<CoreDataSet> GetTrendData_7a()
        {
            return ToCoreDataList(new List<CoreDataSet>()
            {
                new CoreDataSet {Count = 22460, Denominator = 64600, Year = 2006},
                new CoreDataSet {Count = 23490, Denominator = 64830, Year = 2007},
                new CoreDataSet {Count = 22435, Denominator = 65880, Year = 2008},
                new CoreDataSet {Count = 22445, Denominator = 67900, Year = 2009},
                new CoreDataSet {Count = 21365, Denominator = 68705, Year = 2010},
                new CoreDataSet {Count = 20955, Denominator = 69835, Year = 2011},
                new CoreDataSet {Count = 19055, Denominator = 70910, Year = 2012},
            });
        }
        
        private static IEnumerable<CoreDataSet> ToCoreDataList( IEnumerable<CoreDataSet> data)
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
       

       private TrendRequest GetTrendRequest(int valueType = ValueTypeIds.Proportion, double comparatorConfidence = 95.0)
        {
            return new TrendRequest()
            {
                ValueTypeId = valueType,
                ComparatorConfidence = comparatorConfidence,
                YearRange = 1
            };

        }
    }

  

}
