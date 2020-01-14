import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpService } from './http.service';
import { Observable } from 'rxjs';
import { Parameters } from './parameters';
import { UploadJobProgress, UploadJobQueue, UploadJobSummary } from '../model/upload';

@Injectable()
export class UploadService {

    private baseUrl = 'api/upload';

    constructor(private httpService: HttpService, private http: HttpClient) { }

    getCurrentUserJobProgress(userId: number, numberOfRecords: number): Observable<UploadJobProgress> {
        const params = new Parameters();

        return this.httpService.httpGet(this.baseUrl + '/progress/' + userId + '/' + numberOfRecords, params, true);
    }

    getAllActiveJobProgress(): Observable<UploadJobQueue[]> {
        const params = new Parameters();

        return this.httpService.httpGet(this.baseUrl + '/all-active-job-progress', params, true);
    }

    getJobSummary(guid: string): Observable<UploadJobSummary> {
        const params = new Parameters();

        return this.httpService.httpGet(this.baseUrl + '/summary/' + guid, params, true);
    }

    changeJobStatus(guid: string, actionCode: number): Observable<any> {
        const params = new Parameters();

        return this.httpService.httpGet(this.baseUrl + '/change-status/' + guid + '/' + actionCode, params, true);
    }

    downloadFile(guid: string): Observable<any> {
        const params = new Parameters();
        params.addGuid(guid);

        return this.httpService.httpGet(this.baseUrl + '/download-file', params, true);
    }

    uploadFile(fileToUpload: File, action: string): Observable<any> {
        const formData = new FormData();
        formData.append('fileKey', fileToUpload, fileToUpload.name);
        formData.append('action', action);

        return this.httpService.httpPost(this.baseUrl + '/upload-file', formData);
    }
}