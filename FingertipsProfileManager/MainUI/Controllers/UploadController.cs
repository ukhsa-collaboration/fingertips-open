using Fpm.MainUI.Helpers;
using Fpm.MainUI.ViewModels.Upload;
using Fpm.ProfileData.Entities.Job;
using Fpm.ProfileData.Repositories;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace Fpm.MainUI.Controllers
{
    public class UploadController : Controller
    {
        private readonly IFpmUploadRepository _fpmUploadRepository = new UploadJobRepository();
        private CoreDataRepository _coreDataRepository;
        private LoggingRepository _loggingRepository;

        /// <summary>
        ///  Upload Index
        /// </summary>
        public ActionResult Index()
        {
            const string simpleTemplateRelativePath = "/upload-templates/simple-indicator-upload-template.xlsx";
            const string batchTemplateRelativePath = "/upload-templates/batch-indicator-upload-template.xlsx";

            var viewModel = new UploadIndexViewModel
            {
                SimpleTemplateUrl = simpleTemplateRelativePath,
                BatchTemplateUrl = batchTemplateRelativePath,
                SimpleLastUpdated = AppConfig.LastUpdatedDateSimpleTemplate,
                BatchLastUpdated = AppConfig.LastUpdatedDateBatchTemplate,
                User = UserDetails.CurrentUser()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Upload simple file
        /// </summary>
        [HttpPost]
        public ActionResult UploadSimpleFile(HttpPostedFileBase indicatorDataFile)
        {
            var response = SaveFile(UploadJobType.Simple);
            return Content(response ? "ok" : "fail");
        }

        /// <summary>
        /// Upload batch file
        /// </summary>
        [HttpPost]
        [Route("UploadBatchUpload")]
        public ActionResult UploadBatchUpload(HttpPostedFileBase indicatorDataFile)
        {
            var response = SaveFile(UploadJobType.Batch);
            return Content(response ? "ok" : "fail");
        }

        /// <summary>
        /// Returns list of jobs for current user
        /// </summary>
        [HttpGet]
        public ActionResult CurrentUserJobProgress(int userId)
        {
            var jobs = _fpmUploadRepository.GetJobsForCurrentUser(userId);

            var response = new UploadProgressViewModel
            {
                InProgress = jobs.Count(x => x.Status == UploadJobStatus.InProgress),
                InQueue = jobs.Count(x => x.Status == UploadJobStatus.NotStart),
                AwaitingConfomation = GetNumberOfConfirmationAwaitingJobs(jobs),

                Jobs = jobs
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns job summary, i.e. number of processed rows or error
        /// </summary>
        [HttpGet]
        public ActionResult JobSummary(string guid)
        {
            var response = new UploadSummaryViewModel();

            if (guid.IsNullOrWhiteSpace())
                return Json(response, JsonRequestBehavior.AllowGet);

            var jobGuid = Guid.Parse(guid);
            var job = _fpmUploadRepository.GetJob(jobGuid);
            var summary = _fpmUploadRepository.FindJobErrorsByJobGuid(jobGuid).FirstOrDefault();

            response.JobStatus = job.Status;
            response.ErrorType = summary.ErrorType;
            response.ErrorText = summary.ErrorText;
            response.ErrorJson = summary.ErrorJson;

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Change the job starts from ConfirmationAwaited to ConfirmationGiven
        /// </summary>
        [HttpGet]
        public ActionResult ChangeStatus(string guid, int actionCode)
        {
            if (guid.IsNullOrWhiteSpace() || actionCode == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                var errorResponse = new { Success = "false", Message = "guid or action code missing" };
                return Json(errorResponse, JsonRequestBehavior.AllowGet);
            }

            var jobGuid = Guid.Parse(guid);
            var uploadJob = _fpmUploadRepository.GetJob(jobGuid);
            uploadJob.Status = (UploadJobStatus)actionCode;

            var isUpdated = _fpmUploadRepository.UpdateJob(uploadJob);
            var response = new { Success = isUpdated ? "true" : "false", Message = "" };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns xls or csv file
        /// </summary>
        [HttpGet]
        public ActionResult Download(string guid)
        {
            if (guid.IsNullOrWhiteSpace())
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                var errorResponse = new { Success = "false", Message = "file not found" };
                return Json(errorResponse, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var jobGuid = Guid.Parse(guid);
                var job = _fpmUploadRepository.GetJob(jobGuid);
                var fileExt = Path.GetExtension(job.Filename);
                var filePath = Path.Combine(AppConfig.UploadFolder, jobGuid + fileExt);
                var fileData = System.IO.File.ReadAllBytes(filePath);
                var contentType = MimeMapping.GetMimeMapping(filePath);

                var cd = new ContentDisposition
                {
                    FileName = job.Filename,
                    Inline = true
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(fileData, contentType);
            }
            catch (Exception ex)
            {
                return Redirect("/NotFound");
            }
        }

        private bool SaveFile(UploadJobType jobType)
        {
            bool wasFileSaved;

            if (Request.Files == null)
            {
                return false;
            }

            try
            {
                EnsureUploadDirectoryExists();

                // Save file
                var guid = Guid.NewGuid();
                var file = Request.Files[0];
                var actualFileName = file.FileName;
                var fileName = guid + Path.GetExtension(file.FileName);
                file.SaveAs(Path.Combine(AppConfig.UploadFolder, fileName));

                var uploadJob = new UploadJob
                {
                    DateCreated = DateTime.Now,
                    Guid = guid,
                    Filename = actualFileName,
                    JobType = jobType,
                    Status = UploadJobStatus.NotStart,
                    UserId = UserDetails.CurrentUser().Id
                };

                _fpmUploadRepository.CreateJob(uploadJob);

                wasFileSaved = true;
            }
            catch (Exception ex)
            {
                wasFileSaved = false;
            }

            return wasFileSaved;
        }

        private int GetNumberOfConfirmationAwaitingJobs(IEnumerable<UploadJob> jobs)
        {
            var totalJobsAwaitingConfirmation = jobs.Count(x => x.Status == UploadJobStatus.ConfirmationAwaited);

            var totalJobsAwaitingForSmallNumberConfirmation = jobs.Count(x => x.Status == UploadJobStatus.SmallNumberWarningConfirmationAwaited);

            return totalJobsAwaitingConfirmation + totalJobsAwaitingForSmallNumberConfirmation;
        }

        private static void EnsureUploadDirectoryExists()
        {
            if (!Directory.Exists(AppConfig.UploadFolder))
            {
                Directory.CreateDirectory(AppConfig.UploadFolder);
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _coreDataRepository = new CoreDataRepository();
            _loggingRepository = new LoggingRepository();
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _coreDataRepository.Dispose();
            _loggingRepository.Dispose();
            base.OnActionExecuted(filterContext);
        }
    }
}