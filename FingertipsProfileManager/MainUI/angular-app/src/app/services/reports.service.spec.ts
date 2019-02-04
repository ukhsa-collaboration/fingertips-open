import { TestBed, inject, async } from '@angular/core/testing';
import { ReportsService } from './reports.service';
import { HttpService } from './http.service';

describe('ReportsService', () => {

  let reportsService: any;
  let httpService: any;

  beforeEach(() => {

    reportsService = jasmine.createSpyObj('ReportsService', ['getReports']);
    httpService = jasmine.createSpyObj('HttpService', ['httpGet', 'httpPost']);

    TestBed.configureTestingModule({
      providers: [
        ReportsService,
        { provide: ReportsService, useValue: reportsService },
        { provide: HttpService, useValue: httpService }
      ]
    });
  });

  it('should ...',
    async(inject([ReportsService], (reportsService: ReportsService) => {
      expect(reportsService).toBeTruthy();
    })));
});
