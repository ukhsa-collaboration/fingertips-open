import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegendBobComponent } from './legend-bob.component';

describe('LegendBobComponent', () => {
  let component: LegendBobComponent;
  let fixture: ComponentFixture<LegendBobComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegendBobComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegendBobComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
