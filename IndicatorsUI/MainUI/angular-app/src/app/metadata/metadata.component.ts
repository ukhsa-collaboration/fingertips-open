import { Component, HostListener, ViewChild } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import 'rxjs/rx';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import {
  GroupRoot
} from '../typings/FT';

@Component({
  selector: 'ft-metadata',
  templateUrl: './metadata.component.html',
  styleUrls: ['./metadata.component.css']
})
export class MetadataComponent {
  @ViewChild('table', { static: true }) table;

  constructor(private ftHelperService: FTHelperService, ) { }

  @HostListener('window:MetadataSelected', ['$event'])
  public onOutsideEvent(event): void {
    const root: GroupRoot = this.ftHelperService.getCurrentGroupRoot();
    this.table.displayMetadataForGroupRoot(root);
  }
}


