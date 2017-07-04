using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace FingertipsUploadService.ProfileDataTest.Respositories
{
    [TestClass]
    public class WhenUsingFusUploadRepository
    {
        private UploadJobRepository _uploadJobRepository;
        private Guid _guid;

        [TestInitialize]
        public void Init()
        {
            _uploadJobRepository = new UploadJobRepository();
            _guid = Guid.NewGuid();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _uploadJobRepository.DeleteJob(_guid);
        }

        [TestMethod]
        public void TestGetNotStartedUploadJobs()
        {

            var newJob = CreateJob(_guid);
            _uploadJobRepository.SaveJob(newJob);

            var notStartedJobs = _uploadJobRepository.GetNotStartedOrConfirmationGivenUploadJobs();
            Assert.IsTrue(notStartedJobs.Count() == 1);
        }


        [TestMethod]
        public void TestGetNotStartedUploadJobsAfterConfirmationGiven()
        {

            var newJob = CreateJob(_guid);
            _uploadJobRepository.SaveJob(newJob);

            var notStartedJobs = _uploadJobRepository.GetNotStartedOrConfirmationGivenUploadJobs();
            Assert.IsTrue(notStartedJobs.Count() == 1);

            var jobFromDB = notStartedJobs.First();
            jobFromDB.Status = UploadJobStatus.ConfirmationGiven;


            _uploadJobRepository.UpdateJob(jobFromDB);

            var jobsAfterStatusChange = _uploadJobRepository.GetNotStartedOrConfirmationGivenUploadJobs();

            Assert.IsTrue(jobsAfterStatusChange.Count() == 1);

        }


        [TestMethod]
        public void TestGetNotStartedOrConfirmationGivenUploadJobs()
        {

            var newJob = CreateJob(_guid);
            _uploadJobRepository.SaveJob(newJob);

            var confirmationAwaitedGuid = Guid.NewGuid();
            var jobConfirmationAwaited = CreateJob(confirmationAwaitedGuid);
            _uploadJobRepository.SaveJob(jobConfirmationAwaited);

            var notStartedJobs = _uploadJobRepository.GetNotStartedOrConfirmationGivenUploadJobs();
            Assert.IsTrue(notStartedJobs.Count() == 2);

            // Cleanup
            _uploadJobRepository.DeleteJob(confirmationAwaitedGuid);
        }

        private UploadJob CreateJob(Guid guid)
        {
            var newJob = new UploadJob
            {
                Guid = guid,
                Username = @"phe\farrukh",
                Filename = "Fake.xls",
                DateCreated = DateTime.Now,
                JobType = UploadJobType.Batch,
                TotalRows = 0,
                Status = UploadJobStatus.NotStarted
            };
            return newJob;
        }
    }
}
