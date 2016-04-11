using FingertipsUploadService.ProfileData;
using System.Data;

namespace FingertipsUploadService.Upload
{
    public class BatchRowParser : UploadRowParser
    {
        public BatchRowParser(DataRow row)
            : base(row)
        {
        }

        public UploadDataModel GetUploadDataModel()
        {
            var upload = new UploadDataModel
            {
                IndicatorId = ParseInt(UploadColumnNames.IndicatorId),
                Year = ParseInt(UploadColumnNames.Year),
                YearRange = ParseInt(UploadColumnNames.YearRange),
                Quarter = ParseInt(UploadColumnNames.Quarter),
                Month = ParseInt(UploadColumnNames.Month),
                AgeId = ParseInt(UploadColumnNames.AgeId),
                SexId = ParseInt(UploadColumnNames.SexId),
                AreaCode = AreaCode,
                Count = Count,
                Value = ParseDouble(UploadColumnNames.Value),
                LowerCi = ParseDouble(UploadColumnNames.LowerCI),
                UpperCi = ParseDouble(UploadColumnNames.UpperCI),
                Denominator = ParseDouble(UploadColumnNames.Denominator),
                Denominator_2 = ParseDouble(UploadColumnNames.Denominator2),
                ValueNoteId = ParseInt(UploadColumnNames.ValueNoteId),
                CategoryTypeId = ParseNullableInt(UploadColumnNames.CategoryTypeId),
                CategoryId = ParseNullableInt(UploadColumnNames.CategoryId)
            };

            return upload;
        }

        private double ParseDouble(string fieldName)
        {
            return Row.Field<double>(fieldName);
        }

        private int ParseInt(string fieldName)
        {
            return (int)Row.Field<double>(fieldName);
        }

        private int ParseNullableInt(string fieldName)
        {
            if (Row.Table.Columns.Contains(fieldName))
            {
                var field = Row.Field<double?>(fieldName);
                return (field == null) ? UndefinedInt : (int)field;
            }
            return UndefinedInt;
        }
    }
}