import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { MockFTHelperService } from '../../mock/ftHelper.service.mock';
import { IndicatorService } from '../../shared/service/api/indicator.service';
import { MockIndicatorService } from '../../mock/indicator.service.mock';
import { MetadataTableComponent } from './metadata-table.component';

describe('MetadataTableComponent', () => {
    let component: MetadataTableComponent;
    let fixture: ComponentFixture<MetadataTableComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [MetadataTableComponent],
            providers: [
                { provide: IndicatorService, useClass: MockIndicatorService },
                { provide: FTHelperService, useClass: MockFTHelperService }
            ]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(MetadataTableComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should be created', () => {
        expect(component).toBeTruthy();
    });
});
