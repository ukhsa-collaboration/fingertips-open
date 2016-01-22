using System;
using System.Collections.Generic;

using System.Web;

namespace FingertipsBridgeWS.Services
{
    public class GetGroupDataAtTimePoint : IHttpHandler
    {
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            new ServiceBridge { Context = context }.Respond(true, "gt");
        }
    }
}