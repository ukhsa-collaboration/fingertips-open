
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderGroupingTree : JsonBuilderBase
    {
        private GroupingTreeParameters _parameters;

        public JsonBuilderGroupingTree(HttpContextBase context)
            : base(context)
        {
            _parameters = new GroupingTreeParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public JsonBuilderGroupingTree(GroupingTreeParameters parameters)
        {
            _parameters = parameters;
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            var groupingMetadatas = GetGroupingMetadatas();
            return JsonConvert.SerializeObject(groupingMetadatas);
        }

        public IList<GroupingMetadata> GetGroupingMetadatas()
        {
            return new GroupMetadataBuilder(ReaderFactory.GetGroupDataReader()) { GroupIds = _parameters.GroupIds }
                .Build();
        }
    }
}