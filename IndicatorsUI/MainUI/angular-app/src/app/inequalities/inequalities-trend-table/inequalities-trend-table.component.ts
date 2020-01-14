import { Component, OnChanges, Input, SimpleChanges } from '@angular/core';
import { TrendTableData, TrendTableDataFormatted, InequalityValueData } from '../inequalities';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { ValueData, InequalitiesTrendsTableTooltip } from '../../typings/FT';
import * as _ from 'underscore';
import { CommaNumber, TooltipHelper } from '../../shared/shared';
import { isDefined } from '@angular/compiler/src/util';

@Component({
  selector: 'ft-inequalities-trend-table',
  templateUrl: './inequalities-trend-table.component.html',
  styleUrls: ['./inequalities-trend-table.component.css']
})
export class InequalitiesTrendTableComponent implements OnChanges {

  @Input() trendTableData: TrendTableData[];
  columnHeaders: string[];
  rowData: TrendTableDataFormatted[];
  numberOfColumns: number;
  tooltipHelper: TooltipHelper;
  currentTrendsTooltipHtml: string;

  constructor(private ftHelper: FTHelperService) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['trendTableData']) {
      if (this.trendTableData.length > 0) {
        this.generateColumnHeaders();
        this.generateRowData();
      }
    }
  }

  generateColumnHeaders() {
    this.columnHeaders = _.pluck(this.trendTableData, 'label');
  }

  generateRowData() {
    let i = 0;
    let j = 0;

    this.rowData = [];
    for (i = 0; i < this.trendTableData[0].valueData.length; i++) {
      const data = new TrendTableDataFormatted();
      const valueData: InequalityValueData[] = [];
      data.period = this.trendTableData[0].periods[i];
      data.valueData = valueData;
      for (j = 0; j < this.trendTableData.length; j++) {
        let value = this.trendTableData[j].valueData[i].ValF;
        if (value !== '-') {
          value = value + this.trendTableData[j].unitLabel;
        }
        const valF = new CommaNumber(value).unrounded();
        const trendValueData = new InequalityValueData(this.ftHelper, valF,
          this.trendTableData[j].valueData[i].Note.Id,
          this.trendTableData[j].valueData[i].Count,
          this.trendTableData[j].valueData[i].Val);

        data.valueData.push(trendValueData);
      }
      this.rowData.push(data);
      this.numberOfColumns = j;
    }

    this.tooltipHelper = new TooltipHelper(this.ftHelper.newTooltipManager());
  }

  getTrendSource(): string {
    if (this.trendTableData.length > 0) {
      return this.trendTableData[0].trendSource;
    }

    return '';
  }

  getNumberOfColumnsToSpan(): number {
    return this.numberOfColumns + 1;
  }

  public showRecentTrendTooltip(event: MouseEvent, valueData: ValueData) {
    if (isDefined(valueData.Note) && isDefined(valueData.Note.Text)) {
      const tooltipProvider = new InequalitiesTrendTableValueDataTooltip(this.ftHelper, valueData);
      this.currentTrendsTooltipHtml = tooltipProvider.getTooltipMessage();
      this.tooltipHelper.displayHtml(event, this.currentTrendsTooltipHtml);
    } else {
      this.hideTooltip();
    }
  }

  public hideTooltip() {
    this.tooltipHelper.hide();
    this.currentTrendsTooltipHtml = null;
  }

  public repositionTooltip(event: MouseEvent) {
    this.tooltipHelper.reposition(event);
  }
}

class InequalitiesTrendTableValueDataTooltip implements InequalitiesTrendsTableTooltip {
  ftHelper: FTHelperService;
  valueData: ValueData;

  constructor(ftHelper: FTHelperService, data: ValueData) {
    this.ftHelper = ftHelper;
    this.valueData = data;
  }

  getTooltipMessage(): any {
    let htmlTooltipMessage = '';
    if (isDefined(this.valueData.Note) && isDefined(this.valueData.Note.Text)) {
      htmlTooltipMessage = this.ftHelper.getValueNoteSymbol() + this.valueData.Note.Text;
    }
    return htmlTooltipMessage;
  }
}
