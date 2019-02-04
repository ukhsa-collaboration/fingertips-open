
import { Component, OnChanges, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { ProfileService } from '../../../services/profile.service';
import { Profile } from '../../../model/profile';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-profile-list',
  templateUrl: './profile-list.component.html',
  styleUrls: ['./profile-list.component.css'],
  providers: [ProfileService]
})

export class ProfileListComponent implements OnChanges {

  @Input() userProfiles: Profile[];
  @Output() getSelectedProfile = new EventEmitter();

  selectedProfile: Profile;
  profileListForm: FormGroup;

  constructor(private service: ProfileService) {
    this.profileListForm = new FormGroup({
      profileListControl: new FormControl(null)
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['userProfiles']) {
      if (this.userProfiles !== undefined) {
      }
    }
  }

  profileChange(event) {
    let profileId = Number(this.profileListForm.get('profileListControl').value);

    let profile = new Profile();
    profile.Id = profileId;
    profile.Name = this.userProfiles.find(x => x.Id === profileId).Name;
    this.selectedProfile = profile;

    this.getSelectedProfile.emit(this.selectedProfile);
  }
}
