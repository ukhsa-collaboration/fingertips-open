import { Component, Input } from '@angular/core';

@Component({
  selector: 'ft-legend-data-quality',
  templateUrl: './legend-data-quality.component.html',
  styleUrls: ['./legend-data-quality.component.css']
})
export class LegendDataQualityComponent {

  @Input() showDataQuality: boolean = null;
}
