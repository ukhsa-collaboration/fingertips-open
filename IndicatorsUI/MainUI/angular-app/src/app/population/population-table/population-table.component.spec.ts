import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PopulationTableComponent } from './population-table.component';
import { ExportCsvComponent } from '../../shared/component/export-csv/export-csv.component';
import { MetadataComponent } from '../../metadata/metadata.component';
import { MetadataTableComponent } from '../../metadata/metadata-table/metadata-table.component';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../../shared/service/api/indicator.service';
import { ProfileService } from '../../shared/service/api/profile.service';
import { LegendComponent } from '../../shared/component/legend/legend.component';
import { LegendDataQualityComponent } from '../../shared/component/legend/legend-data-quality/legend-data-quality.component';
import { LegendBenchmarkAgainstGoalComponent } from '../../shared/component/legend/legend-benchmark-against-goal/legend-benchmark-against-goal.component';
import { LegendMapComponent } from '../../shared/component/legend/legend-map/legend-map.component';
import { LegendCompareAreasComponent } from '../../shared/component/legend/legend-compare-areas/legend-compare-areas.component';
import { LegendInequalitiesComponent } from '../../shared/component/legend/legend-inequalities/legend-inequalities.component';
import { LegendRecentTrendsComponent } from '../../shared/component/legend/legend-recent-trends/legend-recent-trends.component';
import { LegendTrendComponent } from '../../shared/component/legend/legend-trend/legend-trend.component';

describe('PopulationTableComponent', () => {
  let component: PopulationTableComponent;
  let fixture: ComponentFixture<PopulationTableComponent>;

  let mockFTHelperService: any;
  let mockIndicatorService: any;
  let mockProfileService: any;

  beforeEach(async(() => {

    mockFTHelperService = jasmine.createSpyObj('FTHelperService', ['newTooltipManager', 'getFTConfig']);
    mockIndicatorService = jasmine.createSpyObj('IndicatorService', ['']);
    mockProfileService = jasmine.createSpyObj('ProfileService', ['']);

    TestBed.configureTestingModule({
      declarations: [
        PopulationTableComponent,
        ExportCsvComponent,
        MetadataComponent,
        MetadataTableComponent,
        LegendComponent,
        LegendCompareAreasComponent,
        LegendInequalitiesComponent,
        LegendMapComponent,
        LegendRecentTrendsComponent,
        LegendTrendComponent,
        LegendDataQualityComponent,
        LegendBenchmarkAgainstGoalComponent
      ],
      providers: [
        { provide: FTHelperService, useValue: mockFTHelperService },
        { provide: IndicatorService, useValue: mockIndicatorService },
        { provide: ProfileService, useValue: mockProfileService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PopulationTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
