using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public abstract class GroupRootTrendBuilderBase
    {
        protected Grouping Grouping;
        protected IndicatorMetadata IndicatorMetadata;

        private IndicatorComparer comparer;
        private TargetComparer targetComparer;
        private Dictionary<int, IList<CoreDataSet>> comparatorIdToComparatorTrendData = new Dictionary<int, IList<CoreDataSet>>();
        private IList<CoreDataSet> dataList;
        private IEnumerable<TimePeriod> periods;
        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        private PholioReader pholioReader = ReaderFactory.GetPholioReader();
        private CoreDataProcessor dataProcessor;
        
        private readonly IAreasReader areasReader = ReaderFactory.GetAreasReader();

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
            TrendDataReader trendReader, IList<string> childAreaCodes)
        {
            TrendRoot trendRoot = new TrendRoot(root);

            CreateComparers();

            periods = Grouping.GetTimePeriodIterator(IndicatorMetadata.YearType).TimePeriods;

            var formatter = NumericFormatterFactory.New(IndicatorMetadata, groupDataReader);
            dataProcessor = new CoreDataProcessor(formatter);

            // Get comparator trend data
            foreach (var comparator in comparatorMap.Comparators)
            {
                CategoryArea categoryArea = comparator.Area as CategoryArea;
                if (categoryArea != null)
                {
                    var categoryTypeId = categoryArea.CategoryTypeId;

                    // Currently deprivation deciles are the only category areas expected to provide trend data
                    if (categoryTypeId != CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority &&
                        categoryTypeId != CategoryTypeIds.DeprivationDecileDistrictAndUnitaryAuthority &&
                        categoryTypeId != CategoryTypeIds.DeprivationDecileGp2010 &&
                        categoryTypeId != CategoryTypeIds.DeprivationDecileGp2015)
                    {
                        throw new FingertipsException("Unexpected Category Type ID for trend data");
                    }

                    comparatorIdToComparatorTrendData.Add(comparator.ComparatorId, trendReader.GetTrendDataForSpecificCategory(Grouping, AreaCodes.England, categoryTypeId, categoryArea.CategoryId));
                }
                else
                {
                    comparatorIdToComparatorTrendData.Add(comparator.ComparatorId, trendReader.GetTrendData(Grouping, comparator.Area.Code));
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
                trendRoot.DataPoints[areaCode] = periods.Select(period => GetPoint(root, period)).ToList();

             
            }

            trendRoot.TrendMarkers = root.TrendMarkers;

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
                new SpecifiedTimePeriodFormatter {TimePeriod = p}.Format(IndicatorMetadata)).ToList();
        }

        private void CreateComparers()
        {
            comparer = new IndicatorComparerFactory { PholioReader = pholioReader }.New(Grouping);
            targetComparer = TargetComparerFactory.New(IndicatorMetadata.TargetConfig);
            // Note: TargetComparerHelper.AssignExtraDataIfRequired called in GroupDataProcessor
        }

        private void AssignComparatorDataToTrendRoot(TrendRoot trendRoot, Grouping grouping, IList<string> childAreaCodes)
        {
            foreach (TimePeriod period in periods)
            {
                Dictionary<int, string> comparatorToValueFs = new Dictionary<int, string>();
                Dictionary<int, double> comparatorToValue = new Dictionary<int, double>();

                trendRoot.ComparatorValueFs.Add(comparatorToValueFs);
                trendRoot.ComparatorValue.Add(comparatorToValue);

                foreach (KeyValuePair<int, IList<CoreDataSet>> keyValuePair in comparatorIdToComparatorTrendData)
                {
                    var comparatorId = keyValuePair.Key;
                    var comparatorDataList = keyValuePair.Value;

                    var data = GetFormattedValueData(period, comparatorDataList, grouping, childAreaCodes);
                    comparatorToValueFs.Add(comparatorId, data.ValueFormatted);
                    comparatorToValue.Add(comparatorId, data.Value);
                }
            }
        }

        protected TrendDataPoint GetPoint(GroupRoot root, TimePeriod period)
        {
            CoreDataSet data = GetFormattedPointValueData(period, dataList);

            TrendDataPoint point = new TrendDataPoint(data);

            if (comparer is ICategoryComparer == false)
            {
                // Compare against benchmarks
                foreach (KeyValuePair<int, IList<CoreDataSet>> keyValuePair in comparatorIdToComparatorTrendData)
                {
                    var comparatorId = keyValuePair.Key;
                    CoreDataSet comparatorCoreData =
                        GetDataAtSpecificTimePeriod(keyValuePair.Value, period);
                    point.Significance.Add(comparatorId,
                        comparer.Compare(data, comparatorCoreData, IndicatorMetadata));
                }
            }

            //Compare quintiles
            if (comparer is ICategoryComparer)
            {
                ICategoryComparer categoryComparer = comparer as ICategoryComparer;

                Area nationalArea = areasReader.GetAreaFromCode(AreaCodes.England);
                Area regionalArea = areasReader.GetAreaFromCode(data.AreaCode);

                var nationalComparatorValues = new CoreDataSetListProvider(groupDataReader).GetChildAreaData(root.FirstGrouping, nationalArea, period);
                var rootComparatorValues = new CoreDataSetListProvider(groupDataReader).GetChildAreaData(root.FirstGrouping, regionalArea, period);
                
                // Compare against benchmarks
                foreach (KeyValuePair<int, IList<CoreDataSet>> keyValuePair in comparatorIdToComparatorTrendData)
                {
                    var comparatorId = keyValuePair.Key;

                    List<double> comparatorValues;
                    switch (comparatorId)
                    {
                        case ComparatorIds.England:
                            dataProcessor.FormatAndTruncateList(nationalComparatorValues);
                            comparatorValues = new CoreDataSetFilter(nationalComparatorValues).SelectValidValues().ToList();
                            break;
                        default:
                            dataProcessor.FormatAndTruncateList(rootComparatorValues);
                            comparatorValues = new CoreDataSetFilter(rootComparatorValues).SelectValidValues().ToList();
                            break;
                    }

                    categoryComparer.SetDataForCategories(comparatorValues);

                    point.Significance.Add(comparatorId, (Significance)categoryComparer.GetCategory(data));
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

                        var nationalValues = groupDataReader.GetCoreDataForAllAreasOfType(root.FirstGrouping, period);
                        var percentileCalculator = new BespokeTargetPercentileRangeCalculator(nationalValues.Where(x => x.IsValueValid).Select(x => x.Value).ToList());

                        bespokeComparer.LowerTargetPercentileBenchmarkData =
                            new CoreDataSet() { Value = percentileCalculator.GetPercentileValue(bespokeComparer.GetLowerTargetPercentile()) };

                        bespokeComparer.UpperTargetPercentileBenchmarkData =
                            new CoreDataSet() { Value = percentileCalculator.GetPercentileValue(bespokeComparer.GetUpperTargetPercentile()) };

                    }
                }

                point.Significance.Add(ComparatorIds.Target, targetComparer.CompareAgainstTarget(data));
            }

            return point;
        }

        private CoreDataSet GetFormattedPointValueData(TimePeriod period, IList<CoreDataSet> coreDataSetList)
        {
            CoreDataSet data = GetDataAtSpecificTimePeriod(coreDataSetList, period);
            if (data == null)
            {
                var parentArea = new Area { Code = coreDataSetList.Select(x => x.AreaCode).FirstOrDefault() };
                data = CoreDataSet.GetNullObject(parentArea.Code);
            }

            dataProcessor.FormatAndTruncate(data);

            return data;
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


            dataProcessor.FormatAndTruncate(benchmarkData);
            return benchmarkData;
        }

        protected abstract CoreDataSet GetDataAtSpecificTimePeriod(IList<CoreDataSet> data, TimePeriod period);

    }
}