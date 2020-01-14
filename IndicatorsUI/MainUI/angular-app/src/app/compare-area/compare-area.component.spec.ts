// import { async, ComponentFixture, TestBed } from '@angular/core/testing';
// import { CompareAreaComponent } from './compare-area.component';
// import { CompareAreaChartComponent } from './compare-area-chart/compare-area-chart.component';
// import { CompareAreaTableComponent } from './compare-area-table/compare-area-table.component';
// import { LegendComponent } from '../shared/component/legend/legend.component';
// import { LegendCompareAreasComponent } from '../shared/component/legend/legend-compare-areas/legend-compare-areas.component';
// import { LegendInequalitiesComponent } from '../shared/component/legend/legend-inequalities/legend-inequalities.component';
// import { LegendMapComponent } from '../shared/component/legend/legend-map/legend-map.component';
// import { LegendRecentTrendsComponent } from '../shared/component/legend/legend-recent-trends/legend-recent-trends.component';
// import { LegendTrendComponent } from '../shared/component/legend/legend-trend/legend-trend.component';
// import { FTHelperService } from '../shared/service/helper/ftHelper.service';
// import { IndicatorService } from '../shared/service/api/indicator.service';
// import { UIService } from '../shared/service/helper/ui.service';
// import { DownloadService } from '../shared/service/api/download.service';
// import { ComparatorIds, AreaTypeIds } from '../shared/shared';

// describe('CompareAreaComponent', () => {
//   let component: CompareAreaComponent;
//   let fixture: ComponentFixture<CompareAreaComponent>;

//   let ftHelperService: any;
//   let uiService: any;
//   let downloadService: any;
//   let indicatorService: any;

//   beforeEach(async(() => {

//     ftHelperService = jasmine.createSpyObj('FTHelperService', ['setComparatorId', 'isParentCountry']);
//     uiService = jasmine.createSpyObj('UIService', ['']);
//     downloadService = jasmine.createSpyObj('DownloadService', ['']);
//     indicatorService = jasmine.createSpyObj('IndicatorService', ['']);

//     TestBed.configureTestingModule({
//       declarations: [
//         CompareAreaComponent,
//         CompareAreaChartComponent,
//         CompareAreaTableComponent,
//         LegendComponent,
//         LegendCompareAreasComponent,
//         LegendInequalitiesComponent,
//         LegendMapComponent,
//         LegendRecentTrendsComponent,
//         LegendTrendComponent
//       ],
//       providers: [
//         { provide: FTHelperService, useValue: ftHelperService },
//         { provide: UIService, useValue: uiService },
//         { provide: DownloadService, useValue: downloadService },
//         { provide: IndicatorService, useValue: indicatorService }
//       ]
//     })
//       .compileComponents();
//   }));

//   beforeEach(() => {
//     fixture = TestBed.createComponent(CompareAreaComponent);
//     component = fixture.componentInstance;
//     fixture.detectChanges();
//   });

//   it('should create', () => {
//     expect(component).toBeTruthy();
//   });

//   it('should return true for areatypes', () => {
//     fixture = TestBed.createComponent(CompareAreaComponent);
//     component = fixture.componentInstance;
//     let gp = AreaTypeIds.Practice;
//     let ward = AreaTypeIds.Ward;
//     let lsoa = AreaTypeIds.LSOA;
//     let msoa = AreaTypeIds.MSOA;

//       let result = component.isSmallAreaType(gp);
//     result = result && component.isSmallAreaType(ward);
//       result = result && component.isSmallAreaType(lsoa);
//       result = result && component.isSmallAreaType(msoa);

//     expect(result).toBeTruthy();
//   })

//   it('should set default comparatorId', () => {
//     fixture = TestBed.createComponent(CompareAreaComponent);
//     component = fixture.componentInstance;

//       let isAllInEnglandHideStatus = spyOn(component, 'isSmallAreaType');
//     isAllInEnglandHideStatus.and.returnValue(true);

//     component.;

//     expect(isAllInEnglandHideStatus).toHaveBeenCalled()
//     expect(ftHelperService.setComparatorId).toHaveBeenCalled();
//   })

//   it('should return be true if isParentCountry is false', () => {
//     fixture = TestBed.createComponent(CompareAreaComponent);
//     component = fixture.componentInstance;

//       let isRestrictedAllInEnglandOpts = spyOn(component, 'isSmallAreaType');
//     isRestrictedAllInEnglandOpts.and.returnValue(true);

//     ftHelperService.isParentCountry.and.returnValue(false);

//       let result = component.isAllInEnglandHidden();

//     expect(result).toBeTruthy();
//   })

//   it('should return be true if isRestrictedAllInEnglandOpts is false', () => {
//     fixture = TestBed.createComponent(CompareAreaComponent);
//     component = fixture.componentInstance;

//       let isRestrictedAllInEnglandOpts = spyOn(component, 'isSmallAreaType');
//     isRestrictedAllInEnglandOpts.and.returnValue(false);

//     ftHelperService.isParentCountry.and.returnValue(true);

//     let result = component.isAllInEnglandHidden();

//     expect(result).toBeTruthy();
//   })

//   it('should return be false if isRestrictedAllInEnglandOpts is false and isParentCountry is false', () => {
//     fixture = TestBed.createComponent(CompareAreaComponent);
//     component = fixture.componentInstance;

//       let isRestrictedAllInEnglandOpts = spyOn(component, 'isSmallAreaType');
//     isRestrictedAllInEnglandOpts.and.returnValue(false);

//     ftHelperService.isParentCountry.and.returnValue(false);

//       let result = component.isAllInEnglandHidden();

//     expect(result).toBeFalsy();
//   })

//   it('should set setAreasTabDisplay to false', () => {
//     fixture = TestBed.createComponent(CompareAreaComponent);
//     component = fixture.componentInstance;

//     let isAllInEnglandHideStatus = spyOn(component, 'isAllInEnglandHidden');
//     isAllInEnglandHideStatus.and.returnValue(true);

//     component.setAreasTabDisplay();

//     expect(component['showAllInEngland']).toEqual(false);
//   })

//   it('should set setAreasTabDisplay to true', () => {
//     fixture = TestBed.createComponent(CompareAreaComponent);
//     component = fixture.componentInstance;

//       let isAllInEnglandHideStatus = spyOn(component, 'isAllInEnglandHidden');
//     isAllInEnglandHideStatus.and.returnValue(false);

//     component.setAreasTabDisplay();

//     expect(component['showAllInEngland']).toEqual(true);
//   })

// });
