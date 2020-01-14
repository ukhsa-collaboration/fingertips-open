import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LegendModule } from '../shared/component/legend/legend.module';
import { InequalitiesComponent } from './inequalities.component';
import { InequalitiesBarChartComponent } from './inequalities-bar-chart/inequalities-bar-chart.component';
import { InequalitiesTrendChartComponent } from './inequalities-trend-chart/inequalities-trend-chart.component';
import { InequalitiesTrendTableComponent } from './inequalities-trend-table/inequalities-trend-table.component';
import { LightBoxModule } from '../shared/component/light-box/light-box.module';
import { ExportCsvModule } from '../shared/component/export-csv/export-csv.module';

@NgModule({
    imports: [
        CommonModule,
        LegendModule,
        LightBoxModule,
        ExportCsvModule
    ],
    declarations: [
        InequalitiesComponent,
        InequalitiesBarChartComponent,
        InequalitiesTrendChartComponent,
        InequalitiesTrendTableComponent
    ],
    exports: [
        InequalitiesComponent
    ]
})

export class InequalitiesModule { }
