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
    public class JsonBuilderGroupRoots : JsonBuilderBase
    {
        private GroupRootsParameters _parameters;

        public JsonBuilderGroupRoots(HttpContextBase context)
            : base(context)
        {
            _parameters = new GroupRootsParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public JsonBuilderGroupRoots(GroupRootsParameters parameters)
        {
            _parameters = parameters;
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            var groupRoots = GetGroupRoots();
            return JsonConvert.SerializeObject(groupRoots);
        }

        public IList<GroupRoot> GetGroupRoots()
        {
            var groupings = ReaderFactory.GetGroupDataReader().GetGroupingsByGroupIdAndAreaTypeIdOrderedBySequence(
                _parameters.GroupId, _parameters.AreaTypeId);
            return new GroupRootBuilder().BuildGroupRoots(groupings);
        }
    }
}