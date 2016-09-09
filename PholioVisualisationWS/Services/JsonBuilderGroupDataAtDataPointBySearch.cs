
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderGroupDataAtDataPointBySearch : JsonBuilderBase
    {
        private GroupDataAtDataPointBySearchParameters _parameters;

        public JsonBuilderGroupDataAtDataPointBySearch(HttpContextBase context)
            : base(context)
        {
            _parameters = new GroupDataAtDataPointBySearchParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public JsonBuilderGroupDataAtDataPointBySearch(GroupDataAtDataPointBySearchParameters parameters)
        {
            _parameters = parameters;
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            var roots = GetGroupRoots();
            return roots.Any()
                ? JsonConvert.SerializeObject(roots)
                : string.Empty;
        }

        public IList<GroupRoot> GetGroupRoots()
        {
            int profileId = _parameters.ProfileId;

            var parentArea = new ParentArea(_parameters.ParentAreaCode, _parameters.AreaTypeId);

            // Do not repository as do not want results cached like this (need to be 
            // cached by ID and areatype, i.e. repository by roots)
            GroupData data = new GroupDataBuilderByIndicatorIds
            {
                IndicatorIds = _parameters.IndicatorIds,
                ProfileId = profileId,
                RestrictSearchProfileIds = _parameters.RestrictResultsToProfileIdList,
                ComparatorMap = new ComparatorMapBuilder(parentArea).ComparatorMap,
                AreaTypeId = _parameters.AreaTypeId
            }.Build();

            new GroupDataProcessor().Process(data);

            if (data.IsDataOk)
            {
                var groupDataReader = ReaderFactory.GetGroupDataReader();
                data.GroupRoots = new GroupRootFilter(groupDataReader).RemoveRootsWithoutChildAreaData(data.GroupRoots);
            }

            return data.GroupRoots ?? new List<GroupRoot>();
        }
    }
}