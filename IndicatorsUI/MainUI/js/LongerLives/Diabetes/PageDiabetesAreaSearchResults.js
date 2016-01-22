var markers = [],
    easting,
    northing,
    searchArea;

function initPage() {
    
    updateModelFromHash();
}

function updatePage() {
     
    ajaxMonitor.setCalls(1);
    getPractices();
    ajaxMonitor.monitor(viewPage);
}

function viewPage() {
    renderTable();
}

function getPractices() {
    var parameters = new ParameterBuilder()
        .add('easting', easting)
        .add('northing', northing)
        .add('area_type_id', MT.model.areaTypeId);

    ajaxGet('data/nearby_areas', parameters.build(), getGpPracticesNearbyCallback);
}

function renderTable() {

    var rows = loaded.nearbyPractices,
        rowsToDisplay = [];

    for (var i in rows) {
        var row = rows[i];
        rowsToDisplay.push(row);
        var m = [row.AreaName, row.LatLng.Lat, row.LatLng.Lng, row.AreaCode];
        markers.push(m);
    }

    if (isDefined(rowsToDisplay)) {
        // Render data
        $('#diabetes-rankings-table tbody').html(
            templates.render('rows', { rows: rowsToDisplay })            
        );

        $('#no-of-practices').html(
            'There are ' + rows.length + ' practice(s) within 5 miles of ' + searchArea
        );
    } else {
        $('#diabetes-rankings-table tbody').html('No GP practice found');
    }
    unlock();

    addMarkers();
}

function addMarkers() {

    var gm = google.maps;

    var map = getGoogleMap();
    
    var bounds = new gm.LatLngBounds();

    var marker, i;
  
    for (i = 0; i < markers.length; i++) {
       
        var position = new gm.LatLng(markers[i][1], markers[i][2]);
        bounds.extend(position);

        marker = new gm.Marker({            
            position: position,
            map: map,
            title: markers[i][0]
        });

        marker.set('marker_id', markers[i][3]);
      
        var infoWindow = new gm.InfoWindow({});

        gm.event.addListener(marker, 'click', (function (marker, i) {
            return function () {

                infoWindow.setContent(markers[i][0]);
                infoWindow.open(map, marker);
                
                var $tableContainer = $('#table-container');
                
                var scrollTop = $('#' + marker.get('marker_id')).offset().top - 
                        $tableContainer.offset().top +
                        $tableContainer.scrollTop();
                $tableContainer.scrollTop(scrollTop);
                $('.highlight').removeClass('highlight');

                var areaCode = marker.get('marker_id');
                $('#' + areaCode).addClass('highlight');
                $('#' + areaCode + '-address').addClass('highlight');
            };
        })(marker, i));
    }

    fitMapToPracticeResults(map, bounds);
}

function gotoPractice(practiceAreaCode) {
    var model = MT.model;
    model.areaCode = practiceAreaCode;
    MT.nav.practiceDetails(model);
}

NO_DATA = 'n/a';

templates.add('rows',
    '{{#rows}}<tr id="{{AreaCode}}"><td colspan="2"><div class="header national clearfix">{{AreaName}}</div></td></tr>\
<tr id="{{AreaCode}}-address"><td><div>{{AddressLine1}}</div><div>{{AddressLine2}}</div><div>{{Postcode}}</div></td>\
<td><div class="center-text"><br>{{DistanceValF}} miles</div><div><a href="javascript:gotoPractice(\'{{AreaCode}}\')">Practice details</a></div></td></tr><tr><td class="empty" colspan="2">&nbsp;</td></tr>{{/rows}}');

templates.add('counter','<span>{{noOfPractices}}</span>');