import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { MockFTHelperService } from '../mock/ftHelper.service.mock';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { MockIndicatorService } from '../mock/indicator.service.mock';
import { PopulationComponent } from './population.component';

describe('PopulationComponent', () => {
    let component: PopulationComponent;
    let fixture: ComponentFixture<PopulationComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [PopulationComponent],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
            providers: [
                { provide: IndicatorService, useClass: MockIndicatorService },
                { provide: FTHelperService, useClass: MockFTHelperService }
            ]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(PopulationComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should be created', () => {
        expect(component).toBeTruthy();
    });
});
