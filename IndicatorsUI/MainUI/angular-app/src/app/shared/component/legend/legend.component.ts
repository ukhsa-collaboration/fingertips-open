import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';

@Component({
    selector: 'ft-legend',
    templateUrl: './legend.component.html',
    styleUrls: ['./legend.component.css']
})
export class LegendComponent implements OnChanges {

    @Input() pageType: PageType = null;
    @Input() keyType: KeyType = null;
    @Input() legendType: LegendType = null;
    @Input() showRAG3: Boolean = null;
    @Input() showRAG5: Boolean = null;
    @Input() showBOB: Boolean = null;
    @Input() showQuartiles: Boolean = null;
    @Input() showQuintilesRAG: Boolean = null;
    @Input() showQuintilesBOB: Boolean = null;
    @Input() showContinuous: Boolean = null;
    @Input() showRecentTrends: Boolean = null;

    ngOnChanges(changes: SimpleChanges) {
        if (changes['pageType']) {
            if (this.pageType) {
            }
        }
        if (changes['keyType']) {
            if (this.keyType) {
            }
        }
        if (changes['legendType']) {
            if (this.legendType) {
            }
        }
    }

    showOverview(): Boolean {
        return this.pageType === PageType.Overview;
    }

    showMap(): Boolean {
        return this.pageType === PageType.Map;
    }

    showTrends(): Boolean {
        return this.pageType === PageType.Trends;
    }

    showCompareAreas(): Boolean {
        return this.pageType === PageType.CompareAreas;
    }

    showAreaProfiles(): Boolean {
        return this.pageType === PageType.AreaProfiles;
    }

    showInequalities(): Boolean {
        return this.pageType === PageType.Inequalities;
    }

    showEngland(): Boolean {
        return this.pageType === PageType.England;
    }
}

export class PageType {
    public static readonly None = 0;
    public static readonly Overview = 1;
    public static readonly Map = 2;
    public static readonly Trends = 3;
    public static readonly CompareAreas = 4;
    public static readonly AreaProfiles = 5;
    public static readonly Inequalities = 6;
    public static readonly England = 7;
}

export class KeyType {
    public static readonly None = 0;
    public static readonly BarChart = 1;
    public static readonly ValueNote = 2;
    public static readonly SpineChart = 3;
    public static readonly TartanRug = 4;
    public static readonly InEquality = 5;
    public static readonly RecentTrends = 6;
    public static readonly DataQuality = 7;
}

export class LegendType {
    public static readonly None = 0;
    public static readonly RAG3 = 1;
    public static readonly RAG5 = 2;
    public static readonly BOB = 3;
    public static readonly NotCompared = 4;
    public static readonly Quintiles = 5;
    public static readonly Quartiles = 6;
    public static readonly Continuous = 7;
}
