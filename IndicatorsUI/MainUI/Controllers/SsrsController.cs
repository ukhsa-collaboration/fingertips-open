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
        public const string SsrsBaseUrl = "http://ssrsslcol01/ReportServer/Pages/ReportViewer.aspx?%2f";

        private StringBuilder _sb = new StringBuilder();

        [HttpGet]
        [Route("reports/ssrs")]
        public ActionResult Index(string reportName = "", string areaCode = null, string areaTypeId = null,
            string parentCode = null, string parentTypeId = null, string groupId = null, string format = null)
        {
            _sb.Append(SsrsBaseUrl);
            SetEnvironmentFolder();
            _sb.Append(reportName);

            AddAreaCode(areaCode);
            AddAreaTypeId(areaTypeId);
            AddParentCode(parentCode);
            AddParentTypeId(parentTypeId);
            AddGroupId(groupId);

            AddFormatParameters(format);

            var ssrsFormat = new SsrsFormat(format);

            try
            {
                // Download file from SSRS
                var webClient = new WebClient();
                webClient.UseDefaultCredentials = true;
                var fileData = webClient.DownloadData(_sb.ToString());

                // Return file
                var filename = areaCode + "." + ssrsFormat.Extension;
                return File(fileData, ssrsFormat.ContentType, filename);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, null);
                var errorMessage = GetErrorMessage(ex);
                return Content(errorMessage);
            }
        }

        private static string GetErrorMessage(Exception ex)
        {
            if (AppConfig.Instance.IsEnvironmentLive)
            {
                return "<div>An error has occured, please contact Fingertips team.</div><br>" + ex.Message;
            }

            return ex.Message + "<br>" + ex.InnerException;
        }

        private void SetEnvironmentFolder()
        {
            var env = AppConfig.Instance.IsEnvironmentLive ? "ssrs-live/" : "ssrs-staging/";
            _sb.Append(env);
        }

        private void AddFormatParameters(string format)
        {
            _sb.Append("&rc:Toolbar=false");
            _sb.Append("&rs:Format=");
            _sb.Append(format);
            _sb.Append("&rs:Command=Render");
        }

        private void AddGroupId(string groupId)
        {
            if (groupId != null)
            {
                _sb.Append("&groupId=");
                _sb.Append(groupId);
            }
        }

        private void AddParentTypeId(string parentTypeId)
        {
            if (parentTypeId != null)
            {
                _sb.Append("&parentTypeId=");
                _sb.Append(parentTypeId);
            }
        }

        private void AddParentCode(string parentCode)
        {
            if (parentCode != null)
            {
                _sb.Append("&parentCode=");
                _sb.Append(parentCode);
            }
        }

        private void AddAreaTypeId(string areaTypeId)
        {
            if (areaTypeId != null)
            {
                _sb.Append("&areaTypeId=");
                _sb.Append(areaTypeId);
            }
        }

        private void AddAreaCode(string areaCode)
        {
            if (areaCode != null)
            {
                _sb.Append("&areaCode=");
                _sb.Append(areaCode);
            }
        }
    }
}