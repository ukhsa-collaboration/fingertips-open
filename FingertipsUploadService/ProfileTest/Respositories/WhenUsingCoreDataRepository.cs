using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Core;
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
        public void TestDeletePrecalculatedCoreData()
        {
            _coreDataRepository.DeletePrecalculatedCoreData(IndicatorIds.ObesityYear6);
        }

        [TestMethod]
        public void TestGetCoreDataSetByUploadJobId()
        {
            var data = _coreDataRepository.GetCoreDataSetByUploadJobId(Guid.NewGuid());
            Assert.AreEqual(0 , data.Count);
        }

        [TestMethod]
        public void TestGetDuplicateCoreDataSetForAnIndicator()
        {
            var coreDataSet = new CoreDataSet
            {
                IndicatorId = 337,
                Year = 2010,
                YearRange = 1,
                Quarter = -1,
                Month = -1,
                AgeId = 4,
                SexId = 1,
                AreaCode = "5P3",
                CategoryTypeId = -1,
                CategoryId = -1
            };
            var result = _coreDataRepository.GetDuplicateCoreDataSetForAnIndicator(coreDataSet);
            Assert.IsNotNull(result);
        }

    }
}
