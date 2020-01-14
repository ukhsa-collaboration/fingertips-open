import { Injectable } from "@angular/core";
import { HttpService } from './http.service';
import { Observable } from '../../../node_modules/rxjs';
import { Parameters } from './parameters';
import { User } from '../model/user';

@Injectable()
export class UsersService {
    private baseUrl = "api/users"
    constructor(private httpService: HttpService) { }

    getUsers(): Observable<User[]> {
        const params = new Parameters();

        return this.httpService.httpGet(this.baseUrl, params, true);
    }

    getUserById(userId: number): Observable<User> {
        const params = new Parameters();
        params.addUserId(userId);

        return this.httpService.httpGet(this.baseUrl + '/' + userId, params, true);
    }
}