import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { MockFTHelperService } from '../../mock/ftHelper.service.mock';
import { PopulationSummaryComponent } from './population-summary.component';

describe('PopulationSummaryComponent', () => {
    let component: PopulationSummaryComponent;
    let fixture: ComponentFixture<PopulationSummaryComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [PopulationSummaryComponent],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
            providers: [
                { provide: FTHelperService, useClass: MockFTHelperService }
            ]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(PopulationSummaryComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should be created', () => {
        expect(component).toBeTruthy();
    });
});
