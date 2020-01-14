using PholioVisualisation.DataConstruction;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class IndicatorsWithDataByAreaTypeBuilder
    {
        private IGroupDataReader _groupDataReader;
        private GroupingListProvider _groupingListProvider;

        public IndicatorsWithDataByAreaTypeBuilder(IGroupDataReader groupDataReader, 
            GroupingListProvider groupingListProvider)
        {
            _groupingListProvider = groupingListProvider;
            _groupDataReader = groupDataReader;
        }

        /// <summary>
        /// For each area type finds the indicators which have data available.
        /// </summary>
        /// <returns>Dictionary: Key is area type ID, Value is list of indicator IDs</returns>
        public IDictionary<int, List<int>> GetDictionaryOfAreaTypeToIndicatorIds(IList<AreaType> areaTypes,
            IList<int> indicatorIds, IList<int> profileIds)
        {

            IDictionary<int, List<int>> areaTypeIdToIndicatorIds = new Dictionary<int, List<int>>();
            foreach (var areaType in areaTypes)
            {
                List<int> indicatorIdsWithData = new List<int>();
                int areaTypeId = areaType.Id;

                var groupings = _groupingListProvider.GetGroupings(profileIds, indicatorIds, areaTypeId);

                var groupRoots = new GroupRootBuilder(_groupDataReader).BuildGroupRoots(groupings);

                foreach (var groupRoot in groupRoots)
                {
                    var grouping = groupRoot.FirstGrouping;
                    var count = _groupDataReader.GetCoreDataCountAtDataPoint(grouping);
                    if (count > 0)
                    {
                        indicatorIdsWithData.Add(grouping.IndicatorId);
                    }
                }

                areaTypeIdToIndicatorIds.Add(areaTypeId, indicatorIdsWithData.Distinct().ToList());
            }

            return areaTypeIdToIndicatorIds;
        }
    }
}