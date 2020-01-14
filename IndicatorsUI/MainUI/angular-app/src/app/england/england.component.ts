import { Component, HostListener } from '@angular/core';
import { isDefined } from '@angular/compiler/src/util';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { TooltipHelper } from '../shared/shared';
import { AreaCodes, PageType, Tabs, ComparatorIds } from '../shared/constants';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { DownloadService } from '../shared/service/api/download.service';
import { IndicatorMetadata, GroupRoot, CoreDataSetInfo, TrendMarkerResult, Sex, Age, CoreDataSet, Grouping } from '../typings/FT';
import { LegendConfig } from '../shared/component/legend/legend';
import { CsvConfig, CsvData, CsvDataHelper } from '../shared/component/export-csv/export-csv';
import { TrendMarkerLabelProvider } from '../shared/classes/trendmarker-label-provider';
import { TimePeriod } from '../shared/classes/time-period';

@Component({
  selector: 'ft-england',
  templateUrl: './england.component.html',
  styleUrls: ['./england.component.css']
})
export class EnglandComponent {

  showRecentTrends = false;
  legendConfig: LegendConfig;
  csvConfig: CsvConfig;

  public isChangeFromPreviousPeriodShown = false;
  public rows: Array<EnglandRow>;
  public hasRecentTrends = false;

  private tooltip: TooltipHelper;

  constructor(private ftHelperService: FTHelperService,
    private indicatorService: IndicatorService,
    private downloadService: DownloadService) { }

  @HostListener('window:EnglandSelected', ['$event'])
  public onOutsideEvent(event) {

    const ftHelper = this.ftHelperService;

    this.setConfig();
    this.tooltip = new TooltipHelper(this.ftHelperService.newTooltipManager());

    const groupingSubheadings = ftHelper.getGroupingSubheadings();
    const metadataHash = ftHelper.getMetadataHash();
    const groupRoots = ftHelper.getAllGroupRoots();

    this.rows = [];
    // tslint:disable-next-line: forin
    for (const rootIndex in groupRoots) {
      const root = groupRoots[rootIndex];
      const indicatorId = root.IID;
      const metadata: IndicatorMetadata = metadataHash[indicatorId];
      const unit = !!metadata ? metadata.Unit : null;

      const subheadings = groupingSubheadings.filter(x => x.Sequence === root.Sequence);
      if (isDefined(subheadings)) {
        subheadings.forEach(subheading => {
          const englandRow: EnglandRow = new EnglandRow();
          this.rows.push(englandRow);

          englandRow.indicatorName = subheading.Subheading;
          englandRow.isSubheading = true;
        });
      }

      const row: EnglandRow = new EnglandRow();
      this.rows.push(row);

      row.rootIndex = rootIndex;
      row.period = root.Grouping[0].Period;
      row.indicatorId = indicatorId;
      row.indicatorName = metadata.Descriptive['Name'] + ftHelper.getSexAndAgeLabel(root);
      row.hasNewData = root.DateChanges && root.DateChanges.HasDataChangedRecently;

      // Data
      const englandData = ftHelper.getNationalComparatorGrouping(root).ComparatorData;
      const dataInfo = ftHelper.newCoreDataSetInfo(englandData);
      row.value = ftHelper.newValueDisplayer(unit).byDataInfo(dataInfo);
      row.count = ftHelper.formatCount(dataInfo);
      row.hasValueNote = dataInfo.isNote();
      row.noteId = englandData.NoteId;
      row.sex = root.Sex;
      row.age = root.Age;
      row.data = englandData;
      row.polarityId = root.PolarityId;
      row.comparatorMethodId = root.ComparatorMethodId;
      row.grouping = root.Grouping[0];

      // Recent trend
      if (!!root.RecentTrends) {
        this.setUpRecentTrendOnRow(row, root, englandData.AreaCode, dataInfo);
      }

      row.isSubheading = false;
    }

    this.legendConfig = new LegendConfig(PageType.England, this.ftHelperService);
    this.showRecentTrends = ftHelper.getFTConfig().hasRecentTrends;

    ftHelper.showAndHidePageElements();
    ftHelper.unlock();

    // Enable tooltip
    setTimeout(() => {
      (<any>$('[data-toggle="tooltip"]')).tooltip();
    }, 0);
  }

  public showValueNoteTooltip(event: MouseEvent, row: EnglandRow) {
    if (row.hasValueNote) {
      const tooltipProvider = this.ftHelperService.newValueNoteTooltipProvider();
      this.tooltip.displayHtml(event, tooltipProvider.getHtmlFromNoteId(row.noteId));
    }
  }

  public showRecentTrendTooltip(event: MouseEvent, row: EnglandRow) {
    const tooltipProvider = this.ftHelperService.newRecentTrendsTooltip();
    this.tooltip.displayHtml(event, tooltipProvider.getTooltipByData(row.recentTrend));
  }

  public positionTooltip(event: MouseEvent) {
    this.tooltip.reposition(event);
  }

  public recentTrendClicked(row: EnglandRow) {
    this.ftHelperService.recentTrendSelected().byGroupRoot(row.rootIndex)
  }

  public hideTooltip() {
    this.tooltip.hide();
  }

  public indicatorNameClicked(row: EnglandRow) {
    this.ftHelperService.goToMetadataPage(row.rootIndex);
  }

  private setUpRecentTrendOnRow(row: EnglandRow, root: GroupRoot, areaCode: string, dataInfo: CoreDataSetInfo) {

    const ftHelper = this.ftHelperService;
    const polarityId = root.PolarityId;

    if (dataInfo.isValue() && root.RecentTrends[areaCode]) {
      // Recent trend available
      const recentTrend: TrendMarkerResult = root.RecentTrends[areaCode];
      row.recentTrend = recentTrend;
      row.recentTrendHtml = ftHelper.getTrendMarkerImage(recentTrend.Marker, polarityId);
      row.changeFromPreviousHtml = ftHelper.getTrendMarkerImage(
        recentTrend.MarkerForMostRecentValueComparedWithPreviousValue, polarityId);
    } else {
      // No trend image
      row.recentTrendHtml = ftHelper.getTrendMarkerImage(0/*TrendMarker.CannotBeCalculated*/, polarityId);
      row.changeFromPreviousHtml = row.recentTrendHtml;
    }
  }

  private setConfig() {
    const config = this.ftHelperService.getFTConfig();
    this.hasRecentTrends = config.hasRecentTrends;
    this.isChangeFromPreviousPeriodShown = config.isChangeFromPreviousPeriodShown;
  }

  onExportClick(event: MouseEvent) {
    event.preventDefault();

    // Prepare view
    const $trendInfoIcon = $('.trend-info').hide();
    const trendLegend = $('<div id="trend-legend">'
      + $('#trend-marker-legend').html() +
      '</div>');
    const table = $('#england-table');
    table.prepend(trendLegend);

    // Export image
    this.ftHelperService.saveElementAsImage(table, 'england');

    // Restore view
    $('#trend-legend').remove();
    $trendInfoIcon.show();

    // Log event for analytics
    this.ftHelperService.logEvent('ExportImage', 'England');
  }

  onExportCsvFileClick(event: MouseEvent) {
    const csvData: CsvData[] = [];

    this.rows.forEach(row => {
      const data = this.addCsvRow(row);
      csvData.push(data);
    });

    this.csvConfig = new CsvConfig();
    this.csvConfig.tab = Tabs.England;
    this.csvConfig.csvData = csvData;
  }

  addCsvRow(row: EnglandRow): CsvData {
    const data = new CsvData();

    data.indicatorId = row.indicatorId.toString();
    data.indicatorName = row.indicatorName;
    data.parentCode = '';
    data.parentName = '';
    data.areaCode = AreaCodes.England;
    data.areaName = 'England';
    data.areaType = 'England';
    data.sex = row.sex.Name;
    data.age = row.age.Name;

    data.categoryType = CsvDataHelper.getDisplayValue(row.data.CategoryTypeId);
    data.category = CsvDataHelper.getDisplayValue(row.data.CategoryId);

    data.timePeriod = row.period;
    data.value = CsvDataHelper.getDisplayValue(row.data.Val);
    data.lowerCiLimit95 = CsvDataHelper.getDisplayValue(row.data.LoCI);
    data.upperCiLimit95 = CsvDataHelper.getDisplayValue(row.data.UpCI);
    data.lowerCiLimit99_8 = CsvDataHelper.getDisplayValue(row.data.LoCI99_8);
    data.upperCiLimit99_8 = CsvDataHelper.getDisplayValue(row.data.UpCI99_8);
    data.count = CsvDataHelper.getDisplayValue(row.data.Count);
    data.denominator = CsvDataHelper.getDisplayValue(row.data.Denom);

    data.valueNote = '';
    if (isDefined(row.noteId)) {
      data.valueNote = this.ftHelperService.newValueNoteTooltipProvider().getTextFromNoteId(row.noteId);
    }

    data.recentTrend = '';
    if (isDefined(row.recentTrend)) {
      data.recentTrend = new TrendMarkerLabelProvider(row.polarityId).getLabel(row.recentTrend.Marker);
    }

    data.comparedToEnglandValueOrPercentiles = CsvDataHelper.getSignificanceValue(row.data,
      row.polarityId, ComparatorIds.National, row.comparatorMethodId);

    data.comparedToRegionValueOrPercentiles = CsvDataHelper.getSignificanceValue(row.data,
      row.polarityId, ComparatorIds.SubNational, row.comparatorMethodId);

    data.timePeriodSortable = new TimePeriod(row.grouping).getSortableNumber();

    data.newData = row.hasNewData ? 'New data' : '';
    data.comparedToGoal = '';

    return data;
  }
}

class EnglandRow {
  count: string;
  indicatorId: number;
  indicatorName: string;
  hasNewData: boolean;
  hasValueNote: boolean;
  noteId: number;
  period: string;
  rootIndex: string;
  value: string;
  recentTrend: TrendMarkerResult;
  recentTrendHtml: string;
  /** Change from the previous time point */
  changeFromPreviousHtml: string;
  isSubheading: boolean;
  data: CoreDataSet;
  sex: Sex;
  age: Age;
  polarityId: number;
  comparatorMethodId: number;
  grouping: Grouping;
}
