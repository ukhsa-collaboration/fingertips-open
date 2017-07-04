// Type definitions for ./src/fingertipsGlobal.js
// Project: [LIBRARY_URL_HERE] 
// Definitions by: [YOUR_NAME_HERE] <[YOUR_URL_HERE]> 
// Definitions: https://github.com/borisyankov/DefinitelyTyped


export interface FTModel {
	areaTypeId: number;
	groupId: number;
	parentCode: string;
	profileId: number;
	parentTypeId: number;
	areaCode: string;
	iid: number;
	ageId: number;
	sexId: number;
	nearestNeighbour: string;
}

export interface FTDisplay {
	// 		 getBenchmarkAreaName(): string;

	getAreaName(areaCode: string): string;

	// 		 getparentAreaName(): string;

	// 		 getParentTypeName(): string;

	getAreaTypeName(): string;

	// 		 getIndicatorName(): string;

	// 		 getGroupName(): string;

	// 		 getCurrentTabId(): string;

}
export interface IndicatorMetadataHash {
	data: Map<number, IndicatorMetadata>
}
export interface ValueWithUnit {
	getFullLabel(value: string, options?: any): string
}
export interface CoreDataHelper {
	addOrderandPercentilesToData(coreDataSet: CoreDataSet): Map<string, CoreDataSet>;
	valueWithUnit(unit: Unit): ValueWithUnit;
}
export interface IndicatorHelper {
	getMetadataHash(): Map<number, IndicatorMetadata>;
}
export interface BridgeDataHelper {
	getGroopRoot(): GroupRoot;
	getComparatorId(): number;
	getCurrentComparator(): Area;
}
export interface Url {
	img: string;
	bridge: string;
	corews: string;
	search: string;
	pdf: string;
}
export interface FTRoot {

	model(): FTModel;
	url(): Url;
	display: FTDisplay;
	coreDataHelper: CoreDataHelper;
	bridgeDataHelper: BridgeDataHelper;
	indicatorHelper: IndicatorHelper;
	valueWithUnit: ValueWithUnit

}
export const enum TrendMarker {
	CannotBeCalculated = 0, // i.e. not enough data points
	Increasing = 1,
	Decreasing = 2,
	NoChange = 3
}
export const enum Significance {
	None,
	Worse,
	Same,
	Better
}
export interface Age {
	Id: number;
	Name: string;
}
export interface Area {
	AreaTypeId: number;
	Code: string;
	Name: string;
	ShortName: string;
}
export interface ConfidenceIntervalMethod {
	Description: string;
	Id: number;
	Name: string;
}
export interface CoreDataSet extends ValueWithCIsData {
	AgeId: number;
	AreaCode: string;
	CategoryId: number;
	CategoryTypeId: number;
	Denom: number;
	Denom2: number;
	SexId: number;
	Sig: KeyValuePair<number, number>[];
	Significance: number;
	NoteId: number;
}
export interface Grouping {
	SigLevel: number;
	ComparatorData: CoreDataSet;
	ComparatorId: number;
	ComparatorMethodId: number;
	GroupId: number;
	Period: string;
}
export interface GroupingMetadata {
	Id: number;
	Name: string;
	ProfileId: number;
	Sequence: number;
}
export interface GroupRoot extends RootBase {
	Data: CoreDataSet[];
	Grouping: Grouping[];
}
export interface IndicatorMetadata {
	ConfidenceIntervalMethod: ConfidenceIntervalMethod;
	ConfidenceLevel: number;
	Descriptive: KeyValuePair<string, string>[];
	IID: number;
	Target: TargetConfig;
	Unit: Unit;
	ValueType: ValueType;
	YearType: YearType;

}
export interface IndicatorStatsPercentiles extends Limits {
	P25: number;
	P75: number;
}
export interface Limits {
	Max: number;
	Min: number;
}
export interface RootBase {
	Age: Age;
	ComparatorMethodId: number;
	IID: number;
	PolarityId: number;
	RecentTrends: KeyValuePair<string, TrendMarkerResult>[];
	Sex: Sex;
	StateAge: boolean;
	StateSex: boolean;
	YearRange: number;
}
export interface Sex {
	Id: number;
	Name: string;
	Sequence: number;
}
export interface TargetConfig {
	BespokeKey: string;
	LegendHtml: string;
	LowerLimit: number;
	PolarityId: number;
	UpperLimit: number;
}
export interface TrendDataPoint {
	C: number;
	IsC: boolean;
	L: string;
	Sig: KeyValuePair<number, Significance>[];
	U: string;
	D: number;
	V: string;
	NoteId: number;
}
export interface TrendMarkerResult {
	Marker: TrendMarker;
	Message: string;
	PointsUsed: number;

}
export interface TrendRoot extends RootBase {
	ComparatorData: KeyValuePair<number, CoreDataSet>[][];
	ComparatorValue: KeyValuePair<number, number>[][];
	ComparatorValueFs: KeyValuePair<number, string>[][];
	Data: KeyValuePair<string, TrendDataPoint[]>[];
	Limits: Limits;
	Periods: string[];
}
export interface Unit {
	Id: number;
	Label: string;
	ShowLeft: boolean;
	Value: number;
}
export interface ValueData {
	Count: number;
	Val: number;
	ValF: string;
}
export interface ValueType {
	Id: number;
	Name: string;
}
export interface ValueWithCIsData extends ValueData {
	LoCI: number;
	LoCIF: string;
	UpCI: number;
	UpCIF: string;
}
export interface YearType {
	Id: number;
	Name: string;
}
export interface KeyValuePair<TKey, TValue> {
	Key: TKey;
	Value: TValue;
}
declare module "fingertips" {
	export = FT;
}
declare var FT: FTRoot;

