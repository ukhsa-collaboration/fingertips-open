using System.Web;
using IndicatorsUI.DataAccess;

namespace IndicatorsUI.MainUI.Helpers
{
    public class AjaxUrl
    {
        /// <summary>
        /// URL parameters after '?'
        /// </summary>
        public string ServiceParameters { get; private set; }

        /// <summary>
        /// Full URL to web service
        /// </summary>
        public string WebServicesUrl { get; private set; }

        public AjaxUrl(HttpRequestBase request)
        {
            SetUrl(request);
            ServiceParameters = request.Url.Query.TrimStart('?')/* e.g. "?pid=15" */;
        }

        private void SetUrl(HttpRequestBase request)
        {
            var coreWsUrl = AppConfig.Instance.CoreWsUrlForAjaxBridge;

            // Replace data with api for backwards compatibility
            var rawUrl = request.RawUrl;
            if (rawUrl.StartsWith("/data/"))
            {
                rawUrl = rawUrl.Replace("/data/", "/api/");
            }

            WebServicesUrl = coreWsUrl.TrimEnd('/') + rawUrl/* e.g. "/data/areatypes?pid=15" */;
        }
    }
}