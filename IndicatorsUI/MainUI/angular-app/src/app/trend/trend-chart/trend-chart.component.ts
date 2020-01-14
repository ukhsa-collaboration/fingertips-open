import { Component, OnChanges, Input, SimpleChanges, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import * as Highcharts from 'highcharts';
import { TrendData, ViewModes } from '../trend';
import { Colour } from '../../shared/shared';
import { HC } from '../../shared/constants';
import { CoreDataHelperService } from '../../shared/service/helper/coreDataHelper.service';
import { HighchartsChartComponent } from 'highcharts-angular';

@Component({
  selector: 'ft-trend-chart',
  templateUrl: './trend-chart.component.html',
  styleUrls: ['./trend-chart.component.css']
})
export class TrendChartComponent implements OnChanges {

  @Input() trendData: TrendData;
  @Output() emitAreaCodeSelected = new EventEmitter();
  @Output() emitPeriodSelected = new EventEmitter();
  @Output() emitChart = new EventEmitter();
  @ViewChild('chart', { static: true }) public chartEl: ElementRef;
  chart: Highcharts.ChartObject;

  constructor(private coreDataHelper: CoreDataHelperService) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['trendData']) {
      if (this.trendData) {
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
        this.emitChart.emit({ 'chartId': this.trendData.chartId, 'chart': this.chart });
      }
    }
  }

  private getChartOptions(): Highcharts.Options {
    const coreDataHelper = this.coreDataHelper;
    const trendData = this.trendData;
    const getRadius = this.getRadius;
    const getLabelStep = this.getLabelStep;
    const emitAreaCodeSelected = this.emitAreaCodeSelected;
    const emitPeriodSelected = this.emitPeriodSelected;

    return (
      {
        chart: {
          type: 'line',
          zoomType: 'xy',
          width: trendData.width,
          height: trendData.height,
          events: {
            click: function (e) {
              emitAreaCodeSelected.emit(trendData.areaCode);
            }
          }
        },
        credits: HC.Credits,
        title: {
          text: ''
        },
        xAxis: {
          title: {
            enabled: true
          },
          categories: trendData.labels,
          labels: {
            formatter: function () {
              // Format period so fits on x axis
              let period = this.value;

              if (period.length > 22) {
                // e.g. '2009/10 Q2 - 2010/11 Q1' to '09/10 Q2 - 10/11 Q1' (don't completely replace 2020)
                period = period
                  .replace(/203/g, '3')
                  .replace(/201/g, '1')
                  .replace(/200/g, '0');
              }

              return period.indexOf('-') > -1 ?
                period.replace('-', '<br>-') /*Break after - for ranges*/ :
                period.replace(' ', '<br/>');
            },
            enabled: trendData.displayXAxisLabel,
            step: getLabelStep(trendData.labels),
            maxStaggerLines: 1
          },
          tickLength: 3,
          tickPosition: 'outside',
          tickWidth: 1,
          tickmarkPlacement: 'on'
        },
        yAxis: {
          title: {
            text: trendData.unit.Label,
            enabled: trendData.displayYAxisLabel
          },
          max: trendData.max,
          min: trendData.min
        },
        legend: {
          enabled: trendData.displayLegend,
          layout: 'vertical',
          borderWidth: 0
        },
        plotOptions: {
          line: {
            enableMouseTracking: true,
            lineWidth: 2,
            animation: false,
            marker: {
              radius: getRadius(trendData.viewMode, trendData.trendDataPointsCount, trendData.radius),
              symbol: 'circle',
              lineWidth: 1,
              lineColor: '#000000'
            },
            events: {
              mouseOut: function () {
                emitPeriodSelected.emit({ 'period': 'none', 'areaCode': 'none', 'chartId': trendData.chartId });
              }
            }
          },
          series: {
            events: {
              legendItemClick: function () {
                return false;
              }
            }
          }
        },
        tooltip: {
          formatter: function () {
            const data = this.point.data;

            // Data object type may be CoreDataSet or TrendDataPoint
            const valF = !!data['V'] ? data.V : data.ValF;

            const unit = trendData.unit;
            const value = coreDataHelper.valueWithUnit(unit).getFullLabel(valF);

            emitPeriodSelected.emit({ 'period': this.x, 'areaCode': this.point.data.AreaCode, 'chartId': trendData.chartId });

            return '<b>' + value + '</b> ' +
              '<br/><i>' + this.series.name + '</i><br/>' + this.x;
          }
        },
        series: [
          {
            data: trendData.benchmarkPoints,
            color: Colour.comparator,
            name: trendData.comparatorName
          },
          {
            data: trendData.areaPoints,
            name: trendData.areaName,
            showInLegend: false
          },
          {
            name: trendData.areaName,
            type: 'errorbar',
            animation: false,
            data: trendData.ciPoints,
            visible: trendData.showConfidenceBars
          }
        ],
        exporting: {
          enabled: true,
          allowHTML: true,
          chartOptions: {
            title: {
              text: trendData.indicatorName + ' for ' + trendData.areaName,
              style: {
                fontSize: '10px',
                align: 'center'
              }
            }
          }
        }
      });
  }

  getRadius(viewMode: number, trendDataPointsCount: number, radius: number) {
    if (viewMode === ViewModes.multiArea) {
      if (trendDataPointsCount > 40) {
        return 1;
      } else {
        return 3;
      }
    }

    return radius;
  }

  getLabelStep(labels: string[]): number {
    let labelStep = 1;
    const labelsLength = labels.length;

    if (labelsLength > 0) {

      const firstLabelLength = labels[0].length;

      if (firstLabelLength > 10) {
        // e.g. "2008/09 Q3"
        labelStep = Math.ceil(labelsLength / 4);
      } else if (firstLabelLength > 7) {
        // e.g. "2008 - 09"
        labelStep = Math.ceil(labelsLength / 5);
      } else {
        // e.g. "2004", "2008/09"
        labelStep = Math.ceil(labelsLength / 6);
      }
    }

    return labelStep;
  }

  toggleViewMode(viewMode: number, areaCode: string) {
    if (viewMode === ViewModes.multiArea) {
      this.emitAreaCodeSelected.emit(areaCode);
    }
  }

  isAnyData(): boolean {
    if (this.trendData.areaPoints.length > 0) {
      return true;
    } else {
      return false;
    }
  }
}
