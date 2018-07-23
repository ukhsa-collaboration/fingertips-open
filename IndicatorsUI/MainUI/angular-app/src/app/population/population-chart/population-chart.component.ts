import { Component, Input, ViewChild, SimpleChanges, ElementRef } from '@angular/core';
import { Populations, PopulationMaxFinder } from '../population';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { AreaCodes, SexIds, HC, Colour } from '../../shared/shared';
import {
  FTModel, Population
} from '../../typings/FT.d';
import * as _ from 'underscore';

import * as Highcharts from 'highcharts';
require('highcharts/modules/exporting')(Highcharts);
require('highcharts/highcharts-more')(Highcharts);

@Component({
  selector: 'ft-population-chart',
  templateUrl: './population-chart.component.html',
  styleUrls: ['./population-chart.component.css']
})
export class PopulationChartComponent {

  @Input() populations: Populations;
  private chartTitle: string;
  private chartColours = {
    label: '#333',
    bar: '#76B3B3',
    pink: '#FF66FC'
  }
  private chartTextStyle;

  @ViewChild('chart') public chartEl: ElementRef;
  chart: Highcharts.ChartObject;

  constructor(private ftHelperService: FTHelperService) {
    this.chartTextStyle = {
      color: this.chartColours.label,
      fontWeight: 'normal',
      fontFamily: 'Verdana'
    };
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.isAnyData()) {
      let chartContainer = null;
      if (this.chartEl && this.chartEl.nativeElement) {
        chartContainer = this.chartEl.nativeElement;
        this.chart = new Highcharts.Chart(chartContainer, this.getChartOptions());
      }
    }
  }

  private isAnyData(): boolean {
    return !_.isUndefined(this.populations);
  }

  private getChartOptions(): any /*Highcharts.Options*/ {
    var model: FTModel = this.ftHelperService.getFTModel();
    let max = new PopulationMaxFinder().getMaximumValue(this.populations);

    // Populations
    let areaPopulation = this.populations.areaPopulation;
    let areaParentPopulation = this.populations.areaParentPopulation;
    let nationalPopulation = this.populations.nationalPopulation;

    // Labels
    let subtitle = nationalPopulation.IndicatorName + " " + nationalPopulation.Period;
    let maleString = ' (Male)';
    let femaleString = ' (Female)';
    this.chartTitle = '<div style="text-align:center;">Age Profile<br><span style="font-size:12px;">' + subtitle + '</span></div>';

    let series = [];

    // Area series
    var areaName = this.ftHelperService.getAreaName(model.areaCode);
    series.push(
      {
        name: areaName + maleString,
        data: areaPopulation.Values[SexIds.Male],
        type: 'bar',
        color: Colour.bobLower
      }, {
        name: areaName + femaleString,
        data: areaPopulation.Values[SexIds.Female],
        type: 'bar',
        color: Colour.bobHigher
      }
    );

    // Parent area series
    var isParentNotEngland = model.parentCode.toUpperCase() !== AreaCodes.England;
    if (isParentNotEngland) {
      var parentAreaName = this.ftHelperService.getParentArea().Name;
      let parentColour = this.chartColours.pink;
      series.push({
        name: parentAreaName,
        data: areaParentPopulation.Values[SexIds.Male],
        color: parentColour,
      },
        {
          name: parentAreaName + femaleString,
          data: areaParentPopulation.Values[SexIds.Female],
          color: parentColour,
          showInLegend: false
        });
    }

    // England series
    series.push(
      {
        name: 'England',
        data: nationalPopulation.Values[SexIds.Male],
        color: Colour.comparator
      },
      {
        name: 'England' + femaleString,
        data: nationalPopulation.Values[SexIds.Female],
        color: Colour.comparator,
        showInLegend: false
      }
    );

    return (
      {
        chart: {
          defaultSeriesType: 'line',
          margin: [60, 55, 150, 55],
          /* margins must be set explicitly to avoid labels being positioned outside visible chart area */
          width: 600,
          height: 550
        },
        title: {
          text: this.chartTitle,
          style: this.chartTextStyle,
          useHTML: true
        },
        xAxis: [{
          categories: nationalPopulation.Labels,
          reversed: false
        }, { // mirror axis on right side
          opposite: true,
          reversed: false,
          categories: nationalPopulation.Labels,
          linkedTo: 0
        }
        ],
        yAxis: [{
          min: -max,
          max: max,
          title: {
            text: '% of total population',
            style: this.chartTextStyle
          },
          labels: {
            formatter: function () {
              return Math.abs(this.value);
            }
          }
        }],
        plotOptions: {
          series: {
            events: {
              legendItemClick: function () {
                return false;
              }
            }
          },
          bar: {
            borderWidth: 0,
            pointPadding: 0,
            stacking: 'normal',
            animation: false
          },
          line: {
            // The symbol is a non-valid option here to work round a bug
            // in highcharts where only half of the markers appear on hover
            marker: HC.noLineMarker,
            animation: false,
            states: {
              hover: {
                marker: HC.noLineMarker
              }
            }
          }
        },
        legend: {
          enabled: true,
          layout: 'vertical',
          align: 'center',
          verticalAlign: 'bottom',
          itemStyle: {
            fontWeight: 'normal'
          }
        },
        tooltip: {
          formatter: function () {

            // Get series name
            let name = this.series.name;
            let alreadyHasSexLabel = new RegExp(/e[\)]$/);
            if (!alreadyHasSexLabel.test(name)) {
              name += maleString;
            }

            return '<b>' + name + '<br>Age: ' +
              this.point.category + '</b><br/>' +
              'Population: ' + Math.abs(this.point.y) + '%';
          }
        },
        credits: HC.credits,
        exporting: {
          enabled: false
        },
        series: series
      });
  }

  public exportChart() {

    let chartTitle = this.chartTitle;
    this.chart.exportChart({ type: 'image/png' }, {
      chart: {
        spacingTop: 70,
        events: {
          load: function () {
            this.renderer.text(chartTitle, 300, 15)
              .attr({
                align: 'center'
              })
              .css({
                fontSize: '14px',
                width: '600px'
              })
              .add();
          }
        }
      },
      title: {
        text: '',
        style: this.chartTextStyle
      }
    });

    this.ftHelperService.logEvent('ExportImage', 'Population');
  }
}
