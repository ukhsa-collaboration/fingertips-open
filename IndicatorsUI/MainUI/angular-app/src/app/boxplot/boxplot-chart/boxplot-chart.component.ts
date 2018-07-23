import { Component, Input, OnChanges, SimpleChanges, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { BoxplotData } from '../boxplot';

import * as Highcharts from 'highcharts';
require('highcharts/modules/exporting')(Highcharts);
require('highcharts/highcharts-more')(Highcharts);

@Component({
  selector: 'ft-boxplot-chart',
  templateUrl: './boxplot-chart.component.html',
  styleUrls: ['./boxplot-chart.component.css']
})
export class BoxplotChartComponent implements OnChanges, AfterViewInit {

  @Input() boxplotData: BoxplotData;
  @ViewChild('chart') public chartEl: ElementRef;
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
      }
    }
  }

  private getChartOptions(): Highcharts.Options {

    var metadata = this.boxplotData.metadata;
    var unitLabel = metadata.Unit.Label;

    // Series
    var seriesName = this.boxplotData.areaTypeName + ' in ' + this.boxplotData.comparatorName;
    let series: any[] /*Highcharts.IndividualSeriesOptions[]*/ = [
      {
        name: seriesName,
        data: this.getChartData()
      }
    ];

    // So accessible from tooltip
    var boxplotData = this.boxplotData;

    return (
      {
        chart: {
          type: 'boxplot',
          width: 820,
          animation: false
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
            text: unitLabel
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

            let period = this.x;
            var index = boxplotData.periods.indexOf(period);
            let stats = boxplotData.statsFormatted[index];

            var tooltipContent = [
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

        series: series
      });
  }

  private getChartData(): number[][] {

    let chartDataGrid: number[][] = [];

    for (var stats of this.boxplotData.stats) {

      let pointData: number[] = [];
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
