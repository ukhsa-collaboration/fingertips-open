// Type definitions for ./src/fingertipsGlobal.js
import { ElementRef } from '@angular/core';
import { TrendMarkerArea } from '../compare-area/compare-area';

declare let FT: FTRoot;

declare module 'fingertips' {
  export = FT;
}

export interface Age {
  Id: number;
  Name: string;
  Sequence: number;
}
export interface Area {
  AreaTypeId: number;
  Code: string;
  Name: string;
  Short: string;
  Rank: string;
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

export interface AreaList {
  Id: number;
  ListName: string;
  AreaTypeId: number;
  UserId: string;
  CreatedOn: any;
  UpdatedOn: any;
  PublicId: string;
  AreaListAreaCodes: AreaListAreaCode[];
}

export interface AreaListAreaCode {
  Id: number;
  AreaListId: number;
  AreaCode: string;
}

export interface AreaTextSearchResult {
  PlaceName: string;
  PolygonAreaCode: string;
  PolygonAreaName: string;
  Easting: number;
  Northing: number;
}

export interface AreaType {
  Id: number;
  Name: string;
  Short: string;
  IsCurrent: boolean;
  IsSupported: boolean;
  IsSearchable: boolean;
  CanBeDisplayedOnMap: boolean;
}

export interface BridgeDataHelper {
  getGroopRoot(): GroupRoot;
  getAllGroupRoots(): GroupRoot[];
  getComparatorId(): number;
  getCurrentComparator(): Area;
}

export interface Category {
  Id: number;
  CategoryTypeId: number;
  Name: string;
  ShortName: string;
}

export interface CategoryType {
  Id: number;
  Name: string;
  ShortName: string;
  Description: string;
  Notes: string;
  Categories: Category[];
}

export interface ComparatorMethod {
  Id: number;
  Name: string;
  ShortName: string;
  Description: string;
  Reference: string;
}

export interface ComparisonConfig {
  useTarget: boolean;
  useQuintileColouring: boolean;
  showQuintileLegend: boolean;
  useBlueOrangeBlue: boolean;
  useRagColours: boolean;
  c: boolean;
  comparatorId: number;
}

export interface ConfidenceIntervalMethod {
  Description: string;
  Id: number;
  Name: string;
}

export interface CoreDataHelper {
  addOrderandPercentilesToData(
    coreDataSets: CoreDataSet[]
  ): Map<string, CoreDataSet>;
  valueWithUnit(unit: Unit): ValueWithUnit;
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
  Year: number;
}

export interface CoreDataSetInfo {
  data: CoreDataSet;
  areCIs(): boolean;
  areValueAndCIsZero(): boolean;
  getNoteId(): number;
  getValF(): string;
  isCount(): boolean;
  isDefined(): boolean;
  isNote(): boolean;
  isValue(): boolean;
}

export interface DateChanges {
  HasDataChangedRecently: boolean;
  DateOfLastChange: string;
}

export interface FTConfig {
  areAnyPdfsForProfile: boolean;
  environment: string;
  hasRecentTrends: boolean;
  hasStaticReports: boolean;
  ignoredSpineChartAreas: string;
  isChangeFromPreviousPeriodShown: boolean;
  nearestNeighbour: any;
  profileCollectionUrlKey: string;
  profileName: string;
  showDataQuality: boolean;
  startZeroYAxis: boolean;
  staticReportsFolders: string[];
  staticReportsLabel: string;
  spineChartMinMaxLabelId: number;
  spineHeaders: SpineHeaders;
}

export interface FTData {
  highlightedRowCells: boolean;
  sortedAreas: Area[];
}

export interface FTIndicatorSearch {
  getIndicatorListId(): any;
  isIndicatorList(): boolean;
  isInSearchMode(): boolean;
  getIndicatorIdList(): IndicatorIdList;
  getIndicatorIdsParameter(): string;
  getProfileIdsForSearch(): Array<number>;
}

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
  isNearestNeighbours(): boolean;
  groupRoots: GroupRoot[];
  yAxisSelectedIndicatorId: number;
  yAxisSelectedSexId: number;
  yAxisSelectedAgeId: number;
}

export interface FTRoot {
  config(): FTConfig;
  exportTableAsImage(containerId: string, fileNamePrefix: string, legends: string);
  formatCount(dataInfo: CoreDataSetInfo): string;
  formatTrendDataCount(dataInfo: TrendDataInfo): string;
  getAreaName(areaCode: string): string;
  getAreaList(): Array<Area>;
  getAreaTypeId(): number;
  getAreaTypeName(): string;
  getCurrentDomainName(): string;
  getComparatorId(): number;
  getParentAreaName(): string;
  getParentTypeId(): number;
  getParentTypeName(): string;
  getValueNotes(): Array<ValueNote>;
  getValueNoteById(id: number): ValueNote;
  getAllGroupRoots(): GroupRoot[];
  getArea(areaCode: string): Area;
  getAreaHash(): Map<number, Area>;
  getAreaNameToDisplay(area: Area): string;
  getComparatorById(comparatorId: number): Area;
  getRegionalComparatorGrouping(root: GroupRoot): Grouping;
  getGroupingSubheadings(): GroupingSubheading[];
  getNationalComparatorGrouping(root: GroupRoot): Grouping;
  getComparatorId(): number;
  getCurrentComparator(): Area;
  getGroopRoot(): GroupRoot;
  getMarkerImageFromSignificance(significance: number, useRag: boolean, suffix: string, useQuintileColouring: boolean,
    indicatorId: number, sexId: number, ageId: number): string;
  getMarkerImageFromSignificance(significance: number, useRag: boolean, suffix: string, useQuintileColouring: boolean): string;
  getMiniMarkerImageFromSignificance(significance: number, useRag: boolean, useQuintileColouring: boolean,
    indicatorId: number, sexId: number, ageId: number): string;
  getNationalComparator(): Area;
  getSexAndAgeLabel(groupRoot: GroupRoot): string;
  getTrendMarkerImage(trendMarker: TrendMarker, polarity: number): string;
  getIndicatorDataQualityHtml(text: string): string;
  getIndicatorDataQualityTooltipText(dataQualityCount: number): string;
  getParentArea(): Area;
  getProfileUrlKey(): string;
  goToMetadataPage(rootIndex: number | string): void;
  goToAreaTrendsPage(rootIndex: number | string): void;
  goToAreaProfilePage(): void;
  goToBarChartPage(rootIndex: number | string, triggeredExternally: boolean): void;
  hasDataChanged(groupRoot: GroupRoot): boolean;
  initBootstrapTooltips(): void;
  isFeatureEnabled(feature): boolean;
  isLocked(): boolean;
  isNearestNeighbours(model: FTModel): boolean;
  isParentCountry(model: FTModel): boolean;
  isSubnationalColumn(): boolean;
  lightboxShow(html, top, left, popupWidth);
  lock(): void;
  logEvent(category: string, action: string, label: string): void;
  model(): FTModel;
  data(): FTData;
  newCoreDataSetInfo(data: CoreDataSet): CoreDataSetInfo;
  newTrendDataInfo(trendDataPoint: TrendDataPoint): TrendDataInfo;
  newValueDisplayer(unit: Unit): ValueDisplayer;
  newTrendValueDisplayer(unit: Unit): TrendValueDisplayer;
  newIndicatorFormatter(groupRoot: GroupRoot, metadata: IndicatorMetadata,
    coreDataSet: CoreDataSet, indicatorStatsF: IndicatorStatsPercentilesFormatted): IndicatorFormatter;
  newValueNoteTooltipProvider(): ValueNoteTooltipProvider;
  newValueSuffix(unit: Unit): ValueSuffix;
  newTooltipManager(): TooltipManager;
  newRecentTrendsTooltip(): RecentTrendsTooltip;
  newComparisonConfig(
    groupRoot: GroupRoot,
    indicatorMetadata: IndicatorMetadata
  ): ComparisonConfig;
  newComparisonConfigForTrendRoot(
    trendRoot: TrendRoot,
    indicatorMetadata: IndicatorMetadata
  ): ComparisonConfig;
  recentTrendSelected(): RecentTrendSelected;
  setAreaCode(areaCode: string): void;
  showDataQualityLegend(): void;
  showIndicatorMetadataInLightbox(elementRef: ElementRef): void;
  showAndHidePageElements(): void;
  showTargetBenchmarkOption(roots): void;
  getTargetLegendHtml(comparisonConfig, metadata): string;
  unlock(): void;
  url(): FTUrls;
  version(): string;
  saveElementAsImage(element, outputFilename): void;
  redirectToPopulationPage(): void;
  setComparatorId(id: number): void;
  getAreaMappingsForParentCode(key: string): string[];
  getAreaValues(): string[];
  setAreaValues(key: string, data: CoreDataSet[]): void;
  setTrendMarkers(trendMarkerKey: string, trendMarkerAreas: TrendMarkerArea[]): void;
  getAreaNamesAndCodes(areaCode: string): string;
  getIndicatorIdsParameter(): string;
  getIndicatorKey(groupRoot: GroupRoot, model: FTModel, comparatorCode: string): string;
  getValueNoteSymbol(): string;
  showTrendInfo(): void;
  getYAxisSelectedKey(): string;
  setYAxisSelectedKey(key: string): void;
  hideBenchmarkBox(): void;
  getAreasCodeDisplayed(): string[];
  getLegendDisplayStatus(): boolean;
  setLegendDisplayStatus(state): void;
  setAreaMappings(mappings): void;
  setAreaMenuCode(areaCode): void;
  bridgeDataHelper: BridgeDataHelper;
  coreDataHelper: CoreDataHelper;
  indicatorHelper: IndicatorHelper;
  search: FTIndicatorSearch;
  valueWithUnit: ValueWithUnit;
}

export interface FTUrls {
  img: string;
  bridge: string;
  corews: string;
  search: string;
  pdf: string;
  practiceProfilePdf: string;
}

export interface Grouping {
  SigLevel: number;
  ComparatorData: CoreDataSet;
  ComparatorId: number;
  ComparatorMethodId: number;
  GroupId: number;
  Period: string;
  YearRange: number;
  BaselineYear: number;
  BaselineQuarter: number;
  BaselineMonth: number;
  DataPointYear: number;
  DataPointQuarter: number;
  DataPointMonth: number;
}

export interface GroupingMetadata {
  Id: number;
  Name: string;
  ProfileId: number;
  Sequence: number;
}

export interface GroupingSubheading {
  SubheadingId: number;
  GroupId: number;
  AreaTypeId: number;
  Subheading: string;
  Sequence: number;
}

export interface GroupRoot extends RootBase {
  IID: number;
  Data: CoreDataSet[];
  Grouping: Grouping[];
  DateChanges: DateChanges;
  Sequence: number;
}

export interface GroupRootSummary {
  IID: number;
  IndicatorName: string;
  Sex: Sex;
  Age: Age;
  GroupId: number;
  StateSex: boolean;
  StateAge: boolean;
  Unit: Unit;
}

export interface IndicatorIdList {
  getAllIds(): Array<number>;
}

export interface IndicatorFormatter {
  averageData: CoreDataSet;
  getAreaCount(): string;
  getAreaValue(): string;
  getIndicatorName(): string;
  getIndicatorNamePlusSexAndAge(): string;
  getDataQuality(): string;
  getMin(): string;
  getMax(): string;
  getSuffixIfNoShort(): string;
  get25(): string;
  get75(): string;
  getAverage(): string;
}

export interface IndicatorHelper {
  getMetadataHash(): Map<number, IndicatorMetadata>;
  getIndicatorIndex(): number;
  setIndicatorIndex(indicatorIndex);
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

export interface IndicatorMetadataHash {
  data: Map<number, IndicatorMetadata>;
}

export interface IndicatorMetadataText {
  Name: string;
}

export interface IndicatorMetadataTextProperty {
  ColumnName: string;
  DisplayName: string;
  Order: number;
}

export interface IndicatorProfile {
  Profile: KeyValuePair<number, ProfilePerIndicator[]>;
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

export interface InequalitiesTrendsTableTooltip {
  getTooltipMessage(inequalitiesTrendData: ValueData): string;
}

export interface KeyValuePair<TKey, TValue> {
  Key: TKey;
  Value: TValue;
}

export interface LatitudeLongitude {
  Lat: number;
  Lng: number;
}

export interface Limits {
  Max: number;
  Min: number;
}

export interface NamedIdentity {
  Id: number;
  Name: string;
  Sequence: number;
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

export interface ParentAreaType {
  Id: number;
  Name: string;
  Short: string;
  IsSearchable: boolean;
  CanBeDisplayedOnMap: boolean;
  ParentAreaTypes: ParentAreaType[];
}

export interface PartitionData {
  AreaCode: string;
  IndicatorId: number;
  Data: CoreDataSet[];
}

export interface PartitionDataForAllAges extends PartitionData {
  SexId: number;
  Ages: Age[];
  ChartAverageLineAgeId: number;
}

export interface PartitionDataForAllCategories extends PartitionData {
  SexId: number;
  AgeId: number;
  CategoryTypes: CategoryType[];
  BenchmarkDataSpecialCases: CoreDataSet[];
}

export interface PartitionDataForAllSexes extends PartitionData {
  AgeId: number;
  Sexes: Sex[];
}

export class PartitionTrendData {
  Labels: NamedIdentity[];
  TrendData: Map<number, CoreDataSet[]>;
  Periods: string[];
  Limits: Limits;
  AreaAverage: CoreDataSet[];
}

export interface Population {
  Code: string;
  IndicatorName: string;
  Labels: string[];
  Period: string;
  /** Key: Sex Id, Value: Population percentages  */
  Values: Map<number, number[]>;
  ListSize: ValueData;
  PopulationTotals: Map<number, number[]>;
  ChildAreaCount: number;
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

export interface ProfilePerIndicator {
  ProfileName: string;
  Url: string;
}

export interface RecentTrendSelected {
  byGroupRoot(rootIndex: number | string): void;
  byAreaAndRootIndex(areaCode: string, rootIndex: number | string): void;
}

export interface RecentTrendsTooltip {
  getTooltipByData(recentTrends: TrendMarkerResult): string;
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

/** Should match values in PholioVisualisation.PholioObjects.Significance */
export const enum Significance {
  None,
  Worse,
  Same,
  Better,
  Worst,
  Best
}

export interface SpineHeaders {
  min: string;
  max: string;
}

export interface SSRSReport {
  Id: number;
  Name: string;
  File: string;
  Parameters: string;
  Notes: string;
  IsLive: boolean;
  AreaTypeIds: string;
}

export interface TargetConfig {
  BespokeKey: string;
  LegendHtml: string;
  LowerLimit: number;
  PolarityId: number;
  UpperLimit: number;
}

export interface TooltipManager {
  setHtml(text: string): void;
  positionXY(x: number, y: number): void;
  showOnly(): void;
  hide(): void;
}

export interface TrendDataInfo {
  Count: number;
  Value: number;
  getValF(): string;
  isValue(): boolean;
  isCount(): number;
  isNote(): boolean;
  getNoteId(): number;
}

export interface TrendDataPoint {
  C: number;
  IsC: boolean;
  L: number;
  LF: string;
  L99_8: number;
  LF99_8: string;
  Sig: KeyValuePair<number, Significance>;
  U: number;
  UF: string;
  U99_8: number;
  UF99_8: string;
  D: number;
  V: string;
  Denom: number;
  NoteId: number;
  CategoryId: number;
  CategoryTypeId: number;
}

export const enum TrendMarker {
  /** Not enough data points */
  CannotBeCalculated = 0,
  Increasing = 1,
  Decreasing = 2,
  NoChange = 3
}

export interface TrendMarkerLabel {
  id: TrendMarker;
  text: string;
}

export interface TrendMarkerResult {
  Marker: TrendMarker;
  Message: string;
  PointsUsed: number;
  MarkerForMostRecentValueComparedWithPreviousValue: TrendMarker;
}

export interface TrendRoot extends RootBase {
  ComparatorData: KeyValuePair<number, CoreDataSet>[];
  ComparatorValue: KeyValuePair<number, number>[];
  ComparatorValueFs: KeyValuePair<number, string>[];
  Data: KeyValuePair<string, TrendDataPoint[]>[];
  Limits: Limits;
  Periods: string[];
}

export interface TrendValueDisplayer {
  byDataInfo(dataInfo: TrendDataInfo, options?: any): string;
  byNumberString(num: any): string;
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
  Note: ValueNote;
}

export interface ValueDisplayer {
  byDataInfo(dataInfo: CoreDataSetInfo, options?: any): string;
  byNumberString(num: any): string;
}

export interface ValueNote {
  Id: number;
  Text: string;
}

export interface ValueNoteTooltipProvider {
  getHtmlFromNoteId(id: number | string): string;
  getTextFromNoteId(id: number | string): string;
  getHtml(id: string): string;
  getValueNoteCellText(bits: Array<String>): string;
}

export interface ValueType {
  Id: number;
  Name: string;
}

export interface ValueWithCIsData extends ValueData {
  LoCI: number;
  LoCIF: string;
  LoCI99_8: number;
  LoCI99_8F: string;
  UpCI: number;
  UpCIF: string;
  UpCI99_8: number;
  UpCI99_8F: string;
}

export interface ValueWithUnit {
  getFullLabel(value: string, options?: any): string;
}

export interface ValueSuffix {
  getShortLabel(): string;
  getFullLabelIfNoShort(): string;
  getFullLabel(): string;
}

export interface YearType {
  Id: number;
  Name: string;
}
