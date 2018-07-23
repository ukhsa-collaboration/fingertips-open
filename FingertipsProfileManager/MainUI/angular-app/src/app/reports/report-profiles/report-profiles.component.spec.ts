import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportProfilesComponent } from './report-profiles.component';

describe('ReportProfilesComponent', () => {
  let component: ReportProfilesComponent;
  let fixture: ComponentFixture<ReportProfilesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReportProfilesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportProfilesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
