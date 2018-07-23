import { Injectable, ElementRef } from '@angular/core';
import {
    FTRoot, Area, GroupRoot, FTConfig, FTModel, IndicatorMetadata, Url, ValueNote,
    FTDisplay, CoreDataSet, FTIndicatorSearch, Grouping, CoreDataSetInfo, Unit,
    ValueNoteTooltipProvider, TooltipManager, TrendMarker, CommaNumber, ValueDisplayer,
    RecentTrendsTooltip, RecentTrendSelected, ComparisonConfig, ParentAreaType
} from '../../../typings/FT.d';
declare var FTWrapper: FTRoot;

@Injectable()
export class FTHelperService {
    getAreaName(areaCode: string): string {
        return FTWrapper.display.getAreaName(areaCode);
    }
    getAreaTypeName(): string {
        return FTWrapper.display.getAreaTypeName();
    }
    getAreaList(): Array<Area> {
        return FTWrapper.display.getAreaList();
    }
    getComparatorId(): number {
        return FTWrapper.display.getComparatorId();
    }
    getCurrentComparator(): Area {
        return FTWrapper.bridgeDataHelper.getCurrentComparator();
    }
    getCurrentGroupRoot(): GroupRoot {
        return FTWrapper.bridgeDataHelper.getGroopRoot();
    }
    getAllGroupRoots(): GroupRoot[] {
        return FTWrapper.bridgeDataHelper.getAllGroupRoots();
    }
    getValueNotes(): Array<ValueNote> {
        return FTWrapper.display.getValueNotes();
    }
    getValueNoteById(id: number): ValueNote {
        return FTWrapper.display.getValueNoteById(id);
    }
    formatCount(dataInfo: CoreDataSetInfo): string {
        return FTWrapper.formatCount(dataInfo);
    }
    newCoreDataSetInfo(data: CoreDataSet): CoreDataSetInfo {
        return FTWrapper.newCoreDataSetInfo(data);
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
    getURL(): Url {
        return FTWrapper.url();
    }
    getIndicatorDataQualityHtml(text: string): string {
        return FTWrapper.getIndicatorDataQualityHtml(text);
    }
    getIndicatorDataQualityTooltipText(dataQualityCount: number): string {
        return FTWrapper.getIndicatorDataQualityTooltipText(dataQualityCount);
    }
    goToMetadataPage(rootIndex: number | string): void {
        FTWrapper.goToMetadataPage(rootIndex);
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

    // getParentAreaType(): ParentAreaType[] {
    //   return FTWrapper.getParentArea
    // }
}

