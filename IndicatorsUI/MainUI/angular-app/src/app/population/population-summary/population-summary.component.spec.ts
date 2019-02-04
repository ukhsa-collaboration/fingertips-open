import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { PopulationSummaryComponent } from './population-summary.component';

describe('PopulationSummaryComponent', () => {
    let component: PopulationSummaryComponent;
    let fixture: ComponentFixture<PopulationSummaryComponent>;

    let ftHelperService: any;

    beforeEach(async(() => {

        ftHelperService = jasmine.createSpyObj('FTHelperService', ['getFTModel']);

        TestBed.configureTestingModule({
            declarations: [PopulationSummaryComponent],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
            providers: [
                { provide: FTHelperService, useValue: ftHelperService }
            ]
        })
            .compileComponents();
    }));

    const createComponent = () => {
        fixture = TestBed.createComponent(PopulationSummaryComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    };

    it('should be created', () => {

        // Arrange: FtHelperService
        ftHelperService.getFTModel.and.returnValue({});

        createComponent();
        expect(component).toBeTruthy();
    });

});
