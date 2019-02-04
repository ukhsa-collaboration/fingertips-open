import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegendComponent } from './legend.component';
import { LegendAreaProfilesComponent } from './legend-area-profiles/legend-area-profiles.component';
import { LegendMapComponent } from './legend-map/legend-map.component';
import { LegendBobComponent } from './legend-bob/legend-bob.component';
import { LegendContinuousComponent } from './legend-continuous/legend-continuous.component';
import { LegendQuartilesComponent } from './legend-quartiles/legend-quartiles.component';
import { LegendQuintilesComponent } from './legend-quintiles/legend-quintiles.component';
import { LegendRag3Component } from './legend-rag-3/legend-rag-3.component';
import { LegendRag5Component } from './legend-rag-5/legend-rag-5.component';
import { LegendRecentTrendsComponent } from './legend-recent-trends/legend-recent-trends.component';
import { FTHelperService } from '../../service/helper/ftHelper.service';

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
