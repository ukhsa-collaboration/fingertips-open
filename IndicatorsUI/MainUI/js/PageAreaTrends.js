function goToAreaTrendsPage() {

    var model = FT.model;
    sortedAreasForTrends = sortedAreas;

    $('#goBack').bind('click', function () {
        areaTrendsState.neighbourAreaCode = null;
    });

    if (!groupRoots.length) {
        noDataForAreaType();
    } else {

        if (model.isNearestNeighbours()) {
            model.parentCode = model.nearestNeighbour;
        }

        lock();
        tooltipManager.setTooltipProvider(new ValueNoteTooltipProvider());
        getTrendData();
        $('#benchmark-key-text').html(getCurrentComparator().Name);
    }
}

SPARKLINE_SORTS = {
    AREA: 0,
    DIFF: 1,
    AVERAGE: 2,
    RANK: 3
};

function getTrendData() {
    var model = FT.model;

    var parentCode = model.isNearestNeighbours() && viewModeState !== VIEW_MODES.MULTI_AREA 
        ? model.nearestNeighbour 
        : model.parentCode;

    if (areaTrendsState.isTrendDataLoaded(parentCode)) {
        getTrendDataCallback(areaTrendsState.getTrendData(parentCode));
    } else {
        showSpinner();

        var parameters = 'gid=' + model.groupId +
            '&ati=' + model.areaTypeId +
            '&par=' + parentCode +
            getProfileOrIndicatorsParameters();

        ajaxGet('GetTrendData.ashx', parameters, getTrendDataCallback);
    }
}

function getTrendDataCallback(obj) {

    if (isDefined(obj)) {
        currentTrendRoots = obj;

        areaTrendsState.setTrendData(FT.model.parentCode,
            currentTrendRoots);

        filterTrendRoots();
    } else {
        currentTrendRoots = null;
    }

    setPageMode(PAGE_MODES.AREA_TRENDS);

    if (!areaTrendsState.isInitialised) {
        initAreaSwitch();
        areaTrendsState.isInitialised = true;
    }

    displayTrendRootTables();

    showAndHidePageElements();

    hideAndShowTrendKeys();

    showDataQualityLegend();

    showTargetBenchmarkOption(currentTrendRoots);

    displayChartOptions();

    unlock();

    loadValueNoteToolTips();
}

function initAreaSwitch() {

    var topOptionFunc = function () {
        viewModeState = VIEW_MODES.AREA;
        viewClicked(VIEW_MODES.AREA);
    };

    var bottomOptionFunc = function () {
        viewModeState = VIEW_MODES.MULTI_AREA;
        viewClicked(VIEW_MODES.MULTI_AREA);
    };

    areaTrendsState.areaSwitch = new AreaSwitch({
        eventHanders: [topOptionFunc, bottomOptionFunc],
        eventName: 'ChartCountChanged'
    });
}

function displayChartOptions() {
    var model = FT.model;
    var state = areaTrendsState;
    var areaSwitch = state.areaSwitch;
    var areaCode = getSingleTrendsAreaCode();
    var area = areaHash[areaCode];

    areaSwitch.setHtml({
        label: 'Trends for',
        topOptionText: getAreaNameToDisplay(area),
        bottomOptionText: model.isNearestNeighbours()
            ? 'All nearest neighbours for ' + area.Short
            : 'All in ' + getParentArea().Name
    });

    if (areaSwitch.getOption() === VIEW_MODES.MULTI_AREA) {
        var atozSorter = '<div id="sorting-options" style="text-align:center">Sort charts by: ' +
            '<a id="spark0" href="javascript:sortSparklineClicked(SPARKLINE_SORTS.AREA);" class="" title="Sort alphabetically by area name">A-Z</a> ' +
            '<a id="spark1" href="javascript:sortSparklineClicked(SPARKLINE_SORTS.AVERAGE);" class="" title="Sort by the most recent value">Value</a></div>';

        var templateName = 'atozChartSorter';
        templates.add(templateName, atozSorter);
        var html = templates.render(templateName);

        $('#trends-chart-sorter-az').html(html);

        // add rank sorting option after template is randered
        if (model.isNearestNeighbours()) {
            $('#spark1').after('<a id="spark2" href="javascript:sortSparklineClicked(SPARKLINE_SORTS.RANK);" class="" title="Sort by rank">Rank</a>');
        } else {
            $('#spark2').remove();
        }
    }
}

function addTrendRow(h, dataPoint, comparatorValues, period, comparisonConfig) {

    var regionalVal = comparatorValues[REGIONAL_COMPARATOR_ID],
        nationalVal = comparatorValues[NATIONAL_COMPARATOR_ID],
        dataInfo = new TrendDataInfo(dataPoint),
        isValue = dataInfo.isValue();
    if (!isValue && regionalVal === '-' && nationalVal === '-') {
        return false;
    }

    h.push('<tr>');
    addTd(h, period);

    // Significance marker
    var marker = isValue ?
        getTrendMarkerHtml(dataPoint, comparisonConfig) :
        EMPTY_TD_CONTENTS;
    addTd(h, marker, CSS_CENTER);

    var valueDisplayer = new ValueDisplayer();

    addTd(h, formatCount(dataInfo), CSS_NUMERIC);
    addTd(h, valueDisplayer.byDataInfo(dataInfo), CSS_NUMERIC, null, dataInfo.getNoteId());
    addTd(h, valueDisplayer.byNumberString(dataPoint.L), CSS_NUMERIC);
    addTd(h, valueDisplayer.byNumberString(dataPoint.U), CSS_NUMERIC);
    // Add the row only if not in Nearest Neighbour mode
    if (!FT.model.isNearestNeighbours()) {
        if (isSubnationalColumn()) {
            addTd(h, valueDisplayer.byNumberString(regionalVal), CSS_NUMERIC);
        }
    }

    if (enumParentDisplay !== PARENT_DISPLAY.REGIONAL_ONLY) {
        addTd(h, valueDisplayer.byNumberString(nationalVal), CSS_NUMERIC);
    }

    h.push('</tr>');

    return true;
}

function getTrendMarkerHtml(data, comparisonConfig) {
    var sig = data.Sig[comparisonConfig.comparatorId],
    useRag = comparisonConfig.useRagColours,
    useQuintileColouring = comparisonConfig.useQuintileColouring;

    var marker = ['<img src="', FT.url.img,
        getMiniMarkerImageFromSignificance(sig, useRag, useQuintileColouring), '"'];

    if (comparisonConfig.useTarget) {
        marker.push(' onmouseover="highlightTrendValueWithTarget(this);" onmouseout="unhighlightValueAndComparator();"');
    } else {
        marker.push(' onmouseover="highlightTrendValueAndComparator(this,', sig,
            ',', useRag, ',', useQuintileColouring, ');" onmouseout="unhighlightValueAndComparator();"');
    }
    marker.push('/>');
    return marker.join('');
}

function displayTrendRootTables() {

    var html = [],
    indexesDisplayed = [],
        areaCode, i, root, comparatorName, index;

    var viewMode = areaTrendsState.areaSwitch.getOption();

    if (currentTrendRoots !== null) {
        displayChartOptions();
        html = viewMode === VIEW_MODES.MULTI_AREA ?
            getMultiAreaTrendsHtml() :
            getSingleAreaTrendsHtml(indexesDisplayed);
    }

    pages.getContainerJq().html(html.join(''));

    // Display charts (after HTML has been set)
    if (viewMode === VIEW_MODES.AREA) {
        // One big trend chart for a single area
        comparatorName = getCurrentComparator().Name;

        areaCode = getSingleTrendsAreaCode();

        for (i = 0; i < indexesDisplayed.length; i++) {
            index = indexesDisplayed[i];
            root = displayedTrendRoots[index];

            chart = new Highcharts.Chart(
                getLargeTrendChartOptions('trendChart' + index, root, areaCode, comparatorName)
            );
        }
    }
    else {
        // Multiple small trend charts
        index = getIndicatorIndex();
        root = currentTrendRoots[index];
        comparatorName = getCurrentComparator().Name;

        if (isDefined(root)) {
            var comparatorVals = [],
                vals = root.ComparatorValue;
            for (i in vals) {
                comparatorVals.push(vals[i][comparatorId]);
            }

            var metadata = ui.getMetadataHash()[root.IID],
                comparisonConfig = new ComparisonConfig(root, metadata);

            for (i in sortedAreasForTrends) {
                areaCode = sortedAreasForTrends[i].Code;
                if (new CoreDataSetList(root.Data[areaCode]).areAnyValidTrendValues()) {
                    createSmallTrendChart('at-' + areaCode, root, areaCode,
                        comparatorName, comparisonConfig);
                }
            }
        }
    }
}

function addTrendTableHeader(h, useWide) {
    h.push('<thead class="data-header"><tr>');
    addTh(h, 'Period', useWide ? 'trendPeriodWide' : 'trendPeriod');
    addTh(h, null, 'trendSig');
    addTh(h, 'Count');
    addTh(h, 'Value');
    addTh(h, 'Lower CI', null, 'Lower Confidence Interval');
    addTh(h, 'Upper CI', null, 'Upper Confidence Interval');

    // Don't add Region column for NN
    if (!FT.model.isNearestNeighbours()) {
        if (isSubnationalColumn()) {
            addTh(h, getParentArea().Short);
        }
    }

    if (enumParentDisplay !== PARENT_DISPLAY.REGIONAL_ONLY) {
        addTh(h, getNationalComparator().Name);
    }

    h.push('</tr></thead>');
}

function setTrendRowCss(e, cssClasses, isComparatorInSameRow) {

    var tds = $(e).parent().parent().children(),
    isNationalComparator = comparatorId === NATIONAL_COMPARATOR_ID;

    // Select elements
    if (isComparatorInSameRow) {

        var index = isNationalComparator ?
            6 :
            5;

        var jQ = $([tds[2], tds[index]]);
    }
    else {

        index = isNationalComparator ?
            0 :
            1;

        var td = $('#indicatorDetailsTable tbody tr:eq(' + index + ') td:eq(2)');

        jQ = $([tds[2], td[0]]);
    }

    // Set CSS Class
    for (var i in cssClasses) {
        jQ.togglClass(cssClasses[i]);
    }
}

function getLargeTrendChartOptions(id, root, areaCode, comparatorName) {

    var labels = [];
    var metadata = ui.getMetadataHash()[root.IID];
    var comparisonConfig = new ComparisonConfig(root, metadata);
    var trendData = new TrendData();
    var area = areaHash[areaCode];

    var dataList = root.Data[areaCode];
    if (!dataList) {
        // No data at all
        dataList = [];
    }

    var showBenchmarkData = metadata.ValueTypeId !== ValueTypeIds.Count;

    for (var j in dataList) {

        var data = dataList[j],
        significance = data.Sig[comparisonConfig.comparatorId],
        markerColor = getColourFromSignificance(significance, comparisonConfig.useRagColours, colours, comparisonConfig.useQuintileColouring);

        trendData.addAreaPoint(data, markerColor);

        if (showBenchmarkData) {
            trendData.addBenchmarkPoint(root.ComparatorValue[j][comparatorId],
                root.ComparatorValueFs[j][comparatorId]);
        }

        labels.push(root.Periods[j]);
    }

    // Min/Max
    var limits = root.Limits;
    if (limits) {
        var max = limits.Max;
        var min = getMinYAxis(limits);
    } else {
        max = 0;
        min = 1;
    }

    var options = {
        chart: {
            renderTo: id,
            defaultSeriesType: 'line',
            zoomType: 'xy',
            width: 400,
            height: 300,
        },
        title: {
            text: ''
        },
        xAxis: {
            title: {
                enabled: false
            },
            categories: labels,
            labels: {
                formatter: function () {
                    // Format period so fits on x axis

                    var period = this.value;

                    if (period.length > 22) {
                        // e.g. '2009/10 Q2 - 2010/11 Q1' 
                        period = period.replace(/20/g, '');
                    }

                    return period.indexOf('-') > -1 ?
                        period.replace('-', '<br>-') /*Break after - for ranges*/ :
                        period.replace(' ', '<br/>');

                },
                enabled: true,
                step: getLabelStep(labels),
                maxStaggerLines: 1
            },
            tickLength: 3,
            tickPosition: 'outside',
            tickWidth: 1,
            tickmarkPlacement: 'on'
        },
        yAxis: {
            title: {
                text: metadata.Unit.Label
            },
            max: max,
            min: min
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
                    radius: dataList.length > 40 ? 2 : 5,
                    symbol: 'circle',
                    lineWidth: 1,
                    lineColor: '#000000'
                },
                events: {
                    mouseOut: function () {
                        clearTrendCells();
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
            formatter: chartTooltip
        },
        credits: HC.credits,
        series: [
        {
            data: trendData.getBenchmarkPoints(),
            color: colours.comparator,
            name: comparatorName
        },
        {
            data: trendData.getAreaPoints(),
            name: area.Name,
            showInLegend: false
        },
            {
                name: area.Name,
                type: 'errorbar',
                animation: false,
                data: trendData.getCIPoints(),
                visible: isDefined(areaTrendsState.showConfBars) ? areaTrendsState.showConfBars : false
            }
        ],
        exporting: {
            enabled: false
        }
    };

    return options;
}

function exportChart(groupRootIndex) {
    var areaCode = FT.model.areaCode;
    var root = displayedTrendRoots[groupRootIndex];
    var indicatorName = ui.getMetadataHash()[root.IID].Descriptive.Name + new SexAndAge().getLabel(root);

    var newChart = new Highcharts.Chart(getLargeTrendChartOptions('trendChart' + groupRootIndex,
        root, areaCode, getCurrentComparator().Name));

    newChart.exportChart({ type: 'image/png' }, {
        chart: {
            spacingTop: 70,
            width: 400,
            height: 350,
            events: {
                load: function () {
                    this.renderer.text('<b>' + indicatorName + ' - ' +
                        areaHash[areaCode].Name + '</b>', 190, 15)
                        .attr({
                            align: 'center'
                        })
                        .css({
                            color: '#333',
                            fontSize: '10px',
                            width: '350px'
                        })
                        .add();
                }
            }
        }
    });
}

function toggleCIBars(groupRootIndex) {
    var state = areaTrendsState;
    var showCILink = $('#showCI' + groupRootIndex)[0];

    if (state.showConfBars) {
        state.showConfBars = false;
        var text = 'Show';
    } else {
        state.showConfBars = true;
        text = 'Hide';
    }

    showCILink.text = text + ' confidence intervals';
}

function showErrorBar(groupRootIndex) {
    toggleCIBars(groupRootIndex);

    if (!ajaxLock) {
        lock();

        displayTrendRootTables();
        unlock();
    }
}

function showExportMenu() {
    return areaTrendsState.areaSwitch.getOption() === VIEW_MODES.AREA ? true : false;
}

function getMinYAxis(limits) {
    if (limits.min < 0) {
        return limits.Min;
    } else {
        if (startZeroYAxis) {
            return 0;
        }
    }

    return limits.Min;
}

function getTrendMarkerRadius(dataList) {
    // Use small markers where there are more points on the chart
    return dataList.length > 40 ? 3 : 5;
}

function clearTrendCells() {
    var row = areaTrendsState.row;
    if (row) {
        row.unhighlight();
        areaTrendsState.row = null;
    }
}

function getTrendTooltipHtml(sig, useRag, useQuintileColouring) {

    // Set text
    var name = getCurrentComparator().Name;
    var average = name + ' average';

    if (useQuintileColouring) {
        switch (sig) {
            case 0:
                return 'Significance is not calculated for this indicator';
            case 1:
                return 'Lowest quintile in ' + name;
            case 2:
                return '2nd lowest quintile in ' + name;
            case 3:
                return 'Middle quintile in ' + name;
            case 4:
                return '2nd highest quintile in ' + name;
            case 5:
                return 'Highest quintile in ' + name;
            default:
                // No comparator value
                return 'No ' + name + ' value to compare';
        }
    }

    switch (sig) {
        case 3:
            var adjective = useRag ?
                'better' :
                'above';
            return 'Significantly ' + adjective + ' than ' + average;
        case 2:
            return 'Not significantly different to ' + average;
        case 1:
            adjective = useRag ?
                'worse' :
                'below';
            return 'Significantly ' + adjective + ' than ' + average;
        case 0:
            return 'Significance is not calculated for this indicator';
        default:
            // No comparator value
            return 'No ' + name + ' value to compare';
    }
}

function displayTrendTooltip(e, text) {
    var t = $('#trendHelper');
    t.html(text);

    // Position
    var pos = $(e).offset();
    t.css('top', pos.top + 22);
    t.css('left', pos.left + 25);
    t.show();
}

function highlightTrendValueWithTarget(e) {
    displayTrendTooltip(e, 'This value is benchmarked<br>against the goal above');
}

function highlightTrendValueAndComparator(e, sig, useRag, useQuintileColouring) {

    displayTrendTooltip(e, getTrendTooltipHtml(sig, useRag, useQuintileColouring));

    // Highlight value cells
    var row = new TrendTableRow();
    areaTrendsState.row = row;
    row.setRowFromElement(e);
    row.toggleValueCellHighlights();
}

function unhighlightValueAndComparator() {

    var state = areaTrendsState;
    if (state.row) {
        state.row.toggleValueCellHighlights();
        state.row = null;
    }
    $('#trendHelper').hide();
}

function getComparatorCellIndex() {

    var upperCIIndex = trendTableColumnIndexes.upperCI;
    return (comparatorId === REGIONAL_COMPARATOR_ID ||
            enumParentDisplay !== PARENT_DISPLAY.NATIONAL_AND_REGIONAL) ?
        upperCIIndex + 1 :
        upperCIIndex + 2;
}

function getLabelStep(labels) {
    return labels.length > 0
        ? Math.ceil(labels.length / getLabelCount(labels[0]))
        : 1/*there is no data to display so this is dummy value*/;
}

function isParameterDefinedAndTrue(parameters, key) {

    if (isDefined(parameters)) {
        var p = parameters[key];
        return isDefined(p) && p === true;
    }
    return false;
}

function getLabelCount(label) {

    var labelLength = label.length;

    // e.g. "2008/09 Q3"
    if (labelLength > 10) return 4;

    // e.g. "2008 - 09"
    if (labelLength > 7) return 5;

    // e.g. "2004", "2008/09"
    return 6;
}

function hasWidePeriodLabel(metadata) {
    var id = metadata.YearTypeId;
    // Is rolling year or cumulative quarters
    return (id >= 4 && id <= 7);
}

function chartTooltip() {
    var state = areaTrendsState,
    series = this.series,
    period = this.x,
    dataPoint = series.data[this.point.x],
    rootIndex = $(series.chart.container).parent().attr('id').match(/\d+/);

    // Determine value column index,
    isComparator = dataPoint.isBenchmark;
    columnIndex = isComparator ?
        getComparatorCellIndex() :
        trendTableColumnIndexes.value;

    // Remove existing highlights
    var row = state.row;
    if (row) {
        row.unhighlight();
    }

    // Highlight cells
    row = new TrendTableRow();
    state.row = row;
    row.setRowByPeriod(rootIndex, period);
    row.highlightValue(columnIndex);
    if (!isComparator) {
        row.highlightCIs();
    }
    // Generate HTML
    var indicatorId = displayedTrendRoots[rootIndex].IID;
    var unit = ui.getMetadataHash()[indicatorId].Unit;
    var value = new ValueWithUnit(unit).getFullLabel(row.getValF());
    return '<b> ' + value + '<br/>' +
        '</b><br/><i>' + series.name + '</i><br/>' + period;
}

function getTrendRootsToDisplay() {
    return showAllIndicators() ?
        currentTrendRoots :
        [currentTrendRoots[getIndicatorIndex()]];
}

function getSingleTrendsAreaCode() {
    var neighbourAreaCode  = areaTrendsState.neighbourAreaCode;
    if (FT.model.isNearestNeighbours() && neighbourAreaCode) {
        return neighbourAreaCode;
    }
    return FT.model.areaCode;
}

function getSingleAreaTrendsHtml(indexesDisplayed) {

    var html = [];
    var areaCode = getSingleTrendsAreaCode();
    var areaName = getAreaNameToDisplay(areaHash[areaCode]);
   
    displayedTrendRoots = getTrendRootsToDisplay();

    var indicatorMetadataHash = ui.getMetadataHash();

    for (var i in displayedTrendRoots) {
        var root = displayedTrendRoots[i];
        if (isDefined(root)) {

            var metadata = indicatorMetadataHash[root.IID],
                h = [],
                comparisonConfig = new ComparisonConfig(root, metadata);

            h.push(getTrendHeader(metadata, root, areaName,
                'goToMetadataPage(' + i + ')'));

            var showCIText = areaTrendsState.showConfBars
                ? 'Hide confidence intervals'
                : 'Show confidence intervals';

            var exportTypes = '<div class="export-chart-box" ><a class="export-link"  href="javascript:exportChart(' +
                i + ')">Export chart as image</a>' + '<a  id="showCI' + i +
                '" class="show-ci" href="javascript:showErrorBar(' + i + ')">' + showCIText + '</a></div>';

            h.push('<div class="trendContainer">' + exportTypes + '<div id="trendChart', i, '" class="trendChart"></div>');

            // Trend table contents
            if (root.TrendMarkers)
            {
                h.push('<div class="trendMarker">Overall Trend:' + 
                    GetTrendMarkerImage(root.TrendMarkers[areaCode], root.PolarityId) + 
                    '</div>');
            }

            h.push('<div class="trendTableBox');
            var useWide = hasWidePeriodLabel(metadata);
            if (useWide) {
                h.push('Wide');
            }

            h.push('">');

            // Benchmark target key
            var useTarget = comparisonConfig.useTarget;
            if (useTarget) {
                h.push('<span class="target-label">Benchmarking against goal: </span><div class="target-legend">',
                    getTargetLegendHtml(comparisonConfig, metadata), '</div>');
            }

            // Table of trend data
            h.push('<table id="trendTable', i,
                '" class="borderedTable w100', (useTarget ? ' marginTop5' : ''), '" cellspacing="0">');

            addTrendTableHeader(h, useWide);
            var areAnyRows = false;

            var areaData = root.Data[areaCode];
            var periods = root.Periods;
            for (var j in periods) {

                var period = periods[j];

                if (isDefined(areaData)) {
                    var data = areaData[j];

                    comparatorValues = root.ComparatorValueFs[j];

                    addTrendRow(h, data, comparatorValues, period, comparisonConfig);
                }
                areAnyRows = true;
            }

            h.push('</table>',
                getSource(metadata),
                '</div></div>');

            if (areAnyRows) {
                html = html.concat(h);
                indexesDisplayed.push(i);
            }
        }
    }

    if (displayedTrendRoots.length == 0) {
        html.push(NO_TREND_DATA);
    }

    toggleQuintileLegend($('.quintileKey'), comparisonConfig.useQuintileColouring);

    return html;
}

function toggleViewMode(areaCode) {

    if (areaTrendsState.areaSwitch.getOption() === VIEW_MODES.AREA) {
        changeViewMode(VIEW_MODES.MULTI_AREA);
    } else {
        changeViewMode(VIEW_MODES.AREA);

        if (FT.model.isNearestNeighbours()) {
            areaTrendsState.neighbourAreaCode = areaCode;
        } else {
            FT.menus.area.setCode(areaCode);
            ftHistory.setHistory();
        }
    }

    displayTrends();
}

function displayTrends() {
    displayTrendRootTables();
    hideAndShowTrendKeys();
}

function getMultiAreaTrendsHtml() {

    var h = [],
    index = getIndicatorIndex(),
    root = currentTrendRoots[index];
    var chartCount = 0;

    if (root) {
        var comparatorVals = [],
        vals = root.ComparatorValue;
        for (var i in vals) {
            comparatorVals.push(vals[i][comparatorId]);
        }

        var cssClass = getSparkBoxClass(),
        metadata = ui.getMetadataHash()[root.IID];

        h.push(getTrendHeader(metadata, root, '',
            'goToMetadataPage(' + i + ')'));

        // Benchmark target key
        comparisonConfig = new ComparisonConfig(root, metadata);
        var useTarget = comparisonConfig.useTarget;
        if (useTarget) {
            h.push('<div class="target-box"><span class="target-label">Benchmarking against goal: </span>',
                getTargetLegendHtml(comparisonConfig, metadata), '</div>');
        }

        var isNearestNeighbourAdded = false;

        for (x in sortedAreasForTrends) {
            var area = sortedAreasForTrends[x],
                areaCode = area.Code,
                areaType = area.AreaTypeId;
            var dataList = root.Data[areaCode];

            if (isDefined(dataList)) {
                // Do not include areas without data
                for (var d in dataList) {
                    if (isValidValue(dataList[d].D)) {
                        var args = {};
                        args.cssClass = cssClass;
                        var shortAreaName;
                        if (!isNearestNeighbourAdded && FT.model.isNearestNeighbours() || _.size(sortedAreasForTrends) == 16) {
                            shortAreaName = area.Code === FT.model.areaCode ? area.Short : area.Rank + ' - ' + area.Short;
                            isNearestNeighbourAdded = true;
                        } else {
                            shortAreaName = areaHash[areaCode].Short;
                        }

                        if (areaType == AreaTypeIds.Practice) {
                            shortAreaName = areaCode + ' - ' + shortAreaName;
                        }

                        args.areaNameShort = trimName(shortAreaName, 23);

                        args.areaCode = areaCode;
                        args.chartId = 'at-' + areaCode;
                        h.push(templates.render('areaTrends', args));
                        chartCount++;
                        break;
                    }
                }
            }
        }
    }

    if (!chartCount) {
        h.push(NO_TREND_DATA);
    }

    return h;
}

function createSmallTrendChart(id, root, areaCode, comparatorName, comparisonConfig) {
    var labels = [],
    trendData = new TrendData(),
    dataList = root.Data[areaCode];

    for (var j in dataList) {
        var data = dataList[j],
        significance = data.Sig[comparisonConfig.comparatorId],
        markerColor = getColourFromSignificance(significance, comparisonConfig.useRagColours, colours, comparisonConfig.useQuintileColouring);
        trendData.addAreaPoint(data, markerColor);
        trendData.addBenchmarkPoint(root.ComparatorValue[j][comparatorId],
            parseFloat(root.ComparatorValueFs[j][comparatorId]));
        labels.push(root.Periods[j]);
    }

    // Min/Max
    if (root.Limits) {
        var max = root.Limits.Max;
        var min = getMinYAxis(root.Limits);

    } else {
        max = 0;
        min = 1;
    }

    try {
        new Highcharts.Chart({
            chart: {
                renderTo: id,
                defaultSeriesType: 'line',
                zoomType: 'xy',
                width: 240,
                height: 140,
                events: {
                    click: function (e) {
                        toggleViewMode(areaCode); /* swtich to single chart view  */
                    }
                }
            },
            title: {
                text: ''
            },
            xAxis: {
                title: {
                    enabled: false
                },
                categories: labels,
                labels: {
                    formatter: function () {

                        var val = this.value;

                        if (val.length > 22) {
                            // e.g. '2009/10 Q2 - 2010/11 Q1' 
                            val = val.replace(/20/g, '');
                        }

                        return val.indexOf('-') > -1 ?
                            val.replace('-', '<br>-') /*Break after - for ranges*/ :
                            val.replace(' ', '<br/>');

                    },
                    enabled: false,
                    step: getLabelStep(labels)
                },
                tickLength: 3,
                tickPosition: 'outside',
                tickWidth: 1,
                tickmarkPlacement: 'on'
            },
            yAxis: {
                title: {
                    text: ''
                },
                max: max,
                min: min
            },
            legend: {
                enabled: false,
                layout: 'vertical',
                borderWidth: 0

            },
            plotOptions: {
                line: {
                    enableMouseTracking: true,
                    lineWidth: 1,
                    animation: false,
                    marker: {
                        radius: dataList.length > 40 ? 1 : 3,
                        symbol: 'circle',
                        lineWidth: 1,
                        lineColor: '#000000'
                    },
                    events: {
                        mouseOut: function () {
                            clearTrendCells();
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
                    var series = this.series;
                    rootIndex = $(series.chart.container).parent().attr('id').match(/\d+/);
                    var indicatorId = root.IID,
                        unit = ui.getMetadataHash()[indicatorId].Unit,
                        value = new ValueWithUnit(unit).getFullLabel(this.point.ValF);

                    return '<b>' + value + '</b> ' +
                        '<br/><i>' + this.series.name + '</i><br/>' + this.x;
                }
            },
            credits: HC.credits,
            series: [
                {
                    data: trendData.getBenchmarkPoints(),
                    color: colours.comparator,
                    name: comparatorName
                },
                {
                    data: trendData.getAreaPoints(),
                    name: areaHash[areaCode].Name,
                    showInLegend: false
                }
            ],
            exporting: {
                enabled: false
            }
        });
    } catch (e) {}
}

/* Determines which elements are visible and enabled for pages that 
*  have both single and multi chart states */
function hideAndShowSparklineMenus() {

    var indicatorSelectAllTd = $('#indicatorSelectAllTd'),
        sortSelectionBox = $('#sortSelectionBox');

    if (areaTrendsState.areaSwitch.getOption() === VIEW_MODES.AREA) {
        // One area displayed in detail
        indicatorSelectAllTd.show();
        sortSelectionBox.hide();

    } else {
        // Multiple areas displayed each with its own mini-chart
        indicatorSelectAllTd.hide();
        sortSelectionBox.show();
    }
}

function changeViewMode(mode) {
    var areaSwitch = areaTrendsState.areaSwitch;
    if (areaSwitch.getOption() !== mode) {
        areaSwitch.setOption(mode);
        hideAndShowSparklineMenus();
        configureIndicatorMenu();
    }
}

function getSparkBoxClass() {
    return areaTrendsState.areaSwitch.getOption() === VIEW_MODES.AREA
        ? 'sparklineArea'
        : 'sparklineIndicator';
}

function sortSparklineClicked(mode) {
    if (areaTrendsState.areaSwitch.getOption() === VIEW_MODES.MULTI_AREA) {
        areaTrendsState.sparklineSort = mode;
        sortSparklines();
        displayTrends();
    }
}

function sortSparklines() {

    if (areaTrendsState.sparklineSort == SPARKLINE_SORTS.AREA) {
        sortAreasAToZ(0, sortedAreasForTrends);
    } else if (areaTrendsState.sparklineSort == SPARKLINE_SORTS.RANK) {
        var sortedAreasList = [];
        var areaCodesByNearestNeighboursRank = [];
        // NN ranks are based on areaHash, extract the areaCodes and loop over for sorting
        _.each(areaHash, function (value, key) { areaCodesByNearestNeighboursRank.push(key); });

        for (var i = 0; i < _.size(areaCodesByNearestNeighboursRank) ; i++) {
            sortedAreasList.push(_.findWhere(sortedAreas, { Code: areaCodesByNearestNeighboursRank[i] }));
        }

        sortedAreasForTrends = sortedAreasList;
    } else {
        var rootIndex = getIndicatorIndex();
        var groupRoot = groupRoots[getIndicatorIndex()];
        sortedAreasForTrends = new AreaAndDataSorter(sortOrder.getOrder(rootIndex), groupRoot.Data, sortedAreasForTrends, areaHash).byValue();
    }
}

areaTrendsState = (function () {

    this.row;
    this.isInitialised = false;
    this.sparklineSort = SPARKLINE_SORTS.BY_AREA;

    /* The code of the neighbour area for which the large trend chart is displayed */
    this.neighbourAreaCode = null;
    this.showConfBars = false;

    /* Trend data only used for area trends page so data 
    *  encapsulated here rather than attached to loaded object */
    var data = {};

    getTrendKey = function (areaCode) {
        return getKey(FT.model.groupId, FT.model.areaTypeId, areaCode, FT.model.nearestNeighbour);
    };

    return {

        getTrendData: function (areaCode) {
            var key = getTrendKey(areaCode);
            return isDefined(data[key]) ?
                data[key] :
                null;
        },

        setTrendData: function (areaCode, newData) {
            data[getTrendKey(areaCode)] = newData;
        },

        isTrendDataLoaded: function (areaCode) {
            return isDefined(data[getTrendKey(areaCode)]);
        }
    };
})();

function hideAndShowTrendKeys() {
    var $elements = $('#sort-many-charts,#spine-range-key');
    if (areaTrendsState.areaSwitch.getOption() == VIEW_MODES.AREA) {
        $elements.hide();
    } else {
        $elements.show();
    }
}

function viewClicked(viewMode) {
    changeViewMode(viewMode);
    displayTrends();
}

function TrendData() {

    var benchmarkPoints = [],
    areaPoints = [],
    CIPoints = [],
    getValue = function (v) {
        return isValidValue(v) ? v : null;
    };

    this.addAreaPoint = function (d, markerColor) {
        areaPoints.push({
            y: getValue(d.D),
            ValF: getValue(parseFloat(d.V)),
            color: markerColor,
            noteId: d.NoteId,
            marker: {
                fillColor: markerColor,
                states: {
                    hover: { fillColor: markerColor }
                }
            }
        });

        CIPoints.push([parseFloat(d.L), parseFloat(d.U)]);
    };

    this.addBenchmarkPoint = function (value, valueF) {
        benchmarkPoints.push({
            y: getValue(value),
            ValF: valueF,
            isBenchmark: true
        });
    };

    this.getBenchmarkPoints = function () {
        return benchmarkPoints;
    };

    this.getAreaPoints = function () {
        return areaPoints;
    };

    this.getCIPoints = function () {
        return CIPoints;
    };

}

trendTableColumnIndexes = {
    value: 3,
    lowerCI: 4,
    upperCI: 5
};

function TrendTableRow() {

    var _this = this,
    ciCells, rowCells, cells,
    ciCssClass = 'light';

    _this.getValF = function () {
        return cells.text();
    };

    _this.setRowByPeriod = function (rootIndex, period) {
        // Identify trend table row by period
        var rowIndex = 1;
        while (rowIndex < 100) {
            rowCells = $('#trendTable' + rootIndex +
                ' tr:eq(' + rowIndex++ + ')').children();
            if ($(rowCells[0]).text() === period) break;
        }
    };

    _this.setRowFromElement = function (e) {
        var tds = $(e).parent().parent().children();
        cells = $([tds[trendTableColumnIndexes.value],
            tds[getComparatorCellIndex()]]);
    };

    _this.highlightValue = function (columnIndex) {
        cells = $(rowCells[columnIndex]);
        _this.toggleValueCellHighlights();
    };

    _this.highlightCIs = function () {
        ciCells = rowCells.filter(
            ':eq(' + trendTableColumnIndexes.lowerCI + '),:eq(' + trendTableColumnIndexes.upperCI + ')');
        ciCells.addClass(ciCssClass);
    };

    _this.unhighlight = function () {
        if (cells) {
            _this.toggleValueCellHighlights();
        }
        if (ciCells) {
            ciCells.removeClass(ciCssClass);
        }
        cells = ciCells = null;
    };

    _this.toggleValueCellHighlights = function () {
        if (cells) {
            cells.toggleClass('coloured').toggleClass('bold');
        }
    };
}

NO_TREND_DATA = '<div class="noData300">No trend data available</div>';
currentTrendRoots = null;
viewModeState = 0;
var sortedAreasForTrends = [];

pages.add(PAGE_MODES.AREA_TRENDS, {
    id: 'trends',
    title: 'Trends',
    goto: goToAreaTrendsPage,
    gotoName: 'goToAreaTrendsPage',
    needsContainer: true,
    jqIds: [
        'indicatorSelectAllTd', 'indicatorMenuDiv',
        'sparkLineViews', '.geo-menu', 'trends-chart-sorter-az', 'show-all-indicators-box',
        'areaSwitch', 'key-spine-chart', 'value-note-legend', 'nearest-neighbour-link', 'trend-marker-legend-spine'],
    jqIdsNotInitiallyShown: ['data-quality-key', 'target-benchmark-box', 'key-spine-chart'],
    showHide: hideAndShowSparklineMenus
});

templates.add('areaTrends', '<div class="sparklineBox {{cssClass}} " >' +
    '<div class="multiTrendHeader"><a onclick="toggleViewMode(\'{{areaCode}}\');"> {{areaNameShort}} </a></div>' +
    '<div class="fl">' +
    '<div id={{chartId}}></div>' +
    '</div></div>');

