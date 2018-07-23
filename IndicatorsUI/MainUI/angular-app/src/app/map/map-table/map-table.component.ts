import {
  Component, OnInit, Input, Output, SimpleChanges, OnChanges,
  EventEmitter, DoCheck, KeyValueDiffers, ChangeDetectorRef, ChangeDetectionStrategy, AfterViewChecked
  , ViewChild, ElementRef, AfterViewInit
} from '@angular/core';
import {
  FTModel, FTRoot, Area, GroupRoot, CoreDataSet, CoreDataHelper, Unit, ValueWithUnit, ValueNote, ValueDisplayer,
  IndicatorMetadataHash, IndicatorMetadata, CoreDataSetInfo,
  ValueNoteTooltipProvider, TooltipManager
} from '../../typings/FT.d';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import * as _ from 'underscore';
declare var $: JQueryStatic;

@Component({
  selector: 'ft-map-table',
  templateUrl: './map-table.component.html',
  styleUrls: ['./map-table.component.css']
})
export class MapTableComponent {
  @ViewChild('maptable') el: ElementRef;
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
      let localBoundryNotSupported = changes['isBoundaryNotSupported'].currentValue;
      if (localBoundryNotSupported !== undefined) {
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
    let noteId = event.srcElement.attributes.getNamedItem('data-NoteId');
    if (noteId) {
      let tooltipPrvdr: ValueNoteTooltipProvider = this.ftHelperService.newValueNoteTooltipProvider();
      let html = tooltipPrvdr.getHtmlFromNoteId(noteId.value);
      this.tooltip = this.ftHelperService.newTooltipManager();
      this.tooltip.setHtml(html);
      this.tooltip.positionXY(event.pageX + 10, event.pageY + 15);
      this.tooltip.showOnly();
    }
  }

  onMouseMove(event: MouseEvent): void {
    if (!_.isUndefined(this.tooltip)) {
      this.tooltip.positionXY(event.pageX + 10, event.pageY + 15);
    }
  }

  onMouseLeave(event: MouseEvent): void {
    if (!_.isUndefined(this.tooltip)) {
      this.tooltip.hide();
    }
  }

  onRowMouseOver(event: MouseEvent): void {
    let areaCode = this.getAreaCode(event);
    this.hoverAreaCodeChanged.emit({ areaCode: areaCode });
    let row = this.getRow(event.srcElement);
    this.clearRowHovers();
    $(row).addClass('rowHover');
  }

  onRowMouseLeave(event: MouseEvent): void {
    this.clearRowHovers();
  }

  onRowClick(event: MouseEvent): void {
    let areaCode = this.getAreaCode(event);
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
    let areaCodeProp = 'areaCode';
    let areaCode = event.srcElement.attributes.getNamedItem(areaCodeProp);
    if (areaCode === null) {
      areaCode = event.srcElement.parentElement.attributes.getNamedItem(areaCodeProp);
      if (areaCode === null) {
        areaCode = event.srcElement.parentElement.parentElement.attributes.getNamedItem(areaCodeProp);
      }
    }
    return areaCode != null ? areaCode.value : null;
  }

  loadData() {

    let newData = [];

    this.selectedAreaList.forEach(areaCode => {

      let valueDisplayer: ValueDisplayer = this.ftHelperService.newValueDisplayer(null);
      let coreDataSet: CoreDataSet = this.sortedCoreData[areaCode];
      let coreDatasetInfo: CoreDataSetInfo = this.ftHelperService.newCoreDataSetInfo(coreDataSet);

      // Set up data view model
      let areaName = this.ftHelperService.getAreaName(areaCode);
      let isNote: boolean = coreDatasetInfo.isNote();
      let noteId: number = coreDatasetInfo.getNoteId();
      let formattedCount = coreDatasetInfo.isCount() ? this.ftHelperService.newCommaNumber(coreDataSet.Count).rounded() : '-';
      let formattedValue: string = valueDisplayer.byNumberString(coreDataSet.ValF);
      let loCI = valueDisplayer.byNumberString(coreDataSet.LoCIF);
      let upCI = valueDisplayer.byNumberString(coreDataSet.UpCIF);
      let colour = this.areaCodeColour ? this.areaCodeColour.get(areaCode) : '#B0B0B2';

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
