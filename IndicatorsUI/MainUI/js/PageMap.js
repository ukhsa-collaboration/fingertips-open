// jQuery.XDomainRequest.js
// Author: Jason Moon - @JSONMOON
// IE8+
// See: https://github.com/MoonScript/jQuery-ajaxTransport-XDomainRequest
if (!jQuery.support.cors && jQuery.ajaxTransport && window.XDomainRequest) {
    var httpRegEx = /^https?:\/\//i;
    var getOrPostRegEx = /^get|post$/i;
    var sameSchemeRegEx = new RegExp('^' + location.protocol, 'i');
    var htmlRegEx = /text\/html/i;
    var jsonRegEx = /\/json/i;
    var xmlRegEx = /\/xml/i;
    // ajaxTransport exists in jQuery 1.5+
    jQuery.ajaxTransport('text html xml json', function (options, userOptions, jqXHR) {
        // XDomainRequests must be: asynchronous, GET or POST methods, HTTP or HTTPS protocol, and same scheme as calling page
        if (options.crossDomain && options.async && getOrPostRegEx.test(options.type) && httpRegEx.test(options.url) && sameSchemeRegEx.test(options.url)) {
            var xdr = null;
            var userType = (userOptions.dataType || '').toLowerCase();
            return {
                send: function (headers, complete) {
                    xdr = new XDomainRequest();
                    if (/^\d+$/.test(userOptions.timeout)) {
                        xdr.timeout = userOptions.timeout;
                    }
                    xdr.ontimeout = function () {
                        complete(500, 'timeout');
                    };
                    xdr.onload = function () {
                        var allResponseHeaders = 'Content-Length: ' + xdr.responseText.length + '\r\nContent-Type: ' + xdr.contentType;
                        var status = {
                            code: 200,
                            message: 'success'
                        };
                        var responses = {
                            text: xdr.responseText
                        };
                        try {
                            if (userType === 'html' || htmlRegEx.test(xdr.contentType)) {
                                responses.html = xdr.responseText;
                            } else if (userType === 'json' || (userType !== 'text' && jsonRegEx.test(xdr.contentType))) {
                                try {
                                    responses.json = jQuery.parseJSON(xdr.responseText);
                                } catch (e) {
                                    status.code = 500;
                                    status.message = 'parseerror';
                                    //throw 'Invalid JSON: ' + xdr.responseText;
                                }
                            } else if (userType === 'xml' || (userType !== 'text' && xmlRegEx.test(xdr.contentType))) {
                                var doc = new ActiveXObject('Microsoft.XMLDOM');
                                doc.async = false;
                                try {
                                    doc.loadXML(xdr.responseText);
                                } catch (e) {
                                    doc = undefined;
                                }
                                if (!doc || !doc.documentElement || doc.getElementsByTagName('parsererror').length) {
                                    status.code = 500;
                                    status.message = 'parseerror';
                                    throw 'Invalid XML: ' + xdr.responseText;
                                }
                                responses.xml = doc;
                            }
                        } catch (parseMessage) {
                            throw parseMessage;
                        } finally {
                            complete(status.code, status.message, responses, allResponseHeaders);
                        }
                    };
                    // set an empty handler for 'onprogress' so requests don't get aborted
                    xdr.onprogress = function () { };
                    xdr.onerror = function () {
                        complete(500, 'error', {
                            text: xdr.responseText
                        });
                    };
                    var postData = '';
                    if (userOptions.data) {
                        postData = (jQuery.type(userOptions.data) === 'string') ? userOptions.data : jQuery.param(userOptions.data);
                    }
                    xdr.open(options.type, options.url);
                    xdr.send(postData);
                },
                abort: function () {
                    if (xdr) {
                        xdr.abort();
                    }
                }
            };
        }
    });
}

// ---------------- PageMap functions below --------------------

function goToMapPage() {

    if (groupRoots.length == 0) {
        // Search results empty
        noDataForAreaType();
    } else {
        setPageMode(PAGE_MODES.MAP);

        ajaxMonitor.setCalls(1);

        var root = groupRoots[getIndicatorIndex()];
        mapState.root = root;
        getAreaValues(root, FT.model);

        ajaxMonitor.monitor(displayMap);
    }

    $('#goBack').bind('click', function () {
        NEPHOMaps.SelectedFeatures = [];
        NEPHOMaps.createTable();
    });
}

mapState = {
    //TODO add anything state/configuration that is specific to the page here
    root: null,
    nationalAreaHash: null,
    indicatorKey: null,
    initiated: false,
    areaTypeDisplayed: null,
    metadata: null,
    comparisonConfig: null
};

function displayMap() {

    // This in the event handler fired when the map colour changes
    ui.storeScrollTop();

    initNationalAreaHash();

    initMap();

    if (
        mapState.initiated
            &&
            mapState.areaTypeDisplayed != FT.model.areaTypeId) {
        NEPHOMaps.LoadLayer($('#areaTypes').val());
    }
    else if (mapState.initiated) {
        NEPHOMaps.initAreaCodeToSigHash();
        NEPHOMaps.initGetColour();

        if (NEPHOMaps.AreaLayer) {
            NEPHOMaps.updateMap();
        }

        addNearestNeighboursToMaps();
    }

    showAndHidePageElements();

    // This is the last thing before the unlock statement at the end of displayMap
    ui.setScrollTop();

    unlock();
}

// Get the national data values
function getAreaValues(root, model) {

    var key = getIndicatorKey(root, model) + getCurrentComparator().Code;
    mapState.indicatorKey = key;

    var areaValues = loaded.areaValues;
    if (areaValues.hasOwnProperty(key)) {
        mapState.areaValues = areaValues[key];
        ajaxMonitor.callCompleted();
    } else {
        var parameters = 'par=' + getCurrentComparator().Code +
            '&gid=' + model.groupId + '&off=0&iid=' + root.IID +
            '&sex=' + root.SexId + '&age=' + root.AgeId +
            '&ati=' + model.areaTypeId + '&com=' + comparatorId +
            getRestrictByProfileParameter();
        getData(getAreaValuesCallback, 'av', parameters);
    }
}

function getAreaValuesCallback(obj) {

    // Hash: Key -> area code, Value -> coredataset
    var hash = {},
        areaOrder = [];

    _.each(obj, function (data) {
        hash[data.AreaCode] = data;
        areaOrder.push({ AreaCode: data.AreaCode, Val: data.Val, ValF: data.ValF });
    });

    areaOrder.sort(sortData).reverse();

    // First, count number of areas which have a value.
    var numAreas = 0;
    $.each(areaOrder, function (i, v) {
        if (v.ValF != "-") {
            numAreas++;
        }
    });

    // Second, set orderFrac for each.
    var j = 0;
    $.each(areaOrder, function (i, v) {
        if (v.ValF == "-") {
            hash[v.AreaCode].order = -1;
            hash[v.AreaCode].orderFrac = -1;
        }
        else {
            hash[v.AreaCode].order = numAreas - j;
            hash[v.AreaCode].orderFrac = 1 - j / numAreas;
            hash[v.AreaCode].quartile = Math.ceil(((numAreas + 1) - (j + 1)) / (numAreas / 4));
            hash[v.AreaCode].quintile = Math.ceil(((numAreas + 1) - (j + 1)) / (numAreas / 5));
            j++;
        }
    });


    mapState.areaValues = hash;

    loaded.areaValues[mapState.indicatorKey] = hash;

    ajaxMonitor.callCompleted();
}

// Look up to PHOLIO area object from area code if needed
function initNationalAreaHash() {

    var nationalAreaHash = {};

    _.each(loaded.areaLists[FT.model.areaTypeId], function (area) {
        nationalAreaHash[area.Code] = area;
    });

    mapState.nationalAreaHash = nationalAreaHash;
}

function initMap() {
    if (mapState.initiated) {
        return;
    }

    $('.keyValueNote').hide();

    var exportMap = '<span class="export-chart-box" style="margin-left: 11px;"><a class="export-link" href="javascript:exportMap()">Export map as image</a></span>';
    var exportChart = '<span class="export-chart-box" style="margin-left: 11px;"><a class="export-link" href="javascript:exportMapChart()">Export chart as image</a></span>';

    var html = "<div id='maps_map'></div><div id='maps_resize_width'></div><div id='maps_info'><div id='map_colour_box'>Map colour: <select id='map_colour'><option value='benchmark'>Comparison to benchmark</option><option value='quartile'>Quartiles</option><option value='quintile'>Quintiles</option><option value='continuous'>Continuous</option></select></div><div id='maps_table'>Please wait...</div></div>" + exportMap + exportChart + "<div id='maps_chart'></div>";
    pages.getContainerJq().html(html);


    setTimeout(function () {
        mapState.initiated = true;

        jQuery.tablesorter.addParser({
            id: "fancyNumber",
            is: function (s) {
                return /^[0-9]?[0-9,\.]*$/.test(s);
            },
            format: function (s) {
                return jQuery.tablesorter.formatFloat(s.replace(/,/g, ''));
            },
            type: "numeric"
        });

        NEPHOMaps = new NEPHOMaps();
        NEPHOMaps.initMap("maps_map", FT.model.areaTypeId);

        $('#map_colour').change(function () {
            displayMap();
        });

        $('#maps_resize_width').click(function () {
            var isFullWidth = $(this).hasClass("fullWidth");
            var newWidth = (isFullWidth ? 470 : 980);
            $(this).toggleClass("fullWidth");
            $('#maps_map').animate({ width: newWidth }, 1000);
            window.setTimeout(function () {
                NEPHOMaps.map.invalidateSize();
            }, 1050);
        });

        addNearestNeighboursToMaps();
    }, 0);
}

function updateComparisonConfig() {

    mapState.root = groupRoots[getIndicatorIndex()];
    mapState.metadata = ui.getMetadataHash()[mapState.root.IID];
    mapState.comparisonConfig = new ComparisonConfig(mapState.root, mapState.metadata);
}

function hideAndShowMapMenus() {
    var isNationalComparator = comparatorId === NATIONAL_COMPARATOR_ID;

    updateComparisonConfig();

    if (enumParentDisplay !== PARENT_DISPLAY.NATIONAL_AND_REGIONAL) {
        // Hide area and menus that aren't relevant
        var menus = FT.menus;
        menus.area.hide();
        menus.parentType.hide();
    }

    $('#region-menu-box').toggle(!isNationalComparator);

    // Show appropriate key
    var showAdHocKey = jQuery.inArray($('#map_colour').val(), ['quartile', 'quintile', 'continuous']) != -1;

    if (!showAdHocKey) {
        // Use it if the comparison is against a target

        showAdHocKey = mapState.comparisonConfig.useTarget;
        showAdHocKey = (_.isUndefined(showAdHocKey) ? false : showAdHocKey);
        if (showAdHocKey) {
            setTargetLegendHtml(mapState.comparisonConfig, mapState.metadata);
        }
    }

    $('#keyAdHoc').toggle(showAdHocKey);
    $('#keyTartanRug').toggle(!showAdHocKey);

    if (!isNationalComparator && mapState.initiated) {
        // Zoom to region
        var maxlat = -90;
        var maxlng = -180;
        var minlng = 180;
        var minlat = 90;

        $.each(areaHash, function (areaCode) {
            var layer = NEPHOMaps.areaCodeToAreaLayerHash[areaCode];

            if (!_.isUndefined(layer)) {
                bounds = layer.getBounds();

                maxlat = Math.max(bounds.getNorthEast().lat, maxlat);
                minlat = Math.min(bounds.getSouthWest().lat, minlat);
                maxlng = Math.max(bounds.getNorthEast().lng, maxlng);
                minlng = Math.min(bounds.getSouthWest().lng, minlng);
            }
        });

        var newViewState = isNationalComparator + ":" + FT.model.parentCode;

        if (newViewState != NEPHOMaps.viewState) {
            NEPHOMaps.viewState = newViewState;
            if (maxlat != -90 && maxlng != 90) {
                NEPHOMaps.map.fitBounds([[minlat, minlng], [maxlat, maxlng]]);

            } else {
                NEPHOMaps.defaultView();
            }
        }
    }
    else if (mapState.initiated) {
        var newViewState = isNationalComparator;
        if (newViewState != NEPHOMaps.viewState) {
            NEPHOMaps.viewState = newViewState;
            NEPHOMaps.defaultView();
        }
    }
}

function addNearestNeighboursToMaps() {

    if (FT.model.isNearestNeighbours()) {

        NEPHOMaps.SelectedFeatures = [];
        NEPHOMaps.createTable();
        // Add nearest neighbours
        _.each(areaHash, function (area) {
            NEPHOMaps.SelectFeature(area.Code);
        });
        NEPHOMaps.updateMap();
    }
}


//This call enables visualisation pages to be included dynamically
pages.add(PAGE_MODES.MAP, {
    id: 'map',
    title: 'Map',
    goto: goToMapPage,
    gotoName: 'goToMapPage',
    needsContainer: true,
    jqIds: ['indicatorMenuDiv', '.geo-menu', 'benchmark-box', 'map-key-part2', 'value-note-legend', 'nearest-neighbour-link'],
    jqIdsNotInitiallyShown: ['keyAdHoc', 'keyTartanRug', 'trend-marker-legend-tartan'],
    showHide: hideAndShowMapMenus
});


function NEPHOMaps() {

    var geoLayerIds = {
        19: 2   // CCG
      , 101: 1  // LTLA
      , 102: 0  // UTLA
      , 112: 6  // SCNs
      , 103: 7  // PHE Centers 2013
      , 113: 8  // Westminster Parliament Constituencies
      , 104: 9  // PHE Centers 2015     
    }

    var backgroundLayer = false;
    this.fillOpacity = 1.0;

    this.initMap = function (id, areaType) {
        this.map = L.map(id);
        NEPHOMaps.defaultView();

        var legend = L.control({ position: 'topleft' });

        legend.onAdd = function () {
            var div = L.DomUtil.create('div', 'layerControl info');
            var html = "<a class='leaflet-control-layers-toggle' href='#' title='Layers'></a>"
                    + "<div id='mapOptions' style='display: none;'>"
                    + "<p>Background map</p>";

            for (var i = 0 ; i < baseMaps.length; i++) {
                html += "<div class='basemap " + (baseMaps[i].C ? baseMaps[i].C : baseMaps[i].T) + "'>"
                    + "<input type='radio' id='basemap[" + i + "]' name='basemap'" + (i == 0 ? " checked" : "") + " value='" + i + "'>"
                    + "<label for='basemap[" + i + "]'>"
                    + "<span></span>"
                    + baseMaps[i].T
                    + "</label>"
                    + "</div>";
            }

            html += "<p>Transparency</p>"
                + "<div id='opacity'>";

            for (i = 20; i <= 100; i += 20) {
                html += "<div class='opacity" + (i / 100 == NEPHOMaps.fillOpacity ? " selected" : "") + "'>"
                    + "<input type='radio' name='opacity' value='" + i + "' id='opacity_" + i + "'>"
                    + "<label for='opacity_" + i + "'>"
                    + "<span style='background-position:" + (i * -4.1 + 37) + "px 0px;'></span>"
                    + i + "%"
                    + "</div>";
            }
            html += "</div>"
                 + "</div>";

            div.innerHTML += html;
            return div;
        };

        legend.addTo(this.map);

        $('.layerControl').on("mouseover", function (a) {
            $(this).find("a").hide();
            $(this).find("#mapOptions").show();
        });
        $('.layerControl').on("mouseout", function (a) {
            $(this).find("a").show();
            $(this).find("#mapOptions").hide();
        });

        $('.layerControl input:radio[name=basemap]').on("change", function (a) {
            basemap = baseMaps[$(this).attr("value")];

            if (backgroundLayer) {
                NEPHOMaps.map.removeLayer(backgroundLayer);
            }
            backgroundLayer = basemap.L;
            if (backgroundLayer) {
                NEPHOMaps.map.addLayer(backgroundLayer);
            }
        });

        $('.layerControl input:radio[name=opacity]').on("change", function (a) {
            NEPHOMaps.fillOpacity = $(this).attr("value") / 100;
            $("#opacity div.selected").removeClass("selected");
            $(this).parent().addClass("selected");
            $.each(NEPHOMaps.AreaLayer._layers, function (layer) {
                NEPHOMaps.AreaLayer.resetStyle(layer);
            });
        });

        this.SelectedFeatures = [];
        this.map.attributionControl.setPrefix('');

        this.LoadLayer(areaType);
        this.updateKey();

        $('#maps_table, #maps_chart').mouseout = function () {
            NEPHOMaps.highlightArea("", false);
        }
    };

    this.defaultView = function () {
        this.map.setView(new L.LatLng(53, -2), 6);
    }

    // Load an overlay with LA/CCG geographies.
    this.LoadLayer = function (areaTypeId) {
        if (mapState.areaTypeDisplayed === areaTypeId) {
            console.info("Loading layer " + areaTypeId + " - already displayed...");
            return;
        }

        mapState.areaTypeDisplayed = areaTypeId;

        if (NEPHOMaps.AreaLayer) {
            NEPHOMaps.map.removeLayer(NEPHOMaps.AreaLayer);
        }

        this.areaCodeToAreaLayerHash = {};
        NEPHOMaps.AreaLayer = false;

        var geoLayerId = geoLayerIds[areaTypeId];

        if (_.isUndefined(geoLayerId)) {
            $('#maps_table').html("<h3>Unsupported map type</h3>"
                    + "<p>Maps are not available for " + $('#areaTypes option:selected').html() + "</p>");
            $('#maps_chart').html("..");
            return;
        }

        NEPHOMaps.oldId = 0;
        NEPHOMaps.initAreaCodeToSigHash();
        NEPHOMaps.initGetColour();
        var serviceUrl = 'https://services1.arcgis.com/0IrmI40n5ZYxTUrV/ArcGIS/rest/services/ONS_Boundaries_02/FeatureServer/';
        NEPHOMaps.AreaLayer = L.esri.featureLayer(serviceUrl + geoLayerId,
                {
                    simplifyFactor: .25,//0.75,
                    attribution: "Contains Ordnance Survey data &copy; Crown copyright and database right 2015.",
                    style: function (feature) {
                        var selected = (jQuery.inArray(feature.properties.ONSCD, NEPHOMaps.SelectedFeatures) > -1),
                                        fillColour = getColour(feature.properties.ONSCD),
                                        color = (selected ? "#000000" : "#666666"),
                                        weight = (selected ? 3 : 1);
                        return {
                            weight: weight,
                            dashArray: '',
                            fillOpacity: NEPHOMaps.fillOpacity,
                            color: color,
                            opacity: 1,
                            fillColor: fillColour
                        }
                    },
                    onEachFeature: NEPHOEachFeature
                }
            )
            .addTo(this.map)
            .on('load', function (e) {
                NEPHOMaps.updateMap();
            })
            .on('mouseover', function (e) {

                NEPHOMaps.AreaLayer.resetStyle(NEPHOMaps.oldId);
                NEPHOMaps.oldId = e.layer.feature.id;
                e.layer.bringToFront();
                NEPHOMaps.AreaLayer.setFeatureStyle(e.layer.feature.id, {
                    color: '#9D78D2',
                    weight: 3,
                    opacity: 1
                });

                var html = "<b>" + e.layer.feature.properties.ONSNM + "</b>",
                    values = mapState.areaValues[e.layer.feature.properties.ONSCD];

                NEPHOMaps.BarchartHighlightArea(values.AreaCode, true);

                var root = groupRoots[getIndicatorIndex()],
                    indicatorId = root.IID,
                    unit = ui.getMetadataHash()[indicatorId].Unit,
                    data = new CoreDataSetInfo(values);

                html += data.isValue() ? "<br>" + new ValueWithUnit(unit).getFullLabel(values.ValF) : "<br>-";

                tooltipManager.setHtml(html);
                tooltipManager.positionXY(e.originalEvent.pageX + 10, e.originalEvent.pageY + 15);
                tooltipManager.showOnly();
            })
            .on('mouseout', function (e) {
                if (NEPHOMaps.oldId) {
                    NEPHOMaps.AreaLayer.resetStyle(NEPHOMaps.oldId);
                    NEPHOMaps.oldId = 0;
                }
                tooltipManager.hide();
                // remove highlight for selected area from bar chart
                try {
                    var selecedAreaCode = NEPHOMaps.chart.getSelectedPoints()[0].key;
                    NEPHOMaps.BarchartHighlightArea(selecedAreaCode, false);
                } catch (err) {
                }
            })
            .on('mousemove', function (e) {
                tooltipManager.positionXY(e.originalEvent.pageX + 10, e.originalEvent.pageY + 15);
            })
            .on('click', function (e) {
                NEPHOMaps.SelectFeature(e.layer.feature.properties.ONSCD);
                e.layer.bringToFront();
            });
    }

    // Default function - this is overwitten by initGetColour.
    this.getColour = function () {
        return colours.noComparison;
    }

    this.initAreaCodeToSigHash = function () {
        // You need to get from area code to Sig so I would define a hash
        NEPHOMaps.areaCodeToSigHash = {};

        updateComparisonConfig();

        $.each(mapState.areaValues, function (areaCode, data) {
            if (!_.isUndefined(mapState.comparisonConfig)) {
                NEPHOMaps.areaCodeToSigHash[areaCode] = data.Sig[mapState.comparisonConfig.comparatorId];
            }
            else {
                NEPHOMaps.areaCodeToSigHash[areaCode] = data.Sig[comparatorId/*global I think*/];
            }
        });
    };

    this.initGetColour = function () {
        // Get a function that will give you a colour from a significance value
        NEPHOMaps.getColour = NEPHOMaps.getSigColourFunction(mapState.root.PolarityId);
    }

    this.updateMap = function () {
        if (!NEPHOMaps.AreaLayer) {
            return;
        }

        $.each(NEPHOMaps.AreaLayer._layers, function (layer) {
            NEPHOMaps.AreaLayer.resetStyle(layer);
        });

        $('#ExportToPDFStatus').html("Create PDF");
        NEPHOMaps.updateKey();
        NEPHOMaps.createTable();
        addMapBarChart();
    }

    this.updateKey = function () {
        var html = "<div class='fl' style='width: 85%;'>";

        switch ($('#map_colour').val()) {
            case "quartile":
                $('#keyTartanRug').hide();
                $('#keyAdHoc').show();
                html += "<table class='keyTable customKeyTable' cellspacing='2'><tbody><tr><td class='keyText'>Quartiles:</td>"
                      + "<td style='background-color: #E8C7D1;'>Q1 (lowest)</td>"
                      + "<td style='background-color: #B74D6D;' class='whiteText'>Q2</td>"
                      + "<td style='background-color: #98002E;' class='whiteText'>Q3</td>"
                      + "<td style='background-color: #700023;' class='whiteText'>Q4 (highest)</td>"
                      + "</tr></tbody></table>";
                break;
            case "quintile":
                $('#keyTartanRug').hide();
                $('#keyAdHoc').show();
                html += "<table class='keyTable customKeyTable' cellspacing='2'><tbody><tr><td class='keyText'>Quintiles:</td>"
                      + "<td style='background-color: #DED3EC;'>Q1 (Low)</td>"
                      + "<td style='background-color: #BEA7DA;'>Q2</td>"
                      + "<td style='background-color: #9E7CC8;' class='whiteText'>Q3</td>"
                      + "<td style='background-color: #7E50B6;' class='whiteText'>Q4</td>"
                      + "<td style='background-color: #5E25A4;' class='whiteText'>Q5 (High)</td>"
                      + "</tr></tbody></table>";
                $('#map-key-part1').html(html);
                break;
            case "continuous":
                $('#keyTartanRug').hide();
                $('#keyAdHoc').show();
                html += "<table class='keyTable customKeyTable' cellspacing='2'><tbody><tr><td class='keyText'>Continuous:</td>"
                      + "<td style='background-color: #FFE97F;'>Lowest</td>"
                      + "<td class='continual_range whiteText'>Mid</td>"
                      + "<td style='background-color: #151C55;' class='whiteText'>Highest</td>"
                      + "</tr></tbody></table>";
                break;
            case "benchmark":
            default:
                $('#keyTartanRug').show();
                $('#keyAdHoc').hide();
                return;
        }
        html += "</div>";
        $('#keyAdHoc').html(html).show();
    }

    this.getSigColourFunction = function (polarityId) {

        var c = colours/*global definition of common colours*/;
        same = c.same,
        noComparison = c.noComparison;

        // No colour comparison should be made
        if (polarityId === -1) {
            return function () {
                return noComparison;
            };
        }

        // Blues
        if (polarityId === PolarityIds.BlueOrangeBlue) {
            return function (sig) {
                // Blues
                return sig === 1 ? c.bobLower :
                    sig === 2 ? same :
                    sig === 3 ? c.bobHigher :
                    noComparison;
            }
        }

        // RAG
        return function (sig) {
            if (mapState.comparisonConfig.useQuintileColouring) {
                switch (true) {
                    case (sig > 0 && sig < 6):
                        var quintile = 'quintile' + sig;
                        return colours[quintile];
                }
            }

            return sig === 1 ? c.worse :
                sig === 2 ? same :
                sig === 3 ? c.better :
                noComparison;
        };
    }

    this.createTable = function () {
        var html = '';

        $.each(mapState.areaValues, function (area_code, values) {
            var area = mapState.nationalAreaHash[values.AreaCode];

            if (_.isUndefined(area)) {
                return;
            }

            var selected = jQuery.inArray(values.AreaCode, NEPHOMaps.SelectedFeatures) > -1;

            if (selected) {
                var colour = getColour(values.AreaCode),
                    valueDisplayer = new ValueDisplayer();

                var nearestNeighbourRank = function () {
                    if (FT.model.isNearestNeighbours()) {
                        var rankValue = area.Code === FT.model.areaCode ? '-' : area.Rank;
                        if (_.isUndefined(rankValue)) rankValue = '-';
                        return "<td>" + rankValue + "</td>";
                    } else {
                        return '';
                    }
                }

                html += "<tr id='tr-" + values.AreaCode + "' "
                    + (_.isUndefined(values.NoteId) ? "" : " class='hasNote'")
                    + "><td style='border-left-color: " + colour + "'>"
                          + area.Name + "</td>" + nearestNeighbourRank() +
                    "<td>"
                    + (values.Count === -1 ? "-" : new CommaNumber(values.Count).rounded()) + "</td>"
                          + (_.isUndefined(values.NoteId)
                            ? "<td>" + valueDisplayer.byNumberString(values.ValF) + "</td>"
                            : "<td class='hasNote' data-NoteId='" + values.NoteId + "'>*</td>"
                            )
                          + "<td>" + valueDisplayer.byNumberString(values.LoCIF) + "</td>"
                          + "<td>" + valueDisplayer.byNumberString(values.UpCIF) + "</td>"
                          + "</tr>";
            }

        });

        if (html === '') {
            $('#maps_table').html("<p>Select an area from the map</p>");
        }
        else {
            var nearestNeighbourOption = function () {
                if (FT.model.isNearestNeighbours()) {
                    return '<th>Rank</th>';
                } else {
                    return '';
                }
            }
          
            html = "<table id='map_table'><thead><tr><th>Area</th>" + nearestNeighbourOption() + "<th>Count</th><th>Value</th><th>LCI</th><th>UCI</th></tr><thead><tbody id='map_table_body'>" + html + "</tbody></table>";
            $('#maps_table').html(html);
            $("#map_table").tablesorter({
                sortList: [[0, 0]],
                headers: {
                    1: { sorter: 'fancyNumber' },
                    2: { sorter: 'fancyNumber' },
                    3: { sorter: 'fancyNumber' },
                    4: { sorter: 'fancyNumber' }
                }
            });

            $('#map_table tbody tr')
                .hover(function (event) {
                    $(this).mouseout(function () {
                        $(this).removeClass("hover");
                        tooltipManager.hide();
                    });
                    var areaCode = $(this).attr("id").substring(3);
                    NEPHOMaps.highlightArea(areaCode, true);
                    NEPHOMaps.BarchartHighlightArea(areaCode, true);
                })
                .mouseout(function () {
                    var areaCode = $(this).attr("id").substring(3);
                    NEPHOMaps.highlightArea(areaCode, false);
                    $(this).removeClass("hover");
                    NEPHOMaps.BarchartHighlightArea(areaCode, false);

                    if (NEPHOMaps.BarChartHoverAreaCode == areaCode) {
                        var areaCode = this.key;
                        NEPHOMaps.highlightArea(areaCode, false);

                        delete (NEPHOMaps.BarChartHoverAreaCode);
                    }
                })
                .click(function () {
                    var areaCode = $(this).attr("id").substring(3);
                    NEPHOMaps.SelectFeature(areaCode);
                });

            $('#map_table tbody tr td.hasNote').mouseenter(function (event) {
                var html = new ValueNoteTooltipProvider().getHtmlFromNoteId($(this).attr("data-NoteId"));
                tooltipManager.setHtml(html);
                tooltipManager.positionXY(event.pageX + 10, event.pageY + 15);
                tooltipManager.showOnly();
            }).mousemove(function (event) {
                tooltipManager.positionXY(event.pageX + 10, event.pageY + 15);
            }).mouseout(function (event) {
                tooltipManager.hide();
            });

        }
    };

    this.highlightArea = function (areaCode, highlight) {

        $('#map_table_body tr.hover').removeClass("hover");
        if (highlight) {
            $('tr#tr-' + areaCode).addClass("hover");
        }

        if (NEPHOMaps.highlightedLayer) {
            NEPHOMaps.AreaLayer.resetStyle(NEPHOMaps.highlightedLayer.feature.id);
            NEPHOMaps.highlightedLayer = false;
        }

        $.each(NEPHOMaps.AreaLayer._layers, function (i, layer) {
            if (layer.feature.properties.ONSCD == areaCode && highlight) {
                NEPHOMaps.highlightedLayer = layer;

                layer.setStyle({
                    color: '#9D78D2',
                    weight: 3,
                    opacity: 1
                });


                if (!L.Browser.ie && !L.Browser.opera) {
                    layer.bringToFront();
                }
            }
        });
    };

    this.BarchartHighlightArea = function (areaCode, highlight) {
        try {
            // Highlight on the bar chart.
            var k = NEPHOMaps.BarChartKey[areaCode];
            if (!_.isUndefined(k)) {
                NEPHOMaps.chart.series[0].data[k].select(highlight);
            }
        } catch (err) { }
    };

    this.SelectFeature = function (areaCode) {
        if (_.isUndefined(NEPHOMaps.SelectedFeatures)) {
            NEPHOMaps.SelectedFeatures = [];
        }
        var index = jQuery.inArray(areaCode, NEPHOMaps.SelectedFeatures);
        if (index != -1) { // Remove selected feature
            NEPHOMaps.SelectedFeatures.splice(index, 1);
            NEPHOMaps.createTable();
            var layer = NEPHOMaps.areaCodeToAreaLayerHash[areaCode];
            NEPHOMaps.AreaLayer.resetStyle(layer);
            try {
                if (NEPHOMaps.highlightedLayer.feature.properties.ONSCD == areaCode) { // Remove highlight
                    NEPHOMaps.AreaLayer.resetStyle(NEPHOMaps.highlightedLayer.feature.id);
                    NEPHOMaps.highlightedLayer = false;
                }
            } catch (err) { }
        }
        else // Add selected feature
        {
            NEPHOMaps.SelectedFeatures.push(areaCode);
            NEPHOMaps.createTable();
        }
    }

    this.zoomToFeature = function (e) {
        NEPHOMaps.map.fitBounds(e.target.getBounds());
    };
};

function getColour(areaCode) {
    if (_.isUndefined(mapState.areaValues[areaCode])) {
        return "#FFFFFF";
    }

    if (mapState.areaValues[areaCode].ValF === "-") {
        return "#C9C9C9";
    }

    switch ($('#map_colour').val()) {
        case "quartile":
            switch (mapState.areaValues[areaCode].quartile) {
                case 1:
                    return "#E8C7D1";
                case 2:
                    return "#B74D6D"; // PHE red
                case 3:
                    return "#98002E";
                case 4:
                    return "#700023";
                default:
                    return "#F7FF23";
            }
        case "quintile":
            switch (mapState.areaValues[areaCode].quintile) {
                case 1:
                    return "#DED3EC";
                case 2:
                    return "#BEA7DA";
                case 3:
                    return "#9E7CC8";
                case 4:
                    return "#7E50B6";
                case 5:
                    return "#5E25A4";
                default:
                    return "#F7FF23";
            }
        case "continuous":
            var seed = mapState.areaValues[areaCode].orderFrac;
            var r = 21;
            var g = 28;
            var b = 85;

            r = 255 - Math.round(seed * (255 - r));
            g = 233 - Math.round(seed * (233 - g));
            b = 127 - Math.round(seed * (127 - b));
            return "rgb(" + r + "," + g + "," + b + ")";
        default:
            if (_.isUndefined(NEPHOMaps)) {
                return "rgb(30,230,30)";
            }

            // If areaCodeToSigHash isn't configured, do so now.
            if (_.isUndefined(NEPHOMaps.areaCodeToSigHash)) {
                NEPHOMaps.initAreaCodeToSigHash();
                NEPHOMaps.initGetColour();
            }

            if (_.isUndefined(NEPHOMaps.areaCodeToSigHash)) {
                return 'rgb(240,0,240)';  // Return violent magenta.
            }

            Sig = NEPHOMaps.areaCodeToSigHash[areaCode];

            if (_.isUndefined(Sig)) {
                return 'rgb(150,200,250)';
            }
            else {
                return NEPHOMaps.getColour(Sig);
            }
    }
}

function NEPHOEachFeature(feature, layer) {
    NEPHOMaps.areaCodeToAreaLayerHash[feature.properties.ONSCD] = layer;
}

function mapMoveTooltip(event) {
    event = eventCheckPageXY(event.originalEvent.originalEvent);
    tooltipManager.positionXY(event.pageX + 10, event.pageY + 10);
}

function eventCheckPageXY(event) {
    // Calculate pageX/Y if missing and clientX/Y available
    if (!event.pageX && event.clientX != null) {
        var doc = document.documentElement,
        body = document.body,
        extra = function (prop) {
            return doc && doc[prop] || body && body[prop] || 0;
        };

        event.pageX = event.clientX + extra('scrollLeft') - extra('clientLeft');
        event.pageY = event.clientY + extra('scrollTop') - extra('clientTop');
    }
    return event;
}

function highlightFeature(event) {
    var layer = event.target;

    layer.setStyle({
        weight: 5
    });

    if (!L.Browser.ie && !L.Browser.opera) {
        layer.bringToFront();
    }
    var properties = layer.feature.properties;
    var area = mapState.nationalAreaHash[properties.AreaCode];
    if (_.isUndefined(area)) {
        $('div.copyright').html("Unrecognised area code: " + properties.AreaCode);
    }

    $('#map_table_body tr.hover').removeClass("hover");
    $('#map_table_body tr#tr-' + properties.AreaCode).addClass("hover");

    var data = mapState.areaValues[properties.AreaCode];

    var areaName = _.isUndefined(mapState.nationalAreaHash[properties.AreaCode])
                    ? (properties.AreaName || properties.AreaCode)
                    : mapState.nationalAreaHash[properties.AreaCode].Name;

    // In the tooltip in would be good to display the unit with the value, this should work:
    var html = "<strong>" + areaName + "</strong>";

    if (!_.isUndefined(data)) {
        var root = groupRoots[getIndicatorIndex()],
            indicatorId = root.IID,
            unit = ui.getMetadataHash()[indicatorId].Unit;

        html += "<br />" + (data.NoteId
                ? new ValueNoteTooltipProvider().getHtmlFromNoteId(data.NoteId)
                : new ValueWithUnit(unit).getFullLabel(data.ValF));
    }

    tooltipManager.setHtml(html);

    event = eventCheckPageXY(event);
    tooltipManager.positionXY(event.originalEvent.pageX + 10, event.originalEvent.pageY + 10);
    tooltipManager.showOnly();

    NEPHOMaps.BarchartHighlightArea(properties.AreaCode, true);
}

function buildChartTitle() {
    var root = groupRoots[getIndicatorIndex()],
        unit = ui.getMetadataHash()[root.IID].Unit,
        unitLabel = !String.isNullOrEmpty(unit.Label) ? unit.Label + ', ' : '',
        period = getFirstGrouping(root).Period;        
        return mapState.metadata.ValueType.Label + ' - ' + unitLabel + period;   
}

function addMapBarChart() {
    var chartData = [],
        errorData = [],
        xAxisCategories = [],
        x = 0,
        extraTooltip = "",
        regionValues = {},
        root = groupRoots[getIndicatorIndex()],
        indicatorId = root.IID,
        unit = ui.getMetadataHash()[indicatorId].Unit,
        chartTitle = buildChartTitle(),
        valuesForBarChart = [];

    $.each(NEPHOMaps.AreaLayer._layers, function (layer_id, layer) {
        var values = mapState.areaValues[layer.feature.properties.ONSCD];

        if (!_.isUndefined(values) && values.ValF != "-") {
            valuesForBarChart.push({
                AreaName: layer.feature.properties.ONSNM
                    , Hex: getColour(layer.feature.properties.ONSCD)
                    , AreaCode: layer.feature.properties.ONSCD
                    , Val: values.Val
                    , LCI: values.LoCI
                    , UCI: values.UpCI
                    , ValF: values.ValF
                    , LoCIF: values.LoCIF
                    , UpCIF: values.UpCIF
                    , NoteId: values.NoteId
            });
        }
    });

    valuesForBarChart.sort(sortData);

    NEPHOMaps.BarChartKey = {};

    $.each(valuesForBarChart, function (i, Area) {
        NEPHOMaps.BarChartKey[Area.AreaCode] = i;
        var AreaName = (_.isUndefined(Area.AreaName) ? mapState.nationalAreaHash[Area.AreaCode].Name : Area.AreaName);


        xAxisCategories.push({ AreaName: AreaName, ValF: Area.ValF, LoCIF: Area.LoCIF, UpCIF: Area.UpCIF, AreaCode: Area.AreaCode, NoteId: Area.NoteId });

        chartData.push({
            color: Area.Hex
            , x: x
            , key: Area.AreaCode
            , y: Area.Val
        });
        errorData.push([Area.LCI, Area.UCI]);
        x++;
    });

    var yAxis = {
        labels: {
            formatter: function () {
                return this.value;
            },
            style: {
                color: '#999999'
            }
        },
        title: {
            text: ''
        }
    };

    var series = [
            {
                type: 'column',
                name: 'Value',
                data: chartData,
                showInLegend: false,
            }
            ,
            {
                type: 'errorbar',
                name: 'My Errors',
                data: errorData,
                zIndex: 1000,
                color: '#666666'
            }
    ];

    // If England value is available, add to the barchart.
    _.each(mapState.root.Grouping, function (grouping) {
        if (grouping.ComparatorId == 4 && grouping.ComparatorData.AreaCode == "E92000001") {
            regionValues[series.length] = grouping.ComparatorData.ValF;
            series.push({
                type: 'line',
                name: 'England',
                data: [[0, grouping.ComparatorData.Val], [x - 1, grouping.ComparatorData.Val]],
                marker: {
                    enabled: false
                  , symbol: 'x'
                },
                color: '#333333'
            });
            extraTooltip += "<br>England: " + new ValueWithUnit(unit).getFullLabel(grouping.ComparatorData.ValF);
        }
        else if (grouping.ComparatorId != 4 && grouping.ComparatorId == comparatorId && grouping.ComparatorData.ValF != "-") {
            regionValues[series.length] = grouping.ComparatorData.ValF;
            series.push({
                type: 'line',
                name: getCurrentComparator().Name,
                data: [[0, grouping.ComparatorData.Val], [x - 1, grouping.ComparatorData.Val]],
                marker: {
                    enabled: false
                  , symbol: 'x'
                },
                color: '#0000FF'
            });
            extraTooltip += "<br>" + getCurrentComparator().Name + ": " + new ValueWithUnit(unit).getFullLabel(grouping.ComparatorData.ValF);
        }
    });


    NEPHOMaps.chart = new Highcharts.Chart({
        chart: {
            renderTo: "maps_chart",
            backgroundColor: null
        },
        credits: { enabled: false }
            ,
        legend: {
            enabled: true,
            layout: 'vertical',
            borderWidth: 0
        }
            ,
        title: {
            text: chartTitle
        },
        xAxis: {
            labels: { enabled: false },
            tickLength: 0
        },
        yAxis: yAxis,
        tooltip: {
            shared: false
            ,
            formatter: function () {
                if (this.series.type == "line") {
                    return "<b>" + this.series.name + "</b><br />"
                        + new ValueWithUnit(unit).getFullLabel(regionValues[this.series.index]);
                }


                if (!_.isUndefined(NEPHOMaps.BarChartHoverAreaCode)) {
                    NEPHOMaps.highlightArea(NEPHOMaps.BarChartHoverAreaCode, false);
                }

                NEPHOMaps.highlightArea(xAxisCategories[this.x].AreaCode, true);
                NEPHOMaps.BarChartHoverAreaCode = xAxisCategories[this.x].AreaCode;

                var s = '<b>' + xAxisCategories[this.x].AreaName + '</b>';

                s += "<br>" + new ValueWithUnit(unit).getFullLabel(xAxisCategories[this.x].ValF);


                if (!_.isUndefined(xAxisCategories[this.x].NoteId)) {
                    s += "<br><em>" + loaded.valueNotes[xAxisCategories[this.x].NoteId].Text + "</em>";
                }

                if (!_.isUndefined(xAxisCategories[this.x].LoCIF)) {
                    s += "<br>LCI: " + new ValueWithUnit(unit).getFullLabel(xAxisCategories[this.x].LoCIF);
                }
                if (!_.isUndefined(xAxisCategories[this.x].UpCIF)) {
                    s += "<br>UCI: " + new ValueWithUnit(unit).getFullLabel(xAxisCategories[this.x].UpCIF);
                }


                s += "<br>Rank: " + (this.x + 1);

                s += extraTooltip;

                return s;
            }
        },
        plotOptions: {
            column: {
                animation: false,
                pointPadding: 0,
                borderWidth: 0,
                groupPadding: 0,
                shadow: false,
                fillOpacity: 0.75,
                states: {
                    select: {
                        color: '#000000',
                        brightness: .1,
                        fillOpacity: 0.95
                    },
                    hover: {
                        brightness: .1,
                        fillOpacity: 0.95
                    }
                },
                point: {
                    events: {
                        click: function () {
                            if (!_.isUndefined(NEPHOMaps.BarChartHoverAreaCode)) {
                                NEPHOMaps.SelectFeature(NEPHOMaps.BarChartHoverAreaCode);
                            }
                            else {
                                NEPHOMaps.SelectFeature(this.key);
                            }
                        }
                    }
                }
            }
            ,
            line: {
                animation: false,

                states: {
                    hover: {
                        lineWidth: 0,
                        marker: {
                            enabled: false
                         , symbol: 'x'
                        }
                    }
                }
            }
            ,
            errorbar: {
                animation: false
            }
        },
        exporting: {
            enabled: false,
            chartOptions: {
                title: {
                    text: ''
                }
            }
        },
        series: series
    });
}

function exportMapChart() {

    NEPHOMaps.chart.exportChart({ type: 'image/png' }, {
        chart: {
            spacingTop: 70,
            height: 312,
            width: 490,
            events: {
                load: function () {
                    var title = getTitle();
                    this.renderer.text(title, 250, 15)
                                .attr({
                                    align: 'center'
                                })
                                .css({
                                    color: '#333',
                                    fontSize: '10px',
                                    width: '450px'
                                })
                                .add();
                }
            }
        }
    });
}

function exportMap() {
    if (isIE8()) {
        browserUpgradeMessage();
    } else {          
        $('.leaflet-top').hide(); // hide the map options buttons
        var title = getTitle();
        // Add title div
        $('<div id="map-export-title" style="text-align: center; font-family:Arial;">' + title + '</div>').appendTo('#maps_map');
        var mapContainer = $('#maps_map');
        saveElementAsImage(mapContainer, 'Map');
        $('#map-export-title').remove(); // Remove title div
        $('.leaflet-top').show(); // restore the map options
    }
}

function getTitle() {
    var menus = FT.menus;
    var root = groupRoots[getIndicatorIndex()];
    var indicatorName = ui.getMetadataHash()[root.IID].Descriptive.Name + new SexAndAge().getLabel(root);
    var chartTitle = buildChartTitle();
    var title = '<b>Map of ' + menus.areaType.getName() + 's in ' + getCurrentComparator().Name + ' for ' + indicatorName + '<br/> (' + chartTitle + ')</b>';
    return title;
}

var baseMaps = [
    {
        T: "No background",
        L: false,
        C: "None"
    },
    {
        T: "Topographic",
        L: L.esri.basemapLayer('Topographic'),
        C: "Topographic"
    },
    {
        T: "Streets",
        L: L.esri.basemapLayer('Streets')
    },
    {
        T: "Gray",
        L: L.esri.basemapLayer('Gray')
    }
];