
using FingertipsUploadService.FpmFileReader;
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
        private StatusHelper _jobStatus;

        public void ProcessJob(UploadJob job, IWorksheetNameValidator nameValidator,
            IBatchWorksheetDataProcessor processor, IUploadFileReader fileReader)
        {
            try
            {
                _jobRepository = new UploadJobRepository();
                _jobErrorRepository = new UploadJobErrorRepository();
                var batchUpload = ToBatchUpload(job);
                _jobStatus = new StatusHelper(_jobRepository, _logger);


                _logger.Info("Job ID {0} current status is {1} ", job.Guid, job.Status);

                var originalStatus = job.Status;
                _jobStatus.InProgress(job);

                // Initial check file is OK
                if (originalStatus == UploadJobStatus.NotStarted)
                {
                    if (IsWorksheetNameValid(job, nameValidator, fileReader) == false) return;
                }

                // Parse table
                var batchDataTable = GetBatchData(fileReader);

                // Check the column names
                if (ValidateColumnNames(job, batchUpload, batchDataTable) == false) return;
                // Sanitize the data
                SanitizeDataTable(fileReader, batchDataTable);

                // Save the total number of rows in file
                WorkerHelper.UpdateNumberOfRowsInFile(job, batchDataTable, _jobRepository);


                // Do all validation
                UpdateJobProgress(job, ProgressStage.ValidatingData);
                processor.Validate(batchDataTable, batchUpload);

                // If we have a new job
                if (originalStatus == UploadJobStatus.NotStarted)
                {
                    if (DoesUserHavePermissionForAllIndicators(job, processor) == false) return;
                    if (AreAnySmallNumbers(job, batchUpload)) return;
                    if (AreAnyValidationFailures(job, batchUpload)) return;
                }

                // Duplicate rows have not been accepted
                if (originalStatus != UploadJobStatus.ConfirmationGiven)
                {
                    _jobStatus.InProgress(job);
                    if (AreAnyDuplicateRowsInDatabase(job, batchUpload)) return;
                }

                // Remove duplications in file
                CheckDuplicateRowsInWorksheet(job, batchUpload, ref batchDataTable);

                // Archive rows
                processor.ArchiveDuplicates(batchUpload.DuplicateRowInDatabaseErrors, job);

                // Upload data to core data set
                UploadDataToCoreDataSet(job, processor, batchDataTable);
            }
            catch (Exception ex)
            {
                _jobStatus.UnexpectedError(job);
                _logger.Error(ex);
            }
        }


        /// <summary>
        /// Replace missing values with defaults for Excel file only. This is done on parsing for CSV file.
        /// </summary>
        private static void SanitizeDataTable(IUploadFileReader fileReader, DataTable batchDataTable)
        {
            if (fileReader is ExcelFileReader)
            {
                new DataSanitizer().SanitizeExcelData(batchDataTable);
            }
        }

        private bool AreAnyDuplicateRowsInDatabase(UploadJob job, BatchUpload batchUpload)
        {
            UpdateJobProgress(job, ProgressStage.CheckingDuplicationInDb);

            if (batchUpload.DuplicateRowInDatabaseErrors.Count <= 0) return false;

            _jobStatus.ConfirmationAwaited(job);

            var error = ErrorBuilder.GetDuplicateRowInDatabaseError(job.Guid,
                batchUpload.DuplicateRowInDatabaseErrors);
            _jobErrorRepository.Log(error);
            _logger.Info("Job ID {0}, There are duplicate rows in database", job.Guid);

            return true;
        }

        private bool AreAnySmallNumbers(UploadJob job, BatchUpload batchUpload)
        {
            if (batchUpload.SmallNumberWarnings.Count <= 0) return false;

            _jobStatus.SmallNumbersFound(job);
            var warning = ErrorBuilder.GetSmallNumberWarning(job, batchUpload.SmallNumberWarnings);
            _jobErrorRepository.Log(warning);
            _logger.Info("Job ID {0}, Small number cound in row", job.Guid);

            return true;
        }

        private bool AreAnyValidationFailures(UploadJob job, BatchUpload batchUpload)
        {
            if (batchUpload.UploadValidationFailures.Count <= 0) return false;

            _jobStatus.FailedValidation(job);

            var error = ErrorBuilder.GetConversionError(job.Guid, batchUpload.UploadValidationFailures);
            _jobErrorRepository.Log(error);
            _logger.Info("Job ID {0}, Data type conversion errors occurred ", job.Guid);
            return true;
        }

        private void CheckDuplicateRowsInWorksheet(UploadJob job, BatchUpload batchUpload, ref DataTable batchDataTable)
        {
            if (batchUpload.DuplicateRowInSpreadsheetErrors.Count > 0)
            {
                var dataWithoutDuplicates = new FileDuplicationHandler().RemoveDuplicatesInBatch(batchDataTable);
                batchDataTable = dataWithoutDuplicates;
                _logger.Info("Job ID {0}, There are duplicate rows in spreadsheet", job.Guid);
                _logger.Info("Job ID {0}, Dupllicate rows removed", job.Guid);
            }
        }

        private bool DoesUserHavePermissionForAllIndicators(UploadJob job, IBatchWorksheetDataProcessor processor)
        {
            var indicatorIdsInBatch = processor.GetIndicatorIdsInBatch();
            UpdateJobProgress(job, ProgressStage.CheckingPermission);
            var hasPermission = new IndicatorPermission().Check(indicatorIdsInBatch, job, _jobErrorRepository);
            if (hasPermission) return true;
            _jobStatus.FailedValidation(job);

            _logger.Info("Job ID {0}, User doesn't have permission for indicator", job.Guid);
            return false;
        }

        private bool IsWorksheetNameValid(UploadJob job, IWorksheetNameValidator nameValidator, IUploadFileReader fileReader)
        {
            var worksheets = fileReader.GetWorksheets();
            UpdateJobProgress(job, ProgressStage.ValidatingWorksheets);

            if (nameValidator.ValidateBatch(worksheets)) return true;

            _jobStatus.FailedValidation(job);

            var error = ErrorBuilder.GetWorkSheetNameValidationError(job);
            _jobErrorRepository.Log(error);
            _logger.Info("Job ID {0} doesn't have required worksheets", job.Guid);
            _logger.Info("Job ID {0} status changed to {1} ", job.Guid, job.Status);
            return false;
        }


        private bool ValidateColumnNames(UploadJob job, BatchUpload batchUpload, DataTable batchDataTable)
        {
            // Createm empty datatable 
            var areColumnNamesValid = true;
            var emptyCorrectDataTable = new UploadDataSchema().CreateEmptyTable();
            var wrongColumns = new List<string>();
            for (var i = 0; i < emptyCorrectDataTable.Columns.Count; i++)
            {
                if (batchDataTable.Columns[i].ColumnName.ToLower() !=
                    emptyCorrectDataTable.Columns[i].ColumnName.ToLower())
                {
                    _jobStatus.ColumnNameValidationFailed(job);
                    var wrongColumnName = batchDataTable.Columns[i].ColumnName;
                    wrongColumns.Add("Invalid column name " + wrongColumnName);
                    _logger.Error("{0} Column does not exist or column order is wrong", wrongColumnName);
                }
            }

            if (wrongColumns.Count > 0)
            {
                var error = ErrorBuilder.GetColumnNameValidatoinError(job, wrongColumns);
                _jobErrorRepository.Log(error);
                areColumnNamesValid = false;
            }
            return areColumnNamesValid;
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
            _jobStatus.SuccessfulUpload(job, rowsUploaded);
            UpdateJobProgress(job, ProgressStage.WritingToDb);
        }

        private static DataTable GetBatchData(IUploadFileReader excelFileReader)
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
                UploadValidationFailures = new List<UploadValidationFailure>(),
                ColumnErrors = new List<string>()
            };
            return batchUpload;
        }
    }
}
