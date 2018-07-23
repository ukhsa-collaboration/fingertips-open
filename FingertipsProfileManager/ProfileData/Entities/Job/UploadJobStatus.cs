
namespace Fpm.ProfileData.Entities.Job
{
    public enum UploadJobStatus
    {
        NotStarted = 0,
        InProgress = 200,
        OverrideDatabaseDuplicatesConfirmationAwaited = 300,
        OverrideDatabaseDuplicatesConfirmationGiven = 301,
        OverrideDatabaseDuplicatesConfirmationRefused = 302,
        SmallNumberWarningConfirmationAwaited = 310,
        SmallNumberWarningConfirmationGiven = 311,
        SmallNumberWarningConfirmationRefused = 312,
        FailedValidation = 400,
        UnexpectedError = 500,
        SuccessfulUpload = 1000,
    }

    public enum ProgressStage
    {
        ValidatingWorksheets = 201,
        ValidatingData = 202,
        CheckingPermission = 203,
        CheckingDuplicationInFile = 204,
        CheckingDuplicationInDb = 205,
        WrittingToDb = 206
    }
}
