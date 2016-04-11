using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Core;
using FingertipsUploadService.ProfileData.Repositories;
using FingertipsUploadService.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FingertipsUploadService.UploadTest
{
    [TestClass]
    public class UploadTest
    {
        private static Guid batchId;

        private static CoreDataRepository _coreDataRepository;

        [TestInitialize]
        public void RunBeforeEachTest()
        {
            _coreDataRepository = new CoreDataRepository();
            AutoMapperConfig.RegisterMappings();

            batchId = Guid.NewGuid();
        }

        [TestCleanup]
        public void RunAfterEachTest()
        {
            _coreDataRepository.DeleteCoreData(batchId);
            _coreDataRepository.DeleteCoreDataArchive(batchId);
            _coreDataRepository.Dispose();
        }


        [TestMethod]
        public void TestArchiveCoreData()
        {
            var batchUpload = new BatchUpload();
            batchUpload.DuplicateRowInDatabaseErrors.Add(new DuplicateRowInDatabaseError
            {
                AgeId = AgeIds.AllAges,
                AreaCode = AreaCodes.CountyUa_Cambridgeshire,
                DbValue = 123.456,
                ErrorMessage = "TestRowIsValidIfCategoryColumnsAreNotPresent Error Message",
                ExcelValue = 543.21,
                IndicatorId = IndicatorIds.ObesityYear6,
                RowNumber = 123,
                SexId = SexIds.Male,
                Uid = 999999999
            });

            _coreDataRepository.InsertCoreData(GetRecordToInsert(), batchId);

            var records = ArchivedRecords(batchUpload);
            Assert.IsTrue(records.Any());

            //var duplicateRows = string.Join(",", records.Select(x => x.Uid).Take(10).ToList());
            var duplicateRows = records.Select(x => new DuplicateRowInDatabaseError { Uid = x.Uid }).Take(10).ToList();

            //Insert the duplicates to the CoreDataset Archive table and delete the coredataset rows
            // in question (All in one transaction)
            _coreDataRepository.InsertCoreDataArchive(duplicateRows, batchId);

            //Check that the row has been removed from coredataset.
            Assert.IsFalse(ArchivedRecords(batchUpload).Count == 0);

            //Check that the row has been inserted into the core dataset archive.
            Assert.IsTrue(ArchivedRecords(batchUpload).Any());

            //Finally, delete the CoreDataSetArchive record
            _coreDataRepository.DeleteCoreDataArchive(batchId);

            //Check that it has been deleted
            Assert.IsFalse(ArchivedRecords(batchUpload).Count == 0);


        }

        public IList<CoreDataSetArchive> ArchivedRecords(BatchUpload batchUpload)
        {
            return ReaderFactory.GetProfilesReader().GetCoreDataArchiveForIndicatorIds(
                batchUpload.DuplicateRowInDatabaseErrors.Select(x => x.IndicatorId).ToList());
        }

        private static CoreDataSet GetRecordToInsert()
        {
            var val = new Random(150).Next();
            return new CoreDataSet
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
