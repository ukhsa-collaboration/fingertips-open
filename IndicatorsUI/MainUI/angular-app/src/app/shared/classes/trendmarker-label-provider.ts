import { TrendMarker, TrendMarkerLabel } from '../../typings/FT';
import { PolarityIds } from '../constants';

export class TrendMarkerLabelProvider {
    trendMarkerToLabel: Map<TrendMarker, TrendMarkerLabel>[] = [];
    trendMarkerLabels: TrendMarkerLabel[] = [];
    cannotBeCalculated: TrendMarkerLabel = { id: TrendMarker.CannotBeCalculated, text: 'Cannot be calculated' };
    noChange: TrendMarkerLabel = { id: TrendMarker.NoChange, text: 'No significant change' };

    constructor(private polarityId: number) {
        switch (this.polarityId) {
            case PolarityIds.RAGLowIsGood:
                this.trendMarkerLabels.push(this.cannotBeCalculated);
                this.trendMarkerLabels.push({ id: TrendMarker.Increasing, text: 'Increasing and getting worse' });
                this.trendMarkerLabels.push({ id: TrendMarker.Decreasing, text: 'Decreasing and getting better' });
                this.trendMarkerLabels.push(this.noChange);
                break;

            case PolarityIds.RAGHighIsGood:
                this.trendMarkerLabels.push(this.cannotBeCalculated);
                this.trendMarkerLabels.push({ id: TrendMarker.Increasing, text: 'Increasing and getting better' });
                this.trendMarkerLabels.push({ id: TrendMarker.Decreasing, text: 'Decreasing and getting worse' });
                this.trendMarkerLabels.push(this.noChange);
                break;

            default:
                this.trendMarkerLabels.push(this.cannotBeCalculated);
                this.trendMarkerLabels.push({ id: TrendMarker.Increasing, text: 'Increasing' });
                this.trendMarkerLabels.push({ id: TrendMarker.Decreasing, text: 'Decreasing' });
                this.trendMarkerLabels.push(this.noChange);
                break;
        }

        this.trendMarkerLabels.forEach(trendMarkerLabel => {
            this.trendMarkerToLabel[trendMarkerLabel.id.toString()] = trendMarkerLabel;
        });
    }

    getLabel(trendMarker: number): string {
        return this.trendMarkerLabels[trendMarker].text;
    }
}

