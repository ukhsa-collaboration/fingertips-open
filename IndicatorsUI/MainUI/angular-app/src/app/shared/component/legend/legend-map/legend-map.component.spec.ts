import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegendMapComponent } from './legend-map.component';

describe('LegendMapComponent', () => {
  let component: LegendMapComponent;
  let fixture: ComponentFixture<LegendMapComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegendMapComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegendMapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
