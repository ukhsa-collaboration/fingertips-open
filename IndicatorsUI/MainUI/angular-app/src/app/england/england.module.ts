import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EnglandComponent } from './england.component';
import { LegendModule } from '../shared/component/legend/legend.module';
import { ExportCsvModule } from '../shared/component/export-csv/export-csv.module';

@NgModule({
    imports: [
        CommonModule,
        LegendModule,
        ExportCsvModule
    ],
    declarations: [
        EnglandComponent
    ],
    exports: [
        EnglandComponent
    ]
})

export class EnglandModule { }
