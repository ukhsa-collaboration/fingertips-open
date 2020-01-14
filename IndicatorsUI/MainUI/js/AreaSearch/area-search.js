/**
* Functions of the area search partial page.
* @module areaSearch
*/

var areaSearch = {};

areaSearch.state = {};

var ccgs = [AreaTypeIds.CCGPreApr2017, AreaTypeIds.CCGPostApr2017, AreaTypeIds.CCGSince2018, AreaTypeIds.CcgSinceApr2019];
var countiesUas = [AreaTypeIds.CountyUA, AreaTypeIds.CountyUASince2019];
var districtUas = [AreaTypeIds.DistrictUA, AreaTypeIds.DistrictUASince2019];
var Uas = [AreaTypeIds.UnitaryAuthority, AreaTypeIds.UAUnchanged, AreaTypeIds.UANew2019,
    AreaTypeIds.UAObsolete2019, AreaTypeIds.UAFutureAreas
];
var districts = [AreaTypeIds.LANew2019, AreaTypeIds.LAUnchanged, AreaTypeIds.District, AreaTypeIds.LAObsolete2019]; 

var areaTypeNameHash = {};

function addAreaTypeNames(areaTypeIdList, name) {
    for (var i in areaTypeIdList) {
        areaTypeNameHash[areaTypeIdList[i]] = name;
    }
}

addAreaTypeNames(ccgs, 'CCGs');
addAreaTypeNames(countiesUas, 'Counties / Unitary Authorities');
addAreaTypeNames(districtUas, 'Districts');

/**
* Hides the area search components and shows the spinner.
* @class areaSearch.hideSearchComponents
*/
areaSearch.hideSearchComponents = function () {
    $('#area-search').hide();
    showSpinner();
}

/**
* Initialises the _IntroductionAreaSearch.cshtml partial.
* @class areaSearch.init
*/
areaSearch.init = function () {

    var ns = areaSearch;

    // Global needs to be set
    FT.model.areAnyIndicators = true;

    // Area type to include in the search results
    var areaTypeId = areaSearch.getAreaTypeForSearchResults();
    FT.model.areaTypeId = areaTypeId;

    // Set parent area
    ns.state.parentAreaType = _.contains(ccgs, areaTypeId)
        ? AreaTypeIds.Subregion
        : AreaTypeIds.Region;

    // Init area search menu
    var areaSearchSelector = '#area-search-text';
    initAreaSearch(areaSearchSelector, false, FT.config.frontPageAreaSearchAreaTypes);
    $(areaSearchSelector).show();

    // Populate region menu
    ajaxMonitor.setCalls(1);
    getAllAreas(ns.state.parentAreaType);
    ajaxMonitor.monitor(ns.showParentMenu);
};

/**
* Gets the area types to be included in the search results
* @class areaSearch.getAreaTypeForSearchResults
*/
areaSearch.getAreaTypeForSearchResults = function () {

    var areaTypeId = getDistrictUaAreaType();
    return areaTypeId !== null ? areaTypeId : FT.config.frontPageAreaSearchAreaTypes[0];
}

function getDistrictUaAreaType()
{
    var areaTypeIds = FT.config.frontPageAreaSearchAreaTypes;

    // Use district and UA in preference to County UA
    var ids = _.filter(areaTypeIds,
        function (id) {
            return _.contains(districtUas, id);
        });
    if (ids.length) {
        return ids[0];
    }
    return null;
}

/**
* Populates the parent menu with options and navigates to the
* result page when the selected option is changed.
* @class areaSearch.showParentMenu
*/
areaSearch.showParentMenu = function () {

    var ns = areaSearch;

    var areas = loaded.areaLists[ns.state.parentAreaType];
    _.each(areas, function (area) { area.Name = area.Short; });
    var $menu = $('#parent-area-menu');
    populateAreaMenu(areas, $menu, 'SELECT A REGION');
    $menu.on('change', function () {

        ns.hideSearchComponents();

        var placeName = $menu.find('option:selected').text();

        logEvent('RegionMenuSelection', placeName);

        // Change URL this way to ensure event is logged
        setTimeout(function () {
            var profileUrl = ns.getProfileUrlPart();
            setUrl(profileUrl + '/area-search-results/' + $menu.val() +
                '?search_type=' + ns.searchTypes.listChildAreas +
                '&place_name=' + placeName);
        }, 0);
    });

    // Display all components now loading finished
    hideSpinner();
    $('#area-search').show();
}

/**
* Get the part of the URL that includes the profile and profile collection
* @class areaSearch.getProfileUrlPart
*/
areaSearch.getProfileUrlPart = function () {
    // URL part for profile collection
    var collectionKey = FT.config.profileCollectionUrlKey;
    var collectionUrlPart = collectionKey.length > 0 ? '/profile-group/' + collectionKey : '';
    return collectionUrlPart + '/profile/' + profileUrlKey;
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
        var profileUrl = areaSearch.getProfileUrlPart();
        setUrl(profileUrl +
            '/area-search-results/' + result.PolygonAreaCode +
            '?place_name=' + placeName +
            '&search_type=' + searchType);
    }, 0);
}


/************************* Area Search Results *****************************/

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

    showSpinner();

    tooltipManager.init();

    var ns = areaSearchResults;

    ns.initTimePeriod();

    // Init back to search link
    $('#back-to-search').bind('click', function (e) {
        e.preventDefault();

        setUrl(areaSearch.getProfileUrlPart());
    });

    // Check in case ignored area, e.g. Isles of Scilly
    if (_.contains(state.areasToIgnore, state.parentAreaCode)) {
        $('#no-data-message').show();
        hideSpinner();
        return;
    }

    var searchTypes = areaSearch.searchTypes;
    var areaTypeIds = FT.config.frontPageAreaSearchAreaTypes;

    // Call 1 
    if (state.searchType === searchTypes.placeName) {
        ns.getDataForPlaceNameSearch(areaTypeIds);
    } else if (state.searchType === searchTypes.listChildAreas) {
        ns.getDataForListOfAreas(areaTypeIds);
    } else {
        ns.getDataForParentArea(areaTypeIds);
    }
};

areaSearchResults.getDataForPlaceNameSearch = function (areaTypeIds) {
    var ns = areaSearchResults;
    ajaxMonitor.setCalls(1 + areaTypeIds.length);

    for (var i in areaTypeIds) {
        ns.getParentAreaOfPlaceName(areaTypeIds[i]);
    }

    areaSearchResults.getArePdfsAvailable();

    ajaxMonitor.monitor(ns.prepareViewModel);
}

areaSearchResults.getDataForParentArea = function (areaTypeIds) {
    var ns = areaSearchResults;

    ajaxMonitor.setCalls(2);

    // Get selected area details
    ajaxGet('api/areas/by_area_code',
        'area_codes=' + state.parentAreaCode,
        function (areaList) {
            var areaTypeId = areaList[0].AreaTypeId;
            if (areaList.length === 1 &&
                (_.contains(districts, areaTypeId))) {
                // Where user has selected a district
                loaded.areaLists[getDistrictUaAreaType()] = areaList;
                ajaxMonitor.callCompleted();
            } else {
                loaded.areaLists[areaTypeIds[0]] = areaList;

                if (areaTypeIds.length > 1) {
                    // Get child areas where both lower and upper tier LAs are displayed
                    ns.getChildAreas(areaTypeIds[1]);
                } else {
                    ajaxMonitor.callCompleted();
                }
            }
        });

    // Area PDFs available
    areaSearchResults.getArePdfsAvailable();

    ajaxMonitor.monitor(ns.prepareViewModel);
}

areaSearchResults.getDataForListOfAreas = function (areaTypeIds) {
    var ns = areaSearchResults;
    ajaxMonitor.setCalls(1 + areaTypeIds.length);
    // Get child areas
    for (var i in areaTypeIds) {
        ns.getChildAreas(areaTypeIds[i]);
    }

    // Area PDFs available
    areaSearchResults.getArePdfsAvailable();

    ajaxMonitor.monitor(ns.prepareViewModel);
}

/**
* Initialise time period state.
* @class areaSearchResults.initTimePeriod
*/
areaSearchResults.initTimePeriod = function () {
    // Optional time period for static reports
    var timePeriods = FT.config.staticReportsFolders;
    if (timePeriods.length) {
        // Use first as default until these are options the user can select
        areaSearchResults.timePeriod = timePeriods[0];
    }
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
    ajaxGet('api/areas/by_parent_area_code', parameters.build(), function (areaList) {
        loaded.areaLists[areaTypeId] = areaList;
        ajaxMonitor.callCompleted();
    });
}

/**
* Contains functions for modifying and filtering an area list.
* @class areaSearchResults.AreaListProcessor
*/
areaSearchResults.AreaListProcessor = function (areaList, areaTypeId) {

    /**
    * Assigns a value to the property 'CompositeAreaTypeId' for each area, e.g. 101, 102
    * @method assignCompositeAreaTypeId
    */
    this.assignCompositeAreaTypeId = function () {
        _.each(areaList, function (area) {
            area.CompositeAreaTypeId = areaTypeId;
        });
        return this;
    };

    /**
    * Removes the Unitary Authorities.
    * @method removeUAs
    */
    this.removeUAs = function () {
        areaList = _.filter(areaList, function (area) {
            return !_.contains(Uas, area.AreaTypeId);
        });
        return this;
    };

    /**
    * Assigns a value to the property 'HasReport' for each area
    * @method assignHasReport
    */
    this.assignHasReport = function () {
        _.each(areaList, function (area) {
            return area.hasReport = true;
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

areaSearchResults.findAreaFromCode = function (areaList, areaCode) {
    return _.find(areaList,
        function (area) { return area.Code === areaCode; });
}

/**
* Prepares the view model for the area search results.
* @class areaSearchResults.prepareViewModel
*/
areaSearchResults.prepareViewModel = function () {

    var ns = areaSearchResults;
    var areaTypeIds = FT.config.frontPageAreaSearchAreaTypes;

    // Areas with PDFs
    var areaTypesIdsWithPdfs = _.map(loaded.areaTypesWithPdfs[FT.model.profileId], function (areaType) {
        return areaType.Id;
    });

    // Process area lists
    var areaLists = [];
    var areaTypeNames = [];
    for (var i in areaTypeIds) {

        var areaTypeId = areaTypeIds[i];
        var areaList = loaded.areaLists[areaTypeId];
        if (areaList) {
            areaTypeNames.push(areaTypeNameHash[areaTypeId]);

            var processor = new areaSearchResults.AreaListProcessor(areaList, areaTypeId);

            if (_.contains(districtUas, areaTypeId)) {

                var isCountyUa = _.some(areaTypeIds, function (id) { return _.contains(countiesUas, id); });
                if (isCountyUa) {
                    // Don't want to display UAs twice
                    processor.removeUAs();
                }
            }

            // Determine whether PDF report is available
            if (FT.config
                .hasStaticReports ||
                _.some(areaTypesIdsWithPdfs, function (id) { return id === areaTypeId; })) {
                processor.assignHasReport();
            }

            // Composite area type required for view data link
            processor.assignCompositeAreaTypeId();

            areaLists.push(processor.getAreaList());
        }
    }

    // Only show area type header if more than one area
    var totalAreas = 0;
    _.each(areaLists, function (list) { totalAreas += list.length; });
    var showAreaTypeHeader = totalAreas > 1;

    ns.displayAreaLists({
        areaLists: areaLists,
        areaTypeNames: areaTypeNames,
        showAreaTypeHeader: showAreaTypeHeader,
        placeName: state.placeName,
        profileUrlKey: profileUrlKey
    });

    areaSearchResults.ensureParentAreaCodeIsDefined(areaLists);

    hideSpinner();
}

areaSearchResults.getParentAreaOfPlaceName = function (areaTypeId) {

    // Get parent of place name
    getAreaSearchResults(state.placeName,
        function (results) {
            var areaCode = results[0].PolygonAreaCode;

            // Get area object
            ajaxGet('api/areas/by_area_code',
                'area_codes=' + areaCode,
                function (areaList) {
                    loaded.areaLists[areaTypeId] = areaList;
                    ajaxMonitor.callCompleted();
                });

        },
        areaTypeId, false, [areaTypeId]);
}

/**
* Ajax call to determine the region area code. Only required in the test environment where
* PDFs may be generated on the fly.
* @class areaSearchResults.ensureParentAreaCodeIsDefined
*/
areaSearchResults.ensureParentAreaCodeIsDefined = function (areaLists) {

    if (state.parentAreaCode) {
        // Parent area code will already be defined if region menu selected
        FT.model.parentCode = state.parentAreaCode;
    } else {

        // Define a suitable child area code
        var areaCode;
        for (var i in areaLists) {
            if (areaLists[i].length) {
                // Use first child area code that is available
                areaCode = areaLists[i][0].Code;
                break;
            }
        }

        // Get parent areas
        var parameters = new ParameterBuilder(
        ).add('child_area_code', areaCode
        ).add('parent_area_type_ids', AreaTypeIds.Region);

        ajaxGet('api/area/parent_areas',
            parameters.build(),
            function (obj) {
                FT.model.parentCode = obj.Parents[0].Code;
            });
    }
}

/**
* Event handler for download link click
* @class areaSearchResults.openReportSelected
*/
areaSearchResults.openReportSelected = function (areaCode, areaTypeId, areaName) {

    downloadReport(areaCode, areaName);
    logEvent('Download', 'PDF', areaName);
}

/**
* Downloads a cached PDF. DUPLICATED IN ANGULAR DOWNLOAD COMPONENTS.
*/
function downloadReport(areaCode, areaName) {

    if (FT.config.hasStaticReports) {
        checkStaticReportExistsThenDownload(areaCode, areaName);
    } else if (FT.model.profileId === ProfileIds.Liver) {
        // Liver profiles
        var url = 'http://www.endoflifecare-intelligence.org.uk/profiles/liver-disease/' + areaCode + '.pdf';
        areaSearchResults.openFile(url);
    }
}

areaSearchResults.openFile = function (url) {
    window.open(url.toLowerCase(), '_blank');
}

function getReportExtension() {
    // Health Profiles switched from PDF to HTML reports from 2019
    if (FT.model.profileId === ProfileIds.HealthProfiles) {
        var year = parseInt(areaSearchResults.timePeriod);
        if (year > 2018) {
            return 'html';
        }
    }
    return 'pdf';
}

function checkStaticReportExistsThenDownload(areaCode, areaName) {

    var fileName = areaCode + '.' + getReportExtension();
    var timePeriod = areaSearchResults.timePeriod;

    var parameters = new ParameterBuilder(
    ).add('profile_key', profileUrlKey
    ).add('file_name', fileName);

    // Time period (if required)
    if (timePeriod) {
        parameters.add('time_period', timePeriod);
    }

    var parametersString = parameters.build();

    // Check report exists
    ajaxGet('api/static-reports/exists',
        parametersString,
        function (doesReportExist) {
            if (doesReportExist) {

                // Download report
                var url;
                if (parametersString.indexOf('.html') > -1) {
                    // HTML - open file
                    url = FT.url.bridge + 'static-reports/' + profileUrlKey + '/';
                    if (timePeriod) {
                        url += timePeriod + '/';
                    }

                    // Area name is appended to help track in GA
                    url += fileName + '?area-name=' + areaName;
                } else {
                    // PDF - Download file
                    url = FT.url.corews + 'static-reports?' + parametersString;
                }

                areaSearchResults.openFile(url);

            } else {
                var html = '<div style="padding:15px;"><h3>Sorry, this document is not available</h3></div>';
                var popupWidth = 800;
                var left = ($(window).width() - popupWidth) / 2;
                var top = 500;
                lightbox.show(html, top, left, popupWidth);
            }
        });
}

/**
* Displays the view model for the area search results.
* @class areaSearchResults.displayAreaLists
*/
areaSearchResults.displayAreaLists = function (viewModel) {

    var html = [];

    // Time period options HTML
    if (FT.config.staticReportsFolders.length > 1) {
        templates.add('timePeriodOptions', '<div id="tab-specific-options" class="tab-options clearfix"></div>');
        html.push(templates.render('timePeriodOptions'));
    }

    // Area results
    templates.add('results', '{{#showAreaTypeHeader}}<h3>{{areaTypeName}}</h3>{{/showAreaTypeHeader}}<table class="area-search-results" class="bordered-table">' +
        '{{#areas}}<tr><td>{{Name}}</td><td class="web-links"><a class="pLink" href="/profile/{{profileUrlKey}}/data#page/1/ati/{{CompositeAreaTypeId}}/are/{{Code}}">View data</a></td>' +
        '<td>{{#hasReport}}<a class="pLink" href="javascript:areaSearchResults.openReportSelected(\'{{Code}}\',{{CompositeAreaTypeId}},\'{{Name}}\')">Download report</a>{{/hasReport}}</td></tr>{{/areas}}</table>');

    for (var i in viewModel.areaLists) {

        var areaList = viewModel.areaLists[i];

        if (areaList.length) {

            // Set iteration specific view model properties
            viewModel.areas = areaList;
            viewModel.areaTypeName = viewModel.areaTypeNames[i];

            var htmlAreaList = templates.render('results', viewModel);

            html.push(htmlAreaList);
        }
    }

    $('#area-results-box').html(html);

    areaSearchResults.displayTimePeriodOptions();

    // Hide links to view data if necessary
    if (!FT.config.hasAnyData) {
        $('.web-links').hide();
    }
}

/**
* Click handler for when user selects a time period.
* @class areaSearchResults.displayTimePeriodOptions
*/
areaSearchResults.displayTimePeriodOptions = function () {

    var timePeriods = FT.config.staticReportsFolders;

    // Display time period options
    if (timePeriods.length > 1) {

        // Define event handlers for when user selects a time period
        var clickHandler = areaSearchResults.changeTimePeriod;
        var eventHandlers = _.map(timePeriods,
            function () { return clickHandler; });

        // Init time period options
        var tabSpecificOptions = new TabSpecificOptions({
            eventHandlers: eventHandlers,
            eventName: 'TimePeriodChanged'
        });

        // Display time period options
        tabSpecificOptions.setHtml({
            label: FT.config.staticReportsLabel,
            optionLabels: timePeriods
        });
    }
}

/**
* Click handler for when user selects a time period.
* @class areaSearchResults.changeTimePeriod
*/
areaSearchResults.changeTimePeriod = function ($option) {
    areaSearchResults.timePeriod = $option.html();
}

// Selected time period
areaSearchResults.timePeriod = null;

/**
* AJAX call to determine whether or not PDFs are available for the current profile and area type.
* @class areaSearchResults.getArePdfsAvailable
*/
areaSearchResults.getArePdfsAvailable = function () {
    var model = FT.model;
    var profileId = model.profileId;

    if (!loaded.areaTypesWithPdfs[profileId]) {
        var parameters = new ParameterBuilder(
        ).add('profile_id', profileId);

        ajaxGet('api/profile/area_types_with_pdfs',
            parameters.build(),
            function (obj) {
                loaded.areaTypesWithPdfs[profileId] = obj;
                ajaxMonitor.callCompleted();
            });
    } else {
        ajaxMonitor.callCompleted();
    }
}

loaded.areaTypesWithPdfs = {};

// Constants for positioning of search suggestions
SEARCH_NO_RESULT_TOP_OFFSET = 30;
SEARCH_NO_RESULT_LEFT_OFFSET = -20;
