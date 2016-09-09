using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Fpm.ProfileDataTest.Respositories
{
    [TestClass]
    public class WhenUsingUploadRepository
    {
        private UploadJobRepository _uploadJobRepository;

        [TestInitialize]
        public void Init()
        {
            _uploadJobRepository = new UploadJobRepository();
            _uploadJobRepository.DeleteAllJobs();
        }

        [TestMethod]
        public void TestGetJobsForCurrentUser()
        {
            var jobs = _uploadJobRepository.GetJobsForCurrentUser(11);
            Assert.IsTrue(jobs.ToList().Count == 0);
        }

        [TestMethod]
        public void TestJobByGuid()
        {
            var job = _uploadJobRepository.GetNotStartedOrConfirmationGivenUploadJobs();
            Assert.IsNotNull(job);
        }
    }
}
