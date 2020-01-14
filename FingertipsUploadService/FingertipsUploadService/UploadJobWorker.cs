
using FingertipsUploadService.FpmFileReader;
using FingertipsUploadService.Helpers;
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.ProfileData.Entities.JobError;
using FingertipsUploadService.ProfileData.Repositories;
using FingertipsUploadService.Upload;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FingertipsUploadService
{
    public class UploadJobWorker
    {
        private UploadJobErrorRepository _jobErrorRepository;
        private UploadJobRepository _jobRepository;
        private readonly Logger _logger = LogManager.GetLogger("BatchJobWorker");
        private StatusHelper _jobStatus;

        public void ProcessJob(UploadJob job, IWorksheetNameValidator nameValidator,
            IDataValidator validator, IUploadFileReader fileReader, IDataTableUploader uploader)
        {
            try
            {
                _jobRepository = new UploadJobRepository();
                _jobErrorRepository = new UploadJobErrorRepository();
                var jobAnalysis = new UploadJobAnalysis(job.Filename);
                _jobStatus = new StatusHelper(_jobRepository, _logger);

                _logger.Info("Job ID {0} current status is {1} ", job.Guid, job.Status);

                var originalStatus = job.Status;
                _jobStatus.InProgress(job);

                // Initial check file is OK
                if (originalStatus == UploadJobStatus.NotStarted)
                {
                    if (IsWorksheetNameValid(job, nameValidator, fileReader) == false) return;
                }

                // Read table
                _logger.Debug("Reading the file into DataTable");
                var dataTable = fileReader.ReadData();

                // Clean data table
                _logger.Debug("Changing the column names");
                UploadColumnNames.ChangeAllColumnNamesToLowerCase(dataTable);
                UploadColumnNames.ChangeDeprecatedColumnNames(dataTable);
                _logger.Debug("Removing the empty rows");
                RemoveEmptyRows(dataTable);

                // Check expected columns are present
                _logger.Debug("Checking if the correct columns are present");
                if (CheckExpectedColumnsArePresent(job, dataTable) == false) return;

                new DefaultValueSetter().ReplaceNullsWithDefaultValues(dataTable);

                // Save the total number of rows in file
                _logger.Debug("Updating the number or rows");
                WorkerHelper.UpdateNumberOfRowsInFile(job, dataTable, _jobRepository);

                // Do all validation and parsing
                UpdateJobProgress(job, ProgressStage.ValidatingData);

                // Check if the file is valid
                _logger.Debug("Validating the rows");
                validator.ValidateRows(dataTable, jobAnalysis, job.IsCellValidationPerRowDone);


                if (!job.IsCellValidationPerRowDone)
                {
                    job.IsCellValidationPerRowDone = true;
                    UpdateJobProgress(job, ProgressStage.ValidationIsDone);
                }

                // Check the small numbers
                if (!job.IsSmallNumberOverrideApplied)
                {
                    //_logger.Debug("Checking the small number");
                    validator.CheckSmallNumbers(dataTable, jobAnalysis);
                    job.IsSmallNumberOverrideApplied = true;
                    UpdateJobProgress(job, ProgressStage.SmallNumberCheckIsDone);
                }

                // Check duplicates in the file
                _logger.Debug("Checking duplicates in file");
                validator.CheckDuplicatesInFile(jobAnalysis);

                // Check duplicate in database
                _logger.Debug("Checking duplicates in db");
                validator.CheckGetDuplicatesInDb(dataTable, jobAnalysis);

                // If we have a new job
                if (originalStatus == UploadJobStatus.NotStarted)
                {
                    _logger.Debug("Getting indicator Ids");
                    var indicatorIds = GetIndicatorIds(jobAnalysis.DataToUpload);
                    _logger.Debug("Checking the permissions for all the indicators in the files");
                    if (DoesUserHavePermissionForAllIndicators(job, indicatorIds) == false) return;

                    _logger.Debug("Checking for the validation errors");
                    if (AreAnyValidationFailures(job, jobAnalysis)) return;

                    // This check must be done last as the user can override it if they choose
                    _logger.Debug("Checking for the small numbers");
                    if (AreAnySmallNumbers(job, jobAnalysis)) return;
                }

                _jobStatus.InProgress(job);

                // Duplicate rows have not been accepted
                _logger.Debug("Checking for the duplicate rows");
                if (originalStatus != UploadJobStatus.ConfirmationGiven && job.IsConfirmationRequiredToOverrideDatabaseDuplicates)
                {
                    if (AreAnyDuplicateRowsInDatabase(job, jobAnalysis)) return;
                }

                UpdateJobProgress(job, ProgressStage.WritingToDb);

                // Remove duplicate data 
                _logger.Debug("Removing the duplicate rows");
                RemoveDuplicateRows(job, jobAnalysis);

                // Archive data
                _logger.Debug("Archiving the duplicates");
                uploader.ArchiveDuplicates(jobAnalysis.DuplicateRowInDatabaseErrors, job);

                // Upload data
                _logger.Debug("Uploading to the CoreDataSet");
                UploadDataToCoreDataSet(job, jobAnalysis, uploader);
            }
            catch (Exception ex)
            {
                LogUnexpectedError(job, ex);
            }
        }


        private void LogUnexpectedError(UploadJob job, Exception ex)
        {
            var error = new UploadJobError
            {
                JobGuid = job.Guid,
                ErrorType = UploadJobErrorType.UnexpectedError,
                ErrorText = ex.Message,
                ErrorJson = ""
            };
            _jobErrorRepository.Log(error);
            _jobStatus.UnexpectedError(job);
            _logger.Error(ex);
        }

        /// <summary>
        /// Get a distinct list of the IDs of all the indicators in the data table.
        /// </summary>
        private IList<int> GetIndicatorIds(IList<UploadDataModel> dataList)
        {
            return dataList
                .Select(x => x.IndicatorId)
                .Distinct()
                .ToList();
        }

        private bool AreAnyDuplicateRowsInDatabase(UploadJob job, UploadJobAnalysis uploadJobAnalysis)
        {
            UpdateJobProgress(job, ProgressStage.CheckingDuplicationInDb);

            if (uploadJobAnalysis.DuplicateRowInDatabaseErrors.Count <= 0) return false;

            _jobStatus.ConfirmationAwaited(job);

            var error = ErrorBuilder.GetDuplicateRowInDatabaseError(job.Guid,
                uploadJobAnalysis.DuplicateRowInDatabaseErrors);
            _jobErrorRepository.Log(error);
            _logger.Info("Job ID {0}, There are duplicate rows in database", job.Guid);

            return true;
        }

        private bool AreAnySmallNumbers(UploadJob job, UploadJobAnalysis uploadJobAnalysis)
        {
            if (uploadJobAnalysis.SmallNumberWarnings.Count <= 0) return false;

            _jobStatus.SmallNumbersFound(job);
            var warning = ErrorBuilder.GetSmallNumberWarning(job, uploadJobAnalysis.SmallNumberWarnings);
            _jobErrorRepository.Log(warning);
            _logger.Info("Job ID {0}, Small number found in row", job.Guid);

            return true;
        }

        private bool AreAnyValidationFailures(UploadJob job, UploadJobAnalysis uploadJobAnalysis)
        {
            if (uploadJobAnalysis.UploadValidationFailures.Count <= 0) return false;

            _jobStatus.FailedValidation(job);

            var error = ErrorBuilder.GetConversionError(job.Guid, uploadJobAnalysis.UploadValidationFailures);
            _jobErrorRepository.Log(error);
            _logger.Info("Job ID {0}, Data type conversion errors occurred ", job.Guid);
            return true;
        }

        private void RemoveDuplicateRows(UploadJob job, UploadJobAnalysis uploadJobAnalysis)
        {
            if (uploadJobAnalysis.DuplicateRowInSpreadsheetErrors.Count > 0)
            {
                // Remove the duplicates
                uploadJobAnalysis.DataToUpload = new DuplicateDataFilter()
                    .RemoveDuplicateData(uploadJobAnalysis.DataToUpload);

                _logger.Info("Job ID {0}, There are duplicate rows in spreadsheet", job.Guid);
                _logger.Info("Job ID {0}, Duplicate rows removed", job.Guid);
            }
        }

        private bool DoesUserHavePermissionForAllIndicators(UploadJob job, IList<int> indicatorIds)
        {
            UpdateJobProgress(job, ProgressStage.CheckingPermission);

            var doesUserHasPermission = new IndicatorPermission()
                .DoesUserHasPermissionForAllIndicators(indicatorIds, job, _jobErrorRepository);

            if (doesUserHasPermission == false)
            {
                _jobStatus.FailedValidation(job);
                _logger.Info("Job ID {0}, User doesn't have permission for indicator", job.Guid);
            }

            return doesUserHasPermission;
        }

        private bool IsWorksheetNameValid(UploadJob job, IWorksheetNameValidator nameValidator, IUploadFileReader fileReader)
        {
            _logger.Debug("Validating the worksheet name");
            var worksheets = fileReader.GetWorksheets();
            UpdateJobProgress(job, ProgressStage.ValidatingWorksheets);

            if (nameValidator.Validate(worksheets)) return true;

            _jobStatus.FailedValidation(job);

            var error = ErrorBuilder.GetWorkSheetNameValidationError(job);
            _jobErrorRepository.Log(error);
            _logger.Info("Job ID {0} doesn't have required worksheets", job.Guid);
            _logger.Info("Job ID {0} status changed to {1} ", job.Guid, job.Status);
            return false;
        }

        private bool CheckExpectedColumnsArePresent(UploadJob job, DataTable dataTable)
        {
            // Createm empty datatable 
            var areColumnNamesValid = true;
            var columnNames = UploadColumnNames.GetColumnNames();
            var optionalColumnNames = UploadColumnNames.GetNamesOfOptionalColumns();

            var wrongColumns = new List<string>();
            foreach (var columnName in columnNames)
            {
                if (dataTable.Columns.Contains(columnName) == false)
                {
                    if (optionalColumnNames.Contains(columnName))
                    {
                        // Add column with default values
                        var newColumn = new DataColumn(columnName, typeof(double)) { DefaultValue = -1 };
                        dataTable.Columns.Add(newColumn);
                    }
                    else
                    {
                        // Missing column
                        _jobStatus.ColumnNameValidationFailed(job);
                        wrongColumns.Add("Missing column '" + columnName + ",");
                        _logger.Error("'{0}' column does not exist ", columnName);
                    }
                }
            }

            if (wrongColumns.Any())
            {
                var error = ErrorBuilder.GetColumnNameValidationError(job, wrongColumns);
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

        private void UploadDataToCoreDataSet(UploadJob job, UploadJobAnalysis jobAnalysis,
            IDataTableUploader uploader)
        {
            // Upload to DB
            uploader.UploadData(job, jobAnalysis);

            // Job completed
            var rowsUploaded = jobAnalysis.DataToUpload.Count;
            _jobStatus.SuccessfulUpload(job, rowsUploaded);
            UpdateJobProgress(job, ProgressStage.WritingToDb);
        }

        private void RemoveEmptyRows(DataTable dataTable)
        {
            var emptyRows = dataTable.Select(UploadColumnNames.IndicatorId + " is null");
            foreach (var row in emptyRows)
            {
                row.Delete();
            }

            dataTable.AcceptChanges();
        }
    }
}
