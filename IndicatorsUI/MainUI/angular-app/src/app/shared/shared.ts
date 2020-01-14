import * as _ from 'underscore';
import { isDefined } from '@angular/compiler/src/util';
import { FTHelperService } from './service/helper/ftHelper.service';
import { ComparatorMethodIds, PolarityIds, AreaTypeIds, AreaCodes, CategoryIds, CategoryTypeIds, SexIds } from './constants';
import { TooltipManager, GroupRoot, ComparisonConfig, CoreDataSet, Area, TrendDataPoint, FTConfig, IndicatorMetadata, FTModel } from '../typings/FT';
import { DeviceDetectorService } from '../../../node_modules/ngx-device-detector';

export class AreaAndDataSorterHelper {
    private coreDataSetList: CoreDataSetListHelper;

    constructor(private sortOrder: number, private data: CoreDataSet[], private areas: Area[], private areaHash: any) {
        this.coreDataSetList = new CoreDataSetListHelper(this.data);
    }

    reverseDataIfNecessary() {
        if (this.sortOrder > 0) {
            this.data.reverse();
        }
    }

    sortAreasByDataOrder(): Area[] {
        let areasSorted: Area[] = [];

        for (let counter = 0; counter < this.data.length; counter++) {
            this.areas.forEach(area => {
                if (area.Code === this.data[counter].AreaCode) {
                    areasSorted.push(area);
                }
            });
        }

        if (areasSorted.length === 0) {
            areasSorted = this.areas;
        } else {
            if (areasSorted.length !== _.size(this.areaHash)) {
                this.areaHash.forEach(area => {
                    if ($.inArray(area, areasSorted) === -1) {
                        areasSorted.push(area);
                    }
                });
            }
        }

        return areasSorted;
    }

    sortByCount() {
        this.coreDataSetList.sortByCount();
        this.reverseDataIfNecessary();
        return this.sortAreasByDataOrder();
    }

    sortByValue() {
        this.coreDataSetList.sortByValue();
        this.reverseDataIfNecessary();
        return this.sortAreasByDataOrder();
    }
}

export class AreaHelper {
    static isAreaList(areaCode: string) {
        return !_.isUndefined(areaCode) &&
            areaCode !== null &&
            areaCode.indexOf('al-') === 0;
    }

    constructor(private area: Area) { }

    getAreaNameToDisplay(): string {
        return this.getName(this.area.Name);
    }

    getShortAreaNameToDisplay(): string {
        return this.getName(this.area.Short);
    }

    private getName(name): string {
        if (this.area.AreaTypeId === AreaTypeIds.Practice && !AreaHelper.isAreaList(this.area.Code)) {
            return this.area.Code + ' - ' + name;
        }
        return name;
    }
}

export class AreaTypeHelper {
    private areaTypeId: number;

    public static isAreaListId(areaTypeId: number): boolean {
        return areaTypeId === AreaTypeIds.AreaList;
    }

    constructor(private ftHelper: FTHelperService) {
        this.areaTypeId = ftHelper.getAreaTypeId();
    }

    public isSmallAreaType(): boolean {
        const smallAreaTypes = [AreaTypeIds.Practice, AreaTypeIds.Ward, AreaTypeIds.LSOA, AreaTypeIds.MSOA];
        return _.contains(smallAreaTypes, this.areaTypeId);
    }

    public isGp(): boolean {
        return this.areaTypeId === AreaTypeIds.Practice;
    }

    public getSmallAreaTypeName(): string {
        const areaTypeName = this.ftHelper.getAreaTypeName();

        if (this.isSmallAreaType) {
            if (this.isGp()) {
                return areaTypeName + ' practices';
            }

            return areaTypeName + 's';
        }

        return areaTypeName;
    }
}

export class CategoryAreaCodeHelper {

    public static getCategoryAreaCode(areaCode): string {
        if (this.isCategoryAreacode(areaCode)) {
            return areaCode;
        }
        return null;
    }

    public static isCategoryAreacode(code): boolean {
        return code.includes('cat-');
    }
}

export class CategoryHelper {
    public static getCategoryName(categoryId: number): string {
        switch (categoryId) {
            case CategoryIds.Undefined:
                return '';
            case CategoryIds.MostDeprivedDecile:
                return 'Most deprived decile';
            case CategoryIds.MostDeprivedQuintile:
                return 'Most deprived quintile';
            case CategoryIds.LeastDeprivedQuintile:
                return 'Least deprived quintile';
            case CategoryIds.EthnicityWhite:
                return 'Ethnicity white';
            case CategoryIds.EthnicityMixed:
                return 'Ethnicity mixed';
            case CategoryIds.EthnicityAsian:
                return 'Ethnicity asian';
            case CategoryIds.EthnicityBlack:
                return 'Ethnicity black';
            case CategoryIds.EthnicityOther:
                return 'Ethnicity other';
        }
    }
}

export class CategoryTypeHelper {

    public static getCategoryTypeName(categoryTypeId: number): string {
        switch (categoryTypeId) {
            case CategoryTypeIds.Undefined:
                return '';
            case CategoryTypeIds.DeprivationDecileGp2010:
                return 'DeprivationDecileGp2010';
            case CategoryTypeIds.DeprivationDecileCountyUA2010:
                return 'DeprivationDecileCountyUA2010';
            case CategoryTypeIds.DeprivationDecileDistrictUA2010:
                return 'DeprivationDecileDistrictUA2010';
            case CategoryTypeIds.EthnicGroups7:
                return 'EthnicGroups7';
            case CategoryTypeIds.HealthProfilesSSILimit:
                return 'HealthProfilesSSILimit';
            case CategoryTypeIds.LsoaDeprivationQuintilesInEngland2010:
                return 'LsoaDeprivationQuintilesInEngland2010';
            case CategoryTypeIds.DeprivationDecileCcg2010:
                return 'DeprivationDecileCcg2010';
            case CategoryTypeIds.LsoaDeprivationDecilesWithinArea2010:
                return 'LsoaDeprivationDecilesWithinArea2010';
            case CategoryTypeIds.LsoaDeprivationQuintilesWithinArea2010:
                return 'LsoaDeprivationQuintilesWithinArea2010';
            case CategoryTypeIds.DeprivationDecileCCG2010:
                return 'DeprivationDecileCCG2010';
            case CategoryTypeIds.EthnicGroups5:
                return 'EthnicGroups5';
            case CategoryTypeIds.DeprivationDecileGp2015:
                return 'DeprivationDecileGp2015';
            case CategoryTypeIds.DeprivationDecileCountyUA2015:
                return 'DeprivationDecileCountyUA2015';
            case CategoryTypeIds.DeprivationDecileDistrictUA2015:
                return 'DeprivationDecileDistrictUA2015';
            case CategoryTypeIds.LsoaDeprivationQuintilesInEngland2015:
                return 'LsoaDeprivationQuintilesInEngland2015';
            case CategoryTypeIds.LsoaDeprivationDecilesWithinArea2015:
                return 'LsoaDeprivationDecilesWithinArea2015';
            case CategoryTypeIds.LsoaDeprivationQuintilesWithinArea2015:
                return 'LsoaDeprivationQuintilesWithinArea2015';
            case CategoryTypeIds.SocioeconomicGroup:
                return 'SocioeconomicGroup';
        }
    }
}

export class Colour {
    public static readonly chart = '#a8a8cc';
    public static readonly ragBest = '#00b092';
    public static readonly ragBetter = '#92d050';
    public static readonly ragSame = '#ffc000';
    public static readonly ragWorse = '#c00000';
    public static readonly ragWorst = '#821733';
    public static readonly none = '#ffffff';
    public static readonly limit99 = '#a8a8cc';
    public static readonly limit95 = '#444444';
    public static readonly border = '#666666';
    public static readonly comparator = '#000000';
    public static readonly bobLowest = '#252563';
    public static readonly bobLower = '#5555E6';
    public static readonly bobSame = '#ffc000';
    public static readonly bobHigher = '#C2CCFF';
    public static readonly bobHighest = '#C1F4F0';
    public static readonly bodyText = '#333';
    public static readonly noComparison = '#c9c9c9';
    public static readonly ragQuintile1 = '#DED3EC';
    public static readonly ragQuintile2 = '#BEA7DA';
    public static readonly ragQuintile3 = '#9E7CC8';
    public static readonly ragQuintile4 = '#7E50B6';
    public static readonly ragQuintile5 = '#5E25A4';
    public static readonly bobQuintile1 = '#254BA4';
    public static readonly bobQuintile2 = '#506EB6';
    public static readonly bobQuintile3 = '#7C93C8';
    public static readonly bobQuintile4 = '#A7B6DA';
    public static readonly bobQuintile5 = '#D3DBEC';

    public static getSignificanceColorForBenchmark(comparatorMethodId: number, polarityId: number,
        comparisonConfig: ComparisonConfig, sig: number): string {

        // Quintiles
        if (comparisonConfig.useQuintileColouring ||
            comparatorMethodId === ComparatorMethodIds.Quintiles) {
            return this.getColorForQuintile(sig, polarityId);
        }

        // Blues
        if (polarityId === PolarityIds.BlueOrangeBlue) {
            return this.getColorForBOB(sig);
        }

        // RAG
        if (polarityId === PolarityIds.RAGHighIsGood ||
            polarityId === PolarityIds.RAGLowIsGood) {
            return this.getColorForRAG(sig);
        }

        // No colour comparison should be made
        if (polarityId === -1) {
            return Colour.noComparison;
        }
    }

    // RAG colouring
    public static getColorForRAG(rag: number): string {
        switch (rag) {
            case 1:
                return Colour.ragWorse;
            case 2:
                return Colour.ragSame;
            case 3:
                return Colour.ragBetter;
            case 4:
                return Colour.ragWorst;
            case 5:
                return Colour.ragBest;
            default:
                return Colour.noComparison;
        }
    }

    // BOB Colouring
    public static getColorForBOB(bob: number): string {
        switch (bob) {
            case 1:
                return Colour.bobLower;
            case 2:
                return Colour.bobSame;
            case 3:
                return Colour.bobHigher;
            case 4:
                return Colour.bobLowest;
            case 5:
                return Colour.bobHighest;
            default:
                return Colour.noComparison;
        }
    }

    // Quintile colouring
    public static getColorForQuintile(sig: number, polarityId: number): string {
        const quintile = 'quintile' + sig;
        if (polarityId === PolarityIds.NotApplicable) {
            switch (quintile) {
                case 'quintile1':
                    return Colour.bobQuintile1;
                case 'quintile2':
                    return Colour.bobQuintile2;
                case 'quintile3':
                    return Colour.bobQuintile3;
                case 'quintile4':
                    return Colour.bobQuintile4;
                case 'quintile5':
                    return Colour.bobQuintile5;
                default:
                    return Colour.noComparison;
            }
        } else {
            switch (quintile) {
                case 'quintile1':
                    return Colour.ragQuintile1;
                case 'quintile2':
                    return Colour.ragQuintile2;
                case 'quintile3':
                    return Colour.ragQuintile3;
                case 'quintile4':
                    return Colour.ragQuintile4;
                case 'quintile5':
                    return Colour.ragQuintile5;
                default:
                    return Colour.noComparison;
            }
        }
    }

    // Quartile colouring for map
    public static getQuartileColorForMap(quartile: number): string {
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

    // Quintile colouring for map
    public static getQuintileColorForMap(quintile: number): string {
        switch (quintile) {
            case 1:
                return Colour.bobQuintile1;
            case 2:
                return Colour.bobQuintile2;
            case 3:
                return Colour.bobQuintile3;
            case 4:
                return Colour.bobQuintile4;
            case 5:
            case 6:
                return Colour.bobQuintile5;
            default:
                return '#B0B0B2';
        }
    }

    // Continuous colouring for map
    public static getContinuousColorForMap(orderFrac: number): string {
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

export class CommaNumber {

    constructor(private value: number | string) { }

    private commarate(value: number | string) {
        return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    }

    /**
    * Rounds the number to 0 decimal places before formatting with commas
    */
    rounded() {
        const num = _.isString(this.value) ? Number.parseFloat(this.value) : <number>this.value;
        return this.commarate(Math.round(num));
    }

    unrounded() {
        const commaNumber = this.value.toString();
        if (commaNumber.indexOf('.') > -1) {
            const bits = commaNumber.split('.');
            return this.commarate(bits[0]) + '.' + bits[1];
        }
        return this.commarate(this.value);
    }

};

export class ComparatorHelper {
    constructor(private ftHelper: FTHelperService) { }

    getCurrentComparator(): Area {
        return this.ftHelper.getCurrentComparator();
    }

    getCurrentComparatorCode(): string {
        const model = this.ftHelper.getFTModel();
        const comparator = this.getCurrentComparator();

        if (model.isNearestNeighbours() && comparator.Code !== AreaCodes.England) {
            return model.nearestNeighbour;
        } else {
            return comparator.Code;
        }
    }

    getCurrentComparatorName(): string {
        const model = this.ftHelper.getFTModel();
        const comparator = this.getCurrentComparator();

        if (model.isNearestNeighbours() && comparator.Code !== AreaCodes.England) {
            return this.ftHelper.getAreaName(model.areaCode) + ' and neighbours';
        } else {
            return comparator.Name;
        }
    }
}

export class CoreDataSetHelper {
    static getDummyCoreDataSet(): CoreDataSet {
        const data = {};
        data['AreaCode'] = '';
        data['AgeId'] = -1;
        data['CategoryId'] = -1;
        data['CategoryTypeId'] = -1;
        data['Count'] = -1;
        data['Denom'] = -1;
        data['Denom2'] = -1;
        data['LoCI'] = -1;
        data['UpCI'] = -1;
        data['LoCI99_8'] = -1;
        data['UpCI99_8'] = -1;
        data['LoCI99_8F'] = '-';
        data['UpCI99_8F'] = '-';
        data['Val'] = -1;
        data['ValF'] = '-';
        data['Sig'] = {};

        return <CoreDataSet>data;
    }

    static getDummyCoreDataSetWithAreaCode(areaCode: string): CoreDataSet {
        const data = this.getDummyCoreDataSet();
        data.AreaCode = areaCode;

        return data;
    }
}

export class CoreDataSetListHelper {
    constructor(private dataList: CoreDataSet[]) { }

    areAnyValidTrendValues(): boolean {
        return this.getValidValues('V').length > 0;
    }


    findByAreaCode(areaCode) {
        for (const data of this.dataList) {
            if (data.AreaCode === areaCode) {
                return data;
            }
        }
        return null;
    }

    getValidValues(property) {
        const validCoreData = _.filter(this.dataList, function (obj) {
            return isDefined(obj) && obj[property] !== -1;
        });

        return _.pluck(validCoreData, property);
    }

    sortByValue() {
        this.dataList.sort(function (a, b) {
            return a.Val - b.Val;
        });
    }

    sortByCount() {
        this.dataList.sort(function (a, b) {
            return a.Count - b.Count;
        });
    }
}

export class DeviceServiceHelper {
    constructor(private deviceService: DeviceDetectorService) { }

    isIE(): boolean {
        return this.deviceService.browser.toUpperCase() === 'IE';
    }
}

export class GroupRootHelper {

    constructor(private groupRoot: GroupRoot) { }

    findMatchingItemBySexAgeAndIndicatorId(items: any[]) {
        const groupRoot = this.groupRoot;
        return _.find(items, function (item) {
            return item.IID === groupRoot.IID &&
                item.Sex.Id === groupRoot.Sex.Id &&
                item.Age.Id === groupRoot.Age.Id;
        });
    }
}

export class LegendHelper {
    constructor(private ftConfig: FTConfig) { }

    showDataQualityLegend() {
        if (this.ftConfig.showDataQuality) {
            return true;
        }

        return false;
    }
}

export class MetadataHelper {
    public metadata: any;

    constructor(private ftHelper: FTHelperService) {
        const groupRoot = ftHelper.getCurrentGroupRoot();
        const metadataHash = ftHelper.getMetadataHash();
        this.metadata = metadataHash[groupRoot.IID];
    }

    getMetadata(): any {
        return this.metadata;
    }

    getMetadataUnit(): any {
        return this.metadata.Unit;
    }

    getMetadataUnitShortLabel(): string {
        const metadataUnit = this.getMetadataUnit();
        return this.ftHelper.newValueSuffix(metadataUnit).getShortLabel();
    }
}

export class NearestNeighbourHelper {
    private model: FTModel;
    private config: FTConfig;

    constructor(private ftHelperService: FTHelperService) {
        this.model = ftHelperService.getFTModel();
        this.config = ftHelperService.getFTConfig();
    }

    getNearestNeighbourName(): string {
        return this.ftHelperService.getAreaName(this.model.areaCode) + ' nearest neighbours';
    }

    getNearestNeighbourCode(): string {
        if (this.doesAreaTypeHaveNearestNeighbours()) {
            const config = this.getNearestNeighboursConfig();
            return 'nn-' + config.NeighbourTypeId + '-' + this.model.areaCode;
        }

        return '';
    }

    private getNearestNeighboursConfig() {
        return this.config.nearestNeighbour[this.model.areaTypeId];
    }

    private doesAreaTypeHaveNearestNeighbours(): boolean {
        return !!this.config.nearestNeighbour[this.model.areaTypeId];
    }
}

export class NewDataBadgeHelper {
    static getNewDataBadgeHtml(hasDataChanged: boolean): string {
        let newDataBadge = '';

        if (hasDataChanged) {
            newDataBadge = '&nbsp;<span style="margin-right:8px;" class="badge badge-success">New data</span>';
        }

        return newDataBadge;
    }

    static isNewData(hasDataChanged: boolean): boolean {
        return hasDataChanged ? true : false;
    }
}

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

export class ParameterBuilder {

    private keys = [];
    private values = [];

    /** Add a key value pair to the parameter list */
    add(key: string, value: any) {
        this.keys.push(key);

        if (_.isArray(value)) {
            value = value.join(',');
        }

        this.values.push(value);
        return this;
    };

    /**  Generate the parameter string */
    build() {

        const url = [];
        for (const index in this.keys) {
            if (this.keys.hasOwnProperty(index)) {
                if (index !== '0') {
                    url.push('&');
                }

                url.push(this.keys[index], '=', this.values[index]);
            }
        }

        // Do not prefix with '?' as JQuery ajax will do this
        return url.join('');
    };
}

export class ParentAreaHelper {
    private model: FTModel;

    constructor(private ftHelperService: FTHelperService) {
        this.model = this.ftHelperService.getFTModel();
    }

    getParentAreaNameForCSV(): string {
        // Area list
        if (this.model.parentTypeId === AreaTypeIds.AreaList) {
            return this.ftHelperService.getParentArea().Name;
        }

        // Others
        return this.getParentAreaName();
    }

    getParentAreaName(): string {
        // Area list
        if (this.model.parentTypeId === AreaTypeIds.AreaList) {
            return 'your area list';
        }

        // Nearest neighbours
        if (this.model.isNearestNeighbours()) {
            return new NearestNeighbourHelper(this.ftHelperService).getNearestNeighbourName();
        }

        // England
        if (this.model.areaTypeId === AreaTypeIds.Country) {
            return 'England';
        }

        // Others
        const parentArea = this.ftHelperService.getParentArea();
        return isDefined(parentArea) ? parentArea.Name : '';
    }

    getParentAreaNameWithFirstLetterUpperCase(): string {
        const parentAreaName = this.getParentAreaName();
        return parentAreaName.charAt(0).toUpperCase() +
            parentAreaName.substr(1, parentAreaName.length - 1);
    }

    getParentAreaCode(): string {
        // Area list
        if (this.model.parentTypeId === AreaTypeIds.AreaList) {
            return this.model.parentCode;
        }

        // Nearest neighbours
        if (this.model.isNearestNeighbours()) {
            return this.model.nearestNeighbour;
        }

        // England
        if (this.model.areaTypeId === AreaTypeIds.Country) {
            return AreaCodes.England;
        }

        // Others
        return this.ftHelperService.getParentArea().Code;
    }
}

export class ParentAreaTypeHelper {
    private model: FTModel;

    constructor(private ftHelperService: FTHelperService) {
        this.model = this.ftHelperService.getFTModel();
    }

    getParentAreaTypeId(): number {
        return this.ftHelperService.getParentTypeId();
    }

    getParentAreaTypeName(): string {
        return this.ftHelperService.getParentTypeName();
    }

    getParentAreaTypeNameForCSV(): string {
        if (this.model.parentTypeId === AreaTypeIds.AreaList ||
            this.model.isNearestNeighbours()) {
            return this.ftHelperService.getAreaTypeName();
        }

        return this.ftHelperService.getParentTypeName();
    }
}

export class SexAndAgeLabelHelper {
    constructor(private groupRoot: GroupRoot) { }
    getSexAndAgeLabel(): string {
        const label: string[] = []

        if (this.groupRoot.StateSex || this.groupRoot.StateAge) {

            label.push(' (');

            if (this.groupRoot.StateSex && this.groupRoot.Sex.Id !== -1) {
                label.push(this.groupRoot.Sex.Name);

                const areBothLabelsRequired = this.groupRoot.StateSex && this.groupRoot.StateAge;
                if (areBothLabelsRequired) {
                    label.push(', ');
                }
            }

            if (this.groupRoot.StateAge && this.groupRoot.Age.Id !== -1) {
                label.push(this.groupRoot.Age.Name);
            }

            label.push(')');

            // Neither sex nor age were defined
            if (label.length === 2) {
                return '';
            }
        }

        return label.join('');
    }
}

export class SexHelper {
    static getSexLabel(sexId: number): string {
        switch (sexId) {
            case SexIds.Person:
                return 'Persons';
            case SexIds.Male:
                return 'Male';
            case SexIds.Female:
                return 'Female';
            default:
                return '';
        }
    }
}

export class Sorter {

    static sortNumbers(a: number, b: number) {

        if (a < b) {
            return -1;
        } else if (a > b) {
            return 1;
        } else {
            return 0;
        }
    }

    static sortStrings(a: string, b: string) {

        const lowerA = a.toLowerCase();
        const lowerB = b.toLowerCase();

        if (lowerA < lowerB) {
            return -1;
        } else if (lowerA > lowerB) {
            return 1;
        } else {
            return 0;
        }
    }
}

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

export class TrendDataSetListHelper {
    constructor(private trendDataPoints: TrendDataPoint[]) { }

    getValidValues(property) {
        const validTrendData = _.filter(this.trendDataPoints, function (obj) {
            return isDefined(obj) && obj[property] !== -1;
        });

        return _.pluck(validTrendData, property);
    }

    areAnyValidTrendValues(): boolean {
        return this.getValidValues('V').length > 0;
    };
}

export class TrendSourceHelper {
    constructor(private metadata: any) { }

    getTrendSource(): string {
        let descriptiveSource = this.metadata.Descriptive['DataSource'];
        descriptiveSource = descriptiveSource.replace('<p>', '<div>');
        descriptiveSource = descriptiveSource.replace('</p>', '</div>');

        return 'Source: ' + descriptiveSource;
    }
}
