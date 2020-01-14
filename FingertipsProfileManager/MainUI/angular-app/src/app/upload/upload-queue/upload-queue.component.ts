import { Component, OnInit, Input } from '@angular/core';
import { UploadService } from '../../services/upload.service';
import { UsersService } from '../../services/users.service';
import { forkJoin } from '../../../../node_modules/rxjs';
import { UploadJobQueue, UploadJobStatus } from '../../model/upload';
import { isDefined } from '../../../../node_modules/@angular/compiler/src/util';
import { User } from '../../model/user';

@Component({
  selector: 'app-upload-queue',
  templateUrl: './upload-queue.component.html',
  styleUrls: ['./upload-queue.component.css']
})
export class UploadQueueComponent implements OnInit {

  // Incoming members
  @Input() currentUserId: number;

  // private members
  private currentUser: User;
  private activeJobs: UploadJobQueue[];

  // public members
  public loading = true;
  public areJobsInQueue = false;
  public isCurrentUserAdmin = false;

  constructor(private uploadService: UploadService, private usersService: UsersService) { }

  ngOnInit() {
    // Initialise the API calls
    const currentUserObservable = this.usersService.getUserById(this.currentUserId);
    const allActiveJobProgress = this.uploadService.getAllActiveJobProgress();

    // Make the API calls
    forkJoin([currentUserObservable, allActiveJobProgress]).subscribe(results => {

      // Read the current user from the result
      this.currentUser = results[0];

      // Read the active jobs from the result
      this.activeJobs = results[1];

      // Check whether the current user is administrator
      this.isCurrentUserAdmin = this.currentUser.IsAdministrator;

      // Check whether there is any active job to display
      if (isDefined(this.activeJobs)) {
        this.areJobsInQueue = this.activeJobs.length > 0;
      }

      // Set the loading to false, so the table can be displayed
      this.loading = false;

      let interval = setInterval(() => {
        this.refreshTable();
      }, 2000);
    });

  }

  refreshTable(): void {
    this.uploadService.getAllActiveJobProgress().subscribe(results => {
      this.activeJobs = results;
      this.areJobsInQueue = this.activeJobs.length > 0;
    });
  }

  getRowClass(job: UploadJobQueue): string {
    switch (job.Status) {
      case UploadJobStatus.NotStarted:
        return 'in-queue-bg';
      case UploadJobStatus.InProgress:
        return 'in-progress-bg';
      case UploadJobStatus.OverrideDatabaseDuplicatesConfirmationAwaited:
        return 'awaiting-confirmation-bg';
      case UploadJobStatus.SmallNumberWarningConfirmationAwaited:
        return 'awaiting-confirmation-bg';
      default:
        return 'default-bg';
    }
  }

  // Forcefully fail the job and mark the status as unexpected error
  setJobToUnexpectedError(job: UploadJobQueue): void {
    this.uploadService.changeJobStatus(job.Guid, UploadJobStatus.UnexpectedError)
      .subscribe(
        (response) => {
          const jobIndex = this.activeJobs.findIndex(x => x.Guid === job.Guid);
          if (jobIndex > -1) {
            this.activeJobs.splice(jobIndex, 1);
            this.areJobsInQueue = this.activeJobs.length > 0;
          }
        },
        (error) => {
          console.log('Failure setting the job to unexpected error.');
        }
      );
  }
}
