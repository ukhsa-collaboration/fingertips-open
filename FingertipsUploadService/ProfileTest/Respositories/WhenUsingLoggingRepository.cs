using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FingertipsUploadService.ProfileData.Repositories;

namespace FingertipsUploadService.ProfileDataTest.Respositories
{
    [TestClass]
    public class WhenUsingLoggingRepository
    {
        private LoggingRepository _loggingRepository;

        [TestInitialize]
        public void Init()
        {
            _loggingRepository = new LoggingRepository();
        }

        [TestMethod]
        public void UpdateFusCheckedJobs_Can_Be_Called()
        {
            _loggingRepository.UpdateFusCheckedJobs();
        }
    }
}
