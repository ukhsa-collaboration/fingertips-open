import { Component, Input, SimpleChanges, OnChanges } from '@angular/core';
import { LegendType, KeyType } from '../legend.component';

@Component({
  selector: 'ft-legend-continuous',
  templateUrl: './legend-continuous.component.html',
  styleUrls: ['./legend-continuous.component.css']
})
export class LegendContinuousComponent implements OnChanges {

  @Input() legendType: LegendType = null;
  @Input() keyType: KeyType = null;

  showContinuous = false;

  constructor() { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['legendType']) {
      if (this.legendType) {
        if (this.legendType === LegendType.Continuous) {
          this.showContinuous = true;
        } else {
          this.showContinuous = false;
        }
      }
    }
  }
}
