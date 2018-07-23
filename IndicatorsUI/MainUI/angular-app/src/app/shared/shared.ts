import { TooltipManager, GroupRoot, ComparisonConfig } from '../typings/FT.d';

export class TooltipHelper {
    constructor(private tooltipManager: TooltipManager) { }
    displayHtml(event: MouseEvent, html: string): void {
        this.tooltipManager.setHtml(html);
        this.tooltipManager.positionXY(event.pageX + 10, event.pageY + 15);
        this.tooltipManager.showOnly();
    }
    reposition(event: MouseEvent): void {
        this.tooltipManager.positionXY(event.pageX + 10, event.pageY + 15)
    }
    hide(): void {
        this.tooltipManager.hide();
    }
}
export class AreaCodes {
    public static readonly England = "E92000001";
}

export class SexIds {
    public static readonly Male = 1;
    public static readonly Female = 2;
    public static readonly Person = 4;
}

/** Highcharts helper */
export class HC {
    public static readonly credits = { enabled: false };
    public static readonly noLineMarker = { enabled: false, symbol: 'x' }
}

export class ComparatorIds {
    public static readonly national = 4;
    public static readonly sub_national = 1;
}
export class AreaTypeIds {
    public static readonly District = 1;
    public static readonly Region = 6;
    public static readonly Practice = 7;
    public static readonly County = 9;
    public static readonly AcuteTrust = 14;
    public static readonly Country = 15;
    public static readonly UnitaryAuthority = 16;
    public static readonly GpShape = 18;
    public static readonly DeprivationDecile = 23;
    public static readonly Subregion = 46;
    public static readonly DistrictUA = 101;
    public static readonly CountyUA = 102;
    public static readonly PheCentres2013 = 103;
    public static readonly PheCentres2015 = 104;
    public static readonly OnsClusterGroup = 110;
    public static readonly Scn = 112;
    public static readonly AcuteTrusts = 118;
    public static readonly Stp = 120;
    public static readonly CombinedAuthorities = 126;
    public static readonly CcgSinceApr2017 = 152;
    public static readonly CcgPreApr2017 = 153;
    public static readonly CcgSinceApr2018 = 154;
};

export class IndicatorIds {
    public static readonly QofListSize = 114;
    public static readonly QofPoints = 295;
    public static readonly WouldRecommendPractice = 347;
    public static readonly LifeExpectancy = 650;
    public static readonly EthnicityEstimates = 1679;
    public static readonly DeprivationScore = 91872;
}

export class CategoryTypeIds {
    public static readonly DeprivationDecileCountyUA2015 = 39;
    public static readonly DeprivationDecileDistrictUA2015 = 40;
    public static readonly HealthProfilesSSILimit = 5;
    public static readonly DeprivationDecileCCG2010 = 11;
    public static readonly DeprivationDecileGp2015 = 38;
}

export class ProfileIds {
    public static readonly PracticeProfileSupportingIndicators = 21;
    public static readonly PracticeProfile = 20;
}

export class CommaNumber {

    constructor(private value: number) { }

    private commarate(value: number | string) {
        return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }

    /**
    * Rounds the number to 0 decimal places before formatting with commas
    */
    rounded() {
        return this.commarate(Math.round(this.value));
    }

    unrounded() {
        var commaNumber = this.value.toString();
        if (commaNumber.indexOf('.') > -1) {
            let bits = commaNumber.split('.');
            return this.commarate(bits[0]) + '.' + bits[1];
        }
        return this.commarate(this.value);
    }
};

export class NumberHelper {

    /**
     * Rounds a number to a fixed amount of decimal places
     * @param num Number to round
     * @param dec Number of decimal place to round to
     */
    public static roundNumber(num, dec) {
        return Math.round(num * Math.pow(10, dec)) / Math.pow(10, dec);
    };
}

export class ComparatorMethodIds {
    public static readonly SuicidePlan = 14;
    public static readonly Quintiles = 15;
}

export class PolarityIds {
    public static readonly RAGLowIsGood = 0;
    public static readonly RAGHighIsGood = 1;
    public static readonly Quintiles = 15;
    public static readonly BlueOrangeBlue = 99;
}
export class Colour {
    public static readonly chart = '#a8a8cc';
    public static readonly better = '#92d050';
    public static readonly same = '#ffc000';
    public static readonly worse = '#c00000';
    public static readonly none = '#ffffff';
    public static readonly limit99 = '#a8a8cc';
    public static readonly limit95 = '#444444';
    public static readonly border = '#666666';
    public static readonly comparator = '#000000';
    public static readonly bobLower = '#5555E6';
    public static readonly bobHigher = '#C2CCFF';
    public static readonly bodyText = '#333';
    public static readonly noComparison = '#c9c9c9';
    public static readonly quintile1 = '#DED3EC';
    public static readonly quintile2 = '#BEA7DA';
    public static readonly quintile3 = '#9E7CC8';
    public static readonly quintile4 = '#7E50B6';
    public static readonly quintile5 = '#5E25A4';

    public static getSignificanceColorForBenchmark(groupRoot: GroupRoot, comparisonConfig: ComparisonConfig, sig: number): string {

        const polarityId = groupRoot.PolarityId;
        // Quintiles
        if (comparisonConfig.useQuintileColouring ||
            groupRoot.ComparatorMethodId === ComparatorMethodIds.Quintiles) {
            if (sig > 0 && sig < 6) {
                const quintile = 'quintile' + sig;
                switch (quintile) {
                    case 'quintile1': {
                        return Colour.quintile1;
                    }
                    case 'quintile2': {
                        return Colour.quintile2;
                    }
                    case 'quintile3': {
                        return Colour.quintile3;
                    }
                    case 'quintile4': {
                        return Colour.quintile4;
                    }
                    case 'quintile5': {
                        return Colour.quintile5;
                    }
                }
            } else {
                return Colour.noComparison;
            }
        }
        // No colour comparison should be made
        if (polarityId === -1) {
            return Colour.noComparison;
        }
        // Blues
        if (polarityId === PolarityIds.BlueOrangeBlue) {
            return sig === 1 ? Colour.bobLower :
                sig === 2 ? Colour.same :
                    sig === 3 ? Colour.bobHigher :
                        Colour.noComparison;
        }
        // RAG
        return sig === 1 ? Colour.worse :
            sig === 2 ? Colour.same :
                sig === 3 ? Colour.better :
                    Colour.noComparison;
    }

    public static getColorForQuartile(quartile: number): string {
        switch (quartile) {
            case 1:
                return '#E8C7D1';
            case 2:
                return '#B74D6D';
            case 3:
                return '#98002E';
            case 4:
            case 5:
                return '#700023';
            default:
                return '#B0B0B2';
        }
    }

    public static getColorForQuintile(quintile: number): string {
        switch (quintile) {
            case 1:
                return Colour.quintile1;
            case 2:
                return Colour.quintile2;
            case 3:
                return Colour.quintile3;
            case 4:
                return Colour.quintile4;
            case 5:
            case 6:
                return Colour.quintile5;
            default:
                return '#B0B0B2';
        }
    }

    public static getColorForContinuous(orderFrac: number): string {
        if (orderFrac === -1) {
            return '#B0B0B2';
        }
        const seed = orderFrac;
        let r = 21;
        let g = 28;
        let b = 85;

        r = 255 - Math.round(seed * (255 - r));
        g = 233 - Math.round(seed * (233 - g));
        b = 127 - Math.round(seed * (127 - b));
        return 'rgb(' + r + ',' + g + ',' + b + ')';
    }
}
