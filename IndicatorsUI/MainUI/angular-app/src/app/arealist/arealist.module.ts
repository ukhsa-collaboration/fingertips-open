import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ArealistIndexComponent } from './arealist-index/arealist-index.component';
import { ArealistManageComponent } from './arealist-manage/arealist-manage.component';
import { GoogleMapSimpleComponent } from '../map/google-map-simple/google-map-simple.component';
import { ArealistAreasComponent } from '../shared/component/arealist/arealist-areas/arealist-areas.component';
import { PracticeSearchSimpleComponent } from '../map/practice-search-simple/practice-search-simple.component';
import { TypeaheadModule } from 'ngx-bootstrap';
import { LightBoxModule } from '../shared/component/light-box/light-box.module';
import { LightBoxWithInputModule } from '../shared/component/light-box-with-input/light-box-with-input.module';

@NgModule({
    imports: [
        CommonModule,
        HttpClientModule,
        FormsModule,
        ReactiveFormsModule,
        LightBoxModule,
        LightBoxWithInputModule,
        TypeaheadModule.forRoot()
    ],
    declarations: [
        ArealistIndexComponent,
        ArealistManageComponent,
        GoogleMapSimpleComponent,
        ArealistAreasComponent,
        PracticeSearchSimpleComponent
    ],
    exports: [
        ArealistIndexComponent,
        ArealistManageComponent,
        GoogleMapSimpleComponent,
        ArealistAreasComponent
    ]
})
export class ArealistModule { }
