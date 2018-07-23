import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { Component, OnInit, HostListener, ChangeDetectorRef } from '@angular/core';
import { MapComponent } from './map.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MapModule } from './map.module';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { MockFTHelperService } from '../mock/ftHelper.service.mock';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { MockIndicatorService } from '../mock/indicator.service.mock';
import { CoreDataHelperService } from '../shared/service/helper/coreDataHelper.service';
import { MockCoreDataHelperService } from '../mock/coreDataHelper.service.mock';
import { HttpModule } from '@angular/http';
import { FTModel } from './../typings/FT.d';
import { BaseRequestOptions, Response, ResponseOptions, Http } from '@angular/http';
import { MockBackend, MockConnection } from '@angular/http/testing';
import { AreaTypeIds } from '../shared/shared';

describe('MapComponent', () => {

  let component: MapComponent;
  let fixture: ComponentFixture<MapComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [MapComponent],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      imports: [FormsModule, HttpModule, CommonModule],
      providers: [
        { provide: FTHelperService, useClass: MockFTHelperService },
        { provide: IndicatorService, useClass: MockIndicatorService },
        { provide: CoreDataHelperService, useClass: MockCoreDataHelperService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('Boundry not supported', () => {
    component.onOutsideEvent(null, null);
    fixture.detectChanges();
    let a = fixture.nativeElement.querySelector('#boundryNotSupported');
    expect(a !== null).toBe(true);
  });
});

