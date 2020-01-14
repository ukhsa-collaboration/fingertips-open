import { Component, HostListener, ChangeDetectorRef } from '@angular/core';
import { isDefined } from '@angular/compiler/src/util';
import { forkJoin } from 'rxjs';
import * as _ from 'underscore';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import {
  Colour, NewDataBadgeHelper, TrendDataSetListHelper, TooltipHelper,
  AreaAndDataSorterHelper, ParentAreaHelper, TrendSourceHelper, CategoryAreaCodeHelper, ParentAreaTypeHelper
} from '../shared/shared';
import {
  ComparatorIds, ValueTypeIds, ParentDisplay, ComparatorMethodIds, AreaTypeIds, PageType, Tabs, CsvCoreDataType
} from '../shared/constants';
import { AreaCodes } from '../shared/constants'
import { DisplayOption, TrendRow, TrendData, Data, ViewModes, SortType } from './trend';
import { DownloadService } from '../shared/service/api/download.service';
import { LegendConfig } from '../shared/component/legend/legend';
import {
  FTModel, FTConfig, FTData, GroupRoot, TrendRoot, Area, ComparisonConfig,
  CoreDataSet, TrendMarkerResult, TrendDataPoint, Limits
} from '../typings/FT';
import { UIService } from '../shared/service/helper/ui.service';
import { CsvData, CsvConfig, CsvDataHelper } from '../shared/component/export-csv/export-csv';
import { TrendMarkerLabelProvider } from '../shared/classes/trendmarker-label-provider';
import { SignificanceFormatter } from '../shared/classes/significance-formatter';
import { TimePeriod } from '../shared/classes/time-period';

@Component({
  selector: 'ft-trend',
  templateUrl: './trend.component.html',
  styleUrls: ['./trend.component.css']
})
export class TrendComponent {

  public isSingleArea = true;
  public selectedAreaTypeId: number;

  private model: FTModel;
  private config: FTConfig;
  private data: FTData;
  private groupRoots: GroupRoot[] = [];
  private trendRoots: TrendRoot[] = [];
  private groupRootsToDisplay: GroupRoot[] = [];
  private finalData: Data[] = [];
  private sortedAreas: Area[] = [];

  private areaName: string;
  private selectedParentName: string;
  private selectedIndicatorName: string;
  private selectedIndicatorPeriod: string;
  private selectedIndicatorUnit: string;
  private selectedIndicatorNewDataBadge: string;
  private selectedIndicatorClass = 'button-selected';
  private sortByAlphabeticalClass = 'button-selected';
  private sortByValueClass = '';
  private sortByRankClass = '';
  private allIndicatorsClass: string;
  private showConfidenceIntervalsLabel = false;
  private confidenceIntervalsLabel = 'Show confidence intervals';
  private displayOption: DisplayOption = DisplayOption.SelectedIndicator;
  private isNearestNeighbours = false;

  private tooltipHelper: TooltipHelper;
  private recentTrendImage: string;

  selectedAreaName: string;
  formattedselectedParentName: string;
  areaButtonClass = 'button-selected';
  regionButtonClass: string;
  showAllAreasInRegion = false;
  showIndicatorButtons = true;

  showRecentTrends = false;
  legendConfig: LegendConfig;
  csvConfig: CsvConfig;

  chartList: any[] = [];

  constructor(private ftHelperService: FTHelperService,
    private indicatorService: IndicatorService,
    private downloadService: DownloadService,
    private uiService: UIService,
    private ref: ChangeDetectorRef) { }

  @HostListener('window:AreaTrendSelected', ['$event'])
  public onOutsideEvent(event) {

    const ftHelper = this.ftHelperService;
    this.model = ftHelper.getFTModel();
    this.config = ftHelper.getFTConfig();
    this.data = ftHelper.getFTData();
    this.groupRoots = this.model.groupRoots;

    const currentGroupRoot = ftHelper.getCurrentGroupRoot();
    const groupId = currentGroupRoot.Grouping[0].GroupId;
    const areaTypeId = this.model.areaTypeId;
    const profileId = this.model.profileId;

    this.selectedAreaName = ftHelper.getAreaName(this.model.areaCode);
    this.selectedAreaTypeId = areaTypeId;

    this.isNearestNeighbours = this.model.isNearestNeighbours();

    const parentCode = this.getParentAreaCode();

    if (this.isNearestNeighbours) {
      this.selectedParentName = 'Neighbrs';
      this.formattedselectedParentName = this.selectedAreaName + ' and neighbours';
    } else {
      this.selectedParentName = new ParentAreaHelper(ftHelper).getParentAreaName();
      this.formattedselectedParentName = 'All in ' + this.selectedParentName;
    }

    const trendDataObservable = this.getDataTrends(groupId, areaTypeId, parentCode, profileId);

    forkJoin([trendDataObservable]).subscribe(results => {

      this.trendRoots = <TrendRoot[]>results[0];

      if (this.trendRoots.length > 0) {
        this.selectedArea();
      } else {
        // No data
      }

      ftHelper.showAndHidePageElements();
      ftHelper.showTargetBenchmarkOption(this.ftHelperService.getAllGroupRoots());

      ftHelper.unlock();
    });
  }

  getDataTrends(groupId: number, areaTypeId: number, parentCode: string, profileId: number) {
    const search = this.ftHelperService.getSearch();

    if (!search.isInSearchMode()) {
      return this.indicatorService.getTrendDataForAllIndicatorsInProfileGroupForChildAreas(groupId, areaTypeId, parentCode, profileId);
    } else {
      const indicatorId = this.ftHelperService.getIid();
      const indicatorIds: number[] = _.pluck(this.groupRoots, 'IID');
      const profileIds: number[] = [];

      return this.indicatorService.getTrendDataForSpecificIndicatorsForChildAreas(areaTypeId, parentCode, indicatorIds, profileIds);
    }
  }

  getParentAreaCode(): string {
    if (this.isNearestNeighbours) {
      return this.model.nearestNeighbour;
    } else if (this.selectedAreaTypeId === AreaTypeIds.Country) {
      return AreaCodes.England;
    }
    return this.model.parentCode;
  }

  selectedArea() {
    if (this.isSingleArea) {
      this.areaButtonClass = 'button-selected';
      this.regionButtonClass = '';
      this.showAllAreasInRegion = false;
      this.displayData();
    } else {
      this.areaButtonClass = '';
      this.regionButtonClass = 'button-selected';
      this.showAllAreasInRegion = true;

      this.sortByAlphabeticalClass = '';
      this.sortByValueClass = '';
      this.sortByRankClass = '';

      if (this.isNearestNeighbours) {
        this.sortByRankClass = 'button-selected';
      } else {
        this.sortByAlphabeticalClass = 'button-selected';
      }

      this.sortedAreas = this.data.sortedAreas;

      this.generateSmallMultipleTrendChartData();
    }

    this.ref.detectChanges();
  }

  selectedIndicator(singleIndicatorSelected: boolean) {
    if (singleIndicatorSelected) {
      this.selectedIndicatorClass = 'button-selected';
      this.allIndicatorsClass = '';
      this.displayOption = DisplayOption.SelectedIndicator;
    } else {
      this.selectedIndicatorClass = '';
      this.allIndicatorsClass = 'button-selected';
      this.displayOption = DisplayOption.AllIndicators;
    }

    this.displayData();
  }

  sortSmallMultipleTrendCharts(sortType: SortType) {
    this.sortByAlphabeticalClass = '';
    this.sortByValueClass = '';
    this.sortByRankClass = '';

    if (sortType === SortType.AtoZ) {
      this.sortByAlphabeticalClass = 'button-selected';

      this.sortedAreas = this.data.sortedAreas;

      if (this.isNearestNeighbours) {
        const tempAreas = this.sortedAreas.slice(0);

        tempAreas.sort((a, b) => {
          if (a.Name.toLowerCase() < b.Name.toLowerCase()) {
            return -1;
          } else if (a.Name.toLowerCase() > b.Name.toLowerCase()) {
            return 1;
          } else {
            return 0;
          }
        });

        this.sortedAreas = tempAreas;
      }
    } else {
      if (sortType === SortType.Value) {
        this.sortByValueClass = 'button-selected';

        const groupRoot = this.ftHelperService.getCurrentGroupRoot();
        const areaHash = this.ftHelperService.getAreaHash();
        const areaAndDataSorterHelper = new AreaAndDataSorterHelper(0, groupRoot.Data, this.sortedAreas, areaHash);

        this.sortedAreas = areaAndDataSorterHelper.sortByValue();
      }

      if (sortType === SortType.Rank) {
        this.sortByRankClass = 'button-selected';

        this.sortedAreas = this.data.sortedAreas;
      }
    }

    this.areaButtonClass = '';
    this.regionButtonClass = 'button-selected';
    this.showAllAreasInRegion = true;

    this.generateSmallMultipleTrendChartData();
  }

  generateSmallMultipleTrendChartData() {
    const groupRoot = this.ftHelperService.getCurrentGroupRoot();
    const trendRoot = this.getTrendRootFromGroupRoot(groupRoot);
    const comparatorName = this.ftHelperService.getCurrentComparator().Name;

    if (isDefined(trendRoot)) {
      const metadataHash = this.ftHelperService.getMetadataHash();
      const metadata = metadataHash[trendRoot.IID];
      const comparisonConfig = this.ftHelperService.newComparisonConfigForTrendRoot(trendRoot, metadata);

      const hasDataChanged = this.ftHelperService.hasDataChanged(groupRoot);

      const valueType = metadata.ValueType.Name;

      let unitLabel = '';
      if (metadata.Unit.Label !== '') {
        unitLabel = metadata.Unit.Label;
      }

      this.finalData.length = 0;

      for (let counter = 0; counter < this.sortedAreas.length; counter++) {

        const area = this.sortedAreas[counter];
        const areaCode = area.Code;

        if (new TrendDataSetListHelper(trendRoot.Data[areaCode]).areAnyValidTrendValues()) {

          const areaName = this.getSmallChartAreaName(area);

          const trendData = this.createSmallTrendChart(trendRoot, areaCode, comparatorName,
            comparisonConfig, counter, areaName);

          const data = new Data();
          data.indicatorName = metadata.Descriptive.Name + this.ftHelperService.getSexAndAgeLabel(groupRoot);
          data.newDataBadge = NewDataBadgeHelper.getNewDataBadgeHtml(hasDataChanged);
          data.unit = valueType + ' - ' + unitLabel;
          data.areaName = areaName;
          data.areaCode = areaCode;
          data.trendRows = null;
          data.trendData = trendData;
          data.indicatorId = metadata.IID;

          this.finalData.push(data);
        }
      }
    }

    this.setLegendDisplay();
  }

  private getSmallChartAreaName(area: Area) {
    let areaName = area.Short;

    if (this.isNearestNeighbours) {
      const rank = area.Rank;
      if (Number(rank) > 0) {
        areaName = rank + ' - ' + areaName;
      }
    } else if (area.AreaTypeId === AreaTypeIds.Practice) {
      areaName = areaName + ' - ' + area.Code;
    }

    return areaName;
  }

  createSmallTrendChart(trendRoot: TrendRoot, areaCode: string, comparatorName: string,
    comparisonConfig: ComparisonConfig, chartId: number, areaName: string): TrendData {

    const trendData = new TrendData(this.ftHelperService);
    const trendDataPoints = trendRoot.Data[areaCode];
    const groupRoot = this.groupRoots.find(x => x.IID === trendRoot.IID);
    const metadataHash = this.ftHelperService.getMetadataHash();
    const metadata = metadataHash[groupRoot.IID];

    for (let counter = 0; counter < trendDataPoints.length; counter++) {
      const trendDataPoint = trendDataPoints[counter];
      const significance = trendDataPoint.Sig[comparisonConfig.comparatorId];
      let markerColour = Colour.getSignificanceColorForBenchmark(trendRoot.ComparatorMethodId,
        trendRoot.PolarityId, comparisonConfig, significance);

      if (trendRoot.ComparatorMethodId === ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels) {
        markerColour = '#FFF000';
      }

      const chartLimits = this.getChartLimits(trendRoot.Limits);

      this.showConfidenceIntervalsLabel = false;

      trendData.addAreaPoint(trendDataPoint, markerColour);
      trendData.addCIPoint(trendDataPoint);

      const coreDataSet: CoreDataSet = trendRoot.ComparatorData[counter][comparisonConfig.comparatorId];
      trendData.addBenchmarkPoint(coreDataSet);

      trendData.addLabel(trendRoot.Periods[counter]);
      trendData.width = 240;
      trendData.height = 140;
      trendData.areaCode = areaCode;
      trendData.areaName = areaName;
      trendData.viewMode = ViewModes.multiArea;
      trendData.trendDataPointsCount = trendDataPoints.length;
      trendData.comparatorName = comparatorName;
      trendData.unit = metadata.Unit;
      trendData.displayXAxisLabel = false;
      trendData.displayYAxisLabel = false;
      trendData.displayLegend = false;
      trendData.min = chartLimits.min;
      trendData.max = chartLimits.max;
      trendData.chartId = chartId;
      trendData.setShowConfidenceBars(this.showConfidenceIntervalsLabel);
    }

    return trendData;
  }

  displayData() {
    this.finalData.length = 0;

    let groupRootCounter = 0;
    const metadataHash = this.ftHelperService.getMetadataHash();

    this.groupRootsToDisplay = this.getGroupRootsToDisplay();
    this.groupRootsToDisplay.forEach(groupRoot => {

      const grouping = groupRoot.Grouping[0];
      const hasDataChanged = this.ftHelperService.hasDataChanged(groupRoot);

      const metadata = metadataHash[groupRoot.IID];
      const valueType = metadata.ValueType.Name;

      let period = '';
      if (isDefined(grouping)) {
        period = grouping.Period;
      }

      let unitLabel = '';
      if (metadata.Unit.Label !== '') {
        unitLabel = metadata.Unit.Label;
      }

      const areaCode = this.getSingleTrendsAreaCode();
      this.areaName = this.getAreaName(areaCode);

      const trendSource = new TrendSourceHelper(metadata).getTrendSource();

      if (groupRoot.RecentTrends !== null) {
        const recentTrends = groupRoot.RecentTrends[areaCode];

        if (isDefined(recentTrends)) {
          this.recentTrendImage = this.ftHelperService.getTrendMarkerImage(recentTrends.Marker, groupRoot.PolarityId);
        }
      }

      const data = new Data();
      data.indicatorName = metadata.Descriptive.Name + this.ftHelperService.getSexAndAgeLabel(groupRoot);
      data.period = period;
      data.newDataBadge = NewDataBadgeHelper.getNewDataBadgeHtml(hasDataChanged);
      data.isNewData = NewDataBadgeHelper.isNewData(hasDataChanged);
      data.unit = valueType + ' - ' + unitLabel;
      data.trendRows = this.generateTableData(groupRoot, groupRootCounter);
      data.trendData = this.generateChartData(groupRoot, groupRootCounter);
      data.trendSource = trendSource;
      data.areaName = this.areaName;
      data.indicatorId = metadata.IID;

      this.finalData.push(data);

      groupRootCounter++;
    });

    this.setLegendDisplay();

    this.ref.detectChanges();
  }

  private getAreaName(areaCode: string) {
    if (areaCode === AreaCodes.England) {
      return 'England';
    }

    const area = this.ftHelperService.getArea(areaCode);
    if (area.AreaTypeId === AreaTypeIds.Practice) {
      return area.Name + ' - ' + area.Code;
    }

    return area.Name;
  }

  isAllIndicatorSelected() {
    return this.selectedIndicatorClass === 'button-selected' ? false : true;
  }

  getTrendRootFromGroupRoot(groupRoot: GroupRoot) {
    return this.trendRoots.find(x => x.IID === groupRoot.IID &&
      x.Sex.Id === groupRoot.Sex.Id && x.Age.Id === groupRoot.Age.Id);
  }

  generateTableData(groupRoot: GroupRoot, tableId: number): TrendRow[] {
    const trendRows: TrendRow[] = [];
    const areaCode = this.getSingleTrendsAreaCode();
    const metadataHash = this.ftHelperService.getMetadataHash();
    let recentTrendImage = '-';

    const trendRoot = this.getTrendRootFromGroupRoot(groupRoot);

    if (isDefined(trendRoot)) {

      const metadata = metadataHash[trendRoot.IID];
      let metadataUnit = null;
      if (metadata) {
        metadataUnit = metadata.Unit;
      }

      const comparisonConfig = this.ftHelperService.newComparisonConfigForTrendRoot(trendRoot, metadata);
      const recentTrends = trendRoot.RecentTrends;
      let trendMarkerResult: TrendMarkerResult = null;
      if (recentTrends) {
        if (this.config.hasRecentTrends && recentTrends[areaCode]) {
          trendMarkerResult = recentTrends[areaCode];
          recentTrendImage = this.ftHelperService.getTrendMarkerImage(trendMarkerResult.Marker, trendRoot.PolarityId);
        }
      }

      const useTarget = comparisonConfig.useTarget;
      let targetLegendHtml = '';
      if (useTarget) {
        targetLegendHtml = this.ftHelperService.getTargetLegendHtml(comparisonConfig, metadata);
      }

      const areaData: TrendDataPoint[] = trendRoot.Data[areaCode];

      const periods = trendRoot.Periods;
      for (let counter = 0; counter < periods.length; counter++) {
        const period = trendRoot.Periods[counter];

        if (isDefined(areaData)) {
          const data: TrendDataPoint = areaData[counter];
          const comparatorData = trendRoot.ComparatorData[counter];

          const trendRow = this.addTrendRow(groupRoot, data, comparatorData, period, comparisonConfig,
            metadataUnit, tableId, targetLegendHtml, recentTrendImage, trendMarkerResult);

          if (isDefined(trendRow)) {
            // Set the flag accordingly to determine the last row
            // This will be helpful while writing recent trend to csv
            if (counter === periods.length - 1) {
              trendRow.isLastRow = true;
            } else {
              trendRow.isLastRow = false;
            }

            trendRows.push(trendRow);
          }
        }
      }
    }

    return trendRows;
  }

  addTrendRow(groupRoot: GroupRoot, trendDataPoint: TrendDataPoint, comparatorData, period: string, comparisonConfig: ComparisonConfig,
    unit: any, tableId: number, targetLegendHtml: string, recentTrendImage: string, recentTrendMarkerResult: TrendMarkerResult): TrendRow {

    const regionalData: CoreDataSet = comparatorData[ComparatorIds.SubNational];
    const nationalData: CoreDataSet = comparatorData[ComparatorIds.National];

    const dataInfo = this.ftHelperService.newTrendDataInfo(trendDataPoint);
    const regionalDataInfo = this.ftHelperService.newCoreDataSetInfo(regionalData);
    const nationalDataInfo = this.ftHelperService.newCoreDataSetInfo(nationalData);

    const dataInfoHasValue = dataInfo.isValue();
    const regionalDataHasValue = regionalDataInfo.isValue();
    const nationalDataHasValue = nationalDataInfo.isValue();

    // Omit row if no data and no value notes for any area
    if (!dataInfoHasValue && !regionalDataHasValue && !nationalDataHasValue &&
      !dataInfo.isNote() && !regionalDataInfo.isNote() && !nationalDataInfo.isNote()) {
      return;
    }

    // Significance marker
    let marker = '';
    if (dataInfoHasValue) {
      marker = this.getTrendMarkerHtml(groupRoot, trendDataPoint, comparisonConfig);
    }

    const trendValueDisplayer = this.ftHelperService.newTrendValueDisplayer(unit);
    const count = this.ftHelperService.formatTrendDataCount(dataInfo);
    const value = trendValueDisplayer.byDataInfo(dataInfo);
    const lowerCI = trendValueDisplayer.byNumberString(trendDataPoint.LF);
    const upperCI = trendValueDisplayer.byNumberString(trendDataPoint.UF);
    const valueNoteId = trendDataPoint.NoteId;

    const valueDisplayer = this.ftHelperService.newValueDisplayer(unit);
    let subnational = '';
    let subNationalNoteId = 0;
    if (this.ftHelperService.isSubnationalColumn()) {
      subnational = valueDisplayer.byDataInfo(regionalDataInfo);
      subNationalNoteId = regionalDataInfo.getNoteId();
    }

    let national = '';
    let nationalNoteId = 0;
    if (this.ftHelperService.getEnumParentDisplay() !== ParentDisplay.RegionalOnly) {
      national = valueDisplayer.byDataInfo(nationalDataInfo);
      nationalNoteId = nationalDataInfo.getNoteId();
    }

    const trendRow = new TrendRow();
    trendRow.period = period;
    trendRow.recentTrendImage = recentTrendImage;
    trendRow.trendImage = marker;
    trendRow.count = count;
    trendRow.value = value;
    trendRow.lowerCI = lowerCI;
    trendRow.upperCI = upperCI;
    trendRow.subNational = subnational;
    trendRow.national = national;
    trendRow.significance = trendDataPoint.Sig[comparisonConfig.comparatorId];
    trendRow.highlightEngland = false;
    trendRow.highlightAreaValue = false;
    trendRow.useRagColours = comparisonConfig.useRagColours;
    trendRow.useQuintileColouring = comparisonConfig.useQuintileColouring;
    trendRow.comparatorName = this.ftHelperService.getCurrentComparator().Short;
    trendRow.trendMarkerResult = recentTrendMarkerResult;
    trendRow.subNationalNoteId = subNationalNoteId;
    trendRow.nationalNoteId = nationalNoteId;
    trendRow.tableId = tableId;
    trendRow.useTarget = comparisonConfig.useTarget;
    trendRow.targetLegendHtml = targetLegendHtml;
    trendRow.valueNoteId = valueNoteId;
    trendRow.polarityId = groupRoot.PolarityId;
    trendRow.subNationalSignificance = regionalData.Significance;
    trendRow.nationalSignificance = nationalData.Significance;
    trendRow.nationalData = nationalData;
    trendRow.subnationalData = regionalData;
    trendRow.areaData = trendDataPoint;

    return trendRow;
  }

  getTrendMarkerHtml(groupRoot: GroupRoot, trendDataPoint: TrendDataPoint, comparisonConfig: ComparisonConfig): string {
    const sig = trendDataPoint.Sig[comparisonConfig.comparatorId];
    const useRag = comparisonConfig.useRagColours;
    const useQuintileColouring = comparisonConfig.useQuintileColouring;

    const markerImage = this.ftHelperService.getMiniMarkerImageFromSignificance(sig, useRag, useQuintileColouring,
      groupRoot.IID, groupRoot.Sex.Id, groupRoot.Age.Id);

    const markerHtml = this.ftHelperService.getURL().img + markerImage;

    return markerHtml;
  }

  getSingleTrendsAreaCode(): string {
    const neighbourAreaCode = '';

    if (this.model.isNearestNeighbours() && neighbourAreaCode) {
      return neighbourAreaCode;
    }

    return this.model.areaCode;
  }

  generateChartData(groupRoot: GroupRoot, chartId: number): TrendData {

    const ftHelperService = this.ftHelperService;

    const trendRoot = this.getTrendRootFromGroupRoot(groupRoot);
    const areaCode = this.getSingleTrendsAreaCode();
    const comparatorName = ftHelperService.getCurrentComparator().Name;
    const metadataHash = ftHelperService.getMetadataHash();
    const metadata = metadataHash[trendRoot.IID];
    const comparisonConfig = ftHelperService.newComparisonConfigForTrendRoot(trendRoot, metadata);
    const trendDataPoints: TrendDataPoint[] = trendRoot.Data[areaCode];

    const trendDataPointsCount = _.isUndefined(trendDataPoints) ? 0 : trendDataPoints.length;

    const isEngland = areaCode === AreaCodes.England;

    const currentComparator = ftHelperService.getCurrentComparator();
    let benchmarkComparatorId = ComparatorIds.SubNational;
    if (currentComparator.Code === AreaCodes.England) {
      benchmarkComparatorId = ComparatorIds.National;
    }

    const trendData = new TrendData(ftHelperService);

    for (let counter = 0; counter < trendDataPointsCount; counter++) {
      const trendDataPoint: TrendDataPoint = trendDataPoints[counter];
      const significance = trendDataPoint.Sig[comparisonConfig.comparatorId];

      // Large chart marker colour
      const markerColour = isEngland
        ? Colour.comparator
        : Colour.getSignificanceColorForBenchmark(trendRoot.ComparatorMethodId,
          trendRoot.PolarityId, comparisonConfig, significance);

      trendData.addAreaPoint(trendDataPoint, markerColour);
      trendData.addCIPoint(trendDataPoint);

      // Do not display benchmark for counts as it will be much higher than child
      // area values
      if (metadata.ValueType.Id !== ValueTypeIds.Count) {
        const coreDataSet = trendRoot.ComparatorData[counter][benchmarkComparatorId];
        trendData.addBenchmarkPoint(coreDataSet);
      }

      trendData.addLabel(trendRoot.Periods[counter]);
    }

    const chartLimits = this.getChartLimits(trendRoot.Limits);

    trendData.width = 400;
    trendData.height = 300;
    trendData.areaCode = areaCode;

    if (areaCode === AreaCodes.England) {
      trendData.areaName = 'England';
    } else {
      trendData.areaName = ftHelperService.getArea(areaCode).Name;
    }

    trendData.indicatorName = metadata.Descriptive.Name + ftHelperService.getSexAndAgeLabel(groupRoot);
    trendData.viewMode = ViewModes.area;
    trendData.trendDataPointsCount = trendDataPointsCount;

    if (this.isNearestNeighbours && currentComparator.Code !== AreaCodes.England) {
      trendData.comparatorName = this.formattedselectedParentName;
    } else {
      trendData.comparatorName = comparatorName;
    }

    trendData.unit = metadata.Unit;
    trendData.displayXAxisLabel = true;
    trendData.displayYAxisLabel = true;
    trendData.displayLegend = true;
    trendData.min = chartLimits.min;
    trendData.max = chartLimits.max;
    trendData.chartId = chartId;

    trendData.setUnitLabel(metadata.unitLabel);
    trendData.setRadius(trendDataPointsCount);
    trendData.setShowConfidenceBars(this.showConfidenceIntervalsLabel);

    return trendData;
  }

  getChartLimits(limits: Limits) {
    const min = this.getMinYAxis(limits);
    const max = limits !== null ? limits.Max : null;
    return { min: min, max: max };
  }

  getMinYAxis(limits: Limits): number {

    const startAtZero = this.config.startZeroYAxis;

    if (limits) {
      return startAtZero && limits.Min > 0 ? 0 : limits.Min;
    }
    return startAtZero ? 0 : null;
  }

  goToMetadataPage(index) {
    this.ftHelperService.goToMetadataPage(index);
  }

  exportChart(index: number) {
    this.chartList[index].exportChart({ type: 'image/png' }, {});
  }

  exportTableAsCsv(indicatorId: number) {
    const csvData: CsvData[] = [];
    const root = this.ftHelperService.getCurrentGroupRoot();

    this.finalData.forEach(data => {
      if (Number(data.indicatorId) === indicatorId) {
        data.trendRows.forEach(row => {
          const nationalData = this.addCsvRow(data, row, root, CsvCoreDataType.National);
          csvData.push(nationalData);

          const subnationalData = this.addCsvRow(data, row, root, CsvCoreDataType.Subnational);
          csvData.push(subnationalData);

          const areaData = this.addCsvRow(data, row, root, CsvCoreDataType.Area);
          csvData.push(areaData);
        });
      }
    });

    this.csvConfig = new CsvConfig();
    this.csvConfig.tab = Tabs.Trends;
    this.csvConfig.csvData = csvData;
  }

  addCsvRow(data: Data, row: TrendRow, root: GroupRoot, rowType: number): CsvData {
    /*
    PLEASE NOTE: Changing the order of the columns below will affect
                 the data printed in the resulting CSV file
    */

    const csvData = new CsvData();
    let lowerCI = '';
    let upperCI = '';
    let lowerCI99_8 = '';
    let upperCI99_8 = '';
    let category = '';
    let categoryType = '';
    let value = '';
    let count = '';
    let denominator = '';
    let parentCode = '';
    let parentName = '';
    let areaCode = '';
    let areaName = '';
    let areaType = '';

    const parentAreaHelper = new ParentAreaHelper(this.ftHelperService);
    const parentAreaTypeHelper = new ParentAreaTypeHelper(this.ftHelperService);

    csvData.indicatorId = data.indicatorId;
    csvData.indicatorName = data.indicatorName;

    switch (rowType) {
      case CsvCoreDataType.National:
        categoryType = CsvDataHelper.getDisplayValue(row.nationalData.CategoryTypeId);
        category = CsvDataHelper.getDisplayValue(row.nationalData.CategoryId);
        lowerCI = CsvDataHelper.getDisplayValue(row.nationalData.LoCI);
        upperCI = CsvDataHelper.getDisplayValue(row.nationalData.UpCI);
        lowerCI99_8 = CsvDataHelper.getDisplayValue(row.nationalData.LoCI99_8);
        upperCI99_8 = CsvDataHelper.getDisplayValue(row.nationalData.UpCI99_8);
        count = CsvDataHelper.getDisplayValue(row.nationalData.Count);
        denominator = CsvDataHelper.getDisplayValue(row.nationalData.Denom);
        value = CsvDataHelper.getDisplayValue(row.nationalData.Val);

        parentCode = '';
        parentName = '';
        areaCode = AreaCodes.England;
        areaName = 'England';
        areaType = 'England';
        break;

      case CsvCoreDataType.Subnational:
        categoryType = CsvDataHelper.getDisplayValue(row.subnationalData.CategoryTypeId);
        category = CsvDataHelper.getDisplayValue(row.subnationalData.CategoryId);
        lowerCI = CsvDataHelper.getDisplayValue(row.subnationalData.LoCI);
        upperCI = CsvDataHelper.getDisplayValue(row.subnationalData.UpCI);
        lowerCI99_8 = CsvDataHelper.getDisplayValue(row.subnationalData.LoCI99_8);
        upperCI99_8 = CsvDataHelper.getDisplayValue(row.subnationalData.UpCI99_8);
        count = CsvDataHelper.getDisplayValue(row.subnationalData.Count);
        denominator = CsvDataHelper.getDisplayValue(row.subnationalData.Denom);
        value = CsvDataHelper.getDisplayValue(row.subnationalData.Val);

        parentCode = AreaCodes.England;
        parentName = 'England';
        areaCode = parentAreaHelper.getParentAreaCode();
        areaName = parentAreaHelper.getParentAreaNameForCSV();
        areaType = parentAreaTypeHelper.getParentAreaTypeNameForCSV();
        break;

      case CsvCoreDataType.Area:
        categoryType = CsvDataHelper.getDisplayValue(row.areaData.CategoryTypeId);
        category = CsvDataHelper.getDisplayValue(row.areaData.CategoryId);
        lowerCI = CsvDataHelper.getDisplayValue(row.areaData.L);
        upperCI = CsvDataHelper.getDisplayValue(row.areaData.U);
        lowerCI99_8 = CsvDataHelper.getDisplayValue(row.areaData.L99_8);
        upperCI99_8 = CsvDataHelper.getDisplayValue(row.areaData.U99_8);
        count = CsvDataHelper.getDisplayValue(row.areaData.C);
        denominator = CsvDataHelper.getDisplayValue(row.areaData.Denom);
        value = CsvDataHelper.getDisplayValue(row.areaData.D);

        parentCode = parentAreaHelper.getParentAreaCode();
        parentName = parentAreaHelper.getParentAreaNameForCSV();
        areaCode = this.model.areaCode;
        areaName = this.ftHelperService.getArea(areaCode).Name;
        areaType = this.ftHelperService.getAreaTypeName();
        break;
    }

    csvData.parentCode = parentCode;
    csvData.parentName = parentName;
    csvData.areaCode = areaCode;
    csvData.areaName = areaName;
    csvData.areaType = areaType;

    csvData.sex = root.Sex.Name;
    csvData.age = root.Age.Name;

    csvData.categoryType = categoryType;
    csvData.category = category;
    csvData.timePeriod = row.period;
    csvData.value = value;
    csvData.lowerCiLimit95 = lowerCI;
    csvData.upperCiLimit95 = upperCI;
    csvData.lowerCiLimit99_8 = lowerCI99_8;
    csvData.upperCiLimit99_8 = upperCI99_8;
    csvData.count = count;
    csvData.denominator = denominator;

    csvData.valueNote = this.getValueNoteForCsv(rowType, row);

    // Write trend only for the last non-comparator row
    csvData.recentTrend = '';
    if (isDefined(row.trendMarkerResult) &&
      row.isLastRow &&
      rowType === CsvCoreDataType.Area) {

      csvData.recentTrend = new TrendMarkerLabelProvider(row.polarityId).getLabel(row.trendMarkerResult.Marker);
    }

    csvData.comparedToEnglandValueOrPercentiles = '';
    if (isDefined(row.nationalData.Significance)) {
      csvData.comparedToEnglandValueOrPercentiles = new SignificanceFormatter(row.polarityId,
        root.ComparatorMethodId).getLabel(Number(row.nationalSignificance));
    }

    csvData.comparedToRegionValueOrPercentiles = '';
    if (isDefined(row.subnationalData.Significance)) {
      csvData.comparedToRegionValueOrPercentiles = new SignificanceFormatter(row.polarityId,
        root.ComparatorMethodId).getLabel(Number(row.subNationalSignificance));
    }

    csvData.timePeriodSortable = new TimePeriod(root.Grouping[0]).getSortableNumber();
    csvData.newData = data.isNewData ? 'New data' : '';

    csvData.comparedToGoal = '';
    if (isDefined(row.areaData) &&
      isDefined(row.areaData.Sig[ComparatorIds.Target])) {

      csvData.comparedToGoal = new SignificanceFormatter(row.polarityId,
        root.ComparatorMethodId).getTargetLabel(Number(row.areaData.Sig[ComparatorIds.Target]));
    }

    return csvData;
  }

  getValueNoteForCsv(rowType: number, row: TrendRow): string {
    let valueNote = '';
    const provider = this.ftHelperService.newValueNoteTooltipProvider();
    switch (rowType) {
      case 0:
        if (isDefined(row.nationalNoteId)) {
          valueNote = provider.getTextFromNoteId(row.nationalNoteId);
        }
        break;
      case 1:
        if (isDefined(row.subNationalNoteId)) {
          valueNote = provider.getTextFromNoteId(row.subNationalNoteId);
        }
        break;
      case 2:
        if (isDefined(row.valueNoteId)) {
          valueNote = provider.getTextFromNoteId(row.valueNoteId);
        }
        break;
    }

    return valueNote.replace('* ', '');
  }

  getGroupRootByIndicatorId(indicatorId: number): GroupRoot {
    return this.model.groupRoots.find(x => x.IID === indicatorId);
  }

  getAreaCode(): string {
    if (this.model.isNearestNeighbours()) {
      const neighbourSelectedName = $('#trends-tab-option-0').text();

      const selectedArea = this.data.sortedAreas.find(area => area.Name === neighbourSelectedName);
      return selectedArea.Code;
    }

    return this.model.areaCode;
  }

  showErrorBar() {
    this.showConfidenceIntervalsLabel = !this.showConfidenceIntervalsLabel;

    if (this.showConfidenceIntervalsLabel) {
      this.confidenceIntervalsLabel = 'Hide confidence intervals';
    } else {
      this.confidenceIntervalsLabel = 'Show confidence intervals';
    }

    this.finalData.forEach(data => {
      data.trendData.setShowConfidenceBars(this.showConfidenceIntervalsLabel);
    });

    this.selectedArea();
  }

  getGroupRootsToDisplay(): GroupRoot[] {
    let groupRootsToDisplay: GroupRoot[] = [];
    if (this.displayOption === DisplayOption.SelectedIndicator) {
      const indicatorIndex = this.ftHelperService.getIndicatorIndex();
      groupRootsToDisplay.push(this.groupRoots[indicatorIndex]);
    } else {
      groupRootsToDisplay = this.groupRoots;
    }

    return groupRootsToDisplay;
  }

  // Display legend
  setLegendDisplay() {
    const config = new LegendConfig(PageType.Trends, this.ftHelperService);
    config.configureForMultipleIndicators(this.groupRootsToDisplay);
    this.legendConfig = config;

    this.showRecentTrends = this.ftHelperService.getFTConfig().hasRecentTrends;
  }

  // Event emitter method
  updateData(areaCode: string) {
    // Set the area code
    this.ftHelperService.setAreaCode(areaCode);

    // Update the area name in the tab button
    this.selectedAreaName = this.ftHelperService.getArea(areaCode).Short;

    // Load data
    this.selectedArea();
  }

  showBigArea(areaCode: string) {

    // Select area and show big chart and table
    this.isSingleArea = true;
    this.updateData(areaCode);

    // Ensure user can see chart
    const scrollLimit = 300;
    if (this.uiService.getScrollTop() > scrollLimit) {
      this.uiService.setScrollTop(scrollLimit);
    }
  }

  /**  Event emitter method: hover over chart marker */
  enableHover(event) {
    const areaCode = event.areaCode;
    const period = event.period;
    const chartId = event.chartId;
    const trendRows = this.finalData[chartId].trendRows;

    if (isDefined(trendRows) && trendRows) {

      this.resetHoverStyles(trendRows);

      if (period !== 'none') {
        const trendRow = trendRows.find(x => x.period === period);
        if (isDefined(trendRow)) {
          if (isDefined(areaCode)) {
            // Highlight comparator
            if (areaCode === AreaCodes.England) {
              trendRow.highlightEngland = true;
            } else {
              trendRow.highlightSubnational = true;
            }
          } else {
            // Highlight area value
            trendRow.highlightAreaValue = true;
            trendRow.hoverCI = true;
          }
        }
      }

      this.ref.detectChanges();
    }
  }

  // Event emitter method
  enableRowHover(event) {
    const period = event.period;
    const tableId = event.tableId;
    const trendRows = this.finalData[tableId].trendRows;

    if (trendRows) {

      this.resetHoverStyles(trendRows);

      if (period !== 'none') {
        const trendRow = trendRows.find(x => x.period === period);

        if (trendRow !== null) {

          // No benchmark for quintiles
          if (!trendRow.useQuintileColouring) {

            // Highlight comparator cell
            if (this.ftHelperService.getComparatorId() === ComparatorIds.National) {
              trendRow.highlightEngland = true;
            } else {
              trendRow.highlightSubnational = true;
            }
          }

          trendRow.highlightAreaValue = true;
        }
      }

      this.ref.detectChanges();
    }
  }

  // Event emitter method
  // Used to export chart
  updateChartList(event) {
    if (event.chartId === 0) {
      this.chartList.length = 0;
    }

    this.chartList.push(event.chart);
  }

  resetHoverStyles(trendRows: TrendRow[]) {
    trendRows.forEach(trendRow => {
      trendRow.highlightEngland = false;
      trendRow.highlightSubnational = false;
      trendRow.highlightAreaValue = false;
      trendRow.hoverCI = false;
    });
  }
}
