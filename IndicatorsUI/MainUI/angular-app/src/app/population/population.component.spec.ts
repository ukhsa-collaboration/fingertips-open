import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { PopulationComponent } from './population.component';

describe('PopulationComponent', () => {
    let component: PopulationComponent;
    let fixture: ComponentFixture<PopulationComponent>;

    let mockIndicatorService: any;
    let mockFTHelperService: any;

    beforeEach(async(() => {

        mockIndicatorService = jasmine.createSpyObj('IndicatorService', ['']);
        mockFTHelperService = jasmine.createSpyObj('FTHelperService', ['']);

        TestBed.configureTestingModule({
            declarations: [PopulationComponent],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
            providers: [
                { provide: IndicatorService, useValue: mockIndicatorService },
                { provide: FTHelperService, useValue: mockFTHelperService }
            ]
        })
            .compileComponents();
    }));

    const createComponent = () => {
        fixture = TestBed.createComponent(PopulationComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    };

    it('should be created', () => {
        createComponent();
        expect(component).toBeTruthy();
    });
});
