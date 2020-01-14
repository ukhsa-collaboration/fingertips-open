using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.ProfileDataTest.Repositories
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
        public void GetDatabaseLog()
        {
            var databaseLog = _loggingRepository.GetDatabaseLog(DatabaseLogIds.FusCheckedJobs);
            Assert.IsTrue(databaseLog.Event.Contains("FUS"));
        }
    }
}