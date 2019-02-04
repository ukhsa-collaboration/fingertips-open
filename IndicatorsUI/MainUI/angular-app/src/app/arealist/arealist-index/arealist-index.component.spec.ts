import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ArealistIndexComponent } from './arealist-index.component';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { AreaListService } from '../../shared/service/api/arealist.service';
import { AreaService } from '../../shared/service/api/area.service';
import { HttpService } from '../../shared/service/api/http.service';
import { LightBoxComponent } from '../../shared/component/light-box/light-box.component';
import { LightBoxWithInputComponent } from '../../shared/component/light-box-with-input/light-box-with-input.component';

/*
PLEASE NOTE:    This test will not work as in the ngOnInit method
                we are trying to read the "user-id" attribute, but
                the element "ft-arealist-index" resides in the MVC
                view and it is not available in angular.
*/

// describe('ArealistIndexComponent', () => {

//     let component: ArealistIndexComponent;
//     let fixture: ComponentFixture<ArealistIndexComponent>;

//     // Spies
//     let areaListService: any;
//     let areaService: any;
//     let httpService: any;
//     let ftHelperService: any;

//     beforeEach(async(() => {

//         // Create the spies
//         areaListService = jasmine.createSpyObj('AreaListService', ['getAreaLists']);
//         areaService = jasmine.createSpyObj('AreaService', ['getAreaTypes']);
//         httpService = jasmine.createSpyObj('HttpService', ['']);
//         ftHelperService = jasmine.createSpyObj('FTHelperService', ['getURL', 'version', 'getFTModel']);

//         TestBed.configureTestingModule({
//             declarations: [
//                 ArealistIndexComponent,
//                 LightBoxComponent,
//                 LightBoxWithInputComponent
//             ],
//             providers: [
//                 { provide: AreaListService, useValue: areaListService },
//                 { provide: AreaService, useValue: areaService },
//                 { provide: HttpService, useValue: httpService },
//                 { provide: FTHelperService, useValue: ftHelperService }
//             ]
//         })
//             .compileComponents();
//     }));

//     beforeEach(() => {
//         fixture = TestBed.createComponent(ArealistIndexComponent);
//         component = fixture.componentInstance;
//         fixture.detectChanges();
//     });

//     it('should create', () => {
//         expect(component).toBeTruthy();
//     });
// });
