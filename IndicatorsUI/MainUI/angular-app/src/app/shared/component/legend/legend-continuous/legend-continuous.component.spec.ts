import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegendContinuousComponent } from './legend-continuous.component';

describe('LegendContinuousComponent', () => {
  let component: LegendContinuousComponent;
  let fixture: ComponentFixture<LegendContinuousComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegendContinuousComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegendContinuousComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
