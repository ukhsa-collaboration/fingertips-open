using System.Collections.Generic;
using System.Linq;
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
    public class BodyPeriodComparisonContainerTest
    {
        private Mock<IAreasReader> _areaTypeMock;
        private Mock<QuartilesComparer> _indicatorComparerMock;
        private CoreDataSet _expectedCoreDataSetRef;
        private CoreDataSet _comparisionCoreDataSet;
        private IList<CoreDataSet> _comparision;
        private IList<CoreDataSet> _comparisionForRef;

        private const int AllAges = AgeIds.AllAges;
        private const int From80To84 = AgeIds.From80To84;
        private const int CategoryUndefined = CategoryTypeIds.Undefined;
        private const int CategoryDeprivationDecileGp2010 = CategoryTypeIds.DeprivationDecileGp2010;

        [TestInitialize]
        public void SetUp()
        {
            _areaTypeMock = new Mock<IAreasReader>(MockBehavior.Strict);
            _indicatorComparerMock = new Mock<QuartilesComparer>(MockBehavior.Strict);
            _expectedCoreDataSetRef = new CoreDataSet { AgeId = From80To84, CategoryTypeId = CategoryUndefined };
            _comparisionCoreDataSet = new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryUndefined };
            _comparision = new List<CoreDataSet> { _comparisionCoreDataSet };
            _comparisionForRef = new List<CoreDataSet> { _expectedCoreDataSetRef };
        }

        [TestCleanup]
        public void CleanUp()
        {
            _areaTypeMock.VerifyAll();
            _indicatorComparerMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldCloneCoreDataForComparisonTest()
        {
            const int ageIdValueToClone = 1;
            const int ageIdValueToChange = 2;
            var coreDataSetList = new List<CoreDataSet> {new CoreDataSet {AgeId = ageIdValueToClone } };

            var clonedCoreDataList = BodyPeriodComparisonContainer.CloneCoreDataForComparison(coreDataSetList);
            coreDataSetList[0].AgeId = ageIdValueToChange;

            Assert.AreEqual(clonedCoreDataList[0].AgeId, ageIdValueToClone);
            Assert.AreEqual(coreDataSetList[0].AgeId, ageIdValueToChange);
        }

        [TestMethod]
        public void ShouldGetCategoryComparisonManagerTest()
        {
            var indicatorExportParameters = new IndicatorExportParameters {ParentAreaCode = AreaCodes.England};
            var areaFactory = new AreaFactory(_areaTypeMock.Object);
            var exportAreaHelper = new ExportAreaHelper(_areaTypeMock.Object, indicatorExportParameters);

            var bodyPeriodComparisonContainer = new BodyPeriodComparisonContainer(exportAreaHelper, _indicatorComparerMock.Object);
            var comparision = new List<CoreDataSet>{ new CoreDataSet { AgeId = AllAges }};

            _areaTypeMock.Setup(x => x.GetAreaFromCode(It.IsAny<string>())).Returns(new Area());
            _areaTypeMock.Setup(x => x.GetChildAreas(It.IsAny<string>(), It.IsAny<int>())).Returns(new List<IArea>());
            _areaTypeMock.Setup(x => x.GetParentAreasFromChildAreaId(It.IsAny<int>(), It.IsAny<int>())).Returns(new Dictionary<string, Area>());

            exportAreaHelper.Init();
            var result = bodyPeriodComparisonContainer.GetCategoryComparisonManager(comparision, comparision, comparision);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CategoryComparisonManager));
        }

        [TestMethod]
        public void ShouldGetCoreDataForComparisonToWriteAndNewComparisionRefTest()
        {
            _comparision = new List<CoreDataSet> { new CoreDataSet { AgeId = AllAges, CategoryTypeId = CategoryDeprivationDecileGp2010 } };
            IList<CoreDataSet> comparisionForRef = null;
            var bodyPeriodComparisonContainer = BodyPeriodComparisonContainer.GetCoreDataForComparisonToWrite(ref comparisionForRef, _comparision, CategoryUndefined);

            Assert.IsNotNull(bodyPeriodComparisonContainer);
            Assert.IsTrue(comparisionForRef.Count == 0);
            Assert.IsTrue(bodyPeriodComparisonContainer.ToList().Count == 0);
        }

        [TestMethod]
        public void ShouldnotGetCoreDataForComparisonToWriteAndNewComparisionRefTest()
        {
            IList<CoreDataSet> comparisionForRef = null;
            var bodyPeriodComparisonContainer = BodyPeriodComparisonContainer.GetCoreDataForComparisonToWrite(ref comparisionForRef, _comparision, CategoryUndefined);

            Assert.IsNotNull(bodyPeriodComparisonContainer);
            Assert.AreEqual(_comparision.FirstOrDefault(), bodyPeriodComparisonContainer.FirstOrDefault());
            Assert.AreEqual(_comparision.FirstOrDefault(), comparisionForRef.FirstOrDefault());
        }

        [TestMethod]
        public void ShouldGetCoreDataForComparisonToWriteTest()
        {
            var bodyPeriodComparisonContainer = BodyPeriodComparisonContainer.GetCoreDataForComparisonToWrite(ref _comparisionForRef, _comparision, CategoryUndefined);

            Assert.IsNotNull(bodyPeriodComparisonContainer);
            Assert.AreEqual(_expectedCoreDataSetRef, bodyPeriodComparisonContainer.FirstOrDefault());
            Assert.AreEqual(_comparisionCoreDataSet, _comparisionForRef.FirstOrDefault());
        }
    }
}
