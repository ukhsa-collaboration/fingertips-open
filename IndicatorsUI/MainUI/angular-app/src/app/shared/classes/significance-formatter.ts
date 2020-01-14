import { Significance } from '../../typings/FT';
import { ComparatorMethodIds, PolarityIds } from '../constants';

export class SignificanceFormatter {
    constructor(private polarityId: number, private comparatorMethodId: number) { }

    getLabel(significance: number): string {
        if (this.comparatorMethodId === ComparatorMethodIds.Quintiles) {
            switch (significance) {
                case Significance.Worse:
                    return 'Lowest quintile';
                case Significance.Same:
                    return '2nd lowest quintile';
                case Significance.Better:
                    return 'Middle quintile';
                case Significance.Worst:
                    return '2nd highest quintile';
                case Significance.Best:
                    return 'Highest quintile';
            }
        }

        if (this.polarityId === PolarityIds.BlueOrangeBlue) {
            switch (significance) {
                case Significance.Worse:
                    return 'Lower';
                case Significance.Same:
                    return 'Similar';
                case Significance.Better:
                    return 'Higher';
            }
        }

        if (this.polarityId === PolarityIds.RAGHighIsGood ||
            this.polarityId === PolarityIds.RAGLowIsGood) {

            switch (significance) {
                case Significance.Worse:
                    return 'Worse';
                case Significance.Same:
                    return 'Similar';
                case Significance.Better:
                    return 'Better';
                case Significance.Worst:
                    return 'Worst';
                case Significance.Best:
                    return 'Best';
            }
        }

        return 'Not compared';
    }

    getTargetLabel(significance: number): string {
        switch (significance) {
            case Significance.Worse:
                return 'Red';
            case Significance.Same:
                return 'Amber';
            case Significance.Better:
                return 'Green';
        }

        return 'Not compared';
    }
}
