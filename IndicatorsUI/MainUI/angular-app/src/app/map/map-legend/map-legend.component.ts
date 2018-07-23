import { Component, Input } from '@angular/core';

@Component({
  selector: 'ft-map-legend',
  templateUrl: './map-legend.component.html',
  styleUrls: ['./map-legend.component.css']
})
export class MapLegendComponent {
  @Input() legendDisplay: number = LegendDisplay.NoLegendRequired;

  showBenchmark() {
    return this.legendDisplay === LegendDisplay.RAG ||
      this.legendDisplay === LegendDisplay.BOB ||
      this.legendDisplay === LegendDisplay.NotCompared;
  }

  showRag() {
    return this.legendDisplay === LegendDisplay.RAG;
  }

  showBob() {
    return this.legendDisplay === LegendDisplay.BOB;
  }

  showQuartiles() {
    return this.legendDisplay === LegendDisplay.Quartiles;
  }

  showQuintiles() {
    return this.legendDisplay === LegendDisplay.Quintiles;
  }

  showContinuous() {
    return this.legendDisplay === LegendDisplay.Continuous;
  }
}

export class LegendDisplay {
  public static readonly NoLegendRequired = 1;
  public static readonly RAG = 2;
  public static readonly BOB = 3;
  public static readonly NotCompared = 4;
  public static readonly Quintiles = 5;
  public static readonly Quartiles = 6;
  public static readonly Continuous = 7;
}
