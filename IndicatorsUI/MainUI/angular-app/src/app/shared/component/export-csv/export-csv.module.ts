import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExportCsvComponent } from './export-csv.component';

@NgModule({
    imports: [
        CommonModule,
    ],
    declarations: [
        ExportCsvComponent
    ],
    exports: [
        ExportCsvComponent
    ]
})

export class ExportCsvModule { }
