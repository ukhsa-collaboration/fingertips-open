import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ReportsService } from 'app/services/reports.service';
import { ReportEditViewComponent } from './report-edit-view.component';
import { FormsModule } from '@angular/forms';

describe('ReportListViewComponent', () => {
  let component: ReportEditViewComponent;
  let fixture: ComponentFixture<ReportEditViewComponent>;

  let mockReportsService: any;

  beforeEach(async(() => {

    mockReportsService = jasmine.createSpyObj('ReportsService', ['getReportById']);

    TestBed.configureTestingModule({
      declarations: [ReportEditViewComponent],
      imports: [FormsModule],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        { provide: ReportsService, useValue: mockReportsService },
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportEditViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
