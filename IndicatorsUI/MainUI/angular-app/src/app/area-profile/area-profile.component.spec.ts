import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { AreaProfileComponent } from './area-profile.component';
import { LegendComponent } from '../shared/component/legend/legend.component';
import { LegendMapComponent } from '../shared/component/legend/legend-map/legend-map.component';
import { LegendRecentTrendsComponent } from '../shared/component/legend/legend-recent-trends/legend-recent-trends.component';
import { SpineChartComponent } from './spine-chart/spine-chart.component';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { UIService } from '../shared/service/helper/ui.service';
import { LegendCompareAreasComponent } from '../shared/component/legend/legend-compare-areas/legend-compare-areas.component';
import { LegendInequalitiesComponent } from '../shared/component/legend/legend-inequalities/legend-inequalities.component';
import { LegendTrendComponent } from '../shared/component/legend/legend-trend/legend-trend.component';
import { LegendDataQualityComponent } from '../shared/component/legend/legend-data-quality/legend-data-quality.component';
import {
  LegendBenchmarkAgainstGoalComponent
} from '../shared/component/legend/legend-benchmark-against-goal/legend-benchmark-against-goal.component';
import { LightBoxComponent } from '../shared/component/light-box/light-box.component';
import { ExportCsvComponent } from '../shared/component/export-csv/export-csv.component';
import { DeviceDetectorService } from '../../../node_modules/ngx-device-detector';

describe('AreaProfileComponent', () => {
  let component: AreaProfileComponent;
  let fixture: ComponentFixture<AreaProfileComponent>;

  // Spies
  let ftHelperService: any;
  let indicatorService: any;
  let uiService: any;
  let ddService: any;

  beforeEach(async(() => {

    // Create spies
    ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);
    indicatorService = jasmine.createSpyObj('IndicatorService', ['']);
    uiService = jasmine.createSpyObj('UIService', ['']);
    ddService = jasmine.createSpyObj('DeviceDetectorService', ['']);

    TestBed.configureTestingModule({
      declarations: [
        AreaProfileComponent,
        SpineChartComponent,
        LegendComponent,
        LegendCompareAreasComponent,
        LegendInequalitiesComponent,
        LegendMapComponent,
        LegendRecentTrendsComponent,
        LegendTrendComponent,
        LegendDataQualityComponent,
        LegendBenchmarkAgainstGoalComponent,
        LightBoxComponent,
        ExportCsvComponent
      ],
      providers: [
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: IndicatorService, useValue: indicatorService },
        { provide: UIService, useValue: uiService },
        { provide: DeviceDetectorService, useValue: ddService }
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
