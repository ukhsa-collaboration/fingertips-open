import { Component, Input, SimpleChanges, OnChanges } from '@angular/core';

@Component({
  selector: 'ft-legend-map',
  templateUrl: './legend-map.component.html',
  styleUrls: ['./legend-map.component.css']
})
export class LegendMapComponent implements OnChanges {

  @Input() showRAG3: Boolean = null;
  @Input() showRAG5: Boolean = null;
  @Input() showBOB: Boolean = null;
  @Input() showQuartiles: Boolean = null;
  @Input() showQuintilesRAG: Boolean = null;
  @Input() showQuintilesBOB: Boolean = null;
  @Input() showContinuous: Boolean = null;

  showTartanLegends = false;

  constructor() { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['showRAG3']) {
      if (this.showRAG3) {
      }
    }
    if (changes['showRAG5']) {
      if (this.showRAG5) {
      }
    }
    if (changes['showBOB']) {
      if (this.showBOB) {
      }
    }
    if (changes['showQuartiles']) {
      if (this.showQuartiles) {
      }
    }
    if (changes['showQuintilesRAG']) {
      if (this.showQuintilesRAG) {
      }
    }
    if (changes['showQuintilesBOB']) {
      if (this.showQuintilesBOB) {
      }
    }
    if (changes['showContinuous']) {
      if (this.showContinuous) {
      }
    }

    this.showHideTartanLegends();
  }

  showHideTartanLegends() {
    if (this.showRAG3 || this.showRAG5 || this.showBOB) {
      this.showTartanLegends = true;
    } else {
      this.showTartanLegends = false;
    }
  }
}
