using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Profile;
using System;
using System.Collections.Generic;
using System.Data;

namespace FingertipsUploadService.Upload
{
    public class DataRowValidator
    {
        public enum DataType
        {
            String,
            Integer,
            Double,
            NullableDouble,
            NullableInteger
        }

        private const int MinimumYearValue = 1990;
        private const int MaximumYearValue = 2100;

        private const int MinimumYearRangeValue = 1;
        private const int MaximumYearRangeValue = 10;

        private const int MinimumQuarterValue = 1;

        // Quarter could be larger than 4 for multi-year cumulative quarters
        private const int MaximumQuarterValue = 100;

        private const int MinimumMonthValue = 1;
        private const int MaximumMonthValue = 12;

        private static readonly ProfilesReader Reader = ReaderFactory.GetProfilesReader();

        public static bool DoesIndicatorMetaDataExist(int indicatorId)
        {
            var indicatorMetaData = Reader.GetIndicatorMetadata(indicatorId);
            return indicatorMetaData != null;
        }

        public DataRow Row;
        private int _rowNumber;

        public DataRowValidator(DataRow row, int rowNumber)
        {
            Row = row;
            _rowNumber = rowNumber;
        }

        public UploadValidationFailure ValidateIndicatorId()
        {
            //Do other indicator validation
            var columnName = UploadColumnNames.IndicatorId;
            try
            {
                var indicatorId = (int)Row.Field<double>(columnName);

                if (indicatorId < 1)
                {
                    return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, null);
                }

                //Check that the indicator exists
                if (!DoesIndicatorMetaDataExist(indicatorId))
                {
                    return new UploadValidationFailure(_rowNumber, columnName, "Indicator does not exist", null);
                }
            }
            catch (Exception ex)
            {
                //Indicator is invalid for another reason - Log this and exit
                return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }


        public UploadValidationFailure ValidateYear()
        {
            //Do other year validation
            var columnName = UploadColumnNames.Year;
            try
            {
                var year = (int)Row.Field<double>(columnName);

                if ((year < MinimumYearValue || year > MaximumYearValue) || year.ToString().Length != 4)
                {
                    return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, null);
                }
            }
            catch (Exception ex)
            {
                //Year is invalid for another reason - Log this and exit
                return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        public UploadValidationFailure ValidateYearRange()
        {
            //Do other YearRange validation
            var columnName = UploadColumnNames.YearRange;
            try
            {
                var yearRange = (int)Row.Field<double>(columnName);

                if ((yearRange < MinimumYearRangeValue || yearRange > MaximumYearRangeValue) ||
                    yearRange.ToString().Length != 1)
                {
                    return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, null);
                }
            }
            catch (Exception ex)
            {
                //YearRange is invalid for another reason - Log this and exit
                return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        public UploadValidationFailure ValidateQuarter()
        {
            //Do other Quarter validation
            var columnName = UploadColumnNames.Quarter;
            try
            {
                var quarter = (int)Row.Field<double>(columnName);

                if (quarter != -1)
                {
                    if ((quarter < MinimumQuarterValue || quarter > MaximumQuarterValue))
                    {
                        return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, null);
                    }
                }
            }
            catch (Exception ex)
            {
                //Quarter is invalid for another reason - Log this and exit
                return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        public UploadValidationFailure ValidateMonth()
        {
            //Do other Month validation
            var columnName = UploadColumnNames.Month;
            try
            {
                var month = (int)Row.Field<double>(columnName);

                if (month != -1)
                {
                    if ((month < MinimumMonthValue || month > MaximumMonthValue) ||
                        (month.ToString().Length < 1 || month.ToString().Length > 2))
                    {
                        return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, null);
                    }
                }
            }
            catch (Exception ex)
            {
                //Month is invalid for another reason - Log this and exit
                return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        public UploadValidationFailure ValidateAgeId(int ageId,  List<int> ageIds)
        {
            //Does the ageId actually exist?
            var columnName = UploadColumnNames.AgeId;
            try
            {
                if (!ageIds.Contains(ageId))
                {
                    return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, null);
                }
            }
            catch (Exception ex)
            {
                //AgeId is invalid for another reason - Log this and exit
                return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        public UploadValidationFailure ValidateSexId(int sexId, List<int> sexIds)
        {
            var columnName = UploadColumnNames.SexId;
            //Does the sexId actually exist?
            try
            {
                if (!sexIds.Contains(sexId))
                {
                    return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, null);
                }
            }
            catch (Exception ex)
            {
                //SexId is invalid for another reason - Log this and exit
                return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        public UploadValidationFailure ValidateArea(List<string> allAreaCodes)
        {
            //Does the areaCode actually exist?
            var columnName = UploadColumnNames.AreaCode;
            try
            {
                var areaCode = Row.Field<string>(columnName);
                if (!allAreaCodes.Contains(areaCode))
                {
                    return new UploadValidationFailure(_rowNumber, columnName,
                        "Invalid " + columnName + " - (" + areaCode + ")", null);
                }
            }
            catch (Exception ex)
            {
                //AreaCode is invalid for another reason - Log this and exit
                return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        public UploadValidationFailure ValidateValueNoteId(int valueNoteId, List<int> valueNoteIds)
        {

            var columnName = UploadColumnNames.ValueNoteId;
            try
            {
                // Does the valueNoteId actually exist?
                if (!valueNoteIds.Contains(valueNoteId))
                {
                    return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, null);
                }
            }
            catch (Exception ex)
            {
                //ValueNoteId is invalid for another reason - Log this and exit
                return new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        private int? ParseNullableInt(string fieldName)
        {
            var field = Row.Field<object>(fieldName);
            return field == null ? null : (int?)Convert.ToInt32(field);
        }

        public UploadValidationFailure ValidateCategoryTypeIdAndCategoryId(IList<Category> categories)
        {
            // If columns are not present then ignore
            if (Row.Table.Columns.Contains(UploadColumnNames.CategoryTypeId) == false)
            {
                return null;
            }

            try
            {
                int? categoryTypeId = ParseNullableInt(UploadColumnNames.CategoryTypeId);
                int? categoryId = ParseNullableInt(UploadColumnNames.CategoryId);
                return new CategoryValuesValidator(categories).Validate(_rowNumber, categoryTypeId, categoryId);
            }
            catch (Exception ex)
            {
                return new UploadValidationFailure(_rowNumber, "CategoryTypeId & CategoryId",
                    CategoryValuesValidator.InvalidCategoryAndTypeMessage, ex.Message);
            }
        }

        public Exception ValidateExpectedDataType(string fieldName, DataType dataType)
        {
            try
            {
                switch (dataType)
                {
                    case DataType.Double:
                        double expectedDouble = Convert.ToDouble(Row.Field<double>(fieldName));
                        break;
                    case DataType.NullableDouble:
                        double? expectedNullableDouble = Convert.ToDouble(Row.Field<double?>(fieldName));
                        break;
                    case DataType.Integer:
                        Int32 expectedInteger = Convert.ToInt32(Row.Field<double>(fieldName));
                        break;
                    case DataType.NullableInteger:
                        int? expectedNullableInteger = Convert.ToInt32(Row.Field<int?>(fieldName));
                        break;
                    case DataType.String:
                        var expectedString = Row.Field<string>(fieldName);
                        break;
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
            return null;
        }
    }
}