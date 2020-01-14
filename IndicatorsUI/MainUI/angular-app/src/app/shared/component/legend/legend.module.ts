import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LegendComponent } from './legend.component';
import { LegendRecentTrendsComponent } from './legend-recent-trends/legend-recent-trends.component';
import { LegendMapComponent } from './legend-map/legend-map.component';
import { LegendTrendComponent } from './legend-trend/legend-trend.component';
import { LegendCompareAreasComponent } from './legend-compare-areas/legend-compare-areas.component';
import { LegendInequalitiesComponent } from './legend-inequalities/legend-inequalities.component';
import { LegendDataQualityComponent } from './legend-data-quality/legend-data-quality.component';
import { LegendBenchmarkAgainstGoalComponent } from './legend-benchmark-against-goal/legend-benchmark-against-goal.component';

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [
        LegendComponent,
        LegendRecentTrendsComponent,
        LegendMapComponent,
        LegendTrendComponent,
        LegendCompareAreasComponent,
        LegendInequalitiesComponent,
        LegendDataQualityComponent,
        LegendBenchmarkAgainstGoalComponent
    ],
    exports: [
        LegendComponent
    ]
})

export class LegendModule { }
