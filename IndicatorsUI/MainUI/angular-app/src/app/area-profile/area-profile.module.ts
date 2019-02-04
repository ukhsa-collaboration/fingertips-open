import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AreaProfileComponent } from './area-profile.component';
import { SpineChartComponent } from './spine-chart/spine-chart.component';
import { LegendModule } from 'app/shared/component/legend/legend.module';

@NgModule({
  imports: [
    CommonModule,
    LegendModule
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
