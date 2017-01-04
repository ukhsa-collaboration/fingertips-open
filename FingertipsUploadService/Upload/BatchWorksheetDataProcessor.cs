using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.ProfileData.Helpers;
using FingertipsUploadService.ProfileData.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace FingertipsUploadService.Upload
{
    public class BatchWorksheetDataProcessor : IBatchWorksheetDataProcessor
    {
        private readonly ProfilesReader _profilesReader = ReaderFactory.GetProfilesReader();
        private readonly CoreDataRepository _coreDataRepository;
        private readonly LoggingRepository _loggingRepository;
        private readonly Logger _logger;
        private List<int> _indicators = new List<int>();

        public BatchWorksheetDataProcessor(CoreDataRepository coreDataRepository, LoggingRepository loggingRepository,
            Logger logger)
        {
            _coreDataRepository = coreDataRepository;
            _loggingRepository = loggingRepository;
            _logger = logger;
        }

        public BatchUpload Validate(DataTable indicators, BatchUpload batchUpload)
        {
            var allowedData = new AllowedData(_profilesReader);

            // Validate spreadsheet data
            ValidateSpreadsheetRows(indicators, allowedData, batchUpload);

            if (batchUpload.DataToUpload.Any())
            {
                //Validate the spreadsheet data to see if there's duplication within it.
                batchUpload.DuplicateUploadErrorsExist = ValidateSpreadsheetForDuplicatedRows(batchUpload);

                batchUpload.DuplicateRowInDatabaseErrors =
                    new CoreDataSetDuplicateChecker().GetDuplicates(batchUpload.DataToUpload, _coreDataRepository,
                        UploadJobType.Batch);
            }

            GetDistinctIndicators(indicators);

            return batchUpload;
        }

        private void ValidateSpreadsheetRows(DataTable batchData, AllowedData allowedData, BatchUpload batchUpload)
        {
            batchUpload.ColumnsCount = batchData.Columns.Count;
            batchUpload.TotalDataRows = batchData.Rows.Count;

            //TODO FIN-841 check columns are all present

            for (int i = 0; i < batchData.Rows.Count; i++)
            {
                DataRow row = batchData.Rows[i];

                List<UploadValidationFailure> validationFailures = ValidateUploadedRow(row, i, allowedData);
                if (validationFailures.Count == 0)
                {
                    UploadDataModel upload = new BatchRowParser(row).GetUploadDataModel();
                    upload.RowNumber = i + 2;

                    batchUpload.DataToUpload.Add(upload);
                }
                else
                {
                    foreach (UploadValidationFailure error in validationFailures)
                        batchUpload.UploadValidationFailures.Add(error);
                }
            }
        }

        private List<UploadValidationFailure> ValidateUploadedRow(DataRow row, int rowNumber, AllowedData allowedData)
        {
            rowNumber = rowNumber + 2;

            var duv = new DataUploadValidation();
            Exception dataConversionException;
            var validationFailures = new List<UploadValidationFailure>();

            //Validate the IndicatorID
            string columnName = UploadColumnNames.IndicatorId;
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
                uploadValidationFailure = duv.ValidateAgeId((int)row.Field<double>(columnName), rowNumber,
                    allowedData.AgeIds);
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
                uploadValidationFailure = duv.ValidateSexId((int)row.Field<double>(columnName), rowNumber,
                    allowedData.SexIds);
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


        private void GetDistinctIndicators(DataTable excelData)
        {
            _indicators = excelData.AsEnumerable().Select(x => int.Parse(x[0].ToString())).Distinct().ToList();
        }

        public List<int> GetIndicatorIdsInBatch()
        {
            return _indicators;
        }

        public BatchUpload UploadData(DataTable indicators, UploadJob job)
        {
            var batchUpload = new BatchUpload();

            int totalRows = indicators.Rows.Count;
            var indicatorIds = new HashSet<int>();

            // Upload core data
            for (int i = 0; i < totalRows; i++)
            {
                var upload = new BatchRowParser(indicators.Rows[i]).GetUploadDataModel();
                _coreDataRepository.InsertCoreData(upload.ToCoreDataSet(), job.Guid);
                batchUpload.DataToUpload.Add(upload);
                indicatorIds.Add(upload.IndicatorId);

                // Log periodically that rows have been uploaded
                if (i % 1000 == 0)
                {
                    LogRowsUploaded(i + 1);
                }
            }

            LogRowsUploaded(totalRows);

            // Delete any precalculated core data as it will need to be calculated again
            foreach (var indicatorId in indicatorIds)
            {
                _coreDataRepository.DeletePrecalculatedCoreData(indicatorId);
            }

            var dataToUpload = batchUpload.DataToUpload;

            int uploadId = _loggingRepository.InsertUploadAudit(job.Guid, job.Username, totalRows, job.Filename,
                WorksheetNames.BatchPholio);

            batchUpload.ShortFileName = Path.GetFileName(job.Filename);
            batchUpload.TotalDataRows = dataToUpload.Count;
            batchUpload.UploadBatchId = job.Guid;
            batchUpload.Id = uploadId;

            return batchUpload;
        }

        private void LogRowsUploaded(int rowCount)
        {
            if (_logger != null)
            {
                _logger.Info(rowCount + " rows uploaded");
            }
        }

        public void ArchiveDuplicates(IEnumerable<DuplicateRowInDatabaseError> duplicateRows, UploadJob job)
        {
            _coreDataRepository.InsertCoreDataArchive(duplicateRows, job.Guid);
        }
    }
}