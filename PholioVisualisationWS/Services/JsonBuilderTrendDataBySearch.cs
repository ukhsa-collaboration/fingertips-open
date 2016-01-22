
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderTrendDataBySearch : JsonBuilderBase
    {
        private TrendDataBySearchParameters parameters;

        public JsonBuilderTrendDataBySearch(HttpContextBase context)
            : base(context)
        {
            parameters = new TrendDataBySearchParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            int profileId = parameters.ProfileId;

            var parentArea = new ParentArea(parameters.ParentAreaCode, parameters.AreaTypeId);
            ComparatorMap comparatorMap = new ComparatorMapBuilder(parentArea).ComparatorMap;

            // Do not repository as do not want results cached like this (need to be 
            // cached by ID and areatype, i.e. repository by roots)
            GroupData data = new GroupDataBuilderByIndicatorIds
            {
                IndicatorIds = parameters.IndicatorIds,
                ComparatorMap = comparatorMap,
                AreaTypeId = parameters.AreaTypeId,
                RestrictSearchProfileIds = parameters.RestrictResultsToProfileIdList,
                ProfileId = profileId
            }.Build();

            if (data.IsDataOk)
            {
                data.GroupRoots = new GroupRootFilter(data.GroupRoots).RemoveRootsWithoutChildAreaData();
            }

            bool isParentAreaCodeNearestNeighbour = Area.IsNearestNeighbour(parameters.ParentAreaCode);

            IList<TrendRoot> trendRoots = new TrendRootBuilder().Build(data.GroupRoots, comparatorMap,
                parameters.AreaTypeId, profileId, data.IndicatorMetadata, isParentAreaCodeNearestNeighbour);

            return JsonConvert.SerializeObject(trendRoots);
        }
    }
}