import { Component, OnChanges, Output, EventEmitter, Input, SimpleChanges } from '@angular/core';
import { Profile } from '../../model/profile';
import { ProfileService } from '../../services/profile.service';

@Component({
  selector: 'app-report-profiles',
  templateUrl: './report-profiles.component.html',
  styleUrls: ['./report-profiles.component.css']
})
export class ReportProfilesComponent implements OnChanges {

  @Input() userProfiles: Profile[];
  @Input() selectedProfiles: Profile[];
  @Output() getProfiles = new EventEmitter();

  selectedProfile: Profile;

  constructor(private service: ProfileService) {
  }

  ngOnChanges(changes: SimpleChanges) {
  }

  onProfileChange(profile: Profile) {
    this.selectedProfile = profile;
  }

  addProfile() {
    if (this.selectedProfiles.findIndex(x => x.Id === this.selectedProfile.Id) === -1) {
      this.selectedProfiles.push(this.selectedProfile);
      this.getProfiles.emit(this.selectedProfiles);
    }
  }

  removeProfile(index: number) {
    this.selectedProfiles.splice(index, 1);
    this.getProfiles.emit(this.selectedProfiles);
  }
}
