import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { FTHelperService } from '../helper/ftHelper.service';
import { Parameters } from './parameters';
import { HttpService } from './http.service'

@Injectable()
export class ContentService {

  private version: string = this.ftHelperService.version();

  constructor(private httpService: HttpService, private ftHelperService: FTHelperService) {
  }

  getContent(profileId: number, key: string): Observable<string> {
    const params = new Parameters(this.version);
    params.addProfileId(profileId);
    params.addKey(key);

    return this.httpService.httpGet('api/content', params);
  }
}
