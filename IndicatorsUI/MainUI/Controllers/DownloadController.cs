using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;
using System.Net.Mime;

namespace IndicatorsUI.MainUI.Controllers
{
    public class DownloadController : Controller
    {
        [Route("documents/{filename}.{ext}")]
        public ActionResult Index(string filename, string ext)
        {
            if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(ext)) return HttpNotFound();

            filename = filename + "." + ext;
            Document document = ReaderFactory.GetDocumentReader().GetDocument(filename);
            if (document == null)
            {
                return HttpNotFound();
            }
            
            var cd = new ContentDisposition
            {
                FileName = document.FileName,
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            var contentType = GetContentType(document.FileName);
            return File(document.FileData, contentType);
        }

        private static string GetContentType(string fileName)
        {
            IDictionary<string, string> mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {                                          
                {".avi", "video/x-msvideo"},                
                {".bmp", "image/bmp"},                
                {".csv", "text/csv"},                
                {".doc", "application/msword"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},                                
                {".dvi", "application/x-dvi"},                
                {".fla", "application/octet-stream"},                
                {".flv", "video/x-flv"},                
                {".gif", "image/gif"},                
                {".ico", "image/x-icon"},                
                {".jpe", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},                
                {".mod", "video/mpeg"},
                {".mov", "video/quicktime"},
                {".movie", "video/x-sgi-movie"},
                {".mp2", "video/mpeg"},
                {".mp2v", "video/mpeg"},                
                {".mp4", "video/mp4"},
                {".mp4v", "video/mp4"},
                {".mpa", "video/mpeg"},
                {".mpe", "video/mpeg"},
                {".mpeg", "video/mpeg"},
                {".mpf", "application/vnd.ms-mediapackage"},
                {".mpg", "video/mpeg"},
                {".mpv2", "video/mpeg"},
                {".mqv", "video/quicktime"},                
                {".png", "image/png"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},                                
                {".swf", "application/x-shockwave-flash"},                
                {".tif", "image/tiff"},
                {".tiff", "image/tiff"},                
                {".txt", "text/plain"},
                {".wav", "audio/wav"},
                {".wave", "audio/wav"},                
                {".wm", "video/x-ms-wm"},               
                {".wmp", "video/x-ms-wmp"},
                {".wmv", "video/x-ms-wmv"},
                {".wmx", "video/x-ms-wmx"},                
                {".xbm", "image/x-xbitmap"},                
                {".xla", "application/vnd.ms-excel"},
                {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
                {".xlc", "application/vnd.ms-excel"},
                {".xld", "application/vnd.ms-excel"},
                {".xlk", "application/vnd.ms-excel"},
                {".xll", "application/vnd.ms-excel"},
                {".xlm", "application/vnd.ms-excel"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
                {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".xlt", "application/vnd.ms-excel"},
                {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
                {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
                {".xlw", "application/vnd.ms-excel"},
                {".xml", "text/xml"},                
                {".zip", "application/x-zip-compressed"},
            };

            var ext = Path.GetExtension(fileName);

            if (!ext.StartsWith("."))
            {
                ext = "." + ext;
            }

            string mime;
            return mappings.TryGetValue(ext, out mime) ? mime : "application/octet-stream";
        }

    }
}
