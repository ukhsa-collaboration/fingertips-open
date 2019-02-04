import { TestBed, inject } from '@angular/core/testing';
import { IndicatorService } from './indicator.service';
import { FTHelperService } from '../helper/ftHelper.service';
import { HttpService } from './http.service';
import { StaticReportsService } from './static-reports.service';

describe('StaticReportsService', () => {

  let ftHelperService: any;
  let httpService: any;

  beforeEach(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['version']);
    httpService = jasmine.createSpyObj('HttpService', ['httpGet']);

    TestBed.configureTestingModule({
      providers: [StaticReportsService,
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: HttpService, useValue: httpService }]
    });
  });

  it('should be created', inject([StaticReportsService], (service: StaticReportsService) => {
    expect(service).toBeTruthy();
  }));
});
