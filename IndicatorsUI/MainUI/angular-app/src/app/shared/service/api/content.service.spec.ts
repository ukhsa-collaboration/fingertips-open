import { TestBed, inject } from '@angular/core/testing';

import { ContentService } from './content.service';
import { HttpService } from './http.service';
import { FTHelperService } from '../helper/ftHelper.service';

describe('ContentService', () => {

  let ftHelperService: any;
  let httpService: any;

  beforeEach(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['version']);
    httpService = jasmine.createSpyObj('HttpService', ['']);

    TestBed.configureTestingModule({
      providers: [ContentService,
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: HttpService, useValue: httpService }]
    });
  });

  it('should be created', inject([ContentService], (service: ContentService) => {
    expect(service).toBeTruthy();
  }));
});
