'use strict';

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
		tooltipManager.setTooltipProvider(new BarChartTooltipProvider());
		initBarChart(groupRoots[rootIndex]);
		configureStateAffectedByNearestNeighbours();

		setIndicatorIndex(rootIndex);
		indicatorChanged(rootIndex);
	  

		showAndHidePageElements();
		showDataQualityLegend();
		showTargetBenchmarkOption(groupRoots);
		showAndHideTabSpecificOptions();

		if (FT.model.isNearestNeighbours()) {
			barChartState.tabSpecificOptions.clearHtml();
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

function getRecentTrends(root) {

	var model = FT.model;

	var parameters = new ParameterBuilder(
	).add('parent_area_code', NATIONAL_CODE
	).add('group_id', root.Grouping[0].GroupId // Cannot use model property in search
	).add('area_type_id', model.areaTypeId
	).add('indicator_id', model.iid
	).add('sex_id', model.sexId
	).add('age_id', model.ageId);

	var parameterString = parameters.build();

	if (loaded.trendMarkers[parameterString]) {
		barChartState.trendMarkers = loaded.trendMarkers[parameterString];
		ajaxMonitor.callCompleted();
	} else {
		ajaxGet('api/recent_trends/for_child_areas', parameterString,
			function (trendMarkers) {

				loaded.trendMarkers[parameterString] = trendMarkers;
				barChartState.trendMarkers = trendMarkers;
				ajaxMonitor.callCompleted();
			});
	}
}

function initBarChart(root) {

	var state = barChartState;
		
	if (!state.isInitialised) {

		addBarChartTemplate();

		state.tabSpecificOptions = new TabSpecificOptions({
			eventHandlers: [barAreasClicked, barAreasClicked],
			eventName: 'AreasToDisplaySelected',
			exportImage: {
				label: 'Export table as image',
				clickHandler: function (ev) {
					// Needed so does not reload the page
					ev.preventDefault();
					exportTableAsImage('indicators-container', 'bar-chart');
				}
			}
		});

		state.nationalSortedAreas = loaded.areaLists;        
		// Initialise containers, table and table header
		pages.getContainerJq().html(
			templates.render('indicators', {
				hasRecentTrends: isDefined(root) && isDefined(root.RecentTrends)
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
		'<div style="float:none; clear: both; width: 1000px;"><div id="indicatorDetailsHeader"></div><div id="indicatorDetailsBox" style="width:100%;">' +
		'<table id="indicatorDetailsTable" class="borderedTable" cellspacing="0">' +
		'<thead><tr><th style="width:200px;">Area<a class="columnSort" href="javascript:sortIndicatorDetailsByArea();" title="Sort alphabetically by area"></a></th>' +
		'{{#hasRecentTrends}} <th>Recent<br>Trend</th> {{/hasRecentTrends}}' +
		'<th class="nearest-neighbours-show" style="border-right:none"><div class="center">Neighbour Rank</div><a class="columnSort" href="javascript:sortIndicatorDetailsByRank();" title="Sort by rank"></a></th>' +
		'<th style="border-right: none;"><div class="center">Count</div><a class="columnSort" href="javascript:sortIndicatorDetailsByCount();" title="Sort by count"></a></th>' +
		'<th style="border-right: none;"><div class="center">Value</div><a class="columnSort" href="javascript:sortIndicatorDetailsByValue();" title="Sort by values"></a></th>' +
		'<th class="bar">&nbsp;</th><th title="Lower confidence interval"><span class="sig-level" style="width:100%;display:block;"></span>Lower CI</th>' +
		'<th title="Upper confidence interval"><span class="sig-level" style="width:100%;display:block;"></span>Upper CI</th></tr></thead>' +
		'<tbody></tbody></table><br>' +
		'<div id="indicatorDetailsSource"></div></div></div></div>');
}

function addBarChartOptions() {
	barChartState.tabSpecificOptions.setOption(barChartState.areaDisplayed);
	barChartState.tabSpecificOptions.setHtml({
		label: 'Areas in',
		optionLabels: [getParentArea().Name, 'England']
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
		ajaxMonitor.setCalls(2);
		var root = groupRoots[getIndicatorIndex()];
		getAreaValues(root, FT.model, NATIONAL_CODE);
		getRecentTrends(root);
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
				? FT.data.sortedAreas
				: state.nationalSortedAreas[FT.model.areaTypeId];

			state.currentData = getDataSortedByArea(data, areas);
			displayIndicatorTable(root, regionalGrouping, nationalGrouping, comparisonConfig, areas);
		}

		if (hasStateChanged) {
			barChart.setIndicatorTableHeaderHtml(root, rootIndex);
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

    buildTrendTooltip(root);

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
		var trimLength = 25;
		var areaName = '';
		var nameColumnWidth;
        


		// Show short name only if it is an Acute Trust
		if (area.AreaTypeId === AreaTypeIds.AcuteTrust) {
			areaName = getShortAreaNameToDisplay(area);
			trimLength = 50;
			nameColumnWidth = '300px';
		} else {
			areaName = getAreaNameToDisplay(area);
			nameColumnWidth = '200px';
		}
		$('#indicatorDetailsTable thead tr th:first').css('width', nameColumnWidth);

		addIndicatorTableAreaRow(html, data[i], area.Code,
			trimName(areaName, trimLength)/*area name label*/, i, null, comparisonConfig, root);
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
		if (!FT.ajaxLock) {
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
			behaviour = ' onclick="barChart.selectBenchmarkClicked(' + compId + ')" ';
			var extraLabelClass = '';
		} else {
			// Current benchmark
			extraLabelClass = 'bold ';
		}

		addTd(html, areaName, extraLabelClass);
	}

	var dataInfo = new CoreDataSetInfo(data);


	// Trend if applicable
	if (isDefined(root) && isDefined(root.RecentTrends)) {
		var trendMarker = root.RecentTrends[areaCode];

		if (trendMarker) {
			var marker = trendMarker.Marker;
			barChartState.trendsTooltip.addTooltip(areaCode, trendMarker);
		} else if (barChartState.trendMarkers[areaCode]) {
			marker = barChartState.trendMarkers[areaCode].Marker;
			barChartState.trendsTooltip.addTooltip(areaCode, trendMarker);
		} else {
			marker = TrendMarkerValue.CannotCalculate;
			barChartState.trendsTooltip.addTooltip(areaCode);
		}
			
		barChartState.trendsTooltip.addTooltip(areaCode);        
		html.push(barChartRenderTrendCell(areaCode, getTrendMarkerImage(marker, root.PolarityId)));		

			tooltipManager.initElement('bar-trend_' +areaCode);
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
				rankValue = _.findWhere(FT.data.sortedAreas, { Code: areaCode }).Rank;
				break;
		}

		addTd(html, rankValue, CSS_CENTER);
	}

	addTd(html, count, CSS_NUMERIC);
	addTd(html, val, CSS_NUMERIC, null, dataInfo.getNoteId());

	// Bar
	var bar = isValue ?
		barChart.getBarHtml(data, comparisonConfig) :
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

function sortIndicatorDetailsByRank() {

	var state = barChartState;
	state.valueOrder = 0;

	var rootIndex = getIndicatorIndex();
	var newOrder = invertSortOrder(state.areaOrder);
	state.areaOrder = newOrder;

	FT.data.sortedAreas = sortAreasByRank();

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
	    FT.data.sortedAreas = new AreaAndDataSorter(newOrder, root.Data, FT.data.sortedAreas, areaHash).byCount();
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
		FT.data.sortedAreas = new AreaAndDataSorter(newOrder, groupRoots[rootIndex].Data,
			FT.data.sortedAreas, areaHash).byValue();
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
		? FT.data.sortedAreas
		: state.nationalSortedAreas[FT.model.areaTypeId];

	sortAreasAToZ(newOrder, areasToSort);

	displayIndicatorDetails(rootIndex);
	logEvent('BarChart', 'DataSortedByArea');
};


function buildTrendTooltip(root) {
    var trends;

    if (barChartState.areaDisplayed === BAR_CHART_VIEW_MODES.REGION) {
        trends = root.RecentTrends;
    } else {
        trends = barChartState.trendMarkers;
    }

	for (var area in trends) {	
	    barChartState.trendsTooltip.addTooltip(area, trends[area]);
        tooltipManager.initElement('bar-trend_' + area);
	}
}


var barChartState = {

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
	tabSpecificOptions: null,
	isFirstPassToNearestNeighbours: true,
	trendMarkers: {},
	trendsTooltip: new RecentTrendsTooltip(),

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
	var data = FT.data;
	var root = groupRoots[getIndicatorIndex()];
    var hasRecentTrends = isDefined(root) && isDefined(root.RecentTrends);
    var barChartColumn = hasRecentTrends ? ':eq(4)' : ':eq(3)';

    data.highlightedRowCells.filter(barChartColumn).css({
		'border-top-color': '#fff',
		'border-bottom-color': '#fff'
	});
	data.highlightedRowCells = null;
};

function isNearestNeighboursAndSingleAreaView() {
	return FT.model.isNearestNeighbours() && barChartState.areaDisplayed === BAR_CHART_VIEW_MODES.REGION;
}

function showAndHideTabSpecificOptions() {
	var $options = $('#tab-specific-options');
	if (FT.model.parentCode === NATIONAL_CODE) {
		$options.hide();
	} else {
		$options.show();
	}
}

var BAR_CHART_VIEW_MODES = {
	REGION: 0,
	ENGLAND: 1
};

function barChartRenderTrendCell(areaCode,innerContent) {
    templates.add('barChartTrendCell', '<td id="bar-trend_{{areaCode}}" onclick="recentTrendSelected.fromCompareArea(\'{{areaCode}}\');" class="cursor-pointer center">{{{innerContent}}}</td>');
    var html = templates.render('barChartTrendCell',
	{
		areaCode: areaCode,
		innerContent: innerContent
	});
	return html;
}

// Tooltip provider for BarChart page
function BarChartTooltipProvider() {}

BarChartTooltipProvider.prototype = {

	getHtml: function (id) {		
		var bits = id.split('_'),
			firstBit = bits[0];

		if (firstBit === 'ft') {
			return this.getValueNoteCellText(bits);
		}

		if (firstBit === 'bar-trend') {
		    return barChartState.trendsTooltip.getTooltip(bits[1]);
		}

		return '';
	},

	getValueNoteCellText: function (bits) {
		var valueNoteId = $('#ft_' + bits[1]).attr('vn');

		//TODO template this
		return !valueNoteId ?
			'' :
			'<span id="tooltipData"></span>' + this.getHtmlFromNoteId(valueNoteId);
	},

	getHtmlFromNoteId: function (id) {

		return !id ?
			'' :
			[
				'<span class="tooltipValueNote">', VALUE_NOTE,
				loaded.valueNotes[id].Text, '</span>'
			].join('');
	}

};


FT.data.highlightedRowCells = null;

pages.add(PAGE_MODES.INDICATOR_DETAILS, {
	id: 'indicators',
	title: 'Compare areas',
	goto: goToIndicatorDetailsPage,
	gotoName: 'goToIndicatorDetailsPage',
	needsContainer: true,
	showHide: displayBarChartLegend,
	jqIds: ['indicatorMenuDiv', '.geo-menu', 'tab-specific-options', 'value-note-legend', 'nearest-neighbour-link', 'trend-marker-legend'],
	jqIdsNotInitiallyShown: ['data-quality-key', 'target-benchmark-box', 'keyAdHoc', 'key-bar-chart', 'key-spine-chart']
});

