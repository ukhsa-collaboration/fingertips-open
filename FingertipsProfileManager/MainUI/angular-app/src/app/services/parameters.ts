import { RequestOptions, URLSearchParams, Headers } from '@angular/http';
export class Parameters {

    private params: URLSearchParams = new URLSearchParams();

    constructor() { }

    getRequestOptions(): RequestOptions {
        const requestOptions: RequestOptions = new RequestOptions({
            headers: new Headers({ 'Content-Type': 'application/json' })
        });
        requestOptions.search = this.params;
        return requestOptions;
    }

    /** Returns a concaternated string of the URL parameters */
    getParameterString() {
        return this.params.toString();
    }

    addProfileUrlKey(profileUrlKey: string): void {
        this.params.set('profileUrlKey', profileUrlKey);
    }

    addSequenceNumber(sequenceNumber: number): void {
        this.params.set('sequenceNumber', sequenceNumber.toString());
    }

    addAreaTypeId(areaTypeId: number): void {
        this.params.set('areaTypeId', areaTypeId.toString());
    }

    addGroupId(groupId: number): void {
        this.params.set('groupId', groupId.toString());
    }

    addProfileId(profileId: number): void {
        this.params.set('profileId', profileId.toString());
    }

    addDomainSequence(sequenceNumber: number): void {
        this.params.set('domainSequence', sequenceNumber.toString());
    }

    addReportId(reportId: number): void {
        this.params.set('id', reportId.toString());
    }
}
