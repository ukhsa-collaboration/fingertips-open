using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.Export.FileBuilder.Containers;
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

        private const int DistrictAreaTypeId = AreaTypeIds.District;
        private const int CountyAndUnitaryAuthority = AreaTypeIds.CountyAndUnitaryAuthority;

        [TestInitialize]
        public void SetUp()
        {
            _areasReaderMock = new Mock<IAreasReader>(MockBehavior.Strict);
            _pholioReaderMock = new Mock<PholioReader>(MockBehavior.Strict);
            _indicatorMetadataProvider = IndicatorMetadataProvider.Instance;
            _indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            _areaFactory = new AreaFactory(_areasReaderMock.Object);
            _areaHelper = new ExportAreaHelper(_areasReaderMock.Object, _indicatorExportParameters, _areaFactory);

            
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
            _generalParameters = new IndicatorExportParameters { ChildAreaTypeId = DistrictAreaTypeId };
            _onDemandParameters = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<Inequality>> { { 1, null } }, new List<string> { "stringTest" }, new List<int> { 1 });
            _csvBuilderAttributesForBodyContainer = new CsvBuilderAttributesForBodyContainer(_generalParameters, _onDemandParameters);

            var indicatorDataBodyContainer = new IndicatorDataBodyContainer(_indicatorMetadataProvider, _areaHelper, _areasReaderMock.Object, _csvBuilderAttributesForBodyContainer);

            var result = indicatorDataBodyContainer.GetIndicatorDataFile(1);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == 0);
        }

        [TestMethod]
        public void ShouldWriteIndicatorDataBodyInFileTest()
        {
            var indicatorId = IndicatorIds.TyphoidAndParatyphoidIncidenceRate;
            var groupId = GroupIds.AllIndicators;
            _generalParameters = new IndicatorExportParameters { ChildAreaTypeId = CountyAndUnitaryAuthority };
            _onDemandParameters = new OnDemandQueryParametersWrapper(1, new List<int> { indicatorId }, new Dictionary<int, IList<Inequality>> { { indicatorId, null } }, new List<string> { "stringTest" }, new List<int> { groupId });
            _csvBuilderAttributesForBodyContainer = new CsvBuilderAttributesForBodyContainer(_generalParameters, _onDemandParameters);
            var indicatorDataBodyContainer = new IndicatorDataBodyContainer(_indicatorMetadataProvider, _areaHelper, _areasReaderMock.Object, _csvBuilderAttributesForBodyContainer);

            _areasReaderMock.Setup(x => x.GetAreaFromCode(It.IsAny<string>())).Returns(new Area { Code = "09", Name = "NameTest", ShortName = "shortNameTest" });
            _areasReaderMock.Setup(x => x.GetChildAreas(It.IsAny<string>(), It.IsAny<int>())).Returns(new List<IArea> { new Area { Code = "09", Name = "AreaTest" } });
            _areasReaderMock.Setup(x => x.GetParentAreasFromChildAreaId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Dictionary<string, Area> { { "09", new Area { Code = "09", Name = "NameTest" } } });
            _areasReaderMock.Setup(x => x.GetAllCategoryTypes()).Returns(new List<CategoryType>());
            _areasReaderMock.Setup(x => x.GetAreaType(It.IsAny<int>())).Returns(new AreaType{ CanBeDisplayedOnMap = false, Id = 1, IsCurrent = true, IsSearchable = true, Name = "AreaTypeTest",
                ParentAreaTypes = new List<IAreaType>(), ShortName = "shortNameTest"});
            _areasReaderMock.Setup(x => x.GetAreasByAreaTypeId(It.IsAny<int>())).Returns(new List<IArea> { new Area { AreaTypeId = 1, Code = "09", Name = "NameTest", ShortName = "ShortNameTest" } });
            _areasReaderMock.Setup(x => x.GetCategoryTypes(It.IsAny<IList<int>>())).Returns(new List<CategoryType>());

            var result = indicatorDataBodyContainer.GetIndicatorDataFile(indicatorId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == 446);
        }
    }
}
