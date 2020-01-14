import { Component, HostListener } from '@angular/core';
import * as _ from 'underscore';
import { forkJoin } from 'rxjs';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { UIService } from '../shared/service/helper/ui.service';
import {
  GroupRootHelper, CoreDataSetListHelper, AreaHelper, TooltipHelper, LegendHelper,
  DeviceServiceHelper, NewDataBadgeHelper, ParentAreaHelper, ParentAreaTypeHelper
} from '../shared/shared';
import {
  AreaTypeIds, ComparatorIds, ValueTypeIds, ParentDisplay, PolarityIds,
  TrendMarkerValue, SpineChartMinMaxLabelDescription, SpineChartMinMaxLabel,
  PageType, AreaCodes, Tabs, CsvCoreDataType
} from '../shared/constants';
import { SpineChartCalculator, SpineChartDimensions } from './spine-chart.classes';
import { LegendConfig } from '../shared/component/legend/legend';
import {
  IndicatorStats, FTModel, FTConfig, IndicatorMetadata, IndicatorFormatter,
  GroupRoot, CoreDataSet, IndicatorStatsPercentilesFormatted, IndicatorStatsPercentiles,
  ComparisonConfig, TrendMarkerResult, Area
} from '../typings/FT';
import { LightBoxConfig, LightBoxTypes } from '../shared/component/light-box/light-box';
import { DeviceDetectorService } from '../../../node_modules/ngx-device-detector';
import { isDefined } from '@angular/compiler/src/util';
import { TrendMarkerLabelProvider } from '../shared/classes/trendmarker-label-provider';
import { TimePeriod } from '../shared/classes/time-period';
import { CsvConfig, CsvData, CsvDataHelper } from '../shared/component/export-csv/export-csv';
import { AreaService } from '../shared/service/api/area.service';

@Component({
  selector: 'ft-area-profile',
  templateUrl: './area-profile.component.html',
  styleUrls: ['./area-profile.component.css']
})
export class AreaProfileComponent {

  public areRecentTrendsDisplayed = false;
  public spineChartImageUrl: string = null;
  public lowest: string;
  public highest: string;
  public parentType: string;
  public shortAreaName: string;
  public areaName: string;
  public benchmarkName: string;
  public isNationalAndRegional = false;
  public isNotNN = false;
  public trendColSpan = 0;
  public colSpan = 0;
  public indicatorRows: IndicatorRow[];
  public isAnyData = true;

  private model: FTModel;
  private config: FTConfig;
  private imgUrl: string;
  private tooltipHelper: TooltipHelper;

  private scrollTop: number;
  private isAreaIgnored: boolean;
  private currentTrendsTooltipHtml: string;
  private area: Area;
  private isEnglandAreaSelected: boolean;

  // Data from AJAX
  private indicatorStats: IndicatorStats[];
  private metadataHash: Map<number, IndicatorMetadata>;

  // HTML
  private NoData = '<div class="no-data">-</div>';

  // Legend
  showRecentTrends = false;
  showDataQuality = false;
  legendConfig: LegendConfig;
  csvConfig: CsvConfig;

  // Lightbox
  lightBoxConfig: LightBoxConfig;

  constructor(private ftHelperService: FTHelperService,
    private indicatorService: IndicatorService,
    private areaService: AreaService,
    private uiService: UIService,
    private deviceService: DeviceDetectorService) { }

  @HostListener('window:AreaProfileSelected', ['$event', '$event.detail.isEnglandAreaType'])
  public onOutsideEvent(event, isEnglandAreaType) {
    const ftHelper = this.ftHelperService;

    const groupRoots = ftHelper.getAllGroupRoots();
    this.model = ftHelper.getFTModel();
    this.config = ftHelper.getFTConfig();
    this.isAnyData = true;

    this.isEnglandAreaSelected = isEnglandAreaType;

    this.scrollTop = this.uiService.getScrollTop();

    this.setDisplayConfig();
    this.setIsAreaIgnored();

    const profileId = this.model.profileId;
    const groupId = this.model.groupId;
    const areaTypeId = this.model.areaTypeId;
    const parentAreaTypeId = this.model.parentTypeId;
    const nearestNeighbourCode = this.isNotNN ? '' : this.model.nearestNeighbour;
    const parentAreaCode = new ParentAreaHelper(ftHelper).getParentAreaCode();

    // AJAX calls
    const indicatorStatisticsObservable = this.indicatorService.getIndicatorStatistics(areaTypeId,
      parentAreaCode, profileId, groupId);

    const allIndicatorsInProfileGroupForChildAreasObservable = this.indicatorService.getLatestDataForAllIndicatorsInProfileGroupForChildAreas(groupId,
      areaTypeId, parentAreaCode, profileId);

    const parentToChildAreasObservable = this.areaService.getParentToChildAreas(profileId, areaTypeId, parentAreaTypeId, nearestNeighbourCode);

    forkJoin([indicatorStatisticsObservable, allIndicatorsInProfileGroupForChildAreasObservable, parentToChildAreasObservable]).subscribe(results => {

      this.indicatorStats = _.values(results[0]);
      this.model.groupRoots = results[1];
      const areaMappings = results[2];

      // Set area mappings
      this.ftHelperService.setAreaMappings(areaMappings);

      this.metadataHash = ftHelper.getMetadataHash();

      this.setAreaProfileHtml();

      // Unlock UI
      ftHelper.showAndHidePageElements();
      ftHelper.showTargetBenchmarkOption(groupRoots);
      this.uiService.setScrollTop(this.scrollTop);

      this.showRecentTrends = this.areRecentTrendsDisplayed;
      this.showDataQuality = new LegendHelper(this.config).showDataQualityLegend();

      ftHelper.unlock();
    });

    this.tooltipHelper = new TooltipHelper(this.ftHelperService.newTooltipManager());
  }

  @HostListener('window:NoDataDisplayed', ['$event', '$event.detail.isEnglandAreaType'])
  public refreshVariable(isEnglandAreaType) {
    this.isAnyData = false;
    this.isEnglandAreaSelected = isEnglandAreaType;
  }

  setAreaProfileHtml() {

    const ftHelper = this.ftHelperService;

    // Benchmark
    const comparatorId = ftHelper.getComparatorId();
    const isNationalBenchmark = (comparatorId === ComparatorIds.National);
    const benchmark = isNationalBenchmark ?
      ftHelper.getNationalComparator() :
      ftHelper.getParentArea();

    const areaCode = this.model.areaCode;
    this.area = ftHelper.getArea(areaCode);

    const indicatorRows = [];
    let rootIndex = 0;
    const groupRoots = ftHelper.getAllGroupRoots();

    for (const root of groupRoots) {

      const metadata = this.metadataHash[root.IID];
      const areaData = new CoreDataSetListHelper(root.Data).findByAreaCode(areaCode);

      // Create indicator row
      const row = new IndicatorRow();
      row.rootIndex = rootIndex;
      indicatorRows.push(row);
      rootIndex += 1;

      row.comparisonConfig = ftHelper.newComparisonConfig(root, metadata);
      this.assignStatsToRow(root, row);

      // Parent data
      const subnationalData = ftHelper.getRegionalComparatorGrouping(root).ComparatorData;
      const nationalData = ftHelper.getNationalComparatorGrouping(root).ComparatorData;
      const benchmarkData = isNationalBenchmark ? nationalData : subnationalData;
      row.benchmarkData = benchmarkData;

      row.nationalData = nationalData;
      row.subnationalData = subnationalData;

      // Init formatter
      const formatter = ftHelper.newIndicatorFormatter(root, metadata, areaData, row.statsF);
      formatter.averageData = benchmarkData;

      // Value displayer
      const unit = !!metadata ? metadata.Unit : null;
      const valueDisplayer = ftHelper.newValueDisplayer(unit);

      // Set piggy back object
      row.formatter = formatter;
      row.areaData = areaData;
      row.groupRoot = root;
      row.indicatorMetadata = metadata;
      row.area = this.area;

      // Set display properties
      row.period = root.Grouping[0].Period;
      row.areaCount = formatter.getAreaCount();
      row.areaValue = formatter.getAreaValue();

      // Area data note tooltip
      const dataInfo = ftHelper.newCoreDataSetInfo(row.areaData);
      if (dataInfo.isNote()) {
        row.areaValueTooltip = ftHelper.getValueNoteById(areaData.NoteId).Text;
      }

      // England value
      const nationalDataInfo = ftHelper.newCoreDataSetInfo(nationalData);
      row.englandValue = valueDisplayer.byDataInfo(nationalDataInfo, { noCommas: 'y' })
      if (nationalDataInfo.isNote()) {
        row.englandValueTooltip = ftHelper.getValueNoteById(nationalData.NoteId).Text;
      }

      // Subnational value
      if (ftHelper.isSubnationalColumn()) {
        const subnationalDataInfo = ftHelper.newCoreDataSetInfo(subnationalData);
        row.subnationalValue = valueDisplayer.byDataInfo(subnationalDataInfo, { noCommas: 'y' });
        if (subnationalDataInfo.isNote()) {
          row.subnationalValueTooltip = ftHelper.getValueNoteById(subnationalData.NoteId).Text;
        }
      }

      this.setSpineChartDimensions(row);

      // Min / Max
      const dimensions = row.spineChartDimensions
      if (dimensions.isAnyData && dimensions.isSufficientData) {
        // Data available
        row.englandMin = formatter.getMin();
        row.englandMax = formatter.getMax();
        row.minTooltip = this.polarityToText(root.PolarityId, true);
        row.maxTooltip = this.polarityToText(root.PolarityId, false);
      } else {
        // No data
        row.englandMin = this.NoData;
        row.englandMax = this.NoData;
      }

      // Trend
      if (root.RecentTrends) {

        let trendMarkerValue, polarityId;
        if (areaData) {
          const recentTrends = root.RecentTrends[areaData.AreaCode];
          trendMarkerValue = recentTrends.Marker;
          polarityId = root.PolarityId;
          row.trendMarkerResult = recentTrends;
        } else {
          trendMarkerValue = TrendMarkerValue.CannotCalculate;
          polarityId = 0;
        }
        row.recentTrendImage = ftHelper.getTrendMarkerImage(trendMarkerValue, polarityId);
      }

      if (this.isAreaIgnored) {
        // Do not show any message or chart if area is ignored for spine chart
        dimensions.isAnyData = false;
      }
    }

    this.setAreaHeadings(benchmark);
    this.indicatorRows = indicatorRows;

    this.setLegendDisplay(groupRoots);

    ftHelper.initBootstrapTooltips();
  }

  setLegendDisplay(groupRoots: GroupRoot[]) {
    const config = new LegendConfig(PageType.AreaProfiles, this.ftHelperService);
    config.configureForMultipleIndicators(groupRoots);
    this.legendConfig = config;
  }

  polarityToText(polarity, isForLowest) {

    if (polarity === PolarityIds.RAGLowIsGood || polarity === PolarityIds.RAGHighIsGood) {
      return isForLowest ? 'Worst' : 'Best';
    }
    return isForLowest ? 'Lowest' : 'Highest';
  }

  assignStatsToRow(root: GroupRoot, row: IndicatorRow) {

    const statsBase: IndicatorStats =
      new GroupRootHelper(root).findMatchingItemBySexAgeAndIndicatorId(this.indicatorStats);

    if (statsBase) {
      row.statsF = statsBase.StatsF;
      row.stats = statsBase.Stats;
      row.haveRequiredValuesForSpineChart = statsBase.HaveRequiredValues;
    }
  }

  setSpineChartDimensions(row: IndicatorRow): void {

    const ftHelper = this.ftHelperService;
    const benchmarkDataInfo = ftHelper.newCoreDataSetInfo(row.benchmarkData);

    const isValidBenchmarkValue = benchmarkDataInfo.isValue();

    if (isValidBenchmarkValue && !row.haveRequiredValuesForSpineChart) {
      // Should show not enough values message
      const dimensions = new SpineChartDimensions();
      dimensions.isSufficientData = false;
      row.spineChartDimensions = dimensions;
    } else if (!row.stats ||
      !isValidBenchmarkValue ||
      row.indicatorMetadata.ValueType.Id === ValueTypeIds.Count) {
      // Should show no data message
      const dimensions = new SpineChartDimensions();
      dimensions.isAnyData = false;
      row.spineChartDimensions = dimensions;
    } else {
      // Have data can show spine chart
      const polarityId = row.groupRoot.PolarityId;
      const spineChart = new SpineChartCalculator();
      const proportions = spineChart.getSpineProportions(row.benchmarkData.Val, row.stats, polarityId);

      const dataInfo = ftHelper.newCoreDataSetInfo(row.areaData);
      const spineDimensions = spineChart.getDimensions(proportions, dataInfo, this.imgUrl, row.comparisonConfig,
        row.indicatorMetadata.IID, ftHelper.getMarkerImageFromSignificance);

      row.spineChartDimensions = spineDimensions;
    }
  }

  public getIndicatorNameHtml(row: IndicatorRow): string {

    const formatter = row.formatter;
    const ftHelper = this.ftHelperService;
    const root = row.groupRoot;
    const metadata = row.indicatorMetadata;

    // Indicator name & data quality
    const html = [
      formatter.getIndicatorNamePlusSexAndAge(),
      this.ftHelperService.getIndicatorDataQualityHtml(formatter.getDataQuality())
    ];

    // New data badge
    if (ftHelper.hasDataChanged(root)) {
      html.push('&nbsp;<span style="margin-right:8px;" class="badge badge-success">New data</span>');
    }

    // Target legend
    const targetLegend = ftHelper.getTargetLegendHtml(row.comparisonConfig, metadata);
    if (targetLegend) {
      html.push('<br>', targetLegend);
    }

    return html.join('');
  }

  setAreaHeadings(benchmark) {
    this.shortAreaName = new AreaHelper(this.area).getShortAreaNameToDisplay();
    this.areaName = this.area.Name;
    this.benchmarkName = benchmark.Name;
  }

  setDisplayConfig() {
    const ftHelper = this.ftHelperService;

    const urls = ftHelper.getURL();
    this.imgUrl = urls.img;

    const groupRoots = ftHelper.getAllGroupRoots();

    this.isNationalAndRegional = ftHelper.getEnumParentDisplay() === ParentDisplay.NationalAndRegional &&
      this.model.parentTypeId !== AreaTypeIds.Country;

    const trendMarkerFound = groupRoots && groupRoots[0] && groupRoots[0].RecentTrends;

    this.trendColSpan = trendMarkerFound ? 3 : 2;
    this.areRecentTrendsDisplayed = trendMarkerFound && this.config.hasRecentTrends;

    this.isNotNN = !this.model.isNearestNeighbours()
    this.colSpan = this.isNationalAndRegional && !this.model.isNearestNeighbours() ? 3 : 4;

    if (this.isNotNN) {
      this.parentType = ftHelper.getParentTypeName();
    } else {
      this.parentType = 'Neighbrs average';
    }

    this.setSpineHeadersAndChartLabelImage(urls.img, groupRoots);

    this.lowest = this.config.spineHeaders.min;
    this.highest = this.config.spineHeaders.max;
  }

  setSpineHeadersAndChartLabelImage(imageUrl: string, groupRoots: GroupRoot[]) {

    const headers = new SpineChartHeaderLabeller(groupRoots);

    this.config.spineHeaders.min = headers.minHeader;
    this.config.spineHeaders.max = headers.maxHeader;
    this.config.spineChartMinMaxLabelId = headers.spineChartMinMaxLabelId;

    this.spineChartImageUrl = imageUrl + 'spine-key-label-id-' + headers.spineChartMinMaxLabelId + '.png';
  }

  /** Whether the current area should is ignored for the spine charts */
  setIsAreaIgnored(): void {

    const ignoredSpineChartAreas = this.config.ignoredSpineChartAreas;

    // Is area too small for spine charts
    let isIgnored = false;
    if (ignoredSpineChartAreas) {
      const areaCode = this.model.areaCode;

      const areas = ignoredSpineChartAreas.split(',');
      isIgnored = _.any(areas, function (area) {
        return area === areaCode;
      });
    }

    this.isAreaIgnored = isIgnored;
  }

  goToBarChart(row: IndicatorRow) {
    this.ftHelperService.goToBarChartPage(row.rootIndex, true);
  }

  goToTrends(row: IndicatorRow) {
    const ftHelper = this.ftHelperService;
    ftHelper.recentTrendSelected().byGroupRoot(row.rootIndex);
  }

  onExportClick(event: MouseEvent) {
    // Download table does not work on IE
    // Check whether the browser is IE
    if (new DeviceServiceHelper(this.deviceService).isIE()) {
      // Display lightbox informing user to use a different browser
      const config = new LightBoxConfig();
      config.Type = LightBoxTypes.Ok;
      config.Title = 'Browser not compatible';
      config.Html = 'Exporting the table as an image is not supported by Internet Explorer. Please use a different browser.'
      config.Height = 200;
      config.Top = 500;
      this.lightBoxConfig = config;
    } else {
      this.ftHelperService.exportTableAsImage('area-profile-container',
        'AreaProfilesTable', '#key-spine-chart,#spine-range-key');
    }
  }

  onExportCsvFileClick(event: MouseEvent) {
    const csvData: CsvData[] = [];

    this.indicatorRows.forEach(indicatorRow => {
      const nationalData = this.addCsvRow(indicatorRow, CsvCoreDataType.National);
      csvData.push(nationalData);

      const subnationalData = this.addCsvRow(indicatorRow, CsvCoreDataType.Subnational);
      csvData.push(subnationalData);

      const areaData = this.addCsvRow(indicatorRow, CsvCoreDataType.Area);
      csvData.push(areaData);
    });

    this.csvConfig = new CsvConfig();
    this.csvConfig.tab = Tabs.AreaProfiles;
    this.csvConfig.csvData = csvData;
  }

  addCsvRow(indicatorRow: IndicatorRow, rowType: number): CsvData {
    const data = new CsvData();
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

    const indicatorId = indicatorRow.indicatorMetadata.IID;
    data.indicatorId = indicatorId.toString();
    data.indicatorName = this.ftHelperService.getIndicatorName(indicatorId);

    switch (rowType) {
      case CsvCoreDataType.National:
        categoryType = CsvDataHelper.getDisplayValue(indicatorRow.nationalData.CategoryTypeId);
        category = CsvDataHelper.getDisplayValue(indicatorRow.nationalData.CategoryId);
        lowerCI = CsvDataHelper.getDisplayValue(indicatorRow.nationalData.LoCI);
        upperCI = CsvDataHelper.getDisplayValue(indicatorRow.nationalData.UpCI);
        lowerCI99_8 = CsvDataHelper.getDisplayValue(indicatorRow.nationalData.LoCI99_8);
        upperCI99_8 = CsvDataHelper.getDisplayValue(indicatorRow.nationalData.UpCI99_8);
        count = CsvDataHelper.getDisplayValue(indicatorRow.nationalData.Count);
        denominator = CsvDataHelper.getDisplayValue(indicatorRow.nationalData.Denom);
        value = CsvDataHelper.getDisplayValue(indicatorRow.nationalData.Val);

        parentCode = '';
        parentName = '';
        areaCode = AreaCodes.England;
        areaName = 'England';
        areaType = 'England';
        break;

      case CsvCoreDataType.Subnational:
        categoryType = CsvDataHelper.getDisplayValue(indicatorRow.subnationalData.CategoryTypeId);
        category = CsvDataHelper.getDisplayValue(indicatorRow.subnationalData.CategoryId);
        lowerCI = CsvDataHelper.getDisplayValue(indicatorRow.subnationalData.LoCI);
        upperCI = CsvDataHelper.getDisplayValue(indicatorRow.subnationalData.UpCI);
        lowerCI99_8 = CsvDataHelper.getDisplayValue(indicatorRow.subnationalData.LoCI99_8);
        upperCI99_8 = CsvDataHelper.getDisplayValue(indicatorRow.subnationalData.UpCI99_8);
        count = CsvDataHelper.getDisplayValue(indicatorRow.subnationalData.Count);
        denominator = CsvDataHelper.getDisplayValue(indicatorRow.subnationalData.Denom);
        value = CsvDataHelper.getDisplayValue(indicatorRow.subnationalData.Val);

        parentCode = AreaCodes.England;
        parentName = 'England';
        areaCode = parentAreaHelper.getParentAreaCode();
        areaName = parentAreaHelper.getParentAreaNameForCSV();
        areaType = parentAreaTypeHelper.getParentAreaTypeNameForCSV();
        break;

      case CsvCoreDataType.Area:
        if (isDefined(indicatorRow.areaData)) {
          categoryType = CsvDataHelper.getDisplayValue(indicatorRow.areaData.CategoryTypeId);
          category = CsvDataHelper.getDisplayValue(indicatorRow.areaData.CategoryId);
          lowerCI = CsvDataHelper.getDisplayValue(indicatorRow.areaData.LoCI);
          upperCI = CsvDataHelper.getDisplayValue(indicatorRow.areaData.UpCI);
          lowerCI99_8 = CsvDataHelper.getDisplayValue(indicatorRow.areaData.LoCI99_8);
          upperCI99_8 = CsvDataHelper.getDisplayValue(indicatorRow.areaData.UpCI99_8);
          count = CsvDataHelper.getDisplayValue(indicatorRow.areaData.Count);
          denominator = CsvDataHelper.getDisplayValue(indicatorRow.areaData.Denom);
          value = CsvDataHelper.getDisplayValue(indicatorRow.areaData.Val);
        }

        parentCode = parentAreaHelper.getParentAreaCode();
        parentName = parentAreaHelper.getParentAreaNameForCSV();
        areaCode = this.model.areaCode;
        areaName = this.ftHelperService.getArea(areaCode).Name;
        areaType = this.ftHelperService.getAreaTypeName();
        break;
    }

    data.parentCode = parentCode;
    data.parentName = parentName;
    data.areaCode = areaCode;
    data.areaName = areaName;
    data.areaType = areaType;

    const root = indicatorRow.groupRoot;
    data.sex = root.Sex.Name;
    data.age = root.Age.Name;

    data.categoryType = categoryType;
    data.category = category;
    data.timePeriod = indicatorRow.period;
    data.value = value;
    data.lowerCiLimit95 = lowerCI;
    data.upperCiLimit95 = upperCI;
    data.lowerCiLimit99_8 = lowerCI99_8;
    data.upperCiLimit99_8 = upperCI99_8;
    data.count = count;
    data.denominator = denominator;

    data.valueNote = this.getValueNoteForCsv(rowType, indicatorRow);

    data.recentTrend = '';
    if (isDefined(indicatorRow.trendMarkerResult)) {
      data.recentTrend = new TrendMarkerLabelProvider(root.PolarityId).getLabel(indicatorRow.trendMarkerResult.Marker);
    }

    data.comparedToEnglandValueOrPercentiles = CsvDataHelper.getSignificanceValue(indicatorRow.areaData,
      root.PolarityId, ComparatorIds.National, root.ComparatorMethodId);

    data.comparedToRegionValueOrPercentiles = CsvDataHelper.getSignificanceValue(indicatorRow.areaData,
      root.PolarityId, ComparatorIds.SubNational, root.ComparatorMethodId);

    data.timePeriodSortable = new TimePeriod(root.Grouping[0]).getSortableNumber();

    const hasDataChanged = this.ftHelperService.hasDataChanged(root);
    const isNewData = NewDataBadgeHelper.isNewData(hasDataChanged);
    data.newData = isNewData ? 'New data' : '';

    data.comparedToGoal = CsvDataHelper.getSignificanceValue(indicatorRow.areaData,
      root.PolarityId, ComparatorIds.Target, root.ComparatorMethodId);

    return data;
  }

  getValueNoteForCsv(rowType: number, row: IndicatorRow): string {
    let valueNote = '';

    switch (rowType) {
      case 0:
        if (isDefined(row.englandValueTooltip)) {
          valueNote = row.englandValueTooltip;
        }
        break;

      case 1:
        if (isDefined(row.subnationalValueTooltip)) {
          valueNote = row.subnationalValueTooltip;
        }
        break;

      case 2:
        if (isDefined(row.areaValueTooltip)) {
          valueNote = row.areaValueTooltip;
        }
        break;
    }

    return valueNote;
  }

  public showRecentTrendTooltip(event: MouseEvent, row: IndicatorRow) {
    const tooltipProvider = this.ftHelperService.newRecentTrendsTooltip();
    this.currentTrendsTooltipHtml = tooltipProvider.getTooltipByData(row.trendMarkerResult);
    this.tooltipHelper.displayHtml(event, this.currentTrendsTooltipHtml);
  }

  public hideTooltip() {
    this.tooltipHelper.hide();
    this.currentTrendsTooltipHtml = null;
  }

  public repositionTooltip(event: MouseEvent) {
    this.tooltipHelper.reposition(event);
  }
}

export class IndicatorRow {
  // Displayed properties
  period: string;
  recentTrendImage: string;
  areaCount: string;
  areaValue: string;
  subnationalValue: string;
  englandValue: string;
  englandMin: string;
  englandMax: string;
  minTooltip: string;
  maxTooltip: string;
  areaValueTooltip: string;
  subnationalValueTooltip: string;
  englandValueTooltip: string;
  spineChartDimensions: SpineChartDimensions;

  // Piggy backing objects
  formatter: IndicatorFormatter;
  groupRoot: GroupRoot;
  indicatorMetadata: IndicatorMetadata;
  areaData: CoreDataSet;
  benchmarkData: CoreDataSet;
  stats: IndicatorStatsPercentiles = null;
  statsF: IndicatorStatsPercentilesFormatted = null;
  haveRequiredValuesForSpineChart = false;
  comparisonConfig: ComparisonConfig;
  rootIndex: number;
  trendMarkerResult: TrendMarkerResult;
  area: Area;
  nationalData: CoreDataSet;
  subnationalData: CoreDataSet;
}

export class SpineChartHeaderLabeller {

  public spineChartMinMaxLabelId: number;
  public maxHeader: string;
  public minHeader: string;

  constructor(private groupRoots: GroupRoot[]) {
    this.getLabels();
  }

  getLabels() {

    const anyLowestHighest = this.isPolarity(PolarityIds.NotApplicable) || this.isPolarity(PolarityIds.BlueOrangeBlue);

    const anyWorstBest = this.isPolarity(PolarityIds.RAGHighIsGood) || this.isPolarity(PolarityIds.RAGLowIsGood);

    if (anyWorstBest && anyLowestHighest) {
      // Worst / Lowest -> Best / Highest
      this.minHeader = SpineChartMinMaxLabelDescription.WorstLowest;
      this.maxHeader = SpineChartMinMaxLabelDescription.BestHighest;
      this.spineChartMinMaxLabelId = SpineChartMinMaxLabel.WorstLowestAndBestHighest;
    } else if (anyWorstBest) {
      // Worst  -> Best
      this.minHeader = SpineChartMinMaxLabelDescription.Worst;
      this.maxHeader = SpineChartMinMaxLabelDescription.Best;
      this.spineChartMinMaxLabelId = SpineChartMinMaxLabel.WorstAndBest;
    } else {
      // Lowest ->  Highest
      this.minHeader = SpineChartMinMaxLabelDescription.Lowest;
      this.maxHeader = SpineChartMinMaxLabelDescription.Highest;
      this.spineChartMinMaxLabelId = SpineChartMinMaxLabel.LowestAndHighest;
    }
  }

  isPolarity(id: number): boolean {
    return this.groupRoots.findIndex(x => x.PolarityId === id) > -1;
  }
}