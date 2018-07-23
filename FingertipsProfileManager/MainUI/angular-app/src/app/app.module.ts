import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import {ReportsService} from './services/reports.service';
import {ProfileService} from './services/profile.service';
import { AppComponent } from './app.component';
import { ReportsComponent } from './reports/reports.component';
import { ProfileListComponent } from './shared/profile-list/profile-list.component';
import { ReportParametersComponent } from './reports/report-parameters/report-parameters.component';
import { ReportProfilesComponent } from './reports/report-profiles/report-profiles.component';
import { ReportListViewComponent } from './reports/report-list-view/report-list-view.component';
import { ReportEditViewComponent } from './reports/report-edit-view/report-edit-view.component';

@NgModule({
  declarations: [
    AppComponent,
    ReportsComponent,
    ProfileListComponent,
    ReportParametersComponent,
    ReportProfilesComponent,
    ReportListViewComponent,
    ReportEditViewComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule
  ],
  providers: [
    ReportsService,
    ProfileService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
