
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PholioVisualisation.Services.HttpHandlers
{
    public class GetLabelSeries : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            new JsonBuilderGetLabelSeries(new HttpContextWrapper(context)) { ServiceName = GetType().Name }.Respond();
        }
    }
}