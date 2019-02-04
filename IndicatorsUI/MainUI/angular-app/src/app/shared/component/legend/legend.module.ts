import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LegendBobComponent } from './legend-bob/legend-bob.component';
import { LegendContinuousComponent } from './legend-continuous/legend-continuous.component';
import { LegendQuartilesComponent } from './legend-quartiles/legend-quartiles.component';
import { LegendQuintilesComponent } from './legend-quintiles/legend-quintiles.component';
import { LegendRag3Component } from './legend-rag-3/legend-rag-3.component';
import { LegendRag5Component } from './legend-rag-5/legend-rag-5.component';
import { LegendComponent } from './legend.component';
import { LegendRecentTrendsComponent } from './legend-recent-trends/legend-recent-trends.component';
import { LegendAreaProfilesComponent } from './legend-area-profiles/legend-area-profiles.component';
import { LegendMapComponent } from './legend-map/legend-map.component';

@NgModule({
    imports: [
        CommonModule,
    ],
    declarations: [
        LegendComponent,
        LegendBobComponent,
        LegendContinuousComponent,
        LegendQuartilesComponent,
        LegendQuintilesComponent,
        LegendRag3Component,
        LegendRag5Component,
        LegendRecentTrendsComponent,
        LegendAreaProfilesComponent,
        LegendMapComponent
    ],
    exports: [
        LegendComponent
    ]
})

export class LegendModule { }
