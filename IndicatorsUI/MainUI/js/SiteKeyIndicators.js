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

function addTd(html, text, cssClass, tooltip, noteId) {

    var isNote = !!noteId;

    html.push('<td ');

    if (cssClass) {
        html.push(' class="', isNote ? cssClass + ' valueNote' : cssClass, '"');
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

    sid = getGroupAndCurrentAreaTypeKey();

    if (!isDefined(sid) || sid.length == 0) {
        // No data available
        getGroupingDataCallback(null);
    } else {

        var code = FT.model.parentCode;
        if (ui.isDataLoaded(sid, code)) {
            // Data already loaded
            getGroupingDataCallback(ui.getData(sid, code));
        } else {
            // Data not yet loaded
            lock();

            showSpinner();

            var parameters = getGroupDataParameters();

            ajaxGet('GetGroupDataAtDataPoint.ashx', parameters,
                getGroupingDataCallback, handleAjaxFailure
            );
        }
    }
}

function getGroupingDataCallback(obj) {

    if (isDefined(obj)) {

        addLoadedData(obj);

        try {
            groupRoots = obj;
            filterRoots();
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
    configureIndicatorMenu();
        
    menus.parentType.setVisibility();
    menus.areaType.setVisibility();
    menus.parent.setVisibility();
    menus.benchmark.setSubnationalOptionVisibility();

    // Specific UI updates to do everytime
    showMessageIfIndicatorNotAvailableForNewAreaType();
    showEastMidlandsMessage();
    updateNearestNeighbourElements();
    updatePreferredState();

    hideSpinner();
    main.show();
}

function showMessageIfIndicatorNotAvailableForNewAreaType() {
    var metadata = metadataBeforeAreaTypeChange;
    if (metadata) {
        var iid = getSelectedIndicatorId();
        if (iid !== metadata.IID) {
            var popupWidth = 600;
            var left = ($(window).width() - popupWidth) / 2;
            var top = 200;
            var html = '<br><br>' + '<div id="indicator-not-in-area" ><b>' + metadata.Descriptive.Name +
                '</b> is not available for ' + FT.menus.areaType.getName() + '<br></div>' + '<br>' +
                '<input type="button" id="indicator-not-in-area-ok" value="OK" onclick="lightbox.hide();" ><br><br>';
            lightbox.show(html, top, left, popupWidth);
        }

        metadataBeforeAreaTypeChange = null;
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

    var getCode = function() {
        return preferredAreas[model.areaTypeId];
    }

    var containsAreaTypeId = function() {
        return !!getCode();
    }

    this.doesAreaCodeNeedUpdating = function () {
        return !containsAreaTypeId() || getCode() !== model.areaCode;
    }

    this.updateAreaCode = function() {
            preferredAreas[model.areaTypeId] = model.areaCode;
    };

    this.getAreaCode = function() {
        return containsAreaTypeId()
            ? getCode()
            : null;
    };

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
* Update the preferred areas cookie if required
* @class updatePreferredState
*/
function updatePreferredState() {

    // Update preferred area cookie
    var preferredAreas = FT.preferredAreas;
    if (preferredAreas.doesAreaCodeNeedUpdating()) {
        preferredAreas.updateAreaCode();
        Cookies.set('preferredAreas', preferredAreas.serialise());
    }

    // Update preferred area type ID cookie
    var preferredAreaTypeIds = FT.preferredAreaTypeIds;
    if (preferredAreaTypeIds.doesAreaTypeIdNeedUpdating()) {
        preferredAreaTypeIds.updateAreaTypeId();
        Cookies.set('preferredAreaTypes', preferredAreaTypeIds.serialise());
    }
}

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

    var cells = highlightedRowCells = $(td).parent().children();

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
    var cells = highlightedRowCells;

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
    return pages.getCurrentPage().title;
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
            initAreaSearch('#area-search-text', model, true);
        }
        
        getValueNotesCall();
        // Log initial state
        logEvent('DomainSelected', getCurrentDomainName(), 'LandedOn');
        logEvent('TabSelected', pages.getCurrentPage().title, 'LandedOn');
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

    if (!ajaxLock) {
        lock();
        code();
    }
}

function clearSpineTables() {
    // Necessary to do both so tooltips work
    $('#singleIndicatorBody').html('');
    $('#spineBody').html('');
}

function getAreaTypes() {
    
    if (areAnyIndicators) {
        // Check if areaTypes already loaded
        if (_.isUndefined(loaded.areaTypes[FT.model.areaTypeId])) {
            showSpinner();
            getAreaTypesCall(FT.model.profileId);
        } else {
            ajaxMonitor.callCompleted();
        }
    } else {
        getAreaTypesCallback(null);
    }
}

function getAreaTypesCallback(areaTypesArray) {

    if (areaTypesArray) {
        FT.menus.areaType = new AreaTypeMenu(FT.model,
            new AreaTypes(areaTypesArray, loaded.areaTypes));
    }

    ajaxMonitor.callCompleted();
}

function AreaAndDataSorter(order, data, areas, areaCodeToAreaHash) {
   
    var coreDataSetList = new CoreDataSetList(data);
    
    var sortAreasByDataOrder = function() {
        // Sort areas
        var areasSorted = [];
        for (var i in data) {
            areasSorted.push(areaCodeToAreaHash[data[i].AreaCode]);
        }

        // Add missing areas
        if (areasSorted.length == 0) {
            // No data to sort
            areasSorted = areas;
        } else if (areasSorted.length != _.size(areaCodeToAreaHash)) {
            for (var code in areaCodeToAreaHash) {
                var area = areaCodeToAreaHash[code];
                if ($.inArray(area, areasSorted) == -1) {
                    areasSorted.push(area);
                }
            }
        }

        return areasSorted;
    };

    var reverseDataIfNecessary = function() {
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
* Sort SortedAreas by rank
* @class sortedAreasByRank
*/
function sortSortedAreasByRank() {
    var sortedByRank = sortedAreas.slice(0);
    sortedByRank.sort(function (a, b) {
        return a.Rank - b.Rank;
    });
    return sortedByRank; 
}

function isComparatorValueValid(comparatorGrouping) {

    return isDefined(comparatorGrouping) &&
        isDefined(comparatorGrouping.ComparatorData) &&
        comparatorGrouping.ComparatorData.Val != -1;
}

function getCurrentComparator() {
    return getComparatorById(comparatorId);
}

colours = {
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
    var c = 'pageSelected';
    suffix = '#' + suffix;
    $(suffix + 'Selection > a').removeClass(c);
    $(suffix + id).addClass(c);
}

function ComparisonConfig(groupRoot, indicatorMetadata) {

    var _this = this;

    var useTarget = indicatorMetadata.Target &&
        isCheckboxChecked('#target-benchmark');

    _this.useTarget = useTarget;
    _this.useQuintileColouring = groupRoot.ComparatorMethodId == PolarityIds.Quintiles;

    if (_this.useQuintileColouring) {
        _this.showQuintileLegend = true;
    }

    if (useTarget) {
        if (indicatorMetadata.Target.PolarityId == PolarityIds.BlueOrangeBlue) {
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

function getGroupDataParameters() {
    var model = FT.model;
    
    return 'gid=' + model.groupId +
            '&ati=' + model.areaTypeId +
            '&par=' + getParentCode() +
            getProfileOrIndicatorsParameters();
}

function getParentCode() {
    var model = FT.model;        
    return !model.isNearestNeighbours() ? model.parentCode : model.nearestNeighbour;
}

function doSearch() {
    var model = FT.model;

    var searchText = new SearchText($('#searchBox').val());
    if (searchText.isOk) {
        setUrl(FT.url.search + encodeURIComponent(searchText.text) + '#pat/' + model.parentAreaType + '/ati/' + model.areaTypeId + '/par/' + model.parentCode);
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

function getProfileOrIndicatorsParameters() {
    var p = '';
    var pid = FT.model.profileId;
    if (isDefined(pid)) {
        p += '&pid=' + pid;
    }

    p += getIndicatorIdArgument();

    return p;
}

function populateIndicatorMenu() {

    var selectElement = $('#indicatorMenu')[0],
        options = selectElement.options;
    options.length = 0;
    var metadataHash = ui.getMetadataHash();

    for (var i in groupRoots) {
        var root = groupRoots[i],
            indicator = metadataHash[root.IID];
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
                preferredRoot.SexId === root.SexId) {
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
        a: $('#' + suffix + 'Data'),
        b: $('#' + suffix + 'NoData')
    }).showA(isData);
}

function getIndicatorIndex() {
    var model = FT.model;
    if (model.iid !== null) {
        var indicatorIndex = 0, matchingIndex;
        _.each(groupRoots, function (data) {
            if (data.IID === model.iid && 
	    data.SexId === model.sexId && 
	    data.AgeId === model.ageId) {
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

function getTrendHeader(metadata, root, areaName, clickHandler) {

    var unit = metadata.Unit.Label,
        unitLabel = unit !== '' ?
            ' - ' + unit :
            '';

    return ['<div class="trendHeader"><div class="trendTitle"><a class="trendLink" title="More about this indicator" href="javascript:',
        clickHandler, ';">', metadata.Descriptive.Name, new SexAndAge().getLabel(root), '</a>',
        getIndicatorDataQualityHtml(metadata.Descriptive.DataQuality), 
        '<span class="trendArea">', areaName, '</span>', '</div>',
        '<div class="trendUnit">',metadata.ValueType.Label, unitLabel, '</div>',
        '</div>'].join('');
}

function getSource(metadata) {

    var source = metadata.Descriptive.DataSource;

    return isDefined(source) && !String.isNullOrEmpty(source) ?
        '<div class="trendSource">Source: ' + source + '</div>' :
        '';
}

function indicatorChanged(rootIndex) {
    FT.menus.area.setAdditionalParameters(groupRoots[rootIndex].IID, groupRoots[rootIndex].AgeId, groupRoots[rootIndex].SexId);
    ftHistory.setHistory();

    // Enables a specific indicator to be preferentially chosen when the menu is repopulated
    var root = groupRoots[rootIndex];
    preferredGroupRoots[FT.model.groupId] = root;

    if (mode === PAGE_MODES.INDICATOR_DETAILS) {
        displayIndicatorDetails();
    } else {
        pages.goToCurrent();
    }

    logEvent('IndicatorSelectedInMenu',
        ui.getMetadataHash()[root.IID].Descriptive.Name);
}

function showAllIndicatorsChanged(checked) {

    $('#indicatorMenu').prop('disabled', checked);
    pages.goToCurrent();
}

function showAllIndicators() {
    return isCheckboxChecked('#indicatorSelectAll');
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

function noDataForAreaType() {
    goToTartanRugPage();
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

function getSubgroupsByAreaType() {

    ajaxGet('GetData.ashx', 's=sa&gid=' + groupIds.join(','),
        getSubgroupsByAreaTypeCallback, handleAjaxFailure
    );

}

function getSubgroupsByAreaTypeCallback(obj) {

    setViewableGroups(obj);
    ajaxMonitor.callCompleted();
}

function getGroupAndCurrentAreaTypeKey() {
    return getGroupAndAreaTypeKey(FT.model.areaTypeId);
}

function setIndicatorIndex(i) {
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
        noDataForAreaType();
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
    $('.keyText').html('Compared with ' + benchmark + ':');

    // Selected domain
    $('#domain' + model.groupId).addClass('selectedDomain');

    // Benchmark target option
    $('#target-benchmark').click(function () {
        pages.goToCurrent();
    });

    tooltipManager.init();
}

function initAreaData() {        
    ajaxMonitor.setCalls(3);
    getAreaTypes();    
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
    stems = new SpineChartStems(spineHeaders);
}

PARENT_DISPLAY = {
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

    if (areAnyIndicators && !loaded.areaLists[areaTypeId]) {
        showSpinner();

        var parameters = new ParameterBuilder();
        parameters.add('area_type_id', areaTypeId);
        parameters.add('profile_id', FT.model.profileId);
        parameters.add('template_profile_id', templateProfileId);

        ajaxGet('data/areas',
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

/* Special message that is displayed when East Midlands is selected for 
 the MESHA profiles */
function showEastMidlandsMessage() {

    var e = $('#global-prefooter');
    if (e) {
        var parent = FT.model.parentCode;
        if (parent === 'EMREG' || parent === 'EMSHA') {
            e.show();
        } else {
            e.hide();
        }
    }
}

function enableMenus() {
    $('select').prop('disabled', null);
}

function lock() {
    if (!ajaxLock) {
        ajaxLock = 1;

        // Disable menus
        var disabled = 'disabled';
        $('select').attr(disabled, disabled);
    }
}

function unlock() {
    if (ajaxLock) {
        ajaxLock = null;
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
    // Global
    sortedAreas = getChildAreas();
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

    this.areAnyValidTrendValues = function() {
        return this.getValidValues('V').length > 0;
    };

    this.sortByValue = function () {
        list.sort(sortData);
    };

    this.sortByCount = function() {
        list.sort(function(a, b) {
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

function MinMaxFinder(arrayOfNumbers) {
    this.setValues(arrayOfNumbers);
}

MinMaxFinder.prototype = {
    setValues: function (arrayOfNumbers) {

        var areValues = arrayOfNumbers.length > 0;

        if (areValues) {

            this.min = _.min(arrayOfNumbers),
                this.max = _.max(arrayOfNumbers);
        }
        this.isValid = areValues;
    }

};

//
// Shows the appropriate legend for a bar chart page.
//
function showBarChartLegend(useTarget) {
    new MutuallyExclusiveDisplay({
        a: $('#keyAdHoc'),
        b: $('#key-bar-chart')
    }).showA(useTarget);
}

//
// Displays a legend for the target
//
function setTargetLegendHtml(comparisonConfig, metadata) {
    if (comparisonConfig.useTarget) {
        var targetLegend = getTargetLegendHtml(comparisonConfig, metadata);
        $('#keyAdHoc').html(
                '<div><table class="keyTable"><tr><td class="keyText">Benchmarked against goal:</td><td class="keySpacer"></td><td>' +
                targetLegend + '</td></tr></table></div>').show();
    }
}

//
// Get HTML for target legend
//
function getTargetLegendHtml(comparisonConfig, metadata) {

    if (comparisonConfig.useTarget) {

        var targetConfig = metadata.Target,
            bespokeKey = targetConfig.BespokeKey,
            start = '<span class="target ',
            end = '</span>';
        var worseOrBetter = targetConfig.PolarityId == PolarityIds.BlueOrangeBlue
            ? ['bobLower', 'bobHigher']
            : ['worse', 'better'];

        if (bespokeKey) {
            // Bespoke comparison
            if (bespokeKey === 'last-year-england') {
                return start + worseOrBetter[0] + '">&lt;previous year\'s England value' + end +
                    start + worseOrBetter[1] + '">&ge;previous year\'s England value' + end;
            }

            if (bespokeKey === 'nth-percentile-range') {

                var lowerTargetPercentile = targetConfig.LowerLimit;
                var upperTargetPercentile = targetConfig.UpperLimit;

                // Red, amber green
                if (targetConfig.PolarityId === 0) {    // Lower is green
                    
                    return start + worseOrBetter[0] + '">&gt;' + upperTargetPercentile + 'th-percentile of UTLAs' + end +
                        start + 'same' + '">&le;' + upperTargetPercentile + 'th to ' + '&gt;' + lowerTargetPercentile + 'th' + end +
                        start + worseOrBetter[1] + '">&le;' + lowerTargetPercentile + 'th' + end;
                } 
                
                if (targetConfig.PolarityId === 1) {    // Upper is green
                    return start + worseOrBetter[0] + '">&lt;' + lowerTargetPercentile + 'th-percentile of UTLAs' + end +
                    start + 'same' + '">&ge;' + lowerTargetPercentile + 'th to ' + '&lt;' + upperTargetPercentile + 'th' + end +
                    start + worseOrBetter[1] + '">&ge;' + upperTargetPercentile + 'th' + end;
                }
            }

            return '';
        }

        var lowerLimit = new CommaNumber(targetConfig.LowerLimit).unrounded(),
        upperLimit = targetConfig.UpperLimit,
        suffix = new ValueSuffix(metadata.Unit).getShortLabel(),
        html;

        // Switch colouring if low is good
        if (targetConfig.PolarityId === 0) {
            worseOrBetter = worseOrBetter.reverse();
        }

        // Worse
        html = [start, worseOrBetter[0], '">&lt;', lowerLimit, suffix, end];

        // Same
        if (upperLimit) {
            upperLimit = new CommaNumber(upperLimit).unrounded();
            html.push(start, 'same">', lowerLimit, suffix, ' to ', upperLimit, suffix, end);
        } else {
            upperLimit = lowerLimit;
        }

        // Better
        html.push(start, worseOrBetter[1], '">&ge;', upperLimit, suffix, end);

        return html.join('');
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
        var builder = new ParameterBuilder().add('pid', FT.model.profileId);
        getData(getParentAreaGroupsCallback, 'pg', builder.build());
    } else {
        ajaxMonitor.callCompleted();
    }
}

function getParentAreaGroupsCallback(obj) {

    loaded.parentAreaGroups = obj;
    new ParentTypes(FT.model).setDefault();
    ajaxMonitor.callCompleted();
}

function initAreaData2() {
    var model = FT.model;
    if (!loaded.areaTypes[model.areaTypeId]) {
        // Area type id that was restored from cookie is available for current profile anymore
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

    if (areAnyIndicators && !new AreaMappings(model).areDefined()) {
        var builder = new ParameterBuilder();
        builder.add('pid', model.profileId);
        builder.add('ati', model.areaTypeId);
        builder.add('tem', templateProfileId);
        builder.add('pat', model.parentTypeId);
        builder.add('nn', model.nearestNeighbour);

        getData(getAreaMappingsCallback, 'am', builder.build());
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
        if (!ajaxLock) {
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
        if (!ajaxLock) {
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
            :'Short';

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
        } catch (e) {}
    };

    _this.setVisibility = function() {
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
}

function AreaMenu(jquerySelector, model) {
    var _this = this,
        $menu = $(jquerySelector),
        $box = $('#areaMenuBox');

    $menu.change(function () {
        if (!ajaxLock) {
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
 
    this.setComparatorId = function(id) {
        comparatorId = id;
        $menu.val(id);
    }

    this.getComparatorId = function () {
        return parseInt($menu.val());
    }

    this.setSubnationalOptionVisibility = function () {
        if(model.parentTypeId === AreaTypeIds.Country ||
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

            _.each(areaTypesArray, function (a) {
                if (a.Id === model.areaTypeId) {
                    a.selected = selected;
                }
            });

            templates.add('areaTypes',
                '{{#types}}<option value="{{Id}}" {{selected}} title="{{Name}}">{{Short}}</option>{{/types}}');

            return templates.render('areaTypes', { types: areaTypesArray });
        };
    $box.html(
        '<li><select id="' + id + '">' + getAreaTypeOptions() +
        '</select></li><li class="heading">Area type:</li>');

    var $menu = $('#' + id),
        selectedOption = function () {
            return $menu.find('option:' + selected);
        };

    $menu.change(function () {

        if (!ajaxLock) {

            // So that we know when to show the user a message saying the current
            // indicator isn't available for the new area type
            var metadata = ui.getMetadataHash();
            var iid = getSelectedIndicatorId();
            metadataBeforeAreaTypeChange = metadata[iid];

            var areaTypeId = parseInt(selectedOption().val());
            setAreaType(areaTypeId);
            setAreaSearchOptions(areaTypeId);

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
            areaMappings.getParentAreaCode(model.areaCode) === null /* area code from another area type that doesn't contain the child area */) {
            model.areaCode = FT.preferredAreas.getAreaCode();
        }
    }

    var menus = FT.menus;
    menus.parent.setOptions();
    menus.parentType.setOptions();
    setAreas();
    setAreaSearchOptions(FT.model.areaTypeId);

    if (model.parentTypeId === AreaTypeIds.Country) {
        // Subnational benchmark not valid
        menus.benchmark.setComparatorId(NATIONAL_COMPARATOR_ID);
    }

    // Update all elements that display the name of the parent area type
    $('.parent-area-type').html(menus.parentType.getName());
    refreshCurrentPage();
}

function benchmarkChanged() {

    if (!ajaxLock) {

        lock();

        comparatorId = FT.menus.benchmark.getComparatorId();

        pages.goToCurrent();

        logEvent('BenchmarkSelected', comparatorId === NATIONAL_COMPARATOR_ID
            ? 'National'
            : 'Subnational');
    }
}

function domainClicked(groupId) {
    var model = FT.model;
    if (model.groupId !== groupId && !ajaxLock) {

        lock();
        model.groupId = groupId;
        ftHistory.setHistory();
        refreshCurrentPage();

        updateDomains();

        logEvent('DomainSelected', getCurrentDomainName(), 'UserSelection');
    }
}

function getCurrentDomainName() {
    return $.trim($('#domain' + FT.model.groupId).text());
}

function populateIndicatorMenus() {
    populateIndicatorMenu();
}

function indicatorNameClicked(id) {
    goToIndicatorDetailsPage(id);
}

VIEW_MODES = {
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
ui = (function () {

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

function filterTrendRoots() {
    // Placeholder overriden by Inequality profile
}

function filterRoots() {
    // Placeholder overriden by Inequality profile
}

function exportExcel(parentCode) {

    var model = FT.model;

    setUrl(FT.url.corews + 'GetDataDownload.ashx?pid=' + model.profileId +
        '&ati=' + model.areaTypeId +
        getIndicatorIdArgument() + getRestrictByProfileParameter() +
        '&tem=' + templateProfileId +
        '&par=' + parentCode +
        '&pds=' + enumParentDisplay +
        '&pat=' + model.parentTypeId
    );

    logEvent('Download', 'Excel', parentCode);
}

function advanceIndicatorMenuClicked(step) {
    if (!ajaxLock) {
        lock();
        var i = incArrayIndex(groupRoots, getIndicatorIndex(), step);
        setIndicatorIndex(i);
        indicatorChanged(i);

        logEvent('IndicatorSelectedWithArrowButton');
    }
}

function advanceAreaMenuClicked(step) {
    if (!ajaxLock) {
        lock();

        var areaCode = FT.model.areaCode,
            i = 0;

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

        unlock();

        logEvent('AreaSelectedWithArrowButton');
    }
}

function searchOptionClicked() {
    var $searchOption = $('#area-search-link');
    var $searchText = $('#area-search-text');
    var $areaMenu = $('#areaMenu');
    var $areaNextOrBack = $('.area-next-back');
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

function canAreaTypeBeSearched(areaTypeId) {
    return _.contains([
        AreaTypeIds.Region,
        AreaTypeIds.CCG, AreaTypeIds.DistrictUA,
        AreaTypeIds.Subregion, AreaTypeIds.CountyUA,
        AreaTypeIds.PheCentres2013, AreaTypeIds.PheCentres2015
    ], areaTypeId);
}

function setAreaSearchOptions(areaTypeId) {

    var $searchOption = $('#area-search-link');
    var $span = $('#area-search-link-box span');

    if (canAreaTypeBeSearched(areaTypeId)) {
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

function getIndicatorIdArgument() {
    return '';
}

/* Determines whether or not the indicator menu is enabled */
function configureIndicatorMenu() {

    var menuDisabled = areaTrendsState.areaSwitch &&
        areaTrendsState.areaSwitch.getOption() === VIEW_MODES.AREA && 
        pages.isTrue('showAllAvailable') ?
        showAllIndicators() :
        '';

    $('#indicatorMenu').prop('disabled', menuDisabled);
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
    var cls = 'selectedDomain';
    $('.' + cls).removeClass(cls);
    $('#domain' + FT.model.groupId).addClass(cls);
}

function SearchText(textEnteredByUser) {
    this.isOk = textEnteredByUser !== 'Indicator keywords';
    if (this.isOk) {
        this.text = textEnteredByUser.replace(
            /*valid word separators*//[-]/g, ' ').replace(
            /*valid word combiners*//[+&]/g, ' and ').replace(
            /*nonalphanumerics*//[^A-Za-z0-9 ]/g, '');
    }
}

function ajaxAreaSearch(text, successFunction) {
    getAreaSearchResults(text, successFunction, FT.model.areaTypeId, true);
}

// Selected address
function areaSearchResultSelected(noMatches, searchResult) {
    if (!noMatches.is(':visible')) {
        $('#area-search-text').val('');
        if (!ajaxLock) {
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
* Returns group root for selected indicator
* @method getGroupRoot
*/
function getGroupRoot() {
    return groupRoots[getIndicatorIndex()];
}

/**
* Creates area switch control
* @class AreaSwitch
*/
function AreaSwitch(options) {

    var selectedClass = 'selected';
    var selectedOption = 0;
    var templateName = 'areaSwitch';
    var $areaSwitch = $('#areaSwitch');

    /**
    * Sets the option that is selected.
    * @method setOption
    */
    this.setOption = function(option) {
        selectedOption = option;
    };

    /**
    * Gets the option that is selected.
    * @method getOption
    */
    this.getOption = function() {
        return selectedOption;
    };

    /**
    * Returns compiled html for area switch
    * @method getHtml
    */
    this.clearHtml = function () {
        $areaSwitch.html('');
    }

    /**
    * Returns compiled html for area switch
    * @method getHtml
    */
    this.setHtml = function (model) {

        templates.add(templateName, template);

        // model for switchTmpl
        var displayOptions = {
            label: model.label,
            topOptionText: model.topOptionText,
            bottomOptionText: model.bottomOptionText,
            class0: '',
            class1: ''
        };

        // Option to display as selected
        displayOptions['class' + selectedOption] = selectedClass;

        var html = templates.render(templateName, displayOptions);
        $areaSwitch.html(html);

        this.bind(model);
    };

    /**
    * Bind on click event for topOption and bottomOption
    * @method bind
    */
    this.bind = function (model) {
        var $topOption = $('#topOption');
        var $bottomOption = $('#bottomOption');

        $topOption.bind('click', function (e) {
            e.preventDefault();

            selectedOption = 0;
            $bottomOption.removeClass(selectedClass);
            $topOption.addClass(selectedClass);

            options.eventHanders[0]();

            logEvent(getCurrentPageTitle(), options.eventName, model.topOptionText);
        });

        $bottomOption.bind('click', function (e) {
            e.preventDefault();

            selectedOption = 1;
            $topOption.removeClass(selectedClass);
            $bottomOption.toggleClass(selectedClass);

            options.eventHanders[1]();

            logEvent(getCurrentPageTitle(), options.eventName, model.bottomOptionText);
        });
    };

    var template = '<table><tr>' +
    '<td style="text-align:right;">{{label}}: </td>' +
    '<td>' +
    '<a id="topOption" href="#" class={{class0}} >{{topOptionText}}</a>' +
    '<br>' +
    '<a id="bottomOption" href="#" class={{class1}}>{{bottomOptionText}}</a>' +
    '</td></tr></table>';
};

function setNearestNeighbourLinkText() {
    var areaTypeId = FT.model.areaTypeId;
    var profileId = FT.model.profileId;

    if (profileId !== ProfileIds.ChildHealth &&
        (areaTypeId === AreaTypeIds.CountyUA || areaTypeId === AreaTypeIds.DistrictUA)
        ) {
        var areaName = areaHash.hasOwnProperty(FT.model.areaCode)
        ? areaHash[FT.model.areaCode].Name
            : '?????';
        var text = 'CIPFA nearest neighbours to ' + areaName;
    } else {
        text = '';
    }

    $('#nearest-neighbour-link').text(text);
}

function findNearestNeighbours() {
    if (!ajaxLock) {
        lock();

        $('#nearestNeighbours-wrapper').css('visibility', 'hidden');

        FT.model.nearestNeighbour = getNearestNeighbourCode();
        // We set the comparatorId to National as its only selectable option
        comparatorId = NATIONAL_COMPARATOR_ID;
        ftHistory.setHistory();              
        initAreaData();

        logEvent('NearestNeighbours', 'NearestNeighbourModeSelected');
    }
}

function showAndHideNearestNeighboursMenu() {
    var goBackInnerHtml = '';
    var selectedNearestNeighboursInnerHtml = '';

    if (FT.model.isNearestNeighbours()) {
        goBackInnerHtml = '<a onclick="backToMenu()" class="a-link">Exit nearest neighbours</a><a href="/documents/Nearest_Neighbour_Methodology.docx" class="a-link">More information</a>';
        selectedNearestNeighboursInnerHtml = '<span class="nearest-neighbour-selected-area"><h2>' +
            areaHash[FT.model.areaCode].Name + '</h2> <span>and its CIPFA nearest neighbours</span><div id="nearest-neighbour-extra-text">To send feedback, or to provide suggestions regarding this new feature please contact  <a href="mailto:ProfileFeedback@phe.gov.uk" class="a-link">ProfileFeedback@phe.gov.uk</a></div></span>';
    } else {
        $('#nearestNeighbours-wrapper').css('visibility', 'visible');
    }

    $('#goBack').html(goBackInnerHtml);
    $('#selected-nearest-neighbour').html(selectedNearestNeighboursInnerHtml);
}

function backToMenu() {

    if (!ajaxLock) {
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

function getNearestNeighbourCode() {    
    return  'nn-1-' + FT.model.areaCode;
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
 * Wrapper around the html2canvas, takes the element as save it as an image
 * @method saveElementAsImage
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
        useCORS: true,
    });
}

// UI
CSS_CENTER = 'center';
CSS_NUMERIC = 'numeric';
EMPTY_TD_CONTENTS = '&nbsp;'; // Necessary for empty bordered TDs in IE
NO_DATA = '<div class="noData">-</div>';
VALUE_NOTE = '*';
MAIN = '#main';
allJQs = [];
nextUniqueId = 0;
GET_METADATA_SYSTEM_CONTENT = 'yes';
SEARCH_NO_RESULT_TOP_OFFSET = 21;

// UI State
cells = null;
metadataBeforeAreaTypeChange = null;
sortedAreas = null;
groupRoots = null;
comparatorId = null;
areaHash = null;
preferredGroupRoots = {};

$(document).ready(documentReady);

correctForPolarity = true;
areaState = {};