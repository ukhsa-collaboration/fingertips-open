
import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import {ProfileService} from '../../services/profile.service';

import { Profile} from '../../model/profile';

@Component({
  selector: 'app-profile-list',
  templateUrl: './profile-list.component.html',
  styleUrls: ['./profile-list.component.css']
})

export class ProfileListComponent implements OnInit {
  @Output() getSelectedProfile: EventEmitter<number> = new EventEmitter();

  profiles:Profile[];
  selectedProfile: number;
  
  constructor(private service: ProfileService) { 
    this.service.getProfiles().subscribe(data=> this.profiles = data);    
  }

  ngOnInit() {
  }

  profileChange(event){    
    this.getSelectedProfile.emit(this.selectedProfile);
  }  
}