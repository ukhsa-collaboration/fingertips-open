import {
  Component, OnChanges, SimpleChanges, Input, ViewChild, ElementRef, Output, EventEmitter
} from '@angular/core';
import * as Highcharts from 'highcharts';
import { TrendChartData } from '../inequalities';
import { isDefined } from '@angular/compiler/src/util';
import { CoreDataHelperService } from '../../shared/service/helper/coreDataHelper.service';
import { HC } from '../../shared/constants';

@Component({
  selector: 'ft-inequalities-trend-chart',
  templateUrl: './inequalities-trend-chart.component.html',
  styleUrls: ['./inequalities-trend-chart.component.css']
})
export class InequalitiesTrendChartComponent implements OnChanges {

  @Input() trendChartData: TrendChartData;
  @Output() emitTrendChart = new EventEmitter();
  @ViewChild('chart', { static: true }) public chartEl: ElementRef;
  chart: Highcharts.ChartObject;

  constructor(private coreDataHelper: CoreDataHelperService) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['trendChartData']) {
      if (this.trendChartData) {
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
        this.emitTrendChart.emit({ 'chart': this.chart });
      }
    }
  }

  private getChartOptions(): Highcharts.Options {

    const trendChartData = this.trendChartData;
    const coreDataHelper = this.coreDataHelper;

    return (
      {
        chart: {
          type: 'line',
          zoomType: 'xy',
          width: trendChartData.width,
          height: trendChartData.height,
          animation: false
        },
        credits: HC.Credits,
        title: {
          text: ''
        },
        xAxis: {
          title: {
            enabled: false
          },
          categories: trendChartData.periods,
          enabled: true,
          step: 1
        },
        yAxis: {
          min: trendChartData.min,
          max: trendChartData.max,
          title: {
            text: trendChartData.unit.Label,
          }
        },
        legend: {
          enabled: true,
          layout: 'vertical',
          borderWidth: 0
        },
        plotOptions: {
          line: {
            enableMouseTracking: true,
            lineWidth: 1,
            animation: false,
            marker: {
              radius: 3,
              symbol: 'circle',
              lineWidth: 1,
              lineColor: '#000000'
            },
            events: {
              mouseOut: function () {
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
            let htmlTooltipMessage;
            if (isDefined(this.point.valF) && isDefined(this.point.tooltipValue)) {
              htmlTooltipMessage = '<b>' + this.point.tooltipValue + '</b><br/><i>' + this.series.name + '</i><br/>' + this.point.category;
            }
            if (isDefined(this.point.noteValue)) {
              htmlTooltipMessage = htmlTooltipMessage.concat('<br><br>*' + this.point.noteValue);
            }
            return htmlTooltipMessage;
          }
        },
        series: trendChartData.seriesData,
        exporting: {
          enabled: true,
          allowHTML: true,
          chartOptions: {
            title: {
              text: trendChartData.indicatorName + ' - ' + trendChartData.areaName + ' ' + trendChartData.partitionName,
              style: {
                fontSize: '10px',
                align: 'center'
              }
            }
          }
        }
      });
  }

  isAnyData(): boolean {
    if (this.trendChartData.seriesData.length > 0) {
      return true;
    } else {
      return false;
    }
  }
}
