
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Profile, AreaType } from '../model/profile';
import { ProfileService } from '../services/profile.service';
import { Report, ReportListView } from '../model/report';
import { ReportsService } from '../services/reports.service';
import { forkJoin } from 'rxjs';
import { ReportStatus } from './reports';

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
  areaTypes: AreaType[];
  selectedReport: ReportListView;

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
    const areaTypesObservable = this.profileService.getAllAreaTypes();

    forkJoin([profilesObservable, userProfilesObservable,
      reportsObservable, areaTypesObservable]).subscribe(results => {

        this.profiles = <Profile[]>results[0];
        this.userProfiles = <Profile[]>results[1];
        this.reports = <Report[]>results[2];
        this.areaTypes = this.getSupportedAreaTypes(<AreaType[]>results[3]);

        this.ref.detectChanges();
      });
  }

  getSupportedAreaTypes(areaTypes: AreaType[]): AreaType[] {
    let supportedAreaTypes: AreaType[] = [];

    supportedAreaTypes = areaTypes.filter(x => x.IsCurrent === true && x.IsSupported === true);

    supportedAreaTypes.sort((areaTypeA: AreaType, areaTypeB: AreaType) => {
      if (areaTypeA.ShortName > areaTypeB.ShortName) {
        return 1;
      }

      if (areaTypeA.ShortName < areaTypeB.ShortName) {
        return 0;
      }

      return 0;
    });

    return supportedAreaTypes;
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
