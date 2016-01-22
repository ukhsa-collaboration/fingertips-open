using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Profile;
using Fpm.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace UploadTest
{
    [TestClass]
    public class DataUploadValidationTest
    {
        [TestMethod]
        public void TestRowIsValidIfCategoryColumnsAreNotPresent()
        {
            var dataUploadValidation = new DataUploadValidation();

            var table = GetBatchPholioDataTableWithoutCategoryColumns();

            table.Rows.Add(1, 2000, 1, -1, -1, AgeIds.AllAges, SexIds.Persons, AreaCodes.CountyUa_Cambridgeshire,
                1, 1, 1, 1, 1, 1, 1, ValueNoteIds.ThereIsDataQualityIssueWithThisValue);

            var uploadError = dataUploadValidation.ValidateCategoryTypeIdAndCategoryId(
                GetFirstRow(table), 1, new List<Category>());

            Assert.IsNull(uploadError, "The data was not valid");
        }

        /// <summary>
        /// Row is valid because -1s will be inserted by default
        /// </summary>
        [TestMethod]
        public void TestRowIsValidIfCategoryColumnsArePresentButEmpty()
        {
            var dataUploadValidation = new DataUploadValidation();

            var table = GetBatchPholioDataTableWithCategoryColumns();

            table.Rows.Add(1, 2000, 1, -1, -1, AgeIds.AllAges, SexIds.Persons, AreaCodes.CountyUa_Cambridgeshire,
                1, 1, 1, 1, 1, 1, 1, ValueNoteIds.ThereIsDataQualityIssueWithThisValue, null, null);

            var uploadError = dataUploadValidation.ValidateCategoryTypeIdAndCategoryId(
                GetFirstRow(table), 1, new List<Category>());

            Assert.IsNull(uploadError, "The data was not valid");
        }

        [TestMethod]
        public void TestRowIsNotValidIfCategoryValuesAreNotValid()
        {
            var dataUploadValidation = new DataUploadValidation();

            var table = GetBatchPholioDataTableWithCategoryColumns();

            table.Rows.Add(1, 2000, 1, -1, -1, AgeIds.AllAges, SexIds.Persons, AreaCodes.CountyUa_Cambridgeshire,
                1, 1, 1, 1, 1, 1, 1, ValueNoteIds.ThereIsDataQualityIssueWithThisValue, 1, 2);

            var uploadError = dataUploadValidation.ValidateCategoryTypeIdAndCategoryId(
                GetFirstRow(table), 1, new List<Category>{new Category
                {
                    CategoryTypeId = 1,
                    CategoryId = 3
                }});

            Assert.IsNotNull(uploadError, "The data should not be valid");
        }

        [TestMethod]
        public void TestRowIsValidIfCategoryValuesAreValid()
        {
            var dataUploadValidation = new DataUploadValidation();

            var table = GetBatchPholioDataTableWithCategoryColumns();

            table.Rows.Add(1, 2000, 1, -1, -1, AgeIds.AllAges, SexIds.Persons,
                AreaCodes.CountyUa_Cambridgeshire, 1, 1, 1, 1, 1, 1, 1,
                ValueNoteIds.ThereIsDataQualityIssueWithThisValue, 1, 2);

            var uploadError = dataUploadValidation.ValidateCategoryTypeIdAndCategoryId(
                GetFirstRow(table), 1, new List<Category>{new Category
                {
                    CategoryTypeId = 1,
                    CategoryId = 2
                }});

            Assert.IsNull(uploadError, "The data should be valid");
        }

        private static DataRow GetFirstRow(DataTable table)
        {
            DataRow[] rows = new DataRow[table.Rows.Count];
            table.Rows.CopyTo(rows, 0);
            return rows.First();
        }

        private static DataTable GetBatchPholioDataTableWithoutCategoryColumns()
        {
            DataTable table = new DataTable();
            table.Columns.Add(UploadColumnNames.IndicatorId, typeof(int));
            table.Columns.Add(UploadColumnNames.Year, typeof(int));
            table.Columns.Add(UploadColumnNames.YearRange, typeof(int));
            table.Columns.Add(UploadColumnNames.Quarter, typeof(int));
            table.Columns.Add(UploadColumnNames.Month, typeof(int));
            table.Columns.Add(UploadColumnNames.AgeId, typeof(int));
            table.Columns.Add(UploadColumnNames.SexId, typeof(int));
            table.Columns.Add(UploadColumnNames.AreaCode, typeof(string));
            table.Columns.Add(UploadColumnNames.Count, typeof(double));
            table.Columns.Add(UploadColumnNames.Value, typeof(double));
            table.Columns.Add(UploadColumnNames.ExpectedValue, typeof(double));
            table.Columns.Add(UploadColumnNames.LowerCI, typeof(double));
            table.Columns.Add(UploadColumnNames.UpperCI, typeof(double));
            table.Columns.Add(UploadColumnNames.Denominator, typeof(double));
            table.Columns.Add(UploadColumnNames.Denominator2, typeof(double));
            table.Columns.Add(UploadColumnNames.ValueNoteId, typeof(int));
            return table;
        }

        private static DataTable GetBatchPholioDataTableWithCategoryColumns()
        {
            DataTable table = GetBatchPholioDataTableWithoutCategoryColumns();
            table.Columns.Add(UploadColumnNames.CategoryTypeId, typeof(int));
            table.Columns.Add(UploadColumnNames.CategoryId, typeof(int));
            return table;
        }
    }
}
