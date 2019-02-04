import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ProfileService } from '../../shared/service/api/profile.service';
import { DownloadReportComponent } from './download-report.component';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { StaticReportsService } from '../../shared/service/api/static-reports.service';
import { LightBoxService } from '../../shared/service/helper/light-box.service';

describe('DownloadReportComponent', () => {
  let component: DownloadReportComponent;
  let fixture: ComponentFixture<DownloadReportComponent>;

  let ftHelperService: any;
  let profileService: any;
  let staticReportsService: any;
  let lightBoxService: any;

  beforeEach(async(() => {

    profileService = jasmine.createSpyObj('ProfileService', ['']);
    ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);
    staticReportsService = jasmine.createSpyObj('StaticReportsService', ['']);
    lightBoxService = jasmine.createSpyObj('LightBoxService', ['']);

    TestBed.configureTestingModule({
      providers: [
        { provide: ProfileService, useValue: profileService },
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: LightBoxService, useValue: lightBoxService },
        { provide: StaticReportsService, useValue: staticReportsService },
      ],
      declarations: [DownloadReportComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DownloadReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
