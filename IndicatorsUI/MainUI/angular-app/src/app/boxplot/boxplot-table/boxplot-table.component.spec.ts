import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BoxplotTableComponent } from './boxplot-table.component';

describe('BoxplotTableComponent', () => {
    let component: BoxplotTableComponent;
    let fixture: ComponentFixture<BoxplotTableComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [BoxplotTableComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(BoxplotTableComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should be created', () => {
        expect(component).toBeTruthy();
    });
});
