using System.Linq;
using System.Net;
using System.Text;
using IndicatorsUI.DataAccess;

namespace IndicatorsUI.MainUI.Helpers
{
    public class WebServiceHelper
    {
        /// <summary>
        /// Gets a document from the web services application
        /// </summary>
        public static string GetHtmlDocumentFromWebServices(string url)
        {
            var data = GetDataFromWebServices(url);
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Gets data from the web services application
        /// </summary>
        public static byte[] GetDataFromWebServices(string url)
        {
            var fullUrl = GetCoreWsUrl(url);
            return new WebClient().DownloadData(fullUrl);
        }

        /// <summary>
        /// Gets the full URL for a call to the web services application
        /// </summary>
        public static string GetCoreWsUrl(params string[] urlParts)
        {
            var coreWsUrl = AppConfig.Instance.CoreWsUrlForAjaxBridge;
            var partList = urlParts.ToList();
            partList.Insert(0, coreWsUrl);
            var url = UrlHelper.CombineUrl(partList.ToArray());
            return url;
        }
    }
}