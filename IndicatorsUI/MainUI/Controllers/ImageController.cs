using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Profiles.DataAccess;
using Profiles.MainUI.Caching;
using Profiles.MainUI.Common;

namespace Profiles.MainUI.Controllers
{
    public class ImageController : Controller
    {
        [FingertipsOutputCache]
        [ValidateInput(false)]
        public ActionResult VerticalText(string text)
        {
            CachePolicyHelper.CacheForOneMonth(HttpContext.Response.Cache);
            MemoryStream stream = new VerticalText().GetImage(text);
            stream.Position = 0;
            return new FileStreamResult(stream, "image/png");
        }

        [FingertipsOutputCache]
        public ActionResult PracticeScatterChart()
        {
            var chartUrl = AppConfig.Instance.CoreWsUrlForAjaxBridge +
                "GetChart.ashx" + HttpContext.Request.Url.Query;

            var data = new WebClient().DownloadData(chartUrl);

            var stream = new MemoryStream(data);
            return new FileStreamResult(stream, "image/png");
        }

        /// <summary>
        /// Returns png image from HTML canvas 
        /// </summary>
        [HttpPost]
        public ActionResult CaptureShot(FormCollection formCollection)
        {
            var capturedImage = formCollection["captured-Image"];
            var trimmedData = capturedImage.Replace("data:image/png;base64,", "");
            var imageFileName = formCollection["tab-name"] ?? "image";
            byte[] imageAsBytes = Convert.FromBase64String(trimmedData);
           
            var stream = new MemoryStream(imageAsBytes);
            var result = new FileStreamResult(stream,"image/png");
            result.FileDownloadName = imageFileName.Trim() + ".png";
            return result;
        }
    }
}