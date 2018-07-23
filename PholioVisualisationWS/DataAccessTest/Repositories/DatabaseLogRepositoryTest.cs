using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess.Repositories;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class DatabaseLogRepositoryTest
    {
        private DatabaseLogRepository _repository;

        [TestInitialize]
        public void TestInitialize()
        {
            _repository = new DatabaseLogRepository();
        }

        [TestMethod]
        public void TestGetDatabaseLogs()
        {
            Assert.IsTrue(_repository.GetDatabaseLogs().Any());
        }

        [TestMethod]
        public void TestGetPholioBackUpTimestamp()
        {
            Assert.IsNotNull(_repository.GetPholioBackUpTimestamp());
        }
    }
}
