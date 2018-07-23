import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { BoxplotComponent } from './boxplot.component';
import { BoxplotChartComponent } from './boxplot-chart/boxplot-chart.component';
import { BoxplotTableComponent } from './boxplot-table/boxplot-table.component';

@NgModule({
    imports: [
        CommonModule,
        SharedModule
    ],
    declarations: [
        BoxplotComponent,
        BoxplotChartComponent,
        BoxplotTableComponent
    ],
    exports: [
        BoxplotComponent
    ]
})
export class BoxplotModule { }