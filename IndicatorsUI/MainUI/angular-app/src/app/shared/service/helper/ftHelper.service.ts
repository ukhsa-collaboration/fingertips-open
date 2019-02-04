import { Injectable, ElementRef } from '@angular/core';
import {
    FTRoot, Area, GroupRoot, FTConfig, FTModel, IndicatorMetadata, FTUrls, ValueNote,
    CoreDataSet, FTIndicatorSearch, Grouping, CoreDataSetInfo, Unit,
    ValueNoteTooltipProvider, TooltipManager, TrendMarker, CommaNumber, ValueDisplayer, IndicatorStatsPercentilesFormatted, IndicatorFormatter,
    RecentTrendsTooltip, RecentTrendSelected, ComparisonConfig, ParentAreaType,
    GroupingSubheading
} from '../../../typings/FT.d';
import { AreaTypeIds } from '../../shared';
declare let FTWrapper: FTRoot;

// Global JS variables defined in HTML page
declare let enumParentDisplay: number;
declare let groupRoots: GroupRoot[];

@Injectable()
export class FTHelperService {
    hasDataChanged(groupRoot: GroupRoot): boolean {
        return FTWrapper.hasDataChanged(groupRoot);
    }
    getIndicatorNameTooltip(rootIndex: number, area: Area) {
        return FTWrapper.getIndicatorNameTooltip(rootIndex, area);
    }
    initBootstrapTooltips(): void {
        FTWrapper.initBootstrapTooltips();
    }
    isSubnationalColumn(): boolean {
        return FTWrapper.isSubnationalColumn();
    }
    exportTableAsImage(containerId: string, fileNamePrefix: string, legends: string): void {
        FTWrapper.exportTableAsImage(containerId, fileNamePrefix, legends);
    }
    getMarkerImageFromSignificance(significance: number, useRag: boolean, suffix: string, useQuintileColouring: boolean,
        indicatorId: number, sexId: number, ageId: number): string {
        return FTWrapper.getMarkerImageFromSignificance(significance, useRag, suffix, useQuintileColouring, indicatorId, sexId, ageId);
    }
    getArea(areaCode: string): Area {
        return FTWrapper.getArea(areaCode);
    }
    getAreaName(areaCode: string): string {
        return FTWrapper.getAreaName(areaCode);
    }
    getAreaNameToDisplay(area: Area): string {
        return FTWrapper.getAreaNameToDisplay(area);
    }
    getEnumParentDisplay(): number {
        return enumParentDisplay;
    }
    getNationalComparator(): Area {
        return FTWrapper.getNationalComparator();
    }
    getParentTypeId(): number {
        return FTWrapper.getParentTypeId();
    }
    getParentTypeName(): string {
        return FTWrapper.getParentTypeName();
    }
    getAreaTypeId(): number {
        return FTWrapper.getAreaTypeId();
    }
    getAreaTypeName(): string {
        return FTWrapper.getAreaTypeName();
    }
    getAreaList(): Array<Area> {
        return FTWrapper.getAreaList();
    }
    getComparatorId(): number {
        return FTWrapper.getComparatorId();
    }
    getComparatorById(comparatorId: number): Area {
        return FTWrapper.getComparatorById(comparatorId);
    }
    getCurrentDomainName(): string {
        return FTWrapper.getCurrentDomainName();
    }
    getCurrentComparator(): Area {
        return FTWrapper.getCurrentComparator();
    }
    getCurrentGroupRoot(): GroupRoot {
        return FTWrapper.getGroopRoot();
    }
    getGroupingSubheadings(): GroupingSubheading[] {
        return FTWrapper.getGroupingSubheadings();
    }
    getAllGroupRoots(): GroupRoot[] {
        return groupRoots;
    }
    getValueNotes(): Array<ValueNote> {
        return FTWrapper.getValueNotes();
    }
    getValueNoteById(id: number): ValueNote {
        return FTWrapper.getValueNoteById(id);
    }
    formatCount(dataInfo: CoreDataSetInfo): string {
        return FTWrapper.formatCount(dataInfo);
    }
    newCoreDataSetInfo(data: CoreDataSet): CoreDataSetInfo {
        return FTWrapper.newCoreDataSetInfo(data);
    }
    newIndicatorFormatter(groupRoot: GroupRoot, metadata: IndicatorMetadata,
        coreDataSet: CoreDataSet, indicatorStatsF: IndicatorStatsPercentilesFormatted): IndicatorFormatter {
        return FTWrapper.newIndicatorFormatter(groupRoot, metadata, coreDataSet, indicatorStatsF);
    }
    newValueDisplayer(unit: Unit): ValueDisplayer {
        return FTWrapper.newValueDisplayer(unit);
    }
    newCommaNumber(n: number): CommaNumber {
        return FTWrapper.newCommaNumber(n);
    }
    newValueNoteTooltipProvider(): ValueNoteTooltipProvider {
        return FTWrapper.newValueNoteTooltipProvider();
    }
    newTooltipManager(): TooltipManager {
        return FTWrapper.newTooltipManager();
    }
    newRecentTrendsTooltip(): RecentTrendsTooltip {
        return FTWrapper.newRecentTrendsTooltip();
    }
    newComparisonConfig(groupRoot: GroupRoot, indicatorMetadata: IndicatorMetadata): ComparisonConfig {
        return FTWrapper.newComparisonConfig(groupRoot, indicatorMetadata);
    }
    getRegionalComparatorGrouping(root: GroupRoot): Grouping {
        return FTWrapper.getRegionalComparatorGrouping(root);
    }
    getNationalComparatorGrouping(root: GroupRoot): Grouping {
        return FTWrapper.getNationalComparatorGrouping(root);
    }
    getFTConfig(): FTConfig {
        return FTWrapper.config();
    }
    getFTModel(): FTModel {
        return FTWrapper.model();
    }
    getMetadata(IID: number): IndicatorMetadata {
        return FTWrapper.indicatorHelper.getMetadataHash()[IID];
    }
    getMetadataHash(): Map<number, IndicatorMetadata> {
        return FTWrapper.indicatorHelper.getMetadataHash();
    }
    getIndicatorIndex(): number {
        return FTWrapper.indicatorHelper.getIndicatorIndex();
    }
    getParentArea(): Area {
        return FTWrapper.getParentArea();
    }
    getSearch(): FTIndicatorSearch {
        return FTWrapper.search;
    }
    getSexAndAgeLabel(groupRoot: GroupRoot): string {
        return FTWrapper.getSexAndAgeLabel(groupRoot);
    }
    /** Returns IMG HTML for a recent trend */
    getTrendMarkerImage(trendMarker: TrendMarker, polarity: number): string {
        return FTWrapper.getTrendMarkerImage(trendMarker, polarity);
    }
    getURL(): FTUrls {
        return FTWrapper.url();
    }
    getIndicatorDataQualityHtml(text: string): string {
        return FTWrapper.getIndicatorDataQualityHtml(text);
    }
    getIndicatorDataQualityTooltipText(dataQualityCount: number): string {
        return FTWrapper.getIndicatorDataQualityTooltipText(dataQualityCount);
    }
    goToBarChartPage(rootIndex: number | string): void {
        FTWrapper.goToBarChartPage(rootIndex);
    }
    goToMetadataPage(rootIndex: number | string): void {
        FTWrapper.goToMetadataPage(rootIndex);
    }
    goToAreaTrendsPage(rootIndex: number | string): void {
        FTWrapper.goToAreaTrendsPage(rootIndex);
    }
    recentTrendSelected(): RecentTrendSelected {
        return FTWrapper.recentTrendSelected();
    }
    setAreaCode(areaCode: string): void {
        FTWrapper.setAreaCode(areaCode);
    }
    showIndicatorMetadataInLightbox(element: ElementRef): void {
        FTWrapper.showIndicatorMetadataInLightbox(element);
    }
    showAndHidePageElements(): void {
        FTWrapper.showAndHidePageElements();
    }
    showDataQualityLegend(): void {
        FTWrapper.showDataQualityLegend();
    }
    showTargetBenchmarkOption(roots): void {
        FTWrapper.showTargetBenchmarkOption(roots);
    }
    getTargetLegendHtml(comparisonConfig, metadata): string {
        return FTWrapper.getTargetLegendHtml(comparisonConfig, metadata);
    }
    lock(): void {
        FTWrapper.lock();
    }
    unlock(): void {
        FTWrapper.unlock();
    }

    /** The version number of the static assets, e.g. JS */
    version(): string {
        return FTWrapper.version();
    }

    saveElementAsImage(element, outputFilename): void {
        return FTWrapper.saveElementAsImage(element, outputFilename);
    }

    redirectToPopulationPage(): void {
        return FTWrapper.redirectToPopulationPage();
    }

    isValuePresent(val: string): boolean {
        return val !== undefined && val !== '-' && val !== '';
    }

    logEvent(category: string, action: string, label: string = null): void {
        FTWrapper.logEvent(category, action, label);
    }

    isParentCountry(model: FTModel): boolean {
        return model.parentTypeId === AreaTypeIds.Country;
    }

    isParentUk(): boolean {
        return FTWrapper.getParentTypeId() === AreaTypeIds.Uk;
    }

    isNearestNeighbours(model: FTModel): boolean {
        switch (model.nearestNeighbour) {
            case undefined:
            case null:
            case "":
                return false;
            default:
                return true;
        }
    }

    getProfileUrlKey(): string {
        return FTWrapper.getProfileUrlKey();
    }

    setComparatorId(id: number): void {
        FTWrapper.setComparatorId(id);
    }

    getAreaMappingsForParentCode(key: string): string[] {
        return FTWrapper.getAreaMappingsForParentCode(key);
    }

    showTrendInfo(): void {
        return FTWrapper.showTrendInfo();
    }

    lightboxShow(html, top, left, popupWidth) {
        FTWrapper.lightboxShow(html, top, left, popupWidth);
    }
}

