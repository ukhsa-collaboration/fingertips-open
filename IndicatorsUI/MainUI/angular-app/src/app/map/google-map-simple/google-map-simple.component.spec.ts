import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GoogleMapSimpleComponent } from './google-map-simple.component';
import { GoogleMapService } from '../googleMap.service';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';

describe('GoogleMapSimpleComponent', () => {
    let component: GoogleMapSimpleComponent;
    let fixture: ComponentFixture<GoogleMapSimpleComponent>;

    // Spies
    let ftHelperService: any;
    let googleMapService: any;

    beforeEach(async(() => {

        // Create spies
        ftHelperService = jasmine.createSpyObj('FTHelperService', ['getURL']);
        googleMapService = jasmine.createSpyObj('GoogleMapService', ['']);

        TestBed.configureTestingModule({
            declarations: [GoogleMapSimpleComponent],
            providers: [
                { provide: FTHelperService, useValue: ftHelperService },
                { provide: GoogleMapService, useValue: googleMapService }
            ]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(GoogleMapSimpleComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
