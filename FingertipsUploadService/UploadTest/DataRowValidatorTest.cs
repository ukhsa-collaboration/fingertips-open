using System;
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Profile;
using FingertipsUploadService.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FingertipsUploadService.UploadTest
{
    [TestClass]
    public class DataRowValidatorTest
    {
        [TestMethod]
        public void TestRowIsValidIfCategoryColumnsAreNotPresent()
        {
            var table = GetEmptyDataTable();

            table.Rows.Add(1, 2000, 1, -1, -1, AgeIds.AllAges, SexIds.Persons, AreaCodes.CountyUa_Cambridgeshire,
                1, 1, 1, 1, 1, 1, ValueNoteIds.ThereIsDataQualityIssueWithThisValue);

            var rowValidator = new DataRowValidator(GetFirstRow(table), 1, ReaderFactory.GetProfilesReader());
            var uploadError = rowValidator.ValidateCategoryTypeIdAndCategoryId(new List<Category>());

            Assert.IsNull(uploadError, "The data was not valid");
        }

        /// <summary>
        /// Row is valid because -1s will be inserted by default
        /// </summary>
        [TestMethod]
        public void TestRowIsValidIfCategoryColumnsArePresentButEmpty()
        {
            var table = GetEmptyDataTable();

            table.Rows.Add(1, 2000, 1, -1, -1, AgeIds.AllAges, SexIds.Persons, AreaCodes.CountyUa_Cambridgeshire,
                1, 1, 1, 1, 1, 1, ValueNoteIds.ThereIsDataQualityIssueWithThisValue, null, null);

            var dataUploadValidation = new DataRowValidator(GetFirstRow(table), 1, ReaderFactory.GetProfilesReader());
            var uploadError = dataUploadValidation.ValidateCategoryTypeIdAndCategoryId(new List<Category>());

            Assert.IsNull(uploadError, "The data was not valid");
        }

        [TestMethod]
        public void TestRowIsNotValidIfCategoryValuesAreNotValid()
        {

            var table = GetEmptyDataTable();

            table.Rows.Add(1, 2000, 1, -1, -1, AgeIds.AllAges, SexIds.Persons, AreaCodes.CountyUa_Cambridgeshire,
                1, 1, 1, 1, 1, 1, 1, 1, ValueNoteIds.ThereIsDataQualityIssueWithThisValue, 1, 2);

            var rowValidator = new DataRowValidator(GetFirstRow(table), 1, ReaderFactory.GetProfilesReader());
            var uploadError = rowValidator.ValidateCategoryTypeIdAndCategoryId(
                new List<Category>{new Category
                {
                    CategoryTypeId = 1,
                    CategoryId = 3
                }});

            Assert.IsNotNull(uploadError, "The data should not be valid");
        }

        [TestMethod]
        public void TestRowIsValidIfCategoryValuesAreValid()
        {
            var table = GetTableWithValidRow();

            var rowValidator = new DataRowValidator(GetFirstRow(table), 1, ReaderFactory.GetProfilesReader());
            var uploadError = rowValidator.ValidateCategoryTypeIdAndCategoryId(
                new List<Category>{new Category
                {
                    CategoryTypeId = 1,
                    CategoryId = 2
                }});

            Assert.IsNull(uploadError, "The data should be valid");
        }

        [TestMethod]
        public void TestRowIsNotValidIfValueNoteIs506()
        {
            var rowValidator = new DataRowValidator(GetFirstRow(GetTableWithValidRow()), 1, ReaderFactory.GetProfilesReader());
            var uploadError = rowValidator.ValidateValueNoteId(
                ValueNoteIds.AggregatedFromAllKnownLowerGeographyValuesByFingertips,
                GetAllowedData().ValueNoteIds);
            Assert.IsNotNull(uploadError, "Value note 506 should not be allowed");
        }

        [TestMethod]
        public void TestRowIsValidIfValueNoteIsNoNote()
        {
            var rowValidator = new DataRowValidator(GetFirstRow(GetTableWithValidRow()), 1, ReaderFactory.GetProfilesReader());
            var uploadError = rowValidator.ValidateValueNoteId( ValueNoteIds.NoNote, 
                GetAllowedData().ValueNoteIds);
            Assert.IsNull(uploadError, "Value note not allowed");
        }

        [TestMethod]
        public void Test_ValidateCIs_Fails_Where_Both_95_CIs_Are_Zero()
        {
            var row = GetFirstRow(GetTableWithValidRow());
            row[UploadColumnNames.LowerCI95] = 0;
            row[UploadColumnNames.UpperCI95] = 0;

            var rowValidator = new DataRowValidator(row, 1, ReaderFactory.GetProfilesReader());
            var uploadError = rowValidator.ValidateCIs();

            Assert.AreEqual("Both 95 CIs are zero", uploadError.ErrorMessage);
        }

        [TestMethod]
        public void Test_ValidateCIs_Fails_Where_Both_95_CIs_Are_Less_Than_One()
        {
            var row = GetFirstRow(GetTableWithValidRow());
            row[UploadColumnNames.LowerCI95] = 0.18254103735279;
            row[UploadColumnNames.UpperCI95] = 0.75781166645064;

            var rowValidator = new DataRowValidator(row, 1, ReaderFactory.GetProfilesReader());
            var uploadError = rowValidator.ValidateCIs();

            Assert.IsNull(uploadError);
        }

        [TestMethod]
        public void Test_ValidateCIs_Fails_Where_Both_998_CIs_Are_Zero()
        {
            var row = GetFirstRow(GetTableWithValidRow());
            row[UploadColumnNames.LowerCI99_8] = 0;
            row[UploadColumnNames.UpperCI99_8] = 0;

            var rowValidator = new DataRowValidator(row, 1, ReaderFactory.GetProfilesReader());
            var uploadError = rowValidator.ValidateCIs();

            Assert.AreEqual("Both 99.8 CIs are zero", uploadError.ErrorMessage);
        }

        private static AllowedData GetAllowedData()
        {
            var allowedData = new AllowedData(ReaderFactory.GetProfilesReader());
            return allowedData;
        }

        private static DataRow GetFirstRow(DataTable table)
        {
            DataRow[] rows = new DataRow[table.Rows.Count];
            table.Rows.CopyTo(rows, 0);
            return rows.First();
        }

        private DataTable GetTableWithValidRow()
        {
            var table = GetEmptyDataTable();

            table.Rows.Add(1, 2000, 1, -1, -1, AgeIds.AllAges, SexIds.Persons,
                AreaCodes.CountyUa_Cambridgeshire, 1, 1, 1, 1, 1, 1, 
                ValueNoteIds.ThereIsDataQualityIssueWithThisValue, 1, 2);

            return table;
        }

        private static DataTable GetEmptyDataTable()
        {
            return UploadDataSchema.CreateEmptyTable();
        }
    }
}
