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
        public const string ServiceGroupingTree = "sg";
        public const string ServiceIndicatorStats = "is";
        public const string ServiceAreaData = "ad";
        public const string ServiceAreaValues = "av";
        public const string ServiceAreaAddress = "aa";
        public const string ServiceParentAreaGroups = "pg";
        public const string ServiceAreaMapping = "am";
        public const string ServiceDataDownloadBespoke = "db";
        public const string ServiceParentAreas = "pa";
        public const string ServiceAreaCategories = "ac";
        public const string ServiceGroupRoots = "gr";
        /* 
         * DO NOT ADD ANY MORE SERVICES TO THIS FILE
         * 
         * NEW SERVICES SHOULD BE ADDED TO THE MOST SUITABLE Controller
         */

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
                    new DataDownloadBespoke(contextBase).Respond();
                    break;

                case ServiceGroupingTree:
                    // OBSOLETE - migrated to Controller in ServicesWeb
                    new JsonBuilderGroupingTree(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceIndicatorMetadata:
                    // OBSOLETE - migrated to Controller in ServicesWeb
                    new JsonBuilderIndicatorMetadata(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceIndicatorStats:
                    // OBSOLETE - migrated to Controller in ServicesWeb
                    new JsonBuilderIndicatorStats(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceAreaData:
                    // OBSOLETE - migrated to Controller in ServicesWeb
                    new JsonBuilderAreaData(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceAreaValues:
                    // OBSOLETE - migrated to Controller in ServicesWeb
                    new JsonBuilderAreaValues(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceGroupRoots:
                    // OBSOLETE - migrated to Controller in ServicesWeb
                    new JsonBuilderGroupRoots(contextBase) { ServiceName = GetType().Name }.Respond();
                    break;

                case ServiceAreaAddress:
                    // OBSOLETE - migrated to Controller in ServicesWeb
                    new JsonBuilderAreaAddress(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceAreaMapping:
                    // OBSOLETE - migrated to Controller in ServicesWeb
                    new JsonBuilderAreaMapping(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceParentAreaGroups:
                    // OBSOLETE - migrated to Controller in ServicesWeb
                    new JsonBuilderParentAreaGroups(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceAreaCategories:
                    // OBSOLETE - migrated to Controller in ServicesWeb
                    new JsonBuilderAreaCategories(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceParentAreas:
                    // OBSOLETE - migrated to Controller in ServicesWeb
                    new JsonBuilderParentAreas(contextBase) {ServiceName = GetType().Name}.Respond();
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