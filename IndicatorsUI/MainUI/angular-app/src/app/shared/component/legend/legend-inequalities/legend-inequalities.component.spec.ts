import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegendInequalitiesComponent } from './legend-inequalities.component';

describe('LegendInequalitiesComponent', () => {
  let component: LegendInequalitiesComponent;
  let fixture: ComponentFixture<LegendInequalitiesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegendInequalitiesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegendInequalitiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
