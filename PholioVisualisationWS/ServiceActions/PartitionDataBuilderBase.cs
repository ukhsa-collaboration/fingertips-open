using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public abstract class PartitionDataBuilderBase
    {
        // Data readers
        protected IAreasReader _areasReader = ReaderFactory.GetAreasReader();
        protected IGroupDataReader _groupDataReader = ReaderFactory.GetGroupDataReader();
        protected IProfileReader _profileReader = ReaderFactory.GetProfileReader();
        protected PholioReader _pholioReader = ReaderFactory.GetPholioReader();

        // Instance variables
        protected Grouping _grouping;
        protected IndicatorMetadata _indicatorMetadata;
        protected IndicatorMetadataSpecialCase _specialCase;
        protected TimePeriod _timePeriod;

        protected void InitMetadata(Grouping grouping)
        {
            _indicatorMetadata = IndicatorMetadataProvider.Instance.GetIndicatorMetadata(grouping);

            _specialCase = _indicatorMetadata.HasSpecialCases
                ? new IndicatorMetadataSpecialCase(_indicatorMetadata.SpecialCases)
                : null;
        }

        protected void InitTimePeriod(int profileId, Grouping grouping)
        {
            _timePeriod = TimePeriod.GetDataPoint(grouping);

            // If it is a search or profile id is not provided then
            // get all data including the recent time period
            if (profileId == ProfileIds.Undefined || profileId == ProfileIds.Search)
            {
                var maxYear = _groupDataReader.GetCoreDataMaxYear(grouping.IndicatorId);
                _timePeriod.Year = maxYear;
            }
        }

        protected void FormatData(IList<CoreDataSet> dataList)
        {
            NumericFormatter formatter = new NumericFormatterFactory(_groupDataReader).New(_indicatorMetadata);
            new CoreDataProcessor(formatter).FormatAndTruncateList(dataList);
        }

        protected virtual void CalculateSignificances(string areaCode, TimePeriod timePeriod, 
            IList<CoreDataSet> dataList)
        {
            // Get benchmark data
            var area = AreaFactory.NewArea(_areasReader, areaCode);
            var benchmarkData = GetBenchmarkData(timePeriod, area);

            // Get special case benchmark data
            CoreDataSet specialCaseBenchmarkData = null;
            if (_indicatorMetadata.HasSpecialCases)
            {
                specialCaseBenchmarkData = GetSpecialCaseBenchmarkData(timePeriod, area);
            }

            // Assign significances to data
            var indicatorComparisonHelper = GetIndicatorComparisonHelper();
            foreach (CoreDataSet coreDataSet in dataList)
            {
                CoreDataSet benchmarkmarkDataToCompare;
                if (specialCaseBenchmarkData != null && 
                    _specialCase.ShouldUseForSpecificCategoryTypeId() &&
                    coreDataSet.CategoryTypeId == _specialCase.CategoryTypeId)
                {
                    benchmarkmarkDataToCompare = specialCaseBenchmarkData;
                }
                else
                {
                    benchmarkmarkDataToCompare = benchmarkData;
                }
                coreDataSet.SignificanceAgainstOneBenchmark =
                    indicatorComparisonHelper.GetSignificance(coreDataSet, benchmarkmarkDataToCompare);
            }
        }

        protected virtual CoreDataSet GetSpecialCaseBenchmarkData(TimePeriod timePeriod,
            IArea area)
        {
            return null;
        }

        protected IndicatorComparisonHelper GetIndicatorComparisonHelper()
        {
            var targetComparerProvider = new TargetComparerProvider(_groupDataReader, _areasReader);
            var indicatorComparisonHelper = new IndicatorComparisonHelper(_indicatorMetadata,
                _grouping, _groupDataReader, _pholioReader, targetComparerProvider);
            return indicatorComparisonHelper;
        }

        protected virtual CoreDataSet GetBenchmarkData(TimePeriod timePeriod, IArea area)
        {
            var benchmarkDataProvider = new BenchmarkDataProvider(_groupDataReader);
            AverageCalculator averageCalculator = null; // Assume parent value is in database
            CoreDataSet benchmarkData = benchmarkDataProvider.GetBenchmarkData(_grouping, timePeriod,
                averageCalculator, area);
            return benchmarkData;
        }

        protected IList<string> GetTimePeriodStrings(IList<TimePeriod> timePeriods)
        {
            var timePeriodTextListBuilder = new TimePeriodTextListBuilder(_indicatorMetadata);
            timePeriodTextListBuilder.AddRange(timePeriods);
            return timePeriodTextListBuilder.GetTimePeriodStrings();
        }

        protected static IList<TimePeriod> RemoveEarlyEmptyTimePeriods(PartitionTrendDataDictionaryBuilder dictionaryBuilder,
            IList<TimePeriod> timePeriods)
        {
            int earliestIndexRemoved = dictionaryBuilder.RemoveEarlyEmptyYears();

            return earliestIndexRemoved > -1
                ? timePeriods.Skip(earliestIndexRemoved + 1).ToList()
                : timePeriods;
        }
    }
}