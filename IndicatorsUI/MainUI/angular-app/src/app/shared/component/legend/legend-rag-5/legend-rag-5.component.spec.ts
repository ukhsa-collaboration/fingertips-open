import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegendRag5Component } from './legend-rag-5.component';

describe('LegendRag5Component', () => {
  let component: LegendRag5Component;
  let fixture: ComponentFixture<LegendRag5Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegendRag5Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegendRag5Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
