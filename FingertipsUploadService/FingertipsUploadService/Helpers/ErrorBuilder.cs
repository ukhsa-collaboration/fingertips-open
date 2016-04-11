using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.ProfileData.Entities.JobError;
using FingertipsUploadService.Upload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

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

        public static UploadJobError GetSimplePermissionError(Guid jobGuid, List<string> listOfErrors)
        {
            var error = new UploadJobError
            {
                JobGuid = jobGuid,
                ErrorType = UploadJobErrorType.PermissionError,
                ErrorText = listOfErrors.FirstOrDefault()
            };
            return error;
        }

        public static UploadJobError GetBatchPermissionError(Guid jobGuid, List<string> listOfErrors)
        {
            var error = new UploadJobError
            {
                JobGuid = jobGuid,
                ErrorType = UploadJobErrorType.PermissionError,
                ErrorText =
                    "To upload data for the following indicator(s) you will need permission to the owner profile",
                ErrorJson = new JavaScriptSerializer().Serialize(listOfErrors)
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
                ErrorJson = new JavaScriptSerializer().Serialize(duplicateRows)
            };
            return error;
        }

        public static UploadJobError GetDuplicateRowInDatabaseError(Guid jobGuid,
            List<DuplicateRowInDatabaseError> duplicateRows)
        {
            var errorText = GetRowDuplicationInDatabaseError(duplicateRows.Count);
            var error = new UploadJobError
            {
                JobGuid = jobGuid,
                ErrorType = UploadJobErrorType.DuplicateRowInDatabaseError,
                ErrorText = errorText,
                ErrorJson = new JavaScriptSerializer().Serialize(duplicateRows)
            };
            return error;
        }

        public static UploadJobError GetWorkSheetNameValidationError(UploadJob job)
        {
            const string simpleWorksheetName = @" ""Pholio"" worksheet.";
            const string batchWorksheetName = @" ""IndicatorDetails"" and ""PholioData"" worksheets.";
            var simpleOrBatchText = job.JobType == UploadJobType.Simple ? simpleWorksheetName : batchWorksheetName;
            var errorText = new StringBuilder();
            errorText.Append("The uploaded spreadsheet must contain");
            errorText.Append(simpleOrBatchText);
            var error = CreateError(job.Guid, UploadJobErrorType.WorkSheetValidationError,
                errorText.ToString(), null);
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