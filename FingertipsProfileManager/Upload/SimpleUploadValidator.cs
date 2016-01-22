using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Web;
using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;

namespace Fpm.Upload
{
    public class SimpleUploadValidator
    {
        private SimpleUpload simpleUpload;
        private readonly CoreDataRepository _coreDataRepository;
        private readonly LoggingRepository _loggingRepository;

        public SimpleUpload SimpleUpload
        {
            get { return simpleUpload; }
        }

        public List<int> IndicatorId { get; set; }

        public bool AreValidWorksheets { get; private set; }

        public SimpleUploadValidator(HttpPostedFileBase excelFile, CoreDataRepository coreDataRepository, LoggingRepository loggingRepository)
        {
            _coreDataRepository = coreDataRepository;
            _loggingRepository = loggingRepository;

            Guid uploadBatchId = Guid.NewGuid();

            string filePath = UploadFileHelper.GetFilePath(excelFile.FileName, uploadBatchId);
            excelFile.SaveAs(filePath);

            simpleUpload = new SimpleUpload
            {
                ShortFileName = excelFile.FileName,
                FileName = filePath,
                FileSize = excelFile.ContentLength,
                DataToUpload = new List<UploadDataModel>(),
                DuplicateRowInDatabaseErrors = new List<DuplicateRowInDatabaseError>(),
                DuplicateRowInSpreadsheetErrors = new List<DuplicateRowInSpreadsheetError>(),
                ExcelDataSheets = new List<UploadExcelSheet>(),
                UploadValidationFailures = new List<UploadValidationFailure>()
            };

            if (IsFileTooLarge == false)
            {

                OleDbConnection connExcel = UploadFileHelper.OpenSpreadsheet(filePath, false);
                List<UploadExcelSheet> excelDataSheets = UploadFileHelper.GetAvailableWorksheets(connExcel);

                AreValidWorksheets = ValidateSimpleUploadWorksheetsExist(excelDataSheets);

                if (AreValidWorksheets)
                {
                    simpleUpload.SelectedWorksheet = WorksheetNames.SimpleIndicator;
                    simpleUpload.ExcelDataSheets = excelDataSheets;
                    simpleUpload.UploadBatchId = uploadBatchId;
                }

                connExcel.Close();
            }
        }

        public bool IsFileTooLarge
        {
            get { return UploadFileHelper.IsFileTooLarge(simpleUpload.FileSize); }
        }

        public SimpleUpload Validate()
        {
            if (IsFileTooLarge)
            {
                throw new FpmException("File size is too large");
            }
            if (AreValidWorksheets == false)
            {
                throw new FpmException("Not all worksheets are present");
            }

            new SimpleWorkSheetProcessor(_coreDataRepository,_loggingRepository).Validate(simpleUpload);

            return simpleUpload;
        }

        private static bool ValidateSimpleUploadWorksheetsExist(List<UploadExcelSheet> excelDataSheets)
        {
            bool validSelectSheet = false;
            bool validPholioSheet = false;

            foreach (UploadExcelSheet uploadExcelSheet in excelDataSheets)
            {
                if (uploadExcelSheet.SheetName == WorksheetNames.SimplePholio)
                {
                    validPholioSheet = true;
                }

                if (uploadExcelSheet.SheetName == WorksheetNames.SimpleIndicator)
                {
                    validSelectSheet = true;
                }
            }
            return validSelectSheet && validPholioSheet;
        }
    }
}