using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.Export.FileBuilder.Containers;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest.FileBuilder.Containers
{
    [TestClass]
    public class BodyPeriodSignificanceContainerTest
    {
        private Mock<IAreasReader> _areasReaderMock;
        private Mock<QuartilesComparer> _indicatorComparerMock;
        private CoreDataSet _comparisionCoreDataSet;
        private List<CoreDataSet> _comparision;
        private CategoryComparisonManager _categoryComparisonManager;

        private const int CategoryTypeUndefined = CategoryTypeIds.Undefined;
        private const int CategoryDeprivationDecileGp2010 = CategoryTypeIds.DeprivationDecileGp2010;
        private const int Male = SexIds.Male;

        [TestInitialize]
        public void SetUp()
        {
            _areasReaderMock = new Mock<IAreasReader>(MockBehavior.Strict);
            _indicatorComparerMock = new Mock<QuartilesComparer>(MockBehavior.Strict);

            _comparisionCoreDataSet = new CoreDataSet { AgeId = Male, CategoryTypeId = CategoryTypeUndefined, AreaCode = "areaTest" };
            _comparision = new List<CoreDataSet> { _comparisionCoreDataSet };

            _categoryComparisonManager = new CategoryComparisonManager(new QuintilesComparer(), _comparision);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _areasReaderMock.VerifyAll();
            _indicatorComparerMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldGetSignificanceTest()
        {
            var indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            var areaFactory = new AreaFactory(_areasReaderMock.Object);
            var exportAreaHelper = new ExportAreaHelper(_areasReaderMock.Object, indicatorExportParameters, areaFactory);

            var bodyPeriodSignificanceContainer = new BodyPeriodSignificanceContainer(exportAreaHelper, _indicatorComparerMock.Object);
            var significance = bodyPeriodSignificanceContainer.GetSignificance(new IndicatorMetadata(), _comparisionCoreDataSet,_comparision,new Area());

            Assert.IsNotNull(significance);
            Assert.IsInstanceOfType(significance, typeof(Significance));
        }

        [TestMethod]
        public void ShouldFindSignificanceTest()
        {
            var indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            var areaFactory = new AreaFactory(_areasReaderMock.Object);
            var exportAreaHelper = new ExportAreaHelper(_areasReaderMock.Object, indicatorExportParameters, areaFactory);

            var bodyPeriodSignificanceContainer = new BodyPeriodSignificanceContainer(exportAreaHelper, null);
            var significance = bodyPeriodSignificanceContainer.GetSignificance(new IndicatorMetadata(), _comparisionCoreDataSet, _comparision, new Area());

            Assert.IsNotNull(significance);
            Assert.IsInstanceOfType(significance, typeof(Significance));
        }

        [TestMethod]
        public void ShouldGetSignificanceWithCategoryComparisonManagerTest()
        {
            var indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            var areaFactory = new AreaFactory(_areasReaderMock.Object);
            var exportAreaHelper = new ExportAreaHelper(_areasReaderMock.Object, indicatorExportParameters, areaFactory);

            var bodyPeriodSignificanceContainer = new BodyPeriodSignificanceContainer(exportAreaHelper, _indicatorComparerMock.Object);
            var significance = bodyPeriodSignificanceContainer.GetSignificance(new CategoryComparisonManager(new QuintilesComparer(), _comparision), _comparision, new IndicatorMetadata(), _comparisionCoreDataSet);

            Assert.IsNotNull(significance);
            Assert.IsInstanceOfType(significance, typeof(Significance));
        }

        [TestMethod]
        public void ShouldGetCoreDataForComparisonTest()
        {
            var comparisionCoreDataSetLocal = new CoreDataSet { AgeId = Male, CategoryTypeId = CategoryTypeUndefined, AreaCode = "areaTest" };
            var comparisionLocal = new List<CoreDataSet> { comparisionCoreDataSetLocal };
            var coreDataSet = BodyPeriodSignificanceContainer.GetCoreDataForComparison(comparisionLocal);

            Assert.IsTrue(coreDataSet.Count == 1);
        }

        [TestMethod]
        public void ShouldGetNullCoreDataForComparisonTest()
        {
            var comparisionCoreDataSetLocal = new CoreDataSet { AgeId = Male, CategoryTypeId = CategoryDeprivationDecileGp2010, AreaCode = "areaTest" };
            var comparisionLocal = new List<CoreDataSet> { comparisionCoreDataSetLocal };
            var coreDataSet = BodyPeriodSignificanceContainer.GetCoreDataForComparison(comparisionLocal);

            Assert.IsTrue(coreDataSet.Count == 0);
        }

        [TestMethod]
        public void ShouldGetSubNationalParentSignificanceIsParentEnglandTest()
        {
            var indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            var areaFactory = new AreaFactory(_areasReaderMock.Object);
            var exportAreaHelper = new ExportAreaHelper(_areasReaderMock.Object, indicatorExportParameters, areaFactory);

            _areasReaderMock.Setup(x => x.GetAreaFromCode(It.IsAny<string>())).Returns(new Area());

            var bodyPeriodSignificanceContainer = new BodyPeriodSignificanceContainer(exportAreaHelper, _indicatorComparerMock.Object);
            IArea parentArea;
            var significance = bodyPeriodSignificanceContainer.GetSubNationalParentSignificance( new Dictionary<string, Area>(), _categoryComparisonManager , _comparision,
                                                                                                 new IndicatorMetadata(), _comparisionCoreDataSet, true, out parentArea);
            Assert.IsNotNull(significance);
            Assert.AreEqual(significance, Significance.None);
            Assert.IsNotNull(parentArea);
        }

        [TestMethod]
        public void ShouldGetSubNationalParentSignificanceParentAreaMapNotFoundTest()
        {
            var indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            var areaFactory = new AreaFactory(_areasReaderMock.Object);
            var exportAreaHelper = new ExportAreaHelper(_areasReaderMock.Object, indicatorExportParameters, areaFactory);

            var bodyPeriodSignificanceContainer = new BodyPeriodSignificanceContainer(exportAreaHelper, _indicatorComparerMock.Object);
            IArea parentArea;
            var significance = bodyPeriodSignificanceContainer.GetSubNationalParentSignificance(new Dictionary<string, Area>(), null, _comparision, new IndicatorMetadata(), _comparisionCoreDataSet,
                                                                                                false, out parentArea);

            Assert.IsNotNull(significance);
            Assert.AreEqual(significance, Significance.None);
            Assert.IsNull(parentArea);
        }

        [TestMethod]
        public void ShouldGetSubNationalParentSignificanceTest()
        {
            var indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            var areaFactory = new AreaFactory(_areasReaderMock.Object);
            var exportAreaHelper = new ExportAreaHelper(_areasReaderMock.Object, indicatorExportParameters, areaFactory);
            var dictionaryAreas = new Dictionary<string, Area> { { "areaTest", new Area() } };

            var bodyPeriodSignificanceContainer = new BodyPeriodSignificanceContainer(exportAreaHelper, _indicatorComparerMock.Object);
            IArea parentArea;
            var significance = bodyPeriodSignificanceContainer.GetSubNationalParentSignificance(dictionaryAreas, _categoryComparisonManager, _comparision, new IndicatorMetadata(), _comparisionCoreDataSet, false, out parentArea);

            Assert.IsNotNull(significance);
            Assert.AreEqual(significance, Significance.None);
            Assert.IsNotNull(parentArea);
        }

        [TestMethod]
        public void ShouldGetSubNationalParentSignificanceNotComparisionManagerTest()
        {
            var indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            var areaFactory = new AreaFactory(_areasReaderMock.Object);
            var exportAreaHelper = new ExportAreaHelper(_areasReaderMock.Object, indicatorExportParameters, areaFactory);
            var dictionaryAreas = new Dictionary<string, Area> {{"areaTest", new Area()}};

            _indicatorComparerMock.Setup(x => x.Compare(It.IsAny<CoreDataSet>(), It.IsAny<CoreDataSet>(), It.IsAny<IndicatorMetadata>())).Returns(Significance.Best);

            var bodyPeriodSignificanceContainer = new BodyPeriodSignificanceContainer(exportAreaHelper, _indicatorComparerMock.Object);
            IArea parentArea;
            var significance = bodyPeriodSignificanceContainer.GetSubNationalParentSignificance(dictionaryAreas, null, _comparision, new IndicatorMetadata(), _comparisionCoreDataSet, false, out parentArea);

            Assert.IsNotNull(significance);
            Assert.AreEqual(significance, Significance.Best);
            Assert.IsNotNull(parentArea);
        }

        [TestMethod]
        public void ShouldTakeSignificanceBestTest()
        {
            var indicatorExportParameters = new IndicatorExportParameters {ParentAreaCode = AreaCodes.England};
            var areaFactory = new AreaFactory(_areasReaderMock.Object);
            var exportAreaHelper = new ExportAreaHelper(_areasReaderMock.Object, indicatorExportParameters, areaFactory);

            _indicatorComparerMock.Setup(x => x.Compare(It.IsAny<CoreDataSet>(), It.IsAny<CoreDataSet>(), It.IsAny<IndicatorMetadata>())).Returns(Significance.Best);

            var bodyPeriodSignificanceContainer = new BodyPeriodSignificanceContainer(exportAreaHelper, _indicatorComparerMock.Object);
            var significance = bodyPeriodSignificanceContainer.TakeSignificance(_comparisionCoreDataSet, _comparisionCoreDataSet, new IndicatorMetadata());

            Assert.IsNotNull(significance);
            Assert.AreEqual(significance, Significance.Best);
        }

        [TestMethod]
        public void ShouldTakeSignificanceNoneTest()
        {
            var indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            var areaFactory = new AreaFactory(_areasReaderMock.Object);
            var exportAreaHelper = new ExportAreaHelper(_areasReaderMock.Object, indicatorExportParameters, areaFactory);

            _indicatorComparerMock.Setup(x => x.Compare(It.IsAny<CoreDataSet>(), It.IsAny<CoreDataSet>(), It.IsAny<IndicatorMetadata>())).Returns(Significance.None);

            var bodyPeriodSignificanceContainer = new BodyPeriodSignificanceContainer(exportAreaHelper, _indicatorComparerMock.Object);
            var significance = bodyPeriodSignificanceContainer.TakeSignificance(_comparisionCoreDataSet, _comparisionCoreDataSet, new IndicatorMetadata());

            Assert.IsNotNull(significance);
            Assert.AreEqual(significance, Significance.None);
        }

        [TestMethod]
        public void ShouldTakeSignificanceBetterTest()
        {
            var indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            var areaFactory = new AreaFactory(_areasReaderMock.Object);
            var exportAreaHelper = new ExportAreaHelper(_areasReaderMock.Object, indicatorExportParameters, areaFactory);

            _indicatorComparerMock.Setup(x => x.Compare(It.IsAny<CoreDataSet>(), It.IsAny<CoreDataSet>(), It.IsAny<IndicatorMetadata>())).Returns(Significance.Better);

            var bodyPeriodSignificanceContainer = new BodyPeriodSignificanceContainer(exportAreaHelper, _indicatorComparerMock.Object);
            var significance = bodyPeriodSignificanceContainer.TakeSignificance(_comparisionCoreDataSet, _comparisionCoreDataSet, new IndicatorMetadata());

            Assert.IsNotNull(significance);
            Assert.AreEqual(significance, Significance.Better);
        }

        [TestMethod]
        public void ShouldTakeSignificanceSameTest()
        {
            var indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            var areaFactory = new AreaFactory(_areasReaderMock.Object);
            var exportAreaHelper = new ExportAreaHelper(_areasReaderMock.Object, indicatorExportParameters, areaFactory);

            _indicatorComparerMock.Setup(x => x.Compare(It.IsAny<CoreDataSet>(), It.IsAny<CoreDataSet>(), It.IsAny<IndicatorMetadata>())).Returns(Significance.Same);

            var bodyPeriodSignificanceContainer = new BodyPeriodSignificanceContainer(exportAreaHelper, _indicatorComparerMock.Object);
            var significance = bodyPeriodSignificanceContainer.TakeSignificance(_comparisionCoreDataSet, _comparisionCoreDataSet, new IndicatorMetadata());

            Assert.IsNotNull(significance);
            Assert.AreEqual(significance, Significance.Same);
        }

        [TestMethod]
        public void ShouldTakeSignificanceWorseTest()
        {
            var indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            var areaFactory = new AreaFactory(_areasReaderMock.Object);
            var exportAreaHelper = new ExportAreaHelper(_areasReaderMock.Object, indicatorExportParameters, areaFactory);

            _indicatorComparerMock.Setup(x => x.Compare(It.IsAny<CoreDataSet>(), It.IsAny<CoreDataSet>(), It.IsAny<IndicatorMetadata>())).Returns(Significance.Worse);

            var bodyPeriodSignificanceContainer = new BodyPeriodSignificanceContainer(exportAreaHelper, _indicatorComparerMock.Object);
            var significance = bodyPeriodSignificanceContainer.TakeSignificance(_comparisionCoreDataSet, _comparisionCoreDataSet, new IndicatorMetadata());

            Assert.IsNotNull(significance);
            Assert.AreEqual(significance, Significance.Worse);
        }

        [TestMethod]
        public void ShouldTakeSignificanceWorstTest()
        {
            var indicatorExportParameters = new IndicatorExportParameters { ParentAreaCode = AreaCodes.England };
            var areaFactory = new AreaFactory(_areasReaderMock.Object);
            var exportAreaHelper = new ExportAreaHelper(_areasReaderMock.Object, indicatorExportParameters, areaFactory);

            _indicatorComparerMock.Setup(x => x.Compare(It.IsAny<CoreDataSet>(), It.IsAny<CoreDataSet>(), It.IsAny<IndicatorMetadata>())).Returns(Significance.Worst);

            var bodyPeriodSignificanceContainer = new BodyPeriodSignificanceContainer(exportAreaHelper, _indicatorComparerMock.Object);
            var significance = bodyPeriodSignificanceContainer.TakeSignificance(_comparisionCoreDataSet, _comparisionCoreDataSet, new IndicatorMetadata());

            Assert.IsNotNull(significance);
            Assert.AreEqual(significance, Significance.Worst);
        }
    }
}
