using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.Export.FileBuilder.Containers;
using PholioVisualisation.Export.FileBuilder.SupportModels;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest.FileBuilder.Containers
{
    [TestClass]
    public class IndicatorDataBodyContainerTest
    {
        private IndicatorMetadataProvider _indicatorMetadataProvider;
        private AreaFactory _areaFactory;
        private ExportAreaHelper _areaHelper;
        private Mock<IAreasReader> _areasReaderMock;
        private Mock<PholioReader> _pholioReaderMock;
        private IndicatorExportParameters _indicatorExportParameters;
        private IndicatorExportParameters _generalParameters;
        private OnDemandQueryParametersWrapper _onDemandParameters;
        private CsvBuilderAttributesForBodyContainer _csvBuilderAttributesForBodyContainer;

        [TestInitialize]
        public void SetUp()
        {
            _areasReaderMock = new Mock<IAreasReader>(MockBehavior.Strict);
            _pholioReaderMock = new Mock<PholioReader>(MockBehavior.Strict);
            _indicatorMetadataProvider = IndicatorMetadataProvider.Instance;
            _indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            _areaFactory = new AreaFactory(_areasReaderMock.Object);
            _areaHelper = new ExportAreaHelper(_areasReaderMock.Object, _indicatorExportParameters);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _areasReaderMock.VerifyAll();
            _pholioReaderMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldIgnoreFileWriterTest()
        {
            var indicatorId = 1;

            _generalParameters = new IndicatorExportParameters { ChildAreaTypeId = AreaTypeIds.DistrictAndUnitaryAuthorityPreApr2019 };
            _onDemandParameters = new OnDemandQueryParametersWrapper(ProfileIds.HealthProtection, new List<int> { indicatorId },
                new Dictionary<int, IList<InequalitySearch>> { { 2, null } }, 
                new List<string> { "stringTest" }, new List<int> { 3 });
            _csvBuilderAttributesForBodyContainer = new CsvBuilderAttributesForBodyContainer(_generalParameters, _onDemandParameters);

            var indicatorDataBodyContainer = new IndicatorDataBodyContainer(_indicatorMetadataProvider, _areaHelper, _areasReaderMock.Object, _csvBuilderAttributesForBodyContainer);

            var result = indicatorDataBodyContainer.GetIndicatorDataFile(indicatorId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
            VerifyAll();
        }

        [TestMethod]
        public void ShouldWriteIndicatorDataBodyInFileTest()
        {
            // Arrange data
            var areaTypeId = AreaTypeIds.DistrictAndUnitaryAuthorityPreApr2019;
            var area = new Area { AreaTypeId = areaTypeId, Code = AreaCodes.DistrictUa_SouthCambridgeshire, Name = "NameTest", ShortName = "ShortNameTest" };
            var areaType = new AreaType
            {
                CanBeDisplayedOnMap = false,
                Id = areaTypeId,
                IsCurrent = true,
                IsSearchable = true,
                Name = "AreaTypeTest",
                ParentAreaTypes = new List<IAreaType>(),
                ShortName = "shortNameTest"
            };

            var indicatorId = IndicatorIds.TyphoidAndParatyphoidIncidenceRate;
            var groupId = GroupIds.HealthProtection_AllIndicators;

            // Arrange object
            _generalParameters = new IndicatorExportParameters { ChildAreaTypeId = areaTypeId };
            _onDemandParameters = new OnDemandQueryParametersWrapper(ProfileIds.HealthProtection, new List<int> { indicatorId }, new Dictionary<int, IList<InequalitySearch>> { { indicatorId, null } }, 
                new List<string> { "stringTest" }, new List<int> { groupId });
            _csvBuilderAttributesForBodyContainer = new CsvBuilderAttributesForBodyContainer(_generalParameters, _onDemandParameters);
            var indicatorDataBodyContainer = new IndicatorDataBodyContainer(_indicatorMetadataProvider, _areaHelper, _areasReaderMock.Object, _csvBuilderAttributesForBodyContainer);

            // Arrange: Setup mock
            _areasReaderMock.Setup(x => x.GetAreaFromCode(It.IsAny<string>())).Returns(area);
            _areasReaderMock.Setup(x => x.GetChildAreas(It.IsAny<string>(), It.IsAny<int>())).Returns(new List<IArea> { area });
            _areasReaderMock.Setup(x => x.GetParentAreasFromChildAreaId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Dictionary<string, Area> { { area.Code, area } });
            _areasReaderMock.Setup(x => x.GetAllCategoryTypes()).Returns(new List<CategoryType>());
            _areasReaderMock.Setup(x => x.GetAreaType(It.IsAny<int>())).Returns(areaType);
            _areasReaderMock.Setup(x => x.GetAreasByAreaTypeId(It.IsAny<int>())).Returns(new List<IArea> {area});
            _areasReaderMock.Setup(x => x.GetCategoryTypes(It.IsAny<IList<int>>())).Returns(new List<CategoryType>());

            // Act
            var result = indicatorDataBodyContainer.GetIndicatorDataFile(indicatorId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
            VerifyAll();
        }

        private void VerifyAll()
        {
            _areasReaderMock.VerifyAll();
            _pholioReaderMock.VerifyAll();
        }
    }
}
