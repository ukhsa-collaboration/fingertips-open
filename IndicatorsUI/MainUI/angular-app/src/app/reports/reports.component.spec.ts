import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportsComponent } from './reports.component';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { SsrsReportService } from '../shared/service/api/ssrs-report.service';
import { ContentService } from '../shared/service/api/content.service';

describe('ReportsComponent', () => {
  let component: ReportsComponent;
  let fixture: ComponentFixture<ReportsComponent>;

  let ftHelperService: any;
  let ssrsReportService: any;
  let contentService: any;

  beforeEach(async(() => {

    contentService = jasmine.createSpyObj('ContentService', ['']);
    ssrsReportService = jasmine.createSpyObj('SsrsReportService', ['']);
    ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);

    TestBed.configureTestingModule({
      declarations: [ReportsComponent],
      providers: [
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: ContentService, useValue: contentService },
        { provide: SsrsReportService, useValue: ssrsReportService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
