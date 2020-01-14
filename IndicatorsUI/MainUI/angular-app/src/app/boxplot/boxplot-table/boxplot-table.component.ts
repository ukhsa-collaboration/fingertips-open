import { Component, Input } from '@angular/core';
import { BoxplotDataForTable } from '../boxplot';
import { CommaNumber } from '../../shared/shared';

@Component({
  selector: 'ft-boxplot-table',
  templateUrl: './boxplot-table.component.html',
  styleUrls: ['./boxplot-table.component.css']
})
export class BoxplotTableComponent {

  @Input() boxplotDataForTable: BoxplotDataForTable[];

  getValueAndUnit(data: BoxplotDataForTable, value: string): string {
    return new CommaNumber(value).unrounded() + data.unitLabel;
  }

  getTrendSource(): string {
    if (this.boxplotDataForTable.length > 0) {
      return this.boxplotDataForTable[0].trendSource;
    }
  }
}
