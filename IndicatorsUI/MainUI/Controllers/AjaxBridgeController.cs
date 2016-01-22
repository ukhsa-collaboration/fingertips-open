using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Profiles.DataAccess;
using Profiles.MainUI.Caching;
using Profiles.MainUI.Common;
using Profiles.MainUI.Results;

namespace Profiles.MainUI.Controllers
{
    public class AjaxBridgeController : Controller
    {
        private JsonCache jsonCache = new JsonCache();

        private DateTime startTime = DateTime.Now;

        public JsonResult Data(string serviceAction)
        {
            var request = HttpContext.Request;
            string serviceParameters = request.Url.Query.TrimStart('?')/* e.g. "?pid=15" */;

            byte[] json = jsonCache.GetJson(serviceAction, serviceParameters);

            if (JsonUnit.IsJsonOk(json) == false)
            {
                WebClient wc = new WebClient();
                var url = GetUrl(request);
                json = wc.DownloadData(url);

                if (JsonUnit.IsJsonOk(json))
                {
                    CacheJson(json, serviceAction, serviceParameters, request);
                }
            }

            SetPublicCacheIfLive();

            return new AjaxBridgeJsonResult(json);
        }

        private void SetPublicCacheIfLive()
        {
            if (AppConfig.Instance.IsEnvironmentLive)
            {
                CachePolicyHelper.CacheForOneMonth(HttpContext.Response.Cache);
            }
        }

        [HttpPost]
        public void Log(string serviceAction)
        {
            switch (serviceAction)
            {
                case "exception":
                    ExceptionLogger.LogClientSideException(Request.Params["errorMessage"]);
                    break;
                case "export":
                    ExceptionLogger.LogExport(Request.Params["areaCode"], Request.Params["profileId"], Request.Params["downloadType"]);
                    break;
            }
        }

        private string GetUrl(HttpRequestBase request)
        {
            var coreWsUrl = AppConfig.Instance.CoreWsUrlForAjaxBridge;
            return coreWsUrl.TrimEnd('/') + request.RawUrl/* e.g. "/data/areatypes?pid=15" */;
        }

        private void CacheJson(byte[] json, string serviceAction, string serviceParameters, HttpRequestBase request)
        {
            string absoluteUri = request.Url.AbsoluteUri;

            JsonUnit unit = new JsonUnit(serviceAction, serviceParameters, json, absoluteUri,
                                             (DateTime.Now - startTime).TotalMilliseconds);

            try
            {
                jsonCache.AddJson(unit);
            }
            catch (Exception ex)
            {
                /* Suppress race condition where same JSON added twice 
                     * but log exception in case key is not unique */
                ExceptionLogger.LogException(ex, "Global.asax");
            }
        }

    }
}
