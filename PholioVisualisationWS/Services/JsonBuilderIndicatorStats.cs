
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderIndicatorStats 
    {
        private IndicatorStatsParameters _parameters;

        private IndicatorMetadataProvider _indicatorMetadataProvider = IndicatorMetadataProvider.Instance;
        private IProfileReader _profileReader = ReaderFactory.GetProfileReader();

        private IList<int> _profileIds;

        public JsonBuilderIndicatorStats(IndicatorStatsParameters parameters)
        {
            _parameters = parameters;
        }

        public Dictionary<int, IndicatorStats> GetIndicatorStats()
        {
            _profileIds = _parameters.RestrictResultsToProfileIdList;

            var roots = GetRoots();
            var responseObjects = new Dictionary<int, IndicatorStats>();

            int rootIndex = 0;
            foreach (var root in roots)
            {
                Grouping grouping = root.Grouping[0];
                IndicatorMetadata metadata = _indicatorMetadataProvider.GetIndicatorMetadata(grouping);
                TimePeriod timePeriod = new DataPointOffsetCalculator(grouping, _parameters.DataPointOffset,
                    metadata.YearType).TimePeriod;

                var parentArea = new ParentArea(_parameters.ParentAreaCode, _parameters.ChildAreaTypeId);

                var indicatorStatsBuilder = new IndicatorStatsBuilder(metadata, parentArea, _parameters.ProfileId);
                var indicatorStatsResponse = indicatorStatsBuilder.GetIndicatorStats(grouping, timePeriod);

                responseObjects[rootIndex] = indicatorStatsResponse;

                rootIndex++;
            }

            return responseObjects;
        }

        private IEnumerable<GroupRoot> GetRoots()
        {
            IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
            IList<Grouping> groupings;
            if (_parameters.UseIndicatorIds)
            {
                groupings = new GroupingListProvider(reader, _profileReader).GetGroupings(
                    _profileIds,
                    _parameters.IndicatorIds,
                    _parameters.ChildAreaTypeId);

                var roots = new GroupRootBuilder().BuildGroupRoots(groupings);
                return new GroupRootFilter(reader).RemoveRootsWithoutChildAreaData(roots);
            }

            groupings = reader.GetGroupingsByGroupIdAndAreaTypeIdOrderedBySequence(_parameters.GroupId, _parameters.ChildAreaTypeId);
            return new GroupRootBuilder().BuildGroupRoots(groupings);
        }

    }
}