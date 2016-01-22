using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.ViewModels.Upload;
using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using Fpm.Upload;

namespace Fpm.MainUI.Controllers
{
    public class UploadController : Controller
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();

        private CoreDataRepository _coreDataRepository;
        private LoggingRepository _loggingRepository;

        public ActionResult Index()
        {
            const string simpleTemplateRelativePath = "/upload-templates/simple-indicator-upload-template.xlsx";
            const string batchTemplateRelativePath = "/upload-templates/batch-indicator-upload-template.xlsx";

            var viewModel = new UploadIndexViewModel
            {
                SimpleTemplateUrl = simpleTemplateRelativePath,
                BatchTemplateUrl = batchTemplateRelativePath,
                SimpleLastUpdated = AppConfig.LastUpdatedDateSimpleTemplate,
                BatchLastUpdated = AppConfig.LastUpdatedDateBatchTemplate
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ValidateSimpleUpload(HttpPostedFileBase excelFile)
        {
            ViewBag.IsSimple = true;

            var validator = new SimpleUploadValidator(excelFile, _coreDataRepository, _loggingRepository);

            if (validator.IsFileTooLarge ||
                validator.AreValidWorksheets == false)
            {
                // No point validating spreadsheet
                return View("SpreadsheetSimpleSummary", validator.SimpleUpload);
            }

            // Validate spreadsheet
            SimpleUpload simpleUpload = validator.Validate();

            // Check if Current user has permission for the profile
            var indicatorIds = new List<int> { simpleUpload.IndicatorId };
            ViewBag.Disallowed = CheckIndicatorPermissionForCurrentUser(indicatorIds);

            ViewBag.ExcelFilePath = simpleUpload.FileName;
            ViewBag.SelectedWorksheet = simpleUpload.SelectedWorksheet;
            ViewBag.UploadBatchId = simpleUpload.UploadBatchId;

            return View("SpreadsheetSimpleSummary", simpleUpload);
        }

        [HttpPost]
        public ActionResult UploadSimpleData(string excelFilePath, string shortFilename, string selectedWorksheet,
            Guid uploadBatchId)
        {
            var simpleUpload = new SimpleWorkSheetProcessor(_coreDataRepository, _loggingRepository).UploadData(
                excelFilePath, shortFilename, selectedWorksheet, uploadBatchId, UserDetails.CurrentUser().Name);

            return View("InsertSummary", simpleUpload);
        }

        public ActionResult UploadSimpleDataAndArchiveDuplicates(string excelFilePath, string shortFilename,
            string selectedWorksheet, Guid uploadBatchId)
        {
            var simpleUpload = new SimpleWorkSheetProcessor(_coreDataRepository, _loggingRepository).UploadSimpleDataAndArchiveDuplicates(
                excelFilePath, shortFilename, selectedWorksheet, uploadBatchId, UserDetails.CurrentUser().Name);

            return View("InsertSummary", simpleUpload);
        }

        [HttpPost]
        [Route("ValidateBatchUpload")]
        public ActionResult ValidateBatchUpload(HttpPostedFileBase excelFile)
        {
            ViewBag.IsSimple = false;

            var validator = new BatchUploadValidator(excelFile);

            if (validator.IsFileTooLarge ||
                validator.IsPholioSheet == false)
            {
                // No point validating spreadsheet 
                return View("SpreadsheetBatchSummary", validator.BatchUpload);
            }

            // Validate spreadsheet
            BatchUpload batchUpload = validator.Validate();

            // List of indicator ids in current batch
            var indicatorIds = validator.IndicatorIdsInBatch;
            ViewBag.Disallowed = CheckIndicatorPermissionForCurrentUser(indicatorIds);

            ViewBag.ExcelFilePath = batchUpload.FileName;
            ViewBag.SelectedWorksheet = batchUpload.SelectedWorksheet;
            ViewBag.UploadBatchId = batchUpload.UploadBatchId;

            return View("SpreadsheetBatchSummary", batchUpload);
        }

        private List<string> CheckIndicatorPermissionForCurrentUser(List<int> indicators)
        {
            var permissions = new List<string>();
            // List of indicator ids in current batch
            var listOfIndicatorsInCurrentBatch = indicators;
            // Get list of profiles where current user has permission
            var user = UserDetails.CurrentUser();
            var userProfiles = user.GetProfilesUserHasPermissionsTo().ToList();
            var userProfileIds = userProfiles.Select(x => x.Id).ToList();

            // Get the dictionary for indicators and owner profiles 
            var indicatorIds = _reader.GetIndicatorIdsByProfileIds(userProfileIds);

            var disallowedIndicators = new List<int>();

            foreach (var i in listOfIndicatorsInCurrentBatch)
            {
                if (!indicatorIds.ContainsKey(i))
                {
                    disallowedIndicators.Add(i);
                }
            }
            if (disallowedIndicators.Count > 0)
            {
                disallowedIndicators.Reverse();
                foreach (var indicatorId in disallowedIndicators)
                {
                    var disallowedProfile = _reader.GetOwnerProfilesByIndicatorIds(indicatorId);
                    var message = disallowedProfile != null
                        ? " is owned by " + disallowedProfile.Name
                        : " does not exist";
                    permissions.Add("Indicator " + indicatorId + message);
                }
            }

            return permissions;
        }

        [HttpPost]
        public ActionResult UploadBatchData(string excelFilePath, string shortFilename,
            string selectedWorksheet, Guid uploadBatchId, int duplicateSpreadsheetErrors)
        {
            BatchUpload batchUpload = new BatchUploader(_coreDataRepository, _loggingRepository).UploadBatchData(excelFilePath,
                shortFilename, selectedWorksheet, uploadBatchId, UserDetails.CurrentUser().Name, duplicateSpreadsheetErrors);

            return View("InsertSummary", batchUpload);
        }

        public ActionResult UploadBatchDataAndArchiveDuplicates(string excelFilePath, string shortFilename,
            string selectedWorksheet, Guid uploadBatchId)
        {
            BatchUpload batchUpload = new BatchUploader(_coreDataRepository, _loggingRepository).UploadDataAndArchiveDuplicates(excelFilePath, shortFilename,
                selectedWorksheet, uploadBatchId, UserDetails.CurrentUser().Name);

            return View("InsertSummary", batchUpload);
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