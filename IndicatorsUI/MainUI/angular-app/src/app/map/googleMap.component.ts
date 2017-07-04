import { Component, OnInit, Input, Output, ElementRef, ViewChild, EventEmitter,OnChanges,SimpleChanges } from '@angular/core';
import 'rxjs/rx';
import {GoogleMapService} from './googleMap.service';
import {IndicatorService} from '../shared/service/api/indicator.service';
import {FTHelperService} from '../shared/service/helper/ftHelper.service';
import {BridgeDataHelperService} from '../shared/service/helper/bridgeDataHelper.service';
import {CoreDataHelperService} from '../shared/service/helper/coreDataHelper.service';
import {FTModel, FTRoot, Area, GroupRoot, CoreDataSet, CoreDataHelper, Unit, IndicatorMetadataHash, IndicatorMetadata} from '../typings/FT.d';
import * as _ from 'underscore';

@Component({
  selector: 'FT-google-map',
  template: `
            <div id="wrapper">
            <div [hidden]="isError" id="google-map" class="googleMapNg"></div>
            <div *ngIf="isError" id="polygonError" class="googleMapNg" > {{errorMessage}} </div>
            <div [hidden]="isError" id="floating-panel" class="layerControl info leaflet-control"><div>
                <a class="leaflet-control-layers-toggle" href="#" title="Layers"></a>
                <div id="mapOptions" style="display: none;"></div>
             </div>   `,
  styleUrls: ['./googleMap.component.css'] 
})
export class GoogleMapComponent implements OnInit, OnChanges {
   map: google.maps.Map;
  //@ViewChild('map') mapEl: ElementRef;
  @Output() mapInit = new EventEmitter();
  @Input() areaTypeId: number = null;
  path: string = this.ftHelperService.getURL().img;
  isError = false;
  errorMessage: string;
  currentPolygons: google.maps.Polygon[] = [];
  boundry: geoBoundry.Boundry;
  coreDataSet: CoreDataSet;
  sortedCoreData: Map<string, CoreDataSet>;

  constructor(private mapEl: ElementRef, private mapService: GoogleMapService,
            private indicatorService: IndicatorService , private ftHelperService: FTHelperService,
            private bridgeDataHelper:BridgeDataHelperService,
            private coreDataHelper: CoreDataHelperService) { }

  ngOnInit() {
    if (this.map) {
      // map is already initialized
      return;
    }
    this.loadMap();
  }
  ngOnChanges(changes: SimpleChanges) {
    if (changes['areaTypeId']) {
      this.loadPolygon(this.areaTypeId, this.path) ;
      }
  }
  loadMap() {
    /// Load from GoogleMapService and style it 
    let mapOptions: google.maps.MapOptions = {
        zoom: 6,
        mapTypeId:google.maps.MapTypeId.ROADMAP,
        panControl: false,
        zoomControl: true,
        zoomControlOptions: { position: google.maps.ControlPosition.TOP_LEFT},
        scaleControl: false,
        streetViewControl: false,
        mapTypeControl: false,
        scrollwheel: true
       };
    this.map = this.mapService.loadMap(this.mapEl, mapOptions);
    this.styleMap();

    let currentGrpRoot: GroupRoot =  this.bridgeDataHelper.getCurrentGroupRoot();
    let currentComparator: Area = this.bridgeDataHelper.getCurrentComparator();
    let ftModel: FTModel = this.ftHelperService.getFTModel();
    let comparatorId = this.bridgeDataHelper.getComparatorId();
    this.indicatorService.getSingleIndicatorForAllArea(currentGrpRoot.Grouping[0].GroupId, ftModel.areaTypeId, currentComparator.Code,
                         ftModel.profileId, comparatorId, currentGrpRoot.IID, currentGrpRoot.Sex.Id, currentGrpRoot.Age.Id)
                         .subscribe(
                                    data => {
                                             this.coreDataSet = <CoreDataSet>data;
                                             this.sortedCoreData = this.coreDataHelper.addOrderandPercentilesToData(this.coreDataSet);
                                             },
                                    error => { });
    /// Load polygon boundries from json files
    this.loadPolygon(this.areaTypeId, this.path);

    ///Get coredataset(data) from service
    this.mapInit.emit({map: this.map,
      });
  }
  styleMap(): void {
    if (this.map) {
      let bounds = new google.maps.LatLngBounds();
      let position = new google.maps.LatLng(53.415649, -2.209015);
      bounds.extend(position);

      const noTiles = 'noTiles';
       const visibilityOff: any = [{ visibility: 'off' }];
       const styleArray: any = [
           {
                stylers: [
                    { color: '#ffffff' }
                ]
            }, {
                featureType: 'road',
                elementType: 'geometry',
                stylers: visibilityOff
            }, {
                featureType: 'road',
                elementType: 'labels',
                stylers: visibilityOff
            }
        ];
       this.map.setOptions({styles: styleArray});
       this.map.mapTypes.set(noTiles, new noTileMapType());
       this.map.setCenter(bounds.getCenter());
    }
  }
  loadPolygon(areaTypeId: number, path: string) {
    if (this.map) {
    this.mapService.loadBoundries(areaTypeId, path)
                  .subscribe(
                  data => {
                          this.boundry =  <geoBoundry.Boundry>data;
                          this.isError = false;
                          this.removePolygon();
                          this.fillPolygon(this.boundry);
                  },
                  error => {
                            this.isError = true;
                            this.errorMessage = <any>error;
                           });
    }
  }
  removePolygon(): void {
    if ( this.currentPolygons !== undefined ) {
      this.currentPolygons.forEach(element => {
      element.setMap(null);
    });
      this.currentPolygons.length = 0;
    }
}
fillPolygon(boundry : geoBoundry.Boundry): any {
  let polygon: google.maps.Polygon = null;
    if (boundry.features) {
        for (let x = 0; x < boundry.features.length; x++) {

            let areaCode = boundry.features[x].properties.AreaCode;
            let coordinates = boundry.features[x].geometry.coordinates;
            let coords: Array<Array<google.maps.LatLng>> = [];

            for (let i = 0; i < coordinates.length; i++) {
                for (let j = 0; j < coordinates[i].length; j++) {
                    let path : Array<google.maps.LatLng>= [];
                    for (let k = 0; k < coordinates[i][j].length; k++) {
                        let coord = new google.maps.LatLng(coordinates[i][j][k][1], coordinates[i][j][k][0]);
                        path.push(coord);
                    }
                    coords.push(path);
                }
            }
            let infoWindow: google.maps.InfoWindow = null;
            let polygon = new google.maps.Polygon({
                paths: coords,
                // strokeColor: '#333',
                // strokeOpacity: 1,
                // strokeWeight: 1,
                // fillOpacity: 1
                  strokeColor: '#333333',
                  strokeOpacity: 0.8,
                  strokeWeight: 1,
                  fillColor: '#B0B0B2',
                  fillOpacity: 0.35
                });
            polygon.set('areaCode', boundry.features[x].properties.AreaCode);
            polygon.setMap(this.map);
            google.maps.event.addListener(polygon, 'mouseover', (event) => {
                    let calloutBoxPixelOffset = new google.maps.Size(-125, -18);
                    polygon.set('strokeWeight', 3);
                    polygon.set('strokeColor', '#9D78D2');
                    infoWindow = new google.maps.InfoWindow();
                    let currentGrpRoot: GroupRoot =  this.bridgeDataHelper.getCurrentGroupRoot();
                    let data: IndicatorMetadata = this.ftHelperService.getMetadata(currentGrpRoot.IID);
                    let unit = data.Unit;
                    let areaName = this.ftHelperService.getAreaName(areaCode);
                    let value = this.sortedCoreData !== null ? '<br>' + this.coreDataHelper.valueWithUnit(unit).
                                                                        getFullLabel(this.sortedCoreData[areaCode].ValF)
                                                                        : '<br>-';
                    let toolTipcontent = '<b>' + areaName + '</b>' + value;
                    infoWindow.setContent(toolTipcontent);
                    infoWindow.open(this.map);
                    $('.gm-style-iw').next('div').remove();
                    infoWindow.setPosition(this.setPolygonCentre(areaCode));
            });
             google.maps.event.addListener(polygon, 'mouseout', function () {
              polygon.set('strokeWeight', 1);
              polygon.set('strokeColor', '#333333');
              infoWindow.close();
              });
              this.currentPolygons.push(polygon);
          }
      }
  }
  setPolygonCentre(selectedAreaCode): google.maps.LatLng {
    const boundry = this.boundry;
    let latCentreSum = 0;
    let lngCentreSum = 0;

    for (let x = 0; x < boundry.features.length; x++) {
        const areaCode = boundry.features[x].properties.AreaCode;

        if (areaCode === selectedAreaCode) {
            const coordinates = boundry.features[x].geometry.coordinates;
            const latLng: any = [];

            // TODO: may need to improve the performance of this
            for (let i = 0; i < coordinates.length; i++) {
                for (let j = 0; j < coordinates[i].length; j++) {
                    const coordinate = coordinates[i][j];
                    for (let k = 0; k < coordinate.length; k++) {
                        latLng.push(coordinate[k][1], coordinate[k][0]);
                    }
                }
            }

            for (let i = 0; i < latLng.length; i++) {
                latCentreSum += parseFloat(latLng[i]);
                lngCentreSum += parseFloat(latLng[++i]);
            }

            const boundaryPoints = latLng.length / 2;
            const currentLat = latCentreSum / boundaryPoints;
            const currentLng = lngCentreSum / boundaryPoints;
            return new google.maps.LatLng(currentLat, currentLng);
        }
    }
}
}
export class noTileMapType implements google.maps.MapType {
    tileSize = new google.maps.Size(1024, 1024);
    maxZoom  = 20;
    getTile(tileCoord: google.maps.Point, zoom: number, ownerDocument: Document): Element {
        return ownerDocument.createElement('div');
    }
     releaseTile(tile: Element): void {
            throw new Error('Method not implemented.');
        }
    }
