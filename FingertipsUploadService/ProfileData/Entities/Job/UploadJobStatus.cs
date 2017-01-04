namespace FingertipsUploadService.ProfileData.Entities.Job
{
    public enum UploadJobStatus
    {
        NotStarted = 0,
        InProgress = 200,
        ConfirmationAwaited = 300,
        ConfirmationGiven = 301,
        ConfirmationRefused = 302,
        FailedValidation = 400,
        UnexpectedError = 500,
        SuccessfulUpload = 1000
    }

    public enum ProgressStage
    {
        ValidatingWorksheets = 201,
        ValidatingData = 202,
        CheckingPermission = 203,
        CheckingDuplicationInFile = 204,
        CheckingDuplicationInDb = 205,
        WritingToDb = 206
    }
}