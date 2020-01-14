using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Fpm.ProfileDataTest.Repositories
{
    [TestClass]
    public class WhenUsingUploadRepository
    {
        private UploadJobRepository _uploadJobRepository;

        [TestInitialize]
        public void Init()
        {
            _uploadJobRepository = new UploadJobRepository();
        }

        [TestMethod]
        public void TestGetJobsForCurrentUser()
        {
            var jobs = _uploadJobRepository.GetJobsForCurrentUser(FpmUserIds.Doris, 30);
            Assert.IsTrue(jobs.ToList().Any());
        }

        [TestMethod]
        public void TestGetNotStartedOrConfirmationGivenUploadJobs()
        {
            var job = _uploadJobRepository.GetNotStartedOrConfirmationGivenUploadJobs();
            Assert.IsNotNull(job);
        }
    }
}