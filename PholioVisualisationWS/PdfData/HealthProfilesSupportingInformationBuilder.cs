using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.KeyMessages;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class HealthProfilesSupportingInformationBuilder
    {
        private readonly string areaCode;
        private readonly IAreasReader areasReader = ReaderFactory.GetAreasReader();
        private readonly TrendDataReader trendDataReader = ReaderFactory.GetTrendDataReader();
        private readonly IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();

        private readonly HealthProfilesContent content = new HealthProfilesContent();
        private readonly HealthProfilesData data = new HealthProfilesData();
        private readonly HealthProfilesGroupRootSelector groupRootSelector =
            new HealthProfilesGroupRootSelector();
        private readonly IndicatorMetadataCollection indicatorMetadataCollection =
            new IndicatorMetadataCollection();
        private readonly PholioReader pholioReader = ReaderFactory.GetPholioReader();
        private readonly IProfileReader profileReader = ReaderFactory.GetProfileReader();
        private CoreDataSetProvider benchmarkDataProvider;
        private CoreDataSetProvider coreDataSetProvider;
        private CoreDataProcessor coreDataProcessor = new CoreDataProcessor(null);

        // Hard coded but will allow this to be passed as a parameter in the future
        private string benchmarkAreaCode = AreaCodes.England;

        public HealthProfilesSupportingInformationBuilder(string areaCode)
        {
            this.areaCode = areaCode;
        }

        public HealthProfilesSupportingInformation Build()
        {
            var supportingInformation = new HealthProfilesSupportingInformation
            {
                HealthProfilesData = data,
                HealthProfilesContent = content
            };

            // Init data
            Area area = areasReader.GetAreaFromCode(areaCode);
            coreDataSetProvider = new CoreDataSetProviderFactory().New(area);
            Area benchmarkArea = areasReader.GetAreaFromCode(benchmarkAreaCode);
            benchmarkDataProvider = new CoreDataSetProviderFactory().New(benchmarkArea);
            InitSupportingGroupData();
            InitMainGroupData();

            // Page 1
            AssignPopulation();
            content.AreaType = AreaTypeLabel.GetLabelFromAreaCode(areaCode);
            AssignKeyMessages(area);

            // Page 2 top
            AssignDeprivationQuintilesPopulation();
            AssignLsoaQuintiles();

            // Page 2 bottom
            AssignLifeExpectancy();
            AssignLifeExpectancyByDeciles();

            // Page 3 top
            AssignEarlyDeathFromAllCauses();
            AssignEarlyDeathCvd();
            AssignEarlyDeathCancer();

            // Page 3 bottom
            AssignHealthInequalitiesEthnicity();

            return supportingInformation;
        }

        private void AssignKeyMessages(Area area)
        {
            KeyMessageData data = new HealthProfilesKeyMessageDataBuilder(area, coreDataSetProvider,
                benchmarkDataProvider, indicatorMetadataCollection, groupRootSelector).BuildData();

            IList<KeyMessageOverride> overrides = pholioReader.GetKeyMessageOverrides(
                ProfileIds.HealthProfiles, area.Code);
            var staticMessageProvider = new StaticMessageProvider(overrides);

            var message5 = new KeyMessageOverrideCleaner(staticMessageProvider.GetMessage(5))
                .CleanMessage;

            content.KeyMessages = new List<string>
            {
                staticMessageProvider.GetMessage(1) ?? 
                    new HealthProfilesKeyMessage1Builder().ProcessKeyMessage(data),
                staticMessageProvider.GetMessage(2) ?? 
                    new HealthProfilesKeyMessage2Builder().ProcessKeyMessage(data),
                staticMessageProvider.GetMessage(3) ?? 
                    new HealthProfilesKeyMessage3Builder().ProcessKeyMessage(data),
                staticMessageProvider.GetMessage(4) ?? 
                    new HealthProfilesKeyMessage4Builder().ProcessKeyMessage(data),
                message5
            };
        }

        private void InitSupportingGroupData()
        {
            // Supporting data
            int profileId = ProfileIds.HealthProfilesSupportingIndicators;
            int supportingGroupId = new GroupIdProvider(profileReader).GetGroupIds(profileId)[0];
            GroupData groupData = new GroupDataAtDataPointRepository
            {
                AssignChildAreaData = false,
                AssignAreas = false
            }.GetGroupData(benchmarkAreaCode,
                AreaTypeIds.CountyAndUnitaryAuthority, profileId, supportingGroupId);
            groupRootSelector.SupportingGroupRoots = groupData.GroupRoots;
            indicatorMetadataCollection.AddIndicatorMetadata(groupData.IndicatorMetadata);
        }

        private void InitMainGroupData()
        {
            int profileId = ProfileIds.HealthProfiles;
            IList<int> groupIds = new GroupIdProvider(profileReader).GetGroupIds(profileId);
            var groupRoots = new List<GroupRoot>();
            foreach (int groupId in groupIds)
            {
                GroupData groupDataForSpineChart = new GroupDataAtDataPointRepository
                {
                    AssignChildAreaData = false,
                    AssignAreas = false
                }.GetGroupData(benchmarkAreaCode,
                    AreaTypeIds.CountyAndUnitaryAuthority, profileId, groupId);

                groupRoots.AddRange(groupDataForSpineChart.GroupRoots);

                indicatorMetadataCollection.AddIndicatorMetadata(groupDataForSpineChart.IndicatorMetadata);
            }
            groupRootSelector.MainGroupRoots = groupRoots;
        }

        public void AssignLifeExpectancy()
        {
            Grouping maleGrouping = groupRootSelector.SlopeIndexOfInequalityForLifeExpectancyMale.FirstGrouping;
            Grouping femaleGrouping = groupRootSelector.SlopeIndexOfInequalityForLifeExpectancyFemale.FirstGrouping;

            IndicatorMetadata metadata =
                indicatorMetadataCollection.GetIndicatorMetadataById(maleGrouping.IndicatorId);

            var formatter = NumericFormatterFactory.NewWithLimits(metadata, (Limits) null);

            // Female
            CoreDataSet coreDataSet = coreDataSetProvider.GetData(femaleGrouping,
                TimePeriod.GetDataPoint(femaleGrouping), metadata);
            if (coreDataSet != null && coreDataSet.IsValueValid)
            {
                formatter.Format(coreDataSet);
                data.LifeExpectancyGapFemale = coreDataSet.ValueFormatted;
            }

            // Male
            coreDataSet = coreDataSetProvider.GetData(maleGrouping,
                TimePeriod.GetDataPoint(maleGrouping), metadata);
            if (coreDataSet != null && coreDataSet.IsValueValid)
            {
                formatter.Format(coreDataSet);
                data.LifeExpectancyGapMale = coreDataSet.ValueFormatted;
            }
        }

        private void AssignLifeExpectancyByDeciles()
        {
            var rootLifeExpectancyAtBirthMale = groupRootSelector.LifeExpectancyAtBirthMale;
            var rootXAxisMale = groupRootSelector.XAxisOfHealthProfilesChartAtBottomOfPage2Male;
            data.LifeExpectancyMaleByDeprivationDecile = GetXYPointsForLsoaDeprivationDeciles(
               rootXAxisMale, rootLifeExpectancyAtBirthMale);

            var rootLifeExpectancyAtBirthFemale = groupRootSelector.LifeExpectancyAtBirthFemale;
            var rootXAxisFemale = groupRootSelector.XAxisOfHealthProfilesChartAtBottomOfPage2Female;
            data.LifeExpectancyFemaleByDeprivationDecile = GetXYPointsForLsoaDeprivationDeciles(
               rootXAxisFemale, rootLifeExpectancyAtBirthFemale);

            data.LifeExpectancyMaleLineOfBestFit =
                GetLimitPoints(rootLifeExpectancyAtBirthMale.FirstGrouping);
            data.LifeExpectancyFemaleLineOfBestFit =
                GetLimitPoints(rootLifeExpectancyAtBirthFemale.FirstGrouping);
        }

        private IList<XyPoint> GetLimitPoints(Grouping grouping)
        {
            var yData = groupDataReader.GetCoreDataListForAllCategoryAreasOfCategoryAreaType(
                grouping, TimePeriod.GetDataPoint(grouping),
                CategoryTypeIds.LimitsForHealthProfilesLifeExpectancyChart, areaCode);
            coreDataProcessor.TruncateList(yData);

            var xData = new List<int> { 0, 1 };
            return new XyPointListBuilder(xData, yData).XyPoints;
        }

        private IList<XyPoint> GetXYPointsForLsoaDeprivationDeciles(GroupRoot xRoot, GroupRoot yRoot)
        {
            const int categoryTypeId = CategoryTypeIds.LsoaDeprivationDecilesWithinArea;
            var xData = GetLsoaDeprivationDecilesWithinArea(xRoot, categoryTypeId);
            var yData = GetLsoaDeprivationDecilesWithinArea(yRoot, categoryTypeId);

            coreDataProcessor.TruncateList(xData);
            coreDataProcessor.TruncateList(yData);

            return new XyPointListBuilder(xData, yData).XyPoints;
        }

        private IList<CoreDataSet> GetLsoaDeprivationDecilesWithinArea(GroupRoot root, int categoryTypeId)
        {
            var grouping = root.FirstGrouping;
            var dataList = groupDataReader.GetCoreDataListForAllCategoryAreasOfCategoryAreaType(
                grouping, TimePeriod.GetDataPoint(grouping), categoryTypeId, areaCode);

            coreDataProcessor.TruncateList(dataList);

            return dataList;
        }

        private void AssignLsoaQuintiles()
        {
            data.LsoaQuintiles = groupDataReader.GetCategoriesWithinParentArea(
                CategoryTypeIds.LsoaDeprivationQuintilesInEngland, areaCode, AreaTypeIds.Lsoa);
        }

        private void AssignPopulation()
        {
            Grouping grouping = groupRootSelector.Population.FirstGrouping;
            IndicatorMetadata metadata = indicatorMetadataCollection.GetIndicatorMetadataById(grouping.IndicatorId);
            CoreDataSet areaData = coreDataSetProvider.GetData(grouping, TimePeriod.GetDataPoint(grouping),
                metadata);

            data.Population = areaData == null || areaData.IsValueValid == false
                ? null
                : NumberCommariser.Commarise0DP(NumberRounder.ToNearest1000(areaData.Value));
        }

        private void AssignDeprivationQuintilesPopulation()
        {
            var grouping = groupRootSelector.PercentageOfPeoplePerDeprivationQuintile.FirstGrouping;
            var timePeriod = TimePeriod.GetDataPoint(grouping);

            data.DeprivationQuintilesPopulationLocal = GetDeprivationQuintiles(
                    grouping, timePeriod, areaCode);

            data.DeprivationQuintilesPopulationEngland = GetDeprivationQuintiles(
                    grouping, timePeriod, benchmarkAreaCode);
        }

        private IList<CoreDataSet> GetDeprivationQuintiles(Grouping grouping,
            TimePeriod timePeriod, string _areaCode)
        {
            var dataList = groupDataReader
                .GetCoreDataListForAllCategoryAreasOfCategoryAreaType(
                    grouping, timePeriod, CategoryTypeIds.LsoaDeprivationQuintilesInEngland, _areaCode);

            coreDataProcessor.TruncateList(dataList);

            return dataList;
        }

        private void AssignEarlyDeathFromAllCauses()
        {

            var groupRootMale = groupRootSelector.OverallPrematureDeathsMale;
            var groupRootFemale = groupRootSelector.OverallPrematureDeathsFemale;

            data.EarlyDeathAllCausesMale =
                GetLocalAndEnglandChartDataWithDeprivation(groupRootMale);

            data.EarlyDeathAllCausesFemale =
                GetLocalAndEnglandChartDataWithDeprivation(groupRootFemale);

            // Add limits & step
            var limits = groupDataReader.GetCoreDataLimitsByIndicatorId(groupRootMale.IndicatorId);
            data.EarlyDeathAllCausesMale.CalculateLimitsAndStep(0, limits.Max);
            data.EarlyDeathAllCausesFemale.CalculateLimitsAndStep(0, limits.Max);

            // Time periods - as per spec year should always be year + 1 (middle of 3 year range)
            var maleEnglandAverageCoreDataSets = trendDataReader.GetTrendData(
                groupRootMale.FirstGrouping, benchmarkAreaCode);
            data.EarlyDeathYearRange = maleEnglandAverageCoreDataSets
                .Select(x => x.Year + 1).Distinct().ToList();
        }

        private LocalAndEnglandChartDataWithDeprivation GetLocalAndEnglandChartDataWithDeprivation(GroupRoot groupRoot)
        {
            var grouping = groupRoot.FirstGrouping;

            var chartData = new LocalAndEnglandChartDataWithDeprivation(
                GetLocalAndEnglandChartData(groupRoot));

            chartData.LocalLeastDeprived = GetDeprivationQuintileValues(grouping,
                CategoryIds.LeastDeprived);
            chartData.LocalMostDeprived = GetDeprivationQuintileValues(grouping,
                CategoryIds.MostDeprived);

            return chartData;
        }

        private List<double> GetDeprivationQuintileValues(Grouping grouping, int categoryId)
        {
            var coreDataList = trendDataReader.GetTrendDataForSpecificCategory(grouping,
                areaCode, CategoryTypeIds.LsoaDeprivationQuintilesWithinArea, categoryId);

            coreDataProcessor.TruncateList(coreDataList);

            return coreDataList.Select(x => x.Value).ToList();
        }

        private void AssignEarlyDeathCvd()
        {
            var groupRoot = groupRootSelector.AdultUnder75MortalityRateCvd;
            var chartData = GetLocalAndEnglandChartData(groupRoot);
            SetSpecificCauseLimits(chartData);
            data.EarlyDeathCvd = chartData;
        }

        private void AssignEarlyDeathCancer()
        {
            var groupRoot = groupRootSelector.AdultUnder75MortalityRateCancer;
            var chartData = GetLocalAndEnglandChartData(groupRoot);
            SetSpecificCauseLimits(chartData);
            data.EarlyDeathCancer = chartData;
        }

        private static void SetSpecificCauseLimits(LocalAndEnglandChartData chartData)
        {
            chartData.YAxis = new LimitsWithStep(
                new Limits
                {
                    Min = 0,
                    Max = 250
                },
                50);
        }

        private LocalAndEnglandChartData GetLocalAndEnglandChartData(GroupRoot groupRoot)
        {
            var grouping = groupRoot.FirstGrouping;
            var localAverageCoreDataSets = trendDataReader.GetTrendData(grouping, areaCode);
            var englandAverageCoreDataSets = trendDataReader.GetTrendData(grouping, benchmarkAreaCode);
            return new LocalAndEnglandChartData
            {
                Local = GetValues(localAverageCoreDataSets),
                England = GetValues(englandAverageCoreDataSets)
            };
        }

        private List<double> GetValues(IList<CoreDataSet> data)
        {
            coreDataProcessor.TruncateList(data);
            return data.Select(x => x.Value).ToList();
        }

        private void AssignHealthInequalitiesEthnicity()
        {
            Grouping grouping = groupRootSelector.HealthInequalitiesEthnicity.FirstGrouping;
            IndicatorMetadata metadata = indicatorMetadataCollection.GetIndicatorMetadataById(grouping.IndicatorId);
            TimePeriod timePeriod = TimePeriod.GetDataPoint(grouping);
            var formatter = NumericFormatterFactory.New(metadata, groupDataReader);

            IList<CoreDataSet> coreDataSetsLocal = groupDataReader
                .GetCoreDataListForAllCategoryAreasOfCategoryAreaType(
                    grouping, timePeriod, CategoryTypeIds.EthnicGroups5, areaCode);

            data.EmergencyAdmissionsLocalByEthnicity = AddEthnicityLabelToCoreDataSet(coreDataSetsLocal,
                formatter);

            IList<CoreDataSet> coreDataSetsEngland = groupDataReader.
                GetCoreDataListForAllCategoryAreasOfCategoryAreaType(
                    grouping, timePeriod, CategoryTypeIds.EthnicGroups5, benchmarkAreaCode);

            data.EmergencyAdmissionsEnglandByEthnicity = AddEthnicityLabelToCoreDataSet(coreDataSetsEngland,
                formatter);

            data.EmergencyAdmissionsLocal = coreDataSetProvider.GetData(grouping, timePeriod, metadata);
            data.EmergencyAdmissionsEngland = benchmarkDataProvider.GetData(grouping, timePeriod, metadata);

            formatter.Format(data.EmergencyAdmissionsLocal);
            formatter.Format(data.EmergencyAdmissionsEngland);
        }

        private Dictionary<string, CoreDataSet> AddEthnicityLabelToCoreDataSet(
            IEnumerable<CoreDataSet> coreDataSets, NumericFormatter formatter)
        {
            var data = new Dictionary<string, CoreDataSet>();

            foreach (CoreDataSet coreDataSet in coreDataSets)
            {
                formatter.Format(coreDataSet);
                coreDataProcessor.Truncate(coreDataSet);
                switch (coreDataSet.CategoryId)
                {
                    case 1:
                        data.Add("White", coreDataSet);
                        break;
                    case 2:
                        data.Add("Mixed", coreDataSet);
                        break;
                    case 3:
                        data.Add("Asian", coreDataSet);
                        break;
                    case 4:
                        data.Add("Black", coreDataSet);
                        break;
                    case 5:
                        data.Add("Chinese", coreDataSet);
                        break;
                    case 6:
                        data.Add("Other", coreDataSet);
                        break;
                    case 7:
                        data.Add("Unknown", coreDataSet);
                        break;
                }
            }
            return data;
        }
    }

    class Deprivations
    {
        public List<double> MostDeprived { get; set; }
        public List<double> LeastDeprived { get; set; }
    }
}