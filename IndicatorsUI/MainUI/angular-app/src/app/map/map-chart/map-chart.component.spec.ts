import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MapChartComponent } from './map-chart.component';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { CoreDataHelperService } from '../../shared/service/helper/coreDataHelper.service';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

/* NOTE - Highcharts prevents this component from being tested:
  "Uncaught Error: Highcharts error #16: www.highcharts.com/errors/16"
*/

// describe('MapChartComponent', () => {
//     let component: MapChartComponent;
//     let fixture: ComponentFixture<MapChartComponent>;

//     beforeEach(async(() => {
//         TestBed.configureTestingModule({
//             declarations: [MapChartComponent],
//             schemas: [CUSTOM_ELEMENTS_SCHEMA],
//             providers: [
//                 { provide: FTHelperService, useValue: null },
//                 { provide: CoreDataHelperService, useValue: null }]
//         })
//             .compileComponents();
//     }));

//     beforeEach(() => {
//         fixture = TestBed.createComponent(MapChartComponent);
//         component = fixture.componentInstance;
//         fixture.detectChanges();
//     });

//     it('should create', () => {
//         expect(component).toBeTruthy();
//     });
// });
