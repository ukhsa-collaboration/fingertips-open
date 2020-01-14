import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UploadComponent } from './upload.component';
import { UploadIndexComponent } from './upload-index/upload-index.component';
import { UploadProgressComponent } from './upload-progress/upload-progress.component';
import { UploadQueueComponent } from './upload-queue/upload-queue.component';
import { UploadService } from '../services/upload.service';
import { NgSelect2Module } from '../../../node_modules/ng-select2';
import { LightBoxModule } from '../shared/component/light-box/light-box.module';
import { UploadProgressTableComponent } from './upload-progress-table/upload-progress-table.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        NgSelect2Module,
        LightBoxModule
    ],
    declarations: [
        UploadComponent,
        UploadIndexComponent,
        UploadProgressComponent,
        UploadQueueComponent,
        UploadProgressTableComponent
    ],
    providers: [
        UploadService
    ],
    exports: [
    ]
})
export class UploadModule { }
