declare namespace PholioVisualisation.PholioObjects {
	interface Age {
		Id: number;
		Name: string;
	}
	interface Area {
		AreaTypeId: number;
		Code: string;
		IsCcg: boolean;
		IsCountry: boolean;
		IsCounty: boolean;
		IsCountyAndUADeprivationDecile: boolean;
		IsCurrent: boolean;
		IsGpDeprivationDecile: boolean;
		IsGpPractice: boolean;
		IsPct: boolean;
		IsPheCentre: boolean;
		IsShape: boolean;
		IsUa: boolean;
		Name: string;
		Sequence: number;
		ShortName: string;
	}
	interface ConfidenceIntervalMethod {
		Description: string;
		Id: number;
		Name: string;
	}
	interface CoreDataSet extends PholioVisualisation.PholioObjects.ValueWithCIsData {
		AgeId: number;
		AreaCode: string;
		CategoryId: number;
		CategoryTypeId: number;
		CountPerYear: number;
		Denominator: number;
		Denominator2: number;
		IndicatorId: number;
		IsCountValid: boolean;
		IsDenominator2Valid: boolean;
		IsDenominatorValid: boolean;
		Month: number;
		Quarter: number;
		SexId: number;
		Significance: System.Collections.Generic.KeyValuePair<number, number>[];
		SignificanceAgainstOneBenchmark: number;
		UniqueId: number;
		ValueNoteId: number;
		Year: number;
		YearRange: number;
	}
	interface Grouping {
		Age: PholioVisualisation.PholioObjects.Age;
		AgeId: number;
		AreaTypeId: number;
		BaselineMonth: number;
		BaselineQuarter: number;
		BaselineYear: number;
		ComparatorConfidence: number;
		ComparatorData: PholioVisualisation.PholioObjects.CoreDataSet;
		ComparatorId: number;
		ComparatorMethodId: number;
		ComparatorTargetId: number;
		DataPointMonth: number;
		DataPointQuarter: number;
		DataPointYear: number;
		GroupId: number;
		GroupingId: number;
		IndicatorId: number;
		PolarityId: number;
		Sequence: number;
		Sex: PholioVisualisation.PholioObjects.Sex;
		SexId: number;
		TimePeriodText: string;
		YearRange: number;
	}
	interface GroupingMetadata {
		Id: number;
		Name: string;
		ProfileId: number;
		Sequence: number;
	}
	interface GroupRoot extends PholioVisualisation.PholioObjects.RootBase {
		Data: PholioVisualisation.PholioObjects.CoreDataSet[];
		FirstGrouping: PholioVisualisation.PholioObjects.Grouping;
		Grouping: PholioVisualisation.PholioObjects.Grouping[];
	}
	interface IndicatorMetadata {
		AlwaysShowSpineChart: boolean;
		ConfidenceIntervalMethod: PholioVisualisation.PholioObjects.ConfidenceIntervalMethod;
		ConfidenceIntervalMethodId: number;
		ConfidenceLevel: number;
		DecimalPlacesDisplayed: number;
		Descriptive: System.Collections.Generic.KeyValuePair<string, string>[];
		HasTarget: boolean;
		IndicatorId: number;
		Name: string;
		TargetConfig: PholioVisualisation.PholioObjects.TargetConfig;
		TargetId: number;
		Unit: PholioVisualisation.PholioObjects.Unit;
		UnitId: number;
		ValueType: PholioVisualisation.PholioObjects.ValueType;
		ValueTypeId: number;
		YearType: PholioVisualisation.PholioObjects.YearType;
		YearTypeId: number;
	}
	interface IndicatorStatsPercentiles extends PholioVisualisation.PholioObjects.Limits {
		Percentile25: number;
		Percentile75: number;
	}
	interface Limits {
		Max: number;
		Min: number;
	}
	interface RootBase {
		Age: PholioVisualisation.PholioObjects.Age;
		AgeId: number;
		AreaTypeId: number;
		ComparatorMethodId: number;
		IndicatorId: number;
		PolarityId: number;
		RecentTrends: System.Collections.Generic.KeyValuePair<string, PholioVisualisation.PholioObjects.TrendMarkerResult>[];
		Sex: PholioVisualisation.PholioObjects.Sex;
		SexId: number;
		StateAge: boolean;
		StateSex: boolean;
		YearRange: number;
	}
	interface Sex {
		Id: number;
		Name: string;
		Sequence: number;
	}
	interface TargetConfig {
		BespokeTargetKey: string;
		Description: string;
		Id: number;
		LegendHtml: string;
		LowerLimit: number;
		PolarityId: number;
		UpperLimit: number;
	}
	interface TrendDataPoint {
		Count: number;
		IsCountValid: boolean;
		LowerCIF: string;
		Significance: System.Collections.Generic.KeyValuePair<number, PholioVisualisation.PholioObjects.Significance>[];
		UpperCIF: string;
		Value: number;
		ValueF: string;
		ValueNoteId: number;
	}
	interface TrendMarkerResult {
		ChiSquare: number;
		Intercept: number;
		IsSignificant: boolean;
		Marker: PholioVisualisation.PholioObjects.TrendMarker;
		Message: string;
		NumberOfPointsUsedInCalculation: number;
		Slope: number;
	}
	interface TrendRoot extends PholioVisualisation.PholioObjects.RootBase {
		ComparatorData: System.Collections.Generic.KeyValuePair<number, PholioVisualisation.PholioObjects.CoreDataSet>[][];
		ComparatorValue: System.Collections.Generic.KeyValuePair<number, number>[][];
		ComparatorValueFs: System.Collections.Generic.KeyValuePair<number, string>[][];
		DataPoints: System.Collections.Generic.KeyValuePair<string, PholioVisualisation.PholioObjects.TrendDataPoint[]>[];
		Limits: PholioVisualisation.PholioObjects.Limits;
		Periods: string[];
	}
	interface Unit {
		Id: number;
		Label: string;
		ShowLabelOnLeftOfValue: boolean;
		Value: number;
	}
	interface ValueData {
		Count: number;
		IsValueValid: boolean;
		Value: number;
		ValueFormatted: string;
	}
	interface ValueType {
		Id: number;
		Name: string;
	}
	interface ValueWithCIsData extends PholioVisualisation.PholioObjects.ValueData {
		AreCIsValid: boolean;
		LowerCI: number;
		LowerCIF: string;
		UpperCI: number;
		UpperCIF: string;
	}
	interface YearType {
		Id: number;
		Name: string;
	}
}
declare namespace System.Collections.Generic {
	interface KeyValuePair<TKey, TValue> {
		Key: TKey;
		Value: TValue;
	}
}


