import { Injectable, ElementRef } from '@angular/core';
import { AreaTypeIds } from '../../constants';
import { TrendMarkerArea } from '../../../compare-area/compare-area';
import {
    FTRoot, GroupRoot, Area, GroupingSubheading, ValueNote, CoreDataSetInfo,
    TrendDataInfo, CoreDataSet, TrendDataPoint, IndicatorMetadata, IndicatorStatsPercentilesFormatted,
    IndicatorFormatter, Unit, ValueDisplayer, TrendValueDisplayer, ValueNoteTooltipProvider,
    TooltipManager, RecentTrendsTooltip, ComparisonConfig, TrendRoot, Grouping, FTConfig, FTModel, FTData,
    FTIndicatorSearch, TrendMarker, FTUrls, RecentTrendSelected, ValueSuffix
} from '../../../typings/FT';
import { isDefined } from '@angular/compiler/src/util';
declare let FTWrapper: FTRoot;

// Global JS variables defined in HTML page
declare let enumParentDisplay: number;
declare let groupRoots: GroupRoot[];

@Injectable()
export class FTHelperService {
    exportTableAsImage(containerId: string, fileNamePrefix: string, legends: string): void {
        FTWrapper.exportTableAsImage(containerId, fileNamePrefix, legends);
    }

    formatCount(dataInfo: CoreDataSetInfo): string {
        return FTWrapper.formatCount(dataInfo);
    }

    formatTrendDataCount(dataInfo: TrendDataInfo): string {
        return FTWrapper.formatTrendDataCount(dataInfo);
    }

    getAgeId(): number {
        return FTWrapper.getGroopRoot().Age.Id;
    }

    getAgeIdbyGroupRoot(groupRoot: GroupRoot): number {
        return groupRoot.Age.Id;
    }

    getAllGroupRoots(): GroupRoot[] {
        return groupRoots;
    }

    getArea(areaCode: string): Area {
        return FTWrapper.getArea(areaCode);
    }

    getAreasCodeDisplayed(): string[] {
        return FTWrapper.getAreasCodeDisplayed();
    }

    getAreaHash(): Map<number, Area> {
        return FTWrapper.getAreaHash();
    }

    getAreaMappingsForParentCode(key: string): string[] {
        return FTWrapper.getAreaMappingsForParentCode(key);
    }

    getAreaName(areaCode: string): string {
        return FTWrapper.getAreaName(areaCode);
    }

    getAreaNamesAndCodes(areaCode: string): string {
        return FTWrapper.getAreaNamesAndCodes(areaCode);
    }

    getAreaNameToDisplay(area: Area): string {
        return FTWrapper.getAreaNameToDisplay(area);
    }

    getAreaList(): Array<Area> {
        return FTWrapper.getAreaList();
    }

    getAreaTypeId(): number {
        return FTWrapper.getAreaTypeId();
    }

    getAreaTypeName(): string {
        return FTWrapper.getAreaTypeName();
    }

    getAreaValues(): string[] {
        return FTWrapper.getAreaValues();
    }

    getComparatorId(): number {
        return FTWrapper.getComparatorId();
    }

    getComparatorById(comparatorId: number): Area {
        return FTWrapper.getComparatorById(comparatorId);
    }

    getCurrentComparator(): Area {
        return FTWrapper.getCurrentComparator();
    }

    getCurrentDomainName(): string {
        return FTWrapper.getCurrentDomainName();
    }

    getCurrentGroupRoot(): GroupRoot {
        return FTWrapper.getGroopRoot();
    }

    getEnumParentDisplay(): number {
        return enumParentDisplay;
    }

    getFTConfig(): FTConfig {
        return FTWrapper.config();
    }

    getFTData(): FTData {
        return FTWrapper.data();
    }

    getFTModel(): FTModel {
        return FTWrapper.model();
    }

    getGroupingSubheadings(): GroupingSubheading[] {
        return FTWrapper.getGroupingSubheadings();
    }

    getIid(): number {
        const model = this.getFTModel();
        if (model.iid) {
            return model.iid;
        }

        return this.getCurrentGroupRoot().IID;
    }

    getIndicatorDataQualityHtml(text: string): string {
        return FTWrapper.getIndicatorDataQualityHtml(text);
    }

    getIndicatorDataQualityTooltipText(dataQualityCount: number): string {
        return FTWrapper.getIndicatorDataQualityTooltipText(dataQualityCount);
    }

    getIndicatorIdsParameter(): string {
        return FTWrapper.getIndicatorIdsParameter();
    }

    getIndicatorIndex(): number {
        return FTWrapper.indicatorHelper.getIndicatorIndex();
    }

    getIndicatorKey(groupRoot: GroupRoot, model: FTModel, comparatorCode: string): string {
        return FTWrapper.getIndicatorKey(groupRoot, model, comparatorCode);
    }

    getIndicatorName(indicatorId: number): string {
        return this.getMetadata(indicatorId).Descriptive['Name'];
    }

    getLegendDisplayStatus(): boolean {
        return FTWrapper.getLegendDisplayStatus();
    }

    getMarkerImageFromSignificance(significance: number, useRag: boolean, suffix: string, useQuintileColouring: boolean,
        indicatorId: number, sexId: number, ageId: number): string {
        return FTWrapper.getMarkerImageFromSignificance(significance, useRag, suffix, useQuintileColouring, indicatorId, sexId, ageId);
    }

    getMetadata(IID: number): IndicatorMetadata {
        return FTWrapper.indicatorHelper.getMetadataHash()[IID];
    }

    getMetadataHash(): Map<number, IndicatorMetadata> {
        return FTWrapper.indicatorHelper.getMetadataHash();
    }

    getMiniMarkerImageFromSignificance(significance: number, useRag: boolean, useQuintileColouring: boolean,
        indicatorId: number, sexId: number, ageId: number): string {
        return FTWrapper.getMiniMarkerImageFromSignificance(significance, useRag, useQuintileColouring, indicatorId, sexId, ageId);
    }

    getNationalComparator(): Area {
        return FTWrapper.getNationalComparator();
    }

    getNationalComparatorGrouping(root: GroupRoot): Grouping {
        return FTWrapper.getNationalComparatorGrouping(root);
    }

    getParentArea(): Area {
        return FTWrapper.getParentArea();
    }

    getParentTypeId(): number {
        return FTWrapper.getParentTypeId();
    }

    getParentTypeName(): string {
        return FTWrapper.getParentTypeName();
    }

    getProfileUrlKey(): string {
        return FTWrapper.getProfileUrlKey();
    }

    getRegionalComparatorGrouping(root: GroupRoot): Grouping {
        return FTWrapper.getRegionalComparatorGrouping(root);
    }

    getSearch(): FTIndicatorSearch {
        return FTWrapper.search;
    }

    getSexAndAgeLabel(groupRoot: GroupRoot): string {
        return FTWrapper.getSexAndAgeLabel(groupRoot);
    }

    getSexId(): number {
        return FTWrapper.getGroopRoot().Sex.Id;
    }

    getSexIdByGroupRoot(groupRoot: GroupRoot): number {
        return groupRoot.Sex.Id;
    }

    getTargetLegendHtml(comparisonConfig, metadata): string {
        return FTWrapper.getTargetLegendHtml(comparisonConfig, metadata);
    }

    goToAreaProfilePage(): void {
        FTWrapper.goToAreaProfilePage();
    }

    goToAreaTrendsPage(rootIndex?: number | string): void {
        FTWrapper.goToAreaTrendsPage(rootIndex);
    }

    goToBarChartPage(rootIndex: number | string, triggeredExternally: boolean): void {
        FTWrapper.goToBarChartPage(rootIndex, triggeredExternally);
    }

    goToMetadataPage(rootIndex: number | string): void {
        FTWrapper.goToMetadataPage(rootIndex);
    }

    /** Returns IMG HTML for a recent trend */
    getTrendMarkerImage(trendMarker: TrendMarker, polarity: number): string {
        return FTWrapper.getTrendMarkerImage(trendMarker, polarity);
    }

    getURL(): FTUrls {
        return FTWrapper.url();
    }

    getValueNoteById(id: number): ValueNote {
        return FTWrapper.getValueNoteById(id);
    }

    getValueNotes(): Array<ValueNote> {
        return FTWrapper.getValueNotes();
    }

    getValueNoteSymbol(): string {
        return FTWrapper.getValueNoteSymbol();
    }

    getYAxisSelectedKey(): string {
        return FTWrapper.getYAxisSelectedKey();
    }

    hasDataChanged(groupRoot: GroupRoot): boolean {
        return FTWrapper.hasDataChanged(groupRoot);
    }

    hideBenchmarkBox(): void {
        FTWrapper.hideBenchmarkBox();
    }

    initBootstrapTooltips(): void {
        FTWrapper.initBootstrapTooltips();
    }

    isFeatureEnabled(feature): boolean {
        return FTWrapper.isFeatureEnabled(feature);
    }

    isLocked(): boolean {
        return FTWrapper.isLocked();
    }

    isNearestNeighbours(): boolean {
        const model = this.getFTModel();
        switch (model.nearestNeighbour) {
            case undefined:
            case null:
            case '':
                return false;
            default:
                return true;
        }
    }

    isParentCountry(): boolean {
        return this.getFTModel().parentTypeId === AreaTypeIds.Country;
    }

    isParentUk(): boolean {
        return FTWrapper.getParentTypeId() === AreaTypeIds.Uk;
    }

    isSubnationalColumn(): boolean {
        return FTWrapper.isSubnationalColumn();
    }

    isValuePresent(val: string): boolean {
        return isDefined(val) && val !== '-' && val !== '';
    }

    lightboxShow(html, top, left, popupWidth) {
        FTWrapper.lightboxShow(html, top, left, popupWidth);
    }

    lock(): void {
        FTWrapper.lock();
    }

    logEvent(category: string, action: string, label: string = null): void {
        FTWrapper.logEvent(category, action, label);
    }

    newComparisonConfig(groupRoot: GroupRoot, indicatorMetadata: IndicatorMetadata): ComparisonConfig {
        return FTWrapper.newComparisonConfig(groupRoot, indicatorMetadata);
    }

    newComparisonConfigForTrendRoot(trendRoot: TrendRoot, indicatorMetadata: IndicatorMetadata): ComparisonConfig {
        return FTWrapper.newComparisonConfigForTrendRoot(trendRoot, indicatorMetadata);
    }

    newCoreDataSetInfo(data: CoreDataSet): CoreDataSetInfo {
        return FTWrapper.newCoreDataSetInfo(data);
    }

    newIndicatorFormatter(groupRoot: GroupRoot, metadata: IndicatorMetadata,
        coreDataSet: CoreDataSet, indicatorStatsF: IndicatorStatsPercentilesFormatted): IndicatorFormatter {
        return FTWrapper.newIndicatorFormatter(groupRoot, metadata, coreDataSet, indicatorStatsF);
    }

    newRecentTrendsTooltip(): RecentTrendsTooltip {
        return FTWrapper.newRecentTrendsTooltip();
    }

    newTooltipManager(): TooltipManager {
        return FTWrapper.newTooltipManager();
    }

    newTrendDataInfo(trendDataPoint: TrendDataPoint): TrendDataInfo {
        return FTWrapper.newTrendDataInfo(trendDataPoint);
    }

    newTrendValueDisplayer(unit: Unit): TrendValueDisplayer {
        return FTWrapper.newTrendValueDisplayer(unit);
    }

    newValueDisplayer(unit: Unit): ValueDisplayer {
        return FTWrapper.newValueDisplayer(unit);
    }

    newValueNoteTooltipProvider(): ValueNoteTooltipProvider {
        return FTWrapper.newValueNoteTooltipProvider();
    }

    newValueSuffix(unit: Unit): ValueSuffix {
        return FTWrapper.newValueSuffix(unit);
    }

    recentTrendSelected(): RecentTrendSelected {
        return FTWrapper.recentTrendSelected();
    }

    redirectToPopulationPage(): void {
        return FTWrapper.redirectToPopulationPage();
    }

    saveElementAsImage(element, outputFilename): void {
        return FTWrapper.saveElementAsImage(element, outputFilename);
    }

    setAreaCode(areaCode: string): void {
        FTWrapper.setAreaCode(areaCode);
    }

    setAreaValues(key: string, data: CoreDataSet[]): void {
        FTWrapper.setAreaValues(key, data);
    }

    setComparatorId(id: number): void {
        FTWrapper.setComparatorId(id);
    }

    setIndicatorIndex(indicatorIndex: number) {
        FTWrapper.indicatorHelper.setIndicatorIndex(indicatorIndex);
    }

    setLegendDisplayStatus(state: boolean) {
        FTWrapper.setLegendDisplayStatus(state);
    }

    setTrendMarkers(trendMarkerKey: string, trendMarkerAreas: TrendMarkerArea[]): void {
        FTWrapper.setTrendMarkers(trendMarkerKey, trendMarkerAreas);
    }

    setYAxisSelectedKey(key: string) {
        FTWrapper.setYAxisSelectedKey(key);
    }

    showAndHidePageElements(): void {
        FTWrapper.showAndHidePageElements();
    }

    showDataQualityLegend(): void {
        FTWrapper.showDataQualityLegend();
    }

    showIndicatorMetadataInLightbox(element: ElementRef): void {
        FTWrapper.showIndicatorMetadataInLightbox(element);
    }

    showTargetBenchmarkOption(roots): void {
        FTWrapper.showTargetBenchmarkOption(roots);
    }

    showTrendInfo(): void {
        return FTWrapper.showTrendInfo();
    }

    setAreaMappings(mappings): void {
        FTWrapper.setAreaMappings(mappings);
    }

    setAreaMenuCode(areaCode: string): void {
        FTWrapper.setAreaMenuCode(areaCode);
    }

    unlock(): void {
        FTWrapper.unlock();
    }

    /** The version number of the static assets, e.g. JS */
    version(): string {
        return FTWrapper.version();
    }
}
