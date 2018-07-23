using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.SearchQuerying;

namespace PholioVisualisation.ServiceActions
{
    public class IndicatorSearchAction
    {
        private IAreaTypeListProvider _areaTypeListProvider;
        private IGroupDataReader _groupDataReader;
        private IndicatorsWithDataByAreaTypeBuilder _indicatorsWithDataByAreaTypeBuilder;
        private IndicatorKnowledgeFilter _indicatorFilter;

        public IndicatorSearchAction(IAreaTypeListProvider areaTypeListProvider, 
            IGroupDataReader groupDataReader, 
            IndicatorsWithDataByAreaTypeBuilder indicatorsWithDataByAreaTypeBuilder,
            IndicatorKnowledgeFilter indicatorFilter)
        {
            _areaTypeListProvider = areaTypeListProvider;
            _indicatorsWithDataByAreaTypeBuilder = indicatorsWithDataByAreaTypeBuilder;
            _groupDataReader = groupDataReader;
            _indicatorFilter = indicatorFilter;
        }

        /// <summary>
        /// Gets list of indicators with data for each area type.
        /// </summary>
        /// <returns>Dictionary: Key is area type ID, Value is list of indicator IDs</returns>
        public IDictionary<int, List<int>> GetAreaTypeIdToIndicatorIdsWithData(string searchText, 
            IList<int> restrictToProfileIds)
        {
            var indicatorIds = GetIndicatorIds(searchText);

            var profileIds = restrictToProfileIds;

            var areaTypes = _areaTypeListProvider.GetChildAreaTypesUsedInProfiles(profileIds);

            return _indicatorsWithDataByAreaTypeBuilder.GetDictionaryOfAreaTypeToIndicatorIds(areaTypes,
                indicatorIds, profileIds);
        }

        private IList<int> GetIndicatorIds(string searchText)
        {
            var properties = _groupDataReader
                .GetIndicatorMetadataTextProperties()
                .Where(p => p.SearchBoost > 0);

            var indicatorIds = new IndicatorSearch().SearchIndicators(searchText, properties);

            // Knowledge-based filter of indicators
            indicatorIds = _indicatorFilter.FilterIndicatorIdsForSearchTermExpectations(searchText, indicatorIds);

            return indicatorIds;
        }
    }
}