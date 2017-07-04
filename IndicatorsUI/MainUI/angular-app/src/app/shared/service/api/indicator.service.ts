import { Injectable } from '@angular/core';
import {Http, Response, RequestOptionsArgs, URLSearchParams} from '@angular/http';
import 'rxjs/rx';
import {Observable} from 'rxjs/Observable';
import {HelperService} from '../helper/helper.service';
import {FTModel, FTRoot, CoreDataSet} from '../../../typings/FT.d';
import {FTHelperService} from '../../service/helper/ftHelper.service';

//import * as FT from '../../typings/FT.d';
@Injectable()
export class IndicatorService {
    baseUrl: string = this.ftHelperService.getURL().bridge;
    constructor(private http: Http, private helperService: HelperService,private ftHelperService: FTHelperService) { }

    getSingleIndicatorForAllArea(groupId: number, areaTypeId: number, parentAreaCode: string, prodileId: number, comparatorId: number,
                                 indicatorId: number, sexId: number, ageId: number ) : Observable<CoreDataSet>{
        let params: URLSearchParams = new URLSearchParams();
        params.set('group_id', groupId.toString());
        params.set('area_type_id', areaTypeId.toString());
        params.set('parent_area_code', parentAreaCode);
        params.set('profile_id', prodileId.toString());
        params.set('comparator_id', comparatorId.toString());
        params.set('indicator_id', indicatorId.toString());
        params.set('sex_id', sexId.toString());
        params.set('age_id', ageId.toString());

        let requestOptions: RequestOptionsArgs = this.helperService.createJsonRequestHeader();
        requestOptions.search = params;

        const serviceURL: string = this.baseUrl + 'api/latest_data/single_indicator_for_all_areas';
        return this.http.get(serviceURL, requestOptions).map(res => res.json())
                                                        .catch(this.handleError);
    }
    private handleError(error: any) {
        console.error(error);
        const errorMessage: string = 'Unsupported map type.Maps are not available for ' + this.ftHelperService.getAreaTypeName();
        return Observable.throw(errorMessage);
  }
}

