import { Component, Input, SimpleChanges, OnChanges } from '@angular/core';
import { LegendType, KeyType } from '../legend.component';

@Component({
  selector: 'ft-legend-area-profiles',
  templateUrl: './legend-area-profiles.component.html',
  styleUrls: ['./legend-area-profiles.component.css']
})
export class LegendAreaProfilesComponent implements OnChanges {

  @Input() showRAG3: Boolean = null;
  @Input() showRAG5: Boolean = null;
  @Input() showBOB: Boolean = null;
  @Input() showQuintilesRAG: Boolean = null;
  @Input() showQuintilesBOB: Boolean = null;

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
    if (changes['showQuintilesRAG']) {
      if (this.showQuintilesRAG) {
      }
    }
    if (changes['showQuintilesBOB']) {
      if (this.showQuintilesBOB) {
      }
    }
  }
}
