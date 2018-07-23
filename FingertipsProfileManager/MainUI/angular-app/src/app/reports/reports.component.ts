import { Component, OnInit } from '@angular/core';
import { Report } from '../model/report';
import { Profile } from '../model/profile';
import { ReportsService } from '../services/reports.service';
import { ProfileService } from '../services/profile.service';
import { ProfileListComponent } from '../shared/profile-list/profile-list.component';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css']
})

export class ReportsComponent implements OnInit {

  userProfilesHash: { [id: number]: Profile } = {};
  reports: Report[];
  isInit: boolean = false;
  private status: number;
  // TODO: should have status enum
  private StatusNew: number = 0;
  private StatusEdit: number = 1;
  private StatusList: number = 2;
  userProfiles: Profile[];
  private editProfileId: number;

  constructor(private profileService: ProfileService) {
    this.status = this.StatusList;
  }

  ngOnInit() {
    this.getUserProfiles();
    this.isInit = true;
  }

  getReportViewStatus(status) {
    this.status = status;
  }

  getListViewStatus(listViewState) {
    this.status = listViewState.status;
    this.editProfileId = listViewState.profileId;
  }

  getUserProfiles() {
    this.profileService.getProfiles()
      .subscribe(data => {
        this.userProfiles = data         
        this.buildUserProfileHash();
      }
      );
  }

  buildUserProfileHash() {
    if (this.userProfiles != null) {
      this.userProfiles.forEach(profile => {
        this.userProfilesHash[profile.Id] = profile;
      });    
    }
  }
}
