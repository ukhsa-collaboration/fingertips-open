using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace FingertipsUploadService.ProfileDataTest.Respositories
{
    [TestClass]
    public class WhenUsingJobRepository
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
        public void CleanUp()
        {
            _uploadJobRepository.DeleteJob(_guid);
        }

        [TestMethod]
        public void TestGetNotStartedUploadJobs()
        {
            var job = NewJob(_guid);
            AddNewJob(job);

            var notStartedUploadJobs = _uploadJobRepository.GetNotStartedOrConfirmationGivenUploadJobs();
            var first = notStartedUploadJobs.FirstOrDefault();

            Assert.AreEqual(job.Guid, first.Guid);
            Assert.AreEqual(UploadJobStatus.NotStarted, first.Status);
        }

        [TestMethod]
        public void TestFindUploadByGuid()
        {
            var job = NewJob(_guid);
            AddNewJob(job);

            var jobFromDb = _uploadJobRepository.FindUploadJobByGuid(_guid);

            Assert.AreEqual(_guid, jobFromDb.Guid);
        }

        [TestMethod]
        public void TestUpdateJob()
        {
            var job = NewJob(_guid);
            AddNewJob(job);

            job.Status = UploadJobStatus.InProgress;
            _uploadJobRepository.UpdateJob(job);

            var jobFromDb = _uploadJobRepository.FindUploadJobByGuid(_guid);

            Assert.AreEqual(UploadJobStatus.InProgress, jobFromDb.Status);
        }

        [TestMethod]
        public void TestDelete()
        {
            var job = NewJob(_guid);
            AddNewJob(job);

            _uploadJobRepository.DeleteJob(_guid);
            var jobFromDb = _uploadJobRepository.FindUploadJobByGuid(_guid);
            Assert.IsTrue(jobFromDb == null);
        }

        private void AddNewJob(UploadJob newJob)
        {
            _uploadJobRepository.SaveJob(newJob);
        }

        private UploadJob NewJob(Guid guid)
        {
            var newJob = new UploadJob
            {
                Guid = guid,
                Username = @"phe\farrukh",
                Filename = "Fake.xls",
                DateCreated = DateTime.Now,
                Status = UploadJobStatus.NotStarted
            };
            return newJob;
        }
    }
}
