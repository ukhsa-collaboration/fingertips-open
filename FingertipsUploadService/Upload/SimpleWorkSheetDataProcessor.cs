using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.ProfileData.Helpers;
using FingertipsUploadService.ProfileData.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace FingertipsUploadService.Upload
{
    public class SimpleWorksheetDataProcessor : ISimpleWorksheetDataProcessor
    {
        private readonly ProfilesReader _profilesReader = ReaderFactory.GetProfilesReader();
        private readonly CoreDataRepository _coreDataRepository;
        private readonly LoggingRepository _loggingRepository;

        public SimpleWorksheetDataProcessor(CoreDataRepository coreDataRepository, LoggingRepository loggingRepository)
        {
            _coreDataRepository = coreDataRepository;
            _loggingRepository = loggingRepository;
        }

        public void Validate(DataTable indicatorData, DataTable pholioData, SimpleUpload simpleUpload)
        {
            AllowedData allowedData = new AllowedData(_profilesReader);
            ValidateIndicatorDetails(indicatorData, simpleUpload, allowedData);
            ValidateSpreadsheetRows(pholioData, simpleUpload, allowedData);

            if (simpleUpload.DataToUpload.Count > 0 && simpleUpload.UploadValidationFailures.Count == 0)
            {
                //Validate the spreadsheet data to see if there's replication within it.
                simpleUpload.DuplicateUploadErrorsExist = ValidateSpreadsheetForDuplicatedRows(simpleUpload);
                simpleUpload.DuplicateRowInDatabaseErrors =
                    new CoreDataSetDuplicateChecker().GetDuplicates(simpleUpload.DataToUpload, _coreDataRepository,
                        UploadJobType.Simple);
            }
            simpleUpload.UploadPeriod = new TimePeriodTranslater().Translate(simpleUpload);
        }

        public SimpleUpload UploadData(DataTable indicatorDetails, DataTable pholioData, UploadJob job)
        {
            var simpleUpload = new SimpleUpload();
            var rowCount = 0;

            var allowedData = new AllowedData(_profilesReader);

            // TODO: find out why were are calling it again here as its caled in Validate()
            ValidateIndicatorDetails(indicatorDetails, simpleUpload, allowedData);

            var dataToUpload = new List<UploadDataModel>();

            for (int i = 0; i < pholioData.Rows.Count; i++)
            {
                DataRow row = pholioData.Rows[i];
                var rowParser = new UploadSimpleRowParser(row);

                if (rowParser.DoesRowContainData == false)
                {
                    //There isn't an area code or value so assume the end of the data  
                    break;
                }

                rowCount++;

                var upload = rowParser.GetUploadDataModelWithUnparsedValuesSetToDefaults(simpleUpload);
                _coreDataRepository.InsertCoreData(upload.ToCoreDataSet(), job.Guid);
                dataToUpload.Add(upload);
            }

            simpleUpload.DataToUpload = dataToUpload;

            int uploadId = _loggingRepository.InsertUploadAudit(job.Guid, job.Username,
                rowCount, job.Filename, WorksheetNames.SimplePholio);

            simpleUpload.ShortFileName = Path.GetFileName(job.Filename);
            simpleUpload.TotalDataRows = rowCount;
            simpleUpload.UploadBatchId = job.Guid;
            simpleUpload.Id = uploadId;

            return simpleUpload;
        }


        // Validate indicator details
        private void ValidateIndicatorDetails(DataTable dataTable, SimpleUpload simpleUpload, AllowedData allowedData)
        {
            var duv = new DataUploadValidation();

            GetIndicatorValue(dataTable, simpleUpload);
            GetYearValue(dataTable, simpleUpload);
            GetYearRange(dataTable, simpleUpload);
            GetQuarter(dataTable, simpleUpload);
            GetMonth(dataTable, simpleUpload);

            GetAgeId(dataTable, simpleUpload);
            duv.ValidateAgeId(simpleUpload.AgeId, null, allowedData.AgeIds);

            GetSexId(dataTable, simpleUpload);
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

        private List<UploadValidationFailure> ValidateUploadedRow(DataRow uploadedRow, int rowNumber,
            List<string> allAreaCodes)
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
            else
            {
                var allowedData = new AllowedData(_profilesReader);
                var columnName = UploadColumnNames.ValueNoteId;
                //Ensure this is a value note id in the DB
                uploadValidationFailure = duv.ValidateValueNoteId((int)uploadedRow.Field<double>(columnName), rowNumber,
                    allowedData.ValueNoteIds);
                if (uploadValidationFailure != null)
                {
                    //There was an error so log it
                    uploadErrors.Add(uploadValidationFailure);
                }
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

        public void ArchiveDuplicates(IEnumerable<DuplicateRowInDatabaseError> duplicateRows, UploadJob job)
        {
            _coreDataRepository.InsertCoreDataArchive(duplicateRows, job.Guid);
        }
    }
}