/* This class contains the functionality to validate the worksheet names for simple upload*/

using System.Collections.Generic;
using System.Data.OleDb;

namespace FingertipsUploadService.Upload
{
    public class SimpleUploadWorkSheetsesValidator : ISimpleUploadWorkSheetsValidator
    {
        public bool Validate(SimpleUpload simpleUpload)
        {
            OleDbConnection connExcel = UploadFileHelper.OpenSpreadsheet(simpleUpload.FileName, false);
            List<UploadExcelSheet> excelDataSheets = UploadFileHelper.GetAvailableWorksheets(connExcel);
            connExcel.Close();

            var areValidWorksheets = ValidateSimpleUploadWorksheetsExist(excelDataSheets);

            if (!areValidWorksheets) return false;

            // simple upload values from excel file
            simpleUpload.SelectedWorksheet = WorksheetNames.SimpleIndicator;
            simpleUpload.ExcelDataSheets = excelDataSheets;
            simpleUpload.UploadBatchId = simpleUpload.UploadBatchId;

            return true;
        }


        public bool Validate(List<string> worksheets)
        {
            var validSelectSheet = false;
            var validPholioSheet = false;

            foreach (var worksheet in worksheets)
            {
                if (worksheet == WorksheetNames.SimplePholio)
                {
                    validPholioSheet = true;
                }

                if (worksheet == WorksheetNames.SimpleIndicator)
                {
                    validSelectSheet = true;
                }
            }

            return validSelectSheet && validPholioSheet;
        }

        private bool ValidateSimpleUploadWorksheetsExist(List<UploadExcelSheet> excelDataSheets)
        {
            var validSelectSheet = false;
            var validPholioSheet = false;

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