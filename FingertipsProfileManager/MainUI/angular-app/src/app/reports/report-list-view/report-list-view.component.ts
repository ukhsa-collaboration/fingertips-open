import { Component, OnInit, Output, EventEmitter, Input, OnChanges } from '@angular/core';
import { ReportsService } from '../../services/reports.service';
import { Report } from '../../model/report';
import { Profile } from '../../model/profile';


@Component({
  selector: 'app-report-list-view',
  templateUrl: './report-list-view.component.html',
  styleUrls: ['./report-list-view.component.css']
})

export class ReportListViewComponent implements OnInit {
  @Input() userProfilesHash: any;
  @Output() getListViewStatus: EventEmitter<any> = new EventEmitter();

  private reports: Report[];
  private status: number;
  private editProfileId: number;

  constructor(private service: ReportsService) {
  }

  ngOnInit() {
  }

  ngOnChanges() {
    this.service.getReports().subscribe(data => {
      this.reports = data
    });
  }

  deleteReport(event, index) {
    let report = this.reports[index];
    this.service.deleteReportById(report.id);
    this.reports.splice(index, 1);
  }

  newReport() {
    this.getListViewStatus.emit({ status: 0, profileId: null });
  }

  editReport(id) {
    this.editProfileId = id;
    this.getListViewStatus.emit({ status: 1, profileId: id });
  }  

}
