'use strict';

function goToPracticeSearchPage() {

    lock();

    setPageMode(PAGE_MODES.SELECT);

    ajaxMonitor.setCalls(3);

    getPracticeAndParentLists();

    ajaxMonitor.monitor(displaySearch);
}

function selectPractice(code) {

    if (!FT.ajaxLock) {

        lock();

        loaded.addresses[code] = _.find(practiceSearchState.results,
            function (area) {
                return area.Code === code;
            });

        $('#all_ccg_practices').show();
        getParentAreas(code);
    }
}

function getParentAreasCallback(obj) {

    var practice = obj;

    if (!practice) {
        return;
    }

    // Cache to avoid repeated calls to server
    loaded.parentAreas[practice.Code] = obj;

    var parent = _.find(practice.Parents,
        function (area) { return area.AreaTypeId === PP.model.parentAreaType; });

    PP.model.parentCode = parent ?
        parent.Code :
        null;

    $(ID_PARENT_AREA_MENU).val(parent.Code);

    setPracticeCode(practice.Code);
    PP.model.mode = PAGE_MODES.POPULATION;

    ftHistory.setHistory();

    $('#mapBox').hide();

    goToCurrentPage();
}

/* Find out the parent CCG of the practice */
function getParentAreas(code) {

    var parents = loaded.parentAreas;
    if (parents.hasOwnProperty(code)) {
        getParentAreasCallback(parents[code]);
    }
    else {
        // Get parent areas
        var parameters = new ParameterBuilder(
        ).add('child_area_code', code
        ).add('parent_area_type_ids', AreaTypeIds.CCG);

        ajaxGet('api/area/parent_areas', parameters.build(),
            function (json) { getParentAreasCallback(json); }
            );
    }
}

function displaySearch() {

    // Map must be visible in order to add points to it
    showAndHidePageElements();

    var parentCode = PP.model.parentCode;
    var state = practiceSearchState;
    var currentDisplayedArea = state.displayedArea;
    var currentSelectedArea = state.selectedArea;

    if (state.selectedArea && currentSelectedArea !== currentDisplayedArea) {
        // Display practices near the place the user's selected
        state.displayedArea = currentSelectedArea;
        var rows = initTableAndMapData();
        displayPracticeResultsTable(rows);

        if (!rows.length) {
            defaultGoogleMap(currentSelectedArea.LatLng);
        } else {
            displayMarkersOnMap();
        }

        state.selectedArea = null;
    } else if (state.showAllInCcg && parentCode !== currentDisplayedArea) {
        // Show all practices in CCG
        state.displayedArea = parentCode;
        rows = initTableAndMapData();
        displayPracticeResultsTable(rows);
        displayMarkersOnMap();
        state.showAllInCcg = false;
    } else if (!currentDisplayedArea) {
        state.displayedArea = 'default';
        defaultGoogleMap();
    }

    unlock();
}

function displayPracticeResultsTable(rows) {

    var showAllInCcg = practiceSearchState.showAllInCcg;

    var $searchTbody = $('#search-table tbody');
    if (isDefined(rows)) {

        // Sort by A-Z
        if (showAllInCcg) {
            rows.sort(sortAreasByName);
        }

        // Render practice table
        $searchTbody.html(
            templates.render('rows', {
                rows: rows,
                showAllInCcg: showAllInCcg
            })
        );
        $('.nearby-practices-table').show();

        // Scroll to top of table
        $('#table-container').scrollTop(0);

        // Render data header
        if (showAllInCcg) {
            var html = templates.render('searchCCGHeader', {
                selectedArea: getPracticeParentName(),
                practiceCount: rows.length
            });
        } else {
            html = templates.render('searchNearHeader', {
                selectedArea: practiceSearchState.selectedArea.PlaceName
            });
        }
        $('#data_page_header').html(html);

        // Display number of practices
        if (!showAllInCcg) {
            displayNumberOfPracticesFound(rows.length);
        }

    } else {
        $searchTbody.html('No GP practice found');
    }
}

function areaSearchResultSelected(noMatches, searchResult) {

    if (!FT.ajaxLock) {

        lock();

        practiceSearchState.selectedArea = searchResult;
        practiceSearchState.showAllInCcg = false;

        $('#default_search_header').hide();
        $('#all_ccg_practices').html('Show all practices');

        ajaxMonitor.setCalls(1);
        getPractices(searchResult);
        ajaxMonitor.monitor(goToPracticeSearchPage);
    }
}

function showAllPracticesInCCG() {

    if (!FT.ajaxLock) {

        lock();

        practiceSearchState.selectedArea = null;
        practiceSearchState.showAllInCcg = true;

        $('#default_search_header').hide();
        logEvent('PracticeSearch', 'ShowAllPracticesInCCG');

        ajaxMonitor.setCalls(1);
        getCCGPractices();
        ajaxMonitor.monitor(goToPracticeSearchPage);
    }
}

function getPractices(searchResult) {
    var parameters = new ParameterBuilder()
        .add('easting', searchResult.Easting)
        .add('northing', searchResult.Northing)
        .add('area_type_id', AreaTypeIds.Practice);

    ajaxGet('api/area_search_by_proximity', parameters.build(), getGpPracticesNearbyCallback);
}

function getCCGPractices() {
    var parameters = new ParameterBuilder()
        .add('parent_area_code', PP.model.parentCode)
        .add('area_type_id', AreaTypeIds.Practice);

    ajaxGet('api/area_addresses/by_parent_area_code', parameters.build(), getGpPracticesNearbyCallback);
}

function getGpPracticesNearbyCallback(obj) {
    loaded.nearbyPractices = obj;
    ajaxMonitor.callCompleted();
}

function initTableAndMapData() {

    var rows = loaded.nearbyPractices;

    var practicesOnMap = [];
    practiceSearchState.practicesOnMap = practicesOnMap;

    var showAllInCcg = practiceSearchState.showAllInCcg;

    for (var i in rows) {

        var row = rows[i];

        // Standardise properties
        if (showAllInCcg) {
            row.AreaName = row.Name;
            row.AreaCode = row.Code;
            row.AddressLine1 = row.A1;
            row.AddressLine2 = row.A2;
        }

        if (showAllInCcg) {
            // Practice in selected CCG
            var practice = isDefined(row.Pos) ? {
                Name: row.AreaName,
                Lat: row.Pos.Lat,
                Lng: row.Pos.Lng,
                AreaCode: row.AreaCode
            } : null;
        } else {
            // Practice near selected place
            practice = isDefined(row.LatLng) ? {
                Name: row.AreaName,
                Lat: row.LatLng.Lat,
                Lng: row.LatLng.Lng,
                AreaCode: row.AreaCode
            } : null;
        }

        if (practice) {
            practicesOnMap.push(practice);
        }
    }

    return rows;
}

function displayNumberOfPracticesFound(practiceCount) {

    var placeName = practiceSearchState.selectedArea.PlaceName;

    if (practiceCount === 0) {
        var html = 'are no practices';
    } else if (practiceCount === 1) {
        html = 'is 1 practice';
    } else {
        html = 'are ' + practiceCount + ' practices';
    }

    $('#no-of-practices').html('There ' + html + ' within 5 miles of ' +
        placeName);
}

function defaultGoogleMap(position) {
    var gm = google.maps;

    $('#mapBox').addClass('default');
    $('#default_search_header').html(templates.render('searchDefaultHeader', {}));

    var map = getGoogleMap();
    var bounds = new gm.LatLngBounds();

    if (!position) {
        position = new gm.LatLng(53.415649, -2.209015);
    }

    bounds.extend(position);
    map.setCenter(bounds.getCenter());
}

function displayMarkersOnMap() {

    var practicesOnMap = practiceSearchState.practicesOnMap;

    var gm = google.maps;

    var map = getGoogleMap();

    var bounds = new gm.LatLngBounds();

    for (var i = 0; i < practicesOnMap.length; i++) {

        var position = new gm.LatLng(practicesOnMap[i].Lat, practicesOnMap[i].Lng);
        bounds.extend(position);

        var marker = new gm.Marker({
            position: position,
            map: map
        });

        marker.set('marker_id', practicesOnMap[i].AreaCode);

        var infoWindow = new gm.InfoWindow({});

        gm.event.addListener(marker, 'click', (function (marker, i) {
            return function () {

                if (!FT.ajaxLock) {

                    var areaCode = marker.get('marker_id');

                    var content = '<div class="map-popup-content">' +
                        '<a class="select-area-link" href="javascript:selectPractice(\'' + areaCode + '\')">' + practicesOnMap[i].Name + '</a></div>';
                    infoWindow.setContent(content);
                    infoWindow.open(map, marker);

                    var $tableContainer = $('#table-container');
                    var $practiceHeader = $('#' + areaCode + '-practice-header');

                    // Scroll table so selected practice is at the top
                    var scrollTop = $practiceHeader.offset().top -
                        $tableContainer.offset().top +
                        $tableContainer.scrollTop();
                    $tableContainer.scrollTop(scrollTop);

                    // Unhighlight last selected practice
                    var highlightCss = 'highlight';
                    $('.highlight').removeClass(highlightCss);

                    // Highlight selected practice
                    $practiceHeader.addClass(highlightCss);
                    $('#' + areaCode + '-address').addClass(highlightCss);
                }
            };
        })(marker, i));
    }

    fitMapToPracticeResults(map, bounds);
}

var practiceSearchState = {
    displayedArea: null,
    selectedArea: null,
    practicesOnMap: [],
    showAllInCcg: null
};

loaded.nearbyPractices = [];

templates.add('rows',
    '{{#rows}}\
<tr id="{{AreaCode}}-practice-header"><td colspan="2"><div class="header national clearfix">{{AreaCode}} - {{AreaName}}</div></td></tr>\
<tr id="{{AreaCode}}-address"><td><div>{{AddressLine1}}</div><div>{{AddressLine2}}</div><div>{{Postcode}}</div></td>\
<td><div class="center-text"><br>{{^showAllInCcg}}{{DistanceValF}} miles{{/showAllInCcg}}</div><div><a href="javascript:selectPractice(\'{{AreaCode}}\')">Select</a></div></td></tr><tr><td class="empty" colspan="2">&nbsp;</td></tr>{{/rows}}');

templates.add('counter', '<span>{{noOfPractices}}</span>');

templates.add('searchNearHeader', '<h2 class="area_name">Practices near {{selectedArea}}</h2><h3 id="no-of-practices"></h3>');

templates.add('searchCCGHeader', '<h2 class="area_name">The are {{practiceCount}} practices in {{selectedArea}}</h2>');

templates.add('searchDefaultHeader', '<h3 class="area_name">Search above for a practice by postcode or place name. </h3>');

// Search box, map and empty results table
templates.add('searchResults',
    '<table class="w100" cellspacing="0">{{#areas}}<tr id="{{Code}}-result"><td class="resultCode">{{Code}}</td><td class="result">{{Name}}, {{Address}}</td></tr>{{/areas}}</table>');
