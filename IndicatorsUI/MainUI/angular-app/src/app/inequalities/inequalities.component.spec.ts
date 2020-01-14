import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { InequalitiesComponent } from './inequalities.component';
import { LegendComponent } from '../shared/component/legend/legend.component';
import { InequalitiesBarChartComponent } from './inequalities-bar-chart/inequalities-bar-chart.component';
import { InequalitiesTrendChartComponent } from './inequalities-trend-chart/inequalities-trend-chart.component';
import { InequalitiesTrendTableComponent } from './inequalities-trend-table/inequalities-trend-table.component';
import { LightBoxComponent } from '../shared/component/light-box/light-box.component';
import { LegendCompareAreasComponent } from '../shared/component/legend/legend-compare-areas/legend-compare-areas.component';
import { LegendInequalitiesComponent } from '../shared/component/legend/legend-inequalities/legend-inequalities.component';
import { LegendMapComponent } from '../shared/component/legend/legend-map/legend-map.component';
import { LegendRecentTrendsComponent } from '../shared/component/legend/legend-recent-trends/legend-recent-trends.component';
import { LegendTrendComponent } from '../shared/component/legend/legend-trend/legend-trend.component';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { UIService } from '../shared/service/helper/ui.service';
import { DownloadService } from '../shared/service/api/download.service';
import { CoreDataHelperService } from '../shared/service/helper/coreDataHelper.service';
import { LegendDataQualityComponent } from '../shared/component/legend/legend-data-quality/legend-data-quality.component';
import {
  LegendBenchmarkAgainstGoalComponent
} from '../shared/component/legend/legend-benchmark-against-goal/legend-benchmark-against-goal.component';
import { ExportCsvComponent } from '../shared/component/export-csv/export-csv.component';

describe('InequalitiesComponent', () => {
  let component: InequalitiesComponent;
  let fixture: ComponentFixture<InequalitiesComponent>;

  let ftHelperService: any;
  let uiService: any;
  let downloadService: any;
  let indicatorService: any;
  let coreDataHelperService: any;

  beforeEach(async(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);
    uiService = jasmine.createSpyObj('UIService', ['']);
    downloadService = jasmine.createSpyObj('DownloadService', ['']);
    indicatorService = jasmine.createSpyObj('IndicatorService', ['']);
    coreDataHelperService = jasmine.createSpyObj('CoreDataHelperService', ['']);

    TestBed.configureTestingModule({
      declarations: [
        InequalitiesComponent,
        InequalitiesBarChartComponent,
        InequalitiesTrendChartComponent,
        InequalitiesTrendTableComponent,
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
        { provide: UIService, useValue: uiService },
        { provide: DownloadService, useValue: downloadService },
        { provide: IndicatorService, useValue: indicatorService },
        { provide: CoreDataHelperService, useValue: coreDataHelperService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InequalitiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
