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
        private SearchIndicatorsParameters parameters;

        private IAreasReader areasReader = ReaderFactory.GetAreasReader();
        private IProfileReader profileReader = ReaderFactory.GetProfileReader();
        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();

        public JsonBuilderSearchIndicators(HttpContextBase context)
            : base(context)
        {
            parameters = new SearchIndicatorsParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            var indicatorIds = new IndicatorSearch().SearchIndicators(parameters.SearchText);
            var profileIds = parameters.RestrictResultsToProfileIdList;

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

            return JsonConvert.SerializeObject(areaTypeIdToIndicatorIdsWithData);
        }
    }
}
