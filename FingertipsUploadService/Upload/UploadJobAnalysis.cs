using System;
using FingertipsUploadService.ProfileData;
using System.Collections.Generic;
using System.Linq;

namespace FingertipsUploadService.Upload
{
    public class UploadJobAnalysis
    {
        public UploadJobAnalysis() { }

        public UploadJobAnalysis(string fileName)
        {
            FileName = fileName;
        }

        public IList<UploadDataModel> DataToUpload = new List<UploadDataModel>();
        public readonly List<UploadValidationFailure> UploadValidationFailures = new List<UploadValidationFailure>();
        public readonly List<DuplicateRowInDatabaseError> DuplicateRowInDatabaseErrors = new List<DuplicateRowInDatabaseError>();
        public readonly List<DuplicateRowInSpreadsheetError> DuplicateRowInSpreadsheetErrors = new List<DuplicateRowInSpreadsheetError>();
        public readonly List<SmallNumberWarning> SmallNumberWarnings = new List<SmallNumberWarning>();
        public readonly List<string> ColumnErrors = new List<string>();
        public bool DuplicateUploadErrorsExist { get; set; }
        public string FileName { get; private set; }
        public int Id { get; set; }
        public string SelectedWorksheet { get; set; }
        public int FileSize { get; set; }

        public bool HasPholioSheet()
        {
            return SelectedWorksheet != null;
        }

        public bool CanFileBeUploaded()
        {
            return UploadValidationFailures.Any() == false
                && HasPholioSheet();
        }
    }
}