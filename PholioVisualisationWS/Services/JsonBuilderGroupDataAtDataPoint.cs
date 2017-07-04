﻿
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderGroupDataAtDataPoint : JsonBuilderBase
    {
        private GroupData data;
        private GroupDataAtDataPointParameters _parameters;

        public JsonBuilderGroupDataAtDataPoint(HttpContextBase context)
            : base(context)
        {
            _parameters = new GroupDataAtDataPointParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public JsonBuilderGroupDataAtDataPoint(GroupDataAtDataPointParameters parameters)
        {
            _parameters = parameters;
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            var roots = GetGroupRoots();
            return JsonConvert.SerializeObject(roots);
        }

        public IList<GroupRoot> GetGroupRoots()
        {
            data = new GroupDataAtDataPointRepository().GetGroupDataProcessed(_parameters.ParentAreaCode,
                _parameters.AreaTypeId, _parameters.ProfileId, _parameters.GroupId);

            return data.GroupRoots ?? new List<GroupRoot>();
        }
    }
}