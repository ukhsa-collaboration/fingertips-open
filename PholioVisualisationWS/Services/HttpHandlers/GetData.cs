using System.Web;

namespace PholioVisualisation.Services.HttpHandlers
{
    public class GetData : IHttpHandler
    {
        public const string ServiceKey = "s";

        /* 
         * DO NOT ADD ANY MORE SERVICES TO THIS FILE
         * 
         * NEW SERVICES SHOULD BE ADDED TO ServiceData
         */
        public const string ServiceIndicatorMetadata = "im";
        public const string ServiceGroupingTree = "sg";
        public const string ServiceIndicatorStats = "is";
        public const string ServiceAreaData = "ad";
        public const string ServiceAreaValues = "av";
        public const string ServiceAreaAddress = "aa";
        public const string ServiceGroupedIndicatorNames = "gi";
        public const string ServiceParentAreaGroups = "pg";
        public const string ServiceAreaMapping = "am";
        public const string ServiceDataDownloadBespoke = "db";
        public const string ServiceParentAreas = "pa";
        public const string ServiceHelpText = "ht";
        public const string ServiceAreaCategories = "ac";
        public const string ServiceGroupRoots = "gr";
        /* 
         * DO NOT ADD ANY MORE SERVICES TO THIS FILE
         * 
         * NEW SERVICES SHOULD BE ADDED TO DataController
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
                    new JsonBuilderGroupingTree(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceIndicatorMetadata:
                    new JsonBuilderIndicatorMetadata(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceIndicatorStats:
                    new JsonBuilderIndicatorStats(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceAreaData:
                    new JsonBuilderAreaData(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceAreaValues:
                    new JsonBuilderAreaValues(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceAreaAddress:
                    new JsonBuilderAreaAddress(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceGroupedIndicatorNames:
                    new JsonBuilderGroupedIndicatorNames(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceAreaMapping:
                    new JsonBuilderAreaMapping(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceParentAreaGroups:
                    new JsonBuilderParentAreaGroups(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceAreaCategories:
                    new JsonBuilderAreaCategories(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceParentAreas:
                    new JsonBuilderParentAreas(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceHelpText:
                    new JsonBuilderHelpText(contextBase) {ServiceName = GetType().Name}.Respond();
                    break;

                case ServiceGroupRoots:
                    new JsonBuilderGroupRoots(contextBase) {ServiceName = GetType().Name}.Respond();
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