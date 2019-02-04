import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import 'rxjs/rx';
import { Observable } from 'rxjs/Observable';
import { AreaList, AreaType, Area, AreaListAreaCode } from '../../../typings/FT.d';
import { FTHelperService } from '../../service/helper/ftHelper.service';
import { Parameters } from './parameters';
import { HttpService } from './http.service'

@Injectable()
export class AreaListService {

    private version: string = this.ftHelperService.version();

    constructor(private httpService: HttpService, private ftHelperService: FTHelperService) {
    }

    getAreaLists(userId: string): Observable<AreaList[]> {
        const params = new Parameters(this.version);
        params.addUserId(userId);
        params.addNoCache();

        return this.httpService.httpGet('api/arealists', params);
    }

    getAreaList(areaListId: number): Observable<AreaList> {
        const params = new Parameters(this.version);
        params.addAreaListId(areaListId);
        params.addNoCache();

        return this.httpService.httpGet('api/arealist', params);
    }

    getAreaListByPublicId(
        publicId: string,
        userId: string
    ): Observable<AreaList> {
        const params = new Parameters(this.version);
        params.addPublicId(publicId);
        params.addUserId(userId);
        params.addNoCache();

        return this.httpService.httpGet('api/arealist/by_public_id', params);
    }

    getAreaCodesFromAreaListId(areaListId: number): Observable<AreaListAreaCode[]> {
        const params = new Parameters(this.version);
        params.addAreaListId(areaListId);
        params.addNoCache();

        return this.httpService.httpGet('api/arealist/areacodes', params);
    }

    getAreasFromAreaListAreaCodes(areaCodes: string[]): Observable<Area[]> {
        const areaCodesCommaSeparated = areaCodes.join(',');

        const params = new Parameters(this.version);
        params.addAreaCodes(areaCodesCommaSeparated);

        return this.httpService.httpGet('api/areas/by_area_code', params);
    }

    getAreasWithAddressFromAreaListAreaCodes(areaCodes: string[]): Observable<Area[]> {
        const areaCodesCommaSeparated = areaCodes.join(',');

        const params = new Parameters(this.version);
        params.addAreaCodes(areaCodesCommaSeparated);

        return this.httpService.httpGet('api/areas_with_addresses/by_area_code', params);
    }

    saveAreaList(formData: FormData): Observable<Response> {
        return this.httpService.httpPost('api/arealist/save', formData);
    }

    updateAreaList(formData: FormData): Observable<Response> {
        return this.httpService.httpPost('api/arealist/update', formData);
    }

    deleteAreaList(formData: FormData): Observable<Response> {
        return this.httpService.httpPost('api/arealist/delete', formData);
    }

    copyAreaList(formData: FormData): Observable<Response> {
        return this.httpService.httpPost('api/arealist/copy', formData);
    }
}
