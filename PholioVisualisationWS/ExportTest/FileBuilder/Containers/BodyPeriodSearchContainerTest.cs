using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.Export.FileBuilder.Containers;
using PholioVisualisation.Export.FileBuilder.SupportModels;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.PholioObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.ExportTest.FileBuilder.Containers
{
    [TestClass]
    public class BodyPeriodSearchContainerTest
    {
        private Mock<IAreasReader> _areasReaderMock;
        private Mock<QuartilesComparer> _indicatorComparerMock;
        private CoreDataSet _comparisionCoreDataSetEnglandTest;
        private CoreDataSet _comparisionCoreDataSetSubNationalTest;
        private IList<CoreDataSet> _comparision;

        private IndicatorExportParameters _indicatorExportParameters;
        private AreaFactory _areaFactory;
        private ExportAreaHelper _exportAreaHelper;

        private Mock<IGroupDataReader> _groupDataReaderMock;

        private const int AllAges = AgeIds.AllAges;
        private const int CategoryUndefined = CategoryTypeIds.Undefined;
        private const int CategoryTypeUndefined = CategoryTypeIds.Undefined;
        private const int Male = SexIds.Male;
        private const int Female = SexIds.Female;

        [TestInitialize]
        public void SetUp()
        {
            _areasReaderMock = new Mock<IAreasReader>(MockBehavior.Strict);
            _indicatorComparerMock = new Mock<QuartilesComparer>(MockBehavior.Strict);

            _comparisionCoreDataSetEnglandTest = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Male };
            _comparisionCoreDataSetSubNationalTest = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "SubNationalTest", SexId = Male};
            _comparision = new List<CoreDataSet> { _comparisionCoreDataSetEnglandTest, _comparisionCoreDataSetSubNationalTest };

            _indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            _areaFactory = new AreaFactory(_areasReaderMock.Object);
            _exportAreaHelper = new ExportAreaHelper(_areasReaderMock.Object, _indicatorExportParameters);

            _groupDataReaderMock = new Mock<IGroupDataReader>(MockBehavior.Strict);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _areasReaderMock.VerifyAll();
            _indicatorComparerMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldFindMatchingCoreDataSetNoAreaCodeTest()
        {
            var result = BodyPeriodSearchContainer.FindMatchingCoreDataSet(_comparision, _comparisionCoreDataSetEnglandTest);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(result, _comparisionCoreDataSetEnglandTest);
        }

        [TestMethod]
        public void ShouldFindMatchingCoreDataSetTest()
        {
            var result = BodyPeriodSearchContainer.FindMatchingCoreDataSet(_comparision, _comparisionCoreDataSetEnglandTest, _comparisionCoreDataSetEnglandTest.AreaCode);

            Assert.IsNotNull(result);
            Assert.AreEqual(result, _comparisionCoreDataSetEnglandTest);
        }

        [TestMethod]
        public void ShouldNotFilterAnyChildCoreDataByChildAreaCodeListTest()
        {
            
            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int>{1}, new Dictionary<int, IList<InequalitySearch>> { { 1, null } });
            var comparisionCloned = new List<CoreDataSet> { _comparisionCoreDataSetEnglandTest, _comparisionCoreDataSetSubNationalTest };

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), new GroupDataReader(),
                new AreasReader(), new Grouping());
            var isFiltered = bodyPeriodSearchContainer.FilterChildCoreDataByChildAreaCodeList(ref _comparision);

            Assert.IsFalse(isFiltered);
            Assert.IsTrue(_comparision.Count == 2);
            Assert.IsTrue(_comparision.All(coreDataSet => comparisionCloned.Contains(coreDataSet)));
        }

        [TestMethod]
        public void ShouldFilterChildCoreDataByChildAreaCodeListTest()
        {
            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, null } }, new List<string>{"SubNationalTest"});
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetSubNationalTest };

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), new GroupDataReader(),
                new AreasReader(), new Grouping());
            var isFiltered = bodyPeriodSearchContainer.FilterChildCoreDataByChildAreaCodeList(ref _comparision);

            Assert.IsTrue(isFiltered);
            Assert.IsTrue(_comparision.Count == 1);
            Assert.AreEqual(_comparision.FirstOrDefault(), comparisionExpected.FirstOrDefault());
        }
        [TestMethod]
        public void ShouldNotFilterAnyParentCoreDataByChildAreaCodeListTest()
        {
            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, null } });
            var comparisionCloned = new List<CoreDataSet> { _comparisionCoreDataSetEnglandTest, _comparisionCoreDataSetSubNationalTest };

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), new GroupDataReader(),
                new AreasReader(), new Grouping());
            var isFiltered = bodyPeriodSearchContainer.FilterParentCoreDataByChildAreaCodeList(ref _comparision);

            Assert.IsFalse(isFiltered);
            Assert.IsTrue(_comparision.Count == 2);
            Assert.IsTrue(_comparision.All(coreDataSet => comparisionCloned.Contains(coreDataSet)));
        }

        [TestMethod]
        public void ShouldFilterParentCoreDataByChildAreaCodeListTest()
        {
            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, null } }, new List<string> { "SubNationalTest" });
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetSubNationalTest };

            _areasReaderMock.Setup(x => x.GetAreaFromCode(It.IsAny<string>())).Returns(new Area { Code = "SubNationalTest" });
            _areasReaderMock.Setup(x => x.GetChildAreas(It.IsAny<string>(), It.IsAny<int>())).Returns(new List<IArea>{ new Area{Code = "SubNationalTest" } });
            _areasReaderMock.Setup(x => x.GetParentAreasFromChildAreaId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Dictionary<string, Area>{{ "SubNationalTest", new Area {Code = "SubNationalTest" } }});

            _exportAreaHelper.Init();
            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), new GroupDataReader(),
                new AreasReader(), new Grouping());
            var isFiltered = bodyPeriodSearchContainer.FilterParentCoreDataByChildAreaCodeList(ref _comparision);

            Assert.IsTrue(isFiltered);
            Assert.IsTrue(_comparision.Count == 1);
            Assert.AreEqual(_comparision.FirstOrDefault(), comparisionExpected.FirstOrDefault());
        }

        [TestMethod]
        public void ShouldNotFilterCoreDataByInequalitiesNationalTest()
        {
            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, null } }, new List<string> { "SubNationalTest" });
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetSubNationalTest };

            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(),It.IsAny<TimePeriod>(),It.IsAny<IList<int>>(),It.IsAny<string[]>())).Returns(comparisionExpected);

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), _groupDataReaderMock.Object,
                _areasReaderMock.Object, new Grouping());
            var resultDataCoreSetList = bodyPeriodSearchContainer.FilterCoreDataByInequalitiesNational(1,new TimePeriod(), new[]{"areaTest"});

            Assert.IsNotNull(resultDataCoreSetList);
            Assert.AreEqual(comparisionExpected, resultDataCoreSetList);
        }

        [TestMethod]
        public void ShouldNotFilterCoreDataByInequalitiesSubNationalTest()
        {
            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, null } }, new List<string> { "SubNationalTest" });
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetSubNationalTest };

            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(comparisionExpected);

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), _groupDataReaderMock.Object,
                _areasReaderMock.Object, new Grouping());
            var resultDataCoreSetList = bodyPeriodSearchContainer.FilterCoreDataByInequalitiesSubNational(1, new TimePeriod(), new[] { "areaTest" });

            Assert.IsNotNull(resultDataCoreSetList);
            Assert.AreEqual(comparisionExpected, resultDataCoreSetList);
        }

        [TestMethod]
        public void ShouldNotFilterCoreDataByInequalitiesLocalTest()
        {
            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, null } }, new List<string> { "SubNationalTest" });
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetSubNationalTest };

            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(comparisionExpected);

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), _groupDataReaderMock.Object,
                _areasReaderMock.Object, new Grouping());
            var resultDataCoreSetList = bodyPeriodSearchContainer.FilterCoreDataByInequalitiesLocal(1, new TimePeriod(), new[] { "areaTest" });

            Assert.IsNotNull(resultDataCoreSetList);
            Assert.AreEqual(comparisionExpected, resultDataCoreSetList);
        }

        [TestMethod]
        public void ShouldFilterCoreDataByInequalitiesLocalTest()
        {
            var _comparisionCoreDataSetEngland = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Female };
            var _comparisionCoreDataSetNational = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Male };
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetEngland, _comparisionCoreDataSetNational };

            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, new List<InequalitySearch> {new InequalitySearch(-1,-1,2,1) } } }, new List<string> { "EnglandTest" });

            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(comparisionExpected);

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), _groupDataReaderMock.Object,
                _areasReaderMock.Object, new Grouping());
            var resultDataCoreSetList = bodyPeriodSearchContainer.FilterCoreDataByInequalitiesLocal(1, new TimePeriod(), new[] { "areaTest" });

            Assert.IsNotNull(resultDataCoreSetList);
            Assert.IsTrue( resultDataCoreSetList.All(data => data.SexId == _comparisionCoreDataSetEngland.SexId &&
                                                             data.AgeId == _comparisionCoreDataSetEngland.AgeId &&
                                                             data.CategoryTypeId == _comparisionCoreDataSetEngland.CategoryTypeId &&
                                                             data.CategoryId == _comparisionCoreDataSetEngland.CategoryId));
        }

        [TestMethod]
        public void ShouldFilterCoreDataByInequalitiesNationalTest()
        {
            var _comparisionCoreDataSetEngland = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Female };
            var _comparisionCoreDataSetNational = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Male };
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetEngland, _comparisionCoreDataSetNational };

            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, new List<InequalitySearch> { new InequalitySearch(-1, -1, 2, 1) } } }, new List<string> { "EnglandTest" });

            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(comparisionExpected);

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), _groupDataReaderMock.Object,
                _areasReaderMock.Object, new Grouping());
            var resultDataCoreSetList = bodyPeriodSearchContainer.FilterCoreDataByInequalitiesNational(1, new TimePeriod(), new[] { "areaTest" });

            Assert.IsNotNull(resultDataCoreSetList);
            Assert.IsTrue(resultDataCoreSetList.All(data => data.SexId == _comparisionCoreDataSetEngland.SexId &&
                                                            data.AgeId == _comparisionCoreDataSetEngland.AgeId &&
                                                            data.CategoryTypeId == _comparisionCoreDataSetEngland.CategoryTypeId &&
                                                            data.CategoryId == _comparisionCoreDataSetEngland.CategoryId));
        }

        [TestMethod]
        public void ShouldFilterCoreDataByInequalitiesSubNationalTest()
        {
            var _comparisionCoreDataSetEngland = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Female };
            var _comparisionCoreDataSetNational = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Male };
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetEngland, _comparisionCoreDataSetNational };

            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, new List<InequalitySearch> { new InequalitySearch(-1, -1, 2, 1) } } }, new List<string> { "EnglandTest" });

            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(comparisionExpected);

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), _groupDataReaderMock.Object,
                _areasReaderMock.Object, new Grouping());
            var resultDataCoreSetList = bodyPeriodSearchContainer.FilterCoreDataByInequalitiesSubNational(1, new TimePeriod(), new[] { "areaTest" });

            Assert.IsNotNull(resultDataCoreSetList);
            Assert.IsTrue(resultDataCoreSetList.All(data => data.SexId == _comparisionCoreDataSetEngland.SexId &&
                                                            data.AgeId == _comparisionCoreDataSetEngland.AgeId &&
                                                            data.CategoryTypeId == _comparisionCoreDataSetEngland.CategoryTypeId &&
                                                            data.CategoryId == _comparisionCoreDataSetEngland.CategoryId));
        }

        [TestMethod]
        public void ShouldFilterCoreDataByInequalitiesCompletingCategoriesLocalTest()
        {
            var _comparisionCoreDataSetEngland = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Female };
            var _comparisionCoreDataSetNational = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Male };
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetEngland, _comparisionCoreDataSetNational };

            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, new List<InequalitySearch> { new InequalitySearch(-1,-1,Female, AllAges) } } }, new List<string> { "EnglandTest" });

            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(comparisionExpected);

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), _groupDataReaderMock.Object,
                _areasReaderMock.Object, new Grouping());
            var resultDataCoreSetList = bodyPeriodSearchContainer.FilterCoreDataByInequalitiesLocal(1, new TimePeriod(), new[] { "areaTest" });

            Assert.IsNotNull(resultDataCoreSetList);
            Assert.IsTrue(resultDataCoreSetList.All(data => data.SexId == _comparisionCoreDataSetEngland.SexId &&
                                                            data.AgeId == _comparisionCoreDataSetEngland.AgeId &&
                                                            data.CategoryTypeId == _comparisionCoreDataSetEngland.CategoryTypeId &&
                                                            data.CategoryId == _comparisionCoreDataSetEngland.CategoryId));
        }

        [TestMethod]
        public void ShouldFilterCoreDataByInequalitiesCompletingCategoriesNationalTest()
        {
            var _comparisionCoreDataSetEngland = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Female };
            var _comparisionCoreDataSetNational = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Male };
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetEngland, _comparisionCoreDataSetNational };

            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, new List<InequalitySearch> { new InequalitySearch (-1,-1, Female, AllAges) } } }, new List<string> { "EnglandTest" });

            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(comparisionExpected);

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), _groupDataReaderMock.Object,
                _areasReaderMock.Object, new Grouping());
            var resultDataCoreSetList = bodyPeriodSearchContainer.FilterCoreDataByInequalitiesNational(1, new TimePeriod(), new[] { "areaTest" });

            Assert.IsNotNull(resultDataCoreSetList);
            Assert.IsTrue(resultDataCoreSetList.All(data => data.SexId == _comparisionCoreDataSetEngland.SexId &&
                                                            data.AgeId == _comparisionCoreDataSetEngland.AgeId &&
                                                            data.CategoryTypeId == _comparisionCoreDataSetEngland.CategoryTypeId &&
                                                            data.CategoryId == _comparisionCoreDataSetEngland.CategoryId));
        }

        [TestMethod]
        public void ShouldFilterCoreDataByInequalitiesCompletingCategoriesSubNationalTest()
        {
            var _comparisionCoreDataSetEngland = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Female };
            var _comparisionCoreDataSetNational = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Male };
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetEngland, _comparisionCoreDataSetNational };

            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, new List<InequalitySearch> { new InequalitySearch(-1,-1, Female, AllAges) } } }, new List<string> { "EnglandTest" });

            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(comparisionExpected);

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), _groupDataReaderMock.Object,
                _areasReaderMock.Object, new Grouping());
            var resultDataCoreSetList = bodyPeriodSearchContainer.FilterCoreDataByInequalitiesSubNational(1, new TimePeriod(), new[] { "areaTest" });

            Assert.IsNotNull(resultDataCoreSetList);
            Assert.IsTrue(resultDataCoreSetList.All(data => data.SexId == _comparisionCoreDataSetEngland.SexId &&
                                                            data.AgeId == _comparisionCoreDataSetEngland.AgeId &&
                                                            data.CategoryTypeId == _comparisionCoreDataSetEngland.CategoryTypeId &&
                                                            data.CategoryId == _comparisionCoreDataSetEngland.CategoryId));
        }

        [TestMethod]
        public void ShouldFilterCoreDataByInequalitiesCompletingGroupingNationalTest()
        {
            var _comparisionCoreDataSetEngland = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Female };
            var _comparisionCoreDataSetNational = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Male };
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetEngland, _comparisionCoreDataSetNational };

            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, new List<InequalitySearch> { new InequalitySearch(-1,-1) } } }, new List<string> { "EnglandTest" });

            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(comparisionExpected);

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), _groupDataReaderMock.Object,
                _areasReaderMock.Object, new Grouping { AgeId = AllAges, SexId = Female });
            var resultDataCoreSetList = bodyPeriodSearchContainer.FilterCoreDataByInequalitiesNational(1, new TimePeriod(), new[] { "areaTest" });

            Assert.IsNotNull(resultDataCoreSetList);
            Assert.IsTrue(resultDataCoreSetList.All(data => data.SexId == _comparisionCoreDataSetEngland.SexId &&
                                                            data.AgeId == _comparisionCoreDataSetEngland.AgeId &&
                                                            data.CategoryTypeId == _comparisionCoreDataSetEngland.CategoryTypeId &&
                                                            data.CategoryId == _comparisionCoreDataSetEngland.CategoryId));
        }

        [TestMethod]
        public void ShouldFilterCoreDataByInequalitiesCompletingGroupingSubNationalTest()
        {
            var _comparisionCoreDataSetEngland = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Female };
            var _comparisionCoreDataSetNational = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Male };
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetEngland, _comparisionCoreDataSetNational };

            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, new List<InequalitySearch> { new InequalitySearch(-1, -1) } } }, new List<string> { "EnglandTest" });

            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(comparisionExpected);

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), _groupDataReaderMock.Object,
                _areasReaderMock.Object, new Grouping { AgeId = AllAges, SexId = Female });
            var resultDataCoreSetList = bodyPeriodSearchContainer.FilterCoreDataByInequalitiesSubNational(1, new TimePeriod(), new[] { "areaTest" });

            Assert.IsNotNull(resultDataCoreSetList);
            Assert.IsTrue(resultDataCoreSetList.All(data => data.SexId == _comparisionCoreDataSetEngland.SexId &&
                                                            data.AgeId == _comparisionCoreDataSetEngland.AgeId &&
                                                            data.CategoryTypeId == _comparisionCoreDataSetEngland.CategoryTypeId &&
                                                            data.CategoryId == _comparisionCoreDataSetEngland.CategoryId));
        }

        [TestMethod]
        public void ShouldFilterCoreDataByInequalitiesCompletingGroupingLocalTest()
        {
            var _comparisionCoreDataSetEngland = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Female };
            var _comparisionCoreDataSetNational = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Male };
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetEngland, _comparisionCoreDataSetNational };

            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, new List<InequalitySearch> { new InequalitySearch(-1, -1) } } }, new List<string> { "EnglandTest" });

            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(comparisionExpected);

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), _groupDataReaderMock.Object,
                _areasReaderMock.Object, new Grouping { AgeId = AllAges, SexId = Female });
            var resultDataCoreSetList = bodyPeriodSearchContainer.FilterCoreDataByInequalitiesLocal(1, new TimePeriod(), new[] { "areaTest" });

            Assert.IsNotNull(resultDataCoreSetList);
            Assert.IsTrue(resultDataCoreSetList.All(data => data.SexId == _comparisionCoreDataSetEngland.SexId &&
                                                            data.AgeId == _comparisionCoreDataSetEngland.AgeId &&
                                                            data.CategoryTypeId == _comparisionCoreDataSetEngland.CategoryTypeId &&
                                                            data.CategoryId == _comparisionCoreDataSetEngland.CategoryId));
        }

        [TestMethod]
        public void ShouldThrowAnExceptionWhenInequalityIsEmptyNationalTest()
        {
            var _comparisionCoreDataSetEngland = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Female };
            var _comparisionCoreDataSetNational = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Male };
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetEngland, _comparisionCoreDataSetNational };

            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, new List<InequalitySearch> { new InequalitySearch(-1,-1) } } }, new List<string> { "EnglandTest" });

            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(comparisionExpected);

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), _groupDataReaderMock.Object,
                _areasReaderMock.Object, new Grouping { AgeId = AllAges, SexId = Female });

            try
            {
                bodyPeriodSearchContainer.FilterCoreDataByInequalitiesNational(1, new TimePeriod(), new[] { "areaTest" });
                Assert.Fail("The method should throw an exception");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message != string.Empty);
            }
        }

        [TestMethod]
        public void ShouldThrowAnExceptionWhenInequalityIsEmptySubNationalTest()
        {
            var _comparisionCoreDataSetEngland = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Female };
            var _comparisionCoreDataSetNational = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Male };
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetEngland, _comparisionCoreDataSetNational };

            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, new List<InequalitySearch> { new InequalitySearch(-1, -1) } } }, new List<string> { "EnglandTest" });

            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(comparisionExpected);

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), _groupDataReaderMock.Object,
                _areasReaderMock.Object, new Grouping { AgeId = AllAges, SexId = Female });

            try
            {
                bodyPeriodSearchContainer.FilterCoreDataByInequalitiesSubNational(1, new TimePeriod(), new[] { "areaTest" });
                Assert.Fail("The method should throw an exception");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message != string.Empty);
            }
        }

        [TestMethod]
        public void ShouldThrowAnExceptionWhenInequalityIsEmptyLocalTest()
        {
            var _comparisionCoreDataSetEngland = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Female };
            var _comparisionCoreDataSetNational = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryTypeUndefined, CategoryId = CategoryUndefined, AreaCode = "EnglandTest", SexId = Male };
            var comparisionExpected = new List<CoreDataSet> { _comparisionCoreDataSetEngland, _comparisionCoreDataSetNational };

            var onDemandDataWrapper = new OnDemandQueryParametersWrapper(1, new List<int> { 1 }, new Dictionary<int, IList<InequalitySearch>> { { 1, new List<InequalitySearch> { new InequalitySearch(-1,-1) } } }, new List<string> { "EnglandTest" });

            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(), It.IsAny<TimePeriod>(), It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(comparisionExpected);

            var bodyPeriodSearchContainer = new BodyPeriodSearchContainer(_exportAreaHelper, _indicatorExportParameters, onDemandDataWrapper, new CsvBuilderAttributesForPeriodsWrapper(), _groupDataReaderMock.Object,
                _areasReaderMock.Object, new Grouping { AgeId = AllAges, SexId = Female });

            try
            {
                bodyPeriodSearchContainer.FilterCoreDataByInequalitiesLocal(1, new TimePeriod(), new[] { "areaTest" });
                Assert.Fail("The method should throw an exception");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message != string.Empty);
            }
        }
    }
}
