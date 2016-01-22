using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Fpm.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsUITest
{
    [TestClass]
    public class UploadBatchRowParserTest
    {
        private int indicatorId = 1;
        private int year = 2;
        private int yearRange = 3;
        private int quarter = 4;
        private int month = 5;
        private int ageId = 6;
        private int sexId = 7;
        private string areaCode = "a";
        private double count = 1.1;
        private double value = 1.2;
        private double lowerCI = 1.4;
        private double upperCI = 1.5;
        private double denominator = 1.6;
        private double denominator2 = 1.6;
        private int valueNoteId = 300;
        private int categoryTypeId = 1;
        private int categoryId = 1;

        [TestMethod]
        public void TestGetUploadDataModel()
        {
            var model = new BatchRowParser(DataRow()).GetUploadDataModel();
            Assert.AreEqual(indicatorId, model.IndicatorId);
            Assert.AreEqual(year, model.Year);
            Assert.AreEqual(yearRange, model.YearRange);
            Assert.AreEqual(quarter, model.Quarter);
            Assert.AreEqual(month, model.Month);
            Assert.AreEqual(ageId, model.AgeId);
            Assert.AreEqual(sexId, model.SexId);
            Assert.AreEqual(areaCode, model.AreaCode);
            Assert.AreEqual(count, model.Count);
            Assert.AreEqual(value, model.Value);
            Assert.AreEqual(lowerCI, model.LowerCi);
            Assert.AreEqual(upperCI, model.UpperCi);
            Assert.AreEqual(denominator, model.Denominator);
            Assert.AreEqual(denominator2, model.Denominator_2);
            Assert.AreEqual(valueNoteId, model.ValueNoteId);
            Assert.AreEqual(categoryTypeId, model.CategoryTypeId);
            Assert.AreEqual(categoryId, model.CategoryId);
        }

        private DataRow DataRow()
        {
            DataTable table = new DataTable();
            var columns = table.Columns;

            var doubleType = typeof(double);

            columns.Add(UploadColumnNames.IndicatorId, doubleType);
            columns.Add(UploadColumnNames.Year, doubleType);
            columns.Add(UploadColumnNames.YearRange, doubleType);
            columns.Add(UploadColumnNames.Quarter, doubleType);
            columns.Add(UploadColumnNames.Month, doubleType);
            columns.Add(UploadColumnNames.AgeId, doubleType);
            columns.Add(UploadColumnNames.SexId, doubleType);
            columns.Add(UploadColumnNames.AreaCode, typeof(string));
            columns.Add(UploadColumnNames.Count, doubleType);
            columns.Add(UploadColumnNames.Value, doubleType);
            columns.Add(UploadColumnNames.LowerCI, doubleType);
            columns.Add(UploadColumnNames.UpperCI, doubleType);
            columns.Add(UploadColumnNames.Denominator, doubleType);
            columns.Add(UploadColumnNames.Denominator2, doubleType);
            columns.Add(UploadColumnNames.ValueNoteId, doubleType);
            columns.Add(UploadColumnNames.CategoryTypeId, doubleType);
            columns.Add(UploadColumnNames.CategoryId, doubleType);

            table.Rows.Add(indicatorId, year, yearRange, quarter, month, ageId, sexId, areaCode, count, value,
                           lowerCI, upperCI, denominator, denominator2, valueNoteId, categoryTypeId, categoryId);

            var row = table.Rows[0];
            return row;
        }
    }
}
