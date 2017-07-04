import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GoogleMapComponent} from './googleMap.component';
import {MapComponent} from './map.component';
import {GoogleMapService} from './googleMap.service';
import {IndicatorService} from '../shared/service/api/indicator.service';
import {HelperService} from '../shared/service/helper/helper.service';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    GoogleMapComponent,
    MapComponent
  ],
  exports: [
    GoogleMapComponent,
    MapComponent
  ],
  providers: [
    GoogleMapService,
    IndicatorService,
    HelperService,
  ]
})
export class MapModule { }