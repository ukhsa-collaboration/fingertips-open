import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LightBoxComponent } from './light-box.component';

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [
        LightBoxComponent
    ],
    exports: [
        LightBoxComponent
    ]
})

export class LightBoxModule { }
