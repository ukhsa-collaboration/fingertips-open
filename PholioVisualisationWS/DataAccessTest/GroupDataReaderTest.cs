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
        [TestMethod]
        public void TestGetDataIncludingInequalities()
        {
            var grouping = new Grouping
            {
                IndicatorId = IndicatorIds.PopulationEating5aDay
            };

            var timePeriod = new TimePeriod()
            {
                Year = 2015,
                YearRange = 1
            };

            // Act: get all the data
            var dataList = GroupDataReader().GetDataIncludingInequalities(
                grouping, timePeriod, new List<int>(), AreaCodes.England);

            // Assert: ethnicity data found
            Assert.IsTrue(dataList.Any(x => x.CategoryTypeId == CategoryTypeIds.EthnicGroups7));

            // Assert: age data found
            Assert.IsTrue(dataList.Any(x => x.AgeId == AgeIds.From35To39));

            // Assert: sex data found
            Assert.IsTrue(dataList.Any(x => x.SexId == SexIds.Female));
        }

        [TestMethod]
        public void TestGetLsoaQuintilesWithinParentArea()
        {
            var quintiles = GroupDataReader().GetCategoriesWithinParentArea(
                CategoryTypeIds.LsoaDeprivationQuintilesInEngland2010, AreaCodes.CountyUa_Cambridgeshire,
                AreaTypeIds.Lsoa);
            Assert.IsTrue(quintiles.Count > 300 && quintiles.Count < 400);
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

            int count = GroupDataReader().GetCoreDataCountAtDataPoint(grouping);
            Assert.IsTrue(count > 100);
        }

        [TestMethod]
        public void TestGetGroupingsByIndicatorIdsAndAreaType()
        {
            IList<Grouping> groupings = GroupDataReader().GetGroupingsByGroupIdsAndIndicatorIdsAndAreaType(
                new List<int> { GroupIds.Phof_WiderDeterminantsOfHealth },
                new List<int> { IndicatorIds.IDACI, IndicatorIds.ChildrenInLowIncomeFamilies },
                AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.IsTrue(groupings.Any());
        }

        [TestMethod]
        public void TestGroupingComparatorTargetIdIsReadFromDatabase()
        {
            IList<Grouping> groupings = GroupDataReader().GetGroupingsByGroupId(
                GroupIds.HealthProfiles_OurCommunities);
            Assert.AreEqual(TargetIds.Undefined, groupings.First().ComparatorTargetId);
        }

        [TestMethod]
        public void TestGetCoreDataLimits()
        {
            Limits limits =
                GroupDataReader().GetCoreDataLimitsByIndicatorId(IndicatorIds.LifeExpectancyMsoaBasedEstimate);
            Assert.IsTrue(limits.Max > limits.Min);
        }

        [TestMethod]
        public void TestGetCoreDataLimits_WhereNoData()
        {
            Limits limits = GroupDataReader().GetCoreDataLimitsByIndicatorId(IndicatorIds.IndicatorHasNoData);
            Assert.IsNull(limits);
        }

        [TestMethod]
        public void TestGetCoreDataLimitsByIndicatorIdAndAreaTypeId()
        {
            Limits limits = GroupDataReader().GetCoreDataLimitsByIndicatorIdAndAreaTypeId(
                IndicatorIds.LifeExpectancyMsoaBasedEstimate, AreaTypeIds.GpPractice);
            Assert.IsTrue(limits.Max > limits.Min);
        }

        [TestMethod]
        public void TestGetCoreDataLimitsByIndicatorIdAndAreaTypeId_WhereNoData()
        {
            Limits limits = GroupDataReader().GetCoreDataLimitsByIndicatorIdAndAreaTypeId(
                IndicatorIds.IndicatorHasNoData, AreaTypeIds.GpPractice);
            Assert.IsNull(limits);
        }

        [TestMethod]
        public void TestGetCoreDataLimitsByIndicatorIdAndAreaTypeIdAndParentAreaCode()
        {
            Limits limits = GroupDataReader().GetCoreDataLimitsByIndicatorIdAndAreaTypeIdAndParentAreaCode(
                IndicatorIds.LifeExpectancyMsoaBasedEstimate, AreaTypeIds.GpPractice, AreaCodes.Ccg_Barnet);
            Assert.IsTrue(limits.Max > limits.Min);
        }

        [TestMethod]
        public void TestGetIndicatorMetadataByGroupingList()
        {
            IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
            var groupings = new List<Grouping>
            {
                new Grouping {IndicatorId = IndicatorIds.StatutoryHomelessness},
                new Grouping {IndicatorId = IndicatorIds.SyphilisDiagnosis}
            };
            IList<IndicatorMetadata> metadataList = reader.GetIndicatorMetadata(groupings,
                reader.GetIndicatorMetadataTextProperties());
            Assert.AreEqual(2, metadataList.Count);
            AssertIndicatorMetadataIsOk(metadataList[0]);
        }

        [TestMethod]
        public void TestGetIndicatorMetadataByGroupingTextPropertiesAssignedToCorrectIndicatorMetadataObject()
        {
            IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
            var groupings = new List<Grouping>
            {
                new Grouping {IndicatorId = IndicatorIds.ExcessWinterDeaths},
                new Grouping {IndicatorId = IndicatorIds.DeathsFromLungCancer},
                new Grouping {IndicatorId = IndicatorIds.OverallPrematureDeaths}
            };
            IList<IndicatorMetadata> metadataList = reader.GetIndicatorMetadata(groupings,
                reader.GetIndicatorMetadataTextProperties());

            Assert.AreEqual(IndicatorIds.ExcessWinterDeaths,
                metadataList.FirstOrDefault(x => x.Name.ToLower()
                    .Contains("winter deaths")).IndicatorId
                );

            Assert.AreEqual(IndicatorIds.OverallPrematureDeaths,
                metadataList.FirstOrDefault(x => x.Name.ToLower()
                    .Contains("overall premature deaths")).IndicatorId
                );

            Assert.AreEqual(IndicatorIds.DeathsFromLungCancer,
                metadataList.FirstOrDefault(x => x.Name.ToLower()
                    .Contains("lung cancer")).IndicatorId
                );
        }

        [TestMethod]
        public void TestGetIndicatorMetadataByGrouping()
        {
            IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
            IndicatorMetadata metadata = reader.GetIndicatorMetadata(
                new Grouping { IndicatorId = IndicatorIds.SmokingAtTimeOfDelivery },
                reader.GetIndicatorMetadataTextProperties());
            AssertIndicatorMetadataIsOk(metadata);
        }

        [TestMethod]
        public void TestGetIndicatorMetadataByIndicatorId()
        {
            var indicatorId = IndicatorIds.ObesityYear6;
            IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
            IndicatorMetadata metadata = reader.GetIndicatorMetadata(indicatorId,
                reader.GetIndicatorMetadataTextProperties());
            Assert.AreEqual(indicatorId, metadata.IndicatorId);
            AssertIndicatorMetadataIsOk(metadata);
        }

        [TestMethod]
        public void TestGetIndicatorMetaDataContainsIndicatorContent()
        {
            // Assert Indicator Metadata for an expected diabetes indicator is present
            IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
            IList<IndicatorMetadata> metadataList = reader.GetGroupSpecificIndicatorMetadataTextValues(
                new List<Grouping> { new Grouping { GroupId = GroupIds.Diabetes_TreatmentTargets } },
                reader.GetIndicatorMetadataTextProperties());

            Assert.IsTrue(metadataList.Any());

            const int indicatorId = IndicatorIds.Population;
            IndicatorMetadata genericMetadata = reader.GetIndicatorMetadata(indicatorId,
                reader.GetIndicatorMetadataTextProperties());

            Assert.IsNotNull(genericMetadata.Descriptive[IndicatorMetadataTextColumnNames.IndicatorContent]);
        }

        [TestMethod]
        public void TestGetIndicatorMetadataTextPropertiesContainsSystemContentField()
        {
            // Assert Indicator Metadata contains a field that is defined as SystemContent.
            IList<IndicatorMetadataTextProperty> properties =
                GroupDataReader().GetIndicatorMetadataTextProperties();

            var systemContentProperties = properties.Where(x => x.IsSystemContent).ToList();

            Assert.IsTrue(systemContentProperties.Count > 0);
        }

        [TestMethod]
        public void TestGetIndicatorMetadataByIndicatorIdList()
        {
            IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
            IList<IndicatorMetadata> metadata = reader.GetIndicatorMetadata(
                new List<int> {
                    IndicatorIds.KilledAndSeriouslyInjuredOnRoads,
                    IndicatorIds.ObesityYear6 },
                reader.GetIndicatorMetadataTextProperties()
                );

            // Check metadata is as expected
            Assert.AreEqual(2, metadata.Count);
            AssertIndicatorMetadataIsOk(metadata[0]);
            AssertIndicatorMetadataIsOk(metadata[1]);
        }

        private static void AssertIndicatorMetadataIsOk(IndicatorMetadata metadata)
        {
            Assert.IsNotNull(metadata.ValueType);
            Assert.IsNotNull(metadata.Unit);

            Assert.IsNotNull(metadata.Descriptive);
            Assert.IsTrue(metadata.Descriptive.Count > 0);
        }

        [TestMethod]
        public void TestIndicatorMetadataHasTargetDefined()
        {
            IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
            var groupings = new List<Grouping>
            {
                new Grouping {IndicatorId = IndicatorIds.HIVLateDiagnosis}
            };
            IList<IndicatorMetadata> metadataList = reader.GetIndicatorMetadata(groupings,
                reader.GetIndicatorMetadataTextProperties());
            Assert.IsNotNull(metadataList.First().TargetConfig);
        }

        [TestMethod]
        public void TestGetGroupSpecificIndicatorMetadataTextValues()
        {
            // Assert group specific metadata is found
            IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
            IList<IndicatorMetadata> metadataList = reader.GetGroupSpecificIndicatorMetadataTextValues(
                new List<Grouping> { new Grouping { GroupId = GroupIds.HealthProfiles_OurCommunities } },
                reader.GetIndicatorMetadataTextProperties());

            Assert.IsTrue(metadataList.Any());

            // Assert generic and specific metadata properties are different
            const int indicatorId = IndicatorIds.ViolentCrime;

            IndicatorMetadata specificMetadata =
                (from m in metadataList where m.IndicatorId == indicatorId select m).First();

            IndicatorMetadata genericMetadata = reader.GetIndicatorMetadata(indicatorId,
                reader.GetIndicatorMetadataTextProperties());

            Assert.AreNotEqual(genericMetadata.Descriptive[IndicatorMetadataTextColumnNames.Name],
                specificMetadata.Descriptive[IndicatorMetadataTextColumnNames.Name]);
        }

        [TestMethod]
        public void TestGetGroupSpecificIndicatorMetadataTextValuesWhereNoGroupings()
        {
            // Assert group specific metadata is found
            IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
            IList<IndicatorMetadata> metadataList = reader.GetGroupSpecificIndicatorMetadataTextValues(
                new List<Grouping>(), reader.GetIndicatorMetadataTextProperties());

            Assert.AreEqual(0, metadataList.Count);
        }

        [TestMethod]
        public void TestGetIndicatorMetadataTextProperties()
        {
            IList<IndicatorMetadataTextProperty> properties =
                GroupDataReader().GetIndicatorMetadataTextProperties();

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
            IList<CoreDataSet> data = GroupDataReader().GetCoreData(grouping, TimePeriod.GetDataPoint(grouping));
            Assert.AreEqual(0, data.Count);
        }

        [TestMethod]
        public void TestGetCoreDataNoAreaCodeOneDataRecord()
        {
            Grouping grouping = GetTestGrouping();
            IList<CoreDataSet> data = GroupDataReader().GetCoreData(grouping, TimePeriod.GetDataPoint(grouping),
                AreaCodes.Gor_EastOfEngland);
            Assert.AreEqual(1, data.Count);
        }

        [TestMethod]
        public void TestGetCoreDataForCategoryAreaNoData()
        {
            CategoryArea categoryArea = CategoryArea.New(CategoryTypeIds.DeprivationDecileGp2015, 1);

            CoreDataSet data = GroupDataReader()
                .GetCoreDataForCategoryArea(SmokingPrevalenceGrouping(),
                    new TimePeriod { Month = -1, Quarter = -1, Year = 2000, YearRange = 1 },
                    categoryArea);
            Assert.IsNull(data);
        }

        [TestMethod]
        public void TestGetCoreDataForCategoryAreaOneRecord()
        {
            CategoryArea categoryArea = CategoryArea.New(CategoryTypeIds.DeprivationDecileGp2010, 1);

            CoreDataSet data = GroupDataReader()
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

            var dataList = GroupDataReader()
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


        [TestMethod]
        public void TestGetCoreDataTwoAreas()
        {
            Grouping grouping = GetTestGrouping();
            IList<CoreDataSet> data = GroupDataReader().GetCoreData(grouping, TimePeriod.GetDataPoint(grouping),
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
            IList<CoreDataSet> data = GroupDataReader().GetCoreData(grouping, TimePeriod.GetDataPoint(grouping),
                AreaCodes.Gor_EastOfEngland);
            Assert.AreEqual(0, data.Count);
        }

        [TestMethod]
        public void TestGetCoreDataInvalidIndicatorId()
        {
            Grouping grouping = GetTestGrouping();
            grouping.IndicatorId = -1;
            IList<CoreDataSet> data = GroupDataReader().GetCoreData(grouping, TimePeriod.GetDataPoint(grouping),
                AreaCodes.Gor_EastOfEngland);
            Assert.AreEqual(0, data.Count);
        }

        [TestMethod]
        public void TestGetCoreDataByIndicatorIdAndAreaCode()
        {
            IList<CoreDataSet> data = GroupDataReader().GetCoreData(IndicatorIds.QofPoints, "J83045");
            Assert.IsTrue(data.Count > 1);
        }

        [TestMethod]
        public void TestGetCoreDataForCategory()
        {
            IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
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
            CoreDataSet data = reader.GetCoreDataForCategory(grouping, TimePeriod.GetDataPoint(grouping),
                AreaCodes.CountyUa_CentralBedfordshire, CategoryTypeIds.LsoaDeprivationQuintilesInEngland2010, CategoryIds.LeastDeprivedQuintile);

            // Assert
            AssertDataAndGroupingMatch(data, grouping);
        }

        [TestMethod]
        public void TestGetCoreDataMonthlyOneArea()
        {
            IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
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
            IList<CoreDataSet> data = reader.GetCoreData(grouping, TimePeriod.GetDataPoint(grouping),
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

        private static Grouping GetTestGrouping()
        {
            return new Grouping
            {
                IndicatorId = IndicatorIds.YoungPeopleInTreatmentNotRetainedForMoreThan12Weeks,
                DataPointYear = 2008,
                DataPointQuarter = -1,
                YearRange = 1,
                DataPointMonth = -1,
                SexId = SexIds.Persons,
                AgeId = 180
            };
        }

        [TestMethod]
        public void TestGetGroupDataByGroupIdAndAreaTypeId()
        {
            IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
            IList<Grouping> groupings102 =
                groupDataReader.GetGroupingsByGroupIdAndAreaTypeId(
                GroupIds.Phof_HealthProtection,
                AreaTypeIds.CountyAndUnitaryAuthority);

            IList<Grouping> groupings101 =
                groupDataReader.GetGroupingsByGroupIdAndAreaTypeId(
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

            IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
            IList<Grouping> groupings = groupDataReader.GetGroupingsByGroupIdIndicatorIdAgeId(
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

            IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
            IList<Grouping> groupings = groupDataReader.GetGroupingsByGroupIdIndicatorIdSexId(
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
            IList<Grouping> groupingsFiltered = GroupDataReader().GetGroupingsByIndicatorIds(indicatorIds, groupIds);
            Assert.IsTrue(groupingsFiltered.Count() >= 3);
        }

        [TestMethod]
        public void TestGetGroupDataByIndicatorsFindsMostRecentYear()
        {
            IProfileReader reader = ReaderFactory.GetProfileReader();
            IList<int> groupIds = reader.GetGroupIdsFromAllProfiles();

            var indicatorIds = new List<int> { IndicatorIds.AdultDrugMisuse };
            IList<Grouping> groupings = GroupDataReader().GetGroupingsByIndicatorIds(indicatorIds, groupIds);
            Assert.AreEqual(2011, groupings[0].DataPointYear);
        }

        [TestMethod]
        public void TestGetIndicatorIdsByGroup()
        {
            IList<int> indicatorIds = GroupDataReader().GetIndicatorIdsByGroupIdAndAreaTypeId(
                GroupIds.Phof_HealthProtection,
                AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.IsTrue(indicatorIds.Count > 20 && indicatorIds.Count < 30,
                "Number of indicators should be around 25");
            Assert.IsTrue(indicatorIds.Contains(IndicatorIds.TreatmentCompletionForTB));
        }

        [TestMethod]
        public void TestGetIndicatorIdsByGroupOrderedBySequence()
        {
            IGroupDataReader groupDataReader = GroupDataReader();

            IList<Grouping> groupings = groupDataReader.GetGroupingsByGroupIdAndAreaTypeId(
                GroupIds.HealthProfiles_OurCommunities, AreaTypeIds.CountyAndUnitaryAuthority);

            IList<int> sequencedIds = (from g in groupings orderby g.Sequence select g.IndicatorId).Distinct().ToList();

            IList<int> indicatorIds = groupDataReader.GetIndicatorIdsByGroupIdAndAreaTypeId(
                GroupIds.HealthProfiles_OurCommunities, AreaTypeIds.CountyAndUnitaryAuthority);

            for (int i = 0; i < indicatorIds.Count - 1; i++)
            {
                Assert.AreEqual(sequencedIds[i], indicatorIds[i]);
            }
        }

        [TestMethod]
        public void TestGetCoreDataForAllAges()
        {
            IList<CoreDataSet> data = GroupDataReader().GetCoreDataForAllAges(
                IndicatorIds.QuinaryPopulations,
                new TimePeriod { Year = 2014, YearRange = 1 },
                AreaCodes.Gp_Burnham,
                SexIds.Male);

            Assert.AreEqual(19, data.Count);
        }

        [TestMethod]
        public void TestGetCoreDataForAllSexes()
        {
            IList<CoreDataSet> data = GroupDataReader().GetCoreDataForAllSexes(
                IndicatorIds.LifeExpectancyAtBirth,
                new TimePeriod { Year = 2010, YearRange = 3 },
                AreaCodes.England,
                AgeIds.AllAges);

            Assert.AreEqual(2/*male & female*/, data.Count);
        }

        [TestMethod]
        public void TestGetGroupDataByIndicatorId()
        {
            Grouping grouping = GroupDataReader().GetGroupingsByGroupIdAndIndicatorId(
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

            Grouping grouping = GroupDataReader().GetGroupings(groupId, IndicatorIds.IDAOPI2, areaTypeId,
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

            Grouping grouping = GroupDataReader().GetGroupings(groupId, IndicatorIds.IDAOPI2,
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
                GroupDataReader().GetGroupingsWithoutAgeId(groupId, IndicatorIds.IDAOPI2, areaTypeId, sexId).FirstOrDefault();

            Assert.AreEqual(IndicatorIds.IDAOPI2, grouping.IndicatorId);
            Assert.AreEqual(sexId, grouping.SexId);
            Assert.AreEqual(groupId, grouping.GroupId);
            Assert.AreEqual(areaTypeId, grouping.AreaTypeId);
        }

        private static IGroupDataReader GroupDataReader()
        {
            IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
            return reader;
        }

        [TestMethod]
        public void TestGetGroupMetadata()
        {
            IList<GroupingMetadata> list = GroupDataReader().GetGroupingMetadataList(
                new List<int>
                {
                    GroupIds.Phof_HealthProtection,
                    GroupIds.LongerLives
                });
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void TestGetGroupMetadataOrderedBySequence()
        {
            IList<GroupingMetadata> list = GroupDataReader().GetGroupingMetadataList(new List<int>
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
            IList<int> areaTypeIds = GroupDataReader().GetDistinctGroupingAreaTypeIds(
                new List<int> {
                    GroupIds.Phof_HealthProtection,
                    GroupIds.Phof_WiderDeterminantsOfHealth
                });
            Assert.IsTrue(areaTypeIds.Any());
            Assert.IsTrue(areaTypeIds.Contains(AreaTypeIds.CountyAndUnitaryAuthority));
        }

        [TestMethod]
        public void TestGetDistinctGroupingAreaTypeIds_WhereNoGroupIds()
        {
            IList<int> areaTypeIds = GroupDataReader().GetDistinctGroupingAreaTypeIds(new List<int>());

            Assert.IsFalse(areaTypeIds.Any());
        }

        [TestMethod]
        public void TestGetCoreDataListForChildrenOfArea()
        {
            IList<CoreDataSet> data = GroupDataReader().GetCoreDataListForChildrenOfArea(
                new Grouping
                {
                    AgeId = AgeIds.From45To49,
                    SexId = SexIds.Male,
                    IndicatorId = IndicatorIds.QuinaryPopulations,
                    AreaTypeId = AreaTypeIds.GpPractice
                },
                new TimePeriod { Year = 2010, YearRange = 1 },
                AreaCodes.Ccg_Chiltern);

            Assert.IsTrue(data.Count > 20 && data.Count < 50);
        }

        [TestMethod]
        public void TestGetCoreDataListForChildrenOfCategoryArea()
        {
            CategoryArea categoryArea = CategoryArea.New(
                CategoryTypeIds.DeprivationDecileCountyAndUA2010, 1);

            IList<CoreDataSet> data = GroupDataReader().GetCoreDataListForChildrenOfCategoryArea(
                new Grouping
                {
                    AgeId = AgeIds.Under75,
                    SexId = SexIds.Persons,
                    IndicatorId = IndicatorIds.OverallPrematureDeaths,
                    AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority
                },
                new TimePeriod { Year = 2010, YearRange = 3 },
                categoryArea);

            // Decile contains 152/10 areas
            Assert.IsTrue(data.Count > 10 && data.Count < 20);
        }

        [TestMethod]
        public void TestGetCoreDataListForAllCategoryAreasOfCategoryAreaType()
        {
            IList<CoreDataSet> data = GroupDataReader().GetCoreDataListForAllCategoryAreasOfCategoryAreaType(
                new Grouping
                {
                    AgeId = AgeIds.Under75,
                    SexId = SexIds.Persons,
                    IndicatorId = IndicatorIds.OverallPrematureDeaths,
                    AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority
                },
                new TimePeriod { Year = 2010, YearRange = 3 },
                CategoryTypeIds.DeprivationDecileCountyAndUA2010,
                AreaCodes.England);

            Assert.AreEqual(10, data.Count, "Expected 10 areas in decile");
        }

        [TestMethod]
        public void GetCoreDataForAllAreasOfType()
        {
            IList<CoreDataSet> data = GroupDataReader().GetCoreDataForAllAreasOfType(
                new Grouping { AgeId = 12, SexId = SexIds.Male, IndicatorId = 337, AreaTypeId = 7 },
                new TimePeriod { Year = 2010, YearRange = 1 });

            Assert.IsTrue(data.Count > 7000 && data.Count < 9000);
        }

        [TestMethod]
        public void GetCoreDataValuesForAllAreasOfType()
        {
            var reader = Reader();

            // Test with and without ignored area codes
            foreach (var ignoredAreaCodes in new[] { new List<string>(), new List<string> { "F82025" } })
            {
                IList<double> data = reader.GetOrderedCoreDataValidValuesForAllAreasOfType(
                    new Grouping { AgeId = 12, SexId = SexIds.Male, IndicatorId = 337, AreaTypeId = 7 },
                    new TimePeriod { Year = 2010, YearRange = 1 }, ignoredAreaCodes);

                Assert.IsTrue(data.Count > 7000 && data.Count < 9000);
            }
        }

        [TestMethod]
        public void GetAllAgeIdsForIndicatorTest()
        {
            var ageIds = Reader().GetAllAgeIdsForIndicator(IndicatorIds.AdultPhysicalActivity);
            Assert.IsTrue(ageIds.Count > 1);
            Assert.IsTrue(ageIds.Contains(AgeIds.Over65));
        }

        [TestMethod]
        public void GetGroupingsByGroupIdAndIndicatorIdTest()
        {
            var groupings = Reader()
                .GetGroupingsByIndicatorId(IndicatorIds.MortalityRateFromCausesConsideredPreventable);
            Assert.IsTrue(groupings.Count > 0);
        }

        [TestMethod]
        public void When_Grouping_Read_Then_Age_And_Sex_Populated()
        {
            var groupings = Reader()
                .GetGroupingsByIndicatorId(IndicatorIds.MortalityRateFromCausesConsideredPreventable);

            // Assert
            var grouping = groupings.First();
            Assert.IsNotNull(grouping.Age);
            Assert.IsNotNull(grouping.Sex);
        }

        [TestMethod]
        public void GetAllIndicators()
        {
            var indicators = GroupDataReader().GetAllIndicators();
            Assert.IsTrue(indicators.Count > 1);
        }

        private static IGroupDataReader Reader()
        {
            IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
            return reader;
        }
    }
}