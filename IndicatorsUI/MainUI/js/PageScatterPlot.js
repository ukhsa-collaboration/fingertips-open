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

    setPageMode(PAGE_MODES.SCATTER_PLOT);
    var sp = scatterplot;

    if (!areIndicatorsInDomain()) {
        displayNoData();
        sp.showHideElementsForNoData();

    } else {
        sp.init();
        sp.showHideElements();
        sp.loadPrimaryIndicatorMenu();
    }
}

scatterplot.showHideElements = function () {

    if (FT.model.areaTypeId === AreaTypeIds.Practice) {
        $('#scatterplot-filters').hide();
        $('#export-chart-box').hide();
        $('#export-chart-box-csv').hide();
        $('#scatter-key-box').show();
        $('#scatter-plot-gp-chart').show();
        $('#scatter-plot-chart').hide().html('');
    }
    else {
        $('#scatterplot-filters').show();
        $('#export-chart-box').show();
        $('#export-chart-box-csv').show();
        $('#scatter-key-box').hide();
        $('#scatter-plot-chart').show();
        $('#scatter-plot-gp-chart').hide().html('');
    }
    $('#supporting-indicators-wrapper').show();
};

scatterplot.showHideElementsForNoData = function () {

    if (isEnglandAreaType()) {
        $('#scatterplot-filters').hide();
        $('#export-chart-box').hide();
        $('#export-chart-box-csv').hide();
        $('#scatter-key-box').hide();
        $('#scatter-plot-gp-chart').hide();
        $('#supporting-indicators-wrapper').hide();
        $('#indicator-menu-div').hide();
    }else{
        $('#supporting-indicators-wrapper').show();
    }
};

scatterplot.displayGraph = function () {

    scatterplot.loadSecondaryIndicatorMenu();
    if (FT.model.areaTypeId === AreaTypeIds.Practice) {
        scatterplot.getGpDataForSelectedArea();
    } else {
        scatterplot.getDataForSelectedArea();
    }
};

scatterplot.loadPrimaryIndicatorMenu = function () {
    var model = FT.model;
    ajaxMonitor.setCalls(1);
    scatterplot.getGroupingDataForProfile(model);
    ajaxMonitor.monitor(scatterplot.displayGraph);
};

scatterplot.loadSecondaryIndicatorMenu = function () {

    var $supportingIndicators = $('#supporting-indicators');
    //Set the supporting indicator (Y-Axis) to the currently selected one if it exists. Otherwise set it to the first in the list.
    var selectedSupportingIndicatorIndex = 0;
    var currentlySelectedYIndicator = $('#supporting-indicators option:selected').attr('key');

    // populate supporting indicators
    for (var i = 0; i < _.size(loaded.groupingDataForProfile) ; i++) {
        var sex = new SexAndAge().getLabel(loaded.groupingDataForProfile[i]);
        var indicatorName = loaded.groupingDataForProfile[i].IndicatorName + sex;
        var key = loaded.groupingDataForProfile[i].IID + '-' + loaded.groupingDataForProfile[i].Sex.Id;
        $supportingIndicators.append('<option key="' + key + '" value="' + i + '">' + indicatorName + '</option>');
        if (key === currentlySelectedYIndicator) {
            selectedSupportingIndicatorIndex = i;
        }
    }
    var supportingGroupRoots = loaded.groupingDataForProfile;
    loaded.groupRootSecondary = supportingGroupRoots[selectedSupportingIndicatorIndex];
};
/**
* Initialises the scatterplot page based on the areatypeId. Only the first call has an effect
* @class scatterplot.init
*/
scatterplot.init = function () {
    var sp = scatterplot;
    if (FT.model.areaTypeId === AreaTypeIds.Practice) {
        if (!sp.viewManagerGPScatterPlot) {
            sp.viewManagerGPScatterPlot = new sp.ViewManagerGpScatterPlot(pages.getContainerJq());
            sp.viewManagerGPScatterPlot.init();
        }
    } else {
        if (!sp.viewManager) {
            sp.viewManager = new sp.ViewManager(pages.getContainerJq());
            sp.viewManager.init();
        }
    }
};

scatterplot.getParentAreaCode = function () {
    if (FT.model.isNearestNeighbours()) {
        return FT.model.nearestNeighbour;
    } 
    return getCurrentComparator().Code;
}

scatterplot.getAreasCode = function (){
    if (getCurrentComparator().Name === 'England') {
        return null;
    }

    return getAreasCodeDisplayed();
}

scatterplot.getParentAreaCode = function () {

    if (FT.model.isNearestNeighbours() || getCurrentComparator().Name === 'England'){
        return NATIONAL_CODE;
    }
    return FT.model.parentCode;
}

scatterplot.getIidsString = function () {
    var iidsString = getIid();
    var iidComparasionString = getComparasionIndicatorId();

    if (iidsString != iidComparasionString)
    {
        iidsString = iidsString + "," + iidComparasionString;
    }

    return iidsString;
}

/**
* Create a configuration object for the service call to get area values
* @class scatterplot.getAreaValueModel
*/
scatterplot.getAreaValueModel = function(groupRoot) {
    var model = FT.model;

    scatterplotState.tempIndicatorKey = getIndicatorKey(groupRoot, model) + scatterplot.getParentAreaCode();

    var groupId = groupRoot.hasOwnProperty('GroupId')
        ? groupRoot.GroupId
        : groupRoot.Grouping[0].GroupId;

    var areaValueModel = {
        groupId: groupId,
        indicatorId: groupRoot.IID,
        profileId: model.profileId,
        sexId: groupRoot.Sex.Id,
        ageId: groupRoot.Age.Id,
        areaTypeId: model.areaTypeId,
        key: scatterplotState.tempIndicatorKey
    };

    return areaValueModel;
}

/**
* Get all data for selected area by Ajax()
* @class scatterplot.getDataForSelectedArea
*/
scatterplot.getDataForSelectedArea = function () {
    var groupRoot = getGroupRoot();
    var areaValueModel = scatterplot.getAreaValueModel(groupRoot);
    scatterplotState.indicatorKey1 = areaValueModel.key;

    ajaxMonitor.setCalls(1);
    scatterplot.getAreaValues(areaValueModel);
    ajaxMonitor.monitor(scatterplot.getSecondaryData);
};

/**
* Get secondary data for selected area by Ajax()
* @class scatterplot.getSecondaryData
*/
scatterplot.getSecondaryData = function () {
    scatterplot.makeAreaNamesAndCodes();
    var groupRoot = loaded.groupRootSecondary;
    var areaValueModel = scatterplot.getAreaValueModel(groupRoot);
    scatterplotState.indicatorKey2 = areaValueModel.key;

    ajaxMonitor.setCalls(1);

    scatterplot.getAreaValues(areaValueModel);

    ajaxMonitor.monitor(scatterplot.displayPage);
};

scatterplot.getGpDataForSelectedArea = function () {
    scatterplot.reloadSupportingIndicators();
    scatterplot.displayGpScatterPlotGraph();
};
scatterplot.setScatterSrc = function (src) {

    $('#scatter-plot-gp-chart').html('<img src="' + src + '" alt="" />');
};

scatterplot.displayGpScatterPlotGraph = function () {

    var model = FT.model;
    var groupRoot = getGroupRoot();
    var groupRoot2 = loaded.groupRootSecondary;

    var parameters = new ParameterBuilder(
        ).add('width', 900
        ).add('height',500
        ).add('off',0
        ).add('par',model.parentCode
        ).add('are', model.areaCode
        ).add('gid1', groupRoot2.GroupId
        ).add('iid1', groupRoot2.IID
        ).add('sex1', groupRoot2.Sex.Id
        ).add('age1', groupRoot2.Age.Id
        ).add('gid2', model.groupId
        ).add('iid2', groupRoot.IID
        ).add('sex2', groupRoot.Sex.Id
        ).add('age2', groupRoot.Age.Id);

    scatterplot.setScatterSrc(FT.url.bridge + 'img/gp-scatter-chart?' + parameters.build());

    updateScatterPracticeLegendLabel();
    updateScatterParentLegendLabel();
    updateScatterOtherPracticeLegendLabel();

    showAndHidePageElements();
    unlock();
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

    if (!isEnglandAreaType() && _.size(scatterplot.dataSeries[0].data) > 0) {
        viewManager.createScatterPlot();
    } else {
        viewManager.displayNoData();
        scatterplot.showHideElementsForNoData();
    }

    var areaName = getAreaNameToDisplay(areaHash[FT.model.areaCode]);

    $('#filterSelectedAreaName').text(areaName);
};

scatterplot.reloadSupportingIndicators = function () {

    //Reload the Y-Axis indicator list and set to the previous selected one if it is available
    var currentSelectionValue = $('#supporting-indicators option:selected').attr('key');

    // Re-populate supporting indicators
    var $supportingIndicators = $('#supporting-indicators');
    $supportingIndicators.empty();
    for (var i = 0; i < _.size(loaded.groupingDataForProfile) ; i++) {
        var sex = new SexAndAge().getLabel(loaded.groupingDataForProfile[i]);
        var indicatorName = loaded.groupingDataForProfile[i].IndicatorName + sex;
        var IID = loaded.groupingDataForProfile[i].IID;
        var sexId = loaded.groupingDataForProfile[i].Sex.Id;
        $supportingIndicators.append('<option key="' + IID + '-' + sexId + '" value="' + i + '">' + indicatorName + '</option>');
    }

    $('#supporting-indicators [key="' + currentSelectionValue + '"]').attr('selected', 'selected');
    $supportingIndicators.chosen({ width: '100%', search_contains: true });
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
        chart;

    this.tabSpecificOptions = null;

    this.init = function () {
        if (!isInitialised) {

            var $filters = '<div id="scatterplot-filters">' +
                '<input type="checkbox" id="selectedAreaFilter" onchange="scatterplot.highlightAreaFilter(this);">Highlight <span id="filterSelectedAreaName">' +
                 getAreaNameToDisplay(areaHash[FT.model.areaCode]) +
                '</span><br>' +
                '<input type="checkbox" id="r2Filter" onchange="scatterplot.addR2(this);">Add regression line & R\xB2' +
                ' <br></div>';

            var exportTypes = '<div id="export-chart-box" class="export-chart-box"><a class="export-link" href="javascript:scatterplot.viewManager.exportChart()">Export chart as image</a></div>'+
            '<div id="export-chart-box-csv" class="export-chart-box-csv"><a id="export-link-csv-scatter" class="export-link-csv" href="javascript:scatterplot.viewManager.exportChartAsCsv()">Export chart as csv file</a></div>';

            $chartBox = $('<div id="scatter-plot-chart-box-wrapper">' + exportTypes + '<div id="scatter-plot-chart-box" class="clearfix"><div id="scatter-plot-chart"></div></div>' + $filters + '</div>');
            scatterplot.addSupportingIndicatorsMenu($container, $chartBox);
            this.initTabSpecificOptions();
            isInitialised = true;
        }
    };

    this.exportChart = function () {
        chart.exportChart({ type: 'image/png' }, {});
        logEvent('ExportImage', getCurrentPageTitle());
    };

    this.exportChartAsCsv = function (){
        
        var parameters = new ParameterBuilder()
        .add('parent_area_type_id', FT.model.parentTypeId)
        .add('child_area_type_id', FT.model.areaTypeId)
        .add('profile_id', FT.model.profileId)
        .add('areas_code', scatterplot.getAreasCode())
        .add('parent_area_code', scatterplot.getParentAreaCode())
        .add('indicator_ids', scatterplot.getIidsString());
        
        downloadLatestNoInequalitiesDataCsvFileByIndicator(FT.url.corews, parameters);
    }

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

        var noDataText = '<p style="text-align: center; padding:150px 0 150px 0;">' + getMessageForNoDataText() + '</p>';  

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
                    text: scatterplot.formatTitle(scatterplot.yAxisTitle(), units.y),
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


scatterplot.formatTitle = function (indicatorName, unit) {
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
        ).add('area_type_id', model.areaTypeId);

    if (isInSearchMode()) {
        var path = 'by_indicator_id';
        parameters.add('indicator_ids', getIndicatorIdsParameter());
    } else {
        path = 'by_profile_id';
        parameters.add('profile_id', model.profileId);
    }

    ajaxGet('api/grouproot_summaries/' + path,
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
            ).add('parent_area_code', scatterplot.getParentAreaCode()
            ).add('profile_id', model.profileId
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
scatterplot.getAreaValuesCallback = function (coreDataList) {

    // Hash: Key -> area code, Value -> coredataset
    loaded.areaValues[scatterplotState.tempIndicatorKey] = addOrderandPercentilesToData(coreDataList);

    ajaxMonitor.callCompleted();
};

/**
* Transform data for scatter plot chart series
* @class scatterplot.transformDataForIndicator
*/
scatterplot.transformDataForIndicator = function () {

    var selectedAreaCode = FT.model.areaCode;

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
        var rSquareEquation = 'y = ' + Math.round(linearRegressionData.slope * 100) / 100 + 'x +' +
            Math.round(linearRegressionData.intercept * 100) / 100;
        var rSquaredValue = Math.round(linearRegressionData.r2 * 100) / 100;
        var rSquareThreshold = .15;
        var legendBaseText = rSquareEquation + '<br>R\xB2 = ' + Math.round(linearRegressionData.r2 * 100) / 100;


        scatterplot.dataSeries.push({
            data: rSquaredValue > rSquareThreshold ? linearRegressionData.coordinates : null,
            color: rSquaredValue > rSquareThreshold ? '#ED1F52' : '#ffffff',
            dashStyle: 'solid',
            type: 'line',
            animation: false,
            name: rSquaredValue > rSquareThreshold
                ? legendBaseText
                : 'Trend line not drawn when R\xB2<br>is below 0.15 (R\xB2 = ' + rSquaredValue + ')',
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

    var $supportingIndicator = $('#supporting-indicators :selected');
    var selectedSupportingIndicatorId = loaded.groupingDataForProfile[$supportingIndicator.val()].IID;

    var units = {};
    units.x = ui.getMetadataHash()[selectedMainIndicatorId].Unit.Label;
    units.y = _.where(loaded.groupingDataForProfile, { IID: selectedSupportingIndicatorId })[0].Unit.Label;

    return units;
};

scatterplot.supportingIndicatorChanged = function () {

    lock();

    var selectedSupportingIndicatorIndex = 0;

    // Format of key "<iid>-<sexId>"
    var currentlySelectedYIndicator = $('#supporting-indicators option:selected').attr('key');

    for (var i = 0; i < _.size(loaded.groupingDataForProfile) ; i++) {
        var key = loaded.groupingDataForProfile[i].IID + '-' + loaded.groupingDataForProfile[i].Sex.Id;
        if (key === currentlySelectedYIndicator) {
            selectedSupportingIndicatorIndex = i;
        }
    }

    loaded.groupRootSecondary = loaded.groupingDataForProfile[selectedSupportingIndicatorIndex];
    if (FT.model.areaTypeId === AreaTypeIds.Practice) {
        scatterplot.displayGpScatterPlotGraph();
    } else {
        scatterplot.getSecondaryData();
    }
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
    var yTitle = $('#supporting-indicators :selected').text();
    return yTitle;
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
    supportingIndicatorName = $('#supporting-indicators :selected').text();

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
loaded.groupRootSecondary = {};


    pages.add(PAGE_MODES.SCATTER_PLOT, {
        id: 'scatter',
        title: 'Compare<br>indicators',
        'goto': goToScatterPlotPage,
        gotoName: 'goToScatterPlotPage',
        needsContainer: true,
        jqIds: ['.geo-menu', 'indicator-menu-div', 'nearest-neighbour-link', 'area-list-wrapper', 'filter-indicator-wrapper'],
        jqIdsNotInitiallyShown: []
    });


scatterplot.addSupportingIndicatorsMenu = function($container, $chartBox) {
    var supportingIndicators = $('#supporting-indicators');
    var $supportingIndicators =
      $('<div id="supporting-indicators-wrapper" class="clearfix"><div class="supporting-indicators">Indicator on Y axis&nbsp;</div><div class="supporting-indicators-menu"><select id="supporting-indicators"  onchange="scatterplot.supportingIndicatorChanged(this);"></select></div></div>');
    if (supportingIndicators.length) {
        $container.append($chartBox);
    } else {
        $container.append($supportingIndicators, $chartBox);
    }
}

scatterplot.ViewManagerGpScatterPlot = function ($container) {

    var isInitialised = false,
        $header,
        $chartBox;

    this.tabSpecificOptions = null;

    this.init = function () {
        if (!isInitialised) {

            $chartBox =
                $('<div id="scatter-plot-chart-box-wrapper"><div id="scatter-plot-gp-chart-box" class="clearfix"><div class="fl w100"><table cellspacing="0"><tr><td><div id="scatter-key-box"><table id="scatter-key-table" cellspacing="0"><tr><td id="scatterPractice"></td><td id="scatterParent" class="noLeft"></td><td id="scatterOther" class="noLeft"></td></tr></table></div></td></tr></table></div><div id="scatter-plot-gp-chart"></div></div></div>');
            scatterplot.addSupportingIndicatorsMenu($container, $chartBox);
            isInitialised = true;
        }
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

        var noDataText = '<p style="text-align: center; padding:150px 0 150px 0;">' +
            getMessageForNoDataText() +
            '</p>';

        $('#scatter-plot-chart').html(noDataText);
    };
};

function updateScatterParentLegendLabel() {
    var label = getParentArea().Name;
    setScatterKeyImg('scatterParent', 'parent', label);
};

function updateScatterPracticeLegendLabel() {

    var code = FT.model.areaCode;
    var label = areaHash[code].Name;
    setScatterKeyImg('scatterPractice', 'practice', label);
};

function updateScatterOtherPracticeLegendLabel() {
    setScatterKeyImg('scatterOther', 'other', 'Other practices in England');
};

function setScatterKeyImg(id, item, label) {
    $('#' + id).html('<img src="' + FT.url.img + 'scatter-' + item + '.png" />' + label);
};

function getMessageForNoDataText(){
    var noDataText = '';

        if (isEnglandAreaType())
        {
            noDataText = 'Not applicable for England data' ;
        }else{
            noDataText = scatterplot.getNoDataIndicatorName() ;
        }  

    return noDataText;
}

function getComparasionIndicatorId(){
    var comparasionIids = $('#supporting-indicators option:selected', this).attr('key');
    return comparasionIids.substr(0, comparasionIids.indexOf('-'));
}