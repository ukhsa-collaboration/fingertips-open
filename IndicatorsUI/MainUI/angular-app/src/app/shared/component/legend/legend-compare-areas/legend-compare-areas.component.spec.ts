import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegendCompareAreasComponent } from './legend-compare-areas.component';

describe('LegendCompareAreasComponent', () => {
  let component: LegendCompareAreasComponent;
  let fixture: ComponentFixture<LegendCompareAreasComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegendCompareAreasComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegendCompareAreasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
