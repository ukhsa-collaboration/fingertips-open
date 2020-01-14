using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fpm.ProfileDataTest.Repositories
{
    [TestClass]
    public class WhenUsingCoreDataRepository
    {
        private CoreDataRepository _coreDataRepository;
        private Guid _batchId;

        private const int IndicatorId = IndicatorIds.LifeExpectancyAtBirth;

        [TestInitialize]
        public void Init()
        {
            _coreDataRepository = new CoreDataRepository();
            AutoMapperConfig.RegisterMappings();
            _batchId = Guid.NewGuid();
        }

        [TestCleanup]
        public void CleanUp()
        {
            _coreDataRepository.DeleteCoreDataArchive(_batchId);
            _coreDataRepository.DeleteCoreDataSet(_batchId);

            _coreDataRepository.Dispose();
        }

        [TestMethod]
        public void TestDeleteCoreDataExecutes()
        {
            _coreDataRepository.DeleteCoreDataSet(_batchId);
        }

        [TestMethod]
        public void TestDeleteCoreDataArchive()
        {
            _coreDataRepository.DeleteCoreDataArchive(_batchId);
        }

        [TestMethod]
        public void GetCategoryTypes_Returns_Non_Zero_Result()
        {
            var result = _coreDataRepository.GetCategoryTypes(108);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetAreaTypes_Returns_Non_Zero_Result()
        {
            var result = _coreDataRepository.GetAreaTypes(IndicatorId);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetSexes_Returns_Non_Zero_Result()
        {
            var result = _coreDataRepository.GetSexes(IndicatorId);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetAges_Returns_Non_Zero_Result()
        {
            var result = _coreDataRepository.GetAges(IndicatorId);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetYearRanges_Returns_Non_Zero_Result()
        {
            var result = _coreDataRepository.GetYearRanges(IndicatorId);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetYears_Returns_Non_Zero_Result()
        {
            var result = _coreDataRepository.GetYears(IndicatorId);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetMonths_Returns_Non_Zero_Result()
        {
            var result = _coreDataRepository.GetMonths(IndicatorIds.CDifficileInfectionCounts);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetQuarters_Returns_Non_Zero_Result()
        {
            var result = _coreDataRepository.GetQuarters(IndicatorIds.StaffEngagement);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetCoreDataSet_Returns_Non_Zero_Result()
        {
            int rowsCount;
            var result = _coreDataRepository.GetCoreDataSet(IndicatorId, out rowsCount);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetCoreDataSet_Returns_Result_For_Given_Filters()
        {
            int rowsFound;
            var filters = new Dictionary<string, object>();

            filters.Add(CoreDataFilters.Year, 2011);
            filters.Add(CoreDataFilters.Month, 8);
            filters.Add(CoreDataFilters.SexId, SexIds.Persons);
            filters.Add(CoreDataFilters.AreaTypeId, AreaTypeIds.GpPractice);

            var result = _coreDataRepository.GetCoreDataSet(IndicatorId, filters, out rowsFound);

            Assert.IsTrue(result != null);

            var dataSetRowsCount = result.Count();
            Console.WriteLine("Total records found " + rowsFound);
            Console.WriteLine("Total records " + dataSetRowsCount);

            Assert.IsTrue(dataSetRowsCount <= 500 && rowsFound >= dataSetRowsCount);
        }

        [TestMethod]
        public void CanDeleteDataSet_Returns_True_For_AuthorisedUser()
        {
            var userName = UserNames.Doris;

            var result = _coreDataRepository.CanDeleteDataSet(IndicatorId, userName);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanDeleteDataSet_Returns_False_For_UnInvalidUser()
        {
            var result = _coreDataRepository.CanDeleteDataSet(IndicatorId, UserNames.NotAGenuineUserName);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanDeleteDataSet_Returns_False_For_IncorrectIndicatorId()
        {
            var userName = UserNames.Shan;

            var result = _coreDataRepository.CanDeleteDataSet(11119999, userName);

            Assert.IsTrue(result == false);
        }

        [TestMethod]
        public void DeleteCoreDataSet_Executes_If_Not_A_Real_Indicator_Id()
        {
            var userName = UserNames.Shan;

            var result = _coreDataRepository.DeleteCoreDataSet(IndicatorIds.Undefined,
                new Dictionary<string, object>(), userName);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void DeleteCoreDataSet_Can_Delete_Data()
        {
            var userName = UserNames.Shan;

            var result = _coreDataRepository.DeleteCoreDataSet(IndicatorIds.MrsaBloodstreamInfections,
                new Dictionary<string, object>(), userName);

            Assert.IsTrue(result > -1);
        }
    }
}