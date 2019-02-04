import { Component, Input, SimpleChanges, OnChanges } from '@angular/core';
import { LegendType, KeyType } from '../legend.component';

@Component({
  selector: 'ft-legend-bob',
  templateUrl: './legend-bob.component.html',
  styleUrls: ['./legend-bob.component.css']
})
export class LegendBobComponent implements OnChanges {

  @Input() legendType: LegendType = null;
  @Input() keyType: KeyType = null;

  showBob = false;
  showBenchmark = true;

  constructor() { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['legendType']) {
      if (this.legendType) {
        if (this.legendType === LegendType.BOB) {
          this.showBob = true;
        } else {
          this.showBob = false;
        }
      }
    }
  }
}
