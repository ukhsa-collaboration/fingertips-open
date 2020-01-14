import { Component, Input, SimpleChanges, OnChanges } from '@angular/core';
import { LegendConfig } from '../legend';

@Component({
  selector: 'ft-legend-map',
  templateUrl: './legend-map.component.html',
  styleUrls: ['./legend-map.component.css']
})
export class LegendMapComponent implements OnChanges {

  @Input() legendConfig: LegendConfig = null;

  constructor() { }

  ngOnChanges(changes: SimpleChanges) { }
}
