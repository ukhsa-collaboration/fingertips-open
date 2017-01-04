using AutoMapper;
using FingertipsUploadService.FpmFileReader;
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
                _logger.Info("### Starting new job: '{0}'", job.Guid);
                StartJob(job);
            }
        }

        public void StartJob(UploadJob job)
        {
            var validator = new WorksheetNameValidator();
            var actualFilePath = FilePathHelper.GetActualFilePath(job);
            var fileReader = new FileReaderFactory().Get(actualFilePath, job.JobType);

            if (job.JobType == UploadJobType.Simple)
            {
                SetUsername(job);
                _logger.Info("Processing simple upload for {0} with ID '{1}'", job.Username, job.Guid);
                var worker = new SimpleJobWorker();
                var processor = new SimpleWorksheetDataProcessor(_coreDataRepository, _loggingRepository);
                worker.ProcessJob(job, validator, processor, fileReader);
            }
            else
            {
                SetUsername(job);
                _logger.Info("Processing batch upload for {0} with ID '{1}'", job.Username, job.Guid);
                var worker = new BatchJobWorker();
                var processor = new BatchWorksheetDataProcessor(_coreDataRepository, _loggingRepository, _logger);
                worker.ProcessJob(job, validator, processor, fileReader);
            }
        }

        //        private IUploadFileReader GetFileReader(UploadJob job, string actualFilePath)
        //        {
        //            IUploadFileReader fileReader;
        //            if (isCsv(actualFilePath) && job.JobType == UploadJobType.Batch)
        //            {
        //                fileReader = new CsvFileReader(actualFilePath);
        //            }
        //            else
        //            {
        //                fileReader = new ExcelFileReader(actualFilePath);
        //            }
        //            return fileReader;
        //        }

        //        public bool isCsv(string dataFilePath)
        //        {
        //            var fileExt = Path.GetExtension(dataFilePath);
        //            return fileExt != null && fileExt.ToLower() == ".csv";
        //        }

        private void SetUsername(UploadJob job)
        {
            var fpmUser = _profilesReader.GetUserByUserId(job.UserId);
            job.Username = fpmUser.UserName;
        }

    }
}
