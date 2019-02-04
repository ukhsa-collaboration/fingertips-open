import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { DownloadComponent } from './download.component';
import { DownloadDataComponent } from './download-data/download-data.component';
import { DownloadReportComponent } from './download-report/download-report.component';

@NgModule({
  imports: [
    CommonModule,
    SharedModule
  ],
  declarations: [
    DownloadComponent,
    DownloadDataComponent,
    DownloadReportComponent
  ],
  exports: [
    DownloadComponent
  ]
})
export class DownloadModule { }
