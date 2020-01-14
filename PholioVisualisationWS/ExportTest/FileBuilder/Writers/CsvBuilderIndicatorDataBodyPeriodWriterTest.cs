using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.Export.FileBuilder.Writers;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;
using System;
using System.Collections.Generic;
using PholioVisualisation.Export.FileBuilder.Containers;
using PholioVisualisation.Export.FileBuilder.SupportModels;

namespace PholioVisualisation.ExportTest.FileBuilder.Writers
{
    [TestClass]
    public class CsvBuilderIndicatorDataBodyPeriodWriterTest
    {
        private ExportAreaHelper _areaHelper;

        private Mock<IAreasReader> _areasReaderMock;
        private Mock<PholioReader> _pholioReaderMock;
        private IndicatorExportParameters _generalParameters;
        private OnDemandQueryParametersWrapper _onDemandQueryParameters;
        private IndicatorMetadata _indicatorMetadata;
        private CsvBuilderAttributesForBodyContainer _parameters;

        private IndicatorExportParameters _indicatorExportParameters;
        private AreaFactory _areaFactory;
        private SingleIndicatorFileWriter _singleIndicatorFileWriter;
        private CoreDataSet _coreDataSetEnglandTest;
        private CoreDataSet _coreDataSetSubNationalTest;
        private IList<CoreDataSet> _dataForLocal;

        private const int CategoryTypeUndefined = CategoryTypeIds.Undefined;

        [TestInitialize]
        public void Start()
        {
            _areasReaderMock = new Mock<IAreasReader>(MockBehavior.Strict);
            _pholioReaderMock = new Mock<PholioReader>(MockBehavior.Strict);
            _generalParameters = new IndicatorExportParameters();
            _onDemandQueryParameters = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { IndicatorIds.MrsaBacteraemiaRates, null } });
            _indicatorMetadata = new IndicatorMetadata { Unit = new Unit { Value = 1 }, Descriptive = new Dictionary<string, string> { { "Name", "NameTest" } }, YearType = new YearType() };

            _parameters = new CsvBuilderAttributesForBodyContainer(_generalParameters, new OnDemandQueryParametersWrapper(1, new List<int>(), new Dictionary<int, IList<InequalitySearch>>(), null, new List<int>(), true));

            _indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            _areaFactory = new AreaFactory(_areasReaderMock.Object);
            _areaHelper = new ExportAreaHelper(_areasReaderMock.Object, _indicatorExportParameters);

            _areasReaderMock.Setup(x => x.GetCategoryTypes(It.IsAny<IList<int>>())).Returns(new List<CategoryType>());
            _areasReaderMock.Setup(x => x.GetAreaFromCode(It.IsAny<string>())).Returns(new Area { Code = "SubNationalTest", Name = "NameTest", ShortName = "shortNameTest" });
            _areasReaderMock.Setup(x => x.GetChildAreas(It.IsAny<string>(), It.IsAny<int>())).Returns(new List<IArea> { new Area { Code = "SubNationalTest", Name = "NameTest" } });
            _areasReaderMock.Setup(x => x.GetParentAreasFromChildAreaId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Dictionary<string, Area> { { "SubNationalTest", new Area { Code = "SubNationalTest", Name = "NameTest" } } });
            _areasReaderMock.Setup(x => x.GetAreaType(It.IsAny<int>())).Returns(new AreaType
            {
                CanBeDisplayedOnMap = false,
                Id = 1,
                IsCurrent = true,
                IsSearchable = true,
                Name = "AreaTypeTest",
                ParentAreaTypes = new List<IAreaType>(),
                ShortName = "shortNameTest"
            });
            _areasReaderMock.Setup(x => x.GetAreasByAreaTypeId(It.IsAny<int>())).Returns(new List<IArea> { new Area { AreaTypeId = 1, Code = "SubNationalTest", Name = "NameTest", ShortName = "ShortNameTest" } });
            _areaHelper.Init();

            _singleIndicatorFileWriter = new SingleIndicatorFileWriter(1, _parameters);

            _pholioReaderMock.Setup(x => x.GetAllAges()).Returns(new List<Age> { new Age { Id = 1, Name = "AgeTest" } });
            _pholioReaderMock.Setup(x => x.GetAllSexes()).Returns(new List<Sex> { new Sex { Id = 0, Name = "SexName", Sequence = 1 } });
            _pholioReaderMock.Setup(x => x.GetAllValueNotes()).Returns(new List<ValueNote> { new ValueNote { Id = 1, Text = "NoteTest" } });

            var lookUpManager = new LookUpManager(_pholioReaderMock.Object, _areasReaderMock.Object, new List<int> { 1 }, new List<int> { 1 });

            _singleIndicatorFileWriter.Init(lookUpManager, new TrendMarkerLabelProvider(1), new SignificanceFormatter(1, 1), _indicatorMetadata);
            _coreDataSetEnglandTest = new CoreDataSet
            {
                AgeId = 1,
                CategoryTypeId = CategoryTypeUndefined,
                AreaCode = "EnglandTest",
                YearRange = 1,
                Count = 1,
                IndicatorId = 1,
                Denominator2 = 1,
                Month = 1,
                Denominator = 1,
                CategoryId = 1,
                HasBeenTruncated = true,
                Value = 1,
                Year = 2020,
                Significance = new Dictionary<int, int>(),
                Quarter = 1,
                SexId = 0,
                LowerCI95 = 1,
                LowerCI95F = "",
                LowerCI99_8 = 1,
                LowerCI99_8F = "",
                SignificanceAgainstOneBenchmark = 1,
                UniqueId = 1,
                UpperCI95 = 1,
                UpperCI95F = "",
                UpperCI99_8 = 1,
                UpperCI99_8F = "",
                ValueFormatted = "",
                ValueNoteId = 1
            };
            _coreDataSetSubNationalTest = new CoreDataSet
            {
                AgeId = 1,
                CategoryTypeId = CategoryTypeUndefined,
                AreaCode = "SubNationalTest",
                YearRange = 1,
                Count = 1,
                IndicatorId = 1,
                Denominator2 = 1,
                Month = 1,
                Denominator = 1,
                CategoryId = 1,
                HasBeenTruncated = true,
                Value = 1,
                Year = 2020,
                Significance = new Dictionary<int, int>(),
                Quarter = 1,
                SexId = 0,
                LowerCI95 = 1,
                LowerCI95F = "",
                LowerCI99_8 = 1,
                LowerCI99_8F = "",
                SignificanceAgainstOneBenchmark = 1,
                UniqueId = 1,
                UpperCI95 = 1,
                UpperCI95F = "",
                UpperCI99_8 = 1,
                UpperCI99_8F = "",
                ValueFormatted = "",
                ValueNoteId = 1
            };
            _dataForLocal = new List<CoreDataSet> { _coreDataSetSubNationalTest };
        }

        [TestCleanup]
        public void CleanUp()
        {
            _areasReaderMock.VerifyAll();
            _pholioReaderMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldWritePeriodsForIndicatorBodyInFile()
        {
            const int indicatorId = IndicatorIds.MrsaBacteraemiaRates;
            const int areaTypeId = AreaTypeIds.CountyAndUnitaryAuthority;
            var groupingMock = new Grouping
            {
                Age = new Age(),
                AreaTypeId = 1,
                AgeId = 1,
                BaselineYear = 1,
                YearRange = 1,
                IndicatorId = 1,
                Sequence = 1,
                GroupId = 1,
                SexId = 1,
                BaselineMonth = 1,
                BaselineQuarter = 1,
                ComparatorConfidence = 1,
                ComparatorData = _coreDataSetEnglandTest,
                ComparatorId = 1,
                ComparatorMethodId = 1,
                ComparatorTargetId = 1,
                DataPointMonth = 1,
                DataPointQuarter = 1,
                DataPointYear = 1,
                GroupingId = 1,
                PolarityId = 1,
                Sex = new Sex(),
                TimePeriodText = "periodTextTest"
            };
            var groupDataReaderMock = new Mock<GroupDataReader>(MockBehavior.Strict);

            groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(_dataForLocal);


            var bodyPeriodWriterContainer = new CsvBuilderIndicatorDataBodyPeriodWriter(_indicatorMetadata, groupingMock, _areaHelper, groupDataReaderMock.Object, _areasReaderMock.Object, _generalParameters, _onDemandQueryParameters);

            try
            {
                var grouping = new Grouping()
                {
                    AreaTypeId = areaTypeId
                };

                bodyPeriodWriterContainer.WritePeriodsForIndicatorBodyInFile(indicatorId, _singleIndicatorFileWriter, grouping);
            }
            catch (Exception)
            {
                Assert.Fail("The method should not throw any errors");
            }

            Assert.IsNotNull(_singleIndicatorFileWriter);
            Assert.IsNotNull(_singleIndicatorFileWriter.GetFileContent());
        }
    }
}