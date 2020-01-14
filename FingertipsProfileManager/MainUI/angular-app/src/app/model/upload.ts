export class UploadJobTypes {
    public static readonly Batch = 0;
}

export class UploadJobStatus {
    public static readonly NotStarted = 0;
    public static readonly InProgress = 200;
    public static readonly ValidatingWorksheets = 201;
    public static readonly ValidatingData = 202;
    public static readonly CheckingPermission = 203;
    public static readonly DuplicateCheckInFile = 204;
    public static readonly DuplicateCheckInDatabase = 205;
    public static readonly WritingToDatabase = 206;
    public static readonly OverrideDatabaseDuplicatesConfirmationAwaited = 300;
    public static readonly OverrideDatabaseDuplicatesConfirmationGiven = 301;
    public static readonly OverrideDatabaseDuplicatesConfirmationRefused = 302;
    public static readonly SmallNumberWarningConfirmationAwaited = 310;
    public static readonly SmallNumberWarningConfirmationGiven = 311;
    public static readonly SmallNumberWarningConfirmationRefused = 312;
    public static readonly FailedValidation = 400;
    public static readonly MissingColumn = 401;
    public static readonly UnexpectedError = 500;
    public static readonly SuccessfulUpload = 1000;
}

export class UploadJobProgressStages {
    public static readonly ValidatingWorksheets = 201;
    public static readonly ValidatingData = 202;
    public static readonly CheckingPermission = 203;
    public static readonly CheckingDuplicationInFile = 204;
    public static readonly CheckingDuplicationInDb = 205;
    public static readonly WrittingToDb = 206
}

export class UploadJobErrorType {
    public static readonly PermissionError = 40001;
    public static readonly WorkSheetValidationError = 40002;
    public static readonly DuplicateRowInSpreadsheetError = 40003;
    public static readonly DuplicateRowInDatabaseError = 40004;
    public static readonly ValidationFailureError = 40005;
    public static readonly SmallNumberWarning = 40006;
    public static readonly WrongColumnError = 40007;
    public static readonly UnexpectedError = 40008;
}

export class UploadJobError {
    public Id: number;
    public JobGuid: string;
    public ErrorType: number;
    public ErrorText: string;
    public ErrorJson: string;
    public CreatedAt: Date;
}

export class UploadJob {
    public Id: number;
    public Guid: string;
    public Status: number;
    public DateCreated: Date;
    public UserId: number;
    public UserName: string;
    public TotalRows: number;
    public JobType: number;
    public FileName: string;
    public OriginalFileName: string;
    public ProgressStage: number;
    public TotalRowsCommitted: number;
    public TotalSmallNumberWarnings: number;
    public IsSmallNumberOverrideApplied: boolean;
    public IsDuplicateOverrideApplied: boolean;
    public IsConfirmationRequiredToOverrideDatabaseDuplicates: boolean;
}

export class UploadJobProgress {
    public Jobs: UploadJob[];
    public InProgress: number;
    public InQueue: number;
    public AwaitingConfirmation: number;
}

export class UploadJobProgressStatus {
    public Percent: number;
    public Text: string;
}

export class UploadJobQueue {
    public DateCreated: Date;
    public FileName: string;
    public UserName: string;
    public Status: number;
    public StatusText: string;
    public Guid: string;
}

export class UploadJobSummary {
    public JobStatus: number;
    public ErrorType: number;
    public ErrorText: string;
    public ErrorJson: string;
}

export class TabOptions {
    public static readonly Index = 0;
    public static readonly Progress = 1;
    public static readonly Queue = 2;
}
