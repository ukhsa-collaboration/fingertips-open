import { Component, Input, SimpleChanges, OnChanges } from '@angular/core';
import { LegendConfig } from '../legend';

@Component({
  selector: 'ft-legend-benchmark-against-goal',
  templateUrl: './legend-benchmark-against-goal.component.html',
  styleUrls: ['./legend-benchmark-against-goal.component.css']
})
export class LegendBenchmarkAgainstGoalComponent implements OnChanges {

  @Input() legendConfig: LegendConfig = null;
  showBenchmarkAgainstGoal = false;
  targetLegendForBenchmark = '';

  constructor() { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['legendConfig']) {
      if (this.legendConfig) {
        this.showBenchmarkAgainstGoal = this.legendConfig.showBenchmarkAgainstGoal;
        this.targetLegendForBenchmark = this.legendConfig.targetLegendForBenchmark;
      }
    }
  }
}
