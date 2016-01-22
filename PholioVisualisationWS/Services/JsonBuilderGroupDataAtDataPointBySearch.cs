
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderGroupDataAtDataPointBySearch : JsonBuilderBase
    {
        private GroupDataAtDataPointBySearchParameters parameters;

        public JsonBuilderGroupDataAtDataPointBySearch(HttpContextBase context)
            : base(context)
        {
            parameters = new GroupDataAtDataPointBySearchParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            int profileId = parameters.ProfileId;

            var parentArea = new ParentArea(parameters.ParentAreaCode, parameters.AreaTypeId);

            // Do not repository as do not want results cached like this (need to be 
            // cached by ID and areatype, i.e. repository by roots)
            GroupData data = new GroupDataBuilderByIndicatorIds
            {
                IndicatorIds = parameters.IndicatorIds,
                ProfileId = profileId,
                RestrictSearchProfileIds = parameters.RestrictResultsToProfileIdList,
                ComparatorMap = new ComparatorMapBuilder(parentArea).ComparatorMap,
                AreaTypeId = parameters.AreaTypeId
            }.Build();

            new GroupDataProcessor().Process(data);

            if (data.IsDataOk)
            {
                data.GroupRoots = new GroupRootFilter(data.GroupRoots).RemoveRootsWithoutChildAreaData();
            }

            return new JsonBuilderGroupData
            {
                GroupRoots = data.GroupRoots,
                IndicatorMetadata = data.IndicatorMetadata
            }.BuildJson();
        }

    }
}