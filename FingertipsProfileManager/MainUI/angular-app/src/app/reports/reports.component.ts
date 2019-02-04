import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Profile } from 'app/model/profile';
import { ProfileService } from 'app/services/profile.service';
import { Report } from 'app/model/report';
import { ReportsService } from 'app/services/reports.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css'],
})

export class ReportsComponent implements OnInit {

  status: number;
  profiles: Profile[];
  userProfiles: Profile[];
  reports: Report[];
  selectedReport: Report;

  constructor(private profileService: ProfileService,
    private reportsService: ReportsService,
    private ref: ChangeDetectorRef) {
  }

  ngOnInit() {
    this.status = ReportStatus.List;
    this.loadData(false);
  }

  loadData(reload: boolean) {

    if (reload) {
      this.profiles = undefined;
      this.userProfiles = undefined;
      this.reports = undefined;
    }

    const profilesObservable = this.profileService.getAllProfiles();
    const userProfilesObservable = this.profileService.getUserProfiles();
    const reportsObservable = this.reportsService.getReports();

    Observable.forkJoin([profilesObservable, userProfilesObservable, reportsObservable]).subscribe(results => {
      this.profiles = <Profile[]>results[0];
      this.userProfiles = <Profile[]>results[1];
      this.reports = <Report[]>results[2];

      this.ref.detectChanges();
    });
  }

  getReportViewStatus(status) {
    this.status = status;
    this.loadData(true);
  }

  getListViewStatus(listViewState) {
    this.status = listViewState.status;
    this.selectedReport = listViewState.selectedReport;
  }
}

export enum ReportStatus {
  New = 0,
  Edit = 1,
  List = 2
}
