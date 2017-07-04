using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class AreaRankBuilderTest
    {
        private IndicatorMetadata indicatorMetadata = new IndicatorMetadata
        {
            YearTypeId = YearTypeIds.Calendar
        };

        [TestMethod]
        public void TestSpecificAreas()
        {
            TestArea(AreaCodes.CountyUa_Cumbria); // Cumbria - a County
            TestArea(AreaCodes.CountyUa_NorthTyneside); // North Tyneside  - a UA
            TestArea(AreaCodes.CountyUa_Manchester); // Manchester - the worst
            TestArea(AreaCodes.CountyUa_Buckinghamshire); // Buckinghamshire - the best
        }

        [TestMethod]
        public void TestEnglandReturnsValuesButNotRanks()
        {
            var areasReader = ReaderFactory.GetAreasReader();
            AreaRankBuilder rankBuilder = new AreaRankBuilder
            {
                AreasReader = areasReader,
                GroupDataReader = ReaderFactory.GetGroupDataReader(),
                Area = England()
            };

            var grouping = TestGrouping();
            TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);

            var dataList = GetDataList(grouping, timePeriod);

            AreaRankGrouping rank = rankBuilder.BuildRank(grouping, indicatorMetadata, timePeriod, dataList);

            // Assert
            Assert.IsNull(rank.AreaRank.Rank, "England rank not null");
            Assert.IsNotNull(rank.AreaRank.Value, "England value null");
            Assert.IsNotNull(rank.AreaRank.Count, "England count null");
        }

        private static Area England()
        {
            return new Area
            {
                Code = AreaCodes.England,
                Name = "England",
                AreaTypeId = AreaTypeIds.Country
            };
        }

        private static IEnumerable<CoreDataSet> GetDataList(Grouping grouping, TimePeriod timePeriod)
        {
            var groupDataReader = ReaderFactory.GetGroupDataReader();
            var data = new CoreDataSetListProvider(groupDataReader).GetChildAreaData(grouping, England(), timePeriod);
            return new CoreDataSetFilter(data).RemoveWithAreaCode(IgnoredAreaCodes());
        }

        private static IEnumerable<string> IgnoredAreaCodes()
        {
            return ReaderFactory.GetProfileReader().GetAreaCodesToIgnore(ProfileIds.LongerLives).AreaCodesIgnoredEverywhere;
        }

        /// <summary>
        /// NOTE: check year in TestGrouping() if this test fails
        /// </summary>
        [TestMethod]
        public void TestDeprivationDecileReturnsValuesButNotRanks()
        {
            var areasReader = ReaderFactory.GetAreasReader();
            var categoryArea = CategoryArea.New(CategoryTypeIds.DeprivationDecileCountyAndUA2010, 7);

            AreaRankBuilder rankBuilder = new AreaRankBuilder
            {
                AreasReader = areasReader,
                GroupDataReader = ReaderFactory.GetGroupDataReader(),
                Area = categoryArea
            };

            var grouping = TestGrouping();
            TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);

            var dataList = GetDataList(grouping, timePeriod);

            AreaRankGrouping rank = rankBuilder.BuildRank(grouping, indicatorMetadata, timePeriod, dataList);

            // Assert
            Assert.IsNull(rank.AreaRank.Rank, "decile rank not null");
            Assert.IsNotNull(rank.AreaRank.Value, "decile value null");
            Assert.IsNotNull(rank.AreaRank.Count, "decile count null");
        }

        [TestMethod]
        public void TestCaseInvariant()
        {
            TestArea(AreaCodes.CountyUa_Manchester.ToUpper());
            TestArea(AreaCodes.CountyUa_Manchester.ToLower());
        }

        [TestMethod]
        public void TestNumericValuesAreDefined()
        {
            var rank = TestArea(AreaCodes.CountyUa_Manchester);
            Assert.IsTrue(rank.AreaRank.Count > 0, "Count unexpected zero");
            Assert.IsTrue(rank.AreaRank.Value > 0, "Value unexpected zero");
            Assert.IsTrue(rank.Max.Value > 0, "Value unexpected zero");
            Assert.IsTrue(rank.Min.Value > 0, "Value unexpected zero");
        }

        [TestMethod]
        public void TestTimePeriodIsSet()
        {
            var rank = TestArea(AreaCodes.CountyUa_Manchester);
            Assert.AreEqual("2010 - 12", rank.TimePeriodText);
        }

        private AreaRankGrouping TestArea(string areaCode)
        {
            var areasReader = ReaderFactory.GetAreasReader();

            AreaRankBuilder rankBuilder = new AreaRankBuilder
                {
                    AreasReader = areasReader,
                    GroupDataReader = ReaderFactory.GetGroupDataReader(),
                    Area = areasReader.GetAreaFromCode(areaCode)
                };

            var grouping = TestGrouping();
            TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);

            var dataList = GetDataList(grouping, timePeriod);

            AreaRankGrouping rank = rankBuilder.BuildRank(grouping, indicatorMetadata, timePeriod, dataList);

            // Check ranks defined
            Assert.IsTrue(rank.AreaRank.Rank > 0, "No rank for " + areaCode);
            Assert.IsTrue(rank.Max.Rank > 140 && rank.Max.Rank < 160, "Max rank outside range expected for " + areaCode);
            Assert.AreEqual(1, rank.Min.Rank);

            return rank;
        }

        [TestMethod]
        public void TestAreaPolaritySwitch()
        {
            var highIsGoodPolarityRanking = TestAreaWithHighIsGoodPolarity(AreaCodes.Ccg_CambridgeshirePeterborough.ToLower());
            var lowIsGoodPolarityRanking = TestAreaWithLowIsGoodPolarity(AreaCodes.Ccg_CambridgeshirePeterborough.ToLower());

            Assert.IsTrue(highIsGoodPolarityRanking.AreaRank.Rank < lowIsGoodPolarityRanking.AreaRank.Rank, 
                "Invalid Polarity Comparison Value");
        }

        private AreaRankGrouping TestAreaWithHighIsGoodPolarity(string areaCode)
        {
            var areasReader = ReaderFactory.GetAreasReader();

            AreaRankBuilder rankBuilder = new AreaRankBuilder
                {
                    AreasReader = areasReader,
                    GroupDataReader = ReaderFactory.GetGroupDataReader(),
                    Area = areasReader.GetAreaFromCode(areaCode)
                };

            var grouping = TestDiabetesGrouping();
            grouping.PolarityId = PolarityIds.RagHighIsGood;

            TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);

            var dataList = GetDataList(grouping, timePeriod);

            AreaRankGrouping rank = rankBuilder.BuildRank(grouping, indicatorMetadata, timePeriod, dataList);

            // Check ranks defined
            Assert.IsTrue(rank.AreaRank.Rank > 0, "No rank for " + areaCode);
            Assert.AreEqual(1, rank.Min.Rank);

            return rank;
        }

        private AreaRankGrouping TestAreaWithLowIsGoodPolarity(string areaCode)
        {
            var areasReader = ReaderFactory.GetAreasReader();

            AreaRankBuilder rankBuilder = new AreaRankBuilder
                {
                    AreasReader = areasReader,
                    GroupDataReader = ReaderFactory.GetGroupDataReader(),
                    Area = areasReader.GetAreaFromCode(areaCode)
                };

            var grouping = TestDiabetesGrouping();
            grouping.PolarityId = PolarityIds.RagLowIsGood;

            TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);

            var dataList = GetDataList(grouping, timePeriod);

            AreaRankGrouping rank = rankBuilder.BuildRank(grouping, indicatorMetadata, timePeriod, dataList);

            // Check ranks defined
            Assert.IsTrue(rank.AreaRank.Rank > 0, "No rank for " + areaCode);
            Assert.AreEqual(1, rank.Min.Rank);

            return rank;
        }

        private Grouping TestGrouping()
        {
            return new Grouping
                {
                    GroupId = GroupIds.LongerLives,
                    IndicatorId = IndicatorIds.OverallPrematureDeaths,
                    AgeId = AgeIds.Under75,
                    YearRange = 3,
                    SexId = SexIds.Persons,
                    AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                    DataPointYear = 2010,
                    DataPointMonth = -1,
                    DataPointQuarter = -1
                };
        }

        private Grouping TestDiabetesGrouping()
        {
            return new Grouping
                {
                    GroupId = GroupIds.Diabetes_TreatmentTargets,
                    IndicatorId = IndicatorIds.BMIRecorded,
                    AgeId = AgeIds.Plus17,
                    YearRange = 1,
                    SexId = SexIds.Persons,
                    AreaTypeId = AreaTypeIds.Ccg,
                    DataPointYear = 2012,
                    DataPointMonth = -1,
                    DataPointQuarter = -1
                };
        }
    }
}
