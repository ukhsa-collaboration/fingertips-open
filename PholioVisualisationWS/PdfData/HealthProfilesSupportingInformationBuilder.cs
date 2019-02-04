using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly ITrendDataReader trendDataReader = ReaderFactory.GetTrendDataReader();
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

            // Init areas
            var area = areasReader.GetAreaFromCode(areaCode);
            coreDataSetProvider = new CoreDataSetProviderFactory().New(area);
            var benchmarkArea = areasReader.GetAreaFromCode(benchmarkAreaCode);
            benchmarkDataProvider = new CoreDataSetProviderFactory().New(benchmarkArea);

            // Init data
            var areaTypeId = HealthProfilesAreaTypeHelper.GetCompositeAreaTypeId(area);
            InitSupportingGroupData(areaTypeId);
            InitMainGroupData(areaTypeId);

            // Page 1
            AssignPopulation();
            content.AreaType = AreaTypeLabel.GetLabelFromAreaCode(areaCode);
            AssignKeyMessages(area);

            // Page 2 top
            AssignDeprivationQuintilesPopulation();
            AssignLsoaQuintiles(area);

            // Population 
            AssignEthnicMinorities();
            AssignPopulationChange();

            // Page 2 bottom
            AssignLifeExpectancy();
            AssignLifeExpectancyByDeciles();

            // Page 3 top
            AssignEarlyDeathFromAllCauses();
            AssignEarlyDeathCvd();
            AssignEarlyDeathCancer();

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

        private void InitSupportingGroupData(int areaTypeId)
        {
            // Supporting data
            const int profileId = ProfileIds.HealthProfilesSupportingIndicators;
            int supportingGroupId = new GroupIdProvider(profileReader).GetGroupIds(profileId)[0];
            GroupData groupData = new GroupDataAtDataPointRepository
            {
                AssignChildAreaData = false,
                AssignAreas = false
            }.GetGroupData(benchmarkAreaCode, areaTypeId, profileId, supportingGroupId);
            groupRootSelector.SupportingGroupRoots = groupData.GroupRoots;
            indicatorMetadataCollection.AddIndicatorMetadata(groupData.IndicatorMetadata);
        }

        private void InitMainGroupData(int areaTypeId)
        {
            const int profileId = ProfileIds.HealthProfiles;

            GroupData groupDataForSpineChart = new GroupDataAtDataPointRepository
            {
                AssignChildAreaData = false,
                AssignAreas = false
            }.GetGroupData(benchmarkAreaCode, areaTypeId, profileId, GroupIds.HealthProfiles_AllSpineChartIndicators);

            indicatorMetadataCollection.AddIndicatorMetadata(groupDataForSpineChart.IndicatorMetadata);

            groupRootSelector.MainGroupRoots = groupDataForSpineChart.GroupRoots;
        }

        public void AssignPopulationChange()
        {
            // Find populations
            var populationCombined = GetFormattedCoreData(
                groupRootSelector.PopulationCombined.FirstGrouping).Value;

            var populationMale = GetFormattedCoreData(
                groupRootSelector.PopulationMale.FirstGrouping).Value;

            var populationFemale = GetFormattedCoreData(
                groupRootSelector.PopulationFemale.FirstGrouping).Value;

            // Assign population counts
            data.PopulationCountCombined =FormatPopulation(populationCombined);
            data.PopulationCountMale = FormatPopulation(populationMale);
            data.PopulationCountFemale = FormatPopulation(populationFemale);

            // Assign population percentages
            data.PopulationPercentageMale = NumericFormatter.Format1DP(populationMale / populationCombined * 100);
            data.PopulationPercentageFemale = NumericFormatter.Format1DP(populationFemale / populationCombined * 100);

            // Assign population changes
            var populationProjectCombinedGrouping = groupRootSelector.PopulationProjectionCombined.FirstGrouping;

            IndicatorMetadata metadata = indicatorMetadataCollection
                .GetIndicatorMetadataById(populationProjectCombinedGrouping.IndicatorId);

            data.ProjectedPopulationCombined = GetProjectPopulationFormatted(
                populationProjectCombinedGrouping, metadata);

            data.ProjectedPopulationMale = GetProjectPopulationFormatted(
                groupRootSelector.PopulationProjectionMale.FirstGrouping, metadata);

            data.ProjectedPopulationFemale = GetProjectPopulationFormatted(
                groupRootSelector.PopulationProjectionFemale.FirstGrouping, metadata);
        }

        private string GetProjectPopulationFormatted(Grouping populationProjectCombinedGrouping, IndicatorMetadata metadata)
        {
            CoreDataSet coreDataSet = coreDataSetProvider.GetData(populationProjectCombinedGrouping,
                TimePeriod.GetDataPoint(populationProjectCombinedGrouping), metadata);

           return FormatPopulation(coreDataSet.Value);
        }

        private string FormatPopulation(double population)
        {
            return Math.Round(population/1000, MidpointRounding.AwayFromZero)
                .ToString(CultureInfo.CurrentCulture);
        }


        public void AssignEthnicMinorities()
        {
            data.PercentageEthnicMinoritesCombined = GetFormattedValue(
                groupRootSelector.PercentageEthnicMinoritiesCombined.FirstGrouping);

            data.PercentageEthnicMinoritesMale = GetFormattedValue(
                groupRootSelector.PercentageEthnicMinoritiesMale.FirstGrouping);

            data.PercentageEthnicMinoritesFemale = GetFormattedValue(
                groupRootSelector.PercentageEthnicMinoritiesFemale.FirstGrouping);
        }

        public void AssignLifeExpectancy()
        {
            Grouping maleGrouping = groupRootSelector.SlopeIndexOfInequalityForLifeExpectancyMale.FirstGrouping;
            Grouping femaleGrouping = groupRootSelector.SlopeIndexOfInequalityForLifeExpectancyFemale.FirstGrouping;

            data.LifeExpectancyGapFemale = GetFormattedValue(femaleGrouping);
            data.LifeExpectancyGapMale = GetFormattedValue(maleGrouping);
        }

        private string GetFormattedValue(Grouping grouping)
        {
            var coreDataSet = GetFormattedCoreData(grouping);
            if (coreDataSet == null) return null;
            return coreDataSet.ValueFormatted;
        }

        private CoreDataSet GetFormattedCoreData(Grouping grouping)
        {
            IndicatorMetadata metadata =
                indicatorMetadataCollection.GetIndicatorMetadataById(grouping.IndicatorId);

            CoreDataSet coreDataSet = coreDataSetProvider.GetData(grouping,
                TimePeriod.GetDataPoint(grouping), metadata);

            var formatter = new NumericFormatterFactory(null).NewWithLimits(metadata, (Limits)null);
            if (coreDataSet != null && coreDataSet.IsValueValid)
            {
                formatter.Format(coreDataSet);
                return coreDataSet;
            }
            return null;
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
            const int categoryTypeId = CategoryTypeIds.LsoaDeprivationDecilesWithinArea2015;
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

        private void AssignLsoaQuintiles(IArea area)
        {
            data.LsoaQuintilesWithinLocalArea = groupDataReader.GetCategoriesWithinParentArea(
                CategoryTypeIds.LsoaDeprivationQuintilesWithinArea2015, areaCode, AreaTypeIds.Lsoa,
                area.AreaTypeId);

            data.LsoaQuintilesWithinEngland = groupDataReader.GetCategoriesWithinParentArea(
                CategoryTypeIds.LsoaDeprivationQuintilesInEngland2015, areaCode, AreaTypeIds.Lsoa,
                AreaTypeIds.Country);
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
            TimePeriod timePeriod, string areaCode)
        {
            var dataList = groupDataReader
                .GetCoreDataListForAllCategoryAreasOfCategoryAreaType(
                    grouping, timePeriod, CategoryTypeIds.LsoaDeprivationQuintilesInEngland2015, areaCode);

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

            // IMD2010
            var categoryTypeId = CategoryTypeIds.LsoaDeprivationQuintilesWithinArea2010;
            var year = 2009; // include data up to this year
            chartData.LocalLeastDeprived = GetDeprivationQuintileValues(grouping, categoryTypeId,
                CategoryIds.LeastDeprivedQuintile, year);

            chartData.LocalMostDeprived = GetDeprivationQuintileValues(grouping, categoryTypeId,
                CategoryIds.MostDeprivedQuintile, year);

            // IMD2015
            categoryTypeId = CategoryTypeIds.LsoaDeprivationQuintilesWithinArea2015;
            year = 9999; // include all remaining data
            chartData.LocalLeastDeprived.AddRange(GetDeprivationQuintileValues(grouping, categoryTypeId,
                CategoryIds.LeastDeprivedQuintile, year));

            chartData.LocalMostDeprived.AddRange(GetDeprivationQuintileValues(grouping, categoryTypeId,
                CategoryIds.MostDeprivedQuintile, year));

            return chartData;
        }

        private List<double> GetDeprivationQuintileValues(Grouping grouping, int categoryTypeId, int categoryId, int lastYearToInclude)
        {
            var coreDataList = trendDataReader.GetTrendDataForSpecificCategory(grouping,
                areaCode, categoryTypeId, categoryId);

            // Filter for year
            coreDataList = coreDataList.Where(x => x.Year <= lastYearToInclude).ToList();

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