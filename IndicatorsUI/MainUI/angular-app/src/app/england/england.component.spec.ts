import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { MockFTHelperService } from '../mock/ftHelper.service.mock';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { MockIndicatorService } from '../mock/indicator.service.mock';
import { EnglandComponent } from './england.component';

describe('EnglandComponent', () => {
    let component: EnglandComponent;
    let fixture: ComponentFixture<EnglandComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [EnglandComponent],
            providers: [
                { provide: FTHelperService, useClass: MockFTHelperService },
                { provide: IndicatorService, useClass: MockIndicatorService }
            ]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(EnglandComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

});
