using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess.Repositories;

namespace PholioVisualisation.DataAccessTest.Repositories
{
    [TestClass]
    public class FpmUserRepositoryTest
    {
        private FpmUserRepository _repository;
        [TestInitialize]
        public void TestInitialize()
        {
            _repository = new FpmUserRepository();
        }

        [TestMethod]
        public void TestGetUserById()
        {
            var user = _repository.GetUserById(1);
            Assert.IsNotNull(user);
        }
    }
}
