import { IndicatorStatsPercentiles, CoreDataSet, CoreDataSetInfo, ComparisonConfig } from '../typings/FT';
import { PolarityIds } from 'app/shared/constants';

export class SpineChartCalculator {

    // Not 1 for float precision reasons
    private one = 0.9999;

    public getSpineProportions(average: number, stats: IndicatorStatsPercentiles, polarityId: number): SpineChartProportions {

        let min: number, max: number, p25: number, p75: number;

        const prop = new SpineChartProportions();
        prop.isHighestLeft = this.isHighestLeft(polarityId)

        if (prop.isHighestLeft) {
            min = stats.Max;
            max = stats.Min;
            p25 = stats.P75;
            p75 = stats.P25;
        } else {
            min = stats.Min;
            max = stats.Max;
            p25 = stats.P25;
            p75 = stats.P75;
        }

        // Q1 Offset
        const minAverageDifference = Math.abs(average - min);
        const maxAverageDifference = Math.abs(average - max);
        if (minAverageDifference > maxAverageDifference) {
            prop.unitsOfLargestSide = minAverageDifference;
            prop.q1Offset = 0;
        } else {
            prop.unitsOfLargestSide = maxAverageDifference;
            prop.q1Offset = (maxAverageDifference - minAverageDifference);
        }

        prop.q1 = Math.abs(p25 - min);
        prop.q2 = Math.abs(p75 - p25);
        prop.q4 = Math.abs(max - p75);
        prop.min = min;

        return prop;
    }

    public getDimensions(proportions: SpineChartProportions, dataInfo: CoreDataSetInfo, imgUrl: string,
        comparisonConfig: ComparisonConfig, indicatorId: number, getMarkerImageFromSignificance: Function): SpineChartDimensions {

        const dimensions = new SpineChartDimensions();
        dimensions.imgUrl = imgUrl;

        const calc = new SpineChartDimensionCalculator(proportions.unitsOfLargestSide);

        dimensions.q1 = calc.getDimension(proportions.q1);
        dimensions.q2 = calc.getDimension(proportions.q2);
        dimensions.q4 = calc.getDimension(proportions.q4);

        dimensions.q1Offset = calc.getDimension(proportions.q1Offset);
        dimensions.q1Offset += 12/*margin left*/;

        dimensions.pixelPerUnit = calc.pixelPerUnit;

        // Marker
        if (dataInfo.isValue()) {

            const markerOffset = calc.getMarkerOffset(dataInfo.data.Val, proportions);

            dimensions.markerOffset = markerOffset + dimensions.q1Offset;

            const suffix = comparisonConfig.useQuintileColouring
                ? '_medium'
                : '';
            dimensions.markerImage = getMarkerImageFromSignificance(dataInfo.data.Sig[comparisonConfig.comparatorId],
                comparisonConfig.useRagColours, suffix, comparisonConfig.useQuintileColouring,
                indicatorId, dataInfo.data.SexId, dataInfo.data.AgeId);
        }

        this.adjustDimensionsForRounding(dimensions, calc.roundDifference);

        return dimensions;
    }

    isHighestLeft(polarityId: number): boolean {
        return polarityId === PolarityIds.RAGLowIsGood;
    }

    adjustDimensionsForRounding(dimensions: SpineChartDimensions, roundDiff: number) {
        // Note: q4 may not be most appropriate section to adjust
        if (roundDiff <= -this.one) {
            dimensions.q4 -= 1;
        } else if (roundDiff >= this.one) {
            dimensions.q4 += 1;
        }
    };
}

class SpineChartDimensionCalculator {

    public roundDifference = 0;
    public pixelPerUnit: number;

    private halfMarkerWidth = 18 / 2;

    constructor(unitsOfLargestSide: number) {
        const halfSpineChartWidth = 250 / 2;
        this.pixelPerUnit = halfSpineChartWidth / unitsOfLargestSide;
    }

    public getDimension(proportion: number) {

        const val = proportion * this.pixelPerUnit;
        const rounded = Math.round(val);
        this.roundDifference += val - rounded;
        return rounded;
    }

    /** Left offset to position the centre of marker image appropriate for the value */
    public getMarkerOffset(value: number, proportions: SpineChartProportions): number {

        if (value || value === 0 || value === -0) {
            const min = proportions.min;
            const marker = proportions.isHighestLeft ?
                min - value :
                value - min;

            return Math.round((marker * this.pixelPerUnit) - this.halfMarkerWidth);
        }
        return null;
    }
}

/** Proportion values are expressed in the unit of the indicator */
export class SpineChartProportions {
    q1Offset: number;
    q1: number;
    q2: number;
    q4: number;
    marker: number;
    min: number;
    unitsOfLargestSide: number;
    isHighestLeft: boolean;
}

export class SpineChartDimensions {
    q1: number;
    q2: number;
    q4: number;
    q1Offset: number;
    pixelPerUnit: number;
    imgUrl: string;
    markerOffset: number;
    markerImage: string;
    isSufficientData = true;
    isAnyData = true;
}
