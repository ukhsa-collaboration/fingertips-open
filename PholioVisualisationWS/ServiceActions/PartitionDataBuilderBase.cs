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
        private IArea _nationalArea;

        protected void InitMetadata(Grouping grouping)
        {
            _indicatorMetadata = IndicatorMetadataProvider.Instance.GetIndicatorMetadata(grouping);
        }

        protected void FormatData(IList<CoreDataSet> dataList)
        {
            NumericFormatter formatter = NumericFormatterFactory.New(_indicatorMetadata, _groupDataReader);
            new CoreDataProcessor(formatter).FormatAndTruncateList(dataList);
        }

        protected virtual void CalculateSignificances(string areaCode, TimePeriod timePeriod, IList<CoreDataSet> categoryDataList)
        {
            var area = AreaFactory.NewArea(_areasReader, areaCode);
            var targetComparerProvider = new TargetComparerProvider(_groupDataReader, _areasReader);

            var indicatorComparisonHelper = new IndicatorComparisonHelper(_indicatorMetadata,
                _grouping, _groupDataReader, _pholioReader, targetComparerProvider);

            // Set benchmark data
            var benchmarkDataProvider = new BenchmarkDataProvider(_groupDataReader);
            AverageCalculator averageCalculator = null; // Assume parent value is in database
            CoreDataSet benchmarkData = benchmarkDataProvider.GetBenchmarkData(_grouping, timePeriod,
                averageCalculator, area);

            foreach (CoreDataSet coreDataSet in categoryDataList)
            {
                coreDataSet.SignificanceAgainstOneBenchmark =
                    (int) indicatorComparisonHelper.GetSignificance(coreDataSet, benchmarkData);
            }
        }

        protected IArea GetNationalArea(IArea area)
        {
            if (_nationalArea == null)
            {
                _nationalArea = area.IsCountry
                    ? area
                    : AreaFactory.NewArea(_areasReader, AreaCodes.England);
            }
            return _nationalArea;
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