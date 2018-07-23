using System.Web;

namespace PholioVisualisation.Services.HttpHandlers
{
    public class GetData : IHttpHandler
    {
        public const string ServiceKey = "s";

        /* 
         * DO NOT ADD ANY MORE SERVICES TO THIS FILE
         * 
         * NEW SERVICES SHOULD BE ADDED TO THE MOST SUITABLE Controller
         */
        public const string ServiceIndicatorMetadata = "im";
        public const string ServiceDataDownloadBespoke = "db";

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var contextBase = new HttpContextWrapper(context);

            switch (context.Request.Params[ServiceKey])
            {
                case ServiceDataDownloadBespoke:
                    // Service retained to provide population download for practice profiles
                    new DataDownloadBespoke(contextBase).Respond();
                    break;

                case ServiceIndicatorMetadata:
                    // OBSOLETE - migrated to Controller in ServicesWeb
                    new JsonBuilderIndicatorMetadata(contextBase) { ServiceName = GetType().Name }.Respond();
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