import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ReportParametersComponent } from './report-parameters.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

describe('ReportParametersComponent', () => {
  let component: ReportParametersComponent;
  let fixture: ComponentFixture<ReportParametersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ReportParametersComponent],
      imports: [FormsModule, ReactiveFormsModule],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
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
