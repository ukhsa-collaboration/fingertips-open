import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportParametersComponent } from './report-parameters.component';

describe('ReportParametersComponent', () => {
  let component: ReportParametersComponent;
  let fixture: ComponentFixture<ReportParametersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReportParametersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportParametersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
