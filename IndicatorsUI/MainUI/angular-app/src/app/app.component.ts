import { Component, HostListener } from '@angular/core';
import {FTModel, FTRoot} from './typings/FT.d';
declare var FTWrapper: FTRoot;

@Component({
  selector: '[app-root]',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']  
})

export class AppComponent {
 
  isInitialised: boolean = false;
  i: number = FTWrapper.model().profileId;
  title = 'app works!' ;

  @HostListener('window:Test-Event', ['$event'])
  public onOutsideEvent(event) {
    this.isInitialised = true;
    console.log(`The user just pressed ${event.detail}!`);
  }
}
