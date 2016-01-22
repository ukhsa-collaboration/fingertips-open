using FingertipsUploadService.ProfileData;
using System.Data;

namespace Fpm.Upload
{
    public class UploadSimpleRowParser : UploadRowParser
    {
        public UploadSimpleRowParser(DataRow row)
            : base(row)
        {
        }

        public UploadDataModel GetUploadDataModel(SimpleUpload simpleUpload)
        {
            var upload = new UploadDataModel
            {
                IndicatorId = simpleUpload.IndicatorId,
                Year = simpleUpload.Year,
                YearRange = simpleUpload.YearRange,
                Quarter = simpleUpload.Quarter,
                Month = simpleUpload.Month,
                AgeId = simpleUpload.AgeId,
                SexId = simpleUpload.SexId,
                AreaCode = AreaCode,
                Count = Count,
                Value = Value,
                LowerCi = LowerCI,
                UpperCi = UpperCI,
                Denominator = Denominator,
                ValueNoteId = ValueNoteId,
                CategoryId = UndefinedInt,
                CategoryTypeId = UndefinedInt
            };

            return upload;
        }

        public UploadDataModel GetUploadDataModelWithUnparsedValuesSetToDefaults(SimpleUpload simpleUpload)
        {
            var upload = GetUploadDataModel(simpleUpload);
            upload.Denominator_2 = UndefinedDouble;

            return upload;
        }

        private double Value
        {
            get { return ParseNullableDouble(UploadColumnNames.Value); }
        }

        private double LowerCI
        {
            get { return ParseNullableDouble(UploadColumnNames.LowerCI); }
        }

        private double UpperCI
        {
            get { return ParseNullableDouble(UploadColumnNames.UpperCI); }
        }

        private double Denominator
        {
            get { return ParseNullableDouble(UploadColumnNames.Denominator); }
        }

        private int ValueNoteId
        {
            get { return ParseNullableValueNoteId(UploadColumnNames.ValueNoteId); }
        }

        private bool IsValue
        {
            get
            {
                return Row.Field<double?>(UploadColumnNames.Value) != null;
            }
        }

        public bool DoesRowContainData
        {
            get { return IsAreaCode || IsValue; }
        }

        private bool IsAreaCode
        {
            get { return string.IsNullOrWhiteSpace(AreaCode) == false; }
        }

        private double ParseNullableDouble(string fieldName)
        {
            return Row.Field<double?>(fieldName) == null
                       ? UndefinedDouble
                       : Row.Field<double>(fieldName);
        }

        private int ParseNullableValueNoteId(string fieldName)
        {
            return (int)(Row.Field<double?>(fieldName) == null
                ? DefaultValueNoteId
                : Row.Field<double>(fieldName));
        }
    }
}