import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { EnglandComponent } from './england.component';
import { LegendComponent } from '../shared/component/legend/legend.component';
import { LegendMapComponent } from '../shared/component/legend/legend-map/legend-map.component';
import { LegendRecentTrendsComponent } from '../shared/component/legend/legend-recent-trends/legend-recent-trends.component';
import { LegendTrendComponent } from '../shared/component/legend/legend-trend/legend-trend.component';
import { LegendInequalitiesComponent } from '../shared/component/legend/legend-inequalities/legend-inequalities.component';
import { LegendCompareAreasComponent } from '../shared/component/legend/legend-compare-areas/legend-compare-areas.component';
import { DownloadService } from '../shared/service/api/download.service';
import { LegendDataQualityComponent } from '../shared/component/legend/legend-data-quality/legend-data-quality.component';
import {
    LegendBenchmarkAgainstGoalComponent
} from '../shared/component/legend/legend-benchmark-against-goal/legend-benchmark-against-goal.component';
import { ExportCsvComponent } from '../shared/component/export-csv/export-csv.component';

describe('EnglandComponent', () => {
    let component: EnglandComponent;
    let fixture: ComponentFixture<EnglandComponent>;

    let indicatorService: any;
    let ftHelperService: any;
    let downloadService: any;

    beforeEach(async(() => {

        indicatorService = jasmine.createSpyObj('IndicatorService', ['']);
        ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);
        downloadService = jasmine.createSpyObj('DownloadService', ['']);

        TestBed.configureTestingModule({
            declarations: [
                EnglandComponent,
                LegendComponent,
                LegendCompareAreasComponent,
                LegendInequalitiesComponent,
                LegendMapComponent,
                LegendRecentTrendsComponent,
                LegendTrendComponent,
                LegendDataQualityComponent,
                LegendBenchmarkAgainstGoalComponent,
                ExportCsvComponent
            ],
            providers: [
                { provide: FTHelperService, useValue: ftHelperService },
                { provide: IndicatorService, useValue: indicatorService },
                { provide: DownloadService, useValue: downloadService }
            ]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(EnglandComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

});
