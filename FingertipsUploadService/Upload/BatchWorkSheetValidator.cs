using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fpm.Upload
{
    public class BatchWorkSheetValidator
    {
        private readonly ProfilesReader _profilesReader = ReaderFactory.GetProfilesReader();
        private List<int> _indicators = new List<int>();

        public List<int> GetIndicators { get { return _indicators; } }

        public BatchUpload Validate(string excelFileName, string selectedWorksheet)
        {
            DataTable excelData = UploadFileHelper.ReadWorksheet(excelFileName, selectedWorksheet);

            //Get all the relevent lookup table into memory to validate against
            var allowedData = new AllowedData(_profilesReader);

            //Validate the data in the spreadsheet
            BatchUpload batchUpload = ValidateSpreadsheetRows(excelData, allowedData);

            if (batchUpload.DataToUpload.Any())
            {
                //Validate the spreadsheet data to see if there's replication within it.
                batchUpload.DuplicateUploadErrorsExist = ValidateSpreadsheetForDuplicatedRows(batchUpload);
                batchUpload.DuplicateRowInDatabaseErrors = DoesCoreDataAlreadyExist(batchUpload).DuplicateRowInDatabaseErrors;
            }

            GetDistinctIndicators(excelData);

            return batchUpload;
        }

        private BatchUpload ValidateSpreadsheetRows(DataTable excelData, AllowedData allowedData)
        {
            var validatedUpload = new BatchUpload
            {
                ColumnsCount = excelData.Columns.Count,
                TotalDataRows = excelData.Rows.Count
            };

            //TODO FIN-841 check columns are all present

            for (int i = 0; i < excelData.Rows.Count; i++)
            {
                DataRow row = excelData.Rows[i];

                List<UploadValidationFailure> validationFailures = ValidateUploadedRow(row, i, allowedData);
                if (validationFailures.Count == 0)
                {
                    UploadDataModel upload = new BatchRowParser(row).GetUploadDataModel();
                    upload.RowNumber = i + 2;

                    validatedUpload.DataToUpload.Add(upload);
                }
                else
                {
                    foreach (UploadValidationFailure error in validationFailures)
                        validatedUpload.UploadValidationFailures.Add(error);
                }
            }
            return validatedUpload;
        }

        /// <summary>
        ///     Checks the spreadsheet for duplicate rows within itself.
        /// </summary>
        private static bool ValidateSpreadsheetForDuplicatedRows(BatchUpload spreadsheet)
        {
            IEnumerable<UploadDataModel> validateForRepetition =
                spreadsheet.DataToUpload.Where(
                    t =>
                        spreadsheet.DataToUpload.Count(
                            x =>
                                x.IndicatorId == t.IndicatorId && x.Year == t.Year && x.YearRange == t.YearRange &&
                                x.Quarter == t.Quarter && x.Month == t.Month && x.AgeId == t.AgeId && x.SexId == t.SexId &&
                                x.AreaCode == t.AreaCode && x.CategoryTypeId == t.CategoryTypeId &&
                                x.CategoryId == t.CategoryId) > 1);

            if (validateForRepetition.Any())
            {
                foreach (UploadDataModel row in validateForRepetition)
                {
                    var duplicate = new DuplicateRowInSpreadsheetError
                    {
                        RowNumber = row.RowNumber,
                        DuplicateRowMessage = "Indicator " + row.IndicatorId + " is duplicated. "
                    };
                    spreadsheet.DuplicateRowInSpreadsheetErrors.Add(duplicate);
                }
                return true;
            }
            return false;
        }

        private List<UploadValidationFailure> ValidateUploadedRow(DataRow row, int rowNumber, AllowedData allowedData)
        {
            rowNumber = rowNumber + 2;

            var duv = new DataUploadValidation();
            Exception dataConversionException;
            var validationFailures = new List<UploadValidationFailure>();

            //Validate the IndicatorID
            var columnName = UploadColumnNames.IndicatorId;
            dataConversionException = duv.ValidateExpectedDataType(row, columnName,
                DataUploadValidation.DataType.Integer);
            if (dataConversionException != null)
            {
                validationFailures.Add(new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName, null));
            }

            //Validate the Indicator
            UploadValidationFailure uploadValidationFailure = duv.ValidateIndicator(row, rowNumber);
            if (uploadValidationFailure != null)
            {
                //There was an error so log it
                validationFailures.Add(uploadValidationFailure);
            }

            //Validate the Year Data Type
            columnName = UploadColumnNames.Year;
            dataConversionException = duv.ValidateExpectedDataType(row, columnName,
                DataUploadValidation.DataType.Integer);
            if (dataConversionException != null)
            {
                validationFailures.Add(new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName,
                    dataConversionException.Message));
            }
            else
            {
                //Ensure this is a valid Year value
                uploadValidationFailure = duv.ValidateYear(row, rowNumber);
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    validationFailures.Add(uploadValidationFailure);
                }
            }

            //Validate the YearRange
            columnName = UploadColumnNames.YearRange;
            dataConversionException = duv.ValidateExpectedDataType(row, columnName,
                DataUploadValidation.DataType.Integer);
            if (dataConversionException != null)
            {
                validationFailures.Add(new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName,
                    dataConversionException.Message));
            }
            else
            {
                uploadValidationFailure = duv.ValidateYearRange(row, rowNumber);
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    validationFailures.Add(uploadValidationFailure);
                }
            }

            //Validate the Quarter Data Type
            columnName = UploadColumnNames.Quarter;
            dataConversionException = duv.ValidateExpectedDataType(row, columnName,
                DataUploadValidation.DataType.Integer);
            if (dataConversionException != null)
            {
                validationFailures.Add(new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName,
                    dataConversionException.Message));
            }
            else
            {
                //Validate the Quarter Value
                uploadValidationFailure = duv.ValidateQuarter(row, rowNumber);
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    validationFailures.Add(uploadValidationFailure);
                }
            }

            //Validate the Month Data Type
            columnName = UploadColumnNames.Month;
            dataConversionException = duv.ValidateExpectedDataType(row, columnName,
                DataUploadValidation.DataType.Integer);
            if (dataConversionException != null)
            {
                validationFailures.Add(new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName,
                    dataConversionException.Message));
            }
            else
            {
                //Validate the Quarter Value
                uploadValidationFailure = duv.ValidateMonth(row, rowNumber);
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    validationFailures.Add(uploadValidationFailure);
                }
            }

            //Validate the AgeId Data Type
            columnName = UploadColumnNames.AgeId;
            dataConversionException = duv.ValidateExpectedDataType(row, columnName,
                DataUploadValidation.DataType.Integer);
            if (dataConversionException != null)
            {
                validationFailures.Add(new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName,
                    dataConversionException.Message));
            }
            else
            {
                //Validate the Age Id Value
                uploadValidationFailure = duv.ValidateAgeId((int)row.Field<double>(columnName), rowNumber, allowedData.AgeIds);
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    validationFailures.Add(uploadValidationFailure);
                }
            }

            //Validate the SexId Data Type
            columnName = UploadColumnNames.SexId;
            dataConversionException = duv.ValidateExpectedDataType(row, columnName,
                DataUploadValidation.DataType.Integer);
            if (dataConversionException != null)
            {
                validationFailures.Add(new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName,
                    dataConversionException.Message));
            }
            else
            {
                //Ensure this is a valid Sex Id in the DB
                uploadValidationFailure = duv.ValidateSexId((int)row.Field<double>(columnName), rowNumber, allowedData.SexIds);
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    validationFailures.Add(uploadValidationFailure);
                }
            }

            //Validate the AreaCode
            columnName = UploadColumnNames.AreaCode;
            dataConversionException = duv.ValidateExpectedDataType(row, columnName,
                DataUploadValidation.DataType.String);
            if (dataConversionException != null)
            {
                validationFailures.Add(new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName,
                    dataConversionException.Message));
            }
            else
            {
                //Ensure this is a valid Area Code in the DB
                uploadValidationFailure = duv.ValidateArea(row, rowNumber, allowedData.AreaCodes);
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    validationFailures.Add(uploadValidationFailure);
                }
            }

            //Validate the Count
            columnName = UploadColumnNames.Count;
            dataConversionException = duv.ValidateExpectedDataType(row, columnName,
                DataUploadValidation.DataType.NullableDouble);
            if (dataConversionException != null)
            {
                validationFailures.Add(new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName,
                    dataConversionException.Message));
            }

            //Validate the Value
            columnName = UploadColumnNames.Value;
            dataConversionException = duv.ValidateExpectedDataType(row, columnName,
                DataUploadValidation.DataType.Double);
            if (dataConversionException != null)
            {
                validationFailures.Add(new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName,
                    dataConversionException.Message));
            }

            //Validate the LowerCI
            columnName = UploadColumnNames.LowerCI;
            dataConversionException = duv.ValidateExpectedDataType(row, columnName,
                DataUploadValidation.DataType.Double);
            if (dataConversionException != null)
            {
                validationFailures.Add(new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName,
                    dataConversionException.Message));
            }

            //Validate the UpperCI
            columnName = UploadColumnNames.UpperCI;
            dataConversionException = duv.ValidateExpectedDataType(row, columnName,
                DataUploadValidation.DataType.Double);
            if (dataConversionException != null)
            {
                validationFailures.Add(new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName,
                    dataConversionException.Message));
            }

            //Validate the Denominator
            columnName = UploadColumnNames.Denominator;
            dataConversionException = duv.ValidateExpectedDataType(row, columnName,
                DataUploadValidation.DataType.Double);
            if (dataConversionException != null)
            {
                validationFailures.Add(new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName,
                    dataConversionException.Message));
            }

            //Validate the Denominator_2
            columnName = UploadColumnNames.Denominator2;
            dataConversionException = duv.ValidateExpectedDataType(row, columnName,
                DataUploadValidation.DataType.Double);
            if (dataConversionException != null)
            {
                validationFailures.Add(new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName,
                    dataConversionException.Message));
            }

            //Validate the ValueNoteId
            columnName = UploadColumnNames.ValueNoteId;
            dataConversionException = duv.ValidateExpectedDataType(row, columnName,
                DataUploadValidation.DataType.Integer);
            if (dataConversionException != null)
            {
                validationFailures.Add(new UploadValidationFailure(rowNumber, columnName, "Invalid " + columnName,
                    dataConversionException.Message));
            }
            else
            {
                //Ensure this is a value note id in the DB
                uploadValidationFailure = duv.ValidateValueNoteId((int)row.Field<double>(columnName), rowNumber,
                    allowedData.ValueNoteIds);
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    validationFailures.Add(uploadValidationFailure);
                }
            }

            // Validate CategoryTypeId & CategoryId 
            uploadValidationFailure = duv.ValidateCategoryTypeIdAndCategoryId(row, rowNumber, allowedData.Categories);
            if (uploadValidationFailure != null)
            {
                validationFailures.Add(uploadValidationFailure);
            }

            return validationFailures;
        }

        private BatchUpload DoesCoreDataAlreadyExist(BatchUpload batchUpload)
        {
            List<int> uniqueIndicatorList = batchUpload.DataToUpload.Select(x => x.IndicatorId).Distinct().ToList();
            IEnumerable<CoreDataSet> coreDataSetForIndicatorList =
                _profilesReader.GetCoreDataForIndicatorIds(uniqueIndicatorList);

            int rowIndex = 1;
            foreach (UploadDataModel row in batchUpload.DataToUpload)
            {
                rowIndex++;

                CoreDataSet coreDataRecord = coreDataSetForIndicatorList.FirstOrDefault(
                    i =>
                        i.IndicatorId == row.IndicatorId && i.Year == row.Year && i.YearRange == row.YearRange &&
                        i.Quarter == row.Quarter && i.Month == row.Month && i.AgeId == row.AgeId && i.SexId == row.SexId &&
                        i.AreaCode == row.AreaCode && i.CategoryTypeId == row.CategoryTypeId &&
                        i.CategoryId == row.CategoryId);

                if (coreDataRecord != null)
                {
                    var duplicateDataError = new DuplicateRowInDatabaseError
                    {
                        ErrorMessage = "Data already exists for Indicator Id: " + row.IndicatorId,
                        RowNumber = rowIndex,
                        DbValue = coreDataRecord.Value,
                        ExcelValue = row.Value,
                        IndicatorId = row.IndicatorId,
                        AgeId = row.AgeId,
                        SexId = row.SexId,
                        AreaCode = row.AreaCode,
                        Uid = coreDataRecord.Uid
                    };

                    batchUpload.DuplicateRowInDatabaseErrors.Add(duplicateDataError);
                    row.DuplicateExists = true;
                }
            }
            return batchUpload;
        }

        private void GetDistinctIndicators(DataTable excelData)
        {
            _indicators = excelData.AsEnumerable().Select(x => int.Parse(x[0].ToString())).Distinct().ToList();

        }
    }
}