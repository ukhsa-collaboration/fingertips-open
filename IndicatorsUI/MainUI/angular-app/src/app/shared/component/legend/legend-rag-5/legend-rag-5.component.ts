import { Component, Input, SimpleChanges, OnChanges } from '@angular/core';
import { LegendType, KeyType } from '../legend.component';

@Component({
  selector: 'ft-legend-rag-5',
  templateUrl: './legend-rag-5.component.html',
  styleUrls: ['./legend-rag-5.component.css']
})
export class LegendRag5Component implements OnChanges {

  @Input() legendType: LegendType = null;
  @Input() keyType: KeyType = null;

  showRag = false;
  showTartanRug = false;
  showSpineChart = false;
  showBenchmark = true;

  constructor() { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['legendType']) {
      if (this.legendType) {
        if (this.legendType === LegendType.RAG5) {
          this.showRag = true;
        } else {
          this.showRag = false;
        }
      }
    }
    if (changes['keyType']) {
      if (this.keyType) {
        switch (this.keyType) {
          case KeyType.TartanRug:
            this.showTartanRug = true;
            break;
          case KeyType.SpineChart:
            this.showSpineChart = true;
            break;
          default:
            this.showTartanRug = false;
            this.showSpineChart = false;
            break;
        }
      }
    }
  }
}
