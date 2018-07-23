using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.ServiceActions;

namespace PholioVisualisation.ServiceActionsTest
{
    [TestClass]
    public class DeploymentStatusProviderTest
    {
        public const string ConnectionStringA =
            @"Data Source=sqlclusporcex01\cenx;Initial Catalog=pholio_live_a;integrated security=SSPI;multipleactiveresultsets=True;";
        public const string ConnectionStringB =
            @"Data Source=sqlclusporcex01\cenx;Initial Catalog=pholio_live_b;integrated security=SSPI;multipleactiveresultsets=True;";

        private NameValueCollection _appSettings;
        private Mock<ConnectionStringsWrapper> _connectionStrings;
        private Mock<IDatabaseLogRepository> _databaseLogRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _appSettings = new NameValueCollection();
            _connectionStrings = new Mock<ConnectionStringsWrapper>(MockBehavior.Strict);
            _databaseLogRepository = new Mock<IDatabaseLogRepository>(MockBehavior.Strict);
        }

        [TestMethod]
        public void Test_Status_A()
        {
            SetApplicationName("CoreWs-Live-A");
            SetExportFileDirectory(@"C:\web-sites\excel-files-a");
            SetConnectionString(ConnectionStringA);
            SetDatabaseBackUpTimestamp();

            // Assert
            Assert.AreEqual("a", GetDeploymentStatus().DataFiles);
            Assert.AreEqual("a", GetDeploymentStatus().WebSite);
            Assert.AreEqual("a", GetDeploymentStatus().Database);
            VerifyAll();
        }

        [TestMethod]
        public void Test_Status_B()
        {
            SetApplicationName("CoreWs-Live-B");
            SetExportFileDirectory(@"C:\web-sites\excel-files-b");
            SetConnectionString(ConnectionStringB);
            SetDatabaseBackUpTimestamp();

            // Assert
            Assert.AreEqual("b", GetDeploymentStatus().DataFiles);
            Assert.AreEqual("b", GetDeploymentStatus().WebSite);
            Assert.AreEqual("b", GetDeploymentStatus().Database);
            VerifyAll();
        }

        [TestMethod]
        public void Test_Status_Of_Data_Files_Case_Insensitive()
        {
            SetApplicationName("CoreWs-Live-A");
            SetExportFileDirectory(@"C:\web-sites\excel-files-A");
            SetConnectionString(ConnectionStringA);
            SetDatabaseBackUpTimestamp();

            // Assert
            Assert.AreEqual("a", GetDeploymentStatus().DataFiles);
            VerifyAll();
        }

        private void SetConnectionString(string connectionString)
        {
            _connectionStrings.SetupGet(x => x.PholioConnectionString)
                .Returns(connectionString);
        }

        private void SetDatabaseBackUpTimestamp()
        {
            _databaseLogRepository.Setup(x => x.GetPholioBackUpTimestamp())
                .Returns(new DateTime(2001,2,3));
        }

        private void SetExportFileDirectory(string dir)
        {
            _appSettings.Add("ExportFileDirectory", dir);
        }

        private void VerifyAll()
        {
            _connectionStrings.VerifyAll();
            _databaseLogRepository.VerifyAll();
        }

        private void SetApplicationName(string name)
        {
            _appSettings.Add("ApplicationName", name);
        }

        private DeploymentStatus GetDeploymentStatus()
        {
            var configuration = new ApplicationConfiguration(_appSettings);
            var status = new DeploymentStatusProvider(configuration,
                _connectionStrings.Object, _databaseLogRepository.Object).GetDeploymentStatus();
            return status;
        }
    }
}
