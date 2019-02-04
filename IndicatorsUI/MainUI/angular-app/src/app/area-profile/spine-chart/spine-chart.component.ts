import { Component, OnInit, Input } from '@angular/core';
import { SpineChartDimensions } from '../spine-chart.classes';
import { TooltipHelper } from 'app/shared/shared';
import { IndicatorRow } from '../area-profile.component';
import { FTHelperService } from 'app/shared/service/helper/ftHelper.service';

@Component({
  selector: 'ft-spine-chart',
  templateUrl: './spine-chart.component.html',
  styleUrls: ['./spine-chart.component.css']
})
export class SpineChartComponent {

  @Input() public dimensions: SpineChartDimensions;
  @Input() public tooltip: TooltipHelper;
  @Input() public indicatorRow: IndicatorRow;

  private currentTooltipHtml: string;
  private html: string[];

  constructor(private ftHelperService: FTHelperService) { }

  public showMarkerTooltip(event: MouseEvent) {
    this.html = [];
    let formatter = this.indicatorRow.formatter;

    // Value
    this.addDataText(formatter.getAreaValue() + formatter.getSuffixIfNoShort());

    // Area name
    var areaName = this.ftHelperService.getAreaNameToDisplay(this.indicatorRow.area);
    this.html.push('<span id="tooltipArea">', areaName, '</span>');

    this.addIndicatorName();
    this.addValueNote();

    this.showTooltip(event);
  }

  public showInnerQuartilesTooltip(event: MouseEvent) {

    let formatter = this.indicatorRow.formatter;

    let values = formatter.get25() + ' - ' + formatter.get75();
    let labels = '25th Percentile to 75th Percentile';

    this.showRangeTooltip(event, values, labels);
  }

  public showLeftQuartileTooltip(event: MouseEvent) {

    let formatter = this.indicatorRow.formatter;

    let values = formatter.getMin() + ' - ' + formatter.get25();
    let labels = this.getSpineHeaders().min + ' to 25th Percentile';

    this.showRangeTooltip(event, values, labels);
  }

  public showRightQuartileTooltip(event: MouseEvent) {

    let formatter = this.indicatorRow.formatter;

    let values = formatter.get75() + ' - ' + formatter.getMax();
    let labels = '75th Percentile to ' + this.getSpineHeaders().max;

    this.showRangeTooltip(event, values, labels);
  }

  public showBenchmarkTooltip(event: MouseEvent) {
    this.html = [];
    let formatter = this.indicatorRow.formatter;

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
    let formatter = this.indicatorRow.formatter;

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
    let noteId = this.indicatorRow.areaData.NoteId;
    if (noteId) {
      var valueNoteHtml = this.ftHelperService.newValueNoteTooltipProvider().getHtmlFromNoteId(noteId);

      // Add extra CSS class
      valueNoteHtml = valueNoteHtml.replace('tooltipValueNote',
        'tooltipValueNote tooltip-value-vote-spine-chart');

      this.html.push(valueNoteHtml);
    }
  }

  private showTooltip(event: MouseEvent) {
    this.currentTooltipHtml = this.html.join('');
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
