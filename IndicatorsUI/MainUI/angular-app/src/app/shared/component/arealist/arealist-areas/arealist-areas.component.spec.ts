import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ArealistAreasComponent } from './arealist-areas.component';
import { AreaListService } from '../../../service/api/arealist.service';
import { AreaService } from '../../../service/api/area.service';

describe('ArealistAreasComponent', () => {
    let component: ArealistAreasComponent;
    let fixture: ComponentFixture<ArealistAreasComponent>;

    // Spies
    let areaService: any;
    let areaListService: any;

    beforeEach(async(() => {

        // Create spies
        areaService = jasmine.createSpyObj('AreaSerice', ['']);
        areaListService = jasmine.createSpyObj('AreaListService', ['']);

        TestBed.configureTestingModule({
            declarations: [ArealistAreasComponent],
            providers: [
                { provide: AreaService, useValue: areaService },
                { provide: AreaListService, useValue: areaListService }
            ]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(ArealistAreasComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
