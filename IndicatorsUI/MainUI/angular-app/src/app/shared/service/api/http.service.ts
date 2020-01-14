import { Injectable } from '@angular/core';
import {
    HttpInterceptor, HttpRequest, HttpHandler, HttpResponse,
    HttpEvent, HttpErrorResponse, HttpClient
} from '@angular/common/http';
import { isDefined } from '@angular/compiler/src/util';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/share';
import { FTHelperService } from '../helper/ftHelper.service';
import { Parameters } from './parameters';
import { HttpCacheService } from './http-cache.service';

@Injectable()
export class HttpService implements HttpInterceptor {

    private baseUrl: string;

    constructor(private httpClient: HttpClient, private httpCacheService: HttpCacheService, private ftHelperService: FTHelperService) {
        // Initialise the base url
        this.baseUrl = ftHelperService.getURL().bridge;
    }

    public intercept(httpRequest: HttpRequest<any>, handler: HttpHandler): Observable<HttpEvent<any>> {
        // Only consider GET requests for dealing with cache
        if (httpRequest.method !== "GET") {
            return handler.handle(httpRequest);
        }

        // Return cached response if present
        const lastResponse = this.httpCacheService.getFromCache(httpRequest);
        if (isDefined(lastResponse)) {
            return (lastResponse instanceof Observable) ? lastResponse : Observable.of(lastResponse.clone());
        }

        // Perform an api call and add the respone to cache
        const observable = handler.handle(httpRequest).do((stateEvent) => {
            if (stateEvent instanceof HttpResponse) {
                this.httpCacheService.addToCache(httpRequest, stateEvent.clone());
            }
        }).share();

        // Return the response
        return observable;
    }

    public httpGet(serviceUrl: string, params: Parameters, resetCache: boolean = false): Observable<any> {
        const observable = this.httpClient.get(this.baseUrl + serviceUrl, params.getRequestOptions(resetCache))
            .pipe(catchError(error => { return this.handleError(error) }));
        return observable;
    }

    public httpPost(serviceUrl: string, formData: FormData): Observable<Response> {
        return this.httpClient.post<Response>(this.baseUrl + serviceUrl, formData)
            .pipe(catchError(this.handleError));
    }

    private handleError(errorResponse: HttpErrorResponse) {
        return throwError(errorResponse.message);
    }
}
