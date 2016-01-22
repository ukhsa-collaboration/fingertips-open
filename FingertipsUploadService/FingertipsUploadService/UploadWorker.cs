
using FingertipsUploadService.Entities.Job;
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Repositories;
using Fpm.Upload;
using System.Collections.Generic;

namespace FingertipsUploadService
{
    public class UploadWorker
    {
        private CoreDataRepository _coreDataRepository;
        private LoggingRepository _loggingRepository;
        public UploadWorker(CoreDataRepository coreDataRepository, LoggingRepository loggingRepository)
        {
            _coreDataRepository = coreDataRepository;
            _loggingRepository = loggingRepository;
        }
        public SimpleUpload ProcessJob(UploadJob job)
        {
            UpdateUploadJobStatus(job);

            var simpleUpload = PopulateSimpleUpload(job);
            // Validate the file
            var validator = new SimpleUploadValidator(simpleUpload, _coreDataRepository, _loggingRepository);
            var errors = validator.Validate().UploadValidationFailures;
            // Check if any errors logged during validation
            if (errors.Count > 0)
            {
                job.Status = UploadJobStatus.FailedValidation;
                return null;
            }
            else
            {

                // Validate the Data
                var worksheetValidator = new SimpleWorkSheetProcessor(_coreDataRepository, _loggingRepository);
                simpleUpload = worksheetValidator.UploadData(GetFilePath(job), job.Filename, "", job.Id, job.UploadedBy);
                job.Status = UploadJobStatus.SuccessfulUpload;
                return simpleUpload;
            }
        }

        private void UpdateUploadJobStatus(UploadJob job)
        {
            job.Status = UploadJobStatus.InProgress;
        }

        private string GetFilePath(UploadJob job)
        {
            // File is already saved on the disk
            var filePath = UploadFileHelper.GetFilePath(job.Filename, job.Id);
            return filePath;
        }

        private SimpleUpload PopulateSimpleUpload(UploadJob job)
        {
            var simpleUpload = new SimpleUpload
                {
                    ShortFileName = job.Filename,
                    FileName = GetFilePath(job),
                    FileSize = job.FileSize,
                    DataToUpload = new List<UploadDataModel>(),
                    DuplicateRowInDatabaseErrors = new List<DuplicateRowInDatabaseError>(),
                    DuplicateRowInSpreadsheetErrors = new List<DuplicateRowInSpreadsheetError>(),
                    ExcelDataSheets = new List<UploadExcelSheet>(),
                    UploadValidationFailures = new List<UploadValidationFailure>()
                };
            return simpleUpload;
        }
    }
}
