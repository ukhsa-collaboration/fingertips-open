using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Core;
using FingertipsUploadService.ProfileData.Helpers;
using FingertipsUploadService.ProfileData.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace Fpm.Upload
{
    public class SimpleWorkSheetProcessor
    {
        private readonly ProfilesReader _profilesReader = ReaderFactory.GetProfilesReader();
        private readonly CoreDataRepository _coreDataRepository;
        private readonly LoggingRepository _loggingRepository;

        public SimpleWorkSheetProcessor(CoreDataRepository coreDataRepository, LoggingRepository loggingRepository)
        {
            _coreDataRepository = coreDataRepository;
            _loggingRepository = loggingRepository;
        }

        public void Validate(SimpleUpload simpleUpload)
        {
            AllowedData allowedData = new AllowedData(_profilesReader);

            //Validate the Indicator Details First
            DataTable indicatorDetails = ReadSimpleIndicatorWorksheet(simpleUpload.FileName,
                WorksheetNames.SimpleIndicator, false);

            ValidateIndicatorDetails(indicatorDetails, simpleUpload, allowedData);

            DataTable excelData = ReadSimpleIndicatorWorksheet(simpleUpload.FileName,
                WorksheetNames.SimplePholio, true);

            //Validate the data in the spreadsheet
            ValidateSpreadsheetRows(excelData, simpleUpload, allowedData);

            if (simpleUpload.DataToUpload.Count > 0 && simpleUpload.UploadValidationFailures.Count == 0)
            {
                //Validate the spreadsheet data to see if there's replication within it.
                simpleUpload.DuplicateUploadErrorsExist = ValidateSpreadsheetForDuplicatedRows(simpleUpload);
                simpleUpload.DuplicateRowInDatabaseErrors = DoesCoreDataAlreadyExist(simpleUpload).DuplicateRowInDatabaseErrors;
            }

            simpleUpload.UploadPeriod = new TimePeriodTranslater().Translate(simpleUpload);
        }

        public SimpleUpload UploadData(string excelFile, string shortFilename, string selectedWorksheet,
            Guid uploadBatchId, string userName)
        {
            var simpleUpload = new SimpleUpload();
            int rowCount = 0;

            //If we've got this far then all validation has passed. Continue with the upload into coredataset
            //Validate the Indicator Details First
            DataTable indicatorDetails = ReadSimpleIndicatorWorksheet(excelFile, WorksheetNames.SimpleIndicator, false);

            var excelData = ReadSimpleIndicatorWorksheet(excelFile, WorksheetNames.SimplePholio, true);

            var allowedData = new AllowedData(_profilesReader);
            ValidateIndicatorDetails(indicatorDetails, simpleUpload, allowedData);

            for (int i = 0; i < excelData.Rows.Count; i++)
            {
                DataRow row = excelData.Rows[i];
                var rowParser = new UploadSimpleRowParser(row);

                if (rowParser.DoesRowContainData == false)
                {
                    //There isn't an area code or value so assume the end of the data  
                    break;
                }

                rowCount++;

                UploadDataModel upload = rowParser.GetUploadDataModelWithUnparsedValuesSetToDefaults(simpleUpload);
                _coreDataRepository.InsertCoreData(upload.ToCoreDataSet(), uploadBatchId);
            }

            int uploadId = _loggingRepository.InsertUploadAudit(uploadBatchId, userName,
                rowCount, excelFile, selectedWorksheet);

            simpleUpload.ShortFileName = shortFilename;
            simpleUpload.TotalDataRows = rowCount;
            simpleUpload.UploadBatchId = uploadBatchId;
            simpleUpload.Id = uploadId;

            return simpleUpload;
        }

        public SimpleUpload UploadSimpleDataAndArchiveDuplicates(string excelFileName, string shortFilename,
            string selectedWorksheet, Guid uploadBatchId, string userName)
        {
            var simpleUpload = new SimpleUpload
            {
                FileName = excelFileName,
                DataToUpload = new List<UploadDataModel>(),
                DuplicateRowInDatabaseErrors = new List<DuplicateRowInDatabaseError>(),
                DuplicateRowInSpreadsheetErrors = new List<DuplicateRowInSpreadsheetError>(),
                ExcelDataSheets = new List<UploadExcelSheet>(),
                UploadValidationFailures = new List<UploadValidationFailure>(),
                SelectedWorksheet = selectedWorksheet,
                UploadBatchId = uploadBatchId
            };

            Validate(simpleUpload);

            if (simpleUpload.DuplicateRowInDatabaseErrors.Count > 0)
            {
                //Insert the duplicates to the CoreDataset Archive table and delete the coredataset rows in question
                _coreDataRepository.InsertCoreDataArchive(simpleUpload.DuplicateRowInDatabaseErrors, uploadBatchId);

                int uploadId = _loggingRepository.InsertUploadAudit(uploadBatchId, userName,
                    simpleUpload.DataToUpload.Count, excelFileName, selectedWorksheet);

                //Insert the new rows into the CoreDataset table
                foreach (UploadDataModel uploadDataModel in simpleUpload.DataToUpload)
                {
                    _coreDataRepository.InsertCoreData(uploadDataModel.ToCoreDataSet(), uploadBatchId);
                }

                simpleUpload.ShortFileName = shortFilename;
                simpleUpload.TotalDataRows = simpleUpload.DataToUpload.Count;
                simpleUpload.UploadBatchId = uploadBatchId;
                simpleUpload.Id = uploadId;
            }
            return simpleUpload;
        }

        private void ValidateIndicatorDetails(DataTable excelData, SimpleUpload simpleUpload, AllowedData allowedData)
        {
            var duv = new DataUploadValidation();

            GetIndicatorValue(excelData, simpleUpload);
            GetYearValue(excelData, simpleUpload);
            GetYearRange(excelData, simpleUpload);
            GetQuarter(excelData, simpleUpload);
            GetMonth(excelData, simpleUpload);

            GetAgeId(excelData, simpleUpload);
            duv.ValidateAgeId(simpleUpload.AgeId, null, allowedData.AgeIds);

            GetSexId(excelData, simpleUpload);
            duv.ValidateSexId(simpleUpload.SexId, null, allowedData.SexIds);
        }

        private static void GetSexId(DataTable excelData, SimpleUpload simpleUpload)
        {
            try
            {
                simpleUpload.SexId = int.Parse(excelData.Rows[6].ItemArray[1].ToString());
            }
            catch (Exception)
            {
                //Invalid conversion error
                simpleUpload.UploadValidationFailures.Add(new UploadValidationFailure(null, "SexId", "Invalid Sex Id", null));
            }
        }

        private static void GetSimpleValueNoteId(DataTable excelData, SimpleUpload simpleUpload)
        {
            try
            {
                simpleUpload.ValueNoteId = int.Parse(excelData.Rows[7].ItemArray[1].ToString());
            }
            catch (Exception)
            {
                //Invalid conversion error
                simpleUpload.UploadValidationFailures.Add(new UploadValidationFailure(null, "ValueNoteId", "Invalid Note Id", null));
            }
        }

        private static void GetAgeId(DataTable excelData, SimpleUpload simpleUpload)
        {
            try
            {
                simpleUpload.AgeId = int.Parse(excelData.Rows[5].ItemArray[1].ToString());
            }
            catch (Exception)
            {
                //Invalid conversion error
                new UploadValidationFailure(null, "AgeId", "Invalid Age Id", null);
            }
        }

        private static void GetMonth(DataTable excelData, SimpleUpload simpleUpload)
        {
            try
            {
                simpleUpload.Month = int.Parse(excelData.Rows[4].ItemArray[1].ToString());
            }
            catch (Exception)
            {
                //Invalid conversion error
                new UploadValidationFailure(null, "Month", "Invalid Month Value", null);
            }
        }

        private static void GetQuarter(DataTable excelData, SimpleUpload simpleUpload)
        {
            try
            {
                simpleUpload.Quarter = int.Parse(excelData.Rows[3].ItemArray[1].ToString());
            }
            catch (Exception)
            {
                //Invalid conversion error
                new UploadValidationFailure(null, "Quarter", "Invalid Quarter Value", null);
            }
        }

        private static void GetYearRange(DataTable excelData, SimpleUpload simpleUpload)
        {
            try
            {
                simpleUpload.YearRange = int.Parse(excelData.Rows[2].ItemArray[1].ToString());
            }
            catch (Exception)
            {
                //Invalid conversion error
                new UploadValidationFailure(null, "YearRange", "Invalid Year Range Value", null);
            }
        }

        private static void GetYearValue(DataTable excelData, SimpleUpload simpleUpload)
        {
            try
            {
                simpleUpload.Year = int.Parse(excelData.Rows[1].ItemArray[1].ToString());
            }
            catch (Exception)
            {
                //Invalid conversion error
                new UploadValidationFailure(null, "StartYear", "Invalid Start Year", null);
            }
        }

        private static void GetIndicatorValue(DataTable excelData, SimpleUpload simpleUpload)
        {
            try
            {
                simpleUpload.IndicatorId = int.Parse(excelData.Rows[0].ItemArray[1].ToString());

                if (!DataUploadValidation.DoesIndicatorMetaDataExist(simpleUpload))
                {
                    simpleUpload.UploadValidationFailures.Add(new UploadValidationFailure(null, "IndicatorId",
                        "Indicator does not exist", null));
                }
            }
            catch (Exception)
            {
                //Invalid conversion error
                simpleUpload.UploadValidationFailures.Add(new UploadValidationFailure(null, "IndicatorId", "Invalid indicator ID", null));
            }
        }

        private static DataTable ReadSimpleIndicatorWorksheet(string excelFile, string selectedWorksheet,
            bool firstRowHeader)
        {
            OleDbConnection connExcel = UploadFileHelper.OpenSpreadsheet(excelFile, firstRowHeader);

            var adapter = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}]", selectedWorksheet), connExcel);

            var ds = new DataSet();
            adapter.Fill(ds, "indicatorDetails");
            DataTable data = ds.Tables["indicatorDetails"];

            adapter.Dispose();
            connExcel.Close();
            return data;
        }

        private List<UploadValidationFailure> ValidateUploadedRow(DataRow uploadedRow, int rowNumber, List<string> allAreaCodes)
        {
            var duv = new DataUploadValidation();
            Exception dataConversionException;
            var uploadErrors = new List<UploadValidationFailure>();

            //Validate the AreaCode
            UploadValidationFailure uploadValidationFailure = duv.ValidateArea(uploadedRow, rowNumber, allAreaCodes);
            if (uploadValidationFailure != null)
            {
                //There was an error so log it
                uploadErrors.Add(uploadValidationFailure);
            }

            //Validate the Count
            dataConversionException = duv.ValidateExpectedDataType(uploadedRow, UploadColumnNames.Count,
                DataUploadValidation.DataType.NullableDouble);
            if (dataConversionException != null)
            {
                uploadErrors.Add(new UploadValidationFailure(rowNumber, UploadColumnNames.Count, "Invalid Count Figure",
                    dataConversionException.Message));
            }

            //Validate the Value
            dataConversionException = duv.ValidateExpectedDataType(uploadedRow, UploadColumnNames.Value,
                DataUploadValidation.DataType.NullableDouble);
            if (dataConversionException != null)
            {
                uploadErrors.Add(new UploadValidationFailure(rowNumber, UploadColumnNames.Value, "Invalid Value Figure",
                    dataConversionException.Message));
            }

            //Validate the LowerCI
            dataConversionException = duv.ValidateExpectedDataType(uploadedRow, UploadColumnNames.LowerCI,
                DataUploadValidation.DataType.NullableDouble);
            if (dataConversionException != null)
            {
                uploadErrors.Add(new UploadValidationFailure(rowNumber, UploadColumnNames.LowerCI,
                    "Invalid Lower CI Figure", dataConversionException.Message));
            }

            //Validate the UpperCI
            dataConversionException = duv.ValidateExpectedDataType(uploadedRow, UploadColumnNames.UpperCI,
                DataUploadValidation.DataType.NullableDouble);
            if (dataConversionException != null)
            {
                uploadErrors.Add(new UploadValidationFailure(rowNumber, UploadColumnNames.UpperCI,
                    "Invalid Upper CI Figure", dataConversionException.Message));
            }

            //Validate the Denominator
            dataConversionException = duv.ValidateExpectedDataType(uploadedRow, UploadColumnNames.Denominator,
                DataUploadValidation.DataType.NullableDouble);
            if (dataConversionException != null)
            {
                uploadErrors.Add(new UploadValidationFailure(rowNumber, UploadColumnNames.Denominator,
                    "Invalid Denominator Figure", dataConversionException.Message));
            }

            //Validate the ValueNoteId            
            dataConversionException = duv.ValidateExpectedDataType(uploadedRow, UploadColumnNames.ValueNoteId,
                DataUploadValidation.DataType.NullableDouble);

            if (dataConversionException != null)
            {
                uploadErrors.Add(new UploadValidationFailure(rowNumber, UploadColumnNames.ValueNoteId,
                    "Invalid Value Note Id", dataConversionException.Message));
            }

            return uploadErrors;
        }

        private static bool ValidateSpreadsheetForDuplicatedRows(SimpleUpload spreadsheet)
        {
            IEnumerable<UploadDataModel> validateForRepetition =
                spreadsheet.DataToUpload.Where(
                    t =>
                        spreadsheet.DataToUpload.Count(
                            x =>
                                x.IndicatorId == t.IndicatorId && x.Year == t.Year && x.YearRange == t.YearRange &&
                                x.Quarter == t.Quarter && x.Month == t.Month && x.AgeId == t.AgeId && x.SexId == t.SexId &&
                                x.AreaCode == t.AreaCode) > 1); // Category IDs not present in simple upload

            if (validateForRepetition.Any())
            {
                foreach (UploadDataModel row in validateForRepetition)
                {
                    var duplicate = new DuplicateRowInSpreadsheetError
                    {
                        RowNumber = row.RowNumber,
                        DuplicateRowMessage = "Area " + row.AreaCode + " is duplicated. "
                    };
                    spreadsheet.DuplicateRowInSpreadsheetErrors.Add(duplicate);
                }
                return true;
            }

            return false;
        }

        private SimpleUpload DoesCoreDataAlreadyExist(SimpleUpload simpleUpload)
        {
            List<int> uniqueIndicatorList = simpleUpload.DataToUpload.Select(x => x.IndicatorId).Distinct().ToList();
            IEnumerable<CoreDataSet> coreDataSetForIndicatorList =
                _profilesReader.GetCoreDataForIndicatorIds(uniqueIndicatorList);

            int rowIndex = 1;
            foreach (UploadDataModel row in simpleUpload.DataToUpload)
            {
                rowIndex++;

                CoreDataSet coreDataRecord = coreDataSetForIndicatorList.FirstOrDefault(
                    i =>
                        i.IndicatorId == row.IndicatorId && i.Year == row.Year && i.YearRange == row.YearRange &&
                        i.Quarter == row.Quarter && i.Month == row.Month && i.AgeId == row.AgeId && i.SexId == row.SexId &&
                        i.AreaCode == row.AreaCode && i.CategoryId == -1 && i.CategoryTypeId == -1);

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

                    simpleUpload.DuplicateRowInDatabaseErrors.Add(duplicateDataError);
                    row.DuplicateExists = true;
                }
            }
            return simpleUpload;
        }

        private void ValidateSpreadsheetRows(DataTable excelData, SimpleUpload simpleUpload, AllowedData allowedData)
        {
            simpleUpload.ColumnsCount = excelData.Columns.Count;
            simpleUpload.TotalDataRows = excelData.Rows.Count - 1;

            List<string> allAreaCodes = allowedData.AreaCodes;

            for (int i = 0; i < excelData.Rows.Count; i++)
            {
                DataRow row = excelData.Rows[i];
                var rowParser = new UploadSimpleRowParser(row);

                if (rowParser.DoesRowContainData == false)
                {
                    //There isn't an area code or value so assume the end of the data and stop validating now...   
                    break;
                }

                List<UploadValidationFailure> rowErrors = ValidateUploadedRow(row, i + 2, allAreaCodes);
                if (rowErrors.Count == 0)
                {
                    UploadDataModel upload = rowParser.GetUploadDataModel(simpleUpload);
                    upload.RowNumber = i + 1;
                    simpleUpload.DataToUpload.Add(upload);
                }
                else
                {
                    foreach (UploadValidationFailure error in rowErrors)
                        simpleUpload.UploadValidationFailures.Add(error);
                }

                simpleUpload.TotalDataRows = i + 1;
            }
        }
    }
}