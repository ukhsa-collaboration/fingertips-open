using FingertipsUploadService.FpmFileReader;
using FingertipsUploadService.Helpers;
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Repositories;
using FingertipsUploadService.Upload;
using NLog;
using System.Collections.Generic;
using UploadJob = FingertipsUploadService.ProfileData.Entities.Job.UploadJob;

namespace FingertipsUploadService
{
    public class UploadManager
    {
        private readonly IFusUploadRepository _fusUploadRepository;
        private readonly CoreDataRepository _coreDataRepository;
        private readonly ProfilesReader _profilesReader;
        private readonly Logger _logger = LogManager.GetLogger("UploadManager");

        public UploadManager(IFusUploadRepository fusUploadRepository, CoreDataRepository coreDataRepository)
        {
            _fusUploadRepository = fusUploadRepository;
            _coreDataRepository = coreDataRepository;
            _profilesReader = ReaderFactory.GetProfilesReader();
            AutoMapperConfig.RegisterMappings();
        }

        public UploadManager()
        {
        }

        /// <summary>
        /// Returns list of Jobs which are not yet started,
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UploadJob> GetNotStartedOrConfirmationGivenUploadJobs()
        {
            return _fusUploadRepository.GetNotStartedOrConfirmationGivenUploadJobs();
        }

        public void ProcessUploadJobs()
        {
            var jobs = GetNotStartedOrConfirmationGivenUploadJobs();

            foreach (var job in jobs)
            {
                _logger.Info("### Starting new job: '{0}'", job.Guid);
                StartJob(job);
            }
        }

        public void StartJob(UploadJob job)
        {
            // Log job starting
            SetUsername(job);
            _logger.Info("Processing batch upload for {0} with ID '{1}'", job.Username, job.Guid);
            _logger.Debug("its debug");
            // Create file reader
            var filePath = FilePathHelper.GetActualFilePath(job);
            var fileReader = new FileReaderFactory().Get(filePath);

            // Create worker dependencies
            var worksheetNameValidator = new WorksheetNameValidator();
            var dataValidator = new DataValidator(_coreDataRepository, _logger);
            var uploader = new DataUploader(_coreDataRepository, _logger);

            // Process job
            var worker = new UploadJobWorker();
            worker.ProcessJob(job, worksheetNameValidator, dataValidator, fileReader, uploader);
        }

        private void SetUsername(UploadJob job)
        {
            var fpmUser = _profilesReader.GetUserByUserId(job.UserId);
            job.Username = fpmUser.UserName;
        }

    }
}
