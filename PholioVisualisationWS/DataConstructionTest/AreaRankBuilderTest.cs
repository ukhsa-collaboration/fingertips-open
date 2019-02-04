using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataSorting;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class AreaRankBuilderTest
    {
        public const string ValueNoteText = "note";

        private IndicatorMetadata _indicatorMetadata = new IndicatorMetadata
        {
            YearTypeId = YearTypeIds.Calendar
        };

        private Mock<IGroupDataReader> _groupDataReader;
        private Mock<IAreasReader> _areasReader;
        private Mock<IPholioLabelReader> _pholioLabelReader;
        private Mock<INumericFormatterFactory> _numericFormatterFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            _groupDataReader = new Mock<IGroupDataReader>(MockBehavior.Strict);
            _areasReader = new Mock<IAreasReader>(MockBehavior.Strict);

            // Value note
            _pholioLabelReader = new Mock<IPholioLabelReader>(MockBehavior.Loose);
            _pholioLabelReader.Setup(x => x.LookUpValueNoteLabel(It.IsAny<int>())).Returns(ValueNoteText);

            // Not interested in formatting so data will not be changed
            _numericFormatterFactory = new Mock<INumericFormatterFactory>(MockBehavior.Strict);
            var formatter = new Mock<NumericFormatter>(MockBehavior.Loose);
            _numericFormatterFactory.Setup(x => x.New(_indicatorMetadata)).Returns(formatter.Object);
        }

        [TestMethod]
        public void Test_When_Invalid_Area_Data_Then_Area_Rank_Is_Still_Defined()
        {
            _areasReader.Setup(x => x.GetAreasFromCodes(It.IsAny<IList<string>>()))
                .Returns(new List<IArea> {new Area {Code = "a"}, new Area {Code = "b"} });

            var builder = GetAreaRankBuilder();

            var areaCode = AreaCodes.CountyUa_Bexley;
            var area = new Area
            {
                Code = areaCode,
                AreaTypeId = AreaTypeIds.UnitaryAuthority
            };

            // Data list with min, max and invalid data included
            var dataList = new List<CoreDataSet>
            {
                new CoreDataSet { AreaCode = "a", Value = 1, Count = 4},
                new CoreDataSet { AreaCode = "b", Value = 2, Count = 5},
                CoreDataSet.GetNullObject(areaCode)
            };

            var grouping = TestGrouping();
            TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);

            // Act
            var areaRankGrouping = builder.BuildRank(area, TestGrouping(),
                _indicatorMetadata, timePeriod, dataList);

            // Assert
            AssertAreaRankWhenNoData(areaRankGrouping, areaCode);
            VerifyAll();
        }

        [TestMethod]
        public void Test_When_No_Area_Data_Then_Area_Rank_Is_Still_Defined()
        {
            _areasReader.Setup(x => x.GetAreasFromCodes(It.IsAny<IList<string>>()))
                .Returns(new List<IArea> { new Area { Code = "a" }, new Area { Code = "b" } });

            var builder = GetAreaRankBuilder();

            var areaCode = AreaCodes.CountyUa_Bexley;
            var area = new Area
            {
                Code = areaCode,
                AreaTypeId = AreaTypeIds.UnitaryAuthority
            };

            // Data list with only min, max
            var dataList = new List<CoreDataSet>
            {
                new CoreDataSet { AreaCode = "a", Value = 1, Count = 4},
                new CoreDataSet { AreaCode = "b", Value = 2, Count = 5}
            };

            var grouping = TestGrouping();
            TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);

            // Act
            var areaRankGrouping = builder.BuildRank(area, TestGrouping(),
                _indicatorMetadata, timePeriod, dataList);

            // Assert
            AssertAreaRankWhenNoData(areaRankGrouping, areaCode);
            VerifyAll();
        }

        [TestMethod]
        public void TestSpecificAreas()
        {
            TestArea(AreaCodes.CountyUa_Cumbria); // Cumbria - a County
            TestArea(AreaCodes.CountyUa_NorthTyneside); // North Tyneside  - a UA
            TestArea(AreaCodes.CountyUa_Manchester); // Manchester - the worst
            TestArea(AreaCodes.CountyUa_Buckinghamshire); // Buckinghamshire - the best
        }

        [TestMethod]
        public void TestRanksAreaSameForAreasWithSameValue()
        {
            var grouping = TestGrouping();
            TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);

            var dataList = new List<CoreDataSet>
            {
                new CoreDataSet { AreaCode = AreaCodes.Ccg_Barnet, Value = 1, Count = 4},
                new CoreDataSet { AreaCode = "b", Value = 2, Count = 4},
                new CoreDataSet { AreaCode = "c", Value = 2, Count = 4},
                new CoreDataSet { AreaCode = "d", Value = 2, Count = 4},
                new CoreDataSet { AreaCode = AreaCodes.Ccg_Chiltern, Value = 3, Count = 4}
            };

            var areaCodes = new List<string> { "b", "c", "d" };
            foreach (var areaCode in areaCodes)
            {
                AreaRankBuilder rankBuilder = AreaRankBuilder();
                AreaRankGrouping rank = rankBuilder.BuildRank(new Area { Code = areaCode },
                    grouping, _indicatorMetadata, timePeriod, dataList);

                // Assert: rank is same for areas with same value
                Assert.AreEqual(2, rank.AreaRank.Rank);
            }
        }

        [TestMethod]
        public void TestEnglandReturnsValuesButNotRanks()
        {
            AreaRankBuilder rankBuilder = AreaRankBuilder();

            var grouping = TestGrouping();
            TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);

            var dataList = GetDataList(grouping, timePeriod);

            AreaRankGrouping rank = rankBuilder.BuildRank(England(), grouping,
                _indicatorMetadata, timePeriod, dataList);

            // Assert
            Assert.IsNull(rank.AreaRank.Rank, "England rank not null");
            Assert.IsNotNull(rank.AreaRank.Value, "England value null");
            Assert.IsNotNull(rank.AreaRank.Count, "England count null");
        }

        /// <summary>
        /// NOTE: check year in TestGrouping() if this test fails
        /// </summary>
        [TestMethod]
        public void TestDeprivationDecileReturnsValuesButNotRanks()
        {
            var categoryArea = CategoryArea.New(CategoryTypeIds.DeprivationDecileCountyAndUA2015, 7);
            AreaRankBuilder rankBuilder = AreaRankBuilder();

            var grouping = TestGrouping();
            TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);

            var dataList = GetDataList(grouping, timePeriod);

            AreaRankGrouping rank = rankBuilder.BuildRank(categoryArea,
                grouping, _indicatorMetadata, timePeriod, dataList);

            // Assert
            Assert.IsNull(rank.AreaRank.Rank, "decile rank not null");
            Assert.IsNotNull(rank.AreaRank.Value, "decile value null");
            Assert.IsNotNull(rank.AreaRank.Count, "decile count null");
        }

        /// <summary>
        /// NOTE: check year in TestGrouping() if this test fails
        /// </summary>
        [TestMethod]
        public void TestNearestNeighbourReturnsValuesButNotRanks()
        {
            var nearestNeighbourArea = NearestNeighbourArea.New(NearestNeighbourTypeIds.Cipfa,
                AreaCodes.CountyUa_Buckinghamshire);
            AreaRankBuilder rankBuilder = AreaRankBuilder();

            var grouping = TestGrouping();
            TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);

            var dataList = GetDataList(grouping, timePeriod);

            AreaRankGrouping rank = rankBuilder.BuildRank(nearestNeighbourArea,
                grouping, _indicatorMetadata, timePeriod, dataList);

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
            var area = ReaderFactory.GetAreasReader().GetAreaFromCode(areaCode);
            AreaRankBuilder rankBuilder = AreaRankBuilder();

            var grouping = TestGrouping();
            TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);

            var dataList = GetDataList(grouping, timePeriod);

            AreaRankGrouping rank = rankBuilder.BuildRank(area,
                grouping, _indicatorMetadata, timePeriod, dataList);

            // Check ranks defined
            Assert.IsTrue(rank.AreaRank.Rank > 0, "No rank for " + areaCode);
            Assert.IsTrue(rank.Max.Rank > 140 && rank.Max.Rank < 160, "Max rank outside range expected for " + areaCode);
            Assert.AreEqual(1, rank.Min.Rank);

            return rank;
        }

        [TestMethod]
        public void TestAreaPolaritySwitch()
        {
            var highIsGoodPolarityRanking = TestAreaWithHighIsGoodPolarity(AreaCodes.CountyUa_Cambridgeshire.ToLower());
            var lowIsGoodPolarityRanking = TestAreaWithLowIsGoodPolarity(AreaCodes.CountyUa_Cambridgeshire.ToLower());

            Assert.IsTrue(highIsGoodPolarityRanking.AreaRank.Rank > lowIsGoodPolarityRanking.AreaRank.Rank,
                "Invalid Polarity Comparison Value");
        }

        private AreaRankGrouping TestAreaWithHighIsGoodPolarity(string areaCode)
        {
            var area = ReaderFactory.GetAreasReader().GetAreaFromCode(areaCode);
            AreaRankBuilder rankBuilder = AreaRankBuilder();

            var grouping = TestDiabetesGrouping();
            grouping.PolarityId = PolarityIds.RagHighIsGood;

            TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);

            var dataList = GetDataList(grouping, timePeriod);

            AreaRankGrouping rank = rankBuilder.BuildRank(area,
                grouping, _indicatorMetadata, timePeriod, dataList);

            // Check ranks defined
            Assert.IsTrue(rank.AreaRank.Rank > 0, "No rank for " + areaCode);
            Assert.AreEqual(1, rank.Min.Rank);

            return rank;
        }

        private AreaRankGrouping TestAreaWithLowIsGoodPolarity(string areaCode)
        {
            var area = ReaderFactory.GetAreasReader().GetAreaFromCode(areaCode);
            var rankBuilder = AreaRankBuilder();

            var grouping = TestDiabetesGrouping();
            grouping.PolarityId = PolarityIds.RagLowIsGood;

            TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);

            var dataList = GetDataList(grouping, timePeriod);

            AreaRankGrouping rank = rankBuilder.BuildRank(area,
                grouping, _indicatorMetadata, timePeriod, dataList);

            // Check ranks defined
            Assert.IsTrue(rank.AreaRank.Rank > 0, "No rank for " + areaCode);
            Assert.AreEqual(1, rank.Min.Rank);

            return rank;
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

        private static AreaRankBuilder AreaRankBuilder()
        {
            var groupDataReader = ReaderFactory.GetGroupDataReader();
            AreaRankBuilder rankBuilder = new AreaRankBuilder(groupDataReader,
                ReaderFactory.GetAreasReader(), new PholioLabelReader(),
                new NumericFormatterFactory(groupDataReader));
            return rankBuilder;
        }

        private Grouping TestGrouping()
        {
            return new Grouping
            {
                GroupId = GroupIds.PublicHealthDashboardLongerLives_SummaryRank,
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
                GroupId = GroupIds.PublicHealthDashboardLongerLives_ChildObesity,
                IndicatorId = IndicatorIds.ObesityYear6,
                AgeId = AgeIds.From10To11,
                YearRange = 1,
                SexId = SexIds.Persons,
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                DataPointYear = 2015,
                DataPointMonth = -1,
                DataPointQuarter = -1
            };
        }

        private static IList<CoreDataSet> GetDataList(Grouping grouping, TimePeriod timePeriod)
        {
            var groupDataReader = ReaderFactory.GetGroupDataReader();
            var data = new CoreDataSetListProvider(groupDataReader).GetChildAreaData(grouping, England(), timePeriod);
            return new CoreDataSetFilter(data).RemoveWithAreaCode(IgnoredAreaCodes()).ToList();
        }

        private static IEnumerable<string> IgnoredAreaCodes()
        {
            return ReaderFactory.GetProfileReader().GetAreaCodesToIgnore(ProfileIds.LongerLives).AreaCodesIgnoredEverywhere;
        }

        private AreaRankBuilder GetAreaRankBuilder()
        {
            var builder = new AreaRankBuilder(_groupDataReader.Object, _areasReader.Object,
                _pholioLabelReader.Object, _numericFormatterFactory.Object);
            return builder;
        }

        private void VerifyAll()
        {
            _areasReader.VerifyAll();
            _groupDataReader.VerifyAll();
            _numericFormatterFactory.VerifyAll();
            _pholioLabelReader.VerifyAll();
        }

        private static void AssertAreaRankWhenNoData(AreaRankGrouping areaRankGrouping, string areaCode)
        {
            var areaRank = areaRankGrouping.AreaRank;
            Assert.AreEqual(ValueData.NullValue, areaRank.Value);
            Assert.AreEqual(ValueData.NullValue, areaRank.Count);
            Assert.AreEqual(ValueData.NullValue, areaRank.Denom);
            Assert.AreEqual(ValueNoteText, areaRank.ValueNote.Text);
            Assert.AreEqual(areaCode, areaRank.Area.Code);
            Assert.IsNull(areaRank.Rank);
        }
    }
}
