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

        if (!barChartState.isInitialised) {
            pages.getContainerJq().html(templates.render('indicators', {}));
            barChartState.isInitialised = true;
        }

        // Set root index
        if (!isDefined(rootIndex)) {
            rootIndex = getIndicatorIndex();
        }

        setIndicatorIndex(rootIndex);

        indicatorChanged(rootIndex);

        tooltipManager.setTooltipProvider(new ValueNoteTooltipProvider());

        showAndHidePageElements();

        showDataQualityLegend();

        showTargetBenchmarkOption(groupRoots);

        unlock();

        loadValueNoteToolTips();
    }
};

function displayIndicatorDetails() {

    var rootIndex = getIndicatorIndex(),
        root = groupRoots[rootIndex],
        state = barChartState;

    state.metadata = ui.getMetadataHash()[root.IID];

    var regionalAreaCode = FT.model.parentCode,
    key = getGroupAndCurrentAreaTypeKey(),
    comparisonConfig = new ComparisonConfig(root, state.metadata),
    hasStateChanged = state.hasStateChanged(rootIndex,
        key, regionalAreaCode, comparisonConfig.comparatorId);

    state.comparisonConfig = comparisonConfig;

    var isData = root.Data.length > 0;
    if (isData) {
        
        var comparatorGrouping = getComparatorGrouping(root),
        nationalGrouping = getNationalComparatorGrouping(root),
        regionalGrouping = getRegionalComparatorGrouping(root);
        
        if (hasStateChanged || state.hasOrderChanged()) {
            
            state.currentData = getDataSortedByArea(root.Data);
            
            addPopulationToData(state.currentData, comparatorGrouping);
            
            displayIndicatorTable(root, regionalGrouping, nationalGrouping, comparisonConfig);
        }
        
        if (hasStateChanged) {
            setIndicatorTableHeaderHtml(root, rootIndex);
            displayFunnelPlot(root, comparatorGrouping, comparisonConfig);
        }  
    } 
    
    if (hasStateChanged) {
        toggleDataOrNot('indicatorDetails', isData);
        
        // Set page state
        state.setState(rootIndex, key,
            regionalAreaCode, comparisonConfig.comparatorId);

        // Ensures sort behaviour consistent for new indicator
        state.resetOrder();

        setTargetLegendHtml(state.comparisonConfig, state.metadata);
    }

    state.setOrderKey();

    displayBarChartLegend();

    toggleQuintileLegend($('.quintileKey'), comparisonConfig.useQuintileColouring);

    loadValueNoteToolTips();

    unlock();
};

function displayBarChartLegend() {
    showBarChartLegend(barChartState.comparisonConfig.useTarget);
}

function displayIndicatorTable(root, regionalGrouping, nationalGrouping,
    comparisonConfig) {

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
            NATIONAL_COMPARATOR_ID, comparisonConfig);
    }
    if (isSubnationalColumn()) {
        addIndicatorTableComparatorRow(html, regionalGrouping,
            REGIONAL_COMPARATOR_ID, comparisonConfig);
    }

    for (var i in data) {
        var area = sortedAreas[i];
        
        addIndicatorTableAreaRow(html, data[i], area.Code, 
            trimName(getAreaNameToDisplay(area), 23)/*area name label*/, i, null, comparisonConfig);
    }
    
    $('#indicatorDetailsTable tbody').html(html.join(''));
    
    // Source
    $('#indicatorDetailsSource').html(
        getSource(state.metadata)
    );
};

function addIndicatorTableAreaRow(html, data, areaCode, areaName, dataIndex, compId, comparisonConfig) {

    html.push('<tr>');
    
    if (dataIndex !== null/*i.e. is not benchmark*/) {
        var click = !FT.model.isNearestNeighbours() ? ' onclick="goToAreaProfilePage(\'' + areaCode + '\');" ' : '';
        var behaviour = ' onmouseover="overBar(this,' + dataIndex + ');" onmouseout="outBar();"' + click;
        html.push('<td class="pLink"', click, behaviour, '>',
            areaName, '</td>');
        var extraBarClass = dataIndex == (barChartState.currentData.length -1) ?
            'barLast ' : '';
        
    } else {
        // Benchmark
        extraBarClass = barChartState.row == 0 ? 
            'barFirst ' : 
            '';

        if (barChartState.metadata.ValueTypeId === ValueTypeIds.Count) {
            extraBarClass = 'hide ';
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
    
    // Data values
    if (dataInfo.isDefined()) { 
        var valueDisplayer = new ValueDisplayer(),
        val = valueDisplayer.byDataInfo(dataInfo),
        lCi = valueDisplayer.byNumberString(data.LoCIF),
        uCi = valueDisplayer.byNumberString(data.UpCIF),
        isValue = dataInfo.isValue();
    } else {
        val = lCi = uCi = NO_DATA;
        isValue = false;
    }
    
    addTd(html, val, CSS_NUMERIC, null, dataInfo.getNoteId());
    
    // Bar
    var bar = isValue ?
        getBarHtml(data, comparisonConfig) :
        EMPTY_TD_CONTENTS;
    
    html.push('<td class="',extraBarClass,'bar"', behaviour, '>', bar, '</td>');
    
    addTd(html, lCi, CSS_NUMERIC);
    addTd(html, uCi, CSS_NUMERIC);
    
    html.push('</tr>');
    
    barChartState.row++;
};

function addIndicatorTableComparatorRow(html, grouping, comparatorId, comparisonConfig) {
    if (isDefined(grouping)) {
        var area = getComparatorById(comparatorId);

        addIndicatorTableAreaRow(html, grouping.ComparatorData, area.Code,
            getAreaNameToDisplay(area), null, comparatorId, comparisonConfig);
    }
};

function getDataSortedByArea(data) {
    var a = [];
    for (var i in sortedAreas) {
        var d = getDataFromAreaCode(data, sortedAreas[i].Code);
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

function displayFunnelPlot(root, comparatorGrouping, comparisonConfig) {

    barChartState.comparatorValue = null;
    
    var data = barChartState.currentData,
    plot = $('#funnelPlot'),
    noData = $('#funnelPlotNoData'),
    valueTypeId = barChartState.metadata.ValueTypeId,
    methodId = comparatorGrouping.MethodId;
    
    var isPopulation = false;
    for (var i in data) {
        if (data[i] !== null && data[i].Population !== null) {
            isPopulation = true;
            break;
        }   
    }
    
    if (!isPopulation ||
        !canDrawFunnelPlotForComparatorMethod(methodId) ||
        !canDrawFunnelPlotForValueType(valueTypeId) ||
        data.length == 0) {
        // No funnel plot
        plot.hide();
        noData.show();
        chart = null;
    } else {
        noData.hide();
        plot.show();
        var options = getFunnelPlotOptions(root, data, comparatorGrouping, comparisonConfig);
        setProportionMax(options, barChartState.metadata, root, 95);
        chart = new Highcharts.Chart(options);
    }
};

function canDrawFunnelPlotForComparatorMethod(methodId) {
    return methodId === 5 || methodId === 6;
}

function canDrawFunnelPlotForValueType(valueTypeId) {
    return valueTypeId !== 6 /*Ratio*/ &&
        valueTypeId !== 4 /*Indirectly standardised rates*/ &&
        valueTypeId !== 11 /*Life expectancy*/;
}

function setProportionMax(options, metadata, root, max) {

    if (metadata.Unit.Id == 5 && isDefined(root.IndicatorStats) &&
        root.IndicatorStats.Max > max) {
        options.yAxis.max = 100.3; // Will not be on tick
    }
};

function getFunnelPlotOptions(root, data, comparatorGrouping, comparisonConfig) {
    
    var comparatorData = comparatorGrouping.ComparatorData;
    var isComparatorValue = isDefined(comparatorData) && comparatorData.Val != -1;
    var isDsr = (comparatorGrouping.MethodId == 6);
    var showLimits = (comparatorGrouping.MethodId == 5 || isDsr) && isComparatorValue;
    
    var valueArrays = divideValuesBySignificance(data, comparisonConfig);
    
    if (comparisonConfig.useRagColours) {
        var worse = colours.worse;
        var better = colours.better;
    } else {
        worse = colours.bobLower;
        better = colours.bobHigher;
    }
    
    var options = {
        chart: {
            renderTo: 'funnelPlot',
            defaultSeriesType: 'spline',
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
                text: isDsr ? 'Effective population' : 'Population'
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
                text: barChartState.metadata.Unit.Label
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
                            lineColor: '#444444' /*around marker*/
                        },
                        select: {
                            lineColor: '#000000', /*around marker*/
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
                        resetBarCells(); 
                    }
                }
            },
            spline: {
                enableMouseTracking: false,
                lineWidth: 1,
                animation: false,
                marker: { enabled: false },
                events: {
                    mouseOut: function () { resetBarCells(); }
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
                
                resetBarCells();
                
                var data = barChartState.currentData;
                for (var i in data) {
                    var d = data[i];
                    if (d !== null && 
                            d.Population === this.x &&
                            d.Val === this.y) {
                        var areaCode = d.AreaCode,
                        areaName = areaHash[areaCode].Name,
                        unit = barChartState.metadata.Unit;
                        
                        colourRow(parseInt(i));
                        
                        return '<b>' + areaName + '</b><br/>Value: ' + 
                            new ValueWithUnit(unit).getFullLabel(d.ValF) + 
                            '<br/>' +
                            (isDefined(d.NoteId) ? VALUE_NOTE + loaded.valueNotes[d.NoteId].Text : '') + 
                            '<br/>Population: ' + new CommaNumber(this.x).rounded();
                    } 
                }
            }
        },
        credits: HC.credits,
        exporting: {
            enabled: false
        },
        series: [
            { color: colours.comparator, data: [], name: getCurrentComparator().Name,
                showInLegend: isComparatorValue
            },
            { color: colours.limit99, data: [], showInLegend: false },
            { color: colours.limit95, data: [], showInLegend: false },
            { color: colours.limit95, data: [], name: '95.0% Confidence', showInLegend: showLimits },
            { color: colours.limit99, data: [], name: '99.8% Confidence', showInLegend: showLimits },
            { color: better, data: valueArrays.better, showInLegend: false, type: 'scatter', name: 'better' },
            { color: colours.same, data: valueArrays.same, showInLegend: false, type: 'scatter', name: 'same' },
            { color: worse, data: valueArrays.worse, showInLegend: false, type: 'scatter', name: 'worse' },
            { color: colours.none, data: valueArrays.none, showInLegend: false, type: 'scatter' }
        ]
    };
    
    if (isComparatorValue) {    
        
        var key = 'PopulationMinMax';
        if (!isDefined(data[key])) {
            
            var values = new CoreDataSetList(data).getValidValues('Population');  
            data[key] = new MinMaxFinder(values);  
        }
        addFunnelPlotLimits(options, data[key], comparatorGrouping);
    }
    
    return options;
};

function sortIndicatorDetailsByValue() {

    barChartState.areaOrder = 1;

    var rootIndex = getIndicatorIndex();
    var newOrder = invertSortOrder(barChartState.valueOrder);
    barChartState.valueOrder = newOrder;
    sortedAreas = new AreaAndDataSorter(newOrder, groupRoots[rootIndex].Data, sortedAreas, areaHash).byValue();
    
    displayIndicatorDetails(rootIndex);
    logEvent('BarChart', 'DataSortedByValue');
};

function sortIndicatorDetailsByArea() {

    barChartState.valueOrder = 0;

    var rootIndex = getIndicatorIndex();
    var newOrder = invertSortOrder(barChartState.areaOrder);
    barChartState.areaOrder = newOrder;
    sortAreasAToZ(newOrder, sortedAreas);
    
    displayIndicatorDetails(rootIndex);
    logEvent('BarChart', 'DataSortedByArea');
};

function getSpcForProportionsLimits(comparatorValue, denominatorMin, denominatorMax) {
    var unitValue = barChartState.metadata.Unit.Value;
    var step = (denominatorMax - denominatorMin) / 25;

    var N1 = denominatorMin;
    var P0 = comparatorValue / unitValue;

    // Comparator Method 5
    var controlLimit2 = 1.96; // 95%
    var controlLimit3 = 3.0902; // 99.8%

    var points = [];
    var cl2Power2 = Math.pow(controlLimit2, 2);
    var cl3Power2 = Math.pow(controlLimit3, 2);
    while (N1 < (denominatorMax + step)) {
        var a = { x: N1 };
        points.push(a);

        var E1 = N1 * P0;
        var C2A = (2 * E1) + cl2Power2;
        var C3A = (2 * E1) + cl3Power2;
        var Bs1 = (1 - (E1 / N1));
        var Bs2 = 4 * E1 * Bs1;
        var C2Bs3 = cl2Power2 + Bs2;
        var C3Bs3 = cl3Power2 + Bs2;
        var C2B = controlLimit2 * Math.sqrt(C2Bs3);
        var C3B = controlLimit3 * Math.sqrt(C3Bs3);
        var C2C = 2 * (N1 + cl2Power2);
        var C3C = 2 * (N1 + cl3Power2);

        a.L2 = ((C2A - C2B) / C2C) * unitValue;
        a.U2 = ((C2A + C2B) / C2C) * unitValue;
        a.L3 = ((C3A - C3B) / C3C) * unitValue;
        a.U3 = ((C3A + C3B) / C3C) * unitValue;

        N1 += step;
    }

    return points;
}

function setIndicatorTableHeaderHtml(root, rootIndex) {
    var grouping = getFirstGrouping(root);
    var period = isDefined(grouping)
        ? grouping.Period
        : '';
    
    var html = getTrendHeader(barChartState.metadata, root, period,
        'goToMetadataPage(' + rootIndex + ')');
    $('#indicatorDetailsHeader').html(html);
};

var barChartState = {
    
    isInitialised : false,
    
    // State for reference
    dataStateKey : '',
    orderKey : null,
    
    // State used in displaying
    valueOrder: 0,// value and area sort could be moved to ui sort state if sort order global
    areaOrder: 0,
    currentData: null,
    metadata:null,
    selectedPoint: null, // in funnel plot
    barScale:null,
    row : 0,
    comparisonConfig: null,
    
    // Used for AJAX calls to ensure correct funnel plot is updated
    comparatorValue: null,
    
    /* Funnel plot only used for this page so data 
    *  encapsulated here rather than attached to loaded object */
    limits : {},
    
    getFunnelLimits : function(val, max) {
        var key = val + "-" + max;
        return isDefined(this.limits[key]) ? this.limits[key] : null;
    },
    
    setFunnelLimits : function(val, max, points) {
        this.limits[val + "-" + max] = points;
    },
    
    resetOrder : function() {
        this.valueOrder = 0;
        this.areaOrder = 0;
        this.orderKey = null;
    },
    
    hasOrderChanged : function() {
        return this.getOrderKey() !== this.orderKey;      
    },
    
    getOrderKey : function() {
        return this.valueOrder.toString() + this.areaOrder.toString();
    },
    
    setOrderKey : function() {
        this.orderKey = this.getOrderKey();
    },
    
    setState : function(rootIndex, sid, regionalAreaCode, comparatorId) {
        this.dataStateKey = getKey(rootIndex, sid, regionalAreaCode, comparatorId);
    },
    
    hasStateChanged : function(rootIndex, sid, regionalAreaCode, comparatorId) { 
        return getKey(rootIndex, sid, regionalAreaCode, comparatorId) !==
            this.dataStateKey;
    }
};

function getSpcForDsrLimits(comparatorValue, populationMin, populationMax, yearRange) {
    
    var parameters = new ParameterBuilder();
    parameters.add('comparator_value',comparatorValue);
    parameters.add('population_min',populationMin);
    parameters.add('population_max',populationMax);
    parameters.add('unit_value',barChartState.metadata.Unit.Value);
    parameters.add('year_range',yearRange);
    
    barChartState.comparatorValue = comparatorValue;
    ajaxGet('data/spc_for_dsr_limits', parameters.build(), getSpcForDsrLimitsCallback);
};

function getSpcForDsrLimitsCallback(obj) {
    
    if (mode === PAGE_MODES.INDICATOR_DETAILS && isDefined(obj)) {
        
        var rootIndex = getIndicatorIndex();
        var root = groupRoots[rootIndex];
        
        if (!isDefined(root)) {
            /*TODO figure out why this should ever be true (likely race 
            condition when indicator menu changed)*/
            return;
        }
        
        var val = barChartState.comparatorValue;
        if (val !== null && Math.abs(val - obj.ComparatorValue) < 0.00000001
            /*values are equal ignoring floating point approximation differences*/) {
            
            // Reset immediately to reduce chance of race condition
            barChartState.comparatorValue = null;
            
            points = obj.Points;
            var max = barChartState.currentData['PopulationMinMax'].max;
            barChartState.setFunnelLimits(val, max, points);
            
            var serieses = chart.series,
            u3 = serieses[1],
            u2 = serieses[2],
            l2 = serieses[3],
            l3 = serieses[4];
            
            for (var i in points) {
                var p = points[i],
                x = p.x;
                u3.addPoint([x, p.U3], false);
                u2.addPoint([x, p.U2], false);
                l2.addPoint([x, p.L2], false);
                l3.addPoint([x, p.L3], false);
            }
            
            // Add comparator line
            var comparator = chart.series[0];
            comparator.addPoint([points[0].x, val], false);
            comparator.addPoint([x/*x of last point*/, val], false);
            
            chart.redraw();
        }
    }
};

function divideValuesBySignificance(data, comparisonConfig) {
    
    var worse = [], same = [], better = [], none = [],
        comparatorId = comparisonConfig.comparatorId;
    
    for (var i in data) {
        if (data[i] !== null && data[i].Population !== null) {
            var significance = data[i].Sig[comparatorId];
            
            var point = [data[i].Population, data[i].Val];
            
            switch (significance) {
                
                case 3:
                    better.push(point);
                    break;
                    
                case 2:
                    same.push(point);
                    break;
                    
                case 1:
                    // Add twice because IE loses first point!
                    /*if (worse.length == 0) {
                    worse.push(values[i]);
                    }*/
                    worse.push(point);
                    break;
                    
                default:
                    none.push(point);
                    break;
            }
        }
    }
    
    return { worse: worse, same: same, better: better, none: none};
};

function addFunnelPlotLimits(options, minAndMax, comparatorGrouping) {
    
    var comparatorData = comparatorGrouping.ComparatorData;
    
    // Min/max population
    var max = minAndMax.max;
    var min = minAndMax.min;
    var value = comparatorData.Val;
    
    var series = options.series;
    var limits = null;
    if (comparatorGrouping.MethodId == 5) {
        
        limits = getSpcForProportionsLimits(value, min, max);
        
    } else if (comparatorGrouping.MethodId == 6) {
        
        limits = barChartState.getFunnelLimits(value, max);
        if (!limits) {
            getSpcForDsrLimits(value, min, max, comparatorGrouping.YearRange);
        }
    }
    else {
        throw 'Cannot draw funnel plot for current comparator method ID: ' + comparatorGrouping.MethodId;
    }
    
    if (limits) {
        setFunnelPlotLimitsSeriesData(limits, series, value, min, max);   
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

function overBar(td, dataIndex) {
    
    highlightRow(td);
    
    if (isDefined(chart)) {
        
        // {population:areacode}
        var data = barChartState.currentData[dataIndex];
        
        if (data) {
            var pop = data.Population;
            if (pop) {
                // Highlight point on funnel chart
                for (var i=5; i<=8; i++) {
                    var series = chart.series[i];
                    if (series) {
                        var points = series.data;
                        for (var j in points) {      
                            var point = points[j];
                            if (point.x === pop && point.y === data.Val) {
                                barChartState.selectedPoint = point;
                                point.select(true, false);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
};

function outBar() {
    
    resetBarCells();
    
    if (isDefined(chart)) {
        // Unselect currently selected point
        var point = barChartState.selectedPoint;
        if (point !== null) {
            point.select(false,false);
            barChartState.selectedPoint = null;
        }
    }
};

function addPopulationToData(data, comparatorGrouping) {
    
    // Check population not already added
    for (var i in data) {
        if (data[i]) {
            if (isDefined(data[i].Population)) {
                return;
            } else {
                break;   
            }
        }
    }
    
    // DSR - divide denominator by year range to get population
    var isDsr = (comparatorGrouping.MethodId == 6);
    
    var yearRange = comparatorGrouping.YearRange;
    
    for (i in data) {
        
        var d = data[i];    
        if (d) {
            
            if (d.Val !== -1) {
                
                // Use correct denominator 
                var population = isDsr ?
                    (d.Denom2 / yearRange) :
                    d.Denom;
                
                if (population > 0) {
                    d.Population = population; 
                    continue;
                }
            } 
            d.Population = null;  
        }
    }
};

function setFunnelPlotLimitsSeriesData(limits, series, value, min, max) {
    
    if (limits.length > 0) {
        var u3 = [];
        var u2 = [];
        var l3 = [];
        var l2 = [];
        
        for (var i in limits) {
            var x = limits[i].x;
            u3[i] = [x, limits[i].U3];
            u2[i] = [x, limits[i].U2];
            l2[i] = [x, limits[i].L2];
            l3[i] = [x, limits[i].L3];
        }
        series[1].data = u3;
        series[2].data = u2;
        series[3].data = l2;
        series[4].data = l3;
        
        var comparatorMax = x;
    } else {
        comparatorMax = max;
    }
    
    // Add comparator line
    series[0].data = [[min, value], [comparatorMax, value]];
}

function selectBenchmarkClicked(comparatorId) {
    if (!ajaxLock) {
        FT.menus.benchmark.setComparatorId(comparatorId);
        benchmarkChanged();
        logEvent('BarChart', 'ComparatorChangedByClickOnBar');
    }
}

function resetBarCells() {
    
    if (highlightedRowCells !== null) {
        
        unhighlightRow();
        
        highlightedRowCells.filter(':eq(2)').css({
                'border-top-color': '#fff',
                'border-bottom-color': '#fff'
        });
        highlightedRowCells = null;
    }
}

highlightedRowCells = null;

templates.add('indicators',
    '<div id="indicatorDetailsData" style="display: none;"><div style="float:none; clear: both; width:100%;"><div id="indicatorDetailsHeader"></div>\
    <div id="funnelPlotBox">' +
    showExportChartLink() +
    '<div id="funnelPlot" style="display: none;"></div><div id="funnelPlotNoData" style="" class="noData">Funnel plot is<br>not available</div></div>\
    <div id="indicatorDetailsBox">' +
    showExportTableLink('indicatorDetailsBox', 'CompareAreasTable') +
    '<table id="indicatorDetailsTable" class="borderedTable" cellspacing="0"><thead><tr><th style="width: 200px;">Area<a class="columnSort" href="javascript:sortIndicatorDetailsByArea();" title="Sort alphabetically by area"></a></th><th style="border-right: none;"><div class="center">Value</div><a class="columnSort" href="javascript:sortIndicatorDetailsByValue();" title="Sort by values"></a></th><th class="bar">&nbsp;</th><th title="Lower confidence interval">Lower<br>CI</th><th title="Upper confidence interval">Upper<br>CI</th></tr></thead><tbody></tbody></table><br><div id="indicatorDetailsSource"></div></div></div></div><div id="indicatorDetailsNoData">No Data</div>');

pages.add(PAGE_MODES.INDICATOR_DETAILS, {
    id: 'indicators',
    title: 'Compare areas',
    goto: goToIndicatorDetailsPage,
    gotoName: 'goToIndicatorDetailsPage',
    needsContainer: true,
    showHide: displayBarChartLegend,
    jqIds: ['indicatorMenuDiv', '.geo-menu', 'nearest-neighbour-link'],
    jqIdsNotInitiallyShown: ['data-quality-key', 'target-benchmark-box', 'keyAdHoc',
        'key-bar-chart', 'key-spine-chart', 'value-note-legend']
});