import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { FTHelperService } from '../../service/helper/ftHelper.service';
import { Parameters } from './parameters';
import { HttpService } from './http.service'
import { SSRSReport } from '../../../typings/FT.d'

@Injectable()
export class SsrsReportService {

  private version: string = this.ftHelperService.version();

  constructor(private httpService: HttpService, private ftHelperService: FTHelperService) {
  }

  getSsrsReports(profileId: number): Observable<SSRSReport[]> {
    let params = new Parameters(this.version);

    return this.httpService.httpGet('api/ssrs_reports/' + profileId, params);
  }
}
