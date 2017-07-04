using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Profile;
using System;
using System.Collections.Generic;
using System.Data;

namespace FingertipsUploadService.Upload
{
    public class DataUploadValidation
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

        public static bool DoesIndicatorMetaDataExist(SimpleUpload simpleUpload)
        {
            var indicatorMetaData = Reader.GetIndicatorMetadata(simpleUpload.IndicatorId);
            if (indicatorMetaData != null)
            {
                simpleUpload.YearTypeId = indicatorMetaData.YearTypeId;
                return true;
            }

            return false;
        }

        public UploadValidationFailure ValidateIndicator(DataRow uploadedRow, int rowNumber)
        {
            //Do other indicator validation
            var columnName = UploadColumnNames.IndicatorId;
            try
            {
                var indicatorId = (int)uploadedRow.Field<double>(columnName);

                if (indicatorId < 1)
                {
                    return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, null);
                }

                //Check that the indicator exists
                if (!DoesIndicatorMetaDataExist(new SimpleUpload { IndicatorId = indicatorId }))
                {
                    return new UploadValidationFailure(rowNumber, columnName, "Indicator does not exist", null);
                }
            }
            catch (Exception ex)
            {
                //Indicator is invalid for another reason - Log this and exit
                return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }


        public UploadValidationFailure ValidateYear(DataRow uploadedRow, int? rowNumber)
        {
            //Do other year validation
            var columnName = UploadColumnNames.Year;
            try
            {
                var year = (int)uploadedRow.Field<double>(columnName);

                if ((year < MinimumYearValue || year > MaximumYearValue) || year.ToString().Length != 4)
                {
                    return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, null);
                }
            }
            catch (Exception ex)
            {
                //Year is invalid for another reason - Log this and exit
                return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        public UploadValidationFailure ValidateYearRange(DataRow uploadedRow, int rowNumber)
        {
            //Do other YearRange validation
            var columnName = UploadColumnNames.YearRange;
            try
            {
                var yearRange = (int)uploadedRow.Field<double>(columnName);

                if ((yearRange < MinimumYearRangeValue || yearRange > MaximumYearRangeValue) ||
                    yearRange.ToString().Length != 1)
                {
                    return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, null);
                }
            }
            catch (Exception ex)
            {
                //YearRange is invalid for another reason - Log this and exit
                return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        public UploadValidationFailure ValidateQuarter(DataRow uploadedRow, int rowNumber)
        {
            //Do other Quarter validation
            var columnName = UploadColumnNames.Quarter;
            try
            {
                var quarter = (int)uploadedRow.Field<double>(columnName);

                if (quarter != -1)
                {
                    if ((quarter < MinimumQuarterValue || quarter > MaximumQuarterValue))
                    {
                        return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, null);
                    }
                }
            }
            catch (Exception ex)
            {
                //Quarter is invalid for another reason - Log this and exit
                return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        public UploadValidationFailure ValidateMonth(DataRow uploadedRow, int rowNumber)
        {
            //Do other Month validation
            var columnName = UploadColumnNames.Month;
            try
            {
                var month = (int)uploadedRow.Field<double>(columnName);

                if (month != -1)
                {
                    if ((month < MinimumMonthValue || month > MaximumMonthValue) ||
                        (month.ToString().Length < 1 || month.ToString().Length > 2))
                    {
                        return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, null);
                    }
                }
            }
            catch (Exception ex)
            {
                //Month is invalid for another reason - Log this and exit
                return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        public UploadValidationFailure ValidateAgeId(int ageId, int? rowNumber, List<int> ageIds)
        {
            //Does the ageId actually exist?
            var columnName = UploadColumnNames.AgeId;
            try
            {
                if (!ageIds.Contains(ageId))
                {
                    return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, null);
                }
            }
            catch (Exception ex)
            {
                //AgeId is invalid for another reason - Log this and exit
                return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        public UploadValidationFailure ValidateSexId(int sexId, int? rowNumber, List<int> sexIds)
        {
            var columnName = UploadColumnNames.SexId;
            //Does the sexId actually exist?
            try
            {
                if (!sexIds.Contains(sexId))
                {
                    return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, null);
                }
            }
            catch (Exception ex)
            {
                //SexId is invalid for another reason - Log this and exit
                return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        public UploadValidationFailure ValidateArea(DataRow uploadedRow, int? rowNumber, List<string> allAreaCodes)
        {
            //Does the areaCode actually exist?
            var columnName = UploadColumnNames.AreaCode;
            try
            {
                var areaCode = uploadedRow.Field<string>(columnName);
                if (!allAreaCodes.Contains(areaCode))
                {
                    return new UploadValidationFailure(rowNumber, columnName,
                        "Invalid " + columnName + " - (" + areaCode + ")", null);
                }
            }
            catch (Exception ex)
            {
                //AreaCode is invalid for another reason - Log this and exit
                return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        public UploadValidationFailure ValidateValueNoteId(int valueNoteId, int? rowNumber, List<int> valueNoteIds)
        {
            //Does the valueNoteId actually exist?
            var columnName = UploadColumnNames.ValueNoteId;
            try
            {
                if (!valueNoteIds.Contains(valueNoteId))
                {
                    return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, null);
                }
            }
            catch (Exception ex)
            {
                //ValueNoteId is invalid for another reason - Log this and exit
                return new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, ex.Message);
            }

            return null;
        }

        private int? ParseNullableInt(DataRow row, string fieldName)
        {
            var field = row.Field<object>(fieldName);
            return (field == null) ? null : (int?)Convert.ToInt32(field);
        }

        public UploadValidationFailure ValidateCategoryTypeIdAndCategoryId(DataRow uploadedRow,
            int rowNumber, IList<Category> categories)
        {
            // If columns are not present then ignore
            if (uploadedRow.Table.Columns.Contains(UploadColumnNames.CategoryTypeId) == false)
            {
                return null;
            }

            try
            {
                int? categoryTypeId = ParseNullableInt(uploadedRow, UploadColumnNames.CategoryTypeId);
                int? categoryId = ParseNullableInt(uploadedRow, UploadColumnNames.CategoryId);
                return new CategoryValuesValidator(categories).Validate(rowNumber, categoryTypeId, categoryId);
            }
            catch (Exception ex)
            {
                return new UploadValidationFailure(rowNumber, "CategoryTypeId & CategoryId",
                    CategoryValuesValidator.InvalidCategoryAndTypeMessage, ex.Message);
            }
        }

        public Exception ValidateExpectedDataType(DataRow uploadedRow, string fieldName, DataType dataType)
        {
            try
            {
                switch (dataType)
                {
                    case DataType.Double:
                        double expectedDouble = Convert.ToDouble(uploadedRow.Field<double>(fieldName).ToString());
                        break;
                    case DataType.NullableDouble:
                        double? expectedNullableDouble = Convert.ToDouble(uploadedRow.Field<double?>(fieldName));
                        break;
                    case DataType.Integer:
                        Int32 expectedInteger = Convert.ToInt32(uploadedRow.Field<double>(fieldName).ToString());
                        break;
                    case DataType.NullableInteger:
                        int? expectedNullableInteger = Convert.ToInt32(uploadedRow.Field<int?>(fieldName));
                        break;
                    case DataType.String:
                        var expectedString = uploadedRow.Field<string>(fieldName);
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