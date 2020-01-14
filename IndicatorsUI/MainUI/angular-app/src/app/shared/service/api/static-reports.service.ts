import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpService } from './http.service'
import { Parameters } from './parameters';
import { FTHelperService } from '../helper/ftHelper.service';
import { AreaType } from '../../../typings/FT';

@Injectable()
export class StaticReportsService {
  private version: string = this.ftHelperService.version();

  constructor(private httpService: HttpService, private ftHelperService: FTHelperService) {
  }

  public doesStaticReportExist(profileKey: string, fileName: string,
    timePeriod: string): Observable<boolean> {
    const params = new Parameters(this.version);
    params.addProfileKey(profileKey);
    params.addFileName(fileName);
    if (timePeriod) {
      params.addTimePeriod(timePeriod);
    }

    return this.httpService.httpGet('api/static-reports/exists', params);
  }
}
