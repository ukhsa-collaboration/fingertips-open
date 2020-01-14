import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GoogleMapComponent } from './google-map/google-map.component';
import { MapComponent } from './map.component';
import { GoogleMapService } from './googleMap.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { AreaService } from '../shared/service/api/area.service';
import { MapChartComponent } from './map-chart/map-chart.component';
import { MapTableComponent } from './map-table/map-table.component';
import { PracticeSearchComponent } from './practice-search/practice-search.component';
import { TypeaheadModule } from 'ngx-bootstrap';
import { FormsModule } from '@angular/forms';
import { CoreDataHelperService } from '../shared/service/helper/coreDataHelper.service';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { LegendModule } from '../shared/component/legend/legend.module';
import { DeviceDetectorService } from 'ngx-device-detector';
import { LightBoxModule } from '../shared/component/light-box/light-box.module';
import { ExportCsvModule } from '../shared/component/export-csv/export-csv.module';
import { AgmCoreModule } from '@agm/core';

@NgModule({
  imports: [
    CommonModule,
    TypeaheadModule.forRoot(),
    FormsModule,
    LegendModule,
    LightBoxModule,
    ExportCsvModule,
    AgmCoreModule.forRoot({
      apiKey: 'AIzaSyCyj06EYBaz66Rgi_wTeSmqI1_qBZxtuos'
    })
  ],
  declarations: [
    GoogleMapComponent,
    MapComponent,
    MapChartComponent,
    MapTableComponent,
    PracticeSearchComponent
  ],
  exports: [
    GoogleMapComponent,
    MapComponent
  ],
  providers: [
    GoogleMapService,
    IndicatorService,
    AreaService,
    CoreDataHelperService,
    FTHelperService,
    DeviceDetectorService
  ]
})
export class MapModule { }
