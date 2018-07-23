import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { ProfileListComponent } from '../../shared/profile-list/profile-list.component';
import { Profile } from '../../model/profile';
import { ProfileService } from '../../services/profile.service';
import * as _ from 'underscore';


@Component({
  selector: 'app-report-profiles',
  templateUrl: './report-profiles.component.html',
  styleUrls: ['./report-profiles.component.css']
})
export class ReportProfilesComponent implements OnInit {

  userProfiles: any;
  selectedProfile: any;
  @Input() userProfilesHash: any;
  @Input() selectedProfiles: number[];
  @Output() getProfiles: EventEmitter<number[]> = new EventEmitter();

  constructor(private service: ProfileService) {
  }

  ngOnInit() {
    this.service.getProfiles()
      .subscribe(
      data => {
        this.userProfiles = data;
      }
    );
  }

  onProfileChange(event: number) {
    this.selectedProfile = event;
  }

  addProfile() { 
    // We need to convert is to number please see the link
    // https://stackoverflow.com/questions/39562430/angular2-object-property-typed-as-number-changes-to-string
    let profileId = parseInt(this.selectedProfile);
    
    if (!_.contains(this.selectedProfiles,profileId) && profileId) {
      this.selectedProfiles.push(profileId);
      this.getProfiles.emit(this.selectedProfiles);
    }
  }

  removeProfile(index: number) {
    this.selectedProfiles.splice(index, 1);
    this.getProfiles.emit(this.selectedProfiles);
  }
}
