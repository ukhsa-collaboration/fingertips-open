using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IndicatorsUI.DataAccess;
using IndicatorsUI.MainUI.Caching;
using IndicatorsUI.MainUI.Helpers;
using IndicatorsUI.MainUI.Results;
using Newtonsoft.Json;

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
                // Must provide uncached responses
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

        [HttpPost]
        public JsonResult ApiPath(string serviceAction)
        {
            using (var client = new WebClient())
            {
                var request = HttpContext.Request;
                var ajaxUrl = new AjaxUrl(request).WebServicesUrl;

                var response = client.UploadValues(ajaxUrl, "POST", request.Form);
                using (var responseReader = new StreamReader(new MemoryStream(response)))
                {
                    Object deserializeObject = JsonConvert.DeserializeObject<Object>(responseReader.ReadToEnd());
                }

                return new JsonResult()
                {
                    Data = "success",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
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

        private JsonResult PostJsonToWebServices(string url, string jsonData)
        {
            WebClient wc = new WebClient();
            string result = wc.UploadString(url, jsonData);
            return new JsonResult()
            {
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
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
