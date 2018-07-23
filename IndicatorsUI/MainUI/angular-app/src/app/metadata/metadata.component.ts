import { Component, HostListener, ViewChild } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import {
  GroupRoot
} from '../typings/FT.d';

@Component({
  selector: 'ft-metadata',
  templateUrl: './metadata.component.html',
  styleUrls: ['./metadata.component.css']
})
export class MetadataComponent {
  @ViewChild('table') table;

  constructor(private ftHelperService: FTHelperService, ) { }

  @HostListener('window:MetadataSelected', ['$event'])
  public onOutsideEvent(event): void {
    let root: GroupRoot = this.ftHelperService.getCurrentGroupRoot();
    var grouping = root.Grouping[0];
    this.table.displayMetadataForGroupRoot(root);
  }
}


