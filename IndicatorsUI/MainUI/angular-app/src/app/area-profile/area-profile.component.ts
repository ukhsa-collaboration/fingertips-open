import { Component, HostListener } from '@angular/core';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import {
  IndicatorStats, FTModel, FTConfig, IndicatorMetadata, IndicatorFormatter,
  GroupRoot, CoreDataSet, IndicatorStatsPercentilesFormatted, IndicatorStatsPercentiles, ComparisonConfig, TrendMarkerResult, Area
} from '../typings/FT';
import { Observable } from 'rxjs/Observable';
import { UIService } from '../shared/service/helper/ui.service';
import {
  ParentDisplay, AreaTypeIds, ComparatorIds, GroupRootHelper, CoreDataListHelper,
  ValueTypeIds, AreaHelper, PolarityIds, TrendMarkerValue, TooltipHelper, ComparatorMethodIds,
  SpineChartMinMaxLabelDescription, SpineChartMinMaxLabel, ParameterBuilder
} from '../shared/shared';
import * as _ from 'underscore';
import { SpineChartCalculator, SpineChartDimensions } from './spine-chart.classes';
import { PageType } from 'app/shared/component/legend/legend.component';

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
  public isParentUk = false;
  public isNationalAndRegional = false;
  public isNotNN = false;
  public trendColSpan = 0;
  public colSpan = 0;
  public indicatorRows: IndicatorRow[];
  public isAnyData: boolean = false;

  private model: FTModel;
  private config: FTConfig;
  private imgUrl: string;
  private tooltip: TooltipHelper;

  private scrollTop: number;
  private isAreaIgnored: boolean;
  private currentTooltipHtml: string;
  private area: Area;

  // Data from AJAX
  private indicatorStats: IndicatorStats[];
  private metadataHash: Map<number, IndicatorMetadata>;

  // HTML
  private NoData = '<div class="no-data">-</div>';

  // Legend display properties
  pageType = PageType.None;
  showRAG3 = false;
  showRAG5 = false;
  showBOB = false;
  showQuintilesRAG = false;
  showQuintilesBOB = false;
  showRecentTrends = false;

  constructor(private ftHelperService: FTHelperService, private indicatorService: IndicatorService,
    private uiService: UIService) { }

  @HostListener('window:AreaProfileSelected', ['$event'])
  public onOutsideEvent(event) {
    let ftHelper = this.ftHelperService;

    const groupRoots = ftHelper.getAllGroupRoots();
    this.model = ftHelper.getFTModel();
    this.config = ftHelper.getFTConfig();
    this.isAnyData = groupRoots.length > 0;

    this.scrollTop = this.uiService.getScrollTop();

    let comparatorId = ftHelper.getComparatorId();
    let parentAreaCode = ftHelper.getComparatorById(comparatorId).Code;

    this.setDisplayConfig();
    this.setIsAreaIgnored();

    // AJAX calls
    let indicatorStatisticsObservable = this.indicatorService.getIndicatorStatistics(this.model.areaTypeId,
      parentAreaCode, this.model.profileId, this.model.groupId);

    Observable.forkJoin([indicatorStatisticsObservable]).subscribe(results => {

      this.indicatorStats = _.values(results[0]);
      this.metadataHash = ftHelper.getMetadataHash();

      this.setAreaProfileHtml();

      // Unlock UI
      ftHelper.showAndHidePageElements();
      ftHelper.showDataQualityLegend();
      ftHelper.showTargetBenchmarkOption(groupRoots);
      this.uiService.setScrollTop(this.scrollTop);

      // Legend display configurations
      this.pageType = PageType.AreaProfiles;
      this.showRecentTrends = this.areRecentTrendsDisplayed;

      ftHelper.unlock();
    });

    this.tooltip = new TooltipHelper(this.ftHelperService.newTooltipManager());
  }

  setAreaProfileHtml() {

    let ftHelper = this.ftHelperService;

    // Benchmark
    let comparatorId = ftHelper.getComparatorId();
    let isNationalBenchmark = (comparatorId === ComparatorIds.National);
    let benchmark = isNationalBenchmark ?
      ftHelper.getNationalComparator() :
      ftHelper.getParentArea();

    let areaCode = this.model.areaCode;
    this.area = ftHelper.getArea(areaCode);

    this.uiService.toggleQuintileLegend($('#quintile-key'), false);

    let isSubnationalColumn = !this.model.isNearestNeighbours() && ftHelper.isSubnationalColumn();

    let indicatorRows = [];
    let rootIndex = 0;
    let groupRoots = ftHelper.getAllGroupRoots();

    for (let root of groupRoots) {

      let metadata = this.metadataHash[root.IID];
      let areaData = new CoreDataListHelper(root.Data).findByAreaCode(areaCode);

      // Create indicator row
      let row = new IndicatorRow();
      row.rootIndex = rootIndex;
      indicatorRows.push(row);
      rootIndex += 1;

      row.comparisonConfig = ftHelper.newComparisonConfig(root, metadata);
      this.assignStatsToRow(root, row);

      // Parent data
      let subnationalData = ftHelper.getRegionalComparatorGrouping(root).ComparatorData;
      let nationalData = ftHelper.getNationalComparatorGrouping(root).ComparatorData;
      let benchmarkData = isNationalBenchmark ?
        nationalData :
        subnationalData;
      row.benchmarkData = benchmarkData;

      // Init formatter
      let formatter = ftHelper.newIndicatorFormatter(root, metadata, areaData, row.statsF);
      formatter.averageData = benchmarkData;

      // Value displayer
      var unit = !!metadata ? metadata.Unit : null;
      var valueDisplayer = ftHelper.newValueDisplayer(unit);

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
      var dataInfo = ftHelper.newCoreDataSetInfo(row.areaData);
      if (dataInfo.isNote()) {
        row.areaValueTooltip = ftHelper.getValueNoteById(areaData.NoteId).Text;
      }

      // England value
      let nationalDataInfo = ftHelper.newCoreDataSetInfo(nationalData);
      row.englandValue = valueDisplayer.byDataInfo(nationalDataInfo, { noCommas: 'y' })
      if (nationalDataInfo.isNote()) {
        row.englandValueTooltip = ftHelper.getValueNoteById(nationalData.NoteId).Text;
      }

      // Subnational value
      if (isSubnationalColumn) {
        let subnationalDataInfo = ftHelper.newCoreDataSetInfo(subnationalData);
        row.subnationalValue = valueDisplayer.byDataInfo(subnationalDataInfo, { noCommas: 'y' });
        if (subnationalDataInfo.isNote()) {
          row.subnationalValueTooltip = ftHelper.getValueNoteById(subnationalData.NoteId).Text;
        }
      }

      this.setSpineChartDimensions(row);

      // Min / Max
      let dimensions = row.spineChartDimensions
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
          var recentTrends = root.RecentTrends[areaData.AreaCode];
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
    this.showRAG3 = false;
    this.showRAG5 = false;
    this.showBOB = false;
    this.showQuintilesRAG = false;
    this.showQuintilesBOB = false;

    // Show Quintile BOB legend
    if (groupRoots.findIndex(x => x.ComparatorMethodId === ComparatorMethodIds.Quintiles && x.PolarityId === PolarityIds.NotApplicable) > -1) {
      this.showQuintilesBOB = true;
    }

    // Show Quintile RAG legend
    if (groupRoots.findIndex(x => x.ComparatorMethodId === ComparatorMethodIds.Quintiles && (x.PolarityId === PolarityIds.RAGLowIsGood || x.PolarityId === PolarityIds.RAGHighIsGood)) > -1) {
      this.showQuintilesRAG = true;
    }

    // Show RAG5 legend
    if (groupRoots.findIndex(x => x.ComparatorMethodId === ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels) > -1) {
      this.showRAG5 = true;
    } else {
      // Show RAG3 legend
      if (groupRoots.findIndex(x => x.ComparatorMethodId === ComparatorMethodIds.SingleOverlappingCIsForOneCiLevel) > -1) {
        this.showRAG3 = true;
      }

      if (this.showRAG3 === false &&
        (groupRoots.findIndex(x => x.PolarityId === PolarityIds.RAGHighIsGood) > -1 ||
          groupRoots.findIndex(x => x.PolarityId === PolarityIds.RAGLowIsGood) > -1)) {

        this.showRAG3 = true;
      }
    }

    // Show BOB legend
    if (groupRoots.findIndex(x => x.PolarityId === PolarityIds.BlueOrangeBlue) > -1) {
      this.showBOB = true;
    }
  }

  polarityToText(polarity, isForLowest) {

    if (polarity === PolarityIds.RAGLowIsGood || polarity === PolarityIds.RAGHighIsGood) {
      return isForLowest ? 'Worst' : 'Best';
    }
    return isForLowest ? 'Lowest' : 'Highest';
  }

  assignStatsToRow(root: GroupRoot, row: IndicatorRow) {

    let statsBase: IndicatorStats =
      new GroupRootHelper(root).findMatchingItemBySexAgeAndIndicatorId(this.indicatorStats);

    if (statsBase) {
      row.statsF = statsBase.StatsF;
      row.stats = statsBase.Stats;
      row.haveRequiredValuesForSpineChart = statsBase.HaveRequiredValues;
    }
  }

  setSpineChartDimensions(row: IndicatorRow): void {

    let ftHelper = this.ftHelperService;
    var benchmarkDataInfo = ftHelper.newCoreDataSetInfo(row.benchmarkData);

    let isValidBenchmarkValue = benchmarkDataInfo.isValue();

    if (isValidBenchmarkValue && !row.haveRequiredValuesForSpineChart) {
      // Should show not enough values message
      let dimensions = new SpineChartDimensions();
      dimensions.isSufficientData = false;
      row.spineChartDimensions = dimensions;
    } else if (!row.stats ||
      !isValidBenchmarkValue ||
      row.indicatorMetadata.ValueType.Id === ValueTypeIds.Count) {
      // Should show no data message
      let dimensions = new SpineChartDimensions();
      dimensions.isAnyData = false;
      row.spineChartDimensions = dimensions;
    } else {
      // Have data can show spine chart
      var polarityId = row.groupRoot.PolarityId;
      let spineChart = new SpineChartCalculator();
      var proportions = spineChart.getSpineProportions(row.benchmarkData.Val, row.stats, polarityId);

      var dataInfo = ftHelper.newCoreDataSetInfo(row.areaData);
      var spineDimensions = spineChart.getDimensions(proportions, dataInfo, this.imgUrl, row.comparisonConfig,
        row.indicatorMetadata.IID, ftHelper.getMarkerImageFromSignificance);

      row.spineChartDimensions = spineDimensions;
    }
  }

  public getIndicatorNameHtml(row: IndicatorRow): string {

    let formatter = row.formatter;
    let ftHelper = this.ftHelperService;
    let root = row.groupRoot;
    let metadata = row.indicatorMetadata;

    // Indicator name & data quality
    let html = [
      formatter.getIndicatorName(),
      this.ftHelperService.getIndicatorDataQualityHtml(formatter.getDataQuality())
    ];

    // New data badge
    if (ftHelper.hasDataChanged(root)) {
      html.push('&nbsp;<span style="margin-right:8px;" class="badge badge-success">New data</span>');
    }

    // Target legend
    var targetLegend = ftHelper.getTargetLegendHtml(row.comparisonConfig, metadata);
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
    let ftHelper = this.ftHelperService;

    let urls = ftHelper.getURL();
    this.imgUrl = urls.img;

    let groupRoots = ftHelper.getAllGroupRoots();

    this.isParentUk = ftHelper.isParentUk();
    this.isNationalAndRegional = ftHelper.getEnumParentDisplay() === ParentDisplay.NationalAndRegional &&
      this.model.parentTypeId !== AreaTypeIds.Country;

    let trendMarkerFound = groupRoots && groupRoots[0] && groupRoots[0].RecentTrends;

    this.trendColSpan = trendMarkerFound ? 3 : 2;
    this.areRecentTrendsDisplayed = trendMarkerFound && this.config.hasRecentTrends;

    this.isNotNN = !this.model.isNearestNeighbours()
    this.colSpan = this.isNationalAndRegional && !this.model.isNearestNeighbours() ? 3 : 4;

    this.parentType = ftHelper.getParentTypeName();

    this.setSpineHeadersAndChartLabelImage(urls.img, groupRoots);

    this.lowest = this.config.spineHeaders.min;
    this.highest = this.config.spineHeaders.max;
  }

  setSpineHeadersAndChartLabelImage(imageUrl: string, groupRoots: GroupRoot[]) {
    if (groupRoots.findIndex(x => x.ComparatorMethodId === ComparatorMethodIds.SingleOverlappingCIsForOneCiLevel) > -1 ||
      groupRoots.findIndex(x => x.ComparatorMethodId === ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels) > -1) {

      this.config.spineHeaders.min = SpineChartMinMaxLabelDescription.Worst;
      this.config.spineHeaders.max = SpineChartMinMaxLabelDescription.Best;
      this.config.spineChartMinMaxLabelId = SpineChartMinMaxLabel.WorstAndBest;
    } else {

      if (groupRoots.findIndex(x => x.PolarityId === PolarityIds.RAGHighIsGood) > -1 ||
        groupRoots.findIndex(x => x.PolarityId === PolarityIds.RAGLowIsGood) > -1) {

        this.config.spineHeaders.min = SpineChartMinMaxLabelDescription.Worst;
        this.config.spineHeaders.max = SpineChartMinMaxLabelDescription.Best;
        this.config.spineChartMinMaxLabelId = SpineChartMinMaxLabel.WorstAndBest;
      }
    }

    if (groupRoots.findIndex(x => x.PolarityId === PolarityIds.BlueOrangeBlue) > -1) {
      if (this.config.spineChartMinMaxLabelId === SpineChartMinMaxLabel.WorstAndBest) {

        this.config.spineHeaders.min = SpineChartMinMaxLabelDescription.WorstLowest;
        this.config.spineHeaders.max = SpineChartMinMaxLabelDescription.BestHighest;
        this.config.spineChartMinMaxLabelId = SpineChartMinMaxLabel.WorstLowestAndBestHighest;
      } else {

        this.config.spineHeaders.min = SpineChartMinMaxLabelDescription.Lowest;
        this.config.spineHeaders.max = SpineChartMinMaxLabelDescription.Highest;
        this.config.spineChartMinMaxLabelId = SpineChartMinMaxLabel.LowestAndHighest;
      }
    }

    this.spineChartImageUrl = imageUrl + 'spine-key-label-id-' + this.config.spineChartMinMaxLabelId + '.png';
  }

  /** Whether the current area should is ignored for the spine charts */
  setIsAreaIgnored(): void {

    let ignoredSpineChartAreas = this.config.ignoredSpineChartAreas;

    // Is area too small for spine charts
    var isIgnored = false;
    if (ignoredSpineChartAreas) {
      let areaCode = this.model.areaCode;

      let areas = ignoredSpineChartAreas.split(',');
      isIgnored = _.any(areas, function (area) {
        return area === areaCode;
      });
    }

    this.isAreaIgnored = isIgnored;
  }

  goToBarChart(row: IndicatorRow) {
    this.ftHelperService.goToBarChartPage(row.rootIndex);
  }

  goToTrends(row: IndicatorRow) {
    this.ftHelperService.goToAreaTrendsPage(row.rootIndex);
  }

  onExportClick(event: MouseEvent) {
    this.ftHelperService.exportTableAsImage('area-profile-container',
      'AreaProfilesTable', '#key-spine-chart,#spine-range-key');
  }

  onExportCsvFileClick(event: MouseEvent) {

    var urls = this.ftHelperService.getURL();
    var model = this.ftHelperService.getFTModel();

    var parameters = new ParameterBuilder()
    .add('parent_area_type_id', model.parentTypeId)
    .add('child_area_type_id', model.areaTypeId)
    .add('group_id', model.groupId)
    .add('areas_code', model.areaCode)
    .add('parent_area_code', model.parentCode);
  
    var url = urls.corews + 'api/latest/no_inequalities_data/csv/by_group_id?' + parameters.build();
    window.open(url.toLowerCase(), '_blank');
  }

  public showIndicatorTooltip(event: MouseEvent, row: IndicatorRow) {
    this.currentTooltipHtml = this.ftHelperService.getIndicatorNameTooltip(row.rootIndex, this.area);
    this.tooltip.displayHtml(event, this.currentTooltipHtml);
  }

  public showRecentTrendTooltip(event: MouseEvent, row: IndicatorRow) {
    let tooltipProvider = this.ftHelperService.newRecentTrendsTooltip();
    this.currentTooltipHtml = tooltipProvider.getTooltipByData(row.trendMarkerResult);
    this.tooltip.displayHtml(event, this.currentTooltipHtml);
  }

  public hideTooltip() {
    this.tooltip.hide();
    this.currentTooltipHtml = null;
  }

  public repositionTooltip(event: MouseEvent) {
    this.tooltip.reposition(event);
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
  haveRequiredValuesForSpineChart: boolean = false;
  comparisonConfig: ComparisonConfig;
  rootIndex: number;
  trendMarkerResult: TrendMarkerResult;
  area: Area;
}
