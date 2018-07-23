using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class PracticeReaderTest
    {
        [TestMethod]
        public void TestShape()
        {
            var reader = new PracticeDataAccess();

            Assert.AreEqual(6, reader.GetShape(AreaCodes.Gp_BermudaBasingstoke).Value);
            Assert.AreEqual(3, reader.GetShape(AreaCodes.Gp_AlbionSurgery).Value);
        }

        [TestMethod]
        public void TestGetCcgDataValue()
        {
            var grouping = new Grouping
            {
                IndicatorId = IndicatorIds.Aged0To4Years,
                SexId = SexIds.Persons,
                AgeId = AgeIds.From0To4
            };

            var reader = new PracticeDataAccess();
            double val = reader.GetPracticeAggregateDataValue(grouping, new TimePeriod { Year = 2011, YearRange = 1 },
                AreaCodes.Ccg_Barnet);

            // Value ~6.8 but changes with composition of CCG
            Assert.IsTrue(val < 7 && val > 6);
        }

        [TestMethod]
        public void TestGetDeprivationDecileDataValue()
        {
            CategoryArea categoryArea = CategoryArea.New(
                CategoryTypeIds.DeprivationDecileGp2015, 5);

            var reader = new PracticeDataAccess();
            double val = reader.GetGpDeprivationDecileDataValue(Aged0To4Years(),
                new TimePeriod { Year = 2011, YearRange = 1 }, categoryArea);

            double val2011Rounded = Math.Round(val, 1);
            Assert.AreEqual(6, val2011Rounded);
        }

        private static Grouping Aged0To4Years()
        {
            var grouping = new Grouping
            {
                IndicatorId = IndicatorIds.Aged0To4Years,
                SexId = SexIds.Persons,
                AgeId = AgeIds.From0To4
            };
            return grouping;
        }

        [TestMethod]
        public void TestGetPracticeCodeToValueMap()
        {
            IDictionary<string, float> map = new PracticeDataAccess().GetPracticeCodeToValidValueMap(
                IndicatorIds.EstimatedPrevalenceOfCHD,
                new TimePeriod { Year = 2011, YearRange = 1 }, SexIds.Persons);

            Assert.AreEqual(6.021, Math.Round(map[AreaCodes.Gp_CrossfellHealthCentre], 3));
            Assert.IsTrue(map.Count > 7000 && map.Count < 9000);
        }

        [TestMethod]
        public void TestGetPracticeCodeToValueDataMap()
        {
            var grouping = new Grouping
            {
                IndicatorId = IndicatorIds.EstimatedPrevalenceOfCHD,
                SexId = SexIds.Persons,
                AgeId = 1
            };

            IDictionary<string, ValueData> map = new PracticeDataAccess().GetPracticeCodeToValueDataMap(grouping,
                new TimePeriod { Year = 2011, YearRange = 1 }, AreaCodes.Pct_Hounslow);

            Assert.AreEqual(4.056, Math.Round(map[AreaCodes.Gp_Thornbury].Value, 3));
            Assert.IsTrue(map.Count > 45 && map.Count < 60);
        }

        [TestMethod]
        public void TestGetPracticeAggregateDataValueNull()
        {
            double val = new PracticeDataAccess()
                .GetPracticeAggregateDataValue(Aged0To4Years(),
                new TimePeriod { Year = 2001, YearRange = 1 }, 
                AreaCodes.NotAnActualCode);

            Assert.AreEqual(ValueData.NullValue, val);
        }

        [TestMethod]
        public void TestGetPracticeCodeToBaseDataMap()
        {
            var grouping = new Grouping
            {
                IndicatorId = IndicatorIds.AgedOver85Years,
                AgeId = AgeIds.Over85,
                SexId = SexIds.Persons
            };

            Dictionary<string, CoreDataSet> map = new PracticeDataAccess()
                .GetPracticeCodeToBaseDataMap(grouping,
                new TimePeriod { Year = 2011, YearRange = 1 });

            Assert.AreEqual(Math.Round(map[AreaCodes.Gp_Burnham].Value, 3), 2.909);
            Assert.IsTrue(map[AreaCodes.Gp_Burnham].Denominator > 0);
            Assert.IsTrue(map[AreaCodes.Gp_Burnham].Count > 0);
        }

        [TestMethod]
        public void TestGetPracticeCodeToBaseDataMapNullCIs()
        {
            var grouping = new Grouping
            {
                IndicatorId = IndicatorIds.IDACI,
                AgeId = AgeIds.Under16,
                SexId = SexIds.Persons
            };

            Dictionary<string, CoreDataSet> map = new PracticeDataAccess()
                .GetPracticeCodeToBaseDataMap(grouping,
                new TimePeriod { Year = 2011, YearRange = 1 });

            Assert.AreEqual(map[AreaCodes.Gp_Burnham].LowerCI95, ValueData.NullValue);
        }
    }
}