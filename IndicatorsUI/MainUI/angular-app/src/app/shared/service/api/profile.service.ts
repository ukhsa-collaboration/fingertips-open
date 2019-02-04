import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpService } from './http.service'
import { Parameters } from './parameters';
import { FTHelperService } from '../../service/helper/ftHelper.service';
import { AreaType, ProfilePerIndicator } from '../../../typings/FT.d';

@Injectable()
export class ProfileService {

  private version: string = this.ftHelperService.version();

  constructor(private httpService: HttpService, private ftHelperService: FTHelperService) {
  }

  public areaTypesWithPdfs(profileId: number): Observable<AreaType[]> {
    let params = new Parameters(this.version);
    params.addProfileId(profileId);

    return this.httpService.httpGet('api/profile/area_types_with_pdfs', params);
  }

  public getIndicatorProfiles(indicatorIds: number[]): Observable<Map<number, ProfilePerIndicator[]>> {
    const params = new Parameters(this.version);
    params.addIndicatorIds(indicatorIds);

    return this.httpService.httpGet('api/profiles_containing_indicators', params);
  }
}
