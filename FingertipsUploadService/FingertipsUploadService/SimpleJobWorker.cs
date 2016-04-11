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
    public class SimpleJobWorker
    {
        private UploadJobErrorRepository _jobErrorRepository;
        private UploadJobRepository _jobRepository;
        private readonly Logger _logger = LogManager.GetLogger("SimpleJobWorker");

        public void ProcessJob(UploadJob job, IWorksheetNameValidator nameValidator,
            ISimpleWorksheetDataProcessor processor, IExcelFileReader excelFileReader)
        {
            _jobRepository = new UploadJobRepository();
            _jobErrorRepository = new UploadJobErrorRepository();
            // Create SimpleUpload object from job
            var simpleUpload = ToSimpleUpload(job);

            _logger.Info("Job# {0} current status is {1} ", job.Guid, job.Status);

            if (job.Status == UploadJobStatus.ConfirmationGiven)
            {
                ChangeJobStatusToInProgress(job);
                // Get indicator details worksheet as data table
                var indicatorDetails = excelFileReader.GetIndicatorDetails();
                // Get pholio data worksheet as data table
                var pholioData = excelFileReader.GetPholioData();
                // Save the total number of rows in file
                WorkerHelper.UpdateNumberOfRowsInFile(job, pholioData, _jobRepository, true);
                // Validate the Data            
                processor.Validate(indicatorDetails, pholioData, simpleUpload);
                // Archive rows
                processor.ArchiveDuplicates(simpleUpload.DuplicateRowInDatabaseErrors, job);
                // Upload data to core data set
                UploadDataToCoreDataSet(job, processor, indicatorDetails, pholioData);
            }
            else
            {
                // Update the job status to in progress            
                ChangeJobStatusToInProgress(job);

                // Validate the file   
                var worksheets = excelFileReader.GetWorksheets();

                if (!nameValidator.ValidateSimple(worksheets))
                {
                    job.Status = UploadJobStatus.FailedValidation;
                    _jobRepository.UpdateJob(job);

                    var error = ErrorBuilder.GetWorkSheetNameValidationError(job);
                    _jobErrorRepository.Log(error);
                    _logger.Info("Job# {0} doesn't have required worksheets", job.Guid);
                    _logger.Info("Job# {0} status changed to {1} ", job.Guid, job.Status);
                    return;
                }

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
                var hasPermission = new IndicatorPermission().Check(indicatorIds, job, _jobErrorRepository);

                if (!hasPermission)
                {
                    job.Status = UploadJobStatus.FailedValidation;
                    _jobRepository.UpdateJob(job);

                    _logger.Info("Job# {0}, User doesn't have permission for indicator", job.Guid);
                    _logger.Info("Job# {0} status changed to {1} ", job.Guid, job.Status);
                    return;
                }

                // Check for duplications in file
                if (simpleUpload.DuplicateRowInSpreadsheetErrors.Count > 0)
                {
                    job.Status = UploadJobStatus.ConfirmationAwaited;
                    _jobRepository.UpdateJob(job);

                    var error = ErrorBuilder.GetDuplicateRowInSpreadsheetError(job.Guid,
                        simpleUpload.DuplicateRowInSpreadsheetErrors);
                    _jobErrorRepository.Log(error);
                    _logger.Info("Job# {0}, There are duplicate rows in spreadsheet", job.Guid);
                    _logger.Info("Job# {0} status changed to {1} ", job.Guid, job.Status);
                    return;
                }

                // Check for duplications database rows
                if (simpleUpload.DuplicateRowInDatabaseErrors.Count > 0)
                {
                    job.Status = UploadJobStatus.ConfirmationAwaited;
                    _jobRepository.UpdateJob(job);

                    var error = ErrorBuilder.GetDuplicateRowInDatabaseError(job.Guid,
                        simpleUpload.DuplicateRowInDatabaseErrors);
                    _jobErrorRepository.Log(error);
                    _logger.Info("Job# {0}, There are duplicate rows in database", job.Guid);
                    _logger.Info("Job# {0} status changed to {1} ", job.Guid, job.Status);
                    return;
                }

                // Upload to DB
                UploadDataToCoreDataSet(job, processor, indicatorDetails, pholioData);
            }
        }

        private void SaveNumberOfRowsInFile(UploadJob job, DataTable pholioData)
        {
            job.TotalRows = pholioData.Rows.Count;
            _jobRepository.SaveJob(job);
        }

        private void ChangeJobStatusToInProgress(UploadJob job)
        {
            job.Status = UploadJobStatus.InProgress;
            _jobRepository.UpdateJob(job);
            _logger.Info("Job# {0} status changed to {1} ", job.Guid, job.Status);
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
            processor.UploadData(indicatorDetails, pholioData, job);
            // All good job done
            job.Status = UploadJobStatus.SuccessfulUpload;
            _jobRepository.UpdateJob(job);
            _logger.Info("Job# {0} successfully completed", job.Guid);
        }
    }
}