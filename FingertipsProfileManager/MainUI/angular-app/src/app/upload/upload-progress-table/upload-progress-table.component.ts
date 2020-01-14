import { Component, Input, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { User } from '../../model/user';
import { UploadJobStatus, UploadJob, UploadJobProgressStatus, UploadJobErrorType, UploadJobSummary, UploadJobProgress } from '../../model/upload';
import { LightBoxConfig, LightBoxTypes } from '../../shared/component/light-box/light-box';
import { UploadService } from '../../services/upload.service';
import { isDefined } from '@angular/compiler/src/util';

@Component({
  selector: 'app-upload-progress-table',
  templateUrl: './upload-progress-table.component.html',
  styleUrls: ['./upload-progress-table.component.css']
})
export class UploadProgressTableComponent implements OnChanges {

  // Incoming members
  @Input() selectedUser: User;

  // Outgoing members
  @Output() emitUploadJobProgress = new EventEmitter();

  // private members
  uploadJobProgress: UploadJobProgress;

  // public members
  public loading = true;
  public isAnyJobs: boolean;
  public showMoreUploadJobs: boolean;
  public rowCount = 30;
  public lightBoxConfig: LightBoxConfig;

  constructor(private uploadService: UploadService) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['selectedUser']) {
      if (isDefined(this.selectedUser)) {
        if (isDefined(this.selectedUser.DisplayName)) {
          this.getJobDetails();
        }
      }
    }
  }

  getJobDetails(): void {
    this.uploadService.getCurrentUserJobProgress(this.selectedUser.Id, this.rowCount).subscribe(results => {
      this.uploadJobProgress = results;

      this.emitUploadJobProgress.emit(this.uploadJobProgress);

      this.isAnyJobs = this.uploadJobProgress.Jobs.length > 0;

      this.showMoreUploadJobs = this.uploadJobProgress.Jobs.length >= this.rowCount;

      // Set the loading to false, so the table can be displayed
      this.loading = false;
    });
  }

  getRowClass(job: UploadJob): string {
    switch (job.Status) {
      case UploadJobStatus.InProgress:
        return 'in-progress-bg';
      case UploadJobStatus.OverrideDatabaseDuplicatesConfirmationAwaited:
        return 'awaiting-confirmation-bg';
      default:
        return 'default-bg';
    }
  }

  /** Method to update the job status */
  updateStatus(job: UploadJob, newStatus: number): void {
    this.uploadService.changeJobStatus(job.Guid, newStatus).subscribe(
      (response) => {
        // Succeeded - Refresh the job list
        this.getJobDetails();
      },
      (error) => {
        // Failed - Write the error to the console
        console.log("Failed updating the job status for the job");
      }
    );
  }

  /** Method to increase the job list by 30 */
  updateRowCount(): void {
    // Increment the row count by 30
    this.rowCount += 30;

    // Refresh the job list
    this.getJobDetails();
  }

  /** Method to display the job summary in a modal dialog */
  showSummary(event: any, job: UploadJob): void {
    // Configure the common properties of lightbox
    const config = new LightBoxConfig();
    config.Type = LightBoxTypes.Ok;
    config.OkButtonText = 'Close';

    // API call to get the job summary
    this.uploadService.getJobSummary(job.Guid).subscribe(
      (response) => {
        // Succeeded - Read the job summary from response
        const summary: UploadJobSummary = response;

        // Set the title property of the lightbox
        config.Title = this.getLightBoxTitle(summary);

        // Set the html property of the lightbox
        config.Html = this.getLightBoxHtml(summary);

        // Display the lightbox
        this.lightBoxConfig = config;
      },
      (error) => {
        // Failed - Set the title and html property of the lightbox
        config.Title = 'Error';
        config.Html = 'Unable to display the summary. Please try again.' +
          '<br>If the issue persists then please contact the administrator' +
          error;

        // Display the lightbox
        this.lightBoxConfig = config;
      }
    );
  }

  /** Method to return the title for the lightbox */
  getLightBoxTitle(summary: UploadJobSummary): string {
    switch (summary.JobStatus) {
      case UploadJobStatus.OverrideDatabaseDuplicatesConfirmationAwaited:
        if (summary.ErrorType === 40003) {
          return 'Duplicate data in file';
        }
        if (summary.ErrorType === 40004) {
          return 'Data already exists in database';
        }
        return '';

      case UploadJobStatus.SmallNumberWarningConfirmationAwaited:
        return 'Small numbers';

      case UploadJobStatus.FailedValidation:
        return 'Validation errors';

      case UploadJobStatus.MissingColumn:
        return 'Column errors';

      case UploadJobStatus.UnexpectedError:
        return 'Unexpected error';
    }
  }

  /** Method to return the html of the lightbox */
  getLightBoxHtml(summary: UploadJobSummary): string {

    // Unexpected error
    if (summary.JobStatus === UploadJobStatus.UnexpectedError) {
      return summary.ErrorText;
    }

    // In progress
    if (summary.JobStatus === UploadJobStatus.InProgress) {
      return '<div class="row">' +
        '<h4 class="col-md-4">Rows scanned:</h4>' +
        '<h4 class="col-md-2"></h4>' +
        '</div>';
    }

    // Missing column
    if (summary.JobStatus === UploadJobStatus.MissingColumn) {
      let html = '<div>';

      const parsedErrorJson = JSON.parse(summary.ErrorJson);
      parsedErrorJson.forEach(error => {
        html = html + '<div>' + error + '</div>';
      });

      return html + '</div>';
    }

    // Small number warnings
    if (summary.JobStatus === UploadJobStatus.SmallNumberWarningConfirmationAwaited) {
      let html = '<div class="scrollable-table-container">' +
        '<table class="table">' +
        '<thead>' +
        '<tr>' +
        '<th>Row </th>' +
        '<th>Count</th>' +
        '</tr>' +
        '</thead>' +
        '<tbody>';

      const parsedErrorJson = JSON.parse(summary.ErrorJson);
      parsedErrorJson.forEach(error => {
        html = html + '<tr>' +
          '<td>' + error.RowNumber + '</td>' +
          '<td>' + error.SmallCountValue + '</td>' +
          '</tr>';
      });

      return html + '</tbody>' +
        '</table>' +
        '</div>';
    }

    // Validation errors
    if (summary.JobStatus === UploadJobStatus.FailedValidation) {
      if (summary.ErrorType === UploadJobErrorType.ValidationFailureError) {
        let html = '<div class="scrollable-table-container">' +
          '<table class="table">' +
          '<thead>' +
          '<tr>' +
          '<th>Row</th>' +
          '<th>Error</th>' +
          '</tr>' +
          '</thead>' +
          '<tbody>';

        const parsedErrorJson = JSON.parse(summary.ErrorJson);
        parsedErrorJson.forEach(error => {
          html = html + '<tr>' +
            '<td>' + error.RowNumber + '</td>' +
            '<td>' + error.ErrorMessage + '</td>' +
            '</tr>';
        });

        return html + '</tbody>' +
          '</table>' +
          '</div>';

      } else if (summary.ErrorType === UploadJobErrorType.PermissionError) {
        let html = '<div>' + summary.ErrorText + '</div>' +
          '<br>';

        const parsedErrorJson = JSON.parse(summary.ErrorJson);
        parsedErrorJson.forEach(error => {
          html = html + '<div>' + error + '</div>';
        });

        return html;

      } else {
        return '<div>' + summary.ErrorText + '</div>';
      }
    }

    // Duplication errors 
    if (summary.JobStatus === UploadJobStatus.OverrideDatabaseDuplicatesConfirmationAwaited) {
      let html = '<div>' + summary.ErrorText + '</div>' +
        '<br>';

      if (summary.ErrorType === UploadJobErrorType.DuplicateRowInSpreadsheetError) {
        html = html + '<div class="scrollable-table-container">' +
          '<table class="table">' +
          '<thead>' +
          '<tr> ' +
          '<th>Row</th>' +
          '<th>Error</th>' +
          '</tr>' +
          '</thead>' +
          '<tbody>';

        const parsedErrorJson = JSON.parse(summary.ErrorJson);
        parsedErrorJson.forEach(error => {
          html = html + '<tr>' +
            '<td>' + error.RowNumber + '</td>' +
            '<td>' + error.DuplicateRowMessage + '</td>' +
            '</tr>';
        });

        return html + '</tbody>' +
          '</table>' +
          '</div>';

      } else if (summary.ErrorType === UploadJobErrorType.DuplicateRowInDatabaseError) {
        let html = '<div class="scrollable-table-container">' +
          '<table class="table"> ' +
          '<thead> ' +
          '<tr> ' +
          '<th>Row</th>' +
          '<th>Indicator</th>' +
          '<th>Age Id</th>' +
          '<th>Sex Id</th>' +
          '<th>Area Code</th>' +
          '<th>File Value</th>' +
          '<th>Database Value</th>' +
          '</tr>' +
          '</thead>' +
          '<tbody>';

        const parsedErrorJson = JSON.parse(summary.ErrorJson);
        parsedErrorJson.forEach(error => {
          html = html + '<tr>' +
            '<td>' + error.RowNumber + '</td>' +
            '<td>' + error.IndicatorId + '</td>' +
            '<td>' + error.AgeId + '</td>' +
            '<td>' + error.SexId + '</td>' +
            '<td>' + error.AreaCode + '</td>' +
            '<td>' + error.ExcelValue + '</td>' +
            '<td>' + error.DbValue + '</td>' +
            '</tr>';
        });

        return html + '</tbody>' +
          '</table>' +
          '</div>';
      }
    }

    return '';
  }

  /** Method to return the status of the job */
  getStatusDescription(status: number): string {
    switch (status) {
      case UploadJobStatus.NotStarted:
        return 'In queue';
      case UploadJobStatus.InProgress:
        return 'In progress'
      case UploadJobStatus.OverrideDatabaseDuplicatesConfirmationAwaited:
        return 'Duplicated rows found in database';
      case UploadJobStatus.OverrideDatabaseDuplicatesConfirmationGiven:
        return 'Awaiting upload';
      case UploadJobStatus.OverrideDatabaseDuplicatesConfirmationRefused:
        return 'Cancelled';
      case UploadJobStatus.SmallNumberWarningConfirmationGiven:
        return 'Small numbers accepted';
      case UploadJobStatus.SmallNumberWarningConfirmationRefused:
        return 'Cancelled';
      case UploadJobStatus.FailedValidation:
        return 'Validation Failed';
      case UploadJobStatus.MissingColumn:
        return 'Missing column'
      case UploadJobStatus.UnexpectedError:
        return 'Unexpected Error';
      case UploadJobStatus.SuccessfulUpload:
        return 'Successful Upload';
      default:
        return 'Unknown';
    }
  }

  /** Method to return the progress bar update */
  getProgress(job: UploadJob): UploadJobProgressStatus {
    const progressStatus = new UploadJobProgressStatus();

    switch (job.ProgressStage) {
      case UploadJobStatus.ValidatingWorksheets:
        progressStatus.Percent = 5;
        progressStatus.Text = 'Validating worksheets';
        break;
      case UploadJobStatus.ValidatingData:
        progressStatus.Percent = 15;
        progressStatus.Text = 'Validating data';
        break;
      case UploadJobStatus.CheckingPermission:
        progressStatus.Percent = 25;
        progressStatus.Text = 'Checking permission';
        break;
      case UploadJobStatus.DuplicateCheckInFile:
        progressStatus.Percent = 35;
        progressStatus.Text = 'Duplicate check in file';
        break;
      case UploadJobStatus.DuplicateCheckInDatabase:
        progressStatus.Percent = 60;
        progressStatus.Text = 'Duplicate check in database';
        break;
      case UploadJobStatus.WritingToDatabase:
        progressStatus.Percent = 85;
        progressStatus.Text = 'Writing to database';
        break;
      default:
        progressStatus.Percent = 0;
        progressStatus.Text = '';
        break;
    }

    return progressStatus;
  }

  /** Method to handle the lightbox event emitter function */
  updateLightBoxActionConfirmed(event: any) { }
}
