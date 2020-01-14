import {
  Component, Input, Output, OnChanges, SimpleChanges,
  ViewChild, ElementRef, AfterViewInit, EventEmitter
} from '@angular/core';
import { BoxplotData } from '../boxplot';
import { AreaCodes } from '../../shared/constants';
import * as Highcharts from 'highcharts';

@Component({
  selector: 'ft-boxplot-chart',
  templateUrl: './boxplot-chart.component.html',
  styleUrls: ['./boxplot-chart.component.css']
})
export class BoxplotChartComponent implements OnChanges, AfterViewInit {

  @Input() boxplotData: BoxplotData;
  @Output() emitBoxPlot = new EventEmitter();
  @ViewChild('chart', { static: true }) public chartEl: ElementRef;
  chart: Highcharts.ChartObject;

  ngAfterViewInit() {
    this.displayChart();
  }

  ngOnChanges(changes: SimpleChanges) {
    this.displayChart();
  }

  private displayChart() {
    if (this.isAnyData()) {
      let chartContainer = null;
      if (this.chartEl && this.chartEl.nativeElement) {
        chartContainer = this.chartEl.nativeElement;
        this.chart = new Highcharts.Chart(chartContainer, this.getChartOptions());
        this.emitBoxPlot.emit({ 'chart': this.chart });
      }
    }
  }

  private getChartOptions(): Highcharts.Options {

    const metadata = this.boxplotData.metadata;
    const yAxisLabel = metadata.Unit.Label;

    // Series
    const seriesName = this.boxplotData.isNearestNeighbour && this.boxplotData.comparatorCode !== AreaCodes.England
      ? this.boxplotData.comparatorName
      : this.boxplotData.areaTypeName + ' in ' + this.boxplotData.comparatorName;
    const series: any[] /*Highcharts.IndividualSeriesOptions[]*/ = [
      {
        name: seriesName,
        data: this.getChartData()
      }
    ];

    // So accessible from tooltip
    const boxplotData = this.boxplotData;

    return (
      {
        chart: {
          type: 'boxplot',
          width: 820,
          animation: false
        },

        credits: {
          enabled: false
        },

        title: {
          text: ''
        },

        legend: {
          enabled: false
        },

        xAxis: {
          categories: this.boxplotData.periods,
          title: {
            text: ''
          }
        },

        yAxis: {
          min: this.boxplotData.min,
          title: {
            text: yAxisLabel
          }
        },

        plotOptions: {
          boxplot: {
            animation: false,
            color: '#1e1e1e',
            fillColor: '#cccccc',
            medianColor: '#ff0000'
          }
        },

        tooltip: {
          followPointer: false,
          formatter: function () {

            const period = this.x;
            const index = boxplotData.periods.indexOf(period);
            const stats = boxplotData.statsFormatted[index];

            const tooltipContent = [
              '<b>', period, '</b><br/>',
              '<b>', seriesName, '</b><br/>',
              '95th Percentile: ', stats.P95, '<br/>',
              '75th Percentile: ', stats.P75, '<br/>',
              'Median: ', stats.Median, '<br/>',
              '25th Percentile: ', stats.P25, '<br/>',
              '5th Percentile: ', stats.P5, '<br/>'
            ];

            return tooltipContent.join('');
          }
        },
        exporting: {
          enabled: true,
          allowHTML: true,
          chartOptions: {
            title: {
              text: boxplotData.indicatorName + ' for ' + boxplotData.areaTypeName + ' in ' + boxplotData.comparatorName,
              style: {
                fontSize: '10px',
                align: 'center'
              }
            }
          }
        },
        series: series
      });
  }

  private getChartData(): number[][] {

    const chartDataGrid: number[][] = [];

    for (const stats of this.boxplotData.stats) {

      const pointData: number[] = [];
      pointData[0] = this.getValue(stats.P5);
      pointData[1] = this.getValue(stats.P25);
      pointData[2] = this.getValue(stats.Median);
      pointData[3] = this.getValue(stats.P75);
      pointData[4] = this.getValue(stats.P95);
      chartDataGrid.push(pointData);
    }
    return chartDataGrid;
  }

  private getValue(i: number): number {
    return i == null ? 0 : i;
  }

  private isAnyData(): boolean {
    return this.boxplotData && this.boxplotData.isAnyData();
  }
}
