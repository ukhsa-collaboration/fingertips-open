import { TestBed, inject } from '@angular/core/testing';

import { ProfileService } from './profile.service';
import { HttpService } from './http.service';
import { FTHelperService } from '../helper/ftHelper.service'

describe('ProfileService', () => {

  let httpService: any;
  let ftHelperService: any;

  beforeEach(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['version']);
    httpService = jasmine.createSpyObj('HttpService', ['']);

    TestBed.configureTestingModule({
      providers: [ProfileService,
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: HttpService, useValue: httpService }
      ],
    });
  });

  it('should be created', inject([ProfileService], (service: ProfileService) => {
    expect(service).toBeTruthy();
  }));
});
