using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.DataConstruction
{
    public abstract class GroupRootTrendBuilderBase
    {
        protected Grouping Grouping;
        protected IndicatorMetadata IndicatorMetadata;

        private IndicatorComparer comparer;
        private TargetComparer targetComparer;

        // Category comparison: Data lists retained to prevent repeated trip to database
        private Dictionary<TimePeriod, ICategoryComparer> timePeriodToNationalCategoryComparer =
            new Dictionary<TimePeriod, ICategoryComparer>();
        private Dictionary<TimePeriod, ICategoryComparer> timePeriodToSubnationalCategoryComparer =
            new Dictionary<TimePeriod, ICategoryComparer>();

        private Dictionary<int, IList<CoreDataSet>> comparatorIdToComparatorTrendData = new Dictionary<int, IList<CoreDataSet>>();
        private IList<CoreDataSet> dataList;
        private IEnumerable<TimePeriod> periods;
        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        private PholioReader pholioReader = ReaderFactory.GetPholioReader();
        private CoreDataProcessor _coreDataProcessor;

        private readonly IAreasReader areasReader = ReaderFactory.GetAreasReader();

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

        protected CoreDataSet EnsureNotNull(CoreDataSet data)
        {
            return data ?? CoreDataSet.GetNullObject(null);
        }

        public TrendRoot BuildTrendRoot(ComparatorMap comparatorMap, GroupRoot root,
            ITrendDataReader trendReader, IList<string> childAreaCodes)
        {

            Init();

            TrendRoot trendRoot = new TrendRoot(root);
            periods = Grouping.GetTimePeriodIterator(IndicatorMetadata.YearType).TimePeriods;

            var formatter = NumericFormatterFactory.New(IndicatorMetadata, groupDataReader);
            _coreDataProcessor = new CoreDataProcessor(formatter);

            // Get comparator trend data
            foreach (var grouping in root.Grouping)
            {
                var comparator = comparatorMap.GetComparatorById(grouping.ComparatorId);

                CategoryArea categoryArea = comparator.Area as CategoryArea;
                if (categoryArea != null)
                {
                    var categoryTypeId = categoryArea.CategoryTypeId;
                    //TODO: we we don't have data for trends calculate on the fly
                    var categoryAreaDataList = trendReader.GetTrendDataForSpecificCategory(Grouping,
                        AreaCodes.England, categoryTypeId, categoryArea.CategoryId);
                    comparatorIdToComparatorTrendData.Add(comparator.ComparatorId, categoryAreaDataList);
                }
                else
                {
                    var comparatorAreaDataList = trendReader.GetTrendData(Grouping, comparator.Area.Code);
                    comparatorIdToComparatorTrendData.Add(comparator.ComparatorId, comparatorAreaDataList);
                }
            }

            bool hasData = false;

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

                    var coreDataSet = GetDataAtSpecificTimePeriod(dataList, timePeriod)
                                    ?? CoreDataSet.GetNullObject(areaCode);

                    var significances = AssignSignificanceToTrendDataPoint(coreDataSet, Grouping, timePeriod);

                    // Need to assess count before data is truncated
                    var isCountValid = coreDataSet.IsCountValid;

                    _coreDataProcessor.FormatAndTruncate(coreDataSet);
                    _coreDataProcessor.RemoveRedundantValueNote(coreDataSet);

                    var trendDataPoint = new TrendDataPoint(coreDataSet)
                    {
                        Significance = significances,
                        IsCountValid = isCountValid
                    };
                    trendDataPoints.Add(trendDataPoint);
                }
                trendRoot.DataPoints[areaCode] = trendDataPoints;
            }

            trendRoot.RecentTrends = root.RecentTrends;

            AssignPeriods(trendRoot);

            if (hasData)
            {
                AssignComparatorDataToTrendRoot(trendRoot, root.FirstGrouping, childAreaCodes);

                // Assign limits
                var limitBuilder = new LimitsBuilder()
                {
                    ExcludeComparators = IndicatorMetadata.ValueTypeId == ValueTypeIds.Count
                };
                trendRoot.Limits = limitBuilder.GetLimits(childAreaCodes, Grouping, comparatorMap);
            }

            return trendRoot;
        }

        private void AssignPeriods(TrendRoot trendRoot)
        {
            trendRoot.Periods = periods.Select(p =>
                new SpecifiedTimePeriodFormatter { TimePeriod = p }.Format(IndicatorMetadata)).ToList();
        }

        private void Init()
        {
            comparer = NewIndicatorComparer();
            targetComparer = TargetComparerFactory.New(IndicatorMetadata.TargetConfig);
            // Note: TargetComparerHelper.AssignExtraDataIfRequired called in GroupDataProcessor
        }

        private IndicatorComparer NewIndicatorComparer()
        {
            return new IndicatorComparerFactory { PholioReader = pholioReader }.New(Grouping);
        }

        private void AssignComparatorDataToTrendRoot(TrendRoot trendRoot, Grouping grouping, IList<string> childAreaCodes)
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

                    var data = GetFormattedValueData(period, comparatorDataList, grouping, childAreaCodes);
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
            if (comparer is ICategoryComparer == false)
            {
                // Compare against benchmarks
                foreach (KeyValuePair<int, IList<CoreDataSet>> keyValuePair in comparatorIdToComparatorTrendData)
                {
                    var comparatorId = keyValuePair.Key;
                    CoreDataSet comparatorCoreData =
                        GetDataAtSpecificTimePeriod(keyValuePair.Value, period);
                    var significance = comparer.Compare(data, comparatorCoreData, IndicatorMetadata);
                    sig.Add(comparatorId, significance);
                }
            }

            // Category comparison
            if (comparer is ICategoryComparer)
            {
                var provider = new CoreDataSetListProvider(groupDataReader);

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
            if (targetComparer != null)
            {
                if (targetComparer as BespokeTargetPreviousYearEnglandValueComparer != null)
                {
                    var bespokeComparer = targetComparer as BespokeTargetPreviousYearEnglandValueComparer;

                    // Assign benchmark data to bespoke comparator
                    var comparatorTrendData = comparatorIdToComparatorTrendData[ComparatorIds.England];
                    bespokeComparer.BenchmarkData = GetDataAtSpecificTimePeriod(comparatorTrendData, period.GetTimePeriodForYearBefore());
                }
                else
                {
                    if (targetComparer as BespokeTargetPercentileRangeComparer != null)
                    {
                        var bespokeComparer = targetComparer as BespokeTargetPercentileRangeComparer;

                        var nationalValues = groupDataReader.GetCoreDataForAllAreasOfType(grouping, period);
                        var percentileCalculator = new BespokeTargetPercentileRangeCalculator(nationalValues.Where(x => x.IsValueValid).Select(x => x.Value).ToList());

                        bespokeComparer.LowerTargetPercentileBenchmarkData =
                            new CoreDataSet() { Value = percentileCalculator.GetPercentileValue(bespokeComparer.GetLowerTargetPercentile()) };

                        bespokeComparer.UpperTargetPercentileBenchmarkData =
                            new CoreDataSet() { Value = percentileCalculator.GetPercentileValue(bespokeComparer.GetUpperTargetPercentile()) };
                    }
                }

                sig.Add(ComparatorIds.Target, targetComparer.CompareAgainstTarget(data));
            }

            return sig;
        }

        private CoreDataSet GetFormattedValueData(TimePeriod period, IList<CoreDataSet> coreDataSetList, Grouping grouping,
            IEnumerable<string> childAreaCodes)
        {
            CoreDataSet benchmarkData = GetDataAtSpecificTimePeriod(coreDataSetList, period);

            var parentArea = new Area { Code = coreDataSetList.Select(x => x.AreaCode).FirstOrDefault() };
            if (benchmarkData == null && grouping != null)
            {
                //Get child area data only when necessary
                var childAreaData = new CoreDataSetListProvider(groupDataReader).GetChildAreaData(grouping, parentArea, period);
                var filteredChildAreaData = new CoreDataSetFilter(childAreaData).SelectWithAreaCode(childAreaCodes);
                benchmarkData = AverageCalculatorFactory.New(filteredChildAreaData, IndicatorMetadata).Average;
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
                Area area = areasReader.GetAreaFromCode(areaCode);
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

    }
}