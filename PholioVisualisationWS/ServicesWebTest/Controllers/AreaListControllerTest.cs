using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Controllers;
using PholioVisualisation.ServicesWeb.Helpers;
using PholioVisualisation.UserData;
using PholioVisualisation.UserData.Repositories;

namespace PholioVisualisation.ServicesWebTest.Controllers
{
    [TestClass]
    public class AreaListControllerTest
    {
        public const string UserId = FingertipsUserIds.TestUser;
        public const int AreaListId = 2;
        public const string PublicId = AreaListCodes.TestListId;

        private Mock<IAreaListRepository> _areaListRepository;
        private Mock<IPublicIdGenerator> _publicIdGenerator;
        private AreaListController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _areaListRepository = new Mock<IAreaListRepository>(MockBehavior.Strict);
            _publicIdGenerator = new Mock<IPublicIdGenerator>(MockBehavior.Strict);
            _controller = new AreaListController(_areaListRepository.Object, _publicIdGenerator.Object);
        }

        [TestMethod]
        public void TestGetAreaLists()
        {
            // Arrange
            var list = new List<AreaList>
            {
                new AreaList()
            };
            _areaListRepository.Setup(x => x.GetAll(UserId))
                .Returns(list);

            // Act
            var areaLists = _controller.GetAreaLists(UserId);

            // Assert
            Assert.IsTrue(areaLists.Any());

            VerifyAll();
        }

        [TestMethod]
        public void TestGetAreaList()
        {
            // Arrange
            var list = new AreaList();
            _areaListRepository.Setup(x => x.Get(AreaListId))
                .Returns(list);

            // Act
            var areaList = _controller.GetAreaList(AreaListId);

            // Assert
            Assert.AreEqual(areaList.Id, list.Id);

            VerifyAll();
        }

        [TestMethod]
        public void TestGetAreaListByPublicId()
        {
            // Arrange
            var list = new AreaList {
                PublicId = PublicId,
                UserId = FingertipsUserIds.TestUser
            };
            _areaListRepository.Setup(x => x.GetAreaListByPublicId(PublicId))
                .Returns(list);

            // Act
            var areaList = _controller.GetAreaListByPublicId(PublicId, UserId);

            // Assert
            Assert.AreEqual(areaList.PublicId, PublicId);

            VerifyAll();
        }

        [TestMethod]
        public void TestGetAreaListAreaCodes()
        {
            // Arrange
            var list = new List<AreaListAreaCode>
            {
                new AreaListAreaCode()
            };
            _areaListRepository.Setup(x => x.GetAreaListAreaCodes(AreaListId))
                .Returns(list);

            // Act
            var areaListAreaCodes = _controller.GetAreaListAreaCodes(AreaListId);

            // Assert
            Assert.IsTrue(areaListAreaCodes.Any());

            VerifyAll();
        }

        private void VerifyAll()
        {
            _areaListRepository.VerifyAll();
        }
    }
}
