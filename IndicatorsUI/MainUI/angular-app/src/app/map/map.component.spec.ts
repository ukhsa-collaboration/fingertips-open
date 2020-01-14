import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { MapComponent } from './map.component';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { AreaService } from '../shared/service/api/area.service';
import { CoreDataHelperService } from '../shared/service/helper/coreDataHelper.service';
import { ProfileIds, AreaTypeIds } from '../shared/constants';
import { of } from 'rxjs';


describe('MapComponent', () => {

  let component: MapComponent;
  let fixture: ComponentFixture<MapComponent>;

  // Spies
  let areaService: any;
  let ftHelperService: any;
  let coreDataHelperService: any;
  let indicatorService: any;

  beforeEach(async(() => {

    // Create the spies
    areaService = jasmine.createSpyObj('AreaService', ['getParentAreas']);
    ftHelperService = jasmine.createSpyObj('FTHelperService', ['getFTModel, getCurrentGroupRoot']);
    coreDataHelperService = jasmine.createSpyObj('CoreDataHelperService', ['']);
    indicatorService = jasmine.createSpyObj('IndicatorService', ['']);

    TestBed.configureTestingModule({
      declarations: [MapComponent],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      imports: [FormsModule, HttpClientModule, CommonModule],
      providers: [
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: IndicatorService, useValue: indicatorService },
        { provide: AreaService, useValue: areaService },
        { provide: CoreDataHelperService, useValue: coreDataHelperService }
      ]
    })
      .compileComponents();
  }));

  it('should create', () => {

    // Arrange: FtHelperService
    // ftHelperService.getFTModel.and.returnValue({ areaTypeId: 1, profileId: 1 });

    createComponent();
    expect(component).toBeTruthy();
  });

  // it('Boundary not supported', () => {

  //   // Arrange: AreaService
  //   const observed = { subscribe: function () { return [{ AreaTypeId: 1, CanBeDisplayedOnMap: false }]; } };
  //   areaService.getParentAreas.and.returnValue(observed);

  //   // Arrange: FtHelperService
  //   ftHelperService.getFTModel.and.returnValue({ areaTypeId: 1, profileId: 1 });

  //   // Act
  //   createComponent();
  //   fireOutsideEventOnMap(null);

  //   // Assert: boundary not supported element is displayed
  //   const boundaryNotSupported = fixture.nativeElement.querySelector('#boundryNotSupported');
  //   expect(boundaryNotSupported).toEqual(null);

  //   // Verify spies
  //   expect(areaService.getParentAreas).toHaveBeenCalledTimes(1);
  //   expect(ftHelperService.getFTModel).toHaveBeenCalledTimes(1);
  // });

  // it('Practice profiles search', () => {

  //   // Arrange: AreaService
  //   const data = [{
  //     AreaTypeId: AreaTypeIds.Practice, CanBeDisplayedOnMap: true
  //   }];
  //   areaService.getParentAreas.and.returnValue(of(data));

  //   // Arrange: FtHelperService (areaTypeId is not defined)
  //   ftHelperService.getFTModel.and.returnValue({ profileId: ProfileIds.PracticeProfile });

  //   // Act
  //   createComponent();
  //   fireOutsideEventOnMap({ 'searchMode': true });

  //   // Assert: Practice search displayed
  //   const element = fixture.nativeElement.querySelector('ft-practice-search');
  //   expect(element).not.toBe(null);

  //   // Verify spies
  //   expect(areaService.getParentAreas).toHaveBeenCalledTimes(1);
  //   expect(ftHelperService.getFTModel).toHaveBeenCalledTimes(1);
  // });

  const createComponent = function () {
    fixture = TestBed.createComponent(MapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  const fireOutsideEventOnMap = function (searchMode) {
    const isEnglandAreaType = false;
    // component.onOutsideEvent(null, searchMode, isEnglandAreaType);
    fixture.detectChanges();
  }

});

