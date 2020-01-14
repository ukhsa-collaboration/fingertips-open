import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Profile, GroupingPlusName } from '../model/profile';
import { HttpService } from './http.service';
import { Parameters } from './parameters';

@Injectable()
export class ProfileService {

  constructor(private httpService: HttpService) { }

  getAllProfiles(): Observable<Profile[]> {
    const params = new Parameters();

    return this.httpService.httpGet('profiles/all-profiles', params, true);
  }

  getUserProfiles(): Observable<Profile[]> {
    const params = new Parameters();

    return this.httpService.httpGet('profiles/user-profiles', params, true);
  }

  getGroupingPlusNames(profileUrlKey: string, sequenceNumber: number, areaTypeId: number): Observable<GroupingPlusName[]> {
    const params = new Parameters();
    params.addProfileUrlKey(profileUrlKey);
    params.addSequenceNumber(sequenceNumber);
    params.addAreaTypeId(areaTypeId);
    params.addDateTime();

    return this.httpService.httpGet('/profiles-and-indicators/profile-grouping-indicators', params, false);
  }

  getGroupingSubheadings(areaTypeId: number, groupId: number) {
    const params = new Parameters();
    params.addAreaTypeId(areaTypeId);
    params.addGroupId(groupId);
    params.addDateTime();

    return this.httpService.httpGet('/grouping-subheadings/by-area-type-and-group', params, false);
  }

  getGroupingSubheadingsForProfile(profileId: number) {
    const params = new Parameters();
    params.addProfileId(profileId);
    params.addDateTime();

    return this.httpService.httpGet('/grouping-subheadings/by-profile', params, false);
  }

  getAllAreaTypes() {
    const params = new Parameters();

    return this.httpService.httpGet('/areas/all-area-types', params, true);
  }

  getDomainName(groupId: number, domainSequence: number) {
    const params = new Parameters();
    params.addGroupId(groupId);
    params.addDomainSequence(domainSequence);

    return this.httpService.httpGet('/profiles-and-indicators/domain-name', params, true);
  }

  saveReorderedIndicators(formData: FormData): Observable<any> {
    return this.httpService.httpPost('/profiles-and-indicators/save-reordered-indicators', formData);
  }
}
