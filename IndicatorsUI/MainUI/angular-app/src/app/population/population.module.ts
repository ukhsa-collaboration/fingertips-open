import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { PopulationComponent } from './population.component';
import { PopulationChartComponent } from './population-chart/population-chart.component';
import { PopulationSummaryComponent } from './population-summary/population-summary.component';
import { RegisteredPersonsTableComponent } from './registered-persons-table/registered-persons-table.component';
import { MetadataModule } from '../metadata/metadata.module';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        MetadataModule
    ],
    declarations: [
        PopulationComponent,
        PopulationChartComponent,
        PopulationSummaryComponent,
        RegisteredPersonsTableComponent
    ],
    exports: [
        PopulationComponent
    ]
})
export class PopulationModule { }