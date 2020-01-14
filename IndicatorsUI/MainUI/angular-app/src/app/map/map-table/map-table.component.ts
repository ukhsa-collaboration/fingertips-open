import {
  Component, Input, Output, SimpleChanges, OnChanges,
  EventEmitter, ChangeDetectorRef, ViewChild, ElementRef
} from '@angular/core';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import * as _ from 'underscore';
import { CoreDataSet, TooltipManager, Area, ValueNoteTooltipProvider, ValueDisplayer, CoreDataSetInfo } from '../../typings/FT';
import { isDefined } from '@angular/compiler/src/util';
import { CommaNumber } from '../../shared/shared';
declare let $: JQueryStatic;

@Component({
  selector: 'ft-map-table',
  templateUrl: './map-table.component.html',
  styleUrls: ['./map-table.component.css']
})
export class MapTableComponent implements OnChanges {
  @ViewChild('maptable', { static: true }) el: ElementRef;
  @Input() sortedCoreData: Map<string, CoreDataSet> = null;
  @Input() areaTypeId: number = null;
  @Input() selectedAreaList;
  @Input() areaCodeColour = null;
  @Input() isBoundaryNotSupported;
  @Output() hoverAreaCodeChanged = new EventEmitter();
  @Output() selectedAreaChanged = new EventEmitter();
  tooltip: TooltipManager;
  selectedCoreData: {
    val: string, count: any, countNum: number, upCI: string, loCI: string,
    isNote: boolean, noteId: number, areaName: string, areaCode: string, colour: string
  }[] = [];
  currentAreaList: Array<Area> = [];

  constructor(private ftHelperService: FTHelperService, private ref: ChangeDetectorRef) {
  }

  ngOnChanges(changes: SimpleChanges) {

    if (changes['isBoundaryNotSupported']) {
      const localBoundryNotSupported = changes['isBoundaryNotSupported'].currentValue;
      if (isDefined(localBoundryNotSupported)) {
        if (localBoundryNotSupported) {
          this.selectedCoreData = [];
        }
      }
    }
    if (changes['areaTypeId'] || changes['selectedAreaList'] || changes['areaCodeColour']) {
      this.loadData();
    }
  }

  onMouseEnter(event: MouseEvent): void {
    const noteId = event.fromElement.attributes.getNamedItem('data-NoteId');

    event.fromElement.attributes

    if (noteId) {
      const tooltipPrvdr: ValueNoteTooltipProvider = this.ftHelperService.newValueNoteTooltipProvider();
      const html = tooltipPrvdr.getHtmlFromNoteId(noteId.value);
      this.tooltip = this.ftHelperService.newTooltipManager();
      this.tooltip.setHtml(html);
      this.tooltip.positionXY(event.pageX + 10, event.pageY + 15);
      this.tooltip.showOnly();
    }
  }

  onMouseMove(event: MouseEvent): void {
    if (isDefined(this.tooltip)) {
      this.tooltip.positionXY(event.pageX + 10, event.pageY + 15);
    }
  }

  onMouseLeave(event: MouseEvent): void {
    if (isDefined(this.tooltip)) {
      this.tooltip.hide();
    }
  }

  onRowMouseOver(event: MouseEvent): void {
    const areaCode = this.getAreaCode(event);
    this.hoverAreaCodeChanged.emit({ areaCode: areaCode });
    const row = this.getRow(event.toElement);

    this.clearRowHovers();
    $(row).addClass('rowHover');
  }

  onRowMouseLeave(event: MouseEvent): void {
    this.clearRowHovers();
  }

  onRowClick(event: MouseEvent): void {
    const areaCode = this.getAreaCode(event);
    if (areaCode !== null) {
      // Remove core data
      for (let i = 0; i < this.selectedCoreData.length; i++) {
        if (this.selectedCoreData[i].areaCode === areaCode) {
          this.selectedCoreData.splice(i, 1);
        }
      }
      this.selectedCoreData = this.selectedCoreData.slice();
      this.selectedAreaChanged.emit({ areaCode: areaCode });
      this.ref.detectChanges();
      this.clearRowHovers();
    }
  }

  getRow(element: Element) {
    while (element.tagName !== 'TR') {
      element = element.parentElement;
    }
    return element;
  }

  clearRowHovers(): void {
    $('.rowHover').removeClass('rowHover');
  }

  getAreaCode(event: MouseEvent): string {
    const areaCodeProp = 'areaCode';
    let areaCode = event.toElement.attributes.getNamedItem(areaCodeProp);
    if (areaCode === null) {
      areaCode = event.toElement.parentElement.attributes.getNamedItem(areaCodeProp);
      if (areaCode === null) {
        areaCode = event.toElement.parentElement.parentElement.attributes.getNamedItem(areaCodeProp);
      }
    }
    return areaCode != null ? areaCode.value : null;
  }

  loadData() {

    const newData = [];

    this.selectedAreaList.forEach(areaCode => {

      const valueDisplayer: ValueDisplayer = this.ftHelperService.newValueDisplayer(null);
      const coreDataSet: CoreDataSet = this.sortedCoreData[areaCode];
      const coreDatasetInfo: CoreDataSetInfo = this.ftHelperService.newCoreDataSetInfo(coreDataSet);

      // Set up data view model
      const areaName = this.ftHelperService.getAreaName(areaCode);
      const isNote: boolean = coreDatasetInfo.isNote();
      const noteId: number = coreDatasetInfo.getNoteId();
      const formattedCount = coreDatasetInfo.isCount() ? new CommaNumber(coreDataSet.Count).rounded() : '-';
      const formattedValue: string = valueDisplayer.byNumberString(coreDataSet.ValF);
      const loCI = valueDisplayer.byNumberString(coreDataSet.LoCIF);
      const upCI = valueDisplayer.byNumberString(coreDataSet.UpCIF);
      const colour = this.areaCodeColour ? this.areaCodeColour.get(areaCode) : '#B0B0B2';

      newData.push({
        areaName: areaName, areaCode: areaCode,
        isNote: isNote,
        val: coreDataSet.Val,
        formattedValue: formattedValue,
        count: coreDataSet.Count,
        formattedCount: formattedCount,
        loCI: loCI, upCI: upCI,
        noteId: noteId, colour: colour
      });
    });

    this.selectedCoreData = newData;
  }

  sortByCount() {
    this.sort('count');
  }

  sortByValue() {
    this.sort('val');
  }

  sortByAreaName() {
    this.sort('areaName');
  }

  sort(prop) {
    this.selectedCoreData = _.sortBy(this.selectedCoreData, prop);
    this.ref.detectChanges();
  }

  loadAreaList() {
    this.currentAreaList = [];
    this.currentAreaList = this.ftHelperService.getAreaList();
  }
}
