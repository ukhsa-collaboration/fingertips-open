using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using IndicatorsUI.MainUI.Helpers;

namespace IndicatorsUI.MainUI.Controllers
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
        [Route("api")]
        public ActionResult ApiDocsPage()
        {
            var html = WebServiceHelper.GetHtmlDocumentFromWebServices("swagger/ui/index");

            // Modify asset paths
            html = html
                .Replace("<script src='", "<script src='api/asset/")
                .Replace("<link href='", "<link href='api/asset/");

            var data = Encoding.UTF8.GetBytes(html);
            return ResponseHelper.GetFileStreamResult(data, "text/html");
        }

        /// <summary>
        /// Gets the Swagger details of each service as JSON
        /// </summary>
        [HttpGet]
        [Route("swagger/docs/v1")]
        public ActionResult ApiServiceDetails()
        {
            var data = WebServiceHelper.GetDataFromWebServices("swagger/docs/v1");
            return ResponseHelper.GetFileStreamResult(data, "application/json");
        }

        /// <summary>
        /// Gets JS and CSS Swagger assets
        /// </summary>
        [HttpGet]
        [Route("api/asset/{part1}")]
        public ActionResult ApiAsset(string part1)
        {
            var url = WebServiceHelper.GetCoreWsUrl("swagger/ui", part1);
            var data = new WebClient().DownloadData(url);
            var contentType = GetContentType(url);
            return ResponseHelper.GetFileStreamResult(data, contentType);
        }

        /// <summary>
        /// Gets JS and CSS Swagger assets
        /// </summary>
        [HttpGet]
        [Route("api/asset/{part1}/{part2}")]
        public ActionResult ApiAsset(string part1, string part2 = null)
        {
            var url = WebServiceHelper.GetCoreWsUrl("swagger/ui", part1, part2);
            var data = new WebClient().DownloadData(url);
            var contentType = GetContentType(url);
            return ResponseHelper.GetFileStreamResult(data, contentType);
        }

        private static string GetContentType(string url)
        {
            var contentType = "text/javascript";
            if (url.EndsWith("css"))
            {
                contentType = "text/css";
            }
            return contentType;
        }
    }
}