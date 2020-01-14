import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AreaProfileComponent } from './area-profile.component';
import { SpineChartComponent } from './spine-chart/spine-chart.component';
import { LegendModule } from '../shared/component/legend/legend.module';
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
    AreaProfileComponent,
    SpineChartComponent,
  ],
  exports: [
    AreaProfileComponent,
    SpineChartComponent
  ]
})

export class AreaProfileModule { }
