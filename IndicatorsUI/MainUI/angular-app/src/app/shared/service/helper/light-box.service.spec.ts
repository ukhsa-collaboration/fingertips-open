import { TestBed, inject } from '@angular/core/testing';
import { FTHelperService } from './ftHelper.service';
import { LightBoxService } from './light-box.service';

describe('LightBoxService', () => {

  let ftHelperService: any;

  beforeEach(() => {
    ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);

    TestBed.configureTestingModule({
      providers: [LightBoxService,
        { provide: FTHelperService, useValue: ftHelperService },]
    });
  });

  it('should be created', inject([LightBoxService], (service: LightBoxService) => {
    expect(service).toBeTruthy();
  }));
});
