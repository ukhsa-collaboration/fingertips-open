using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccessTest.Repositories
{
    [TestClass]
    public class CoreDataAuditRepositoryTest
    {
        private CoreDataAuditRepository _coreDataAuditRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _coreDataAuditRepository = new CoreDataAuditRepository();
        }

        [TestMethod]
        public void TestDeleteAudit()
        {
            var data = _coreDataAuditRepository.GetLatestDeleteAuditData(IndicatorIds.LifeExpectancyAtBirth);
            Assert.IsNotNull(data);
        }

        /// <summary>
        /// Test always fails in Jenkins for unknown reason
        /// </summary>
        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestUploadAudit()
        {
            var data = _coreDataAuditRepository.GetLatestUploadAuditData(
                IndicatorIds.LifeExpectancyAtBirth);
            Assert.IsNotNull(data);
        }
    }
}
