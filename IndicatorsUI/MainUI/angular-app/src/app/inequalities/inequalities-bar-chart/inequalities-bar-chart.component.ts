import {
  Component, OnChanges, SimpleChanges, Input, ViewChild, ElementRef, Output, EventEmitter
} from '@angular/core';
import * as Highcharts from 'highcharts';
import { BuilderData } from '../inequalities';
import { CoreDataHelperService } from '../../shared/service/helper/coreDataHelper.service';
import * as _ from 'underscore';
import { HC } from '../../shared/constants';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { CommaNumber } from '../../shared/shared';
import { isDefined } from '@angular/compiler/src/util';

@Component({
  selector: 'ft-inequalities-bar-chart',
  templateUrl: './inequalities-bar-chart.component.html',
  styleUrls: ['./inequalities-bar-chart.component.css']
})
export class InequalitiesBarChartComponent implements OnChanges {

  @Input() barChartData: BuilderData;
  @Output() emitBarChart = new EventEmitter();
  @ViewChild('chart', { static: true }) public chartEl: ElementRef;
  chart: Highcharts.ChartObject;

  constructor(private ftHelper: FTHelperService, private coreDataHelper: CoreDataHelperService) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['barChartData']) {
      if (this.barChartData) {
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
        this.emitBarChart.emit({ 'chart': this.chart });
      }
    }
  }

  private getChartOptions(): Highcharts.Options {

    const barChartData = this.barChartData;
    const getTooltipHtml = this.getTooltipHtml;
    const shortCategoryLabels = this.shortCategoryLabels;
    const barThicknessCalculator = this.barThicknessCalculator;
    const barLabelOffsetCalculator = this.barLabelOffsetCalculator;
    const getSeriesData = this.getSeriesData;
    const coreDataHelper = this.coreDataHelper;
    const ftHelper = this.ftHelper;

    const itemStyle = { color: '#333333', fontWeight: 'normal', textShadow: '0' };

    return (
      {
        chart: {
          defaultSeriesType: 'bar',
          zoomType: 'xy',
          width: 700,
          marginRight: 20
        },
        credits: HC.Credits,
        title: {
          text: ''
        },
        xAxis: {
          categories: shortCategoryLabels(barChartData.labels),
          labels: {
            style: itemStyle,
            labels: {
              step: 1
            }
          }
        },
        yAxis: {
          title: {
            text: barChartData.unit.Label,
            style: itemStyle,
          }
        },
        plotOptions: {
          series: {
            animation: false,
            events: {
              legendItemClick: function () {
                return false;
              }
            }
          },
          bar: {
            borderColor: '#333',
            pointPadding: 0,
            pointWidth: barThicknessCalculator(barChartData.labels.length),
            stacking: 'normal',
            shadow: false,
            dataLabels: {
              allowOverlap: true,
              enabled: true,
              style: itemStyle,
              align: 'right',
              y: 1, // Ignored IE 6-8
              x: barLabelOffsetCalculator(barChartData.dataSeries, ftHelper),
              formatter: function () {
                return this.y === 0 || barChartData.showConfidenceIntervalBars ? '' : new CommaNumber(this.point.valF).unrounded();
              }
            }
          },
          line: {
            // The symbol is a non-valid option here to work round a bug
            // in highcharts where only half of the markers appear on hover
            borderWidth: 0,
            pointPadding: 0,
            marker: {
              enabled: false,
              symbol: 'x'
            },
            states: {
              hover: {
                marker: {
                  enabled: false,
                  symbol: 'x'
                }
              }
            }
          }
        },
        legend: {
          enabled: true,
          borderWidth: 0,
          layout: 'vertical',
          itemStyle: itemStyle
        },
        tooltip: {
          formatter: function () {
            return getTooltipHtml(this, barChartData.unit, coreDataHelper);
          }
        },
        series: getSeriesData(barChartData, ftHelper),
        exporting: {
          enabled: true,
          allowHTML: true,
          chartOptions: {
            title: {
              text: barChartData.indicatorName + ' (' + barChartData.timePeriod + ') - '
                + barChartData.areaName + ' ' + barChartData.partitionName,
              style: {
                fontSize: '10px',
                align: 'center'
              }
            }
          }
        }
      });
  }

  getSeriesData(barChartData: BuilderData, ftHelper: FTHelperService): any {
    const seriesData = [];

    if (barChartData.showAverageLine) {
      seriesData.push({ data: barChartData.dataSeries, name: 'data', type: 'bar', showInLegend: false });
      seriesData.push({ data: barChartData.averageDataSeries, name: barChartData.averageLegend, type: 'line', showInLegend: true });
    } else {
      seriesData.push({ data: barChartData.dataSeries, name: 'data', type: 'bar', showInLegend: false });
    }

    if (barChartData.showConfidenceIntervalBars) {
      const cis = [];

      barChartData.dataList.forEach(data => {
        const dataInfo = ftHelper.newCoreDataSetInfo(data);

        if (dataInfo.isValue()) {
          if (dataInfo.areCIs()) {
            cis.push([data.LoCI, data.UpCI]);
          } else {
            cis.push(null);
          }
        }
      });

      seriesData.push({ data: cis, name: 'Errorbars', type: 'errorbar', animation: false });
    }

    return seriesData;
  }

  barLabelOffsetCalculator(dataList: any[], ftHelper: FTHelperService): number {
    const maxData = _.max(dataList, function (d) { return d.y; });

    // e.g. 22.2
    let offset = 29;

    if (ftHelper.newCoreDataSetInfo(maxData).isValue()) {
      const valFLength = maxData.valF.length;

      if (valFLength === 5) {
        offset = 36;
      }
      if (valFLength === 3) {
        if (maxData.valF.indexOf('.') > -1) {
          offset = 22;
        } else {
          offset = 25;
        }
      }
    }

    return offset + 1;
  }

  barThicknessCalculator(numberOfBars: number): number {
    let thickness: number;

    if (numberOfBars < 4) {
      // e.g. Sex
      thickness = 30;
    } else if (numberOfBars < 6) {
      // e.g. Sexuality
      thickness = 20;
    } else {
      thickness = 11;
    }

    return thickness;
  }

  shortCategoryLabels(labels: string[]): string[] {
    const trimmedLabels: string[] = [];
    let trimmedLabel: string;

    labels.forEach(label => {
      if (label.length > 30) {
        trimmedLabel = label.substr(0, 30) + '...';
      } else {
        trimmedLabel = label;
      }

      trimmedLabels.push(trimmedLabel);
    });

    return trimmedLabels;
  }

  getTooltipHtml(c: any, unit: any, coreDataHelper: CoreDataHelperService) {
    const isAverageSeries = c.series.index === 1;
    const point = c.point;

    let category = point.category;
    if (isAverageSeries) {
      category = c.point.series.data['0'].series.name;
    }

    let htmlTooltipMessage;
    if (isDefined(point.valF) && isDefined(point.tooltipValue)) {
      htmlTooltipMessage = '<b>' + point.tooltipValue + '<b><br><i>' + category + '</i>';
    }

    if (isDefined(point.noteValue)) {
      htmlTooltipMessage = htmlTooltipMessage.concat('<br><br>*' + point.noteValue);
    }
    return htmlTooltipMessage;
  }

  isAnyData(): boolean {
    if (this.barChartData.dataSeries.length > 0) {
      return true;
    } else {
      return false;
    }
  }
}
