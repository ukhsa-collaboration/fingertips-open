import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../../shared/service/api/indicator.service';
import { MetadataTableComponent } from './metadata-table.component';
import { ProfileService } from '../../shared/service/api/profile.service';
import { LegendCompareAreasComponent } from '../../shared/component/legend/legend-compare-areas/legend-compare-areas.component';
import { LegendComponent } from '../../shared/component/legend/legend.component';
import { LegendInequalitiesComponent } from '../../shared/component/legend/legend-inequalities/legend-inequalities.component';
import { LegendMapComponent } from '../../shared/component/legend/legend-map/legend-map.component';
import { LegendRecentTrendsComponent } from '../../shared/component/legend/legend-recent-trends/legend-recent-trends.component';
import { LegendTrendComponent } from '../../shared/component/legend/legend-trend/legend-trend.component';
import { LegendDataQualityComponent } from '../../shared/component/legend/legend-data-quality/legend-data-quality.component';
import { LegendBenchmarkAgainstGoalComponent } from '../../shared/component/legend/legend-benchmark-against-goal/legend-benchmark-against-goal.component';
import { ProfileIds } from '../../shared/constants';

describe('MetadataTableComponent', () => {
    let component: MetadataTableComponent;
    let fixture: ComponentFixture<MetadataTableComponent>;

    let indicatorService: any;
    let ftHelperService: any;
    let profileService: any;

    beforeEach(() => {

        profileService = jasmine.createSpyObj('IndicatorService', ['']);
        indicatorService = jasmine.createSpyObj('ProfileService', ['']);
        ftHelperService = jasmine.createSpyObj('FTHelperService', ['getFTConfig', 'getFTModel']);

        TestBed.configureTestingModule({
            declarations: [
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
                { provide: IndicatorService, useValue: indicatorService },
                { provide: ProfileService, useValue: profileService },
                { provide: FTHelperService, useValue: ftHelperService }
            ]
        })
            .compileComponents();
    });

    it('should be created', () => {
        ftHelperService.getFTConfig.and.returnValue({ showDataQuality: true });
        ftHelperService.getFTModel.and.returnValue({ profileId: ProfileIds.Phof });
        createComponent();

        expect(component).toBeTruthy();
        expect(ftHelperService.getFTConfig).toHaveBeenCalledTimes(1);
    });

    const createComponent = function () {
        fixture = TestBed.createComponent(MetadataTableComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    };
});
