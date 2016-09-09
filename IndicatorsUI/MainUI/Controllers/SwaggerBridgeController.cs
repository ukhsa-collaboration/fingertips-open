using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Profiles.DataAccess;
using UrlHelper = Profiles.MainUI.Helpers.UrlHelper;

namespace Profiles.MainUI.Controllers
{
    /// <summary>
    /// End points that provide a bridge between the UI and WS for 
    /// Swagger API docs.
    /// </summary>
    public class SwaggerBridgeController : Controller
    {
        /// <summary>
        /// Gets the Swagger docs landing page
        /// </summary>
        [HttpGet]
        public ActionResult ApiDocsPage()
        {
            var url = GetCoreWsUrl("swagger/ui/index");
            var data = new WebClient().DownloadData(url);
            var html = Encoding.UTF8.GetString(data);

            // Modify asset paths
            html = html
                .Replace("<script src='", "<script src='api/asset/")
                .Replace("<link href='", "<link href='api/asset/");

            data = Encoding.UTF8.GetBytes(html);
            return GetFileStreamResult(data, "text/html");
        }

        /// <summary>
        /// Gets the Swagger details of each service as JSON
        /// </summary>
        [HttpGet]
        public ActionResult ApiServiceDetails()
        {
            var url = GetCoreWsUrl("swagger/docs/v1");
            var data = new WebClient().DownloadData(url);
            return GetFileStreamResult(data, "application/json");
        }

        /// <summary>
        /// Gets JS and CSS Swagger assets
        /// </summary>
        [HttpGet]
        public ActionResult ApiAsset(string part1, string part2 = null)
        {
            var url = GetCoreWsUrl("swagger/ui", part1, part2);
            var data = new WebClient().DownloadData(url);

            var contentType = "text/javascript";
            if (url.EndsWith("css"))
            {
                contentType = "text/css";
            }

            return GetFileStreamResult(data, contentType);
        }

        private static FileStreamResult GetFileStreamResult(byte[] data, string contentType)
        {
            var stream = new MemoryStream(data);
            return new FileStreamResult(stream, contentType);
        }

        private static string GetCoreWsUrl(params string[] urlParts)
        {
            var coreWsUrl = AppConfig.Instance.CoreWsUrlForAjaxBridge;
            var partList = urlParts.ToList();
            partList.Insert(0, coreWsUrl);
            var url = UrlHelper.CombineUrl(partList.ToArray());
            return url;
        }
    }
}