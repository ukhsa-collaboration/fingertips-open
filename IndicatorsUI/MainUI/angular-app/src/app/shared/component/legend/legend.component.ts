import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { PageType } from '../../constants';
import { LegendConfig } from './legend';

@Component({
    selector: 'ft-legend',
    templateUrl: './legend.component.html',
    styleUrls: ['./legend.component.css']
})
export class LegendComponent implements OnChanges {

    @Input() legendConfig: LegendConfig = null;
    @Input() showRecentTrends: boolean = null;
    @Input() showDataQuality: boolean = null;

    showAreaProfiles = false;
    showCompareAreas = false;
    showEngland = false;
    showInequalities = false;
    showMap = false;
    showNoteLegend = true;
    showOverview = false;
    showTrends = false;
    showLegend = false;
    showBenchmarkAgainstGoal = false;
    legendLinkText = '';

    ngOnChanges(changes: SimpleChanges) {

        if (changes['legendConfig']) {
            if (this.legendConfig) {
                this.legendDisplayer();
            }
        }
    }

    legendDisplayer() {
        const pageType = this.legendConfig.pageType;

        switch (pageType) {
            case PageType.AreaProfiles:
                this.showAreaProfiles = true;
                break;

            case PageType.CompareAreas:
                this.showCompareAreas = true;
                break;

            case PageType.England:
                this.showEngland = true;
                break;

            case PageType.Inequalities:
                this.showInequalities = true;
                this.showNoteLegend = false;
                break;

            case PageType.Map:
                this.showMap = true;
                break;

            case PageType.Overview:
                this.showOverview = true;
                break;

            case PageType.Trends:
                this.showTrends = true;
                break;
        }

        // Decide whether to display the legend by default
        // and set the link text accordingly
        this.showLegend = this.legendConfig.ftHelper.getLegendDisplayStatus();
        this.setLegendLinkText();

        // Decide whether to display benchmark against goal
        this.showBenchmarkAgainstGoal = this.legendConfig.showBenchmarkAgainstGoal;
    }

    toggleLegend() {
        this.showLegend = !this.showLegend;
        this.legendConfig.ftHelper.setLegendDisplayStatus(this.showLegend);
        this.setLegendLinkText();
    }

    setLegendLinkText() {
        this.legendLinkText = this.showLegend ? 'Hide legend' : 'Show legend';
    }

    canShowRecentTrends() {
        return this.showLegend && this.showRecentTrends;
    }
}
