import { Component, Input, SimpleChanges, OnChanges, ViewChild, ElementRef } from '@angular/core';
import { isDefined } from '@angular/compiler/src/util';
import {
  AllPopulationData, RegisteredPersons
} from '../population';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import {
  FTModel, Population, ValueData
} from '../../typings/FT';
import { CommaNumber, NumberHelper } from '../../shared/shared';
import { AreaTypeIds, IndicatorIds, ProfileIds } from '../../shared/constants';
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
  @ViewChild('metadata', { static: false }) metadata: MetadataTableComponent;

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
      const areaName = this.ftHelperService.getAreaName(this.ftModel.areaCode);

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
      const metadata = this.metadata;
      // Set timeout so metadata table view will have finished rendering
      setTimeout(function () { metadata.showInLightbox(); });
    }
  }

  public shouldDisplay(): boolean {
    return this.ftModel.areaTypeId === AreaTypeIds.Practice;
  }

  private defineDeprivationDecile() {

    // Decile label
    const decileNumber = this.allPopulationData.populationSummary.GpDeprivationDecile;
    if (isDefined(decileNumber)) {
      const deciles = this.allPopulationData.deprivationDeciles;
      this.decileLabel = _.find(deciles, function (decile) { return decile.Id === decileNumber; }).Name;
    } else {
      this.decileLabel = '<i>Data not available for current practice</i>';
    }

    // Decile blocks
    const decileLabels: DecileLabel[] = [];
    for (let i = 1; i <= 10; i++) {
      decileLabels.push(new DecileLabel(i));
    }

    decileLabels[decileNumber - 1].text = '<div class="decile decile' +
      decileNumber + '" >' + decileNumber + '</div>';

    this.decileLabels = decileLabels;
  }

  private defineAdHocIndicatorRows() {

    const rows = [];
    const values = this.allPopulationData.populationSummary.AdHocValues;

    // QOF achievement
    const qof = values.Qof;
    let text = isDefined(qof)
      ? NumberHelper.roundNumber(qof.Count, 1) + ' (out of ' + qof.Denom + ')'
      : this.noData;
    rows.push(new AdHocIndicatorRow(IndicatorIds.QofPoints, 'QOF achievement', text));

    // Life expectancy
    rows.push(this.getLifeExpectancy('Male', values.LifeExpectancyMale));
    rows.push(this.getLifeExpectancy('Female', values.LifeExpectancyFemale));

    // Patients that recommend practice
    const recommend = values.Recommend;
    text = isDefined(recommend)
      ? recommend.ValF + '%'
      : this.noData;

    rows.push(new AdHocIndicatorRow(IndicatorIds.WouldRecommendPractice,
      '% having a positive experience of their practice', text));

    this.adHocIndicatorRows = rows;
  }

  private getLifeExpectancy(sex: string, data: ValueData): AdHocIndicatorRow {

    const text = isDefined(data)
      ? data.ValF + ' years'
      : this.noData;

    return new AdHocIndicatorRow(IndicatorIds.LifeExpectancy, 'Life expectancy (' + sex + ')', text);
  }

  private defineEthnicityLabel() {
    const ethnicity = this.allPopulationData.populationSummary.Ethnicity;
    this.ethnicityLabel = isDefined(ethnicity)
      ? ethnicity
      : '<i>Insufficient data to provide accurate summary</i>';
  }

  private defineRegisteredPersons(areaName: string) {
    const populations = this.allPopulationData.populations;
    const parentAreaName = this.ftHelperService.getParentArea().Name;

    this.registeredPersons = [
      this.getRegisteredPersons(areaName, false, populations.areaPopulation),
      this.getRegisteredPersons(parentAreaName, true, populations.areaParentPopulation),
      this.getRegisteredPersons('ENGLAND', true, populations.nationalPopulation)
    ];
  }

  private getRegisteredPersons(areaName: string, isAverage: boolean, population: Population): RegisteredPersons {
    const persons = new RegisteredPersons();
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
  public text = '';
  constructor(public num: number) { }
  public getClass(): string {
    return 'decile' + this.num;
  }
}
