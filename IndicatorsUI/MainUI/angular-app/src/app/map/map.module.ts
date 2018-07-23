import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GoogleMapComponent } from './google-map/google-map.component';
import { MapComponent } from './map.component';
import { GoogleMapService } from './googleMap.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { AreaService } from '../shared/service/api/area.service';
import { MapChartComponent } from './map-chart/map-chart.component';
import { ScriptLoaderService } from '../shared/service/helper/script.service';
import { MapTableComponent } from './map-table/map-table.component';
import { PracticeSearchComponent } from './practice-search/practice-search.component';
import { TypeaheadModule } from 'ngx-bootstrap';
import { FormsModule } from '@angular/forms';
import { CoreDataHelperService } from '../shared/service/helper/coreDataHelper.service';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { MapLegendComponent } from './map-legend/map-legend.component';

@NgModule({
  imports: [
    CommonModule,
    TypeaheadModule.forRoot(),
    FormsModule
  ],
  declarations: [
    GoogleMapComponent,
    MapComponent,
    MapChartComponent,
    MapTableComponent,
    PracticeSearchComponent,
    MapLegendComponent
  ],
  exports: [
    GoogleMapComponent,
    MapComponent
  ],
  providers: [
    GoogleMapService,
    IndicatorService,
    ScriptLoaderService,
    AreaService,
    CoreDataHelperService,
    FTHelperService
  ]
})
export class MapModule { }