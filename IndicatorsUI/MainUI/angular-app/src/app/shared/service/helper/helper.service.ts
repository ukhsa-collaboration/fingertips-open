import { Injectable } from '@angular/core';
import {Http, RequestOptions, Headers} from '@angular/http';


@Injectable()
export class HelperService {
    createJsonRequestHeader(): RequestOptions {
        return new RequestOptions({headers: new Headers({'Content-Type': 'application/json'})});
    }
    
}
