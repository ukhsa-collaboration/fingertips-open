import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PracticeSearchComponent } from './practice-search.component';

describe('PracticeSearchComponent', () => {
    let component: PracticeSearchComponent;
    let fixture: ComponentFixture<PracticeSearchComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [PracticeSearchComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(PracticeSearchComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    // it('should create', () => {
    //     expect(component).toBeTruthy();
    // });
});
