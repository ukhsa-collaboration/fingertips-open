import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { MapTableComponent } from './map-table.component';

describe('MapTableComponent', () => {
    let component: MapTableComponent;
    let fixture: ComponentFixture<MapTableComponent>;

    let mockFTHelperService: any;

    beforeEach(() => {

        mockFTHelperService = jasmine.createSpyObj('FTHelperService', ['']);

        TestBed.configureTestingModule({
            declarations: [MapTableComponent],
            providers: [
                { provide: FTHelperService, useValue: mockFTHelperService }
            ]
        })
            .compileComponents();
    });

    it('should create', () => {
        createComponent();
        expect(component).toBeTruthy();
    });

    const createComponent = function () {
        fixture = TestBed.createComponent(MapTableComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    };
});
