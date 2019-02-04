import { Component, HostListener } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import {
  FTModel, FTRoot, GroupRoot, Population, Category, PopulationSummary
} from '../typings/FT.d';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { AreaCodes, AreaTypeIds, CategoryTypeIds } from '../shared/shared';
import { Populations, PopulationModifier, AllPopulationData } from './population';

@Component({
  selector: 'ft-population',
  templateUrl: './population.component.html',
  styleUrls: ['./population.component.css']
})
export class PopulationComponent {

  populations: Populations = null;
  allPopulationData: AllPopulationData;
  showSummary: boolean;

  constructor(private ftHelperService: FTHelperService, private indicatorService: IndicatorService) { }

  @HostListener('window:PopulationSelected', ['$event'])
  public onOutsideEvent(event): void {

    let model = this.ftHelperService.getFTModel();
    this.showSummary = model.areaTypeId === AreaTypeIds.Practice;

    // Get populations
    const observables = [];
    observables.push(
      this.indicatorService.getPopulation(model.areaCode, model.areaTypeId),
      this.indicatorService.getPopulation(model.parentCode, model.areaTypeId),
      this.indicatorService.getPopulation(AreaCodes.England, model.areaTypeId)
    );

    // Get summary data
    if (this.showSummary) {
      observables.push(
        this.indicatorService.getPopulationSummary(model.areaCode, model.areaTypeId),
        this.indicatorService.getCategories(CategoryTypeIds.DeprivationDecileGp2015)
      );
    }

    Observable.forkJoin(observables).subscribe(results => {

      // Init populations
      let populations = new Populations();
      populations.areaPopulation = <Population>results[0];
      populations.areaParentPopulation = <Population>results[1];
      populations.nationalPopulation = <Population>results[2];
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
}



