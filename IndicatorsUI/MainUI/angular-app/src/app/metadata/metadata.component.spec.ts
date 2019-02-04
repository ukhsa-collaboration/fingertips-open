import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { MetadataComponent } from './metadata.component';

describe('MetadataComponent', () => {
    let component: MetadataComponent;
    let fixture: ComponentFixture<MetadataComponent>;

    let ftHelperService: any;

    beforeEach(async(() => {

        ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);

        TestBed.configureTestingModule({
            declarations: [MetadataComponent],
            schemas: [CUSTOM_ELEMENTS_SCHEMA],
            providers: [
                { provide: FTHelperService, useValue: ftHelperService }
            ]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(MetadataComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should be created', () => {
        expect(component).toBeTruthy();
    });
});
