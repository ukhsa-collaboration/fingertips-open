import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../../shared/service/api/indicator.service';
import { MetadataTableComponent } from './metadata-table.component';
import { ProfileService } from 'app/shared/service/api/profile.service';

describe('MetadataTableComponent', () => {
    let component: MetadataTableComponent;
    let fixture: ComponentFixture<MetadataTableComponent>;

    let indicatorService: any;
    let ftHelperService: any;
    let profileService: any;

    beforeEach(() => {

        profileService = jasmine.createSpyObj('IndicatorService', ['']);
        indicatorService = jasmine.createSpyObj('IndicatorService', ['']);
        ftHelperService = jasmine.createSpyObj('FTHelperService', ['getFTConfig']);

        TestBed.configureTestingModule({
            declarations: [MetadataTableComponent],
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
        createComponent();

        expect(component).toBeTruthy();
        expect(ftHelperService.getFTConfig).toHaveBeenCalledTimes(1);
    });

    let createComponent = function () {
        fixture = TestBed.createComponent(MetadataTableComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    };
});
