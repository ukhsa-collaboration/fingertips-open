using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Helpers;
using Fpm.ProfileData.Repositories;

namespace Fpm.Upload
{
    public class BatchUploader
    {
        private readonly CoreDataRepository _coreDataRepository;
        private readonly LoggingRepository _loggingRepository;

        public BatchUploader(CoreDataRepository coreDataRepository, LoggingRepository loggingRepository)
        {
            _coreDataRepository = coreDataRepository;
            _loggingRepository = loggingRepository;
        }

        public BatchUpload UploadBatchData(string excelFile, string shortFilename,
            string selectedWorksheet, Guid uploadBatchId, string userName, int duplicateSpreadsheetErrors)
        {
            var batchUpload = new BatchUpload();

            //If we've got this far then all validation has passed. Continue with the upload into coredataset
            try
            {
                var dataToUpload = new List<UploadDataModel>();

                if (duplicateSpreadsheetErrors > 0)
                {
                    batchUpload = new BatchWorkSheetValidator().Validate(excelFile, selectedWorksheet);
                    //If there are duplicates in the spreadsheet then build an object that just uses the first occurence of the duplicate row and ignores the others.
                    CreateUniqueBatchUpload(batchUpload, dataToUpload);

                    foreach (UploadDataModel uniqueUploadDataModel in dataToUpload)
                    {
                        //Insert the new rows into the CoreDataset table
                        _coreDataRepository.InsertCoreData(uniqueUploadDataModel.ToCoreDataSet(), uploadBatchId);
                    }
                }
                else
                {
                    DataTable excelData = UploadFileHelper.ReadWorksheet(excelFile, selectedWorksheet);
                    for (int i = 0; i < excelData.Rows.Count; i++)
                    {
                        UploadDataModel upload = new BatchRowParser(excelData.Rows[i]).GetUploadDataModel();
                        _coreDataRepository.InsertCoreData(upload.ToCoreDataSet(), uploadBatchId);
                        batchUpload.DataToUpload.Add(upload);
                    }

                    dataToUpload = batchUpload.DataToUpload;
                }

                int uploadId = _loggingRepository.InsertUploadAudit(uploadBatchId, userName,
                    dataToUpload.Count, excelFile, selectedWorksheet);

                batchUpload.ShortFileName = shortFilename;
                batchUpload.TotalDataRows = dataToUpload.Count;
                batchUpload.UploadBatchId = uploadBatchId;
                batchUpload.Id = uploadId;
            }
            catch (Exception ex)
            {
                batchUpload.UploadValidationFailures.Add(new UploadValidationFailure { Exception = ex.Message });
            }

            return batchUpload;
        }

        public BatchUpload UploadDataAndArchiveDuplicates(string excelFileName, string shortFilename,
            string selectedWorksheet, Guid uploadBatchId, string userName)
        {
            BatchUpload batchUpload = new BatchWorkSheetValidator()
                .Validate(excelFileName, selectedWorksheet);

            if (batchUpload.DuplicateRowInDatabaseErrors.Any())
            {
                //string duplicateRows = string.Join(",", batchUpload.DuplicateRowInDatabaseErrors.Select(x => x.Uid).ToList());

                //Insert the duplicates to the CoreDataset Archive table and delete the coredataset rows in question
                _coreDataRepository.InsertCoreDataArchive(batchUpload.DuplicateRowInDatabaseErrors, uploadBatchId);

                int uploadId = _loggingRepository.InsertUploadAudit(uploadBatchId, userName,
                    batchUpload.DataToUpload.Count, excelFileName, selectedWorksheet);

                var dataToUpload = new List<UploadDataModel>();
                if (batchUpload.DuplicateRowInSpreadsheetErrors.Count > 0)
                {
                    //If there are duplicates in the spreadsheet then build an object that just uses the first occurence of the duplicate row and ignores the others.
                    CreateUniqueBatchUpload(batchUpload, dataToUpload);
                }
                else
                {
                    dataToUpload = batchUpload.DataToUpload;
                }

                foreach (UploadDataModel uniqueUploadDataModel in dataToUpload)
                {
                    //Insert the new rows into the CoreDataset table
                    _coreDataRepository.InsertCoreData(uniqueUploadDataModel.ToCoreDataSet(), uploadBatchId);
                }

                batchUpload.ShortFileName = shortFilename;
                batchUpload.TotalDataRows = dataToUpload.Count;
                batchUpload.UploadBatchId = uploadBatchId;
                batchUpload.Id = uploadId;
            }

            return batchUpload;
        }

        private static void CreateUniqueBatchUpload(BatchUpload batchUpload, List<UploadDataModel> dataToUpload)
        {
            foreach (UploadDataModel model in batchUpload.DataToUpload)
            {
                if (dataToUpload.Count(
                    x => x.IndicatorId == model.IndicatorId &&
                         x.Year == model.Year &&
                         x.YearRange == model.YearRange &&
                         x.Quarter == model.Quarter &&
                         x.Month == model.Month &&
                         x.AgeId == model.AgeId &&
                         x.SexId == model.SexId &&
                         x.AreaCode == model.AreaCode &&
                         x.CategoryTypeId == model.CategoryTypeId &&
                         x.CategoryId == model.CategoryId) == 0)
                {
                    dataToUpload.Add(model);
                }
            }
        }
    }
}