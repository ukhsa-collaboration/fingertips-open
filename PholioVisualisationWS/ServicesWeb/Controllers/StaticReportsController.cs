using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using ServicesWeb.Helpers;

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

                // To avoid System.UnauthorizedAccessException when reading file
                File.SetAttributes(filePath, FileAttributes.Normal);
                var bytes = File.ReadAllBytes(filePath);

                var responseBuilder = new FileResponseBuilder();
                responseBuilder.SetFileContent(bytes);
                responseBuilder.SetFilename(file_name);

                return responseBuilder.Message;
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new FingertipsException("Static report cannot be downloaded." +
                    " Assign full control permissions to static_reports_live_a/b directory for fingertips_web_user.", ex);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Whether or not a static document is available
        /// </summary>
        /// <param name="profile_key">Profile URL key</param>
        /// <param name="file_name">Name of the file</param>
        /// <param name="time_period">The time period the file is associated with (optional)</param>
        /// <returns>Whether or not a static document is available</returns>
        [HttpGet]
        [Route("api/static-reports/exists")]
        public bool IsStaticReportAvailable(string profile_key, string file_name, string time_period = null)
        {
            try
            {
                var filePath = GetFilePath(profile_key, file_name, time_period);
                return File.Exists(filePath);
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