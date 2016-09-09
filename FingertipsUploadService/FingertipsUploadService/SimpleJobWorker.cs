using FingertipsUploadService.Helpers;
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.ProfileData.Repositories;
using FingertipsUploadService.Upload;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;

namespace FingertipsUploadService
{
    public class SimpleJobWorker
    {
        private readonly Logger _logger = LogManager.GetLogger("SimpleJobWorker");
        private UploadJobErrorRepository _jobErrorRepository;
        private UploadJobRepository _jobRepository;
        private StatusHelper jobStatus;

        public void ProcessJob(UploadJob job, IWorksheetNameValidator nameValidator,
            ISimpleWorksheetDataProcessor processor, IExcelFileReader excelFileReader)
        {
            try
            {
                _jobRepository = new UploadJobRepository();
                _jobErrorRepository = new UploadJobErrorRepository();
                // Create SimpleUpload object from job
                var simpleUpload = ToSimpleUpload(job);
                jobStatus = new StatusHelper(_jobRepository, _logger);
                _logger.Info("Job# {0} current status is {1} ", job.Guid, job.Status);

                if (job.Status == UploadJobStatus.ConfirmationGiven)
                {
                    jobStatus.InProgress(job);
                    // Get indicator details worksheet as data table
                    var indicatorDetails = excelFileReader.GetIndicatorDetails();
                    // Get pholio data worksheet as data table
                    var pholioData = excelFileReader.GetPholioData();
                    // Save the total number of rows in file
                    WorkerHelper.UpdateNumberOfRowsInFile(job, pholioData, _jobRepository, true);
                    // Validate the Data            
                    processor.Validate(indicatorDetails, pholioData, simpleUpload);
                    // Remove duplicate rows
                    CheckDuplicateRowsInWorksheet(job, simpleUpload, ref pholioData);
                    // Archive rows
                    processor.ArchiveDuplicates(simpleUpload.DuplicateRowInDatabaseErrors, job);
                    // Upload data to core data set
                    UploadDataToCoreDataSet(job, processor, indicatorDetails, pholioData);
                }
                else
                {
                    // Update the job status to in progress            
                    jobStatus.InProgress(job);
                    // Get worksheets from file
                    var worksheets = excelFileReader.GetWorksheets();
                    // Check worksheet names are correct
                    var worksheetsOk = CheckWorksheets(job, worksheets, nameValidator);
                    if (!worksheetsOk) return;
                    //  Get indicator details worksheet as data table
                    var indicatorDetails = excelFileReader.GetIndicatorDetails();
                    // Get pholio data worksheet as data table
                    var pholioData = excelFileReader.GetPholioData();
                    // Save the total number of rows in file
                    WorkerHelper.UpdateNumberOfRowsInFile(job, pholioData, _jobRepository, true);
                    // Validate the Data            
                    processor.Validate(indicatorDetails, pholioData, simpleUpload);

                    // Check user permission for indicator
                    var indicatorIds = new List<int> { simpleUpload.IndicatorId };
                    var permissionsOk = CheckPermission(indicatorIds, job);
                    if (!permissionsOk) return;

                    // Check for duplications in file, if there will be any duplicate rows
                    // we will remove them.
                    CheckDuplicateRowsInWorksheet(job, simpleUpload, ref pholioData);

                    // Check validation errors
                    var validationOk = CheckValidationFailures(job, simpleUpload);
                    if (!validationOk) return;

                    // Check for duplications database rows
                    var haveDuplicates = CheckDuplicateRowsInDatabase(job, simpleUpload);
                    if (haveDuplicates) return;

                    // Upload to DB
                    UploadDataToCoreDataSet(job, processor, indicatorDetails, pholioData);
                }
            }
            catch (Exception ex)
            {
                jobStatus.UnexpectedError(job);
                _logger.Error(ex);
            }
        }

        private bool CheckDuplicateRowsInDatabase(UploadJob job, SimpleUpload simpleUpload)
        {
            if (simpleUpload.DuplicateRowInDatabaseErrors.Count <= 0) return false;

            jobStatus.ConfirmationAwaited(job);

            var error = ErrorBuilder.GetDuplicateRowInDatabaseError(job.Guid,
                simpleUpload.DuplicateRowInDatabaseErrors);
            _jobErrorRepository.Log(error);
            _logger.Info("Job# {0}, There are duplicate rows in database", job.Guid);
            return true;
        }

        private bool CheckValidationFailures(UploadJob job, SimpleUpload simpleUpload)
        {
            if (simpleUpload.UploadValidationFailures.Count <= 0) return true;

            jobStatus.FailedValidation(job);

            var error = ErrorBuilder.GetConversionError(job.Guid, simpleUpload.UploadValidationFailures);
            _jobErrorRepository.Log(error);
            _logger.Info("Job# {0}, Data type conversion errors occurred ", job.Guid);
            return false;
        }

        private void CheckDuplicateRowsInWorksheet(UploadJob job, SimpleUpload simpleUpload, ref DataTable pholioData)
        {
            if (simpleUpload.DuplicateRowInSpreadsheetErrors.Count > 0)
            {
                var dataWithoutDuplicates = new FileDuplicationHandler().RemoveDuplicatesInSimple(pholioData);
                pholioData = dataWithoutDuplicates;
                _logger.Info("Job# {0}, There are duplicate rows in spreadsheet", job.Guid);
                _logger.Info("Job# {0}, Dupllicate rows removed", job.Guid);
            }
        }

        private SimpleUpload ToSimpleUpload(UploadJob job)
        {
            var simpleUpload = new SimpleUpload
            {
                ShortFileName = job.Filename,
                FileName = job.Filename,
                DataToUpload = new List<UploadDataModel>(),
                DuplicateRowInDatabaseErrors = new List<DuplicateRowInDatabaseError>(),
                DuplicateRowInSpreadsheetErrors = new List<DuplicateRowInSpreadsheetError>(),
                ExcelDataSheets = new List<UploadExcelSheet>(),
                UploadValidationFailures = new List<UploadValidationFailure>()
            };
            return simpleUpload;
        }

        private void UploadDataToCoreDataSet(UploadJob job, ISimpleWorksheetDataProcessor processor,
            DataTable indicatorDetails, DataTable pholioData)
        {
            // Upload to DB
            var rowsUploaded = processor.UploadData(indicatorDetails, pholioData, job).DataToUpload.Count;
            // All good job done
            jobStatus.SuccessfulUpload(job, rowsUploaded);
        }



        private bool CheckWorksheets(UploadJob job, List<string> worksheets, IWorksheetNameValidator nameValidator)
        {
            if (nameValidator.ValidateSimple(worksheets)) return true;

            jobStatus.FailedValidation(job);

            var error = ErrorBuilder.GetWorkSheetNameValidationError(job);
            _jobErrorRepository.Log(error);
            _logger.Info("Job# {0} doesn't have required worksheets", job.Guid);
            _logger.Info("Job# {0} status changed to {1} ", job.Guid, job.Status);
            return false;
        }


        private bool CheckPermission(List<int> indicatorIds, UploadJob job)
        {
            var hasPermission = new IndicatorPermission().Check(indicatorIds, job, _jobErrorRepository);

            if (hasPermission) return true;

            jobStatus.FailedValidation(job);

            _logger.Info("Job# {0}, User doesn't have permission for indicator", job.Guid);
            return false;
        }

    }
}