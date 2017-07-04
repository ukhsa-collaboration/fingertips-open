'use strict';

function goToTrendsPage() {
    lock();
    showSpinner();

    var year = latestYear,
    offset = getYearOffset(),
    groupId = PP.model.groupId,
    parentCode = PP.model.parentCode;

    setPageMode(PAGE_MODES.TRENDS);

    ajaxMonitor.setCalls(9);
    ajaxMonitor.setState({ year: year, offset: offset });

    getPracticeAndParentLists();

    getIndicatorMetadata(groupId);
    getValueLimits(groupId, AreaTypeIds.Practice, parentCode);
    getMetadataProperties();

    getAreaData(groupId, parentCode, false);
    getAreaData(groupId, PP.model.practiceCode);
    getNationalData(groupId);

    ajaxMonitor.monitor(displayTrends);
}

function displayTrends() {

    var practiceCode = PP.model.practiceCode,
    groupId = PP.model.groupId,
    isPractice = PP.model.isPractice();

    var subgroupData = loaded.data[groupId];
    var nationalSubgroupData = subgroupData[NATIONAL_CODE];
    var metadata = loaded.indicatorMetadata[groupId];

    var practiceSubgroupData = subgroupData[practiceCode],
    benchmarkSubgroupData = subgroupData[getBenchmarkCode()],
    isCcg = !!PP.model.parentCode;

    var html = [],
    benchmarkName = getPracticeParentLabel(),
    trendDataList = [],
    periodList = [];

    var maxRowCount = 0;

    for (var rootIndex in nationalSubgroupData) {

        if (nationalSubgroupData[rootIndex].Data.length > maxRowCount)
            maxRowCount = nationalSubgroupData[rootIndex].Data.length;

        var nationalItem = nationalSubgroupData[rootIndex],
        iid = nationalItem.IID;
        var indicatorMetadata = metadata[iid];

        var args = {
            rows: [],
            benchmark: benchmarkName,
            practice: isPractice ? practiceCode : 'Practice',
            iid: iid,
            groupId: groupId,
            name: indicatorMetadata.Descriptive.Name
        };

        var nationalData = nationalSubgroupData[rootIndex].Data;

        if (isPractice) {
            var dataPractice = practiceSubgroupData[rootIndex].Data;
        }

        if (isCcg) {
            var dataBenchmark = benchmarkSubgroupData[rootIndex].Data;
        }

        var trendData = new TrendData();
        var periods = [];
        trendDataList.push(trendData);
        periodList.push(periods);

        var unit = getUnitLabel(indicatorMetadata);
        var index = 0;
        while (index < nationalData.length) {

            var nationalPoint = nationalData[index],
            period = nationalItem.Periods[index],
            practicePoint = isPractice ? dataPractice[index] : null,
            ccgPoint = isCcg ? dataBenchmark[index] : null;

            if (nationalPoint) {

                // Add table row data
                args.rows.push([
                        period,
                        getValF(practicePoint, unit),
                        getValF(ccgPoint, unit),
                        getValF(nationalPoint, unit)
                ]);

                // Add trend data
                periods.push(period);
                trendData.addPracticePoint(practicePoint);
                trendData.addCcgPoint(ccgPoint);
                trendData.addNationalPoint(nationalPoint);
            }
            index++;
        }

        args.index = rootIndex;
        html.push(templates.render('trends', args));
    }

    $('#trendsBox').html(html.join(''));

    displayTrendCharts(trendDataList, periodList, nationalSubgroupData);

    showAndHidePageElements();

    //Calculate additional height for trend box if required
    var maxBoxHeight = (maxRowCount * 20) + 280 + 'px';
    $(".small-trend-box").css("height", maxBoxHeight);

    unlock();
}

function createTrendChart(id, trendData, periods, stats) {

    var series = [
        {
            data: trendData.getNationalPoints(),
            color: colours.comparator,
            name: 'England'
        }
    ];

    if (PP.model.parentCode) {
        series.push({
            data: trendData.getCcgPoints(),
            color: chartColours.pink,
            name: 'CCG'
        });
    }

    if (PP.model.isPractice()) {
        series.push({
            data: trendData.getPracticePoints(),
            color: colours.bobLower,
            name: ui.getCurrentPractice().Name
        });
    }

    try {
        new Highcharts.Chart({
            chart: {
                renderTo: id,
                defaultSeriesType: 'line',
                zoomType: 'xy',
                width: 300,
                height: 200
            },
            title: {
                text: ''
            },
            xAxis: {
                title: {
                    enabled: false
                },
                categories: periods,
                tickLength: 3,
                tickPosition: 'outside',
                tickWidth: 1,
                tickmarkPlacement: 'on'
            },
            yAxis: {
                title: {
                    text: ''
                },
                max: stats.Max,
                min: stats.Min
            },
            legend: {
                enabled: true,
                layout: 'vertical',
                borderWidth: 0

            },
            plotOptions: {
                line: {
                    enableMouseTracking: true,
                    lineWidth: 2,
                    animation: false,
                    marker: {
                        radius: 3,
                        symbol: 'circle',
                        lineWidth: 1,
                        lineColor: '#000000'
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
            credits: HC.credits,
            series: series,
            exporting: {
                enabled: false
            }
        });
    } catch (e) {
        // HighChart reports errors via console.log which is not available <=IE8
    }
};

function TrendData() {

    var nationalPoints = [],
    ccgPoints = [],
    practicePoints = [],
    getValue = function (v) {

        if (!v) {
            return null;
        }

        return isValidValue(v.Val) ?
            parseFloat(v.ValF) :
            null;
    };

    this.addPracticePoint = function (d) {
        practicePoints.push(getValue(d));
    };

    this.addNationalPoint = function (d) {
        nationalPoints.push(getValue(d));
    };

    this.getNationalPoints = function () {
        return nationalPoints;
    };

    this.getPracticePoints = function () {
        return practicePoints;
    };

    this.addCcgPoint = function (d) {
        ccgPoints.push(getValue(d));
    };

    this.getCcgPoints = function () {
        return ccgPoints;
    };
}

function displayTrendCharts(trendDataList, periodList, nationalSubgroupData) {

    var limitsList = loaded.valueLimits[getValueLimitsKey()];
    for (var rootIndex in trendDataList) {
        var limits = limitsList[rootIndex];
        if (limits) {

            adjustLimitsForNationalValue(limits, nationalSubgroupData[rootIndex].Data);

            createTrendChart('trendChart' + rootIndex,
                trendDataList[rootIndex], periodList[rootIndex], limits);
        }
    }
}

function getValueLimits(groupId, areaTypeId, parentCode) {

    var key = getValueLimitsKey();

    if (!loaded.valueLimits[key]) {

        var parameters = new ParameterBuilder(
        ).add('group_id', groupId
        ).add('area_type_id', areaTypeId);

        if (parentCode) {
            parameters.add('parent_area_code', parentCode);
        }

        ajaxGet('api/value_limits',
            parameters.build(),
            getValueLimitsCallback);
    } else {
        ajaxMonitor.callCompleted();
    }
}

function getValueLimitsCallback(obj) {
    loaded.valueLimits[getValueLimitsKey()] = obj;
    ajaxMonitor.callCompleted();
}

function getValueLimitsKey() {

    var parentCode = PP.model.parentCode;
    return getKey(PP.model.groupId, !!parentCode ? parentCode : '');
}

/**
* Make sure the min and max overlap with the national average.
*/
function adjustLimitsForNationalValue(limits, nationalDataList) {

    for (var i in nationalDataList) {
        var nationalData = nationalDataList[i];
        if (nationalData) {

            var val = nationalData.Val;
            if (limits.Min > val) {
                limits.Min = val;
            }

            if (limits.Max < val) {
                limits.Max = val;
            }
        }
    }
}

templates.add('trends', '<div class="small-trend-box">\
<table class="bordered-table">\
<thead><tr><th colspan="4" class="indicator">\
<div class="indicator">{{name}}\
<div class="info-tooltip" onclick="showMetadata({{groupId}},{{iid}},this)"></div></div></th></tr>\
<tr><th>Period</th><th>{{practice}}</th>\
<th>{{benchmark}}</th><th>England</th></tr></thead>\
</tbody>{{#rows}}<tr><td>{{0}}</td><td>{{& 1}}</td>\
<td>{{& 2}}</td><td>{{& 3}}</td></tr>{{/rows}}</tbody></table>\
<div id="trendChart{{index}}"></div></div>');

