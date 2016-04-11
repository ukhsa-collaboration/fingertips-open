
using AutoMapper;
using FingertipsUploadService.Helpers;
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Core;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.ProfileData.Repositories;
using FingertipsUploadService.Upload;
using NLog;
using System.Collections.Generic;

namespace FingertipsUploadService
{
    public class UploadManager
    {
        private IFusUploadRepository _fusUploadRepository;
        private CoreDataRepository _coreDataRepository;
        private LoggingRepository _loggingRepository;
        private ProfilesReader _profilesReader;
        private readonly Logger _logger = LogManager.GetLogger("UploadManager");


        public UploadManager(IFusUploadRepository fusUploadRepository, CoreDataRepository coreDataRepository,
            LoggingRepository loggingRepository)
        {
            _fusUploadRepository = fusUploadRepository;
            _coreDataRepository = coreDataRepository;
            _loggingRepository = loggingRepository;
            _profilesReader = ReaderFactory.GetProfilesReader();
            var config = new MapperConfiguration(c => c.CreateMap<CoreDataSet, CoreDataSetArchive>().ReverseMap());
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
                _logger.Info("Starting a new job, batch id is {0}", job.Guid);
                StartJob(job);
            }
        }

        public void StartJob(UploadJob job)
        {
            var validator = new WorksheetNameValidator();
            var actualFilePath = FilePathHelper.GetActualFilePath(job);
            var excelFileReader = new ExcelFileReader(actualFilePath);

            if (job.JobType == UploadJobType.Simple)
            {
                SetUsername(job);
                _logger.Info("Processing at Simple upload for {0} and jobid# is {1}", job.Username, job.Guid);
                var worker = new SimpleJobWorker();
                var processor = new SimpleWorksheetDataProcessor(_coreDataRepository, _loggingRepository);
                worker.ProcessJob(job, validator, processor, excelFileReader);
            }
            else
            {
                SetUsername(job);
                _logger.Info("Processing at Batch upload for {0} and jobid# is {1}", job.Username, job.Guid);
                var worker = new BatchJobWorker();
                var processor = new BatchWorksheetDataProcessor(_coreDataRepository, _loggingRepository);
                worker.ProcessJob(job, validator, processor, excelFileReader);
            }
        }

        private void SetUsername(UploadJob job)
        {
            var fpmUser = _profilesReader.GetUserByUserId(job.UserId);
            job.Username = fpmUser.UserName;
        }
    }
}
