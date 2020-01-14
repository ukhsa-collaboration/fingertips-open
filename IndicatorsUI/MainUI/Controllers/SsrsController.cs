using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using IndicatorsUI.DataAccess;
using IndicatorsUI.MainUI.Helpers;

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
            var fileName = String.Format("{0}-{1}.{2}", reportName.Replace("/", "-"), areaCode, ssrsFormat.Extension);

            var ssrsUrl = _sb.ToString();
            try
            {
                // Return file
                var fileData = GetFileData(fileName, ssrsUrl);
                return File(fileData, ssrsFormat.ContentType, fileName);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, ssrsUrl);
                var errorMessage = GetErrorMessage(ex);
                return Content(errorMessage);
            }
        }

        private byte[] GetFileData(string fileName, string ssrsUrl)
        {
            byte[] fileData;

            if (AppConfig.Instance.UseFileCaching)
            {
                // Return the saved report if exists or generate the report
                var filePath = GetReportFilePath(fileName);         
                fileData = ReadFileFromDisk(filePath);

                if (fileData == null)
                {
                    // Download file
                    fileData = DownloadFile(ssrsUrl);

                    // Save file to disk
                    SaveFileToDisk(filePath, fileData);
                }
            }
            else
            {
                // Download file
                fileData = DownloadFile(ssrsUrl);
            }

            return fileData;
        }

        private byte[] ReadFileFromDisk(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllBytes(filePath);
            }

            return null;
        }

        private void SaveFileToDisk(string filePath, byte[] fileData)
        {
            try
            {
                System.IO.File.WriteAllBytes(filePath, fileData);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, null);
            }
        }

        private static string GetReportFilePath(string fileName)
        {
            var directory = Path.Combine(AppConfig.Instance.StaticReportsDirectory, "ssrs");

            // Ensure directory exists
            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }

            var fileNameWithPath = Path.Combine(directory, fileName);
            return fileNameWithPath;
        }

        private byte[] DownloadFile(string address)
        {
            // Download file from SSRS
            var webClient = new WebClient();
            webClient.UseDefaultCredentials = true;

            return webClient.DownloadData(address);
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