import { Component, Input, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { Area } from '../../../../typings/FT.d';
import { AreaService } from '../../../service/api/area.service';
import { AreaListService } from '../../../../shared/service/api/arealist.service';

@Component({
  selector: 'ft-arealist-areas',
  templateUrl: './arealist-areas.component.html',
  styleUrls: ['./arealist-areas.component.css']
})
export class ArealistAreasComponent implements OnChanges {

  @Input() areaTypeId;
  @Input() selectedAreas;
  @Output() emitSelectedArea = new EventEmitter();
  @Output() emitDeSelectedArea = new EventEmitter();
  @Output() emitMouseOverArea = new EventEmitter();
  @Output() emitMouseOutArea = new EventEmitter();
  @Output() emitAreas = new EventEmitter();
  areas: Area[] = [];

  constructor(private arealistService: AreaListService, private areaService: AreaService) {
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['areaTypeId']) {
      if (this.areaTypeId) {
        this.searchForAreas();
      }
    }
  }

  searchForAreas() {
    this.areaService.getAreas(this.areaTypeId)
      .subscribe((result: any) => {
        this.areas = <Area[]>result;
        this.emitAreas.emit(this.areas);
      });
  }

  mouseOverArea(area) {
    this.emitMouseOverArea.emit(area);
  }

  mouseOutArea(area) {
    this.emitMouseOutArea.emit(area);
  }

  selectArea(item) {
    const area = this.areas.find(x => x.Code === item);
    const areaInSelectedAreas = this.selectedAreas.find(x => x.Code === item);
    if (areaInSelectedAreas === undefined) {
      this.emitSelectedArea.emit(area);
    } else {
      this.emitDeSelectedArea.emit(area);
    }
  }
}
