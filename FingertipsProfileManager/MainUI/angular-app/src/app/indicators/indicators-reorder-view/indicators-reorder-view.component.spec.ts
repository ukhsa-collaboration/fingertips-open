import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { IndicatorsReorderViewComponent } from './indicators-reorder-view.component';
import { LightBoxComponent } from '../../shared/component/light-box/light-box.component';
import { LightBoxIndicatorReorderComponent } from '../../shared/component/light-box-indicator-reorder/light-box-indicator-reorder.component';
import { ProfileService } from '../../services/profile.service';
import { HttpService } from '../../services/http.service';
import { HttpClient } from '@angular/common/http';

describe('IndicatorsReorderViewComponent', () => {

    let component: IndicatorsReorderViewComponent;
    let fixture: ComponentFixture<IndicatorsReorderViewComponent>;

    let mockProfileService: any;
    let mockHttpService: any;

    beforeEach(async(() => {

        mockProfileService = jasmine.createSpyObj('ProfileService',
            ['getAllAreaTypes', 'getDomainName', 'getGroupingPlusNames', 'getGroupingSubheadingsForProfile', 'saveReorderedIndicators']);
        mockHttpService = jasmine.createSpyObj('HttpService', ['httpGet', 'httpPost']);

        TestBed.configureTestingModule({
            declarations: [
                IndicatorsReorderViewComponent,
                LightBoxComponent,
                LightBoxIndicatorReorderComponent
            ],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
            providers: [
                { provide: ProfileService, useValue: mockProfileService },
                { provide: HttpService, useValue: mockHttpService },
                { provide: HttpClient }
            ]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(IndicatorsReorderViewComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
