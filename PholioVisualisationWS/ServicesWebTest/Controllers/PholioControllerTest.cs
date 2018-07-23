using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Controllers;
using PholioVisualisation.ServicesWeb.Helpers;

namespace PholioVisualisation.ServicesWebTest.Controllers
{
    [TestClass]
    public class PholioControllerTest
    {
        public const int ProfileId = ProfileIds.Phof;
        public const int IndicatorId = IndicatorIds.DeprivationScoreIMD2010;

        private Mock<IProfileReader> _profileReader;
        private Mock<IGroupDataReader> _groupDataReader;
        private Mock<ICoreDataSetValidator> _coreDataSetValidator;
        private Mock<IIndicatorMetadataRepository> _indicatorMetadataRepository;
        private Mock<IRequestContentParserHelper> _parserHelper;

        private PholioController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _profileReader = new Mock<IProfileReader>(MockBehavior.Strict);
            _groupDataReader = new Mock<IGroupDataReader>(MockBehavior.Strict);
            _coreDataSetValidator = new Mock<ICoreDataSetValidator>(MockBehavior.Strict);
            _indicatorMetadataRepository = new Mock<IIndicatorMetadataRepository>(MockBehavior.Strict);
            _parserHelper = new Mock<IRequestContentParserHelper>(MockBehavior.Strict);
            _controller = CreateController();
        }

        [TestMethod]
        public void TestGetMetadata()
        {
            // Arrange
            var list = new List<IndicatorMetadataTextValue>
            {
                new IndicatorMetadataTextValue()
            };
            _groupDataReader.Setup(x => x.GetIndicatorMetadataTextValues(IndicatorId))
                .Returns(list);

            // Act
            var indicatorMetadataTextValues = _controller.GetMetadata(IndicatorId);

            // Assert
            Assert.AreEqual(1, indicatorMetadataTextValues.Count);

            VerifyAll();
        }

        [TestMethod]
        public void TestGetGroupings()
        {
            // Arrange
            var groupIds = new List<int>
            {
                new int()
            };
            _profileReader.Setup(x => x.GetGroupIdsForProfile(ProfileId))
                .Returns(groupIds);

            var groupings = new List<Grouping>
            {
                new Grouping()
            };
            _groupDataReader.Setup(x => x.GetGroupingsByGroupIdsAndIndicatorId(groupIds, IndicatorId))
                .Returns(groupings);

            // Act
            var controllerInvokedGroupings = _controller.GetGroupings(ProfileId, IndicatorId);

            // Assert
            Assert.AreEqual(1, controllerInvokedGroupings.Count);

            VerifyAll();
        }

        [TestMethod]
        public void TestGetCoreDataSets()
        {
            // Arrange
            var list = new List<CoreDataSet>
            {
                new CoreDataSet()
            };
            _groupDataReader.Setup(x => x.GetCoreDataForIndicatorId(IndicatorId))
                .Returns(list);

            // Act
            var coreDataSets = _controller.GetCoreDataSets(IndicatorId);

            // Assert
            Assert.AreEqual(1, coreDataSets.Count);

            VerifyAll();
        }

        private PholioController CreateController()
        {
            return new PholioController(_profileReader.Object, _groupDataReader.Object, _coreDataSetValidator.Object,
                _indicatorMetadataRepository.Object, _parserHelper.Object);
        }

        private void VerifyAll()
        {
            _profileReader.VerifyAll();
            _groupDataReader.VerifyAll();
            _coreDataSetValidator.VerifyAll();
            _indicatorMetadataRepository.VerifyAll();
            _parserHelper.VerifyAll();
        }
    }
}