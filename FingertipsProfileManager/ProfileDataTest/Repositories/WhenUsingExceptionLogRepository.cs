using Fpm.ProfileData.Entities.Exceptions;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;

namespace Fpm.ProfileDataTest.Repositories
{
    [TestClass]
    public class WhenUsingExceptionLogRepository
    {
        private ExceptionsRepository _exceptionLogRepository;

        [TestInitialize]
        public void Init()
        {
            _exceptionLogRepository = new ExceptionsRepository();
        }

        [TestMethod]
        public void TestGetExceptionsForSpecificServers()
        {
            var server = GetTestServerName();
            IList<ExceptionLog> exceptions1day = _exceptionLogRepository.GetExceptionsForSpecificServers(1, server);
            IList<ExceptionLog> exceptions10days = _exceptionLogRepository.GetExceptionsForSpecificServers(10, server);

            Assert.IsTrue(
                exceptions10days.Count > exceptions1day.Count ||
                exceptions10days.Count == ExceptionsRepository.MaxExceptionCount &&
                exceptions1day.Count == ExceptionsRepository.MaxExceptionCount);
        }

        [TestMethod]
        public void TestGetExceptionsForAllServers()
        {
            const int days = 5;
            var allServersCount = _exceptionLogRepository.GetExceptionsForAllServers(days).Count;
            var oneServerCount = _exceptionLogRepository.GetExceptionsForSpecificServers(days, GetTestServerName()).Count;

            Assert.IsTrue(allServersCount >= oneServerCount,
                "Expected more exceptions from all servers than just the test server");
        }

        private static string GetTestServerName()
        {
            return ConfigurationManager.AppSettings["TestServerName"];
        }
    }
}