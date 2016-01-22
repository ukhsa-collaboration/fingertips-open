using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public abstract class GroupDataBuilderBase
    {
        protected IAreasReader AreasReader = ReaderFactory.GetAreasReader();
        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();

        public bool AssignAreas = true;
        public bool AssignChildAreaData = true;
        public bool AssignData = true;

        public ComparatorMap ComparatorMap;

        protected GroupData GroupData;
        protected IList<Grouping> Groupings;
        public int ProfileId;

        /// <summary>
        ///     The specific time point at which to assign core data and comparator data.
        /// </summary>
        public TimePeriod TimePeriodOfData;

        public GroupData Build()
        {
            GroupData = new GroupData();

            try
            {
                ReadGroupings(groupDataReader);
                if (Groupings.Count > 0)
                {
                    if (AssignAreas)
                    {
                        GroupData.Areas = GetChildAreas();
                    }

                    GroupData.InitIndicatorMetadata(
                        IndicatorMetadataRepository.Instance.GetIndicatorMetadataCollection(Groupings));
                    BuildGroupRoots();

                    if (AssignData)
                    {
                        AssignCoreData(groupDataReader);
                    }

                    var profileReader = ReaderFactory.GetProfileReader();
                    ProfileConfig profileConfig = profileReader.GetProfileConfig(ProfileId);

                    if (profileConfig != null && profileConfig.HasTrendMarkers)
                    {
                        AssignTrendMarkers();
                    }

                }
                else
                {
                    GroupData.Clear();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.LogException(ex, "");
            }
            return GroupData;
        }

        protected abstract void ReadGroupings(IGroupDataReader groupDataReader);
        protected abstract IList<IArea> GetChildAreas();

        private void BuildGroupRoots()
        {
            var rootBuilder = new GroupRootBuilder();
            GroupData.GroupRoots = rootBuilder.BuildGroupRoots(Groupings);
        }

        private void AssignCoreData(IGroupDataReader groupDataReader)
        {
            string[] areaCodes = AssignChildAreaData
                ? GroupData.Areas.Select(x => x.Code).ToArray()
                : null;

            foreach (GroupRoot root in GroupData.GroupRoots)
            {
                Grouping firstGrouping = root.Grouping.FirstOrDefault();
                TimePeriod timePeriod = TimePeriodOfData ?? TimePeriod.GetDataPoint(firstGrouping);

                if (AssignChildAreaData)
                {
                    root.Data = groupDataReader.GetCoreData(firstGrouping, timePeriod, areaCodes);
                }

                AssignComparatorData(groupDataReader, root, timePeriod);
            }
        }

        private void AssignComparatorData(IGroupDataReader groupDataReader, GroupRoot root, TimePeriod timePeriod)
        {
            IndicatorMetadata indicatorMetaData = GroupData.GetIndicatorMetadataById(root.IndicatorId);
            var benchmarkDataProvider = new BenchmarkDataProvider(groupDataReader);
            var averageCalculator = AverageCalculatorFactory.New(root.Data, indicatorMetaData);

            // Comparator data
            foreach (Grouping grouping in root.Grouping)
            {
                Comparator comparator = ComparatorMap.GetComparatorById(grouping.ComparatorId, grouping.AreaTypeId);
                CoreDataSet data;
                if (comparator == null)
                {
                    data = CoreDataSet.GetNullObject(string.Empty);
                }
                else
                {
                    var comparatorArea = NearestNeighbourArea.IsNearestNeighbourAreaCode(comparator.Area.Code)
                         ? AreaFactory.NewArea(AreasReader, comparator.Area.Code.Substring(5))
                         : comparator.Area;

                    data = benchmarkDataProvider.GetBenchmarkData(grouping, timePeriod,
                            averageCalculator, comparatorArea);
                }
                grouping.ComparatorData = data;
            }
        }

        protected string GetParentAreaCode()
        {
            return (ComparatorMap.GetSubnationalComparator() ?? ComparatorMap.GetNationalComparator()).Area.Code;
        }

        protected IList<IArea> ReadChildAreas(string parentAreaCode, int childAreaTypeId)
        {
            IList<IArea> childAreas;
            if (NearestNeighbourArea.IsNearestNeighbourAreaCode(parentAreaCode))
            {
                var nearestNeighbourArea = (NearestNeighbourArea)AreaFactory.NewArea(AreasReader, parentAreaCode);
                childAreas = new NearestNeighbourAreaListBuilder(AreasReader, nearestNeighbourArea).Areas;
            }
            else
            {
                childAreas = new ChildAreaListBuilder(AreasReader, parentAreaCode, childAreaTypeId)
                    .ChildAreas;
            }

            IgnoredAreasFilter filter = IgnoredAreasFilterFactory.New(ProfileId);
            return filter.RemoveAreasIgnoredEverywhere(childAreas).ToList();
        }


        private void AssignTrendMarkers()
        {
            var trendCalculator = new TrendMarkerCalculator();
            var trendReader = ReaderFactory.GetTrendDataReader();

            foreach (var groupRoot in GroupData.GroupRoots)
            {
                groupRoot.TrendMarkers = new Dictionary<string, TrendMarker>();

                var indicatorMetaData =
                    GroupData.IndicatorMetadata.FirstOrDefault(i => i.IndicatorId == groupRoot.IndicatorId);

                if (indicatorMetaData == null) continue;

                foreach (var area in GroupData.Areas)
                {
                    var grouping = groupRoot.Grouping.FirstOrDefault();

                    var trendRequest = new TrendRequest()
                    {
                        ValueTypeId = indicatorMetaData.ValueTypeId,
                        ComparatorConfidence = grouping.ComparatorConfidence,
                        Data = trendReader.GetTrendData(grouping, area.Code),
                    };

                    var trendResponse = trendCalculator.GetResults(trendRequest);

                    groupRoot.TrendMarkers.Add(area.Code, trendResponse.Marker);
                }
            }


        }
    }
}