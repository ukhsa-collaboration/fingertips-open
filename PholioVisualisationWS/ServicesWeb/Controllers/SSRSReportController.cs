using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Web.Http;

namespace PholioVisualisation.ServicesWeb.Controllers
{
    [RoutePrefix("api")]
    public class SSRSReportController : BaseController
    {
        /// <summary>
        /// Gets a list of SSRS reports for a profile.
        /// </summary>
        /// <param name="profileId">Profile ID</param>
        [HttpGet]
        [Route("ssrs_reports/{profileId}")]
        public IEnumerable<SSRSReport> GetReports(int profileId)
        {
            var repo = new SSRSReportRepository();
            var reportMappings = repo.GetMappingByProfileId(profileId);
            var reports = new List<SSRSReport>();

            foreach (var mapping in reportMappings)
            {
                var reportId = mapping.ReportId;
                var report = repo.GetReportById(reportId);
                if (report != null)
                {
                    // If env is testing show all reports, if env 
                    // is live show only reports with IsLive flag true
                    if (ApplicationConfiguration.Instance.IsEnvironmentLive)
                    {
                        if (report.IsLive)
                        {
                            reports.Add(report);
                        }
                    }
                    else
                    {
                        reports.Add(report);
                    }
                }
            }

            return reports;
        }
    }
}