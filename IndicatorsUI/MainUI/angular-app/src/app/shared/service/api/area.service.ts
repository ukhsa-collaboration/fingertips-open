import { Injectable } from "@angular/core";
import {
  Http,
  Response,
  RequestOptions,
  URLSearchParams,
  Headers
} from "@angular/http";
import "rxjs/rx";
import { Observable } from "rxjs/Observable";
import {
  FTRoot,
  AreaTextSearchResult,
  NearByAreas,
  AreaAddress,
  ParentAreaType
} from "../../../typings/FT.d";
import { FTHelperService } from "../../service/helper/ftHelper.service";
import { Parameters } from "./parameters";
@Injectable()
export class AreaService {

  /** Observables for calls that have previously been made */
  private observables: any = {};

  private baseUrl: string = this.ftHelperService.getURL().bridge;
  private version: string = this.ftHelperService.version();

  constructor(private http: Http, private ftHelperService: FTHelperService) { }

  getAreaSearchByText(
    text: string,
    areaTypeId: number,
    shouldSearchRetreiveCoordinates: boolean,
    parentAreasToIncludeInResults: boolean
  ): Observable<AreaTextSearchResult> {
    let params = new Parameters(this.version);
    params.addPolygonAreaTypeId(areaTypeId);
    params.addNoCache(true);
    params.addIncludeCoordinates(shouldSearchRetreiveCoordinates);
    params.addParentAreasToIncludeInResults(parentAreasToIncludeInResults);
    params.addSearchText(text);

    return this.getObservable("api/area_search_by_text", params);
  }

  getAreaSearchByProximity(
    easting: number,
    northing: number,
    areaTypeId: number
  ): Observable<NearByAreas> {
    let params = new Parameters(this.version);
    params.addAreaTypeId(areaTypeId);
    params.addEasting(easting);
    params.addNorhing(northing);

    return this.getObservable("api/area_search_by_proximity", params);
  }

  getAreaAddressesByParentAreaCode(
    parentAreaCode: string,
    areaTypeId: number
  ): Observable<AreaAddress> {
    let params = new Parameters(this.version);
    params.addAreaTypeId(areaTypeId);
    params.addParentAreaCode(parentAreaCode);

    return this.getObservable("api/area_addresses/by_parent_area_code", params);
  }

  getParentAreas(profileId: number): Observable<ParentAreaType[]> {
    let params = new Parameters(this.version);
    params.addProfileId(profileId);

    return this.getObservable("api/area_types/parent_area_types", params);
  }

  private getObservable(serviceUrl: string, params?: Parameters): Observable<any> {

    // Ensure paramaters is defined
    if (!params) {
      params = new Parameters(this.version);
    }

    // Check whether call has already been made and cached observable is available
    let parameterString = params.getParameterString();
    let serviceKey = serviceUrl + parameterString;
    if (this.observables[serviceKey]) {
      return this.observables[serviceKey];
    }

    let observable = this.http.get(this.baseUrl + serviceUrl, params.getRequestOptions())
      .publishReplay(1).refCount() // Call once then use same response for repeats
      .map(res => res.json())
      .catch(this.handleError);

    this.observables[serviceKey] = observable;
    return observable;
  }

  private handleError(error: any) {
    console.error(error);
    const errorMessage = "AJAX call failed";
    return Observable.throw(errorMessage);
  }

}
