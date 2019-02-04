import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ProfileListComponent } from './profile-list.component';
import { ProfileService } from 'app/services/profile.service';
import { HttpService } from 'app/services/http.service';
import { Http } from '@angular/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

describe('ProfileListComponent', () => {
  let component: ProfileListComponent;
  let fixture: ComponentFixture<ProfileListComponent>;

  let mockProfileService: any;
  let mockHttpService: any;
  let mockHttp: any;

  beforeEach(async(() => {

    mockProfileService = jasmine.createSpyObj('ProfileService', ['getProfiles']);
    mockHttpService = jasmine.createSpyObj('HttpService', ['httpGet', 'httpPost']);
    mockHttp = jasmine.createSpyObj('Http', ['get', 'post']);

    TestBed.configureTestingModule({
      declarations: [ProfileListComponent],
      imports: [FormsModule, ReactiveFormsModule],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        { provide: ProfileService, useValue: mockProfileService },
        { provide: HttpService, useValue: mockHttpService },
        { provide: Http, useValue: mockHttp }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProfileListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
