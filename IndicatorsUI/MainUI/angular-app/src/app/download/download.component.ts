import { Component, HostListener, ViewChild } from '@angular/core';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';

@Component({
  selector: 'ft-download',
  templateUrl: './download.component.html',
  styleUrls: ['./download.component.css']
})
export class DownloadComponent {

  @ViewChild('downloadData') downloadDataComponent;
  @ViewChild('downloadReport') downloadReportComponent;

  constructor(private ftHelperService: FTHelperService) { }

  @HostListener('window:DownloadSelected', ['$event'])
  public onOutsideEvent(event) {

    // Refresh child components
    this.downloadDataComponent.refresh();
    this.downloadReportComponent.refresh();

    // Unlock UI
    this.ftHelperService.showAndHidePageElements();
    this.ftHelperService.unlock();
  }
}