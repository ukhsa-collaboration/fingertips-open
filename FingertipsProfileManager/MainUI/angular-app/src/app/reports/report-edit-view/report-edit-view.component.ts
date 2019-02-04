import { Component, OnChanges, Output, EventEmitter, Input, AfterViewInit, SimpleChanges } from '@angular/core';
import { Report, ReportListView } from 'app/model/report';
import { Profile } from 'app/model/profile';
import { ReportsService } from 'app/services/reports.service';
import { ReportStatus } from '../reports.component';
import { LightBoxConfig, LightBoxTypes } from 'app/shared/component/light-box/light-box.component';

@Component({
  selector: 'app-report-edit-view',
  templateUrl: './report-edit-view.component.html',
  styleUrls: ['./report-edit-view.component.css']
})

export class ReportEditViewComponent implements OnChanges {
  @Input() status: any;
  @Input() profiles: Profile[];
  @Input() userProfiles: Profile[];
  @Input() selectedReport: ReportListView;
  @Output() getReportViewStatus: EventEmitter<number> = new EventEmitter<number>();

  reportId: number;
  validationError: string;
  selectedParameters: string[] = [];
  selectedProfiles: number[] = [];
  lightBoxConfig: LightBoxConfig;

  constructor(private service: ReportsService) {
  }

  ngOnChanges(changes: SimpleChanges) {
  }

  saveReport() {
    if (this.isFormValid()) {

      let report = new Report();
      report.name = this.selectedReport.name;
      report.file = this.selectedReport.file;
      report.parameters = this.selectedReport.parameters;
      report.notes = this.selectedReport.notes;
      report.isLive = this.selectedReport.isLive;

      let profiles: number[] = [];
      this.selectedReport.profiles.forEach(profile => {
        profiles.push(profile.Id);
      });

      report.profiles = profiles;

      if (this.selectedReport.id != null) {
        report.id = this.selectedReport.id;

        this.service.updateReport(report).subscribe(data => {
          this.getReportViewStatus.emit(ReportStatus.List);
        });
      } else {
        this.service.saveReport(report).subscribe(data => {
          this.getReportViewStatus.emit(ReportStatus.List);
        });
      }
    }
  }

  cancelReport() {
    this.getReportViewStatus.emit(ReportStatus.List);
  }

  getProfiles(profiles) {
    this.selectedProfiles = profiles;
  }

  getParameters(parameters) {
    this.selectedParameters = parameters;
  }

  // Show light box for delete report confirmation
  deleteReportConfirmation() {

    // Show light box
    const config = new LightBoxConfig();
    config.Type = LightBoxTypes.OkCancel;
    config.Title = 'Delete report';
    config.Html = 'Are you sure you want to delete the report <b>' + this.selectedReport.name + '</b>?';
    config.OkButtonText = 'Delete';
    config.CancelButtonText = 'Cancel';
    config.ActionType = 'DELETE';
    this.lightBoxConfig = config;
  }

  deleteReport() {
    this.service.deleteReportById(this.selectedReport.id)
      .subscribe(data => {
        this.getReportViewStatus.emit(ReportStatus.List);
      });
  }

  updateLightBoxActionConfirmed(actionConfirmed: boolean) {
    if (actionConfirmed) {
      switch (this.lightBoxConfig.ActionType) {
        case 'DELETE':
          this.deleteReport();
          break;
      }
    }
  }

  isFormValid(): boolean {
    let isValid = false;

    if (!this.selectedReport.name) {
      this.validationError = 'Report name is required';
    } else if (this.selectedReport.profiles.length === 0) {
      this.validationError = 'At least one profile is required ';
    } else {
      isValid = true;
    }

    return isValid;
  }
}
