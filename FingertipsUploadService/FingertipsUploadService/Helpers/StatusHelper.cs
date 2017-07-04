using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using NLog;

namespace FingertipsUploadService.Helpers
{
    public class StatusHelper
    {
        private Logger _logger;
        private UploadJobRepository _jobRepository;

        public StatusHelper(UploadJobRepository jobRepository, Logger logger)
        {
            _logger = logger;
            _jobRepository = jobRepository;
        }

        public void InProgress(UploadJob job)
        {
            job.Status = UploadJobStatus.InProgress;
            job.TotalRowsCommitted = 0;
            _jobRepository.UpdateJob(job);
            _logger.Info("Job ID {0} status changed to {1} ", job.Guid, job.Status);
        }

        public void UnexpectedError(UploadJob job)
        {
            job.Status = UploadJobStatus.UnexpectedError;
            _jobRepository.UpdateJob(job);
            _logger.Info("Job ID {0} status changed to {1} ", job.Guid, job.Status);
        }

        public void ConfirmationAwaited(UploadJob job)
        {
            job.Status = UploadJobStatus.ConfirmationAwaited;
            _jobRepository.UpdateJob(job);
            _logger.Info("Job ID {0} status changed to {1} ", job.Guid, job.Status);
        }

        public void SmallNumbersFound(UploadJob job)
        {
            job.Status = UploadJobStatus.SmallNumberWarningConfirmationAwaited;
            _jobRepository.UpdateJob(job);
            _logger.Info("Job ID {0} status changed to {1} ", job.Guid, job.Status);
        }

        public void FailedValidation(UploadJob job)
        {
            job.Status = UploadJobStatus.FailedValidation;
            _jobRepository.UpdateJob(job);
            _logger.Info("Job ID {0} status changed to {1} ", job.Guid, job.Status);
        }

        public void ColumnNameValidationFailed(UploadJob job)
        {
            job.Status = UploadJobStatus.ColumnNameValidationFailed;
            _jobRepository.UpdateJob(job);
            _logger.Info("Job ID {0} status changed to {1} ", job.Guid, job.Status);
        }

        public void SuccessfulUpload(UploadJob job, int rowsUploaded)
        {
            job.Status = UploadJobStatus.SuccessfulUpload;
            job.TotalRowsCommitted = rowsUploaded;

            _jobRepository.UpdateJob(job);
            _logger.Info("Job ID {0} successfully completed", job.Guid);
        }
    }
}
