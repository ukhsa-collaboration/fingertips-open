import { isDefined } from '@angular/compiler/src/util';
import * as _ from 'underscore';
import { forkJoin } from 'rxjs';
import { Component, ChangeDetectorRef, HostListener } from '@angular/core';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { NewDataBadgeHelper, SexAndAgeLabelHelper, MetadataHelper, TrendSourceHelper, ParentAreaHelper, SexHelper } from '../shared/shared';
import { AgeIds, SexIds, CategoryTypeIds, CategoryIds, PageType, Tabs } from '../shared/constants';
import { AreaCodes } from '../shared/constants'
import {
  CategoryDataManager, AgeDataManager, SexDataManager, CategoryDataAnalyser, AgeDataAnalyser, SexDataAnalyser,
  PreferredPartitionType, ViewModes, SexDataBuilder, BuilderData, AgeDataBuilder,
  TrendChartData, CategoryDataBuilder, TrendOption, TrendTableData, InequalityValueData, InequalityValueNote
} from './inequalities';
import { UIService } from '../shared/service/helper/ui.service';
import { LightBoxConfig, LightBoxTypes } from '../shared/component/light-box/light-box';
import { LegendConfig } from '../shared/component/legend/legend';

import {
  FTModel, FTConfig, FTData, GroupRoot, CoreDataSet, PartitionDataForAllCategories,
  PartitionDataForAllAges, PartitionDataForAllSexes, PartitionTrendData, CategoryType,
  IndicatorMetadata, ValueData, TrendMarkerResult, Age, Sex
} from '../typings/FT';
import { CsvConfig, CsvData, CsvDataHelper } from '../shared/component/export-csv/export-csv';
import { SignificanceFormatter } from '../shared/classes/significance-formatter';
import { TimePeriod } from '../shared/classes/time-period';
import { TrendMarkerLabelProvider } from '../shared/classes/trendmarker-label-provider';

@Component({
  selector: 'ft-inequalities',
  templateUrl: './inequalities.component.html',
  styleUrls: ['./inequalities.component.css']
})
export class InequalitiesComponent {

  private model: FTModel;
  private config: FTConfig;
  private data: FTData;

  private indicatorName: string;
  private selectedCategoryTypeId: number;
  private displayCategoryTypeId: number;
  private showConfidenceIntervalsLabel = false;
  private confidenceIntervalsLabel = 'Show confidence intervals';

  private isCategoryData = false;
  private isAgeData = false;
  private isSexData = false;

  private scrollTop: number;
  private isNationalTabButtonSelected = true;
  private trendsByCategories: PartitionTrendData[];
  private trendsByAge: PartitionTrendData;
  private trendsBySex: PartitionTrendData;
  private categoryDataManager: CategoryDataManager;
  private ageDataManager: AgeDataManager;
  private sexDataManager: SexDataManager;
  private preferredPartitionType = PreferredPartitionType.Category;
  private viewModes = ViewModes.LatestValues;
  private partitionOptions: CategoryType[];
  private partitionOptionsSex: any;
  private partitionOptionsAge: any;
  private trendOptions: TrendOption[] = [];
  private barChartData: BuilderData;
  private trendChartData: TrendChartData;
  private trendTableData: TrendTableData[] = [];
  private showBarChart: boolean;
  private groupRoot: GroupRoot;
  private metadata: IndicatorMetadata;
  private allAgesLookup: Age[];
  private allSexesLookup: Sex[];

  noData = false;
  areaName: string;
  selectedIndicatorName: string;
  selectedIndicatorAreaWithPeriod: string;
  selectedIndicatorNewDataBadge: string;
  selectedIndicatorUnit: string;
  latestValuesButtonClass = 'button-selected';
  trendsButtonClass = '';
  nationalButtonClass = 'button-selected';
  areaButtonClass = '';

  // Csv Config
  csvConfig: CsvConfig;

  // Legend
  legendConfig: LegendConfig;

  // Lightbox
  lightBoxConfig: LightBoxConfig;

  barChart: any;
  trendChart: any;

  constructor(private ftHelperService: FTHelperService,
    private indicatorService: IndicatorService,
    private uiService: UIService,
    private ref: ChangeDetectorRef) { }

  @HostListener('window:InequalitiesSelected', ['$event'])
  public onOutsideEvent(event) {

    const ftHelper = this.ftHelperService;

    this.model = ftHelper.getFTModel();
    this.config = ftHelper.getFTConfig();

    this.categoryDataManager = new CategoryDataManager();
    this.sexDataManager = new SexDataManager();
    this.ageDataManager = new AgeDataManager();

    this.areaName = ftHelper.getAreaName(this.model.areaCode);
    this.groupRoot = ftHelper.getCurrentGroupRoot();
    const iid = this.groupRoot.IID;
    this.indicatorName = ftHelper.getIndicatorName(iid);
    const metadataHash = this.ftHelperService.getMetadataHash();
    this.metadata = metadataHash[iid];

    this.loadData();
  }

  applyTrendHeader() {
    const groupRoot = this.groupRoot;
    const hasDataChanged = this.ftHelperService.hasDataChanged(groupRoot);
    const newDataBadge = NewDataBadgeHelper.getNewDataBadgeHtml(hasDataChanged);

    let unitLabel = '';
    const metadata = this.metadata;
    const valueType = metadata.ValueType.Name;
    if (metadata.Unit.Label !== '') {
      unitLabel = metadata.Unit.Label;
    }

    const sexAndAgeLabel = new SexAndAgeLabelHelper(groupRoot).getSexAndAgeLabel();
    this.selectedIndicatorName = this.indicatorName + sexAndAgeLabel;
    this.selectedIndicatorNewDataBadge = newDataBadge;
    this.selectedIndicatorUnit = valueType + ' - ' + unitLabel;
    this.setIndicatorAreaWithPeriodLabel();
  }

  setIndicatorAreaWithPeriodLabel(): void {
    const grouping = this.groupRoot.Grouping[0];
    let period = '';
    if (isDefined(grouping)) {
      period = grouping.Period;
    }

    if (this.isNationalTabButtonSelected) {
      this.selectedIndicatorAreaWithPeriod = this.getSelectedAreaName();
    } else {
      this.selectedIndicatorAreaWithPeriod = this.areaName;
    }

    if (this.viewModes === ViewModes.LatestValues && period.trim().length > 0) {
      this.selectedIndicatorAreaWithPeriod = this.selectedIndicatorAreaWithPeriod + ', ' + period;
    }
  }

  loadData() {
    const groupRoot = this.groupRoot;
    const profileId = this.model.profileId;
    const areaCode = this.getSelectedAreaCode();
    const indicatorId = groupRoot.IID;
    const areaTypeId = this.model.areaTypeId;
    const sexId = groupRoot.Sex.Id;
    const ageId = groupRoot.Age.Id;

    const allCategoriesObservable = this.indicatorService.getDataForAllCategories(profileId, areaCode,
      indicatorId, areaTypeId, sexId, ageId);
    const allAgesObservable = this.indicatorService.getDataForAllAges(profileId, areaCode, indicatorId, areaTypeId, sexId);
    const allSexesObservable = this.indicatorService.getDataForAllSexes(profileId, areaCode, indicatorId, areaTypeId, ageId);
    const trendsByCategoriesObservable = this.indicatorService.getTrendsByCategories(profileId, areaCode,
      indicatorId, areaTypeId, sexId, ageId);
    const trendsByAgeObservable = this.indicatorService.getTrendsByAge(profileId, areaCode, indicatorId, areaTypeId, sexId);
    const trendsBySexObservable = this.indicatorService.getTrendsBySex(profileId, areaCode, indicatorId, areaTypeId, ageId);

    forkJoin([allCategoriesObservable, allAgesObservable, allSexesObservable, trendsByCategoriesObservable,
      trendsByAgeObservable, trendsBySexObservable]).subscribe(results => {
        const allCategories: PartitionDataForAllCategories = results[0];
        const allAges: PartitionDataForAllAges = results[1];
        const allSexes: PartitionDataForAllSexes = results[2];
        this.trendsByCategories = results[3];
        this.trendsByAge = results[4];
        this.trendsBySex = results[5];

        this.applyTrendHeader();

        const categoryDataAnalyser = new CategoryDataAnalyser(allCategories);
        this.categoryDataManager.setData(groupRoot, areaCode, areaTypeId, categoryDataAnalyser);

        const sexDataAnalyser = new SexDataAnalyser(allSexes);
        this.sexDataManager.setData(groupRoot, areaCode, areaTypeId, sexDataAnalyser);

        const ageDataAnalyser = new AgeDataAnalyser(allAges);
        this.ageDataManager.setData(groupRoot, areaCode, areaTypeId, ageDataAnalyser);

        if (categoryDataAnalyser.categoryTypesWithData.length > 0) {
          // Set the default category id
          this.selectedCategoryTypeId = categoryDataAnalyser.categoryTypesWithData[0].Id;
          this.displayCategoryTypeId = this.selectedCategoryTypeId;
        }

        this.determinePartitionToDisplay();

        this.ref.detectChanges();

        // Scroll top
        this.scrollTop = this.uiService.getScrollTop();
        this.uiService.setScrollTop(this.scrollTop);

        // Unlock UI
        this.ftHelperService.showAndHidePageElements();
        this.ftHelperService.hideBenchmarkBox();
        this.ftHelperService.unlock();
      });
  }

  determinePartitionToDisplay() {
    const groupRoot = this.groupRoot;
    const areaCode = this.getSelectedAreaCode();
    const areaTypeId = this.model.areaTypeId;

    const categoryDataAnalyser = this.categoryDataManager.getData(groupRoot, areaCode, areaTypeId);
    const sexDataAnalyser = this.sexDataManager.getData(groupRoot, areaCode, areaTypeId);
    const ageDataAnalyser = this.ageDataManager.getData(groupRoot, areaCode, areaTypeId);

    this.isCategoryData = categoryDataAnalyser.isAnyData();
    this.isAgeData = ageDataAnalyser.isAnyData();
    this.isSexData = sexDataAnalyser.isAnyData();

    const isAnyData = this.isCategoryData || this.isAgeData || this.isSexData;

    let categoryTypeId = this.selectedCategoryTypeId;
    if (!isDefined(categoryTypeId) && this.isCategoryData) {
      categoryTypeId = categoryDataAnalyser.categoryTypesWithData[0].Id;
      this.selectedCategoryTypeId = categoryTypeId;
    }

    this.displayCategoryTypeId = this.selectedCategoryTypeId;
    this.partitionOptions = categoryDataAnalyser.categoryTypesWithData;
    this.partitionOptionsSex = sexDataAnalyser;
    this.partitionOptionsAge = ageDataAnalyser;

    if (isAnyData) {
      this.noData = false;
      switch (this.preferredPartitionType) {
        case PreferredPartitionType.Age:
          if (this.isAgeData) {
            this.selectAge();
          } else {
            this.preferredPartitionType = PreferredPartitionType.Category;
            this.selectCategoryType();
          }
          break;
        case PreferredPartitionType.Sex:
          if (this.isSexData) {
            this.selectSex();
          } else {
            this.preferredPartitionType = PreferredPartitionType.Category;
            this.selectCategoryType();
          }
          break;
        default:
          this.preferredPartitionType = PreferredPartitionType.Category;
          if (this.isAgeData && !this.isCategoryData) {
            this.selectAge();
          } else if (this.isSexData && !this.isCategoryData) {
            this.selectSex();
          } else {
            this.selectCategoryType();
          }
          break;
      }

      this.setLegendDisplay();
    } else {
      // No data
      this.noData = true;
    }
  }

  selectCategoryType() {
    if (this.viewModes === ViewModes.LatestValues) {

      const groupRoot = this.groupRoot;
      const areaCode = this.getSelectedAreaCode();
      const areaTypeId = this.model.areaTypeId;
      const average = this.getAverage();
      const selectedAreaName = this.getSelectedAreaName();
      const metadata = this.metadata;
      const comparisonConfig = this.ftHelperService.newComparisonConfig(groupRoot, metadata);
      const partitionName = this.partitionOptions.find(x => x.Id === this.selectedCategoryTypeId).Name;
      const timePeriod = groupRoot.Grouping[0].Period;
      const useSpecialCaseSocioeconomicGroup = this.shouldUseSpecialCaseSocioeconomicGroup();

      const categoryDataAnalyser = this.categoryDataManager.getData(groupRoot, areaCode, areaTypeId);
      const categoryDataBuilder = new CategoryDataBuilder(this.ftHelperService, categoryDataAnalyser,
        this.selectedCategoryTypeId, average, groupRoot, comparisonConfig, selectedAreaName, metadata.Unit,
        this.showConfidenceIntervalsLabel, this.selectedIndicatorName, partitionName, timePeriod,
        useSpecialCaseSocioeconomicGroup);

      this.barChartData = null;
      this.ref.detectChanges();

      this.barChartData = categoryDataBuilder.getData();
      this.showBarChart = true;
    } else {
      this.generateTrendData();
      this.showBarChart = false;
    }
  }

  selectAge() {
    this.displayCategoryTypeId = null;
    this.preferredPartitionType = PreferredPartitionType.Age;

    const allAgesLookupObservable = this.indicatorService.getAllAges();
    forkJoin([allAgesLookupObservable]).subscribe(results => {
      this.allAgesLookup = results[0];

      if (this.viewModes === ViewModes.LatestValues) {
        const groupRoot = this.groupRoot;
        const areaCode = this.getSelectedAreaCode();
        const areaTypeId = this.model.areaTypeId;
        const metadata = this.metadata;
        const comparisonConfig = this.ftHelperService.newComparisonConfig(groupRoot, metadata);
        const selectedAreaName = this.getSelectedAreaName();
        const partitionName = 'Age';
        const timePeriod = groupRoot.Grouping[0].Period;

        const ageDataAnalyser = this.ageDataManager.getData(groupRoot, areaCode, areaTypeId);
        const ageDataBuilder = new AgeDataBuilder(this.ftHelperService, ageDataAnalyser,
          groupRoot, comparisonConfig, selectedAreaName, metadata.Unit, this.selectedIndicatorName,
          partitionName, timePeriod);

        const data = ageDataBuilder.getData();

        this.barChartData = null;
        this.ref.detectChanges();

        this.barChartData = data;
        this.showBarChart = true;
      } else {
        this.generateTrendData();
        this.showBarChart = false;
      }
    });
  }

  selectSex() {
    this.displayCategoryTypeId = null;
    this.preferredPartitionType = PreferredPartitionType.Sex;

    const allSexesLookupObservable = this.indicatorService.getAllSexes();
    forkJoin([allSexesLookupObservable]).subscribe(results => {
      this.allSexesLookup = results[0];

      if (this.viewModes === ViewModes.LatestValues) {
        const groupRoot = this.ftHelperService.getCurrentGroupRoot();
        const areaCode = this.getSelectedAreaCode();
        const areaTypeId = this.model.areaTypeId;
        const metadata = this.metadata;
        const comparisonConfig = this.ftHelperService.newComparisonConfig(groupRoot, metadata);
        const selectedAreaName = this.getSelectedAreaName();
        const partitionName = 'Sex';
        const timePeriod = groupRoot.Grouping[0].Period;

        const sexDataAnalyser = this.sexDataManager.getData(groupRoot, areaCode, areaTypeId);
        const sexDataBuilder = new SexDataBuilder(this.ftHelperService, sexDataAnalyser, groupRoot,
          comparisonConfig, selectedAreaName, metadata.Unit, this.selectedIndicatorName,
          partitionName, timePeriod);
        const data: BuilderData = sexDataBuilder.getData();

        // Assess whether there is sex data to display
        const isNoAverageOrMaleFemale = data.averageDataSeries.length === 0 && data.dataSeries.length === 0;
        const isNoAverageAndMaleFemaleInvalid = data.averageDataSeries.length === 0 &&
          data.dataSeries.length > 0 && data.dataSeries[0].y === 0;
        const isAverageButItIsZero = data.averageDataSeries.length > 1 &&
          data.averageDataSeries[0].y === 0 && data.averageDataSeries[1].y === 0;

        if (isNoAverageOrMaleFemale || isNoAverageAndMaleFemaleInvalid || isAverageButItIsZero) {
          // No data
          this.noData = true;
        } else {
          this.noData = false;

          // Create bar chart
          this.barChartData = null;
          this.ref.detectChanges();

          this.barChartData = data;
          this.showBarChart = true;
        }
      } else {
        this.generateTrendData();
        this.showBarChart = false;
      }
    });
  }

  generateTrendData() {
    const groupRoot = this.groupRoot;
    const profileId = this.model.profileId;
    const areaCode = this.getSelectedAreaCode();
    const indicatorId = groupRoot.IID;
    const areaTypeId = this.model.areaTypeId;
    const sexId = groupRoot.Sex.Id;
    const ageId = groupRoot.Age.Id;

    const trendsByCategoriesObservable = this.indicatorService.getTrendsByCategories(
      profileId, areaCode, indicatorId, areaTypeId, sexId, ageId);
    const trendsByAgeObservable = this.indicatorService.getTrendsByAge(profileId, areaCode, indicatorId, areaTypeId, sexId);
    const trendsBySexObservable = this.indicatorService.getTrendsBySex(profileId, areaCode, indicatorId, areaTypeId, ageId);

    forkJoin([trendsByCategoriesObservable, trendsByAgeObservable, trendsBySexObservable]).subscribe(results => {
      this.trendsByCategories = results[0];
      this.trendsByAge = results[1];
      this.trendsBySex = results[2];

      this.displayTrends();
    });
  }

  displayTrends() {
    let partitionTrendData: PartitionTrendData = null;

    // For trends, show all the checkboxes checked by default
    this.setAllTrendsCheckboxState(true);

    switch (this.preferredPartitionType) {
      case PreferredPartitionType.Age:
        partitionTrendData = this.trendsByAge;
        break;

      case PreferredPartitionType.Sex:
        partitionTrendData = this.trendsBySex;
        break;

      case PreferredPartitionType.Category:
        for (let counter = 0; counter < this.trendsByCategories.length; counter++) {
          if (partitionTrendData === null) {
            for (let i = 1; i <= _.size(this.trendsByCategories[counter].TrendData); i++) {
              const trendData: CoreDataSet[] = this.trendsByCategories[counter].TrendData[i];
              trendData.forEach(data => {
                if (isDefined(data) && data) {
                  if (partitionTrendData === null && data.CategoryTypeId === this.selectedCategoryTypeId) {
                    partitionTrendData = this.trendsByCategories[counter];
                  }
                }
              });
            }
          }
        }

        break;
    }

    if (partitionTrendData.Limits) {
      let average: any;
      let hasAverage: boolean;

      switch (this.preferredPartitionType) {
        case PreferredPartitionType.Sex:

          // Check if person exists
          const person = _.findWhere(partitionTrendData.Labels, { Id: SexIds.Person });
          if (isDefined(person)) {
            average = person;
            hasAverage = true;
          } else {
            hasAverage = false;
          }

          this.setTrendOptions(partitionTrendData);
          break;

        case PreferredPartitionType.Age:
          const allAges = _.findWhere(partitionTrendData.Labels, { Id: AgeIds.AllAges });
          if (isDefined(allAges)) {
            average = allAges;
            hasAverage = true;
          } else {
            hasAverage = false;
          }

          this.trendOptions.length = 0;
          const allAgesIndex = partitionTrendData.Labels.findIndex(x => x.Id === AgeIds.AllAges);
          if (allAgesIndex !== -1) {
            const allAgesTrendOption = new TrendOption();
            allAgesTrendOption.NamedIdentity = partitionTrendData.Labels.find(x => x.Id === AgeIds.AllAges);
            allAgesTrendOption.OptionSelected = true;
            this.trendOptions.push(allAgesTrendOption);

            partitionTrendData.Labels.forEach(label => {
              if (label.Id !== AgeIds.AllAges) {
                const trendOption = new TrendOption();
                trendOption.NamedIdentity = label;
                trendOption.OptionSelected = true;

                this.trendOptions.push(trendOption);
              }
            });
          } else {
            this.setTrendOptions(partitionTrendData);
          }
          break;

        default:
          if (_.size(partitionTrendData.AreaAverage) > 0 && !this.shouldUseSpecialCaseSocioeconomicGroup()) {
            hasAverage = true;
            // Selected area id will always be zero
            average = { Id: 0, Name: this.getSelectedAreaName() };
          } else {
            hasAverage = false;
          }

          this.setTrendOptions(partitionTrendData);
          break;
      }

      this.createTrendChartData(partitionTrendData);
      this.createTrendTableData(partitionTrendData);
    }
  }

  createTrendChartData(partitionTrendData: PartitionTrendData) {
    const metadataHelper = new MetadataHelper(this.ftHelperService);
    const unitLabel = metadataHelper.getMetadataUnitShortLabel();
    const unit = metadataHelper.getMetadataUnit();
    let chartHeight = 350;
    const heightPerLegend = 18;
    const seriesData = [];

    for (let i = 0; i < partitionTrendData.Labels.length; i++) {
      const key = partitionTrendData.Labels[i].Id;
      const trendData: CoreDataSet[] = partitionTrendData.TrendData[key];
      const points = [];
      const cis = [];

      trendData.forEach(data => {
        if (data === null || data.Val === -1) {
          points.push(null);
          if (this.showConfidenceIntervalsLabel) {
            cis.push(null);
          }
        } else {
          const inequalityTrendValueData = new InequalityValueData(this.ftHelperService, data.ValF,
            data.NoteId, data.Count, data.Val, unitLabel);
          points.push({
            y: data.Val, valF: inequalityTrendValueData.ValF, noteValue: inequalityTrendValueData.Note.Text,
            tooltipValue: inequalityTrendValueData.ValFWithUnitLabel
          });
          if (this.showConfidenceIntervalsLabel) {
            const dataInfo = this.ftHelperService.newCoreDataSetInfo(data);
            if (dataInfo.areCIs()) {
              cis.push([data.LoCI, data.UpCI]);
            }
          }
        }
      });

      // Points
      const label = partitionTrendData.Labels.find(x => x.Id === Number(key)).Name;
      seriesData.push({ data: points, name: label, visible: true });

      // CIs
      if (this.showConfidenceIntervalsLabel) {
        seriesData.push({ data: cis, name: 'Errorbars', type: 'errorbar', animation: false });
      }
    }

    const avg = this.areaTrendSeriesData(partitionTrendData);
    if (avg !== null) {
      // Only show area average if we have data
      if (_.size(avg.data) > 0 && !this.shouldUseSpecialCaseSocioeconomicGroup()) {
        seriesData.splice(0, 0, avg);
      }
    }

    // Adjust chart height to allow for different legend sizes
    const noOfPartitions = _.size(seriesData);
    if (noOfPartitions > 0) {
      chartHeight = chartHeight + (noOfPartitions * heightPerLegend);
    }

    // Determine whether y-axis can start with zero
    const canStartZeroYAxis = this.canAxisStartAtZero(seriesData);

    const trendChartData = new TrendChartData();
    trendChartData.min = canStartZeroYAxis ? 0 : partitionTrendData.Limits.Min;
    trendChartData.max = partitionTrendData.Limits.Max;
    trendChartData.width = 480;
    trendChartData.height = chartHeight;
    trendChartData.heightPerLegend = heightPerLegend;
    trendChartData.unit = new MetadataHelper(this.ftHelperService).getMetadataUnit();
    trendChartData.seriesData = seriesData;
    trendChartData.periods = partitionTrendData.Periods;
    trendChartData.unit = unit;
    trendChartData.indicatorName = this.selectedIndicatorName;
    trendChartData.areaName = this.getSelectedAreaName();
    trendChartData.partitionName = this.getSelectedPartitionName();
    trendChartData.trendData = partitionTrendData.TrendData;

    this.trendChartData = null;
    this.ref.detectChanges();
    this.trendChartData = trendChartData;
  }

  createTrendTableData(partitionTrendData: PartitionTrendData) {
    const rows: TrendTableData[] = [];
    const metadataHelper = new MetadataHelper(this.ftHelperService);
    const unitLabel = metadataHelper.getMetadataUnitShortLabel();
    const trendSource = new TrendSourceHelper(this.metadata).getTrendSource();

    for (let i = 0; i < partitionTrendData.Labels.length; i++) {
      const period: string[] = [];
      const valF: string[] = [];
      let trendDataCounter = 0;
      const key = partitionTrendData.Labels[i].Id;
      const trendData: CoreDataSet[] = partitionTrendData.TrendData[key];

      trendData.forEach(data => {
        if (data === null) {
          valF.push('-');
        } else {
          valF.push(data.ValF);
        }
        period.push(partitionTrendData.Periods[trendDataCounter]);
        trendDataCounter++;
      });

      const row = new TrendTableData();
      row.label = partitionTrendData.Labels.find(x => x.Id === Number(key)).Name;
      row.unitLabel = unitLabel;
      row.trendSource = trendSource;
      row.valueData = this.getValueData(trendData);
      row.periods = period;
      rows.push(row);
    }

    this.trendTableData = null;
    this.trendTableData = rows;
  }

  getSelectedPartitionName(): string {
    switch (this.preferredPartitionType) {
      case PreferredPartitionType.Age:
        return 'Age';
      case PreferredPartitionType.Sex:
        return 'Sex';
      case PreferredPartitionType.Category:
        const partitionOption = this.partitionOptions.find(x => x.Id === this.selectedCategoryTypeId);
        if (partitionOption !== undefined) {
          return partitionOption.Name;
        }
        return '';
    }
  }

  getValueData(trendData: CoreDataSet[]): ValueData[] {
    return trendData.map(data => { return this.buildValueDataObject(data); });
  }

  buildValueDataObject(data: CoreDataSet): ValueData {
    if (isDefined(data)) {
      return {
        ValF: data.ValF,
        Note: new InequalityValueNote(this.ftHelperService, data.NoteId),
        Val: data.Val,
        Count: data.Count
      };
    } else {
      return { ValF: null, Note: new InequalityValueNote(this.ftHelperService, null), Val: null, Count: null };
    }
  }

  canAxisStartAtZero(seriesData) {
    const canStartZeroYAxis = this.config.startZeroYAxis;
    if (canStartZeroYAxis) {
      if (seriesData.length > 0) {
        for (let i = 0; i < seriesData.length; i++) {
          const dataList = seriesData[i].data;
          if (dataList) {
            for (let j = 0; j < dataList.length; j++) {
              if (dataList[j] && dataList[j].y < 0) {
                return false;
              }
            }
          }
        }
      }
    }
    return canStartZeroYAxis;
  }

  areaTrendSeriesData(partitionTrendData: PartitionTrendData): any {
    if (partitionTrendData.AreaAverage !== null) {
      const points = [];
      const areaData = this.ftHelperService.newCoreDataSetInfo(partitionTrendData.AreaAverage[0]);

      partitionTrendData.AreaAverage.forEach(data => {
        if (data.Val !== -1) {
          const valueData = new InequalityValueData(this.ftHelperService, data.ValF, data.NoteId, data.Count, data.Val);
          points.push({ y: valueData.Val, valF: valueData.ValF, noteValue: valueData.Note.Text, tooltipValue: valueData.ValF });
        } else {
          points.push(null);
        }
      });

      const seriesData = {
        data: points,
        name: this.getSelectedAreaName(),
        marker: { symbol: 'triangle' },
        color: '#000'
      };
      return seriesData;
    }

    return null;
  }

  shouldUseSpecialCaseSocioeconomicGroup() {
    const categoryTypeId = this.selectedCategoryTypeId;
    const specialCases = this.getIndicatorSpecialCases();

    return specialCases !== null &&
      categoryTypeId === CategoryTypeIds.SocioeconomicGroup &&
      specialCases.indexOf('inequalityBenchmark_UseAgeId:183') > -1 &&
      specialCases.indexOf('inequalityBenchmark_ForCategoryTypeId:59') > -1;
  }

  selectedLatestValues(isLatestValuesTabButtonSelected: boolean) {
    if (isLatestValuesTabButtonSelected) {
      this.viewModes = ViewModes.LatestValues;
      this.latestValuesButtonClass = 'button-selected';
      this.trendsButtonClass = '';
    } else {
      this.viewModes = ViewModes.Trends;
      this.latestValuesButtonClass = '';
      this.trendsButtonClass = 'button-selected';
    }

    const groupRoot = this.ftHelperService.getCurrentGroupRoot();
    this.setIndicatorAreaWithPeriodLabel();

    this.determinePartitionToDisplay();
  }

  selectNational(isNationalTabButtonSelected: boolean) {

    if (!this.ftHelperService.isLocked()) {

      this.isNationalTabButtonSelected = isNationalTabButtonSelected;

      if (this.viewModes === ViewModes.LatestValues) {
        this.preferredPartitionType = PreferredPartitionType.Category;
      }

      if (isNationalTabButtonSelected) {
        this.nationalButtonClass = 'button-selected';
        this.areaButtonClass = '';
      } else {
        this.nationalButtonClass = '';
        this.areaButtonClass = 'button-selected';
      }

      this.ftHelperService.lock();
      this.loadData();

      // For trends, show all the checkboxes checked by default
      if (this.viewModes === ViewModes.Trends) {
        this.setAllTrendsCheckboxState(true);
      }
    }
  }

  getIndicatorSpecialCases() {
    return (<any>this.metadata).SpecialCases;
  }

  getSelectedAreaCode(): string {
    if (this.isNationalTabButtonSelected) {
      return AreaCodes.England;
    } else {
      return this.model.areaCode;
    }
  }

  getSelectedAreaName(): string {
    if (this.isNationalTabButtonSelected) {
      return 'England';
    } else {
      const areaCode = this.model.areaCode;
      const areaName = this.ftHelperService.getAreaName(areaCode);

      return areaName;
    }
  }

  getAverage(): CoreDataSet {
    let average: CoreDataSet;
    const groupRoot = this.groupRoot;

    if (this.isNationalTabButtonSelected) {
      average = this.ftHelperService.getNationalComparatorGrouping(groupRoot).ComparatorData;
    } else {
      const areaCode = this.model.areaCode;
      for (let counter = 0; counter < groupRoot.Data.length; counter++) {
        if (groupRoot.Data[counter].AreaCode === areaCode) {
          average = groupRoot.Data[counter];
          break;
        }
      }
    }

    return average;
  }

  selectPartitionOption(categoryType: CategoryType) {
    this.selectedCategoryTypeId = categoryType.Id;
    this.displayCategoryTypeId = categoryType.Id;
    this.preferredPartitionType = PreferredPartitionType.Category;

    this.determinePartitionToDisplay();
  }

  selectTrendPartitionOption(categoryType: CategoryType) {
    this.selectedCategoryTypeId = categoryType.Id;
    this.displayCategoryTypeId = categoryType.Id;
    this.preferredPartitionType = PreferredPartitionType.Category;

    // For trends, show all the checkboxes checked by default
    this.setAllTrendsCheckboxState(true);

    this.displayTrends();
  }

  // Display legend
  setLegendDisplay() {
    const config = new LegendConfig(PageType.Inequalities, this.ftHelperService);
    config.configureForOneIndicator(this.groupRoot);
    this.legendConfig = config;
  }

  showCategoryTypeDescription(categoryType: CategoryType) {
    // Show light box
    const config = new LightBoxConfig();
    config.Type = LightBoxTypes.Ok;
    config.Title = categoryType.Name;
    config.Html = categoryType.Description;
    config.Height = 370;
    this.lightBoxConfig = config;
  }

  hideOrShowAllTrends(show: boolean) {
    const temp = this.trendChartData;

    temp.seriesData.forEach(series => {
      series.visible = show;
    });

    this.setAllTrendsCheckboxState(show);

    // Trigger chart refresh
    this.trendChartData = null;
    this.ref.detectChanges();
    this.trendChartData = temp;
  }

  toggleTrend(option: TrendOption) {
    const seriesName = option.NamedIdentity.Name;

    const temp = this.trendChartData;

    temp.seriesData.forEach(series => {
      if (series.name === seriesName) {
        series.visible = !series.visible;
        option.OptionSelected = !option.OptionSelected;
      }
    });

    // Trigger chart refresh
    this.trendChartData = null;
    this.ref.detectChanges();
    this.trendChartData = temp;
  }

  hideOrShowTrendOption(option: TrendOption) {
    if (this.preferredPartitionType === PreferredPartitionType.Sex && option.NamedIdentity.Id === 4) {
      return true;
    }

    if (this.preferredPartitionType === PreferredPartitionType.Age && option.NamedIdentity.Id === 1) {
      return true;
    }

    return false;
  }

  updateLightBoxActionConfirmed(actionConfirmed: boolean) {
  }

  showErrorBar() {
    this.showConfidenceIntervalsLabel = !this.showConfidenceIntervalsLabel;

    if (this.showConfidenceIntervalsLabel) {
      this.confidenceIntervalsLabel = 'Hide confidence intervals';
    } else {
      this.confidenceIntervalsLabel = 'Show confidence intervals';
    }

    this.determinePartitionToDisplay();
  }

  goToMetadataPage() {
    const indicatorIndex = this.ftHelperService.getIndicatorIndex();
    this.ftHelperService.goToMetadataPage(indicatorIndex);
  }

  exportSelectedChartCsvFile() {
    let csvData: CsvData[] = [];

    if (this.viewModes === ViewModes.LatestValues) {
      csvData = this.generateLatestValuesCSV();
    } else {
      csvData = this.generateTrendsCSV();
    }

    this.csvConfig = new CsvConfig();
    this.csvConfig.tab = Tabs.Inequalities;
    this.csvConfig.csvData = csvData;
  }

  generateLatestValuesCSV(): CsvData[] {
    const csvData: CsvData[] = [];

    const barChartData = this.barChartData;

    if (this.partitionOptions) {

      for (let counter = 0; counter < barChartData.dataList.length; counter++) {
        const data = this.addCsvRow(counter);
        csvData.push(data);
      }
    }

    return csvData;
  }

  generateTrendsCSV(): CsvData[] {
    const csvData: CsvData[] = [];

    const trendChartData = this.trendChartData;

    switch (this.preferredPartitionType) {
      case PreferredPartitionType.Category:
        for (let seriesDataCounter = 0; seriesDataCounter < trendChartData.seriesData.length; seriesDataCounter++) {
          for (let trendDataCounter = 0; trendDataCounter < trendChartData.trendData[seriesDataCounter + 1].length; trendDataCounter++) {
            const data = this.addTrendCsvRow(seriesDataCounter, trendDataCounter, trendChartData.seriesData[seriesDataCounter].name);
            if (isDefined(data)) {
              csvData.push(data);
            }
          }
        }
        break;

      case PreferredPartitionType.Sex:
        for (let seriesDataCounter = 0; seriesDataCounter < trendChartData.seriesData.length; seriesDataCounter++) {
          const sex = this.allSexesLookup.find(x => x.Name === trendChartData.seriesData[seriesDataCounter].name);
          for (let trendDataCounter = 0; trendDataCounter < trendChartData.trendData[sex.Id].length; trendDataCounter++) {
            const sexData = this.addTrendCsvRow(sex.Id, trendDataCounter, sex.Name);
            if (isDefined(sexData)) {
              csvData.push(sexData);
            }
          }
        }
        break;

      case PreferredPartitionType.Age:
        for (let seriesDataCounter = 0; seriesDataCounter < trendChartData.seriesData.length; seriesDataCounter++) {
          const age = this.allAgesLookup.find(x => x.Name === trendChartData.seriesData[seriesDataCounter].name);
          for (let trendDataCounter = 0; trendDataCounter < trendChartData.trendData[age.Id].length; trendDataCounter++) {
            const ageData = this.addTrendCsvRow(age.Id, trendDataCounter, age.Name);
            if (isDefined(csvData)) {
              csvData.push(ageData);
            }
          }
        }

        break;
    }

    return csvData;
  }

  addCsvRow(index: number): CsvData {
    const csvData = new CsvData();

    const root = this.groupRoot;
    const barChartData = this.barChartData;
    let data: CoreDataSet;
    let label: string;

    switch (this.preferredPartitionType) {
      case PreferredPartitionType.Category:
        data = barChartData.dataList[index];
        break;

      case PreferredPartitionType.Sex:
        const sexId = barChartData.dataList[index].SexId;
        data = barChartData.dataList.find(x => x.SexId === sexId);
        label = SexHelper.getSexLabel(sexId);
        break;

      case PreferredPartitionType.Age:
        const ageId = barChartData.dataList[index].AgeId;
        data = barChartData.dataList.find(x => x.AgeId === ageId);
        label = this.allAgesLookup.find(x => x.Id === ageId).Name;
        break;
    }

    csvData.indicatorId = this.ftHelperService.getIid().toString();
    csvData.indicatorName = barChartData.indicatorName;

    if (this.isNationalTabButtonSelected) {
      csvData.parentCode = '';
      csvData.parentName = '';
      csvData.areaCode = AreaCodes.England;
      csvData.areaName = 'England';
      csvData.areaType = 'England';
    } else {
      const parentAreaHelper = new ParentAreaHelper(this.ftHelperService);
      csvData.parentCode = parentAreaHelper.getParentAreaCode();
      csvData.parentName = parentAreaHelper.getParentAreaNameForCSV();
      csvData.areaCode = this.model.areaCode;
      csvData.areaName = barChartData.areaName;
      csvData.areaType = this.ftHelperService.getAreaTypeName();
    }

    switch (this.preferredPartitionType) {
      case PreferredPartitionType.Category:
        csvData.sex = root.Sex.Name;
        csvData.age = root.Age.Name;
        break;

      case PreferredPartitionType.Sex:
        csvData.sex = label === undefined ? 'Persons' : label;
        csvData.age = root.Age.Name;
        break;

      case PreferredPartitionType.Age:
        csvData.sex = root.Sex.Name;
        csvData.age = label === undefined ? 'All ages' : label;
        break;
    }

    if (this.preferredPartitionType === PreferredPartitionType.Sex ||
      this.preferredPartitionType === PreferredPartitionType.Age) {
      csvData.categoryType = '';
      csvData.category = '';
    } else {
      csvData.categoryType = barChartData.partitionName;
      csvData.category = barChartData.labels[index];
    }

    csvData.timePeriod = barChartData.timePeriod;
    csvData.value = isDefined(data.Val) && data.Val !== -1 ? data.Val.toString() : '';
    csvData.lowerCiLimit95 = isDefined(data.LoCI) && data.LoCI !== -1 ? data.LoCI.toString() : '';
    csvData.upperCiLimit95 = isDefined(data.UpCI) && data.UpCI !== -1 ? data.UpCI.toString() : '';
    csvData.lowerCiLimit99_8 = isDefined(data.LoCI99_8) && data.LoCI99_8 !== -1 ? data.LoCI99_8.toString() : '';
    csvData.upperCiLimit99_8 = isDefined(data.UpCI99_8) && data.UpCI99_8 !== -1 ? data.UpCI99_8.toString() : '';
    csvData.count = isDefined(data.Count) && data.Count !== -1 ? data.Count.toString() : '';
    csvData.denominator = isDefined(data.Denom) && data.Denom !== -1 ? data.Denom.toString() : '';

    csvData.valueNote = this.getNoteId(data);

    csvData.recentTrend = '';
    const areaCode = this.model.areaCode;
    const recentTrend: TrendMarkerResult = root.RecentTrends[areaCode];
    if (isDefined(recentTrend)) {
      csvData.recentTrend = new TrendMarkerLabelProvider(root.PolarityId).getLabel(recentTrend.Marker);
    }

    csvData.comparedToEnglandValueOrPercentiles = '';
    if (this.isNationalTabButtonSelected && isDefined(data.Significance)) {
      csvData.comparedToEnglandValueOrPercentiles = new SignificanceFormatter(root.PolarityId,
        root.ComparatorMethodId).getLabel(Number(data.Significance));
    }

    csvData.comparedToRegionValueOrPercentiles = '';
    if (!this.isNationalTabButtonSelected && isDefined(data.Significance)) {
      csvData.comparedToRegionValueOrPercentiles = new SignificanceFormatter(root.PolarityId,
        root.ComparatorMethodId).getLabel(Number(data.Significance));
    }

    csvData.timePeriodSortable = new TimePeriod(root.Grouping[0]).getSortableNumber();
    csvData.newData = root.DateChanges && root.DateChanges.HasDataChangedRecently ? 'New data' : '';
    csvData.comparedToGoal = '';

    return csvData;
  }

  addTrendCsvRow(seriesDataCounter: number, trendDataCounter: number, seriesDataName: string): CsvData {
    const csvData = new CsvData();

    const root = this.groupRoot;
    const trendChartData = this.trendChartData;
    const data = trendChartData.trendData;

    let coreData: any;
    if (this.preferredPartitionType === PreferredPartitionType.Sex ||
      this.preferredPartitionType === PreferredPartitionType.Age) {
      coreData = data[seriesDataCounter][trendDataCounter];
    } else {
      coreData = data[seriesDataCounter + 1][trendDataCounter];
    }

    if (!isDefined(coreData)) {
      return null;
    }

    csvData.indicatorId = this.ftHelperService.getIid().toString();
    csvData.indicatorName = trendChartData.indicatorName;

    if (this.isNationalTabButtonSelected) {
      csvData.parentCode = '';
      csvData.parentName = '';
      csvData.areaCode = AreaCodes.England;
      csvData.areaName = 'England';
      csvData.areaType = 'England';
    } else {
      const parentAreaHelper = new ParentAreaHelper(this.ftHelperService);
      csvData.parentCode = parentAreaHelper.getParentAreaCode();
      csvData.parentName = parentAreaHelper.getParentAreaNameForCSV();
      csvData.areaCode = this.model.areaCode;
      csvData.areaName = trendChartData.areaName;
      csvData.areaType = this.ftHelperService.getAreaTypeName();
    }

    switch (this.preferredPartitionType) {
      case PreferredPartitionType.Category:
        csvData.sex = root.Sex.Name;
        csvData.age = root.Age.Name;
        break;

      case PreferredPartitionType.Sex:
        csvData.sex = seriesDataName;
        csvData.age = root.Age.Name;
        break;

      case PreferredPartitionType.Age:
        csvData.sex = root.Sex.Name;
        csvData.age = seriesDataName;
        break;
    }

    if (this.preferredPartitionType === PreferredPartitionType.Sex ||
      this.preferredPartitionType === PreferredPartitionType.Age) {
      csvData.categoryType = '';
      csvData.category = '';
    } else {
      csvData.categoryType = trendChartData.partitionName;
      csvData.category = trendChartData.seriesData[seriesDataCounter].name;
    }

    csvData.timePeriod = trendChartData.periods[trendDataCounter];
    csvData.value = CsvDataHelper.getDisplayValue(coreData.Val);
    csvData.lowerCiLimit95 = CsvDataHelper.getDisplayValue(coreData.LoCI);
    csvData.upperCiLimit95 = CsvDataHelper.getDisplayValue(coreData.UpCI);
    csvData.lowerCiLimit99_8 = CsvDataHelper.getDisplayValue(coreData.LoCI99_8);
    csvData.upperCiLimit99_8 = CsvDataHelper.getDisplayValue(coreData.UpCI99_8);
    csvData.count = CsvDataHelper.getDisplayValue(coreData.Count);
    csvData.denominator = CsvDataHelper.getDisplayValue(coreData.Denom);

    csvData.valueNote = this.getNoteId(coreData);

    csvData.recentTrend = '';
    const areaCode = this.model.areaCode;
    const recentTrend: TrendMarkerResult = root.RecentTrends[areaCode];
    if (isDefined(recentTrend)) {
      csvData.recentTrend = new TrendMarkerLabelProvider(root.PolarityId).getLabel(recentTrend.Marker);
    }

    csvData.comparedToEnglandValueOrPercentiles = '';
    if (this.isNationalTabButtonSelected && isDefined(coreData.Significance)) {
      csvData.comparedToEnglandValueOrPercentiles = new SignificanceFormatter(root.PolarityId,
        root.ComparatorMethodId).getLabel(Number(coreData.Significance));
    }

    csvData.comparedToRegionValueOrPercentiles = '';
    if (!this.isNationalTabButtonSelected && isDefined(coreData.Significance)) {
      csvData.comparedToRegionValueOrPercentiles = new SignificanceFormatter(root.PolarityId,
        root.ComparatorMethodId).getLabel(Number(coreData.Significance));
    }

    csvData.timePeriodSortable = new TimePeriod(root.Grouping[0]).getSortableNumber();
    csvData.newData = root.DateChanges && root.DateChanges.HasDataChangedRecently ? 'New data' : '';
    csvData.comparedToGoal = '';

    return csvData;
  }

  getNoteId(coreData: any): string {
    if (isDefined(coreData.NoteId)) {
      return this.ftHelperService.newValueNoteTooltipProvider().getTextFromNoteId(
        coreData.NoteId).replace('* ', '');
    }
    return '';
  }

  getPartitionIdSelected() {
    return this.getSelectedPartition().text();
  }

  getSelectedPartition() {
    return $('ft-inequalities a.selected');
  }

  getInequalitiesListForValues(partitionSelectedId, indicatorid) {
    const areaCode = this.getSelectedAreaCode();
    const areaTypeId = this.model.areaTypeId;
    const groupRoot = this.ftHelperService.getCurrentGroupRoot();

    const categoryDataAnalyser = this.categoryDataManager.getData(groupRoot, areaCode, areaTypeId);
    const ageDataAnalyser = this.ageDataManager.getData(groupRoot, areaCode, areaTypeId);
    const sexDataAnalyser = this.sexDataManager.getData(groupRoot, areaCode, areaTypeId);

    if (partitionSelectedId === 'Age') {
      const dictionaryInequality = ageDataAnalyser.ages.map(age => ({ Age: age.Id, Sex: groupRoot.Sex.Id }));
      return JSON.stringify({ [indicatorid]: dictionaryInequality });
    }

    if (partitionSelectedId === 'Sex') {
      const dictionaryInequality = sexDataAnalyser.sexData.map(item => ({ Sex: item.SexId, Age: groupRoot.Age.Id }));
      return JSON.stringify({ [indicatorid]: dictionaryInequality });
    }

    const list = categoryDataAnalyser.categoryTypes
      .map(categoryType => categoryType.Categories
        .map(category => ({
          CategoryTypeId: category.CategoryTypeId,
          CategoryId: category.Id,
          Sex: groupRoot.Sex.Id,
          Age: groupRoot.Age.Id
        })));
    const categoryIdList = _.flatten(list);

    const filteredValues = categoryIdList.filter(x => x.CategoryTypeId === this.selectedCategoryTypeId);

    // Adding general inequality
    filteredValues.push({
      Sex: groupRoot.Sex.Id,
      Age: groupRoot.Age.Id,
      CategoryTypeId: CategoryTypeIds.Undefined,
      CategoryId: CategoryIds.Undefined
    });

    return JSON.stringify({ [indicatorid]: filteredValues });
  }

  getInequalitiesListForTrends(partitionSelectedId, indicatorid) {
    const groupRoot = this.ftHelperService.getCurrentGroupRoot();
    const selectedInequalities = this.getSelectedInequalitiesChartBoxesIdArray();
    let dictionaryInequality;

    if (partitionSelectedId === 'Age') {
      dictionaryInequality = selectedInequalities.map(elem => ({ Age: elem.NamedIdentity.Id, Sex: groupRoot.Sex.Id }));
      return JSON.stringify({ [indicatorid]: dictionaryInequality });
    }
    if (partitionSelectedId === 'Sex') {
      dictionaryInequality = selectedInequalities.map(elem => ({ Sex: elem.NamedIdentity.Id, Age: groupRoot.Age.Id }));
      return JSON.stringify({ [indicatorid]: dictionaryInequality });
    }

    dictionaryInequality = selectedInequalities.map(elem => ({
      CategoryTypeId: this.selectedCategoryTypeId,
      CategoryId: elem.NamedIdentity.Id,
      Sex: groupRoot.Sex.Id,
      Age: groupRoot.Age.Id
    }));
    return JSON.stringify({ [indicatorid]: dictionaryInequality });
  }

  getCategoriesAreaCode() {
    const partitionSelectedId = this.getPartitionIdSelected();

    if (partitionSelectedId === 'Age' || partitionSelectedId === 'Sex') {
      return null;
    }

    let categoriesAreaCode;
    let selectedInequalities;

    if (this.viewModes === ViewModes.Trends) {
      selectedInequalities = this.getSelectedInequalitiesChartBoxesIdArray();

      categoriesAreaCode = selectedInequalities.map(elem => 'cat-' + this.selectedCategoryTypeId + '-' + elem.NamedIdentity.Id);

    } else {
      const areaCode = this.getSelectedAreaCode();
      const areaTypeId = this.model.areaTypeId;
      const categoryDataAnalyser = this.categoryDataManager.getData(this.groupRoot, areaCode, areaTypeId);

      const list = categoryDataAnalyser.categoryTypes
        .map(categoryType => categoryType.Categories
          .map(category => ({
            CategoryTypeId: category.CategoryTypeId,
            CategoryId: category.Id
          })));
      const categoryIdList = _.flatten(list);

      const filteredValues = categoryIdList.filter(x => x.CategoryTypeId === this.selectedCategoryTypeId);
      categoriesAreaCode = filteredValues.map(elem => 'cat-' + this.selectedCategoryTypeId + '-' + elem.CategoryId);
    }

    return categoriesAreaCode.join(',');
  }

  getSelectedInequalitiesChartBoxesIdArray() {
    const selected = [];

    this.trendOptions.forEach((element) => {
      if (element.OptionSelected) {
        selected.push(element);
      }
    });

    return selected;
  }

  setAllTrendsCheckboxState(state) {
    this.trendOptions.forEach(trendOption => {
      trendOption.OptionSelected = state;
    });
  }

  setTrendOptions(partitionTrendData: PartitionTrendData) {
    // Reset trend options array
    this.trendOptions.length = 0;

    // populate trend options array
    partitionTrendData.Labels.forEach(label => {
      const trendOption = new TrendOption();
      trendOption.NamedIdentity = label;
      trendOption.OptionSelected = true;

      this.trendOptions.push(trendOption);
    });
  }

  setBarChart(event) {
    this.barChart = event.chart;
  }

  setTrendChart(event) {
    this.trendChart = event.chart;
  }

  exportChart() {
    switch (this.viewModes) {
      case ViewModes.LatestValues:
        this.barChart.exportChart({ type: 'image/png' }, {});
        break;

      case ViewModes.Trends:
        this.trendChart.exportChart({ type: 'image/png' }, {});
        break;
    }
  }
}
