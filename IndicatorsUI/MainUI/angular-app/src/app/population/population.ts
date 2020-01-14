import * as _ from 'underscore';
import { SexIds } from '../shared/constants';
import { Population, PopulationSummary, Category } from '../typings/FT';
import { isDefined } from '@angular/compiler/src/util';

export class Populations {
    public nationalPopulation: Population;
    public areaParentPopulation: Population;
    public areaPopulation: Population;
}

export class PopulationTableData {
    public ageBand: string;
    public maleNational: string;
    public femaleNational: string;
    public maleAreaParent: string;
    public femaleAreaParent: string;
    public maleArea: string;
    public femaleArea: string;
}

export class AllPopulationData {
    constructor(public populations: Populations,
        public populationSummary: PopulationSummary,
        public deprivationDeciles: Category[]) { }
}

export class RegisteredPersons {
    areaName: string;
    personCount: string;
    isAverage: boolean;
    public hasPersonCount(): boolean {
        return this.personCount !== null;
    }
}

export class PopulationModifier {

    constructor(private populations: Populations) { }

    /** Make male population percentages -ve so they are displayed on the opposite side of the axis
     * to female values
     */
    public makeMalePopulationsNegative() {
        this.makeMaleValuesNegative(this.populations.areaPopulation);
        this.makeMaleValuesNegative(this.populations.areaParentPopulation);
        this.makeMaleValuesNegative(this.populations.nationalPopulation);
    }

    private makeMaleValuesNegative(population: Population) {
        const values = population.Values;
        if (isDefined(values[SexIds.Male]) || isDefined(SexIds.Female)) {
            const maleValues = values[SexIds.Male];
            for (const i in maleValues) {
                if (maleValues.hasOwnProperty(i)) {
                    maleValues[i] = -Math.abs(maleValues[i]);
                }
            }
        }
    }
}

export class PopulationMaxFinder {

    /** Finds the maximum population value to enable equal axis limits to be
     * set on both the male and female sides
     */
    getMaximumValue(populations: Populations): number {
        return this.getPopulationMax([
            populations.areaPopulation,
            populations.areaParentPopulation,
            populations.nationalPopulation
        ]);
    }

    private getPopulationMax(populations: Population[]): number {
        let max = 5;
        let min = -max;
        for (const i in populations) {
            if (populations[i] != null) {
                min = _.min([min, _.min(populations[i].Values[SexIds.Male])]);
                max = _.max([max, _.max(populations[i].Values[SexIds.Female])]);
            }
        }
        return Math.ceil(_.max([max, -min]));
    }
}
