'use strict';

/**
* Scatter namespace
* @module scatter
*/
var scatterplot = {};

/**
* Entry point to scatter plot page
* @class goToScatterPlotPage
*/
function goToScatterPlotPage() {
    if (!groupRoots.length) {
        // Search results empty
        noDataForAreaType();
    } else {
        setPageMode(PAGE_MODES.SCATTER_PLOT);
        var sp = scatterplot;
        sp.init();
        sp.getDataForSelectedArea();
    }
}

/**
* Initialises the scatterplot page. Only the first call has an effect
* @class scatterplot.init
*/
scatterplot.init = function () {
    var sp = scatterplot;

    if (!sp.viewManager) {
        sp.viewManager = new sp.ViewManager(pages.getContainerJq());
        sp.viewManager.init();
    }
};

/**
* Get all data for selected area by Ajax()
* @class scatterplot.getDataForSelectedArea
*/
scatterplot.getDataForSelectedArea = function () {
    var model = FT.model,
        groupRoot = getGroupRoot();

    ajaxMonitor.setCalls(2);

    scatterplot.getGroupingDataForProfile(model);

    scatterplotState.tempIndicatorKey = scatterplotState.indicatorKey1 = getIndicatorKey(groupRoot, model) + getCurrentComparator().Code;

    var areaValueModel = {
        groupId: model.groupId,
        indicatorId: groupRoot.IID,
        sexId: groupRoot.Sex.Id,
        ageId: groupRoot.Age.Id,
        areaTypeId: model.areaTypeId,
        key: scatterplotState.tempIndicatorKey
    };
    scatterplot.getAreaValues(areaValueModel);

    ajaxMonitor.monitor(scatterplot.getSecondaryData);
};

/**
* Get secondary data for selected area by Ajax()
* @class scatterplot.getSecondaryData
*/
scatterplot.getSecondaryData = function () {

    //Set the supporting indicator (Y-Axis) to the currently selected one if it exists. Otherwise set it to the first in the list.
    var selectedSupportingIndicatorIndex = 0;
    var currentlySelectedYIndicator = $('#supportingIndicators option:selected').attr('key');

    // populate supporting indicators
    var $supportingIndicators = $('#supportingIndicators');
    for (var i = 0; i < _.size(loaded.groupingDataForProfile) ; i++) {
        var sex = new SexAndAge().getLabel(loaded.groupingDataForProfile[i]);
        var indicatorName = loaded.groupingDataForProfile[i].IndicatorName + sex;
        var key = loaded.groupingDataForProfile[i].IID + '-' + loaded.groupingDataForProfile[i].Sex.Id;
        $supportingIndicators.append('<option key="' + key + '" value="' + i + '">' + indicatorName + '</option>');
        if (key == currentlySelectedYIndicator) {
            selectedSupportingIndicatorIndex = i;
        }
    }

    scatterplot.makeAreaNamesAndCodes();
    ajaxMonitor.setCalls(1);

    var supportingGroupRoots = loaded.groupingDataForProfile,
        model = FT.model;

    var groupRoot = supportingGroupRoots[selectedSupportingIndicatorIndex];

    scatterplotState.tempIndicatorKey = scatterplotState.indicatorKey2 = getIndicatorKey(groupRoot, model) + getCurrentComparator().Code;

    var areaValueModel = {
        groupId: groupRoot.GroupId,
        indicatorId: groupRoot.IID,
        sexId: groupRoot.Sex.Id,
        ageId: groupRoot.Age.Id,
        areaTypeId: model.areaTypeId,
        key: scatterplotState.tempIndicatorKey
    };
    scatterplot.getAreaValues(areaValueModel);

    ajaxMonitor.monitor(scatterplot.displayPage);
};

/**
* Displays the scatterplot page with the chart
* @class scatterplot.displayPage
*/
scatterplot.displayPage = function () {

    var viewManager = scatterplot.viewManager;

    scatterplot.reloadSupportingIndicators();

    scatterplot.transformDataForIndicator();

    showAndHidePageElements();
    unlock();

    if (_.size(scatterplot.dataSeries[0].data) > 0) {
        viewManager.createScatterPlot();
    } else {
        viewManager.displayNoData();
    }

    var areaName = getAreaNameToDisplay(areaHash[FT.model.areaCode]);

    $('#filterSelectedAreaName').text(areaName);
};

scatterplot.reloadSupportingIndicators = function () {
    //Reload the Y-Axis indicator list and set to the previous selected one if it is available
    var currentSelectionValue = $('#supportingIndicators option:selected').attr('key');

    $('#supportingIndicators').empty();

    // Re-populate supporting indicators
    var $supportingIndicators = $('#supportingIndicators');
    for (var i = 0; i < _.size(loaded.groupingDataForProfile) ; i++) {
        var sex = new SexAndAge().getLabel(loaded.groupingDataForProfile[i]);
        var indicatorName = loaded.groupingDataForProfile[i].IndicatorName + sex;
        var IID = loaded.groupingDataForProfile[i].IID;
        var sexId = loaded.groupingDataForProfile[i].Sex.Id;
        $supportingIndicators.append('<option key="' + IID + '-' + sexId + '" value="' + i + '">' + indicatorName + '</option>');
    }

    $('#supportingIndicators [key="' + currentSelectionValue + '"]').attr('selected', 'selected');
    $supportingIndicators.chosen({ width: '95%', search_contains: true });
    $supportingIndicators.prop('disabled', false).trigger("chosen:updated");
}
/**
* Manages the views on the scatterplot page
* @class scatterplot.ViewManager
*/
scatterplot.ViewManager = function ($container) {

    var isInitialised = false,
        $header,
        $chartBox,
        $supportingIndicators,
        chart;

    this.tabSpecificOptions = null;

    this.init = function () {
        if (!isInitialised) {

            var $filters = '<div id="scatterplot-filters">' +
                '<input type="checkbox" id="selectedAreaFilter" onchange="scatterplot.highlightAreaFilter(this);">Highlight <span id="filterSelectedAreaName">' +
                 getAreaNameToDisplay(areaHash[FT.model.areaCode]) +
                '</span><br>' +
                '<input type="checkbox" id="r2Filter" onchange="scatterplot.addR2(this);">Add trendline & R\xB2' +
                ' <br></div>';

            var exportTypes = '<div class="export-chart-box"><a class="export-link" href="javascript:scatterplot.viewManager.exportChart()">Export chart as image</a></div>';

            $header = $('<div id="scatterplot-header" class="clearfix"></div>');
            $chartBox = $('<div id="scatter-plot-chart-box-wrapper">' + exportTypes + '<div id="scatter-plot-chart-box" class="clearfix"><div id="scatter-plot-chart"></div></div>' + $filters + '</div>');
            $supportingIndicators = $('<div id="supportingIndicatorsWrapper"><div class="supportingIndicators">Indicator on Y axis&nbsp;</div><div class="supportingIndicatorsMenu"><select id="supportingIndicators"  onchange="scatterplot.getSelectedSupportingIndicator(this);"></select></div></div>');
            $container.prepend($header, $supportingIndicators, $chartBox);
            this.initTabSpecificOptions();
            isInitialised = true;
        }
    };

    this.exportChart = function () {
        chart.exportChart({ type: 'image/png' }, {
        });
    };

    /**
    * Sets the HTML to display above the chart
    * @method setHeaderHTML
    */
    this.setHeaderHtml = function (html) {
        $header.html(html);
    };

    /**
    * Hides the chart and filter menu and tells the user there is no data
    * @method displayNoData
    */
    this.displayNoData = function () {

        scatterplot.getNoDataIndicatorName();
        var noDataText = '<p style="text-align: center; padding:150px 0 150px 0;">' +
            scatterplot.getNoDataIndicatorName() + '</p>';

        $('#scatter-plot-chart').html(noDataText);
        $('#scatterplot-filters').hide();
    };

    /**
    * Initialises the area switch 
    * @method initTabSpecificOptions
    */
    this.initTabSpecificOptions = function () {
        var clickHandler = scatterplot.getDataForSelectedArea;
        this.tabSpecificOptions = new TabSpecificOptions({
            eventHandlers: [clickHandler, clickHandler],
            eventName: 'ScatterplotAreaSelected'
        });
    };

    /**
    * Updates the area switch 
    * @method updateTabSpecificOptionsOptions
    */
    this.updateTabSpecificOptionsOptions = function () {
        this.tabSpecificOptions.setHtml({
            label: 'Scatter plot for',
            optionLabels: ['England', areaHash[FT.model.areaCode].Name]
        });
    };

    /**
    * Creates the scatter plot chart
    * @method createScatterChart
    */
    this.createScatterPlot = function () {

        $('#scatterplot-filters').show();
        var units = scatterplot.getIndicatorsUnit();

        chart = new Highcharts.Chart({
            chart: {
                renderTo: 'scatter-plot-chart',
                type: 'scatter',
                zoomType: 'xy',
                width: 900,
                marginRight: 280
            },
            credits: {
                enabled: false
            },
            title: {
                text: ''
            },
            xAxis: {
                title: {
                    enabled: true,
                    text: scatterplot.formatTitle(scatterplot.xAxisTitle(), units.x),                      
                    style: {
                        width: 400
                    }
                }
            },
            yAxis: {
                title: {
                text: scatterplot.formatTitle( scatterplot.yAxisTitle(), units.y),                   
                margin: scatterplot.calculateMargin(scatterplot.yAxisTitle()),
                style: {
                    width: 300
                }
                }
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                y: -200,
                floating: true,
                backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF',
                borderWidth: 0,
                width: 200
            },
            plotOptions: {
                scatter: {
                    marker: {
                        radius: 5,
                        states: {
                            hover: {
                                enabled: true,
                                lineColor: 'rgb(100,100,100)'
                            }
                        },
                        lineWidth: 1,
                        lineColor: '#000000'
                    },
                    states: {
                        hover: {
                            marker: {
                                enabled: false
                            }
                        }
                    },
                    tooltip: {
                        headerFormat: '{point.name}',
                        pointFormat: '<b>{point.name}</b><br>x:<b>{point.xValF}</b> ' + units.x +
                            '<br>y:<b>{point.yValF}</b> ' + units.y,
                    },
                    animation: false
                },
                series: {
                    events: {
                        legendItemClick: function () {
                            return false;
                        }
                    }
                }
            },
            series: scatterplot.dataSeries,
            navigation: {
                menuStyle: {
                    background: '#E0E0E0',
                    margin: '0 0 0 -70px'
                },
                buttonOptions: {
                    x: -270,
                    y: -10
                }
            },
            exporting: {
                enabled: false
            }
        });
    };
};


scatterplot.formatTitle = function(indicatorName, unit) {
    if (unit !== '') {        
        return indicatorName + ' / ' + unit;
    } else {
        return indicatorName;
    }
};

scatterplot.calculateMargin = function (indicatorName) {    
    var charCount = indicatorName.length;
    var newMargin = 30;
    if (charCount > 60) {
        newMargin = (charCount / 60) * 30;
    }    
    return newMargin;
}

scatterplot.isNationalSelected = function () {
    return scatterplot.viewManager.tabSpecificOptions.getOption() === inequalities.AreaOptions.NATIONAL;
};

scatterplot.getSelectedAreaName = function () {
    return scatterplot.isNationalSelected() ? 'England' : areaHash[FT.model.areaCode].Name;
};

/**
* Get grouping data for profile by AJAX
* @class scatterplot.getGroupingDataForProfile
*/
scatterplot.getGroupingDataForProfile = function (model) {
    var parameters = new ParameterBuilder(
    ).add('profile_id', model.profileId
    ).add('area_type_id', model.areaTypeId);

    ajaxGet('api/grouproot_summaries',
        parameters.build(),
        function (obj) {
            loaded.groupingDataForProfile = obj;
            ajaxMonitor.callCompleted();
        });
};

/**
* Get area values by AJAX
* @class scatterplot.getAreaValues
*/
scatterplot.getAreaValues = function (model) {

    var areaValues = loaded.areaValues;

    if (areaValues.hasOwnProperty(model.key)) {
        ajaxMonitor.callCompleted();
    } else {
        var parameters = new ParameterBuilder(
            ).add('group_id', model.groupId
            ).add('area_type_id', model.areaTypeId
            ).add('parent_area_code', getCurrentComparator().Code
            ).add('comparator_id', comparatorId
            ).add('indicator_id', model.indicatorId
            ).add('sex_id', model.sexId
            ).add('age_id', model.ageId);

        ajaxGet('api/latest_data/single_indicator_for_all_areas', parameters.build(), scatterplot.getAreaValuesCallback);
    }
};

/**
* Callback for getAreaValues
* @method scatterplot.getAreaValuesCallback
*/
scatterplot.getAreaValuesCallback = function (obj) {

    // Hash: Key -> area code, Value -> coredataset
    var hash = {},
        areaOrder = [];

    _.each(obj, function (data) {
        hash[data.AreaCode] = data;
        areaOrder.push({ AreaCode: data.AreaCode, Val: data.Val, ValF: data.ValF });
    });

    areaOrder.sort(sortData).reverse();

    // First, count number of areas which have a value.
    var numAreas = 0;
    $.each(areaOrder, function (i, coreData) {
        if (coreData.ValF !== '-') {
            numAreas++;
        }
    });

    // Second, set orderFrac for each.
    var j = 0;
    $.each(areaOrder, function (i, coreData) {
        var data = hash[coreData.AreaCode];
        if (coreData.ValF === '-') {
            data.order = -1;
            data.orderFrac = -1;
        }
        else {
            data.order = numAreas - j;
            data.orderFrac = 1 - j / numAreas;
            var position = numAreas + 1 - j + 1;
            data.quartile = Math.ceil(position / (numAreas / 4));
            data.quintile = Math.ceil(position / (numAreas / 5));
            j++;
        }
    });

    loaded.areaValues[scatterplotState.tempIndicatorKey] = hash;

    ajaxMonitor.callCompleted();
};

/**
* Transform data for scatter plot chart series
* @class scatterplot.transformDataForIndicator
*/
scatterplot.transformDataForIndicator = function () {

    var selectedAreaCode = $('#areaMenu :selected').val();
    var isAreaHighlight = $('#selectedAreaFilter').is(':checked');
    var isR2Selected = $('#r2Filter').is(':checked');

    var rawData = loaded.areaValues;

    var rawX = rawData[scatterplotState.indicatorKey1];
    var rawY = rawData[scatterplotState.indicatorKey2];

    var keysX = _.keys(rawX);

    var mapX = _.object(_.map(rawX, function (item) {
        var values = {};
        values.Val = item.Val;
        values.ValF = item.ValF;
        return [item.AreaCode, values];
    }));

    var mapY = _.object(_.map(rawY, function (item) {
        var values = {};
        values.Val = item.Val;
        values.ValF = item.ValF;
        return [item.AreaCode, values];
    }));

    var allAreaData = [];
    var selectedAreaData = [];
    scatterplot.dataSeries = [];
    var xAxis = [], yAxis = [];
    var xyPair = [];
    for (var i = 0; i < _.size(keysX) ; i++) {
        var x = mapX[keysX[i]];
        var y = mapY[keysX[i]];

        if (!_.isUndefined(y) && !_.isUndefined(x)) {

            var point = {};
            point.name = loaded.areaNamesAndCodes[keysX[i]];
            point.x = x.Val;
            point.y = y.Val;
            point.xValF = x.ValF;
            point.yValF = y.ValF;

            if ((x.Val > -1) && (y.Val > -1)) {

                if (isAreaHighlight && keysX[i] === selectedAreaCode) {
                    selectedAreaData.push(point);
                } else {
                    allAreaData.push(point);
                }
                // x and y axis will be used for R2
                xAxis.push(x.Val);
                yAxis.push(y.Val);
                xyPair.push([x.Val, y.Val]);
            }
        }
    }

    scatterplot.dataSeries.push({
        name: loaded.areaTypes[FT.model.areaTypeId].Short + ' in ' + getCurrentComparator().Short,
        data: allAreaData,
        color: '#7CB5EC',
        marker: {
            symbol: 'circle',
            states: {
                hover: {
                    enabled: true,
                    lineColor: 'rgb(100,100,100)'
                }
            }
        }
    });

    if (isAreaHighlight) {
        scatterplot.dataSeries.push({
            name: areaHash[FT.model.areaCode].Name,
            data: selectedAreaData,
            color: '#fffff',
            marker: {
                symbol: 'diamond',
                radius: 6,
                states: {
                    hover: {
                        enabled: true,
                        lineColor: 'rgb(100,100,100)'
                    }
                }
            },
        });
    }

    if (isR2Selected) {

        var linearRegressionData = linearRegression(yAxis, xAxis);
        var rSquareEquation = 'y = ' + Math.round(linearRegressionData.slope * 100) / 100 + 'x +' + Math.round(linearRegressionData.intercept * 100) / 100;
        var rSquaredValue = Math.round(linearRegressionData.r2 * 100) / 100;
        var rSquareThreshold = .15;
        var legendBaseText = rSquareEquation + '<br>R\xB2 = ' + Math.round(linearRegressionData.r2 * 100) / 100;


        scatterplot.dataSeries.push({
            data: rSquaredValue > rSquareThreshold ? linearRegressionData.coordinates : null,
            color: rSquaredValue > rSquareThreshold ? '#ED1F52' : '#ffffff',
            dashStyle: 'solid',
            type: 'line',
            animation: false,
            name: rSquaredValue > rSquareThreshold ? legendBaseText : 'Trend line not drawn when R\xB2<br>is below 0.15 (R\xB2 = ' + rSquaredValue + ')',
            lineWidth: 2,
            marker: {
                enabled: false
            },
            enableMouseTracking: false
        });
    }
};

scatterplot.getIndicatorsUnit = function () {

    var selectedMainIndicatorId = groupRoots[getIndicatorIndex()].IID;

    var $supportingIndicator = $('#supportingIndicators :selected');
    var selectedSupportingIndicatorId = loaded.groupingDataForProfile[$supportingIndicator.val()].IID;

    var units = {};
    units.x = ui.getMetadataHash()[selectedMainIndicatorId].Unit.Label;
    units.y = _.where(loaded.groupingDataForProfile, { IID: selectedSupportingIndicatorId })[0].Unit.Label;

    return units;
};

scatterplot.getSelectedSupportingIndicator = function () {
    scatterplot.getSecondaryData();
};

/**
* Returns title for x axis of chart
* @method scatterplot.xAxisTitle
*/
scatterplot.xAxisTitle = function () {
    var xTitle = $('#indicatorMenu :selected').text();
    return xTitle;
};

/**
* Returns title for y axis of chart
* @method scatterplot.yAxisTitle
*/
scatterplot.yAxisTitle = function () {
    var yTitle = $('#supportingIndicators :selected').text();
    return yTitle; //.wordWrap(60, '<br>');
};

scatterplot.makeAreaNamesAndCodes = function () {
    var areas = loaded.areaLists[FT.model.areaTypeId];
    loaded.areaNamesAndCodes = _.object(_.map(areas,
        function (item) { return [item.Code, item.Name]; }));
};

scatterplot.highlightAreaFilter = function () {
    scatterplot.displayPage();
};

scatterplot.addR2 = function () {
    scatterplot.displayPage();
};

scatterplot.getNoDataIndicatorName = function () {
    var rawData = loaded.areaValues;

    var rawX = rawData[scatterplotState.indicatorKey1];
    var rawY = rawData[scatterplotState.indicatorKey2];

    var mainIndicatorName,
        supportingIndicatorName,
        returnText = '';

    mainIndicatorName = $('#indicatorMenu :selected').text();
    supportingIndicatorName = $('#supportingIndicators :selected').text();

    var xLength = _.size(rawX);
    var yLength = _.size(rawY);

    var dataIsNotAvailableFor = 'Data is not available for <br> ';
    if (xLength === 0) {
        returnText = dataIsNotAvailableFor + mainIndicatorName;
    }

    if (yLength === 0) {
        returnText = dataIsNotAvailableFor + supportingIndicatorName;
    }

    if (xLength === 0 && yLength === 0) {
        returnText = dataIsNotAvailableFor + mainIndicatorName +
            '<br> & <br>' + supportingIndicatorName;
    }

    return returnText;
}


function linearRegression(y, x) {
    var lr = {};
    var n = y.length;
    var sum_x = 0;
    var sum_y = 0;
    var sum_xy = 0;
    var sum_xx = 0;
    var sum_yy = 0;

    for (var i = 0; i < y.length; i++) {

        sum_x += x[i];
        sum_y += y[i];
        sum_xy += (x[i] * y[i]);
        sum_xx += (x[i] * x[i]);
        sum_yy += (y[i] * y[i]);
    }

    var slope = (n * sum_xy - sum_x * sum_y) / (n * sum_xx - sum_x * sum_x);
    var intercept = (sum_y - slope * sum_x) / n;
    var r2 = Math.pow((n * sum_xy - sum_x * sum_y) /
        Math.sqrt((n * sum_xx - sum_x * sum_x) * (n * sum_yy - sum_y * sum_y)), 2);


    var points = [];
    for (var k = 0; k < x.length; k++) {
        var point = [x[k], x[k] * slope + intercept];
        points.push(point);
    }

    points.sort(function (a, b) {
        if (a[0] > b[0]) { return 1 }
        if (a[0] < b[0]) { return -1 }
        return 0;
    });

    lr['coordinates'] = points;
    lr['slope'] = slope;
    lr['intercept'] = intercept;
    lr['r2'] = r2;

    return lr;
}


scatterplot.AreaOptions = {
    NATIONAL: 0,
    LOCAL: 1
};

var scatterplotState = {
    tempIndicatorKey: null,
    indicatorKey1: null,
    indicatorKey2: null,
    showRSquare: false
};

scatterplot.dataSeries = [];

loaded.groupingDataForProfile = {};
loaded.areaNamesAndCodes = {};

// Scatter plot does not currently work for search results
if (FT.model.profileId !== ProfileIds.SearchResults) {
    pages.add(PAGE_MODES.SCATTER_PLOT, {
        id: 'scatter',
        title: 'Compare<br>indicators',
        'goto': goToScatterPlotPage,
        gotoName: 'goToScatterPlotPage',
        needsContainer: true,
        jqIds: ['.geo-menu', 'indicatorMenuDiv', 'nearest-neighbour-link'],
        jqIdsNotInitiallyShown: []
    });
}
