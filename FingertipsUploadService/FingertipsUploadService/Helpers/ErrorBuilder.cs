using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.JobError;
using FingertipsUploadService.Upload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using UploadJob = FingertipsUploadService.ProfileData.Entities.Job.UploadJob;

namespace FingertipsUploadService.Helpers
{
    public class ErrorBuilder
    {
        private static string GetRowDuplicationInDatabaseError(int noOfDuplicateRows)
        {
            var isOrAre = noOfDuplicateRows == 1 ? "is" : "are";
            var sb = new StringBuilder();
            sb.Append("There ")
                .Append(isOrAre)
                .Append(" ")
                .Append(noOfDuplicateRows)
                .Append(" duplicated row(s) in the PHOLIO database. ")
                .Append(
                    "This file can still be uploaded. The duplicate rows that already exist in PHOLIO will be archived and replaced with those in this file.");
            return sb.ToString();
        }

        private static string GetRowDuplicationInFileErrorText(int noOfDuplicateRowsInFile)
        {
            var sb = new StringBuilder();
            sb.Append("There are ")
                .Append(noOfDuplicateRowsInFile)
                .Append(" ")
                .Append("rows in the batch file that are duplicated. ")
                .Append(
                    "This file can still be uploaded. Where rows are duplicated in the batch file the first occurence of the row will be uploaded and subsequent duplicates will be ignored.");
            return sb.ToString();
        }

        public static UploadJobError GetBatchPermissionError(Guid jobGuid, List<string> listOfErrors, bool doesAllIndicatorsExist)
        {
            var error = new UploadJobError
            {
                JobGuid = jobGuid,
                ErrorType = UploadJobErrorType.PermissionError,
                ErrorText = doesAllIndicatorsExist ?
                    "To upload data for the following indicator(s) you will need permission to the owner profile" :
                    "Following indicator(s) does not exist",

                ErrorJson = new JavaScriptSerializer().Serialize(listOfErrors.Take(1000))
            };
            return error;
        }

        public static UploadJobError GetSmallNumberWarning(UploadJob job, List<SmallNumberWarning> warnings)
        {
            var error = new UploadJobError
            {
                JobGuid = job.Guid,
                ErrorType = UploadJobErrorType.SmallNumberWarning,
                ErrorText = "Found small number",
                ErrorJson = new JavaScriptSerializer().Serialize(warnings.Take(1000))
            };

            return error;
        }

        public static UploadJobError GetConversionError(Guid jobGuid, List<UploadValidationFailure> validationFailures)
        {
            var error = new UploadJobError
            {
                JobGuid = jobGuid,
                ErrorType = UploadJobErrorType.ValidationFailureError,
                ErrorText = "Data type conversion errors occurred in the following spreadsheet rows",
                ErrorJson = new JavaScriptSerializer().Serialize(validationFailures.Take(1000))
            };
            return error;
        }

        public static UploadJobError GetDuplicateRowInSpreadsheetError(Guid jobGuid,
            List<DuplicateRowInSpreadsheetError> duplicateRows)
        {
            var error = new UploadJobError
            {
                JobGuid = jobGuid,
                ErrorType = UploadJobErrorType.DuplicateRowInSpreadsheetError,
                ErrorText = GetRowDuplicationInFileErrorText(duplicateRows.Count),
                ErrorJson = new JavaScriptSerializer().Serialize(duplicateRows.Take(1000))
            };
            return error;
        }

        public static UploadJobError GetDuplicateRowInDatabaseError(Guid jobGuid,
            List<DuplicateRowInDatabaseError> duplicateRows)
        {
            var errorText = GetRowDuplicationInDatabaseError(duplicateRows.Count);
            const int maxDuplicateRows = 500;
            var duplicateRowsForError = duplicateRows.Count > maxDuplicateRows
                ? duplicateRows.Take(30) // we only show 30 duplicate rows in UI, it is pointless to return more than 30 rows.
                : duplicateRows;

            var error = new UploadJobError
            {
                JobGuid = jobGuid,
                ErrorType = UploadJobErrorType.DuplicateRowInDatabaseError,
                ErrorText = errorText,
                ErrorJson = new JavaScriptSerializer().Serialize(duplicateRowsForError)
            };
            return error;
        }

        public static UploadJobError GetWorkSheetNameValidationError(UploadJob job)
        {
            var error = CreateError(job.Guid, UploadJobErrorType.WorkSheetValidationError,
                "The uploaded spreadsheet must contain \"PHOLIO\" worksheet.", null);
            return error;
        }

        public static UploadJobError GetColumnNameValidationError(UploadJob job, List<string> columnNames)
        {
            var errorText = "Wrong or missing column";
            var error = CreateError(job.Guid, UploadJobErrorType.WrongColumnError, errorText, new JavaScriptSerializer().Serialize(columnNames));
            return error;
        }

        private static UploadJobError CreateError(Guid jobGuid, UploadJobErrorType errorType, string errorText,
            string errorJson)
        {
            var error = new UploadJobError
            {
                JobGuid = jobGuid,
                ErrorType = errorType,
                ErrorText = errorText,
                ErrorJson = errorJson
            };
            return error;
        }
    }
}