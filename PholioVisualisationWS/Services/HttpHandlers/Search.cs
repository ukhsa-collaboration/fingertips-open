using System.Web;

namespace PholioVisualisation.Services.HttpHandlers
{
    public class Search : IHttpHandler
    {
        public const string ServiceKey = "s";

        public const string ServiceSearchIndicators = "in";

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var contextBase = new HttpContextWrapper(context);

            switch (context.Request.Params[ServiceKey])
            {
                case ServiceSearchIndicators:
                    new JsonBuilderSearchIndicators(contextBase) { ServiceName = GetType().Name }.Respond();
                    break;

                default:
                    context.Response.Write("Requested service is not recognised.");
                    context.Response.StatusCode = 500;
                    break;
            }

            //TODO add error response
        }
    }
}