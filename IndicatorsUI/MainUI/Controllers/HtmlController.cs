using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using IndicatorsUI.MainUI.Helpers;

namespace IndicatorsUI.MainUI.Controllers
{
    /// <summary>
    /// Allows HTML pages to be served by Fingertips. Also will return other 
    /// document formats if requested although thes are normally downloaded 
    /// directly from the web services
    /// </summary>
    public class HtmlController : Controller
    {
        public const string HtmlContentType = "text/html";
        public const string ByteStreamContentType = "application/octet-stream";

        /// <summary>
        /// Bridge between web services and UI for HTML documents. Other document types
        /// are downloaded directly from the web services.
        /// </summary>
        [Route("static-reports/{profileKey}/{documentName}")]
        public ActionResult GetHtmlPage(string profileKey, string documentName)
        {
            var data = WebServiceHelper.GetDataFromWebServices(
                "static-reports?profile_key=" + profileKey + 
                "&file_name=" + documentName);

            return ReturnDocument(documentName, data);
        }

        /// <summary>
        /// Bridge between web services and UI for HTML documents. Other document types
        /// are downloaded directly from the web services.
        /// </summary>
        [Route("static-reports/{profileKey}/{subfolder}/{documentName}")]
        public ActionResult GetHtmlPageInSubfolder(string profileKey, string subfolder, string documentName)
        {
            var data = WebServiceHelper.GetDataFromWebServices(
                "static-reports?profile_key=" + profileKey +
                "&file_name=" + documentName +
                "&subfolder=" + subfolder);

            return ReturnDocument(documentName, data);
        }

        private static ActionResult ReturnDocument(string documentName, byte[] data)
        {
            if (documentName.EndsWith(".html") || documentName.EndsWith(".htm"))
            {
                return ResponseHelper.GetFileStreamResult(data, HtmlContentType);
            }
            return ResponseHelper.GetFileStreamResult(data, ByteStreamContentType);
        }
    }
}