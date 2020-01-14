import { Component, HostListener } from '@angular/core';
import { forkJoin } from 'rxjs';
import { BoxplotData, BoxplotDataForTable } from './boxplot';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { IndicatorHeader } from '../shared/component/indicator-header/indicator-header';
import { GroupRoot, IndicatorMetadata, IndicatorStats } from '../typings/FT';
import { TrendSourceHelper, ComparatorHelper, AreaHelper } from '../shared/shared';

@Component({
  selector: 'ft-boxplot',
  templateUrl: './boxplot.component.html',
  styleUrls: ['./boxplot.component.css']
})
export class BoxplotComponent {

  public header: IndicatorHeader;
  public boxplotData: BoxplotData;
  public boxplotDataForTable: BoxplotDataForTable[] = [];
  public isEnglandAreaSelected = true;

  isAnyData = true;
  indicatorMetadata: IndicatorMetadata;
  groupRoot: GroupRoot;
  isNearestNeighbour = false;
  boxPlotChart: any;

  constructor(private ftHelperService: FTHelperService, private indicatorService: IndicatorService) { }

  @HostListener('window:BoxplotSelected', ['$event', '$event.detail.isEnglandAreaType'])
  public onOutsideEvent(event, isEnglandAreaSelected): void {

    const ftHelper = this.ftHelperService;
    const model = ftHelper.getFTModel();
    this.isNearestNeighbour = model.isNearestNeighbours();

    this.groupRoot = ftHelper.getCurrentGroupRoot();
    this.isEnglandAreaSelected = isEnglandAreaSelected;
    this.isAnyData = true;

    // Get data
    const groupRoot = this.groupRoot;
    const comparatorCode = new ComparatorHelper(ftHelper).getCurrentComparatorCode();
    const metadataObservable = this.indicatorService.getIndicatorMetadata(model.groupId);
    const statsObservable = this.indicatorService.getIndicatorStatisticsTrendsForSingleIndicator(
      groupRoot.IID, groupRoot.Sex.Id, groupRoot.Age.Id, model.areaTypeId, comparatorCode, model.profileId);

    forkJoin([metadataObservable, statsObservable]).subscribe(results => {
      const metadataHash: Map<number, IndicatorMetadata> = results[0];
      const statsArray: IndicatorStats[] = results[1];

      this.indicatorMetadata = metadataHash[groupRoot.IID];

      this.displayBoxplot(statsArray, groupRoot);

      ftHelper.showAndHidePageElements();
      ftHelper.unlock();
    });
  }

  @HostListener('window:NoDataDisplayed', ['$event', '$event.detail.isEnglandAreaType'])
  public refershVariables(event, isEnglandAreaSelected): void {
    this.isAnyData = false;
    this.isEnglandAreaSelected = isEnglandAreaSelected;
  }

  displayBoxplot(statsArray: IndicatorStats[], groupRoot: GroupRoot) {
    let hasNegativeStats = false;

    // Define header
    this.displayHeader();

    // Define data
    const comparatorHelper = new ComparatorHelper(this.ftHelperService);
    const comparatorName = comparatorHelper.getCurrentComparatorName();
    const comparatorCode = comparatorHelper.getCurrentComparatorCode();

    const indicatorName = this.ftHelperService.getIndicatorName(this.indicatorMetadata.IID) +
      this.ftHelperService.getSexAndAgeLabel(groupRoot);

    const areaTypeName = this.ftHelperService.getAreaTypeName();
    const areaCode = this.ftHelperService.getFTModel().areaCode;
    const areaName = this.ftHelperService.getArea(areaCode).Name;
    const data = new BoxplotData(this.indicatorMetadata, indicatorName, areaTypeName, areaName,
      comparatorName, comparatorCode, this.isNearestNeighbour);
    for (let i = 0; i < statsArray.length; i++) {
      const indicatorStats = statsArray[i];
      if (indicatorStats.Stats) {
        data.addStats(indicatorStats);

        // Check whether any of the stats values are negative
        // Helps to decide whether y axis can start with zero
        const stats = indicatorStats.Stats;
        hasNegativeStats = stats.Min < 0;
      }
    }

    // Define whether y axis can start with zero
    const canStartZeroYAxis = this.ftHelperService.getFTConfig().startZeroYAxis;
    data.setMin(canStartZeroYAxis && !hasNegativeStats);

    this.formatBoxplotDataForTable(data);

    this.boxplotData = data;
  }

  formatBoxplotDataForTable(data: BoxplotData) {

    const unitLabel = this.ftHelperService.newValueSuffix(this.indicatorMetadata.Unit).getShortLabel();
    const trendSource = new TrendSourceHelper(this.indicatorMetadata).getTrendSource();

    const periodsCount = data.periods.length;
    const rows = [];
    for (let i = 0; i < periodsCount; i++) {
      const row = new BoxplotDataForTable();
      row.period = data.periods[i];
      row.statsFormatted = data.statsFormatted[i];
      row.unitLabel = unitLabel;
      row.trendSource = trendSource;
      rows.push(row);
    }

    this.boxplotDataForTable = rows;
  }

  displayHeader(): void {

    // Define unit label
    const metadata = this.indicatorMetadata;
    let unitLabel = metadata.Unit.Label;
    if (unitLabel !== '') {
      unitLabel = ' - ' + unitLabel;
    }

    // Has data changed
    const groupRoot = this.groupRoot;
    const hasDataChangedRecently = groupRoot.DateChanges && groupRoot.DateChanges.HasDataChangedRecently;

    const comparatorName = new ComparatorHelper(this.ftHelperService).getCurrentComparatorName();

    this.header = new IndicatorHeader(metadata.Descriptive['Name'], hasDataChangedRecently,
      comparatorName, metadata.ValueType.Name, unitLabel, this.ftHelperService.getSexAndAgeLabel(groupRoot));
  }

  onExportClick(event: MouseEvent) {
    this.boxPlotChart.exportChart({ type: 'image/png' }, {});
  }

  setBoxPlotChart(event) {
    this.boxPlotChart = event.chart;
  }
}
