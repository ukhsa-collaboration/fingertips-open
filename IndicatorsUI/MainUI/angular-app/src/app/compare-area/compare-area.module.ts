import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LegendModule } from '../shared/component/legend/legend.module';
import { CompareAreaComponent } from './compare-area.component';
import { CompareAreaTableComponent } from './compare-area-table/compare-area-table.component';
import { CompareAreaChartComponent } from './compare-area-chart/compare-area-chart.component';
import { ExportCsvModule } from '../shared/component/export-csv/export-csv.module';

@NgModule({
    imports: [
        CommonModule,
        LegendModule,
        ExportCsvModule
    ],
    declarations: [
        CompareAreaComponent,
        CompareAreaTableComponent,
        CompareAreaChartComponent
    ],
    exports: [
        CompareAreaComponent
    ]
})

export class CompareAreaModule { }
