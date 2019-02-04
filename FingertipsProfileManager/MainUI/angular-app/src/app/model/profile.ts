export class Profile {
    public Id: number;
    public Name: string;
}

export class GroupingPlusName {
    public ComparatorId: number;
    public IndicatorId: number;
    public IndicatorName: string;
    public SexId: number;
    public Sex: string;
    public AgeId: number;
    public Age: string;
    public Sequence: number;
    public YearRange: number;
    public BaselineYear: number;
    public BaselineQuarter: number;
    public BaselineMonth: number;
    public DatapointYear: number;
    public DatapointQuarter: number;
    public DatapointMonth: number;
    public AreaTypeId: number;
    public AreaType: string;
    public ComparatorConfidence: string;
    public ComparatorMethodId: number;
    public ComparatorMethod: string;
    public YearTypeId: number;
    public UsedElsewhere: boolean;
    public ProfileId: number;
    public GroupId: number;
}

export class GroupingSubheading {
    public SubheadingId: number;
    public GroupId: number;
    public AreaTypeId: number;
    public Subheading: string;
    public Sequence: number;
}

export class AreaType {
    Id: number;
    Name: string;
    ShortName: string;
    IsCurrent: boolean;
    IsSupported: boolean;
    IsSearchable: boolean;
    CanBeDisplayedOnMap: boolean;
}

export class ReorderIndicator {
    IndicatorId?: number;
    IndicatorName: string;
    Sequence?: number;
    Sex: string;
    Age: string;
    IsSubheading: boolean;
}
