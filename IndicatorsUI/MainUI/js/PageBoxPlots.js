'use strict';

/**
* BoxPlot namespace
* @module england
*/
var boxPlot = {
    state: {
        boxPlotStats: null
    }
};

function goToBoxPlotPage() {

    setPageMode(PAGE_MODES.BOX_PLOT);

    if (!areIndicatorsInDomain()) {
        displayNoData();
    } else {

        showSpinner();

        pages.getContainerJq().html(templates.render('boxplots', {}) + templates.render('boxplotTable', {}));

        ajaxMonitor.setCalls(3);

        getGroupingData();
        getIndicatorMetadata(FT.model.groupId);//  for yAxis, unit, name eg 0.1i
        getBoxPlotStats();

        ajaxMonitor.monitor(displayBoxPlotProfile);
    }
}


function getBoxPlotStats() {

    var rootIndex = getIndicatorIndex(),
        root = groupRoots[rootIndex];
    var sexId = root.Sex.Id;
    var ageId = root.Age.Id;
    var indicatorId = root.IID;

    var html = getSelectedHeader(root, 'goToMetadataPage(' + rootIndex + ')');
    $('#indicator-details-header').html(html);
    $('#boxplot-chart').html('');
    var parentAreaCode = getComparatorById(comparatorId).Code;

    var parameters = new ParameterBuilder(
    ).add('indicator_id', indicatorId
    ).add('sex_id', sexId
    ).add('age_id', ageId
    ).add('child_area_type_id', FT.model.areaTypeId
    ).add('parent_area_code', parentAreaCode);

    ajaxGet('api/indicator_statistics/trends_for_single_indicator', parameters.build(), getBoxPlotStatsCallback);
}

function getBoxPlotStatsCallback(data) {

    boxPlot.state.boxPlotStats = data;
    ajaxMonitor.callCompleted();
}

function displayBoxPlotProfile() {

    CreateBoxPlotGraph();

    showAndHidePageElements();

    ui.setScrollTop();

    unlock();
}

function CreateBoxPlotGraph() {

    var data = Object.values(boxPlot.state.boxPlotStats);

    var yAxisArr = [], yAxisArrGrid = [];
    var Period = [];
    var isDataAvailable = false;
    for (var i = 0; i < data.length; i++) {
        if (data[i].StatsF) {
            isDataAvailable = true;
            var xAxisArr = [], xAxisArrGrid = [];
            var statsF = data[i].StatsF;

            xAxisArr[0] = parseFloat(statsF.P5 == null ? '0' : statsF.P5);
            xAxisArr[1] = parseFloat(statsF.P25 == null ? '0' : statsF.P25);
            xAxisArr[2] = parseFloat(statsF.Median == null ? '0' : statsF.Median);
            xAxisArr[3] = parseFloat(statsF.P75 == null ? '0' : statsF.P75);
            xAxisArr[4] = parseFloat(statsF.P95 == null ? '0' : statsF.P95);
            yAxisArr.push(xAxisArr);

            xAxisArrGrid[0] = parseFloat(statsF.Min == null ? '0' : statsF.Min);
            xAxisArrGrid[1] = xAxisArr[0];
            xAxisArrGrid[2] = xAxisArr[1];
            xAxisArrGrid[3] = xAxisArr[2];
            xAxisArrGrid[4] = xAxisArr[3];
            xAxisArrGrid[5] = xAxisArr[4];
            xAxisArrGrid[6] = parseFloat(statsF.Max == null ? '0' : statsF.Max);
            yAxisArrGrid.push(xAxisArrGrid);

            Period.push(data[i].Period);
        }
    }

    if (isDataAvailable) {

        var rootIndex = getIndicatorIndex(),
            root = groupRoots[rootIndex];
        var metadata = ui.getMetadataHash()[root.IID];
        var unitLabel = metadata.Unit.Label;

        var seriesName = FT.menus.areaType.getName() + ' in ' + getCurrentComparator().Name;

        Highcharts.chart('boxplot-chart',
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
                categories: Period,
                title: {
                    text: ''
                }
            },

            yAxis: {
                title: {
                    text: unitLabel
                }
            },

            plotOptions: {
                boxplot: {
                    animation: false,
                    color: '#1e1e1e',
                    fillColor: '#cccccc',
                    //lineWidth: 2,
                    medianColor: '#ff0000'
                    //medianWidth: 3,
                    //stemColor: '#A63400',
                    //stemDashStyle: 'line',
                    //stemWidth: 1,
                    //whiskerColor: '#3D9200',
                    //whiskerLength: '20%',
                    //whiskerWidth: 3
                }
            },

            tooltip: {
                followPointer: false,
                formatter: function () {
                    var tooltipContent = '<b>' + this.x + '</b><br/>';
                    tooltipContent += '<b>' + this.series.name + '</b><br/>';
                    tooltipContent += '95th Percentile: ' + this.point.high + '<br/>';
                    tooltipContent += '75th Percentile: ' + this.point.q3 + '<br/>';
                    tooltipContent += 'Median: ' + this.point.median + '<br/>';
                    tooltipContent += '25th Percentile: ' + this.point.q1 + '<br/>';
                    tooltipContent += '5th Percentile: ' + this.point.low + '<br/>';

                    return tooltipContent;
                }
            },

            series: [
                {
                    name: seriesName,
                    data: yAxisArr,
                    showInLegend: false
                }
            ]
        });

        //loading grid 
        CreateBoxplotTable(Period, yAxisArrGrid);
    } else {
        $('#boxplot-chart').append('<div id="box-plot-no-data">No Data</div>');
    }

    $('#indicator-details-boxplot-data').show();
}

templates.add('boxplots',
    '<div id="indicator-details-boxplot-data" class="col-md-12" style="display: none;"><div style="float:none; clear: both; width:100%;"><div id="indicator-details-header"></div>\
    <div id="boxplot-chart"></div>');

templates.add('boxplotTable',
    '<div class="box-plot-table"><table id="tbl-boxplot-data" class="bordered-table table-hover table"><thead>\
<tr></tr></thead>\
<tbody></tbody></table></div>');


pages.add(PAGE_MODES.BOX_PLOT, {
    id: 'boxplots',
    title: 'Box<br/>Plots',
    goto: goToBoxPlotPage,
    gotoName: 'goToBoxPlotPage',
    needsContainer: true,
    jqIds: ['indicator-menu-div', 'spine-range-key', 'spineKey', '.geo-menu', 'export-chart-box',
        'spineRangeKeyContainer', 'nearest-neighbour-link'],
    jqIdsNotInitiallyShown: []
});

function getSelectedHeader(root, clickHandler) {
    var metadata = ui.getMetadataHash()[root.IID];

    var unit = metadata.Unit.Label,
		unitLabel = unit !== '' ? ' - ' + unit : '';

    return ['<div class="trend-header"><div class="trend-title"><a class="trend-link" title="More about this indicator" href="javascript:',
         clickHandler, ';">', metadata.Descriptive.Name, new SexAndAge().getLabel(root), '</a>',
         '<span class="benchmark-name">', getCurrentComparator().Name, '</span>', '</div>',
         '<div class="trend-unit"><span>', metadata.ValueType.Name, unitLabel, '</span></div>', '</div>', '<div class="box-plot-legend-img">', '</div>'].join('');
}


function CreateBoxplotTable(period, yAxisArr) {
    var legend = ["Minimum", "5th Percentile", "25th Percentile", "Median", "75th Percentile", "95th Percentile", "Maximum"];

    var htmlHeader = ['<tr class="box-plot-header">'];
    addTh(htmlHeader, "", "x-axis", "");
    $.each(period,
        function (i, v) {
            addTh(htmlHeader, v, "x-axis-year", "");
        });

    $('#tbl-boxplot-data > thead:last').append(htmlHeader.join(''));

    var htmlContent = [], index = 0;
    for (index = 6; index >= 0; index--) {
        htmlContent.push('<tr>');
        addTd(htmlContent, legend[index], "numeric-name", "");
        var j = 0;
        for (j = 0; j < yAxisArr.length; j++) {
            addTd(htmlContent, yAxisArr[j][index], "numeric", "");
        }
        htmlContent.push('</tr>');
    }
    $('#tbl-boxplot-data > tbody:last').append(htmlContent.join(''));
}