import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { MockFTHelperService } from '../../mock/ftHelper.service.mock';
import { MapTableComponent } from './map-table.component';

describe('MapTableComponent', () => {
    let component: MapTableComponent;
    let fixture: ComponentFixture<MapTableComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [MapTableComponent],
            providers: [
                { provide: FTHelperService, useClass: MockFTHelperService }
            ]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(MapTableComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
