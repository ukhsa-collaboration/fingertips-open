import { Component, OnChanges, SimpleChanges, Input, Output, EventEmitter } from '@angular/core';
import { TooltipHelper } from '../../shared/shared';
import { AreaTypeIds, PolarityIds } from '../../shared/constants';
import { TrendRow } from '../trend';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';

@Component({
  selector: 'ft-trend-table',
  templateUrl: './trend-table.component.html',
  styleUrls: ['./trend-table.component.css']
})
export class TrendTableComponent implements OnChanges {

  @Input() trendRows: TrendRow[];
  @Input() trendSource: string;
  @Input() selectedAreaTypeId: number;
  @Input() selectedParentName: string;
  @Input() showRecentTrends: boolean;
  @Output() emitRowHovered = new EventEmitter();

  public shouldShowSubnationalColumn = false;
  public shouldShowEnglandColumn = false;
  public areaName = '';
  tooltipHelper: TooltipHelper;
  currentTooltipHtml: string;
  showAdhocKey: boolean;
  htmlAdhocKey: string;
  recentTrendImage: string;

  constructor(private ftHelperService: FTHelperService) { }

  ngOnChanges(changes: SimpleChanges) {

    this.areaName = this.ftHelperService.getAreaName(this.ftHelperService.getFTModel().areaCode);

    if (changes['trendRows']) {
      if (this.trendRows && this.trendRows.length) {
        this.tooltipHelper = new TooltipHelper(this.ftHelperService.newTooltipManager());
        this.showAdhocKey = this.trendRows[0].useTarget;

        if (this.showAdhocKey) {
          this.htmlAdhocKey =
            '<span class="target-label">Benchmarking against goal: </span>' +
            '<div class="target-legend">' +
            this.trendRows[0].targetLegendHtml +
            '</div>';
        }

        this.recentTrendImage = this.trendRows[0].recentTrendImage;

        this.shouldShowSubnationalColumn = this.selectedAreaTypeId !== AreaTypeIds.Country &&
          !this.ftHelperService.isParentCountry();
        this.shouldShowEnglandColumn = this.selectedAreaTypeId !== AreaTypeIds.Country;
      }
    }
    if (changes['selectedParentName']) {
      if (this.selectedParentName) {
      }
    }
    if (changes['trendSource']) {
      if (this.trendSource) {
      }
    }
  }

  showRecentTrendTooltip(event: MouseEvent) {
    const tooltipProvider = this.ftHelperService.newRecentTrendsTooltip();
    const tooltipHtml = tooltipProvider.getTooltipByData(this.trendRows[0].trendMarkerResult);
    this.tooltipHelper.displayHtml(event, tooltipHtml);
  }

  showValueNoteTooltip(event: MouseEvent, row: TrendRow) {
    if (row.valueNoteId > 0) {
      const tooltipProvider = this.ftHelperService.newValueNoteTooltipProvider();
      this.tooltipHelper.displayHtml(event, tooltipProvider.getHtmlFromNoteId(row.valueNoteId));
    }
  }

  showSubNationalValueNoteTooltip(event: MouseEvent, row: TrendRow) {
    if (row.subNationalNoteId > 0) {
      const tooltipProvider = this.ftHelperService.newValueNoteTooltipProvider();
      this.tooltipHelper.displayHtml(event, tooltipProvider.getHtmlFromNoteId(row.subNationalNoteId));
    }
  }

  showNationalValueNoteTooltip(event: MouseEvent, row: TrendRow) {
    if (row.nationalNoteId > 0) {
      const tooltipProvider = this.ftHelperService.newValueNoteTooltipProvider();
      this.tooltipHelper.displayHtml(event, tooltipProvider.getHtmlFromNoteId(row.nationalNoteId));
    }
  }

  positionTooltip(event: MouseEvent) {
    this.tooltipHelper.reposition(event);
  }

  highlightTrendValueAndComparator(event: MouseEvent, row: TrendRow) {
    this.emitRowHovered.emit({ 'period': row.period, 'tableId': row.tableId });
    const trendTooltip = new TrendTableMarkerTooltopProvider().getTooltipText(row)
    this.tooltipHelper.displayHtml(event, trendTooltip);
  }

  highlightTrendValueWithTarget(event: MouseEvent, row: TrendRow) {
    const trendTooltip = 'This value is benchmarked<br>against the goal above';
    row.highlightAreaValue = true;
    this.tooltipHelper.displayHtml(event, trendTooltip);
  }

  unhighlightValueAndComparator() {
    this.emitRowHovered.emit({ 'period': 'none', 'tableId': '0' });
    this.hideTooltip();
  }

  hideTooltip() {
    this.tooltipHelper.hide();
  }

  public displayRecentTrends(): boolean {
    return this.showRecentTrends;
  }
}

export class TrendTableMarkerTooltopProvider {

  getTooltipText(row: TrendRow): string {
    let adjective;
    const comparatorName = row.comparatorName;
    const useJudgement = row.polarityId === PolarityIds.RAGHighIsGood ||
      row.polarityId === PolarityIds.RAGLowIsGood;

    // Quintiles
    if (row.useQuintileColouring) {
      switch (row.significance) {
        case 1:
          adjective = useJudgement ? 'Best' : 'Lowest';
          return adjective + ' quintile in ' + comparatorName;
        case 2:
          adjective = useJudgement ? 'best' : 'lowest';
          return '2nd ' + adjective + ' quintile in ' + comparatorName;
        case 3:
          return 'Middle quintile in ' + comparatorName;
        case 4:
          adjective = useJudgement ? 'worst' : 'highest';
          return '2nd ' + adjective + ' quintile in ' + comparatorName;
        case 5:
          adjective = useJudgement ? 'Worst' : 'Highest';
          return adjective + ' quintile in ' + comparatorName;
        default:
          // Quintile not assessed
          return 'Quintile not determined';
      }
    }

    // Comparison with benchmark
    const average = comparatorName + ' average';

    switch (row.significance) {
      case 0:
        return 'Significance is not calculated for this indicator';
      case 1:
        // Worse/lower 95.0 CI
        return this.getWorseStatement(useJudgement, average);
      case 2:
        return 'Not significantly different to ' + average;
      case 3:
        // Better/higher 95.0 CI
        return this.getBetterStatement(useJudgement, average);
      case 4:
        // Worse/lower 99.8 CI
        return this.getWorseStatement(useJudgement, average);
      case 5:
        // Better/higher 99.8 CI
        return this.getBetterStatement(useJudgement, average);
      default:
        // No comparator value
        return 'No ' + average + ' value to compare';
    }
  }

  private getBetterStatement(useJudgement: boolean, average: string): string {
    const adjective = useJudgement ? 'better' : 'above';
    return 'Significantly ' + adjective + ' than ' + average;
  }

  private getWorseStatement(useJudgement: boolean, average: string): string {
    const adjective = useJudgement ? 'worse' : 'below';
    return 'Significantly ' + adjective + ' than ' + average;
  }
}
