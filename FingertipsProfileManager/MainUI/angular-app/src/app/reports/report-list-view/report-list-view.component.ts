import { Component, Output, EventEmitter, Input, OnChanges, SimpleChanges } from '@angular/core';
import * as _ from 'underscore';
import { Profile, AreaType } from '../../model/profile';
import { ReportsService } from '../../services/reports.service';
import { Report, ReportListView } from '../../model/report';
import { ReportStatus } from '../reports';

@Component({
  selector: 'app-report-list-view',
  templateUrl: './report-list-view.component.html',
  styleUrls: ['./report-list-view.component.css']
})

export class ReportListViewComponent implements OnChanges {
  @Input() profiles: Profile[];
  @Input() userProfiles: Profile[];
  @Input() reports: Report[];
  @Input() areaTypes: AreaType[];
  @Output() getListViewStatus = new EventEmitter();

  editProfileId: number;
  reportsToDisplay: ReportListView[] = [];
  selectedReport: Report;

  constructor(private reportService: ReportsService) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['reports']) {
      if (this.reports !== undefined) {
        this.displayReports();
      }
    }
  }

  displayReports() {
    this.reports.forEach(report => {
      const reportListView = new ReportListView();
      reportListView.id = report.id;
      reportListView.name = report.name;
      reportListView.file = report.file;
      reportListView.parameters = report.parameters;
      reportListView.notes = report.notes;
      reportListView.isLive = report.isLive;

      // Assign profiles
      let profiles: Profile[] = [];
      report.profiles.forEach(profile => {
        profiles.push(this.profiles.find(x => x.Id === profile));
      });

      // Sort profiles
      profiles = _.sortBy(profiles,
        function (profile) { return profile.Name; });

      reportListView.profiles = profiles;

      // Assign area types
      if (report.areaTypeIds !== undefined && report.areaTypeIds.length > 0) {
        report.areaTypeIds.forEach(areaTypeId => {
          const areaType = this.areaTypes.find(x => x.Id === Number(areaTypeId));
          reportListView.areaTypes.push(areaType);
        });
      }

      // Sort area types
      reportListView.areaTypes = _.sortBy(reportListView.areaTypes,
        function (areaType) { return areaType.ShortName; });

      this.reportsToDisplay.push(reportListView);
    });
  }

  deleteReport(event, index) {
    const report = this.reports[index];
    this.reportService.deleteReportById(report.id)
      .subscribe((response) => {
        this.reports.splice(index, 1);
      });
  }

  newReport() {
    this.getListViewStatus.emit({ status: ReportStatus.New, selectedReport: new ReportListView() });
  }

  editReport(report: Report) {
    this.selectedReport = report;
    this.getListViewStatus.emit({ status: ReportStatus.Edit, selectedReport: this.selectedReport });
  }
}
