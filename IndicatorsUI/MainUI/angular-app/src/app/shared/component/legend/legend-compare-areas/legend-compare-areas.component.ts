import { Component, Input, SimpleChanges, OnChanges } from '@angular/core';
import { LegendConfig } from '../legend';

@Component({
  selector: 'ft-legend-compare-areas',
  templateUrl: './legend-compare-areas.component.html',
  styleUrls: ['./legend-compare-areas.component.css']
})
export class LegendCompareAreasComponent implements OnChanges {

  @Input() legendConfig: LegendConfig = null;

  constructor() { }

  ngOnChanges(changes: SimpleChanges) { }
}
