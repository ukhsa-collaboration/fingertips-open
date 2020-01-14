import { Component, OnInit, Input, ChangeDetectorRef } from '@angular/core';
import { UploadService } from '../../services/upload.service';
import { UsersService } from '../../services/users.service';
import { forkJoin } from '../../../../node_modules/rxjs';
import { User } from '../../model/user';
import { Select2OptionData } from 'ng-select2';
import { Options } from 'select2';
import { isDefined } from '../../../../node_modules/@angular/compiler/src/util';
import { UploadJobProgress, UploadJob } from '../../model/upload';

@Component({
  selector: 'app-upload-progress',
  templateUrl: './upload-progress.component.html',
  styleUrls: ['./upload-progress.component.css']
})
export class UploadProgressComponent implements OnInit {

  // Incoming members
  @Input() currentUserId: number;

  // private members
  users: User[];
  currentUser: User;

  // public members
  public selectedUser: User;
  public isAdminUser: boolean;
  public options: Options = { width: '600', multiple: false, tags: true };
  public userOptionData: Select2OptionData[] = [];
  public showUserDropdown = false;
  public inProgressCount = 0;
  public inQueueCount = 0;
  public awaitingConfirmationCount = 0;

  constructor(private uploadService: UploadService,
    private usersService: UsersService,
    private ref: ChangeDetectorRef) { }

  ngOnInit() {

    // Define the observable(s)
    const usersObservable = this.usersService.getUsers();

    // API call(s)
    forkJoin([usersObservable]).subscribe(results => {
      // Initialise the list of users
      this.users = results[0];

      // Initialise the current user
      // Selected user will be the same as current user to begin with
      this.currentUser = this.selectedUser = this.users.find(x => x.Id === this.currentUserId);

      // Initialise the user selection dropdown
      this.initialiseUsersDropdown();

      // The user selection dropdown is displayed if the
      // current user is an administrator
      this.isAdminUser = this.currentUser.IsAdministrator;

      let interval = setInterval(() => {
        this.refreshTable();
      }, 2000);
    });
  }

  refreshTable(): void {
    const user = this.selectedUser;
    this.selectedUser = new User();
    if (!this.ref['destroyed']) {
      this.ref.detectChanges();
    }

    this.selectedUser = user;
    if (!this.ref['destroyed']) {
      this.ref.detectChanges();
    }
  }

  initialiseUsersDropdown(): void {
    if (isDefined(this.users)) {
      this.users.forEach(user => {
        this.userOptionData.push({ id: user.Id.toString(), text: user.DisplayName });
      });
    }
  }

  /** Method to handle the user dropdown change event */
  onUserChange(event: any): void {
    // The event input variable holds the user id
    // Find the user
    const user = this.users.find(x => x.Id === Number(event));

    // Hide the user dropdown
    this.toggleUserDropdown();

    // Refresh the job list
    this.selectedUser = user;
    this.ref.detectChanges();
  }

  /** Method to toggle the user dropdown */
  toggleUserDropdown(): void {
    this.showUserDropdown = !this.showUserDropdown;
  }

  /** Method to handle the upload job progress members event emitter function */
  updateUploadJobProgressMembers(uploadJobProgress: UploadJobProgress): void {
    if (uploadJobProgress) {
      this.inProgressCount = uploadJobProgress.InProgress;
      this.inQueueCount = uploadJobProgress.InQueue;
      this.awaitingConfirmationCount = uploadJobProgress.AwaitingConfirmation;
    }
  }
}
