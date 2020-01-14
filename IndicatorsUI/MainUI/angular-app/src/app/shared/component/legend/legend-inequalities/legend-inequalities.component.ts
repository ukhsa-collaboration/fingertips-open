import { Component, Input, SimpleChanges, OnChanges } from '@angular/core';
import { LegendConfig } from '../legend';

@Component({
  selector: 'ft-legend-inequalities',
  templateUrl: './legend-inequalities.component.html',
  styleUrls: ['./legend-inequalities.component.css']
})
export class LegendInequalitiesComponent implements OnChanges {

  @Input() legendConfig: LegendConfig = null;

  constructor() { }

  ngOnChanges(changes: SimpleChanges) { }
}
