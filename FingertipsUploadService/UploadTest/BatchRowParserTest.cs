
using FingertipsUploadService.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace FingertipsUploadService.UploadTest
{
    [TestClass]
    public class UploadBatchRowParserTest
    {
        private const int IndicatorId = 1;
        private const int Year = 2;
        private const int YearRange = 3;
        private const int Quarter = 4;
        private const int Month = 5;
        private const int AgeId = 6;
        private const int SexId = 7;
        private const string AreaCode = "a";
        private const double Count = 1.1;
        private const double Value = 1.2;
        private const double LowerCI = 1.4;
        private const double UpperCI = 1.5;
        private const double Denominator = 1.6;
        private const double Denominator2 = 1.6;
        private const int ValueNoteId = 300;
        private const int CategoryTypeId = 1;
        private const int CategoryId = 1;

        [TestMethod]
        public void TestGetUploadDataModel()
        {
            var model = new BatchRowParser(GetTestDataRow()).GetUploadDataModel();
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
            Assert.AreEqual(LowerCI, model.LowerCi);
            Assert.AreEqual(UpperCI, model.UpperCi);
            Assert.AreEqual(Denominator, model.Denominator);
            Assert.AreEqual(Denominator2, model.Denominator_2);
            Assert.AreEqual(ValueNoteId, model.ValueNoteId);
            Assert.AreEqual(CategoryTypeId, model.CategoryTypeId);
            Assert.AreEqual(CategoryId, model.CategoryId);
        }

        public static DataRow GetTestDataRow()
        {
            var table = new UploadDataSchema().CreateEmptyTable();
            table.Rows.Add(IndicatorId, Year, YearRange, Quarter, Month, AgeId, SexId, AreaCode, Count, Value,
                           LowerCI, UpperCI, Denominator, Denominator2, ValueNoteId, CategoryTypeId, CategoryId);

            var row = table.Rows[0];
            return row;
        }
    }
}
