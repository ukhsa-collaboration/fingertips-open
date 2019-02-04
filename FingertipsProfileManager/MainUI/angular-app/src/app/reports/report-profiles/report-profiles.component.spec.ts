import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ReportProfilesComponent } from './report-profiles.component';
import { ProfileService } from 'app/services/profile.service';
import { HttpService } from 'app/services/http.service';
import { Http } from '@angular/http';

describe('ReportProfilesComponent', () => {

  let component: ReportProfilesComponent;
  let fixture: ComponentFixture<ReportProfilesComponent>;

  let mockProfileService: any;
  let mockHttpService: any;

  beforeEach(async(() => {

    mockProfileService = jasmine.createSpyObj('ProfileService',
      ['getProfiles', 'getAllAreaTypes', 'getDomainName', 'getGroupingPlusNames', 'getGroupingSubheadingsForProfile', 'saveReorderedIndicators']);
    mockHttpService = jasmine.createSpyObj('HttpService', ['httpGet', 'httpPost']);

    TestBed.configureTestingModule({
      declarations: [ReportProfilesComponent],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        { provide: ProfileService, useValue: mockProfileService },
        { provide: HttpService, useValue: mockHttpService },
        { provide: Http }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportProfilesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
