import { Component, Input, SimpleChanges, OnChanges } from '@angular/core';
import { LegendType, KeyType } from '../legend.component';

@Component({
  selector: 'ft-legend-quintiles',
  templateUrl: './legend-quintiles.component.html',
  styleUrls: ['./legend-quintiles.component.css']
})
export class LegendQuintilesComponent implements OnChanges {

  @Input() legendType: LegendType = null;
  @Input() keyType: KeyType = null;

  showQuintiles = false;

  constructor() { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['legendType']) {
      if (this.legendType) {
        if (this.legendType === LegendType.Quintiles) {
          this.showQuintiles = true;
        } else {
          this.showQuintiles = false;
        }
      }
    }
  }

}
