
using FingertipsUploadService.Helpers;
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.ProfileData.Repositories;
using FingertipsUploadService.Upload;
using NLog;
using System.Collections.Generic;
using System.Data;

namespace FingertipsUploadService
{
    public class BatchJobWorker
    {
        private UploadJobErrorRepository _jobErrorRepository;
        private UploadJobRepository _jobRepository;
        private readonly Logger _logger = LogManager.GetLogger("BatchJobWorker");

        public void ProcessJob(UploadJob job, IWorksheetNameValidator nameValidator,
            IBatchWorksheetDataProcessor processor, IExcelFileReader excelFileReader)
        {
            _jobRepository = new UploadJobRepository();
            _jobErrorRepository = new UploadJobErrorRepository();
            var batchUpload = ToBatchUpload(job);

            _logger.Info("Job# {0} current status is {1} ", job.Guid, job.Status);

            // If user wants to override duplications 
            if (job.Status == UploadJobStatus.ConfirmationGiven)
            {
                ChangeJobStatusToInProgress(job);
                // Read indicators in datatable
                var batchDataTable = GetBatchData(excelFileReader);
                // Save the total number of rows in file
                WorkerHelper.UpdateNumberOfRowsInFile(job, batchDataTable, _jobRepository, true);
                // 
                //Perform validation once again to get the list 
                // of duplicate rows in database
                //
                processor.Validate(batchDataTable, batchUpload);
                // Archive rows
                processor.ArchiveDuplicates(batchUpload.DuplicateRowInDatabaseErrors, job);
                // Upload data to core data set
                UploadDataToCoreDataSet(job, processor, batchDataTable);
            }
            else // If we have a new job
            {
                ChangeJobStatusToInProgress(job);

                var worksheets = excelFileReader.GetWorksheets();

                UpdateJobProgress(job, ProgressStage.ValidatingWorksheets);

                if (!nameValidator.ValidateBatch(worksheets))
                {
                    job.Status = UploadJobStatus.FailedValidation;
                    _jobRepository.UpdateJob(job);

                    var error = ErrorBuilder.GetWorkSheetNameValidationError(job);
                    _jobErrorRepository.Log(error);
                    _logger.Info("Job# {0} doesn't have required worksheets", job.Guid);
                    _logger.Info("Job# {0} status changed to {1} ", job.Guid, job.Status);
                    return;
                }

                var batchDataTable = GetBatchData(excelFileReader);

                // Save the total number of rows in file
                WorkerHelper.UpdateNumberOfRowsInFile(job, batchDataTable, _jobRepository, true);

                UpdateJobProgress(job, ProgressStage.ValidatingData);

                processor.Validate(batchDataTable, batchUpload);

                var indicatorIdsInBatch = processor.GetIndicatorIdsInBatch();

                UpdateJobProgress(job, ProgressStage.CheckingPermission);

                // Check user permission for indicators
                var hasPermission = new IndicatorPermission().Check(indicatorIdsInBatch, job, _jobErrorRepository);
                if (!hasPermission)
                {
                    job.Status = UploadJobStatus.FailedValidation;
                    _jobRepository.UpdateJob(job);

                    _logger.Info("Job# {0}, User doesn't have permission for indicator", job.Guid);
                    _logger.Info("Job# {0} status changed to {1} ", job.Guid, job.Status);
                    return;
                }

                UpdateJobProgress(job, ProgressStage.CheckingDuplicationInFile);

                // Check for duplications in file
                if (batchUpload.DuplicateRowInSpreadsheetErrors.Count > 0)
                {
                    job.Status = UploadJobStatus.ConfirmationAwaited;
                    _jobRepository.UpdateJob(job);

                    var error = ErrorBuilder.GetDuplicateRowInSpreadsheetError(job.Guid,
                        batchUpload.DuplicateRowInSpreadsheetErrors);
                    _jobErrorRepository.Log(error);
                    _logger.Info("Job# {0}, There are duplicate rows in spreadsheet", job.Guid);
                    _logger.Info("Job# {0} status changed to {1} ", job.Guid, job.Status);
                    return;
                }

                UpdateJobProgress(job, ProgressStage.CheckingDuplicationInDb);

                // Check for duplications database rows
                if (batchUpload.DuplicateRowInDatabaseErrors.Count > 0)
                {
                    job.Status = UploadJobStatus.ConfirmationAwaited;
                    _jobRepository.UpdateJob(job);

                    var error = ErrorBuilder.GetDuplicateRowInDatabaseError(job.Guid,
                        batchUpload.DuplicateRowInDatabaseErrors);
                    _jobErrorRepository.Log(error);
                    _logger.Info("Job# {0}, There are duplicate rows in database", job.Guid);
                    _logger.Info("Job# {0} status changed to {1} ", job.Guid, job.Status);
                    return;
                }
                UploadDataToCoreDataSet(job, processor, batchDataTable);
            }
        }

        private void ChangeJobStatusToInProgress(UploadJob job)
        {
            job.Status = UploadJobStatus.InProgress;
            _jobRepository.UpdateJob(job);
            _logger.Info("Job# {0} status changed to {1} ", job.Guid, job.Status);
        }

        private void UpdateJobProgress(UploadJob job, ProgressStage stage)
        {
            job.ProgressStage = stage;
            _jobRepository.UpdateJob(job);
        }

        private void UploadDataToCoreDataSet(UploadJob job, IBatchWorksheetDataProcessor processor, DataTable batchDataTable)
        {
            // Upload to DB
            processor.UploadData(batchDataTable, job);
            // All good job completed
            job.Status = UploadJobStatus.SuccessfulUpload;
            _jobRepository.UpdateJob(job);

            UpdateJobProgress(job, ProgressStage.WrittingToDb);

            _logger.Info("Job# {0} successfully completed", job.Guid);
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
