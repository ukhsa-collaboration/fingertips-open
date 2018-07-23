import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisteredPersonsTableComponent } from './registered-persons-table.component';

describe('RegisteredPersonsTableComponent', () => {
    let component: RegisteredPersonsTableComponent;
    let fixture: ComponentFixture<RegisteredPersonsTableComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [RegisteredPersonsTableComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(RegisteredPersonsTableComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should be created', () => {
        expect(component).toBeTruthy();
    });
});
