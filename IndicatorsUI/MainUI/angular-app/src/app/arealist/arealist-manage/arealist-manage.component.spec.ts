import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ArealistManageComponent } from './arealist-manage.component';
import { AreaListService } from '../../shared/service/api/arealist.service';
import { AreaService } from '../../shared/service/api/area.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { CommonModule } from '@angular/common';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { HttpService } from '../../shared/service/api/http.service';

/*
PLEASE NOTE:    This test will not work as in the ngOnInit method
                we are trying to read the "user-id" attribute, but
                the element "ft-arealist-manage" resides in the MVC
                view and it is not available in angular.
*/

// describe('ArealistManageComponent', () => {

//     let component: ArealistManageComponent;
//     let fixture: ComponentFixture<ArealistManageComponent>;

//     // Spies
//     let areaListService: any;
//     let areaService: any;
//     let ftHelperService: any;
//     let httpService: any;

//     beforeEach(async(() => {

//         // Create the spies
//         areaListService = jasmine.createSpyObj('AreaListService', ['getAreaLists']);
//         areaService = jasmine.createSpyObj('AreaService', ['']);
//         ftHelperService = jasmine.createSpyObj('FTHelperService', ['getURL', 'version', 'getFTModel']);
//         httpService = jasmine.createSpyObj('HttpService', ['httpGet', 'httpPost']);

//         TestBed.configureTestingModule({
//             declarations: [ArealistManageComponent],
//             schemas: [CUSTOM_ELEMENTS_SCHEMA],
//             imports: [FormsModule, HttpModule, CommonModule, ReactiveFormsModule],
//             providers: [
//                 { provide: FTHelperService, useValue: ftHelperService },
//                 { provide: AreaListService, useValue: areaListService },
//                 { provide: AreaService, useValue: areaService },
//                 { provide: HttpService, useValue: httpService }
//             ]
//         })
//             .compileComponents();
//     }));

//     it('should create', () => {

//         ftHelperService.getFTModel.and.returnValue(() => { return { areaTypeId: 1, profileId: 1 } });
//         ftHelperService.getURL.and.returnValue(() => { return {} });

//         createComponent();
//         expect(component).toBeTruthy();
//     });

//     let createComponent = function () {
//         fixture = TestBed.createComponent(ArealistManageComponent);
//         component = fixture.componentInstance;
//         fixture.detectChanges();
//     }
// });
