using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using PholioVisualisation.DataAccess;


namespace ServicesWeb.Controllers
{
    public class StaticReportsController : BaseController
    {
        [HttpGet]
        [Route("static-reports")]
        public HttpResponseMessage GetStaticReport(string profile_key, string file_name, string time_period = null)
        {
            try
            {
                var filePath = GetFilePath(profile_key, file_name, time_period);
                var stream = new FileStream(filePath, FileMode.Open);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");
                return result;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        private static string GetFilePath(string profile_key, string file_name, string time_period)
        {
            var path = Path.Combine(ApplicationConfiguration.StaticReportsDirectory, profile_key);
            if (time_period != null)
            {
                path = Path.Combine(path, time_period);
            }
            string filePath = Path.Combine(path, file_name);
            return filePath;
        }
    }
}