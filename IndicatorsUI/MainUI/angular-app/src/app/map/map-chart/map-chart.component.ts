import { Component, ElementRef, ViewChild, Input, Output, SimpleChanges, OnChanges, EventEmitter } from '@angular/core';
import {
    FTModel, FTRoot, Area, GroupRoot, CoreDataSet, CoreDataHelper, Unit, ValueWithUnit, ValueNote,
    IndicatorMetadataHash, IndicatorMetadata
} from '../../typings/FT.d';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { CoreDataHelperService } from '../../shared/service/helper/coreDataHelper.service';
import { AreaCodes } from '../../shared/shared'
import * as Highcharts from 'highcharts';
require('highcharts/modules/exporting')(Highcharts);
require('highcharts/highcharts-more')(Highcharts);
import * as _ from 'underscore';

@Component({
    selector: 'ft-map-chart',
    templateUrl: './map-chart.component.html',
    styleUrls: ['./map-chart.component.css']
})
export class MapChartComponent implements OnChanges {
    @ViewChild('mapChart') public chartEl: ElementRef;
    @Input() sortedCoreData: Map<string, CoreDataSet> = null;
    @Input() areaTypeId: number = null;
    @Input() currentAreaCode = null;
    @Input() areaCodeColour = null;
    @Input() isBoundaryNotSupported;
    @Output() selectedAreaCodeChanged = new EventEmitter();
    chart: Highcharts.ChartObject;
    chartData: { color: String, x: number, key: string, y: number }[] = [];
    @Output() hoverAreaCodeChanged = new EventEmitter();
    constructor(private ftHelperService: FTHelperService,
        private coreDataHelperService: CoreDataHelperService) { }

    ngOnChanges(changes: SimpleChanges) {

        if (changes['isBoundaryNotSupported']) {
            let localBoundryNotSupported = changes['isBoundaryNotSupported'].currentValue;
            if (localBoundryNotSupported !== undefined) {
                if (localBoundryNotSupported) {
                    this.chartData = [];
                }
            }
        }
        if (changes['sortedCoreData']) {
            let sortedataChange = changes['sortedCoreData'].currentValue;
            if (sortedataChange) {
                this.loadHighChart(sortedataChange);
            }
        }
        if (changes['currentAreaCode']) {
            let currentAreaCode = changes['currentAreaCode'].currentValue;
            if (currentAreaCode) {
                this.highlightArea(currentAreaCode);
            }
        }
        if (changes['areaCodeColour']) {
            let areaCodeColour = changes['areaCodeColour'].currentValue;
            if (areaCodeColour) {
                if (this.sortedCoreData) {
                    this.loadHighChart(this.sortedCoreData);
                }
            }
        }
    }
    loadHighChart(sortedata: Map<string, CoreDataSet>) {

        let xVal = 0;
        let xAxisCategories: {
            AreaName: String, ValF: string, LoCIF: string,
            UpCIF: string, AreaCode: string, NoteId: number
        }[] = [];
        let errorData = [];
        this.chartData = [];
        let regionValues = [];
        const x: number = 0;
        let currentGrpRoot: GroupRoot = this.ftHelperService.getCurrentGroupRoot();
        let unit = this.ftHelperService.getMetadata(currentGrpRoot.IID).Unit;
        let valuesForBarChart: any[] = [];
        let extraTooltip = '';

        Object.keys(sortedata).forEach(key => {
            let value: CoreDataSet = sortedata[key];
            let colour = null;
            if (this.areaCodeColour) {
                colour = this.areaCodeColour.get(key);
            }
            if (value.ValF !== '-') {
                valuesForBarChart.push({
                    Colour: colour
                    , AreaCode: value.AreaCode
                    , Val: value.Val
                    , LCI: value.LoCI
                    , UCI: value.UpCI
                    , ValF: value.ValF
                    , LoCIF: value.LoCIF
                    , UpCIF: value.UpCIF
                    , NoteId: value.NoteId
                });
            }
        });

        valuesForBarChart.sort((leftside, righside): number => {
            if (leftside.Val < righside.Val) { return -1; }
            if (leftside.Val > righside.Val) { return 1; }
            return 0;
        });

        for (let value of valuesForBarChart) {
            this.chartData.push({ color: value.Colour, x: xVal, key: value.AreaCode, y: value.Val });
            xVal++;

            let areaName = this.ftHelperService.getAreaName(value.AreaCode);
            xAxisCategories.push({
                AreaName: areaName, ValF: value.ValF, LoCIF: value.LoCIF,
                UpCIF: value.UpCIF, AreaCode: value.AreaCode, NoteId: value.NoteId
            });

            errorData.push([value.LoCI, value.UpCI]);
        }
        let series: any[] /*Highcharts.IndividualSeriesOptions[]*/ = [
            {
                type: 'column',
                name: 'Value',
                data: this.chartData
            }
            ,
            {
                type: 'errorbar',
                name: 'My Errors',
                data: errorData,
                zIndex: 1000,
                color: '#666666'
            }
        ];

        const comparatorName: string = this.ftHelperService.getCurrentComparator().Name;
        const valueWithUnit: ValueWithUnit = this.coreDataHelperService.valueWithUnit(unit);
        for (let grouping of currentGrpRoot.Grouping) {

            if (grouping.ComparatorData.ValF === '-') { continue; }
            let valF = grouping.ComparatorData.ValF;
            let val = grouping.ComparatorData.Val;

            if (grouping.ComparatorId === 4 && grouping.ComparatorData.AreaCode === AreaCodes.England) {
                // England data
                regionValues[series.length] = valF;
                series.push({
                    type: 'line',
                    name: 'England',
                    color: '#333333',
                    data: [[0, val], [xVal - 1, val]]
                });
                extraTooltip += '<br>England: ' + valueWithUnit.getFullLabel(valF);
            } else if (grouping.ComparatorId !== 4 && grouping.ComparatorId === this.ftHelperService.getComparatorId()) {
                // Subnational data
                regionValues[series.length] = valF;
                series.push({
                    type: 'line',
                    name: comparatorName,
                    data: [[0, val], [xVal - 1, val]],
                    color: '#0000FF'
                });
                extraTooltip += '<br>' + comparatorName + ': ' + valueWithUnit.getFullLabel(valF);
            }
        }
        let yAxis = {
            labels: {
                formatter: function () {
                    return this.value;
                },
                style: {
                    color: '#999999'
                }
            },
            title: {
                text: ''
            }
        };
        const chartTitle: string = this.buildChartTitle();
        let valueNotes: Array<ValueNote> = [] = this.ftHelperService.getValueNotes();
        let areaCodeChanged = this.hoverAreaCodeChanged;
        let _thisLocal = this;

        // Locals for events
        let unhighlightArea = this.unhighlightArea;
        let chartData = this.chartData;

        const chartOptions: any /*Highcharts.Options*/ = {
            title: {
                text: chartTitle
            },
            credits: false,
            legend: {
                enabled: true,
                layout: 'vertical',
                borderWidth: 0
            },
            xAxis: {
                labels: { enabled: false },
                tickLength: 0
            },
            yAxis: yAxis,
            tooltip: {
                shared: false,
                formatter: function () {
                    if (this.series.type === 'line') {
                        return '<b>' + this.series.name + '</b><br />'
                            + valueWithUnit.getFullLabel(regionValues[this.series.index]);
                    }
                    let data = xAxisCategories[this.x];

                    _thisLocal.hoverAreaCodeChanged.emit({ areaCode: data.AreaCode });
                    let s = '<b>' + data.AreaName + '</b>';
                    s += '<br>' + valueWithUnit.getFullLabel(data.ValF);

                    if (data.NoteId !== undefined) {
                        s += '<br><em>' + valueNotes[data.NoteId].Text + '</em>';
                    }

                    if (data.LoCIF !== undefined) {
                        s += '<br>LCI: ' + valueWithUnit.getFullLabel(data.LoCIF);
                    }
                    if (data.UpCIF !== undefined) {
                        s += '<br>UCI: ' + valueWithUnit.getFullLabel(data.UpCIF);
                    }

                    s += '<br>Rank: ' + (this.x + 1);

                    s += extraTooltip;

                    return s;
                }
            },
            plotOptions: {
                column: {
                    pointPadding: 0,
                    showInLegend: false,
                    animation: false,
                    borderWidth: 0,
                    groupPadding: 0,
                    pointWidth: 470 / this.chartData.length, // width calculated to avoid white lines in the chart
                    shadow: false,
                    states: {
                        hover:
                        {
                            brightness: 0,
                            color: '#000000',
                        }
                    },
                    point: {
                        events: {
                            mouseOut: function (e) {
                                let areaCode = this.options.key;
                                _thisLocal.hoverAreaCodeChanged.emit({ areaCode: null });
                                unhighlightArea(chart, chartData, areaCode);
                            }
                        }
                    }
                },
                line: {
                    animation: false,
                    marker: {
                        enabled: false,
                    },
                    states: {
                        hover: {
                            lineWidth: 0,
                            marker:
                            {
                                enabled: false,
                                symbol: 'x'
                            }
                        }
                    }
                },
                errorbar: {
                    animation: false
                }
            },
            exporting: {
                enabled: false,
                chartOptions: {
                    title: {
                        text: ''
                    }
                }
            },
            series: series
        };

        let chartContainer = null;
        if (this.chartEl && this.chartEl.nativeElement) {
            chartContainer = this.chartEl.nativeElement;
        }
        this.chart = new Highcharts.Chart(chartContainer, chartOptions);
        let chart = this.chart;
    }

    highlightArea(areaCode: string): void {
        if (this.chart && this.chartData != null) {
            let data = _.where(this.chartData, { key: areaCode })[0];
            if (data !== undefined) {
                this.chart.series[0].data[data.x].select(true);
            }
        }
    }

    unhighlightArea(chart: Highcharts.ChartObject,
        chartData: any, areaCode: string): void {
        if (chart && chartData != null) {
            let data = _.where(chartData, { key: areaCode })[0];
            if (data !== undefined) {
                chart.series[0].data[data['x']].select(false);
            }
        }
    }

    onExportClick(event: MouseEvent) {
        let title = this.buildChartTitle();
        if (this.chart) {
            this.chart.exportChart({ type: 'image/png' }, {
                chart: {
                    spacingTop: 70,
                    height: 312,
                    width: 490,
                    events: {
                        load: function () {
                            this.renderer.text(title, 250, 15)
                                .attr({
                                    align: 'center'
                                })
                                .css({
                                    color: '#333',
                                    fontSize: '10px',
                                    width: '450px'
                                })
                                .add();
                        }
                    }
                }
            });
            this.ftHelperService.logEvent('ExportImage', 'MapBarChart');
        }
    }
    buildChartTitle(): string {
        const currentGrpRoot: GroupRoot = this.ftHelperService.getCurrentGroupRoot();
        const data: IndicatorMetadata = this.ftHelperService.getMetadata(currentGrpRoot.IID);
        const unit = data.Unit;
        const unitLabel = (typeof unit.Label !== 'undefined') ? unit.Label : '';
        const period: string = currentGrpRoot.Grouping[0].Period;
        return data.ValueType.Name + ' - ' + unitLabel + ' ' + period;
    }
}
