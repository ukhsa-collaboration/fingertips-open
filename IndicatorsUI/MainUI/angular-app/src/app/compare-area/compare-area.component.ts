import { Component, HostListener, ChangeDetectorRef } from '@angular/core';
import * as _ from 'underscore';
import { forkJoin } from 'rxjs';
import { isDefined } from '@angular/compiler/src/util';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import {
  AreaHelper, CommaNumber, Colour, TooltipHelper, NewDataBadgeHelper, ParentAreaHelper,
  AreaTypeHelper, Sorter, SexAndAgeLabelHelper, CategoryTypeHelper, CategoryHelper,
  ParentAreaTypeHelper, CoreDataSetHelper
} from '../shared/shared';
import {
  ComparatorIds, AreaTypeIds, ComparatorMethodIds, TrendMarkerValue, PageType,
  Tabs, ProfileIds, GroupIds, CsvCoreDataType
} from '../shared/constants';
import { AreaCodes } from '../shared/constants';
import { IndicatorService } from '../shared/service/api/indicator.service';
import {
  AreaRow, BarScale, FunnelPlotChartData, TrendMarkerArea, SortBy, AreaPopulation,
  MinMaxFinder, Limit, DisplayOption, SpcForDsrLimits, BarChartOptions, SortState
} from './compare-area';
import { DownloadService } from '../shared/service/api/download.service';
import { LegendConfig } from '../shared/component/legend/legend';
import {
  FTModel, CoreDataSet, FTData, GroupRoot, Grouping, Area, CoreDataSetInfo, IndicatorMetadata,
  ComparisonConfig, TrendMarkerResult
} from '../typings/FT';
import { TrendMarkerLabelProvider } from '../shared/classes/trendmarker-label-provider';
import { TimePeriod } from '../shared/classes/time-period';
import { CsvConfig, CsvData, CsvDataHelper } from '../shared/component/export-csv/export-csv';

/* Options that determine how many child areas to display */
export class AreasOptions {
  public static readonly AllInParent = 0;
  public static readonly AllInEngland = 1;
}

@Component({
  selector: 'ft-compare-area',
  templateUrl: './compare-area.component.html',
  styleUrls: ['./compare-area.component.css']
})

export class CompareAreaComponent {

  public isAnyData = false;
  public areaTypeId: number;
  public displayOption: DisplayOption = DisplayOption.Table;
  public tooltipHelper: TooltipHelper;
  public comparatorId: number;
  public trendSource = '';
  public comparatorAreaRows: AreaRow[] = [];
  public nonComparatorAreaRows: AreaRow[] = [];
  public selectedIndicatorPeriod: string;
  public selectedIndicatorUnit: string;
  public showFunnelPlot: boolean;
  public chartData: FunnelPlotChartData;
  public nationalButtonClass: string;
  public subNationalButtonClass: string;
  public isEnglandAreaTypeSelected: boolean;
  public sortBy: SortBy = SortBy.Value;
  public displayChart = false;
  public csvConfig: CsvConfig;

  private model: FTModel;
  private data: FTData;
  private areas: Area[] = [];
  private indicatorId: number;
  private indicatorName: string;
  private indicatorIndex: number;
  private indicatorMetadata: IndicatorMetadata;
  private groupRoot: GroupRoot;
  private metadata: IndicatorMetadata;
  private currentComparator: any;
  private areaCodeSelectedOnChart: string;
  private areasOption = AreasOptions.AllInParent;
  private nationalGrouping: Grouping;
  private subnationalGrouping: Grouping;
  private trendMarkerAreas: TrendMarkerArea[] = [];
  private areaPopulations: AreaPopulation[] = [];
  private minMaxFinder: MinMaxFinder;
  private limits: Limit[] = [];
  private comparatorLineData: number[][] = [];
  private u2LineData: number[][] = [];
  private u3LineData: number[][] = [];
  private l2LineData: number[][] = [];
  private l3LineData: number[][] = [];
  private areaDisplayed: number;
  private nationalCoreDataSet: CoreDataSet[] = [];
  private currentData: CoreDataSet[] = [];
  private parentAreaCode: string;
  private formattedParentAreaName: string;
  private tableButtonClass = 'button-selected';
  private tableAndChartButtonClass: string;
  private selectedIndicatorName: string;
  private isNewData: boolean;
  private htmlAdhocKey = '';
  private showRecentTrends = false;
  private legendConfig: LegendConfig;
  private sortState = new SortState();

  constructor(private ftHelperService: FTHelperService,
    private indicatorService: IndicatorService,
    private downloadService: DownloadService,
    private ref: ChangeDetectorRef) { }

  @HostListener('window:CompareAreaSelected', [
    '$event',
    '$event.detail.isEnglandAreaType',
    '$event.detail.rootIndex',
    '$event.detail.triggeredExternally'
  ])
  public onOutsideEvent(event, isEnglandAreaType, rootIndex, triggeredExternally) {
    const ftHelper = this.ftHelperService;
    this.isAnyData = true;

    this.isEnglandAreaTypeSelected = isEnglandAreaType;
    if (isEnglandAreaType) {
      this.ftHelperService.showAndHidePageElements();
      this.ftHelperService.unlock();
    } else {
      this.model = ftHelper.getFTModel();
      this.data = ftHelper.getFTData();

      this.areaTypeId = this.model.areaTypeId;

      // Set root index if passed in
      if (isDefined(rootIndex) && rootIndex !== null) {
        this.ftHelperService.setIndicatorIndex(rootIndex);
      }
      this.indicatorIndex = this.ftHelperService.getIndicatorIndex();

      // Set component reference variables
      this.groupRoot = this.model.groupRoots[this.indicatorIndex];
      const metadataHash = ftHelper.getMetadataHash();
      this.metadata = metadataHash[this.groupRoot.IID];

      this.nationalGrouping = this.getNationalComparatorGrouping();
      this.subnationalGrouping = this.getSubnationalComparatorGrouping();

      // Apply the trend header
      this.applyTrendHeader();

      this.loadData();
    }
  }

  @HostListener('window:NoDataDisplayed', ['$event', '$event.detail.isEnglandAreaType'])
  public refreshVariable(isEnglandAreaType) {
    this.isAnyData = false;
    this.isEnglandAreaTypeSelected = isEnglandAreaType;
  }

  loadData() {
    // Reset the data used to generate table and chart
    this.resetData();

    this.comparatorId = this.ftHelperService.getComparatorId();
    this.currentComparator = this.ftHelperService.getCurrentComparator();

    // Only parent data can be compared at a subnational level
    if (this.comparatorId === ComparatorIds.SubNational) {
      this.areasOption = AreasOptions.AllInParent;
    }

    // Areas option label for parent area
    if (this.model.isNearestNeighbours()) {
      const area = this.ftHelperService.getArea(this.model.areaCode);
      this.parentAreaCode = this.model.nearestNeighbour;
      const nnAreaName = new AreaHelper(area).getShortAreaNameToDisplay();
      this.formattedParentAreaName = nnAreaName + ' and neighbours';
    } else {
      const parentAreaHelper = new ParentAreaHelper(this.ftHelperService);
      this.parentAreaCode = parentAreaHelper.getParentAreaCode();
      const parentAreaName = parentAreaHelper.getParentAreaName();
      this.formattedParentAreaName = 'All in ' + parentAreaName;
    }

    const root = this.groupRoot;
    let groupId = root.Grouping[0].GroupId;
    if (this.model.profileId === ProfileIds.SearchResults) {
      groupId = GroupIds.Search;
    }

    // Get single indicator data for all areas
    const areasObservable = this.indicatorService.getSingleIndicatorForAllAreas(groupId,
      this.model.areaTypeId, this.currentComparator.Code, this.model.profileId, this.comparatorId, root.IID,
      root.Sex.Id, root.Age.Id);

    // Get recent trends
    const recentTrendsObservable = this.indicatorService.getRecentTrends(this.currentComparator.Code,
      groupId, this.model.areaTypeId, root.IID, root.Sex.Id,
      root.Age.Id);

    forkJoin([areasObservable, recentTrendsObservable]).subscribe(results => {
      this.nationalCoreDataSet = <CoreDataSet[]>results[0];
      this.trendMarkerAreas = <TrendMarkerArea[]>results[1];

      if (isDefined(this.nationalCoreDataSet)) {
        const key = this.ftHelperService.getIndicatorKey(root, this.model, AreaCodes.England);
        this.ftHelperService.setAreaValues(key, this.nationalCoreDataSet);

        const trendMarkerKey = this.getTrendMarkerKey();
        this.ftHelperService.setTrendMarkers(trendMarkerKey, this.trendMarkerAreas);
      }

      this.applyTabButtonStyles();
      this.displayTableAndChart();

      this.legendConfig = new LegendConfig(PageType.CompareAreas, this.ftHelperService);
      this.canDisplayBenchmarkAgainstGoalLegend();
      this.setLegendDisplay();

      this.ftHelperService.showAndHidePageElements();
      this.ftHelperService.showTargetBenchmarkOption(this.ftHelperService.getAllGroupRoots());

      this.ftHelperService.setIndicatorIndex(this.indicatorIndex);

      this.ftHelperService.unlock();
    });

    this.tooltipHelper = new TooltipHelper(this.ftHelperService.newTooltipManager());
  }

  resetData() {
    this.l2LineData.length = 0;
    this.u2LineData.length = 0;
    this.l3LineData.length = 0;
    this.u3LineData.length = 0;
    this.comparatorLineData.length = 0;
    this.areaPopulations.length = 0;
    this.limits.length = 0;
  }

  // Display legend
  setLegendDisplay() {
    this.legendConfig.configureForOneIndicator(this.groupRoot);
    this.showRecentTrends = this.ftHelperService.getFTConfig().hasRecentTrends;
  }

  canDisplayBenchmarkAgainstGoalLegend(): void {
    const currentGrpRoot: GroupRoot = this.groupRoot;
    const metadata: IndicatorMetadata = this.metadata;

    const comparisonConfig = this.ftHelperService.newComparisonConfig(
      currentGrpRoot,
      metadata
    );

    if (comparisonConfig) {
      if (comparisonConfig.useTarget) {
        const targetLegend = this.ftHelperService.getTargetLegendHtml(
          comparisonConfig, metadata);

        this.legendConfig.configureBenchmarkAgainstGoal(true, targetLegend);
      } else {
        this.legendConfig.configureBenchmarkAgainstGoal(false, '');
      }
    }
  }

  generateCurrentData() {
    // Reset current data
    this.currentData.length = 0;

    let coreDataSets: CoreDataSet[] = [];

    if (this.areasOption === AreasOptions.AllInParent) {
      this.areas = this.data.sortedAreas;
      coreDataSets = this.groupRoot.Data;
    } else {
      this.areas = this.ftHelperService.getAreaList();
      coreDataSets = this.nationalCoreDataSet;
    }

    // Assign current data
    this.currentData = this.getDataSortedByArea(coreDataSets, this.areas);

    // Remove the undefined rows
    for (let i = 0; i < this.currentData.length; i++) {
      if (this.currentData[i] === undefined) {
        this.currentData.splice(i, 1);
      }
    }
  }

  displayTableAndChart() {
    // Generate current data
    this.generateCurrentData();

    // Display table
    this.displayIndicatorTable();

    const grouping = this.getComparatorGrouping(this.comparatorId);

    if (this.canFunnelPlotBeDisplayed(grouping.ComparatorMethodId)) {
      this.displayFunnelPlot(grouping);
    } else {
      this.showFunnelPlot = false;
    }

    this.ref.detectChanges();
  }

  canFunnelPlotBeDisplayed(comparatorMethodId: number): boolean {
    return comparatorMethodId === ComparatorMethodIds.SpcForProportions ||
      comparatorMethodId === ComparatorMethodIds.SpcForDsr;
  }

  applyTabButtonStyles() {
    if (this.areasOption === AreasOptions.AllInParent) {
      this.nationalButtonClass = '';
      this.subNationalButtonClass = 'button-selected';
    } else {
      this.nationalButtonClass = 'button-selected';
      this.subNationalButtonClass = '';
    }
  }

  showAllAreasInParent() {
    this.areasOption = AreasOptions.AllInParent;
    this.loadData();
  }

  showAllAreasInEngland() {
    this.areasOption = AreasOptions.AllInEngland;
    this.ftHelperService.setComparatorId(ComparatorIds.National);
  }

  isAllInEnglandHidden(): boolean {
    return new AreaTypeHelper(this.ftHelperService).isSmallAreaType() || this.ftHelperService.isParentCountry();
  }

  applyTrendHeader() {
    const groupRoot = this.groupRoot;
    const grouping = groupRoot.Grouping[0];
    const hasDataChanged = this.ftHelperService.hasDataChanged(groupRoot);
    const metadata = this.metadata;
    const valueType = metadata.ValueType.Name;

    let period = '';
    if (isDefined(this.groupRoot.Grouping[0])) {
      period = grouping.Period;
    }

    let unitLabel = '';
    if (metadata.Unit.Label !== '') {
      unitLabel = metadata.Unit.Label;
    }

    const sexAndAgeLabel = new SexAndAgeLabelHelper(groupRoot).getSexAndAgeLabel();
    this.selectedIndicatorName = metadata.Descriptive['Name'] + sexAndAgeLabel;
    this.selectedIndicatorPeriod = period;
    this.isNewData = NewDataBadgeHelper.isNewData(hasDataChanged);
    this.selectedIndicatorUnit = valueType + ' - ' + unitLabel;
  }

  goToMetadataPage() {
    this.ftHelperService.goToMetadataPage(this.indicatorIndex);
  }

  getTrendMarkerKey() {
    const root = this.groupRoot;

    return 'parent_area_code=' + this.currentComparator.Code +
      '&group_id=' + root.Grouping[0].GroupId +
      '&area_type_id=' + this.model.areaTypeId +
      '&indicator_id=' + root.IID +
      '&sex_id=' + root.Sex.Id +
      '&age_id=' + root.Age.Id;
  }

  displayData(displayId: DisplayOption) {
    this.displayOption = displayId;
    switch (displayId) {
      case DisplayOption.Table:
        this.displayChart = false;
        this.tableButtonClass = 'button-selected';
        this.tableAndChartButtonClass = '';
        break;
      case DisplayOption.TableAndChart:
        this.displayChart = true;
        this.tableButtonClass = '';
        this.tableAndChartButtonClass = 'button-selected';
        break;
    }
  }

  updateAreaCode(areaCode: string) {
    this.areaCodeSelectedOnChart = areaCode;
  }

  updateSortBy(sortBy: SortBy) {
    this.sortState.reverseOrderIfSameSort(sortBy, this.sortBy);

    this.sortBy = sortBy;
    this.sort(sortBy);
  }

  sort(sortBy: SortBy) {
    let tempAreaRows;
    let noDataAreaRows;
    if (sortBy === SortBy.Area) {
      // Text sort
      tempAreaRows = this.nonComparatorAreaRows.slice(0);
      noDataAreaRows = [];
    } else {
      // Numeric sort
      tempAreaRows = _.filter(this.nonComparatorAreaRows, function (row) { return row.isValue; });
      noDataAreaRows = _.filter(this.nonComparatorAreaRows, function (row) { return !row.isValue; });
    }

    // Sort rows
    tempAreaRows.sort((a, b) => {
      switch (sortBy) {
        case SortBy.Area:
          return Sorter.sortStrings(a.areaName, b.areaName);
        case SortBy.Rank:
          return Sorter.sortNumbers(a.rankSortIndex, b.rankSortIndex);
        case SortBy.Count:
          return Sorter.sortNumbers(a.data.Count, b.data.Count);
        case SortBy.Value:
          return Sorter.sortNumbers(a.data.Val, b.data.Val);
      }
    });

    // Refine sort order
    if (!this.sortState.isOrderAscending(this.sortBy)) {
      tempAreaRows.reverse();
    }

    // Display rows with no data after the others
    this.nonComparatorAreaRows = tempAreaRows.concat(noDataAreaRows);
  }

  displayFunnelPlot(grouping: Grouping) {
    const comparatorData = grouping.ComparatorData;
    const isComparatorValue = isDefined(comparatorData) && comparatorData.Val !== -1;
    const isDsr = grouping.ComparatorMethodId === ComparatorMethodIds.SpcForDsr;

    // Generate current data
    this.generateCurrentData();

    const comparisonConfig = this.ftHelperService.newComparisonConfig(this.groupRoot, this.metadata);
    const valueArrays = this.divideValuesBySignificance(comparisonConfig, isDsr);

    if (isComparatorValue) {
      const values: number[] = [];

      this.areaPopulations.forEach(areaPopulation => {
        values.push(areaPopulation.population);
      });

      this.minMaxFinder = new MinMaxFinder(values);

      const max = this.minMaxFinder.max;
      const min = this.minMaxFinder.min;
      const value = comparatorData.Val;

      const comparatorMethodId = grouping.ComparatorMethodId;
      if (this.canFunnelPlotBeDisplayed(comparatorMethodId)) {
        // Display funnel plot
        if (comparatorMethodId === ComparatorMethodIds.SpcForProportions) {
          this.getSpcForProportionsLimits(value, min, max, valueArrays, grouping, isDsr);
        } else if (comparatorMethodId === ComparatorMethodIds.SpcForDsr) {
          if (min > 0 && max > 0 && value > 0) {
            this.getSpcForDsrLimits(value, min, max, this.groupRoot.YearRange, valueArrays, comparatorData, grouping, isDsr);
          } else {
            this.showFunnelPlot = false;
          }
        }
      } else {
        // Funnel plot cannot be displayed
        this.showFunnelPlot = false;
      }
    }
  }

  getSpcForProportionsLimits(comparatorValue: number, denominatorMin: number, denominatorMax: number,
    valueArrays: any, grouping: Grouping, isDsr: boolean) {
    const unitValue = this.metadata.Unit.Value;
    const step = (denominatorMax - denominatorMin) / 25;

    let N1 = denominatorMin;
    const P0 = comparatorValue / unitValue;

    // Comparator Method 5
    const controlLimit2 = 1.96; // 95%
    const controlLimit3 = 3.0902; // 99.8%

    const cl2Power2 = Math.pow(controlLimit2, 2);
    const cl3Power2 = Math.pow(controlLimit3, 2);

    this.limits.length = 0;

    while (N1 < (denominatorMax + step)) {
      const E1 = N1 * P0;
      const C2A = (2 * E1) + cl2Power2;
      const C3A = (2 * E1) + cl3Power2;
      const Bs1 = (1 - (E1 / N1));
      const Bs2 = 4 * E1 * Bs1;
      const C2Bs3 = cl2Power2 + Bs2;
      const C3Bs3 = cl3Power2 + Bs2;
      const C2B = controlLimit2 * Math.sqrt(C2Bs3);
      const C3B = controlLimit3 * Math.sqrt(C3Bs3);
      const C2C = 2 * (N1 + cl2Power2);
      const C3C = 2 * (N1 + cl3Power2);

      const L2 = ((C2A - C2B) / C2C) * unitValue;
      const U2 = ((C2A + C2B) / C2C) * unitValue;
      const L3 = ((C3A - C3B) / C3C) * unitValue;
      const U3 = ((C3A + C3B) / C3C) * unitValue;

      const limit = new Limit();
      limit.x = N1;
      limit.L2 = L2;
      limit.U2 = U2;
      limit.L3 = L3;
      limit.U3 = U3;
      this.limits.push(limit);

      N1 += step;
    }

    if (this.limits.length > 0) {
      this.setFunnelPlotLimitsSeriesData(comparatorValue, denominatorMin, denominatorMax);

      const comparisonConfig = this.ftHelperService.newComparisonConfig(this.groupRoot, this.metadata);
      this.applyFunnelPlotData(comparisonConfig, valueArrays, grouping, isDsr);

      // Show funnel plot
      this.showFunnelPlot = true;
    } else {
      // Hide funnel plot
      this.showFunnelPlot = false;
    }
  }

  getSpcForDsrLimits(comparatorValue: number, populationMin: number, populationMax: number, yearRange: number,
    valueArrays: any, comparatorData: CoreDataSet, grouping: Grouping, isDsr: boolean) {
    const metadata = this.metadata;
    const unitValue = metadata.Unit.Value;
    const comparisonConfig = this.ftHelperService.newComparisonConfig(this.groupRoot, metadata);

    this.indicatorService.getSpcForDsrLimits(comparatorValue, populationMin, populationMax, unitValue, yearRange)
      .subscribe((result: SpcForDsrLimits) => {
        const spcForDsrLimits = <SpcForDsrLimits>result;

        if (comparatorValue !== null && Math.abs(comparatorValue - spcForDsrLimits.ComparatorValue) < 0.00000001) {
          this.limits = spcForDsrLimits.Points;

          if (this.limits.length > 0) {
            this.setFunnelPlotLimitsSeriesData(comparatorValue, populationMin, populationMax);
            this.applyFunnelPlotData(comparisonConfig, valueArrays, grouping, isDsr);

            // Show funnel plot
            this.showFunnelPlot = true;
          } else {
            // Hide funnel plot
            this.showFunnelPlot = false;
          }
        }
      });
  }

  applyFunnelPlotData(comparisonConfig: ComparisonConfig, valueArrays: any, grouping: Grouping, isDsr: boolean) {
    const comparatorData = grouping.ComparatorData;
    const isComparatorValue = isDefined(comparatorData) && comparatorData.Val !== -1;
    const showLimits = (grouping.ComparatorMethodId === ComparatorMethodIds.SpcForProportions || isDsr) && isComparatorValue;
    const metadata = this.metadata;

    // Determine funnel plot colours
    let colourWorse: string;
    let colourBetter: string;
    if (comparisonConfig.useRagColours) {
      colourWorse = Colour.ragWorse;
      colourBetter = Colour.ragBetter;
    } else {
      colourWorse = Colour.bobLower;
      colourBetter = Colour.bobHigher;
    }

    this.chartData = null;

    this.chartData = new FunnelPlotChartData();
    this.chartData.better = valueArrays.better;
    this.chartData.worse = valueArrays.worse;
    this.chartData.same = valueArrays.same;
    this.chartData.none = valueArrays.none;
    this.chartData.colourBetter = colourBetter;
    this.chartData.colourWorse = colourWorse;
    this.chartData.currentComparatorName = this.ftHelperService.getCurrentComparator().Name;
    this.chartData.isComparatorValue = isComparatorValue;
    this.chartData.isDsr = isDsr;
    this.chartData.showLimits = showLimits;
    this.chartData.unit = metadata.Unit;
    this.chartData.currentData = this.currentData;
    this.chartData.areas = this.areas;
    this.chartData.areaPopulations = this.areaPopulations;
    this.chartData.comparatorLineData = this.comparatorLineData;
    this.chartData.l2LineData = this.l2LineData;
    this.chartData.u2LineData = this.u2LineData;
    this.chartData.l3LineData = this.l3LineData;
    this.chartData.u3LineData = this.u3LineData;
    this.chartData.indicatorName = this.selectedIndicatorName;
    this.chartData.timePeriod = grouping.Period;
    this.chartData.areaTypeName = this.ftHelperService.getAreaTypeName();
    this.chartData.parentAreaName = this.ftHelperService.getParentArea().Short;
  }

  setFunnelPlotLimitsSeriesData(value: number, min: number, max: number) {
    let comparatorMax: number;
    let x: number;

    if (this.limits.length > 0) {
      this.limits.forEach(limit => {
        x = limit.x;
        this.l2LineData.push([x, limit.L2]);
        this.u2LineData.push([x, limit.U2]);
        this.l3LineData.push([x, limit.L3]);
        this.u3LineData.push([x, limit.U3]);
      });

      comparatorMax = x;
    } else {
      comparatorMax = max;
    }

    this.comparatorLineData.push([min, value], [comparatorMax, value]);
  }

  divideValuesBySignificance(comparisonConfig: ComparisonConfig, isDsr: boolean) {
    const comparatorId = comparisonConfig.comparatorId;
    const data = this.currentData;

    const worse: any[] = [];
    const same: any[] = [];
    const better: any[] = [];
    const none: any[] = [];

    // Reset area populations array
    this.areaPopulations.length = 0;

    // tslint:disable-next-line: forin
    for (const i in data) {

      const dataInfo = this.ftHelperService.newCoreDataSetInfo(data[i]);

      if (dataInfo.isValue()) {
        let population;
        if (isDsr) {
          population = data[i].Denom2 / this.groupRoot.YearRange;
        } else {
          population = data[i].Denom;
        }

        if (population > 0) {
          const significance = Number(data[i].Sig[comparatorId]);

          const areaCode = data[i].AreaCode;
          const areaName = this.ftHelperService.getArea(areaCode).Name;

          switch (significance) {
            case 3:
              better.push({
                x: population,
                y: data[i].Val,
                data: data[i]
              });
              break;

            case 2:
              same.push({
                x: population,
                y: data[i].Val,
                data: data[i]
              });
              break;

            case 1:
              worse.push({
                x: population,
                y: data[i].Val,
                data: data[i]
              });
              break;

            default:
              none.push({
                x: population,
                y: data[i].Val,
                data: data[i]
              });
              break;
          }

          // Add to population
          const areaPopulation = new AreaPopulation();
          areaPopulation.areaCode = areaCode;
          areaPopulation.population = population;
          this.areaPopulations.push(areaPopulation);
        }
      }
    }

    return { worse: worse, same: same, better: better, none: none };
  }

  getDataSortedByArea(coreDataSet: CoreDataSet[], areas: Area[]): CoreDataSet[] {
    const sortedCoreDataSet: CoreDataSet[] = [];

    areas.forEach(area => {
      const data = this.getDataFromAreaCode(coreDataSet, area.Code);
      sortedCoreDataSet.push(data);
    });

    return sortedCoreDataSet;
  }

  getDataFromAreaCode(coreData: CoreDataSet[], areaCode: string): CoreDataSet {
    let data = coreData.find(x => x.AreaCode === areaCode);

    if (data === undefined) {
      data = CoreDataSetHelper.getDummyCoreDataSetWithAreaCode(areaCode);
    }

    return data;
  }

  getNationalComparatorGrouping(): Grouping {
    return this.getComparatorGrouping(ComparatorIds.National);
  }

  getSubnationalComparatorGrouping() {
    return this.getComparatorGrouping(ComparatorIds.SubNational);
  }

  getComparatorGrouping(comparatorId: number): Grouping {

    const groupRoot = this.groupRoot;
    const groupings = groupRoot.Grouping;
    const count = groupings.length;

    for (let i = 0; i < count; i++) {
      if (groupings[i].ComparatorId === comparatorId) {
        return groupings[i];
      }
    }

    for (let i = 0; i < count; i++) {
      if (groupings[i].ComparatorId === -1) {
        return groupings[i]
      }
    }

    // It has not found any data for the comparator Id
    return null;
  }

  displayIndicatorTable() {

    const groupRoot = this.groupRoot

    // Determine size of bars
    // Include national and subnational comparator data
    this.updateCurrentDataToIncludeNationalAndSubnationalComparatorData();
    const barScale = new BarScale(this.currentData, this.ftHelperService);
    barScale.calculateRangeAndPixels();

    // Re-initialise the array
    this.comparatorAreaRows.length = 0;
    this.nonComparatorAreaRows.length = 0;

    const metadata = this.metadata;
    const comparisonConfig = this.ftHelperService.newComparisonConfig(groupRoot, metadata);
    this.trendSource = 'Source: ' + metadata.Descriptive['DataSource'];

    // National row
    if (this.nationalGrouping !== null) {
      const nationalAreaRow = this.addNationalAreaRow(comparisonConfig, barScale);
      this.comparatorAreaRows.push(nationalAreaRow);
    }


    if (this.subnationalGrouping !== null &&
      // Do not include subnational comparator if All in England selected
      this.areasOption !== AreasOptions.AllInEngland &&
      // Do not show England twice
      this.model.parentTypeId !== AreaTypeIds.Country) {
      const subNationalAreaRow = this.addSubNationalAreaRow(comparisonConfig, barScale);
      this.comparatorAreaRows.push(subNationalAreaRow);
    }

    let counter = 0;
    this.areas.forEach(area => {

      const areaData = this.currentData.find(x => x.AreaCode === area.Code)

      if (isDefined(areaData)) {
        const areaNameToDisplay = this.getAreaName(area);

        const areaRow = this.getAreaRow(areaData, area.Code, areaNameToDisplay, counter,
          this.comparatorId, comparisonConfig, barScale);

        this.nonComparatorAreaRows.push(areaRow);
      }

      counter++;
    });

    this.sort(this.sortBy);
  }

  updateCurrentDataToIncludeNationalAndSubnationalComparatorData() {
    if (this.nationalGrouping !== null) {
      this.currentData.push(this.nationalGrouping.ComparatorData);
    }
    if (this.subnationalGrouping !== null) {
      this.currentData.push(this.subnationalGrouping.ComparatorData);
    }
  }

  addNationalAreaRow(comparisonConfig: ComparisonConfig, barScale: BarScale): AreaRow {
    const comparatorId = ComparatorIds.National;
    const area = this.ftHelperService.getComparatorById(comparatorId);
    const areaNameToDisplay = this.getAreaName(area);

    return this.getAreaRow(this.nationalGrouping.ComparatorData, area.Code, areaNameToDisplay, -1,
      comparatorId, comparisonConfig, barScale);
  }

  addSubNationalAreaRow(comparisonConfig: ComparisonConfig, barScale: BarScale): AreaRow {
    const comparatorId = ComparatorIds.SubNational;
    const area = this.ftHelperService.getComparatorById(comparatorId);
    let areaNameToDisplay = '';
    if (this.model.isNearestNeighbours()) {
      areaNameToDisplay = 'Neighbours average';
    } else {
      areaNameToDisplay = this.getAreaName(area);
    }

    return this.getAreaRow(this.subnationalGrouping.ComparatorData, area.Code, areaNameToDisplay, -1,
      comparatorId, comparisonConfig, barScale);
  }

  getAreaRow(data: CoreDataSet, areaCode: string, areaName: string, dataIndex: number, comparatorId: number,
    comparisonConfig: ComparisonConfig, barScale: BarScale): AreaRow {

    const dataInfo = this.ftHelperService.newCoreDataSetInfo(data);
    const valueDisplayer = this.ftHelperService.newValueDisplayer(null);
    let count = '-';
    let valF = '-';
    let loCIF = '-';
    let upCIF = '-';
    let isValue = false;
    let valueNoteId = 0;
    let trendMarkerTooltip: string;
    let recentTrends: TrendMarkerResult;
    let recentTrendImage = '';

    const sigLevel = this.groupRoot.Grouping[0].SigLevel;

    if (dataInfo.isDefined()) {
      count = this.formatCount(dataInfo);
      valF = valueDisplayer.byDataInfo(dataInfo);
      loCIF = sigLevel === 99.8 ? valueDisplayer.byNumberString(data.LoCI99_8F) : valueDisplayer.byNumberString(data.LoCIF);
      upCIF = sigLevel === 99.8 ? valueDisplayer.byNumberString(data.UpCI99_8F) : valueDisplayer.byNumberString(data.UpCIF);
      isValue = dataInfo.isValue();
      valueNoteId = dataInfo.getNoteId();
    }

    // Recent trend
    const groupRoot = this.groupRoot;
    if (isDefined(groupRoot) && isDefined(groupRoot.RecentTrends)) {
      let trendMarkerValue;
      const tooltipProvider = this.ftHelperService.newRecentTrendsTooltip();

      if (groupRoot.RecentTrends[areaCode]) {
        recentTrends = groupRoot.RecentTrends[areaCode];
        trendMarkerValue = recentTrends.Marker;
        trendMarkerTooltip = tooltipProvider.getTooltipByData(recentTrends);
      } else if (this.trendMarkerAreas[areaCode]) {
        trendMarkerValue = this.trendMarkerAreas[areaCode].Marker;
        trendMarkerTooltip = tooltipProvider.getTooltipByData(this.trendMarkerAreas[areaCode]);
      } else {
        trendMarkerValue = TrendMarkerValue.CannotCalculate;
        trendMarkerTooltip = '';
      }

      recentTrendImage = this.ftHelperService.getTrendMarkerImage(trendMarkerValue, groupRoot.PolarityId);
    }

    // Rank - nearest neighbours
    const rank = this.getRankValue(areaCode);
    let rankSortIndex = dataIndex;
    if (dataIndex !== -1) {
      if (rank === '-') {
        rankSortIndex = 1000;
      } else {
        rankSortIndex = Number(rank);
      }
    }

    // Define view model
    const areaRow = new AreaRow();
    areaRow.data = data;
    areaRow.areaName = areaName;
    areaRow.areaCode = areaCode;
    areaRow.comparatorId = comparatorId;
    areaRow.trendMarkerResult = recentTrends;
    areaRow.trendMarkerTooltip = trendMarkerTooltip;
    areaRow.recentTrendImage = recentTrendImage;
    areaRow.count = count;
    areaRow.rank = rank;
    areaRow.rankSortIndex = rankSortIndex;
    areaRow.value = valF;
    areaRow.loCI = loCIF;
    areaRow.upCI = upCIF;
    areaRow.isValue = isValue;
    areaRow.valueNoteId = valueNoteId;

    // Assign bar parameters
    const barChartOptions = barScale.getBarChartOptions(data, comparisonConfig);
    this.assignBarChartProperties(areaRow, barChartOptions, isValue);

    return areaRow;
  }

  assignBarChartProperties(areaRow: AreaRow, barChartOptions: BarChartOptions, isValue: boolean) {

    // Only show bar if there is a value
    areaRow.barImageWidth = isValue ? barChartOptions.barImageWidth : 0;

    areaRow.barImage = barChartOptions.barImage;
    areaRow.barImageLeft = barChartOptions.barImageLeft;
    areaRow.horizontalLineImage = barChartOptions.horizontalLineImage;
    areaRow.horizontalLineImageWidth = barChartOptions.horizontalLineImageWidth;
    areaRow.horizontalLineImageLeft = barChartOptions.horizontalLineImageLeft;
    areaRow.verticalLineImage1 = barChartOptions.verticalLineImage1;
    areaRow.verticalLineImage1Left = barChartOptions.verticalLineImage1Left;
    areaRow.verticalLineImage2 = barChartOptions.verticalLineImage2;
    areaRow.verticalLineImage2Left = barChartOptions.verticalLineImage2Left;
  }

  formatCount(dataInfo: CoreDataSetInfo): string {
    return dataInfo.isCount() ? new CommaNumber(dataInfo.data.Count).rounded() : '-';
  }

  getAreaName(area: Area): string {
    const areaHelper = new AreaHelper(area);

    if (area.AreaTypeId === AreaTypeIds.AcuteTrust) {
      return areaHelper.getShortAreaNameToDisplay();
    }

    return areaHelper.getAreaNameToDisplay();
  }

  getRankValue(areaCode: string): string {
    let rankValue = '-';
    if (this.model.isNearestNeighbours()) {
      switch (areaCode) {
        case AreaCodes.England:
        case this.model.areaCode:
          rankValue = '-';
          break;
        default:
          const areaIndex = this.data.sortedAreas.findIndex(x => x.Code === areaCode);
          if (areaIndex !== -1) {
            rankValue = this.data.sortedAreas.find(x => x.Code === areaCode).Rank;
          } else {
            rankValue = '-';
          }

          break;
      }
    }

    return rankValue;
  }

  exportTable(event: MouseEvent) {
    this.ftHelperService.exportTableAsImage('indicator-details-table',
      'CompareAreasTable', '');
  }

  exportTableAsCsv(event: MouseEvent) {
    const csvData: CsvData[] = [];

    this.indicatorId = this.ftHelperService.getIid();
    this.indicatorName = this.ftHelperService.getIndicatorName(this.indicatorId);

    // National data row
    const nationalData = this.addCsvRow(this.comparatorAreaRows[0], CsvCoreDataType.National);
    csvData.push(nationalData);

    // Subnational data row
    if (this.areasOption === AreasOptions.AllInParent) {
      const subnationalData = this.addCsvRow(this.comparatorAreaRows[1], CsvCoreDataType.Subnational);
      csvData.push(subnationalData);
    }

    // All other rows
    this.nonComparatorAreaRows.forEach(areaRow => {
      const data = this.addCsvRow(areaRow, CsvCoreDataType.Area);
      csvData.push(data);
    });

    this.csvConfig = new CsvConfig();
    this.csvConfig.tab = Tabs.CompareAreas;
    this.csvConfig.csvData = csvData;
  }

  addCsvRow(areaRow: AreaRow, rowType: number): CsvData {
    const data = new CsvData();
    const root = this.groupRoot;

    data.indicatorId = this.indicatorId.toString();
    data.indicatorName = this.indicatorName;

    const parentAreaHelper = new ParentAreaHelper(this.ftHelperService);
    const parentAreaTypeHelper = new ParentAreaTypeHelper(this.ftHelperService);

    switch (rowType) {
      case CsvCoreDataType.National:
        data.parentCode = '';
        data.parentName = '';
        data.areaCode = AreaCodes.England;
        data.areaName = 'England';
        data.areaType = 'England';
        break;

      case CsvCoreDataType.Subnational:
        data.parentCode = AreaCodes.England;
        data.parentName = 'England';
        data.areaCode = parentAreaHelper.getParentAreaCode();
        data.areaName = parentAreaHelper.getParentAreaNameForCSV();
        data.areaType = parentAreaTypeHelper.getParentAreaTypeNameForCSV();
        break;

      case CsvCoreDataType.Area:
        if (this.areasOption === AreasOptions.AllInEngland) {
          data.parentCode = AreaCodes.England;
          data.parentName = 'England';
        } else {
          data.parentCode = parentAreaHelper.getParentAreaCode();
          data.parentName = parentAreaHelper.getParentAreaNameForCSV();
        }

        data.areaCode = areaRow.areaCode;
        data.areaName = areaRow.areaName;
        data.areaType = this.ftHelperService.getAreaTypeName();
        break;
    }

    data.sex = root.Sex.Name;
    data.age = root.Age.Name;
    data.category = CategoryHelper.getCategoryName(areaRow.data.CategoryId);
    data.categoryType = CategoryTypeHelper.getCategoryTypeName(areaRow.data.CategoryTypeId);
    data.timePeriod = root.Grouping[0].Period;
    data.value = CsvDataHelper.getDisplayValue(areaRow.data.Val);
    data.lowerCiLimit95 = CsvDataHelper.getDisplayValue(areaRow.data.LoCI);
    data.upperCiLimit95 = CsvDataHelper.getDisplayValue(areaRow.data.UpCI);
    data.lowerCiLimit99_8 = CsvDataHelper.getDisplayValue(areaRow.data.LoCI99_8);
    data.upperCiLimit99_8 = CsvDataHelper.getDisplayValue(areaRow.data.UpCI99_8);
    data.count = areaRow.count;
    data.denominator = CsvDataHelper.getDisplayValue(areaRow.data.Denom);

    data.valueNote = '';
    if (isDefined(areaRow.valueNoteId)) {
      data.valueNote = this.ftHelperService.newValueNoteTooltipProvider().getTextFromNoteId(areaRow.valueNoteId);
    }

    data.recentTrend = '';
    if (isDefined(areaRow.trendMarkerResult)) {
      data.recentTrend = new TrendMarkerLabelProvider(root.PolarityId).getLabel(areaRow.trendMarkerResult.Marker);
    }

    data.comparedToEnglandValueOrPercentiles = CsvDataHelper.getSignificanceValue(areaRow.data,
      root.PolarityId, ComparatorIds.National, root.ComparatorMethodId);

    data.comparedToRegionValueOrPercentiles = CsvDataHelper.getSignificanceValue(areaRow.data,
      root.PolarityId, ComparatorIds.SubNational, root.ComparatorMethodId);

    data.timePeriodSortable = new TimePeriod(root.Grouping[0]).getSortableNumber();

    data.newData = this.isNewData ? 'New data' : '';

    data.comparedToGoal = CsvDataHelper.getSignificanceValue(areaRow.data, root.PolarityId, ComparatorIds.Target, root.ComparatorMethodId);

    return data;
  }

  exportChart(event: MouseEvent) {
    this.ftHelperService.saveElementAsImage($('#funnel-plot-chart-box'), 'Chart');
  }
}
