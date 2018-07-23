import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportEditViewComponent } from './report-edit-view.component';

describe('ReportEditViewComponent', () => {
  let component: ReportEditViewComponent;
  let fixture: ComponentFixture<ReportEditViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReportEditViewComponent ]
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
