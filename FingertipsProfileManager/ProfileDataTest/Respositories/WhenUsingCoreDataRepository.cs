using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Core;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fpm.ProfileDataTest.Respositories
{
    [TestClass]
    public class WhenUsingCoreDataRepository
    {
        private CoreDataRepository _coreDataRepository;
        private LoggingRepository _loggingRepository;
        private static Guid batchId;

        private const int indicatorId = IndicatorIds.ChildrenInPoverty;

        [TestInitialize]
        public void Init()
        {
            _coreDataRepository = new CoreDataRepository();
            _loggingRepository = new LoggingRepository();
            AutoMapperConfig.RegisterMappings();
            batchId = Guid.NewGuid();
        }

        [TestCleanup]
        public void CleanUp()
        {
            _coreDataRepository.DeleteCoreDataArchive(batchId);
            _coreDataRepository.DeleteCoreData(batchId);

            _coreDataRepository.Dispose();
            _loggingRepository.Dispose();
        }

        [TestMethod]
        public void TestInsertUploadAudit()
        {
            //Insert the upload audit record
            var uploadId = _loggingRepository.InsertUploadAudit(Guid.NewGuid(),
                "TestUser", 10, @"C:\\Fingertips\\Test.xls", "Pholio$");

            //Read the upload audit record back

            var uploadAudit = _loggingRepository.GetUploadAudit(uploadId);

            Assert.IsTrue(uploadAudit != null);

            Assert.IsTrue(uploadAudit.Id == uploadId);

            Assert.IsTrue(uploadAudit.UploadedBy == "TestUser");

            //Delete the new record
            _loggingRepository.DeleteUploadAudit(uploadId);

            uploadAudit = _loggingRepository.GetUploadAudit(uploadId);

            Assert.IsTrue(uploadAudit == null);
        }

        [TestMethod]
        public void TestDeleteCoreDataExecutes()
        {
            _coreDataRepository.DeleteCoreData(batchId);
        }

        [TestMethod]
        public void TestDeleteCoreDataArchive()
        {
            _coreDataRepository.DeleteCoreDataArchive(batchId);
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
            var result = _coreDataRepository.GetAreaTypes(indicatorId);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetSexes_Returns_Non_Zero_Result()
        {
            var result = _coreDataRepository.GetSexes(indicatorId);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetAges_Returns_Non_Zero_Result()
        {
            var result = _coreDataRepository.GetAges(indicatorId);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetYearRanges_Returns_Non_Zero_Result()
        {
            var result = _coreDataRepository.GetYearRanges(indicatorId);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetYears_Returns_Non_Zero_Result()
        {
            var result = _coreDataRepository.GetYears(indicatorId);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetMonths_Returns_Non_Zero_Result()
        {
            var result = _coreDataRepository.GetMonths(IndicatorIds.LongTermUnemployment);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetQuarters_Returns_Non_Zero_Result()
        {
            var result = _coreDataRepository.GetQuarters(IndicatorIds.PeopleWhoDieAtHome);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetCoreDataSet_Returns_Non_Zero_Result()
        {
            int rowsCount;
            var result = _coreDataRepository.GetCoreDataSet(indicatorId, out rowsCount);

            Assert.IsTrue(result != null
                && result.Any());
        }

        [TestMethod]
        public void GetCoreDataSet_Returns_Result_For_Given_Filters()
        {
            int rowsFound;
            var filters = new Dictionary<string, int>();

            filters.Add(CoreDataFilters.Year, 2011);
            filters.Add(CoreDataFilters.Month, 8);
            filters.Add(CoreDataFilters.SexId, SexIds.Persons);
            filters.Add(CoreDataFilters.AreaTypeId, AreaTypeIds.GpPractice);

            var result = _coreDataRepository.GetCoreDataSet(indicatorId, filters, out rowsFound);

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

            var result = _coreDataRepository.CanDeleteDataSet(indicatorId, userName);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanDeleteDataSet_Returns_False_For_UnInvalidUser()
        {
            var result = _coreDataRepository.CanDeleteDataSet(indicatorId, UserNames.NotAGenuineUserName);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanDeleteDataSet_Returns_False_For_IncorrectIndicatorId()
        {
            var userName = UserNames.Shan;

            var result = _coreDataRepository.CanDeleteDataSet(11119999, userName);

            Assert.IsTrue(result == false);
        }

        private static CoreDataSet GetRecordToInsert()
        {
            var val = new Random(150).Next();
            return new CoreDataSet()
            {
                IndicatorId = IndicatorIds.ObesityYear6,
                Year = 2014,
                YearRange = 1,
                Quarter = -1,
                Month = -1,
                SexId = SexIds.Male,
                AgeId = AgeIds.AllAges,
                AreaCode = AreaCodes.CountyUa_Cambridgeshire,
                Count = val,
                Value = val,
                LowerCi = val,
                UpperCi = val,
                Denominator = 0,
                Denominator_2 = 0,
                ValueNoteId = 0,
                CategoryTypeId = 1,
                CategoryId = 1
            };
        }
    }
}
