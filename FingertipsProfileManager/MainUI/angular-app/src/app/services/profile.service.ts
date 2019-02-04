import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Observable } from 'rxjs/Rx';
import { Profile, GroupingPlusName } from '../model/profile';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import { Parameters } from './parameters';
import { HttpService } from './http.service';

@Injectable()
export class ProfileService {

  constructor(private httpService: HttpService) { }

  getAllProfiles(): Observable<Profile[]> {
    const params = new Parameters();
    return this.httpService.httpGet('profile/all-profiles', params, true);
  }

  getUserProfiles(): Observable<Profile[]> {
    const params = new Parameters();
    return this.httpService.httpGet('profile/user-profiles', params, true);
  }

  getGroupingPlusNames(profileUrlKey: string, sequenceNumber: number, areaTypeId: number): Observable<GroupingPlusName[]> {
    const params = new Parameters();
    params.addProfileUrlKey(profileUrlKey);
    params.addSequenceNumber(sequenceNumber);
    params.addAreaTypeId(areaTypeId);

    return this.httpService.httpGet('ProfileGroupingIndicators', params, false);
  }

  getGroupingSubheadings(areaTypeId: number, groupId: number) {
    const params = new Parameters();
    params.addAreaTypeId(areaTypeId);
    params.addGroupId(groupId);

    return this.httpService.httpGet('grouping-subheadings/by-area-type-and-group', params, false);
  }

  getGroupingSubheadingsForProfile(profileId: number) {
    const params = new Parameters();
    params.addProfileId(profileId);

    return this.httpService.httpGet('grouping-subheadings/by-profile', params, false);
  }

  getAllAreaTypes() {
    const params = new Parameters();

    return this.httpService.httpGet('areas/all-area-types', params, true);
  }

  getDomainName(groupId: number, domainSequence: number) {
    const params = new Parameters();
    params.addGroupId(groupId);
    params.addDomainSequence(domainSequence);

    return this.httpService.httpGet('DomainName', params, true);
  }

  saveReorderedIndicators(formData: FormData): Observable<Response> {
    return this.httpService.httpPost('ProfileGroupingIndicators/SaveReorderedIndicators', formData);
  }
}
