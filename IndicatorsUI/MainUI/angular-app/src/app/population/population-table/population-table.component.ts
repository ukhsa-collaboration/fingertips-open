import { Component, Input, SimpleChanges, OnChanges, ViewChild } from '@angular/core';
import { Populations, PopulationTableData } from '../population';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { isDefined } from '@angular/compiler/src/util';
import { AreaHelper, ParentAreaHelper, CommaNumber, TooltipHelper } from '../../shared/shared';
import { SexIds, IndicatorIds, ProfileIds } from '../../shared/constants';
import { MetadataTableComponent } from '../../metadata/metadata-table/metadata-table.component';

@Component({
  selector: 'ft-population-table',
  templateUrl: './population-table.component.html',
  styleUrls: ['./population-table.component.css']
})
export class PopulationTableComponent implements OnChanges {

  @Input() populations: Populations;
  @ViewChild('metadata', { static: true }) metadata: MetadataTableComponent;

  public populationTableData: PopulationTableData[] = [];
  public areaName: string;
  public parentAreaName: string;
  public indicatorName: string;
  public period: string;

  public sumOfMaleAreaPopulation: string;
  public sumOfFemaleAreaPopulation: string;
  public sumOfMaleAreaParentPopulation: string;
  public sumOfFemaleAreaParentPopulation: string;
  public sumOfMaleNationalPopulation: string;
  public sumOfFemaleNationalPopulation: string;

  tooltipHelper: TooltipHelper;
  currentTooltipHtml = null;

  constructor(private ftHelperService: FTHelperService) {
    this.tooltipHelper = new TooltipHelper(this.ftHelperService.newTooltipManager());
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.isAnyData) {
      const model = this.ftHelperService.getFTModel();
      const area = this.ftHelperService.getArea(model.areaCode);

      this.indicatorName = this.populations.nationalPopulation.IndicatorName;
      this.period = this.populations.nationalPopulation.Period;
      this.areaName = new AreaHelper(area).getAreaNameToDisplay();

      if (AreaHelper.isAreaList(model.parentCode)) {
        this.parentAreaName = new ParentAreaHelper(this.ftHelperService).getParentAreaNameWithFirstLetterUpperCase();
      } else {
        this.parentAreaName = new ParentAreaHelper(this.ftHelperService).getParentAreaName();
      }

      this.populationTableData = [];

      this.generatePopulationTableData();
    }
  }

  public showTooltip(event, data, sexId: number, areaId: number) {

    if (!this.currentTooltipHtml) {

      const sex = sexId === SexIds.Male ? 'Male' : 'Female';

      const area = areaId === 1 ? this.areaName :
        areaId === 2 ? this.parentAreaName + ' (average)' : 'England (average)';

      const age = data === null ? 'All ages' : data.ageBand;

      const value = $(event.target).html();

      this.currentTooltipHtml = '<b>' + sex + ' ' + age + '</b><br>' + value + '<br><span id="tooltipArea">' + area + '</span>';
    }
    this.tooltipHelper.displayHtml(event, this.currentTooltipHtml);
  }

  public hideTooltip() {
    this.tooltipHelper.hide();
    this.currentTooltipHtml = null;
  }

  public showQuinaryPopulationMetadata() {
    this.showMetadata(IndicatorIds.QuinaryAgeBands);
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

  private isAnyData(): boolean {
    return isDefined(this.populations);
  }

  private generatePopulationTableData() {

    const areaPopulation = this.populations.areaPopulation;
    const areaParentPopulation = this.populations.areaParentPopulation;
    const nationalPopulation = this.populations.nationalPopulation;

    let sumOfMaleAreaPopulation = 0;
    let sumOfFemaleAreaPopulation = 0;
    let sumOfMaleAreaParentPopulation = 0;
    let sumOfFemaleAreaParentPopulation = 0;
    let sumOfMaleNationalPopulation = 0;
    let sumOfFemaleNationalPopulation = 0;

    for (let i = 0; i < areaPopulation.Labels.length; i++) {
      const populationTableData = new PopulationTableData();
      populationTableData.ageBand = areaPopulation.Labels[i];

      populationTableData.maleArea = new CommaNumber(areaPopulation.PopulationTotals[1][i]).rounded();
      populationTableData.femaleArea = new CommaNumber(areaPopulation.PopulationTotals[2][i]).rounded();

      populationTableData.maleAreaParent = new CommaNumber(areaParentPopulation.PopulationTotals[1][i]).rounded();
      populationTableData.femaleAreaParent = new CommaNumber(areaParentPopulation.PopulationTotals[2][i]).rounded();

      populationTableData.maleNational = new CommaNumber(nationalPopulation.PopulationTotals[1][i]).rounded();
      populationTableData.femaleNational = new CommaNumber(nationalPopulation.PopulationTotals[2][i]).rounded();

      this.populationTableData.push(populationTableData);

      sumOfMaleAreaPopulation += areaPopulation.PopulationTotals[1][i];
      sumOfFemaleAreaPopulation += areaPopulation.PopulationTotals[2][i];

      sumOfMaleAreaParentPopulation += areaParentPopulation.PopulationTotals[1][i];
      sumOfFemaleAreaParentPopulation += areaParentPopulation.PopulationTotals[2][i];

      sumOfMaleNationalPopulation += nationalPopulation.PopulationTotals[1][i];
      sumOfFemaleNationalPopulation += nationalPopulation.PopulationTotals[2][i];
    }

    this.sumOfMaleAreaPopulation = new CommaNumber(sumOfMaleAreaPopulation).rounded();
    this.sumOfFemaleAreaPopulation = new CommaNumber(sumOfFemaleAreaPopulation).rounded();

    this.sumOfMaleAreaParentPopulation = new CommaNumber(sumOfMaleAreaParentPopulation).rounded();
    this.sumOfFemaleAreaParentPopulation = new CommaNumber(sumOfFemaleAreaParentPopulation).rounded();

    this.sumOfMaleNationalPopulation = new CommaNumber(sumOfMaleNationalPopulation).rounded();
    this.sumOfFemaleNationalPopulation = new CommaNumber(sumOfFemaleNationalPopulation).rounded();
  }
}
