using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.UserData;
using PholioVisualisation.UserData.Repositories;

namespace PholioVisualisation.UserDataTest
{
    [TestClass]
    public class AreaListRepositoryTest
    {
        public const string UserId = "58189c36-969d-4e13-95c9-67a01832ab24";
        public const int AreaListId = 10;
        public const string PublicId = "al-ZY6zmuVONE";

        private readonly IAreaListRepository _areaListRepository;

        public AreaListRepositoryTest()
        {
            var dbContext = new fingertips_usersEntities();
            _areaListRepository = new AreaListRepository(dbContext);
        }

        [TestMethod]
        public void TestGet()
        {
            Assert.AreEqual(AreaListId, _areaListRepository.Get(AreaListId).Id);
        }

        [TestMethod]
        public void TestGetAreaListByPublicId()
        {
            Assert.AreEqual(AreaListId, _areaListRepository.GetAreaListByPublicId(PublicId).Id);
        }

        [TestMethod]
        public void TestGetUserIdByPublicId()
        {
            Assert.AreEqual(UserId, _areaListRepository.GetUserIdByPublicId(PublicId));
        }

        [TestMethod]
        public void TestGetAll()
        {
            Assert.IsTrue(_areaListRepository.GetAll(UserId).Any());
        }

        [TestMethod]
        public void TestGetAreaListAreaCodes()
        {
            Assert.IsTrue(_areaListRepository.GetAreaListAreaCodes(AreaListId).Any());
        }
    }
}
