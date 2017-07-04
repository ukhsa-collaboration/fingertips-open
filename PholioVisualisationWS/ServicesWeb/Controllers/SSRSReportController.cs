using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Web.Http;

namespace ServicesWeb.Controllers
{
    [RoutePrefix("api")]
    public class SSRSReportController : BaseController
    {
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
                reports.Add(repo.GetReportById(reportId));
            }


            return reports;
        }
    }
}