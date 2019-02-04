import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpModule } from '@angular/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ArealistIndexComponent } from './arealist-index/arealist-index.component';
import { ArealistManageComponent } from './arealist-manage/arealist-manage.component';
import { GoogleMapSimpleComponent } from '../map/google-map-simple/google-map-simple.component';
import { ArealistAreasComponent } from '../shared/component/arealist/arealist-areas/arealist-areas.component';
import { PracticeSearchSimpleComponent } from '../map/practice-search-simple/practice-search-simple.component';
import { TypeaheadModule } from 'ngx-bootstrap';
import { LightBoxComponent } from '../shared/component/light-box/light-box.component';
import { LightBoxWithInputComponent } from 'app/shared/component/light-box-with-input/light-box-with-input.component';

@NgModule({
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        ReactiveFormsModule,
        TypeaheadModule.forRoot()
    ],
    declarations: [
        ArealistIndexComponent,
        ArealistManageComponent,
        GoogleMapSimpleComponent,
        ArealistAreasComponent,
        PracticeSearchSimpleComponent,
        LightBoxComponent,
        LightBoxWithInputComponent
    ],
    exports: [
        ArealistIndexComponent,
        ArealistManageComponent,
        GoogleMapSimpleComponent,
        ArealistAreasComponent,
        LightBoxComponent,
        LightBoxWithInputComponent
    ]
})
export class ArealistModule { }
