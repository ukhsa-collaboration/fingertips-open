using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataSorting;

namespace PholioVisualisation.DataConstruction
{
    public abstract class GroupRootTrendBuilderBase
    {
        protected Grouping Grouping;
        protected IndicatorMetadata IndicatorMetadata;

        private IGroupDataReader _groupDataReader = ReaderFactory.GetGroupDataReader();
        private PholioReader _pholioReader = ReaderFactory.GetPholioReader();
        private readonly IAreasReader _areasReader = ReaderFactory.GetAreasReader();
        private ITrendDataReader _trendReader;

        private BenchmarkDataProvider _benchmarkDataProvider;
        private CoreDataProcessor _coreDataProcessor;
        private IndicatorComparer _comparer;
        private TargetComparer _targetComparer;


        // Category comparison: Data lists retained to prevent repeated trip to database
        private Dictionary<TimePeriod, ICategoryComparer> timePeriodToNationalCategoryComparer =
            new Dictionary<TimePeriod, ICategoryComparer>();

        private Dictionary<TimePeriod, ICategoryComparer> timePeriodToSubnationalCategoryComparer =
            new Dictionary<TimePeriod, ICategoryComparer>();

        private Dictionary<int, IList<CoreDataSet>> comparatorIdToComparatorTrendData = new Dictionary<int, IList<CoreDataSet>>();
        private IList<CoreDataSet> dataList;
        private IList<TimePeriod> periods;
        private bool hasData;


        public GroupRootTrendBuilderBase()
        {
            _benchmarkDataProvider = new BenchmarkDataProvider(_groupDataReader);
        }

        /// <summary>
        /// Factory method
        /// </summary>
        public static GroupRootTrendBuilderBase New(Grouping grouping, IndicatorMetadata indicatorMetadata)
        {
            GroupRootTrendBuilderBase builder = GetAppropriateBuilderType(grouping);
            builder.Grouping = grouping;
            builder.IndicatorMetadata = indicatorMetadata;
            return builder;
        }

        private static GroupRootTrendBuilderBase GetAppropriateBuilderType(Grouping grouping)
        {
            if (grouping.IsDataMonthly())
            {
                return new GroupRootTrendBuilderMonthly();
            }
            if (grouping.IsDataQuarterly())
            {
                return new GroupRootTrendBuilderQuarterly();
            }
            return new GroupRootTrendBuilderYearly();
        }

        public TrendRoot BuildTrendRoot(ComparatorMap comparatorMap, GroupRoot groupRoot,
            ITrendDataReader trendReader, IList<string> childAreaCodes)
        {
            Init(trendReader);

            TrendRoot trendRoot = new TrendRoot(groupRoot);
            periods = Grouping.GetTimePeriodIterator(IndicatorMetadata.YearType).TimePeriods.ToList();

            var formatter = new NumericFormatterFactory(_groupDataReader).New(IndicatorMetadata);
            _coreDataProcessor = new CoreDataProcessor(formatter);

            AssignComparatorData(comparatorMap, groupRoot, trendReader);

            AssignChildAreaData(trendReader, childAreaCodes, trendRoot);

            AssignComparatorDataAverages(comparatorMap, groupRoot, trendRoot, periods);

            trendRoot.RecentTrends = groupRoot.RecentTrends;

            AssignPeriods(trendRoot);

            if (hasData)
            {
                FormatAndCalculateSignificancesForChildAreaData(trendRoot);

                AssignComparatorDataToTrendRoot(trendRoot, groupRoot.FirstGrouping, comparatorMap, childAreaCodes);

                AssignLimits(comparatorMap, childAreaCodes, trendRoot);
            }

            return trendRoot;
        }

        private void AssignLimits(ComparatorMap comparatorMap, IList<string> childAreaCodes, TrendRoot trendRoot)
        {
            var limitBuilder = new LimitsBuilder()
            {
                // Comparator values will be much higher than child areas for counts
                ExcludeComparators = IndicatorMetadata.ValueTypeId == ValueTypeIds.Count
            };
            trendRoot.Limits = limitBuilder.GetLimits(childAreaCodes, Grouping, comparatorMap);
        }

        private void FormatAndCalculateSignificancesForChildAreaData(TrendRoot trendRoot)
        {
            foreach (string areaCode in trendRoot.DataPoints.Keys)
            {
                var dataList = trendRoot.DataPoints[areaCode];

                // Create trend data points
                foreach (var timePeriod in periods)
                {
                    var trendDataPoint = GetTrendDataAtSpecificTimePeriod(dataList, timePeriod);

                    if (trendDataPoint != null)
                    {
                        _targetComparer = new TargetComparerProvider(_groupDataReader, _areasReader)
                            .GetTargetComparer(IndicatorMetadata, Grouping, timePeriod);

                        var coreDataSet = trendDataPoint.CoreDataSet;

                        // Benchmark and target calculations
                        var significances = AssignSignificanceToTrendDataPoint(coreDataSet, Grouping, timePeriod);
                        trendDataPoint.Significance = significances;

                        // Need to assess count before data is truncated
                        trendDataPoint.IsCountValid = coreDataSet.IsCountValid;

                        // Format data
                        _coreDataProcessor.FormatAndTruncate(coreDataSet);
                        trendDataPoint.CopyValueProperties(coreDataSet);

                        // Assign value note ID
                        _coreDataProcessor.RemoveRedundantValueNote(coreDataSet);
                        trendDataPoint.ValueNoteId = coreDataSet.ValueNoteId;
                    }
                }
            }
        }

        private void AssignChildAreaData(ITrendDataReader trendReader, IList<string> childAreaCodes, TrendRoot trendRoot)
        {
            hasData = false;

            foreach (string areaCode in childAreaCodes)
            {
                dataList = trendReader.GetTrendData(Grouping, areaCode);

                // Do not include areas without data
                if (dataList.Count == 0)
                {
                    bool isData = comparatorIdToComparatorTrendData
                        .Any(keyValuePair => keyValuePair.Value.Count > 0);
                    if (isData == false)
                    {
                        continue;
                    }
                }

                hasData = true;

                // Create trend data points
                IList<TrendDataPoint> trendDataPoints = new List<TrendDataPoint>();
                foreach (var timePeriod in periods)
                {
                    // Find core data for current time period
                    var coreDataSet = GetDataAtSpecificTimePeriod(dataList, timePeriod)
                                      ?? CoreDataSet.GetNullObject(areaCode, timePeriod);

                    var trendDataPoint = new TrendDataPoint(coreDataSet);
                    trendDataPoints.Add(trendDataPoint);
                }

                trendRoot.DataPoints[areaCode] = trendDataPoints;
            }
        }

        private void AssignComparatorDataAverages(ComparatorMap comparatorMap, GroupRoot groupRoot,
            TrendRoot trendRoot, IList<TimePeriod> timePeriods)
        {
            const int comparatorId = ComparatorIds.Subnational;

            var comparator = comparatorMap.GetComparatorById(comparatorId);
            var comparatorArea = comparator.Area;

            // Check comparator is an area list
            if (!Area.IsAreaListAreaCode(comparatorArea.Code) &&
                !NearestNeighbourArea.IsNearestNeighbourAreaCode(comparatorArea.Code))
            {
                return;
            }

            // Get area list grouping
            var grouping = groupRoot.Grouping.FirstOrDefault(x => x.ComparatorId == comparatorId);
            if (grouping != null)
            {
                var data = trendRoot.DataPoints;

                var comparatorAreaDataList = new List<CoreDataSet>();
                for (var i = 0; i < timePeriods.Count; i++)
                {
                    var childAreaData = new List<CoreDataSet>();
                    foreach (var areaCode in data.Keys)
                    {
                        var areaData = data[areaCode][i].CoreDataSet;
                        childAreaData.Add(areaData);
                    }

                    AverageCalculator averageCalculator =
                        AverageCalculatorFactory.New(childAreaData, IndicatorMetadata);

                    var areaListData = _benchmarkDataProvider.CalculateBenchmarkDataAverage(grouping, timePeriods[i],
                        averageCalculator, comparatorArea);

                    comparatorAreaDataList.Add(areaListData);
                }

                comparatorIdToComparatorTrendData.Add(comparatorId, comparatorAreaDataList);
            }
        }

        private void AssignComparatorData(ComparatorMap comparatorMap, GroupRoot root, ITrendDataReader trendReader)
        {
            foreach (var grouping in root.Grouping)
            {
                var comparator = comparatorMap.GetComparatorById(grouping.ComparatorId);
                var comparatorArea = comparator.Area;

                CategoryArea categoryArea = comparatorArea as CategoryArea;
                if (categoryArea != null)
                {
                    var categoryTypeId = categoryArea.CategoryTypeId;
                    var categoryAreaDataList = trendReader.GetTrendDataForSpecificCategory(Grouping,
                        AreaCodes.England, categoryTypeId, categoryArea.CategoryId);
                    comparatorIdToComparatorTrendData.Add(comparator.ComparatorId, categoryAreaDataList);
                }
                else if (Area.IsAreaListAreaCode(comparatorArea.Code) || 
                         NearestNeighbourArea.IsNearestNeighbourAreaCode(comparatorArea.Code))
                {
                    // Cannot calculate averages until after the child area data is available
                }
                else
                {
                    var comparatorAreaDataList = trendReader.GetTrendData(Grouping, comparatorArea.Code);
                    comparatorIdToComparatorTrendData.Add(comparator.ComparatorId, comparatorAreaDataList);
                }
            }
        }

        private void AssignPeriods(TrendRoot trendRoot)
        {
            trendRoot.Periods = periods.Select(p =>
                new SpecifiedTimePeriodFormatter { TimePeriod = p }.Format(IndicatorMetadata)).ToList();
        }

        private void Init(ITrendDataReader trendReader)
        {
            _comparer = NewIndicatorComparer();
            _trendReader = trendReader;
        }

        private IndicatorComparer NewIndicatorComparer()
        {
            return new IndicatorComparerFactory { PholioReader = _pholioReader }.New(Grouping);
        }

        private void AssignComparatorDataToTrendRoot(TrendRoot trendRoot, Grouping grouping,
            ComparatorMap comparatorMap, IList<string> childAreaCodes)
        {
            foreach (TimePeriod period in periods)
            {
                Dictionary<int, string> comparatorToValueFs = new Dictionary<int, string>();
                Dictionary<int, double> comparatorToValue = new Dictionary<int, double>();
                Dictionary<int, CoreDataSet> comparatorToCoreData = new Dictionary<int, CoreDataSet>();

                trendRoot.ComparatorValueFs.Add(comparatorToValueFs);
                trendRoot.ComparatorValue.Add(comparatorToValue);
                trendRoot.ComparatorData.Add(comparatorToCoreData);

                foreach (KeyValuePair<int, IList<CoreDataSet>> keyValuePair in comparatorIdToComparatorTrendData)
                {
                    var comparatorId = keyValuePair.Key;
                    var comparatorDataList = keyValuePair.Value;
                    var parentArea = comparatorMap.GetComparatorById(comparatorId).Area;

                    var data = GetFormattedValueData(period, comparatorDataList, grouping, parentArea, childAreaCodes);
                    _coreDataProcessor.RemoveRedundantValueNote(data);
                    comparatorToCoreData.Add(comparatorId, data);

                    // These two redundant now have data object
                    comparatorToValueFs.Add(comparatorId, data.ValueFormatted);
                    comparatorToValue.Add(comparatorId, data.Value);
                }
            }
        }

        protected Dictionary<int, Significance> AssignSignificanceToTrendDataPoint(
            CoreDataSet data, Grouping grouping, TimePeriod period)
        {
            Dictionary<int, Significance> sig = new Dictionary<int, Significance>();

            // Benchmark comparisons
            if (_comparer is ICategoryComparer == false)
            {
                // Compare against benchmarks
                foreach (KeyValuePair<int, IList<CoreDataSet>> keyValuePair in comparatorIdToComparatorTrendData)
                {
                    var comparatorId = keyValuePair.Key;
                    CoreDataSet comparatorCoreData = GetDataAtSpecificTimePeriod(keyValuePair.Value, period);
                    var significance = _comparer.Compare(data, comparatorCoreData, IndicatorMetadata);
                    sig.Add(comparatorId, significance);
                }
            }

            // Category comparison
            if (_comparer is ICategoryComparer)
            {
                var provider = new CoreDataSetListProvider(_groupDataReader);

                // Compare against benchmarks
                foreach (KeyValuePair<int, IList<CoreDataSet>> keyValuePair in comparatorIdToComparatorTrendData)
                {
                    var comparatorId = keyValuePair.Key;
                    ICategoryComparer categoryComparer;

                    switch (comparatorId)
                    {
                        case ComparatorIds.England:

                            // Get national comparer
                            categoryComparer = GetCategoryComparerWithValues(timePeriodToNationalCategoryComparer,
                                    provider, grouping, period, AreaCodes.England);
                            break;
                        default:

                            // Get subnational comparer
                            categoryComparer = GetCategoryComparerWithValues(timePeriodToSubnationalCategoryComparer,
                                    provider, grouping, period, data.AreaCode); ;
                            break;
                    }

                    sig.Add(comparatorId, (Significance)categoryComparer.GetCategory(data));
                }
            }

            // Compare against target
            if (_targetComparer != null)
            {
                sig.Add(ComparatorIds.Target, _targetComparer.CompareAgainstTarget(data));
            }

            return sig;
        }

        private CoreDataSet GetFormattedValueData(TimePeriod period, IList<CoreDataSet> coreDataSetList,
            Grouping grouping, IArea parentArea, IEnumerable<string> childAreaCodes)
        {
            CoreDataSet benchmarkData = GetDataAtSpecificTimePeriod(coreDataSetList, period);

            if (benchmarkData == null && grouping != null)
            {
                // Get child area data only when necessary
                var childAreaData = new CoreDataSetListProvider(_groupDataReader).GetChildAreaData(grouping, parentArea, period);
                if (parentArea.IsCountry == false)
                {
                    childAreaData = new CoreDataSetFilter(childAreaData)
                        .SelectWithAreaCodes(childAreaCodes).ToList();
                }

                benchmarkData = AverageCalculatorFactory.New(childAreaData, IndicatorMetadata).Average;
                if (benchmarkData != null)
                {
                    benchmarkData.AreaCode = parentArea.Code;
                }
            }

            if (benchmarkData == null)
            {
                benchmarkData = CoreDataSet.GetNullObject(parentArea.Code);
            }

            _coreDataProcessor.FormatAndTruncate(benchmarkData);
            return benchmarkData;
        }

        private ICategoryComparer GetCategoryComparerWithValues(Dictionary<TimePeriod, ICategoryComparer> periodToComparer,
            CoreDataSetListProvider provider, Grouping grouping, TimePeriod period, string areaCode)
        {

            if (periodToComparer.ContainsKey(period) == false)
            {
                IArea area = _areasReader.GetAreaFromCode(areaCode);
                var childDataList = provider.GetChildAreaData(grouping, area, period);
                var comparatorValues = new CoreDataSetFilter(childDataList).SelectValidValues().ToList();

                // Get comparer
                var categoryComparer = (ICategoryComparer)NewIndicatorComparer();
                categoryComparer.SetDataForCategories(comparatorValues);
                periodToComparer[period] = categoryComparer;

                // Truncate last
                _coreDataProcessor.FormatAndTruncateList(childDataList);
            }

            return periodToComparer[period];
        }

        protected abstract CoreDataSet GetDataAtSpecificTimePeriod(IList<CoreDataSet> data, TimePeriod period);

        protected abstract TrendDataPoint GetTrendDataAtSpecificTimePeriod(IList<TrendDataPoint> dataList,
            TimePeriod period);

    }
}