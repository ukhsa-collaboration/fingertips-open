import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegendTrendComponent } from './legend-trend.component';

describe('LegendTrendComponent', () => {
  let component: LegendTrendComponent;
  let fixture: ComponentFixture<LegendTrendComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegendTrendComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegendTrendComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
