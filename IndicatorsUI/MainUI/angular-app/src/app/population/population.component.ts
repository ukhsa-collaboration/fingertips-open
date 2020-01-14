import { Component, HostListener } from '@angular/core';
import { forkJoin } from 'rxjs';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { AreaTypeIds, CategoryTypeIds } from '../shared/constants';
import { AreaCodes } from '../shared/constants';
import { Populations, PopulationModifier, AllPopulationData } from './population';
import { Population, Category, PopulationSummary, FTModel } from '../typings/FT';

@Component({
  selector: 'ft-population',
  templateUrl: './population.component.html',
  styleUrls: ['./population.component.css']
})
export class PopulationComponent {

  public populations: Populations = null;
  public allPopulationData: AllPopulationData = null;
  public showSummary = false;

  constructor(private ftHelperService: FTHelperService, private indicatorService: IndicatorService) { }

  @HostListener('window:PopulationSelected', ['$event'])
  public onOutsideEvent(event): void {

    const model = this.ftHelperService.getFTModel();
    this.showSummary = model.areaTypeId === AreaTypeIds.Practice;

    // Get populations
    const observables = this.getPopulationObservables(model);

    // Get summary data
    if (this.showSummary) {
      observables.push(
        this.indicatorService.getPopulationSummary(model.areaCode, model.areaTypeId),
        this.indicatorService.getCategories(CategoryTypeIds.DeprivationDecileGp2015)
      );
    }

    forkJoin(observables).subscribe(results => {

      // Init populations
      const populations = this.setPopulations(results);
      new PopulationModifier(populations).makeMalePopulationsNegative();
      this.populations = populations;

      // Summary
      if (this.showSummary) {
        const deprivationDeciles = <Category[]>results[4];
        const populationSummary = <PopulationSummary>results[3];
        this.allPopulationData = new AllPopulationData(this.populations,
          populationSummary, deprivationDeciles);
      }

      this.ftHelperService.showAndHidePageElements();
      this.ftHelperService.unlock();
    });
  }

  public isAnyData(): boolean {
    return this.populations &&
      this.populations.nationalPopulation.IndicatorName !== null;
  }

  private getPopulationObservables(model: FTModel): any[] {

    const areaTypeId = model.areaTypeId;

    const nationalPopulation = this.indicatorService.getPopulation(AreaCodes.England, areaTypeId);
    const areaPopulation = this.indicatorService.getPopulation(model.areaCode, areaTypeId);

    const parentCode = model.isNearestNeighbours() ? model.nearestNeighbour : model.parentCode;
    const subnationalPopulation = this.indicatorService.getPopulation(parentCode, areaTypeId);

    return [nationalPopulation, subnationalPopulation, areaPopulation];
  }

  private setPopulations(results: any[]): Populations {
    const populations = new Populations();

    populations.nationalPopulation = <Population>results[0];
    populations.areaParentPopulation = <Population>results[1];
    populations.areaPopulation = <Population>results[2];

    return populations;
  }
}



