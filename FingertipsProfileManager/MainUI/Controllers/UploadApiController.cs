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
    [RoutePrefix("api/upload")]
    public class UploadApiController : Controller
    {
        private readonly IUploadJobRepository _uploadJobRepository;
        private readonly IUserRepository _userRepository;

        public UploadApiController(IUploadJobRepository uploadJobRepository, IUserRepository userRepository)
        {
            _uploadJobRepository = uploadJobRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Returns list of jobs for current user
        /// </summary>
        [Route("progress/{userId}/{numberOfRecords}")]
        [HttpGet]
        public ActionResult CurrentUserJobProgress(int userId, int numberOfRecords)
        {
            var jobs = _uploadJobRepository.GetJobsForCurrentUser(userId, numberOfRecords);
            var response = new UploadProgressViewModel
            {
                InProgress = jobs.Count(x => x.Status == UploadJobStatus.InProgress),
                InQueue = jobs.Count(x => x.Status == UploadJobStatus.NotStarted),
                AwaitingConfirmation = GetNumberOfConfirmationAwaitingJobs(jobs),

                Jobs = jobs
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [Route("all-active-job-progress")]
        [HttpGet]
        public ActionResult GetAllActiveJobProgress()
        {
            var jobs = _uploadJobRepository.GetAllJobsProgress();
            var fpmUsers = _userRepository.GetAllFpmUsers();

            var response = new List<UploadQueueViewModel>();

            foreach (var uploadJob in jobs)
            {
                response.Add(new UploadQueueViewModel
                {
                    DateCreatedF = uploadJob.DateCreatedF,
                    Filename = uploadJob.OriginalFile,
                    Username = fpmUsers.FirstOrDefault(x => x.Id == uploadJob.UserId).DisplayName,
                    StatusText = UploadHelper.GetTextFromStatusCodeForActiveJobs(uploadJob.Status),
                    Status = uploadJob.Status,
                    Guid = uploadJob.Guid
                });
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns job summary, i.e. number of processed rows or error
        /// </summary>
        [Route("summary/{guid}")]
        [HttpGet]
        public ActionResult JobSummary(string guid)
        {
            var response = new UploadSummaryViewModel();

            if (guid.IsNullOrWhiteSpace())
                return Json(response, JsonRequestBehavior.AllowGet);

            var jobGuid = Guid.Parse(guid);
            var job = _uploadJobRepository.GetJob(jobGuid);
            var summary = _uploadJobRepository.FindJobErrorsByJobGuid(jobGuid).FirstOrDefault();

            response.JobStatus = job.Status;
            response.ErrorType = summary.ErrorType;
            response.ErrorText = summary.ErrorText;
            response.ErrorJson = summary.ErrorJson;

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Change the job starts from OverrideDatabaseDuplicatesConfirmationAwaited to OverrideDatabaseDuplicatesConfirmationGiven
        /// </summary>
        [Route("change-status/{guid}/{actionCode}")]
        [HttpGet]
        public ActionResult ChangeStatus(string guid, int actionCode)
        {
            if (guid.IsNullOrWhiteSpace() || actionCode == 0)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                var errorResponse = new { Success = "false", Message = "guid or action code missing" };
                return Json(errorResponse, JsonRequestBehavior.AllowGet);
            }

            var jobGuid = Guid.Parse(guid);
            var uploadJob = _uploadJobRepository.GetJob(jobGuid);
            uploadJob.Status = (UploadJobStatus)actionCode;

            if (actionCode == (int)UploadJobStatus.SmallNumberWarningConfirmationGiven)
            {
                uploadJob.IsSmallNumberOverrideApplied = true;
            }

            var isUpdated = _uploadJobRepository.UpdateJob(uploadJob);
            var response = new { Success = isUpdated ? "true" : "false", Message = "" };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns xls or csv file
        /// </summary>
        [Route("download-file/{guid}")]
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
                var job = _uploadJobRepository.GetJob(jobGuid);
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
            catch (Exception)
            {
                return Redirect("/NotFound");
            }
        }

        /// <summary>
        /// Upload batch file
        /// </summary>
        [Route("upload-file")]
        [HttpPost]
        public ActionResult UploadBatchUpload()
        {
            var response = SaveFile(UploadJobType.Batch);

            return new HttpStatusCodeResult(response ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
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
                var selectionValue = Request.Form[0];
                var actualFileName = file.FileName;
                var fileName = guid + Path.GetExtension(file.FileName);
                file.SaveAs(Path.Combine(AppConfig.UploadFolder, fileName));

                var uploadJob = new UploadJob
                {
                    DateCreated = DateTime.Now,
                    Guid = guid,
                    Filename = actualFileName,
                    JobType = jobType,
                    Status = UploadJobStatus.NotStarted,
                    UserId = UserDetails.CurrentUser().Id,
                    IsConfirmationRequiredToOverrideDatabaseDuplicates = Convert.ToBoolean(selectionValue)
                };

                _uploadJobRepository.CreateJob(uploadJob);

                wasFileSaved = true;
            }
            catch (Exception)
            {
                wasFileSaved = false;
            }

            return wasFileSaved;
        }

        private int GetNumberOfConfirmationAwaitingJobs(IEnumerable<UploadJob> jobs)
        {
            var totalJobsAwaitingConfirmation = jobs.Count(x => x.Status == UploadJobStatus.OverrideDatabaseDuplicatesConfirmationAwaited);

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
    }
}