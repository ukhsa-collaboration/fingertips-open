import {
  IndicatorMetadata, IndicatorStatsPercentilesFormatted,
  IndicatorStats, IndicatorStatsPercentiles
} from '../typings/FT.d';

export class BoxplotData {

  stats: IndicatorStatsPercentiles[] = []
  statsFormatted: IndicatorStatsPercentilesFormatted[] = [];
  periods: string[] = [];
  min: number = null;

  constructor(public metadata: IndicatorMetadata, public areaTypeName: string,
    public comparatorName: string) { }

  addStats(indicatorStats: IndicatorStats): void {

    // Chart data
    this.stats.push(indicatorStats.Stats);

    // Table data
    this.statsFormatted.push(indicatorStats.StatsF);

    // Time periods
    this.periods.push(indicatorStats.Period);

    // Set min limit if zero to prevent Y axis starting at negative number
    if (indicatorStats.Limits.Min === 0) {
      this.min = indicatorStats.Limits.Min;
    }
  }

  isAnyData(): boolean {
    return this.periods.length > 0;
  }
}