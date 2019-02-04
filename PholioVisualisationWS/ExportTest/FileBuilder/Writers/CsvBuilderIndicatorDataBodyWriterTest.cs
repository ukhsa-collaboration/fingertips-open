using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.Export.FileBuilder.Containers;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.Export.FileBuilder.Writers;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest.FileBuilder.Writers
{
    [TestClass]
    public class CsvBuilderIndicatorDataBodyWriterTest
    {
        private ExportAreaHelper _areaHelper;

        private Mock<IAreasReader> _areasReaderMock;
        private Mock<PholioReader> _pholioReaderMock;
        private IndicatorExportParameters _generalParameters;
        private OnDemandQueryParametersWrapper _onDemandQueryParameters;
        private IndicatorMetadata _indicatorMetadata;

        private IndicatorExportParameters _indicatorExportParameters;
        private AreaFactory _areaFactory;
        private SingleIndicatorFileWriter _singleIndicatorFileWriter;

        [TestInitialize]
        public void Start()
        {
            _areasReaderMock = new Mock<IAreasReader>(MockBehavior.Strict);
            _pholioReaderMock = new Mock<PholioReader>(MockBehavior.Strict);
            _generalParameters = new IndicatorExportParameters();
            _onDemandQueryParameters = new OnDemandQueryParametersWrapper(1, new List<int>{1}, new Dictionary<int, IList<Inequality>> { { 1, null } });
            _indicatorMetadata = new IndicatorMetadata { Unit = new Unit { Value = 1 }, Descriptive = new Dictionary<string, string>{{"Name", "NameTest"}},YearType = new YearType()};

            _indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            _areaFactory = new AreaFactory(_areasReaderMock.Object);
            _areaHelper = new ExportAreaHelper(_areasReaderMock.Object, _indicatorExportParameters, _areaFactory);

            _areasReaderMock.Setup(x => x.GetCategoryTypes(It.IsAny<IList<int>>())).Returns(new List<CategoryType> ());
            _areasReaderMock.Setup(x => x.GetAreaFromCode(It.IsAny<string>())).Returns(new Area { Code = "SubNationalTest", Name = "NameTest", ShortName = "shortNameTest"});
            _areasReaderMock.Setup(x => x.GetChildAreas(It.IsAny<string>(), It.IsAny<int>())).Returns(new List<IArea> { new Area { Code = "SubNationalTest", Name = "NameTest"} });
            _areasReaderMock.Setup(x => x.GetParentAreasFromChildAreaId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Dictionary<string, Area> { { "SubNationalTest", new Area { Code = "SubNationalTest", Name = "NameTest"} } });
            _areasReaderMock.Setup(x => x.GetAreaType(It.IsAny<int>())).Returns(new AreaType{ CanBeDisplayedOnMap = false, Id = 1, IsCurrent = true, IsSearchable = true, Name = "AreaTypeTest",
                ParentAreaTypes = new List<IAreaType>(), ShortName = "shortNameTest"});
            _areasReaderMock.Setup(x => x.GetAreasByAreaTypeId(It.IsAny<int>())).Returns(new List<IArea>{ new Area{ AreaTypeId = AreaTypeIds.District, Code = "SubNationalTest", Name = "NameTest", ShortName = "ShortNameTest"}});
            _areaHelper.Init();

            _singleIndicatorFileWriter = new SingleIndicatorFileWriter(1, _generalParameters);

            _pholioReaderMock.Setup(x => x.GetAllAges()).Returns(new List<Age> { new Age { Id = 1, Name = "AgeTest"}});
            _pholioReaderMock.Setup(x => x.GetAllSexes()).Returns(new List<Sex> { new Sex { Id = 0, Name = "SexName", Sequence = 1 }});
            _pholioReaderMock.Setup(x => x.GetAllValueNotes()).Returns(new List<ValueNote> { new ValueNote {Id = 1, Text = "NoteTest"}});

            var lookUpManager = new LookUpManager(_pholioReaderMock.Object, _areasReaderMock.Object, new List<int> {1}, new List<int> {1});

            _singleIndicatorFileWriter.Init(lookUpManager, new TrendMarkerLabelProvider(1) , new SignificanceFormatter(1, 1), _indicatorMetadata);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _areasReaderMock.VerifyAll();
            _pholioReaderMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldGetContentTest()
        {
            var csvBuilderIndicatorDataBodyWriter = new CsvBuilderIndicatorDataBodyWriter(IndicatorMetadataProvider.Instance, _areaHelper, _areasReaderMock.Object,
                                                                                            new CsvBuilderAttributesForBodyContainer(_generalParameters, _onDemandQueryParameters));
            try
            {
                var result = csvBuilderIndicatorDataBodyWriter.GetBody();
                Assert.IsNotNull(result);

            }
            catch (Exception)
            {
                Assert.Fail("The method should not throw any errors");
            } 
        }
    }
}
