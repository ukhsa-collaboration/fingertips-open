/**
* Functions of the indicator search.
* @module indicatorSearch
*/

var indicatorSearch = {};


function getGroupingData() {

    if (!indicatorIdList.anyForAreaType(FT.model.areaTypeId)) {
        getGroupingDataCallback(null);
    } else {

        var sid = getGroupAndCurrentAreaTypeKey();

        // Subgroup purely used as ID for caching data on client
        var code = FT.model.parentCode;
        if (ui.isDataLoaded(sid, code)) {
            getGroupingDataCallback(ui.getData(sid, code));
        } else {

            showSpinner();

            var parameters = new ParameterBuilder();
            addGroupDataParameters(parameters);
            addProfileOrIndicatorsParameters(parameters);
            addRestrictToProfilesIdsParameter(parameters);

            ajaxGet('api/latest_data/specific_indicators_for_child_areas', parameters.build(),
                getGroupingDataCallback, handleAjaxFailure);
        }
    }
};

function getTrendData() {

    var parentCode = FT.model.parentCode;

    if (areaTrendsState.isTrendDataLoaded(parentCode)) {
        getTrendDataCallback(areaTrendsState.getTrendData(parentCode));
    } else {

        showSpinner();

        var parameters = new ParameterBuilder(
            ).add('area_type_id', FT.model.areaTypeId
            ).add('parent_area_code', parentCode);

        addRestrictToProfilesIdsParameter(parameters);
        addProfileOrIndicatorsParameters(parameters);

        ajaxGet('api/trend_data/specific_indicators_for_child_areas', parameters.build(), getTrendDataCallback);
    }
};

function addSearchLink(h, text, areaTypeId, cssClass) {

    h.push('<td id="search-area-type-', areaTypeId,
        '" class="search-area-type ', cssClass, '" onclick="setAreaType(',
        areaTypeId, ');">', text, '</td>');
};

function getSearchResults() {

    var parameters = new ParameterBuilder().add('search_text', searchText);

    addRestrictToProfilesIdsParameter(parameters);

    ajaxGet('api/indicator_search',
        parameters.build(),
        function (areaTypeToIndicatorIdsMap) {
            indicatorIdList = new IndicatorIdList(areaTypeToIndicatorIdsMap);
            ajaxMonitor.callCompleted();
        }, handleAjaxFailure);

    logEvent('Search', 'IndicatorSearch', searchText);
}


function initData() {
    initSearch();
    stems = new SpineChartStems(spineHeaders);
    getValueNotesCall();
}

function getAreaTypes() {

    if (!FT.menus.areaType) {
        getAreaTypesCall(FT.model.profileId);
    } else {
        ajaxMonitor.callCompleted();
    }
}

function SearchResultSummary(indicatorIdList) {

    var messageId = 'searchResultText';
    $('<br><div id="' + messageId + '" class="standardWidth">&nbsp;</div>').insertBefore('#main');
    var $label = $('#' + messageId);
    var indicatorProfileOriginLink = 'indicator-profile-origin-link';
    $('<br><div id="' + indicatorProfileOriginLink + '" class="standardWidth" onclick="showIndicatorProfileOrigin()"><a> Show me the profiles these indicators are from</a></div>'
        ).insertAfter('#' + messageId);

    this.display = function (selectedAreaTypeId) {

        var h = ['<table class="fl"><tr>'];

        addTd(h, 'Search results for <i><b>' + searchText + '</i></b>&nbsp;&nbsp;');

        var areaTypes = new AreaTypes().getAreaTypes();
        for (var i in areaTypes) {

            var areaTypeId = areaTypes[i].Id,
            count = indicatorIdList.getIndicatorCount(areaTypeId);

            var selected = areaTypeId === selectedAreaTypeId ?
                'selected' :
                '';

            addSearchLink(h, areaTypes[i].Short + ' (' + count + ')',
                areaTypeId, selected);
        }

        h.push('<tr/></table>');

        $label.html(h.join(''));
    }
}

function initSearch() {
    if (!indicatorIds) {

        ajaxMonitor.setCalls(2);

        getSearchResults();

        // Need area types to display the search result message
        getAreaTypes();

        ajaxMonitor.monitor(searchResultsLoaded);
    }
    else {
        searchResultsLoaded();
    }
}

/**
* AJAX call to fetch the area types for a profile.
* @class getAreaTypes
*/
function getAreaTypes() {
    var parameters = new ParameterBuilder().add('profile_ids',
        getProfileIds(FT.model.areaTypeId));

    ajaxGet('api/area_types',
        parameters.build(),
        function (areaTypes) {
            FT.menus.areaType = new AreaTypeMenu(FT.model,
                new AreaTypes(areaTypes, loaded.areaTypes));
            ajaxMonitor.callCompleted();
        });
}

function displaySearchFindsNothing() {
    $('#searchContents').html(
        '<div class="tallCentralMessage">No matching indicators found for<div id="noMatch">' +
            searchText + '</div>Please click <a href="/">Home</a> to browse indicators by theme, domain or area</div>');

}

function searchResultsLoaded() {
    var model = FT.model;
    model.areAnyIndicators = indicatorIdList.any();

    if (model.areAnyIndicators) {

        // If parentCode defined then use URL hash parameters
        var useModelForAreaTypeId = !!model.parentCode;

        var areaTypeId = useModelForAreaTypeId ?
            model.areaTypeId :
            indicatorIdList.areaTypeIdWithMostIndicators();

        new SearchResultSummary(indicatorIdList).display(areaTypeId);

        FT.menus.areaType.setTypeId(areaTypeId);
    } else {
        displaySearchFindsNothing();
    }

    initAreaData();
}

function getIndicatorIdArgument() {

    var areaTypeId = FT.model.areaTypeId;

    return indicatorIdList.anyForAreaType(areaTypeId) ?
        '&iids=' + indicatorIdList.getIds(areaTypeId) :
        indicatorIds = [];
}

function addIndicatorIdParameter(parameters) {

    var areaTypeId = FT.model.areaTypeId;

    if (indicatorIdList.anyForAreaType(areaTypeId)) {
        parameters.add('indicator_ids', indicatorIdList.getIds(areaTypeId).join(','));
    } else {
        indicatorIds = [];
    }
}

function IndicatorIdList(areaTypeToIndicatorIdsMap) {

    var totalCounts = maxIndicators = 0,
    _this = this,
    allAreaTypeIds = _(areaTypeToIndicatorIdsMap).keys(),
    areaTypeIdWithMostIndicators;

    _this.any = function () {
        return totalCounts > 0;
    };

    _this.anyForAreaType = function (areaTypeId) {
        return areaTypeToIndicatorIdsMap[areaTypeId].length > 0;
    };

    _this.areaTypeIdWithMostIndicators = function () {
        return parseInt(areaTypeIdWithMostIndicators);
    };

    _this.getIndicatorCount = function (areaTypeId) {
        return areaTypeToIndicatorIdsMap[areaTypeId].length;
    };

    _this.getIds = function (areaTypeId) {
        return areaTypeToIndicatorIdsMap[areaTypeId];
    };

    _this.getAllIds = function () {
        return _(_(_(areaTypeToIndicatorIdsMap).toArray()).flatten()).union();
    };

    _.each(allAreaTypeIds, function (areaTypeId) {
        var count = _this.getIndicatorCount(areaTypeId);

        if (count > maxIndicators) {
            maxIndicators = count;
            areaTypeIdWithMostIndicators = areaTypeId;
        }
        totalCounts += count;
    });
}

function canDataBeDisplayed() {

    var areaTypeId = FT.model.areaTypeId;

    // Side effect - this method always called if area type changes
    highlightSearchSummaryAreaType(areaTypeId);

    return indicatorIdList.anyForAreaType(areaTypeId);
}

function highlightSearchSummaryAreaType(areaTypeId) {

    var selected = 'selected';

    // Clear existing selection
    $('#searchResultText td').removeClass(selected);

    // Highlight specified area type
    $('#search-area-type-' + areaTypeId).addClass(selected);
}

function showIndicatorProfileOrigin() {

    if (!FT.ajaxLock) {
        lock();
        var model = FT.model;
        var indicatorIds = indicatorIdList.getIds(model.areaTypeId);

        var parameters = new ParameterBuilder(
        ).add('indicator_ids', indicatorIds
        ).add('area_type_id', model.areaTypeId);

        ajaxGet('api/profiles_containing_indicators', parameters.build(),
            function (data) {
                searchState.profilesPerIndicator = data;

                displayProfilesForIndicatorPopUp();

                var iid = indicatorSearch.getSelectedOriginIndicatorId();
                renderProfilesForSelectedIndicator(iid);
            });
    }
}

function displayProfilesForIndicatorPopUp() {

    var html = '<div id="profiles-for-indicators">' +
        '<h2>Originating profiles</h2>' +
        '<span>Select an indicator to see which profiles it is included in.</span>' +
        '<table><tr><td class="label">Indicator:</td>' +
        '<td><div id="indicator-list"></div></td></tr></table>' +
        '<div class="label">Profiles:</div>' +
        '<div id="profiles-for-selected-indicator"></div></div>';

    // Must unlock before lightbox is shown
    unlock();

    // Show lightbox
    var popupWidth = 900;
    var left = ($(window).width() - popupWidth) / 2;
    var top = 200;
    lightbox.show(html, top, left, popupWidth);

    // Display pop up indicator menu
    var $indicatorMenu = $('#indicatorMenu');
    var $indicatorMenuCloned = $indicatorMenu.clone().prop('id', 'search-indicator-list');
    $('#indicator-list').append($indicatorMenuCloned);

    // Init pop up indicator menu
    var $popUpIndicatorMenu = $('#search-indicator-list');
    $popUpIndicatorMenu.bind('change', function () {
        var iid = indicatorSearch.getSelectedOriginIndicatorId();
        renderProfilesForSelectedIndicator(iid);
    });
    $popUpIndicatorMenu.val($indicatorMenu.val());
}

/**
* Get the ID of the indicator selected in the origin profiles pop up.
* @class indicatorSearch.getSelectedOriginIndicatorId
*/
indicatorSearch.getSelectedOriginIndicatorId = function () {
    var groupRootIndex = $('#search-indicator-list').val();
    return groupRoots[groupRootIndex].IID;
}

function renderProfilesForSelectedIndicator(indicatorId) {
    var profiles = searchState.profilesPerIndicator[indicatorId];
    var template = templates.render('profiles-per-indicator', { data: profiles });
    $('#profiles-for-selected-indicator').html(template);
}

searchState = {
    profilesPerIndicator: {}
};

templates.add('profiles-per-indicator',
    '<div><ul>{{#data}} <li><span><a href="{{{Url}}}" target="_blank">{{ProfileName}}</a><span>{{/data}}</ul></div>');
