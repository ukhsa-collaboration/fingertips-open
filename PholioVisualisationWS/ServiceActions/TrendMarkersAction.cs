using System.Collections.Generic;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class TrendMarkersAction
    {
        private readonly IFilteredChildAreaListProvider _areaListBuilder;
        private readonly ITrendMarkersProvider _trendMarkersProvider;
        private readonly SingleGroupingProvider _singleGroupingProvider;

        public TrendMarkersAction(IFilteredChildAreaListProvider areaListBuilder, ITrendMarkersProvider trendMarkersProvider,
            SingleGroupingProvider singleGroupingProvider)
        {
            _areaListBuilder = areaListBuilder;
            _trendMarkersProvider = trendMarkersProvider;
            _singleGroupingProvider = singleGroupingProvider;
        }

        public Dictionary<string, TrendMarkerResult> GetTrendMarkers(string parentAreaCode, int profileId, int groupId, int areaTypeId, int indicatorId, int sexId, int ageId)
        {
            var indicatorMetadata = IndicatorMetadataRepository.Instance.GetIndicatorMetadata(indicatorId);

            var grouping = _singleGroupingProvider.GetGroupingByGroupIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(
                groupId, areaTypeId, indicatorId, sexId, ageId);

            var areas = _areaListBuilder.ReadChildAreas(parentAreaCode, profileId, areaTypeId);

            return _trendMarkersProvider.GetTrendResults(areas, indicatorMetadata, grouping);
        }
    }
}