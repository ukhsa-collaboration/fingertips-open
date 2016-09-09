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
        /// <summary>
        /// Gets a named document
        /// </summary>
        /// <param name="profile_key">Profile URL key</param>
        /// <param name="file_name">Name of the file</param>
        /// <param name="time_period">The time period the file is associated with (optional)</param>
        /// <returns>Static document, e.g. PDF report</returns>
        [HttpGet]
        [Route("static-reports")]
        public HttpResponseMessage GetStaticReport(string profile_key, string file_name, string time_period = null)
        {
            try
            {
                var filePath = GetFilePath(profile_key, file_name, time_period);

                // To avoid System.UnauthorizedAccessException
                File.SetAttributes(filePath, FileAttributes.Normal);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                SetFileContent(filePath, result);
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");
                SetFilename(file_name, result);

                return result;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        private static void SetFileContent(string filePath, HttpResponseMessage result)
        {
            var bytes = File.ReadAllBytes(filePath);
            result.Content = new ByteArrayContent(bytes);
        }

        private static void SetFilename(string file_name, HttpResponseMessage result)
        {
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = file_name
            };
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