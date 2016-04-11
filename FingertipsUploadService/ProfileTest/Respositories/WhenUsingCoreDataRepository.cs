using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FingertipsUploadService.ProfileDataTest.Respositories
{
    [TestClass]
    public class WhenUsingCoreDataRepository
    {
        private CoreDataRepository _coreDataRepository;
        private LoggingRepository _loggingRepository;
        private static Guid batchId;

        private const int indicatorId = IndicatorIds.IDAOPI;

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

    }
}
