export class LightBoxConfig {
    public Type: number;
    public Html: string;
    public Title: string;
    public OkButtonText: string;
    public CancelButtonText: string;
    public ActionType: string;
}

export class LightBoxTypes {
    public static readonly Ok = 1;
    public static readonly OkCancel = 2;
}
