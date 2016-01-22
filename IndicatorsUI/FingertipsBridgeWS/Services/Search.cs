
using System.Web;

namespace FingertipsBridgeWS.Services
{
    public class Search : IHttpHandler
    {
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            new ServiceBridge { Context = context }.Respond(true, null);
        }
    }
}

