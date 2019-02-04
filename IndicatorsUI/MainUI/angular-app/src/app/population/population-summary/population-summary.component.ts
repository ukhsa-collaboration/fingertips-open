import { Component, Input, SimpleChanges, OnChanges, ViewChild, ElementRef } from '@angular/core';
import {
  AllPopulationData, RegisteredPersons
} from '../population';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import {
  FTModel, Population, ValueData
} from '../../typings/FT.d';
import { CommaNumber, AreaTypeIds, NumberHelper, IndicatorIds, ProfileIds } from '../../shared/shared';
import * as _ from 'underscore';
import { MetadataTableComponent } from '../../metadata/metadata-table/metadata-table.component';

@Component({
  selector: 'ft-population-summary',
  templateUrl: './population-summary.component.html',
  styleUrls: ['./population-summary.component.css']
})
export class PopulationSummaryComponent implements OnChanges {

  private noData = '<div class="no-data">-</div>';

  @Input() allPopulationData: AllPopulationData;
  @ViewChild('metadata') metadata: MetadataTableComponent;

  registeredPersons: RegisteredPersons[];
  ethnicityLabel: string;
  practiceLabel: string;
  decileLabel: string;
  decileLabels: DecileLabel[];
  adHocIndicatorRows: AdHocIndicatorRow[];
  ftModel: FTModel;

  constructor(private ftHelperService: FTHelperService) {
    this.ftModel = this.ftHelperService.getFTModel();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.allPopulationData) {
      let areaName = this.ftHelperService.getAreaName(this.ftModel.areaCode);

      this.defineRegisteredPersons(areaName);
      this.practiceLabel = this.ftModel.areaCode + ' - ' + areaName;
      this.defineAdHocIndicatorRows();
      this.defineDeprivationDecile();
      this.defineEthnicityLabel();
    }
  }

  public showDeprivationMetadata() {
    this.showMetadata(IndicatorIds.DeprivationScore);
  }

  public showEthnicityMetadata() {
    this.showMetadata(IndicatorIds.EthnicityEstimates);
  }

  public showMetadata(indicatorId: number) {
    this.metadata.displayMetadataForIndicator(indicatorId, [ProfileIds.PracticeProfileSupportingIndicators]);
  }

  public metadataIsReady(isReady: boolean) {
    if (isReady) {
      let metadata = this.metadata;
      // Set timeout so metadata table view will have finished rendering
      setTimeout(function () { metadata.showInLightbox(); });
    }
  }

  public shouldDisplay(): boolean {
    return this.ftModel.areaTypeId === AreaTypeIds.Practice;
  }

  private defineDeprivationDecile() {

    // Decile label
    let decileNumber = this.allPopulationData.populationSummary.GpDeprivationDecile;
    if (_.isUndefined(decileNumber)) {
      this.decileLabel = '<i>Data not available for current practice</i>';
    } else {
      let deciles = this.allPopulationData.deprivationDeciles;
      this.decileLabel = _.find(deciles, function (decile) { return decile.Id === decileNumber; }).Name;
    }

    // Decile blocks
    let decileLabels: DecileLabel[] = [];
    for (let i = 1; i <= 10; i++) {
      decileLabels.push(new DecileLabel(i));
    }

    decileLabels[decileNumber - 1].text = '<div class="decile decile' +
      decileNumber + '" >' + decileNumber + '</div>';

    this.decileLabels = decileLabels;
  }

  private defineAdHocIndicatorRows() {

    let rows = [];
    let values = this.allPopulationData.populationSummary.AdHocValues;

    // QOF achievement
    let qof = values.Qof;
    let text = _.isUndefined(qof)
      ? this.noData
      : NumberHelper.roundNumber(qof.Count, 1) + ' (out of ' + qof.Denom + ')';
    rows.push(new AdHocIndicatorRow(IndicatorIds.QofPoints, 'QOF achievement', text));

    // Life expectancy
    rows.push(this.getLifeExpectancy('Male', values.LifeExpectancyMale));
    rows.push(this.getLifeExpectancy('Female', values.LifeExpectancyFemale));

    // Patients that recommend practice
    let recommend = values.Recommend;
    text = _.isUndefined(recommend) ?
      this.noData
      : recommend.ValF + '%';
    rows.push(new AdHocIndicatorRow(IndicatorIds.WouldRecommendPractice,
      '% having a positive experience of their practice', text));

    this.adHocIndicatorRows = rows;
  }

  private getLifeExpectancy(sex: string, data: ValueData): AdHocIndicatorRow {
    let text = _.isUndefined(data)
      ? this.noData
      : data.ValF + ' years';
    return new AdHocIndicatorRow(IndicatorIds.LifeExpectancy, 'Life expectancy (' + sex + ')', text);
  }

  private defineEthnicityLabel() {
    let ethnicity = this.allPopulationData.populationSummary.Ethnicity;
    this.ethnicityLabel = _.isUndefined(ethnicity)
      ? '<i>Insufficient data to provide accurate summary</i>'
      : ethnicity;
  }

  private defineRegisteredPersons(areaName: string) {
    let populations = this.allPopulationData.populations;
    let parentAreaName = this.ftHelperService.getParentArea().Name;

    this.registeredPersons = [
      this.getRegisteredPersons(areaName, false, populations.areaPopulation),
      this.getRegisteredPersons(parentAreaName, true, populations.areaParentPopulation),
      this.getRegisteredPersons('ENGLAND', true, populations.nationalPopulation)
    ];
  }

  private getRegisteredPersons(areaName: string, isAverage: boolean, population: Population): RegisteredPersons {
    let persons = new RegisteredPersons();
    persons.isAverage = isAverage;
    persons.areaName = areaName;
    persons.personCount = this.formatListSize(population.ListSize);
    return persons;
  }

  private formatListSize(listSize: ValueData): string {
    return listSize && listSize.Val > 0
      ? new CommaNumber(listSize.Val).rounded()
      : null;
  }
}

class AdHocIndicatorRow {
  constructor(public indicatorId: number,
    public indicatorName: string,
    public valueText: string) { }
}

class DecileLabel {
  constructor(public num: number) { }
  public text = '';
  public getClass(): string {
    return 'decile' + this.num;
  }
}
