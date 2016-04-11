namespace FingertipsUploadService.Upload
{
    public class UploadValidationFailure
    {
        public UploadValidationFailure()
        {
            
        }

        public UploadValidationFailure(int? rowNumber, string fieldName, string errorMessage, string exceptionMessage)
        {
            RowNumber = rowNumber;
            FieldName = fieldName;
            ErrorMessage = errorMessage;
            Exception = exceptionMessage;
        }

        public int? RowNumber { get; set; }
        public string FieldName { get; set; }
        public string ErrorMessage { get; set; }
        public string Exception { get; set; }
    }
}
