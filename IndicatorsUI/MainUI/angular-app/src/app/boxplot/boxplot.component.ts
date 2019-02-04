import { Component, HostListener } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import {
  FTModel, FTRoot, GroupRoot, IndicatorMetadata, IndicatorStatsPercentilesFormatted,
  IndicatorStats, TooltipManager, IndicatorStatsPercentiles
} from '../typings/FT.d';
import { BoxplotData } from './boxplot';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { IndicatorHeader } from '../shared/component/indicator-header/indicator-header.component';
import { AreaTypeIds } from 'app/shared/shared';

@Component({
  selector: 'ft-boxplot',
  templateUrl: './boxplot.component.html',
  styleUrls: ['./boxplot.component.css']
})
export class BoxplotComponent {

  public header: IndicatorHeader;
  public boxplotData: BoxplotData;
  public isAvailable = true;

  constructor(private ftHelperService: FTHelperService, private indicatorService: IndicatorService) { }

  @HostListener('window:BoxplotSelected', ['$event'])
  public onOutsideEvent(event): void {

    let ftHelper = this.ftHelperService;

    let groupRoot: GroupRoot = this.ftHelperService.getCurrentGroupRoot();
    let model = ftHelper.getFTModel();

    this.isAvailable = model.areaTypeId !== AreaTypeIds.Country;

    // Get data
    let metadataObservable = this.indicatorService.getIndicatorMetadata(model.groupId);
    let statsObservable = this.indicatorService.getIndicatorStatisticsTrendsForSingleIndicator(
      groupRoot.IID, groupRoot.Sex.Id, groupRoot.Age.Id, model.areaTypeId,
      this.ftHelperService.getCurrentComparator().Code);

    Observable.forkJoin([metadataObservable, statsObservable]).subscribe(results => {
      let metadataHash: Map<number, IndicatorMetadata> = results[0];
      let statsArray: IndicatorStats[] = results[1];

      this.displayBoxplot(metadataHash[groupRoot.IID], groupRoot, statsArray);

      this.ftHelperService.showAndHidePageElements();
      this.ftHelperService.unlock();
    });
  }

  displayBoxplot(metadata: IndicatorMetadata, groupRoot: GroupRoot, statsArray: IndicatorStats[]) {

    // Define header
    this.displayHeader(metadata, groupRoot);

    // Define data
    let data = new BoxplotData(metadata, this.ftHelperService.getAreaTypeName(),
      this.ftHelperService.getCurrentComparator().Name);
    for (let i = 0; i < statsArray.length; i++) {
      let indicatorStats = statsArray[i];
      if (indicatorStats.Stats) {
        data.addStats(indicatorStats);
      }
    }
    this.boxplotData = data;
  }

  displayHeader(metadata: IndicatorMetadata, groupRoot: GroupRoot): void {
    let unitLabel = metadata.Unit.Label;
    if (unitLabel !== '') {
      unitLabel = ' - ' + unitLabel;
    }

    let hasDataChangedRecently = groupRoot.DateChanges && groupRoot.DateChanges.HasDataChangedRecently;

    this.header = new IndicatorHeader(metadata.Descriptive['Name'], hasDataChangedRecently,
      this.ftHelperService.getCurrentComparator().Name,
      metadata.ValueType.Name, unitLabel, this.ftHelperService.getSexAndAgeLabel(groupRoot));
  }

  public isAnyData(): boolean {
    return this.boxplotData && this.boxplotData.isAnyData();
  }

  onExportClick(event: MouseEvent) {
    event.preventDefault();
    let boxplotTable = $('.boxplot-table').hide();
    let chart = $('#indicator-details-boxplot-data');
    $('.highcharts-credits,.highcharts-contextbutton').hide();
    this.ftHelperService.saveElementAsImage(chart, 'boxplot');
    $(boxplotTable).show();
    this.ftHelperService.logEvent('ExportImage', 'Boxplot');
  }

}
