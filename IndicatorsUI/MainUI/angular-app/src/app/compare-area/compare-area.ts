import { PolarityIds, AreaCodes } from '../shared/constants';
import { ComparisonConfig, CoreDataSet, Area, Unit, TrendMarkerResult } from '../typings/FT';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import * as _ from 'underscore';
import { isDefined } from '@angular/compiler/src/util';

export class AreaPopulation {
    areaCode: string;
    population: number;
}

export class AreaRow {
    data: CoreDataSet;
    areaName: string;
    areaCode: string;
    comparatorId: number;
    trendMarkerTooltip: string;
    trendMarkerResult: TrendMarkerResult;
    recentTrendImage: string;
    count: string;
    value: string;
    rank: string;
    rankSortIndex: number;
    barchartOptions: BarChartOptions;
    loCI: string;
    upCI: string;
    barImage: string;
    barImageWidth: number;
    barImageLeft: number;
    verticalLineImage1: string;
    verticalLineImage1Left: number;
    verticalLineImage2: string;
    verticalLineImage2Left: number;
    horizontalLineImage: string;
    horizontalLineImageWidth: number;
    horizontalLineImageLeft: number;
    isValue: boolean;
    valueNoteId: number;
}

export class BarChartOptions {
    barImage: string;
    barImageWidth: number;
    barImageLeft: number;
    verticalLineImage1: string;
    verticalLineImage1Left: number;
    verticalLineImage2: string;
    verticalLineImage2Left: number;
    horizontalLineImage: string;
    horizontalLineImageWidth: number;
    horizontalLineImageLeft: number;
}

export class BarScale {
    private data: CoreDataSet[];
    private pixelsPerUnit: number;
    private negativePixels: number;

    constructor(private allData: CoreDataSet[], private ftHelperService: FTHelperService) {
        this.data = allData;
    }

    calculateRangeAndPixels() {
        const width = 240;
        const buffer = 1.05;
        const values = this.getValidValues('Val');
        const lowerCIs = this.getValidValues('LoCI');
        const upperCIs = this.getValidValues('UpCI');
        const mmVal = new MinMaxFinder(values);
        const mmLo = new MinMaxFinder(lowerCIs);
        const mmUp = new MinMaxFinder(upperCIs);

        // Max
        let max = !mmUp.isValid || mmVal.max > mmUp.max ? mmVal.max : mmUp.max;

        // Adjust max
        if (max < 0) {
            // Start bars from zero
            max = 0;
        } else {
            // Add buffer to max
            max = max * buffer;
        }

        // Min
        let min = !mmLo.isValid || mmVal.min < mmLo.min ? mmVal.min : mmLo.min;

        // Adjust min
        if (min > 0) {
            min = 0;
        }

        const range = max - min;
        this.pixelsPerUnit = width / range;
        this.negativePixels = min < 0 ? Math.floor(-min * this.pixelsPerUnit) : 0;
    }

    getValidValues(property: string): any[] {
        const validCoreData = _.filter(this.data, function (data) {
            return isDefined(data) && data[property] !== -1;
        });

        return _.pluck(validCoreData, property);
    }

    getBarChartOptions(data: CoreDataSet, comparisonConfig: ComparisonConfig): BarChartOptions {
        const dataInfo = this.ftHelperService.newCoreDataSetInfo(data);
        if (dataInfo.areValueAndCIsZero()) {
            return new BarChartOptions();
        }

        const imageUrl = this.ftHelperService.getURL().img;
        const significance = Number(data.Sig[comparisonConfig.comparatorId]);
        let image = '';

        // For England colour the bars only if benchmarked against the goal
        const isEngland = data.AreaCode === AreaCodes.England;
        if (!isEngland || (isEngland && comparisonConfig.useTarget)) {
            image = this.getSignificanceImage(significance, comparisonConfig.useRagColours, comparisonConfig.useQuintileColouring);
        }

        if (!image) {
            image = 'lightGrey.png';
        }

        let width = Number(this.pixelsPerUnit) * data.Val;
        width = Math.abs(width);

        const left = data.Val < 0 ? this.negativePixels - width : this.negativePixels;

        const barChartOptions = new BarChartOptions();
        barChartOptions.barImage = imageUrl + image;
        barChartOptions.barImageWidth = width;
        barChartOptions.barImageLeft = left;

        barChartOptions.verticalLineImage1Left = 0;
        barChartOptions.horizontalLineImageLeft = 0;
        barChartOptions.horizontalLineImageWidth = 0;
        barChartOptions.verticalLineImage2Left = 0;

        if (data.UpCIF) {
            // Add the error bars
            const root = this.ftHelperService.getCurrentGroupRoot();
            const sigLevel = root.Grouping[0].SigLevel;

            let lower = 0;
            let upper = 0;
            if (sigLevel > 0) {
                lower = sigLevel === 99.8 ? this.getErrorBarPixelStart(data.LoCI99_8) : this.getErrorBarPixelStart(data.LoCI);
                upper = sigLevel === 99.8 ? this.getErrorBarPixelStart(data.UpCI99_8) : this.getErrorBarPixelStart(data.UpCI);
            } else {
                lower = this.getErrorBarPixelStart(data.LoCI);
                upper = this.getErrorBarPixelStart(data.UpCI);
            }

            const pixelSpan = upper - lower - 2;

            barChartOptions.verticalLineImage1 = imageUrl + 'black.png';
            barChartOptions.verticalLineImage1Left = lower;

            if (pixelSpan > 0) {
                barChartOptions.horizontalLineImage = imageUrl + 'black.png';
                barChartOptions.horizontalLineImageWidth = pixelSpan;
                barChartOptions.horizontalLineImageLeft = lower + 1;
            }

            barChartOptions.verticalLineImage2 = imageUrl + 'black.png';
            barChartOptions.verticalLineImage2Left = upper - 1;
        }

        return barChartOptions;
    }

    getErrorBarPixelStart(cI): number {
        const pixels = cI * this.pixelsPerUnit;
        let pixelsInt = Math.round(pixels);

        if (pixels < pixelsInt) {
            pixelsInt -= 1;
        }

        return pixelsInt + this.negativePixels;
    }

    getSignificanceImage(sig: number, useRagColours: boolean, useQuintileColouring: boolean) {
        if (useQuintileColouring) {
            if (sig > 0 && sig < 6) {
                const groupRoot = this.ftHelperService.getCurrentGroupRoot();
                if (groupRoot.PolarityId === PolarityIds.NotApplicable) {
                    return 'bob-quintile-' + sig + '.png';
                } else {
                    return 'rag-quintile-' + sig + '.png';
                }
            }
        } else {
            if (useRagColours) {
                // Order should match order in PholioVisualisation.PholioObjects.Significance
                switch (sig) {
                    case 1:
                        return 'red.png';
                    case 2:
                        return 'same.png';
                    case 3:
                        return 'better.png';
                    case 4:
                        return 'worst.png';
                    case 5:
                        return 'best.png';
                }
            } else {
                switch (sig) {
                    case 1:
                        return 'bobLower.png';
                    case 2:
                        return 'same.png';
                    case 3:
                        return 'bobHigher.png';
                    case 4:
                        return 'bobLowest.png';
                    case 5:
                        return 'bobHighest.png';
                }
            }
        }

        return null;
    }
}

export class DisplayOption {
    public static readonly None = 0;
    public static readonly Table = 1;
    public static readonly TableAndChart = 2;
}

export class FunnelPlotChartData {
    better: any[];
    worse: any[];
    same: any[];
    none: any[];
    colourBetter: string;
    colourWorse: string;
    currentComparatorName: string;
    isComparatorValue: boolean;
    isDsr: boolean;
    showLimits: boolean;
    unit: Unit;
    currentData: CoreDataSet[];
    areas: Area[];
    areaPopulations: AreaPopulation[];
    comparatorLineData: number[][];
    u2LineData: number[][];
    u3LineData: number[][];
    l2LineData: number[][];
    l3LineData: number[][];
    indicatorName: string;
    timePeriod: string;
    areaTypeName: string;
    parentAreaName: string;
}

export class Limit {
    x: number;
    L2: number;
    U2: number;
    L3: number;
    U3: number;
}

export class MinMaxFinder {
    min: number;
    max: number;
    isValid: boolean;

    constructor(private numbersArray: number[]) {
        this.isValid = numbersArray.length > 0;

        if (this.isValid) {
            this.min = _.min(numbersArray);
            this.max = _.max(numbersArray);
        } else {
            this.min = 0;
            this.max = 0;
        }
    }
}

export class SortBy {
    public static readonly None = 0;
    public static readonly Area = 1;
    public static readonly Rank = 2;
    public static readonly Count = 3;
    public static readonly Value = 4;
}

export class SortState {

    private orderState: {};

    constructor() {
        // Init order states
        this.orderState = {};
        this.orderState[SortBy.Area.toString()] = true;
        this.orderState[SortBy.Rank.toString()] = false;
        this.orderState[SortBy.Count.toString()] = false;
        this.orderState[SortBy.Value.toString()] = false;
    }

    public isOrderAscending(sortBy: SortBy): boolean {
        return this.orderState[sortBy.toString()];
    }

    public reverseOrderIfSameSort(sortByA: SortBy, sortByB: SortBy) {
        if (sortByA === sortByB) {
            // Reverse order
            const key = sortByA.toString();
            this.orderState[key] = !this.orderState[key];
        }
    }
}

export class SpcForDsrLimits {
    ComparatorValue: number;
    Points: Limit[];
}

export class TrendMarkerArea {
    areaCode: string;
    marker: number;
    markerForMostRecentValueComparedWithPreviousValue: number;
    message: string;
    pointsUsed: number;
}
