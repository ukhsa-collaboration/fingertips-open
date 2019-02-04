import { Component, OnInit, Output, EventEmitter, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Profile } from 'app/model/profile';
import { ReportsService } from 'app/services/reports.service';
import { Report, ReportListView } from 'app/model/report';
import { ReportStatus } from '../reports.component';

@Component({
  selector: 'app-report-list-view',
  templateUrl: './report-list-view.component.html',
  styleUrls: ['./report-list-view.component.css']
})

export class ReportListViewComponent implements OnChanges {
  @Input() profiles: Profile[];
  @Input() userProfiles: Profile[];
  @Input() reports: Report[];
  @Output() getListViewStatus = new EventEmitter();

  editProfileId: number;
  reportsToDisplay: ReportListView[] = [];
  selectedReport: Report;

  constructor(private service: ReportsService) {
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['reports']) {
      if (this.reports !== undefined) {
        this.displayReports();
      }
    }
  }

  displayReports() {
    this.reports.forEach(report => {
      let reportListView = new ReportListView();
      reportListView.id = report.id;
      reportListView.name = report.name;
      reportListView.file = report.file;
      reportListView.parameters = report.parameters;
      reportListView.notes = report.notes;
      reportListView.isLive = report.isLive;

      let profiles: Profile[] = [];
      report.profiles.forEach(profile => {
        profiles.push(this.profiles.find(x => x.Id === profile));
      });

      reportListView.profiles = profiles;

      this.reportsToDisplay.push(reportListView);
    });
  }

  deleteReport(event, index) {
    let report = this.reports[index];
    this.service.deleteReportById(report.id)
      .subscribe((response) => {
        this.reports.splice(index, 1);
      });
  }

  newReport() {
    this.selectedReport = new Report();
    this.getListViewStatus.emit({ status: ReportStatus.New, selectedReport: this.selectedReport });
  }

  editReport(report: Report) {
    this.selectedReport = report;
    this.getListViewStatus.emit({ status: ReportStatus.Edit, selectedReport: this.selectedReport });
  }
}
