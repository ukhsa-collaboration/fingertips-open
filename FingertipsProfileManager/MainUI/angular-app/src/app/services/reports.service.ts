import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Rx';
import { Report } from '../model/report';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable()
export class ReportsService {
  private baseUrl: string = 'api/reports';
  constructor(private http: Http) { }

  getReports(): Observable<Report[]> {
    return this.http.get(this.baseUrl)
      .map((res: Response) => res.json());
  }

  getReportById(id): Observable<Report> {
    return this.http
      .get(this.baseUrl + '/' + id)
      .map((response: Response) => <Report>response.json());
  }

  public deleteReportById(id: number): Observable <any> {    
  return this.http
    .delete(this.baseUrl + '/' + id);

}

  public saveReport(report:Report): Observable <any> {
  let headers = new Headers({ 'Content-Type': 'application/json;charset=UTF-8' });
  let options = new RequestOptions({ headers: headers });
  return this.http.post(this.baseUrl + '/new', JSON.stringify(report), options);      
}

  public updateReport(report:Report): Observable < any > {
  let headers = new Headers({ 'Content-Type': 'application/json;charset=UTF-8' });
  let options = new RequestOptions({ headers: headers });
  return this.http.put(this.baseUrl + '/new', JSON.stringify(report), options);    
}

  private handleError(error: Response | any){
  console.log('api error', error);
  return Observable.throw(error);
}
}
