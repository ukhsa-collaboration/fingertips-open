import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { FTHelperService } from '../helper/ftHelper.service';
import { Parameters } from './parameters';
import { HttpService } from './http.service'
import { SSRSReport } from '../../../typings/FT'

@Injectable()
export class SsrsReportService {

  private version: string = this.ftHelperService.version();

  constructor(private httpService: HttpService, private ftHelperService: FTHelperService) {
  }

  getSsrsReports(profileId: number): Observable<SSRSReport[]> {
    const params = new Parameters(this.version);

    return this.httpService.httpGet('api/ssrs_reports/' + profileId, params);
  }
}
