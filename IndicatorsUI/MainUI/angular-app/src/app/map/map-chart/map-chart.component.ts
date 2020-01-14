import { Component, ElementRef, ViewChild, Input, Output, SimpleChanges, OnChanges, EventEmitter } from '@angular/core';
import * as _ from 'underscore';
import { isDefined } from '@angular/compiler/src/util';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { CoreDataHelperService } from '../../shared/service/helper/coreDataHelper.service';
import { AreaCodes } from '../../shared/constants';
import * as Highcharts from 'highcharts';
require('highcharts/modules/exporting')(Highcharts);
require('highcharts/highcharts-more')(Highcharts);
import { AreaHelper } from '../../shared/shared';
import { CoreDataSet, GroupRoot, ValueWithUnit } from '../../typings/FT';

@Component({
    selector: 'ft-map-chart',
    templateUrl: './map-chart.component.html',
    styleUrls: ['./map-chart.component.css']
})
export class MapChartComponent implements OnChanges {
    @ViewChild('mapChart', { static: true }) public chartEl: ElementRef;
    @Input() sortedCoreData: Map<string, CoreDataSet> = null;
    @Input() areaTypeId: number = null;
    @Input() currentAreaCode = null;
    @Input() areaCodeColour = null;
    @Input() subNationalTabButtonClicked = null;
    @Input() isBoundaryNotSupported;
    @Output() selectedAreaCodeChanged = new EventEmitter();
    @Output() hoverAreaCodeChanged = new EventEmitter();

    chart: Highcharts.ChartObject;
    chartData: { color: String, x: number, key: string, y: number }[] = [];
    isNearestNeighbours = false;

    constructor(private ftHelperService: FTHelperService,
        private coreDataHelperService: CoreDataHelperService) { }

    ngOnChanges(changes: SimpleChanges) {

        this.isNearestNeighbours = this.ftHelperService.isNearestNeighbours();

        if (changes['isBoundaryNotSupported']) {
            const isBoundaryNotSupported = changes['isBoundaryNotSupported'].currentValue;
            if (isDefined(isBoundaryNotSupported) && isBoundaryNotSupported) {
                this.chartData = [];
            }
        }
        if (changes['sortedCoreData']) {
            const sortedCoreData = changes['sortedCoreData'].currentValue;
            if (sortedCoreData) {
                this.loadHighChart(sortedCoreData);
            }
        }
        if (changes['currentAreaCode']) {
            const currentAreaCode = changes['currentAreaCode'].currentValue;
            if (currentAreaCode) {
                this.highlightArea(currentAreaCode);
            }
        }
        if (changes['areaCodeColour']) {
            const areaCodeColour = changes['areaCodeColour'].currentValue;
            if (areaCodeColour) {
                if (this.sortedCoreData) {
                    this.loadHighChart(this.sortedCoreData);
                }
            }
        }
    }

    loadHighChart(sortedata: Map<string, CoreDataSet>) {

        let xVal = 0;
        const xAxisCategories: {
            AreaName: String, ValF: string, LoCIF: string,
            UpCIF: string, AreaCode: string, NoteId: number
        }[] = [];
        const errorData = [];
        this.chartData = [];
        const regionValues = [];
        const currentGrpRoot: GroupRoot = this.ftHelperService.getCurrentGroupRoot();
        const unit = this.ftHelperService.getMetadata(currentGrpRoot.IID).Unit;
        const valuesForBarChart: any[] = [];
        let extraTooltip = '';

        // Mapping between parent and child areas if subnational comparator
        let areaMappings = null;
        if (this.subNationalTabButtonClicked) {
            const parentTypeId = this.ftHelperService.getParentTypeId();
            const areaTypeId = this.ftHelperService.getAreaTypeId();
            const areaCode = this.ftHelperService.getFTModel().areaCode;

            const key = parentTypeId.toString() + '-' + areaTypeId.toString() + '-';
            areaMappings = this.ftHelperService.getAreaMappingsForParentCode(key);

            if (_.findIndex(areaMappings, areaCode) === -1) {
                areaMappings.push(areaCode);
            }
        }

        Object.keys(sortedata).forEach(key => {
            const value: CoreDataSet = sortedata[key];
            let colour = null;

            if (areaMappings === null ||
                areaMappings.findIndex(x => x.toString() === value.AreaCode) > -1) {
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
            }
        });

        valuesForBarChart.sort((leftside, righside): number => {
            if (leftside.Val < righside.Val) { return -1; }
            if (leftside.Val > righside.Val) { return 1; }
            return 0;
        });

        for (const value of valuesForBarChart) {
            this.chartData.push({ color: value.Colour, x: xVal, key: value.AreaCode, y: value.Val });
            xVal++;

            const areaName = this.ftHelperService.getAreaName(value.AreaCode);
            xAxisCategories.push({
                AreaName: areaName, ValF: value.ValF, LoCIF: value.LoCIF,
                UpCIF: value.UpCIF, AreaCode: value.AreaCode, NoteId: value.NoteId
            });

            errorData.push([value.LoCI, value.UpCI]);
        }
        const series: any[] /*Highcharts.IndividualSeriesOptions[]*/ = [
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


        // Label for subnational option
        let comparatorName: string;
        if (this.isNearestNeighbours) {
            const model = this.ftHelperService.getFTModel();
            const area = this.ftHelperService.getArea(model.areaCode);
            const nnAreaName = new AreaHelper(area).getShortAreaNameToDisplay();
            comparatorName = nnAreaName + ' and neighbours';
        } else {
            comparatorName = this.ftHelperService.getCurrentComparator().Name;
        }

        const valueWithUnit: ValueWithUnit = this.coreDataHelperService.valueWithUnit(unit);
        for (const grouping of currentGrpRoot.Grouping) {

            if (grouping.ComparatorData.ValF === '-') { continue; }
            const valF = grouping.ComparatorData.ValF;
            const val = grouping.ComparatorData.Val;

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
        const yAxis = {
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
        const valueNotes = this.ftHelperService.getValueNotes();

        // Locals for events
        const _thisLocal = this;

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

                    // Benchmark tooltip
                    if (this.series.type === 'line') {
                        return '<b>' + this.series.name + '</b><br />'
                            + valueWithUnit.getFullLabel(regionValues[this.series.index]);
                    }

                    // Child area tooltip
                    const data = xAxisCategories[this.x];

                    _thisLocal.hoverAreaCodeChanged.emit({ areaCode: data.AreaCode });
                    let s = '<b>' + data.AreaName + '</b>';
                    s += '<br>' + valueWithUnit.getFullLabel(data.ValF);

                    if (data.NoteId) {
                        s += '<br><em>' + valueNotes[data.NoteId].Text + '</em>';
                    }

                    if (data.LoCIF) {
                        s += '<br>LCI: ' + valueWithUnit.getFullLabel(data.LoCIF);
                    }
                    if (data.UpCIF) {
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
                                const areaCode = this.options.key;
                                _thisLocal.hoverAreaCodeChanged.emit({ areaCode: null });

                                // Highlighting throws exception in nearest neighbours mode for unknown reason
                                if (!_thisLocal.isNearestNeighbours) {
                                    // Unhighlight area
                                    _thisLocal.changeAreaHighlight(_thisLocal.chart, _thisLocal.chartData, areaCode, false);
                                }
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
        const chart = this.chart;
    }

    highlightArea(areaCode: string): void {
        // Highlighting throws exception in nearest neighbours mode for unknown reason
        if (!this.isNearestNeighbours) {
            this.changeAreaHighlight(this.chart, this.chartData, areaCode, true);
        }
    }

    changeAreaHighlight(chart: Highcharts.ChartObject,
        chartData: any, areaCode: string, shouldBeSelected): void {

        if (chart && chartData != null) {
            const data = _.where(chartData, { key: areaCode })[0];
            if (isDefined(data)) {
                const pointList = chart.series[0].data;
                const pointIndex = [data['x']];
                const point = pointList[pointIndex];
                point.select(shouldBeSelected);
            }
        }
    }

    onExportClick(event: MouseEvent) {
        const title = this.buildChartTitle();
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
        const groupRoot: GroupRoot = this.ftHelperService.getCurrentGroupRoot();
        const metadata = this.ftHelperService.getMetadata(groupRoot.IID);
        const unit = metadata.Unit;
        const unitLabel = (typeof unit.Label !== 'undefined') ? unit.Label : '';
        const period: string = groupRoot.Grouping[0].Period;
        return metadata.ValueType.Name + ' - ' + unitLabel + ' ' + period;
    }
}
