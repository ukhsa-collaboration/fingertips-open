using FingertipsUploadService.ProfileData;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;

namespace Fpm.Upload
{
    public class BatchUploadValidator
    {
        private readonly BatchUpload batchUpload;

        public BatchUploadValidator(HttpPostedFileBase excelFile)
        {
            const string CSV = ".csv";
            Guid uploadBatchId = Guid.NewGuid();
            string filePath = UploadFileHelper.GetFilePath(excelFile.FileName, uploadBatchId);
            excelFile.SaveAs(filePath);

            var fileName = Path.GetFileNameWithoutExtension(excelFile.FileName);
            var fileExt = Path.GetExtension(excelFile.FileName);

            // In case of CSV we save csv file to server and then convert it into xls 
            // and keep that xls file on server too.
            if (fileExt == CSV)
            {
                IWorkbookSet workbookSet = Factory.GetWorkbookSet();
                IWorkbook workbook = workbookSet.Workbooks.Open(filePath);
                workbook.Worksheets[0].Name = "Pholio";

                filePath = UploadFileHelper.GetFilePath(fileName, uploadBatchId);
                workbook.SaveAs(filePath + ".xls", SpreadsheetGear.FileFormat.Excel8);
            }

            batchUpload = new BatchUpload
            {
                ShortFileName = excelFile.FileName,
                FileName = filePath,
                FileSize = excelFile.ContentLength
            };

            if (IsFileTooLarge == false)
            {
                OleDbConnection connExcel = UploadFileHelper.OpenSpreadsheet(filePath, true);

                List<UploadExcelSheet> excelDataSheets =
                    UploadFileHelper.GetAvailableWorksheets(connExcel);

                IsPholioSheet =
                    excelDataSheets.Any(
                        x => x.SheetName.Equals(WorksheetNames.BatchPholio, StringComparison.InvariantCultureIgnoreCase));

                if (IsPholioSheet)
                {
                    batchUpload.SelectedWorksheet = WorksheetNames.BatchPholio;
                    batchUpload.ExcelDataSheets = excelDataSheets;
                    batchUpload.UploadBatchId = uploadBatchId;
                }

                connExcel.Close();
            }
        }

        public BatchUpload BatchUpload
        {
            get { return batchUpload; }
        }

        public bool IsPholioSheet { get; private set; }

        public bool IsFileTooLarge
        {
            get { return UploadFileHelper.IsFileTooLarge(batchUpload.FileSize); }
        }

        public List<int> IndicatorIdsInBatch { get; set; }

        public BatchUpload Validate()
        {
            if (IsFileTooLarge)
            {
                throw new FpmException("File size is too large");
            }
            if (IsPholioSheet == false)
            {
                throw new FpmException("No PHOLIO worksheet");
            }

            var batchWorkSheetValidator = new BatchWorkSheetValidator();
            BatchUpload validatedSpreadsheet = batchWorkSheetValidator.Validate(batchUpload.FileName, batchUpload.SelectedWorksheet);

            validatedSpreadsheet.SelectedWorksheet = batchUpload.SelectedWorksheet;
            validatedSpreadsheet.ExcelDataSheets = batchUpload.ExcelDataSheets;
            validatedSpreadsheet.UploadBatchId = batchUpload.UploadBatchId;
            validatedSpreadsheet.FileName = batchUpload.FileName;
            validatedSpreadsheet.ShortFileName = batchUpload.ShortFileName;

            IndicatorIdsInBatch = batchWorkSheetValidator.GetIndicators;

            return validatedSpreadsheet;
        }
    }
}