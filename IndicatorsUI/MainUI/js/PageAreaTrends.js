'use strict';

/**
* Area trends namespace
* @module areaTrends
*/
var areaTrends = {};

function goToAreaTrendsPage() {

    setPageMode(PAGE_MODES.AREA_TRENDS);

    if (!areIndicatorsInDomain()) {
        displayNoData();
    } else {

        var model = FT.model;
        sortedAreasForTrends = FT.data.sortedAreas;

        if (model.isNearestNeighbours()) {
            model.parentCode = model.nearestNeighbour;
        }

        lock();
        tooltipManager.setTooltipProvider(new ValueNoteTooltipProvider());
        getTrendData();
        $('#benchmark-key-text').html(getCurrentComparator().Name);
    }

    $('#nearest-neighbour-links').bind('click', function () {
        areaTrendsState.neighbourAreaCode = null;
    });
}

var SPARKLINE_SORTS = {
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

        var parameters = new ParameterBuilder(
        ).add('group_id', model.groupId
            ).add('area_type_id', model.areaTypeId
            ).add('parent_area_code', parentCode);

        addProfileOrIndicatorsParameters(parameters);

        ajaxGet('api/trend_data/all_indicators_in_profile_group_for_child_areas', parameters.build(), getTrendDataCallback);
    }
}

function getTrendDataCallback(trendRoots) {

    if (isDefined(trendRoots)) {
        currentTrendRoots = trendRoots;

        areaTrendsState.setTrendData(FT.model.parentCode,
            currentTrendRoots);

    } else {
        currentTrendRoots = null;
    }

    if (!areaTrendsState.isInitialised) {
        initTabSpecificOptions();
        areaTrendsState.isInitialised = true;
    }

    displayTrendRootTables();

    showAndHidePageElements();

    hideAndShowTrendKeys();

    showDataQualityLegend();

    showTargetBenchmarkOption(currentTrendRoots);

    displayChartOptions();

    overrideLegend();

    unlock();

    loadValueNoteToolTips();
}

function overrideLegend() {
    if ($('#key-spine-chart').is(':visible')) {

        var $rag3 = $('#spine-chart-legend-rag-3'),
            $rag5 = $('#spine-chart-legend-rag-5'),
            $bob = $('#spine-chart-legend-bob'),
            $quintileRag = $('#spine-quintile-rag'),
            $quintileBob = $('#spine-quintile-bob'),
            $ragAndBobSection = $('#spine-rag-and-bob-section'),
            $quintileSection = $('#spine-quintile-key-table');

        $rag3.hide();
        $rag5.hide();
        $bob.hide();
        $quintileRag.hide();
        $quintileBob.hide();
        $ragAndBobSection.hide();
        $quintileSection.hide();

        if (areaTrendsState.showAll) {
            for (var i = 0; i < groupRoots.length; i++) {
                overrideLegendForGroupRoot(groupRoots[i], $rag3, $rag5, $bob, $quintileRag, $quintileBob, $ragAndBobSection, $quintileSection);
            }
        } else {
            overrideLegendForGroupRoot(getGroupRoot(), $rag3, $rag5, $bob, $quintileRag, $quintileBob, $ragAndBobSection, $quintileSection);
        }
    }
}

function overrideLegendForGroupRoot(groupRoot, rag3, rag5, bob, quintileRag, quintileBob, ragAndBobSection, quintileSection) {
    var polarityId = groupRoot.PolarityId,
        comparatorMethodId = groupRoot.ComparatorMethodId;

    switch (comparatorMethodId) {
        // Quintiles
        case ComparatorMethodIds.Quintiles:
            if (polarityId === PolarityIds.NotApplicable) {
                // Quintile BOB
                quintileBob.show();
            } else {
                // Quintile RAG
                quintileRag.show();
            }

            quintileSection.show();
            break;

        // RAG3
        case ComparatorMethodIds.SingleOverlappingCIsForOneCiLevel:
            rag3.show();
            ragAndBobSection.show();
            break;

        // RAG5
        case ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels:
            rag5.show();
            ragAndBobSection.show();
            break;

        default:
            switch (polarityId) {
                // BOB
                case PolarityIds.BlueOrangeBlue:
                    bob.show();
                    break;

                // RAG
                case PolarityIds.RAGLowIsGood:
                case PolarityIds.RAGHighIsGood:
                    rag3.show();
                    break;
            }

            ragAndBobSection.show();
            break;
    }
}

function initTabSpecificOptions() {

    var singleAreaClickHandler = function () {
        viewModeState = VIEW_MODES.AREA;
        viewClicked(VIEW_MODES.AREA);
    };

    var multiAreaClickHandler = function () {
        viewModeState = VIEW_MODES.MULTI_AREA;
        viewClicked(VIEW_MODES.MULTI_AREA);
    };

    areaTrendsState.tabSpecificOptions = new TabSpecificOptions({
        eventHandlers: [singleAreaClickHandler, multiAreaClickHandler],
        eventName: 'ChartCountChanged'
    });

}

areaTrends.displayIndicatorsOptions = function () {

    // Define template
    var singleOrAllOption = '<span id="tab-specific-more-options">' +
        '<div>' +
        '<span>Display</span>' +
        '<button id="show-single" class="{{singleSelected}}" onclick="setShowAllIndicators(false);">Selected indicator</button>' +
        '<button id="show-all" class="{{allSelected}}" onclick="setShowAllIndicators(true);">All indicators</button>' +
        '</div>' +
        '</span>';

    var templateName = 'singleOrAll';
    templates.add(templateName, singleOrAllOption);

    // Create view model
    var selectedClass = 'button-selected';
    var singleIndicatorSelected = selectedClass,
        showAllSelected = '';
    if (areaTrendsState.showAll) {
        singleIndicatorSelected = '';
        showAllSelected = selectedClass;
    }

    // Render template
    var viewModel = { singleSelected: singleIndicatorSelected, allSelected: showAllSelected };
    return templates.render(templateName, viewModel);
}

function setShowAllIndicators(shouldShowAll) {
    var cssClass = 'button-selected';
    var $all = $('#show-all');
    var $single = $('#show-single');

    if (shouldShowAll) {
        $single.removeClass(cssClass);
        $all.addClass(cssClass);
    } else {
        $all.removeClass(cssClass);
        $single.addClass(cssClass);
    }

    showAllIndicatorsChanged(shouldShowAll);
}

function showAllIndicatorsChanged(shouldShowAll) {
    areaTrendsState.showAll = shouldShowAll;
    pages.goToCurrent();
}

function displayChartOptions() {
    var model = FT.model;
    var state = areaTrendsState;
    var tabSpecificOptions = state.tabSpecificOptions;
    var areaCode = getSingleTrendsAreaCode();
    var area = areaHash[areaCode];

    // Display area options
    var labels = [getAreaNameToDisplay(area)]
    var label2 = model.isNearestNeighbours()
        ? 'All nearest neighbours for ' + area.Short
        : 'All in ' + getParentArea().Name;

    if (!isEnglandAreaType()) {
        labels.push(label2);
    } else {
        // When england area type it forces to preview the area model state
        viewModeState = VIEW_MODES.AREA;
    }

    tabSpecificOptions.setHtml({
        label: 'Trends for',
        optionLabels: labels
    });

    if (tabSpecificOptions.getOption() === VIEW_MODES.MULTI_AREA) {
        var atozSorter = '<div id="sorting-options" style="text-align:center"><span style="float:left;">Sort charts by: </span>' +
            '<button id="spark0" onclick="javascript:sortSparklineClicked(SPARKLINE_SORTS.AREA);" class="optionButton" title="Sort alphabetically by area name">A-Z</button> ' +
            '<button id="spark1" onclick="javascript:sortSparklineClicked(SPARKLINE_SORTS.AVERAGE);" class="optionButton" title="Sort by the most recent value">Value</button></div>';

        var templateName = 'atozChartSorter';
        templates.add(templateName, atozSorter);
        var html = templates.render(templateName);

        $('#trends-chart-sorter-az').html(html);
        $("#tab-specific-options").append(html);

        // add rank sorting option after template is randered
        if (model.isNearestNeighbours()) {
            $('#spark1').after('<button id="spark2" onclick="javascript:sortSparklineClicked(SPARKLINE_SORTS.RANK);" class="optionButton" title="Sort by rank">Rank</button>');
        } else {
            $('#spark2').remove();
        }
    }

    // Display indicator options for large chart
    if (viewModeState === VIEW_MODES.AREA) {
        $('#tab-specific-options').append(areaTrends.displayIndicatorsOptions());
    }

}

function addTrendRow(h, dataPoint, comparatorData, period, comparisonConfig, indicatorId, sexId, ageId) {

    var regionalData = comparatorData[REGIONAL_COMPARATOR_ID];
    var nationalData = comparatorData[NATIONAL_COMPARATOR_ID];
    var dataInfo = new TrendDataInfo(dataPoint);
    var regionalDataInfo = new CoreDataSetInfo(regionalData);
    var nationalDataInfo = new CoreDataSetInfo(nationalData);

    var isValue = dataInfo.isValue();

    // Omit row if no data for any area
    if (!isValue && !regionalDataInfo.isValue() && !nationalDataInfo.isValue()) {
        return;
    }

    h.push('<tr>');
    addTd(h, period, CSS_CENTER);

    // Significance marker
    var marker = isValue ?
        getTrendMarkerHtml(dataPoint, comparisonConfig, indicatorId, sexId, ageId) :
        EMPTY_TD_CONTENTS;
    addTd(h, marker, CSS_CENTER);

    // Area count, value & CIs
    var valueDisplayer = new ValueDisplayer();
    addTd(h, formatCount(dataInfo), CSS_NUMERIC);
    addTd(h, valueDisplayer.byDataInfo(dataInfo), CSS_NUMERIC, null, dataInfo.getNoteId());
    addTd(h, valueDisplayer.byNumberString(dataPoint.L), CSS_NUMERIC);
    addTd(h, valueDisplayer.byNumberString(dataPoint.U), CSS_NUMERIC);

    // Subnational column
    if (isSubnationalColumn()) {
        addTd(h, valueDisplayer.byDataInfo(regionalDataInfo), CSS_NUMERIC, null, regionalDataInfo.getNoteId());
    }

    if (!isEnglandAreaType()) {
        // National column
        if (enumParentDisplay !== PARENT_DISPLAY.REGIONAL_ONLY) {
            addTd(h, valueDisplayer.byDataInfo(nationalDataInfo), CSS_NUMERIC, null, nationalDataInfo.getNoteId());
        }
    }

    h.push('</tr>');

    return;
}

function getTrendMarkerHtml(data, comparisonConfig, indicatorId, sexId, ageId) {
    var sig = data.Sig[comparisonConfig.comparatorId],
        useRag = comparisonConfig.useRagColours,
        useQuintileColouring = comparisonConfig.useQuintileColouring;

    var marker = ['<img src="', FT.url.img,
        getMiniMarkerImageFromSignificance(sig, useRag, useQuintileColouring, indicatorId, sexId, ageId), '"'];

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

    var html = [];
    var rootIndexesDisplayed = [];

    var viewMode = areaTrendsState.tabSpecificOptions.getOption();

    // Set HTML for page
    if (currentTrendRoots !== null) {
        displayChartOptions();
        html = viewMode === VIEW_MODES.MULTI_AREA ?
            getSmallMultipleTrendChartsHtml() :
            getLargeTrendChartAndTableHtml(rootIndexesDisplayed);
    }
    pages.getContainerJq().html(html.join(''));

    // Display charts (after HTML has been set)
    if (viewMode === VIEW_MODES.AREA) {
        initLargeTrendCharts(rootIndexesDisplayed);
    }
    else {
        initSmallMultipleTrendCharts();
    }
}

function initSmallMultipleTrendCharts() {
    // Multiple small trend charts
    var i;
    var rootIndex = getIndicatorIndex();
    var groupRoot = groupRoots[rootIndex];
    var trendRoot = areaTrends.findMatchingTrendRoot(groupRoot, currentTrendRoots);

    var comparatorName = getCurrentComparator().Name;

    if (isDefined(trendRoot)) {
        var comparatorVals = [],
            vals = trendRoot.ComparatorValue;
        for (i in vals) {
            comparatorVals.push(vals[i][comparatorId]);
        }

        var metadata = ui.getMetadataHash()[trendRoot.IID];
        var comparisonConfig = new ComparisonConfig(trendRoot, metadata);

        for (i in sortedAreasForTrends) {
            var areaCode = sortedAreasForTrends[i].Code;
            if (new CoreDataSetList(trendRoot.Data[areaCode]).areAnyValidTrendValues()) {
                createSmallTrendChart('at-' + areaCode, trendRoot, areaCode,
                    comparatorName, comparisonConfig);
            }
        }
    }
}

function initLargeTrendCharts(indexesDisplayed) {
    var comparatorName = getCurrentComparator().Name;
    var areaCode = getSingleTrendsAreaCode();

    for (var i = 0; i < indexesDisplayed.length; i++) {
        var index = indexesDisplayed[i];
        var groupRoot = displayedGroupRoots[index];
        var trendRoot = areaTrends.findMatchingTrendRoot(groupRoot, currentTrendRoots);
        new Highcharts.Chart(
            getLargeTrendChartOptions('trendChart' + index, trendRoot, areaCode, comparatorName)
        );
    }
}

function addTrendTableHeader(h, useWide) {
    h.push('<thead class="data-header"><tr>');
    addTh(h, 'Period', useWide ? 'trend-period-wide' : 'trendPeriod');
    addTh(h, null, 'trendSig');
    addTh(h, 'Count');
    addTh(h, 'Value');
    addTh(h, 'Lower CI', null, 'Lower Confidence Interval');
    addTh(h, 'Upper CI', null, 'Upper Confidence Interval');

    if (isSubnationalColumn()) {
        addTh(h, getParentArea().Short);
    }

    if (!isEnglandAreaType()) {
        if (enumParentDisplay !== PARENT_DISPLAY.REGIONAL_ONLY) {
            addTh(h, getNationalComparator().Name);
        }
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

        var td = $('#indicator-details-table tbody tr:eq(' + index + ') td:eq(2)');

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

    var showBenchmarkData = metadata.ValueType.Id !== ValueTypeIds.Count;

    for (var j in dataList) {

        var data = dataList[j],
            significance = data.Sig[comparisonConfig.comparatorId],
            markerColor = getColourFromSignificance(significance, comparisonConfig.useRagColours,
                colours, comparisonConfig.useQuintileColouring, root.IID, root.Sex.Id, root.Age.Id);

        trendData.addAreaPoint(data, markerColor);

        if (showBenchmarkData) {
            trendData.addBenchmarkPoint(root.ComparatorData[j][comparatorId]);
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
    var groupRoot = displayedGroupRoots[groupRootIndex];
    var indicatorName = ui.getMetadataHash()[groupRoot.IID].Descriptive.Name + new SexAndAge().getLabel(groupRoot);
    var trendRoot = areaTrends.findMatchingTrendRoot(groupRoot, currentTrendRoots);
    var chartOptions = getLargeTrendChartOptions('trendChart' + groupRootIndex,
        trendRoot, areaCode, getCurrentComparator().Name);

    var newChart = new Highcharts.Chart(chartOptions);

    newChart.exportChart({ type: 'image/png' }, {
        chart: {
            spacingTop: 70,
            width: 400,
            height: 350,
            events: {
                load: function () {
                    // Add title to chart
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

    logEvent('ExportImage', getCurrentPageTitle());
}

function exportChartAsCsv(iidIndex) {

    var parameters = new ParameterBuilder()
    .add('parent_area_type_id', FT.model.parentTypeId)
    .add('child_area_type_id', FT.model.areaTypeId)
    .add('profile_id', FT.model.profileId)
    .add('areas_code', getAreaCode())
    .add('indicator_ids', getTrendsIid(iidIndex))
    .add('parent_area_code', FT.model.parentCode);
      
    downloadAllPeriodsNoInequalitiesDataCsvFileByIndicator(FT.url.corews, parameters);
}

function getTrendsIid(index) {
    var trendLinknumber = $(".trend-link").length;

    if (trendLinknumber === 1) {
        index = $('#indicatorMenu').prop('selectedIndex');
    }

    return FT.model.groupRoots[index].IID;
}

function getAreaCode() {
    if (FT.model.isNearestNeighbours()) {        
        var neighbourSelectedName = $("#tab-option-0").text();
        
        var selectedArea = FT.data.sortedAreas.find(area => area.Name === neighbourSelectedName );
        return selectedArea["Code"];  
    }

    return FT.model.areaCode;
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

    if (!FT.ajaxLock) {
        lock();

        displayTrendRootTables();
        unlock();
    }
}

function showExportMenu() {
    return areaTrendsState.tabSpecificOptions.getOption() === VIEW_MODES.AREA ? true : false;
}

function getMinYAxis(limits) {
    var min = limits.Min;
    return FT.config.startZeroYAxis && min >= 0
        ? 0
        : min;
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
    var t = $('#trend-helper');
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
    $('#trend-helper').hide();
}

function getComparatorCellIndex() {

    var upperCIIndex = trendTableColumnIndexes.upperCI;
    return (comparatorId === REGIONAL_COMPARATOR_ID ||
        enumParentDisplay !== PARENT_DISPLAY.NATIONAL_AND_REGIONAL ||
        FT.model.parentTypeId === AreaTypeIds.Country) ?
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
    var id = metadata.YearType.Id;
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
    var isComparator = dataPoint.isBenchmark;
    var columnIndex = isComparator ?
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
    var indicatorId = displayedGroupRoots[rootIndex].IID;
    var unit = ui.getMetadataHash()[indicatorId].Unit;
    var value = new ValueWithUnit(unit).getFullLabel(row.getValF());
    return '<b> ' + value + '<br/>' +
        '</b><br/><i>' + series.name + '</i><br/>' + period;
}

function getGroupRootsToDisplay() {
    return showAllIndicators() ?
        groupRoots :
        [groupRoots[getIndicatorIndex()]];
}

function getSingleTrendsAreaCode() {
    var neighbourAreaCode = areaTrendsState.neighbourAreaCode;
    if (FT.model.isNearestNeighbours() && neighbourAreaCode) {
        return neighbourAreaCode;
    }
    return FT.model.areaCode;
}

/**
* Finds the trend root that matches the group root. These should be in same order
* but are not always in the search results on live (reason unknown)
* @class findMatchingTrendRoot
*/
areaTrends.findMatchingTrendRoot = function (groupRoot, trendRoots) {
    return matchBySexAgeAndIID(groupRoot, trendRoots);
}

function getLargeTrendChartAndTableHtml(indexesDisplayed) {

    var html = [];
    var areaCode = getSingleTrendsAreaCode();
    var areaName = getAreaNameToDisplay(areaHash[areaCode]);

    displayedGroupRoots = getGroupRootsToDisplay();

    var indicatorMetadataHash = ui.getMetadataHash();

    for (var i in displayedGroupRoots) {

        var groupRoot = displayedGroupRoots[i];

        var trendRoot = areaTrends.findMatchingTrendRoot(groupRoot, currentTrendRoots);

        if (isDefined(trendRoot)) {

            var metadata = indicatorMetadataHash[trendRoot.IID],
                h = [],
                comparisonConfig = new ComparisonConfig(trendRoot, metadata);

            h.push(getTrendHeader(metadata, trendRoot, areaName,
                'goToMetadataPage(' + i + ')', hasDataChanged(groupRoot)));

            var showCIText = areaTrendsState.showConfBars
                ? 'Hide confidence intervals'
                : 'Show confidence intervals';

            var exportTypes = '<div class="export-chart-box" ><a class="export-link"  href="javascript:exportChart(' +
                i + ')">Export chart as image</a>' + '<a  id="showCI' + i +
                '" class="show-ci" href="javascript:showErrorBar(' + i + ')">' + showCIText + '</a></div>'+
                '<div class="export-chart-box-csv"><a id="export-link-csv-trend" class="export-link-csv" href="javascript:exportChartAsCsv(' + i + ')">Export chart as csv file</a></div>';

            h.push('<div class="trend-container">', exportTypes, '<div id="trendChart', i, '" class="trendChart"></div>');

            // Trend table contents
            var recentTrends = trendRoot.RecentTrends;
            if (recentTrends) {
                var recentTrend = FT.config.hasRecentTrends && recentTrends[areaCode]
                    ? 'Recent trend:' + getTrendMarkerImage(recentTrends[areaCode].Marker, trendRoot.PolarityId)
                    : '';
                h.push('<div class="trend-marker" onmouseover="showRecentTrendTooltip(this,true);"  onmouseout="showRecentTrendTooltip(this,false);"">',
                    recentTrend, '</div>');
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
                '" class="bordered-table w100', (useTarget ? ' marginTop5' : ''), '" cellspacing="0">');

            addTrendTableHeader(h, useWide);
            var areAnyRows = false;

            var areaData = trendRoot.Data[areaCode];
            var periods = trendRoot.Periods;
            for (var j in periods) {

                var period = periods[j];

                if (isDefined(areaData)) {
                    var data = areaData[j];
                    var comparatorData = trendRoot.ComparatorData[j];
                    addTrendRow(h, data, comparatorData, period, comparisonConfig, trendRoot.IID, trendRoot.Sex.Id, trendRoot.Age.Id);
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

    if (displayedGroupRoots.length === 0) {
        html.push(NO_TREND_DATA);
    }

    toggleQuintileLegend($('.quintile-key'), comparisonConfig.useQuintileColouring);

    return html;
}

function showRecentTrendTooltip(e, showOrHide) {

    if (showOrHide) {
        var trendTooltip = new RecentTrendsTooltip(),
            recentTrends = groupRoots[getIndicatorIndex()].RecentTrends,
            trend = recentTrends[getSingleTrendsAreaCode()],
            tooltip = trendTooltip.getTooltipByData(trend);
        displayTrendTooltip(e, tooltip);
    } else {
        $('#trend-helper').hide();
    }
}

function toggleViewMode(areaCode) {

    if (areaTrendsState.tabSpecificOptions.getOption() === VIEW_MODES.AREA) {
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


function getSmallMultipleTrendChartsHtml() {
    var h = [],
        index = getIndicatorIndex(),
        root = currentTrendRoots[index];
    var chartCount = 0;
    if (root) {
        var comparatorVals = [],
            vals = root.ComparatorValue;

        var hasDataChanged = false;
        var currentRoot = getGroupRootsToDisplay();
        if (currentRoot && currentRoot[0].DateChanges) {
            hasDataChanged = currentRoot[0].DateChanges.HasDataChangedRecently;
        }

        for (var i in vals) {
            comparatorVals.push(vals[i][comparatorId]);
        }

        var cssClass = getSparkBoxClass(),
            metadata = ui.getMetadataHash()[root.IID];

        h.push(getTrendHeader(metadata, root, '',
            'goToMetadataPage(' + i + ')', hasDataChanged));

        // Benchmark target key
        var comparisonConfig = new ComparisonConfig(root, metadata);
        var useTarget = comparisonConfig.useTarget;
        if (useTarget) {
            h.push('<div class="target-box"><span class="target-label">Benchmarking against goal: </span>',
                getTargetLegendHtml(comparisonConfig, metadata), '</div>');
        }

        for (var x in sortedAreasForTrends) {
            var area = sortedAreasForTrends[x];
            var areaCode = area.Code;
            var areaTypeId = area.AreaTypeId;
            var dataList = root.Data[areaCode];

            // Do not include areas without data
            if (isDefined(dataList)) {
                for (var d in dataList) {
                    if (isValidValue(dataList[d].D)) {

                        // Set area name as chart title
                        if (areaTypeId === AreaTypeIds.Practice) {
                            var chartTitle = areaCode + ' - ' + area.Short;
                        }
                        else if (FT.model.isNearestNeighbours()) {
                            chartTitle = areaCode === FT.model.areaCode
                                ? area.Short
                                : area.Rank + ' - ' + area.Short;
                        } else {
                            chartTitle = area.Short;
                        }

                        // Define view model
                        var viewModel = {};
                        viewModel.cssClass = cssClass;
                        viewModel.areaName = trimName(chartTitle, 23);
                        viewModel.areaCode = areaCode;
                        viewModel.chartId = 'at-' + areaCode;

                        h.push(templates.render('areaTrends', viewModel));

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
    var labels = [], data,
        trendData = new TrendData(),
        dataList = root.Data[areaCode];

    for (var j in dataList) {
        var data = dataList[j];
        var significance = data.Sig[comparisonConfig.comparatorId];
        var markerColor = getColourFromSignificance(significance, comparisonConfig.useRagColours, colours,
            comparisonConfig.useQuintileColouring, root.IID, root.Sex.Id, root.Age.Id);

        if (root.ComparatorMethodId === ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels) {
            markerColor = '#FFF000';
        }
        trendData.addAreaPoint(data, markerColor);
        trendData.addBenchmarkPoint(root.ComparatorData[j][comparatorId]);
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
                        toggleViewMode(areaCode); /* switch to single chart view  */
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

                    var data = this.point.data;

                    // Data object type may be CoreDataSet or TrendDataPoint
                    var valF = !!data['V'] ? data.V : data.ValF;

                    // Tooltip over value on small trend chart
                    var indicatorId = root.IID,
                        unit = ui.getMetadataHash()[indicatorId].Unit,
                        value = new ValueWithUnit(unit).getFullLabel(valF);

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
    } catch (e) { }
}

/* Determines which elements are visible and enabled for pages that 
*  have both single and multi chart states */
function hideAndShowSparklineMenus() {
    if (areIndicatorsInDomain()) {
        var $indicatorSelectAllTd = $('#indicator-select-all-td'),
            $sortSelectionBox = $('#sort-selection-box');

        if (areaTrendsState.tabSpecificOptions.getOption() === VIEW_MODES.AREA) {
            // One area displayed in detail
            $indicatorSelectAllTd.show();
            $sortSelectionBox.hide();

        } else {
            // Multiple areas displayed each with its own mini-chart
            $indicatorSelectAllTd.hide();
            $sortSelectionBox.show();
        }
    }
}

function changeViewMode(mode) {
    var tabSpecificOptions = areaTrendsState.tabSpecificOptions;
    if (tabSpecificOptions.getOption() !== mode) {
        tabSpecificOptions.setOption(mode);
        hideAndShowSparklineMenus();
    }
}

function getSparkBoxClass() {
    return areaTrendsState.tabSpecificOptions.getOption() === VIEW_MODES.AREA
        ? 'sparkline-area'
        : 'sparkline-indicator';
}

function sortSparklineClicked(mode) {
    if (areaTrendsState.tabSpecificOptions.getOption() === VIEW_MODES.MULTI_AREA) {
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

        for (var i = 0; i < _.size(areaCodesByNearestNeighboursRank); i++) {
            sortedAreasList.push(_.findWhere(FT.data.sortedAreas, { Code: areaCodesByNearestNeighboursRank[i] }));
        }

        sortedAreasForTrends = sortedAreasList;
    } else {
        var rootIndex = getIndicatorIndex();
        var groupRoot = groupRoots[getIndicatorIndex()];
        sortedAreasForTrends = new AreaAndDataSorter(sortOrder.getOrder(rootIndex), groupRoot.Data, sortedAreasForTrends, areaHash).byValue();
    }
}

var areaTrendsState = new (function () {
    this.showAll = false;
    this.row = null;
    this.isInitialised = false;
    this.sparklineSort = SPARKLINE_SORTS.BY_AREA;

    /* The code of the neighbour area for which the large trend chart is displayed */
    this.neighbourAreaCode = null;
    this.showConfBars = false;

    /* Trend data only used for area trends page so data 
    *  encapsulated here rather than attached to loaded object */
    var data = {};

    var getTrendKey = function (areaCode) {
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
    if (areaTrendsState.tabSpecificOptions.getOption() == VIEW_MODES.AREA) {
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
        CIPoints = [];

    this.addAreaPoint = function (trendDataPoint, markerColor) {

        var info = new TrendDataInfo(trendDataPoint);
        areaPoints.push({
            y: info.isValue() ? trendDataPoint.D : null,
            data: trendDataPoint,
            color: markerColor,
            noteId: trendDataPoint.NoteId,
            marker: {
                fillColor: markerColor,
                states: {
                    hover: { fillColor: markerColor }
                }
            }
        });

        CIPoints.push([
            parseFloat(trendDataPoint.L),
            parseFloat(trendDataPoint.U)]);
    };

    this.addBenchmarkPoint = function (trendDataPoint) {

        var info = new CoreDataSetInfo(trendDataPoint);
        var y = info.isValue() ? trendDataPoint.Val : null;

        benchmarkPoints.push({
            y: y,
            data: trendDataPoint,
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

var trendTableColumnIndexes = {
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


var NO_TREND_DATA = '<div class="no-indicator-data">No trend data available</div>';
var currentTrendRoots = null;
var viewModeState = 0;
var sortedAreasForTrends = [];
var displayedGroupRoots;

pages.add(PAGE_MODES.AREA_TRENDS, {
    id: 'trends',
    title: 'Trends',
    goto: goToAreaTrendsPage,
    gotoName: 'goToAreaTrendsPage',
    needsContainer: true,
    jqIds: [
        'indicator-select-all-td', 'indicator-menu-div',
        'sparkline-views', '.geo-menu', 'trends-chart-sorter-az',
        'tab-specific-options', 'key-spine-chart', 'value-note-legend', 'nearest-neighbour-link', 'trend-marker-legend', 'area-list-wrapper', 'filter-indicator-wrapper'],
    jqIdsNotInitiallyShown: ['data-quality-key', 'target-benchmark-box', 'key-spine-chart'],
    showHide: hideAndShowSparklineMenus
});

templates.add('areaTrends', '<div class="sparkline-box {{cssClass}} " >' +
    '<div class="multi-trend-header"><a onclick="toggleViewMode(\'{{areaCode}}\');">{{areaName}}</a></div>' +
    '<div class="fl">' +
    '<div id={{chartId}}></div>' +
    '</div></div>');

