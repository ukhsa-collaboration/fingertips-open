using System;
using FingertipsUploadService.ProfileData;
using System.Data;

namespace FingertipsUploadService.Upload
{
    public class UploadRowParser 
    {
        public const double UndefinedDouble = -1;
        public const int UndefinedInt = -1;
        public const int DefaultValueNoteId = ValueNoteIds.NoNote;

        protected DataRow Row;

        public UploadRowParser(DataRow row)
        {
            Row = row;
        }

        public string AreaCode
        {
            get { return Row.Field<string>(UploadColumnNames.AreaCode); }
        }

        public double? Count
        {
            get { return Row.Field<double?>(UploadColumnNames.Count); }
        }

        public bool DoesRowContainData
        {
            get { return string.IsNullOrWhiteSpace(AreaCode) == false; }
        }

        public UploadDataModel ParseRow()
        {
            var data = new UploadDataModel
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
                LowerCI95 = ParseDoubleThatMayBeEmpty(UploadColumnNames.LowerCI95),
                UpperCI95 = ParseDoubleThatMayBeEmpty(UploadColumnNames.UpperCI95),
                LowerCI99_8 = ParseDoubleThatMayBeEmpty(UploadColumnNames.LowerCI99_8),
                UpperCI99_8 = ParseDoubleThatMayBeEmpty(UploadColumnNames.UpperCI99_8),
                Denominator = ParseDouble(UploadColumnNames.Denominator),
                Denominator_2 = ParseDouble(UploadColumnNames.Denominator2),
                ValueNoteId = ParseInt(UploadColumnNames.ValueNoteId),
                CategoryTypeId = ParseIntThatMayBeEmpty(UploadColumnNames.CategoryTypeId),
                CategoryId = ParseIntThatMayBeEmpty(UploadColumnNames.CategoryId)
            };

            ReplaceMinusOnesWithNull(data);

            ReplaceMinusOneValueNoteWithNoNote(data);

            return data;
        }

        private static void ReplaceMinusOneValueNoteWithNoNote(UploadDataModel data)
        {
            if (data.ValueNoteId == UndefinedInt)
            {
                data.ValueNoteId = ValueNoteIds.NoNote;
            }
        }

        private static void ReplaceMinusOnesWithNull(UploadDataModel data)
        {
            // If both 95 CIs are -1
            if (data.LowerCI95.HasValue && data.LowerCI95 == UndefinedDouble &&
                data.UpperCI95.HasValue && data.UpperCI95 == UndefinedDouble)
            {
                data.LowerCI95 = null;
                data.UpperCI95 = null;
            }

            // If both 99.8 CIs are -1
            if (data.LowerCI99_8.HasValue && data.LowerCI99_8 == UndefinedDouble &&
                data.UpperCI99_8.HasValue && data.UpperCI99_8 == UndefinedDouble)
            {
                data.LowerCI99_8 = null;
                data.UpperCI99_8 = null;
            }
        }

        private double ParseDouble(string fieldName)
        {
            return Row.Field<double>(fieldName);
        }

        private int ParseInt(string fieldName)
        {
            return Convert.ToInt32(Row.Field<double>(fieldName));
        }

        private int ParseIntThatMayBeEmpty(string fieldName)
        {
            if (Row.Table.Columns.Contains(fieldName))
            {
                if (Row.Field<object>(fieldName) != null)
                {
                    return ParseInt(fieldName);
                }
            }
            return UndefinedInt;
        }

        private double? ParseDoubleThatMayBeEmpty(string fieldName)
        {
            if (Row.Table.Columns.Contains(fieldName))
            {
                if (Row.Field<object>(fieldName) != null)
                {
                    return ParseDouble(fieldName);
                }
            }
            return null;
        }
    }
}