
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderGroupingTree : JsonBuilderBase
    {
        private GroupingTreeParameters parameters;

        public JsonBuilderGroupingTree(HttpContextBase context)
            : base(context)
        {
            parameters = new GroupingTreeParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            return JsonConvert.SerializeObject(
                new GroupMetadataBuilder(ReaderFactory.GetGroupDataReader()) { GroupIds = parameters.GroupIds }.Build());
        }
    }
}