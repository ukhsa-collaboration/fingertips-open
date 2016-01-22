using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;

namespace Fpm.Upload
{
    public abstract class BaseUpload : IEnumerable
    {
        public List<UploadDataModel> DataToUpload { get; set; }
        public List<UploadExcelSheet> ExcelDataSheets { get; set; }
        public List<UploadValidationFailure> UploadValidationFailures { get; set; }
        public List<DuplicateRowInDatabaseError> DuplicateRowInDatabaseErrors { get; set; }
        public List<DuplicateRowInSpreadsheetError> DuplicateRowInSpreadsheetErrors { get; set; }
        public bool DuplicateUploadErrorsExist { get; set; }
        public int ColumnsCount { get; set; }
        public int TotalDataRows { get; set; }
        public string FileName { get; set; }
        public string ShortFileName { get; set; }
        public int Id { get; set; }
        public Guid UploadBatchId { get; set; }
        public string SelectedWorksheet { get; set; }
        public int FileSize { get; set; }

        public abstract string RequiredWorksheetText();

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool DoesFileExceedMaximumSize()
        {
            return UploadFileHelper.IsFileTooLarge(FileSize);
        }

        public bool HasPholioSheet()
        {
            return SelectedWorksheet != null;
        }

        public bool CanFileBeUploaded()
        {
            return UploadValidationFailures.Any() == false
                && DoesFileExceedMaximumSize() == false
                && HasPholioSheet();
        }
    }
}