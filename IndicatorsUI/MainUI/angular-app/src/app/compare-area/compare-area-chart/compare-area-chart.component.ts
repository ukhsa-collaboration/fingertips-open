import { Component, OnChanges, SimpleChanges, Input, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { Colour, CommaNumber } from '../../shared/shared';
import { FunnelPlotChartData } from '../compare-area';
import { CoreDataHelperService } from '../../shared/service/helper/coreDataHelper.service';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import * as Highcharts from 'highcharts';
import { CoreDataHelper } from '../../typings/FT';

@Component({
  selector: 'ft-compare-area-chart',
  templateUrl: './compare-area-chart.component.html',
  styleUrls: ['./compare-area-chart.component.css']
})
export class CompareAreaChartComponent implements OnChanges {

  @Input() chartData: FunnelPlotChartData;
  @Output() emitAreaCodeSelected = new EventEmitter();

  @ViewChild('chart', { static: true }) public chartEl: ElementRef;
  chart: Highcharts.ChartObject;

  constructor(private ftHelper: FTHelperService, private coreDataHelper: CoreDataHelperService) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['chartData']) {
      if (this.chartData) {
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
      }
    }
  }

  private getChartOptions(): Highcharts.Options {
    const ftHelper = this.ftHelper;
    const coreDataHelper = this.coreDataHelper;
    const getTooltipHtml = this.getTooltipHtml;
    const emitAreaCodeSelected = this.emitAreaCodeSelected;
    const chartData = this.chartData;

    return (
      {
        chart: {
          type: 'spline',
          zoomType: 'xy',
          width: 400,
          height: 450
        },
        title: {
          text: ''
        },
        xAxis: {
          title: {
            enabled: true,
            text: chartData.isDsr ? 'Effective population' : 'Population'
          },
          startOnTick: false,
          endOnTick: false,
          showLastLabel: true,
          gridLineWidth: 1,
          minPadding: 0.025,
          maxPadding: 0.025
        },
        yAxis: {
          title: {
            text: chartData.unit.Label
          },
          endOnTick: false,
          startOnTick: false
        },
        legend: {
          enabled: true,
          layout: 'vertical',
          borderWidth: 0
        },
        plotOptions: {
          scatter: {
            allowPointSelect: true,
            animation: false,
            marker: {
              lineColor: '#000000',
              lineWidth: 1,
              radius: 5,
              symbol: 'circle',
              states: {
                hover: {
                  lineColor: '#444444'
                },
                select: {
                  lineColor: '#000000',
                  fillColor: '#555555',
                  lineWidth: 2,
                  radius: 7
                }
              }
            },
            states: {
              hover: {
                marker: {
                  enabled: false
                }
              }
            },
            events: {
              mouseOut: function () {
                emitAreaCodeSelected.emit('none');
              }
            }
          },
          spline: {
            enableMouseTracking: false,
            lineWidth: 1,
            animation: false,
            marker: { enabled: false },
            events: {
              mouseOut: function () {
                emitAreaCodeSelected.emit('none');
              }
            }
          },
          line: {
            enableMouseTracking: false,
            lineWidth: 1,
            animation: false,
            marker: { enabled: false },
            events: {
              mouseOut: function () {
                emitAreaCodeSelected.emit('none');
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
            return getTooltipHtml(this, chartData, coreDataHelper, emitAreaCodeSelected);
          }
        },
        exporting: {
          enabled: true,
          allowHTML: true,
          chartOptions: {
            title: {
              text: chartData.indicatorName + ' (' + chartData.timePeriod + ') - ' +
                chartData.areaTypeName + ' in ' + chartData.parentAreaName,
              style: {
                fontSize: '10px',
                align: 'center'
              }
            }
          }
        },
        series: [
          {
            color: Colour.comparator, data: chartData.comparatorLineData, showInLegend: chartData.isComparatorValue, type: 'line',
            name: chartData.currentComparatorName
          },
          { color: Colour.limit99, data: chartData.u3LineData, showInLegend: false, type: 'spline' },
          { color: Colour.limit95, data: chartData.u2LineData, showInLegend: false, type: 'spline' },
          {
            color: Colour.limit95, data: chartData.l2LineData, showInLegend: chartData.showLimits, type: 'spline',
            name: '95.0% Confidence'
          },
          {
            color: Colour.limit99, data: chartData.l3LineData, showInLegend: chartData.showLimits, type: 'spline',
            name: '99.8% Confidence'
          },
          { color: chartData.colourBetter, data: chartData.better, showInLegend: false, type: 'scatter', name: 'better' },
          { color: Colour.ragSame, data: chartData.same, showInLegend: false, type: 'scatter', name: 'same' },
          { color: chartData.colourWorse, data: chartData.worse, showInLegend: false, type: 'scatter', name: 'worse' },
          { color: Colour.none, data: chartData.none, showInLegend: false, type: 'scatter' }
        ]
      });
  }

  isAnyData(): boolean {
    if (this.chartData.areas.length > 0) {
      return true;
    } else {
      return false;
    }
  }

  onExportChartAsImage() {
    this.chart.exportChart({ type: 'image/png' }, {});
  }

  getTooltipHtml(c: any, chartData: FunnelPlotChartData, coreDataHelper: CoreDataHelper,
    emitAreaCodeSelected: EventEmitter<{}>): string {

    let htmlTooltipMessage = '';
    const point = c.point;
    const unit = chartData.unit;
    const areas = chartData.areas;
    const areaCode = point.data.AreaCode;
    const areaName = areas.find(x => x.Code === areaCode).Name;

    // Emit the area code to the parent page so the
    // corresponding table row can be highlighted
    emitAreaCodeSelected.emit(areaCode);

    htmlTooltipMessage = '<b>' + areaName + '</b>' +
      '<br>Value: ' + coreDataHelper.valueWithUnit(unit).getFullLabel(point.data.ValF) +
      '<br>Population: ' + new CommaNumber(point.x).rounded();

    return htmlTooltipMessage;
  }
}
