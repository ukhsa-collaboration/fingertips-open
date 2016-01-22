
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PholioVisualisation.Services.HttpHandlers
{
    public class GetGroupDataAtDataPointBySearch : IHttpHandler
    {
        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            new JsonBuilderGroupDataAtDataPointBySearch(new HttpContextWrapper(context)) { ServiceName = GetType().Name }.Respond();
        }
    }
}