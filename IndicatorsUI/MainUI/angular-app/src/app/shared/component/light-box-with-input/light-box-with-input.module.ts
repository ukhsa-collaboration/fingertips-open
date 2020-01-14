import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LightBoxWithInputComponent } from './light-box-with-input.component';

@NgModule({
    imports: [
        CommonModule,
    ],
    declarations: [
        LightBoxWithInputComponent
    ],
    exports: [
        LightBoxWithInputComponent
    ]
})

export class LightBoxWithInputModule { }
