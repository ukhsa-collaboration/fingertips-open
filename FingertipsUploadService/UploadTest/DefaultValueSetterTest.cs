using FingertipsUploadService.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FingertipsUploadService.ProfileData;

namespace FingertipsUploadService.UploadTest
{
    [TestClass]
    public class DefaultValueSetterTest
    {
        private readonly double minusOne = -1;

        [TestMethod]
        public void Test_Converts_Nulls_To_Default_Values()
        {
            var dataTable = UploadDataSchema.CreateEmptyTable();
            dataTable.Rows.Add(IndicatorIds.GeneralHealthExcellent, 2030, 1, DBNull.Value, DBNull.Value, AgeIds.Aged15, 
                SexIds.Persons, AreaCodes.England, DBNull.Value,
                DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
                DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);
            new DefaultValueSetter().ReplaceNullsWithDefaultValues(dataTable);

            var row = dataTable.Rows[0];
            Assert.AreEqual(minusOne, row[UploadColumnNames.Quarter]);
            Assert.AreEqual(minusOne, row[UploadColumnNames.Month]);
            Assert.AreEqual(minusOne, row[UploadColumnNames.Count]);
            Assert.AreEqual(minusOne, row[UploadColumnNames.Value]);
            Assert.AreEqual(minusOne, row[UploadColumnNames.LowerCI95]);
            Assert.AreEqual(minusOne, row[UploadColumnNames.UpperCI95]);
            Assert.AreEqual(minusOne, row[UploadColumnNames.LowerCI99_8]);
            Assert.AreEqual(minusOne, row[UploadColumnNames.UpperCI99_8]);
            Assert.AreEqual(minusOne, row[UploadColumnNames.Denominator]);
            Assert.AreEqual(minusOne, row[UploadColumnNames.Denominator2]);
            Assert.AreEqual((double)ValueNoteIds.NoNote, row[UploadColumnNames.ValueNoteId]);
            Assert.AreEqual(minusOne, row[UploadColumnNames.CategoryTypeId]);
            Assert.AreEqual(minusOne, row[UploadColumnNames.CategoryId]);
        }
    }
}
