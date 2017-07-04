"use strict";

/**
* Inequalities namespace
* @module inequalities
*/
var inequalities = {};

/**
* Entry point to inequalities page
* @class goToInequalitiesPage
*/
function goToInequalitiesPage() {

    lock();

    setPageMode(PAGE_MODES.INEQUALITIES);

    if (!areIndicatorsInDomain()) {
        displayNoData();
    } else {
        var ns = inequalities;
        ns.init();
        ns.selectValuesOrTrends();
    }
}

inequalities.selectValuesOrTrends = function() {
    lock();
    var ns = inequalities;
    if (ns.state.isViewModeValues) {
        ns.selectValues();
    } else {
        ns.selectTrends();
    }
};

/**
* Initialises the inequalities page. Only the first call has an effect
* @class init
*/
inequalities.init = function() {
    var ns = inequalities;

    if (!ns.viewManager) {
        // Init view
        ns.viewManager = new ns.ViewManager(pages.getContainerJq());
        ns.viewManager.init();

        // Init data managers
        ns.categoryDataManager = new ns.CategoryDataManager();
        ns.sexDataManager = new ns.SexDataManager();
        ns.ageDataManager = new ns.AgeDataManager();
    }
};

/**
* Get all data for selected area by AJAX
* @class getDataForValues
*/
inequalities.getDataForValues = function() {

    var model = FT.model;
    var groupRoot = getGroupRoot();
    var ns = inequalities;

    ajaxMonitor.setCalls(3);

    ns.getDataForAllCategories(model, groupRoot);
    ns.getDataForAllAges(model, groupRoot);
    ns.getDataForAllSexes(model, groupRoot);

    ajaxMonitor.monitor(ns.determinePartitionToDisplay);
};

/**
* Gets category data by AJAX
* @class getDataForAllCategories
*/
inequalities.getDataForAllCategories = function(model, groupRoot) {

    var ns = inequalities;
    var dataManager = ns.categoryDataManager;

    // Area to get data for
    var areaCode = ns.getSelectedAreaCode();

    if (dataManager.getData(groupRoot, areaCode, model.areaTypeId)) {
        // Data already loaded
        ajaxMonitor.callCompleted();
    } else {
        // Load data
        var parameters = inequalities.getCategoryAjaxParameters(model, groupRoot, areaCode);

        ajaxGet("api/partition_data/by_category",
            parameters.build(),
            function(obj) {
                dataManager.setData(groupRoot,
                    areaCode,
                    model.areaTypeId,
                    new ns.CategoryDataAnalyser(obj));
                ajaxMonitor.callCompleted();
            });
    }
};

/**
* Returns the AJAX parameters for getting the category data
* @class getCategoryAjaxParameters
*/
inequalities.getCategoryAjaxParameters = function(model, groupRoot, areaCode) {
    return new ParameterBuilder(
        ).add("profile_id",
            model.profileId
        )
        .add("area_code",
            areaCode
        )
        .add("indicator_id",
            groupRoot.IID
        )
        .add("area_type_id",
            model.areaTypeId
        )
        .add("sex_id",
            groupRoot.Sex.Id
        )
        .add("age_id", groupRoot.Age.Id);
};

/**
* Gets the most recent data partitioned by sex by AJAX
* @class getDataForAllSexes
*/
inequalities.getDataForAllSexes = function(model, groupRoot) {

    var ns = inequalities;
    var dataManager = ns.sexDataManager;

    // Area to get data for
    var areaCode = ns.getSelectedAreaCode();

    if (dataManager.getData(groupRoot, areaCode, model.areaTypeId)) {
        // Data already loaded
        ajaxMonitor.callCompleted();
    } else {
        // Load data
        var parameters = ns.getSexAjaxParameters(model, groupRoot, areaCode);

        ajaxGet("api/partition_data/by_sex",
            parameters.build(),
            function(obj) {
                dataManager.setData(groupRoot,
                    areaCode,
                    model.areaTypeId,
                    new ns.SexDataAnalyser(obj));
                ajaxMonitor.callCompleted();
            });
    }
};

/**
* Returns the AJAX parameters for getting the sex data
* @class getSexAjaxParameters
*/
inequalities.getSexAjaxParameters = function(model, groupRoot, areaCode) {
    return new ParameterBuilder(
        ).add("profile_id",
            model.profileId
        )
        .add("area_code",
            areaCode
        )
        .add("indicator_id",
            groupRoot.IID
        )
        .add("area_type_id",
            model.areaTypeId
        )
        .add("age_id", groupRoot.Age.Id);
};

/**
* Gets the most recent data partitioned by age by AJAX
* @class getDataForAllAges
*/
inequalities.getDataForAllAges = function(model, groupRoot) {

    var ns = inequalities;
    var dataManager = ns.ageDataManager;

    // Area to get data for
    var areaCode = ns.getSelectedAreaCode();

    if (dataManager.getData(groupRoot, areaCode, model.areaTypeId)) {
        // Data already loaded
        ajaxMonitor.callCompleted();
    } else {
        // Load data
        var parameters = ns.getAgeAjaxParameters(model, groupRoot, areaCode);

        ajaxGet("api/partition_data/by_age",
            parameters.build(),
            function(obj) {
                dataManager.setData(groupRoot,
                    areaCode,
                    model.areaTypeId,
                    new ns.AgeDataAnalyser(obj));
                ajaxMonitor.callCompleted();
            });
    }
};

/**
* Returns the AJAX parameters for getting the age data
* @class getAgeAjaxParameters
*/
inequalities.getAgeAjaxParameters = function(model, groupRoot, areaCode) {
    return new ParameterBuilder(
        ).add("profile_id",
            model.profileId
        )
        .add("area_code",
            areaCode
        )
        .add("indicator_id",
            groupRoot.IID
        )
        .add("area_type_id",
            model.areaTypeId
        )
        .add("sex_id", groupRoot.Sex.Id);
};

/**
* Displays the inequalities page
* @class determinePartitionToDisplay
*/
inequalities.determinePartitionToDisplay = function() {

    var ns = inequalities;
    var state = ns.state;
    var areaCode = ns.getSelectedAreaCode();
    var areaTypeId = FT.model.areaTypeId;
    var groupRoot = getGroupRoot();
    var viewManager = ns.viewManager;

    var categoryDataAnalyser = ns.categoryDataManager.getData(groupRoot, areaCode, areaTypeId);
    var ageDataAnalyser = ns.ageDataManager.getData(groupRoot, areaCode, areaTypeId);
    var sexDataAnalyser = ns.sexDataManager.getData(groupRoot, areaCode, areaTypeId);

    var isCategoryData = categoryDataAnalyser.isAnyData;
    var isAgeData = ageDataAnalyser.isAnyData;
    var isSexData = sexDataAnalyser.isAnyData;

    // Define default category in case needed
    var defaultCategoryTypeId = null;
    if (isCategoryData) {
        defaultCategoryTypeId = categoryDataAnalyser.categoryTypes[0].Id;
    }

    var isAnyData = isCategoryData || isAgeData || isSexData;

    if (isAnyData) {
        viewManager.displayMenu(categoryDataAnalyser, ageDataAnalyser, sexDataAnalyser);
        var preferredPartition = state.preferredPartition;
        if (isDefined(preferredPartition)) {
            // Use last thing the user looked at
            switch (preferredPartition) {
            case ns.PartitionsTypes.BySex:
                if (isSexData) {
                    ns.selectSex();
                } else {
                    state.preferredPartition = null;
                    ns.selectCategoryType(defaultCategoryTypeId);
                }
                break;
            case ns.PartitionsTypes.ByAge:
                if (isAgeData) {
                    ns.selectAge();
                } else {
                    state.preferredPartition = null;
                    ns.selectCategoryType(defaultCategoryTypeId);
                }
                break;
            default:
                if (isSexData && !isCategoryData) {
                    ns.selectSex();
                } else if (isAgeData && !isCategoryData) {
                    ns.selectAge();
                } else {
                    var categoryTypeId = ns.getCategoryTypeIdToDisplay();
                    ns.selectCategoryType(categoryTypeId);
                }
                break;
            }
        } else {
            // Nothing has been looked at yet
            if (isCategoryData) {
                ns.selectCategoryType(defaultCategoryTypeId);
            } else if (isAgeData) {
                ns.selectAge();
            } else if (isSexData) {
                ns.selectSex();
            }
        }
    } else {
        viewManager.displayNoData();
    }

    // Display values
    if (state.isViewModeValues || !isAnyData) {
        ns.displayValues();
    } else {
        // change the header
        inequalities.updateTrendHeader();
        inequalities.displayOptions();
        inequalities.toggleDisplayOptionCss(ns.state.isViewModeValues);
    }
};

inequalities.updateTrendHeader = function() {
    var ns = inequalities;
    var viewManager = ns.viewManager;
    viewManager.updateTabSpecificOptionsOptions();

    var indicatorIndex = getIndicatorIndex();
    var groupRoot = getGroupRoot();
    var metadata = ui.getMetadataHash()[groupRoot.IID];
    var period = getFirstGrouping(groupRoot).Period;
    var areaLabel = ns.getSelectedAreaName() + ", " + period + "";
    // Header
    var html = getTrendHeader(metadata, groupRoot, areaLabel, "goToMetadataPage(" + indicatorIndex + ")");
    viewManager.setHeaderHtml(html);
};


/**
* Displays the most recent values trend chart
* @class displayValues
*/
inequalities.displayValues = function() {
    var ns = inequalities;
    var viewManager = ns.viewManager;
    viewManager.updateTabSpecificOptionsOptions();

    var groupRoot = getGroupRoot();
    var metadata = ui.getMetadataHash()[groupRoot.IID];

    ns.updateTrendHeader();
    var comparisonConfig = new ComparisonConfig(groupRoot, metadata);
    setTargetLegendHtml(comparisonConfig, metadata);
    showAndHidePageElements();
    showBarChartLegend(comparisonConfig.useTarget);
    $("#benchmark-box").hide();
    ns.displayOptions();
    ns.toggleDisplayOptionCss(ns.state.isViewModeValues);

    unlock();
};

/**
* Gets the category type ID that should be displayed
* @class getCategoryTypeIdToDisplay
*/
inequalities.getCategoryTypeIdToDisplay = function() {
    var ns = inequalities;

    var areaCode = ns.getSelectedAreaCode();
    var areaTypeId = FT.model.areaTypeId;
    var groupRoot = getGroupRoot();
    var categoryDataAnalyser = ns.categoryDataManager.getData(groupRoot, areaCode, areaTypeId);

    var preferredPartition = inequalities.state.preferredPartition;

    var defaultCategoryTypeId = null;
    if (categoryDataAnalyser.isAnyData) {
        defaultCategoryTypeId = categoryDataAnalyser.categoryTypes[0].Id;
    }

    if (!String.isNullOrEmpty(preferredPartition) && categoryDataAnalyser.isCategoryTypeById(preferredPartition)) {
        return preferredPartition;
    } else if (isDefined(defaultCategoryTypeId) &&
        categoryDataAnalyser.isCategoryTypeById(defaultCategoryTypeId)) {
        return defaultCategoryTypeId;
    }
    return null;
};

/**
* Create html template for latest values / trends buttons
* @class displayOptions
*/
inequalities.displayOptions = function() {

    templates.add("inequalitiesTrendSwitch",
        '<div id="inequalities-trend-switch" class="tab-options clearfix">' +
        '<span id="area-options-inequalities">' +
        "<span>Display</span>" +
        '<button id="inequalities-values" onclick="inequalities.selectValues();">Latest values</button>' +
        '{{#showTrends}}<button id="inequalities-trends" onclick="inequalities.selectTrends();">Trends</button>{{/showTrends}}' +
        "</span>" +
        "</div>");

    var html = templates.render("inequalitiesTrendSwitch",
    {
        showTrends: isFeatureEnabled("inequalityTrends")
    });

    $("#tab-specific-options").prepend(html);
};

/**
* Render and show latest values
* @class selectValues
*/
inequalities.selectValues = function() {
    var ns = inequalities;
    var state = ns.state;

    state.isViewModeTrends = false;
    state.isViewModeValues = true;

    ns.getDataForValues();
};

/**
* Render and show trends
* @class selectTrends
*/
inequalities.selectTrends = function() {
    var ns = inequalities;
    var state = ns.state;

    state.isViewModeTrends = true;
    state.isViewModeValues = false;

    ns.getDataForValues();
};

/**
* AJAX call get trends by age
* @class getTrendsByAge
*/
inequalities.getTrendsByAge = function (groupRoot) {
    var ns = inequalities;
    var model = FT.model;
    var areaCode = ns.getSelectedAreaCode();

    var parameters = ns.getAgeAjaxParameters(model, groupRoot, areaCode);
    ns.state.preferredPartition = ns.PartitionsTypes.ByAge;
    ajaxGet("api/partition_trend_data/by_age", parameters.build(), ns.displayTrends);
};

/**
* AJAX call get trends by sex
* @class getTrendsBySex
*/
inequalities.getTrendsBySex = function(groupRoot) {
    var model = FT.model;
    var ns = inequalities;
    var areaCode = ns.getSelectedAreaCode();

    var parameters = ns.getSexAjaxParameters(model, groupRoot, areaCode);
    ns.state.preferredPartition = ns.PartitionsTypes.BySex;
    ajaxGet("api/partition_trend_data/by_sex", parameters.build(), ns.displayTrends);
};

/**
* AJAX call get trends by category
* @class getTrendsByCategory
*/
inequalities.getTrendsByCategory = function(groupRoot, categoryTypeId) {
    var model = FT.model;
    var ns = inequalities;
    var areaCode = ns.getSelectedAreaCode();
    var parameters = ns.getCategoryAjaxParameters(model, groupRoot, areaCode);
    parameters.add("category_type_id", categoryTypeId);
    ns.state.preferredPartition = categoryTypeId;
        ajaxGet("api/partition_trend_data/by_category", parameters.build(), ns.displayTrends);
};

/**
* Render trend chart template
* @class renderTrendOptions
*/
inequalities.renderTrendOptions = function(labels, hasAverage, average) {
    templates.add("inequalitiesTrendOptions",
        '<div id="inequalities-trend-filters">' +
        "<p>Display on chart:</p>" +
        '<div class="trend-options-filter"><a href="javascript:inequalities.hideOrShowAllTrends(true)">Show all</a></div>' +
        '<div class="trend-options-filter"><a href="javascript:inequalities.hideOrShowAllTrends(false)">Clear chart</a></div>' +
        '<div class="trend-options-filter"><hr></div>' +
        "{{#hasAverage}}" +
        '<div class="trend-option"><input type="checkbox" id="chk-{{average.Id}}"  class="trend-option-checkbox" checked onclick="inequalities.toggleTrend({{average.Id}})" />' +
        '<a href="javascript:inequalities.toggleTrend({{average.Id}})">{{average.Name}}</a>' +
        "</div>" +
        '<div class="trend-options-filter"><hr></div>' +
        "{{/hasAverage}}" +
        "{{#trendFilters}}" +
        '<div class="trend-option">' +
        '<input id="chk-{{Id}}" class="trend-option-checkbox" type="checkbox" checked  onclick="inequalities.toggleTrend({{Id}})" />' +
        '<a href="javascript:inequalities.toggleTrend({{Id}})">{{Name}}</a>' +
        "</div>" +
        "{{/trendFilters}}" +
        "</div>");

    return templates.render("inequalitiesTrendOptions",
    {
        trendFilters: labels,
        average: average,
        hasAverage: hasAverage
    });
};

/**
* Callback for rendering trends 
* @class displayTrends
*/
inequalities.displayTrends = function(data) {
    var ns = inequalities;
    var state = ns.state;
    state.trendData = data;
    var chartOptions, hasAverage;
    var average = {};

    switch (state.preferredPartition) {
        case  ns.PartitionsTypes.BySex:
        // Check if person exists
        var person = _.findWhere(data.Labels, { Id: SexIds.Person });
        // If Person exists don't show in sub-list
        if (!_.isUndefined(person)) {
            chartOptions = _.without(data.Labels, person);
            average = person;
            hasAverage = true;
        } else {
            hasAverage = false;
            chartOptions = data.Labels;
        }
        break;
        case ns.PartitionsTypes.ByAge:
        // Check if AllAges exists
        var allAges = _.findWhere(data.Labels, { Id: AgeIds.AllAges });
        if (!_.isUndefined(allAges)) {
            chartOptions = _.without(data.Labels, allAges);
            average = allAges;
            hasAverage = true;
        } else {
            hasAverage = false;
            chartOptions = data.Labels;
        }
        break;
        default:
        if (_.size(data.AreaAverage) > 0) {
            hasAverage = true;
            // Selected area id will always be zero
            average = {
                Id: 0,
                Name: inequalities.getSelectedAreaName()
            };
            chartOptions = data.Labels;
        } else {            
            hasAverage = false;
            chartOptions = data.Labels;
        }
        break;
    }

    var html = '<div id="inequalities-trend-chart"></div>' +
        ns.renderTrendOptions(chartOptions, hasAverage, average);

    var $trendBox = $("#inequalities-trend-box");
    $trendBox.html(html);

    ns.toggleDisplayOptionCss(ns.state.isViewModeValues); //TODO: this seems worng

    if (data.Limits !== null) {
        ns.createTrendChart(data);

        ns.viewManager.restoreTrendSeriesVisibility();

        showAndHidePageElements();

        $("#benchmark-box").hide();

        $trendBox.show();
    }

    unlock();
};

/**
* Extract and format data for trend chart 
* @class getTrendSeriesData
*/
inequalities.getTrendSeriesData = function(data) {
    var seriesData = [];
    var areErrorBars = inequalities.state.showErrorBars;

    _.each(data.TrendData,
        function(dataList, entityId) {
            var points = [];
            var cis = [];

            _.each(dataList,
                function(data) {
                    var dataInfo = new CoreDataSetInfo(data);

                    // Add point
                    var point = dataInfo.isValue()
                        ? { y: data.Val, ValF: data.ValF }
                        : null;
                    points.push(point);

                    // Add CI
                    if (areErrorBars) {
                        var ci = dataInfo.areCIs()
                            ? [data.LoCI, data.UpCI]
                            : null;
                        cis.push(ci);
                    }
                });

            var label = _.find(data.Labels,
                function(labels) {
                    // Comparison should be == not ===
                    return entityId == labels.Id;
                });

            seriesData.push({ data: points, name: label.Name });
            seriesData.push(inequalities.getErrorBarSeries(cis));
        });

    return seriesData;
};
inequalities.AreaTrendSeriesData = function(data) {

    var points = [];

    var areaData = new CoreDataSetInfo(data.AreaAverage);
    // if we have data process else reutrn null
    _.each(areaData.data,
        function(data) {
            var point = { y: data.Val, ValF: data.ValF };
            points.push(point);
        });
    var seriesData = {
        data: points,
        name: inequalities.getSelectedAreaName(),
        marker: { symbol: "triangle" },
        color: "black"
    };

    return seriesData;
};
inequalities.getErrorBarSeries = function(data) {
    return {
        name: "Errorbars",
        type: "errorbar",
        animation: false,
        data: data
    };
};

/**
* Gets the HighCharts error bar series data for the bar chart
* @class getBarChartCIs
*/
inequalities.getBarChartCIs = function(dataList) {

    var points = [];

    _.each(dataList,
        function(data) {
            var point = new CoreDataSetInfo(data).areCIs()
                ? [data.LoCI, data.UpCI]
                : null;
            points.push(point);
        });

    return points;
};

/**
* Check or uncheck filters
* @class checkOrUncheckAllOptions
*/
inequalities.checkOrUncheckAllOptions = function(isChecked) {
    var shouldBeChecked = isChecked ? "checked" : "";
    $(".trend-option-checkbox").prop("checked", shouldBeChecked);
};

/**
* Shows or hides all trend lines.
* @class hideOrShowAllTrends
*/
inequalities.hideOrShowAllTrends = function(show) {

    var ns = inequalities;
    var series = ns.trendChart.series;

    var seriesLength = series.length;
    for (var i = seriesLength - 1; i > -1; i--) {
        show ? series[i].show() : series[i].hide();
    }
    ns.checkOrUncheckAllOptions(show);
};

/**
* Toggle a trend series on the chart
* @class toggleTrend
*/
inequalities.toggleTrend = function (id) {

    var selectedPartition = inequalities.state.preferredPartition;
    var selectedSeries = null;
    if (id === 0 &&
        selectedPartition !== inequalities.PartitionsTypes.BySex &&
        selectedPartition !== inequalities.PartitionsTypes.ByAge) {
        var area = inequalities.getSelectedAreaName();
        selectedSeries = getSeriesByName(area);
    } else {
        selectedSeries = getSeriesByEntityId(id);
    }

    setSeriesVisibility(selectedSeries, id, !selectedSeries.visible);
};

/**
* Sets whether a series is visible on the chart and updates the check status of the relevant checkbox
* @class setSeriesVisibility
*/
function setSeriesVisibility(series, id, isVisible) {
    // Update check box
    var $checkBox = $("#chk-" + id);
    var checked = "checked";
    if (isVisible) {
        $checkBox.prop(checked, checked);
        series.show();
    } else {
        $checkBox.prop(checked, "");
        series.hide();
    }
}

/**
* Gets a series by the entity ID
* @class getSeriesByEntityId
*/
function getSeriesByEntityId(id) {
    var ns = inequalities;

    var labels = ns.state.trendData.Labels;
    var selectedLabel = _.find(labels,
        function(label) {
            return id == label.Id; // Must be == not ===
        });
    // Find the series
    return getSeriesByName(selectedLabel.Name);
}

/**
* Gets a series by its name
* @class getSeriesByName
*/
function getSeriesByName(name) {    
    return _.find(inequalities.trendChart.series,
        function(s) {
            return name === s.name;
        });
}

/**
* Render high chart for trend
* @class createTrendChart
*/
inequalities.createTrendChart = function(data) {
    var ns = inequalities;
    var groupRoot = getGroupRoot();

    var min = data.Limits.Min,
        max = data.Limits.Max,
        metadata = ui.getMetadataHash()[groupRoot.IID],
        seriesData = ns.getTrendSeriesData(data),
        chartHeight = 350,
        heightPerLegend = 18;
    var unit = metadata.Unit;


    var avg = inequalities.AreaTrendSeriesData(data);
    // Only show area average if we have data
    if (_.size(avg.data) > 0) {
        //        seriesData.push(avg);
        seriesData.splice(0, 0, avg);
    }

    // Adjust chart height to allow for different legend sizes
    var noOfPartitions = _.size(seriesData);
    if (noOfPartitions > 0) {
        chartHeight = chartHeight + (noOfPartitions * heightPerLegend);
    }

    try {
        ns.trendChart = new Highcharts.Chart({
            chart: {
                renderTo: "inequalities-trend-chart",
                defaultSeriesType: "line",
                zoomType: "xy",
                width: 480,
                height: chartHeight,
                animation: false
            },
            title: {
                text: ""
            },
            xAxis: {
                title: {
                    enabled: false
                },
                categories: data.Periods,
                enabled: true,
                step: 1
            },
            yAxis: {
                title: {
                    text: unit.Label,
                    min: min,
                    max: max,
                    enabled: true
                }
            },
            legend: {
                enabled: true,
                layout: "vertical",
                borderWidth: 0
            },
            plotOptions: {
                line: {
                    enableMouseTracking: true,
                    lineWidth: 1,
                    animation: false,
                    marker: {
                        radius: 3,
                        symbol: "circle",
                        lineWidth: 1,
                        lineColor: "#000000"
                    },
                    events: {
                        mouseOut: function() {
                        }
                    }
                },
                series: {
                    events: {
                        legendItemClick: function() {
                            return false;
                        }
                    }
                }
            },
            tooltip: {
                formatter: function() {
                    if (isDefined(this.point.ValF)) {
                        var value = new ValueWithUnit(unit).getFullLabel(this.point.ValF);
                        return "<b>" + value + "</b><br/><i>" + this.series.name + "</i><br/>" + this.point.category;
                    }
                }
            },
            credits: HC.credits,
            series: seriesData,
            exporting: {
                enabled: false,
                chartOptions: {
                    chart: {
                        width: 650
                    }
                }
            }
        });
    } catch (e) {
        console.log(e);
    }
};

inequalities.toggleDisplayOptionCss = function(isValueSelected) {
    var cssClass = "button-selected";
    var $values = $("#inequalities-values");
    var $trends = $("#inequalities-trends");
    if (isValueSelected) {
        $trends.removeClass(cssClass);
        $values.addClass(cssClass);
    } else {
        $values.removeClass(cssClass);
        $trends.addClass(cssClass);
    }
};

/**
* Wrapper for response object from the recent data for all categories service
* @class CategoryDataAnalyser
*/
inequalities.CategoryDataAnalyser = function(data) {

    var categoryData = data.Data,
        categoryTypes = data.CategoryTypes,
        categoryTypeIdToData = {},
        categoryTypesWithData = [],
        i;

    // Filter out invalid values
    categoryData = _.filter(categoryData, function(d) { return d.Val !== -1; });

    for (i in categoryTypes) {
        var categoryType = categoryTypes[i];
        var dataOfType = _.filter(categoryData,
            function(d) { return d.CategoryTypeId === categoryType.Id; });

        if (categoryType.Id !== CategoryTypeIds.HealthProfilesSSILimit && dataOfType.length) {
            categoryTypeIdToData[categoryType.Id] = dataOfType;
            categoryTypesWithData.push(categoryType);
        }
    }

    /**
    * Whether or not there is any category data for the selected indicator
    * @property isAnyData
    */
    this.isAnyData = categoryData.length > 0;

    /**
    * Gets array of category types for which there is data
    * @property categoryTypes
    */
    this.categoryTypes = categoryTypesWithData;

    /**
    * Gets array of data with the specified category type ID
    * @method getDataByCategoryTypeId
    */
    this.getDataByCategoryTypeId = function(categoryTypeId) {
        return categoryTypeIdToData[categoryTypeId];
    };

    /**
    * Get a category type by its ID
    * @method getCategoryTypeById
    */
    this.getCategoryTypeById = function(categoryTypeId) {
        return _.filter(categoryTypesWithData,
            function(type) { return type.Id === categoryTypeId; })[0];
    };

    /**
    * Whether a category type is available by its ID
    * @method isCategoryTypeById
    */
    this.isCategoryTypeById = function(categoryTypeId) {
        return _.some(categoryTypesWithData,
            function(type) { return type.Id === categoryTypeId; });
    };

    /**
    * Get the labels for each category of the specified category type ID
    * @method getCategoryLabels
    */
    this.getCategoryLabels = function(categoryTypeId) {
        var categories = this.getCategoryTypeById(categoryTypeId).Categories;
        return _.pluck(categories, "Name");
    };
};

/**
* Changes the style of the appropriate link to appear selected
* @class setCategoryTypeOptionSelected
*/
inequalities.setCategoryTypeOptionSelected = function(categoryTypeId) {
    inequalities.unselectedPartitionOptions();
    $("#partition-category-type-" + categoryTypeId).addClass("selected");
};

/**
* Build data for categories
* @class CategoryDataBuilder
*/
inequalities.CategoryDataBuilder = function(categoryDataAnalyser, categoryTypeId, average, useRagColours) {

    categoryTypeId = !!categoryTypeId ? categoryTypeId : categoryDataAnalyser.categoryTypes[0].Id;

    var categoryType = categoryDataAnalyser.getCategoryTypeById(categoryTypeId),
        dataList = categoryDataAnalyser.getDataByCategoryTypeId(categoryTypeId),
        categoryIdToData = _.indexBy(dataList, "CategoryId"),
        categories = categoryType.Categories;

    // Define average
    if (new CoreDataSetInfo(average).isValue()) {
        average.y = average.Val;
    } else {
        // Point will not be displayed
        average = null;
    }

    var dataSeries = [],
        averageSeries = [];

    for (var i in categories) {
        var categoryId = categories[i].Id;
        var data = categoryIdToData[categoryId];

        if (new CoreDataSetInfo(data).isValue()) {
            data.y = data.Val;
            var sig = data.Significance;

            data.color = sig === 0
                ? colours.noComparison
                : getColourFromSignificance(sig, useRagColours, colours);
        } else {
            // Show no bar
            data = { Val: 0, y: 0 };
        }

        dataSeries.push(data);
        averageSeries.push(average);
    }

    /**
    * Get category data
    * @method getData
    */
    this.getData = function() {
        var builderData = {};
        builderData.labels = categoryDataAnalyser.getCategoryLabels(categoryTypeId);
        builderData.dataSeries = dataSeries;
        builderData.averageDataSeries = averageSeries;
        builderData.showAverageLine = true;
        builderData.averageLegend = inequalities.getSelectedAreaName() + " average";
        return builderData;
    };
};

/**
* Wrapper for response object from the recent data for all sexes service
* @class SexDataAnalyser
*/
inequalities.SexDataAnalyser = function(data) {

    var sexData = data.Data,
        sexes = data.Sexes;

    /**
    * Is there more than one type of sex data for indicator
    * @property isAnyData
    */
    this.isAnyData = sexData.length > 1;

    /**
    * Returns sexes related data
    * @property sexData
    */
    this.sexData = sexData;

    /**
    * Returns list of sexes available for indicator
    * @property sexes
    */
    this.sexes = sexes;
};

/**
* Build data for Sex
* @class SexDataBuilder
*/
inequalities.SexDataBuilder = function(sexDataAnalyser) {
    var sexData = sexDataAnalyser.sexData,
        sexes = sexDataAnalyser.sexes,
        dataSeries = [],
        averageSeries = [],
        person = _.where(sexData, { SexId: SexIds.Person })[0],
        personExist = _.isUndefined(person) ? false : true;

    if (personExist) {
        sexData = _.clone(sexData);
        // remove the object from sexData array where sex id         
        for (var j in sexData) {
            if (sexData[j].SexId === SexIds.Person) {
                sexData.splice(j, 1);
            }
        }
        // remove the label for Person 
        for (var k in sexes) {
            if (sexes[k].Id === SexIds.Person) {
                sexes.splice(k, 1);
            }
        }
    }

    for (var i in sexData) {
        var data = sexData[i];
        // if value is -1 replace it with zero
        data.y = data.Val === -1 ? 0 : data.Val;

        var sig = data.Significance;

        data.color = sig === 0
            ? colours.noComparison
            : getColourFromSignificance(sig, useRagColours(), colours);

        dataSeries.push(data);
        // If person object exists, set its value to average series data.
        // we don't need to show average line, if we only have male and female.
        if (personExist) {
            averageSeries.push({ y: person.Val, ValF: person.ValF });
        }
    }

    /**
    * Get sex data
    * @method getData
    */
    this.getData = function() {

        var builderData = {};
        builderData.labels = _.pluck(sexes, "Name");
        builderData.dataSeries = dataSeries;
        builderData.averageDataSeries = averageSeries;
        builderData.showAverageLine = personExist;
        builderData.averageLegend = inequalities.getSelectedAreaName() + " persons";
        return builderData;
    };
};

/**
* Wrapper for response object from the recent data for all ages service
* @class AgeDataAnalyser
*/
inequalities.AgeDataAnalyser = function(data) {

    var ageData = data.Data,
        ages = data.Ages;

    /**
    * Is there more than one type of age data for indicator
    * @property isAnyData
    */
    this.isAnyData = ageData.length > 1;

    /**
    * Returns age related data for indicator
    * @property ageData
    */
    this.ageData = ageData;

    /**
    * Returns ages for indicator
    * @property ages
    */
    this.ages = ages;
};


/**
* Build data for Ages
* @class AgeDataBuilder
*/
inequalities.AgeDataBuilder = function(ageDataAnalyser) {
    var average = {},
        ageData = ageDataAnalyser.ageData,
        dataSeries = [],
        averageSeries = [],
        indicatorAgeId = getGroupRoot().Age.Id;

    average = inequalities.getIndicatorAgeRange(ageData, average);

    // Define average
    if (new CoreDataSetInfo(average).isValue()) {
        average.y = average.Val;
    } else {
        // Point will not be displayed
        average = null;
    }

    for (var i in ageData) {
        if (ageData[i].AgeId !== indicatorAgeId) {

            var data = ageData[i];
            data.y = data.Val;

            var sig = data.Significance;

            data.color = sig === 0
                ? colours.noComparison
                : getColourFromSignificance(sig, useRagColours(), colours);

            dataSeries.push(data);
            averageSeries.push(average);
        }
    }

    var avgLabel = inequalities.getAgeLabels(ageDataAnalyser, getGroupRoot().Age.Id);

    /**
    * Get age data
    * @method getData
    */
    this.getData = function() {
        var builderData = {};
        builderData.labels = _.pluck(ageDataAnalyser.ages, "Name");
        builderData.dataSeries = dataSeries;
        builderData.averageDataSeries = averageSeries;
        builderData.showAverageLine = true;
        builderData.averageLegend = inequalities.getSelectedAreaName() + " " + avgLabel;
        return builderData;
    };
};

/**
* Get age average series data
* @class getAgeAverageData
*/
inequalities.getIndicatorAgeRange = function(ageData, average) {
    var ageDataCopy = _.clone(ageData),
        groupRoot = getGroupRoot();

    for (var i in ageDataCopy) {
        if (groupRoot.Age.Id === ageDataCopy[i].AgeId) {
            average.Val = ageDataCopy[i].Val;
            average.ValF = ageDataCopy[i].ValF;
        }
    }
    return average;
};

/**
* Gets the age labels
* @class getAgeLabels
*/
inequalities.getAgeLabels = function(ageDataAnalyser, indicatorAgeId) {
    var ages = ageDataAnalyser.ages;
    for (var i in ages) {
        // remove the object which match the indicator Age Range
        if (ages[i].Id === indicatorAgeId) {
            var indicatorAgeLabel = ages[i].Name;
            ages.splice(i, 1);
        }
    }
    return indicatorAgeLabel;
};

/**
* Event handler for partition menu option
* @class selectAge
*/
inequalities.selectAge = function() {

    var ns = inequalities;
    var state = ns.state;

    ns.unselectedPartitionOptions();
    $("#byAge").addClass("selected");

    var groupRoot = getGroupRoot();

    if (state.isViewModeTrends) {
        ns.getTrendsByAge(groupRoot);
    } else {

        var metadata = ui.getMetadataHash()[groupRoot.IID],
            areaCode = ns.getSelectedAreaCode(),
            ageDataAnalyser = ns.ageDataManager.getData(groupRoot, areaCode, FT.model.areaTypeId),
            data = new ns.AgeDataBuilder(ageDataAnalyser).getData();

            ns.viewManager.createBarChart(data, metadata);
    }
    state.preferredPartition = ns.PartitionsTypes.ByAge;
};

/**
* Event handler for partition menu option
* @class selectSex
*/
inequalities.selectSex = function() {

    var ns = inequalities;
    var state = ns.state;

    ns.unselectedPartitionOptions();
    $("#bySex").addClass("selected");

    var groupRoot = getGroupRoot();

    if (state.isViewModeTrends) {
        ns.getTrendsBySex(groupRoot);
    } else {

        var metadata = ui.getMetadataHash()[groupRoot.IID],
            areaCode = ns.getSelectedAreaCode(),
            sexDataAnalyser = ns.sexDataManager.getData(groupRoot, areaCode, FT.model.areaTypeId),
            data = new ns.SexDataBuilder(sexDataAnalyser).getData();
        ns.viewManager.createBarChart(data, metadata);
    }
    state.preferredPartition = ns.PartitionsTypes.BySex;
};

/**
* Stores and provides the category data retrieved by AJAX.
* @class CategoryDataManager
*/
inequalities.CategoryDataManager = function() {

    this._data = {};

    this._getDataKey = function(groupRoot, areaCode, areaTypeId) {
        return getKey(groupRoot.IID, groupRoot.Sex.Id, groupRoot.Age.Id, areaTypeId, areaCode);
    };
};
inequalities.CategoryDataManager.prototype = {

    /**
    * Gets complex data object that was retrieved by AJAX
    * @method getData
    */
    getData: function(groupRoot, areaCode, areaTypeId) {
        var key = this._getDataKey(groupRoot, areaCode, areaTypeId);
        return this._data[key];
    },

    /**
    * Adds data object that was retrieved by AJAX to the manager
    * @method setData
    */
    setData: function(groupRoot, areaCode, areaTypeId, categoryData) {
        var key = this._getDataKey(groupRoot, areaCode, areaTypeId);
        this._data[key] = categoryData;
    }
};
inequalities.dataManagerPrototype = inequalities.CategoryDataManager.prototype;

/**
* Stores and provides the category data retrieved by AJAX.
* @class CategoryDataManager
*/
inequalities.SexDataManager = function() {

    this._data = {};

    this._getDataKey = function(groupRoot, areaCode, areaTypeId) {
        return getKey(groupRoot.IID, groupRoot.Age.Id, areaCode, areaTypeId);
    };
};

/**
* Gets the data
* @method getData
*/
inequalities.SexDataManager.prototype.getData = inequalities.dataManagerPrototype.getData;

/**
* Sets the data
* @method setData
*/
inequalities.SexDataManager.prototype.setData = inequalities.dataManagerPrototype.setData;

/**
* Stores and provides the category data retrieved by AJAX.
* @class CategoryDataManager
*/
inequalities.AgeDataManager = function() {

    this._data = {};

    this._getDataKey = function(groupRoot, areaCode, areaTypeId) {
        return getKey(groupRoot.IID, groupRoot.Sex.Id, areaCode, areaTypeId);
    };
};


/**
* Gets the data
* @method getData
*/
inequalities.AgeDataManager.prototype.getData = inequalities.dataManagerPrototype.getData;

/**
* Sets the data
* @method setData
*/
inequalities.AgeDataManager.prototype.setData = inequalities.dataManagerPrototype.setData;


/**
* Determines how much offset should be used for the bar labels.
* @class BarLabelOffsetCalculator
*/
inequalities.BarLabelOffsetCalculator = function(dataList) {

    var maxData = _.max(dataList, function(d) { return d.Val; });

    // e.g. 22.2
    var offset = 29;

    if (new CoreDataSetInfo(maxData).isValue()) {
        var valF = new CommaNumber(maxData.ValF).unrounded();
        var len = valF.length;
        if (len === 5) {
            offset = 36;
        }
        if (len === 3) {
            if (valF.indexOf(".") > -1) {
                offset = 22;
            } else {
                offset = 25;
            }
        }
    }

    /** 
    * The value of the label offset 
    * @property
    */
    this.offset = offset;
};

/**
* Determines how much offset should be used for the bar labels.
* @class BarThicknessCalculator
*/
inequalities.BarThicknessCalculator = function(numberOfBars) {

    if (numberOfBars < 4) {
        // e.g. Sex
        var thickness = 30;
    } else if (numberOfBars < 6) {
        // e.g. Sexuality
        thickness = 20;
    } else {
        thickness = 11;
    }

    this.thickness = thickness;
};

/**
* Provides HTML for the inequalities bar chart tooltip
* @class BarChartTooltip
*/
inequalities.BarChartTooltip = function(unit, averageLabel, categoryLabels) {
    /**
    * Gets the tooltip HTML
    * @method getHtml
    */
    this.getHtml = function(obj) {

        var isAverageSeries = obj.series.index === 1,
            point = obj.point;

        var category = isAverageSeries
            ? averageLabel
            : categoryLabels[point.index]; // use these non-truncated labels

        return "<b>" +
            new ValueWithUnit(unit).getFullLabel(point.ValF) +
            "<b><br><i>" +
            category +
            "</i>";
    };
};

/**
* Manages the views on the inequalities page
* @class ViewManager
*/
inequalities.ViewManager = function($container) {

    var isInitialised = false,
        $chartBox,
        $header,
        $partitionMenu,
        chart;
    var seriesVisible = null;
    var optionsChecked = null;
    var checked = "checked";


    templates.add("inequalitiesMenu",
        '<p>Partition data by:</p>{{#types}}<a id="partition-category-type-{{Id}}" categoryTypeId="{{Id}}" {{#isSelected}}class="selected" {{/isSelected}}href="javascript:inequalities.selectCategoryType({{Id}})">{{Name}}</a>{{/types}}' +
        '{{#hasAgeData}}<a id="byAge" {{#isSelected}}class="selected"{{/isSelected}} href="javascript:javascript:inequalities.selectAge()">Age</a>{{/hasAgeData}}' +
        '{{#hasSexData}}<a id="bySex" {{#isSelected}}class="selected"{{/isSelected}} href="javascript:javascript:inequalities.selectSex()">Sex</a>{{/hasSexData}}');

    /**
    * The area switch.
    * @property tabSpecificOptions
    */
    this.tabSpecificOptions = null;

    /**
    * Initialises the view manager
    * @method init
    */
    this.init = function() {
        if (!isInitialised) {

            $chartBox = $('<div id="inequalities-chart-box" class="clearfix"></div>');
            $header = $('<div id="inequalities-header" class="clearfix"></div>');
            $partitionMenu = $('<div id="inequalities-partition-menu" class="clearfix"></div>');
            $container.prepend($header, $chartBox, $partitionMenu);
            this.initTabSpecificOptions();
            isInitialised = true;
        }
    };

    /**
    * Sets the HTML to display above the chart
    * @method setHeaderHtml
    */
    this.setHeaderHtml = function(html) {
        $header.html(html);
    };

    /**
    * Displays the menu of category types available for the selected indicator
    * @method displayMenu
    */
    this.displayMenu = function(categoryDataAnalyser, ageDataAnalyser, sexDataAnalyser) {

        $partitionMenu.html(templates.render("inequalitiesMenu",
        {
            types: categoryDataAnalyser.categoryTypes,
            hasAgeData: ageDataAnalyser.isAnyData,
            hasSexData: sexDataAnalyser.isAnyData
        }));

        var exportTypes =
            '<div class="export-chart-box"><a class="export-link" href="javascript:inequalities.exportSelectedChart()">Export chart as image</a>';

        if (isFeatureEnabled("inequalityErrorBars")) {
            var showHide = inequalities.state.showErrorBars
                ? 'Show values' 
                : 'Show confidence intervals';
            var errorBars = '<a id="inequalities-toggle-cis">' + showHide + "</a></div>";
        } else {
            errorBars = '';
        }

        $chartBox.html('<div id="inequalities-chart"></div><div id="inequalities-trend-box"></div>');
        $("#export-link-inequalities").remove();
        $($header)
            .after('<div id="export-link-inequalities" style="margin-top:10px;">' + exportTypes + errorBars + "</div>");

        this.bindToggleConfidenceIntervals();
    };

    /**
    * Binds the toggle CI event to the event handler
    * @method bindToggleConfidenceIntervals
    */
    this.bindToggleConfidenceIntervals = function() {
        var ns = inequalities;
        var state = ns.state;

        $("#inequalities-toggle-cis")
            .click(function(e) {
                e.preventDefault();
                state.showErrorBars = !state.showErrorBars;

                if (inequalities.trendChart) {

                    // Visibility of series
                    seriesVisible = {};
                    var serieses = inequalities.trendChart.series;
                    for (var i in serieses) {
                        var series = serieses[i];
                        if (series.name !== "Errorbars") {
                            seriesVisible[series.name] = series.visible;
                        }
                    }

                    // Checked state for trend options
                    var options = $(".trend-option input");
                    optionsChecked = _.map(options,
                        function(option) {
                            return $(option).prop(checked);
                        });
                }

                ns.determinePartitionToDisplay();
            });
    };
    this.restoreTrendSeriesVisibility = function() {
        if (seriesVisible) {
            for (var name in seriesVisible) {
                var series = getSeriesByName(name);
                setSeriesVisibility(series, "id", seriesVisible[name]);
            }
            seriesVisible = null;

            var options = $(".trend-option input");
            for (var i in options) {
                var $option = $(options[i]);
                var current = $option.prop(checked);
                if (isDefined(current)) {
                    var target = optionsChecked[i];
                    if (current !== target) {
                        var newChecked = target ? checked : "";
                        $option.prop(checked, newChecked);
                    }
                } else {
                    break;
                }
            }
            optionsChecked = null;
        }
    };

    /**
    * Exports the current chart as an image
    * @method exportChart
    */
    this.exportChart = function() {

        var selectedPartitionName = $("#inequalities-partition-menu a.selected")[0];
        if (!selectedPartitionName) {
            // No chart displayed
            return;
        }

        var root = groupRoots[getIndicatorIndex()];
        var period = getFirstGrouping(root).Period;
        var indicatorName = ui.getMetadataHash()[root.IID].Descriptive.Name + new SexAndAge().getLabel(root);
        var partition = $.parseHTML(selectedPartitionName.innerHTML)[0].data;

        chart.exportChart({ type: "image/png" },
        {
            chart: {
                spacingTop: 70,
                events: {
                    load: function() {
                        this.renderer.text("<b>" +
                                indicatorName +
                                " - " +
                                inequalities.getSelectedAreaName() +
                                ", " +
                                period +
                                " - Data partitioned by " +
                                partition +
                                "</b>",
                                350,
                                15)
                            .attr({
                                align: "center"
                            })
                            .css({
                                color: "#333",
                                fontSize: "10px",
                                width: "650px"
                            })
                            .add();
                    }
                }
            }
        });

    };

    /**
    * Hides the chart and category type menu and tells the user there is no data
    * @method displayNoData
    */
    this.displayNoData = function() {
        $chartBox.html('<p class="no-data">Inequality data is not available<br>for this indicator</p>');
        $partitionMenu.html("");
        this.tabSpecificOptions.clearHtml();
    };

    /**
    * Initialises the area switch 
    * @method initTabSpecificOptions
    */
    this.initTabSpecificOptions = function() {
        var clickHandler = inequalities.selectValuesOrTrends;
        this.tabSpecificOptions = new TabSpecificOptions({
            eventHandlers: [clickHandler, clickHandler],
            eventName: "InequalityAreaSelected"
        });
    };

    this.updateTabSpecificOptionsOptions = function() {
        this.tabSpecificOptions.setHtml({
            label: "Inequalities for",
            optionLabels: ["England", areaHash[FT.model.areaCode].Name]
        });
    };

    /**
    * Creates the bar chart
    * @method createBarChart
    */
    this.createBarChart = function(data, metadata) {

        var itemStyle = { color: "#333333", fontWeight: "normal", textShadow: "0" };
        var unitLabel = metadata.Unit.Label;
        var tooltip = new inequalities.BarChartTooltip(metadata.Unit, data.averageLegend, data.labels);

        if (data.showAverageLine) {
            var seriesData = [
                {
                    name: "data",
                    data: data.dataSeries,
                    showInLegend: false
                },
                {
                    name: data.averageLegend,
                    data: data.averageDataSeries,
                    type: "line",
                    showInLegend: true
                }
            ];
        } else {
            seriesData = [
                {
                    name: "data",
                    data: data.dataSeries,
                    showInLegend: false
                }
            ];
        }

        var errorBars = inequalities.state.showErrorBars;
        if (errorBars) {

            var errorBarData = inequalities.getBarChartCIs(data.dataSeries);
            var errorBarSeries = inequalities.getErrorBarSeries(errorBarData);
            seriesData.push(errorBarSeries);
        }

        var barWidth = new inequalities.BarThicknessCalculator(
            data.labels.length).thickness;

        var shortCategoryLabels = _.map(data.labels,
            function(label) {
                return trimName(label, 30);
            });

        chart = new Highcharts.Chart({
            chart: {
                renderTo: "inequalities-chart",
                animation: false,
                defaultSeriesType: "bar",
                width: 700,
                zoomType: "xy",
                marginRight: 20 // margin necessary for all labels to be displayed
            },
            title: {
                text: ""
            },
            xAxis: {
                categories: shortCategoryLabels,
                labels: {
                    style: itemStyle,
                    labels: {
                        step: 1
                    }
                }
            },
            yAxis: {
                title: {
                    text: unitLabel,
                    style: itemStyle
                }
            },
            plotOptions: {
                series: {
                    animation: false,
                    events: {
                        legendItemClick: function() {
                            return false;
                        }
                    }
                },
                bar: {
                    borderColor: "#333",
                    pointPadding: 0,
                    pointWidth: barWidth,
                    stacking: "normal",
                    shadow: false,
                    dataLabels: {
                        allowOverlap: true,
                        enabled: true,
                        style: itemStyle,
                        align: "right",
                        y: 1, // Ignored IE 6-8
                        x: new inequalities.BarLabelOffsetCalculator(data.dataSeries).offset + 1,
                        formatter: function() {
                            return this.y === 0 || errorBars ? "" : new CommaNumber(this.point.ValF).unrounded();
                        }
                    }
                },
                line: {
                    // The symbol is a non-valid option here to work round a bug
                    // in highcharts where only half of the markers appear on hover
                    borderWidth: 0,
                    pointPadding: 0,
                    marker: HC.noLineMarker,
                    states: {
                        hover: {
                            marker: HC.noLineMarker
                        }
                    }
                }
            },
            legend: {
                enabled: true,
                borderWidth: 0,
                layout: "vertical",
                itemStyle: itemStyle
            },
            tooltip: {
                formatter: function() {
                    return tooltip.getHtml(this);
                }
            },
            credits: HC.credits,
            series: seriesData,
            exporting: {
                enabled: false
            },
        });
    };
};

/**
* Gets the average data object for the selected area
* @class getAverage
*/
inequalities.getAverage = function(groupRoot) {
    var average, i, areaCode;

    if (inequalities.isNationalSelected()) {
        // National inequalities
        average = getNationalComparatorGrouping(groupRoot).ComparatorData;
    } else {
        // Local area inequalities
        areaCode = FT.model.areaCode;
        for (i = 0; i < groupRoot.Data.length; i++) {
            if (groupRoot.Data[i].AreaCode === areaCode) {
                average = groupRoot.Data[i];
                break;
            }
        }
    }
    return average;
};

/**
* Event handler for partition menu option
* @class selectCategoryType
*/
inequalities.selectCategoryType = function(categoryTypeId) {
    var ns = inequalities;
    var state = ns.state;

    var groupRoot = getGroupRoot();

    if (state.isViewModeTrends) {
        ns.unselectedPartitionOptions();
        ns.setCategoryTypeOptionSelected(categoryTypeId);
        ns.getTrendsByCategory(groupRoot, categoryTypeId);
        state.preferredPartition = categoryTypeId;

    } else {
        var areaCode = ns.getSelectedAreaCode();

        var categoryDataAnalyser = ns.categoryDataManager.getData(groupRoot, areaCode, FT.model.areaTypeId);

        var preferredPartition = state.preferredPartition;
        var isPreferredInequalityDefined = isDefined(preferredPartition);
        var isCategoryTypeIdDefined = isDefined(categoryTypeId);
        var defaultCategory = categoryDataAnalyser.categoryTypes[0];

        if (isCategoryTypeIdDefined) {
            // User has selected this category or else viewed it previously
            preferredPartition = categoryDataAnalyser.isCategoryTypeById(categoryTypeId)
                ? categoryTypeId
                : defaultCategory.Id;
        } else if (!isPreferredInequalityDefined) {
            // First view of page
            preferredPartition = defaultCategory.Id;
        }

        ns.setCategoryTypeOptionSelected(preferredPartition);

        // Keep state for next view
        state.preferredPartition = preferredPartition;

        // Get data to display
        var average = ns.getAverage(groupRoot);
        var data = new ns.CategoryDataBuilder(categoryDataAnalyser,
            preferredPartition,
            average,
            useRagColours()).getData();

        // Display bar chart
        var metadata = ui.getMetadataHash()[groupRoot.IID];
        ns.viewManager.createBarChart(data, metadata);
    }
};

/**
* Whether the national area (instead of local) is currently selected
* @class isNationalSelected
*/
inequalities.isNationalSelected = function() {
    return inequalities.viewManager.tabSpecificOptions.getOption() === inequalities.AreaOptions.NATIONAL;
};

/**
* Gets the name of the currently selected area 
* @class getSelectedAreaName
*/
inequalities.getSelectedAreaName = function() {
    return inequalities.isNationalSelected()
        ? "England"
        : areaHash[FT.model.areaCode].Name;
};

/**
* Gets the currently selected area code
* @class getSelectedAreaCode
*/
inequalities.getSelectedAreaCode = function() {
    return inequalities.isNationalSelected()
        ? NATIONAL_CODE
        : FT.model.areaCode;
};

/**
* Exports the displayed chart
* @class exportSelectedChart
*/
inequalities.unselectedPartitionOptions = function() {
    $("#inequalities-partition-menu").find("*").removeAttr("class");
};

/**
* Exports the displayed chart
* @class exportSelectedChart
*/
inequalities.exportSelectedChart = function() {
    inequalities.state.isViewModeValues ? inequalities.viewManager.exportChart() : inequalities.exportTrendChart();
};

/**
* Gets the link element of the selected partition
* @class getSelectedPartition
*/
inequalities.getSelectedPartition = function() {
    return $("#inequalities-partition-menu a.selected");
};

/**
* Export trend chart to an image
* @class inequalities.exportTrendChart
*/
inequalities.exportTrendChart = function() {

    var ns = inequalities;

    var root = groupRoots[getIndicatorIndex()];
    var indicatorName = ui.getMetadataHash()[root.IID].Descriptive.Name + new SexAndAge().getLabel(root);
    var $option = ns.getSelectedPartition();
    var partition = $.parseHTML($option[0].innerHTML)[0].data;

    ns.trendChart.exportChart({ type: "image/png" },
    {
        chart: {
            spacingTop: 70,
            events: {
                load: function() {
                    this.renderer.text("<b>" +
                            indicatorName +
                            " - " +
                            ns.getSelectedAreaName() +
                            ", " +
                            " - Data partitioned by " +
                            partition +
                            "</b>",
                            350,
                            15)
                        .attr({
                            align: "center"
                        })
                        .css({
                            color: "#333",
                            fontSize: "7px",
                            width: "450px"
                        })
                        .add();
                }
            }
        }
    });
};

/**
* Whether or not to use RAG colours
* @class useRagColours
*/
function useRagColours() {
    var groupRoot = getGroupRoot();
    var metadata = ui.getMetadataHash()[groupRoot.IID];
    return new ComparisonConfig(groupRoot, metadata).useRagColours;
}


inequalities.AreaOptions = {
    NATIONAL: 0,
    LOCAL: 1
};

inequalities.PartitionsTypes = {
    BySex: "bySex",
    ByAge: "byAge"
}

inequalities.state = {
    isViewModeTrends: false,
    isViewModeValues: true,
    trendData: {},
    preferredPartition: null,
    showErrorBars: false
};

pages.add(PAGE_MODES.INEQUALITIES,
{
    id: "inequalities",
    title: "Inequalities",
    icon: "inequalities",
    'goto': goToInequalitiesPage,
    gotoName: "goToInequalitiesPage",
    needsContainer: true,
    jqIds: [".geo-menu", "indicator-menu-div", "tab-specific-options", "nearest-neighbour-link"],
    jqIdsNotInitiallyShown: ["key-ad-hoc", "key-bar-chart", "inequalities-trend-box"]
});