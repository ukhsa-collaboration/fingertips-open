
using System.Web;

namespace PholioVisualisation.Services
{
    public static class ExportHelper
    {
        public static void SetResponseAsExcelFile(HttpResponseBase response, string filename)
        {
            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
        }
    }
}