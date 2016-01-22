/* 
 * Created by: Daniel Flint    
 * Date: 21/09/2011
 */
using System;
using System.IO;
using System.Web;
using ErphoBusiness.DataAccess;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export;

namespace PholioVisualisation.Services.HttpHandlers
{
    public class GetIndicatorIndex : IHttpHandler
    {
        public const string FileName = "index.xls";

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                byte[] bytes;
                FileInfo fileInfo = new FileInfo(Path.Combine(ApplicationConfiguration.ExportFileDirectory, FileName));
                if (ApplicationConfiguration.UseFileCache && fileInfo.Exists)
                {
                    bytes = File.ReadAllBytes(fileInfo.FullName);
                }
                else
                {
                    bytes = new IndicatorIndexBuilder { FilePath = fileInfo.FullName }.CreateFile();
                }

                HttpResponse response = context.Response;
                ExportHelper.SetResponseAsExcelFile(response, FileName);
                response.BinaryWrite(bytes);
            }
            catch (Exception ex)
            {
                new DatabaseLogger().LogException(ex, context.Request.Url.AbsoluteUri);
            }
        }

    }
}