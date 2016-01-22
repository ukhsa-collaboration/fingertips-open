/* 
 * Created by: Marcus Wade      
 * Date: 18/02/2013
 */
using System.Web;

namespace PholioVisualisation.Services.HttpHandlers
{
    public class AreaLookup : IHttpHandler
    {
        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            new JsonpBuilderAreaLookup(new HttpContextWrapper(context)) { ServiceName = GetType().Name }.Respond();
        }

    }
}