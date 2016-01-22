
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PholioVisualisation.Services.HttpHandlers
{
    public class GetLabel : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            new JsonBuilderGetLabel(new HttpContextWrapper(context)) { ServiceName = GetType().Name }.Respond();
        }
    }
}