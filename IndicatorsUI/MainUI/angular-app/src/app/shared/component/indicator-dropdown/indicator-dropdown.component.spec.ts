import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IndicatorDropdownComponent } from './indicator-dropdown.component';

describe('IndicatorDropdownComponent', () => {
  let component: IndicatorDropdownComponent;
  let fixture: ComponentFixture<IndicatorDropdownComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [IndicatorDropdownComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IndicatorDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
