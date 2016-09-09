using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.SearchQuerying;

namespace PholioVisualisation.Services
{
    public class JsonBuilderSearchIndicators : JsonBuilderBase
    {
        private SearchIndicatorsParameters _parameters;

        private IAreasReader areasReader = ReaderFactory.GetAreasReader();
        private IProfileReader profileReader = ReaderFactory.GetProfileReader();
        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();

        public JsonBuilderSearchIndicators(HttpContextBase context)
            : base(context)
        {
            _parameters = new SearchIndicatorsParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public JsonBuilderSearchIndicators(SearchIndicatorsParameters parameters)
        {
            _parameters = parameters;
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            var areaTypeIdToIndicatorIdsWithData = GetAreaTypeIdToIndicatorIdsWithData();
            return JsonConvert.SerializeObject(areaTypeIdToIndicatorIdsWithData);
        }

        public IDictionary<int, List<int>> GetAreaTypeIdToIndicatorIdsWithData()
        {
            var indicatorIds = new IndicatorSearch().SearchIndicators(_parameters.SearchText);
            var profileIds = _parameters.RestrictResultsToProfileIdList;

            var areaTypes = new AreaTypeListProvider(new GroupIdProvider(profileReader), areasReader, groupDataReader)
                .GetChildAreaTypesUsedInProfiles(profileIds);

            IDictionary<int, List<int>> areaTypeIdToIndicatorIdsWithData = new Dictionary<int, List<int>>();
            foreach (var areaType in areaTypes)
            {
                List<int> indicatorIdsWithData = new List<int>();
                int areaTypeId = areaType.Id;

                var groupings = new GroupingListProvider(groupDataReader, profileReader)
                    .GetGroupings(profileIds, indicatorIds, areaTypeId);

                var groupRoots = new GroupRootBuilder().BuildGroupRoots(groupings);

                foreach (var groupRoot in groupRoots)
                {
                    var grouping = groupRoot.FirstGrouping;
                    var count = groupDataReader.GetCoreDataCountAtDataPoint(grouping);
                    if (count > 0)
                    {
                        indicatorIdsWithData.Add(grouping.IndicatorId);
                    }
                }

                areaTypeIdToIndicatorIdsWithData.Add(areaTypeId, indicatorIdsWithData);
            }

            return areaTypeIdToIndicatorIdsWithData;
        }
    }
}
