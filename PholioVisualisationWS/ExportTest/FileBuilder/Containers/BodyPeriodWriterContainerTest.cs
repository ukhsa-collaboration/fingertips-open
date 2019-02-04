using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.Export.FileBuilder.Containers;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;

namespace PholioVisualisation.ExportTest.FileBuilder.Containers
{
    [TestClass]
    public class BodyPeriodWriterContainerTest
    {
        private Mock<IAreasReader> _areasReaderMock;
        private Mock<PholioReader> _pholioReaderMock;
        private IndicatorExportParameters _generalParameters;
        private OnDemandQueryParametersWrapper _onDemandQueryParameters;
        private IndicatorMetadata _indicatorMetadata;

        private IndicatorExportParameters _indicatorExportParameters;
        private AreaFactory _areaFactory;
        private ExportAreaHelper _areaHelper;
        private IList<TimePeriod> _timePeriods;
        private SingleIndicatorFileWriter _singleIndicatorFileWriter;
        private CoreDataSet _coreDataSetEnglandTest;
        private CoreDataSet _coreDataSetSubNationalTest;
        private IList<CoreDataSet> _dataForEngland;
        private IList<CoreDataSet> _dataForSubNational;
        private IList<CoreDataSet> _dataForLocal;

        private const int CategoryTypeUndefined = CategoryTypeIds.Undefined;

        [TestInitialize]
        public void Start()
        {
            _areasReaderMock = new Mock<IAreasReader>(MockBehavior.Strict);
            _pholioReaderMock = new Mock<PholioReader>(MockBehavior.Strict);
            _generalParameters = new IndicatorExportParameters();
            _onDemandQueryParameters = new OnDemandQueryParametersWrapper(1, new List<int>{1}, new Dictionary<int, IList<Inequality>> { { 1, null } });
            _indicatorMetadata = new IndicatorMetadata { Unit = new Unit { Value = 1 }, Descriptive = new Dictionary<string, string>{{"Name", "NameTest"}}};

            _indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            _areaFactory = new AreaFactory(_areasReaderMock.Object);
            _areaHelper = new ExportAreaHelper(_areasReaderMock.Object, _indicatorExportParameters, _areaFactory);
            _timePeriods = new List<TimePeriod>
            {
                new TimePeriod { Month = 1, Quarter = 1, Year = 2016, YearRange = 1 },
                new TimePeriod{ Month = 1, Quarter = 1, Year = 2017, YearRange = 1 },
                new TimePeriod{ Month = 1, Quarter = 1, Year = 2018, YearRange = 1 },
                new TimePeriod{ Month = 1, Quarter = 1, Year = 2019, YearRange = 1 },
                new TimePeriod{ Month = 1, Quarter = 1, Year = 2020, YearRange = 1 }
            };


            _areasReaderMock.Setup(x => x.GetCategoryTypes(It.IsAny<IList<int>>())).Returns(new List<CategoryType> ());
            _areasReaderMock.Setup(x => x.GetAreaFromCode(It.IsAny<string>())).Returns(new Area { Code = "SubNationalTest", Name = "NameTest", ShortName = "shortNameTest"});
            _areasReaderMock.Setup(x => x.GetChildAreas(It.IsAny<string>(), It.IsAny<int>())).Returns(new List<IArea> { new Area { Code = "SubNationalTest", Name = "NameTest"} });
            _areasReaderMock.Setup(x => x.GetParentAreasFromChildAreaId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Dictionary<string, Area> { { "SubNationalTest", new Area { Code = "SubNationalTest", Name = "NameTest"} } });
            _areasReaderMock.Setup(x => x.GetAreaType(It.IsAny<int>())).Returns(new AreaType{ CanBeDisplayedOnMap = false, Id = 1, IsCurrent = true, IsSearchable = true, Name = "AreaTypeTest",
                ParentAreaTypes = new List<IAreaType>(), ShortName = "shortNameTest"});
            _areasReaderMock.Setup(x => x.GetAreasByAreaTypeId(It.IsAny<int>())).Returns(new List<IArea>{ new Area{ AreaTypeId = 1, Code = "SubNationalTest", Name = "NameTest", ShortName = "ShortNameTest"}});
            _areaHelper.Init();

            _singleIndicatorFileWriter = new SingleIndicatorFileWriter(1, _generalParameters);

            _pholioReaderMock.Setup(x => x.GetAllAges()).Returns(new List<Age> { new Age { Id = 1, Name = "AgeTest"}});
            _pholioReaderMock.Setup(x => x.GetAllSexes()).Returns(new List<Sex> { new Sex { Id = 0, Name = "SexName", Sequence = 1 }});
            _pholioReaderMock.Setup(x => x.GetAllValueNotes()).Returns(new List<ValueNote> { new ValueNote {Id = 1, Text = "NoteTest"}});

            var lookUpManager = new LookUpManager(_pholioReaderMock.Object, _areasReaderMock.Object, new List<int> {1}, new List<int> {1});

            _singleIndicatorFileWriter.Init(lookUpManager, new TrendMarkerLabelProvider(1) , new SignificanceFormatter(1, 1), _indicatorMetadata);
            _coreDataSetEnglandTest = new CoreDataSet {
                AgeId = 1, CategoryTypeId = CategoryTypeUndefined, AreaCode = "EnglandTest", YearRange = 1, Count = 1, IndicatorId = 1, Denominator2 = 1, Month = 1, Denominator = 1, CategoryId = 1, HasBeenTruncated = true,
                Value = 1, Year = 2020, Significance = new Dictionary<int, int>(), Quarter = 1, SexId = 0, LowerCI95 = 1, LowerCI95F = "", LowerCI99_8 = 1, LowerCI99_8F = "", SignificanceAgainstOneBenchmark = 1,
                Uid = 1, UniqueId = 1, UpperCI95 = 1, UpperCI95F = "", UpperCI99_8 = 1, UpperCI99_8F = "", ValueFormatted = "", ValueNoteId = 1
            };
            _coreDataSetSubNationalTest = new CoreDataSet
            {
                AgeId = 1, CategoryTypeId = CategoryTypeUndefined, AreaCode = "SubNationalTest", YearRange = 1, Count = 1, IndicatorId = 1, Denominator2 = 1, Month = 1, Denominator = 1, CategoryId = 1, HasBeenTruncated = true,
                Value = 1, Year = 2020, Significance = new Dictionary<int, int>(), Quarter = 1, SexId = 0, LowerCI95 = 1, LowerCI95F = "", LowerCI99_8 = 1, LowerCI99_8F = "", SignificanceAgainstOneBenchmark = 1,
                Uid = 1, UniqueId = 1, UpperCI95 = 1, UpperCI95F = "",UpperCI99_8 = 1, UpperCI99_8F = "", ValueFormatted = "",ValueNoteId = 1
            };
            _dataForEngland = new List<CoreDataSet> { _coreDataSetEnglandTest };
            _dataForSubNational = new List<CoreDataSet> { _coreDataSetSubNationalTest };
            _dataForLocal = new List<CoreDataSet> { _coreDataSetSubNationalTest };
        }

        [TestCleanup]
        public void CleanUp()
        {
            _areasReaderMock.VerifyAll();
            _pholioReaderMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldWriteChildProcessedDataTest()
        {
            var dataCollector = new MultipleCoreDataCollector();
            var groupDataReaderMock = new Mock<GroupDataReader>(MockBehavior.Strict);

            groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(),It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string>())).Returns(_dataForLocal);

            var bodyPeriodWriterContainer = new BodyPeriodWriterContainer(_generalParameters, _onDemandQueryParameters, _indicatorMetadata, _areaHelper, groupDataReaderMock.Object, _timePeriods,
                dataCollector, new Grouping ());

            dataCollector.AddChildDataList(_dataForLocal);
            

            var result = bodyPeriodWriterContainer.WriteChildProcessedData(1, _timePeriods[_timePeriods.Count - 1],"timeStringTest", 1, ref _singleIndicatorFileWriter, _dataForEngland, _dataForSubNational);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
            Assert.IsNotNull(_singleIndicatorFileWriter.GetFileContent());
        }

        [TestMethod]
        public void ShouldWriteProcessedDataTest()
        {
            var dataCollector = new MultipleCoreDataCollector();
            var groupDataReaderMock = new Mock<GroupDataReader>(MockBehavior.Strict);

            groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(_dataForSubNational);

            var bodyPeriodWriterContainer = new BodyPeriodWriterContainer(_generalParameters, _onDemandQueryParameters, _indicatorMetadata, _areaHelper, groupDataReaderMock.Object, _timePeriods,
                dataCollector, new Grouping());

            dataCollector.AddParentDataList(_dataForSubNational);

            IList<CoreDataSet> coreDataSetComparision = null;
            var result = bodyPeriodWriterContainer.WriteProcessedData(1, _timePeriods[_timePeriods.Count - 1], "timeStringTest", 1, ref _singleIndicatorFileWriter, ref coreDataSetComparision, null);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
            Assert.IsNotNull(_singleIndicatorFileWriter.GetFileContent());
        }
    }
}
