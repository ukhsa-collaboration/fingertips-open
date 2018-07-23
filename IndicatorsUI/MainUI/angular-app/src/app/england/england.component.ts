import { Component, HostListener } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import {
  FTModel, FTRoot, GroupRoot, IndicatorMetadata, CoreDataSetInfo,
  TrendMarkerResult, TrendMarker, TooltipManager
} from '../typings/FT.d';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { TooltipHelper } from '../shared/shared';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'ft-england',
  templateUrl: './england.component.html',
  styleUrls: ['./england.component.css']
})
export class EnglandComponent {

  public isChangeFromPreviousPeriodShown: boolean = false;
  public rows: Array<EnglandRow>;
  public hasRecentTrends: boolean = false;
  private tooltip: TooltipHelper;

  constructor(private ftHelperService: FTHelperService, private indicatorService: IndicatorService) { }

  @HostListener('window:EnglandSelected', ['$event'])
  public onOutsideEvent(event) {

    let ftHelper = this.ftHelperService;
    let model = ftHelper.getFTModel();
    let groupRootsObservable = this.indicatorService.getLatestDataForAllIndicatorsInProfileGroupForChildAreas(model.groupId, model.areaTypeId, model.parentCode, model.profileId);
    let metadataObservable = this.indicatorService.getIndicatorMetadata(model.groupId);

    Observable.forkJoin([groupRootsObservable, metadataObservable]).subscribe(results => {
      let groupRoots: GroupRoot[] = results[0];
      let metadataHash: Map<number, IndicatorMetadata> = results[1];

      this.setConfig();
      this.tooltip = new TooltipHelper(this.ftHelperService.newTooltipManager());

      this.rows = [];
      for (let rootIndex in groupRoots) {
        let root = groupRoots[rootIndex];
        let indicatorId = root.IID;
        let metadata: IndicatorMetadata = metadataHash[indicatorId];
        let unit = !!metadata ? metadata.Unit : null;

        let row: EnglandRow = new EnglandRow();
        this.rows.push(row)
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
      }

      ftHelper.showAndHidePageElements();
      ftHelper.unlock();
    });
  }

  private setUpRecentTrendOnRow(row: EnglandRow, root: GroupRoot, areaCode: string, dataInfo: CoreDataSetInfo) {

    let ftHelper = this.ftHelperService;
    var polarityId = root.PolarityId;

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

  private hideTooltip() {
    this.tooltip.hide();
  }

  private indicatorNameClicked(row: EnglandRow) {
    this.ftHelperService.goToMetadataPage(row.rootIndex);
  }

  private showValueNoteTooltip(event: MouseEvent, row: EnglandRow) {
    if (row.hasValueNote) {
      var tooltipProvider = this.ftHelperService.newValueNoteTooltipProvider();
      this.tooltip.displayHtml(event, tooltipProvider.getHtmlFromNoteId(row.noteId));
    }
  }

  private showRecentTrendTooltip(event: MouseEvent, row: EnglandRow) {
    var tooltipProvider = this.ftHelperService.newRecentTrendsTooltip();
    this.tooltip.displayHtml(event, tooltipProvider.getTooltipByData(row.recentTrend));
  }

  private positionTooltip(event: MouseEvent) {
    this.tooltip.reposition(event);
  }

  private recentTrendClicked(row: EnglandRow) {
    this.ftHelperService.recentTrendSelected().byGroupRoot(row.rootIndex)
  }

  private setConfig() {
    var config = this.ftHelperService.getFTConfig();
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
}
