import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegendDataQualityComponent } from './legend-data-quality.component';

describe('LegendDataQualityComponent', () => {
  let component: LegendDataQualityComponent;
  let fixture: ComponentFixture<LegendDataQualityComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegendDataQualityComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegendDataQualityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
