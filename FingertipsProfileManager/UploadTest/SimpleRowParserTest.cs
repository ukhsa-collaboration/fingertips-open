using System;
using System.Data;
using Fpm.ProfileData;
using Fpm.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsUITest
{
    [TestClass]
    public class UploadSimpleRowParserTest
    {
        private const double doubleValue = 2.3;
        private const double count = 1.1;
        private const double lowerCI = 2.2;
        private const double upperCI = 3.3;
        private const double denominator = 4.4;

        private const string areaCode = "a";

        private const int indicatorId = 6;
        private const int year = 2001;
        private const int yearRange = 2;
        private const int quarter = 1;
        private const int month = 3;
        private const int ageId = 4;
        private const int sexId = 5;

        [TestMethod]
        public void TestAreaCode()
        {
            var rows = Rows(UploadColumnNames.AreaCode, typeof(string));
            rows.Add(areaCode);
            var parser = new UploadSimpleRowParser(rows[0]);
            Assert.AreEqual(areaCode, parser.AreaCode);
        }

        [TestMethod]
        public void TestIsValidFalseIfBothAreaCodeAndValueNull()
        {
            Assert.IsFalse(ParserForAreaCodeAndValue(null, null).DoesRowContainData);
        }

        [TestMethod]
        public void TestIsValidFalseIfAreaCodeEmptyString()
        {
            Assert.IsFalse(ParserForAreaCodeAndValue(string.Empty, null).DoesRowContainData);
        }

        [TestMethod]
        public void TestIsValidFalseIfAreaCodeWhitespace()
        {
            Assert.IsFalse(ParserForAreaCodeAndValue(" ", null).DoesRowContainData);
        }

        [TestMethod]
        public void TestIsValidTrueIfOnlyValueDefined()
        {
            Assert.IsTrue(ParserForAreaCodeAndValue(null, doubleValue).DoesRowContainData);
        }

        [TestMethod]
        public void TestIsValidTrueIfOnlyAreaCodeDefined()
        {
            Assert.IsTrue(ParserForAreaCodeAndValue(areaCode, null).DoesRowContainData);
        }

        [TestMethod]
        public void TestCount()
        {
            var rows = Rows(UploadColumnNames.Count, typeof(double));
            rows.Add(doubleValue);
            Assert.AreEqual(doubleValue, new UploadSimpleRowParser(rows[0]).Count);
        }

        [TestMethod]
        public void TestGetUploadDataModel()
        {
            var model = new UploadSimpleRowParser(GetValidRow()).GetUploadDataModel(GetSimpleUpload());

            AssertSimpleUploadPropertiesCopiedOk(model);
            AssertRowValuesParsedOk(model, count, lowerCI, upperCI, denominator);
        }

        [TestMethod]
        public void TestGetUploadDataModelWithUnparsedValuesSetToDefaults()
        {
            var model = new UploadSimpleRowParser(GetValidRow()).GetUploadDataModelWithUnparsedValuesSetToDefaults(GetSimpleUpload());

            AssertSimpleUploadPropertiesCopiedOk(model);
            AssertRowValuesParsedOk(model, count, lowerCI, upperCI, denominator);
            AssertUnparsedValuesAreSetToDefaults(model);
        }

        private static void AssertUnparsedValuesAreSetToDefaults(UploadDataModel model)
        {
            Assert.AreEqual(UploadRowParser.UndefinedDouble, model.Denominator_2);
            Assert.AreEqual(UploadRowParser.DefaultValueNoteId, model.ValueNoteId);
            Assert.AreEqual(UploadRowParser.UndefinedInt, model.CategoryId);
            Assert.AreEqual(UploadRowParser.UndefinedInt, model.CategoryTypeId);
        }

        private DataRow GetValidRow()
        {
            var doubleType = typeof(double);

            DataTable table = new DataTable();
            table.Columns.Add(UploadColumnNames.AreaCode, typeof(string));
            table.Columns.Add(UploadColumnNames.Count, doubleType);
            table.Columns.Add(UploadColumnNames.Value, doubleType);
            table.Columns.Add(UploadColumnNames.LowerCI, doubleType);
            table.Columns.Add(UploadColumnNames.UpperCI, doubleType);
            table.Columns.Add(UploadColumnNames.Denominator, doubleType);
            table.Columns.Add(UploadColumnNames.ValueNoteId, typeof(int));
            var rows = table.Rows;

            rows.Add(areaCode, count, doubleValue, lowerCI, upperCI, denominator);
            return rows[0];
        }

        private static void AssertRowValuesParsedOk(UploadDataModel model, double count, double lowerCI, double upperCI,
                                                    double denominator)
        {
            Assert.AreEqual(doubleValue, model.Value);
            Assert.AreEqual(count, model.Count);
            Assert.AreEqual(lowerCI, model.LowerCi);
            Assert.AreEqual(upperCI, model.UpperCi);
            Assert.AreEqual(denominator, model.Denominator);
        }

        private static void AssertSimpleUploadPropertiesCopiedOk(UploadDataModel model)
        {
            Assert.AreEqual(indicatorId, model.IndicatorId);
            Assert.AreEqual(year, model.Year);
            Assert.AreEqual(yearRange, model.YearRange);
            Assert.AreEqual(quarter, model.Quarter);
            Assert.AreEqual(month, model.Month);
            Assert.AreEqual(ageId, model.AgeId);
            Assert.AreEqual(sexId, model.SexId);
        }

        private static SimpleUpload GetSimpleUpload()
        {
            var simpleUpload = new SimpleUpload
                {
                    IndicatorId = indicatorId,
                    Year = year,
                    YearRange = yearRange,
                    Quarter = quarter,
                    Month = month,
                    AgeId = ageId,
                    SexId = sexId
                };
            return simpleUpload;
        }

        private DataRowCollection Rows(string columnName, Type type)
        {
            DataTable table = new DataTable();
            table.Columns.Add(columnName, type);
            return table.Rows;
        }

        private UploadSimpleRowParser ParserForAreaCodeAndValue(string areaCode, double? val)
        {
            DataTable table = new DataTable();
            table.Columns.Add(UploadColumnNames.AreaCode, typeof(string));
            table.Columns.Add(UploadColumnNames.Value, typeof(double));
            var rows = table.Rows;

            rows.Add(areaCode, val);

            return new UploadSimpleRowParser(rows[0]);
        }

    }
}
