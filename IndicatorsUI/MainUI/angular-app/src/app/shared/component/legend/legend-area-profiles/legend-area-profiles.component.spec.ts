import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegendAreaProfilesComponent } from './legend-area-profiles.component';

describe('LegendAreaProfilesComponent', () => {
  let component: LegendAreaProfilesComponent;
  let fixture: ComponentFixture<LegendAreaProfilesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegendAreaProfilesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegendAreaProfilesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
