import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LegendModule } from '../shared/component/legend/legend.module';
import { TrendComponent } from './trend.component';
import { TrendTableComponent } from './trend-table/trend-table.component';
import { TrendChartComponent } from './trend-chart/trend-chart.component';
import { ExportCsvModule } from '../shared/component/export-csv/export-csv.module';

@NgModule({
    imports: [
        CommonModule,
        LegendModule,
        ExportCsvModule
    ],
    declarations: [
        TrendComponent,
        TrendTableComponent,
        TrendChartComponent
    ],
    exports: [
        TrendComponent
    ]
})

export class TrendModule { }
