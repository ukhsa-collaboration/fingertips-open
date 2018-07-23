import { Population, PopulationSummary, Category } from '../typings/FT.d';
import { AreaCodes, SexIds } from '../shared/shared';
import * as _ from 'underscore';

export class Populations {
    public areaPopulation: Population;
    public areaParentPopulation: Population;
    public nationalPopulation: Population;
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
        let values = population.Values;
        if (!_.isUndefined(values[SexIds.Male]) || !_.isUndefined(SexIds.Female)) {
            let maleValues = values[SexIds.Male];
            for (var i in maleValues) {
                maleValues[i] = -Math.abs(maleValues[i]);
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
        var max = 5;
        var min = -max;
        for (var i in populations) {
            if (populations[i] != null) {
                min = _.min([min, _.min(populations[i].Values[SexIds.Male])]);
                max = _.max([max, _.max(populations[i].Values[SexIds.Female])]);
            }
        }
        return Math.ceil(_.max([max, -min]));
    }
}