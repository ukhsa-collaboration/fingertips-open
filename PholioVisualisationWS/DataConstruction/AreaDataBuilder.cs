using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    /// <summary>
    /// Gets all data from baseline to datapoint with significances compared to comparator areas.
    /// </summary>
    public class AreaDataBuilder
    {
        public int GroupId { get; set; }
        public int AreaTypeId { get; set; }
        public IList<IArea> Areas { get; set; }
        public IList<string> ComparatorAreaCodes { get; set; }
        public bool IncludeTimePeriods { get; set; }
        public bool LatestDataOnly { get; set; }

        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        private PholioReader pholioReader = ReaderFactory.GetPholioReader();
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();
        private IProfileReader profileReader = ReaderFactory.GetProfileReader();

        public Dictionary<string, IList<SimpleAreaData>> Build()
        {
            IndicatorMetadataProvider indicatorMetadataProvider = IndicatorMetadataProvider.Instance;
            IList<Grouping> groupings = groupDataReader.GetGroupingsByGroupIdAndAreaTypeIdOrderedBySequence(GroupId, AreaTypeId);
            GroupRootBuilder rootBuilder = new GroupRootBuilder();
            IList<GroupRoot> roots = rootBuilder.BuildGroupRoots(groupings);

            CoreDataSetProviderFactory coreDataSetProviderFactory = new CoreDataSetProviderFactory();

            Dictionary<string, IList<SimpleAreaData>> responseObjects = new Dictionary<string, IList<SimpleAreaData>>();

            foreach (IArea area in Areas)
            {
                List<SimpleAreaData> dataObjects = new List<SimpleAreaData>();
                responseObjects.Add(area.Code, dataObjects);

                var isAreaCcg = area.IsCcg;

                bool isAreaAggregate = isAreaCcg || area.IsGpDeprivationDecile || area.IsShape;

                CoreDataSetProvider coreDataSetProvider = coreDataSetProviderFactory.New(area);

                var indicatorComparerFactory =
                    new IndicatorComparerFactory { PholioReader = pholioReader };
                foreach (GroupRoot root in roots)
                {
                    Grouping grouping = root.FirstGrouping;

                    IndicatorComparer comparer = indicatorComparerFactory.New(grouping);
                    IndicatorMetadata metadata = indicatorMetadataProvider.GetIndicatorMetadata(grouping.IndicatorId);

                    var formatter = NumericFormatterFactory.New(metadata, groupDataReader);
                    var dataProcessor = new ValueWithCIsDataProcessor(formatter);

                    List<ValueData> dataList = new List<ValueData>();

                    ITimePeriodTextListBuilder timePeriodTextListBuilder =
                        TimePeriodTextListBuilderFactory.New(IncludeTimePeriods, metadata);

                    Dictionary<string, List<int?>> significanceHash = null;
                    if (isAreaAggregate == false || isAreaCcg)
                    {
                        significanceHash = GetSignificanceHash(ComparatorAreaCodes);
                    }

                    var timePeriods = GetTimePeriods(grouping, metadata.YearType);
                    foreach (TimePeriod timePeriod in timePeriods)
                    {
                        timePeriodTextListBuilder.Add(timePeriod);

                        CoreDataSet areaData = coreDataSetProvider.GetData(grouping, timePeriod, metadata);

                        if (areaData != null)
                        {
                            ValueWithCIsData data = areaData.GetValueWithCIsData();
                            dataProcessor.FormatAndTruncate(data);

                            if (isAreaAggregate && isAreaCcg == false)
                            {
                                dataList.Add(data.GetValueData());
                            }
                            else
                            {
                                dataList.Add(data);

                                foreach (var comparatorAreaCode in ComparatorAreaCodes)
                                {
                                    CoreDataSet comparatorData =
                                        groupDataReader.GetCoreData(grouping, timePeriod, comparatorAreaCode)
                                            .FirstOrDefault();
                                    try
                                    {
                                        int significance;
                                        if (comparer is ICategoryComparer)
                                        {
                                            var d = new ChildAreaValuesBuilder(indicatorComparerFactory,
                                                groupDataReader, areasReader, profileReader)
                                            {
                                                ParentAreaCode = comparatorAreaCode,
                                                AreaTypeId = AreaTypeId,
                                                ComparatorId = grouping.ComparatorId
                                            }.Build(grouping);
                                            var coreData = d.First(x => x.AreaCode.Equals(area.Code));
                                            significance = coreData.Significance.Values.First();
                                        }
                                        else
                                        {
                                            significance =
                                                (int)comparer.Compare(areaData, comparatorData, metadata);
                                        }

                                        var significanceList = significanceHash[comparatorAreaCode];
                                        significanceList.Add(significance);
                                    }
                                    catch (Exception ex)
                                    {
                                        ExceptionLog.LogException(ex, string.Empty);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Placeholders for missing data
                            dataList.Add(null);
                            if (significanceHash != null)
                            {
                                foreach (var comparatorAreaCode in ComparatorAreaCodes)
                                {
                                    significanceHash[comparatorAreaCode].Add(null);
                                }
                            }
                        }
                    }

                    SimpleAreaData dataObject;
                    if (IncludeTimePeriods)
                    {
                        // Only attach metadata when requested (hybrid of GroupRoot & CoreDataSet)
                        //TODO this is difficult to keep aligned with Grouping class
                        dataObject = new FullAreaData
                        {
                            IndicatorId = grouping.IndicatorId,
                            Significances = significanceHash,
                            Data = dataList,
                            StateSex = root.StateSex,
                            Sex = grouping.Sex,
                            Age = grouping.Age,
                            ComparatorConfidence = grouping.ComparatorConfidence,
                            ComparatorMethodId = grouping.ComparatorMethodId,
                            Periods = timePeriodTextListBuilder.GetTimePeriodStrings()
                        };
                    }
                    else
                    {
                        dataObject = new SimpleAreaData
                        {
                            IndicatorId = grouping.IndicatorId,
                            Significances = significanceHash,
                            Data = dataList
                        };
                    }
                    dataObjects.Add(dataObject);
                }
            }

            return responseObjects;
        }

        private IEnumerable<TimePeriod> GetTimePeriods(Grouping grouping, YearType yearType)
        {
            if (LatestDataOnly)
            {
                return new List<TimePeriod> {TimePeriod.GetDataPoint(grouping)};
            }

            TimePeriodIterator timePeriodIterator = grouping.GetTimePeriodIterator(yearType);
            return timePeriodIterator.TimePeriods;
        }

        public static Dictionary<string, List<int?>> GetSignificanceHash(IList<string> comparatorCodes)
        {
            return comparatorCodes.ToDictionary(
                comparatorCode => comparatorCode,
                comparatorCode => new List<int?>()
                );
        }
    }
}

