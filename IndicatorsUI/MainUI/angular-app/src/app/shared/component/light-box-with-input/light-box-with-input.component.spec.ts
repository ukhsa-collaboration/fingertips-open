import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LightBoxWithInputComponent } from './light-box-with-input.component';

describe('LightBoxWithInputComponent', () => {
    let component: LightBoxWithInputComponent;
    let fixture: ComponentFixture<LightBoxWithInputComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [LightBoxWithInputComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(LightBoxWithInputComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
