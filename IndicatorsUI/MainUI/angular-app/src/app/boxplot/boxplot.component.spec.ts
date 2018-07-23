import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { MockFTHelperService } from '../mock/ftHelper.service.mock';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { MockIndicatorService } from '../mock/indicator.service.mock';
import { BoxplotComponent } from './boxplot.component';

describe('BoxplotComponent', () => {
    let component: BoxplotComponent;
    let fixture: ComponentFixture<BoxplotComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [BoxplotComponent],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
            providers: [
                { provide: IndicatorService, useClass: MockIndicatorService },
                { provide: FTHelperService, useClass: MockFTHelperService }
            ]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(BoxplotComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should be created', () => {
        expect(component).toBeTruthy();
    });
});
