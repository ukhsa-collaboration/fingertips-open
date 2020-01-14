import { TestBed, inject, async } from '@angular/core/testing';
import { ProfileService } from './profile.service';
import { HttpService } from './http.service';

describe('ProfileService', () => {

  let profileService: any;
  let httpService: any;

  beforeEach(() => {

    profileService = jasmine.createSpyObj('ProfileService', ['getProfiles']);
    httpService = jasmine.createSpyObj('HttpService', ['httpGet', 'httpPost']);

    TestBed.configureTestingModule({
      providers: [
        ProfileService,
        { provide: ProfileService, useValue: profileService },
        { provide: HttpService, useValue: httpService }
      ]
    });
  });

  it('should ...',
    async(inject([ProfileService], (service: ProfileService) => {
      expect(service).toBeTruthy();
    })));
});
