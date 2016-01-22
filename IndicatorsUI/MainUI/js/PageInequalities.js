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

    if (!groupRoots.length) {
        // Search results empty
        noDataForAreaType();
    } else {
        lock();

        setPageMode(PAGE_MODES.CONTENT);
        var ns = inequalities;
        ns.init();

        inequalities.getDataForSelectedArea();
    }
}

/**
* Initialises the inequalities page. Only the first call has an effect
* @class inequalities.init
*/
inequalities.init = function () {
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
* Get all data for selected area by AJAX()
* @class inequalities.getDataForSelectedArea
*/
inequalities.getDataForSelectedArea = function () {

    var model = FT.model;
    var groupRoot = getGroupRoot();

    ajaxMonitor.setCalls(3);

    inequalities.getDataForAllCategories(model, groupRoot);
    inequalities.getDataForAllAges(model, groupRoot);
    inequalities.getDataForAllSexes(model, groupRoot);

    ajaxMonitor.monitor(inequalities.displayPage);
};

/**
* Get category data by AJAX
* @class inequalities.getDataForAllCategories
*/
inequalities.getDataForAllCategories = function (model, groupRoot) {

    var ns = inequalities;
    var dataManager = ns.categoryDataManager;

    // Area to get data for
    var areaCode = ns.getSelectedAreaCode();

    if (dataManager.getData(groupRoot, areaCode, model.areaTypeId)) {
        // Data already loaded
        ajaxMonitor.callCompleted();
    } else {
        // Load data
        var parameters = new ParameterBuilder(
        ).add('profile_id', model.profileId
        ).add('area_code', areaCode
        ).add('indicator_id', groupRoot.IID
        ).add('area_type_id', model.areaTypeId
        ).add('sex_id', groupRoot.SexId
        ).add('age_id', groupRoot.AgeId);

        ajaxGet('data/most_recent_data_for_all_categories',
            parameters.build(),
            function (obj) {
                dataManager.setData(groupRoot, areaCode, model.areaTypeId,
                    new ns.CategoryDataAnalyser(obj));
                ajaxMonitor.callCompleted();
            });
    }
};

/**
* Get data by AJAX
* @class inequalities.getDataForAllSexes
*/
inequalities.getDataForAllSexes = function (model, groupRoot) {

    var ns = inequalities;
    var dataManager = ns.sexDataManager;

    // Area to get data for
    var areaCode = ns.getSelectedAreaCode();

    if (dataManager.getData(groupRoot, areaCode, model.areaTypeId)) {
        // Data already loaded
        ajaxMonitor.callCompleted();
    } else {
        // Load data
        var parameters = new ParameterBuilder(
        ).add('profile_id', model.profileId
        ).add('area_code', areaCode
        ).add('indicator_id', groupRoot.IID
        ).add('area_type_id', model.areaTypeId
        ).add('age_id', groupRoot.AgeId);

        ajaxGet('data/most_recent_data_for_all_sexes',
            parameters.build(),
            function (obj) {
                dataManager.setData(groupRoot, areaCode, model.areaTypeId,
                    new ns.SexDataAnalyser(obj));
                ajaxMonitor.callCompleted();
            });
    }
};

/**
* Get data by AJAX
* @class inequalities.getDataForAllAges
*/
inequalities.getDataForAllAges = function (model, groupRoot) {

    var ns = inequalities;
    var dataManager = ns.ageDataManager;

    // Area to get data for
    var areaCode = ns.getSelectedAreaCode();

    if (dataManager.getData(groupRoot, areaCode, model.areaTypeId)) {
        // Data already loaded
        ajaxMonitor.callCompleted();
    } else {
        // Load data
        var parameters = new ParameterBuilder(
        ).add('profile_id', model.profileId
        ).add('area_code', areaCode
        ).add('indicator_id', groupRoot.IID
        ).add('area_type_id', model.areaTypeId
        ).add('sex_id', groupRoot.SexId);

        ajaxGet('data/most_recent_data_for_all_ages',
            parameters.build(),
            function (obj) {
                dataManager.setData(groupRoot, areaCode, model.areaTypeId,
                    new ns.AgeDataAnalyser(obj));
                ajaxMonitor.callCompleted();
            });
    }
};

inequalities.preferredInequality = null;

/**
* Displays the inequalities page with the bar chart
* @class inequalities.displayPage
*/
inequalities.displayPage = function () {

    var areaCode = inequalities.getSelectedAreaCode();
    var areaTypeId = FT.model.areaTypeId;
    var groupRoot = getGroupRoot();
    var viewManager = inequalities.viewManager;

    var categoryDataAnalyser = inequalities.categoryDataManager.getData(groupRoot, areaCode, areaTypeId);
    var ageDataAnalyser = inequalities.ageDataManager.getData(groupRoot, areaCode, areaTypeId);
    var sexDataAnalyser = inequalities.sexDataManager.getData(groupRoot, areaCode, areaTypeId);

    var isCategoryData = categoryDataAnalyser.isAnyData;
    var isAgeData = ageDataAnalyser.isAnyData;
    var isSexData = sexDataAnalyser.isAnyData;

    // Define default category in case needed 
    if (isCategoryData) {
        var defaultCategoryTypeId = categoryDataAnalyser.categoryTypes[0].Id;
    }

    if (isCategoryData || isAgeData || isSexData) {
        viewManager.displayMenu(categoryDataAnalyser, ageDataAnalyser, sexDataAnalyser);
        var preferredInequality = inequalities.preferredInequality;
        if (isDefined(preferredInequality)) {
            // Use last thing the user looked at
            switch (preferredInequality) {
                case 'bySex':
                    if (isSexData) {
                        inequalities.bySex();
                    } else {
                        inequalities.preferredInequality = null;
                        inequalities.selectCategoryType(defaultCategoryTypeId);
                    }
                    break;
                case 'byAge':
                    if (isAgeData) {
                        inequalities.byAge();
                    } else {
                        inequalities.preferredInequality = null;
                        inequalities.selectCategoryType(defaultCategoryTypeId);
                    }
                    break;
                default:
                    if (isSexData && !isCategoryData) {
                        inequalities.bySex();
                    } else if (isAgeData && !isCategoryData) {
                        inequalities.byAge();
                    } else {
                        inequalities.selectCategoryType(preferredInequality);
                    }
                    break;
            }
        } else {
            // Nothing has been looked at yet
            if (isCategoryData) {
                inequalities.selectCategoryType(defaultCategoryTypeId);
            } else if (isAgeData) {
                inequalities.byAge();
            } else if (isSexData) {
                inequalities.bySex();
            }
        } 
    } else {
        viewManager.displayNoData();
    }

    viewManager.updateAreaSwitchOptions();

    var indicatorIndex = getIndicatorIndex();
    var metadata = ui.getMetadataHash()[groupRoot.IID];
    var period = getFirstGrouping(groupRoot).Period;
    var areaLabel = inequalities.getSelectedAreaName() + ', ' + period + '';

    // Header
    var html = getTrendHeader(metadata, groupRoot, areaLabel, 'goToMetadataPage(' + indicatorIndex + ')');
    viewManager.setHeaderHtml(html);

    var comparisonConfig = new ComparisonConfig(groupRoot, metadata);
    setTargetLegendHtml(comparisonConfig, metadata);

    showAndHidePageElements();

    showBarChartLegend(comparisonConfig.useTarget);

    $('#benchmark-box').hide();

    unlock();
};

/**
* Wrapper for response object from the recent data for all categories service
* @class inequalities.CategoryDataAnalyser
*/
inequalities.CategoryDataAnalyser = function (data) {

    var categoryData = data.Data;
    var categoryTypes = data.CategoryTypes;
    var categoryTypeIdToData = {};
    var categoryTypesWithData = [];
    var i;

    // Filter out invalid values
    categoryData = _.filter(categoryData, function (d) { return d.Val !== -1; });

    for (i in categoryTypes) {
        var categoryType = categoryTypes[i];
        var dataOfType = _.filter(categoryData,
            function (d) { return d.CategoryTypeId === categoryType.Id; });

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
    this.getDataByCategoryTypeId = function (categoryTypeId) {
        return categoryTypeIdToData[categoryTypeId];
    };

    /**
    * Get a category type by its ID
    * @method getCategoryTypeById
    */
    this.getCategoryTypeById = function (categoryTypeId) {
        return _.filter(categoryTypesWithData,
            function (type) { return type.Id === categoryTypeId; })[0];
    };

    /**
    * Get a category type by its ID
    * @method getCategoryTypeById
    */
    this.isCategoryTypeById = function (categoryTypeId) {
        return _.some(categoryTypesWithData,
            function (type) { return type.Id === categoryTypeId; });
    };

    /**
	* Get the labels for each category of the specified category type ID
	* @method getCategoryLabels
	*/
    this.getCategoryLabels = function (categoryTypeId) {
        var categories = this.getCategoryTypeById(categoryTypeId).Categories;
        return _.pluck(categories, 'Name');
    };

    /**
	* Set the category type to be selected
	* @method setSelectedCategoryType
	*/
    this.setSelectedCategoryTypeInMenu = function (categoryTypeId) {
        inequalities.unselectedPartitionOptions();
        $('#' + categoryTypeId).addClass('selected');
    };
}

/**
* Build data for categories
* @class inequalities.CategoryDataBuilder
*/
inequalities.CategoryDataBuilder = function (categoryDataAnalyser, categoryTypeId, average, useRagColours) {

    categoryTypeId = !!categoryTypeId ? categoryTypeId : categoryDataAnalyser.categoryTypes[0].Id;

    var categoryType = categoryDataAnalyser.getCategoryTypeById(categoryTypeId);
    var dataList = categoryDataAnalyser.getDataByCategoryTypeId(categoryTypeId);

    var categoryIdToData = _.indexBy(dataList, 'CategoryId');
    var categories = categoryType.Categories;

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
        var categoryId = categories[i].CategoryId;
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
    this.getData = function () {
        var builderData = {};
        builderData.labels = categoryDataAnalyser.getCategoryLabels(categoryTypeId);
        builderData.dataSeries = dataSeries;
        builderData.averageDataSeries = averageSeries;
        builderData.showAverageLine = true;
        builderData.averageLegend = inequalities.getSelectedAreaName() + ' average';
        return builderData;
    };
}

/**
* Wrapper for response object from the recent data for all sexes service
* @class inequalities.SexDataAnalyser
*/
inequalities.SexDataAnalyser = function (data) {

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
}



/**
* Build data for Sex
* @class inequalities.SexDataBuilder
*/
inequalities.SexDataBuilder = function (sexDataAnalyser) {
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
    this.getData = function () {

        var builderData = {};
        builderData.labels = _.pluck(sexes, 'Name');
        builderData.dataSeries = dataSeries;
        builderData.averageDataSeries = averageSeries;
        builderData.showAverageLine = personExist;
        builderData.averageLegend = inequalities.getSelectedAreaName() + ' persons';
        return builderData;
    };
};

/**
* Wrapper for response object from the recent data for all ages service
* @class inequalities.AgeDataAnalyser
*/
inequalities.AgeDataAnalyser = function (data) {

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
}


/**
* Build data for Ages
* @class inequalities.AgeDataBuilder
*/
inequalities.AgeDataBuilder = function (ageDataAnalyser) {
    var average = {};
    var ageData = ageDataAnalyser.ageData;
    var dataSeries = [];
    var averageSeries = [];
    var indicatorAgeId = getGroupRoot().AgeId;

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

    var avgLabel = inequalities.getAgeLabels(ageDataAnalyser, getGroupRoot().AgeId);
    //    var viewManager = inequalities.viewManager;

    /**
    * Get age data
    * @method getData
    */
    this.getData = function () {
        var builderData = {};
        builderData.labels = _.pluck(ageDataAnalyser.ages, 'Name');
        builderData.dataSeries = dataSeries;
        builderData.averageDataSeries = averageSeries;
        builderData.showAverageLine = true;
        builderData.averageLegend = inequalities.getSelectedAreaName() + ' ' + avgLabel;
        return builderData;
    };
}

/**
* Get age average series data
* @method inequalities.getAgeAverageData
*/
inequalities.getIndicatorAgeRange = function (ageData, average) {
    var ageDataCopy = _.clone(ageData);
    var groupRoot = getGroupRoot();
    for (var i in ageDataCopy) {
        if (groupRoot.AgeId === ageDataCopy[i].AgeId) {
            average.Val = ageDataCopy[i].Val;
            average.ValF = ageDataCopy[i].ValF;
        }
    }
    return average;
}


var indicatorAgeLabel = '';
// remove object from array which match age range and return age range text
inequalities.getAgeLabels = function (ageDataAnalyser, indicatorAgeId) {
    var ages = ageDataAnalyser.ages;
    for (var i in ages) {
        // remove the object which match the indicator Age Range
        if (ages[i].Id === indicatorAgeId) {
            indicatorAgeLabel = ages[i].Name;
            ages.splice(i, 1);
        }
    }
    return indicatorAgeLabel;
};

/**
* Event handler for partition menu option
* @class inequalities.byAge
*/
inequalities.byAge = function () {

    inequalities.unselectedPartitionOptions();
    $('#byAge').addClass('selected');
    var groupRoot = getGroupRoot();
    var metadata = ui.getMetadataHash()[groupRoot.IID];
    var areaCode = inequalities.getSelectedAreaCode();
    var ageDataAnalyser = inequalities.ageDataManager.getData(groupRoot, areaCode, FT.model.areaTypeId);
    var data = new inequalities.AgeDataBuilder(ageDataAnalyser).getData();
    inequalities.viewManager.createBarChart(data, metadata);
    inequalities.preferredInequality = "byAge";
};

/**
* Event handler for partition menu option
* @class inequalities.bySex
*/
inequalities.bySex = function () {
    inequalities.unselectedPartitionOptions();
    $('#bySex').addClass('selected');
    var groupRoot = getGroupRoot();
    var metadata = ui.getMetadataHash()[groupRoot.IID];
    var areaCode = inequalities.getSelectedAreaCode();
    var sexDataAnalyser = inequalities.sexDataManager.getData(groupRoot, areaCode, FT.model.areaTypeId);
    var data = new inequalities.SexDataBuilder(sexDataAnalyser).getData();
    inequalities.viewManager.createBarChart(data, metadata);
    inequalities.preferredInequality = 'bySex';
};

/**
* Stores and provides the category data retrieved by AJAX.
* @class inequalities.CategoryDataManager
*/
inequalities.CategoryDataManager = function () {

    this._data = {};

    this._getDataKey = function (groupRoot, areaCode, areaTypeId) {
        return getKey(groupRoot.IID, groupRoot.SexId, groupRoot.AgeId, areaTypeId, areaCode);
    }
}

inequalities.CategoryDataManager.prototype = {

    /**
	* Gets complex data object that was retrieved by AJAX
	* @method getData
	*/
    getData: function (groupRoot, areaCode, areaTypeId) {
        var key = this._getDataKey(groupRoot, areaCode, areaTypeId);
        return this._data[key];
    },

    /**
    * Adds data object that was retrieved by AJAX to the manager
    * @method setData
    */
    setData: function (groupRoot, areaCode, areaTypeId, categoryData) {
        var key = this._getDataKey(groupRoot, areaCode, areaTypeId);
        this._data[key] = categoryData;
    }
}

inequalities.dataManagerPrototype = inequalities.CategoryDataManager.prototype;

/**
* Stores and provides the category data retrieved by AJAX.
* @class inequalities.CategoryDataManager
*/
inequalities.SexDataManager = function () {

    this._data = {};

    this._getDataKey = function (groupRoot, areaCode, areaTypeId) {
        return getKey(groupRoot.IID, groupRoot.AgeId, areaCode, areaTypeId);
    }
}

inequalities.SexDataManager.prototype.getData = inequalities.dataManagerPrototype.getData;

inequalities.SexDataManager.prototype.setData = inequalities.dataManagerPrototype.setData;

/**
* Stores and provides the category data retrieved by AJAX.
* @class inequalities.CategoryDataManager
*/
inequalities.AgeDataManager = function () {

    this._data = {};

    this._getDataKey = function (groupRoot, areaCode, areaTypeId) {
        return getKey(groupRoot.IID, groupRoot.SexId, areaCode, areaTypeId);
    };
}

inequalities.AgeDataManager.prototype.getData = inequalities.dataManagerPrototype.getData;

inequalities.AgeDataManager.prototype.setData = inequalities.dataManagerPrototype.setData;


/**
* Determines how much offset should be used for the bar labels.
* @class inequalities.BarLabelOffsetCalculator
*/
inequalities.BarLabelOffsetCalculator = function (dataList) {

    var maxData = _.max(dataList, function (d) { return d.Val; });

    // e.g. 22.2
    var offset = 29;

    if (new CoreDataSetInfo(maxData).isValue()) {
        var valF = new CommaNumber(maxData.ValF).unrounded();
        var len = valF.length;
        if (len === 5) {
            offset = 36;
        }
        if (len === 3) {
            if (valF.indexOf('.') > -1) {
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
* @class inequalities.BarThicknessCalculator
*/
inequalities.BarThicknessCalculator = function (numberOfBars) {

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
* @class inequalities.BarChartTooltip
*/
inequalities.BarChartTooltip = function (unit, averageLabel, categoryLabels) {
    /**
    * Gets the tooltip HTML
    * @method getHtml
    */
    this.getHtml = function (obj) {

        var isAverageSeries = obj.series.index === 1;
        var point = obj.point;

        var category = isAverageSeries
            ? averageLabel
            : categoryLabels[point.index]; // use these non-truncated labels

        return '<b>' +
            new ValueWithUnit(unit).getFullLabel(point.ValF) + '<b><br><i>' +
            category + '</i>';
    }
};

/**
* Manages the views on the inequalities page
* @class inequalities.ViewManager
*/
inequalities.ViewManager = function ($container) {

    var isInitialised = false,
        $chartBox, $header, $partitionMenu, chart;

    templates.add('inequalitiesMenu',
        '<p>Partition data by:</p>{{#types}}<a id="{{Id}}" {{#isSelected}}class="selected" {{/isSelected}}href="javascript:inequalities.selectCategoryType({{Id}})">{{Name}}</a>{{/types}}' +
        '{{#hasAgeData}}<a id="byAge" {{#isSelected}}class="selected"{{/isSelected}} href="javascript:javascript:inequalities.byAge()">Age</a>{{/hasAgeData}}' +
        '{{#hasSexData}}<a id="bySex" {{#isSelected}}class="selected"{{/isSelected}} href="javascript:javascript:inequalities.bySex()">Sex</a>{{/hasSexData}}');


    /**
    * The area switch.
    * @property areaSwitch
    */
    this.areaSwitch = null;

    /**
	* Initialises the view manager
	* @method init
	*/
    this.init = function () {
        if (!isInitialised) {

            $chartBox = $('<div id="inequalities-chart-box" class="clearfix"></div>');
            $header = $('<div id="inequalities-header" class="clearfix"></div>');
            $partitionMenu = $('<div id="inequalities-partition-menu" class="clearfix"></div>');
            $container.prepend($header, $chartBox, $partitionMenu);
            this.initAreaSwitch();
            isInitialised = true;
        }
    };

    /**
	* Sets the HTML to display above the chart
	* @method setHeaderHtml
	*/
    this.setHeaderHtml = function (html) {
        var exportTypes = '<div class="export-chart-box"><a class="export-link" href="javascript:inequalities.viewManager.exportChart()">Export chart as image</a></div>';
        $header.html(exportTypes + html);
    };

    /**
	* Displays the menu of category types available for the selected indicator
	* @method displayMenu
	*/
    this.displayMenu = function (categoryDataAnalyser, ageDataAnalyser, sexDataAnalyser) {
        $partitionMenu.html(templates.render('inequalitiesMenu', { types: categoryDataAnalyser.categoryTypes, hasAgeData: ageDataAnalyser.isAnyData, hasSexData: sexDataAnalyser.isAnyData }));
        var exportTypes = '<div class="export-chart-box"><a class="export-link" href="javascript:inequalities.viewManager.exportChart()">Export chart as image</a></div>';

        $chartBox.html('<div id="inequalities-chart"></div>');
    };

    this.exportChart = function () {

        var root = groupRoots[getIndicatorIndex()];
        var period = getFirstGrouping(root).Period;
        var indicatorName = ui.getMetadataHash()[root.IID].Descriptive.Name + new SexAndAge().getLabel(root);
        var partition = $.parseHTML($('#inequalities-partition-menu a.selected')[0].innerHTML)[0].data;

        chart.exportChart({ type: 'image/png' }, {
            chart: {
                spacingTop: 70,
                events: {
                    load: function () {
                        this.renderer.text('<b>' + indicatorName + ' - ' +
                            inequalities.getSelectedAreaName() + ', ' + period +
                            ' - Data partitioned by ' + partition + '</b>',
                            350, 15)
                            .attr({
                                align: 'center'
                            })
                            .css({
                                color: '#333',
                                fontSize: '10px',
                                width: '650px'
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
    this.displayNoData = function () {
        $chartBox.html('<p>Inequality data is not available<br>for this indicator</p>');
        $partitionMenu.html('');
        this.areaSwitch.clearHtml();
    };

    /**
    * Initialises the area switch 
    * @method initAreaSwitch
    */
    this.initAreaSwitch = function () {
        var func = inequalities.getDataForSelectedArea;
        this.areaSwitch = new AreaSwitch({
            eventHanders: [func, func],
            eventName: 'InequalityAreaSelected'
        });
    };

    this.updateAreaSwitchOptions = function () {
        this.areaSwitch.setHtml({
            label: 'Inequalities for',
            topOptionText: 'England',
            bottomOptionText: areaHash[FT.model.areaCode].Name
        });
    };

    /**
    * Creates the bar chart
    * @method createBarChart
    */
    this.createBarChart = function (data, metadata) {

        var itemStyle = { color: '#333333', fontWeight: 'normal', textShadow: '0' };
        var unitLabel = metadata.Unit.Label;
        var tooltip = new inequalities.BarChartTooltip(metadata.Unit, data.averageLegend, data.labels);

        if (data.showAverageLine) {
            var seriesData = [
                {
                    name: 'data',
                    data: data.dataSeries,
                    showInLegend: false
                },
                {
                    name: data.averageLegend,
                    data: data.averageDataSeries,
                    type: 'line',
                    showInLegend: true
                }
            ];
        } else {
            seriesData = [
                {
                    name: 'data',
                    data: data.dataSeries,
                    showInLegend: false
                }
            ];
        }

        var barWidth = new inequalities.BarThicknessCalculator(
            data.labels.length).thickness;

        var shortCategoryLabels = _.map(data.labels, function (label) {
            return trimName(label, 30);
        });

        chart = new Highcharts.Chart({
            chart: {
                renderTo: 'inequalities-chart',
                animation: false,
                defaultSeriesType: 'bar',
                width: 700,
                marginRight: 20 // margin necessary for all labels to be displayed
            },
            title: {
                text: ''
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
                        legendItemClick: function () {
                            return false;
                        }
                    }
                },
                bar: {
                    borderWidth: 0,
                    pointPadding: 0,
                    pointWidth: barWidth,
                    stacking: 'normal',
                    shadow: false,
                    dataLabels: {
                        allowOverlap: true,
                        enabled: true,
                        style: itemStyle,
                        align: 'right',
                        y: 1, // Ignored IE 6-8
                        x: new inequalities.BarLabelOffsetCalculator(data.dataSeries).offset,
                        formatter: function () {
                            return this.y === 0 ?
                                '' :
                                new CommaNumber(this.point.ValF).unrounded();
                        }
                    }
                },
                line: {
                    // The symbol is a non-valid option here to work round a bug
                    // in highcharts where only half of the markers appear on hover
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
                layout: 'vertical',
                itemStyle: itemStyle
            },
            tooltip: {
                formatter: function () {
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

inequalities.getAverage = function (groupRoot) {
    if (inequalities.isNationalSelected()) {
        // National inequalities
        average = getNationalComparatorGrouping(groupRoot).ComparatorData;
    } else {
        // Local area inequalities
        var areaCode = FT.model.areaCode;
        for (var i = 0; i < groupRoot.Data.length; i++) {
            if (groupRoot.Data[i].AreaCode === areaCode) {
                var average = groupRoot.Data[i];
                break;
            }
        }
    }
    return average;
}

/**
* Event handler for partition menu option
* @class inequalities.selectCategoryType
*/
inequalities.selectCategoryType = function (categoryTypeId) {
    var groupRoot = getGroupRoot();
    var areaCode = inequalities.getSelectedAreaCode();

    var categoryDataAnalyser = inequalities.categoryDataManager.getData(groupRoot, areaCode, FT.model.areaTypeId);

    var preferredInequality = inequalities.preferredInequality;
    var isPreferredInequalityDefined = isDefined(preferredInequality);
    var isCategoryTypeIdDefined = isDefined(categoryTypeId);
    var defaultCategory = categoryDataAnalyser.categoryTypes[0];

    if (isCategoryTypeIdDefined) {
        // User has selected this category or else viewed it previously
        preferredInequality = categoryDataAnalyser.isCategoryTypeById(categoryTypeId)
            ? categoryTypeId
            : defaultCategory.Id;
    }
    else if (!isPreferredInequalityDefined) {
        // First view of page
        preferredInequality = defaultCategory.Id;
    }

    categoryDataAnalyser.setSelectedCategoryTypeInMenu(preferredInequality);

    // Keep state for next view
    inequalities.preferredInequality = preferredInequality;

    // Get data to display
    var average = inequalities.getAverage(groupRoot);
    var data = new inequalities.CategoryDataBuilder(categoryDataAnalyser,
        preferredInequality, average, useRagColours()).getData();

    // Display bar chart
    var metadata = ui.getMetadataHash()[groupRoot.IID];
    inequalities.viewManager.createBarChart(data, metadata);
};

inequalities.isPartitionAvailable = function (partitionOption) {
    var ids = [];
    $('#inequalities-partition-menu').find('a').each(function () { ids.push(this.id); });
    return _.contains(ids, partitionOption.toString());
}

inequalities.isNationalSelected = function () {
    return inequalities.viewManager.areaSwitch.getOption() === inequalities.AreaOptions.NATIONAL;
};

inequalities.getSelectedAreaName = function () {
    return inequalities.isNationalSelected()
        ? 'England'
        : areaHash[FT.model.areaCode].Name;
};

inequalities.getSelectedAreaCode = function () {
    return inequalities.isNationalSelected()
        ? NATIONAL_CODE
        : FT.model.areaCode;
};

inequalities.AreaOptions = {
    NATIONAL: 0,
    LOCAL: 1
};

inequalities.unselectedPartitionOptions = function () {
    $('#inequalities-partition-menu').find('*').removeAttr('class');
};

function useRagColours() {
    var groupRoot = getGroupRoot();
    var metadata = ui.getMetadataHash()[groupRoot.IID];
    return new ComparisonConfig(groupRoot, metadata).useRagColours;
}

pages.add(PAGE_MODES.CONTENT, {
    id: 'content',
    title: 'Inequalities',
    icon: 'inequalities',
    'goto': goToInequalitiesPage,
    gotoName: 'goToInequalitiesPage',
    needsContainer: false,
    jqIds: ['content', '.geo-menu', 'indicatorMenuDiv', 'areaSwitch', 'nearest-neighbour-link'],
    jqIdsNotInitiallyShown: ['keyAdHoc', 'key-bar-chart']
});