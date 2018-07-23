// Type definitions for ./src/fingertipsGlobal.js
import { ElementRef } from "@angular/core";

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
  getAreaList(): Array<Area>;
  // 		 getparentAreaName(): string;

  // 		 getParentTypeName(): string;

  getAreaTypeName(): string;
  getComparatorId(): number;
  getValueNotes(): Array<ValueNote>;
  getValueNoteById(id: number): ValueNote;

  // 		 getIndicatorName(): string;

  // 		 getGroupName(): string;

  // 		 getCurrentTabId(): string;
}

export interface FTIndicatorSearch {
  getIndicatorListId(): any;
  isIndicatorList(): boolean;
  isInSearchMode(): boolean;
  getIndicatorIdList(): IndicatorIdList;
  getProfileIdsForSearch(): Array<number>;
}
export interface IndicatorIdList {
  getAllIds(): Array<number>;
}
export interface IndicatorMetadataHash {
  data: Map<number, IndicatorMetadata>;
}
export interface ValueWithUnit {
  getFullLabel(value: string, options?: any): string;
}
export interface CoreDataHelper {
  addOrderandPercentilesToData(
    coreDataSet: CoreDataSet
  ): Map<string, CoreDataSet>;
  valueWithUnit(unit: Unit): ValueWithUnit;
}
export interface IndicatorHelper {
  getMetadataHash(): Map<number, IndicatorMetadata>;
  getIndicatorIndex(): number;
}
export interface BridgeDataHelper {
  getGroopRoot(): GroupRoot;
  getAllGroupRoots(): GroupRoot[];
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
export interface FTConfig {
  hasRecentTrends: boolean;
  isChangeFromPreviousPeriodShown: boolean;
  profileCollectionUrlKey: string;
  startZeroYAxis: boolean;
  areAnyPdfsForProfile: boolean;
  hasStaticReports: boolean;
  nearestNeighbour: any;
  profileName: string;
  showDataQuality: boolean;
}
export interface FTRoot {
  config(): FTConfig;
  formatCount(dataInfo: CoreDataSetInfo): string;
  getNationalComparatorGrouping(root: GroupRoot): Grouping;
  getSexAndAgeLabel(groupRoot: GroupRoot): string;
  getTrendMarkerImage(trendMarker: TrendMarker, polarity: number): string;
  getIndicatorDataQualityHtml(text: string): string;
  getIndicatorDataQualityTooltipText(dataQualityCount: number): string;
  getParentArea(): Area;
  goToMetadataPage(rootIndex: number | string): void;
  lock(): void;
  logEvent(category: string, action: string, label: string): void;
  model(): FTModel;
  newCoreDataSetInfo(data: CoreDataSet): CoreDataSetInfo;
  newValueDisplayer(unit: Unit): ValueDisplayer;
  newCommaNumber(n: number): CommaNumber;
  newValueNoteTooltipProvider(): ValueNoteTooltipProvider;
  newTooltipManager(): TooltipManager;
  newRecentTrendsTooltip(): RecentTrendsTooltip;
  newComparisonConfig(
    groupRoot: GroupRoot,
    indicatorMetadata: IndicatorMetadata
  ): ComparisonConfig;
  recentTrendSelected(): RecentTrendSelected;
  setAreaCode(areaCode: string): void;
  showIndicatorMetadataInLightbox(elementRef: ElementRef): void;
  showAndHidePageElements(): void;
  showTargetBenchmarkOption(roots): void;
  getTargetLegendHtml(comparisonConfig, metadata): string;
  unlock(): void;
  url(): Url;
  version(): string;
  saveElementAsImage(element, outputFilename): void;
  redirectToPopulationPage(): void;
  bridgeDataHelper: BridgeDataHelper;
  coreDataHelper: CoreDataHelper;
  display: FTDisplay;
  indicatorHelper: IndicatorHelper;
  search: FTIndicatorSearch;
  valueWithUnit: ValueWithUnit;
}
export interface Category {
  Id: number;
  CategoryTypeId: number;
  Name: string;
  ShortName: string;
}
export interface RecentTrendSelected {
  byGroupRoot(rootIndex: number | string): void;
}
export interface CommaNumber {
  rounded(): number;
}
export interface ValueDisplayer {
  byDataInfo(dataInfo: CoreDataSetInfo): string;
  byNumberString(num: any): string;
}
export interface ValueNoteTooltipProvider {
  getHtmlFromNoteId(id: number | string): string;
  getHtml(id: string): string;
  getValueNoteCellText(bits: Array<String>): string;
}
export interface RecentTrendsTooltip {
  getTooltipByData(recentTrends: TrendMarkerResult): string;
}
export interface TooltipManager {
  setHtml(text: string): void;
  positionXY(x: number, y: number): void;
  showOnly(): void;
  hide(): void;
}
export interface ComparisonConfig {
  useTarget: boolean;
  useQuintileColouring: boolean;
  showQuintileLegend: boolean;
  useBlueOrangeBlue: boolean;
  c: boolean;
  comparatorId: number;
}
export interface CoreDataSetInfo {
  areCIs(): boolean;
  areValueAndCIsZero(): boolean;
  getNoteId(): number;
  getValF(): string;
  isCount(): boolean;
  isDefined(): boolean;
  isNote(): boolean;
  isValue(): boolean;
}
export interface IndicatorMetadataTextProperty {
  ColumnName: string;
  DisplayName: string;
  Order: number;
}
export const enum TrendMarker {
  /** Not enough data points */
  CannotBeCalculated = 0,
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
  IID: number;
  Data: CoreDataSet[];
  Grouping: Grouping[];
  DateChanges: DateChanges;
}
export interface DateChanges {
  HasDataChangedRecently: boolean;
  DateOfLastChange: string;
}
export interface Category {
  Id: number;
  CategoryTypeId: number;
  Name: string;
  ShortName: string;
}
export interface IndicatorMetadataText {
  Name: string;
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
export interface IndicatorStats {
  IID: number;
  Sex: Sex;
  Age: Age;
  Stats: IndicatorStatsPercentiles;
  StatsF: IndicatorStatsPercentilesFormatted;
  HaveRequiredValues: boolean;
  Period: string;
  Limits: Limits;
}
export interface IndicatorStatsPercentiles extends Limits {
  P5: number;
  P25: number;
  P75: number;
  P95: number;
  Median: number;
}
export interface IndicatorStatsPercentilesFormatted {
  Min: string;
  Max: string;
  P5: string;
  P25: string;
  Median: string;
  P75: string;
  P95: string;
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
export interface ComparatorMethod {
  Id: number;
  Name: string;
  ShortName: string;
  Description: string;
  Reference: string;
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
  MarkerForMostRecentValueComparedWithPreviousValue: TrendMarker;
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
export interface ValueNote {
  Id: number;
  Text: string;
}
export interface KeyValuePair<TKey, TValue> {
  Key: TKey;
  Value: TValue;
}
export interface Population {
  Code: string;
  IndicatorName: string;
  Labels: string[];
  Period: string;
  /** Key: Sex Id, Value: Population percentages  */
  Values: Map<number, number[]>;
  ListSize: ValueData;
}
export interface PopulationAdHocValues {
  LifeExpectancyFemale: ValueData;
  LifeExpectancyMale: ValueData;
  Qof: CoreDataSet;
  Recommend: ValueData;
}
export interface PopulationSummary {
  Code: string;
  Ethnicity: string;
  GpDeprivationDecile: number;
  AdHocValues: PopulationAdHocValues;
}
export interface AreaTextSearchResult {
  PlaceName: string;
  PolygonAreaCode: string;
  PolygonAreaName: string;
  Easting: number;
  Northing: number;
}
export interface NearByAreas {
  AreaCode: string;
  AreaName: string;
  AddressLine1: string;
  AddressLine2: string;
  Postcode: string;
  AreaTypeID: string;
  Distance: number;
  DistanceValF: string;
  Easting: number;
  Northing: number;
  LatLng: LatitudeLongitude;
}
export interface LatitudeLongitude {
  Lat: number;
  Lng: number;
}
export interface AreaAddress {
  Code: string;
  Name: string;
  Short: string;
  AreaTypeId: number;
  A1: string;
  A2: string;
  A3: string;
  A4: string;
  Postcode: string;
  IsCurrent: boolean;
  Pos: LatitudeLongitude;
}

export interface ParentAreaType {
  Id: number;
  Name: string;
  Short: string;
  IsSearchable: boolean;
  CanBeDisplayedOnMap: boolean;
  ParentAreaTypes: ParentAreaType[];
}

declare module "fingertips" {
  export = FT;
}
declare var FT: FTRoot;
