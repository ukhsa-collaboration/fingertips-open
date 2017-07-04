using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Fpm.ProfileData.Entities.Exceptions;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.ProfileDataTest.Respositories
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
        public void TestGetExceptionsByServer()
        {
            var server = GetTestServerName();
            IList<ExceptionLog> exceptions1day = _exceptionLogRepository.GetExceptionsByServer(1, server);
            IList<ExceptionLog> exceptions10days = _exceptionLogRepository.GetExceptionsByServer(10, server);
            Assert.IsTrue(exceptions10days.Count > exceptions1day.Count);
        }

        [TestMethod]
        public void TestGetExceptionsForAllServers()
        {
            const int days = 5;
            var allServersCount = _exceptionLogRepository.GetExceptionsForAllServers(days).Count;
            var oneServerCount = _exceptionLogRepository.GetExceptionsByServer(days, GetTestServerName()).Count;

            Assert.IsTrue(allServersCount >= oneServerCount,
                "Expected more exceptions from all servers than just the test server");
        }

        private static string GetTestServerName()
        {
            return ConfigurationManager.AppSettings["TestServerName"];
        }
    }
}
