using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using IndicatorsUI.DataAccess;
using IndicatorsUI.MainUI.Caching;
using IndicatorsUI.MainUI.Helpers;

namespace IndicatorsUI.MainUI.Controllers
{
    public class ImageController : Controller
    {
        [Route("img/vertical-text")]
        [FingertipsOutputCache]
        [ValidateInput(false)]
        public ActionResult VerticalText(string text)
        {
            CachePolicyHelper.CacheForOneMonth(HttpContext.Response.Cache);
            MemoryStream stream = new VerticalText().GetImage(text);
            stream.Position = 0;
            return new FileStreamResult(stream, "image/png");
        }

        [Route("img/gp-scatter-chart")]
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
        [Route("capture-shot")]
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