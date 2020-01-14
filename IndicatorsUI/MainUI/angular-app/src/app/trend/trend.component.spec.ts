import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TrendComponent } from './trend.component';
import { TrendChartComponent } from './trend-chart/trend-chart.component';
import { TrendTableComponent } from './trend-table/trend-table.component';
import { LegendComponent } from '../shared/component/legend/legend.component';
import { LegendCompareAreasComponent } from '../shared/component/legend/legend-compare-areas/legend-compare-areas.component';
import { LegendInequalitiesComponent } from '../shared/component/legend/legend-inequalities/legend-inequalities.component';
import { LegendMapComponent } from '../shared/component/legend/legend-map/legend-map.component';
import { LegendRecentTrendsComponent } from '../shared/component/legend/legend-recent-trends/legend-recent-trends.component';
import { LegendTrendComponent } from '../shared/component/legend/legend-trend/legend-trend.component';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { UIService } from '../shared/service/helper/ui.service';
import { DownloadService } from '../shared/service/api/download.service';
import { LegendDataQualityComponent } from '../shared/component/legend/legend-data-quality/legend-data-quality.component';
import {
  LegendBenchmarkAgainstGoalComponent
} from '../shared/component/legend/legend-benchmark-against-goal/legend-benchmark-against-goal.component';
import { FTIndicatorSearch, IndicatorIdList } from '../typings/FT';
import { ExportCsvComponent } from '../shared/component/export-csv/export-csv.component';

const groupId = 1;
const areaTypeId = 1;
const areaCode = 'areaCodeTest';
const parentCode = 'parentCodeTest';
const profileId = 1;
const indicatorId = 1;
const areaName = 'AreaNameTest';

describe('TrendComponent', () => {
  let component: TrendComponent;
  let fixture: ComponentFixture<TrendComponent>;

  let ftHelperService: any;
  let indicatorService: any;
  let uiService: any;
  let downloadService: any;

  beforeEach(async(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService',
      ['getSearch', 'getIid', 'getFTModel', 'getFTConfig', 'getFTData', 'getCurrentGroupRoot',
        'getAreaName', 'getParentAreaCode'
      ]);
    indicatorService = jasmine.createSpyObj('IndicatorService', ['getTrendDataForAllIndicatorsInProfileGroupForChildAreas',
      'getTrendDataForSpecificIndicatorsForChildAreas']);
    downloadService = jasmine.createSpyObj('DownloadService', ['']);
    uiService = jasmine.createSpyObj('UIService', ['']);

    TestBed.configureTestingModule({
      declarations: [
        TrendComponent,
        TrendChartComponent,
        TrendTableComponent,
        LegendComponent,
        LegendCompareAreasComponent,
        LegendInequalitiesComponent,
        LegendMapComponent,
        LegendRecentTrendsComponent,
        LegendTrendComponent,
        LegendDataQualityComponent,
        LegendBenchmarkAgainstGoalComponent,
        ExportCsvComponent
      ],
      providers: [
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: IndicatorService, useValue: indicatorService },
        { provide: DownloadService, useValue: downloadService },
        { provide: UIService, useValue: uiService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TrendComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call getTrendDataForAllIndicatorsInProfileGroupForChildAreas', () => {

    // Arrange
    ftHelperService.getSearch.and.returnValue(new FTIndicatorSearchTestSupport(false));
    ftHelperService.getIid.and.returnValue(indicatorId);

    // Act
    component.getDataTrends(groupId, areaTypeId, parentCode, profileId);

    // Arrange
    expect(indicatorService.getTrendDataForAllIndicatorsInProfileGroupForChildAreas).toHaveBeenCalled();

  });

  // it('should call getTrendDataForSpecificIndicatorsForChildAreas', () => {

  //   // Arrange
  //   ftHelperService.getSearch.and.returnValue(new FTIndicatorSearchTestSupport(true));
  //   ftHelperService.getIid.and.returnValue(indicatorId);

  //   // Act
  //   component.getDataTrends(groupId, areaTypeId, parentCode, profileId);

  //   // Arrange
  //   expect(indicatorService.getTrendDataForSpecificIndicatorsForChildAreas).toHaveBeenCalled();

  // });

  it('should call getDataTrends', () => {

    ftHelperService.getFTModel.and.returnValue(model);
    ftHelperService.getCurrentGroupRoot.and.returnValue(currentGroupRoot);
    ftHelperService.getAreaName.and.returnValue(areaName);

    const getDataTrendsSpy = spyOn(component, 'getDataTrends');

    component.onOutsideEvent({});

    expect(getDataTrendsSpy).toHaveBeenCalled();
  });

});

export class FTIndicatorSearchTestSupport implements FTIndicatorSearch {

  isSeachModeDefault = false;
  isIndicatorListDefault = true;
  indicatorListIdDefault = 1;
  indicatorIdsParameterDefault = '';
  profileIdsForSearchDefault: Array<number> = [];

  constructor(isSeachModeDefault: boolean) {
    this.isSeachModeDefault = isSeachModeDefault;
  }

  isInSearchMode = function () {
    return this.isSeachModeDefault;
  }
  getIndicatorListId = function () {
    return this.indicatorListIdDefault;
  }
  isIndicatorList = function () {
    return this.isIndicatorListDefault;
  }
  getIndicatorIdList = function () {
    return new IndicatorIdListTestSupport();
  }
  getIndicatorIdsParameter = function () {
    return this.indicatorIdsParameterDefault;
  }
  getProfileIdsForSearch = function () {
    return this.profileIdsForSearchDefault;
  }
}

export class IndicatorIdListTestSupport implements IndicatorIdList {

  allIdsDefault: Array<number> = [];

  getAllIds = function () {
    return this.allIdsDefault;
  }
}

const currentGroupRoot = {
  Grouping: [
    {
      GroupId: groupId
    }]
}

const model = {
  areaTypeId: areaTypeId,
  groupId: null,
  parentCode: parentCode,
  profileId: profileId,
  parentTypeId: null,
  areaCode: areaCode,
  iid: null,
  ageId: null,
  sexId: null,
  nearestNeighbour: null,
  groupRoots: currentGroupRoot,
  filterIndicatorPeriod: null,
  yAxisSelectedIndicatorId: null,
  isNearestNeighbours: function () {
    return true;
  }
}

