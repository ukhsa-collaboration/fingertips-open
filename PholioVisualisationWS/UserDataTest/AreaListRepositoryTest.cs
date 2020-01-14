using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.UserData;
using PholioVisualisation.UserData.Repositories;

namespace PholioVisualisation.UserDataTest
{
    [TestClass]
    public class AreaListRepositoryTest
    {
        public const string UserId = FingertipsUserIds.TestUser;
        public const string PublicId = AreaListCodes.TestListId;
        public int _areaListId;

        private IAreaListRepository _areaListRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            var dbContext = new fingertips_usersEntities();
            _areaListRepository = new AreaListRepository(dbContext);

            var codes = new List<string> {AreaCodes.CountyUa_Cambridgeshire};
           _areaListId = new AreaListTestHelper().CreateTestList(codes);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _areaListRepository.Delete(_areaListId);
        }

        [TestMethod]
        public void TestGet()
        {
            Assert.AreEqual(_areaListId, _areaListRepository.Get(_areaListId).Id);
        }

        [TestMethod]
        public void TestGetAreaListByPublicId()
        {
            Assert.AreEqual(_areaListId, _areaListRepository.GetAreaListByPublicId(PublicId).Id);
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
            Assert.IsTrue(_areaListRepository.GetAreaListAreaCodes(_areaListId).Any());
        }
    }
}
