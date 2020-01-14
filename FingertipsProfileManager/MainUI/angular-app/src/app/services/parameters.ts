import { HttpParams, HttpHeaders } from '@angular/common/http';

export class Parameters {

    private params: HttpParams = new HttpParams();

    constructor() { }

    getRequestOptions(resetCache: boolean): any {
        let headers = this.getHeaders();
        const params = this.params;

        if (resetCache) {
            headers = headers.set('reset-cache', 'true');
        }

        const options = ({
            headers: headers,
            params: params
        });

        return options;
    }

    getHeaders(): HttpHeaders {
        return new HttpHeaders({ 'Content-Type': 'application/json;charset=UTF-8' });
    }

    /** Returns a concatenated string of the URL parameters */
    getParameterString(): string {
        return this.params.toString();
    }

    addProfileUrlKey(profileUrlKey: string): void {
        this.params = this.params.set('profileUrlKey', profileUrlKey);
    }

    addSequenceNumber(sequenceNumber: number): void {
        this.params = this.params.set('sequenceNumber', sequenceNumber.toString());
    }

    addAreaTypeId(areaTypeId: number): void {
        this.params = this.params.set('areaTypeId', areaTypeId.toString());
    }

    addGroupId(groupId: number): void {
        this.params = this.params.set('groupId', groupId.toString());
    }

    addProfileId(profileId: number): void {
        this.params = this.params.set('profileId', profileId.toString());
    }

    addDomainSequence(sequenceNumber: number): void {
        this.params = this.params.set('domainSequence', sequenceNumber.toString());
    }

    addReportId(reportId: number): void {
        this.params = this.params.set('id', reportId.toString());
    }

    addNoCache(): void {
        this.params = this.params.set('no_cache', true.toString());
    }

    addDateTime(): void {
        this.params = this.params.set('datetime', (new Date()).getTime().toString());
    }

    addUserId(userId: number): void {
        this.params = this.params.set('userId', userId.toString());
    }

    addNumberOfRecords(numberOfRecords: number): void {
        this.params = this.params.set('numberOfRecords', numberOfRecords.toString());
    }

    addGuid(guid: string): void {
        this.params = this.params.set('guid', guid);
    }

    addActionCode(actionCode: number): void {
        this.params = this.params.set('actionCode', actionCode.toString());
    }
}