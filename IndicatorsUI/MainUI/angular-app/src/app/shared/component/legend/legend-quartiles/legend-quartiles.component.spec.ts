import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegendQuartilesComponent } from './legend-quartiles.component';

describe('LegendQuartilesComponent', () => {
  let component: LegendQuartilesComponent;
  let fixture: ComponentFixture<LegendQuartilesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegendQuartilesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegendQuartilesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
