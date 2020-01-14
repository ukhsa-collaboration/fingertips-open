import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CompareIndicatorComponent } from './compare-indicator.component';
import { SharedModule } from '../shared/shared.module';
import { ScatterPlotChartComponent } from './scatter-plot-chart/scatter-plot-chart.component';
import { FormsModule } from '@angular/forms';
import { ExportCsvModule } from '../shared/component/export-csv/export-csv.module';
import { IndicatorDropdownModule } from '../shared/component/indicator-dropdown/indicator-dropdown.module';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        FormsModule,
        ExportCsvModule,
        IndicatorDropdownModule
    ],
    declarations: [
        CompareIndicatorComponent,
        ScatterPlotChartComponent
    ],
    exports: [
        CompareIndicatorComponent
    ]
})

export class CompareIndicatorModule { }
