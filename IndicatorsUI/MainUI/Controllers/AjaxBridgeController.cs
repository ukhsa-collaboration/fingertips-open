using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IndicatorsUI.DataAccess;
using IndicatorsUI.MainUI.Caching;
using IndicatorsUI.MainUI.Helpers;
using IndicatorsUI.MainUI.Results;

namespace IndicatorsUI.MainUI.Controllers
{
    public class AjaxBridgeController : Controller
    {
        private readonly JsonCache _jsonCache = new JsonCache();

        private readonly DateTime _startTime = DateTime.Now;

        /// <summary>
        /// General proxy bridge for API calls 
        /// </summary>
        [HttpGet]
        public JsonResult ApiPath(string serviceAction1, string serviceAction2 = "", string serviceAction3 = "")
        {
            var request = HttpContext.Request;
            var ajaxUrl = new AjaxUrl(request);
            var serviceParameters = ajaxUrl.ServiceParameters;

            byte[] json;
            if (serviceParameters.Contains("no_cache") ||
                serviceAction2 == "csv")
            {
                // Must provide uncached responeses
                json = GetJsonFromWebServices(ajaxUrl.WebServicesUrl);
                CachePolicyHelper.SetNoCaching(HttpContext.Response.Cache);
            }
            else
            {
                // Check caches for response
                var serviceKey = serviceAction1 + serviceAction2 + serviceAction3;
                json = _jsonCache.GetJson(serviceKey, serviceParameters);

                if (JsonUnit.IsJsonOk(json) == false)
                {
                    json = GetJsonFromWebServices(ajaxUrl.WebServicesUrl);

                    if (JsonUnit.IsJsonOk(json))
                    {
                        CacheJson(json, serviceKey, serviceParameters, request);
                    }
                }

                // Response is cacheable
                SetCachePolicyForCacheableResponse();
            }

            return new AjaxBridgeJsonResult(json);
        }

        /// <summary>
        /// General logging action
        /// </summary>
        [Route("log/{serviceAction}")]
        [HttpPost]
        public void Log(string serviceAction)
        {
            switch (serviceAction)
            {
                case "exception":
                    ExceptionLogger.LogClientSideException(Request.Params["errorMessage"]);
                    break;
                case "export":
                    ExceptionLogger.LogExport(Request.Params["areaCode"],
                        Request.Params["profileId"], Request.Params["downloadType"]);
                    break;
            }
        }

        private byte[] GetJsonFromWebServices(string url)
        {
            WebClient wc = new WebClient();
            byte[] json = wc.DownloadData(url);
            return json;
        }

        private void SetCachePolicyForCacheableResponse()
        {
            // No caching for API calls in any context
            CachePolicyHelper.SetNoCaching(HttpContext.Response.Cache);
        }

        private void CacheJson(byte[] json, string serviceAction, string serviceParameters, HttpRequestBase request)
        {
            string absoluteUri = request.Url.AbsoluteUri;

            JsonUnit unit = new JsonUnit(serviceAction, serviceParameters, json, absoluteUri,
                                             (DateTime.Now - _startTime).TotalMilliseconds);

            try
            {
                _jsonCache.AddJson(unit);
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
