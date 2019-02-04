import { Observable } from 'rxjs/Observable';
import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/publishReplay';
import 'rxjs/add/operator/catch';
import { Parameters } from './parameters';

@Injectable()
export class HttpService {

    private observables: any = {};

    private baseUrl: string = '';

    constructor(private http: Http) { }

    public httpGet(serviceUrl: string, params: Parameters, cachedResult: boolean): Observable<any> {

        // Check whether call has already been made and cached observable is available
        const parameterString = params.getParameterString();
        const serviceKey = serviceUrl + parameterString;
        if (cachedResult && this.observables[serviceKey]) {
            return this.observables[serviceKey];
        }

        const observable = this.http.get(this.baseUrl + serviceUrl, params.getRequestOptions())
            .publishReplay(1)
            .refCount() // Call once then use same response for repeats
            .map(res => res.json())
            .catch(this.handleError);

        this.observables[serviceKey] = observable;
        return observable;
    }

    public httpPost(serviceUrl: string, formData: FormData): Observable<Response> {
        return this.http.post(this.baseUrl + serviceUrl, formData)
            .map((response: Response) => {
                return response;
            }).catch(this.handleError);
    }

    private handleError(error: any) {
        console.error(error);
        const errorMessage = 'AJAX call failed';
        return Observable.throw(errorMessage);
    }
}
