import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FTHelperService } from '../../service/helper/ftHelper.service';
import { LegendComponent } from './legend.component';
import { LegendRecentTrendsComponent } from './legend-recent-trends/legend-recent-trends.component';
import { LegendMapComponent } from './legend-map/legend-map.component';
import { LegendTrendComponent } from './legend-trend/legend-trend.component';
import { LegendCompareAreasComponent } from './legend-compare-areas/legend-compare-areas.component';
import { LegendInequalitiesComponent } from './legend-inequalities/legend-inequalities.component';
import { LegendDataQualityComponent } from './legend-data-quality/legend-data-quality.component';
import { LegendBenchmarkAgainstGoalComponent } from './legend-benchmark-against-goal/legend-benchmark-against-goal.component';

describe('LegendComponent', () => {
    let component: LegendComponent;
    let fixture: ComponentFixture<LegendComponent>;

    // Spies
    let ftHelperService: any;

    beforeEach(async(() => {

        // Create the spies
        ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);

        TestBed.configureTestingModule({
            declarations: [
                LegendComponent,
                LegendRecentTrendsComponent,
                LegendMapComponent,
                LegendTrendComponent,
                LegendCompareAreasComponent,
                LegendInequalitiesComponent,
                LegendDataQualityComponent,
                LegendBenchmarkAgainstGoalComponent
            ],
            providers: [{ provide: FTHelperService, useValue: ftHelperService }]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(LegendComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
