import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegendQuintilesComponent } from './legend-quintiles.component';

describe('LegendQuintilesComponent', () => {
  let component: LegendQuintilesComponent;
  let fixture: ComponentFixture<LegendQuintilesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegendQuintilesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegendQuintilesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
