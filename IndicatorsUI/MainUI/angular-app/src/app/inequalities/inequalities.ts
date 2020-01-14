import {
    CoreDataSet, Age, Sex, GroupRoot, ComparisonConfig, CategoryType, PartitionDataForAllCategories,
    PartitionDataForAllSexes, PartitionDataForAllAges, NamedIdentity
} from '../typings/FT';
import { Colour } from '../shared/shared';
import { CategoryTypeIds, SexIds, ComparatorMethodIds } from '../shared/constants';
import * as _ from 'underscore';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { isDefined } from '@angular/compiler/src/util';
import { ValueData, ValueNote } from '../typings/FT';
import { analyzeAndValidateNgModules } from '@angular/compiler';

export class PreferredPartitionType {
    public static readonly Category = 0;
    public static readonly Sex = 1;
    public static readonly Age = 2;
}

export class ViewModes {
    public static readonly LatestValues = 0;
    public static readonly Trends = 1;
}

export class CategoryDataAnalyser {
    categoryData: CoreDataSet[];
    categoryTypes: CategoryType[];
    benchmarkSpecialCases: CoreDataSet[];
    categoryTypeIdToData: Map<number, CoreDataSet[]> = new Map<number, CoreDataSet[]>();
    categoryTypesWithData: CategoryType[] = [];

    constructor(partitionDataForAllCategories: PartitionDataForAllCategories) {
        this.categoryData = _.filter(partitionDataForAllCategories.Data, function (d) { return d.Val !== -1 });
        this.categoryTypes = partitionDataForAllCategories.CategoryTypes;
        this.benchmarkSpecialCases = partitionDataForAllCategories.BenchmarkDataSpecialCases;


        this.categoryTypes.forEach(categoryType => {
            const dataOfType = _.filter(this.categoryData, function (d) { return d.CategoryTypeId === categoryType.Id });

            if (categoryType.Id !== CategoryTypeIds.HealthProfilesSSILimit && dataOfType.length) {
                this.categoryTypeIdToData[categoryType.Id] = dataOfType;
                this.categoryTypesWithData.push(categoryType);
            }
        });
    }

    isAnyData(): boolean {
        return this.categoryData.length > 0;
    }

    getDataByCategoryTypeId(categoryTypeId: number): CoreDataSet[] {
        return this.categoryTypeIdToData[categoryTypeId];
    }

    getDataYear(): number {
        if (this.isAnyData()) {
            return this.categoryData[0].Year;
        }

        return null;
    }

    getCategoryTypeById(categoryTypeId: number): CategoryType {
        return _.filter(this.categoryTypesWithData, function (type) { return type.Id === categoryTypeId })[0];
    }

    isCategoryTypeById(categoryTypeId): boolean {
        return _.some(this.categoryTypesWithData, function (type) { return type.Id === categoryTypeId });
    }

    getCategoryLabels(categoryTypeId): string[] {
        const categories = this.getCategoryTypeById(categoryTypeId).Categories;
        return _.pluck(categories, 'Name');
    }

    getCategoryTypes(): CategoryType[] {
        return this.categoryTypes;
    }
}

export class SexDataAnalyser {
    ageId: number;
    sexes: Sex[];
    sexData: CoreDataSet[];

    constructor(partitionDataForAllSexes: PartitionDataForAllSexes) {
        this.ageId = partitionDataForAllSexes.AgeId;
        this.sexes = partitionDataForAllSexes.Sexes;
        this.sexData = partitionDataForAllSexes.Data;
    }

    isAnyData(): boolean {
        return this.sexes.length > 1;
    }

    getSexes(): Sex[] {
        return this.sexes;
    }
}

export class AgeDataAnalyser {
    sexId: number;
    ages: Age[];
    ageData: CoreDataSet[];
    chartAverageLineAgeId: number;

    constructor(partitionDataForAllAges: PartitionDataForAllAges) {
        this.sexId = partitionDataForAllAges.SexId;
        this.ages = partitionDataForAllAges.Ages;
        this.ageData = partitionDataForAllAges.Data;
        this.chartAverageLineAgeId = partitionDataForAllAges.ChartAverageLineAgeId;
    }

    isAnyData(): boolean {
        return this.ages.length > 1;
    }
}

export class CategoryDataManager {
    private data: object = {};

    constructor() {
    }

    getDataKey(groupRoot: GroupRoot, areaCode: string, areaTypeId: number): string {
        const keyParts = [groupRoot.IID, groupRoot.Sex.Id, groupRoot.Age.Id, areaTypeId, areaCode];
        return keyParts.join('-');
    }

    getData(groupRoot: GroupRoot, areaCode: string, areaTypeId: number) {
        const key = this.getDataKey(groupRoot, areaCode, areaTypeId);
        return this.data[key];
    }

    setData(groupRoot: GroupRoot, areaCode: string, areaTypeId: number, categoryData: any): void {
        const key = this.getDataKey(groupRoot, areaCode, areaTypeId);
        this.data[key] = categoryData;
    }
}

export class SexDataManager {
    private data: object = {};

    constructor() {
    }

    getDataKey(groupRoot: GroupRoot, areaCode: string, areaTypeId: number): string {
        const keyParts = [groupRoot.IID, groupRoot.Age.Id, areaCode, areaTypeId];
        return keyParts.join('-');
    }

    getData(groupRoot: GroupRoot, areaCode: string, areaTypeId: number) {
        const key = this.getDataKey(groupRoot, areaCode, areaTypeId);
        return this.data[key];
    }

    setData(groupRoot: GroupRoot, areaCode: string, areaTypeId: number, sexData: any): void {
        const key = this.getDataKey(groupRoot, areaCode, areaTypeId);
        this.data[key] = sexData;
    }
}

export class AgeDataManager {
    private data: object = {};

    constructor() {
    }

    getDataKey(groupRoot: GroupRoot, areaCode: string, areaTypeId: number): string {
        const keyParts = [groupRoot.IID, groupRoot.Sex.Id, areaCode, areaTypeId];
        return keyParts.join('-');
    }

    getData(groupRoot: GroupRoot, areaCode: string, areaTypeId: number) {
        const key = this.getDataKey(groupRoot, areaCode, areaTypeId);
        return this.data[key];
    }

    setData(groupRoot: GroupRoot, areaCode: string, areaTypeId: number, ageData: any): void {
        const key = this.getDataKey(groupRoot, areaCode, areaTypeId);
        this.data[key] = ageData;
    }
}

export class CategoryDataBuilder {
    private ftHelper: FTHelperService;
    private categoryDataAnalyser: CategoryDataAnalyser;
    private categoryTypeId: number;
    private average: CoreDataSet;
    private groupRoot: GroupRoot;
    private comparisonConfig: ComparisonConfig;
    private dataSeries: any[] = [];
    private averageDataSeries: any[] = [];
    private selectedAreaName: string;
    private unit: any;
    private showConfidenceIntervalBars: boolean;
    private indicatorName: string;
    private partitionName: string;
    private timePeriod: string;
    private useSpecialCaseSocioeconomicGroup: boolean;

    constructor(ftHelper: FTHelperService, categoryDataAnalyser: CategoryDataAnalyser,
        categoryTypeId: number, average: CoreDataSet, groupRoot: GroupRoot,
        comparisonConfig: ComparisonConfig, selectedAreaName: string, unit: any,
        showConfidenceIntervalBars: boolean, indicatorName: string, partitionName: string,
        timePeriod: string, useSpecialCaseSocioeconomicGroup: boolean) {

        this.ftHelper = ftHelper;
        this.categoryDataAnalyser = categoryDataAnalyser;
        this.categoryTypeId = categoryTypeId;
        this.average = average;
        this.groupRoot = groupRoot;
        this.comparisonConfig = comparisonConfig;
        this.selectedAreaName = selectedAreaName;
        this.unit = unit;
        this.showConfidenceIntervalBars = showConfidenceIntervalBars;
        this.indicatorName = indicatorName;
        this.partitionName = partitionName;
        this.timePeriod = timePeriod;
        this.useSpecialCaseSocioeconomicGroup = useSpecialCaseSocioeconomicGroup;

        this.generateDataSeries();
    }

    generateDataSeries() {
        const analyser = this.categoryDataAnalyser;
        const categoryType = analyser.getCategoryTypeById(this.categoryTypeId);
        const dataList = analyser.getDataByCategoryTypeId(this.categoryTypeId);
        const categoryIdToData = _.indexBy(dataList, 'CategoryId');
        const categories = categoryType.Categories;

        let avg: any = {};

        if (this.useSpecialCaseSocioeconomicGroup) {
            this.average = analyser.benchmarkSpecialCases[0];
        }

        if (this.ftHelper.newCoreDataSetInfo(this.average).isValue()) {
            const dataValue = new InequalityValueData(this.ftHelper, this.average.ValF,
                this.average.NoteId, this.average.Count, this.average.Val, this.unit.Label);
            avg.y = this.average.Val;
            avg.valF = this.average.ValF;
            avg.noteValue = dataValue.Note.Text;
            avg.tooltipValue = dataValue.ValFWithUnitLabel;
        } else {
            // Point will not be displayed
            avg = null;
        }

        for (let counter = 0; counter < categories.length; counter++) {
            const categoryId = categories[counter].Id;
            const data = categoryIdToData[categoryId];

            let dataToPush: any = {};

            if (this.ftHelper.newCoreDataSetInfo(data).isValue()) {
                let value = data.Val;
                if (value === -1) {
                    value = 0;
                }

                let colour = Colour.noComparison;
                const significance = data.Significance;
                // Quintile colouring does not apply for inequalities
                if (significance > 0 && this.groupRoot.ComparatorMethodId !== ComparatorMethodIds.Quintiles) {
                    colour = Colour.getSignificanceColorForBenchmark(this.groupRoot.ComparatorMethodId,
                        this.groupRoot.PolarityId, this.comparisonConfig, significance);
                }

                const dataValue = new InequalityValueData(this.ftHelper, data.ValF, data.NoteId, data.Count, data.Val, this.unit.Label);

                dataToPush = {
                    y: value, valF: dataValue.ValF, color: colour,
                    noteValue: dataValue.Note.Text, tooltipValue: dataValue.ValFWithUnitLabel
                };

            } else {
                // Show no bar
                dataToPush = { Val: 0, y: 0 };
            }

            this.dataSeries.push(dataToPush);
            this.averageDataSeries.push(avg);
        }
    }

    getData(): BuilderData {
        const builderData = new BuilderData();
        builderData.labels = this.categoryDataAnalyser.getCategoryLabels(this.categoryTypeId);
        builderData.dataSeries = this.dataSeries;
        builderData.averageDataSeries = this.averageDataSeries;
        builderData.showAverageLine = true;

        builderData.averageLegend = this.selectedAreaName;
        if (this.useSpecialCaseSocioeconomicGroup) {
            builderData.averageLegend += ' average (18-64 yrs)';
        }

        builderData.unit = this.unit;
        builderData.dataList = this.categoryDataAnalyser.getDataByCategoryTypeId(this.categoryTypeId);
        builderData.showConfidenceIntervalBars = this.showConfidenceIntervalBars;
        builderData.indicatorName = this.indicatorName;
        builderData.timePeriod = this.timePeriod;
        builderData.areaName = this.selectedAreaName;
        builderData.partitionName = this.partitionName;

        return builderData;
    }
}

export class SexDataBuilder {
    private ftHelper: FTHelperService;
    private sexes: Sex[];
    private sexData: CoreDataSet[];
    private person: CoreDataSet;
    private personExist: boolean;
    private dataSeries = [];
    private averageDataSeries = [];
    private selectedAreaName: string;
    private unit: any;
    private indicatorName: string;
    private partitionName: string;
    private timePeriod: string;

    constructor(ftHelper: FTHelperService, sexDataAnalyser: SexDataAnalyser, groupRoot: GroupRoot,
        comparisonConfig: ComparisonConfig, selectedAreaName: string, unit: any, indicatorName: string,
        partitionName: string, timePeriod: string) {
        this.ftHelper = ftHelper;
        this.sexes = sexDataAnalyser.sexes;
        this.sexData = sexDataAnalyser.sexData;
        this.selectedAreaName = selectedAreaName;
        this.unit = unit;
        this.indicatorName = indicatorName;
        this.partitionName = partitionName;
        this.timePeriod = timePeriod;

        this.person = _.where(this.sexData, { SexId: SexIds.Person })[0];

        this.personExist = isDefined(this.person);

        const sexData = _.clone(this.sexData);

        if (this.personExist) {
            for (let counter = 0; counter < sexData.length; counter++) {
                if (sexData[counter].SexId === SexIds.Person) {
                    sexData.splice(counter, 1);
                }
            }

            for (let counter = 0; counter < this.sexes.length; counter++) {
                if (this.sexes[counter].Id === SexIds.Person) {
                    this.sexes.splice(counter, 1);
                }
            }
        }

        for (let counter = 0; counter < sexData.length; counter++) {
            // If value is -1 replace it with zero
            let value = sexData[counter].Val;
            if (value === -1) {
                value = 0;
            }

            let colour = Colour.noComparison;
            const significance = sexData[counter].Significance;
            // Quintile colouring does not apply for inequalities
            if (significance > 0 && groupRoot.ComparatorMethodId !== ComparatorMethodIds.Quintiles) {
                colour = Colour.getSignificanceColorForBenchmark(groupRoot.ComparatorMethodId,
                    groupRoot.PolarityId, comparisonConfig, significance);
            }

            const valueData = new InequalityValueData(this.ftHelper, sexData[counter].ValF,
                sexData[counter].NoteId, sexData[counter].Count, sexData[counter].Val, this.unit.Label);

            this.dataSeries.push({
                y: value, valF: valueData.ValF, color: colour,
                noteValue: valueData.Note.Text, tooltipValue: valueData.ValFWithUnitLabel
            });

            // If person object exists, set its value to average series data.
            // We don't need to show average line, if we only have male and female.
            if (this.personExist) {
                let personValue = this.person.Val;
                if (personValue === -1) {
                    personValue = 0;
                }

                const inequalityValueData = new InequalityValueData(this.ftHelper, this.person.ValF,
                    this.person.NoteId, this.person.Count, this.person.Val, this.unit.Label);
                this.averageDataSeries.push({
                    y: personValue, valF: inequalityValueData.ValF,
                    noteValue: inequalityValueData.Note.Text, tooltipValue: inequalityValueData.ValFWithUnitLabel
                });
            }
        }
    }

    getData(): BuilderData {
        const builderData = new BuilderData();
        builderData.labels = _.pluck(this.sexes, 'Name');
        builderData.dataSeries = this.dataSeries;
        builderData.averageDataSeries = this.averageDataSeries;
        builderData.showAverageLine = this.personExist;
        builderData.averageLegend = this.selectedAreaName + ' persons';
        builderData.unit = this.unit;
        builderData.indicatorName = this.indicatorName;
        builderData.timePeriod = this.timePeriod;
        builderData.areaName = this.selectedAreaName;
        builderData.partitionName = this.partitionName;
        builderData.dataList = this.sexData;

        return builderData;
    }
}

export class AgeDataBuilder {
    private ftHelper: FTHelperService;
    private ageDataAnalyser: AgeDataAnalyser;
    private ageData: CoreDataSet[];
    private dataSeries = [];
    private averageDataSeries = [];
    private groupRoot: GroupRoot;
    private selectedAreaName: string;
    private unit: any;
    private chartAverageLineAgeId: number;
    private canGenerateAverageLineOnChart = false;
    private indicatorName: string;
    private partitionName: string;
    private timePeriod: string;

    constructor(ftHelper: FTHelperService, ageDataAnalyser: AgeDataAnalyser, groupRoot: GroupRoot,
        comparisonConfig: ComparisonConfig, selectedAreaName: string, unit: any, indicatorName: string,
        partitionName: string, timePeriod: string) {

        this.ftHelper = ftHelper;
        this.ageDataAnalyser = ageDataAnalyser;
        this.ageData = ageDataAnalyser.ageData;
        this.groupRoot = groupRoot;
        this.selectedAreaName = selectedAreaName;
        this.unit = unit;
        this.chartAverageLineAgeId = ageDataAnalyser.chartAverageLineAgeId;
        this.indicatorName = indicatorName;
        this.partitionName = partitionName;
        this.timePeriod = timePeriod;

        for (let counter = 0; counter < this.ageData.length; counter++) {
            const ageData = this.ageData[counter];

            if (ageData.AgeId !== this.chartAverageLineAgeId) {
                let value = ageData.Val;
                if (value === -1) {
                    value = 0;
                }

                let colour = Colour.noComparison;
                const significance = ageData.Significance;
                // Quintile colouring does not apply for inequalities
                if (significance > 0 && groupRoot.ComparatorMethodId !== ComparatorMethodIds.Quintiles) {
                    colour = Colour.getSignificanceColorForBenchmark(groupRoot.ComparatorMethodId,
                        groupRoot.PolarityId, comparisonConfig, significance);
                }

                const valueData = new InequalityValueData(this.ftHelper, ageData.ValF,
                    ageData.NoteId, ageData.Count, ageData.Val, this.unit.Label);

                const data = {
                    y: value, valF: valueData.ValF, color: colour,
                    noteValue: valueData.Note.Text, tooltipValue: valueData.ValFWithUnitLabel
                };

                this.dataSeries.push(data);
            } else {
                this.canGenerateAverageLineOnChart = true;
            }
        }

        if (this.canGenerateAverageLineOnChart) {
            this.generateAverageDataSeries();
        }
    }

    generateAverageDataSeries() {
        const average = this.getIndicatorAgeRange();
        let averageY: number;

        // Define average
        if (this.ftHelper.newCoreDataSetInfo(average).isValue()) {
            averageY = average.Val;
        } else {
            averageY = null;
        }

        for (let counter = 0; counter < this.ageData.length; counter++) {
            const ageData = this.ageData[counter];

            if (ageData.AgeId !== this.chartAverageLineAgeId) {
                const valueData = new InequalityValueData(this.ftHelper, average.ValF,
                    average.NoteId, average.Count, average.Val, this.unit.Label);

                this.averageDataSeries.push({
                    y: averageY, valF: valueData.ValF,
                    noteValue: valueData.Note.Text, tooltipValue: valueData.ValFWithUnitLabel
                });
            }
        }
    }

    getIndicatorAgeRange(): CoreDataSet {
        const ageDataCopy = _.clone(this.ageData);

        for (let counter = 0; counter < ageDataCopy.length; counter++) {
            if (this.chartAverageLineAgeId === ageDataCopy[counter].AgeId) {
                const average = ageDataCopy[counter];
                average.Val = ageDataCopy[counter].Val;
                average.ValF = ageDataCopy[counter].ValF;
                average.NoteId = ageDataCopy[counter].NoteId;
                average.Count = ageDataCopy[counter].Count;
                return average;
            }
        }

        return null;
    }

    getData(): BuilderData {
        let labels: Age[] = [];
        let averageLabel = '';

        if (this.canGenerateAverageLineOnChart) {
            labels = this.ageDataAnalyser.ages.filter(x => x.Id !== this.chartAverageLineAgeId);
            const groupRoots = this.ftHelper.getAllGroupRoots();
            const groupRootIndex = groupRoots.findIndex(x => x.Age.Id === this.chartAverageLineAgeId);
            if (groupRootIndex !== -1) {
                averageLabel = groupRoots.find(x => x.Age.Id === this.chartAverageLineAgeId).Age.Name;
            }
        } else {
            labels = this.ageDataAnalyser.ages;
        }

        const builderData = new BuilderData();
        builderData.labels = _.pluck(labels, 'Name');
        builderData.dataSeries = this.dataSeries;
        builderData.averageDataSeries = this.averageDataSeries;
        builderData.showAverageLine = true;
        builderData.averageLegend = this.selectedAreaName + ' ' + averageLabel;
        builderData.unit = this.unit;
        builderData.indicatorName = this.indicatorName;
        builderData.timePeriod = this.timePeriod;
        builderData.areaName = this.selectedAreaName;
        builderData.partitionName = this.partitionName;
        builderData.dataList = this.ageData;

        return builderData;
    }
}

export class BuilderData {
    labels: string[];
    dataSeries: any[];
    averageDataSeries: any[];
    showAverageLine: boolean;
    averageLegend: string;
    unit: any;
    dataList: CoreDataSet[];
    showConfidenceIntervalBars: boolean;
    indicatorName: string;
    timePeriod: string;
    areaName: string;
    partitionName: string;
}

export class TrendChartData {
    min: number;
    max: number;
    width: number;
    height: number;
    heightPerLegend: number;
    unit: any;
    seriesData: any[];
    averageSeriesData: any[];
    periods: string[];
    indicatorName: string;
    areaName: string;
    partitionName: string;
    trendData: Map<number, CoreDataSet[]>;
}

export class TrendOption {
    OptionSelected: boolean;
    NamedIdentity: NamedIdentity;
}

export class TrendTableData {
    public periods: string[];
    public label: string;
    public valueData: ValueData[];
    public unitLabel: string;
    public trendSource: string;
    public noteId: string;
}

export class TrendTableDataFormatted {
    period: string;
    valueData: ValueData[];
}

export class InequalityValueData implements ValueData {
    Count: number;
    Val: number;
    ValF: string;
    ValFWithUnitLabel: string;
    Note: ValueNote;

    public static getWithValueNotes(ftHelper: FTHelperService, val: number, valF: string, note: ValueNote): string {
        const valueNoteSymbol = this.getValueNoteSymbol(ftHelper, note);

        if (val !== -1 && note.Id !== 0) {
            return valF + valueNoteSymbol;
        }

        if (val === -1 && note.Id !== 0 && note.Id !== 506) {
            return valueNoteSymbol;
        }

        if (val === -1 && (note.Id === 0 || note.Id === 506)) {
            return '-';
        }
    }

    public static getWithUnitLabelAndValueNotes(ftHelper: FTHelperService,
        val: number, valF: string, note: ValueNote, unitLabel: string): string {

        const valueNoteSymbol = this.getValueNoteSymbol(ftHelper, note);

        if (val !== -1 && note.Id !== 0) {
            return valF + unitLabel + valueNoteSymbol;
        }

        if (val === -1 && note.Id !== 0 && note.Id !== 506) {
            return valueNoteSymbol;
        }

        if (val === -1 && (note.Id === 0 || note.Id === 506)) {
            return '-';
        }
    }

    public static getValueNoteSymbol(ftHelper: FTHelperService, note: ValueNote): string {
        const existNoteId = isDefined(note) && isDefined(note.Id);
        return existNoteId ? ftHelper.getValueNoteSymbol() : '';
    }

    constructor(ftHelper: FTHelperService, valF?: string, noteId?: number, count?: number, val?: number, unitLabel?: string) {
        const noDataMessage = '-';

        this.Count = count;
        this.Val = val;
        this.Note = new InequalityValueNote(ftHelper, noteId);
        this.ValF = isDefined(val) ?
            InequalityValueData.getWithValueNotes(ftHelper, val, valF, this.Note) :
            noDataMessage;

        this.ValFWithUnitLabel = isDefined(val) ?
            InequalityValueData.getWithUnitLabelAndValueNotes(ftHelper, val, valF, this.Note, unitLabel) :
            noDataMessage;
    }
}

export class InequalityValueNote implements ValueNote {
    Id: number;
    Text: string;

    public static getValueNoteText(ftHelper: FTHelperService, noteId: any): string {
        const valueNoteObj = ftHelper.getValueNoteById(noteId);
        let noteValue: string;
        if (valueNoteObj) {
            noteValue = valueNoteObj.Text;
        }
        return noteValue;
    }

    constructor(ftHelper: FTHelperService, noteId: number) {
        if (isDefined(noteId)) {
            this.Id = noteId;
            this.Text = InequalityValueNote.getValueNoteText(ftHelper, noteId);
        } else {
            this.Id = null;
            this.Text = null;
        }
    }
}
