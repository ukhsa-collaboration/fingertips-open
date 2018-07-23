import { Component, OnInit, Output, EventEmitter, Input, AfterViewInit } from '@angular/core';
import { Report } from '../../model/report';
import { Profile } from '../../model/profile';
import { ReportsService } from '../../services/reports.service';


@Component({
  selector: 'app-report-edit-view',
  templateUrl: './report-edit-view.component.html',
  styleUrls: ['./report-edit-view.component.css']
})

export class ReportEditViewComponent implements OnInit {

  report: Report = { id: null, name: '', file: '', parameters: [], profiles: [], notes: '', isLive: true };
  private selectedParameters: string[] = [];
  private selectedProfiles: number[] = [];
  private reportName: string = '';
  private status: number;
  errorMessage: any;
  private profiles: Profile[] = [];
  private validationError:string;

  @Output() getReportViewStatus: EventEmitter<number> = new EventEmitter<number>();
  @Input() editProfileId: number;
  @Input() userProfilesHash: any;

  constructor(private service: ReportsService) {
  }

  ngOnInit() {
    if (this.editProfileId != null) {
      this.service.getReportById(this.editProfileId)
        .subscribe(
        data => {
          console.log(data);
          this.report = data;          
          console.log(this.report);
        }
        ), error => this.errorMessage = <any>error;
    }
  }

  saveReport() {
    if (this.isFormValid()) {
      if (this.editProfileId != null) {
        this.service.updateReport(this.report).subscribe(data => {
          this.getReportViewStatus.emit(2);
        });
      } else {
        this.service.saveReport(this.report).subscribe(data => {
          this.getReportViewStatus.emit(2);
        });
      }
    }
  }

  cancelReport() {
    // TODO : reset the model
    this.getReportViewStatus.emit(2);
  }

  getProfiles(profiles) {
    this.selectedProfiles = profiles;
  }

  getParameters(parameters) {
    this.selectedParameters = parameters;
  }

  deleteReport(id: number) {
    this.service.deleteReportById(id)
      .subscribe(data => {
        this.getReportViewStatus.emit(2);
      });
  }

  isFormValid(): boolean {
    let isValid = false;

    if (!this.report.name) {
      this.validationError = 'Report name is required';
    } else if (this.report.profiles.length == 0) {
      this.validationError = 'At least one profile is required ';
    } else {
      isValid = true;
    }

    return isValid;
  }
}
