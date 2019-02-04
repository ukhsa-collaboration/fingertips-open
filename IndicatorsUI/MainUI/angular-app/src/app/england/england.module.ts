import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EnglandComponent } from './england.component';
import { LegendModule } from 'app/shared/component/legend/legend.module';

@NgModule({
    imports: [
        CommonModule,
        LegendModule
    ],
    declarations: [
        EnglandComponent
    ],
    exports: [
        EnglandComponent
    ]
})

export class EnglandModule { }
