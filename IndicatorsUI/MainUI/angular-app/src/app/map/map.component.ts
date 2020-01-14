import { Component, HostListener, ChangeDetectorRef } from '@angular/core';
import * as _ from 'underscore';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { CoreDataHelperService } from '../shared/service/helper/coreDataHelper.service';
import { AreaService } from '../shared/service/api/area.service';
import { Colour, DeviceServiceHelper } from '../shared/shared';
import {
  AreaTypeIds, ProfileIds, ComparatorMethodIds, PolarityIds, ComparatorIds, PageType
} from '../shared/constants';
import { LegendConfig } from '../shared/component/legend/legend';
import { CoreDataSet, Area, ComparisonConfig, FTModel, ParentAreaType, GroupRoot, IndicatorMetadata } from '../typings/FT';
import { isDefined } from '@angular/compiler/src/util';
import { DeviceDetectorService } from '../../../node_modules/ngx-device-detector';

@Component({
  selector: 'ft-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent {

  isEnglandAreaSelected: boolean;
  isInitialised = false;
  map: google.maps.Map;
  areaTypeId: number = null;
  coreDataSets: CoreDataSet[];
  sortedCoreData: Map<string, CoreDataSet>;
  currentAreaCode: string;
  counter: number;
  selectedAreaList: Array<string> = new Array<string>();
  areaCodeSignificance: Map<string, number> = new Map();
  nationalArea: Map<string, Area> = new Map();
  comparisonConfig: ComparisonConfig;
  areaCodeColour: Map<string, string> = new Map();
  areaCodeColourValue: Map<string, MapColourData> = new Map();
  refreshColour = 0;
  htmlAdhocKey = '';
  IsPracticeAreaType = false;
  isBoundaryNotSupported = false;
  mapColourSelectedValue = 'benchmark';
  updateMap: boolean;
  searchMode: boolean;
  searchModeNoDisplay: boolean;
  profileId: number;
  ftModel: FTModel;
  comparatorId: number;
  benchmarkIndex: number;
  subNationalTabButtonClicked = false;
  legendConfig: LegendConfig;
  currentGrpRoot: GroupRoot;
  isAnyData = true;
  isBrowserIE = false;

  constructor(
    private ftHelperService: FTHelperService,
    private indicatorService: IndicatorService,
    private coreDataHelper: CoreDataHelperService,
    private ref: ChangeDetectorRef,
    private areaService: AreaService,
    private deviceService: DeviceDetectorService
  ) { }

  @HostListener('window:MapSelected', [
    '$event',
    '$event.detail.searchMode',
    '$event.detail.isEnglandAreaType'
  ])
  public onOutsideEvent(event, searchMode, isEnglandAreaType) {

    // Map does not display on IE11 due to Angular 8 upgrade
    // TODO: rewrite code to use @agm/core
    if (new DeviceServiceHelper(this.deviceService).isIE()) {
      this.isAnyData = false;
      this.isBrowserIE = true;

      this.ftHelperService.showAndHidePageElements();
      this.ftHelperService.unlock();
      this.ref.detectChanges();

      return;
    }

    this.isEnglandAreaSelected = isEnglandAreaType;
    this.isAnyData = true;

    this.ftModel = this.ftHelperService.getFTModel();
    this.profileId = this.ftModel.profileId;

    this.currentGrpRoot = this.ftHelperService.getCurrentGroupRoot();

    this.legendConfig = new LegendConfig(PageType.Map, this.ftHelperService);

    // Determine new area type and whether search mode
    this.searchModeNoDisplay = false;
    this.searchMode = false;
    let newAreaTypeId;
    if (isDefined(searchMode) && searchMode) {
      // In search mode
      if (this.profileId === ProfileIds.PracticeProfile) {
        // Practice profiles search
        this.searchMode = true;
      } else {
        // HOW can get here???
        this.searchModeNoDisplay = true;
      }
      newAreaTypeId = AreaTypeIds.Practice;
    } else {
      // Not in search mode
      newAreaTypeId = this.ftModel.areaTypeId;
    }

    this.areaService.getParentAreas(this.profileId).subscribe((result: any) => {

      // Determine whether current area type can be shown on map
      const parentAreaTypes = <ParentAreaType[]>result;
      if (parentAreaTypes != null) {
        for (const areaType of parentAreaTypes) {
          if (areaType.Id === newAreaTypeId) {
            this.isBoundaryNotSupported = !areaType.CanBeDisplayedOnMap;
            break;
          }
        }
      }

      this.initMap(newAreaTypeId);
    });
  }

  @HostListener('window:NoDataDisplayed', ['$event', '$event.detail.isEnglandAreaType'])
  public refreshVariable(isEnglandAreaType) {
    this.isAnyData = false;
    this.isEnglandAreaSelected = isEnglandAreaType;
  }

  initMap(newAreaTypeId: number): void {

    this.isInitialised = true;
    this.updateMap = true;
    this.ref.detectChanges();

    // Clear area list of area type changes
    if (this.areaTypeId !== newAreaTypeId) {
      this.selectedAreaList = new Array<string>();
    }

    this.areaTypeId = newAreaTypeId;
    this.IsPracticeAreaType = newAreaTypeId === AreaTypeIds.Practice;
    if (!this.isBoundaryNotSupported) {
      // Have boundaries so can load data
      this.loadData();
    } else {
      // No boundaries for current area type
      this.ftHelperService.showAndHidePageElements();
      this.ftHelperService.unlock();
      this.ref.detectChanges();
    }

    this.updateMap = false;
  }

  loadData(): void {
    if (this.IsPracticeAreaType) {
      // Don't need to display any data for practices
      if (!this.searchMode && !this.searchModeNoDisplay) {
        this.showElementsAndUnlock();
      }
    } else {
      // Get data for all other area types
      const currentGrpRoot: GroupRoot = this.currentGrpRoot;

      this.comparatorId = this.ftHelperService.getComparatorId();
      this.benchmarkIndex = this.comparatorId;

      let comparatorCode = '';
      if (this.ftModel.isNearestNeighbours() && this.comparatorId === ComparatorIds.SubNational) {
        comparatorCode = this.ftModel.nearestNeighbour;
      } else {
        comparatorCode = this.ftHelperService.getCurrentComparator().Code;
      }

      this.indicatorService
        .getSingleIndicatorForAllAreas(
          currentGrpRoot.Grouping[0].GroupId,
          this.ftModel.areaTypeId,
          comparatorCode,
          this.ftModel.profileId,
          this.comparatorId,
          currentGrpRoot.IID,
          currentGrpRoot.Sex.Id,
          currentGrpRoot.Age.Id
        )
        .subscribe(
          data => {
            this.coreDataSets = <CoreDataSet[]>data;
            this.sortedCoreData = this.coreDataHelper.addOrderandPercentilesToData(
              this.coreDataSets
            );
            this.loadColourData();
            this.onMapColourBoxChange(this.mapColourSelectedValue);
            this.ref.detectChanges();
            this.showElementsAndUnlock();
          },
          error => { }
        );
    }
  }

  showElementsAndUnlock() {
    this.ftHelperService.showAndHidePageElements();
    this.ftHelperService.showTargetBenchmarkOption(
      this.ftHelperService.getAllGroupRoots()
    );
    this.ftHelperService.unlock();
  }

  loadColourData(): void {
    const areaOrder = [];
    Object.keys(this.sortedCoreData).forEach(key => {
      const value: CoreDataSet = this.sortedCoreData[key];
      areaOrder.push({ AreaCode: key, Val: value.Val, ValF: value.ValF });
    });
    areaOrder
      .sort(
        (leftside, rightside): number => {
          if (leftside.Val < rightside.Val) {
            return -1;
          }
          if (leftside.Val > rightside.Val) {
            return 1;
          }
          return 0;
        }
      )
      .reverse();
    let numAreas = 0;
    $.each(areaOrder, function (i, coreData) {
      if (coreData.ValF !== '-') {
        numAreas++;
      }
    });
    let j = 0;
    const sortedCoreData = this.sortedCoreData;
    const localAreaCodeColourValue: Map<string, MapColourData> = new Map();
    $.each(areaOrder, function (i, coreData) {
      const data = sortedCoreData[coreData.AreaCode];
      if (coreData.ValF === '-') {
        const colourData: MapColourData = new MapColourData();
        colourData.order = -1;
        colourData.orderFrac = -1;
        localAreaCodeColourValue.set(coreData.AreaCode, colourData);
      } else {
        const colourData: MapColourData = new MapColourData();
        colourData.order = numAreas - j;
        colourData.orderFrac = 1 - j / numAreas;
        const position = numAreas + 1 - j + 1;
        colourData.quartile = Math.ceil(position / (numAreas / 4));
        colourData.quintile = Math.ceil(position / (numAreas / 5));
        j++;
        localAreaCodeColourValue.set(coreData.AreaCode, colourData);
      }
    });
    this.areaCodeColourValue = localAreaCodeColourValue;
  }

  canDisplayBenchmarkAgainstGoalLegend(root: GroupRoot): void {
    const indicatorMetadata: IndicatorMetadata = this.ftHelperService.getMetadata(
      root.IID
    );

    this.comparisonConfig = this.ftHelperService.newComparisonConfig(
      root,
      indicatorMetadata
    );
    if (this.comparisonConfig) {
      if (this.comparisonConfig.useTarget) {
        const targetLegend = this.ftHelperService.getTargetLegendHtml(
          this.comparisonConfig,
          indicatorMetadata
        );

        this.legendConfig.configureBenchmarkAgainstGoal(true, targetLegend);
      }
    }
  }

  onMapInit(mapInfo: { map: google.maps.Map }) {
    this.map = mapInfo.map;
  }

  onhoverAreaCodeChangedMap(eventDetail: { areaCode: string }) {
    this.currentAreaCode = eventDetail.areaCode;
    this.ref.detectChanges();
  }

  onSelectedAreaCodeChanged(eventDetail: { areaCode: string }) {

  }

  onhoverAreaCodeChangedChart(eventDetail: { areaCode: string }) {
    this.currentAreaCode = eventDetail.areaCode;
    this.ref.detectChanges();
  }

  onhoverAreaCodeChangedData(eventDetail: { areaCode: string }) {
    this.currentAreaCode = eventDetail.areaCode;
    this.ref.detectChanges();
  }

  onBoundaryNotSupported(eventDetail) {
    this.isBoundaryNotSupported = true;
    this.ref.detectChanges();
  }

  onSelectedAreaChanged(eventDetail: { areaCode: string }) {
    const index = this.selectedAreaList.indexOf(eventDetail.areaCode);

    if (index > -1) {
      this.selectedAreaList.splice(index, 1);
    } else {
      this.selectedAreaList.push(eventDetail.areaCode);
    }
    this.selectedAreaList = this.selectedAreaList.slice();

    this.ref.detectChanges();
  }

  OnBenchMarkIndexChanged(eventDetail: { benchmarkIndex: number }) {
    this.benchmarkIndex = eventDetail.benchmarkIndex;

    if (this.benchmarkIndex === ComparatorIds.National) {
      this.subNationalTabButtonClicked = false;
      this.ftHelperService.setComparatorId(this.benchmarkIndex);
    } else {
      this.subNationalTabButtonClicked = true;
      this.loadData();
    }

    this.ref.detectChanges();
  }

  resetLegends() {
    this.legendConfig = new LegendConfig(PageType.Map, this.ftHelperService);
  }

  onMapColourBoxChange(selectedColour): void {
    // Reset legend flags
    this.resetLegends();

    switch (selectedColour) {
      case 'quartile': {
        this.getQuartileColorData();
        break;
      }
      case 'quintile': {
        this.getQuintileColorData();
        break;
      }
      case 'continuous': {
        this.legendConfig.showContinuous = true;
        this.legendConfig.showBenchmarkAgainstGoal = false;
        this.getContinuousColorData();
        break;
      }
      case 'benchmark': {
        const root = this.ftHelperService.getCurrentGroupRoot();
        this.setBenchmarkLegendDisplay(root);
        this.canDisplayBenchmarkAgainstGoalLegend(root);
        this.getBenchMarkColorData();
        break;
      }
      default: {
        break;
      }
    }
    this.refreshColour++;
    this.ref.detectChanges();
  }

  setBenchmarkLegendDisplay(root: GroupRoot) {
    const polarityId = root.PolarityId;

    switch (root.ComparatorMethodId) {
      case ComparatorMethodIds.Quintiles:
        if (polarityId === PolarityIds.NotApplicable) {
          // Quintile BOB
          this.legendConfig.showQuintilesBOB = true;
        } else {
          // Quintile RAG
          this.legendConfig.showQuintilesRAG = true;
        }
        break;
      case ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels:
        if (polarityId === PolarityIds.RAGLowIsGood || polarityId === PolarityIds.RAGHighIsGood) {
          this.legendConfig.showRAG5 = true;
        } else if (polarityId === PolarityIds.BlueOrangeBlue) {
          this.legendConfig.showBOB5 = true;
        }
        break;
      default:
        this.setLegendDisplayByPolarity(root.PolarityId);
        break;
    }
  }

  setLegendDisplayByPolarity(polarityId): void {
    switch (polarityId) {
      case PolarityIds.BlueOrangeBlue:
        this.legendConfig.showBOB3 = true;
        break;
      case PolarityIds.RAGHighIsGood:
        this.legendConfig.showRAG3 = true;
        break;
      case PolarityIds.RAGLowIsGood:
        this.legendConfig.showRAG3 = true;
        break;
      default:
        break;
    }
  }

  getBenchMarkColorData(): void {
    const currentGrpRoot: GroupRoot = this.currentGrpRoot;
    const indicatorMetadata: IndicatorMetadata = this.ftHelperService.getMetadata(
      currentGrpRoot.IID
    );
    this.comparisonConfig = this.ftHelperService.newComparisonConfig(
      currentGrpRoot,
      indicatorMetadata
    );

    const globalComparatorId = this.ftHelperService.getComparatorId();
    this.areaCodeSignificance = new Map();

    Object.keys(this.sortedCoreData).forEach(key => {
      const value: CoreDataSet = this.sortedCoreData[key];
      if (isDefined(this.comparisonConfig)) {
        this.areaCodeSignificance[key] =
          value.Sig[this.comparisonConfig.comparatorId];
      } else {
        this.areaCodeSignificance[key] = value.Sig[globalComparatorId];
      }
    });

    const localAreaCodeColour: Map<string, string> = new Map();
    Object.keys(this.areaCodeSignificance).forEach(key => {
      const value: number = this.areaCodeSignificance[key];
      let colour: string;
      let selectedGroupRoot: GroupRoot;
      // If the use target is clicked change the
      // polarity of currentGroup to target polarity
      if (this.comparisonConfig.useTarget) {
        selectedGroupRoot = Object.create(currentGrpRoot);
        selectedGroupRoot.PolarityId = indicatorMetadata.Target.PolarityId;
      } else {
        selectedGroupRoot = currentGrpRoot;
      }

      colour = Colour.getSignificanceColorForBenchmark(
        selectedGroupRoot.ComparatorMethodId,
        selectedGroupRoot.PolarityId,
        this.comparisonConfig,
        value
      );

      localAreaCodeColour.set(key, colour);
    });

    this.areaCodeColour = localAreaCodeColour;
  }

  getContinuousColorData(): void {
    const localAreaCodeColour: Map<string, string> = new Map();
    for (const key of Array.from(this.areaCodeColourValue.keys())) {
      const value: MapColourData = this.areaCodeColourValue.get(key);
      const colour = Colour.getContinuousColorForMap(value.orderFrac);
      localAreaCodeColour.set(key, colour);
    }
    this.areaCodeColour = localAreaCodeColour;
  }

  getQuintileColorData(): void {
    const root = this.currentGrpRoot;
    if (root.ComparatorMethodId === ComparatorMethodIds.Quintiles) {
      this.setBenchmarkLegendDisplay(root);
      this.getBenchMarkColorData();

      return;
    }

    this.legendConfig.showQuintilesBOB = true;
    this.legendConfig.showBenchmarkAgainstGoal = false;

    const localAreaCodeColour: Map<string, string> = new Map();
    for (const key of Array.from(this.areaCodeColourValue.keys())) {
      const value: MapColourData = this.areaCodeColourValue.get(key);
      const colour = Colour.getQuintileColorForMap(value.quintile);
      localAreaCodeColour.set(key, colour);
    }

    this.areaCodeColour = localAreaCodeColour;
  }

  getQuartileColorData(): void {
    const root = this.currentGrpRoot;
    if (root.ComparatorMethodId === ComparatorMethodIds.Quartiles) {
      this.setBenchmarkLegendDisplay(root);
      this.getBenchMarkColorData();

      return;
    }

    this.legendConfig.showQuartiles = true;
    this.legendConfig.showBenchmarkAgainstGoal = false;

    const localAreaCodeColour: Map<string, string> = new Map();
    for (const key of Array.from(this.areaCodeColourValue.keys())) {
      const value: MapColourData = this.areaCodeColourValue.get(key);
      const colour = Colour.getQuartileColorForMap(value.quartile);
      localAreaCodeColour.set(key, colour);
    }

    this.areaCodeColour = localAreaCodeColour;
  }
}

class MapColourData {
  quartile: number;
  quintile: number;
  orderFrac: number;
  order: number;
}
