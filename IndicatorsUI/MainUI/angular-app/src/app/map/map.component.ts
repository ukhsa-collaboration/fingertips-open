import {
  Component,
  HostListener,
  ChangeDetectorRef
} from '@angular/core';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { CoreDataHelperService } from '../shared/service/helper/coreDataHelper.service';
import { AreaService } from '../shared/service/api/area.service';
import {
  Colour,
  AreaTypeIds,
  ProfileIds,
  ComparatorMethodIds,
  PolarityIds,
  ComparatorIds
} from '../shared/shared';
import {
  FTModel,
  FTRoot,
  Area,
  GroupRoot,
  CoreDataSet,
  IndicatorMetadata,
  ComparisonConfig,
  ParentAreaType
} from '../typings/FT.d';
import * as _ from 'underscore';
import { PageType } from 'app/shared/component/legend/legend.component';

@Component({
  selector: 'ft-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent {
  isInitialised = false;
  map: google.maps.Map;
  areaTypeId: number = null;
  coreDataSet: CoreDataSet;
  sortedCoreData: Map<string, CoreDataSet>;
  currentAreaCode: string;
  counter: number;

  // Legend display flags
  pageType = PageType.Map;
  showRAG3 = false;
  showRAG5 = false;
  showBOB = false;
  showQuartiles = false;
  showQuintilesRAG = false;
  showQuintilesBOB = false;
  showContinuous = false;
  showAdhocKey = false;

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

  constructor(
    private ftHelperService: FTHelperService,
    private indicatorService: IndicatorService,
    private coreDataHelper: CoreDataHelperService,
    private ref: ChangeDetectorRef,
    private areaService: AreaService
  ) {
    this.ftModel = this.ftHelperService.getFTModel();
    this.profileId = this.ftModel.profileId;
  }

  @HostListener('window:MapSelected', [
    '$event',
    '$event.detail.searchMode'
  ])

  public onOutsideEvent(event, searchMode) {

    // Determine new area type and whether search mode
    this.searchModeNoDisplay = false;
    this.searchMode = false;
    let newAreaTypeId;
    if (searchMode !== undefined && searchMode) {
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
      let parentAreaTypes = <ParentAreaType[]>result;
      if (parentAreaTypes != null) {
        for (let areaType of parentAreaTypes) {
          if (areaType.Id === newAreaTypeId) {
            this.isBoundaryNotSupported = !areaType.CanBeDisplayedOnMap;
            break;
          }
        }
      }

      this.initMap(newAreaTypeId);
    });
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
      const currentGrpRoot: GroupRoot = this.ftHelperService.getCurrentGroupRoot();

      let currentComparator: Area;
      currentComparator = this.ftHelperService.getCurrentComparator();
      this.comparatorId = this.ftHelperService.getComparatorId();
      this.benchmarkIndex = this.comparatorId;

      this.indicatorService
        .getSingleIndicatorForAllArea(
          currentGrpRoot.Grouping[0].GroupId,
          this.ftModel.areaTypeId,
          currentComparator.Code,
          this.ftModel.profileId,
          this.comparatorId,
          currentGrpRoot.IID,
          currentGrpRoot.Sex.Id,
          currentGrpRoot.Age.Id
        )
        .subscribe(
          data => {
            this.coreDataSet = <CoreDataSet>data;
            this.sortedCoreData = this.coreDataHelper.addOrderandPercentilesToData(
              this.coreDataSet
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
    let areaOrder = [];
    Object.keys(this.sortedCoreData).forEach(key => {
      let value: CoreDataSet = this.sortedCoreData[key];
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
    let sortedCoreData = this.sortedCoreData;
    let localAreaCodeColourValue: Map<string, MapColourData> = new Map();
    $.each(areaOrder, function (i, coreData) {
      let data = sortedCoreData[coreData.AreaCode];
      if (coreData.ValF === '-') {
        let colourData: MapColourData = new MapColourData();
        colourData.order = -1;
        colourData.orderFrac = -1;
        localAreaCodeColourValue.set(coreData.AreaCode, colourData);
      } else {
        let colourData: MapColourData = new MapColourData();
        colourData.order = numAreas - j;
        colourData.orderFrac = 1 - j / numAreas;
        let position = numAreas + 1 - j + 1;
        colourData.quartile = Math.ceil(position / (numAreas / 4));
        colourData.quintile = Math.ceil(position / (numAreas / 5));
        j++;
        localAreaCodeColourValue.set(coreData.AreaCode, colourData);
      }
    });
    this.areaCodeColourValue = localAreaCodeColourValue;
  }

  showHideAdHocKey(): void {
    const currentGrpRoot: GroupRoot = this.ftHelperService.getCurrentGroupRoot();
    const indicatorMetadata: IndicatorMetadata = this.ftHelperService.getMetadata(
      currentGrpRoot.IID
    );

    this.comparisonConfig = this.ftHelperService.newComparisonConfig(
      currentGrpRoot,
      indicatorMetadata
    );
    if (this.comparisonConfig) {
      if (this.comparisonConfig.useTarget) {
        let targetLegend = this.ftHelperService.getTargetLegendHtml(
          this.comparisonConfig,
          indicatorMetadata
        );
        this.htmlAdhocKey =
          '<div><table class="key-table" style="width: 85%;height:50px;"><tr><td class="key-text">Benchmarked against goal:</td><td class="key-spacer"></td><td>' +
          targetLegend +
          '</td></tr></table></div>';
        this.showAdhocKey = true;
      } else {
        this.showAdhocKey = false;
        this.setBenchmarkLegendDisplay(currentGrpRoot);
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
    let index = this.selectedAreaList.indexOf(eventDetail.areaCode);

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
      this.ftHelperService.setComparatorId(this.benchmarkIndex);
    } else {
      this.loadData();
    }

    this.ref.detectChanges();
  }

  resetLegends() {
    this.showRAG3 = false;
    this.showRAG5 = false;
    this.showBOB = false;
    this.showQuartiles = false;
    this.showQuintilesRAG = false;
    this.showQuintilesBOB = false;
    this.showContinuous = false;
  }

  onMapColourBoxChange(selectedColour): void {
    // Reset legend flags
    this.resetLegends();

    switch (selectedColour) {
      case 'quartile': {
        this.showQuartiles = true;
        this.showAdhocKey = false;
        this.getQuartileColorData();
        break;
      }
      case 'quintile': {
        let root = this.ftHelperService.getCurrentGroupRoot();

        if (root.PolarityId === PolarityIds.NotApplicable) {
          this.showQuintilesBOB = true;
        } else {
          this.showQuintilesRAG = true;
        }

        this.showAdhocKey = false;

        this.getQuintileColorData(root);
        break;
      }
      case 'continuous': {
        this.showContinuous = true;
        this.showAdhocKey = false;
        this.getContinuousColorData();
        break;
      }
      case 'benchmark': {
        let root = this.ftHelperService.getCurrentGroupRoot();
        this.setBenchmarkLegendDisplay(root);
        this.showHideAdHocKey();
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
    switch (root.ComparatorMethodId) {
      case ComparatorMethodIds.Quintiles:
        if (root.PolarityId === PolarityIds.NotApplicable) {
          // Quintile BOB
          this.showQuintilesBOB = true;
        } else {
          // Quintile RAG
          this.showQuintilesRAG = true;
        }
        break;
      case ComparatorMethodIds.SingleOverlappingCIsForOneCiLevel:
        this.showRAG3 = true;
        break;
      case ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels:
        this.showRAG5 = true;
        break;
      default:
        this.setLegendDisplayByPolarity(root.PolarityId);
        break;
    }
  }

  setLegendDisplayByPolarity(polarityId): void {
    switch (polarityId) {
      case PolarityIds.BlueOrangeBlue:
        this.showBOB = true;
        break;
      case PolarityIds.RAGHighIsGood:
        this.showRAG3 = true;
        break;
      case PolarityIds.RAGLowIsGood:
        this.showRAG3 = true;
        break;
      default:
        break;
    }
  }

  getBenchMarkColorData(): void {
    const currentGrpRoot: GroupRoot = this.ftHelperService.getCurrentGroupRoot();
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
      let value: CoreDataSet = this.sortedCoreData[key];
      if (this.comparisonConfig !== undefined) {
        this.areaCodeSignificance[key] =
          value.Sig[this.comparisonConfig.comparatorId];
      } else {
        this.areaCodeSignificance[key] = value.Sig[globalComparatorId];
      }
    });

    let localAreaCodeColour: Map<string, string> = new Map();
    Object.keys(this.areaCodeSignificance).forEach(key => {
      let value: number = this.areaCodeSignificance[key];
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
        selectedGroupRoot,
        this.comparisonConfig,
        value
      );

      localAreaCodeColour.set(key, colour);
    });

    this.areaCodeColour = localAreaCodeColour;
  }

  getContinuousColorData(): void {
    let localAreaCodeColour: Map<string, string> = new Map();
    for (let key of Array.from(this.areaCodeColourValue.keys())) {
      let value: MapColourData = this.areaCodeColourValue.get(key);
      let colour = Colour.getColorForContinuous(value.orderFrac);
      localAreaCodeColour.set(key, colour);
    }
    this.areaCodeColour = localAreaCodeColour;
  }

  getQuintileColorData(root: GroupRoot): void {
    let localAreaCodeColour: Map<string, string> = new Map();
    for (let key of Array.from(this.areaCodeColourValue.keys())) {
      let value: MapColourData = this.areaCodeColourValue.get(key);
      let colour = Colour.getColorForQuintile(value.quintile, root.PolarityId);
      localAreaCodeColour.set(key, colour);
    }
    this.areaCodeColour = localAreaCodeColour;
  }

  getQuartileColorData(): void {
    let localAreaCodeColour: Map<string, string> = new Map();
    for (let key of Array.from(this.areaCodeColourValue.keys())) {
      let value: MapColourData = this.areaCodeColourValue.get(key);
      let colour = Colour.getColorForQuartile(value.quartile);
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
