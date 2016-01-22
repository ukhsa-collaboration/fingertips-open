
using System.Web;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderGroupDataAtDataPoint : JsonBuilderBase
    {
        private GroupData data;
        private GroupDataAtDataPointParameters parameters;

        public JsonBuilderGroupDataAtDataPoint(HttpContextBase context)
            : base(context)
        {
            parameters = new GroupDataAtDataPointParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            data = new GroupDataAtDataPointRepository().GetGroupDataProcessed(parameters.ParentAreaCode,
                parameters.AreaTypeId, parameters.ProfileId, parameters.GroupId);

            return new JsonBuilderGroupData
            {
                GroupRoots = data.GroupRoots,
                IndicatorMetadata = data.IndicatorMetadata
            }.BuildJson();
        }

    }
}