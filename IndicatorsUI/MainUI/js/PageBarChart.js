/**
* Defined in PageBarChart.js
* @module barChart
*/

function goToIndicatorDetailsPage(rootIndex) {

    if (!groupRoots.length) {
        // Search results empty
        noDataForAreaType();
    } else {

        setPageMode(PAGE_MODES.INDICATOR_DETAILS);

        // The user may have come from another page on which they have scrolled down
        var $window = $(window);
        if ($window.scrollTop() > 200) {
            $window.scrollTop(0);
        }

       

        // Set root index
        if (!isDefined(rootIndex)) {
            rootIndex = getIndicatorIndex();
        }

        initBarChart(groupRoots[rootIndex]);
        configureStateAffectedByNearestNeighbours();

        setIndicatorIndex(rootIndex);
        indicatorChanged(rootIndex);
        tooltipManager.setTooltipProvider(new ValueNoteTooltipProvider());
        showAndHidePageElements();
        showDataQualityLegend();
        showTargetBenchmarkOption(groupRoots);
        showAndHideAreaSwitch();
        


        if (FT.model.isNearestNeighbours()) {        
            barChartState.areaSwitch.clearHtml();
            if (isNearestNeighboursAndSingleAreaView()) {
                barChartState.areaDisplayed = BAR_CHART_VIEW_MODES.REGION;
            }            
        }
        else {          
            addBarChartOptions();
        }
        
        unlock();
    }
};

function initBarChart(root) {
    var state = barChartState;
    if (!state.isInitialised) {

        addBarChartTemplate();

        state.areaSwitch = new AreaSwitch({
            eventHanders: [barAreasClicked, barAreasClicked],
            eventName: 'AreasToDisplaySelected'
        });

        state.nationalSortedAreas = loaded.areaLists;

        // Initialise containers, table and table header
        pages.getContainerJq().html(
            templates.render('indicators', {
                hasTrendMarkers: isDefined(root) && isDefined(root.TrendMarkers)
            }));

        state.isInitialised = true;
    }
}

function configureStateAffectedByNearestNeighbours() {
    var state = barChartState;
    var firstInNN = state.isFirstPassToNearestNeighbours;
    if (FT.model.isNearestNeighbours()) {
        if (firstInNN) {
            state.isFirstPassToNearestNeighbours = false;
            state.previousAreaDisplayed = state.areaDisplayed;
            state.areaDisplayed = BAR_CHART_VIEW_MODES.REGION;
        }
    } else {
        if (!firstInNN) {
            state.areaDisplayed = state.previousAreaDisplayed;
            state.isFirstPassToNearestNeighbours = true;
        }
    }
}

function barAreasClicked() {   
    var option = (-barChartState.areaDisplayed) + 1;
    barChartState.areaDisplayed = option;
    displayIndicatorDetails();
}

function addBarChartTemplate() {
    templates.add('indicators',
        '<div id="indicatorDetailsNoData">No Data</div>' + 
        '<div id="indicatorDetailsData" style="display: none;">' +
        showExportTableLink('indicators-container', 'CompareAreasTable') +
        '<div style="float:none; clear: both; width: 1000px;"><div id="indicatorDetailsHeader"></div><div id="indicatorDetailsBox" style="width:100%;">' +
        '<table id="indicatorDetailsTable" class="borderedTable" cellspacing="0">' +
        '<thead><tr><th style="width: 200px;">Area<a class="columnSort" href="javascript:sortIndicatorDetailsByArea();" title="Sort alphabetically by area"></a></th>' +
        '{{#hasTrendMarkers}} <th>Trend</th> {{/hasTrendMarkers}}' +
        '<th class="nearest-neighbours-show" style="border-right:none"><div class="center">Rank</div><a class="columnSort" href="javascript:sortIndicatorDetailsByRank();" title="Sort by rank"></a></th>' +
        '<th style="border-right: none;"><div class="center">Count</div><a class="columnSort" href="javascript:sortIndicatorDetailsByCount();" title="Sort by count"></a></th>' +
        '<th style="border-right: none;"><div class="center">Value</div><a class="columnSort" href="javascript:sortIndicatorDetailsByValue();" title="Sort by values"></a></th>' +
        '<th class="bar">&nbsp;</th><th title="Lower confidence interval"><span class="sig-level" style="width:100%;display:block;"></span>Lower CI</th>' +
        '<th title="Upper confidence interval"><span class="sig-level" style="width:100%;display:block;"></span>Upper CI</th></tr></thead>' +
        '<tbody></tbody></table><br>' +
        '<div id="indicatorDetailsSource"></div></div></div></div>');
}

function addBarChartOptions() {
    barChartState.areaSwitch.setOption(barChartState.areaDisplayed);
    barChartState.areaSwitch.setHtml({
        label: 'Areas in',
        topOptionText: getParentArea().Name,
        bottomOptionText: 'England'
    });
}

function getBarChartData(root) {
    if (barChartState.areaDisplayed === BAR_CHART_VIEW_MODES.REGION) {
        return root.Data;
    }

    var areaCodeToDataHash = loaded.areaValues[getIndicatorKey(root, FT.model, NATIONAL_CODE)];
    return _.map(areaCodeToDataHash, function (coreDataSet) {
        return coreDataSet;
    });
}

function displayIndicatorDetails() {

    if (barChartState.areaDisplayed === BAR_CHART_VIEW_MODES.ENGLAND) {
        ajaxMonitor.setCalls(1);
        getAreaValues(groupRoots[getIndicatorIndex()], FT.model, NATIONAL_CODE);
        ajaxMonitor.monitor(displayIndicatorDetailsHtml);
    } else {
        displayIndicatorDetailsHtml();
    }
}

function displayIndicatorDetailsHtml() {

    var rootIndex = getIndicatorIndex(),
        root = groupRoots[rootIndex],
        state = barChartState;

    state.metadata = ui.getMetadataHash()[root.IID];

    var regionalAreaCode = FT.model.parentCode,
        key = getGroupAndCurrentAreaTypeKey(),
        comparisonConfig = new ComparisonConfig(root, state.metadata),
        hasStateChanged = state.hasStateChanged(rootIndex,
            key, regionalAreaCode, comparisonConfig.comparatorId, state.areaDisplayed),
        data = getBarChartData(root);
    state.comparisonConfig = comparisonConfig;

    var $quintileKey = $('.quintileKey');
    if (state.comparisonConfig.useQuintileColouring) {
        $quintileKey.show();
    } else {
        $quintileKey.hide();
    }

    var isData = data.length > 0;
    if (isData) {

        var nationalGrouping = getNationalComparatorGrouping(root),
        regionalGrouping = getRegionalComparatorGrouping(root);

        if (hasStateChanged || state.hasOrderChanged()) {

            var areas = state.areaDisplayed === 0
                ? sortedAreas
                : state.nationalSortedAreas[FT.model.areaTypeId];

            state.currentData = getDataSortedByArea(data, areas);
            displayIndicatorTable(root, regionalGrouping, nationalGrouping, comparisonConfig, areas);
        }

        if (hasStateChanged) {
            setIndicatorTableHeaderHtml(root, rootIndex);
        }
    }

    if (hasStateChanged) {
        toggleDataOrNot('indicatorDetails', isData);

        // Set page state
        state.setState(rootIndex, key, regionalAreaCode, comparisonConfig.comparatorId, state.areaDisplayed);

        // Ensures sort behaviour consistent for new indicator
        state.resetOrder();

        setTargetLegendHtml(comparisonConfig, state.metadata);
    }

    state.setOrderKey();

    displayBarChartLegend();

    loadValueNoteToolTips();

    unlock();
};

function displayBarChartLegend() {
    var $keyAdHoc = $('#keyAdHoc');
    var $keyBarChart = $('#key-bar-chart');

    if (barChartState.comparisonConfig.useTarget) {
        $keyAdHoc.show();
        $keyBarChart.hide();
    } else {
        $keyAdHoc.hide();
        $keyBarChart.show();
    }
}

function displayIndicatorTable(root, regionalGrouping, nationalGrouping,
    comparisonConfig, areas) {

    var state = barChartState,
        data = state.currentData,
        html = [];

    state.row = 0;

    // Determine size of bars
    var allData = data.concat([regionalGrouping.ComparatorData,
        nationalGrouping.ComparatorData]);
    state.barScale = new BarScale(allData);

    // Add comparator rows
    if (enumParentDisplay !== PARENT_DISPLAY.REGIONAL_ONLY) {
        addIndicatorTableComparatorRow(html, nationalGrouping,
            NATIONAL_COMPARATOR_ID, comparisonConfig, root);
    }

    // Do not show regional value if Nearest Neighbour
    if (isSubnationalColumn() && state.areaDisplayed === 0) {
        addIndicatorTableComparatorRow(html, regionalGrouping,
            REGIONAL_COMPARATOR_ID, comparisonConfig, root);
    }

    for (var i in areas) {
        var area = areas[i];

        addIndicatorTableAreaRow(html, data[i], area.Code,
            trimName(getAreaNameToDisplay(area), 25)/*area name label*/, i, null, comparisonConfig, root);
    }

    $('#indicatorDetailsTable tbody').html(html.join(''));

    var metadata = state.metadata;

    // Update significance labels
    var sig = metadata.ConfidenceLevel,
    sigLabel = sig > -1 ? sig + '%' : '&nbsp;';
    $('.sig-level').html(sigLabel);

    // Source
    $('#indicatorDetailsSource').html(
        getSource(state.metadata)
    );
};

/**
* Event handler for when an area is selected.
* @class barAreaSelected
*/
function barAreaSelected(areaCode) {
    if (!FT.model.isNearestNeighbours()) {
        if (!ajaxLock) {
            lock();

            FT.menus.parent.setCode(new AreaMappings(FT.model).getParentAreaCode(areaCode));
            FT.menus.area.setCode(areaCode);

            setAreas();
            ftHistory.setHistory();
            goToAreaProfilePage();

            logEvent('BarChart', 'AreaSelectedInTable', areaCode);
        }
    }
}

function addIndicatorTableAreaRow(html, data, areaCode, areaName, dataIndex, compId, comparisonConfig, root) {
    html.push('<tr>');

    if (dataIndex !== null/*i.e. is not benchmark*/) {
        var click = ' onclick="barAreaSelected(\'' + areaCode + '\');" ';

        var behaviour = ' onmouseover="overBar(this,' + dataIndex + ');" onmouseout="outBar();"' + click;
        html.push('<td class="pLink"', click, behaviour, '>',
            areaName, '</td>');
        var extraBarClass = dataIndex == (barChartState.currentData.length - 1) ?
            'barLast ' : '';

    } else {
        // Benchmark
        if (barChartState.metadata.ValueTypeId === ValueTypeIds.Count) {
            // Do not show bar for certain value types
            extraBarClass = 'hide ';
        } else {
            extraBarClass = barChartState.row == 0 ?
                'barFirst ' :
                '';
        }

        // Make non-selected comparator clickable
        if (compId !== comparatorId) {
            // Not benchmark
            extraBarClass += 'clickable ';
            behaviour = ' onclick="selectBenchmarkClicked(' + compId + ')" ';
            var extraLabelClass = '';
        } else {
            // Current benchmark
            extraLabelClass = 'bold ';
        }

        addTd(html, areaName, extraLabelClass);
    }

    var dataInfo = new CoreDataSetInfo(data);


    // Trend if applicable
    if(isDefined(root) && isDefined(root.TrendMarkers))
    {
        if (root.TrendMarkers[areaCode]) {
            addTd(html, GetTrendMarkerImage(root.TrendMarkers[areaCode], root.PolarityId), CSS_CENTER);
        } else {
            addTd(html, GetTrendMarkerImage(TrendMarkerValue.CannotCalculate, root.PolarityId), CSS_CENTER);
        }
    }

    // Data values
    if (dataInfo.isDefined()) {
        var valueDisplayer = new ValueDisplayer(),
        count = formatCount(dataInfo),
        val = valueDisplayer.byDataInfo(dataInfo),
        lCi = valueDisplayer.byNumberString(data.LoCIF),
        uCi = valueDisplayer.byNumberString(data.UpCIF),
        isValue = dataInfo.isValue();
    } else {
        val = count = lCi = uCi = NO_DATA;
        isValue = false;
    }

    // Rank
    if (FT.model.isNearestNeighbours() && barChartState.areaDisplayed === BAR_CHART_VIEW_MODES.REGION) {
        var rankValue;
        switch (areaCode) {
            case NATIONAL_CODE:
            case FT.model.areaCode:
                rankValue = '-';
                break;
            default:
                rankValue = _.findWhere(sortedAreas, { Code: areaCode }).Rank;
                break;
        }

        addTd(html, rankValue, CSS_CENTER);
    }

    addTd(html, count, CSS_NUMERIC);
    addTd(html, val, CSS_NUMERIC, null, dataInfo.getNoteId());

    // Bar
    var bar = isValue ?
        getBarHtml(data, comparisonConfig) :
        EMPTY_TD_CONTENTS;

    html.push('<td class="', extraBarClass, 'bar"', behaviour, '>', bar, '</td>');

    addTd(html, lCi, CSS_NUMERIC);
    addTd(html, uCi, CSS_NUMERIC);

    html.push('</tr>');

    barChartState.row++;
};

function addIndicatorTableComparatorRow(html, grouping, comparatorId, comparisonConfig, root) {
    if (isDefined(grouping)) {
        var area = getComparatorById(comparatorId);
        addIndicatorTableAreaRow(html, grouping.ComparatorData, area.Code,
            getAreaNameToDisplay(area), null/*data index*/, comparatorId, comparisonConfig, root);
    }
};

function getDataSortedByArea(data, areas) {

    var a = [];
    for (var i in areas) {
        var d = getDataFromAreaCode(data, areas[i].Code);
        a.push(d);
    }
    return a;
};

function colourRow(dataIndex) {

    var ignoredRowCount = enumParentDisplay === PARENT_DISPLAY.NATIONAL_AND_REGIONAL ? 2 : 1;

    $('#indicatorDetailsTable tbody tr:eq(' + (dataIndex + ignoredRowCount) + ') td').each(function () {
        highlightRow(this);
    });
};

function setProportionMax(options, metadata, root, max) {

    if (metadata.Unit.Id == 5 && isDefined(root.IndicatorStats) &&
        root.IndicatorStats.Max > max) {
        options.yAxis.max = 100.3; // Will not be on tick
    }
};

function sortIndicatorDetailsByRank() {

    var state = barChartState;
    state.valueOrder = 0;

    var rootIndex = getIndicatorIndex();
    var newOrder = invertSortOrder(state.areaOrder);
    state.areaOrder = newOrder;

    sortedAreas = sortSortedAreasByRank();

    displayIndicatorDetails(rootIndex);
    logEvent('BarChart', 'DataSortedByArea');
}

function sortIndicatorDetailsByCount() {

    var state = barChartState;
    state.areaOrder = 1;

    var rootIndex = getIndicatorIndex(),
        root = groupRoots[rootIndex],
        newOrder = invertSortOrder(state.countOrder);
    state.countOrder = newOrder;

    if (state.areaDisplayed === 0) {
        sortedAreas = new AreaAndDataSorter(newOrder, root.Data, sortedAreas, areaHash).byCount();
    }
    else {
        var areaTypeId = FT.model.areaTypeId;
        var areas = state.nationalSortedAreas[areaTypeId];
        state.nationalSortedAreas[areaTypeId] = new AreaAndDataSorter(newOrder, getBarChartData(root), areas, getAreaHash(areas)).byCount();
    }

    displayIndicatorDetails(rootIndex);
    logEvent('BarChart', 'DataSortedByCount');
};

function sortIndicatorDetailsByValue() {

    var state = barChartState;
    state.areaOrder = 2;

    var rootIndex = getIndicatorIndex(),
        root = groupRoots[rootIndex],
        newOrder = invertSortOrder(state.valueOrder);
    state.valueOrder = newOrder;

    if (state.areaDisplayed === 0) {
        sortedAreas = new AreaAndDataSorter(newOrder, groupRoots[rootIndex].Data,
            sortedAreas, areaHash).byValue();
    } else {
        var areaTypeId = FT.model.areaTypeId;
        var areas = state.nationalSortedAreas[areaTypeId];
        state.nationalSortedAreas[areaTypeId] = new AreaAndDataSorter(newOrder,
            getBarChartData(root), areas, getAreaHash(areas)).byValue();
    }

    displayIndicatorDetails(rootIndex);
    logEvent('BarChart', 'DataSortedByValue');
};

function sortIndicatorDetailsByArea() {

    var state = barChartState;
    state.valueOrder = 0;

    var rootIndex = getIndicatorIndex();
    var newOrder = invertSortOrder(state.areaOrder);
    state.areaOrder = newOrder;

    var areasToSort = state.areaDisplayed === 0
        ? sortedAreas
        : state.nationalSortedAreas[FT.model.areaTypeId];

    sortAreasAToZ(newOrder, areasToSort);

    displayIndicatorDetails(rootIndex);
    logEvent('BarChart', 'DataSortedByArea');
};

function setIndicatorTableHeaderHtml(root, rootIndex) {
    var grouping = getFirstGrouping(root);

    var period = isDefined(grouping)
        ? grouping.Period
        : '';

    var html = getTrendHeader(barChartState.metadata, root, period,
        'goToMetadataPage(' + rootIndex + ')');
    $('#indicatorDetailsHeader').html(html);
};

barChartState = {

    // State for reference
    dataStateKey: '',
    orderKey: null,

    // State used in displaying
    valueOrder: 0, // value and area sort could be moved to ui sort state if sort order global
    areaOrder: 0,
    countOrder: 0,
    currentData: null,
    selectedPoint: null, // in funnel plot
    barScale: null,
    row: 0,
    comparisonConfig: null,
    areaDisplayed: 0,
    previousAreaDisplayed: 0,
    nationalSortedAreas: null,
    areaSwitch: null,
    isFirstPassToNearestNeighbours:true,

    resetOrder: function () {
        this.countOrder = 0;
        this.valueOrder = 0;
        this.areaOrder = 0;
        this.orderKey = null;
    },

    hasOrderChanged: function () {
        return this.getOrderKey() !== this.orderKey;
    },

    getOrderKey: function () {
        return this.valueOrder.toString() + this.areaOrder.toString() + this.countOrder.toString();
    },

    setOrderKey: function () {
        this.orderKey = this.getOrderKey();
    },

    setState: function (rootIndex, sid, regionalAreaCode, comparatorId, areaDisplayed) {
        this.dataStateKey = getKey(rootIndex, sid, regionalAreaCode, comparatorId, areaDisplayed);
    },

    hasStateChanged: function (rootIndex, sid, regionalAreaCode, comparatorId, areaDisplayed) {
        return getKey(rootIndex, sid, regionalAreaCode, comparatorId, areaDisplayed) !==
            this.dataStateKey;
    }
};

function getBarHtml(data, comparisonConfig) {

    // No bar or CIs if everything is zero
    var info = new CoreDataSetInfo(data);
    if (info.areValueAndCIsZero()) {
        return '';
    }

    var barScale = barChartState.barScale,
    imgUrl = FT.url.img;

    var img = getSignificanceImg(data.Sig[comparisonConfig.comparatorId],
        comparisonConfig.useRagColours, comparisonConfig.useQuintileColouring);
    if (!img) { img = 'lightGrey.png'; }

    var width = parseInt(barScale.pixelsPerUnit * data.Val, 10);
    width = Math.abs(width);

    var left = data.Val < 0 ?
        barScale.negativePixels - width :
        barScale.negativePixels;

    var h = ['<div class="barBox"><img class="bar" style="width:', width,
        'px;left:', left,
        'px" src="', imgUrl, img, '"/>'];

    if (data.UpCIF) {

        // Add the error bars
        var lower = getErrorBarPixelStart(data.LoCI, barScale),
        upper = getErrorBarPixelStart(data.UpCI, barScale),
        pixelSpan = upper - lower - 2;

        var start = '<img class="error',
        end = 'px;"/>',
        src = ' src="' + imgUrl + 'black.png" style="left:';

        h.push(start, '"', src, lower, end);
        if (pixelSpan > 0) {
            h.push(start, 'Mid"', src, (lower + 1), 'px;width:', pixelSpan, end);
        }
        h.push(start, '"', src, (upper - 1), end);
    }
    h.push('</div>');
    return h.join('');
};

function getErrorBarPixelStart(cI, barScale) {

    // Value should fall within 1 pixel width of error bar
    var pixels = cI * barScale.pixelsPerUnit,
    pixelsInt = Math.round(pixels);

    if (pixels < pixelsInt) {
        pixelsInt -= 1;
    }

    return pixelsInt + barScale.negativePixels;
};

function overBar(td) {
    highlightRow(td);
};

function outBar() {
    unhighlightRow();
    highlightedRowCells.filter(':eq(3)').css({
        'border-top-color': '#fff',
        'border-bottom-color': '#fff'
    });
    highlightedRowCells = null;
};

function selectBenchmarkClicked(comparatorId) {
    if (!ajaxLock) {
        FT.menus.benchmark.setComparatorId(comparatorId);
        benchmarkChanged();
        logEvent('BarChart', 'ComparatorChangedByClickOnBar');
    }
}

function isNearestNeighboursAndSingleAreaView() {
    return FT.model.isNearestNeighbours() && barChartState.areaDisplayed === BAR_CHART_VIEW_MODES.REGION;
}

function showAndHideAreaSwitch() {
    if (FT.model.parentCode === NATIONAL_CODE) {
        $('#areaSwitch').hide();
    } else {
        $('#areaSwitch').show();
    }
}

var BAR_CHART_VIEW_MODES = {
    REGION: 0,
    ENGLAND: 1
};

highlightedRowCells = null;

pages.add(PAGE_MODES.INDICATOR_DETAILS, {
    id: 'indicators',
    title: 'Compare areas',
    goto: goToIndicatorDetailsPage,
    gotoName: 'goToIndicatorDetailsPage',
    needsContainer: true,
    showHide: displayBarChartLegend,
    jqIds: ['indicatorMenuDiv', '.geo-menu', 'areaSwitch', 'value-note-legend', 'nearest-neighbour-link','trend-marker-legend-barchart'],
    jqIdsNotInitiallyShown: ['data-quality-key', 'target-benchmark-box', 'keyAdHoc', 'key-bar-chart', 'key-spine-chart' ]
});

