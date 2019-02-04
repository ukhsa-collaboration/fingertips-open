import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ReportsComponent } from './reports.component';
import { ProfileService } from 'app/services/profile.service';
import { HttpService } from 'app/services/http.service';
import { Http } from '@angular/http';

describe('ReportsComponent', () => {
  let component: ReportsComponent;
  let fixture: ComponentFixture<ReportsComponent>;

  let mockProfileService: any;
  let mockHttpService: any;

  beforeEach(async(() => {

    mockProfileService = jasmine.createSpyObj('ProfileService', ['getProfiles']);
    mockHttpService = jasmine.createSpyObj('HttpService', ['httpGet', 'httpPost']);

    TestBed.configureTestingModule({
      declarations: [ReportsComponent],
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
    fixture = TestBed.createComponent(ReportsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
