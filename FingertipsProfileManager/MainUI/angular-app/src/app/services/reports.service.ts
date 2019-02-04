import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Rx';
import { Report } from '../model/report';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import { Parameters } from './parameters';
import { HttpService } from './http.service';

@Injectable()
export class ReportsService {
  private baseUrl: string = 'api/reports';
  constructor(private httpService: HttpService, private http: Http) { }

  getReports(): Observable<Report[]> {
    const params = new Parameters();
    return this.httpService.httpGet(this.baseUrl, params, false);
  }

  getReportById(id): Observable<Report> {
    const params = new Parameters();
    params.addReportId(id);
    return this.httpService.httpGet(this.baseUrl + '/' + id, params, false);
  }

  public deleteReportById(id: number): Observable<any> {
    return this.http
      .delete(this.baseUrl + '/' + id);
  }

  public saveReport(report: Report): Observable<any> {
    let headers = new Headers({ 'Content-Type': 'application/json;charset=UTF-8' });
    let options = new RequestOptions({ headers: headers });
    return this.http.post(this.baseUrl + '/new', JSON.stringify(report), options);
  }

  public updateReport(report: Report): Observable<any> {
    let headers = new Headers({ 'Content-Type': 'application/json;charset=UTF-8' });
    let options = new RequestOptions({ headers: headers });
    return this.http.put(this.baseUrl + '/new', JSON.stringify(report), options);
  }

  private handleError(error: Response | any) {
    console.log('api error', error);
    return Observable.throw(error);
  }
}
