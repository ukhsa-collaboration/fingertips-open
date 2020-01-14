import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { catchError } from 'rxjs/operators';


@Injectable()
export class GoogleMapService {
  isLoaded = new EventEmitter();
  map: google.maps.Map;

  constructor(private http: HttpClient, private ftHelperservice: FTHelperService) { }

  loadMap(mapDiv: Element, mapOptions: google.maps.MapOptions): google.maps.Map {
    this.map = null;
    if (mapDiv != null) {
      this.map = new google.maps.Map(mapDiv, mapOptions);
    }
    return this.map;
  }

  loadBoundries(areaTypeId: number, path: string): Observable<any> {
    const baseUrl: string = path + 'maps/' + areaTypeId + '/geojson/boundaries.js';
    return this.http.get(baseUrl).pipe(catchError(error => { return this.handleError(error) }));
  }

  private handleError(errorResponse: HttpErrorResponse) {
    console.error(errorResponse);
    return throwError(errorResponse.error.message);
  }
}
