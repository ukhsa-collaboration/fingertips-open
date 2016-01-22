function addIndicatorRow(groupRoot, rowNumber, coreDataSet,
    rootIndex, stats, formatter, regionalData, nationalData, benchmarkData,
    period, metadata, haveRequiredValues) {

    var html = ['<tr>'],
        comparisonConfig = new ComparisonConfig(groupRoot, metadata);

    // Indicator name
    html.push('<td id="spine-indicator_', rootIndex, '" class="pLink" onclick="indicatorNameClicked(\'', rootIndex,
        '\');" onmouseover="highlightRow(this);" onmouseout="unhighlightRow();">',
        formatter.getIndicatorName(), getIndicatorDataQualityHtml(formatter.getDataQuality()),
        '<br>', getTargetLegendHtml(comparisonConfig, metadata),
        '</td>');

    addTd(html, period, CSS_CENTER);

    // Trend
    if (groupRoot.TrendMarkers) {
        if (coreDataSet) {
            addTd(html, GetTrendMarkerImage(groupRoot.TrendMarkers[coreDataSet.AreaCode], groupRoot.PolarityId), CSS_CENTER);
        } else {
            addTd(html, GetTrendMarkerImage(TrendMarkerValue.CannotCalculate, 0), CSS_CENTER); 
        }
    }
   

    // Count
    var count = formatter.getAreaCount();
    addTd(html, count, CSS_NUMERIC);

    // Area value
    var dataInfo = new CoreDataSetInfo(coreDataSet);
    addTd(html, formatter.getAreaValue(), CSS_NUMERIC, null, dataInfo.getNoteId());

    if (!FT.model.isNearestNeighbours()) {
        // Regional average
        if (isSubnationalColumn()) {
            addTd(html, formatter.getVal(regionalData, 'ValF'), CSS_NUMERIC);
        }
    }

    // England average
    if (enumParentDisplay != PARENT_DISPLAY.REGIONAL_ONLY) {
        addTd(html, formatter.getVal(nationalData, 'ValF'), CSS_NUMERIC);
    }

    // Min, spine chart, max
    var spine = getSpineChartHtml(rowNumber, groupRoot, coreDataSet, stats, benchmarkData, metadata, comparisonConfig, haveRequiredValues);

    if (spine === NO_SPINE_DATA || spine === INSUFFICIENT_DATA) {
        var min = NO_DATA,
            max = NO_DATA;
    } else {
        min = formatter.getMin();
        max = formatter.getMax();
    }
    addTd(html, min, CSS_NUMERIC);

    if (areaProfileState.isAreaIgnored) {
        if (rowNumber === 1) {
            html.push(areaProfileState.ignoreMessage);
        } else {
            addTd(html, '<div style="height:23px;">' + NO_DATA + '</div>');
        }
    } else {
        addTd(html, spine, 'spine250');
    }

    addTd(html, max, CSS_NUMERIC);

    html.push('</tr>');

    $('#' + ID_SINGLE_AREA_TABLE + ' > tbody:last').append(html.join(''));

    // Tooltips
    tooltipManager.initElement('spine-indicator_' + rootIndex);
    addTableSpineChartTooltip(rowNumber, formatter);

    if (comparisonConfig.useQuintileColouring) {
        toggleQuintileLegend($('#quintile-key'), true);
    }
}

function goToAreaProfilePage(areaCode) {

    if (!groupRoots.length) {
        // Search results empty
        noDataForAreaType();
    } else {

        FT.menus.area.setCode(areaCode);

        setPageMode(PAGE_MODES.AREA_SPINE);

        ajaxMonitor.setCalls(3);

        getGroupingData();

        getIndicatorMetadata(FT.model.groupId);
        getIndicatorStats();

        ajaxMonitor.monitor(displayAreaProfile);
    }
};

function displayAreaProfile() {

    //NOTE: ajaxLock test cannot go here 

    var provider = new AreaTooltipProvider(stems),
    model = FT.model;
    tooltipManager.setTooltipProvider(provider);
    areaProfileState.tooltipProvider = provider;

    initAreaProfile();

    var areaCode = model.areaCode;
    setIsAreaIgnored(areaCode);
    setAreaProfileHtml(areaCode);

    showAndHidePageElements();

    showDataQualityLegend();

    showTargetBenchmarkOption(groupRoots);

    ui.setScrollTop();

    unlock();

    loadValueNoteToolTips();

};

function initAreaProfile() {
    // Populate container
    var isNationalAndRegional = enumParentDisplay == PARENT_DISPLAY.NATIONAL_AND_REGIONAL &&
       !isParentCountry();

    var trendMarkerFound = isDefined(groupRoots) && isDefined(groupRoots[0]) && isDefined(groupRoots[0].TrendMarkers);
    
    pages.getContainerJq().html(
        templates.render('areaProfile', {
            colSpan: isNationalAndRegional && !FT.model.isNearestNeighbours() ? 3 : 4,
            isNationalAndRegional: isNationalAndRegional,
            lowest: spineHeaders.min,
            highest: spineHeaders.max,
            parentType: FT.menus.parentType.getName(),
            isNotNN: !FT.model.isNearestNeighbours(),
            trendColSpan: trendMarkerFound ? 3 : 2,
            trendHeaderIfEnabled: trendMarkerFound
        }));

    areaProfileState.isInitialised = true;
}

function getSpineChartHtml(rowNumber, groupRoot, coreDataSet, stats, benchmarkData, metadata,
    comparisonConfig, haveRequiredValues) {

    // Should show not enough values message
    if (isDefined(benchmarkData) && isValidValue(benchmarkData.Val) &&
        haveRequiredValues === false) {
        return INSUFFICIENT_DATA;
    }

    // Should show no data message
    if (!isDefined(stats) ||
        !isDefined(benchmarkData) ||
        !isValidValue(benchmarkData.Val) ||
        metadata.ValueTypeId === ValueTypeIds.Count) {
        return NO_SPINE_DATA;
    }

    var polarity = groupRoot.PolarityId;
    var proportions = getSpineProportions(benchmarkData.Val, stats, polarity);
    var spineDimensions = spineChart.getDimensions(proportions);

    var dataInfo = new CoreDataSetInfo(coreDataSet);
    if (dataInfo.isValue()) {

        var areaDimension = spineChart.getMarkerOffset(coreDataSet.Val, spineDimensions,
            proportions, polarity);

        var left = areaDimension + spineDimensions.q1Offset;
        var markerHtml = ['<img id="m_', rowNumber, '" src="', FT.url.img,
            getMarkerImageFromSignificance(coreDataSet.Sig[comparisonConfig.comparatorId],
                comparisonConfig.useRagColours, comparisonConfig.useQuintileColouring ? '_medium' : '', comparisonConfig.useQuintileColouring),
            '" class="marker" style="left:', left, 'px;" alt=""/>'];
    } else {
        markerHtml = [];
    }

    return spineChart.getHtml(spineDimensions, rowNumber, markerHtml.join(''));
}

function addTableSpineChartTooltip(rowNumber, formatter) {

    // IDs of elements can be known here so can associate those
    // with data in global look up
    areaProfileState.tooltipProvider.add(rowNumber, formatter);
    var stemKeys = stems.getKeys();
    for (var i in stemKeys) {
        tooltipManager.initElement(stemKeys[i] + '_' + rowNumber);
    }
};

function getIndicatorStats() {

    ui.storeScrollTop();

    // National spine chart
    var code = getComparatorById(comparatorId).Code;
    if (ui.areIndicatorStatsLoaded(code)) {
        ajaxMonitor.callCompleted();
    } else {

        showSpinner();

        var parameters = 's=is&gid=' + FT.model.groupId +
            '&par=' + code +
            '&ati=' + FT.model.areaTypeId +
            getProfileOrIndicatorsParameters() +
            getRestrictByProfileParameter();

        ajaxGet('GetData.ashx', parameters, getIndicatorStatsCallback);
    }
};

function setAreaProfileHtml(areaCode) {
    // Benchmark
    var isNationalBenchmark = (comparatorId == NATIONAL_COMPARATOR_ID),
        benchmark = isNationalBenchmark ?
            getNationalComparator() :
            getParentArea(),
        indicatorStats = ui.getIndicatorStats(benchmark.Code);
    var area = areaHash[areaCode];
    var metadataHash = ui.getMetadataHash();
    var row = 1;

    toggleQuintileLegend($('#quintile-key'), false);

    setIgnoreMessage(area, benchmark);

    // Update table rows
    clearSpineTables();
    for (var i in groupRoots) {

        var statsBase = indicatorStats[i];
        if (statsBase) {

            var statsF = statsBase.StatsF,
                stats = statsBase.Stats;

            var haveRequiredValues = statsBase.HaveRequiredValues;

        } else {
            // If no data then indicatorStats[i] will be null
            haveRequiredValues = false;
            stats = statsF = null;
        }


        var root = groupRoots[i],
            period = root.Grouping[0].Period,
            metadata = metadataHash[root.IID],
            coreDataSet = getDataFromAreaCode(root.Data, areaCode);

        var formatter = new IndicatorFormatter(root, metadata, coreDataSet, statsF),
         regionalData = getRegionalComparatorGrouping(root).ComparatorData,
         nationalData = getNationalComparatorGrouping(root).ComparatorData,
         benchmarkData = isNationalBenchmark ?
            nationalData :
             regionalData;

        formatter.averageData = benchmarkData;

        addIndicatorRow(root, row, coreDataSet, i, stats, formatter,
            regionalData, nationalData, benchmarkData, period, metadata, haveRequiredValues);

        row += 1;
    }

    setAreaHeadings(area, benchmark);
};

function getIndicatorStatsCallback(obj) {
    ui.setIndicatorStats(getComparatorById(comparatorId).Code, obj);
    ajaxMonitor.callCompleted();
};

areaProfileState = {
    isInitialised: false,
    isAreaIgnored: false,
    ignoreMessage: '',
    tooltipProvider: null
}

function setIgnoreMessage(area, benchmark) {
    if (areaProfileState.isAreaIgnored) {
        areaProfileState.ignoreMessage = '<td style="position:relative;display:block;height:23px;">' +
            NO_DATA + '<div id="noSpineBox"><p style="padding:5px;">Due to its small population <i>' +
            area.Name + '</i> is not used to determine the lowest, highest and percentile values required for the spine charts. However the area value is included in the <i>' +
            benchmark.Name + '</i> average.</p></div></td>';
    }
}

function setAreaHeadings(area, benchmark) {
    var headers = $('#' + ID_SINGLE_AREA_TABLE + ' THEAD TH');

    // Update area name
    $(headers[2]).html('<div>' + getShortAreaNameToDisplay(area) + '</div>&nbsp;');

    // Update benchmark name
    var i = (enumParentDisplay === PARENT_DISPLAY.NATIONAL_AND_REGIONAL &&
        !isParentCountry())
        ? 5
        : 3;
    // If nearest neighbours don't show region column, hence england column will -1 from normal 
    i = FT.model.isNearestNeighbours() ? i - 1 : i;

    $(headers[i]).html(benchmark.Name);
}

function setIsAreaIgnored(areaCode) {

    // Is area too small for spine charts
    var isIgnored = false;
    if (ignoredSpineChartAreas) {
        var a = ignoredSpineChartAreas.split(',');
        isIgnored = _.any(a, function (area) {
            return area === areaCode;
        });
    }
    areaProfileState.isAreaIgnored = isIgnored;
}

function AreaTooltipProvider(stems) {
    this.map = {};
    this.stems = stems;
}

AreaTooltipProvider.prototype = {

    add: function (key, formatter) {
        this.map[key.toString()] = formatter;
    },

    getHtml: function (id) {

        var bits = id.split('_'),
            firstBit = bits[0];

        // Value cell with a value note asterisk
        if (firstBit === 'ft') {
            return new ValueNoteTooltipProvider().getHtml(id);
        }

        // Indicator name, e.g. 'spine-indicator_1'
        if (firstBit === 'spine-indicator') {
            return getIndicatorNameTooltip(bits[1]/*root index*/, areaHash[FT.model.areaCode]);
        }

        return id !== '' ?
            this.getSpineChartText(id) :
            '';
    },

    getSpineChartText: function (id) {

        var bits = id.split('_');

        // e.g. bits = 'q1_3'
        var stem = bits[0],
        rowNumber = bits[1];

        try {
            var formatter = this.map[rowNumber];
        } catch (e) { }

        var html = [];
        if (isDefined(formatter)) {

            // Value or range
            html.push('<span id="tooltipData">',
                this.stems.getStemText(stem, formatter),
                formatter.getSuffixIfNoShort(),
                '</span>', this.stems.getStemQualifier(stem),
                '<span id="tooltipArea">');

            // Area name
            if (stem == 'm') {
                var areaName = getAreaNameToDisplay(formatter.getArea());
                html.push(areaName);
            }
            else {
                html.push(getCurrentComparator().Name);
            }

            // Indicator name
            html.push('</span><span id="tooltipIndicator">',
                formatter.getIndicatorName(), '</span>');
        }

        return html.join('');
    }
}


ID_SINGLE_AREA_TABLE = 'singleAreaTable';
NO_SPINE_DATA = '<div class="noSpine">-</div>';
INSUFFICIENT_DATA = '<div class="noSpine">Insufficient number of values for a spine chart</div>';


templates.add('areaProfile',
    showExportTableLink('areas-container', 'AreaProfilesTable', '#key-spine-chart,#spine-range-key') +
    '<table id="' + ID_SINGLE_AREA_TABLE + '" class="borderedTable" style="table-layout: auto;" cellspacing="0" cellpadding="0"><thead>\
<tr><th id="spineIndicatorHeader" rowspan="2">Indicator</th><th id="spinePeriodHeader" rowspan="2">Period</th><th style="position: relative;" class="numericHeader areaName topRow" colspan="{{trendColSpan}}">-</th>\
{{#isNationalAndRegional}}{{#isNotNN}}<th class="numericHeader topRow parent-area-type">{{parentType}}</th>{{/isNotNN}}<th class="numericHeader topRow">England</th>\
{{/isNationalAndRegional}}<th colspan="{{colSpan}}" class="numericHeader topRow" style="width: 390px;">-</th></tr><tr>\
{{#trendHeaderIfEnabled}} <th class="numericHeader">Trend</th> {{/trendHeaderIfEnabled}}\
<th class="numericHeader">Count</th><th class="numericHeader">Value</th>\
{{#isNationalAndRegional}}{{#isNotNN}}<th class="numericHeader">Value</th>{{/isNotNN}}{{/isNationalAndRegional}}\
<th class="numericHeader">Value</th><th class="numericHeader">{{lowest}}</th>\
<th class="spine250">Range</th><th class="numericHeader">{{highest}}</th></tr></thead>\
<tbody id="spineBody"><tr><td></td></tr></tbody></table>');

pages.add(PAGE_MODES.AREA_SPINE, {
    id: 'areas',
    title: 'Area<br>profiles',
    goto: goToAreaProfilePage,
    gotoName: 'goToAreaProfilePage',
    needsContainer: true,
    jqIds: ['spine-range-key', 'key-spine-chart', 'spineKey', '.geo-menu',
        'spineRangeKeyContainer', 'value-note-legend', 'nearest-neighbour-link', 'trend-marker-legend-spine'],
    jqIdsNotInitiallyShown: ['data-quality-key', 'target-benchmark-box']
});

