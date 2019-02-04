import {
    Component, Input, Output, ElementRef,
    ViewChild, EventEmitter, OnChanges, SimpleChanges
} from '@angular/core';
import 'rxjs/rx';
import { GoogleMapService } from '../googleMap.service';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';

@Component({
    selector: 'ft-google-map-simple',
    templateUrl: './google-map-simple.component.html',
    styleUrls: ['./google-map-simple.component.css']
})
export class GoogleMapSimpleComponent implements OnChanges {
    map: google.maps.Map;
    @ViewChild('googleMap') mapEl: ElementRef;
    @Output() mapInit = new EventEmitter();
    @Output() selectedAreaChanged = new EventEmitter();
    @Input() areaTypeId: number = null;
    @Input() areaSearchText = null;
    @Input() areaCode = null;
    @Input() availableAreas = null;
    @Input() selectedAreas = null;
    @Input() mapAreaCodes = null;
    @Input() mapPolygonSelected = null;
    path: string;
    isError = false;
    errorMessage: string;
    showOverlay = false;
    currentPolygons: google.maps.Polygon[] = [];
    selectedPolygon: google.maps.Polygon = null;
    boundry: geoBoundry.Boundry;
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

    constructor(private mapService: GoogleMapService, private ftHelperService: FTHelperService) { }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['areaTypeId']) {
            if (this.areaTypeId) {
                this.path = this.ftHelperService.getURL().img;
                this.loadMap();
                this.loadPolygon(this.areaTypeId, this.path);
            }
        }
        if (changes['areaCode']) {
            if (this.areaCode) {
                this.updateMapSelection(this.areaCode);
            }
        }
        if (changes['mapAreaCodes']) {
            if (this.mapAreaCodes) {
                this.initialiseMapSelection(this.mapAreaCodes);
            }
        }
    }

    initialiseMapSelection(areaCodes) {
        areaCodes.forEach(element => {
            this.updateMapSelection(element);
        });
    }

    updateMapSelection(areaCode) {
        for (let i = 0; i < (this.currentPolygons.length); i++) {
            const polygon = this.currentPolygons[i];
            const polygonAreaCode = polygon.get('areaCode');

            if (polygonAreaCode === undefined || polygonAreaCode === areaCode) {
                if (this.mapPolygonSelected) {
                    polygon.set('fillColor', '#7FFF92');
                } else {
                    polygon.set('fillColor', '#63A1C3');
                }
            }
        }
    }

    loadMap() {
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

    loadPolygon(areaTypeId: number, path: string) {
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

    removePolygon(): void {
        if (this.currentPolygons !== undefined) {
            this.currentPolygons.forEach(element => {
                element.setMap(null);
            });
            this.currentPolygons.length = 0;
        }
    }

    fillPolygon(boundry: geoBoundry.Boundry, opacity: number): any {
        if (boundry.features) {

            // Variables to track most recent mouseover event
            let overDate = null;
            let overAreaCode = null;

            const infoWindow: google.maps.InfoWindow = new google.maps.InfoWindow();

            for (let x = 0; x < boundry.features.length; x++) {
                const areaCode = boundry.features[x].properties.AreaCode;
                const coordinates = boundry.features[x].geometry.coordinates;
                const coords = this.getPolygonCoordinates(coordinates);

                const polygon = new google.maps.Polygon({
                    paths: coords,
                    strokeColor: '#333333',
                    strokeOpacity: 0.8,
                    strokeWeight: 1,
                    fillColor: '#63A1C3',
                    fillOpacity: opacity,
                    clickable: true
                });

                polygon.set('areaCode', areaCode);
                polygon.setMap(this.map);

                google.maps.event.addListener(polygon, 'mouseover', (event) => {
                    overAreaCode = areaCode;
                    overDate = new Date();

                    // Display tooltip
                    const tooltip = this.getToolTipContent(areaCode);
                    if (tooltip) {
                        infoWindow.setContent(tooltip);
                        this.setInfoWindowPosition(event, infoWindow);
                        infoWindow.open(this.map);
                    }
                });

                google.maps.event.addListener(polygon, 'mousemove', (event) => {
                    infoWindow.close();
                });

                google.maps.event.addListener(polygon, 'mouseout', (event) => {
                    // Wait in case immediate mouseover event and this mouseover event was
                    // caused by mouse moving over the infowindow
                    setTimeout(function () {
                        const time = new Date().getTime();
                        if (time - overDate.getTime() > 25 && areaCode === overAreaCode) {
                            infoWindow.close();
                        }
                    }, 25);
                });

                google.maps.event.addListener(polygon, 'click', (event) => {
                    if (polygon.get('fillColor') === '#63A1C3') {
                        polygon.set('fillColor', '#7FFF92');
                        this.selectedAreaChanged.emit({ areaCode: areaCode, add: true });
                    } else {
                        polygon.set('fillColor', '#63A1C3');
                        this.selectedAreaChanged.emit({ areaCode: areaCode, add: false });
                    }
                });

                this.currentPolygons.push(polygon);
            }
        }
    }

    getToolTipContent(areaCode: string): string {
        let areaName;
        const areaCodeInAvailableArea = this.availableAreas.find(x => x.Code === areaCode);
        if (areaCodeInAvailableArea == null || areaCodeInAvailableArea === undefined) {
            areaName = '';
        } else {
            areaName = areaCodeInAvailableArea.Name;
        }

        return areaName;
    }

    setInfoWindowPosition(event, infoWindow) {
        const pos: google.maps.LatLng = event.latLng;
        infoWindow.setPosition(new google.maps.LatLng(pos.lat() + 0.02, pos.lng()));
    }

    colourFillPolygon(center: boolean): void {
        if (this.map) {
            const regionPolygons: Array<google.maps.Polygon> = [];
            const currentComparatorId = this.ftHelperService.getComparatorId();
            for (let i = 0; i < (this.currentPolygons.length); i++) {
                const polygon = this.currentPolygons[i];

                // Set polygon fill colour
                polygon.setMap(null);
                const areaCode = polygon.get('areaCode');
                const areaCodeInSelectedAreas = this.selectedAreas.find(x => x.Code === areaCode);
                if (areaCodeInSelectedAreas !== null && areaCodeInSelectedAreas !== undefined) {
                    polygon.set('fillColor', '#7FFF92');
                } else {
                    polygon.set('fillColor', '#63A1C3');
                }

                polygon.setMap(this.map);
            }
            if (center) {
                this.setMapCenter();
            }
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

    getPolygonCoordinates(coordinates): Array<Array<google.maps.LatLng>> {

        const coords: Array<Array<google.maps.LatLng>> = [];
        for (let i = 0; i < coordinates.length; i++) {
            for (let j = 0; j < coordinates[i].length; j++) {
                const path: Array<google.maps.LatLng> = [];
                for (let k = 0; k < coordinates[i][j].length; k++) {
                    const coord = new google.maps.LatLng(coordinates[i][j][k][1], coordinates[i][j][k][0]);
                    path.push(coord);
                }
                coords.push(path);
            }
        }
        return coords;
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
            google.maps.event.addListener(this.map, 'idle', event => {
                const bounds = this.map.getCenter();
                google.maps.event.trigger(this.map, 'resize');
                this.map.setCenter(bounds);
            });
        }
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
