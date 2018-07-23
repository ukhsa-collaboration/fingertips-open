import { Injectable } from '@angular/core';
import {Http, Response} from '@angular/http';
import {Observable} from 'rxjs/Rx';
import {Profile} from '../model/profile';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';


@Injectable()
export class ProfileService {

  constructor(private http:Http) { }

  getProfiles(): Observable<Profile[]>{
    return this.http.get('profile/user-profiles')
      .map((res:Response) => res.json());
  }
}
