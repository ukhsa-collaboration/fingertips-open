using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using FingertipsBridgeWS.Cache;

namespace FingertipsBridgeWS.Services
{
    public class ServiceBridge
    {
        public const string ContentTypeJson = "application/json; charset=utf-8";

        private JsonWebCache webCache;
        private bool isResponseOk = true;
        private bool isServiceCachable = true;
        private DateTime startTime = DateTime.Now;
        private string absoluteUri;
        private CacheKeyBuilder keyBuilder;

        public HttpContext Context { get; set; }
        private HttpStatusCode statusCode = HttpStatusCode.OK;

        public JsonWebCache WebCache
        {
            get { return webCache ?? (webCache = new JsonWebCache()); }
        }

        public void Respond(bool isServiceCachable, string serviceId)
        {
            this.isServiceCachable = isServiceCachable;

            absoluteUri = Context.Request.Url.AbsoluteUri;

            try
            {
                NameValueCollection parameters = Context.Request.QueryString;

                keyBuilder = new CacheKeyBuilder(parameters, serviceId);

                byte[] json = null;

                if (isServiceCachable && keyBuilder.CanJsonBeCached)
                {
                    json = WebCache.GetJson(keyBuilder.ServiceId, keyBuilder.CacheKey);
                }

                if (JsonUnit.IsJsonOk(json) == false)
                {
                    json = GetJsonFromCoreWs();
                }

                WriteResponse(json);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, absoluteUri);

                Context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Context.Response.Write(ex.Message + " STACK=" + ex.StackTrace + " URL=" + absoluteUri);
                Context.Response.ContentType = ContentTypeJson;
                Context.Response.Flush();
            }
        }

        private void WriteResponse(byte[] json)
        {
            if (isResponseOk)
            {
                if (AppConfiguration.UseClientCaching)
                {
                    CachePolicyHelper.SetToBeCached(Context.Response.Cache);
                }
            }

            Context.Response.StatusCode = (int)statusCode;

            Context.Response.ContentType = ContentTypeJson;

            Context.Response.BinaryWrite(json);

            Context.Response.Flush();
        }

        private byte[] GetJsonFromCoreWs()
        {
            // Download JSON
            var json = DownloadJson(absoluteUri);

            // Cache JSON if necessary
            if (JsonUnit.IsJsonOk(json))
            {
                CacheJson(json);
                return json;
            }

            // Failure
            statusCode = HttpStatusCode.InternalServerError;
            isResponseOk = false;
            return json;
        }

        private void CacheJson(byte[] json)
        {
            if (isServiceCachable && keyBuilder.CanJsonBeCached)
            {
                JsonUnit unit = new JsonUnit(keyBuilder.ServiceId, keyBuilder.CacheKey, json, absoluteUri,
                                             (DateTime.Now - startTime).TotalMilliseconds);

                try
                {
                    WebCache.AddJson(unit);
                }
                catch (Exception ex)
                {
                    /* Suppress race condition where same JSON added twice 
                         * but log exception in case key is not unique */
                    ExceptionLogger.LogException(ex, absoluteUri);
                }
            }
        }

        private byte[] DownloadJson(string absoluteUri)
        {
            byte[] json = null;
            try
            {
                string url = AppConfiguration.CoreWsUrlForAjaxBridge + Context.Request.RawUrl;

                WebRequest request = WebRequest.Create(url);
                request.Headers.Add("IndicatorUI-Bridge", AppConfiguration.UserAgent);
                request.Timeout = 5 * 60 * 1000; // 5 minutes
                json = GetResponseAsBytes(request);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, absoluteUri);
            }
            return json;
        }

        private static byte[] GetResponseAsBytes(WebRequest request)
        {

            string text;
            var response = (HttpWebResponse)request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            return System.Text.Encoding.UTF8.GetBytes(text);
        }

    }
}