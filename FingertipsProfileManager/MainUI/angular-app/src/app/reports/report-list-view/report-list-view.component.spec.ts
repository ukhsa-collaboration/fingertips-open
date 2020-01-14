import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ReportsService } from '../../services/reports.service';
import { ReportListViewComponent } from './report-list-view.component';
import { HttpService } from '../../services/http.service';
import { Observable, of } from 'rxjs';

describe('ReportListViewComponent', () => {

  let component: ReportListViewComponent;
  let fixture: ComponentFixture<ReportListViewComponent>;

  let mockReportsService: any;
  let mockHttpService: any;

  beforeEach(async(() => {

    mockReportsService = jasmine.createSpyObj('ReportsService', ['getReports', 'deleteReportById']);
    mockHttpService = jasmine.createSpyObj('HttpService', ['httpGet', 'httpPost']);

    TestBed.configureTestingModule({
      declarations: [ReportListViewComponent],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        { provide: ReportsService, useValue: mockReportsService },
        { provide: HttpService, useValue: mockHttpService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportListViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {

  //   let data = [
  //     {
  //       id: 36,
  //       name: 'Mental health in pregnancy and infants',
  //       file: 'chimat/Mental_health_in_pregnancy_and_infants',
  //       notes: 'These reports are available for local authorities and CCGs.',
  //       isLive: true
  //     }];
  //   mockReportsService.getReports.and.returnValue(Observable.of(data));

  //   expect(component).toBeTruthy();
  // });
});
