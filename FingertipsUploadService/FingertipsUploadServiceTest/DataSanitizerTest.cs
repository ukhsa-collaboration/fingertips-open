using FingertipsUploadService.Helpers;
using FingertipsUploadService.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FingertipsUploadService.ProfileData;

namespace FingertipsUploadServiceTest
{
    [TestClass]
    public class DataSanitizerTest
    {
        private readonly double minusOne = -1;

        [TestMethod]
        public void TestSanitizeExcelData_Converts_Nulls_To_Default_Values()
        {
            var dataTable = new UploadDataSchema().CreateEmptyTable();
            dataTable.Rows.Add(91491, 2030, 1, DBNull.Value, DBNull.Value, 44, 1, "E92000001", DBNull.Value,
                DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);
            new DataSanitizer().SanitizeExcelData(dataTable);

            var row = dataTable.Rows[0];
            Assert.AreEqual(minusOne, row[UploadColumnIndexes.Quarter]);
            Assert.AreEqual(minusOne, row[UploadColumnIndexes.Month]);
            Assert.AreEqual(minusOne, row[UploadColumnIndexes.Count]);
            Assert.AreEqual(minusOne, row[UploadColumnIndexes.Value]);
            Assert.AreEqual(minusOne, row[UploadColumnIndexes.LowerCI]);
            Assert.AreEqual(minusOne, row[UploadColumnIndexes.UpperCI]);
            Assert.AreEqual(minusOne, row[UploadColumnIndexes.Denominator]);
            Assert.AreEqual(minusOne, row[UploadColumnIndexes.Denominator2]);
            Assert.AreEqual((double)ValueNoteIds.NoNote, row[UploadColumnIndexes.ValueNoteId]);
            Assert.AreEqual(minusOne, row[UploadColumnIndexes.CategoryTypeId]);
            Assert.AreEqual(minusOne, row[UploadColumnIndexes.CategoryId]);
        }
    }
}
