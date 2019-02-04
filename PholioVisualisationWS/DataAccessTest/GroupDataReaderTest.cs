using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class GroupDataReaderTest
    {
        private IGroupDataReader _groupDataReader;

        [TestInitialize]
        public void TestInitialize()
        {
            _groupDataReader = ReaderFactory.GetGroupDataReader();
        }

        [TestMethod]
        public void TestGetDataIncludingInequalities()
        {
            var timePeriod = new TimePeriod()
            {
                Year = 2015,
                YearRange = 1
            };

            // Act: get all the data
            var dataList = _groupDataReader.GetDataIncludingInequalities(
                IndicatorIds.Under16Conceptions, timePeriod, new List<int>(), AreaCodes.England);

            // Assert: category data found
            Assert.IsTrue(dataList.Any(x => x.CategoryTypeId == CategoryTypeIds.DeprivationDecileCountyAndUA2015));

            // Assert: age data found
            Assert.IsTrue(dataList.Any(x => x.AgeId == AgeIds.Under16));

            // Assert: sex data found
            Assert.IsTrue(dataList.Any(x => x.SexId == SexIds.Female));
        }

        [TestMethod]
        public void TestGetLsoaQuintilesInEngland()
        {
            var quintiles = _groupDataReader.GetCategoriesWithinParentArea(
                CategoryTypeIds.LsoaDeprivationQuintilesInEngland2015, AreaCodes.CountyUa_Cambridgeshire,
                AreaTypeIds.Lsoa, AreaTypeIds.Country);
            var count = quintiles.Count;
            Assert.IsTrue(count > 300 && count < 400, count + " quintiles");
        }

        [TestMethod]
        public void TestGetLsoaQuintilesInLocalArea()
        {
            var quintiles = _groupDataReader.GetCategoriesWithinParentArea(
                CategoryTypeIds.LsoaDeprivationQuintilesWithinArea2015, AreaCodes.DistrictUa_Mansfield,
                AreaTypeIds.Lsoa, AreaTypeIds.District);
            var count = quintiles.Count;
            Assert.IsTrue(count > 50 && count < 100, count + " quintiles");
        }

        [TestMethod]
        public void GetAvailableDataByIndicatorIdAndAreaTypeId_For_One_Indicator()
        {
            var indicatorId = IndicatorIds.Aged0To4Years;
            var data = _groupDataReader.GetAvailableDataByIndicatorIdAndAreaTypeId(indicatorId, null);

            // Assert: only data for one indicator is found
            Assert.AreEqual(data.Count, data.Select(x => x.IndicatorId == indicatorId).Count());
        }

        [TestMethod]
        public void GetAvailableDataByIndicatorIdAndAreaTypeId_For_One_Area_Type()
        {
            var areaTypeId = AreaTypeIds.GoRegion;
            var data = _groupDataReader.GetAvailableDataByIndicatorIdAndAreaTypeId(null, areaTypeId);

            // Assert: only data for one area type is found
            Assert.AreEqual(data.Count, data.Select(x => x.AreaTypeId == areaTypeId).Count());
        }

        [TestMethod]
        public void TestGetCoreDataCountAtDataPoint()
        {
            var grouping = new Grouping
            {
                DataPointYear = 2012,
                YearRange = 1,
                IndicatorId = IndicatorIds.PupilAbsence,
                AgeId = AgeIds.From5To15,
                SexId = SexIds.Persons,
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority
            };

            int count = _groupDataReader.GetCoreDataCountAtDataPoint(grouping);
            Assert.IsTrue(count > 100);
        }

        [TestMethod]
        public void TestGetGroupingsByIndicatorIdsAndAreaType()
        {
            IList<Grouping> groupings = _groupDataReader.GetGroupingsByGroupIdsAndIndicatorIdsAndAreaType(
                new List<int> { GroupIds.Phof_WiderDeterminantsOfHealth },
                new List<int> { IndicatorIds.IDACI, IndicatorIds.ChildrenInLowIncomeFamilies },
                AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.IsTrue(groupings.Any());
        }

        [TestMethod]
        public void TestGroupingComparatorTargetIdIsReadFromDatabase()
        {
            IList<Grouping> groupings = _groupDataReader.GetGroupingsByGroupId(
                GroupIds.HealthProfiles_OurCommunities);
            Assert.AreEqual(TargetIds.Undefined, groupings.First().ComparatorTargetId);
        }

        [TestMethod]
        public void TestGetCoreDataLimits()
        {
            Limits limits =
                _groupDataReader.GetCoreDataLimitsByIndicatorId(IndicatorIds.LifeExpectancyMsoaBasedEstimate);
            Assert.IsTrue(limits.Max > limits.Min);
        }

        [TestMethod]
        public void TestGetCoreDataLimits_WhereNoData()
        {
            Limits limits = _groupDataReader.GetCoreDataLimitsByIndicatorId(IndicatorIds.IndicatorHasNoData);
            Assert.IsNull(limits);
        }

        [TestMethod]
        public void TestGetCoreDataLimitsByIndicatorIdAndAreaTypeId()
        {
            Limits limits = _groupDataReader.GetCoreDataLimitsByIndicatorIdAndAreaTypeId(
                IndicatorIds.LifeExpectancyMsoaBasedEstimate, AreaTypeIds.GpPractice);
            Assert.IsTrue(limits.Max > limits.Min);
        }

        [TestMethod]
        public void TestGetCoreDataLimitsByIndicatorIdAndAreaTypeId_WhereNoData()
        {
            Limits limits = _groupDataReader.GetCoreDataLimitsByIndicatorIdAndAreaTypeId(
                IndicatorIds.IndicatorHasNoData, AreaTypeIds.GpPractice);
            Assert.IsNull(limits);
        }

        [TestMethod]
        public void TestGetCoreDataLimitsByIndicatorIdAndAreaTypeIdAndParentAreaCode()
        {
            Limits limits = _groupDataReader.GetCoreDataLimitsByIndicatorIdAndAreaTypeIdAndParentAreaCode(
                IndicatorIds.LifeExpectancyMsoaBasedEstimate, AreaTypeIds.GpPractice, AreaCodes.Ccg_Barnet);
            Assert.IsTrue(limits.Max > limits.Min);
        }

        [TestMethod]
        public void TestGetIndicatorMetadataByGroupingList()
        {
            var groupings = new List<Grouping>
            {
                new Grouping {IndicatorId = IndicatorIds.StatutoryHomelessness},
                new Grouping {IndicatorId = IndicatorIds.SyphilisDiagnosis}
            };
            IList<IndicatorMetadata> metadataList = _groupDataReader.GetIndicatorMetadata(groupings,
                _groupDataReader.GetIndicatorMetadataTextProperties());
            Assert.AreEqual(2, metadataList.Count);
            AssertIndicatorMetadataIsOk(metadataList[0]);
        }

        [TestMethod]
        public void TestGetIndicatorMetadataByGroupingTextPropertiesAssignedToCorrectIndicatorMetadataObject()
        {
            var groupings = new List<Grouping>
            {
                new Grouping {IndicatorId = IndicatorIds.ExcessWinterDeaths},
                new Grouping {IndicatorId = IndicatorIds.DeathsFromLungCancer},
                new Grouping {IndicatorId = IndicatorIds.OverallPrematureDeaths}
            };
            IList<IndicatorMetadata> metadataList = _groupDataReader.GetIndicatorMetadata(groupings,
                _groupDataReader.GetIndicatorMetadataTextProperties());

            Assert.AreEqual(IndicatorIds.ExcessWinterDeaths,
                metadataList.FirstOrDefault(x => x.Name.ToLower()
                    .Contains("winter deaths")).IndicatorId
                );

            Assert.AreEqual(IndicatorIds.OverallPrematureDeaths,
                metadataList.FirstOrDefault(x => x.Name.ToLower()
                    .Contains("mortality rate")).IndicatorId
                );

            Assert.AreEqual(IndicatorIds.DeathsFromLungCancer,
                metadataList.FirstOrDefault(x => x.Name.ToLower()
                    .Contains("lung cancer")).IndicatorId
                );
        }

        [TestMethod]
        public void TestGetIndicatorMetadataByGrouping()
        {
            IndicatorMetadata metadata = _groupDataReader.GetIndicatorMetadata(
                new Grouping { IndicatorId = IndicatorIds.SmokingAtTimeOfDelivery },
                _groupDataReader.GetIndicatorMetadataTextProperties());
            AssertIndicatorMetadataIsOk(metadata);
        }

        [TestMethod]
        public void TestGetIndicatorMetadataByIndicatorId()
        {
            var indicatorId = IndicatorIds.ObesityYear6;
            IndicatorMetadata metadata = _groupDataReader.GetIndicatorMetadata(indicatorId,
                _groupDataReader.GetIndicatorMetadataTextProperties());
            Assert.AreEqual(indicatorId, metadata.IndicatorId);
            AssertIndicatorMetadataIsOk(metadata);
        }

        [TestMethod]
        public void TestGetIndicatorMetadataTextPropertiesContainsSystemContentField()
        {
            // Assert Indicator Metadata contains a field that is defined as SystemContent.
            IList<IndicatorMetadataTextProperty> properties =
                _groupDataReader.GetIndicatorMetadataTextProperties();

            var systemContentProperties = properties.Where(x => x.IsSystemContent).ToList();

            Assert.IsTrue(systemContentProperties.Count > 0);
        }

        [TestMethod]
        public void TestGetIndicatorMetadataByIndicatorIdList()
        {
            IList<IndicatorMetadata> metadata = _groupDataReader.GetIndicatorMetadata(
                new List<int> {
                    IndicatorIds.KilledAndSeriouslyInjuredOnRoads,
                    IndicatorIds.ObesityYear6 },
                _groupDataReader.GetIndicatorMetadataTextProperties()
                );

            // Check metadata is as expected
            Assert.AreEqual(2, metadata.Count);
            AssertIndicatorMetadataIsOk(metadata[0]);
            AssertIndicatorMetadataIsOk(metadata[1]);
        }

        [TestMethod]
        public void TestIndicatorMetadataHasTargetDefined()
        {
            var groupings = new List<Grouping>
            {
                new Grouping {IndicatorId = IndicatorIds.HIVLateDiagnosis}
            };
            IList<IndicatorMetadata> metadataList = _groupDataReader.GetIndicatorMetadata(groupings,
                _groupDataReader.GetIndicatorMetadataTextProperties());
            Assert.IsNotNull(metadataList.First().TargetConfig);
        }

        [TestMethod]
        public void TestGetGroupSpecificIndicatorMetadataTextValues()
        {
            // Assert group specific metadata is found
            IList<IndicatorMetadata> metadataList = _groupDataReader.GetGroupSpecificIndicatorMetadataTextValues(
                new List<Grouping> { new Grouping { GroupId = GroupIds.HealthProfiles_OurCommunities } },
                _groupDataReader.GetIndicatorMetadataTextProperties());

            Assert.IsTrue(metadataList.Any());

            // Assert generic and specific metadata properties are different
            const int indicatorId = IndicatorIds.ViolentCrime;

            IndicatorMetadata specificMetadata =
                (from m in metadataList where m.IndicatorId == indicatorId select m).First();

            IndicatorMetadata genericMetadata = _groupDataReader.GetIndicatorMetadata(indicatorId,
                _groupDataReader.GetIndicatorMetadataTextProperties());

            Assert.AreNotEqual(genericMetadata.Descriptive[IndicatorMetadataTextColumnNames.Name],
                specificMetadata.Descriptive[IndicatorMetadataTextColumnNames.Name]);
        }

        [TestMethod]
        public void TestGetGroupSpecificIndicatorMetadataTextValuesWhereNoGroupings()
        {
            // Assert group specific metadata is found
            IList<IndicatorMetadata> metadataList = _groupDataReader.GetGroupSpecificIndicatorMetadataTextValues(
                new List<Grouping>(), _groupDataReader.GetIndicatorMetadataTextProperties());

            Assert.AreEqual(0, metadataList.Count);
        }

        [TestMethod]
        public void TestGetIndicatorMetadataTextProperties()
        {
            IList<IndicatorMetadataTextProperty> properties =
                _groupDataReader.GetIndicatorMetadataTextProperties();

            Assert.IsTrue(properties.Count > 20);
            foreach (IndicatorMetadataTextProperty indicatorMetadataTextProperty in properties)
            {
                // All essential names are defined
                Assert.IsNotNull(indicatorMetadataTextProperty.ColumnName);
                Assert.IsNotNull(indicatorMetadataTextProperty.DisplayName);

                // All DisplayOrders greater than zero
                Assert.IsTrue(indicatorMetadataTextProperty.DisplayOrder > 0);
            }
        }

        [TestMethod]
        public void TestGetCoreDataNoAreaCodeNoData()
        {
            Grouping grouping = GetTestGrouping();
            IList<CoreDataSet> data = _groupDataReader.GetCoreData(grouping, TimePeriod.GetDataPoint(grouping));
            Assert.AreEqual(0, data.Count);
        }

        [TestMethod]
        public void TestGetCoreDataOneDataRecord()
        {
            Grouping grouping = GetTestGrouping();
            IList<CoreDataSet> data = _groupDataReader.GetCoreData(grouping, TimePeriod.GetDataPoint(grouping),
                AreaCodes.Gor_EastOfEngland);
            Assert.AreEqual(1, data.Count);
        }

        [TestMethod]
        public void TestGetCoreDataForCategoryAreaNoData()
        {
            CategoryArea categoryArea = CategoryArea.New(CategoryTypeIds.DeprivationDecileGp2015, 1);

            CoreDataSet data = _groupDataReader
                .GetCoreDataForCategoryArea(SmokingPrevalenceGrouping(),
                    new TimePeriod { Month = -1, Quarter = -1, Year = 2000, YearRange = 1 },
                    categoryArea);
            Assert.IsNull(data);
        }

        [TestMethod]
        public void TestGetCoreDataForCategoryAreaOneRecord()
        {
            CategoryArea categoryArea = CategoryArea.New(CategoryTypeIds.DeprivationDecileGp2010, 1);

            CoreDataSet data = _groupDataReader
                .GetCoreDataForCategoryArea(SmokingPrevalenceGrouping(),
                    new TimePeriod { Month = -1, Quarter = -1, Year = 2010, YearRange = 1 }, categoryArea);

            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void TestGetAllCategoryDataWithinParentArea()
        {
            var sexId = SexIds.Male;
            var ageId = AgeIds.AllAges;
            var indicatorId = IndicatorIds.LifeExpectancyAtBirth;

            var dataList = _groupDataReader
                .GetAllCategoryDataWithinParentArea(AreaCodes.England,
                    indicatorId,
                    sexId,
                    ageId,
                    new TimePeriod { Month = -1, Quarter = -1, Year = 2010, YearRange = 3 });

            Assert.IsNotNull(dataList);
            Assert.IsTrue(dataList.Any(), "No category data found");
            foreach (var coreDataSet in dataList)
            {
                Assert.IsNotNull(coreDataSet);
                Assert.AreEqual(sexId, coreDataSet.SexId);
                Assert.AreEqual(ageId, coreDataSet.AgeId);
                Assert.AreEqual(indicatorId, coreDataSet.IndicatorId);
                Assert.AreNotEqual(-1, coreDataSet.CategoryTypeId);
                Assert.AreNotEqual(-1, coreDataSet.CategoryId);
            }
        }


        [TestMethod]
        public void TestGetCoreDataTwoAreas()
        {
            Grouping grouping = GetTestGrouping();
            IList<CoreDataSet> data = _groupDataReader.GetCoreData(grouping, TimePeriod.GetDataPoint(grouping),
                AreaCodes.CountyUa_Bedfordshire,
                AreaCodes.CountyUa_Cambridgeshire);
            Assert.AreEqual(2, data.Count);

            AssertDataAndGroupingMatch(data[0], grouping);
            AssertDataAndGroupingMatch(data[1], grouping);
        }

        [TestMethod]
        public void TestGetCoreDataInvalidYear()
        {
            Grouping grouping = GetTestGrouping();
            grouping.DataPointYear = -1;
            IList<CoreDataSet> data = _groupDataReader.GetCoreData(grouping, TimePeriod.GetDataPoint(grouping),
                AreaCodes.Gor_EastOfEngland);
            Assert.AreEqual(0, data.Count);
        }

        [TestMethod]
        public void TestGetCoreDataInvalidIndicatorId()
        {
            Grouping grouping = GetTestGrouping();
            grouping.IndicatorId = -1;
            IList<CoreDataSet> data = _groupDataReader.GetCoreData(grouping, TimePeriod.GetDataPoint(grouping),
                AreaCodes.Gor_EastOfEngland);
            Assert.AreEqual(0, data.Count);
        }

        [TestMethod]
        public void TestGetCoreDataByIndicatorIdAndAreaCode()
        {
            IList<CoreDataSet> data = _groupDataReader.GetCoreData(IndicatorIds.QofPoints, "J83045");
            Assert.IsTrue(data.Count > 1);
        }

        [TestMethod]
        public void TestGetCoreDataForCategory()
        {
            var grouping = new Grouping
            {
                IndicatorId = IndicatorIds.PercentageOfPeoplePerDeprivationQuintile,
                DataPointYear = 2010,
                DataPointQuarter = -1,
                YearRange = 1,
                DataPointMonth = -1,
                SexId = SexIds.Persons,
                AgeId = AgeIds.AllAges
            };

            // Act: read core data
            CoreDataSet data = _groupDataReader.GetCoreDataForCategory(grouping, TimePeriod.GetDataPoint(grouping),
                AreaCodes.CountyUa_CentralBedfordshire, CategoryTypeIds.LsoaDeprivationQuintilesInEngland2010, CategoryIds.LeastDeprivedQuintile);

            // Assert
            AssertDataAndGroupingMatch(data, grouping);
        }

        [TestMethod]
        public void TestGetCoreDataMonthlyOneArea()
        {
            var grouping = new Grouping
            {
                IndicatorId = IndicatorIds.PercentageLivingInEachDeprivationQuintile,
                DataPointYear = 2009,
                DataPointQuarter = -1,
                YearRange = 1,
                DataPointMonth = 6,
                SexId = SexIds.Persons,
                AgeId = AgeIds.From16To64
            };
            IList<CoreDataSet> data = _groupDataReader.GetCoreData(grouping, TimePeriod.GetDataPoint(grouping),
                AreaCodes.CountyUa_CentralBedfordshire);
            Assert.AreEqual(1, data.Count);

            AssertDataAndGroupingMatch(data[0], grouping);
        }

        private static void AssertDataAndGroupingMatch(CoreDataSet data, Grouping grouping)
        {
            Assert.AreEqual(grouping.DataPointMonth, data.Month);
            Assert.AreEqual(grouping.DataPointYear, data.Year);
            Assert.AreEqual(grouping.YearRange, data.YearRange);
            Assert.AreEqual(grouping.DataPointQuarter, data.Quarter);
            Assert.AreEqual(grouping.SexId, data.SexId);
        }

        [TestMethod]
        public void TestGetGroupDataByGroupIdAndAreaTypeId()
        {
            IList<Grouping> groupings102 =
                _groupDataReader.GetGroupingsByGroupIdAndAreaTypeId(
                GroupIds.Phof_HealthProtection,
                AreaTypeIds.CountyAndUnitaryAuthority);

            IList<Grouping> groupings101 =
                _groupDataReader.GetGroupingsByGroupIdAndAreaTypeId(
                GroupIds.Phof_HealthProtection,
                AreaTypeIds.DistrictAndUnitaryAuthority);

            Assert.AreNotEqual(groupings102.Count, groupings101.Count());
            Assert.AreNotEqual(0, groupings102.Count);
            Assert.AreNotEqual(0, groupings101.Count);
        }

        [TestMethod]
        public void TestGetGroupingsByGroupIdIndicatorIdAgeId()
        {
            var ageId = AgeIds.Under16;
            var indicatorId = IndicatorIds.ChildrenInLowIncomeFamilies;

            IList<Grouping> groupings = _groupDataReader.GetGroupingsByGroupIdIndicatorIdAgeId(
                GroupIds.Phof_WiderDeterminantsOfHealth,
                AreaTypeIds.CountyAndUnitaryAuthority,
                indicatorId, ageId
                );

            // Assert groupings
            Assert.AreNotEqual(0, groupings.Count);
            foreach (var grouping in groupings)
            {
                Assert.AreEqual(indicatorId, grouping.IndicatorId);
                Assert.AreEqual(ageId, grouping.AgeId);
            }
        }

        [TestMethod]
        public void TestGetGroupingsByGroupIdIndicatorIdSexId()
        {
            var indicatorId = IndicatorIds.ChildrenInLowIncomeFamilies;
            var sexId = SexIds.Persons;

            IList<Grouping> groupings = _groupDataReader.GetGroupingsByGroupIdIndicatorIdSexId(
                GroupIds.Phof_WiderDeterminantsOfHealth,
                AreaTypeIds.CountyAndUnitaryAuthority,
                indicatorId, sexId);

            // Assert groupings
            Assert.AreNotEqual(0, groupings.Count);
            foreach (var grouping in groupings)
            {
                Assert.AreEqual(indicatorId, grouping.IndicatorId);
                Assert.AreEqual(sexId, grouping.SexId);
            }
        }

        [TestMethod]
        public void TestGetGroupingsByIndicatorIds()
        {
            IProfileReader reader = ReaderFactory.GetProfileReader();
            IList<int> groupIds = reader.GetGroupIdsFromAllProfiles();

            var indicatorIds = new List<int> {
                IndicatorIds.AdultDrugMisuse,
                IndicatorIds.AdultPhysicalActivity,
                IndicatorIds.DeprivationScoreIMD2010
            };
            IList<Grouping> groupingsFiltered = _groupDataReader.GetGroupingsByIndicatorIds(indicatorIds, groupIds);
            Assert.IsTrue(groupingsFiltered.Count() >= 3);
        }

        [TestMethod]
        public void TestGetGroupDataByIndicators_Most_Recent_First()
        {
            IProfileReader reader = ReaderFactory.GetProfileReader();
            IList<int> groupIds = reader.GetGroupIdsFromAllProfiles();

            var indicatorIds = new List<int> { IndicatorIds.AdultUnder75MortalityRateCancer };
            IList<Grouping> groupings = _groupDataReader.GetGroupingsByIndicatorIds(indicatorIds, groupIds);
            Assert.IsTrue(groupings.First().DataPointYear > groupings.Last().DataPointYear);
        }

        [TestMethod]
        public void TestGetIndicatorIdsByGroup()
        {
            IList<int> indicatorIds = _groupDataReader.GetIndicatorIdsByGroupIdAndAreaTypeId(
                GroupIds.Phof_HealthProtection,
                AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.IsTrue(indicatorIds.Count > 20 && indicatorIds.Count < 30,
                "Number of indicators should be around 25");
            Assert.IsTrue(indicatorIds.Contains(IndicatorIds.TreatmentCompletionForTB));
        }

        [TestMethod]
        public void TestGetIndicatorIdsByGroupOrderedBySequence()
        {
            IList<Grouping> groupings = _groupDataReader.GetGroupingsByGroupIdAndAreaTypeId(
                GroupIds.HealthProfiles_OurCommunities, AreaTypeIds.CountyAndUnitaryAuthority);

            IList<int> sequencedIds = (from g in groupings orderby g.Sequence select g.IndicatorId).Distinct().ToList();

            IList<int> indicatorIds = _groupDataReader.GetIndicatorIdsByGroupIdAndAreaTypeId(
                GroupIds.HealthProfiles_OurCommunities, AreaTypeIds.CountyAndUnitaryAuthority);

            for (int i = 0; i < indicatorIds.Count - 1; i++)
            {
                Assert.AreEqual(sequencedIds[i], indicatorIds[i]);
            }
        }

        [TestMethod]
        public void TestGetCoreDataForAllAges()
        {
            IList<CoreDataSet> data = _groupDataReader.GetCoreDataForAllAges(
                IndicatorIds.QuinaryPopulations,
                new TimePeriod { Year = 2014, YearRange = 1 },
                AreaCodes.Gp_Burnham,
                SexIds.Male);

            Assert.AreEqual(19, data.Count);
        }

        [TestMethod]
        public void TestGetCoreDataForAllSexes()
        {
            IList<CoreDataSet> data = _groupDataReader.GetCoreDataForAllSexes(
                IndicatorIds.LifeExpectancyAtBirth,
                new TimePeriod { Year = 2010, YearRange = 3 },
                AreaCodes.England,
                AgeIds.AllAges);

            Assert.AreEqual(2/*male & female*/, data.Count);
        }

        [TestMethod]
        public void TestGetGroupDataByIndicatorId()
        {
            Grouping grouping = _groupDataReader.GetGroupingsByGroupIdAndIndicatorId(
                GroupIds.Phof_HealthProtection,
                IndicatorIds.TreatmentCompletionForTB);
            Assert.AreEqual(IndicatorIds.TreatmentCompletionForTB, grouping.IndicatorId);
        }

        [TestMethod]
        public void TestGetGroupDataByAllDiscriminatorIds()
        {
            int areaTypeId = AreaTypeIds.GpPractice;
            int groupId = GroupIds.PracticeProfiles_PracticeSummary;
            int sexId = SexIds.Persons;
            int ageId = AgeIds.Over60;

            Grouping grouping = _groupDataReader.GetGroupings(groupId, IndicatorIds.IDAOPI2, areaTypeId,
                sexId, ageId).FirstOrDefault();

            Assert.AreEqual(IndicatorIds.IDAOPI2, grouping.IndicatorId);
            Assert.AreEqual(sexId, grouping.SexId);
            Assert.AreEqual(groupId, grouping.GroupId);
            Assert.AreEqual(areaTypeId, grouping.AreaTypeId);
            Assert.AreEqual(ageId, grouping.AgeId);
        }

        [TestMethod]
        public void TestGetGroupDataByAllDiscriminatorIdsApartFromAreaTypeId()
        {
            int groupId = GroupIds.PracticeProfiles_PracticeSummary;
            int sexId = SexIds.Persons;
            int ageId = AgeIds.Over60;

            Grouping grouping = _groupDataReader.GetGroupings(groupId, IndicatorIds.IDAOPI2,
                sexId, ageId).FirstOrDefault();

            Assert.AreEqual(IndicatorIds.IDAOPI2, grouping.IndicatorId);
            Assert.AreEqual(sexId, grouping.SexId);
            Assert.AreEqual(groupId, grouping.GroupId);
            Assert.AreEqual(ageId, grouping.AgeId);
        }

        [TestMethod]
        public void TestGetGroupDataByAllDiscriminatorIdsBarAgeId()
        {
            int areaTypeId = AreaTypeIds.GpPractice;
            int groupId = GroupIds.PracticeProfiles_PracticeSummary;
            int sexId = SexIds.Persons;

            Grouping grouping =
                _groupDataReader.GetGroupingsWithoutAgeId(groupId, IndicatorIds.IDAOPI2, areaTypeId, sexId).FirstOrDefault();

            Assert.AreEqual(IndicatorIds.IDAOPI2, grouping.IndicatorId);
            Assert.AreEqual(sexId, grouping.SexId);
            Assert.AreEqual(groupId, grouping.GroupId);
            Assert.AreEqual(areaTypeId, grouping.AreaTypeId);
        }

        [TestMethod]
        public void TestGetGroupMetadata()
        {
            IList<GroupingMetadata> list = _groupDataReader.GetGroupingMetadataList(
                new List<int>
                {
                    GroupIds.Phof_HealthProtection,
                    GroupIds.SexualAndReproductiveHealth
                });
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void TestGetGroupMetadataOrderedBySequence()
        {
            IList<GroupingMetadata> list = _groupDataReader.GetGroupingMetadataList(new List<int>
            {
                2000001,
                2000002,
                2000003,
                2000004,
                GroupIds.PracticeProfiles_PracticeSummary,
                2000006,
                2000007
            });

            int sequence = -1;
            foreach (GroupingMetadata groupingMetadata in list)
            {
                Assert.IsTrue(groupingMetadata.Sequence > sequence);
                sequence = groupingMetadata.Sequence;
            }
        }

        [TestMethod]
        public void TestGetDistinctGroupingAreaTypeIds()
        {
            IList<int> areaTypeIds = _groupDataReader.GetDistinctGroupingAreaTypeIds(
                new List<int> {
                    GroupIds.Phof_HealthProtection,
                    GroupIds.Phof_WiderDeterminantsOfHealth
                });
            Assert.IsTrue(areaTypeIds.Any());
            Assert.IsTrue(areaTypeIds.Contains(AreaTypeIds.CountyAndUnitaryAuthority));
        }

        [TestMethod]
        public void Test_GetCoreDataMaxYear()
        {
            var year = _groupDataReader.GetCoreDataMaxYear(IndicatorIds.Aged0To4Years);

            Assert.IsTrue(year > 2015);
        }
       
        [TestMethod]
        public void Test_GetCommonestPolarityForIndicator()
        {
            var polarityId = _groupDataReader.GetCommonestPolarityForIndicator(IndicatorIds.Aged0To4Years);

            Assert.AreEqual(PolarityIds.BlueOrangeBlue, polarityId);
        }

        [TestMethod]
        public void TestGetDistinctGroupingAreaTypeIds_WhereNoGroupIds()
        {
            IList<int> areaTypeIds = _groupDataReader.GetDistinctGroupingAreaTypeIds(new List<int>());

            Assert.IsFalse(areaTypeIds.Any());
        }

        [TestMethod]
        public void TestGetCoreDataListForChildrenOfArea_For_CCG()
        {
            IList<CoreDataSet> data = _groupDataReader.GetCoreDataListForChildrenOfArea(
                 PracticeGrouping(),
                new TimePeriod { Year = 2010, YearRange = 1 },
                AreaCodes.Ccg_Chiltern);

            Assert.IsTrue(data.Count > 20 && data.Count < 50);
        }

        [TestMethod]
        public void TestGetCoreDataListForChildrenOfArea_For_CountyUA_In_England()
        {
            IList<CoreDataSet> data = _groupDataReader.GetCoreDataListForChildrenOfArea(
                CountyUaGrouping(),
                new TimePeriod { Year = 2010, YearRange = 3 },
                AreaCodes.England);

            Assert.IsTrue(data.Count > 150 && data.Count < 155);
        }

        [TestMethod]
        public void TestGetCoreDataListForChildrenOfArea_For_DistrictUA_In_England()
        {
            var grouping = CountyUaGrouping();
            grouping.AreaTypeId = AreaTypeIds.DistrictAndUnitaryAuthority;

            IList<CoreDataSet> data = _groupDataReader.GetCoreDataListForChildrenOfArea(
                grouping,
                new TimePeriod { Year = 2010, YearRange = 3 },
                AreaCodes.England);

            Assert.IsTrue(data.Count > 320 && data.Count < 340,
                "Only " + data.Count + " were found");
        }

        [TestMethod]
        public void TestGetCoreDataListForChildrenOfCategoryArea()
        {
            CategoryArea categoryArea = CategoryArea.New(
                CategoryTypeIds.DeprivationDecileCountyAndUA2010, 1);

            IList<CoreDataSet> data = _groupDataReader.GetCoreDataListForChildrenOfCategoryArea(
                CountyUaGrouping(),
                new TimePeriod { Year = 2010, YearRange = 3 },
                categoryArea);

            // Decile contains 152/10 areas
            Assert.IsTrue(data.Count > 10 && data.Count < 20, "Unexpected number of data: " + data.Count);
        }

        [TestMethod]
        public void TestGetCoreDataListForChildrenOfNearestNeighbourArea()
        {
            NearestNeighbourArea neighbourArea = NearestNeighbourArea.New(NearestNeighbourTypeIds.Cipfa,
                AreaCodes.CountyUa_Cumbria);

            IList<CoreDataSet> data = _groupDataReader.GetCoreDataListForChildrenOfNearestNeighbourArea(
                CountyUaGrouping(),
                new TimePeriod { Year = 2010, YearRange = 3 },
                neighbourArea);

            // Assert: 15 neighbours
            Assert.AreEqual(15, data.Count);
        }

        [TestMethod]
        public void TestGetCoreDataListForAllCategoryAreasOfCategoryAreaType()
        {
            IList<CoreDataSet> data = _groupDataReader.GetCoreDataListForAllCategoryAreasOfCategoryAreaType(
                CountyUaGrouping(),
                new TimePeriod { Year = 2010, YearRange = 3 },
                CategoryTypeIds.DeprivationDecileCountyAndUA2015,
                AreaCodes.England);

            Assert.AreEqual(10, data.Count, "Expected 10 areas in decile");
        }

        [TestMethod]
        public void GetCoreDataForAllAreasOfType()
        {
            IList<CoreDataSet> data = _groupDataReader.GetCoreDataForAllAreasOfType(PracticeGrouping(),
                new TimePeriod { Year = 2010, YearRange = 1 });

            Assert.IsTrue(data.Count > 7000 && data.Count < 9000);
        }

        [TestMethod]
        public void GetCoreDataValuesForAllAreasOfType()
        {
            // Test with and without ignored area codes
            foreach (var ignoredAreaCodes in new[] { new List<string>(), new List<string> { "F82025" } })
            {
                IList<double> data = _groupDataReader.GetOrderedCoreDataValidValuesForAllAreasOfType(PracticeGrouping(),
                    new TimePeriod { Year = 2010, YearRange = 1 }, ignoredAreaCodes);

                Assert.IsTrue(data.Count > 7000 && data.Count < 9000);
            }
        }

        [TestMethod]
        public void GetGroupingByAreaTypeIdAndIndicatorIdAndSexIdAndAgeId()
        {
            var groupings = _groupDataReader.GetGroupingByAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(
                AreaTypeIds.CountyAndUnitaryAuthority, IndicatorIds.LifeExpectancyAtBirth,
                SexIds.Male, AgeIds.AllAges);
            Assert.IsTrue(groupings.Any());
        }

        [TestMethod]
        public void GetAllAgeIdsForIndicatorTest()
        {
            var ageIds = _groupDataReader.GetAllAgeIdsForIndicator(IndicatorIds.AdultPhysicalActivity);
            Assert.IsTrue(ageIds.Count > 1);
            Assert.IsTrue(ageIds.Contains(AgeIds.Over85));
        }

        [TestMethod]
        public void GetGroupingsByGroupIdAndIndicatorIdTest()
        {
            var groupings = _groupDataReader
                .GetGroupingsByIndicatorId(IndicatorIds.Aged0To4Years);
            Assert.IsTrue(groupings.Count > 0);
        }

        [TestMethod]
        public void When_Grouping_Read_Then_Age_And_Sex_Populated()
        {
            var groupings = _groupDataReader
                .GetGroupingsByIndicatorId(IndicatorIds.GcseAchievement);

            // Assert
            var grouping = groupings.First();
            Assert.IsNotNull(grouping.Age);
            Assert.IsNotNull(grouping.Sex);
        }

        [TestMethod]
        public void GetAllIndicators()
        {
            var indicators = _groupDataReader.GetAllIndicatorIds();
            Assert.IsTrue(indicators.Count > 1);
        }

        [TestMethod]
        public void TestGetCoreDataForIndicatorId()
        {
            var coreDataSets = _groupDataReader.GetCoreDataForIndicatorId(IndicatorIds.OnsMidYearPopulationEstimates);
            Assert.IsTrue(coreDataSets.Count > 1);

            var coreDataSet = coreDataSets.First();
            Assert.IsNotNull(coreDataSet.AgeId);
            Assert.IsNotNull(coreDataSet.SexId);
        }

        [TestMethod]
        public void TestGetIndicatorMetadataTextValues()
        {
            var indicatorMetadataTextValues = _groupDataReader.GetIndicatorMetadataTextValues(IndicatorIds.OnsMidYearPopulationEstimates);
            Assert.IsTrue(indicatorMetadataTextValues.Count > 0);
        }

        [TestMethod]
        public void TestGetGroupingsByGroupIdsAndIndicatorId()
        {
            IList<int> groupIds = new List<int>();
            groupIds.Add(GroupIds.CommonMentalHealthDisorders_Prevalence);
            groupIds.Add(GroupIds.CommonMentalHealthDisorders_RiskAndRelatedFactors);
            groupIds.Add(GroupIds.CommonMentalHealthDisorders_Services);
            groupIds.Add(GroupIds.CommonMentalHealthDisorders_QualityAndOutcomes);
            groupIds.Add(GroupIds.CommonMentalHealthDisorders_Finance);

            var groupings = _groupDataReader.GetGroupingsByGroupIdsAndIndicatorId(groupIds.ToList(), IndicatorIds.IDAOPI2);
            Assert.IsTrue(groupings.Count > 0);

            var grouping = groupings.First();
            Assert.IsNotNull(grouping.Age);
            Assert.IsNotNull(grouping.Sex);
        }

        [TestMethod]
        public void TestGetGroupings()
        {
            var indicatorId = IndicatorIds.DeprivationScoreIMD2015;
            var groupIds = new List<int> { GroupIds.PracticeProfiles_SupportingIndicators };
            var groupings = _groupDataReader.GetGroupingsByGroupIdsAndIndicatorId(groupIds, indicatorId);

            Assert.IsTrue(groupings.Count > 0);
        }

        [TestMethod]
        public void TestGetCoreDataSets()
        {
            var indicatorId = IndicatorIds.DeprivationScoreIMD2015;
            var coreDataSets = _groupDataReader.GetCoreDataForIndicatorId(indicatorId);

            Assert.IsTrue(coreDataSets.Count > 0);
        }

        private Grouping CountyUaGrouping()
        {
            return new Grouping
            {
                AgeId = AgeIds.Under75,
                SexId = SexIds.Persons,
                IndicatorId = IndicatorIds.OverallPrematureDeaths,
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority
            };
        }

        private Grouping PracticeGrouping()
        {
            return new Grouping
            {
                AgeId = AgeIds.From45To49,
                SexId = SexIds.Male,
                IndicatorId = IndicatorIds.QuinaryPopulations,
                AreaTypeId = AreaTypeIds.GpPractice
            };
        }

        private static Grouping SmokingPrevalenceGrouping()
        {
            var grouping = new Grouping
            {
                IndicatorId = IndicatorIds.AdultSmokingPrevalence,
                SexId = SexIds.Persons,
                AgeId = AgeIds.Over18
            };
            return grouping;
        }

        private static void AssertIndicatorMetadataIsOk(IndicatorMetadata metadata)
        {
            Assert.IsNotNull(metadata.ValueType);
            Assert.IsNotNull(metadata.Unit);

            Assert.IsNotNull(metadata.Descriptive);
            Assert.IsTrue(metadata.Descriptive.Count > 0);
        }

        private static Grouping GetTestGrouping()
        {
            return new Grouping
            {
                IndicatorId = IndicatorIds.AdultSmokingPrevalence2,
                DataPointYear = 2016,
                DataPointQuarter = -1,
                YearRange = 1,
                DataPointMonth = -1,
                SexId = SexIds.Persons,
                AgeId = AgeIds.Over18
            };
        }
    }
}