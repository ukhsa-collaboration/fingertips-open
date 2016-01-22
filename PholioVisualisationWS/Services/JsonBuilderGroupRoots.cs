using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderGroupRoots : JsonBuilderBase
    {
        private GroupRootsParameters parameters;

        public JsonBuilderGroupRoots(HttpContextBase context)
            : base(context)
        {
            parameters = new GroupRootsParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            var groupings = ReaderFactory.GetGroupDataReader().GetGroupingsByGroupIdAndAreaTypeIdOrderedBySequence(
                parameters.GroupId, parameters.AreaTypeId);
            var groupRoots = new GroupRootBuilder().BuildGroupRoots(groupings);
            return JsonConvert.SerializeObject(groupRoots);
        }

    }
}