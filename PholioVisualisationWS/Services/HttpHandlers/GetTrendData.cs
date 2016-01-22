
using System.Web;

namespace PholioVisualisation.Services.HttpHandlers
{
    public class GetTrendData : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            new JsonBuilderTrendData(new HttpContextWrapper(context)) { ServiceName = GetType().Name }.Respond();
        }
    }
}