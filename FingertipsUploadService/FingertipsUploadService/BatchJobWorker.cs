
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
    public class BatchJobWorker
    {
        private UploadJobErrorRepository _jobErrorRepository;
        private UploadJobRepository _jobRepository;
        private readonly Logger _logger = LogManager.GetLogger("BatchJobWorker");
        private StatusHelper jobStatus;

        public void ProcessJob(UploadJob job, IWorksheetNameValidator nameValidator,
            IBatchWorksheetDataProcessor processor, IExcelFileReader excelFileReader)
        {
            try
            {
                _jobRepository = new UploadJobRepository();
                _jobErrorRepository = new UploadJobErrorRepository();
                var batchUpload = ToBatchUpload(job);
                jobStatus = new StatusHelper(_jobRepository, _logger);

                _logger.Info("Job# {0} current status is {1} ", job.Guid, job.Status);

                // If user wants to override duplications 
                if (job.Status == UploadJobStatus.ConfirmationGiven)
                {
                    jobStatus.InProgress(job);
                    // Read indicators in datatable
                    var batchDataTable = GetBatchData(excelFileReader);
                    // Save the total number of rows in file
                    WorkerHelper.UpdateNumberOfRowsInFile(job, batchDataTable, _jobRepository, true);
                    // 
                    //Perform validation once again to get the list 
                    // of duplicate rows in database
                    //
                    processor.Validate(batchDataTable, batchUpload);
                    // Remove duplications in file
                    CheckDuplicateRowsInWorksheet(job, batchUpload, ref batchDataTable);
                    // Archive rows
                    processor.ArchiveDuplicates(batchUpload.DuplicateRowInDatabaseErrors, job);
                    // Upload data to core data set
                    UploadDataToCoreDataSet(job, processor, batchDataTable);
                }
                else // If we have a new job
                {
                    jobStatus.InProgress(job);

                    // Get worksheets from file
                    var worksheets = excelFileReader.GetWorksheets();

                    UpdateJobProgress(job, ProgressStage.ValidatingWorksheets);

                    // Check worksheet names are correct
                    var worksheetsOk = CheckWorksheets(job, nameValidator, worksheets);
                    if (!worksheetsOk) return;


                    var batchDataTable = GetBatchData(excelFileReader);

                    // Save the total number of rows in file
                    WorkerHelper.UpdateNumberOfRowsInFile(job, batchDataTable, _jobRepository, true);

                    UpdateJobProgress(job, ProgressStage.ValidatingData);

                    processor.Validate(batchDataTable, batchUpload);

                    var indicatorIdsInBatch = processor.GetIndicatorIdsInBatch();

                    UpdateJobProgress(job, ProgressStage.CheckingPermission);

                    // Check user permission for indicators
                    var permissionsOk = CheckPermission(job, indicatorIdsInBatch);
                    if (!permissionsOk) return;

                    UpdateJobProgress(job, ProgressStage.CheckingDuplicationInFile);

                    // Check for duplications in file
                    CheckDuplicateRowsInWorksheet(job, batchUpload, ref batchDataTable);

                    // Check validation errors
                    var validationOk = CheckValidationFailures(job, batchUpload);
                    if (!validationOk) return;

                    UpdateJobProgress(job, ProgressStage.CheckingDuplicationInDb);

                    // Check for duplications database rows
                    var haveDuplicates = CheckDuplicateRowsInDatabase(job, batchUpload);
                    if (haveDuplicates) return;

                    UploadDataToCoreDataSet(job, processor, batchDataTable);
                }
            }
            catch (Exception ex)
            {
                jobStatus.UnexpectedError(job);
                _logger.Error(ex);
            }
        }

        private bool CheckDuplicateRowsInDatabase(UploadJob job, BatchUpload batchUpload)
        {
            if (batchUpload.DuplicateRowInDatabaseErrors.Count <= 0) return false;

            jobStatus.ConfirmationAwaited(job);

            var error = ErrorBuilder.GetDuplicateRowInDatabaseError(job.Guid,
                batchUpload.DuplicateRowInDatabaseErrors);
            _jobErrorRepository.Log(error);
            _logger.Info("Job# {0}, There are duplicate rows in database", job.Guid);

            return true;
        }

        private bool CheckValidationFailures(UploadJob job, BatchUpload batchUpload)
        {
            if (batchUpload.UploadValidationFailures.Count <= 0) return true;

            jobStatus.FailedValidation(job);

            var error = ErrorBuilder.GetConversionError(job.Guid, batchUpload.UploadValidationFailures);
            _jobErrorRepository.Log(error);
            _logger.Info("Job# {0}, Data type conversion errors occurred ", job.Guid);
            return false;
        }

        private void CheckDuplicateRowsInWorksheet(UploadJob job, BatchUpload batchUpload, ref DataTable batchDataTable)
        {
            if (batchUpload.DuplicateRowInSpreadsheetErrors.Count > 0)
            {
                var dataWithoutDuplicates = new FileDuplicationHandler().RemoveDuplicatesInBatch(batchDataTable);
                batchDataTable = dataWithoutDuplicates;
                _logger.Info("Job# {0}, There are duplicate rows in spreadsheet", job.Guid);
                _logger.Info("Job# {0}, Dupllicate rows removed", job.Guid);
            }
        }

        private bool CheckPermission(UploadJob job, List<int> indicatorIdsInBatch)
        {
            var hasPermission = new IndicatorPermission().Check(indicatorIdsInBatch, job, _jobErrorRepository);
            if (hasPermission) return true;
            jobStatus.FailedValidation(job);

            _logger.Info("Job# {0}, User doesn't have permission for indicator", job.Guid);
            return false;
        }

        private bool CheckWorksheets(UploadJob job, IWorksheetNameValidator nameValidator, List<string> worksheets)
        {
            if (nameValidator.ValidateBatch(worksheets)) return true;

            jobStatus.FailedValidation(job);

            var error = ErrorBuilder.GetWorkSheetNameValidationError(job);
            _jobErrorRepository.Log(error);
            _logger.Info("Job# {0} doesn't have required worksheets", job.Guid);
            _logger.Info("Job# {0} status changed to {1} ", job.Guid, job.Status);
            return false;
        }

        private void UpdateJobProgress(UploadJob job, ProgressStage stage)
        {
            job.ProgressStage = stage;
            _jobRepository.UpdateJob(job);
        }

        private void UploadDataToCoreDataSet(UploadJob job, IBatchWorksheetDataProcessor processor, DataTable batchDataTable)
        {
            // Upload to DB
            var rowsUploaded = processor.UploadData(batchDataTable, job).DataToUpload.Count;
            // All good job completed
            jobStatus.SuccessfulUpload(job, rowsUploaded);
            UpdateJobProgress(job, ProgressStage.WrittingToDb);
        }

        private static DataTable GetBatchData(IExcelFileReader excelFileReader)
        {
            // Get batch indicator data from worksheet as data table 
            DataTable batchDataTable = excelFileReader.GetBatchData();

            // Delete non-valid rows
            foreach (var row in batchDataTable.Select("IndicatorId is null"))
            {
                row.Delete();
            }

            batchDataTable.AcceptChanges();
            return batchDataTable;
        }

        private BatchUpload ToBatchUpload(UploadJob job)
        {
            var batchUpload = new BatchUpload
            {
                ShortFileName = job.Filename,
                FileName = job.Filename,
                DataToUpload = new List<UploadDataModel>(),
                DuplicateRowInDatabaseErrors = new List<DuplicateRowInDatabaseError>(),
                DuplicateRowInSpreadsheetErrors = new List<DuplicateRowInSpreadsheetError>(),
                ExcelDataSheets = new List<UploadExcelSheet>(),
                UploadValidationFailures = new List<UploadValidationFailure>()
            };
            return batchUpload;
        }
    }
}
