using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Http;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Helpers;

namespace PholioVisualisation.ServicesWeb.Controllers
{
    public class StaticReportsController : BaseController
    {
        /// <summary>
        /// Gets a named document
        /// </summary>
        /// <param name="profile_key">Profile URL key</param>
        /// <param name="file_name">Name of the file</param>
        /// <param name="time_period">The time period the file is associated with [optional]</param>
        /// <returns>Static document, e.g. PDF report</returns>
        [HttpGet]
        [Route("static-reports")]
        public HttpResponseMessage GetStaticReport(string profile_key, string file_name,
            string time_period = null, string subfolder = null)
        {
            try
            {
                var filePath = GetFilePath(profile_key, file_name, time_period, subfolder);

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
        /// <param name="time_period">The time period the file is associated with [optional]</param>
        /// <returns>Whether or not a static document is available</returns>
        [HttpGet]
        [Route("api/static-reports/exists")]
        public bool IsStaticReportAvailable(string profile_key, string file_name, string time_period = null,
            string subfolder = null)
        {
            try
            {
                var filePath = GetFilePath(profile_key, file_name, time_period, subfolder);
                return File.Exists(filePath);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        private static string GetFilePath(string profileKey, string fileName, string timePeriod, string subfolder)
        {
            var path = Path.Combine(ApplicationConfiguration.Instance.StaticReportsDirectory, profileKey);

            if (subfolder != null)
            {
                path = Path.Combine(path, subfolder);
            }

            if (timePeriod != null)
            {
                path = Path.Combine(path, timePeriod);
            }

            string filePath = Path.Combine(path, fileName);
            return filePath;
        }
    }
}