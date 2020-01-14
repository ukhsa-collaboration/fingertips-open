import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ReportsComponent } from './reports.component';
import { ReportListViewComponent } from './report-list-view/report-list-view.component';
import { ReportEditViewComponent } from './report-edit-view/report-edit-view.component';
import { LightBoxModule } from '../shared/component/light-box/light-box.module';
import { NgSelect2Module } from 'ng-select2';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        LightBoxModule,
        NgSelect2Module
    ],
    declarations: [
        ReportsComponent,
        ReportListViewComponent,
        ReportEditViewComponent
    ],
    exports: [
    ]
})
export class ReportsModule { }
