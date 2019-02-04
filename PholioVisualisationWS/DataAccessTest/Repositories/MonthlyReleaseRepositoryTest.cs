using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess.Repositories;

namespace PholioVisualisation.DataAccessTest.Repositories
{
    [TestClass]
    public class MonthlyReleaseRepositoryTest
    {
        private MonthlyReleaseRepository _repository;

        [TestInitialize]
        public void TestInitialize()
        {
            _repository = new MonthlyReleaseRepository();
        }

        [TestMethod]
        public void TestGetReleaseDatas()
        {
            var dates = _repository.GetReleaseDates();
            Assert.IsNotNull(dates);
        }

        [TestMethod]
        public void TestGetPastReleaseDates()
        {
            var dates = _repository.GetPastReleaseDates();
            Assert.IsNotNull(dates);
        }
    }
}
