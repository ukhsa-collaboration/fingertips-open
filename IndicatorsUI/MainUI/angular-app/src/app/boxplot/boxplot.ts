import {
  IndicatorMetadata, IndicatorStatsPercentilesFormatted,
  IndicatorStats, IndicatorStatsPercentiles
} from '../typings/FT';

export class BoxplotData {

  stats: IndicatorStatsPercentiles[] = []
  statsFormatted: IndicatorStatsPercentilesFormatted[] = [];
  periods: string[] = [];
  min: number = null;

  constructor(public metadata: IndicatorMetadata, public indicatorName: string,
    public areaTypeName: string, public areaName: string, public comparatorName: string,
    public comparatorCode: string, public isNearestNeighbour) { }

  addStats(indicatorStats: IndicatorStats): void {

    // Chart data
    this.stats.push(indicatorStats.Stats);

    // Table data
    this.statsFormatted.push(indicatorStats.StatsF);

    // Time periods
    this.periods.push(indicatorStats.Period);
  }

  setMin(min: boolean): void {
    this.min = min ? 0 : null;
  }

  isAnyData(): boolean {
    return this.periods.length > 0;
  }
}

export class BoxplotDataForTable {
  public period: string;
  public statsFormatted: IndicatorStatsPercentilesFormatted;
  public unitLabel: string;
  public trendSource: string;
}
