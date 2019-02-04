import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AreaProfileComponent } from './area-profile.component';
import { LegendComponent } from '../shared/component/legend/legend.component';
import { LegendAreaProfilesComponent } from '../shared/component/legend/legend-area-profiles/legend-area-profiles.component';
import { LegendMapComponent } from '../shared/component/legend/legend-map/legend-map.component';
import { LegendBobComponent } from '../shared/component/legend/legend-bob/legend-bob.component';
import { LegendContinuousComponent } from '../shared/component/legend/legend-continuous/legend-continuous.component';
import { LegendQuartilesComponent } from '../shared/component/legend/legend-quartiles/legend-quartiles.component';
import { LegendQuintilesComponent } from '../shared/component/legend/legend-quintiles/legend-quintiles.component';
import { LegendRag3Component } from '../shared/component/legend/legend-rag-3/legend-rag-3.component';
import { LegendRag5Component } from '../shared/component/legend/legend-rag-5/legend-rag-5.component';
import { LegendRecentTrendsComponent } from '../shared/component/legend/legend-recent-trends/legend-recent-trends.component';
import { SpineChartComponent } from './spine-chart/spine-chart.component';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { UIService } from '../shared/service/helper/ui.service';

describe('AreaProfileComponent', () => {
  let component: AreaProfileComponent;
  let fixture: ComponentFixture<AreaProfileComponent>;

  // Spies
  let ftHelperService: any;
  let indicatorService: any;
  let uiService: any;

  beforeEach(async(() => {

    // Create spies
    ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);
    indicatorService = jasmine.createSpyObj('IndicatorService', ['']);
    uiService = jasmine.createSpyObj('UIService', ['']);

    TestBed.configureTestingModule({
      declarations: [
        AreaProfileComponent,
        SpineChartComponent,
        LegendComponent,
        LegendAreaProfilesComponent,
        LegendMapComponent,
        LegendBobComponent,
        LegendContinuousComponent,
        LegendQuartilesComponent,
        LegendQuintilesComponent,
        LegendRag3Component,
        LegendRag5Component,
        LegendRecentTrendsComponent
      ],
      providers: [
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: IndicatorService, useValue: indicatorService },
        { provide: UIService, useValue: uiService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AreaProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
