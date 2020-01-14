import { Component, OnChanges, SimpleChanges, Input, Output, EventEmitter } from '@angular/core';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { AreaRow, SortBy, DisplayOption } from '../compare-area';
import { TooltipHelper, NearestNeighbourHelper } from '../../shared/shared';
import { isDefined } from '@angular/compiler/src/util';
import { forkJoin } from 'rxjs';
import { AreaService } from '../../shared/service/api/area.service';
import * as _ from 'underscore';

@Component({
  selector: 'ft-compare-area-table',
  templateUrl: './compare-area-table.component.html',
  styleUrls: ['./compare-area-table.component.css']
})
export class CompareAreaTableComponent implements OnChanges {

  @Input() comparatorAreaRows: AreaRow[];
  @Input() nonComparatorAreaRows: AreaRow[];
  @Input() areaTypeId: number;
  @Input() areaCodeSelectedOnChart: string;
  @Input() comparatorId: number;
  @Input() displayOption: DisplayOption;
  @Input() tooltipHelper: TooltipHelper;
  @Input() trendSource: string;
  @Input() showRecentTrends: boolean;
  @Output() emitSortBy = new EventEmitter();

  selectedAreaCode: string;
  currentTrendsTooltipHtml: string;
  currentAreasTooltipHtml: string;
  columnHeaderLowerCI: string;
  columnHeaderUpperCI: string;

  constructor(private ftHelperService: FTHelperService,
    private areaService: AreaService) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['areaCodeSelectedOnChart']) {
      if (this.areaCodeSelectedOnChart) {
        this.selectedAreaCode = this.areaCodeSelectedOnChart;
      }
    }

    this.setSigLevelColumnHeader();
  }

  setSigLevelColumnHeader(): void {
    const root = this.ftHelperService.getCurrentGroupRoot();
    const sigLevel = root.Grouping[0].SigLevel;
    if (sigLevel > 0) {
      this.columnHeaderLowerCI = sigLevel + '%<br>Lower CI';
      this.columnHeaderUpperCI = sigLevel + '%<br> Upper CI';
    } else {
      this.columnHeaderLowerCI = '95%<br>Lower CI';
      this.columnHeaderUpperCI = '95%<br> Upper CI';
    }
  }

  barAreaSelected(areaCode: string) {
    const model = this.ftHelperService.getFTModel();

    model.areaCode = areaCode;

    if (this.ftHelperService.isNearestNeighbours()) {
      model.nearestNeighbour = new NearestNeighbourHelper(this.ftHelperService).getNearestNeighbourCode();
    }

    const profileId = model.profileId;
    const areaTypeId = model.areaTypeId;
    const parentAreaTypeId = model.parentTypeId;

    const parentToChildAreasObservable = this.areaService.getParentToChildAreas(profileId, areaTypeId, parentAreaTypeId, model.nearestNeighbour);

    forkJoin([parentToChildAreasObservable]).subscribe(results => {

      const areaMappings = results[0];

      // Set area mappings
      this.ftHelperService.setAreaMappings(areaMappings);

      this.ftHelperService.setAreaCode(areaCode);
      this.ftHelperService.goToAreaProfilePage();
    });
  }

  highlightRow(areaRow: AreaRow) {
    this.selectedAreaCode = areaRow.areaCode;
  }

  unHighlightRow() {
    this.selectedAreaCode = '';
  }

  sortByArea() {
    this.emitSortBy.emit(SortBy.Area);
  }

  sortByRank() {
    this.emitSortBy.emit(SortBy.Rank);
  }

  sortByCount() {
    this.emitSortBy.emit(SortBy.Count);
  }

  sortByValue() {
    this.emitSortBy.emit(SortBy.Value);
  }

  isNearestNeighbourSelected(): boolean {
    const model = this.ftHelperService.getFTModel();
    return model.isNearestNeighbours();
  }

  showDataTable(): boolean {
    let comparatorAreaRowsLength = 0;
    let nonComparatorAreaRowsLength = 0;

    if (isDefined(this.comparatorAreaRows)) {
      comparatorAreaRowsLength = this.comparatorAreaRows.length;
    }

    if (isDefined(this.nonComparatorAreaRows)) {
      nonComparatorAreaRowsLength = this.nonComparatorAreaRows.length;
    }

    return comparatorAreaRowsLength + nonComparatorAreaRowsLength > 0 ? true : false;
  }

  public showRecentTrendTooltip(event: MouseEvent, row: AreaRow) {
    const tooltipProvider = this.ftHelperService.newRecentTrendsTooltip();
    this.tooltipHelper.displayHtml(event, row.trendMarkerTooltip);
  }

  public showAreaNameTooltip(event: MouseEvent, row: AreaRow) {
    this.currentAreasTooltipHtml = row.areaName;
    this.tooltipHelper.displayHtml(event, this.currentAreasTooltipHtml);
  }

  public showValueNoteTooltip(event: MouseEvent, row: AreaRow) {
    if (row.valueNoteId > 0) {
      const valueNoteTooltipProvider = this.ftHelperService.newValueNoteTooltipProvider();
      this.currentTrendsTooltipHtml = valueNoteTooltipProvider.getHtmlFromNoteId(row.valueNoteId);
      this.tooltipHelper.displayHtml(event, this.currentTrendsTooltipHtml);
    }
  }

  public hideTooltip() {
    this.tooltipHelper.hide();
    this.currentTrendsTooltipHtml = null;
  }

  public goToTrends(row: AreaRow) {
    const ftHelper = this.ftHelperService;
    ftHelper.recentTrendSelected().byAreaAndRootIndex(row.areaCode, ftHelper.getIndicatorIndex());
  }

  public displayRecentTrends() {
    return this.showRecentTrends;
  }
}
