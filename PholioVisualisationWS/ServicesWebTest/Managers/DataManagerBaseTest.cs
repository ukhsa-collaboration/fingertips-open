using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export;
using PholioVisualisation.Export.FileBuilder.SupportModels;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Managers;
using PholioVisualisation.ServicesWebTest.Helpers;

namespace PholioVisualisation.ServicesWebTest.Managers
{
    [TestClass]
    public class DataManagerBaseTest
    {
        private int _profileId;
        private IList<int> _indicatorIdList;
        private IDictionary<int, IList<InequalitySearch>> _inequalities;
        private IList<string> _childAreasCodeList;
        private IList<int> _groupIdList;
        private bool _allPeriods;
        private string[] _categoryAreaCodeArray;
        private int _sexId;
        private int _ageId;

        private int _childAreaTypeId;
        private int _parentAreaTypeId;
        private string _parentAreaCode;

        private DataManagerBase _manager;

        private Mock<IGroupDataReader> _groupDataReaderMock;
        private Mock<IAreasReader> _areasReaderMock;
        private Mock<IProfileReader> _profileReaderMock;

        private IList<GroupingInequality> _indicatorAreaTypeGroupList;
        private IList<Grouping> _groupingList;
        private IList<GroupingMetadata> _groupingMetadataList;

        

        [TestInitialize]
        public void StartUp()
        {
            _profileId = 1;
            _indicatorIdList = new List<int> {1};
            _inequalities = new Dictionary<int, IList<InequalitySearch>>{ { _indicatorIdList.First(), new List<InequalitySearch> {new InequalitySearch(1, 1, 1, 1)} } };
            _childAreasCodeList = new List<string> {"childAreaCodeTest"};
            _groupIdList = new List<int> {1};
            _allPeriods = true;
            _categoryAreaCodeArray = new[] {"cat-01-01"};
            _sexId = 1;
            _ageId = 1;

            _childAreaTypeId = 1;
            _parentAreaTypeId = 1;
            _parentAreaCode = "parentAreaCodeTest";

            _manager = new DataManagerBase();

            _groupDataReaderMock = new Mock<IGroupDataReader>();
            _areasReaderMock = new Mock<IAreasReader>();
            _profileReaderMock = new Mock<IProfileReader>();

            _indicatorAreaTypeGroupList = new List<GroupingInequality> {new GroupingInequality(1, 1, 1, 1, 1)};

            _groupingMetadataList = new List<GroupingMetadata> { new GroupingMetadata { Id = 1, Name = "DevelopmentTest", ProfileId = _profileId, Sequence = 1 } };
            _groupingList = new List<Grouping> { new Grouping() };
        }

        [TestMethod]
        public void ShouldBeNullExportParameters()
        {
            Assert.IsNull(_manager.ExportParameters);
        }

        [TestMethod]
        public void ShouldNotBeNullExportParameters()
        {
            _manager.SetExportParameters(_profileId, _childAreaTypeId, _parentAreaTypeId, _parentAreaCode);

            Assert.IsNotNull(_manager.ExportParameters);
        }

        [TestMethod]
        public void ShouldBeNullOnDemandParameters()
        {
            Assert.IsNull(_manager.OnDemandParameters);
        }

        [TestMethod]
        public void ShouldNotBeNullOnDemandParameters()
        {
            _manager.SetOnDemandParameters(_profileId, _indicatorIdList, _inequalities,_childAreasCodeList,_groupIdList, _allPeriods, _categoryAreaCodeArray);

            Assert.IsNotNull(_manager.OnDemandParameters);
        }

        [TestMethod]
        public void ShouldNotBeNullDictionaryInequalities()
        {
            _groupDataReaderMock.Setup(x => x.GetGroupingListByGroupIdIndicatorIdAreaType(_groupIdList.First(), _indicatorIdList.First(), It.IsAny<int>())).Returns(_groupingList);

            _areasReaderMock.Setup(x => x.GetCategory(1, 1)).Returns(new Category{CategoryTypeId = 1, Id = 1, Name = "TestName", ShortName = "ShortNameTest"});

            var resultDic = DataManagerBase.GetDictionaryInequalities(_groupDataReaderMock.Object, _areasReaderMock.Object, _indicatorAreaTypeGroupList, _categoryAreaCodeArray, _inequalities);

            Assert.IsNotNull(resultDic);
        }

        [TestMethod]
        public void ShouldGetProfileIdByGroupId()
        {
            _groupDataReaderMock.Setup(x => x.GetGroupingMetadataList(_groupIdList)).Returns(_groupingMetadataList);

            var result = DataManagerBase.GetProfileIdByGroupId(_groupDataReaderMock.Object, _groupIdList.First());

            Assert.AreEqual(_profileId, result);
        }

        [TestMethod]
        public void ShouldGetIndicatorIdListProvider()
        {
            var result = DataManagerBase.GetIndicatorIdListProvider(_groupDataReaderMock.Object, _profileReaderMock.Object);

            Assert.IsTrue(result is IndicatorIdListProvider);
        }

        [TestMethod]
        public void ShouldGetIndicatorAreaTypeGroupIdList()
        {
            var sexIdList = new List<int> { _sexId };
            var ageIdList = new List<int> { _ageId };
            var result = DataManagerBase.GetIndicatorAreaTypeGroupIdList(_indicatorIdList, _indicatorAreaTypeGroupList.First().AreaTypeId, _groupIdList.First(), sexIdList, ageIdList);

            AssertHelper.AreEqual(_indicatorAreaTypeGroupList, result);
        }
    }
}
