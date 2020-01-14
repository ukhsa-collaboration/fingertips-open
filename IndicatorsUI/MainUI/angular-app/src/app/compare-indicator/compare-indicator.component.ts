import { Component, HostListener, ChangeDetectorRef } from '@angular/core';
import { isDefined } from '@angular/compiler/src/util';
import { forkJoin } from 'rxjs';
import * as _ from 'underscore';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { UIService } from '../shared/service/helper/ui.service';
import { ParentAreaHelper, CommaNumber, AreaTypeHelper, NewDataBadgeHelper } from '../shared/shared';
import { AreaTypeIds, ComparatorIds, AreaCodes, Tabs } from '../shared/constants';
import { FTModel, FTUrls, GroupRootSummary, CoreDataSet, KeyValuePair, GroupRoot, TrendMarkerResult } from '../typings/FT';
import { IndicatorMenuItem, DataSeries, IndicatorUnit, Point } from './compare-indicator.classes';
import { CsvData, CsvConfig, CsvDataHelper } from '../shared/component/export-csv/export-csv';
import { TimePeriod } from '../shared/classes/time-period';
import { TrendMarkerLabelProvider } from '../shared/classes/trendmarker-label-provider';

@Component({
    selector: 'ft-compare-indicator',
    templateUrl: './compare-indicator.component.html',
    styleUrls: ['./compare-indicator.component.css']
})
export class CompareIndicatorComponent {

    public isEnglandAreaSelected: boolean;
    public isAnyData = false;
    public dataSeries: DataSeries;
    public noDataMessage: string;
    public subNationalButtonClass = '';
    public nationalButtonClass = 'button-selected';
    public imageChart: string;
    public r2Filter = false;
    public shouldHighlightSelectedArea = false;
    public selectedAreaName: string;
    public formattedParentAreaName: string;
    public dropdownConfig: any;
    public scatterParent: string;
    public scatterPractice: string;
    public scatterOther: string;
    public indicatorNames: string[] = [];
    public yAxisSelectedIndicatorName: string;
    public showIndicatorDropdown = false;

    private model: FTModel;
    private ftUrls: FTUrls;
    private areaTypeId: number;
    private scrollTop: number;
    private groupingDataForProfile: GroupRootSummary[];
    private indicatorMenuItems: IndicatorMenuItem[] = [];
    private areaCodeToNameLookUp: KeyValuePair<string, string>[] = [];
    private benchmarkIndex = ComparatorIds.National;
    private isGpPractice = false;

    // X Y Data
    private groupRootX: GroupRoot = null;
    private groupRootSummaryX: GroupRootSummary = null;
    private groupRootSummaryY: GroupRootSummary = null;
    private xAxisIndicatorData: CoreDataSet[] = [];
    private yAxisIndicatorData: CoreDataSet[] = [];
    private xAxisKey: string;
    private yAxisKey: string;
    private groupRootsY: GroupRoot[] = [];

    // Chart
    private chart: any;

    // Csv export
    public csvConfig: CsvConfig;

    constructor(private ftHelperService: FTHelperService,
        private indicatorService: IndicatorService,
        private uiService: UIService,
        private ref: ChangeDetectorRef) { }

    @HostListener('window:CompareIndicatorSelected', [
        '$event',
        '$event.detail.isEnglandAreaType'
    ])
    public onOutsideEvent(event) {
        // Set the alias
        const ftHelper = this.ftHelperService;
        this.model = ftHelper.getFTModel();
        this.isEnglandAreaSelected = this.model.areaTypeId === AreaTypeIds.Country;
        this.isGpPractice = this.model.areaTypeId === AreaTypeIds.Practice;

        if (this.isEnglandAreaSelected) {
            this.isAnyData = false;

            // Unlock UI
            ftHelper.showAndHidePageElements();
            ftHelper.unlock();

            // Scroll the browser to the top
            this.uiService.setScrollTop(this.scrollTop);
        } else {
            this.isAnyData = true;

            // Initialise the members
            this.ftUrls = ftHelper.getURL();
            this.areaTypeId = this.model.areaTypeId;

            // Get the position of the browser window
            this.scrollTop = this.uiService.getScrollTop();

            this.loadData();
        }
    }

    @HostListener('window:NoDataDisplayed', ['$event', '$event.detail.isEnglandAreaType'])
    public refreshVariable(isEnglandAreaType) {
        this.isAnyData = false;
        this.isEnglandAreaSelected = isEnglandAreaType;
    }

    public displaySubnationalAreas() {
        this.setBenchMark(ComparatorIds.SubNational);
    }

    public displayAllAreasInEngland() {
        this.setBenchMark(ComparatorIds.National);
    }

    public shouldHighlightSelectedAreaChanged(event: any) {
        this.shouldHighlightSelectedArea = !this.shouldHighlightSelectedArea;
        this.refreshChart();
    }

    public r2FilterChanged(event: any) {
        this.r2Filter = !this.r2Filter;
        this.refreshChart();
    }

    public yAxisIndicatorChanged(indicatorName: string) {
        // Set Y axis indicator name label
        this.yAxisSelectedIndicatorName = indicatorName;

        // Get the indicator menu item
        const indicatorMenuItem = this.getIndicatorMenuItem(indicatorName);

        if (isDefined(indicatorMenuItem)) {
            const indicatorId = indicatorMenuItem.IndicatorId;
            const sexId = indicatorMenuItem.SexId;
            const ageId = indicatorMenuItem.AgeId;

            // Show/hide indicator dropdown
            this.toggleIndicatorDropdown();

            // Set Y axis key
            this.ftHelperService.setYAxisSelectedKey(indicatorId + '-' + sexId + '-' + ageId);
            this.model.yAxisSelectedIndicatorId = indicatorId;
            this.model.yAxisSelectedSexId = sexId;
            this.model.yAxisSelectedAgeId = ageId;

            // Update group root summary for selected Y axis
            this.groupRootSummaryY = this.findGroupRootSummary(indicatorId, sexId, ageId);

            if (this.showChartImage()) {
                const root = this.groupingDataForProfile.find(x =>
                    x.IID === indicatorId &&
                    x.Sex.Id === sexId &&
                    x.Age.Id === ageId);
                this.gpChart(root);
            } else {

                const comparatorId = this.benchmarkIndex;
                const parentAreaCode = this.getParentAreaCode();

                // Generate the y-axis key
                this.yAxisKey = this.getKey(indicatorId, sexId, ageId);

                const coreDataObservable = this.indicatorService.getSingleIndicatorForAllAreas(this.groupRootSummaryY.GroupId,
                    this.model.areaTypeId, parentAreaCode, this.model.profileId, comparatorId, indicatorId, sexId, ageId);

                const groupRootsYObservable = this.indicatorService.getLatestGroupRootDataForOneIndicator(this.model.areaTypeId,
                    this.groupRootSummaryY.IID, parentAreaCode, this.model.profileId);

                forkJoin([coreDataObservable, groupRootsYObservable]).subscribe(results => {
                    const coreDataForSingleIndicator = _.values(<CoreDataSet[]>results[0]);
                    this.groupRootsY = _.values(<GroupRoot[]>results[1]);

                    // Store the area values against the key at the client
                    this.ftHelperService.setAreaValues(this.yAxisKey, coreDataForSingleIndicator);

                    // Refresh the chart
                    this.refreshChart();
                });
            }
        }
    }

    getIndicatorMenuItem(indicatorName: string): IndicatorMenuItem {
        return this.indicatorMenuItems.find(x => x.IndicatorName === indicatorName);
    }

    loadData() {
        // Set the alias
        const ftHelper = this.ftHelperService;

        // Initialise the grouping data for profile observable
        const groupingDataForProfileObservable = this.indicatorService.getGroupingDataForProfile(this.areaTypeId, this.model.profileId);

        // Load the grouping data for profile
        forkJoin([groupingDataForProfileObservable]).subscribe(results1 => {

            // Set the group root summaries
            this.groupingDataForProfile = _.values(results1[0]);

            this.setGroupRootX();

            this.checkGroupRootSummaryYDefined();

            const comparatorId = this.benchmarkIndex;
            const profileId = this.model.profileId;
            const areaTypeId = this.model.areaTypeId;
            const currentComparatorCode = this.getParentAreaCode();

            // Get X data
            const groupRootX = this.groupRootX;
            const xAxisDataObservable = this.indicatorService.getSingleIndicatorForAllAreas(groupRootX.Grouping[0].GroupId,
                areaTypeId, currentComparatorCode, profileId, comparatorId, groupRootX.IID, groupRootX.Sex.Id,
                groupRootX.Age.Id);

            // Get Y data
            const summaryY = this.groupRootSummaryY;
            const yAxisDataObservable = this.indicatorService.getSingleIndicatorForAllAreas(summaryY.GroupId,
                areaTypeId, currentComparatorCode, profileId, comparatorId, summaryY.IID, summaryY.Sex.Id,
                summaryY.Age.Id);

            const groupRootsYObservable = this.indicatorService.getLatestGroupRootDataForOneIndicator(this.model.areaTypeId,
                this.groupRootSummaryY.IID, currentComparatorCode, this.model.profileId);

            forkJoin([xAxisDataObservable, yAxisDataObservable, groupRootsYObservable]).subscribe(results2 => {
                this.xAxisIndicatorData = <CoreDataSet[]>results2[0];
                this.yAxisIndicatorData = <CoreDataSet[]>results2[1];
                this.groupRootsY = <GroupRoot[]>results2[2];

                this.setXAxisKeyAndAreaValues();
                this.setYAxisKeyAndAreaValues();
                this.setAreaCodeToNameLookUp();

                // Set the selected filter area name
                this.selectedAreaName = this.areaCodeToNameLookUp[this.model.areaCode];

                // Load y-axis indicator dropdown
                this.loadYAxisIndicatorDropdown();

                // Get y-axis selected key
                const yAxisSelectedKey = this.ftHelperService.getYAxisSelectedKey();

                // Set the y-axis indicator name label
                this.setYAxisIndicatorNameLabel(yAxisSelectedKey);

                if (this.showChartImage()) {
                    const root = this.getGroupRootSummaryForGPChart(yAxisSelectedKey);
                    this.gpChart(root);
                } else {
                    this.transformDataForIndicator();
                }

                this.applyTabButtonStyles();

                // Unlock UI
                ftHelper.showAndHidePageElements();
                ftHelper.hideBenchmarkBox();
                ftHelper.unlock();

                // Scroll the browser to the top
                this.uiService.setScrollTop(this.scrollTop);
            });
        });
    }

    private setGroupRootX() {
        this.groupRootX = this.ftHelperService.getCurrentGroupRoot();
        const root = this.groupRootX;
        this.groupRootSummaryX = this.findGroupRootSummary(root.IID, root.Sex.Id, root.Age.Id);
    }

    private checkGroupRootSummaryYDefined() {
        if (!isDefined(this.groupRootSummaryY)) {
            // Use first indicator in A-Z order
            this.groupRootSummaryY = this.groupingDataForProfile[0];
        }
    }

    displayNoDataMessage(xLength, yLength) {

        const xAxisTitle = this.getXAxisTitle();
        const yAxisTitle = this.getYAxisTitle();

        if (xLength === 0 && yLength === 0) {
            this.noDataMessage = 'Data is not available for' + '<br />' + xAxisTitle + '<br /> & <br />' + yAxisTitle;
            return;
        }

        if (xLength > 0 && yLength === 0) {
            this.noDataMessage = 'Data is not available for' + '<br />' + yAxisTitle;
            return;
        }

        if (xLength === 0 && yLength > 0) {
            this.noDataMessage = 'Data is not available for' + '<br />' + xAxisTitle;
            return;
        }

        if (xLength > 0 && yLength > 0) {
            this.noDataMessage = 'Unable to display chart for the selected options';
            return;
        }
    }

    setAreaCodeToNameLookUp() {
        // Get the area list
        const areas = this.ftHelperService.getAreaList();

        // Store the area names and codes mapping at the client
        this.areaCodeToNameLookUp = _.object(_.map(areas, function (item) { return [item.Code, item.Name] }));
    }

    setXAxisKeyAndAreaValues() {
        // Set the alias
        const ftHelper = this.ftHelperService;

        // Set the x-axis key
        this.xAxisKey = ftHelper.getIndicatorKey(this.groupRootX, this.model, this.getParentAreaCode());

        // Store the area values against the key at the client
        ftHelper.setAreaValues(this.xAxisKey, this.xAxisIndicatorData);
    }

    setYAxisKeyAndAreaValues() {

        // Set the y-axis key
        const summary = this.groupRootSummaryY;
        this.yAxisKey = this.getKey(summary.IID, summary.Sex.Id, summary.Age.Id);

        this.model.yAxisSelectedIndicatorId = summary.IID;
        this.model.yAxisSelectedSexId = summary.Sex.Id;
        this.model.yAxisSelectedAgeId = summary.Age.Id;

        // Store the area values against the key at the client
        this.ftHelperService.setAreaValues(this.yAxisKey, this.yAxisIndicatorData);
    }

    loadYAxisIndicatorDropdown() {
        this.indicatorMenuItems.length = 0;
        for (let i = 0; i < this.groupingDataForProfile.length; i++) {

            const rootSummary = this.groupingDataForProfile[i];

            const ageAndSexLabel = this.getSexAndAgeLabel(rootSummary);

            const menuItem = new IndicatorMenuItem();
            menuItem.IndicatorId = rootSummary.IID;
            menuItem.AgeId = rootSummary.Age.Id;
            menuItem.SexId = rootSummary.Sex.Id;
            menuItem.IndicatorName = rootSummary.IndicatorName + ageAndSexLabel;
            menuItem.Key = rootSummary.IID + '-' +
                rootSummary.Sex.Id + '-' +
                rootSummary.Age.Id;
            menuItem.Sequence = i;

            this.indicatorMenuItems.push(menuItem);

            this.indicatorNames.push(menuItem.IndicatorName);
        }
    }

    setYAxisIndicatorNameLabel(yAxisSelectedKey: string): void {

        if (isDefined(yAxisSelectedKey)) {

            const indicatorId = this.model.yAxisSelectedIndicatorId;
            const sexId = this.model.yAxisSelectedSexId;
            const ageId = this.model.yAxisSelectedAgeId;

            this.yAxisSelectedIndicatorName = this.indicatorMenuItems.find(x => x.IndicatorId === indicatorId && x.SexId === sexId && x.AgeId === ageId).IndicatorName;
        } else {
            this.yAxisSelectedIndicatorName = this.indicatorMenuItems[0].IndicatorName;
        }
    }

    getGroupRootSummaryForGPChart(yAxisSelectedKey: string): GroupRootSummary {
        if (isDefined(yAxisSelectedKey)) {
            const indicatorId = this.model.yAxisSelectedIndicatorId;
            const sexId = this.model.yAxisSelectedSexId;
            const ageId = this.model.yAxisSelectedAgeId;

            return this.groupingDataForProfile.find(x =>
                x.IID === indicatorId &&
                x.Sex.Id === sexId &&
                x.Age.Id === ageId);
        } else {
            return this.groupingDataForProfile[0];
        }
    }

    getKey(indicatorId: number, sexId: number, ageId: number) {
        const model = this.model;

        const groupId = model.groupId;
        const areaTypeId = model.areaTypeId;
        const parentAreaCode = this.getParentAreaCode();

        // Return key
        return groupId + '-' + indicatorId + '-' + sexId + '-' + ageId + '-' +
            areaTypeId + '-' + parentAreaCode;
    }

    findGroupRootSummary(indicatorId: number, sexId: number, ageId: number): GroupRootSummary {
        return this.groupingDataForProfile.find(x =>
            x.IID === indicatorId && x.Sex.Id === sexId && x.Age.Id === ageId)
    }

    getSexAndAgeLabel(groupRootSummary: GroupRootSummary) {
        const label: string[] = []

        if (groupRootSummary.StateSex || groupRootSummary.StateAge) {

            label.push(' (');

            if (groupRootSummary.StateSex && groupRootSummary.Sex.Id !== -1) {
                label.push(groupRootSummary.Sex.Name);

                const areBothLabelsRequired = groupRootSummary.StateSex && groupRootSummary.StateAge;
                if (areBothLabelsRequired) {
                    label.push(', ');
                }
            }

            if (groupRootSummary.StateAge && groupRootSummary.Age.Id !== -1) {
                label.push(groupRootSummary.Age.Name);
            }

            label.push(')');

            // Neither sex nor age were defined
            if (label.length === 2) {
                return '';
            }
        }

        return label.join('');
    }

    getIndicatorUnit(): IndicatorUnit {

        const indicatorUnit = new IndicatorUnit();

        indicatorUnit.UnitX = this.getAxisUnitLabel(this.groupRootSummaryX);
        indicatorUnit.UnitY = this.getAxisUnitLabel(this.groupRootSummaryY);

        return indicatorUnit;
    };

    getAxisUnitLabel(groupRootSummary: GroupRootSummary): string {
        const unit = groupRootSummary.Unit;

        // Do not need unit label if it is already included in the title
        if (unit.Value > 1) {
            const commaNumber = new CommaNumber(unit.Value).unrounded();
            if (groupRootSummary.IndicatorName.indexOf(commaNumber) > -1) {
                return '';
            }
        }

        return unit.Label;
    }

    getXAxisTitle(): string {
        return this.getAxisTitle(this.groupRootSummaryX);
    }

    getYAxisTitle(): string {
        return this.getAxisTitle(this.groupRootSummaryY);
    }

    getAxisTitle(groupRootSummary: GroupRootSummary) {
        if (!groupRootSummary) { return ''; }

        const ageAndSexLabel = this.getSexAndAgeLabel(groupRootSummary);
        return groupRootSummary.IndicatorName + ageAndSexLabel;
    }

    calculateMargin() {
        let newMargin = 30;

        const title = this.getYAxisTitle();

        const charCount = title.length;
        if (charCount > 60) {
            newMargin = (charCount / 60) * 30;
        }

        return newMargin;
    }

    gpChart(root: GroupRootSummary) {
        const width = 900;
        const height = 500;
        const offset = 0;

        const parentCode = this.model.parentCode;
        const areaCode = this.model.areaCode;
        const areaTypeId = this.model.areaTypeId;

        const groupId1 = root.GroupId;
        const indicatorId1 = root.IID;
        const sexId1 = root.Sex.Id;
        const ageId1 = root.Age.Id;

        const currentGroupRoot = this.groupRootX;

        const groupId2 = this.model.groupId;
        const indicatorId2 = currentGroupRoot.IID;
        const sexId2 = currentGroupRoot.Sex.Id;
        const ageId2 = currentGroupRoot.Age.Id;

        const bridgeUrl = this.ftHelperService.getURL().bridge;
        const serviceUrl = 'img/gp-scatter-chart?'

        const imageChartUrl = bridgeUrl +
            serviceUrl +
            '&width=' + width +
            '&height=' + height +
            '&off=' + offset +
            '&par=' + parentCode +
            '&are=' + areaCode +
            '&ati=' + areaTypeId +
            '&gid1=' + groupId1 +
            '&iid1=' + indicatorId1 +
            '&sex1=' + sexId1 +
            '&age1=' + ageId1 +
            '&gid2=' + groupId2 +
            '&iid2=' + indicatorId2 +
            '&sex2=' + sexId2 +
            '&age2=' + ageId2;

        this.imageChart = null;
        this.imageChart = '<img src="' + imageChartUrl + '" alt="" />';

        this.updateScatterPracticeLegendLabel();
        this.updateScatterParentLegendLabel();
        this.updateScatterOtherPracticeLegendLabel();
    }

    transformDataForIndicator() {

        const rawData = this.ftHelperService.getAreaValues();
        const dataX = rawData[this.xAxisKey];
        const dataY = rawData[this.yAxisKey];

        // Define points
        const points: Point[] = [];
        const xAxisData: number[] = [];
        const yAxisData: number[] = [];
        let xCount = 0;
        let yCount = 0;

        // Get codes of areas to display on the scatter plot
        const areaCodes = this.getAreasToDisplayOnChart();

        for (let i = 0; i < _.size(areaCodes); i++) {

            const areaCode = areaCodes[i];

            const x = dataX[areaCode];
            const y = dataY[areaCode];

            // Are there values for both x and y?
            const isX = isDefined(x) && x.ValF !== '-';
            const isY = isDefined(y) && y.ValF !== '-';
            if (isX) { xCount++; }
            if (isY) { yCount++; }

            if (isX && isY) {

                // Create point
                const point = new Point();
                point.Name = this.getPointAreaName(areaCode);
                point.AreaCode = areaCode;
                this.assignPointColour(point);
                this.assignPointValues(point, x, y);

                // Add data to arrays
                points.push(point);
                xAxisData.push(x.Val);
                yAxisData.push(y.Val);
            }
        }

        this.dataSeries = new DataSeries();
        this.dataSeries.Points = points;

        if (this.dataSeries.Points.length === 0) {
            // Not enough data
            this.isAnyData = false;
            this.displayNoDataMessage(xCount, yCount);
        } else {
            // Data available
            this.isAnyData = true;
            this.noDataMessage = '';
            this.dataSeries.Name = this.ftHelperService.getAreaTypeName() + ' in ' + this.ftHelperService.getCurrentComparator().Short;
            this.dataSeries.XAxisTitle = this.getXAxisTitle();
            this.dataSeries.YAxisTitle = this.getYAxisTitle();
            this.dataSeries.IndicatorUnit = this.getIndicatorUnit();
            this.dataSeries.Margin = this.calculateMargin();
            this.dataSeries.XAxisData = xAxisData;
            this.dataSeries.YAxisData = yAxisData;
            this.dataSeries.SelectedAreaName = this.selectedAreaName;
            this.dataSeries.R2Selected = this.r2Filter;
        }

        this.ref.detectChanges();
    }

    getAreasToDisplayOnChart(): string[] {
        let areaCodes: string[];
        if (this.benchmarkIndex === ComparatorIds.National) {
            // All areas
            areaCodes = _.keys(this.areaCodeToNameLookUp);
        } else {
            // Subnational areas
            const parentTypeId = this.ftHelperService.getParentTypeId();
            const areaTypeId = this.ftHelperService.getAreaTypeId();
            const key = parentTypeId.toString() + '-' + areaTypeId.toString() + '-';
            areaCodes = this.ftHelperService.getAreaMappingsForParentCode(key);

            // Add selected area if nearest neighbours
            if (this.model.isNearestNeighbours()) {
                areaCodes.push(this.model.areaCode);
            }
        }
        return areaCodes;
    }

    assignPointColour(point: Point): void {
        point.Highlighted = this.shouldHighlightSelectedArea &&
            point.AreaCode === this.model.areaCode;
    }

    assignPointValues(point: Point, x: CoreDataSet, y: CoreDataSet): void {
        // Point values
        point.X = x.Val;
        point.Y = y.Val;
        point.XValF = x.ValF;
        point.YValF = y.ValF;
    }

    getPointAreaName(areaCode: string): string {
        const areaName = this.areaCodeToNameLookUp[areaCode];
        if (isDefined(areaName)) {
            if (this.isGpPractice) {
                return areaCode + ' - ' + areaName;
            }

            return areaName;
        }
        return '';
    }

    getParentAreaCode(): string {

        if (this.benchmarkIndex === ComparatorIds.National) {
            return AreaCodes.England;
        }

        if (this.model.isNearestNeighbours()) {
            return this.model.nearestNeighbour;
        }

        return this.model.parentCode;
    }

    refreshChart() {
        this.dataSeries.Points.length = 0;
        this.dataSeries = null;
        this.transformDataForIndicator();
    }

    updateScatterPracticeLegendLabel() {
        const label = this.ftHelperService.getArea(this.model.areaCode).Name;
        this.scatterPractice = '<img src="' + this.ftUrls.img + 'scatter-practice.png" />' + label;
    }

    updateScatterParentLegendLabel() {
        const label = new AreaTypeHelper(this.ftHelperService).getSmallAreaTypeName() + ' in ' + this.ftHelperService.getParentArea().Short;
        this.scatterParent = '<img src="' + this.ftUrls.img + 'scatter-parent.png" />' + label;
    }

    updateScatterOtherPracticeLegendLabel() {
        const label = new AreaTypeHelper(this.ftHelperService).getSmallAreaTypeName() + ' in England';
        this.scatterOther = '<img src="' + this.ftUrls.img + 'scatter-other.png" />' + label;
    }

    setBenchMark(comparatorId: number) {
        // Set the benchmark index
        this.benchmarkIndex = comparatorId;
        this.loadData();
    }

    applyTabButtonStyles() {
        // Set tab button name
        if (this.model.isNearestNeighbours()) {
            const areaCode = this.model.areaCode;
            const areaName = this.ftHelperService.getArea(areaCode).Name;
            this.formattedParentAreaName = areaName + ' & nearest neighbours';
        } else {
            const parentAreaName = new ParentAreaHelper(this.ftHelperService).getParentAreaName();
            this.formattedParentAreaName = 'All in ' + parentAreaName;
        }

        // Set tab button style
        // Override the styles if areas grouped by is England
        if (this.model.parentCode === AreaCodes.England) {
            this.nationalButtonClass = 'button-selected'
            this.subNationalButtonClass = 'hidden';
        } else {
            if (this.benchmarkIndex === ComparatorIds.SubNational) {
                this.nationalButtonClass = '';
                this.subNationalButtonClass = 'button-selected';
            } else {
                this.nationalButtonClass = 'button-selected';
                this.subNationalButtonClass = '';
            }
        }
    }

    toggleIndicatorDropdown(): void {
        this.showIndicatorDropdown = !this.showIndicatorDropdown;
    }

    showChartImage(): boolean {

        return new AreaTypeHelper(this.ftHelperService).isSmallAreaType() &&
            this.benchmarkIndex === ComparatorIds.National;
    }

    sortCompareIndicators(postIndicator, prevIndicator) {
        if (postIndicator.IndicatorName < prevIndicator.IndicatorName) {
            return -1;
        }
        if (postIndicator.IndicatorName > prevIndicator.IndicatorName) {
            return 1;
        }
        return 0;
    }

    // Event emitter method
    // Used to export chart
    updateChart(event) {
        this.chart = event.chart;
    }

    onExportChartAsImage() {
        this.chart.exportChart({ type: 'image/png' }, {});
    }

    onExportChartAsCsv(event: MouseEvent) {
        const csvData: CsvData[] = [];
        const areaValues = this.ftHelperService.getAreaValues();

        // X-axis indicator
        this.dataSeries.Points.forEach(point => {
            const areaData: CoreDataSet = areaValues[this.xAxisKey][point.AreaCode];
            const data = this.addCsvRow(point, areaData, this.groupRootSummaryX, true);
            csvData.push(data);
        });

        // Y-axis indicator
        this.dataSeries.Points.forEach(point => {
            const areaData: CoreDataSet = areaValues[this.yAxisKey][point.AreaCode];
            const data = this.addCsvRow(point, areaData, this.groupRootSummaryY, false);
            csvData.push(data);
        });

        this.csvConfig = new CsvConfig();
        this.csvConfig.tab = Tabs.CompareIndicators;
        this.csvConfig.csvData = csvData;
    }

    addCsvRow(point: Point, areaData: CoreDataSet, groupRootSummary: GroupRootSummary, xIndicator: boolean): CsvData {
        const data = new CsvData();

        let groupRoot: GroupRoot;
        if (xIndicator) {

            groupRoot = this.model.groupRoots.find(x => x.IID === groupRootSummary.IID &&
                x.Sex.Id === groupRootSummary.Sex.Id &&
                x.Age.Id === groupRootSummary.Age.Id);

        } else {

            groupRoot = this.groupRootsY.find(x => x.IID === groupRootSummary.IID &&
                x.Sex.Id === groupRootSummary.Sex.Id &&
                x.Age.Id === groupRootSummary.Age.Id);
        }

        data.indicatorId = groupRootSummary.IID.toString();
        data.indicatorName = groupRootSummary.IndicatorName;

        const parentAreaHelper = new ParentAreaHelper(this.ftHelperService);

        if (this.benchmarkIndex === ComparatorIds.National) {
            data.parentCode = AreaCodes.England;
            data.parentName = 'England';
        } else {
            data.parentCode = parentAreaHelper.getParentAreaCode();
            data.parentName = parentAreaHelper.getParentAreaNameForCSV();
        }

        data.areaCode = point.AreaCode;
        data.areaName = point.Name;
        data.areaType = '';

        data.sex = groupRootSummary.Sex.Name;
        data.age = groupRootSummary.Age.Name;

        if (isDefined(areaData)) {
            data.categoryType = CsvDataHelper.getDisplayValue(areaData.CategoryTypeId);
            data.category = CsvDataHelper.getDisplayValue(areaData.CategoryId);
            data.timePeriod = groupRoot.Grouping[0].Period;
            data.value = CsvDataHelper.getDisplayValue(areaData.Val);
            data.lowerCiLimit95 = CsvDataHelper.getDisplayValue(areaData.LoCI);
            data.upperCiLimit95 = CsvDataHelper.getDisplayValue(areaData.UpCI);
            data.lowerCiLimit99_8 = CsvDataHelper.getDisplayValue(areaData.LoCI99_8);
            data.upperCiLimit99_8 = CsvDataHelper.getDisplayValue(areaData.UpCI99_8);
            data.count = CsvDataHelper.getDisplayValue(areaData.Count);
            data.denominator = CsvDataHelper.getDisplayValue(areaData.Denom);
        } else {
            data.categoryType = '';
            data.category = '';
            data.timePeriod = groupRoot.Grouping[0].Period;
            data.value = '';
            data.lowerCiLimit95 = '';
            data.upperCiLimit95 = '';
            data.lowerCiLimit99_8 = '';
            data.upperCiLimit99_8 = '';
            data.count = '';
            data.denominator = '';
        }

        data.valueNote = '';
        if (isDefined(areaData.NoteId)) {
            data.valueNote = this.ftHelperService.newValueNoteTooltipProvider().getTextFromNoteId(areaData.NoteId);
        }

        data.recentTrend = '';
        if (isDefined(groupRoot.RecentTrends)) {
            const recentTrends: TrendMarkerResult = groupRoot.RecentTrends[areaData.AreaCode];
            if (isDefined(recentTrends)) {
                data.recentTrend = new TrendMarkerLabelProvider(groupRoot.PolarityId).getLabel(recentTrends.Marker);
            }
        }

        data.comparedToEnglandValueOrPercentiles = CsvDataHelper.getSignificanceValue(areaData,
            groupRoot.PolarityId, ComparatorIds.National, groupRoot.ComparatorMethodId);

        data.comparedToRegionValueOrPercentiles = CsvDataHelper.getSignificanceValue(areaData,
            groupRoot.PolarityId, ComparatorIds.SubNational, groupRoot.ComparatorMethodId);

        data.timePeriodSortable = new TimePeriod(groupRoot.Grouping[0]).getSortableNumber();

        const hasDataChanged = this.ftHelperService.hasDataChanged(groupRoot);
        const isNewData = NewDataBadgeHelper.isNewData(hasDataChanged);
        data.newData = isNewData ? 'New data' : '';

        data.comparedToGoal = CsvDataHelper.getSignificanceValue(areaData, groupRoot.PolarityId,
            ComparatorIds.Target, groupRoot.ComparatorMethodId);

        return data;
    }
}
