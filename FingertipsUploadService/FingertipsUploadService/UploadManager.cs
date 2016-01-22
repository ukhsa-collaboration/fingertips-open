
using FingertipsUploadService.Entities.Job;
using FingertipsUploadService.Repositories;
using System.Collections.Generic;

namespace FingertipsUploadService
{
    public class UploadManager
    {
        private IFusUploadRepository _fusUploadRepository;

        public UploadManager(IFusUploadRepository fusUploadRepository)
        {
            _fusUploadRepository = fusUploadRepository;
        }

        public UploadManager()
        {
        }

        /// <summary>
        /// Returns list of Jobs which are not yet started,
        /// </summary>
        /// <returns></returns>
        public List<UploadJob> GetNotStartedUploadJobs()
        {
            return _fusUploadRepository.GetNotStartedUploadJobs();
        }

        public void ProcessUploadJobs()
        {
            var jobs = GetNotStartedUploadJobs();
            foreach (var job in jobs)
            {
                StartJob(job);
            }
        }

        public void StartJob(UploadJob job)
        {
            var worker = new UploadWorker();
            worker.ProcessJob(job);
        }



        //        private void StartJob()
        //        {
        //            var jobs = _fusUploadRepository.GetNotStartedUploadJobs();
        //            foreach (var job in jobs)
        //            {
        //                var status = _fusUploadRepository.UpdateJob(job);
        //                if (status == UploadJobStatus.FailedValidation)
        //                {
        //                    _fusUploadRepository.SaveValidationFailure(status);
        //                }
        //            }
        //        }
    }
}
