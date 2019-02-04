import { Component, Input, SimpleChanges, OnChanges } from '@angular/core';
import { LegendType, KeyType } from '../legend.component';

@Component({
  selector: 'ft-legend-quartiles',
  templateUrl: './legend-quartiles.component.html',
  styleUrls: ['./legend-quartiles.component.css']
})
export class LegendQuartilesComponent implements OnChanges {

  @Input() legendType: LegendType = null;
  @Input() keyType: KeyType = null;

  showQuartiles = false;

  constructor() { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['legendType']) {
      if (this.legendType) {
        if (this.legendType === LegendType.Quartiles) {
          this.showQuartiles = true;
        } else {
          this.showQuartiles = false;
        }
      }
    }
  }
}
