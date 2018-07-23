using IndicatorsUI.DataAccess;
using IndicatorsUI.MainUI.Helpers;
using System;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace IndicatorsUI.MainUI.Controllers
{
    public class SsrsController : Controller
    {
        [HttpGet]
        [Route("reports/ssrs")]
        public ActionResult Index(string reportName = "", string areaCode = null, string areaTypeId = null,
            string parentCode = null, string parentTypeId = null, string groupId = null, string format = null)
        {
            var ssrsBaseUrl = "http://ssrsslcol01/ReportServer/Pages/ReportViewer.aspx?%2f";

            var env = AppConfig.Instance.IsEnvironmentLive ? "ssrs-live/" : "ssrs-staging/";

            var sb = new StringBuilder();
            sb.Append(ssrsBaseUrl);
            sb.Append(env);
            sb.Append(reportName);

            if (areaCode != null)
            {
                sb.Append("&areaCode=");
                sb.Append(areaCode);
            }
            if (areaTypeId != null)
            {
                sb.Append("&areaTypeId=");
                sb.Append(areaTypeId);
            }
            if (parentCode != null)
            {
                sb.Append("&parentCode=");
                sb.Append(parentCode);
            }
            if (parentTypeId != null)
            {
                sb.Append("&parentTypeId=");
                sb.Append(parentTypeId);
            }
            if (groupId != null)
            {
                sb.Append("&groupId=");
                sb.Append(groupId);
            }
            sb.Append("&rc:Toolbar=false");
            sb.Append("&rs:Format=");
            sb.Append(format);
            sb.Append("&rs:Command=Render");

            string contentType = null;
            string extension = null;

            if (format == "pdf")
            {
                contentType = "application/pdf";
                extension = "pdf";
            }
            else if (format == "word")
            {
                contentType = "application/msword";
                extension = "doc";
            }

            var filename = areaCode + "." + extension;

            try
            {
                var webClient = new WebClient();
                webClient.UseDefaultCredentials = true;
                var buf = webClient.DownloadData(sb.ToString());
                return File(buf, contentType, filename);
            }
            catch (Exception ex)
            {

                ExceptionLogger.LogException(ex, null);

                var errorMessage = "<div>An error has occured, please contact Fingertips team.</div><br>" + ex.Message;

                if (AppConfig.Instance.IsEnvironmentLive)
                {
                    return Content(errorMessage);
                }

                return Content(ex.Message + "<br>" + ex.InnerException);
            }
        }
    }
}