using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class SparklineRootBuilder
    {
        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        private IProfileReader profileReader = ReaderFactory.GetProfileReader();
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();
        private ValueWithCIsData nullData = ValueWithCIsData.GetNullObject();
        private CoreDataSetProvider parentCoreDataSetProvider;
        private CoreDataSetProvider nationalCoreDataSetProvider;

        public List<SparklineRoot> BuildRoots(string parentAreaCode, int areaTypeId,
            int profileId, int groupId, IList<string> dataTypes, int sexId)
        {
            var groupIds = profileReader.GetProfile(profileId).GroupIds;

            IList<IArea> childAreas = areasReader
                .GetChildAreas(parentAreaCode, areaTypeId).Cast<IArea>().ToList();
            childAreas = IgnoredAreasFilterFactory.New(profileId).RemoveAreasIgnoredEverywhere(childAreas).ToList();

            ComparatorMap comparatorMap = new ComparatorMapBuilder(new ParentArea(parentAreaCode, areaTypeId)).ComparatorMap;
            var parentArea = AreaFactory.NewArea(areasReader,parentAreaCode);
            parentCoreDataSetProvider = new CoreDataSetProviderFactory().New(parentArea);
            var nationalArea = AreaFactory.NewArea(areasReader, AreaCodes.England);
            nationalCoreDataSetProvider = new CoreDataSetProviderFactory().New(nationalArea);

            // Get grouping for time info
            IList<int> indicatorIds = groupDataReader.GetIndicatorIdsByGroupIdAndAreaTypeId(groupId, areaTypeId);

            DoubleOverlappingCIsComparer comparer = new DoubleOverlappingCIsComparer();
            IndicatorMetadataProvider provider = IndicatorMetadataProvider.Instance;

            List<SparklineRoot> roots = new List<SparklineRoot>();

            foreach (var indicatorId in indicatorIds)
            {
                var grouping = GetGrouping(groupIds, indicatorId, areaTypeId, sexId);

                IndicatorMetadata metadata = provider.GetIndicatorMetadata(grouping);

                IList<TimePeriod> timePeriods = grouping.GetTimePeriodIterator(metadata.YearType).TimePeriods;

                SparklineRoot root = new SparklineRoot();
                roots.Add(root);

                List<ValueWithCIsData> allData = new List<ValueWithCIsData>();

                foreach (IArea area in childAreas)
                {
                    SparklineArea sparklineArea = new SparklineArea();
                    int startingAllDataCount = allData.Count;

                    foreach (TimePeriod timePeriod in timePeriods)
                    {
                        SparklineTimePoint timePoint = new SparklineTimePoint();
                        sparklineArea.TimePoints.Add(timePoint);

                        foreach (string dataType in dataTypes)
                        {
                            var data = GetData(area, dataType, grouping, timePeriod, allData);
                            timePoint.Data.Add(dataType, data);
                        }

                        // Are different? (comparatorId=10 “Within area 80/20 by deprivation”)
                        if (timePoint.Data.Count > 0)
                        {
                            ValueWithCIsData p1 = timePoint.Data[dataTypes[0]];
                            ValueWithCIsData p2 = timePoint.Data[dataTypes[1]];

                            Significance sig = comparer.Compare(p1, p2, metadata);
                            timePoint.AreDifferent = sig == Significance.Better || sig == Significance.Worse;
                        }
                    }

                    // Do not include areas with no data
                    if (allData.Count > startingAllDataCount)
                    {
                        root.AreaData.Add(area.Code, sparklineArea);
                    }
                }

                // Add statsPercentiles
                if (allData.Count > 0)
                {
                    IndicatorStatsPercentiles statsPercentiles = new IndicatorStatsPercentiles
                    {
                        Min = (from d in allData select d.LowerCI).Min<double>(),
                        Max = (from d in allData select d.UpperCI).Max<double>()
                    };

                    var builder = new TimePeriodTextListBuilder(metadata);
                    builder.AddRange(timePeriods);
                    IList<string> xLabels = builder.GetTimePeriodStrings();

                    SparklineStats sparklineStats = new SparklineStats(xLabels);
                    sparklineStats.IndicatorId = grouping.IndicatorId;
                    sparklineStats.Limits = new MinMaxRounder(statsPercentiles.Min, statsPercentiles.Max).Limits;
                    root.Stats = sparklineStats;

                    // Format
                    NumericFormatter formatter = NumericFormatterFactory.NewWithLimits(metadata, statsPercentiles);
                    new ValueWithCIsDataProcessor(formatter).FormatAndTruncateList(allData);

                    // Add comparator data
                    foreach (var comparator in comparatorMap.Comparators)
                    {
                        List<string> values = GetFormattedComparatorValues(timePeriods, metadata, grouping, formatter,
                            comparator.ComparatorId);
                        sparklineStats.ComparatorValues.Add(comparator.ComparatorId, values);
                    }
                }
            }

            return roots;
        }


        private Grouping GetGrouping(IList<int> groupIds, int indicatorId, int areaTypeId, int sexId)
        {
            Grouping grouping = null;
            foreach (var groupId in groupIds)
            {
                // NOTE: age ID may be different for each indicator 
                grouping = groupDataReader.GetGroupingsWithoutAgeId(
                    groupId,
                    indicatorId,
                    areaTypeId,
                    sexId).FirstOrDefault();

                if (grouping != null)
                {
                    break;
                }
            }
            return grouping;
        }

        private ValueWithCIsData GetData(IArea area, string dataType, Grouping grouping, 
            TimePeriod timePeriod, List<ValueWithCIsData> allData)
        {
            string areaCode = dataType != "val" ?
                dataType + area.Code :
                area.Code;

            IList<CoreDataSet> dataList = groupDataReader.GetCoreData(
                grouping, timePeriod, areaCode);

            CoreDataSet data = dataList.FirstOrDefault();
            ValueWithCIsData dataPoint;
            if (data != null && data.IsValueValid)
            {
                dataPoint = data.GetValueWithCIsData();
                allData.Add(dataPoint);
            }
            else
            {
                dataPoint = nullData;
            }
            return dataPoint;
        }

        private List<string> GetFormattedComparatorValues(IList<TimePeriod> timePeriods, IndicatorMetadata indicatorMetadata,
            Grouping grouping, NumericFormatter formatter, int comparatorId)
        {
            List<string> values = new List<string>();

            var dataProvider = comparatorId == ComparatorIds.England ?
                nationalCoreDataSetProvider :
                parentCoreDataSetProvider;

            foreach (TimePeriod timePeriod in timePeriods)
            {
                var data = dataProvider.GetData(grouping, timePeriod, indicatorMetadata);
                formatter.Format(data);
                var valF = data != null
                    ? data.ValueFormatted
                    : null;
                values.Add(valF);
            }

            return values;
        }


    }
}
