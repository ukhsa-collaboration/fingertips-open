import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { LightBoxIndicatorReorderComponent } from './light-box-indicator-reorder.component';

describe('LightBoxIndicatorReorderComponent', () => {
  let component: LightBoxIndicatorReorderComponent;
  let fixture: ComponentFixture<LightBoxIndicatorReorderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LightBoxIndicatorReorderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LightBoxIndicatorReorderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
