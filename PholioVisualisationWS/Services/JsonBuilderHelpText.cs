using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderHelpText : JsonBuilderBase
    {
        private HelpTextParameters parameters;

        public JsonBuilderHelpText(HttpContextBase context)
            : base(context)
        {
            parameters = new HelpTextParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            var reader = ReaderFactory.GetContentReader();
            return JsonConvert.SerializeObject(reader.GetContent(parameters.HelpKey));
        }

    }
}
