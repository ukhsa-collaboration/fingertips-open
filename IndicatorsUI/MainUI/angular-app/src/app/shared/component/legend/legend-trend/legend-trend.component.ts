import { Component, Input, SimpleChanges, OnChanges } from '@angular/core';
import { LegendConfig } from '../legend';

@Component({
  selector: 'ft-legend-trend',
  templateUrl: './legend-trend.component.html',
  styleUrls: ['./legend-trend.component.css']
})
export class LegendTrendComponent implements OnChanges {

  @Input() legendConfig: LegendConfig = null;

  constructor() { }

  ngOnChanges(changes: SimpleChanges) { }
}
