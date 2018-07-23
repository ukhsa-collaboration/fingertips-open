'use strict';

/**
* SiteKeyIndicators.js
* @module SiteKeyIndicators
*/

function IndicatorFormatter(groupRoot, metadata, coreDataSet, indicatorStatsF) {

    this.groupRoot = groupRoot;
    this.stats = indicatorStatsF;

    // Private variables
    var dataInfo = new CoreDataSetInfo(coreDataSet),
		unit = !!metadata ? metadata.Unit : null,
		valueAndUnit = new ValueWithUnit(unit),
		isInverted = correctForPolarity && groupRoot.PolarityId === 0;

    this.getIndicatorName = function () {
        return metadata.Descriptive.Name + new SexAndAge().getLabel(groupRoot);
    };

    this.getIndicatorNameLong = function () {
        return metadata.Descriptive.NameLong;
    };

    this.getAreaCount = function () {
        return formatCount(dataInfo);
    };

    this.getAreaValue = function () {
        return new ValueDisplayer(unit).byDataInfo(dataInfo);
    };

    this.getVal = function (data, key) {
        if (!data || data[key] === '-') {
            return NO_DATA;
        }

        return valueAndUnit.getShortLabel(data[key]);
    };

    this.getSuffixIfNoShort = function () {
        return new ValueSuffix(unit).getFullLabelIfNoShort();
    };

    this.getArea = function () {
        return areaHash[coreDataSet.AreaCode];
    };

    this.getAverage = function () {
        return this.getVal(this.averageData, 'ValF');
    };

    this.getMin = function () {
        return this.getVal(this.stats,
			(isInverted ? 'Max' : 'Min')
		);
    };

    this.getMax = function () {
        return this.getVal(this.stats,
			(isInverted ? 'Min' : 'Max')
		);
    };

    this.get25 = function () {
        return this.getVal(this.stats,
			(isInverted ? 'P75' : 'P25')
		);
    };

    this.get75 = function () {
        return this.getVal(this.stats,
			(isInverted ? 'P25' : 'P75')
		);
    };

    this.getDataQuality = function () {
        return metadata.Descriptive.DataQuality;
    };
}

function addTd(html, text, cssClass, tooltip, noteId, id) {


    var isNote = !!noteId;

    if (id) {
        html.push('<td ', 'id="', id, '"');
    } else {
        html.push('<td ');
    }

    if (cssClass) {
        html.push(' class="', isNote ? cssClass + ' valueNote' : cssClass, '"');
    }

    var isBootstrapTooltip = isDefined(cssClass) &&
    cssClass.indexOf('boot-tooltip') !== -1;
    if (isBootstrapTooltip) {
        html.push(' data-toggle="tooltip" data-placement="top" ');
    }

    if (tooltip) {
        html.push(' title="', tooltip, '"');
    }

    if (isNote) {
        html.push(' id=ft_', nextUniqueId++, ' vn="', noteId, '"');
    }

    html.push('>', text, '</td>');
}

function getGroupingData() {

    var key = getGroupAndCurrentAreaTypeKey();
    var model = FT.model;

    if (!isDefined(key) || key.length === 0) {
        // No data available
        getGroupingDataCallback(null);
    } else {

        var parentAreaCode = model.parentCode;
        if (ui.isDataLoaded(key, parentAreaCode)) {
            // Data already loaded
            getGroupingDataCallback(ui.getData(key, parentAreaCode));
        } else {
            // Data not yet loaded
            lock();

            showSpinner();

            var parameters = new ParameterBuilder().add('group_id', model.groupId);
            addGroupDataParameters(parameters);
            addProfileOrIndicatorsParameters(parameters);

            ajaxGet('api/latest_data/all_indicators_in_profile_group_for_child_areas', parameters.build(),
                getGroupingDataCallback, handleAjaxFailure);
        }
    }
}

function getGroupingDataCallback(obj) {

    if (isDefined(obj)) {

        addLoadedData(obj);
        try {
            groupRoots = obj;
        } catch (e) {
            handleAjaxFailure(e);
            return;
        }
    } else {
        // No data available
        groupRoots = [];
    }

    ajaxMonitor.callCompleted();
}

function getDataFromAreaCode(data, areaCode) {

    for (var i in data) {
        if (data[i].AreaCode === areaCode) {
            return data[i];
        }
    }

    return null;
}

function showAndHidePageElements() {

    var main = $(MAIN),
		menus = FT.menus;

    main.hide();

    pages.displayElements();

    menus.parentType.setVisibility();
    menus.areaType.setVisibility();
    menus.parent.setVisibility();
    menus.benchmark.setSubnationalOptionVisibility();

    // Specific UI updates to do everytime
    showMessageIfIndicatorNotAvailableForNewAreaType();
    updateNearestNeighbourElements();
    updatePreferredState();
    showHideAreaAddress();
    $('#no-domain-data').hide();

    hideSpinner();
    main.show();
}

function showMessageIfIndicatorNotAvailableForNewAreaType() {
    var root = groupRootBeforeAreaTypeChange;
    if (root) {
        var previousIID = groupRootBeforeAreaTypeChange.IID;
        var currentIID = getSelectedIndicatorId();

        // Is the previous indicator available for the current area type
        var isNotAvailableForCurrentAreaType = _.some(groupRoots, function (root) {
            return root.IID === previousIID;
        });

        // Should the message be shown for the current tab
        var mode = pages.getCurrent();
        var modes = PAGE_MODES;
        var shouldShowMessageForCurrentTab = mode !== modes.TARTAN &&
            mode !== modes.AREA_SPINE && mode !== modes.DOWNLOAD &&
            mode !== modes.POPULATION && mode !== modes.ENGLAND;

        if (currentIID !== previousIID && !isNotAvailableForCurrentAreaType && shouldShowMessageForCurrentTab) {

            // Show message that previous indicator is not available
            var popupWidth = 600;
            var left = lightbox.getLeftForCenteredPopup(popupWidth);
            var top = 300;

            var indicatorName = ui.getMetadataHash()[previousIID].Descriptive.Name;
            var html = '<div id="indicator-not-in-area" ><b>' + indicatorName +
				'</b> is not available for ' + FT.menus.areaType.getName() + '</div>' +
				'<input type="button" id="indicator-not-in-area-ok" value="OK" onclick="lightbox.hide();" >';

            lightbox.show(html, top, left, popupWidth);
        }

        groupRootBeforeAreaTypeChange = null;
    }
}

function updateNearestNeighbourElements() {

    setNearestNeighbourLinkText();
    showAndHideNearestNeighboursMenu();

    var $showForNearestNeighbours = $('.nearest-neighbours-show');
    if (FT.model.isNearestNeighbours()) {
        toggleNearestNeighboursControls(false);
        $('#comparator').val(NATIONAL_COMPARATOR_ID);
        $showForNearestNeighbours.show();
    } else {
        $showForNearestNeighbours.hide();
    }
}

/**
* Serialises a simple hash to a string. e.g. {a:1,b:2} -> "a:1,b:2" 
* @class HashSerialiser
*/
function HashSerialiser() {

    this.deserialise = function (serialisedState) {
        var hash = {};
        if (isDefined(serialisedState) && serialisedState !== '') {
            var pairs = serialisedState.split(',');
            for (var i in pairs) {
                var keyValue = pairs[i].split(':');
                hash[keyValue[0]] = keyValue[1];
            }
        }
        return hash;
    };

    this.serialise = function (hash) {
        var a = [];
        for (var key in hash) {
            a.push(key + ':' + hash[key]);
        }
        return a.join(',');
    };
}

/**
* Manages the preferred area for each area type. For each area type this is the last area the
* user has viewed.
* @class PreferredAreas
*/
function PreferredAreas(serialisedState, model) {

    var preferredAreas = new HashSerialiser().deserialise(serialisedState);

    var getCode = function () {
        return preferredAreas[model.areaTypeId];
    }

    var containsAreaTypeId = function () {
        return !!getCode();
    }

    /**
    * Whether or not the preferred area code needs updating
    * @method doesAreaCodeNeedUpdating
    */
    this.doesAreaCodeNeedUpdating = function () {
        return !containsAreaTypeId() || getCode() !== model.areaCode;
    }

    /**
    * Updates the preferred area code for the current area type ID
    * @method updateAreaCode
    */
    this.updateAreaCode = function () {
        preferredAreas[model.areaTypeId] = model.areaCode;
    };

    /**
    * Gets the preferred area code for the current area type ID
    * @method getAreaCode
    */
    this.getAreaCode = function () {
        return containsAreaTypeId()
			? getCode()
			: null;
    };

    /**
    * Serialises this class to a string
    * @method serialise
    */
    this.serialise = function () {
        return new HashSerialiser().serialise(preferredAreas);
    }
}

/**
* Manages the preferred area type for each profile. For each profile this is the last area type the
* user has viewed.
* @class PreferredAreaTypeId
*/
function PreferredAreaTypeId(serialisedState, model) {

    var _this = this;
    var preferredAreaTypeIds = new HashSerialiser().deserialise(serialisedState);

    var _getAreaTypeId = function () {
        return preferredAreaTypeIds[model.profileId];
    }

    _this.containsProfileId = function () {
        return !!_getAreaTypeId();
    }

    _this.doesAreaTypeIdNeedUpdating = function () {
        return !_this.containsProfileId() || _getAreaTypeId() !== model.areaTypeId;
    }

    _this.updateAreaTypeId = function () {
        preferredAreaTypeIds[model.profileId] = model.areaTypeId;
    };

    _this.getAreaTypeId = function () {
        return _this.containsProfileId()
			? parseInt(_getAreaTypeId())
			: null;
    };

    _this.serialise = function () {
        return new HashSerialiser().serialise(preferredAreaTypeIds);
    }
}

/**
* Update the preferred areas and preferred area types cookies if required.
* @class updatePreferredState
*/
function updatePreferredState() {

    // Update preferred area cookie
    var preferredAreas = FT.preferredAreas;
    if (preferredAreas.doesAreaCodeNeedUpdating()) {
        preferredAreas.updateAreaCode();
        Cookies.set('preferredAreas', preferredAreas.serialise(), { expires: 1000 });
    }

    // Update preferred area type ID cookie
    var preferredAreaTypeIds = FT.preferredAreaTypeIds;
    if (preferredAreaTypeIds.doesAreaTypeIdNeedUpdating()) {
        preferredAreaTypeIds.updateAreaTypeId();
        Cookies.set('preferredAreaTypes', preferredAreaTypeIds.serialise(), { expires: 1000 });
    }
}

/**
* Wrapper around the html2canvas, takes the element as save it as an image
* @class saveElementAsImage
*/
function saveElementAsImage(element, outputFilename) {
    html2canvas(element, {
        onrendered: function (canvas) {
            var img = canvas.toDataURL('image/png');
            img = img.replace(/^data:image\/(png|jpg);base64,/, "");
            // save the captured image to hidden field and submit the form.
            $('#captured-image').val(img);
            $('#tab-name').val(outputFilename);
            $('#image-capture-form').submit();
        }
    }, {
        allowTaint: false, // allowTaint is required for maps
        logging: true,
        useCORS: true
    });
}

/**
* Whether or not search results are displayed
* @class isInSearchMode
*/
function isInSearchMode() {
    return FT.model.profileId === ProfileIds.SearchResults;
}

/**
* Returns group root for selected indicator
* @class getGroupRoot
*/
function getGroupRoot() {
    return groupRoots[getIndicatorIndex()];
}

/**
* Button options at top of tab pages.
* @class TabSpecificOptions
* @constructor
* @param options eventHandlers, eventName, 
* exportImage.label, exportImage.clickHandler, infoIcon.label, infoIcon.clickHandler
*/
function TabSpecificOptions(options) {

    var selectedClass = 'button-selected';
    var selectedOption = 0;
    var templateName = 'tabOptions';
    var $tabSpecificOptions = $('#tab-specific-options');

    var template = '{{#label}}<div>' +
        '<span>{{label}} </span>' +
        '{{#options}}<button id="tab-option-{{index}}" class="{{cssClass}}">{{text}}</button>{{/options}}' +
        '</div>{{/label}}' +
        '{{#isExport}}<div class="export-chart-box"><a href="">{{exportLabel}}</a></div>{{/isExport}}';

    /**
    * Show or hide the options. 
    * @method show
    * @param {Boolean} isVisible whether the buttons should be visible or not
    */
    this.show = function (isVisible) {
        if (isVisible) {
            $tabSpecificOptions.show();
        } else {
            $tabSpecificOptions.hide();
        }
    }

    /**
    * Sets the option that is selected.
    * @method setOption
    * @param {Integer} index index of option to select
    */
    this.setOption = function (index) {
        selectedOption = index;
    };

    /**
	* Gets the option that is selected.
	* @method getOption
    * @return {Integer} index of selected option
	*/
    this.getOption = function () {
        return parseInt(selectedOption);
    };

    /**
	* Clears the options HTML
	* @method clearHtml
	*/
    this.clearHtml = function () {
        $tabSpecificOptions.html('');
    }

    /**
	* Displays the options
	* @method setHtml
    * @param model label, optionLabels
	*/
    this.setHtml = function (model) {

        templates.add(templateName, template);

        var viewModel = {};

        // Export
        var isExport = isDefined(options.exportImage);
        viewModel.exportLabel = isExport ? options.exportImage.label : '';
        viewModel.isExport = isExport;

        // Buttons
        if (model.label) {

            viewModel.label = model.label;

            // Define options
            var i = 0;
            var optionConfigs = _.map(model.optionLabels,
                function (label) {
                    return { text: label, index: i++ }
                });
            viewModel.options = optionConfigs;

            // Selected option is highlighted
            viewModel.options[selectedOption].cssClass = selectedClass;
        }

        // Render HTML
        var html = templates.render(templateName, viewModel);
        $tabSpecificOptions.html(html);

        this._bind(model);

        // Bind export
        if (isExport) {
            $tabSpecificOptions.find('a').bind('click', options.exportImage.clickHandler);
        }
    };

    /**
	* Binds click event for each option
	* @method _bind
    * @param model
	*/
    this._bind = function (model) {

        if (options.eventHandlers) {
            var indexAttr = 'index';
            var eventHandlers = options.eventHandlers;

            for (var i in eventHandlers) {

                var $option = $('#tab-option-' + i);
                $option.attr(indexAttr, i);

                $option.bind('click',
                    function (e) {

                        $option = $(this);
                        var index = $option.attr(indexAttr);;

                        if (selectedOption != index) {
                            selectedOption = index;

                            // Highlight selected option
                            $tabSpecificOptions.find('button').removeClass(selectedClass);
                            $option.addClass(selectedClass);

                            // Event handler must be called after selected class is displayed
                            eventHandlers[selectedOption]($option);

                            logEvent(getCurrentPageTitle(), options.eventName, model.optionLabels[selectedOption]);
                        }
                    });
            }
        }
    };
};

function addTdLink(h, clickFunction, argument, text, tooltip) {

    h.push('<td');
    if (isDefined(tooltip)) {
        h.push(' title="', tooltip, '"');
    }

    h.push(' class="pLink" onclick="', clickFunction, '(\'', argument,
		'\');" onmouseover="highlightRow(this);" onmouseout="unhighlightRow();">',
		text, '</td>');
}

function highlightRow(td, isBgChanged) {

    var cells = FT.data.highlightedRowCells = $(td).parent().children();

    var border = colours.border;

    var attrs = ({
        'border-top-color': border,
        'border-bottom-color': border,
        'cursor': 'pointer'
    });

    if (isDefined(isBgChanged) && isBgChanged) {
        attrs['background-color'] = '#FDFFDD';
    }

    cells.css(attrs);
    cells.first().css({ 'border-left-color': border });
    cells.last().css({ 'border-right-color': border });
}

function unhighlightRow(isBgReset) {
    var cells = FT.data.highlightedRowCells;

    if (cells) {
        var c = '#eee';

        var attrs = {
            'border-top-color': c,
            'border-bottom-color': c,
            'cursor': 'default'
        };

        if (isDefined(isBgReset) && isBgReset) {
            attrs['background-color'] = '#fff';
        }

        cells.css(attrs);
        cells.first().css({ 'border-left-color': c });
        cells.last().css({ 'border-right-color': c });
    }
}

function setPageMode(newMode) {
    var c = pages.getCurrent();
    if (c !== newMode) {
        pages.setCurrent(newMode);
        ftHistory.setHistory();
        logEvent('TabSelected', getCurrentPageTitle(), 'UserSelection');
    }
}

function getCurrentPageTitle() {
    var page = pages.getCurrentPage();
    return isDefined(page)
        ? page.title
        : 'Unknown';
}

function restoreLastViewedAreaTypeId(model) {
    
    // Is area type ID defined for current profile
    if (!ftHistory.isParameterDefinedInHash('ati')) {
        // No area type in bookmarked state
        var preferredAreaTypeIds = FT.preferredAreaTypeIds;
        if (preferredAreaTypeIds.containsProfileId()) {
            model.areaTypeId = preferredAreaTypeIds.getAreaTypeId();
        }
    }
}

function documentReady() {
    var f = 'function';
    if (isInitRequired) {

        comparatorId = NATIONAL_COMPARATOR_ID;

        // Init model
        var model = FT.model;
  
        ftHistory.init(model);
        model.update();

        // Rehydrate cookies
        FT.preferredAreaTypeIds = new PreferredAreaTypeId(Cookies.get('preferredAreaTypes'), model);
        FT.preferredAreas = new PreferredAreas(Cookies.get('preferredAreas'), model);

        restoreLastViewedAreaTypeId(model);

        pages.init();

        if (typeof initSite === f) initSite();
        initData();

        initPageElements();

        if (isNational) {
            $('#area-search-link-box').show();
            initAreaSearch('#area-search-text', true);
        }

        stems = new SpineChartStems(spineHeaders);
        getValueNotesCall();

        // Log initial state
        logEvent('DomainSelected', getCurrentDomainName(), 'LandedOn');
        logEvent('TabSelected', getCurrentPageTitle(), 'LandedOn');
    }
}

function handleAjaxFailure(e) {

    alert('Data could not be loaded.');
    hideSpinner();
}

function refreshCurrentPage() {
    lock();
    // Get data from server
    ajaxMonitor.setCalls(2);
    getGroupingData();
    getIndicatorMetadata(FT.model.groupId, GET_METADATA_SYSTEM_CONTENT);
    ajaxMonitor.monitor(refreshCurrentPage2);
}

function executeWithLock(code) {

    if (!FT.ajaxLock) {
        lock();
        code();
    }
}

function clearSpineTables() {
    // Necessary to do both so tooltips work
    $('#spine-body').html('');
}

function AreaAndDataSorter(order, data, areas, areaCodeToAreaHash) {

    var coreDataSetList = new CoreDataSetList(data);

    var sortAreasByDataOrder = function () {
        // Sort areas
        var areasSorted = [];
        for (var i in data) {
            var areaInHash = areaCodeToAreaHash[data[i].AreaCode];

            if (areaInHash) {
                areasSorted.push(areaInHash);
            }
        }

        // Add missing areas
        if (areasSorted.length === 0) {
            // No data to sort
            areasSorted = areas;
        } else if (areasSorted.length !== _.size(areaCodeToAreaHash)) {

            for (var code in areaCodeToAreaHash) {
                var area = areaCodeToAreaHash[code];
                if ($.inArray(area, areasSorted) === -1) {
                    areasSorted.push(area);
                }
            }
        }

        return areasSorted;
    };

    var reverseDataIfNecessary = function () {
        if (order > 0) {
            data.reverse();
        }
    };

    this.byValue = function () {
        coreDataSetList.sortByValue();
        reverseDataIfNecessary();
        return sortAreasByDataOrder();
    }

    this.byCount = function () {
        coreDataSetList.sortByCount();
        reverseDataIfNecessary();
        return sortAreasByDataOrder();
    }
}

function getComparatorGrouping(groupRoot) {
    return getComparatorGroupingWithId(groupRoot, comparatorId);
}

function getComparatorGroupingWithId(groupRoot, comparatorId) {
    var groupings = groupRoot.Grouping;
    var count = groupings.length;

    for (var i = 0; i < count; i++) {
        var g = groupings[i];
        if (g.ComparatorId == comparatorId) {
            return g;
        }
    }

    for (i = 0; i < count; i++) {
        var g = groupings[i];
        if (g.ComparatorId == -1) {
            return g;
        }
    }
    return { Value: -1, ValF: NO_DATA };
}

function getRegionalComparatorGrouping(groupRoot) {
    return getComparatorGroupingWithId(groupRoot, REGIONAL_COMPARATOR_ID);
}

function ComparatorDataFormatter(groupRoot, data) {
    this.data = data;
    this.metadata = ui.getMetadataHash()[groupRoot.IID];
    this.getIndicatorNameLong = function () {
        return this.metadata.Descriptive.NameLong;
    };
}

function getGroupRootByIndicatorId(id, groupRootsAlt) {

    if (isDefined(groupRootsAlt)) {
        var g = groupRootsAlt;
    } else {
        g = groupRoots;
    }

    var intId = parseInt(id, 10);
    for (var i in g) {
        if (g[i].IID == intId) {
            return g[i];
        }
    }
}

function getNationalComparatorGrouping(groupRoot) {
    return getComparatorGroupingWithId(groupRoot, NATIONAL_COMPARATOR_ID);
}

function invertSortOrder(order) {
    return order == 0 ? 1 : 0;
}

/**
* Sort FT.data.sortedAreas by rank
* @class sortAreasByRank
*/
function sortAreasByRank() {
    var sortedByRank = FT.data.sortedAreas.slice(0);
    sortedByRank.sort(function (a, b) {
        return a.Rank - b.Rank;
    });
    return sortedByRank;
}

function isComparatorValueValid(comparatorGrouping) {

    return isDefined(comparatorGrouping) &&
		isDefined(comparatorGrouping.ComparatorData) &&
		comparatorGrouping.ComparatorData.Val !== -1;
}

function getCurrentComparator() {
    return getComparatorById(comparatorId);
}

/**
* List of colour constants
* @class colours
*/
var colours = {
    chart: '#a8a8cc',
    better: '#92d050',
    same: '#ffc000',
    worse: '#c00000',
    none: '#ffffff',
    limit99: '#a8a8cc',
    limit95: '#444444',
    border: '#666666',
    comparator: '#000000',
    bobLower: '#5555E6',
    bobHigher: '#C2CCFF',
    bodyText: '#333',
    noComparison: '#c9c9c9',
    quintile1: '#DED3EC',
    quintile2: '#BEA7DA',
    quintile3: '#9E7CC8',
    quintile4: '#7E50B6',
    quintile5: '#5E25A4'
};

function getComparatorFromAreaCode(areaCode) {

    return areaCode === NATIONAL_CODE ?
		getNationalComparator() :
		getParentArea();
}

function getComparatorById(id) {

    if (id === REGIONAL_COMPARATOR_ID) {
        return getParentArea();
    }

    // Find national area
    var name = 'England';
    return { Name: name, Code: NATIONAL_CODE, Short: name };
}

function getMiniMarkerImageFromSignificance(significance, useRag, useQuintileColouring) {
    return getMarkerImageFromSignificance(significance, useRag, '_mini', useQuintileColouring);
}

function getNationalComparator() {
    return getComparatorById(NATIONAL_COMPARATOR_ID);
}

function updatePageSelected(suffix, id) {
    var c = 'page-selected';
    suffix = '#' + suffix;
    $(suffix + 'Selection > a').removeClass(c);
    $(suffix + id).addClass(c);
}

function ComparisonConfig(groupRoot, indicatorMetadata) {

    var _this = this;

    var useTarget = indicatorMetadata.Target &&
		isCheckboxChecked('#target-benchmark');

    _this.useTarget = useTarget;
    _this.useQuintileColouring = groupRoot.ComparatorMethodId === ComparatorMethodIds.Quintiles;

    if (_this.useQuintileColouring) {
        _this.showQuintileLegend = true;
    }

    if (useTarget) {
        if (indicatorMetadata.Target.PolarityId === PolarityIds.BlueOrangeBlue) {
            _this.useBlueOrangeBlue = true;
            _this.useRagColours = false;
        } else {
            _this.useBlueOrangeBlue = false;
            _this.useRagColours = true;
        }
    } else {
        if (groupRoot.PolarityId !== PolarityIds.BlueOrangeBlue) {
            _this.useRagColours = true;
            _this.useBlueOrangeBlue = false;
        } else {
            _this.useRagColours = false;
            _this.useBlueOrangeBlue = true;
        }
    }

    // Which comparator to use
    _this.comparatorId = useTarget
		? TARGET_COMPARATOR_ID
		: comparatorId;
}

/**
* Adds 'area_type_id' and 'parent_area_code' to AJAX parameters builder
* @class addGroupDataParameters
*/
function addGroupDataParameters(parameters) {
    var model = FT.model;

    parameters.add('area_type_id', model.areaTypeId
        ).add('parent_area_code', getParentCode());
}

/**
* Gets the current parent area code
* @class getParentCode
*/
function getParentCode() {
    var model = FT.model;
    return !model.isNearestNeighbours() ? model.parentCode : model.nearestNeighbour;
}

function doSearch() {
    var searchText = new SearchTextValidator($('#searchBox').val());
    if (searchText.isOk) {
        if (isDefined(FT.model.parentTypeId) && isDefined(FT.model.areaTypeId)) {
            setUrl(FT.url.search + encodeURIComponent(searchText.text) + '#pat/' + FT.model.parentTypeId + '/ati/' + FT.model.areaTypeId + '/par/' + FT.model.parentCode);
        } else {
            setUrl(FT.url.search + encodeURIComponent(searchText.text));
        }
    }
}

function searchKeyPress(event) {
    var code = (window.event) ? event.keyCode : event.which;
    if (code == 13) {
        doSearch();
    }
}

function searchFocus(e) {
    var jq = $(e);
    if (jq.val() == 'Indicator keywords') {
        jq.val('');
        jq.removeClass('searchBlur');
    }
}

function searchBlur(e) {
    var jq = $(e);
    if (jq.val() == '') {
        jq.val('Indicator keywords');
        jq.addClass('searchBlur');
    }
}

function addProfileOrIndicatorsParameters(parameters) {

    // Profile ID
    var profileId = FT.model.profileId;
    if (isDefined(profileId)) {
        parameters.add('profile_id', profileId);
    }

    addIndicatorIdParameter(parameters);
}

function populateIndicatorMenu() {

    var root, i;
    var selectElement = $('#indicatorMenu')[0],
		options = selectElement.options;
    options.length = 0;
    var metadataHash = ui.getMetadataHash();

    for (i in groupRoots) {
        root = groupRoots[i];
        var indicator = metadataHash[root.IID];
        options[i] = new Option(indicator.Descriptive.Name +
			new SexAndAge().getLabel(root), i);
    }

    // Select preferred group root if user has already viewed this domain
    var preferredRoot = preferredGroupRoots[FT.model.groupId];
    if (isDefined(preferredRoot)) {

        var index = null;

        // By IID and SexId
        for (i in groupRoots) {
            root = groupRoots[i];
            if (preferredRoot.IID === root.IID &&
				preferredRoot.Sex.Id === root.Sex.Id) {
                index = i;
                break;
            }
        }

        if (index !== null) {
            var s = 'selected';
            $(options[index]).attr(s, s);
        }
    }
}

function parseValF(valF) {
    var val = parseFloat(valF);
    return isNaN(val) ? null : val;
}

function toggleDataOrNot(suffix, isData) {
    new MutuallyExclusiveDisplay({
        a: $('#' + suffix + '-data'),
        b: $('#' + suffix + '-no-data')
    }).showA(isData);
}

function getIndicatorIndex() {
    var model = FT.model;
    if (model.iid !== null) {
        var indicatorIndex = 0, matchingIndex;
        _.each(groupRoots, function (root) {
            if (root.IID === model.iid &&
		root.Sex.Id === model.sexId &&
		root.Age.Id === model.ageId) {
                matchingIndex = indicatorIndex;
            }
            indicatorIndex++;
        });

        if (isDefined(matchingIndex)) {
            setIndicatorIndex(matchingIndex);
            return matchingIndex;
        } else {
            return $('#indicatorMenu')[0].value;
        }
    }

    return $('#indicatorMenu')[0].value;
}

function getDataValues(data, attribute) {
    var a = [],
		i,
		val;
    for (i in data) {
        val = data[i] === null
			? 0
			: parseFloat(data[i][attribute]);
        a.push(val);
    }
    return a;
}

function getTrendHeader(metadata, root, areaName, clickHandler, isNewData) {

    var unit = metadata.Unit.Label,
		unitLabel = unit !== '' ?
			' - ' + unit :
			'';

    return ['<div class="trend-header"><div class="trend-title"><a class="trend-link" title="More about this indicator" href="javascript:',
		clickHandler, ';">', metadata.Descriptive.Name, new SexAndAge().getLabel(root), '</a>' + getNewDataBadge(isNewData),
		getIndicatorDataQualityHtml(metadata.Descriptive.DataQuality),
		'<span class="trend-area">', areaName, '</span>', '</div>',
		'<div class="trend-unit">', metadata.ValueType.Name, unitLabel, '</div>',
		'</div>'].join('');
}

function hasDataChanged(groupRoot) {
    var isNewData = false;

    if (groupRoot.DateChanges !== null) {
        isNewData = groupRoot.DateChanges.HasDataChangedRecently;
    }
    return isNewData;
}

function getNewDataBadge(isNewData) {
    var badge = '';
    if (isNewData === true) {
        badge = NEW_DATA_BADGE;
    }
    return badge;
}

function getSource(metadata) {

    var source = metadata.Descriptive.DataSource;

    return isDefined(source) && !String.isNullOrEmpty(source) ?
		'<div class="trend-source">Source: ' + source + '</div>' :
		'';
}

function indicatorChanged(rootIndex) {

    var root = groupRoots[rootIndex];
    FT.menus.area.setAdditionalParameters(root.IID, root.Age.Id, root.Sex.Id);
    ftHistory.setHistory();

    // Enables a specific indicator to be preferentially chosen when the menu is repopulated
    root = groupRoots[rootIndex];
    preferredGroupRoots[FT.model.groupId] = root;

    if (mode === PAGE_MODES.INDICATOR_DETAILS) {
        // Bar chart page is special case
        displayIndicatorDetails();
    } else {
        pages.goToCurrent();
    }

    logEvent('IndicatorSelectedInMenu',
		ui.getMetadataHash()[root.IID].Descriptive.Name);
}

function showAllIndicators() {
    return areaTrendsState.showAll;
}

function isCheckboxChecked(id) {
    return $(id).prop('checked');
}

function getSelectedIndicatorId() {
    if (groupRoots.length) {
        return groupRoots[getIndicatorIndex()].IID;
    }

    // If no group roots are available for the current domain and area type
    return null;
}

function setShowAllDisabledState() {
    $('#indicatorMenu').prop('disabled', showAllIndicators());
}

function getFirstGrouping(root) {
    return root.Grouping[0];
}

function canDataBeDisplayed() {
    return true;
}

function getSignificanceImg(sig, useRag, useQuintileColouring) {
    if (useQuintileColouring) {
        switch (true) {
            case (sig > 0 && sig < 6):
                return 'quintile' + sig + '.png';
        }
    } else {
        if (useRag) {
            switch (sig) {

                case 1:
                    return 'red.png';
                case 2:
                    return 'same.png';
                case 3:
                    return 'better.png';
            }
        } else {
            switch (sig) {

                case 1:
                    return 'bobLower.png';
                case 2:
                    return 'same.png';
                case 3:
                    return 'bobHigher.png';
            }
        }
    }

    return null;
}

function getGroupAndCurrentAreaTypeKey() {
    return getGroupAndAreaTypeKey(FT.model.areaTypeId);
}

function setIndicatorIndex(i) {
    // Select indicator in menu
    $('#indicatorMenu').val(i);
}

function getGroupAndAreaTypeKey(areaTypeId) {
    return getKey(FT.model.groupId, areaTypeId, FT.model.nearestNeighbour);
}

function displayFingertipsMetadata() {

    var root = groupRoots[getIndicatorIndex()];
    var indicatorMetadata = ui.getMetadataHash()[root.IID];
    displayMetadata(indicatorMetadata, root);
    showAndHidePageElements();
    unlock();
}

function refreshCurrentPage2() {
    // Continuation of refreshCurrentPage now data has been loaded
    if (canDataBeDisplayed()) {

        populateIndicatorMenus();

        if (FT.model.isNearestNeighbours()) {
            toggleNearestNeighboursControls(false);
        }
        pages.goToCurrent();
    } else {
        goToTartanRugPage();
        unlock();
    }
}

function initPageElements() {

    spineChart.init(250, 18);

    var model = FT.model;

    // Init menus
    var menus = FT.menus;
    menus.parent = new ParentMenu('#regionMenu', model, loaded.areaLists);
    menus.parentType = new ParentTypeMenu('#parentAreaTypesMenu', model);
    menus.area = new AreaMenu('#areaMenu', model);
    menus.benchmark = new BenchmarkMenu('#comparator', model);

    // Key heading
    var benchmark = enumParentDisplay === PARENT_DISPLAY.NATIONAL_ONLY ?
		'England' :
		'benchmark';
    $('.key-text').html('Compared with ' + benchmark);

    // Selected domain
    $('#domain' + model.groupId).addClass('selected-domain');
    var $domainMenu = $('#domain-dropdown');
    if ($domainMenu.length) {
        $domainMenu.val(model.groupId);
        // Make text visible now domain is selected
        $domainMenu.css('color', '');
    }

    // Benchmark target option
    $('#target-benchmark').click(function () {
        pages.goToCurrent();
    });

    tooltipManager.init();
}

function initAreaData() {
    ajaxMonitor.setCalls(2);
    getParentAreaGroups(); // parent area types
    getAllAreas(FT.model.areaTypeId); // need all for map
    ajaxMonitor.monitor(initAreaData2);
}

function getValueNotesCallback(obj) {

    if (isDefined(obj)) {
        // Set up loaded value notes hash
        _.each(obj, function (valueNote) {
            loaded.valueNotes[valueNote.Id] = valueNote;
        });
    }
}

function loadValueNoteToolTips() {
    // Tooltips    
    var $valueNotes = $('.valueNote');
    _.each($valueNotes, function (valueNote) {
        tooltipManager.initElement(valueNote.id);
    });
}

function ValueNoteTooltipProvider() { }

ValueNoteTooltipProvider.prototype = {
    getHtml: function (id) {

        if (id !== '') {
            var bits = id.split('_');
            return this.getValueNoteCellText(bits);
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

function initData() {
    initAreaData();
}

var PARENT_DISPLAY = {
    NATIONAL_AND_REGIONAL: 0,
    REGIONAL_ONLY: 1,
    NATIONAL_ONLY: 2
};

function getAreaListKey() {
    return FT.model.parentCode + '-' + FT.model.areaTypeId;
}

function getMarkerImageFromSignificance(significance, useRag, suffix, useQuintileColouring) {

    var tail = suffix + '.png';
    if (useQuintileColouring) {
        switch (true) {
            case (significance > 0 && significance < 6):
                return 'circle_quintile' + significance + tail;
        }
    } else {
        if (useRag) {
            switch (significance) {
                case 1:
                    return 'circle_red' + tail;
                case 2:
                    return 'circle_orange' + tail;
                case 3:
                    return 'circle_green' + tail;
            }
        } else {
            switch (significance) {
                case 1:
                    return 'circle_darkblue' + tail;
                case 2:
                    return 'circle_orange' + tail;
                case 3:
                    return 'circle_lightblue' + tail;
            }
        }
    }

    return 'circle_white' + tail;
}

function getAllAreas(areaTypeId) {

    if (FT.model.areAnyIndicators && !loaded.areaLists[areaTypeId]) {
        showSpinner();

        var parameters = new ParameterBuilder();
        parameters.add('area_type_id', areaTypeId);
        parameters.add('profile_id', FT.model.profileId);
        parameters.add('template_profile_id', templateProfileId);

        ajaxGet('api/areas/by_area_type',
			parameters.build(),
			function (areas) {

			    if (isDefined(areas)) {
			        loaded.areaLists[areaTypeId] = areas;
			        // If missing for one area then will be missing for all, e.g. practices
			        if (!areas[0].Short) {
			            for (var i in areas) {
			                // Ensure Short name is set, may need to trim this if too long
			                areas[i].Short = areas[i].Name;
			            }
			        }
			    }

			    ajaxMonitor.callCompleted();
			});
    } else {
        ajaxMonitor.callCompleted();
    }
}


function getChildAreas() {
    var codes = new AreaMappings(FT.model).getChildAreaCodes();
    return new AreaCollection(loaded.areaLists[FT.model.areaTypeId]).findAll(codes);
}

function getParentArea() {
    var parentAreas = loaded.areaLists[FT.model.parentTypeId];
    return new AreaCollection(parentAreas).find(FT.model.parentCode);
}

function GetParentAndChildAreas() {
    var parentCode = FT.model.parentCode;
    var list = loaded.areaLists[FT.model.areaTypeId];
    return _.find(list, function (obj) {
        return obj.Parent.Code === parentCode;
    });
}

function enableMenus() {
    $('select').prop('disabled', null);
}

function lock() {
    if (!FT.ajaxLock) {
        FT.ajaxLock = 1;

        // Disable menus
        var disabled = 'disabled';
        $('select').attr(disabled, disabled);
    }
}

function unlock() {
    if (FT.ajaxLock) {
        FT.ajaxLock = null;
        enableMenus();
    }
}

function getAreaHash(areas) {
    var areaHash = {};
    for (var i in areas) {
        areaHash[areas[i].Code] = areas[i];
    }
    return areaHash;
};

function setAreas() {
    var sortedAreas = FT.data.sortedAreas = getChildAreas();

    var isNearestNeighbours = FT.model.isNearestNeighbours();

    if (isNearestNeighbours) {

        // add selected neighbour areacode to zeroth index 
        var areaList = loaded.areaLists[FT.model.areaTypeId];
        var area = new AreaCollection(areaList).find(FT.model.areaCode);
        sortedAreas.splice(0, 0, area);

        // add nearest neighbour rank property
        for (var x in sortedAreas) {
            sortedAreas[x].Rank = x;
        }
    }

    areaHash = getAreaHash(sortedAreas);

    if (!isNearestNeighbours) {
        FT.menus.area.setAreas(sortedAreas);
    }
}

function getChildAreasForRegion(parentCode, areaTypeId) {
    var list = loaded.areaLists[areaTypeId];
    return _.find(list, function (obj) {
        return obj.Parent.Code === parentCode;
    }).Children;
}

//
// Start of collection that pulls all area related code together
//
// areas : an array of Area objects
//
function AreaCollection(areas) {
    this.containsAreaCode = function (code) {
        return code !== null &&
			_.any(areas, function (area) { return area.Code === code; });
    };

    this.find = function (code) {
        return _.find(areas, function (area) { return area.Code === code; });
    };

    this.findAll = function (codes) {

        var codeToArea = {};
        _.each(areas, function (area) { codeToArea[area.Code] = area; });
        var selectedAreas = [];
        _.each(codes, function (code) { selectedAreas.push(codeToArea[code]); });

        return selectedAreas;
    };
}

//
// Performs operations on a list of CoreDataSet objects
//
// list : array of CoreDataSet objects
//
function CoreDataSetList(list) {

    this.getValidValues = function (property) {
        var validCoreData = _.filter(list, function (obj) {
            return isDefined(obj) && isValidValue(obj[property]);
        });

        return _.pluck(validCoreData, property);
    };

    this.areAnyValidTrendValues = function () {
        return this.getValidValues('V').length > 0;
    };

    this.sortByValue = function () {
        list.sort(sortData);
    };

    this.sortByCount = function () {
        list.sort(function (a, b) {
            return a.Count - b.Count;
        });
    };
};

function isParentCountry() {
    return FT.model.parentTypeId === AreaTypeIds.Country;
}

function isSubnationalColumn() {
    return enumParentDisplay !== PARENT_DISPLAY.NATIONAL_ONLY &&
		!isParentCountry() &&
		!FT.model.isNearestNeighbours();
}

function BarScale(dataList) {

    var _this = this;

    _this.width = 240; // value also in IndicatorDetailsHtml.cshtml 
    _this.buffer = 1.05; // bit of white space on the right

    // Find limits 

    var mapper = new CoreDataSetList(dataList),
		values = mapper.getValidValues('Val'),
		lowerCIs = mapper.getValidValues('LoCI'),
		upperCIs = mapper.getValidValues('UpCI'),
		mmVal = new MinMaxFinder(values),
		mmLo = new MinMaxFinder(lowerCIs),
		mmUp = new MinMaxFinder(upperCIs);

    // Max
    var max = !mmUp.isValid || mmVal.max > mmUp.max ?
		mmVal.max :
		mmUp.max;

    // Adjust max
    if (max < 0) {
        // Start bars from zero
        max = 0;
    } else {
        // Add buffer to max
        max = max * _this.buffer;
    }

    // Min
    var min = !mmLo.isValid || mmVal.min < mmLo.min ?
		mmVal.min :
		mmLo.min;
    if (min > 0) {
        min = 0;
    }

    _this.range = (max - min);

    _this.pixelsPerUnit = (_this.width) / _this.range;

    _this.negativePixels = min < 0 ?
		Math.floor(-min * _this.pixelsPerUnit) :
		0;
}

//
// Shows the appropriate legend for a bar chart page.
//
function showBarChartLegend(useTarget) {
    new MutuallyExclusiveDisplay({
        a: $('#key-ad-hoc'),
        b: $('#key-bar-chart')
    }).showA(useTarget);
}

//
// Displays a legend for the target
//
function setTargetLegendHtml(comparisonConfig, metadata) {    
    if (comparisonConfig.useTarget) {
        var targetLegend = getTargetLegendHtml(comparisonConfig, metadata);
        $('#key-ad-hoc').html(
				'<div><table class="key-table"><tr><td class="key-text">Benchmarked against goal:</td><td class="key-spacer"></td><td>' +
				targetLegend + '</td></tr></table></div>').show();
    }
}

function TargetLegend(metadata) {
    var targetConfig = metadata.Target;
    var bespokeKey = targetConfig.BespokeKey,
    start = '<span class="target ',
    end = '</span>';

    var colourClasses = targetConfig.PolarityId === PolarityIds.BlueOrangeBlue
        ? ['bobLower', 'bobHigher']
        : ['worse', 'better'];

    this.render = function () {

        // Legend may already be defined
        var legendHtml = targetConfig.LegendHtml;
        if (legendHtml && !String.isNullOrEmpty(legendHtml)) {
            return legendHtml;
        }

        if (bespokeKey) {
            if (bespokeKey === 'last-year-england') {
                return this.renderLastYearEngland();
            }

            if (bespokeKey === 'nth-percentile-range') {
                return this.renderPercentileRange();
            }

            // Unknown bespoke key
            return '';
        }

        return this.renderValueComparison();
    }

    this.renderLastYearEngland = function () {
        var viewModel = {
            lowerColour: colourClasses[0],
            higherColour: colourClasses[1]
        };
        templates.add(bespokeKey,
            start + '{{lowerColour}}">&lt;previous year\'s England value</span>' +
            start + '{{higherColour}}">&ge;previous year\'s England value</span>');

        return templates.render(bespokeKey, viewModel);
    }

    this.renderPercentileRange = function () {
        var lowerTargetPercentile = targetConfig.LowerLimit;
        var upperTargetPercentile = targetConfig.UpperLimit;

        // Red, amber green
        if (targetConfig.PolarityId === PolarityIds.RAGLowIsGood) {
            return start + colourClasses[0] + '">&gt;' + upperTargetPercentile + 'th-percentile of UTLAs' + end +
                start + 'same' + '">&le;' + upperTargetPercentile + 'th to ' + '&gt;' + lowerTargetPercentile + 'th' + end +
                start + colourClasses[1] + '">&le;' + lowerTargetPercentile + 'th' + end;
        }

        if (targetConfig.PolarityId === PolarityIds.RAGHighIsGood) {
            return start + colourClasses[0] + '">&lt;' + lowerTargetPercentile + 'th-percentile of UTLAs' + end +
            start + 'same' + '">&ge;' + lowerTargetPercentile + 'th to ' + '&lt;' + upperTargetPercentile + 'th' + end +
            start + colourClasses[1] + '">&ge;' + upperTargetPercentile + 'th' + end;
        }
    }

    this.renderValueComparison = function () {

        var lowerLimit = new CommaNumber(targetConfig.LowerLimit).unrounded();
        var upperLimit = targetConfig.UpperLimit;
        var useCIs = targetConfig.UseCIsForLimitComparison;
        var suffix = new ValueSuffix(metadata.Unit).getShortLabel();
        var html;

        // Switch colouring if low is good
        if (targetConfig.PolarityId === PolarityIds.RAGLowIsGood) {
            colourClasses = colourClasses.reverse();
        }

        // Worse
        html = [start, colourClasses[0], '">&lt;', lowerLimit, suffix, end];

        // Same
        if (upperLimit) {
            upperLimit = new CommaNumber(upperLimit).unrounded();
            html.push(start, 'same">', lowerLimit, suffix, ' to ', upperLimit, suffix, end);
        } else {
            if (useCIs) {
                html.push(start, 'same">Similar to ', lowerLimit, suffix, end);
            }
            upperLimit = lowerLimit;
        }

        // Better
        html.push(start, colourClasses[1], '">&ge;', upperLimit, suffix, end);

        return html.join('');
    }
}

//
// Get HTML for target legend
//
function getTargetLegendHtml(comparisonConfig, metadata) {

    if (comparisonConfig.useTarget) {
        return new TargetLegend(metadata).render();
    } else {
        return '';
    }
}

//
// Provides information about a TrendDataPoint object
//
function TrendDataInfo(trendDataPoint) {
    this.data = trendDataPoint;
    if (trendDataPoint) {
        // Allow TrendDataPoint to be used as a CoreDataSet 
        trendDataPoint.Count = trendDataPoint.C;
        trendDataPoint.Val = trendDataPoint.V;
    }
}

TrendDataInfo.prototype = {
    isDefined: CoreDataSetInfo.prototype.isDefined,

    getValF: function () {
        return this.data.V;
    },

    isValue: function () {
        var data = this.data;
        return data ?
			data.V !== '-' :
			false;
    },

    isCount: function () {
        return this.data.IsC;
    },

    isNote: CoreDataSetInfo.prototype.isNote,
    getNoteId: CoreDataSetInfo.prototype.getNoteId
};

function ValueDisplayer(unit) {

    this.unit = unit;
    this.symbol = VALUE_NOTE;
}

//
// Generates HTML for displaying a value with the unit and value note symbol.
// 
ValueDisplayer.prototype = {
    _getNumber: function (number, options) {
        return new ValueWithUnit(this.unit).getShortLabel(number, options);
    },

    byDataInfo: function (dataInfo, options) {

        var isValueNote = dataInfo.isNote();

        if (dataInfo.isValue()) {
            return this._getNumber(dataInfo.getValF(), options) +
			(isValueNote ? this.symbol : '');
        }

        // No value
        return isValueNote ?
			'<span class="value-note-symbol">' + this.symbol + '</span>' :
			NO_DATA;
    },

    byNumberString: function (number, options) {
        return !number || number === '-' || number === -1 ?
            NO_DATA :
			this._getNumber(number, options);
    }
};

function getParentAreaGroups() {
    if (_.size(loaded.parentAreaGroups) === 0) {

        var parameters = new ParameterBuilder(
        ).add('profile_id', FT.model.profileId);

        ajaxGet('api/area_types/parent_area_types', parameters.build(),
            function (areaTypes/* [] -> {Id} */) {

                // Area types menu
                FT.menus.areaType = new AreaTypeMenu(FT.model,
                    new AreaTypes(areaTypes, loaded.areaTypes));

                // Map of child area type ID to parent area type
                var areaTypeMap = _.object(_.map(areaTypes,
                    function (areaType) {
                        return [areaType.Id, areaType.ParentAreaTypes];
                    }));

                loaded.parentAreaGroups = areaTypeMap;

                new ParentTypes(FT.model).setDefault();

                ajaxMonitor.callCompleted();
            });

    } else {
        ajaxMonitor.callCompleted();
    }
}

function initAreaData2() {
    var model = FT.model;

    if (!loaded.areaTypes[model.areaTypeId]) {
        // Area type id that was restored from cookie is not available for current profile anymore
        FT.model.areaTypeId = defaultAreaType;
        initAreaData();
        return;
    }

    ajaxMonitor.setCalls(2);
    getAreaMappings(model);
    getAllAreas(model.parentTypeId);
    ajaxMonitor.monitor(initAreaElements);
}

function getAreaMappings(model) {

    if (model.areAnyIndicators && !new AreaMappings(model).areDefined()) {
        var parameters = new ParameterBuilder();
        parameters.add('profile_id', model.profileId);
        parameters.add('child_area_type_id', model.areaTypeId);
        parameters.add('parent_area_type_id', model.parentTypeId);
        parameters.add('nearest_neighbour_code', model.nearestNeighbour);

        // Get parent areas
        ajaxGet('api/parent_to_child_areas', parameters.build(), getAreaMappingsCallback);
    } else {
        ajaxMonitor.callCompleted();
    }
}

function getAreaMappingsCallback(obj) {

    new AreaMappings(FT.model).set(obj);
    ajaxMonitor.callCompleted();
}

function ParentTypeMenu(jquerySelector, model) {
    var _this = this,
		$menu = $(jquerySelector),
		selected = 'selected',
		$box = $('#parentTypeBox'),
		selectedOption = function () {
		    return $menu.find('option:' + selected);
		};

    $menu.change(function () {
        // Lock test required so change is only fired by the user selecting a different option
        if (!FT.ajaxLock) {
            lock();
            model.parentTypeId = _this.getId();
            model.parentCode = null;
            ftHistory.setHistory();
            initAreaData2();

            logEvent('ParentAreaTypeSelected', _this.getName());
        }
    }
	);

    _this.setOptions = function () {

        // Check the menu exists for the current profile
        if ($menu.length) {

            var parentTypes = new ParentTypes(model).getTypes(),
				options = $menu[0].options;

            options.length = 0;

            var parentTypeId = model.parentTypeId,
				optionIndex = 0;

            for (var i in parentTypes) {
                var areaType = parentTypes[i],
					id = areaType.Id,
					option = new Option(areaType.Short, id);
                options[optionIndex++] = option;

                // Select option
                if (parentTypeId && id === parentTypeId) {
                    $(option).prop(selected, selected);
                }
            }
        }
    };

    _this.getId = function () {
        return parseInt(selectedOption().val());
    };

    _this.getName = function () {
        var isDepDecRegex = /(IMD \d+)/i;
        return isDepDecRegex.test(selectedOption().text()) ? 'Deprivation decile' : selectedOption().text();
    };

    _this.count = function () {
        return $menu[0].options.length;
    };

    _this.hide = function () {
        $box.hide();
    };

    _this.setVisibility = function () {
        // If visible then only display if multiple options
        $box.filter(':visible').toggle(_this.count() > 1);
    };
}

function AreaMappings(model) {

    var key = getKey(model.parentTypeId, model.areaTypeId, model.nearestNeighbour),
		loadedMappings = loaded.areaMappings;

    this.set = function (mappings) {
        loadedMappings[key] = mappings;
    };

    this.areDefined = function () {
        return !!loadedMappings[key];
    };

    this.getChildAreaCodes = function () {
        return model.isNearestNeighbours() ?
			loadedMappings[key][model.areaCode] :
			loadedMappings[key][model.parentCode];
    };

    this.getParentAreaCode = function (childCode) {

        if (childCode) {
            var mappings = loadedMappings[key];
            for (var parentCode in mappings) {
                if (_.contains(mappings[parentCode], childCode)) {
                    return parentCode;
                }
            }
            return null;
        }

        return model.parentCode;
    };
}

function ParentMenu(jquerySelector, model, loadedAreas) {
    var _this = this,
		$menu = $(jquerySelector),
		$box = $('#region-menu-box');

    $menu.change(function () {
        // Lock test required so change is only fired by the user selecting a different option
        if (!FT.ajaxLock) {
            lock();
            model.parentCode = $menu.val();
            setAreas();
            ftHistory.setHistory();
            refreshCurrentPage();

            logEvent('ParentAreaSelected', getParentArea().Name, 'UserSelection');
        }
    });

    _this.setCode = function (code) {
        if (code) {
            $menu.val(code);
            model.parentCode = code;
        }
    };

    _this.setOptions = function () {

        var parentTypeId = model.parentTypeId;

        // Change parentAreaCode to keep same child area
        var parentAreaCode = new AreaMappings(model).getParentAreaCode(model.areaCode);
        if (parentAreaCode) {
            model.parentCode = parentAreaCode;
        }

        // Populate parent menu
        var parents = loadedAreas[parentTypeId],
			j = 0,
			options = $menu[0].options;

        options.length = 0;

        // Name type
        var nameType = parentTypeId === AreaTypeIds.CountyUA
			? 'Name'
			: 'Short';

        // Populate menu options
        for (var i in parents) {
            var parent = parents[i];
            options[j++] = new Option(parent[nameType], parent.Code);
        }

        // Set default parent if not specified in hash
        if (!new AreaCollection(parents).containsAreaCode(model.parentCode)) {
            model.parentCode = parents[0].Code;
        }

        $menu.val(model.parentCode);

        try {
            // Log fails in tests
            logEvent('ParentAreaSelected', getParentArea().Name, 'MenuPopulated');
        } catch (e) { }
    };

    _this.setVisibility = function () {
        var hide = _this.count() === 1 && model.parentTypeId === AreaTypeIds.Country;
        $box.filter(':visible').toggle(!hide);
    }

    // Number of options
    _this.count = function () {
        return $menu[0].options.length;
    };
}

function ParentTypes(model) {
    var types = loaded.parentAreaGroups[model.areaTypeId];

    this.setDefault = function () {
        if (!model.parentTypeId) {
            model.parentTypeId = types[0].Id;
        }
    };

    this.matchTypeNameOrChange = function (typeName) {

        for (var i in types) {
            if (types[i].Short === typeName) {
                model.parentTypeId = types[i].Id;
                return;
            }
        }

        // Currently selected parent type not available so change
        model.parentTypeId = types[0].Id;
    };

    this.getTypes = function () {
        return types;
    };

    this.getCurrent = function () {
        var parentTypeId = model.parentTypeId;
        return _.find(types, function (type) {
            return type.Id === parentTypeId;
        });
    };
}

function AreaMenu(jquerySelector, model) {
    var _this = this,
		$menu = $(jquerySelector),
		$box = $('#areaMenuBox');

    $menu.change(function () {
        if (!FT.ajaxLock) {
            lock();
            model.areaCode = _this.getCode();
            ftHistory.setHistory();
            pages.goToCurrent();
            logEvent('AreaSelectedInMenu', areaHash[model.areaCode].Name);
        }
    });

    _this.getCode = function () {
        return $menu.val();
    };

    _this.setCode = function (code) {
        if (code) {
            $menu.val(code);
            model.areaCode = code;
        }
    };

    //TODO this has nothing to do with the area menu and should be moved elsewhere
    _this.setAdditionalParameters = function (iid, ageId, sexId) {
        if (iid) {
            model.iid = iid;
        }

        if (ageId) {
            model.ageId = ageId;
        }

        if (sexId) {
            model.sexId = sexId;
        }
    };

    _this.hide = function () {
        $box.hide();
    };

    _this.setAreas = function (areas) {
        populateAreaMenu(areas, $menu);
        var areaCode = model.areaCode;
        _this.setCode(
			new AreaCollection(areas).containsAreaCode(areaCode) ?
            areaCode :
			this.getCode()
		);
    };
}

function BenchmarkMenu(jquerySelector, model) {

    var $menu = $(jquerySelector);
    var options = $menu[0].options;
    var $subnationalOption = $(options[1]);

    this.setComparatorId = function (id) {
        comparatorId = id;
        $menu.val(id);
    }

    this.getComparatorId = function () {
        return parseInt($menu.val());
    }

    this.setSubnationalOptionVisibility = function () {
        if (model.parentTypeId === AreaTypeIds.Country ||
			model.isNearestNeighbours() ||
			enumParentDisplay === PARENT_DISPLAY.NATIONAL_ONLY) {
            $subnationalOption.hide();
        }
        else {
            $subnationalOption.show();
        }
    };
}

function AreaTypeMenu(model, areaTypes) {

    var _this = this,
		id = 'areaTypes',
		selected = 'selected',
		$box = $('#areaTypeBox'),
		visibility = 'visibility',
		getAreaTypeOptions = function () {

		    var areaTypesArray = areaTypes.getAreaTypes();

		    // Select current area type
		    _.each(areaTypesArray, function (a) {
		        if (a.Id === model.areaTypeId) {
		            a.selected = selected;
		        }
		    });

		    // Sort area types
		    areaTypesArray = _.sortBy(areaTypesArray, 'Short');

		    templates.add('areaTypes',
				'{{#types}}<option value="{{Id}}" {{selected}} title="{{Name}}">{{Short}}</option>{{/types}}');

		    return templates.render('areaTypes', { types: areaTypesArray });
		};
    $box.html(
		'<li><select id="' + id + '">' + getAreaTypeOptions() +
		'</select></li><li class="heading">Area type</li>');

    var $menu = $('#' + id),
		selectedOption = function () {
		    return $menu.find('option:' + selected);
		};

    $menu.change(function () {

        if (!FT.ajaxLock) {

            // So that we know when to show the user a message saying the current
            // indicator isn't available for the new area type
            groupRootBeforeAreaTypeChange = getGroupRoot();

            var areaTypeId = parseInt(selectedOption().val());
            setAreaType(areaTypeId);
            displayAreaSearchOptions(areaTypeId);

            logEvent('AreaTypeSelected', _this.getName());
        }
    });

    _this.setTypeId = function (areaTypeId) {
        if (areaTypeId) {
            $menu.val(areaTypeId);
            model.areaTypeId = areaTypeId;
        }
    };

    _this.getName = function () {
        return selectedOption().text();
    };

    _this.count = function () {
        return $menu[0].options.length;
    };

    _this.setVisibility = function () {
        // If visible then only display if multiple options
        if (_this.count() > 1) {
            $box.show();
            var boxVisibility = '';
        } else {
            // Want to use visibility property so that "Areas grouped by" 
            // menu position is not affected by the state of this menu
            boxVisibility = visibility;
        }
        $box.css(visibility, boxVisibility);
    };
}

function initAreaElements() {
    var model = FT.model;
    var areaMappings = new AreaMappings(model);

    if (!model.isNearestNeighbours()) {
        // Preferred area only if its not NN
        if (!model.areaCode /*no area code in hash*/ ||
            areaMappings.getParentAreaCode(model.areaCode) ===
            null /* area code from another area type that doesn't contain the child area */) {
            model.areaCode = FT.preferredAreas.getAreaCode();
        }
    }

    var menus = FT.menus;
    menus.parent.setOptions();
    menus.parentType.setOptions();
    setAreas();
    displayAreaSearchOptions(model.areaTypeId);
    
    // Set area type ID in case (e.g user selected browser back or forward)
    menus.areaType.setTypeId(model.areaTypeId);

    if (model.parentTypeId === AreaTypeIds.Country) {
        // Subnational benchmark not valid
        menus.benchmark.setComparatorId(NATIONAL_COMPARATOR_ID);
    }

    // Update all elements that display the name of the parent area type
    $('.parent-area-type').html(menus.parentType.getName());

    // Hide options for area types with no results
    if (isInSearchMode()) {
        indicatorSearch.hideAreaTypesWithNoResults();
    }

    refreshCurrentPage();
}

function benchmarkChanged() {

    if (!FT.ajaxLock) {

        lock();

        comparatorId = FT.menus.benchmark.getComparatorId();

        pages.goToCurrent();

        logEvent('BenchmarkSelected', comparatorId === NATIONAL_COMPARATOR_ID
			? 'National'
			: 'Subnational');
    }
}

/**
 * Domain is selected by domain buttons
 */
function domainClicked(groupId) {
    var model = FT.model;
    if (model.groupId !== groupId && !FT.ajaxLock) {

        lock();
        model.groupId = groupId;
        ftHistory.setHistory();
        refreshCurrentPage();

        updateDomains();

        logEvent('DomainSelected', getCurrentDomainName(), 'UserSelection');
    }
}

/**
 * Domain is selected using drop down menu
 */
function domainSelected() {
    var selectedGroupId = parseInt($('#domain-dropdown').val());

    if (FT.model.groupId !== selectedGroupId) {
        domainClicked(selectedGroupId);
    }
}

function getCurrentDomainName() {
    var groupId = FT.model.groupId;
    if ($('#domain-dropdown').length > 0) //if dropdown is visible
        return $('#domain-dropdown>option[value=' + groupId + ']').text();
    else
        return $.trim($('#domain' + groupId).text());
}

function populateIndicatorMenus() {
    populateIndicatorMenu();
}

function indicatorNameClicked(id) {
    goToIndicatorDetailsPage(id);
}

var VIEW_MODES = {
    AREA: 0,
    MULTI_AREA: 1
};

function getIndicatorNameTooltip(rootIndex, area) {
    var root = groupRoots[rootIndex],
		metadata = ui.getMetadataHash()[root.IID],
		comparisonConfig = new ComparisonConfig(root, metadata),
		useTarget = comparisonConfig.useTarget,
		targetLabel = (area
				? 'The value for ' + area.Name + ' is'
				: 'These area values are') +
			' benchmarked against the goal</span>';

    return [
		(useTarget ? '<span id="tooltip-target">' + targetLabel + '</span>' : ''),
		'<span id="tooltipIndicator"', (!useTarget ? 'style="margin-top:0;"' : ''), '>',
		metadata.Descriptive.NameLong, '</span>'
    ].join('');
}

// Stores data loaded by AJAX
var ui = (function () {

    // Stores loaded group data
    var data = {},
		stats = {};

    // Window position (prevents vertical scrollbar being reset to top)
    var scrollTop = null;

    return {
        callbackIds: {},

        // Window position
        storeScrollTop: function () {
            scrollTop = $(window).scrollTop();
        },

        setScrollTop: function () {
            if (isDefined(scrollTop)) {
                $(window).scrollTop(scrollTop);
            }
        },

        _getStatsKey: function (parentAreaCode) {
            return getKey(FT.model.groupId, FT.model.areaTypeId, parentAreaCode);
        },

        // Cache indicator stats responses
        getIndicatorStats: function (parentAreaCode) {
            var key = this._getStatsKey(parentAreaCode);
            return stats[key] ?
				stats[key] :
				null;
        },

        setIndicatorStats: function (parentAreaCode, newData) {
            stats[this._getStatsKey(parentAreaCode)] = newData;
        },

        areIndicatorStatsLoaded: function (parentAreaCode) {
            return isDefined(stats[this._getStatsKey(parentAreaCode)]);
        },

        // Cache GetGroupData responses
        getData: function (sid, areaCode) {
            var key = getKey(sid, areaCode);
            return isDefined(data[key]) ?
				data[key] :
				null;
        },

        setData: function (sid, areaCode, newData) {
            var key = getKey(sid, areaCode);
            data[key] = newData;
        },

        isDataLoaded: function (sid, areaCode) {
            var key = getKey(sid, areaCode);
            return isDefined(data[key]);
        },

        getMetadataHash: function () {
            return loaded.indicatorMetadata[FT.model.groupId];
        }
    };
})();

function addLoadedData(obj) {
    ui.setData(getGroupAndCurrentAreaTypeKey(), FT.model.parentCode, obj);
}

function advanceIndicatorMenuClicked(step) {
    if (!FT.ajaxLock) {
        lock();
        var i = incArrayIndex(groupRoots, getIndicatorIndex(), step);
        setIndicatorIndex(i);
        indicatorChanged(i);

        logEvent('IndicatorSelectedWithArrowButton');
    }
}

function advanceAreaMenuClicked(step) {
    if (!FT.ajaxLock) {
        lock();

        var i = 0;

        var areaCode = FT.model.areaCode;
        var sortedAreas = FT.data.sortedAreas;
        for (i in sortedAreas) {
            if (sortedAreas[i].Code === areaCode) {
                break;
            }
        }

        i = incArrayIndex(sortedAreas, i, step);
        var newAreaCode = sortedAreas[i].Code;

        FT.menus.area.setCode(newAreaCode);

        ftHistory.setHistory();

        pages.goToCurrent();

        unlock();//TODO is this necessary? unlock should be called at end of each page

        logEvent('AreaSelectedWithArrowButton');
    }
}

function searchOptionClicked() {

    if (!FT.ajaxLock) {

        // Elements
        var $searchOption = $('#area-search-link');
        var $searchText = $('#area-search-text');
        var $areaMenu = $('#areaMenu');
        var $areaNextOrBack = $('.area-next-back');

        // Text labels
        var searchLabel = 'Search for an area';
        var returnToMenuLabel = 'Return to area menu';
        var currentText = $searchOption.text();

        if (currentText === searchLabel) {
            $searchOption.text(returnToMenuLabel);
            $areaNextOrBack.hide();
            $areaMenu.hide();
            $searchText.show().focus();

        } else if (currentText === returnToMenuLabel) {
            $searchOption.text(searchLabel);
            $areaNextOrBack.show();
            $areaMenu.show();
            $searchText.hide().val('');
        }
    }
}

function displayAreaSearchOptions(areaTypeId) {

    var $searchOption = $('#area-search-link');
    var $span = $('#area-search-link-box span');

    if (areaTypeId === AreaTypeIds.Practice) {
        // Hide all - search is on map tab
        $searchOption.hide();
        $span.hide();
    } else if (loaded.areaTypes[areaTypeId].IsSearchable) {
        // Area search is only available for the above area types
        $searchOption.show();
        $span.hide();
    } else {
        // Area search is not available for this area type
        $searchOption.hide();
        $span.show();
    }
}

function incArrayIndex(array, index, step) {
    var i = parseInt(index, 10) + step;
    var maxIndex = array.length - 1;
    if (i < 0) {
        i = maxIndex;
    } else if (i > maxIndex) {
        i = 0;
    }
    return i;
}

// Dummy for search equivalent
function addIndicatorIdParameter() {

}

function setAreaType(areaTypeId) {

    var model = FT.model;
    if (model.areaTypeId !== areaTypeId) {
        lock();

        FT.menus.areaType.setTypeId(areaTypeId);
        new ParentTypes(model).matchTypeNameOrChange(FT.menus.parentType.getName());

        ftHistory.setHistory();
        // Get all areas for new area type ID
        ajaxMonitor.setCalls(1);

        getAllAreas(areaTypeId);

        ajaxMonitor.monitor(initAreaData2);
    }
}

function getSignificanceFunction(polarityId) {
    var c = colours;
    var noComparison = c.noComparison, same = c.same;

    if (polarityId === PolarityIds.BlueOrangeBlue) {
        // Use blue
        return function (sig) {
            switch (sig) {
                case 3:
                    // Higher
                    return c.bobHigher;
                case 2:
                    return same;
                case 1:
                    // Lower
                    return c.bobLower;
            }
            return noComparison;
        };
    }

    if (polarityId === -1) {
        return function () { return noComparison; };
    }

    var ragColors = [c.worse, c.better];

    if (polarityId === 0) {
        // High is good
        ragColors.reverse();
    }

    return function (sig) {
        switch (sig) {
            case 1:
                return ragColors[1];
            case 2:
                return same;
            case 3:
                return ragColors[0];
        }
        return noComparison;
    };
}

function updateDomains() {
    var cls = 'selected-domain';
    $('.' + cls).removeClass(cls);
    $('#domain' + FT.model.groupId).addClass(cls);
}

/**
* Event handler for when a user selects an option from area search results
* @class SearchTextValidator
*/
function SearchTextValidator(textEnteredByUser) {
    this.isOk = textEnteredByUser !== 'Indicator keywords';
    if (this.isOk) {
        this.text = textEnteredByUser.replace(
			/*valid word separators*//[-]/g, ' ').replace(
			/*valid word combiners*//[+&]/g, ' and ').replace(
			/*nonalphanumerics*//[^A-Za-z0-9 ]/g, '');
    }
}

function ajaxAreaSearch(text, successFunction, excludeParentAreasFromSearchResults, extraParentAreaTypeIds) {
    var areaTypeId = FT.model.areaTypeId;

    var parentAreas = [];
    if (!excludeParentAreasFromSearchResults) {
        parentAreas.push(FT.model.areaTypeId);
    }
    for (var i in extraParentAreaTypeIds) {
        parentAreas.push(extraParentAreaTypeIds[i]);
    }

    getAreaSearchResults(text, successFunction, areaTypeId, true, parentAreas);
}

/**
* Event handler for when a user selects an option from area search results
* @class areaSearchResultSelected
*/
function areaSearchResultSelected(noMatches, searchResult) {
    if (!noMatches.is(':visible')) {
        $('#area-search-text').val('');
        if (!FT.ajaxLock) {
            lock();

            var model = FT.model;
            var polygonAreaCode = searchResult.PolygonAreaCode;
            var parentAreaCode = new AreaMappings(model).getParentAreaCode(polygonAreaCode);

            if (parentAreaCode) {
                model.areaCode = polygonAreaCode;
                FT.menus.parent.setCode(parentAreaCode);
                setAreas();
                ftHistory.setHistory();
                refreshCurrentPage();
            } else {
                // This may happen for certain areas that are ignored, e.g. City of London, Isles of Scilly
                //TODO - see FIN-191
                unlock();
            }
        }
    }
}

/**
* Returns as HTML a link to export a chart as an image 
* @class showExportChartLink
*/
function showExportChartLink(chartVariable) {
    return '<div class="export-chart-box"><a class="export-link" href="javascript:exportChartAsImage(' + chartVariable
        + ')">Export chart as image</a></div>';
}

/**
* Returns as HTML a link to export a table as an image 
* @class showExportTableLink
*/
function showExportTableLink(containerId, fileNamePrefix, legends) {
    return '<div class="export-chart-box"><a class="export-link" href="javascript:exportTableAsImage(\''
        + containerId + '\',\'' + fileNamePrefix + '\',\'' + legends + '\')">Export table as image</a></div>';
}

/**
* Exports a table as an image. 
* @class exportTableAsImage
*/
function exportTableAsImage(containerId, fileNamePrefix, legends) {

    containerId = '#' + containerId;
    $(containerId + ' .export-chart-box').hide();
    $(containerId + ' .columnSort').hide();
    $(containerId).css('font-family', 'Arial');
    $(containerId).css('background', 'white');
    var container = $(containerId);

    if (legends && legends.length > 0) {
        var legendContainer = $("<div id='legends'></div>");

        $.each(legends.split(','), function (index, legendId) {
            var legend = $(legendId).clone().attr('id', 'legendId').after('#id');
            legendContainer.append(legend);
        });
        $('#legends').css('font-family', 'Arial');
        container.prepend(legendContainer);
    }

    saveElementAsImage(container, fileNamePrefix);

    if (legends) $('#legends').remove();
    $(containerId + ' .export-chart-box').show();
    $(containerId + ' .columnSort').show();

    logEvent('ExportImage', getCurrentPageTitle());
}

function nearestNeighboursSelected() {
    if (!FT.ajaxLock) {
        lock();

        $('#nearest-neighbour-wrapper').css('visibility', 'hidden');

        FT.model.nearestNeighbour = getNearestNeighbourCode();
        // We set the comparatorId to National as its only selectable option
        comparatorId = NATIONAL_COMPARATOR_ID;
        ftHistory.setHistory();
        initAreaData();

        logEvent('NearestNeighbours', 'NearestNeighbourModeSelected');
    }
}

function showAndHideNearestNeighboursMenu() {

    var $neighbourHeader = $('#nearest-neighbour-header');
    var $neighbourLinks = $('#nearest-neighbour-links');
    var $neighbourSelect = $('#nearest-neighbour-wrapper');

    if (FT.model.isNearestNeighbours()) {
        // Nearest neighbours displayed

        var config = getNearestNeighboursConfig();

        // Display neighbours explanation
        var headerHtml = '<span class="nearest-neighbour-selected-area"><h2>' +
            areaHash[FT.model.areaCode].Name + ' ' + nnSelectedText + '</h2></span>';
        $neighbourHeader.html(headerHtml).show();

        // Back text
        var viewModel = {};
        viewModel.text = config.altText
            ? config.altText
            : 'nearest neighbours';

        // More information link
        viewModel.isLink = config.ExtraLink.length > 0;
        if (viewModel.isLink) {
            viewModel.link = config.ExtraLink;
        }

        // Render links
        var templateName = 'neighbours';
        templates.add(templateName, '<a id="exit-nearest-neighbours" onclick="exitNearestNeighboursSelected()" class="a-link">Exit {{text}}</a>' +
            '{{#isLink}}<a href="{{{link}}}" class="a-link" target="_blank">More information</a>{{/isLink}}');
        var linksHtml = templates.render(templateName, viewModel);

        $neighbourLinks.html(linksHtml).show();
        $neighbourSelect.hide();
    } else {
        // Nearest neighbours not displayed
        $neighbourHeader.hide();
        $neighbourLinks.hide();
        $neighbourSelect.show();
        $neighbourSelect.css("visibility", "visible");
    }
}

/**
* Exits nearest neighbours mode. 
* @class exitNearestNeighboursSelected
*/
function exitNearestNeighboursSelected() {

    if (!FT.ajaxLock) {
        lock();

        var model = FT.model;
        model.nearestNeighbour = null;

        showAndHidePageElements();
        ftHistory.setHistory();
        initAreaData();
    }
}

function toggleNearestNeighboursControls(state) {
    var $areaMenuBox = $('#areaMenuBox'),
		$regionMenuBox = $('#region-menu-box'),
		$areaTypeBox = $('#areaTypeBox'),
		$parentTypeBox = $('#parentTypeBox'),
		$nearestNeighboursLink = $('#nearest-neighbour-link');

    if (state) {
        pages.displayElements();
        $nearestNeighboursLink.show();
    } else {
        $areaMenuBox.hide();
        $regionMenuBox.hide();
        $areaTypeBox.hide();
        $parentTypeBox.hide();
        $nearestNeighboursLink.hide();
    }
}

function setNearestNeighbourLinkText() {
    var model = FT.model;

    var area = areaHash[model.areaCode];
    if (doesAreaTypeHaveNearestNeighbours() && area) {
        var config = getNearestNeighboursConfig();

        var name = FT.model.areaTypeId === AreaTypeIds.CCGPreApr2017
            ? area.Short
            : area.Name;

        nnLinkText = config.LinkText + ' ' + name;
        nnSelectedText = ' ' + config.SelectedText;
    } else {
        nnLinkText = '';
        nnSelectedText = '';
    }

    $('#nearest-neighbour-link').text(nnLinkText);
}

/**
* Whether nearest neighbours are available for the current area type
* @class doesAreaTypeHaveNearestNeighbours
*/
function doesAreaTypeHaveNearestNeighbours() {
    return !!FT.config.nearestNeighbour[FT.model.areaTypeId];
}

/**
* Gets the nearest neighbour area code for the current area. 
* @class getNearestNeighbourCode
*/
function getNearestNeighbourCode() {

    var nnCode = '';
    if (doesAreaTypeHaveNearestNeighbours()) {
        var config = getNearestNeighboursConfig();
        nnCode = 'nn-' + config.NeighbourTypeId + '-' + FT.model.areaCode;
    }

    return nnCode;
}

function getNearestNeighboursConfig() {
    return FT.config.nearestNeighbour[FT.model.areaTypeId];
}

FT.model.isNearestNeighbours = function () {
    var model = FT.model;
    switch (model.nearestNeighbour) {
        case undefined:
        case null:
        case "":
            return false;
        default:
            return true;
    }
}

/**
* Display message if there is no data in a domain
* @class displayNoData
*/
function displayNoData() {

    showAndHidePageElements();

    // Hide current tab contents
    $('#' + pages.getCurrentPage().id + '-container').hide();

    // Show no data message
    $('#no-domain-data').show();

    // No indicators to display so hide menu
    $('#indicator-menu-div,#tab-specific-options').hide();

    unlock();
}

function areIndicatorsInDomain() {
    return groupRoots.length > 0;
}

/**
* Event handlers for when a recent trend icon or a tartan rug cell is selected.
* @class recentTrendSelected
*/
var recentTrendSelected = {

    /**
    * Set the history and navigate to the area trends tab.
    * @method _showTrend
    */
    _showTrend: function () {
        ftHistory.setHistory();
        $(window).scrollTop(250);
        goToAreaTrendsPage();
    },

    /**
    * 
    * @method _showTrend
    */
    _setIndicator: function (rootIndex) {
        var root = groupRoots[rootIndex];
        var model = FT.model;
        model.iid = root.IID;
        model.ageId = root.Age.Id;
        model.sexId = root.Sex.Id;
    },

    /**
    * Change the current area code or benchmark.
    * @method _setArea
    */
    _setArea: function (areaCode) {
        if (areaCode === NATIONAL_CODE) {
            FT.menus.benchmark.setComparatorId(NATIONAL_COMPARATOR_ID);
        } else if (areaCode === FT.model.parentCode) {

            var popupWidth = 400,
                left = lightbox.getLeftForCenteredPopup(popupWidth),
                top = $('body').scrollTop() + 200,
                html = '<br><br>' + '<div>The benchmark has been changed to ' + getParentArea().Name + '</div><br><br>';
            lightbox.show(html, top, left, popupWidth);

            FT.menus.benchmark.setComparatorId(REGIONAL_COMPARATOR_ID);
        } else {
            FT.menus.area.setCode(areaCode);
        }
    },

    /**
    * Event handler for when a tartan rug cell is selected
    * @method fromTartanRug
    */
    fromTartanRug: function (areaCode, rootIndex) {
        // Go to trends page
        this._setArea(areaCode);
        this._setIndicator(rootIndex);
        this._showTrend();
    },

    /**
    * Event handler for when a recent trend icon is selected in the area profile
    * @method fromAreaProfile
    */
    byGroupRoot: function (rootIndex) {
        // Go to trends page
        this._setIndicator(rootIndex);
        this._showTrend();
    },

    /**
    * Event handler for when a recent trend icon is selected in the bar chart
    * @method fromCompareArea
    */
    fromCompareArea: function (areaCode) {
        // Go to trends page
        this._setArea(areaCode);
        this._showTrend();
    }
};

var barChart = {
    getBarHtml: function (data, comparisonConfig) {

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

        var h = ['<div class="bar-box"><img class="bar" style="width:', width,
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
    },
    selectBenchmarkClicked: function (comparatorId) {
        if (!FT.ajaxLock) {
            FT.menus.benchmark.setComparatorId(comparatorId);
            benchmarkChanged();
            logEvent('BarChart', 'ComparatorChangedByClickOnBar');
        }
    },
    setIndicatorTableHeaderHtml: function (root, rootIndex) {
        var grouping = getFirstGrouping(root);
       
        var period = isDefined(grouping)
            ? grouping.Period
            : '';

        var html = getTrendHeader(barChartState.metadata, root, period,
            'goToMetadataPage(' + rootIndex + ')', hasDataChanged(root));

        $('#indicator-details-header').html(html);
    }
};

/**
* Finds the indicator stat that matches the group root.
* @class matchBySexAgeAndIID
*/
function matchBySexAgeAndIID(root, list) {
    return _.find(list, function (item) {
        return item.IID === root.IID &&
            item.Sex.Id === root.Sex.Id &&
            item.Age.Id === root.Age.Id;
    });
}

function addOrderandPercentilesToData(coreDataList) {
    // Hash: Key -> area code, Value -> coredataset
    var hash = {},
        areaOrder = [];

    _.each(coreDataList, function (data) {
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
    var areaIndex = 0;
    $.each(areaOrder, function (i, coreData) {
        var data = hash[coreData.AreaCode];
        if (coreData.ValF === '-') {
            data.order = -1;
            data.orderFrac = -1;
        }
        else {
            data.order = numAreas - areaIndex;
            data.orderFrac = 1 - areaIndex / numAreas;
            var position = numAreas + 1 - areaIndex + 1;
            data.quartile = Math.ceil(position / (numAreas / 4));
            data.quintile = Math.ceil(position / (numAreas / 5));
            areaIndex++;
        }
    });

    return hash;
}

function getIndicatorName(indicatorId) {
    return ui.getMetadataHash()[indicatorId].Descriptive.Name;
}

var nnLinkText = '';
var nnSelectedText = '';

// UI
var CSS_CENTER = 'center';
var CSS_NUMERIC = 'numeric';
var CSS_NUMERIC_WITH_TOOLTIP = 'numeric boot-tooltip';
var EMPTY_TD_CONTENTS = '&nbsp;'; // Necessary for empty bordered TDs in IE
var NO_DATA = '<div class="no-data">-</div>';
var VALUE_NOTE = '*';
var MAIN = '#main';
var nextUniqueId = 0;
var GET_METADATA_SYSTEM_CONTENT = 'yes';
var SEARCH_NO_RESULT_TOP_OFFSET = 21;
var stems;
var NEW_DATA_BADGE = '<span style="margin-right:8px;" class="badge badge-success">New data</span>';

// UI State
var cells = null;
var groupRootBeforeAreaTypeChange = null;
FT.data.sortedAreas = null;
var groupRoots = null;
var comparatorId = null;
var areaHash = null;
var preferredGroupRoots = {};
var correctForPolarity = true;
var chart;

$(document).ready(documentReady);

function showHideAreaAddress() {
    var id = "#gp-address-placeholder";
    var $addressBox = $(id);

    if (FT.model.areaTypeId === AreaTypeIds.Practice
        && pages.getCurrent() !== PAGE_MODES.METADATA && pages.getCurrent() !== PAGE_MODES.ENGLAND) {
        var $address = $(id + ' address');
        $address.html('&nbsp;');
        var parameters = new ParameterBuilder().add('area_code', FT.model.areaCode);
        ajaxGet('api/area_address',
            parameters.build(),
            function (result) {
                $address.html(result.Name + ", " + getAddressText(result));
            },
            handleAjaxFailure);
        $addressBox.show();
    } else {
        $addressBox.hide();
    }
}
