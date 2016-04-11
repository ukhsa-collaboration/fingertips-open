
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class TrendDataReaderTest
    {
        [TestMethod]
        public void TestYearlyData()
        {
            TrendDataReader reader = ReaderFactory.GetTrendDataReader();

            Grouping grouping = new Grouping
            {
                IndicatorId = IndicatorIds.ObesityYear6,
                BaselineYear = 2006,
                BaselineQuarter = -1,
                DataPointYear = 2009,
                YearRange = 1,
                DataPointQuarter = -1,
                SexId = SexIds.Persons,
                AgeId = AgeIds.From10To11
            };

            IList<CoreDataSet> data = reader.GetTrendData(grouping, 
                AreaCodes.CountyUa_Leicestershire);

            Assert.AreEqual(4, data.Count);

            // Assert order is correct
            for (int i = 0; i < data.Count; i++)
            {
                Assert.AreEqual(2006 + i, data[i].Year);
            }

            foreach (CoreDataSet coreDataSet in data)
            {
                Assert.AreEqual(1, coreDataSet.YearRange);
                Assert.AreEqual(-1, coreDataSet.Quarter);
            }
        }

        [TestMethod]
        public void TestQuarterlyData()
        {
            TrendDataReader reader = ReaderFactory.GetTrendDataReader();

            Grouping grouping = new Grouping
            {
                IndicatorId = IndicatorIds.AdultSmokingPrevalence,
                BaselineYear = 2009,
                BaselineQuarter = 2,
                DataPointYear = 2010,
                YearRange = 1,
                DataPointQuarter = 3,
                SexId = SexIds.Persons,
                AgeId = AgeIds.Over18
            };

            IList<CoreDataSet> data = reader.GetTrendData(grouping, AreaCodes.CountyUa_Cambridgeshire);

            Assert.AreEqual(6, data.Count);

            // Assert order is correct
            int year = grouping.BaselineYear;
            int quarter = grouping.BaselineQuarter;
            for (int i = 0; i < data.Count; i++)
            {
                if (quarter > 4)
                {
                    quarter = 1;
                    year++;
                }

                Assert.AreEqual(year, data[i].Year);
                Assert.AreEqual(quarter, data[i].Quarter);

                quarter++;
            }
        }

        [TestMethod]
        public void TestMonthlyData()
        {
            /* DF 19-12-2011 : test fails because data in database incorrect, 2011 data has been entered with year 2011 */

            TrendDataReader reader = ReaderFactory.GetTrendDataReader();

            Grouping grouping = new Grouping
            {
                IndicatorId = 734,
                BaselineYear = 2008,
                BaselineQuarter = -1,
                BaselineMonth = 6,
                DataPointYear = 2011,
                YearRange = 1,
                DataPointQuarter = -1,
                DataPointMonth = 1,
                SexId = 4,
                AgeId = 204
            };

            IList<CoreDataSet> data = reader.GetTrendData(grouping, AreaCodes.CountyUa_CityOfLondon);

            Assert.AreEqual(32, data.Count);

            StringBuilder sb = new StringBuilder();

            // Assert order is correct
            int year = grouping.BaselineYear;
            int month = grouping.BaselineMonth;
            for (int i = 0; i < data.Count; i++)
            {
                sb.AppendLine(string.Join(" ", data[i].Year, data[i].YearRange, data[i].Quarter, data[i].Month));

                if (month > 12)
                {
                    month = 1;
                    year++;
                }

                Assert.AreEqual(year, data[i].Year);
                Assert.AreEqual(month, data[i].Month);

                month++;
            }
        }

        [TestMethod]
        public void TestMin()
        {
            TrendDataReader reader = ReaderFactory.GetTrendDataReader();
            IList<string> areaCodes = new[]
            {
                AreaCodes.CountyUa_Cambridgeshire, 
                AreaCodes.CountyUa_Cumbria
            };
            var min = reader.GetMin(GetLimitsGrouping(), areaCodes);
            Assert.IsTrue(min.Value < 20);
        }

        [TestMethod]
        public void TestMax()
        {
            TrendDataReader reader = ReaderFactory.GetTrendDataReader();
            IList<string> areaCodes = new[]
            {
                AreaCodes.CountyUa_Cambridgeshire,
                AreaCodes.CountyUa_Cumbria
            };
            var max = reader.GetMax(GetLimitsGrouping(), areaCodes).Value;
            Assert.IsTrue(max > 20);
        }

        [TestMethod]
        public void TestMinWithNoResults()
        {
            TrendDataReader reader = ReaderFactory.GetTrendDataReader();
            IList<string> areaCodes = new[] { AreaCodes.Pct_Norfolk, AreaCodes.Pct_MidEssex };
            Grouping g = GetLimitsGrouping();
            g.IndicatorId = -1;
            Assert.IsFalse(reader.GetMin(g, areaCodes).HasValue);
        }

        [TestMethod]
        public void TestMaxWithNoResults()
        {
            TrendDataReader reader = ReaderFactory.GetTrendDataReader();
            IList<string> areaCodes = new[] { AreaCodes.Pct_Norfolk, AreaCodes.Pct_MidEssex };
            Grouping g = GetLimitsGrouping();
            g.IndicatorId = -1;
            Assert.IsFalse(reader.GetMax(g, areaCodes).HasValue);
        }

        [TestMethod]
        public void TestGetTrendDataForSpecificCategory()
        {
            TrendDataReader reader = ReaderFactory.GetTrendDataReader();
            Grouping grouping = new Grouping
            {
                SexId = SexIds.Persons,
                IndicatorId = IndicatorIds.PercentageOfPeoplePerDeprivationQuintile,
                AgeId = AgeIds.AllAges,
                YearRange = 1,
                BaselineYear = 2010,
                DataPointYear = 2012
            };

            IList<CoreDataSet> data = reader.GetTrendDataForSpecificCategory(grouping,
                AreaCodes.CountyUa_Buckinghamshire,
                CategoryTypeIds.LsoaDeprivationQuintilesInEngland, 1);

            Assert.AreEqual(2, data.Count);
        }

        private static Grouping GetLimitsGrouping()
        {
            return new Grouping
            {
                IndicatorId = IndicatorIds.ObesityYear6,
                BaselineYear = 2006,
                BaselineQuarter = -1,
                BaselineMonth = -1,
                DataPointYear = 2011,
                YearRange = 1,
                DataPointQuarter = -1,
                DataPointMonth = -1,
                SexId = SexIds.Persons,
                AgeId = AgeIds.From10To11
            };
        }
    }
}
