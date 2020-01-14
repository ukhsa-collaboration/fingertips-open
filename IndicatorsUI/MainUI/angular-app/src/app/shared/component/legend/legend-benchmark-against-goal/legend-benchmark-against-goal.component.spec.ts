import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegendBenchmarkAgainstGoalComponent } from './legend-benchmark-against-goal.component';

describe('LegendBenchmarkAgainstGoalComponent', () => {
  let component: LegendBenchmarkAgainstGoalComponent;
  let fixture: ComponentFixture<LegendBenchmarkAgainstGoalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegendBenchmarkAgainstGoalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegendBenchmarkAgainstGoalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
