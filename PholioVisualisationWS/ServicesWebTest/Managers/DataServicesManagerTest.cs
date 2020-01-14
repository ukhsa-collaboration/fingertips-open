using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.ServicesWeb.Managers;
using PholioVisualisation.ServicesWeb.Validations;
using System;
using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesWebTest.Managers
{
    [TestClass]
    public class DataServicesManagerTest
    {
        private int _childAreaTypeId;
        private int _parentAreaTypeId;
        private string _parentAreaCode;
        private string _indicatorsIds;
        private string _areasCode;
        private string _categoryAreaCode;
        private int? _profileId;
        private int _groupId;
        private string _inequalities;
        private string _sexId;
        private string _ageId;
        private DataServicesParameters _receivedParameters;
        private DataServicesManager _manager;
        
        private Mock<IGroupDataReader> _groupDataReaderMock;
        private Mock<IProfileReader> _profileReaderMock;
        private Mock<IAreasReader> _areasReaderMock;

        private IList<Grouping> _groupingList;
        private IList<GroupingMetadata> _groupingMetadataList;

        private IList<CoreDataSet> coreDataSetList;

        [TestInitialize]
        public void StartUp()
        {
            _childAreaTypeId = Int32.MaxValue;
            _parentAreaTypeId = Int32.MinValue;
            _parentAreaCode = "parentAreaCodeTest";
            _indicatorsIds = "1";
            _areasCode = "areaCodeTest";
            _categoryAreaCode = null;
            _profileId = 1;
            _groupId = 1;
            _inequalities = @"{
                                '1': [
                                            {
                                                'CategoryTypeId': 4,
                                                'CategoryId': 1,
                                                'Sex': 4,
                                                'Age': 204
                                            }
                                         ]
                              }";
            _sexId = SexIds.Persons.ToString();
            _ageId = AgeIds.AllAges.ToString();
            _groupingMetadataList = new List<GroupingMetadata> { new GroupingMetadata { Id = 1, Name = "DevelopmentTest", ProfileId = (int)_profileId, Sequence = 1 } };
            _groupingList = new List<Grouping> {new Grouping()};

            _groupDataReaderMock = new Mock<IGroupDataReader>();
            _profileReaderMock = new Mock<IProfileReader>();
            _areasReaderMock = new Mock<IAreasReader>();

            coreDataSetList = new List<CoreDataSet>{ new CoreDataSet{ CategoryTypeId = CategoryTypeIds.DeprivationDecileCcg2010, CategoryId =  CategoryIds.LeastDeprivedQuintile }};
        }

        [TestCleanup]
        public void TearDown()
        {
            _groupDataReaderMock.VerifyAll();
            _profileReaderMock.VerifyAll();
            _areasReaderMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldNotThrowExceptionBuildPropertiesForAllDataFileForIndicatorList()
        {
            _groupDataReaderMock.Setup(x => x.GetGroupingsByIndicatorId(It.IsAny<int>())).Returns(_groupingList);
            _groupDataReaderMock.Setup(x => x.GetCoreDataForIndicatorId(It.IsAny<int>())).Returns(coreDataSetList);

            try
            {
                _receivedParameters = new DataServicesParameters(DataServiceUse.AllDataFileForIndicatorList, _childAreaTypeId, _parentAreaTypeId, _parentAreaCode, _sexId, _ageId, _indicatorsIds, _areasCode,
                    _categoryAreaCode, _profileId, _groupId, _inequalities);

                _manager = new DataServicesManager(_receivedParameters, _groupDataReaderMock.Object, _profileReaderMock.Object, _areasReaderMock.Object);

                var resultExportParameters = _manager.ExportParameters;
                var resultOnDemandParameters = _manager.OnDemandParameters;

                Assert.IsNotNull(resultExportParameters);
                Assert.IsNotNull(resultOnDemandParameters);
            }
            catch (Exception)
            {
                Assert.Fail("It should not throw an exception");
            }
        }

        [TestMethod]
        public void ShouldNotThrowExceptionBuildPropertiesForAllDataFileForGroup()
        {
            _groupDataReaderMock.Setup(x => x.GetGroupingMetadataList(It.IsAny<IList<int>>())).Returns(_groupingMetadataList);
            _groupDataReaderMock.Setup(x => x.GetGroupingsByGroupId(It.IsAny<int>())).Returns(_groupingList);

            try
            {
               _receivedParameters = new DataServicesParameters(DataServiceUse.AllDataFileForGroup, _childAreaTypeId, _parentAreaTypeId, _parentAreaCode, _sexId, _ageId, _indicatorsIds, _areasCode,
                    _categoryAreaCode, _profileId, _groupId, _inequalities);

                _manager = new DataServicesManager(_receivedParameters, _groupDataReaderMock.Object, _profileReaderMock.Object, _areasReaderMock.Object);

                var resultExportParameters = _manager.ExportParameters;
                var resultOnDemandParameters = _manager.OnDemandParameters;

                Assert.IsNotNull(resultExportParameters);
                Assert.IsNotNull(resultOnDemandParameters);
            }
            catch (Exception)
            {
                Assert.Fail("It should not throw an exception");
            }
        }

        [TestMethod]
        public void ShouldNotThrowExceptionBuildPropertiesForAllDataFileForProfile()
        {
            try
            {
                _receivedParameters = new DataServicesParameters(DataServiceUse.AllDataFileForProfile, _childAreaTypeId, _parentAreaTypeId, _parentAreaCode, _sexId, _ageId, _indicatorsIds, _areasCode,
                    _categoryAreaCode, _profileId, _groupId, _inequalities);

                _manager = new DataServicesManager(_receivedParameters, _groupDataReaderMock.Object, _profileReaderMock.Object, _areasReaderMock.Object);

                var resultExportParameters = _manager.ExportParameters;
                var resultOnDemandParameters = _manager.OnDemandParameters;

                Assert.IsNotNull(resultExportParameters);
                Assert.IsNotNull(resultOnDemandParameters);
            }
            catch (Exception)
            {
                Assert.Fail("It should not throw an exception");
            }
        }
    }
}
