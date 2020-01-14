using Fpm.MainUI.Helpers;
using Fpm.MainUI.ViewModels.Report;
using Fpm.ProfileData.Entities.Report;
using Fpm.ProfileData.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Fpm.MainUI.Controllers
{
    public class ReportsApiController : Controller
    {
        private readonly IReportRepository _reportRepository;

        public ReportsApiController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        [Route("api/reports/")]
        public ActionResult All()
        {
            var reports = _reportRepository.GetAllReports();
            var mappings = _reportRepository.GetAllMapping();

            var reportViewModels = new List<ReportViewModel>();

            foreach (var report in reports)
            {
                var profiles = mappings.Where(x => x.ReportId == report.Id).Select(x => x.ProfileId).ToList();
                var reportViewModel = GetReportViewModel(report, profiles);
                reportViewModels.Add(reportViewModel);
            }

            reportViewModels = reportViewModels.OrderBy(x => x.Name).ToList();

            var json = JsonConvert.SerializeObject(reportViewModels);
            return Content(json, "application/json");
        }


        [Route("api/reports/{id}")]
        public ActionResult GetReport(int id)
        {
            var report = _reportRepository.GetReportById(id);
            var profilesForReport = _reportRepository.GetMappingByReportId(id);

            var profiles = profilesForReport.Where(x => x.ReportId == report.Id).Select(x => x.ProfileId).ToList();

            var reportViewModel = GetReportViewModel(report, profiles);

            var json = JsonConvert.SerializeObject(reportViewModel);
            return Content(json, "application/json");
        }

        [HttpDelete]
        [Route("api/reports/{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                _reportRepository.DeleteReportById(id);

                var json = JsonConvert.SerializeObject(string.Empty);
                return Content(json, "application/json");
            }
            catch (Exception exception)
            {
                ExceptionLogger.LogException(exception, "Global.asax");
            }

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
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
                IsLive = model.IsLive,
                AreaTypeIds = model.AreaTypeIds != null ? string.Join(",", model.AreaTypeIds) : ""
            };


            var reportId = _reportRepository.AddReport(report);

            if (model.Profiles != null)
            {
                foreach (var profile in model.Profiles)
                {
                    var mapping = new ReportsProfileMapping
                    {
                        ReportId = reportId,
                        ProfileId = profile
                    };

                    _reportRepository.AddProfileMapping(mapping);
                }
            }

            var json = JsonConvert.SerializeObject(report);
            return Content(json, "application/json");
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
                IsLive = model.IsLive,
                AreaTypeIds = model.AreaTypeIds != null ? string.Join(",", model.AreaTypeIds) : ""
            };

            _reportRepository.UpdateReport(report);


            _reportRepository.DeleteMappingForReport(model.Id);

            if (model.Profiles != null)
            {
                foreach (var profile in model.Profiles)
                {
                    var mapping = new ReportsProfileMapping
                    {
                        ReportId = model.Id,
                        ProfileId = profile
                    };

                    _reportRepository.AddProfileMapping(mapping);
                }
            }

            var json = JsonConvert.SerializeObject(report);
            return Content(json, "application/json");
        }

        private static ReportViewModel GetReportViewModel(Report report, List<int> profiles)
        {
            var response = new ReportViewModel
            {
                Id = report.Id,
                Name = report.Name,
                File = report.File,
                Parameters = report.Parameters.Split(',').ToList(),
                Profiles = profiles,
                Notes = report.Notes,
                IsLive = report.IsLive,
                AreaTypeIds = report.AreaTypeIds != null ? report.AreaTypeIds.Split(',').ToList() : new List<string>()
            };

            return response;
        }
    }
}