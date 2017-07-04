import { Component, OnInit,HostListener } from '@angular/core';
import {FTModel, FTRoot} from '../typings/FT.d';
import {FTHelperService} from '../shared/service/helper/ftHelper.service';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',  styleUrls: ['./map.component.css']
})
export class MapComponent implements OnInit {
  isInitialised: boolean = false;
  map: google.maps.Map
  areaTypeId: number = null;
  constructor(private ftHelperService: FTHelperService) { }
  ngOnInit() { }

  @HostListener('window:MapEnvReady-Event', ['$event'])
  public onOutsideEvent(event) {
    this.isInitialised = true;
    this.areaTypeId = this.ftHelperService.getFTModel().areaTypeId;
    console.log(`The user just pressed ${event.detail}!`);
  }

  onMapInit(mapInfo: {map: google.maps.Map}) {
    this.map = mapInfo.map;
  }
}