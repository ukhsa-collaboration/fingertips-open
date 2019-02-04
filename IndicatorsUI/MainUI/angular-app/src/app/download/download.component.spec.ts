import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { DownloadComponent } from './download.component';
import { DownloadReportComponent } from './download-report/download-report.component';
import { DownloadDataComponent } from './download-data/download-data.component';
import { ProfileService } from '../shared/service/api/profile.service';
import { StaticReportsService } from '../shared/service/api/static-reports.service';
import { LightBoxService } from '../shared/service/helper/light-box.service';

describe('DownloadComponent', () => {
  let component: DownloadComponent;
  let fixture: ComponentFixture<DownloadComponent>;

  let ftHelperService: any;
  let profileService: any;
  let staticReportsService: any;
  let lightBoxService: any;

  const createComponent = function () {
    fixture = TestBed.createComponent(DownloadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  const fireOutsideEvent = function () {
    component.onOutsideEvent(null);
    fixture.detectChanges();
  }

  beforeEach(async(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['unlock', 'showAndHidePageElements', 'getFTModel', 'getCurrentGroupRoot']);
    profileService = jasmine.createSpyObj('ProfileService', ['']);
    staticReportsService = jasmine.createSpyObj('StaticReportsService', ['']);
    lightBoxService = jasmine.createSpyObj('LightBoxService', ['']);

    TestBed.configureTestingModule({
      declarations: [DownloadComponent, DownloadReportComponent, DownloadDataComponent],
      providers: [
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: ProfileService, useValue: profileService },
        { provide: StaticReportsService, useValue: staticReportsService },
        { provide: LightBoxService, useValue: lightBoxService }
      ]
    })
      .compileComponents();
  }));

  it('should create', () => {
    createComponent();
    expect(component).toBeTruthy();
  });

});
