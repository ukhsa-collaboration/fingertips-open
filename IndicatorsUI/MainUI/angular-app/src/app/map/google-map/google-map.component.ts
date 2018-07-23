import {
    Component, OnInit, Input, Output, ElementRef,
    ViewChild, EventEmitter, OnChanges, SimpleChanges, ChangeDetectionStrategy
} from '@angular/core';
import 'rxjs/rx';
import { GoogleMapService } from '../googleMap.service';
import { IndicatorService } from '../../shared/service/api/indicator.service';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { CoreDataHelperService } from '../../shared/service/helper/coreDataHelper.service';
import {
    FTModel, FTRoot, Area, GroupRoot, CoreDataSet, CoreDataHelper, Unit,
    IndicatorMetadataHash, IndicatorMetadata
} from '../../typings/FT.d';
import { ComparatorIds } from '../../shared/shared';
import * as _ from 'underscore';

@Component({
    selector: 'ft-google-map',
    templateUrl: './google-map.component.html',
    styleUrls: ['./google-map.component.css'],
    providers: [GoogleMapService]
})
export class GoogleMapComponent implements OnChanges {
    map: google.maps.Map;
    @ViewChild('googleMap') mapEl: ElementRef;
    @Output() mapInit = new EventEmitter();
    @Output() hoverAreaCodeChanged = new EventEmitter();
    @Output() selectedAreaChanged = new EventEmitter();
    @Input() areaTypeId: number = null;
    @Input() currentAreaCode: string = null;
    @Input() areaCodeColour = null;
    @Input() refreshColour;
    @Input() isBoundaryNotSupported;
    @Input() selectedAreaList;
    @Input() sortedCoreData: Map<string, CoreDataSet> = null;
    path: string = this.ftHelperService.getURL().img;
    isError = false;
    errorMessage: string;
    showOverlay = false;
    currentPolygons: google.maps.Polygon[] = [];
    selectedPolygon: google.maps.Polygon = null;
    boundry: geoBoundry.Boundry;
    coreDataSet: CoreDataSet;
    baseMaps: BaseMap[] = [
        {
            name: 'No background',
            val: 0,
            cssClass: 'None'
        },
        {
            name: 'Streets',
            val: 1,
            cssClass: 'Streets'
        }
    ];
    selectedbaseMap: BaseMap;
    fillOpacity = 1.0;
    opacityArray: number[] = [20, 40, 60, 80, 100];

    /*purple boundry on polygon with bold border -represent that polygon is highlighted but not in a table */
    purpleHighlightPolyOption: google.maps.PolygonOptions = { strokeColor: '#9D78D2', strokeWeight: 3 };
    /*black boundry on polygon with bold border -represent that polygon is in table */
    blackSelectedPolyOption: google.maps.PolygonOptions = { strokeColor: '#000000', strokeWeight: 3 };
    /*black boundry on polygon with bolder border -represent that polygon is in table as well as highlighted */
    blackHighlightPolyOption: google.maps.PolygonOptions = { strokeColor: '#000000', strokeWeight: 5 };
    /* gray boundry on polygon with normal regular border */
    grayPolyOption: google.maps.PolygonOptions = { strokeColor: '#333333', strokeWeight: 1 };
    /*black boundry on polygon with bold border -represent that polygon is in table */
    unselectedPolyOption: google.maps.PolygonOptions = { strokeColor: '#333333', strokeWeight: 1 };

    constructor(private mapService: GoogleMapService,
        private indicatorService: IndicatorService, private ftHelperService: FTHelperService,
        private coreDataHelper: CoreDataHelperService
    ) { }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['areaTypeId']) {
            if (this.areaTypeId) {
                this.loadMap();
                this.loadPolygon(this.areaTypeId, this.path);
            }
        }
        if (changes['currentAreaCode']) {
            let areaCode = changes['currentAreaCode'].currentValue;
            if (areaCode) {
                this.highlightPolygon(this.currentAreaCode);
            } else {
                this.unhighlightSelectedPolygon();
            }
        }
        if (changes['refreshColour']) {
            let localRefreshColour = changes['refreshColour'].currentValue;
            if (localRefreshColour !== undefined) {
                if (this.areaCodeColour) {
                    this.colourFillPolygon(false);
                }
            }
        }
        if (changes['selectedAreaList']) {
            let localSelectedAreaList = changes['selectedAreaList'].currentValue;
            if (localSelectedAreaList) {
                this.removeSelectedPolygon();
            }
        }
    }

    unhighlightSelectedPolygon() {
        if (this.selectedPolygon) {
            this.selectedPolygon.setOptions(this.unselectedPolyOption);
        }
    }

    removeSelectedPolygon() {
        if (this.map) {
            for (let i = 0; i < this.currentPolygons.length; i++) {
                let areaCode = this.currentPolygons[i].get('areaCode');
                if (!_.contains(this.selectedAreaList, areaCode)) {
                    this.currentPolygons[i].setOptions(this.grayPolyOption);
                } else {
                    this.currentPolygons[i].setOptions(this.blackSelectedPolyOption);
                }
            }
        }
    }

    displayOverlay(): void {
        this.showOverlay = true;
    }

    hideOverlay(): void {
        this.showOverlay = false;
    }

    onOverlaySelectionChange(basemap): void {
        this.selectedbaseMap = Object.assign({}, this.selectedbaseMap, basemap);
        this.styleMap(this.selectedbaseMap);
    }

    onOpacitySelectionChange(opacity): void {
        this.fillOpacity = opacity / 100;
        this.loadPolygon(this.areaTypeId, this.path);
    }

    loadMap() {
        if (!this.isBoundaryNotSupported) {
            /// Load from GoogleMapService and style it 
            const mapOptions: google.maps.MapOptions = {
                zoom: 6,
                disableDoubleClickZoom: false,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                panControl: false,
                zoomControl: true,
                zoomControlOptions: { position: google.maps.ControlPosition.TOP_LEFT },
                scaleControl: false,
                streetViewControl: false,
                mapTypeControl: false,
                fullscreenControl: true,
                backgroundColor: 'hsla(0, 0%, 0%, 0)',
            };
            let mapContainer = null;
            if (this.mapEl && this.mapEl.nativeElement) {
                mapContainer = this.mapEl.nativeElement;
            }
            this.map = this.mapService.loadMap(mapContainer, mapOptions);
            this.selectedbaseMap = this.baseMaps[0];
            if (this.baseMaps) { // No Backgroud at the time of load
                this.onOverlaySelectionChange(this.baseMaps[0]);
            }
            if (this.areaTypeId) {
                this.loadPolygon(this.areaTypeId, this.path);
            }

            this.mapInit.emit({
                map: this.map,
            });
        }
    }

    styleMap(selectedbaseMap: BaseMap): void {
        if (this.map) {
            const noTiles = 'noTiles';
            let styleArrayForNoBackground: any = [];
            if (selectedbaseMap.val === 0) {
                const visibilityOff: any = [{ visibility: 'off' }];
                styleArrayForNoBackground = [
                    {
                        stylers: [
                            {
                                color: '#ffffff',
                                fillOpacity: 0.0
                            }
                        ]
                    },
                    {
                        featureType: 'road',
                        elementType: 'geometry',
                        stylers: visibilityOff
                    }, {
                        featureType: 'road',
                        elementType: 'labels',
                        stylers: visibilityOff
                    }
                ];
            }
            this.map.setOptions({ styles: styleArrayForNoBackground });
            this.map.mapTypes.set(noTiles, new NoTileMapType());
            this.setMapCenter();
            google.maps.event.addListener(this.map, 'idle', event => {
                let bounds = this.map.getCenter();
                google.maps.event.trigger(this.map, 'resize');
                this.map.setCenter(bounds);
            });
        }
    }

    setMapCenter() {
        if (this.map) {
            const bounds = new google.maps.LatLngBounds();
            const position = new google.maps.LatLng(53.415649, -2.209015);
            bounds.extend(position);
            this.map.setCenter(bounds.getCenter());
            this.map.setZoom(6);
        }
    }

    loadPolygon(areaTypeId: number, path: string) {
        if (this.map && this.areaTypeId && !this.isBoundaryNotSupported) {
            this.mapService.loadBoundries(areaTypeId, path)
                .subscribe(
                data => {
                    this.boundry = <geoBoundry.Boundry>data;
                    this.isError = false;
                    this.removePolygon();
                    this.fillPolygon(this.boundry, this.fillOpacity);
                    this.colourFillPolygon(true);
                },
                error => {
                    this.isError = true;
                    this.errorMessage = <any>error;
                });
        }
    }

    removePolygon(): void {
        if (this.currentPolygons !== undefined) {
            this.currentPolygons.forEach(element => {
                element.setMap(null);
            });
            this.currentPolygons.length = 0;
        }
    }

    getPolygonCoordinates(coordinates): Array<Array<google.maps.LatLng>> {

        let coords: Array<Array<google.maps.LatLng>> = [];
        for (let i = 0; i < coordinates.length; i++) {
            for (let j = 0; j < coordinates[i].length; j++) {
                let path: Array<google.maps.LatLng> = [];
                for (let k = 0; k < coordinates[i][j].length; k++) {
                    let coord = new google.maps.LatLng(coordinates[i][j][k][1], coordinates[i][j][k][0]);
                    path.push(coord);
                }
                coords.push(path);
            }
        }
        return coords;
    }

    fillPolygon(boundry: geoBoundry.Boundry, opacity: number): any {
        if (boundry.features) {

            // Variables to track most recent mouseover event
            let overDate = null;
            let overAreaCode = null;

            let infoWindow: google.maps.InfoWindow = new google.maps.InfoWindow();

            for (let x = 0; x < boundry.features.length; x++) {
                let areaCode = boundry.features[x].properties.AreaCode;
                let coordinates = boundry.features[x].geometry.coordinates;
                let coords = this.getPolygonCoordinates(coordinates);

                // Set polygon fill colour
                let fillColour = '#B0B0B2';
                if (this.areaCodeColour && this.areaCodeColour.length > 0) {
                    fillColour = this.areaCodeColour.get(areaCode);
                }

                let polygon = new google.maps.Polygon({
                    paths: coords,
                    strokeColor: '#333333',
                    strokeOpacity: 0.8,
                    strokeWeight: 1,
                    fillColor: fillColour,
                    fillOpacity: opacity,
                    clickable: true
                });

                polygon.set('areaCode', areaCode);
                polygon.setMap(this.map);

                google.maps.event.addListener(polygon, 'mouseover', (event) => {

                    overAreaCode = areaCode;
                    overDate = new Date();

                    // Display tooltip
                    let tooltip = this.getToolTipContent(areaCode);
                    if (tooltip) {
                        infoWindow.setContent(tooltip);
                        this.setInfoWindowPosition(event, infoWindow);
                        infoWindow.open(this.map);
                    }

                    this.setPolygonBorderColour(polygon);
                });

                google.maps.event.addListener(polygon, 'mousemove', (event) => {
                    this.setInfoWindowPosition(event, infoWindow);
                });

                google.maps.event.addListener(polygon, 'mouseout', (event) => {

                    // Wait in case immediate mouseover event and this mouseover event was
                    // caused by mouse moving over the infowindow
                    setTimeout(function () {
                        var time = new Date().getTime();
                        if (time - overDate.getTime() > 25 && areaCode == overAreaCode)
                            infoWindow.close();
                    }, 25);
                    this.setPolygonBorderColour(polygon);
                });

                google.maps.event.addListener(polygon, 'click', (event) => {
                    if (this.sortedCoreData[areaCode] && this.ftHelperService.isValuePresent(this.sortedCoreData[areaCode].ValF)) {
                        polygon.setOptions(this.blackSelectedPolyOption);
                        this.selectedAreaChanged.emit({ areaCode: areaCode });
                    }
                });

                this.currentPolygons.push(polygon);
            }
        }
    }

    setPolygonBorderColour(polygon) {
        let currentAreaCode = polygon.get('areaCode');
        if (_.contains(this.selectedAreaList, currentAreaCode)) {
            polygon.setOptions(this.blackSelectedPolyOption);
        } else {
            polygon.setOptions(this.grayPolyOption);
        }
    }

    setInfoWindowPosition(event, infoWindow) {
        let pos: google.maps.LatLng = event.latLng;
        infoWindow.setPosition(new google.maps.LatLng(pos.lat() + 0.02, pos.lng()));
    }

    getToolTipContent(areaCode: string): string {
        this.hoverAreaCodeChanged.emit({ areaCode: areaCode });
        let currentGrpRoot: GroupRoot = this.ftHelperService.getCurrentGroupRoot();
        let data: IndicatorMetadata = this.ftHelperService.getMetadata(currentGrpRoot.IID);
        let unit = data.Unit;
        let areaName = '';
        if (areaCode) {
            areaName = this.ftHelperService.getAreaName(areaCode);
        }
        let value = '';
        if (unit !== undefined && this.sortedCoreData[areaCode] !== undefined
            && this.ftHelperService.isValuePresent(this.sortedCoreData[areaCode].ValF)) {
            value = this.sortedCoreData !== null ? '<br>' + this.coreDataHelper.valueWithUnit(unit).
                getFullLabel(this.sortedCoreData[areaCode].ValF)
                : '<br>-';
        }
        let toolTipcontent;
        if (areaName !== '' || value !== '') {
            toolTipcontent = '<b>' + areaName + '</b>' + value;
        }
        return toolTipcontent;
    }

    highlightPolygon(areaCode): void {
        if (this.map) {
            const polygon: google.maps.Polygon = _.where(this.currentPolygons, { areaCode: areaCode })[0];
            if (this.selectedPolygon) {
                let areaCode = this.selectedPolygon.get('areaCode');
                this.selectedPolygon.setMap(null);
                if (_.contains(this.selectedAreaList, areaCode)) {
                    this.selectedPolygon.setOptions(this.blackSelectedPolyOption);
                } else {
                    this.selectedPolygon.setOptions(this.grayPolyOption);
                }
                this.selectedPolygon.setMap(this.map);
            }

            this.selectedPolygon = polygon;

            if (polygon) {
                polygon.setMap(null);
                if (!_.contains(this.selectedAreaList, areaCode)) {
                    polygon.setOptions(this.purpleHighlightPolyOption);
                } else {
                    polygon.setOptions(this.blackHighlightPolyOption);
                }
                polygon.setMap(this.map);
            }
        }
    }

    colourFillPolygon(center: boolean): void {
        if (this.map) {
            let regionPolygons: Array<google.maps.Polygon> = [];
            const currentComparatorId = this.ftHelperService.getComparatorId();
            for (let i = 0; i < (this.currentPolygons.length); i++) {
                let polygon = this.currentPolygons[i];

                // Set polygon fill colour             
                polygon.setMap(null);
                let areaCode = polygon.get('areaCode');
                let color = this.areaCodeColour.get(areaCode);
                if (color === undefined) {
                    color = '#B0B0B2';
                }
                polygon.set('fillColor', color);
                polygon.setMap(this.map);

                if (currentComparatorId !== ComparatorIds.national) {
                    let coreDataset = this.sortedCoreData[areaCode];
                    if (coreDataset) {
                        regionPolygons.push(polygon);
                    }
                }
            }
            /*if Benchmark is region, center and zoom in into that region */
            if (center) {
                if (regionPolygons.length > 0 && currentComparatorId !== ComparatorIds.national) {
                    const bounds = new google.maps.LatLngBounds();
                    for (let i = 0; i < regionPolygons.length; i++) {
                        bounds.extend(this.getPolygonBounds(regionPolygons[i]).getCenter());
                    }
                    this.map.fitBounds(bounds);
                    this.map.setCenter(bounds.getCenter());
                    this.map.setZoom(7);
                }
                if (currentComparatorId === ComparatorIds.national) {
                    this.setMapCenter();
                }
            }
        }
    }

    getPolygonBounds(polygon: google.maps.Polygon) {
        let bounds = new google.maps.LatLngBounds();
        polygon.getPath().forEach(function (element, index) { bounds.extend(element); });
        return bounds;
    }

    onExportClick(event: MouseEvent) {
        event.preventDefault();
        let chartTitle = this.buildChartTitle();
        let root = this.ftHelperService.getCurrentGroupRoot();
        let indicatorName = this.ftHelperService.getMetadataHash()[root.IID].Descriptive.Name +
            this.ftHelperService.getSexAndAgeLabel(root);
        let title = '<b>Map of ' + this.ftHelperService.getAreaTypeName() +
            's in ' + this.ftHelperService.getCurrentComparator().Name +
            ' for ' + indicatorName + '<br/> (' + chartTitle + ')</b>';
        $('<div id="map-export-title" style="text-align: center; font-family:Arial;">' +
            title + '</div>').appendTo(this.mapEl.nativeElement);
        this.ftHelperService.saveElementAsImage(this.mapEl.nativeElement, 'Map');
        $('#map-export-title').remove();

        this.ftHelperService.logEvent('ExportImage', 'Map');
    }

    buildChartTitle(): string {
        const currentGrpRoot: GroupRoot = this.ftHelperService.getCurrentGroupRoot();
        const data: IndicatorMetadata = this.ftHelperService.getMetadata(currentGrpRoot.IID);
        const unit = data.Unit;
        const unitLabel = (typeof unit.Label !== 'undefined') ? unit.Label : '';
        const period: string = currentGrpRoot.Grouping[0].Period;
        return data.ValueType.Name + ' - ' + unitLabel + ' ' + period;
    }
}

export class NoTileMapType implements google.maps.MapType {
    tileSize = new google.maps.Size(1024, 1024);
    maxZoom = 20;
    getTile(tileCoord: google.maps.Point, zoom: number, ownerDocument: Document): Element {
        return ownerDocument.createElement('div');
    }
    releaseTile(tile: Element): void {
        throw new Error('Method not implemented.');
    }
}

export interface BaseMap {
    name: string; val: number; cssClass: string;
}
