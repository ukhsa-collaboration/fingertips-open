import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Report } from '../model/report';
import { HttpService } from './http.service';
import { Parameters } from './parameters';


@Injectable()
export class ReportsService {
  private baseUrl = 'api/reports';
  constructor(private httpService: HttpService, private http: HttpClient) { }

  getReports(): Observable<Report[]> {
    const params = new Parameters();

    return this.httpService.httpGet(this.baseUrl, params, true);
  }

  getReportById(id): Observable<Report> {
    const params = new Parameters();
    params.addReportId(id);

    return this.httpService.httpGet(this.baseUrl + '/' + id, params, true);
  }

  public deleteReportById(id: number): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json;charset=UTF-8' });
    const options = ({ headers: headers });

    return this.http.delete(this.baseUrl + '/' + id, options);
  }

  public saveReport(report: Report): Observable<Report> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json;charset=UTF-8' });
    const options = ({ headers: headers });

    return this.http.post<Report>(this.baseUrl + '/new', JSON.stringify(report), options)
      .pipe(catchError(this.handleError));
  }

  public updateReport(report: Report): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json;charset=UTF-8' });
    const options = ({ headers: headers });

    return this.http.put(this.baseUrl + '/new', JSON.stringify(report), options)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: Response | any) {
    console.log(error);
    return Observable.throw(error);
  }
}
