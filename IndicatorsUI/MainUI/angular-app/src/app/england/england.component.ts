import { Component, HostListener } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import {
  FTModel, FTRoot, GroupRoot, IndicatorMetadata, CoreDataSetInfo,
  GroupingSubheading, TrendMarkerResult, TrendMarker, TooltipManager
} from '../typings/FT.d';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { TooltipHelper, ParameterBuilder, AreaCodes } from '../shared/shared';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { DatePipe } from '@angular/common';
import { LegendType, KeyType } from 'app/shared/component/legend/legend.component';

@Component({
  selector: 'ft-england',
  templateUrl: './england.component.html',
  styleUrls: ['./england.component.css']
})
export class EnglandComponent {

  keyType: KeyType = KeyType.None;
  legendType: LegendType = LegendType.None;
  showRecentTrends: Boolean = true;

  public isChangeFromPreviousPeriodShown = false;
  public rows: Array<EnglandRow>;
  public hasRecentTrends = false;
  private tooltip: TooltipHelper;

  constructor(private ftHelperService: FTHelperService, private indicatorService: IndicatorService) { }

  @HostListener('window:EnglandSelected', ['$event'])
  public onOutsideEvent(event) {

    let ftHelper = this.ftHelperService;

    this.setConfig();
    this.tooltip = new TooltipHelper(this.ftHelperService.newTooltipManager());

    let groupingSubheadings = ftHelper.getGroupingSubheadings();
    let metadataHash = ftHelper.getMetadataHash();
    let groupRoots = ftHelper.getAllGroupRoots();

    this.rows = [];
    for (let rootIndex in groupRoots) {
      let root = groupRoots[rootIndex];
      let indicatorId = root.IID;
      let metadata: IndicatorMetadata = metadataHash[indicatorId];
      let unit = !!metadata ? metadata.Unit : null;

      let subheadings = groupingSubheadings.filter(x => x.Sequence === root.Sequence);
      if (subheadings !== undefined) {
        subheadings.forEach(subheading => {
          let row: EnglandRow = new EnglandRow();
          this.rows.push(row);

          row.indicatorName = subheading.Subheading;
          row.isSubheading = true;
        });
      }

      let row: EnglandRow = new EnglandRow();
      this.rows.push(row);

      row.rootIndex = rootIndex;
      row.period = root.Grouping[0].Period;
      row.indicatorName = metadata.Descriptive['Name'] + ftHelper.getSexAndAgeLabel(root);
      row.hasNewData = root.DateChanges && root.DateChanges.HasDataChangedRecently;

      // Data
      let englandData = ftHelper.getNationalComparatorGrouping(root).ComparatorData;
      let dataInfo = ftHelper.newCoreDataSetInfo(englandData);
      row.value = ftHelper.newValueDisplayer(unit).byDataInfo(dataInfo);
      row.count = ftHelper.formatCount(dataInfo);
      row.hasValueNote = dataInfo.isNote();
      row.noteId = englandData.NoteId;

      // Recent trend
      if (!!root.RecentTrends) {
        this.setUpRecentTrendOnRow(row, root, englandData.AreaCode, dataInfo);
      }

      row.isSubheading = false;
    }

    ftHelper.showAndHidePageElements();
    ftHelper.unlock();

    // Enable tooltip
    setTimeout(() => {
      (<any>$('[data-toggle="tooltip"]')).tooltip();
    }, 0);
  }

  public showValueNoteTooltip(event: MouseEvent, row: EnglandRow) {
    if (row.hasValueNote) {
      let tooltipProvider = this.ftHelperService.newValueNoteTooltipProvider();
      this.tooltip.displayHtml(event, tooltipProvider.getHtmlFromNoteId(row.noteId));
    }
  }

  public showRecentTrendTooltip(event: MouseEvent, row: EnglandRow) {
    let tooltipProvider = this.ftHelperService.newRecentTrendsTooltip();
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

    let ftHelper = this.ftHelperService;
    let polarityId = root.PolarityId;

    if (dataInfo.isValue() && root.RecentTrends[areaCode]) {
      ''
      // Recent trend available
      let recentTrend: TrendMarkerResult = root.RecentTrends[areaCode];
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
    let config = this.ftHelperService.getFTConfig();
    this.hasRecentTrends = config.hasRecentTrends;
    this.isChangeFromPreviousPeriodShown = config.isChangeFromPreviousPeriodShown;
  }

  onExportClick(event: MouseEvent) {
    event.preventDefault();

    // Prepare view
    let $trendInfoIcon = $('.trend-info').hide();
    let trendLegend = $('<div id="trend-legend">'
      + $('#trend-marker-legend').html() +
      '</div>');
    let table = $('#england-table');
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
    event.preventDefault();

    var urls = this.ftHelperService.getURL();
    var model = this.ftHelperService.getFTModel();

    var parameters = new ParameterBuilder()
    .add('parent_area_type_id', model.parentTypeId)
    .add('child_area_type_id', model.areaTypeId)
    .add('group_id', model.groupId)
    .add('areas_code', AreaCodes.England);
  
    var url = urls.corews + 'api/latest/no_inequalities_data/csv/by_group_id?' + parameters.build();
    window.open(url.toLowerCase(), '_blank');
  }
}

class EnglandRow {
  count: string;
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
}
