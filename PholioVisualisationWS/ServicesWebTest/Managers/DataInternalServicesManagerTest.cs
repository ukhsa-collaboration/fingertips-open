using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.ServicesWeb.Managers;
using PholioVisualisation.ServicesWeb.Validations;
using System;
using System.Collections.Generic;
using Moq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesTest;

namespace PholioVisualisation.ServicesWebTest.Managers
{
    [TestClass]
    public class DataInternalServicesManagerTest
    {
        private int _childAreaTypeId;
        private int _parentAreaTypeId;
        private string _parentAreaCode;
        private string _indicatorsIds;
        private string _areasCode;
        private string _categoryAreaCode;
        private int _profileId;
        private int? _profileIdNullable;
        private int _groupId;
        private string _inequalities;
        private string _sexId;
        private string _ageId;
        private Mock<IGroupDataReader> _groupDataReaderMock;
        private Mock<IProfileReader> _profileReaderMock;
        private Mock<IAreasReader> _areasReaderMock;
        private IList<Grouping> _groupingList;
        private IList<GroupingMetadata>  _groupingMetadataList;

        private IList<CoreDataSet> coreDataSetList;

        [TestInitialize]
        public void StartUp()
        {
            _childAreaTypeId = TestHelper.GetRandomInt();
            _parentAreaTypeId = TestHelper.GetRandomInt();
            _parentAreaCode = "parentAreaCodeTest";
            _indicatorsIds = "1";
            _areasCode = "areaCodeTest";
            _categoryAreaCode = null;
            _profileId = TestHelper.GetRandomInt();
            _profileIdNullable = _profileId;
            _groupId = TestHelper.GetRandomInt();
            _inequalities = @"{
                                '1': [
                                        { 
                                            CategoryInequalitySearch: { 
                                                                        CategoryTypeId: 4, 
                                                                        CategoryId: 1 
                                                                      } , 
                                            SexAgeInequalitySearch: { 
                                                                        'Sex': 4,
                                                                        'Age': 204
                                                                    }
                                        }                             
                                     ]
                              }";
            _sexId = "4";
            _ageId = "204";
            _groupDataReaderMock = new Mock<IGroupDataReader>();
            _profileReaderMock =  new Mock<IProfileReader>();
            _areasReaderMock = new Mock<IAreasReader>();

            _groupingMetadataList = new List<GroupingMetadata> { new GroupingMetadata { Id = 1, Name = "DevelopmentTest", ProfileId = _profileId, Sequence = 1 } };
            _groupingList = new List<Grouping> { new Grouping { IndicatorId = 1, AreaTypeId = _childAreaTypeId } };

            coreDataSetList = new List<CoreDataSet> { new CoreDataSet { CategoryTypeId = CategoryTypeIds.DeprivationDecileCcg2010, CategoryId = CategoryIds.LeastDeprivedQuintile } };
        }

        [TestCleanup]
        public void TearDown()
        {
            _groupDataReaderMock.VerifyAll();
            _profileReaderMock.VerifyAll();
            _areasReaderMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldNotThrowExceptionBuildPropertiesForInitParametersForInitParametersForLatestDataFileForIndicator()
        {
            try
            {
                var receivedParameters = new DataInternalServicesParameters(DataInternalServiceUse.LatestDataFileForIndicator, _childAreaTypeId, _parentAreaTypeId, _sexId, _ageId, _parentAreaCode, _indicatorsIds, _areasCode,
                    _categoryAreaCode, _profileIdNullable, _groupId, _inequalities);

                var manager = new DataInternalServicesManager(receivedParameters, _groupDataReaderMock.Object, _profileReaderMock.Object, _areasReaderMock.Object);

                var resultExportParameters = manager.ExportParameters;
                var resultOnDemandParameters = manager.OnDemandParameters;

                Assert.IsNotNull(resultExportParameters);
                Assert.IsNotNull(resultOnDemandParameters);
            }
            catch (Exception ex)
            {
                AssertException(ex);
            }
        }

        [TestMethod]
        public void ShouldNotThrowExceptionBuildPropertiesForInitParametersForLatestWithInequalitiesDataFileForIndicator()
        {
            try
            {
                var receivedParameters = new DataInternalServicesParameters(DataInternalServiceUse.LatestWithInequalitiesDataFileForIndicator, _childAreaTypeId, _parentAreaTypeId, _sexId, _ageId, _parentAreaCode, _indicatorsIds,
                    _areasCode, _categoryAreaCode, _profileIdNullable, _groupId, _inequalities);

                var manager = new DataInternalServicesManager(receivedParameters, _groupDataReaderMock.Object, _profileReaderMock.Object, _areasReaderMock.Object);

                var resultExportParameters = manager.ExportParameters;
                var resultOnDemandParameters = manager.OnDemandParameters;

                Assert.IsNotNull(resultExportParameters);
                Assert.IsNotNull(resultOnDemandParameters);
            }
            catch (Exception ex)
            {
                AssertException(ex);
            }
        }

        [TestMethod]
        public void ShouldNotThrowExceptionBuildPropertiesForInitParametersForAllPeriodsWithInequalitiesDataFileForIndicator()
        {
            try
            {
                var receivedParameters = new DataInternalServicesParameters(DataInternalServiceUse.AllPeriodsWithInequalitiesDataFileForIndicator, _childAreaTypeId, _parentAreaTypeId, _sexId, _ageId, _parentAreaCode, _indicatorsIds,
                    _areasCode, _categoryAreaCode, _profileIdNullable, _groupId, _inequalities);

                var manager = new DataInternalServicesManager(receivedParameters, _groupDataReaderMock.Object, _profileReaderMock.Object, _areasReaderMock.Object);

                var resultExportParameters = manager.ExportParameters;
                var resultOnDemandParameters = manager.OnDemandParameters;

                Assert.IsNotNull(resultExportParameters);
                Assert.IsNotNull(resultOnDemandParameters);
            }
            catch (Exception ex)
            {
                AssertException(ex);
            }
        }

        [TestMethod]
        public void ShouldNotThrowExceptionBuildPropertiesForInitParametersForInitParametersForAllPeriodDataFileByIndicator()
        {
            try
            {
                var receivedParameters = new DataInternalServicesParameters(DataInternalServiceUse.AllPeriodDataFileByIndicator, _childAreaTypeId, _parentAreaTypeId, _sexId, _ageId, _parentAreaCode, _indicatorsIds,
                    _areasCode, _categoryAreaCode, _profileIdNullable, _groupId, _inequalities);

                var manager = new DataInternalServicesManager(receivedParameters, _groupDataReaderMock.Object, _profileReaderMock.Object, _areasReaderMock.Object);

                var resultExportParameters = manager.ExportParameters;
                var resultOnDemandParameters = manager.OnDemandParameters;

                Assert.IsNotNull(resultExportParameters);
                Assert.IsNotNull(resultOnDemandParameters);
            }
            catch (Exception ex)
            {
                AssertException(ex);
            }
        }

        [TestMethod]
        public void ShouldNotThrowExceptionBuildPropertiesForInitParametersForInitParametersForLatestPopulationDataFile()
        {
            _groupDataReaderMock.Setup(x => x.GetDataIncludingInequalities(It.IsAny<int>(),It.IsAny<TimePeriod>(),It.IsAny<IList<int>>(), It.IsAny<string[]>())).Returns(coreDataSetList);
            _groupDataReaderMock.Setup(x => x.GetGroupingMetadataList(It.IsAny<IList<int>>())).Returns(_groupingMetadataList);
            _groupDataReaderMock.Setup(x => x.GetGroupingsByGroupId(It.IsAny<int>())).Returns(_groupingList);
            
            try
            {
                var receivedParameters = new DataInternalServicesParameters(DataInternalServiceUse.LatestPopulationDataFile, _childAreaTypeId, _parentAreaTypeId, _sexId, _ageId, _parentAreaCode, _indicatorsIds,
                    _areasCode, _categoryAreaCode, _profileIdNullable, _groupId, _inequalities);

                var manager = new DataInternalServicesManager(receivedParameters, _groupDataReaderMock.Object, _profileReaderMock.Object, _areasReaderMock.Object);

                var resultExportParameters = manager.ExportParameters;
                var resultOnDemandParameters = manager.OnDemandParameters;

                Assert.IsNotNull(resultExportParameters);
                Assert.IsNotNull(resultOnDemandParameters);
            }
            catch (Exception ex)
            {
                AssertException(ex);
            }
        }

        private static void AssertException(Exception ex)
        {
            Assert.Fail("It should not throw an exception", ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }
}