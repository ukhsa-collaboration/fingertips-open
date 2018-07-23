using FingertipsUploadService.ProfileData;
using FingertipsUploadService.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace FingertipsUploadServiceTest
{
    [TestClass]
    public class DuplicateDataFilterTest
    {
        [TestMethod]
        public void TestRemoveDuplicatesReturnsUnchangedListWhenNoDuplicates()
        {
            var dataList = GetDataList();
            var filteredData = new DuplicateDataFilter().RemoveDuplicateData(dataList);
            Assert.AreEqual(dataList.Count, filteredData.Count);
        }

        [TestMethod]
        public void TestRemoveDuplicatesReturnsListWithoutDuplicates()
        {
            var dataList = GetDuplicatedDataList();
            var filteredData = new DuplicateDataFilter().RemoveDuplicateData(dataList);
            Assert.AreEqual(2, filteredData.Count);
        }

        [TestMethod]
        public void TestGetDuplicatesReturnsEmptyListWhenNoDuplicates()
        {
            var dataList = GetDataList();
            var filteredData = new DuplicateDataFilter().GetDuplicatedData(dataList);
            Assert.AreEqual(0, filteredData.Count);
        }

        [TestMethod]
        public void TestGetDuplicatesReturnsDuplicates()
        {
            var dataList = GetDuplicatedDataList();
            var filteredData = new DuplicateDataFilter().GetDuplicatedData(dataList);
            Assert.AreEqual(2, filteredData.Count);
        }

        private IList<UploadDataModel> GetDataList()
        {
            return new List<UploadDataModel>
            {
                new UploadDataModel { Year= 1, YearRange = 2, Quarter = 3, Month = 4, AgeId = 5, SexId = 6, AreaCode = "a",CategoryTypeId = 8, CategoryId = 9},
                new UploadDataModel { Year= 99, YearRange = 2, Quarter = 3, Month = 4, AgeId = 5, SexId = 6, AreaCode = "a",CategoryTypeId = 8, CategoryId = 99},
                new UploadDataModel { Year= 1, YearRange = 99, Quarter = 3, Month = 4, AgeId = 5, SexId = 6, AreaCode = "a",CategoryTypeId = 8, CategoryId = 9},
                new UploadDataModel { Year= 1, YearRange = 2, Quarter = 99, Month = 4, AgeId = 5, SexId = 6, AreaCode = "a",CategoryTypeId = 8, CategoryId = 9},
                new UploadDataModel { Year= 1, YearRange = 2, Quarter = 3, Month = 99, AgeId = 5, SexId = 6, AreaCode = "a",CategoryTypeId = 8, CategoryId = 9},
                new UploadDataModel { Year= 1, YearRange = 2, Quarter = 3, Month = 4, AgeId = 99, SexId = 6, AreaCode = "a",CategoryTypeId = 8, CategoryId = 9},
                new UploadDataModel { Year= 1, YearRange = 2, Quarter = 3, Month = 4, AgeId = 5, SexId = 99, AreaCode = "b",CategoryTypeId = 8, CategoryId = 9},
                new UploadDataModel { Year= 1, YearRange = 2, Quarter = 3, Month = 4, AgeId = 5, SexId = 6, AreaCode = "a",CategoryTypeId = 99, CategoryId = 9}
            };
        }

        private IList<UploadDataModel> GetDuplicatedDataList()
        {
            return new List<UploadDataModel>
            {
                new UploadDataModel { Year= 1, YearRange = 2, Quarter = 3, Month = 4, AgeId = 5, SexId = 6, AreaCode = "a",CategoryTypeId = 8, CategoryId = 9},
                new UploadDataModel { Year= 99, YearRange = 2, Quarter = 3, Month = 4, AgeId = 5, SexId = 6, AreaCode = "a",CategoryTypeId = 8, CategoryId = 99},
                new UploadDataModel { Year= 1, YearRange = 2, Quarter = 3, Month = 4, AgeId = 5, SexId = 6, AreaCode = "a",CategoryTypeId = 8, CategoryId = 9}
            };
        }
    }
}
