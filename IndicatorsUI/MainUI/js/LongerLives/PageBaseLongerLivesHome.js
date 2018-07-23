function getMap() {

    var noTiles = 'noTiles',
        road = 'road',
        mapOptions = {
            center: getMapCenter(),
            zoom: 7,
            scrollwheel: false,
            zoomControlOptions: {
                style: google.maps.ZoomControlStyle.LARGE
            },
            backgroundColor: getMapBackgroundColour(),
            disableDefaultUI: true, // a way to quickly hide all controls
            zoomControl: false
        };

    var visibilityOff = [{ visibility: 'off' }],
        styles = [
            {
                stylers: [
                    { color: '#ffffff' }
                ]
            }, {
                featureType: road,
                elementType: 'geometry',
                stylers: visibilityOff
            }, {
                featureType: road,
                elementType: 'labels',
                stylers: visibilityOff
            }
        ];

    var map = new google.maps.Map($('.map-canvas')[0], mapOptions);

    map.setOptions({ styles: styles });
    map.mapTypes.set(noTiles, getNoTilesMapType());

    // Overlay used to map from LngLat to pixel position
    overlay = new google.maps.OverlayView();
    overlay.draw = function () { };
    overlay.setMap(map);

    return map;
}

function getMapBackgroundColour() {

    return MT.model.profileId === ProfileIds.PublicHealthDashboard ? '#fff' : '#E5E3DF';
}

function redrawMap() {

    ajaxMonitor.setCalls(1);

    var model = MT.model;

    if (selectedRootIndex === 'dep') {
        //NOTE Longer Lives premature mortality only
        if (model.areaTypeId === AreaTypeIds.CountyUA) {
            getDecileData(model);
        } else {
            if (isSimilarAreas()) {
                //Get ONS Cluster data
                getAreaValues(groupRoots[ROOT_INDEXES.DEPRIVATION], model, getCurrentComparator().Code);
            } else {
                //Get Deprivation decile data
                getDecileData(model);
            }
        }
    } else {
        getAreaValues(groupRoots[selectedRootIndex], model);
    }

    ajaxMonitor.monitor(displayPage);
}

function initMap() {

    var state = mapState;

    if (!state.map) { // Change this later when multiple area types

        // Does any map exist?
        if (!state.map) {
            state.map = getMap();
        }

        state.areaTypeId = MT.model.areaTypeId;

        initPolygons(state.map);

        $('#gmap').css('background', 'none');

        $('#map-help').append('<p><a href="javascript:MT.nav.rankings();">See ' +
                getAreaTypeNameSingular(MT.model.areaTypeId) + ' comparison table</a></p>');
    }
}

function unselectPolygon(polygon) {
    if (polygon) {
        polygon.set('strokeWeight', 1);
        mapState.selectedPolygon = null;

        if (!$('.map-template').is(':visible') &&
            !$('.no-data-found').is(':visible')) {
            CallOutBox.hide();
        }
    }
}

function getMapCenter() {
    return new google.maps.LatLng(52.85, -3);
}

function initNoMap() {

    if (!mapState.noMapJq) {

        var id = 'no-map';

        pages.getContainerJq().append('<div id="' + id +
            '" class="noData">No map for current area type</div>');

        mapState.noMapJq = $('#' + id);
    }
}

function changeZoom(modifier) {
    var map = mapState.map;
    map.setZoom(map.getZoom() + modifier);
    if (mapState.isPopup) {
        CallOutBox.fitToMapViewPort();
    }
}

function closeInfo() {
    CallOutBox.hide();
    mapState.isPopup = false;
    if (event.preventDefault) {
        event.preventDefault();
    } else {
        event.returnValue = false;
    }
}

CallOutBox.show = function () {
    CallOutBox.fitToMapViewPort();
    $('.map-info-box').show();
    unlock();
}

CallOutBox.fitToMapViewPort = function () {

    var state = mapState;

    var zoom = state.map.getZoom();
    var latLng = state.latLng;

    var pixelsPerLatitude = 151;
    var zoomDiff = (zoom - 7);
    pixelsPerLatitude = pixelsPerLatitude * Math.pow(2, zoomDiff);

    var popUpHeight = $('.map-info-box').height() + 40;
    var mapHeight = $('#map').height();

    var pixelDiff = (mapHeight / 2) - popUpHeight - 190/*distance from top of pop up to top of map view port*/;
    var latDiff = pixelDiff / pixelsPerLatitude;

    // Center the map so the pop up is fully visible
    state.map.setCenter({
        lat: latLng.lat() - latDiff,
        lng: latLng.lng()
    });
}

CallOutBox.hide = function () {
    $('.map-info-box').hide();
}

function setHoverPopupHtml(areaCode) {
    tooltipManager.setHtml(
        templates.render('areaHover', {
            nameofplace: loaded.areaLists[MT.model.areaTypeId][areaCode].Name
        }));
}

function displayPopup() {
    setPolygonCentre(MT.model.areaCode);

    var marker = CallOutBox.getPositionOfPointer();

    if (MT.model.profileId === ProfileIds.Mortality) {
        var content = getPopUpHtml();
        var infoBox = CallOutBox.newInfoBox(content);
        CallOutBox.openInfo(infoBox, marker, content);
        updateCompareBox();
    } else {
        if (isAnyIndicatorData()) {
            content = CallOutBox.getPopUpHtml();
            infoBox = CallOutBox.newInfoBox(content);
            CallOutBox.openInfo(infoBox, marker, content);
        } else {
            unlock();
        }
    }

    logEvent(AnalyticsCategories.MAP, 'MapAreaClicked',
        loaded.areaDetails.getData().Area.Name);
}

CallOutBox.newInfoBox = function (content) {
    return new InfoBox({
        content: content,
        disableAutoPan: true,
        pixelOffset: calloutBoxPixelOffset, // <- offset in css edited 
        closeBoxMargin: '0 0 0 0',
        boxClass: 'map-info-box',
        alignBottom: true,
        zIndex: 3,
        boxStyle: {
            width: 'auto'
        }
    });
}

CallOutBox.getPositionOfPointer = function () {

    // Marker position
    if (!mapState.latLng) {
        mapState.latLng = mapState.map.getCenter();
    }

    setCalloutBoxPixelOffset();

    var latLng = mapState.latLng;

    return new google.maps.Marker({
        position: latLng
    });
}

function colourCodePopup() {

    switch (ragColorCode) {
        case 1:
            return 'ranked_low';
        case 2:
            return 'ranked_medium';
        case 3:
            return 'ranked_high';
        default:
            return '';
    }
}

function removeSelectedDisease(mapInfoRight) {
    mapInfoRight.removeClass('stat_cancer stat_heart stat_liver stat_lung');
}

colours = {
    better: '#7CD540',
    sameBetter: '#FDEE21', // yellow
    sameWorse: '#FB9A02', // orange
    worse: '#BC1E2C',
    noComparison: '#c9c9c9',
    bobHigher: '#BED2FF',
    bobSimilar: '#FB9A02',
    bobLower: '#5555E6'
};

mapState = {
    noMapJq: null,
    map: null,
    // Key -> area code, Value -> polygon on the map
    areaCodeToPolygonHash: null,
    selectedPolygon: null,
    // Whether or not the URL parameters have already been parsed,
    isHashRestored: false,
    // Whether or not the area pop up is displayed
    isPopup: false,
    comparisonArea: null,
    // The area code of the last area hovered over
    hoverAreaCode: null,
    // Whether or not the definition is open on the pop up
    isDefinitionOpen: false
};

function colorPolygonsByDeprivation() {

    var polygons = mapState.areaCodeToPolygonHash;

    var dataList = loaded.categories[AreaTypeIds.DeprivationDecile];

    for (var areaCode in dataList) {
        var decileInfo = getDecileInfo(dataList[areaCode]);
        polygons[areaCode].set('fillColor', decileInfo.color);
    }
}

function getDeprivationPopUpHtml(areaDetails, areaPopulation) {

    var area = areaDetails.Area;

    var dataList = loaded.categories[AreaTypeIds.DeprivationDecile];

    var decileInfo = getDecileInfo(dataList[area.Code]);

    var topLink = getTopLink('View ');

    return templates.render('deprivationOverlay', {
        nameofplace: area.Name,
        population: new CommaNumber(areaPopulation).rounded(),
        areaCode: MT.model.areaCode,
        text: decileInfo.text,
        quintile: decileInfo.quintile,
        topLinkText: topLink.text,
        topLinkFunction: topLink.func
    });
}

function viewNearestNeighbours() {
    if (!FT.ajaxLock) {
        lock();
        var model = MT.model;
        model.parentCode = getNearestNeighbourCode();
        model.similarAreaCode = model.parentCode;

        ftHistory.setHistory();

        ajaxMonitor.setCalls(2);

        getAreaValues(groupRoots[selectedRootIndex], model);
        getNearestNeighbours();

        ajaxMonitor.monitor(displayPage);
    }
}

function viewSimilar() {

    if (!FT.ajaxLock) {

        lock();

        var model = MT.model;
        if (doesAreaTypeCompareToOnsCluster()) {
            // Need to know what the parent area code is first
            ajaxMonitor.setCalls(1);
            getOnsClusterCode(model);
            ajaxMonitor.monitor(viewSimilarPart2);
        } else {
            // Compare to deprivation group
            viewSimilarPart2();
        }
    }
}

function viewSimilarPart2() {

    var model = MT.model;
    var areaCode = model.areaCode;

    // Set parent
    var areaDetails = loaded.areaDetails.getData();
    mapState.comparisonArea = areaCode;
    if (doesAreaTypeCompareToOnsCluster()) {
        model.parentCode = getOnsCodeForArea(areaCode);
    } else {
        model.parentCode = areaDetails.Decile.Code;
    }
    model.similarAreaCode = model.parentCode;

    comparatorId = DEPRIVATION_DECILE_COMPARATOR_ID;

    ftHistory.setHistory();

    // Get decile and area details
    ajaxMonitor.setCalls(3);

    getDecileData(model);
    loaded.areaDetails.fetchDataByAjax({ areaCode: model.parentCode });

    if (selectedRootIndex === 'dep') {
        //NOTE Longer lives premature mortality only Deprivation selected
        getAreaValues(groupRoots[ROOT_INDEXES.DEPRIVATION], model,
            getCurrentComparator().Code);
    } else {
        getAreaValues(groupRoots[selectedRootIndex], model);
    }

    ajaxMonitor.monitor(displayPage);
}

function viewNational() {

    if (!FT.ajaxLock) {

        var model = MT.model;

        model.parentCode = NATIONAL_CODE;
        model.similarAreaCode = null;
        comparatorId = NATIONAL_COMPARATOR_ID;
        ftHistory.setHistory();

        if (selectedRootIndex === 'dep') {
            displayPage();
        } else {

            ajaxMonitor.setCalls(1);

            getAreaValues(groupRoots[selectedRootIndex], model);

            ajaxMonitor.monitor(displayPage);
        }
    }
}

function flattenOnsClusters(onsClusters) {
    var onsClusterHash = {};
    _.each(onsClusters, function (a) { onsClusterHash[a.AreaCode] = a; });
    return onsClusterHash;
}

function isAreaInCurrentDecile(areaCode) {
    var model = MT.model;
    if (model.areaTypeId === AreaTypeIds.CountyUA) {
        var deciles = loaded.categories[AreaTypeIds.DeprivationDecile];
        return deciles[MT.model.areaCode] === deciles[areaCode];
    } else {
        var comparatorAreaCode = getCurrentComparator().Code;
        var root = selectedRootIndex === 'dep'
            ? groupRoots[ROOT_INDEXES.DEPRIVATION]
            : groupRoots[selectedRootIndex];
        var key = getIndicatorKey(root, model, comparatorAreaCode);

        var similarAreas = getAreaCodeToCoreDataHash(loaded.areaValues[key]);
        return isDefined(similarAreas[areaCode]);
    }
}

function setCalloutBoxPixelOffset() {
    calloutBoxPixelOffset = new google.maps.Size(-70/*horizontal*/, -40/*vertical*/);
}

// Called when the triangle underneath the pop up
function pointerClicked() {
    // Reposition pointer over the selected area polygon
    if (!FT.ajaxLock) {
        var selectedAreaCode = mapState.selectedPolygon.areaCode;
        MT.model.areaCode = selectedAreaCode;

        // Ignore clicks on areas that are not part of the current decile
        if (isSimilarAreas() && !isAreaInCurrentDecile(selectedAreaCode)) {
            return;
        }

        setCalloutBoxPixelOffset();

        // Unlocked in CallOutBox.show
        lock();

        var strokeWeight = 'strokeWeight';
        polygon.set(strokeWeight, 3);
        showInfoBox();

        // Correct issue where last polygon border is highlighted
        var lastBoundary = _.last(loaded.boundaries);
        if (lastBoundary) {
            var lastAreaCode = lastBoundary.code;
            if (selectedAreaCode !== lastAreaCode) {
                areaCodeToPolygonHash[lastAreaCode].set(strokeWeight, 1);
            }
        }
    }
}

function getTopLink(text) {

    if (isSimilarAreas()) {
        text += "all";
        var func = "viewNational";
    } else {
        text += "similar";
        func = "viewSimilar";
    }

    return {
        text: text,
        func: func
    };
}

function updateCompareBox() {

    var $mapOverlayTitle = $('.map-overlay-title');
    if (isSimilarAreas()) {

        if (!mapState.comparisonArea) {
            mapState.comparisonArea = MT.model.areaCode;
        }

        // Display comparing text
        var text = getComparisonText();
        $('#similar-text').html(text);

        // Set the similar areas tooltip text
        $('#similar-text-info-icon i').html(getSimilarAreaTooltipText());

        $mapOverlayTitle.show();
    } else {
        $mapOverlayTitle.hide();
    }
}

function getComparisonText() {
    var data = loaded.areaDetails.getData();
    var areaName = data.Area.Name;

    var text;

    if (selectedRootIndex === 'dep') {
        // Deprivation decile only: all areas are the same colour
        if (MT.model.areaTypeId === AreaTypeIds.CountyUA) {
            text = 'Showing similar areas to ' + areaName + ' only';
        } else {
            text = 'Showing similar areas to ' + areaName + ' in ' + getOnsClusterName(MT.model.parentCode);
        }
    } else {
        // Only decile but areas are different colours
        text = 'Comparing ' + areaName + ' to similar areas only';
    }

    return text;
}

function getNoTilesMapType() {

    var NoTilesMapType = function () { };

    NoTilesMapType.prototype.tileSize = new google.maps.Size(1024, 1024);
    NoTilesMapType.prototype.maxZoom = 20;

    NoTilesMapType.prototype.getTile = function (coord, zoom, ownerDocument) {
        return ownerDocument.createElement('div');
    };

    return new NoTilesMapType();
}

function setHoverTooltipHeight() {
    tooltipHeight = $('.hover-map-template').height() + 25;
}

function positionHoverPopup(event, mapOffset) {

    var pixel = overlay.getProjection().fromLatLngToContainerPixel(event.latLng);
    setHoverTooltipHeight();

    tooltipManager.positionXY(
        pixel.x + mapOffset.left - 122 /*half tooltip width*/,
        pixel.y + mapOffset.top - tooltipHeight);
}

function getBoundaries(areaTypeId) {

    // Are boundaries loaded for current area type
    var boundaries = loaded.boundaries;
    if (boundaries.hasOwnProperty(areaTypeId)) {
        ajaxMonitor.callCompleted();
    } else {
        var parameters = {
            url: FT.url.img + 'maps/' + areaTypeId + '/geojson/boundaries.js',
            dataType: 'json',
            success: getBoundariesCallback,
            error: ajaxError
        };
        $.ajax(parameters);
    }
}

function getBoundariesCallback(data) {
    // Variable mapAreas set in boundary JS file
    loaded.boundaries[MT.model.areaTypeId] = data;
    ajaxMonitor.callCompleted();
}

function initPolygons(map) {

    var areaCode;

    var boundaries = loaded.boundaries[MT.model.areaTypeId],
        areaCodeToPolygonHash = {},
        mapOffset = null,
        visible = ':visible',
        jqMap = $('#map'),
        isTouch = $('html.touch').length,
        googleMaps = google.maps;

    // this can be moved to geojson parser file
    if (boundaries.features) {
        for (var x = 0; x < boundaries.features.length; x++) {

            areaCode = boundaries.features[x].properties.AreaCode;
            var coordinates = boundaries.features[x].geometry.coordinates;
            coords = [];

            for (var i = 0; i < coordinates.length; i++) {
                for (var j = 0; j < coordinates[i].length; j++) {
                    var path = [];
                    for (var k = 0; k < coordinates[i][j].length; k++) {
                        var coord = new googleMaps.LatLng(coordinates[i][j][k][1], coordinates[i][j][k][0]);
                        path.push(coord);
                    }
                    coords.push(path);
                }
            }

            // This must be global
            polygon = new googleMaps.Polygon({
                paths: coords,
                strokeColor: '#333',
                strokeOpacity: 1,
                strokeWeight: 1,
                fillOpacity: 1,
                areaCode: areaCode
            });

            areaCodeToPolygonHash[areaCode] = polygon;

            polygon.setMap(map);

            googleMaps.event.addListener(polygon, 'mouseover', function (event) {

                if (!isTouch && !FT.ajaxLock) {

                    mapOffset = jqMap.offset();

                    calloutBoxPixelOffset = new googleMaps.Size(-125, -18);
                    areaCode = this.areaCode;
                    polygon = mapState.areaCodeToPolygonHash[areaCode];

                    // Select polygon
                    polygon.set('strokeWeight', 3);
                    mapState.selectedPolygon = polygon;

                    // Show hover popup
                    if (!$('.map-template').is(visible) &&
                        polygon.fillColor !== '#999' /*an area that is not in the peer group*/ &&
                        !$('.no-data-found').is(visible)) {

                        setHoverPopupHtml(areaCode);

                        tooltipManager.showOnly();

                        // Need to use the height for long area names that wrap
                        setHoverTooltipHeight();

                        positionHoverPopup(event, mapOffset);
                    }
                }
            });

            googleMaps.event.addListener(polygon, 'mousemove', function (event) {
                if (!isTouch && !FT.ajaxLock) {
                    positionHoverPopup(event, mapOffset);
                }
            });

            googleMaps.event.addListener(polygon, 'mouseout', function () {
                unselectPolygon(this);
                tooltipManager.hide();
            });

            googleMaps.event.addListener(polygon, 'click', function () {
                tooltipManager.hide();

                if (!FT.ajaxLock) {

                    areaCode = $(this)[0].areaCode;

                    // Check whether pop up should be displayed
                    if (isMapWithNoData()) {
                        // Navigate straight to area details
                        MT.model.areaCode = areaCode;
                        MT.nav.areaDetails();
                        return;
                    } else if (isSimilarAreas() && !isAreaInSimilarAreas(areaCode)) {
                        // Ignore clicks on areas that are non-similar areas
                        return;
                    }

                    mapState.isPopup = true;
                    setCalloutBoxPixelOffset();

                    // Unlocked in CallOutBox.show
                    lock();

                    var strokeWeight = 'strokeWeight';
                    polygon.set(strokeWeight, 3);

                    // Show pop up
                    MT.model.areaCode = areaCode;
                    showInfoBox();

                    // Correct issue where last polygon border is highlighted
                    var lastAreaCode = _.last(boundaries.features).properties.AreaCode;
                    if (areaCode !== lastAreaCode) {
                        areaCodeToPolygonHash[lastAreaCode].set(strokeWeight, 1);
                    }
                }
            });
        }

        mapState.areaCodeToPolygonHash = areaCodeToPolygonHash;
    }
}

function isMapWithNoData() {
    return MT.model.profileId === ProfileIds.PublicHealthDashboard && !MT.config.showMapWithNoData;
}

function isAreaInSimilarAreas(areaCode) {

    if (isNearestNeighbour()) {
        // Get list of neighbours
        var codeWithNeighbours = getAreaCodeOfAreaWithNeighbours();
        var neighbours = loaded.neighbours[codeWithNeighbours].slice();
        neighbours.push(codeWithNeighbours);

        // Do not show pop up unless neighbour area
        if (_.contains(neighbours, areaCode))
            return true;
    } else if (isAreaInCurrentDecile(areaCode)) {
        return true;
    }
    return false;
}

function colorPolygons(areaValues, root, parentValue) {

    var polygons = mapState.areaCodeToPolygonHash;
    var significanceFunction = getPolygonColourFunction(root, parentValue);
    var colorProp = 'color';

    var shouldShowArea = isNearestNeighbour() 
        ? function (areaCode) { return isAreaInSimilarAreas(areaCode); }
        : function () { return true; }
    for (var areaCode in polygons) {

        var data = areaValues[areaCode];
        var dataInfo = new CoreDataSetInfo(data);

        if (dataInfo.isValue() && shouldShowArea(areaCode)) {
            if (!data.hasOwnProperty(colorProp)) {
                // Colour is not already defined for this data object
                data[colorProp] = significanceFunction(data.Sig[comparatorId], data.Val);
            }
            var color = data[colorProp];

        } else {
            // Grey - no value
            color = '#C9C9C9';
        }

        polygons[areaCode].set('fillColor', color);
    }
}

function colorAllPolygonsGrey() {
    colorAllPolygons('#C9C9C9');
}

function colorAllPolygonsBlue() {
    colorAllPolygons('#63A1C3');
}

function colorAllPolygons(color) {
    var polygons = mapState.areaCodeToPolygonHash;
    for (var areaCode in polygons) {
        polygons[areaCode].set('fillColor', color);
    }
}

function setPolygonCentre(selectedAreaCode) {

    var state = mapState;

    state.latLng = null;

    var boundaries = loaded.boundaries[MT.model.areaTypeId],
        latCentreSum = 0,
        lngCentreSum = 0,
        i, j, k;

    for (var areaIndex in boundaries.features) {
        var area = boundaries.features[areaIndex];
        var areaCode = area.properties.AreaCode;

        if (areaCode === selectedAreaCode) {
            var coordinates = area.geometry.coordinates;
            var latLng = [];

            // TODO: may need to improve the performance of this
            for (i = 0; i < coordinates.length; i++) {
                for (j = 0; j < coordinates[i].length; j++) {
                    var coordinate = coordinates[i][j];
                    for (k = 0; k < coordinate.length; k++) {
                        latLng.push(coordinate[k][1], coordinate[k][0]);
                    }
                }
            }

            for (i = 0; i < latLng.length; i++) {
                latCentreSum += parseFloat(latLng[i]);
                lngCentreSum += parseFloat(latLng[++i]);
            }

            var boundaryPoints = latLng.length / 2;
            var currentLat = latCentreSum / boundaryPoints;
            var currentLng = lngCentreSum / boundaryPoints;
            state.latLng = new google.maps.LatLng(currentLat, currentLng);
        }
    }
}

function initHomeElements() {

    populateAreaTypes();

    // Adjust page title size
    var $title = $('.home-intro h1');
    var size = $title.html().length;
    if (size > 28) {
        $title.css('font-size', '62px');
    }

    $('#map-overlay-zoom').show();
}

function populateAreaTypes() {

    var templateName = 'areaFilter';

    templates.add(templateName, '<div class="compare" id="map-overlay-filters"><h5>{{#isOneType}}Areas on map{{/isOneType}}{{^isOneType}}Change map to show{{/isOneType}}</h5><ul class="filters">\
    {{#types}}<li id="{{Id}}" class="areaFilter {{class}}">{{#isOneType}}{{Short}}{{/isOneType}}{{^isOneType}}<a href="javascript:selectAreaType({{Id}});">{{Short}}</a>{{/isOneType}}</li>{{/types}}');

    setAreaTypeOptionHtml(templateName);
}

CallOutBox.openInfo = function (info, marker, content) {

    if (mapState.isPopup) {

        var map = mapState.map;
        var hasAreaChanged = activeInfoArea !== null &&
            (activeInfoArea !== MT.model.areaCode);

        // Close if area changed
        if (activeInfo && hasAreaChanged) {
            activeInfo.close();
        }

        if (!activeInfo || hasAreaChanged) {
            info.open(map, marker);
            activeInfo = info;
            activeInfoArea = MT.model.areaCode;
        } else {
            // Just change content
            $('.map-info-box').html(content);
        }

        setTimeout('CallOutBox.show()', 0);
    } else {

        if (activeInfo) {
            activeInfo.close();
        }
    }
}
DEPRIVATION_DECILE_COMPARATOR_ID = 1;

/**
 * Template displayed when user hovers over area on map
 */
templates.add('areaHover',
    '<div class="hover-map-template map-info"><div class="hover-map-info-body clearfix">'
    + '<h4 class="hover-place-name">{{nameofplace}}</h4></div><div class="map-info-footer clearfix"></div>'
    + '<div class="map-info-tail"><i></i></div></div>');

// Pop up content is selected area has no data
templates.add('nodatafound',
    '<div id="map-overlay" class="no-data-found"><div class="map-info map-info-empty" id="map-empty-template"><div class="map-info-header clearfix"><span class="map-info-close" onclick="closeInfo()">&times;</span></div><div class="map-info-body"><h4>{{nameofplace}}</h4><p>Data unavailable</p></div><div class="map-info-tail"><i></i></div></div></div>');

var activeInfo;
var info_Empty;
var info_Deprivation;
var calloutBoxPixelOffset = null;
activeInfoArea = null;

