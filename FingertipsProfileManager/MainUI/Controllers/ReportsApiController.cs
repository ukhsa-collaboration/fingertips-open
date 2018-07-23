using Fpm.MainUI.ViewModels.Report;
using Fpm.ProfileData.Entities.Report;
using Fpm.ProfileData.Repositories;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Fpm.MainUI.Controllers
{
    public class ReportsApiController : Controller
    {
        private readonly ReportRepository _repo = new ReportRepository();

        [Route("api/reports/")]
        public ActionResult All()
        {
            var reports = _repo.GetAllReports();
            var mappings = _repo.GetAllMapping();

            var response = new List<ReportViewModel>();

            foreach (var report in reports)
            {
                var profiles = mappings.Where(x => x.ReportId == report.Id).Select(x => x.ProfileId).ToList();

                response.Add(new ReportViewModel
                {
                    Id = report.Id,
                    Name = report.Name,
                    File = report.File,
                    Parameters = report.Parameters.Split(',').ToList(),
                    Profiles = profiles,
                    Notes = report.Notes,
                    IsLive = report.IsLive
                });
            }

            var json = JsonConvert.SerializeObject(response);
            return Content(json, "application/json");
        }


        [Route("api/reports/{id}")]
        public ActionResult GetReport(int id)
        {
            var report = _repo.GetReportById(id);
            var profilesForReport = _repo.GetMappingByReportId(id);

            var response = new ReportViewModel
            {
                Id = report.Id,
                Name = report.Name,
                File = report.File,
                Parameters = report.Parameters.Split(',').ToList(),
                Profiles = profilesForReport.Where(x => x.ReportId == report.Id).Select(x => x.ProfileId).ToList(),
                Notes = report.Notes,
                IsLive = report.IsLive
            };

            var json = JsonConvert.SerializeObject(response);
            return Content(json, "application/json");
        }

        [HttpDelete]
        [Route("api/reports/{id}")]
        public ActionResult Delete(int id)
        {
            _repo.DeleteReportById(id);
            return Content("delete reports");
        }

        [Route("api/reports/new")]
        [HttpPost]
        public ActionResult New(ReportViewModel model)
        {

            var report = new Report
            {
                Name = model.Name,
                File = model.File,
                Parameters = model.Parameters != null ? string.Join(",", model.Parameters) : "",
                Notes = model.Notes,
                IsLive = model.IsLive
            };


            var reportId = _repo.AddReport(report);

            if (model.Profiles != null)
            {
                foreach (var profile in model.Profiles)
                {
                    var mapping = new ReportsProfileMapping
                    {
                        ReportId = reportId,
                        ProfileId = profile
                    };

                    _repo.AddProfileMapping(mapping);
                }
            }

            return Content("all reports");
        }



        [Route("api/reports/new")]
        [HttpPut]
        public ActionResult Update(ReportViewModel model)
        {
            var report = new Report
            {
                Id = model.Id,
                Name = model.Name,
                File = model.File,
                Parameters = model.Parameters != null ? string.Join(",", model.Parameters) : "",
                Notes = model.Notes,
                IsLive = model.IsLive
            };

            _repo.UpdateReport(report);


            _repo.DeleteMappingForReport(model.Id);

            if (model.Profiles != null)
            {
                foreach (var profile in model.Profiles)
                {
                    var mapping = new ReportsProfileMapping
                    {
                        ReportId = model.Id,
                        ProfileId = profile
                    };

                    _repo.AddProfileMapping(mapping);
                }
            }

            return Content("update reports");
        }
    }
}