import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { SpineChartDimensions } from '../spine-chart.classes';
import { TooltipHelper } from '../../shared/shared';
import { IndicatorRow } from '../area-profile.component';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { isDefined } from '@angular/compiler/src/util';

@Component({
  selector: 'ft-spine-chart',
  templateUrl: './spine-chart.component.html',
  styleUrls: ['./spine-chart.component.css']
})
export class SpineChartComponent implements OnChanges {

  @Input() public dimensions: SpineChartDimensions;
  @Input() public tooltipHelper: TooltipHelper;
  @Input() public indicatorRow: IndicatorRow;
  public doesContainMarkerImage = false;

  private currentTooltipHtml: string;
  private html: string[];

  constructor(private ftHelperService: FTHelperService) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['dimensions']) {
      if (this.dimensions) {
        if (isDefined(this.dimensions.markerImage)) {
          this.doesContainMarkerImage = true;
        }
      }
    }
  }

  public showMarkerTooltip(event: MouseEvent) {
    this.html = [];
    const formatter = this.indicatorRow.formatter;

    // Value
    this.addDataText(formatter.getAreaValue() + formatter.getSuffixIfNoShort());

    // Area name
    const areaName = this.ftHelperService.getAreaNameToDisplay(this.indicatorRow.area);
    this.html.push('<span id="tooltipArea">', areaName, '</span>');

    this.addIndicatorName();
    this.addValueNote();

    this.showTooltip(event);
  }

  public showInnerQuartilesTooltip(event: MouseEvent) {

    const formatter = this.indicatorRow.formatter;

    const values = formatter.get25() + ' - ' + formatter.get75();
    const labels = '25th Percentile to 75th Percentile';

    this.showRangeTooltip(event, values, labels);
  }

  public showLeftQuartileTooltip(event: MouseEvent) {

    const formatter = this.indicatorRow.formatter;

    const values = formatter.getMin() + ' - ' + formatter.get25();
    const labels = this.getSpineHeaders().min + ' to 25th Percentile';

    this.showRangeTooltip(event, values, labels);
  }

  public showRightQuartileTooltip(event: MouseEvent) {

    const formatter = this.indicatorRow.formatter;

    const values = formatter.get75() + ' - ' + formatter.getMax();
    const labels = '75th Percentile to ' + this.getSpineHeaders().max;

    this.showRangeTooltip(event, values, labels);
  }

  public showBenchmarkTooltip(event: MouseEvent) {
    this.html = [];
    const formatter = this.indicatorRow.formatter;

    this.addDataText(formatter.getAverage() + formatter.getSuffixIfNoShort());
    this.addComparatorName();
    this.addIndicatorName();

    this.showTooltip(event);
  }

  private addDataText(text: string) {
    this.html.push('<span id="tooltipData">', text, '</span>');
  }

  private showRangeTooltip(event: MouseEvent, values: string, labels: string) {
    this.html = [];

    this.addDataText(values);
    this.addRangeText(labels);
    this.addComparatorName();
    this.addIndicatorName();

    this.showTooltip(event);
  }

  private addComparatorName() {
    this.html.push('<span id="tooltipArea">', this.ftHelperService.getCurrentComparator().Name, '</span>');
  }

  private getSpineHeaders() {
    return this.ftHelperService.getFTConfig().spineHeaders;
  }

  private addIndicatorName() {
    this.html.push('<span id="tooltipIndicator">',
      this.indicatorRow.formatter.getIndicatorName(), '</span>');
  }

  private addRangeText(text: string) {
    this.html.push('<span id="range-text">', text, '</span>');
  }

  private addValueNote() {
    const noteId = this.indicatorRow.areaData.NoteId;
    if (noteId) {
      let valueNoteHtml = this.ftHelperService.newValueNoteTooltipProvider().getHtmlFromNoteId(noteId);

      // Add extra CSS class
      valueNoteHtml = valueNoteHtml.replace('tooltipValueNote',
        'tooltipValueNote tooltip-value-vote-spine-chart');

      this.html.push(valueNoteHtml);
    }
  }

  private showTooltip(event: MouseEvent) {
    this.currentTooltipHtml = this.html.join('');
    this.tooltipHelper.displayHtml(event, this.currentTooltipHtml);
  }

  public hideTooltip() {
    this.tooltipHelper.hide();
    this.currentTooltipHtml = null;
  }

  public repositionTooltip(event: MouseEvent) {
    this.tooltipHelper.reposition(event);
  }
}
