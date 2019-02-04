import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegendRag3Component } from './legend-rag-3.component';

describe('LegendRag3Component', () => {
  let component: LegendRag3Component;
  let fixture: ComponentFixture<LegendRag3Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegendRag3Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegendRag3Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
