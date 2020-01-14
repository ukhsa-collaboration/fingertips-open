import { Component, OnInit, Input } from '@angular/core';
import { TabOptions } from '../model/upload';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {

  // private members
  private batchTemplateUrl: string;
  private batchLastUpdated: string;
  private currentUserId: number;

  // public members
  public tabOption: TabOptions;
  public tabClass = 'cursor-pointer';
  public tabActiveClass = 'cursor-pointer active';
  public uploadTabClass = 'cursor-pointer active';
  public progressTabClass = 'cursor-pointer';
  public queueTabClass = 'cursor-pointer';

  constructor() { }

  ngOnInit() {

    // Read the attributes of the app-upload tag and populate the local variables
    const appRootElement = $('#app-upload-container');
    this.batchTemplateUrl = appRootElement.attr('batch-template-url');
    this.batchLastUpdated = appRootElement.attr('batch-last-updated');
    this.currentUserId = Number(appRootElement.attr('current-user'));

    // Initialise tab styles
    this.initialiseTabs();
  }

  initialiseTabs(): void {
    // Display the upload new data tab
    this.setTabOption(TabOptions.Index);

    // Set the tab classes accordingly
    this.setTabStyles(TabOptions.Index);
  }

  setTabStyles(tabId: number): void {
    this.uploadTabClass = this.progressTabClass = this.queueTabClass = this.tabClass;

    switch (tabId) {
      case TabOptions.Index:
        this.uploadTabClass = this.tabActiveClass;
        break;
      case TabOptions.Progress:
        this.progressTabClass = this.tabActiveClass;
        break;
      case TabOptions.Queue:
        this.queueTabClass = this.tabActiveClass;
        break;
    }
  }

  setTabOption(tabId: number): void {
    this.tabOption = tabId;
  }

  navigateToYourUploadJobsTab(): void {
    this.setTabOption(TabOptions.Progress);
    this.setTabStyles(TabOptions.Progress);
  }
}
