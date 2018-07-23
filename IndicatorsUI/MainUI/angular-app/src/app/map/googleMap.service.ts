import { Injectable, ElementRef, EventEmitter } from '@angular/core';
import {Http, Response} from '@angular/http';
import 'rxjs/rx';
import {Observable} from 'rxjs/Observable';
import {FTModel, FTRoot} from '../typings/FT.d';
import {FTHelperService} from '../shared/service/helper/ftHelper.service';


@Injectable()
export class GoogleMapService {
  isLoaded = new EventEmitter();
  map: google.maps.Map;

  constructor(private http: Http, private ftHelperservice: FTHelperService){ }

  loadMap(mapDiv: Element, mapOptions: google.maps.MapOptions): google.maps.Map {
      this.map = null;
      if(mapDiv != null){
        this.map = new google.maps.Map(mapDiv, mapOptions);
      }
      return this.map;
  }
  loadBoundries(areaTypeId: number, path: string): any {
       const baseUrl: string = path + 'maps/' + areaTypeId + '/geojson/boundaries.js';
       return this.http.get( baseUrl).map(res => res.json()).catch(this.handleError);
  }
  private handleError(error: any) {
        const errorMessage: string = 'Unsupported map type. Maps are not available for this area type.';
        return Observable.throw(errorMessage);
  }
}
