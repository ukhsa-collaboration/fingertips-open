using System;
using System.Collections.Generic;
using System.Data;
using FingertipsUploadService.ProfileData;

namespace FingertipsUploadService.Upload
{
    public class DataRowValidationManager
    {
        private readonly AllowedData _allowedData;
        private List<UploadValidationFailure> _validationFailures;
        private DataRow _row;
        private int _rowNumber;
        private DataRowValidator _rowValidator;

        public DataRowValidationManager(AllowedData allowedData)
        {
            _allowedData = allowedData;
        }

        public List<UploadValidationFailure> ValidateRow(DataRowValidator rowValidator, int rowNumber)
        {
            // Init instance properties
            _validationFailures = new List<UploadValidationFailure>();
            _rowNumber = rowNumber;
            _rowValidator = rowValidator;
            _row = _rowValidator.Row;

            // Validate row
            ValidateIndicatorId();
            ValidateYear();
            ValidateYearRange();
            ValidateQuarter();
            ValidateMonth();
            ValidateAgeId();
            ValidateSexId();
            ValidateAreaCode();
            ValidateDoubleType(UploadColumnNames.Count);
            ValidateDoubleType(UploadColumnNames.Value);
            ValidateDoubleType(UploadColumnNames.LowerCI95);
            ValidateDoubleType(UploadColumnNames.UpperCI95);
            ValidateDoubleType(UploadColumnNames.LowerCI99_8);
            ValidateDoubleType(UploadColumnNames.UpperCI99_8);
            ValidateDoubleType(UploadColumnNames.Denominator);
            ValidateDoubleType(UploadColumnNames.Denominator2);
            ValidateValueNoteId();
            ValidateCategoryTypeIdAndCategoryId();

            return _validationFailures;
        }

        private void ValidateCategoryTypeIdAndCategoryId()
        {
            UploadValidationFailure uploadValidationFailure;
            uploadValidationFailure = _rowValidator.ValidateCategoryTypeIdAndCategoryId(_allowedData.Categories);
            if (uploadValidationFailure != null)
            {
                _validationFailures.Add(uploadValidationFailure);
            }
        }

        private void ValidateValueNoteId()
        {
            var columnName = UploadColumnNames.ValueNoteId;
            var dataConversionException = _rowValidator.ValidateExpectedDataType(columnName, DataRowValidator.DataType.Integer);
            if (dataConversionException != null)
            {
                LogFailure(columnName, dataConversionException);
            }
            else
            {
                //Ensure this is a value note id in the DB
                var valueNoteId = (int)_row.Field<double>(columnName);
                var uploadValidationFailure = _rowValidator.ValidateValueNoteId(valueNoteId, _allowedData.ValueNoteIds);
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    _validationFailures.Add(uploadValidationFailure);
                }
            }
        }

        private void LogFailure(string columnName, Exception dataConversionException)
        {
            _validationFailures.Add(new UploadValidationFailure(_rowNumber, columnName, "Invalid " + columnName,
                dataConversionException.Message));
        }

        private void ValidateDoubleType(string columnName)
        {
            var dataConversionException = _rowValidator.ValidateExpectedDataType(columnName, DataRowValidator.DataType.Double);
            if (dataConversionException != null)
            {
                LogFailure(columnName, dataConversionException);
            }
        }

        private void ValidateAreaCode()
        {
            var columnName = UploadColumnNames.AreaCode;
            var dataConversionException = _rowValidator.ValidateExpectedDataType(columnName, DataRowValidator.DataType.String);
            if (dataConversionException != null)
            {
                LogFailure(columnName, dataConversionException);
            }
            else
            {
                //Ensure this is a valid Area Code in the DB
                var uploadValidationFailure = _rowValidator.ValidateArea(_allowedData.AreaCodes);
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    _validationFailures.Add(uploadValidationFailure);
                }
            }
        }

        private void ValidateSexId()
        {
            var columnName = UploadColumnNames.SexId;
            var dataConversionException = _rowValidator.ValidateExpectedDataType(columnName, DataRowValidator.DataType.Integer);
            if (dataConversionException != null)
            {
                LogFailure(columnName, dataConversionException);
            }
            else
            {
                //Ensure this is a valid Sex Id in the DB
                var sexId = (int)_rowValidator.Row.Field<double>(columnName);
                var uploadValidationFailure = _rowValidator.ValidateSexId(sexId, _allowedData.SexIds);
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    _validationFailures.Add(uploadValidationFailure);
                }
            }
        }

        private void ValidateAgeId()
        {
            var columnName = UploadColumnNames.AgeId;
            var dataConversionException = _rowValidator.ValidateExpectedDataType(columnName, DataRowValidator.DataType.Integer);
            if (dataConversionException != null)
            {
                LogFailure(columnName, dataConversionException);
            }
            else
            {
                //Validate the Age Id Value
                var ageId = (int)_row.Field<double>(columnName);
                var uploadValidationFailure = _rowValidator.ValidateAgeId(ageId, _allowedData.AgeIds);
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    _validationFailures.Add(uploadValidationFailure);
                }
            }
        }

        private void ValidateMonth()
        {
            var columnName = UploadColumnNames.Month;
            var dataConversionException = _rowValidator.ValidateExpectedDataType(columnName, DataRowValidator.DataType.Integer);
            if (dataConversionException != null)
            {
                LogFailure(columnName, dataConversionException);
            }
            else
            {
                //Validate the Quarter Value
                var uploadValidationFailure = _rowValidator.ValidateMonth();
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    _validationFailures.Add(uploadValidationFailure);
                }
            }
        }

        private void ValidateQuarter()
        {
            var columnName = UploadColumnNames.Quarter;
            var dataConversionException = _rowValidator.ValidateExpectedDataType(columnName, DataRowValidator.DataType.Integer);
            if (dataConversionException != null)
            {
                LogFailure(columnName, dataConversionException);
            }
            else
            {
                //Validate the Quarter Value
                var uploadValidationFailure = _rowValidator.ValidateQuarter();
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    _validationFailures.Add(uploadValidationFailure);
                }
            }
        }

        private void ValidateYearRange()
        {
            var columnName = UploadColumnNames.YearRange;
            var dataConversionException = _rowValidator.ValidateExpectedDataType(columnName, DataRowValidator.DataType.Integer);
            if (dataConversionException != null)
            {
                LogFailure(columnName, dataConversionException);
            }
            else
            {
                var uploadValidationFailure = _rowValidator.ValidateYearRange();
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    _validationFailures.Add(uploadValidationFailure);
                }
            }
        }

        private void ValidateYear()
        {
            var columnName = UploadColumnNames.Year;
            var dataConversionException = _rowValidator.ValidateExpectedDataType(columnName, DataRowValidator.DataType.Integer);
            if (dataConversionException != null)
            {
                LogFailure(columnName, dataConversionException);
            }
            else
            {
                //Ensure this is a valid Year value
                var uploadValidationFailure = _rowValidator.ValidateYear();
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    _validationFailures.Add(uploadValidationFailure);
                }
            }
        }

        private void ValidateIndicatorId()
        {
            var columnName = UploadColumnNames.IndicatorId;
            var dataConversionException = _rowValidator.ValidateExpectedDataType(columnName, DataRowValidator.DataType.Integer);
            if (dataConversionException != null)
            {
                LogFailure(columnName, dataConversionException);
            }
            else
            {
                var uploadValidationFailure = _rowValidator.ValidateIndicatorId();
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    _validationFailures.Add(uploadValidationFailure);
                }
            }
        }
    }
}