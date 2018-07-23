import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IndicatorHeaderComponent } from './component/indicator-header/indicator-header.component';

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [
        IndicatorHeaderComponent
    ],
    exports: [
        IndicatorHeaderComponent
    ]
})

export class SharedModule { }
