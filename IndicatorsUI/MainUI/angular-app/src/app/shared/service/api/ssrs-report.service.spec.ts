import { TestBed, inject } from '@angular/core/testing';

import { SsrsReportService } from './ssrs-report.service';
import { HttpService } from './http.service';
import { FTHelperService } from '../helper/ftHelper.service';

describe('SsrsReportService', () => {

  let ftHelperService: any;
  let httpService: any;

  beforeEach(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['version']);
    httpService = jasmine.createSpyObj('HttpService', ['']);

    TestBed.configureTestingModule({
      providers: [SsrsReportService,
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: HttpService, useValue: httpService }]
    });


  });

  it('should be created', inject([SsrsReportService], (service: SsrsReportService) => {
    expect(service).toBeTruthy();
  }));
});
