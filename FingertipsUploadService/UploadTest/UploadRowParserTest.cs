
using FingertipsUploadService.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Core;

namespace FingertipsUploadService.UploadTest
{
    [TestClass]
    public class UploadRowParserTest
    {
        private const int IndicatorId = 1;
        private const int Year = 2;
        private const int YearRange = 3;
        private const int Quarter = 4;
        private const int Month = 5;
        private const int AgeId = AgeIds.Aged15;
        private const int SexId = SexIds.Female;
        public const string AreaCode = AreaCodes.England;
        private const double Count = 1.1;
        private const double Value = 1.2;
        private const double LowerCI95 = 1.4;
        private const double UpperCI95 = 1.5;
        private const double LowerCI99_8 = 1.7;
        private const double UpperCI99_8 = 1.8;
        private const double Denominator = 1.6;
        private const double Denominator2 = 1.6;
        private const int ValueNoteId = ValueNoteIds.ThereIsDataQualityIssueWithThisValue;
        private const int CategoryTypeId = 1;
        private const int CategoryId = 1;

        [TestMethod]
        public void TestParseFullRow()
        {
            var model = new UploadRowParser(GetTestDataRow(GetCoreData())).ParseRow();
            AssertRowParsed(model);
            Assert.AreEqual(CategoryTypeId, model.CategoryTypeId);
            Assert.AreEqual(CategoryId, model.CategoryId);
        }

        [TestMethod]
        public void TestParseRowWithoutCategoryEmpty()
        {
            var model = new UploadRowParser(GetTestDataRowWithoutCategoryColumns()).ParseRow();
            AssertRowParsed(model);
            Assert.AreEqual(CategoryTypeIds.Undefined, model.CategoryTypeId);
            Assert.AreEqual(CategoryIds.Undefined, model.CategoryId);
        }

        [TestMethod]
        public void Test_Minus_One_CIs_Converted_To_Nulls()
        {
            var data = GetCoreData();
            data.LowerCI95 = -1;
            data.UpperCI95 = -1;
            data.LowerCI99_8 = -1;
            data.UpperCI99_8 = -1;

            var model = new UploadRowParser(GetTestDataRow(data)).ParseRow();

            // Assert: minus ones are converted to nulls
            Assert.IsNull(model.LowerCI95);
            Assert.IsNull(model.UpperCI95);
            Assert.IsNull(model.LowerCI99_8);
            Assert.IsNull(model.UpperCI99_8);
        }

        [TestMethod]
        public void Test_Null_CIs_Parsed_As_Nulls()
        {
            var data = GetCoreData();
            data.LowerCI95 = null;
            data.UpperCI95 = null;
            data.LowerCI99_8 = null;
            data.UpperCI99_8 = null;

            var model = new UploadRowParser(GetTestDataRow(data)).ParseRow();

            // Assert: minus ones are converted to nulls
            Assert.IsNull(model.LowerCI95);
            Assert.IsNull(model.UpperCI95);
            Assert.IsNull(model.LowerCI99_8);
            Assert.IsNull(model.UpperCI99_8);
        }

        private static void AssertRowParsed(UploadDataModel model)
        {
            Assert.AreEqual(IndicatorId, model.IndicatorId);
            Assert.AreEqual(Year, model.Year);
            Assert.AreEqual(YearRange, model.YearRange);
            Assert.AreEqual(Quarter, model.Quarter);
            Assert.AreEqual(Month, model.Month);
            Assert.AreEqual(AgeId, model.AgeId);
            Assert.AreEqual(SexId, model.SexId);
            Assert.AreEqual(AreaCode, model.AreaCode);
            Assert.AreEqual(Count, model.Count);
            Assert.AreEqual(Value, model.Value);
            Assert.AreEqual(LowerCI95, model.LowerCI95);
            Assert.AreEqual(UpperCI95, model.UpperCI95);
            Assert.AreEqual(LowerCI99_8, model.LowerCI99_8);
            Assert.AreEqual(UpperCI99_8, model.UpperCI99_8);
            Assert.AreEqual(Denominator, model.Denominator);
            Assert.AreEqual(Denominator2, model.Denominator_2);
            Assert.AreEqual(ValueNoteId, model.ValueNoteId);
        }

        public static CoreDataSet GetCoreData()
        {
            return new CoreDataSet
            {
                IndicatorId = IndicatorId,
                Year = Year,
                YearRange = YearRange,
                Quarter = Quarter,
                Month = Month,
                AgeId = AgeId,
                SexId = SexId,
                AreaCode = AreaCode,
                Count = Count,
                Value = Value,
                LowerCI95 = LowerCI95,
                UpperCI95 = UpperCI95,
                LowerCI99_8 = LowerCI99_8,
                UpperCI99_8 = UpperCI99_8,
                Denominator = Denominator,
                Denominator_2 = Denominator2,
                ValueNoteId = ValueNoteId,
                CategoryTypeId = CategoryTypeId,
                CategoryId = CategoryId
            };
        }

        public static DataRow GetTestDataRow(CoreDataSet data)
        {
            var table = UploadDataSchema.CreateEmptyTable();
            table.Rows.Add(data.IndicatorId, data.Year, data.YearRange, data.Quarter, data.Month, 
                data.AgeId, data.SexId, data.AreaCode, data.Count, data.Value, 
                data.LowerCI95, data.UpperCI95, data.LowerCI99_8, data.UpperCI99_8, 
                data.Denominator, data.Denominator_2, data.ValueNoteId, data.CategoryTypeId, data.CategoryId);

            var row = table.Rows[0];
            return row;
        }

        private static DataRow GetTestDataRowWithoutCategoryColumns()
        {
            var table = UploadDataSchema.CreateEmptyTable();

            table.Rows.Add(IndicatorId, Year, YearRange, Quarter, Month, AgeId, SexId, AreaCode, Count, Value,
                           LowerCI95, UpperCI95, LowerCI99_8, UpperCI99_8, Denominator, Denominator2, ValueNoteId);

            var row = table.Rows[0];
            return row;
        }

    }
}
