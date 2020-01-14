import { Component, OnChanges, SimpleChanges, Input, ViewChild, ElementRef, EventEmitter, Output } from '@angular/core';
import { isDefined } from '@angular/compiler/src/util';
import * as Highcharts from 'highcharts';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { DownloadService } from '../../shared/service/api/download.service';
import { HC } from '../../shared/constants';
import { DataSeries, IndicatorMenuItem, ChartSeries, LinearRegression, Point } from '../compare-indicator.classes';
import { CsvConfig } from '../../shared/component/export-csv/export-csv';

@Component({
  selector: 'ft-scatter-plot-chart',
  templateUrl: './scatter-plot-chart.component.html',
  styleUrls: ['./scatter-plot-chart.component.css']
})
export class ScatterPlotChartComponent implements OnChanges {

  @Input() dataSeries: DataSeries = null;
  @Input() selectedOptions: IndicatorMenuItem[] = null;
  @Output() emitChart = new EventEmitter();
  @ViewChild('chart', { static: true }) public chartEl: ElementRef;

  chart: Highcharts.ChartObject;
  linearRegression: LinearRegression;
  csvConfig: CsvConfig;

  constructor(private ftHelperService: FTHelperService,
    private downloadService: DownloadService) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['dataSeries']) {
      if (this.dataSeries) {
        this.displayChart();
      }
    }
  }

  private displayChart() {
    if (this.isAnyData()) {
      let chartContainer = null;
      if (this.chartEl && this.chartEl.nativeElement) {
        chartContainer = this.chartEl.nativeElement;
        this.chart = new Highcharts.Chart(chartContainer, this.getChartOptions());
        this.emitChart.emit({ 'chart': this.chart });
      }
    }
  }

  private getChartOptions(): Highcharts.Options {

    const dataSeries = this.dataSeries;

    const hcSeriesList = [
      this.getHCSeriesForData(),
      this.getHCSeriesForHighlightedPoint(),
      this.getHCSeriesForRSquaredLine()
    ];

    return (
      {
        chart: {
          type: 'scatter',
          zoomType: 'xy',
          width: 900,
          marginRight: 280,
          animation: false
        },
        credits: HC.Credits,
        title: {
          text: ''
        },
        subtitle: {
          text: ''
        },
        xAxis: {
          title: {
            enabled: true,
            text: this.xAxisTitle(),
            style: {
              width: 400
            }
          },
          startOnTick: true,
          endOnTick: true,
          showLastLabel: true
        },
        yAxis: {
          title: {
            text: this.yAxisTitle(),
            margin: this.dataSeries.Margin,
            style: {
              width: 300
            }
          }
        },
        legend: {
          layout: 'vertical',
          align: 'right',
          y: -200,
          floating: true,
          backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#fff',
          borderWidth: 0,
          width: 200
        },
        plotOptions: {
          line: {
            animation: false
          },
          scatter: {
            animation: false,
            marker: {
              lineColor: '#000000',
              lineWidth: 1,
              radius: 5,
              states: {
                hover: {
                  enabled: true,
                  lineColor: 'rgb(100,100,100)'
                }
              }
            },
            states: {
              hover: {
                marker: {
                  enabled: false
                }
              }
            }
          }
        },
        tooltip: {
          followPointer: false,
          formatter: function () {

            const tooltipContent: string[] = [];

            // Tooltip for point
            const point = dataSeries.Points.find(x => x.X === this.x && x.Y === this.y);
            if (isDefined(point)) {
              tooltipContent.push(
                '<b>', point.Name, '</b><br/>',
                'x:<b>', point.XValF, '</b> ', dataSeries.IndicatorUnit.UnitX, '<br/>',
                'y:<b>', point.YValF, '</b> ', dataSeries.IndicatorUnit.UnitY, '<br/>'
              );
            }

            return tooltipContent.join('');
          }
        },
        series: hcSeriesList
      });
  }

  getHCSeriesForData(): any {

    const data = this.dataSeries.Points.filter(x => !x.Highlighted);

    return {
      type: 'scatter',
      name: this.dataSeries.Name,
      data: this.getScatterPoints(data),
      color: '#7cb5ec',
      showInLegend: true,
      marker: {
        symbol: 'circle',
        states: {
          hover: {
            enabled: true,
            lineColor: 'rgb(100,100,100)'
          }
        }
      }
    };
  }

  getHCSeriesForHighlightedPoint(): any {

    const series = this.getSeriesToHighlightSelectedArea();

    return {
      type: series.Type,
      name: series.Name,
      data: series.Data,
      color: series.Colour,
      showInLegend: series.ShowNameInLegend,
      marker: {
        symbol: series.MarkerSymbol,
        states: {
          hover: {
            enabled: true,
            lineColor: 'rgb(100,100,100)'
          }
        }
      }
    };
  }

  getHCSeriesForRSquaredLine(): any {

    const series = this.getRSquaredSeries();

    return {
      type: series.Type,
      name: series.Name,
      data: series.Coordinates,
      color: series.Colour,
      showInLegend: series.ShowNameInLegend,
      marker: {
        enabled: false
      },
      states: {
        hover: {
          lineWidth: 0
        }
      },
      enableMouseTracking: false
    };
  }

  getSeriesToHighlightSelectedArea(): ChartSeries {
    const series = new ChartSeries();

    const highlightedPoint = this.dataSeries.Points.find(x => x.Highlighted);

    if (highlightedPoint) {
      series.Name = this.dataSeries.SelectedAreaName;
      series.Type = 'scatter';
      series.Data = this.getScatterPoints([highlightedPoint]);
      series.MarkerSymbol = 'diamond';
      series.Colour = '#000';
      series.ShowNameInLegend = true;
    } else {
      series.Name = '';
      series.Type = '';
      series.Data = [];
      series.MarkerSymbol = '';
      series.Colour = '#fff';
      series.ShowNameInLegend = false;
    }
    return series;
  }

  getRSquaredSeries(): ChartSeries {

    const series = new ChartSeries();

    if (this.dataSeries.R2Selected) {
      const linearRegressionData = this.getLinearRegressionData(this.dataSeries.YAxisData, this.dataSeries.XAxisData);
      const rSquareEquation = 'y = ' + Math.round(linearRegressionData.Slope * 100) / 100 + 'x +' +
        Math.round(linearRegressionData.Intercept * 100) / 100;
      const legendBaseText = rSquareEquation + '<br>R\xB2 = ' + Math.round(linearRegressionData.R2 * 100) / 100;

      const rSquaredValue = Math.round(linearRegressionData.R2 * 100) / 100;
      const rSquareThreshold = 0.15;
      const isValueOverThreshold = rSquaredValue > rSquareThreshold;

      series.Name = isValueOverThreshold
        ? legendBaseText
        : 'Trend line not drawn when<br>R\xB2 is below 0.15 (R\xB2 = ' + rSquaredValue + ')';
      series.Type = 'line';
      series.Coordinates = isValueOverThreshold ? linearRegressionData.Coordinates : null;
      series.MarkerSymbol = 'diamond';
      series.Colour = isValueOverThreshold ? '#ed1f52' : '#fff';
      series.ShowNameInLegend = true;
    } else {
      series.Name = '';
      series.Type = '';
      series.Data = [];
      series.MarkerSymbol = '';
      series.Colour = '#fff';
      series.ShowNameInLegend = false;
    }
    return series;
  }

  getScatterPoints(points: Point[]): number[][] {
    const scatterPoints: number[][] = [];

    for (const i in points) {
      if (points.hasOwnProperty(i)) {
        scatterPoints.push([points[i].X, points[i].Y]);
      }
    }

    return scatterPoints;
  }

  xAxisTitle() {
    return this.formatTitle(this.dataSeries.XAxisTitle, this.dataSeries.IndicatorUnit.UnitX);
  }

  yAxisTitle() {
    return this.formatTitle(this.dataSeries.YAxisTitle, this.dataSeries.IndicatorUnit.UnitY);
  }

  formatTitle(indicatorName: string, unit: string): string {
    if (unit !== '') {
      return indicatorName + ' / ' + unit;
    }

    return indicatorName;
  }

  calculateMargin(indicatorName: string): number {
    const indicatorNameLength = indicatorName.length;
    let newMargin = 30;
    if (indicatorNameLength > 60) {
      newMargin = (indicatorNameLength / 60) * 30;
    }

    return newMargin;
  }

  getLinearRegressionData(y: number[], x: number[]) {
    const n = y.length;
    let sum_x = 0;
    let sum_y = 0;
    let sum_xy = 0;
    let sum_xx = 0;
    let sum_yy = 0;

    for (let i = 0; i < y.length; i++) {
      sum_x += x[i];
      sum_y += y[i];
      sum_xy += (x[i] * y[i]);
      sum_xx += (x[i] * x[i]);
      sum_yy += (y[i] * y[i]);
    }

    const slope = (n * sum_xy - sum_x * sum_y) / (n * sum_xx - sum_x * sum_x);
    const intercept = (sum_y - slope * sum_x) / n;
    const r2 = Math.pow((n * sum_xy - sum_x * sum_y) /
      Math.sqrt((n * sum_xx - sum_x * sum_x) * (n * sum_yy - sum_y * sum_y)), 2);

    const points = [];
    for (let k = 0; k < x.length; k++) {
      const point = [x[k], x[k] * slope + intercept];
      points.push(point);
    }

    points.sort(function (a, b) {
      if (a[0] > b[0]) { return 1 }
      if (a[0] < b[0]) { return -1 }
      return 0;
    });

    this.linearRegression = new LinearRegression();
    this.linearRegression.Coordinates = points;
    this.linearRegression.Slope = slope;
    this.linearRegression.Intercept = intercept;
    this.linearRegression.R2 = r2;

    return this.linearRegression;
  }

  isAnyData(): boolean {
    return this.dataSeries.Points.length > 0;
  }

  // This method is to keep going with development and it will be replace for
  // getAreasCode when getAreasCode is correct
  getTempMethodAreasCode(): string {
    let areaCodesCommaSeparated = '';

    const comparatorName = $('#compare-indicator-tabs>div>button.button-selected').text();

    if (comparatorName === 'All in England') {
      areaCodesCommaSeparated = '';
    } else {
      const areaCodes = this.ftHelperService.getAreasCodeDisplayed();
      areaCodesCommaSeparated = areaCodes.join(',');
    }

    return areaCodesCommaSeparated;
  }

  getAreasCode(): string {
    let areaCodesCommaSeparated = '';

    const comparatorName = this.ftHelperService.getCurrentComparator().Name;

    if (comparatorName === 'England') {
      areaCodesCommaSeparated = '';
    } else {
      const areaCodes = this.ftHelperService.getAreasCodeDisplayed();
      areaCodesCommaSeparated = areaCodes.join(',');
    }

    return areaCodesCommaSeparated;
  }

  getIids(): number[] {
    const iids: number[] = [];

    const iid = this.ftHelperService.getIid();
    iids.push(iid);

    const iidComparisionString = this.getComparasionIndicatorId();
    if (iid !== iidComparisionString) {
      iids.push(Number(iidComparisionString));
    }

    return iids;
  }

  getComparatorSexId(): number {
    return this.selectedOptions[0].SexId;
  }

  getComparatorAgeId(): number {
    return this.selectedOptions[0].AgeId;
  }

  getComparasionIndicatorId(): number {
    return this.selectedOptions[0].IndicatorId;
  }

  getParentArea(): string {
    const comparatorName = this.ftHelperService.getCurrentComparator().Name;

    if (comparatorName !== 'England') {
      return this.ftHelperService.getParentArea().Code;
    }
    return null;
  }
}
