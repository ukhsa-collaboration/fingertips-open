import { Injectable } from "@angular/core";
import { HttpRequest, HttpResponse } from "@angular/common/http";
import { isDefined } from "@angular/compiler/src/util";

@Injectable()
export class HttpCacheService {
    private cacheMap = new Map<string, any>();

    public getFromCache(httpRequest: HttpRequest<any>): HttpResponse<any> | undefined {
        const url = httpRequest.urlWithParams;
        const cached = this.cacheMap.get(url);

        if (!cached) {
            return undefined;
        }

        if (httpRequest.headers.get("reset-cache")) {
            if (cached) {
                this.cacheMap.delete(url);
            }

            return undefined;
        }

        return cached;
    }

    public addToCache(httpRequest: HttpRequest<any>, httpResponse: HttpResponse<any>): void {
        if (!isDefined(this.getFromCache(httpRequest))) {
            this.cacheMap.set(httpRequest.urlWithParams, httpResponse);
        }
    }
}
