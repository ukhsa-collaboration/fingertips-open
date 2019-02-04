import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegendRecentTrendsComponent } from './legend-recent-trends.component';
import { FTHelperService } from '../../../service/helper/ftHelper.service';

describe('LegendRecentTrendsComponent', () => {
  let component: LegendRecentTrendsComponent;
  let fixture: ComponentFixture<LegendRecentTrendsComponent>;

  // Spies
  let ftHelperService: any;

  beforeEach(async(() => {

    // Create the spies
    ftHelperService = jasmine.createSpyObj('FTHelperService', ['showTrendInfo']);

    TestBed.configureTestingModule({
      declarations: [LegendRecentTrendsComponent],
      providers: [{ provide: FTHelperService, useValue: ftHelperService }]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegendRecentTrendsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
