
/**
* Functions of the area search partial page.
* @module areaSearch
*/

var areaSearch = {};

areaSearch.hideSearchComponents = function() {
    $('#area-search').hide();
    showSpinner();
}

// Override existing function
function areaSearchResultSelected($noMatches, result) {

    var searchTypes = areaSearch.searchTypes;
    var placeName = result.PlaceName;
    var searchType = placeName === result.PolygonAreaName
        ? searchTypes.parentArea
        : searchTypes.placeName;

    areaSearch.hideSearchComponents();

    logEvent('AreaSearchSelection', placeName);

    // Change URL this way to ensure event is logged
    setTimeout(function () {
        setUrl('/profile/' + profileUrlKey +
         '/area-search-results/' + result.PolygonAreaCode +
         '?place_name=' + placeName +
         '&search_type=' + searchType);
    }, 0);
}

/**
* Initialises the _IntroductionAreaSearch.cshtml partial.
* @class areaSearch.init
*/
areaSearch.init = function () {

    // Init area search menu
    var areaSearchSelector = '#area-search-text';
    initAreaSearch(areaSearchSelector, FT.model);
    $(areaSearchSelector).show();

    // Populate region menu
    areAnyIndicators = true;
    ajaxMonitor.setCalls(1);
    getAllAreas(AreaTypeIds.Region);
    ajaxMonitor.monitor(areaSearch.showParentMenu);
};

/**
* Populates the parent menu with options and navigates to the
* result page when the selected option is changed.
* @class areaSearch.showParentMenu
*/
areaSearch.showParentMenu = function () {

    var areas = loaded.areaLists[AreaTypeIds.Region];
    _.each(areas, function (area) { area.Name = area.Short; });
    var $menu = $('#parent-area-menu');
    populateAreaMenu(areas, $menu, 'SELECT A REGION >');
    $menu.on('change', function () {

        areaSearch.hideSearchComponents();

        var placeName = $menu.find('option:selected').text();

        logEvent('RegionMenuSelection', placeName);

        // Change URL this way to ensure event is logged
        setTimeout(function() {
            setUrl('/profile/' + profileUrlKey + '/area-search-results/' + $menu.val() +
             '?search_type=' + areaSearch.searchTypes.listChildAreas +
             '&place_name=' + placeName);
        },0);
    });

    // Display all components now loading finished
    hideSpinner();
    $('#area-search').show();
}

/**
* Enum of the possible search types
* @class areaSearch.searchTypes
*/
areaSearch.searchTypes = {
    placeName: 'place-name',
    listChildAreas: 'list-child-areas',
    parentArea: 'parent-area'
};


/**
* Functions of the area search results page.
* @module areaSearchResults
*/

var areaSearchResults = {};

/**
* Initialises the AreaSearchResults.cshtml page.
* @class areaSearch.init
*/
areaSearchResults.init = function () {

    var parentAreaCode = state.parentAreaCode;

    // Check in case ignored area, e.g. Isles of Scilly
    if (_.contains(state.areasToIgnore, parentAreaCode)) {
        $('#no-data-message').show();
        return;
    }

    // Get child areas
    var searchTypes = areaSearch.searchTypes;
    ajaxMonitor.setCalls(3);

    // Call 1
    areaSearchResults.getChildAreas(AreaTypeIds.DistrictUA);

    // Call 2
    ajaxGet('data/areas', 'area_codes=' + parentAreaCode, function (areaList) {
        loaded.areaLists[AreaTypeIds.CountyUA] = areaList;
        ajaxMonitor.callCompleted();
    });

    // Call 3
    switch (state.searchType) {
        case searchTypes.placeName:
            getAreaSearchResults(state.placeName,
                areaSearchResults.areaSearchCallback, AreaTypeIds.DistrictUA);
            break;
        case searchTypes.parentArea:
            ajaxMonitor.callCompleted();
            break;
        case searchTypes.listChildAreas:
            areaSearchResults.getChildAreas(AreaTypeIds.CountyUA);
            break;
    }

    ajaxMonitor.monitor(areaSearchResults.prepareViewModel);
};

/**
* Page specific callback for an place name search.
* @class areaSearchResults.areaSearchCallback
*/
areaSearchResults.areaSearchCallback = function (results) {
    loaded.searchResults = results;
    ajaxMonitor.callCompleted();
}

/**
* Gets codes of the child areas
* @class areaSearchResults.getChildAreas
*/
areaSearchResults.getChildAreas = function (areaTypeId) {

    var model = FT.model;
    var parameters = new ParameterBuilder(
    ).add('profile_id', model.profileId
    ).add('parent_area_code', state.parentAreaCode
    ).add('area_type_id', areaTypeId);

    // Get child area codes
    ajaxGet('data/areas', parameters.build(), function (areaList) {
        loaded.areaLists[areaTypeId] = areaList;
        ajaxMonitor.callCompleted();
    });
}

/**
* Contains functions for modifying and filtering an area list.
* @class areaSearchResults.AreaListProcessor
*/
areaSearchResults.AreaListProcessor = function (areaList) {

    /**
    * Assigns a value to the property 'CompositeAreaTypeId' for each area, e.g. 101, 102
    * @method assignCompositeAreaTypeId
    */
    this.assignCompositeAreaTypeId = function () {
        for (var i in areaList) {
            var area = areaList[i];
            area.CompositeAreaTypeId = area.AreaTypeId === AreaTypeIds.District
                ? AreaTypeIds.DistrictUA
                : AreaTypeIds.CountyUA;
        }
        return this;
    };

    /**
    * Removes the Unitary Authorities.
    * @method removeUAs
    */
    this.removeUAs = function () {
        areaList = _.filter(areaList, function (area) {
            return area.AreaTypeId !== AreaTypeIds.UnitaryAuthority;
        });
        return this;
    };

    /**
    * Get the processed area list.
    * @method getAreaList
    */
    this.getAreaList = function () {
        return areaList;
    };
}

/**
* Finds the first area that is included in the search results.
* @class areaSearchResults.findAreaIncludedInSearchResults
*/
areaSearchResults.findAreaIncludedInSearchResults = function (areaList, results) {
    var allowedCodes = _.pluck(areaList, 'Code');

    // Find the result that is the child of the parent
    var result = _.find(results,
        function (result) { return _.contains(allowedCodes, result.PolygonAreaCode); });

    // Find the child area
    var childAreaCode = result.PolygonAreaCode;
    var area = _.find(areaList,
        function (area) { return area.Code === childAreaCode; });

    return [area];
}

/**
* Prepares the view model for the area search results.
* @class areaSearchResults.prepareViewModel
*/
areaSearchResults.prepareViewModel = function () {

    var ns = areaSearchResults;
    var areaList101 = loaded.areaLists[AreaTypeIds.DistrictUA];
    var areaList102 = loaded.areaLists[AreaTypeIds.CountyUA];

    // Filter 101 list to find search result
    if (loaded.searchResults) {
        areaList101 = ns.findAreaIncludedInSearchResults(areaList101,
            loaded.searchResults);
    }

    // Remove UAs
    areaList101 = new ns.AreaListProcessor(areaList101).removeUAs().getAreaList();

    // Only show area type header if more than one area
    var showAreaTypeHeader = (areaList101.length + areaList102.length) > 1;

    var areaLists = [areaList102, areaList101];
    ns.displayAreaLists({
        areaLists: areaLists,
        areaTypeNames: ['Counties / Unitary Authorities', 'Districts'],
        showAreaTypeHeader: showAreaTypeHeader,
        placeName: state.placeName,
        profileUrlKey: profileUrlKey,
        showWebLinks: FT.isProfileWithOnlyStaticReports
    });

    areaSearchResults.ensureParentAreaCodeIsDefined(areaLists);
}

/**
* Ajax call to determine the region area code. Only required in the test environment where
* PDFs may be generated on the fly.
* @class areaSearchResults.ensureParentAreaCodeIsDefined
*/
areaSearchResults.ensureParentAreaCodeIsDefined = function (areaLists) {
    var areaCode;
    for (var i in areaLists) {
        if (areaLists[i].length) {
            // Use first child area code that is available
            areaCode = areaLists[i][0].Code;
            break;
        }
    }

    getData(function (obj) {
        FT.model.parentCode = obj.Parents[0].Code;
    }, 'pa', 'are=' + areaCode + '&ati=6');
}

/**
* Event handler for download link click
* @class areaSearchResults.exportPdf
*/
areaSearchResults.exportPdf = function (areaCode, areaTypeId) {

    // Optional time period for static reports
    var timePeriods = FT.staticReportTimePeriods;
    if (timePeriods.length) {
        // Use first as default until these are options the user can select
        download.timePeriod = timePeriods[0];
    }

    FT.model.areaTypeId = areaTypeId;
    exportPdf(areaCode);
}

/**
* Displays the view model for the area search results.
* @class areaSearchResults.displayAreaLists
*/
areaSearchResults.displayAreaLists = function (viewModel) {
    templates.add('results', '{{#showAreaTypeHeader}}<h3>{{areaTypeName}}</h3>{{/showAreaTypeHeader}}<table class="area-search-results" class="borderedTable">' +
        '{{#areas}}<tr><td>{{Name}}</td><td class="web-links"><a class="pLink" href="/profile/{{profileUrlKey}}/data#page/1/ati/{{CompositeAreaTypeId}}/are/{{Code}}">View data</a></td><td><a class="pLink" href="javascript:areaSearchResults.exportPdf(\'{{Code}}\',{{CompositeAreaTypeId}})">Download report</a></td></tr>{{/areas}}</table>');

    var html = [];
    for (var i in viewModel.areaLists) {

        var areaList = viewModel.areaLists[i];

        if (areaList.length) {

            areaList = new areaSearchResults.AreaListProcessor(areaList)
                .assignCompositeAreaTypeId()
                .getAreaList();

            // Set iteration specific view model properties
            viewModel.areas = areaList;
            viewModel.areaTypeName = viewModel.areaTypeNames[i];

            var htmlAreaList = templates.render('results', viewModel);

            html.push(htmlAreaList);
        }
    }

    $('#area-results-box').html(html);

    // Hide web links if necessary
    if (FT.isProfileWithOnlyStaticReports) {
        $('.web-links').hide();
    }
}

// Constants for positioning of search suggestions
SEARCH_NO_RESULT_TOP_OFFSET = 30;
SEARCH_NO_RESULT_LEFT_OFFSET = -20;
