import { Component, OnInit, ChangeDetectorRef, ViewChild } from '@angular/core';
import { ProfileService } from '../../services/profile.service';
import { GroupingPlusName, GroupingSubheading, ReorderIndicator, AreaType } from '../../model/profile';
import { LightBoxTypes, LightBoxConfig } from '../../shared/component/light-box/light-box.component';
import { LightBoxIndicatorReorderConfig } from '../../shared/component/light-box-indicator-reorder/light-box-indicator-reorder.component';
import { Observable } from 'rxjs';
import { SpinnerComponent } from 'app/shared/component/spinner/spinner.component';
declare let $: JQueryStatic;

@Component({
  selector: 'app-indicators-reorder-view',
  templateUrl: './indicators-reorder-view.component.html',
  styleUrls: ['./indicators-reorder-view.component.css'],
  providers: [ProfileService]
})
export class IndicatorsReorderViewComponent implements OnInit {

  @ViewChild(SpinnerComponent) spinner;

  profileId: number;
  profileUrlKey: string;
  domainSequence: number;
  domainName: string;
  subheadingId: number;
  areaTypeId: number;
  areaTypeName: string;
  sequence: number;
  groupId: number;
  groupingPlusNames: GroupingPlusName[];
  groupingSubheadings: GroupingSubheading[];
  allgroupingSubheadings: GroupingSubheading[];
  areaTypes: AreaType[];
  reorderIndicators: ReorderIndicator[] = [];
  lightBoxConfig: LightBoxConfig;
  lightBoxIndicatorReorderConfig: LightBoxIndicatorReorderConfig;
  subheading: string;
  subheadingToEdit: string;
  errorMessage: string;

  constructor(private profileService: ProfileService, private ref: ChangeDetectorRef) { }

  ngOnInit() {

    // Read the attributes of the app-root tag and populate the local variables
    const appRootElement = $('#app-root');
    this.profileId = Number(appRootElement.attr('profile-id'));
    this.profileUrlKey = appRootElement.attr('profile-key');
    this.domainSequence = Number(appRootElement.attr('domain-seq'));
    this.areaTypeId = Number(appRootElement.attr('area-type-id'));
    this.groupId = Number(appRootElement.attr('group-id'));

    // Load indicators and subheadings data
    this.loadData();
  }

  // Load the indicators and subheadings data from the database
  loadData() {
    const areaTypesObservable = this.profileService.getAllAreaTypes();
    const domainNameObservable = this.profileService.getDomainName(this.groupId, this.domainSequence);

    Observable.forkJoin([areaTypesObservable, domainNameObservable]).subscribe(results => {
      this.areaTypes = <AreaType[]>results[0];
      this.domainName = <string>results[1];

      this.areaTypeName = this.areaTypes.find(x => x.Id === this.areaTypeId).ShortName;
    });

    const groupingPlusNamesObservable = this.profileService.getGroupingPlusNames(this.profileUrlKey, this.domainSequence, this.areaTypeId);
    const groupingSubheadingsObservable = this.profileService.getGroupingSubheadingsForProfile(this.profileId);

    Observable.forkJoin([groupingPlusNamesObservable, groupingSubheadingsObservable]).subscribe(results => {
      this.groupingPlusNames = <GroupingPlusName[]>results[0];
      this.allgroupingSubheadings = <GroupingSubheading[]>results[1];

      this.groupingSubheadings = this.allgroupingSubheadings.filter(x => x.AreaTypeId === this.areaTypeId && x.GroupId === this.groupId);

      this.displayIndicatorsAndSubheadings();

      this.spinner.hide();
    });
  }

  // Display indicators and subheadings in a table
  displayIndicatorsAndSubheadings() {
    this.groupingPlusNames.forEach(groupingPlusName => {
      const groupingSubheadings = this.groupingSubheadings.filter(x => x.Sequence === groupingPlusName.Sequence);
      groupingSubheadings.forEach(groupingSubheading => {
        // Subheading
        this.addGroupingSubheadingToReorderIndicatorList(groupingSubheading, false);
      });

      // Indicator
      this.addGroupingPlusNameToReorderIndicatorList(groupingPlusName);
    });
  }

  // Add subheading to the reorder indicator list for display
  addGroupingSubheadingToReorderIndicatorList(groupingSubheading: GroupingSubheading, addToFirstPosition: boolean) {
    let reorderIndicator: ReorderIndicator;

    reorderIndicator = {
      IndicatorId: groupingSubheading.SubheadingId,
      IndicatorName: groupingSubheading.Subheading,
      Sequence: groupingSubheading.Sequence,
      Sex: '',
      Age: '',
      IsSubheading: true
    }

    if (addToFirstPosition) {
      this.reorderIndicators.unshift(reorderIndicator);
    } else {
      this.reorderIndicators.push(reorderIndicator);
    }

    this.ref.detectChanges();
  }

  // Add indicator to the reorder indicator list for display
  addGroupingPlusNameToReorderIndicatorList(groupingPlusName: GroupingPlusName) {
    let reorderIndicator: ReorderIndicator;

    reorderIndicator = {
      IndicatorId: groupingPlusName.IndicatorId,
      IndicatorName: groupingPlusName.IndicatorName,
      Sequence: groupingPlusName.Sequence,
      Sex: groupingPlusName.Sex,
      Age: groupingPlusName.Age,
      IsSubheading: false,
    }

    this.reorderIndicators.push(reorderIndicator);
  }

  // Save subheading details and indicators sequence changes to the database
  save() {
    // Define an array by reading all the rows displayed
    const rows = Array.from($('.tablesorter-dataRow'));

    // Update the sequence for indicators
    let sequenceCounter = 0;
    rows.forEach(row => {
      const isSubheading = row.children[0].textContent;
      if (isSubheading === 'false') {
        sequenceCounter++;
        const indicatorId = Number(row.children[2].textContent);
        const sex = row.children[4].textContent.toLowerCase();
        this.groupingPlusNames.find(x => x.IndicatorId === indicatorId && x.Sex.toLowerCase() === sex).Sequence = sequenceCounter;
      }
    });

    // Update the sequence for subheadings
    rows.forEach(row => {
      const isSubheading = row.children[0].textContent;
      if (isSubheading === 'true') {
        // Read the subheading id
        const subheadingId = Number(row.children[2].textContent);
        if (subheadingId !== -1) {
          // Update the sequence number if it is not -1
          this.groupingSubheadings.find(x => x.SubheadingId === subheadingId).Sequence = Number(row.children[1].textContent);
        } else {
          // Set the sequence number to 1
          this.groupingSubheadings.find(x => x.SubheadingId === subheadingId).Sequence = 1;
        }
      }
    });

    // Save the details
    const formData: FormData = new FormData();
    formData.append('profileId', this.profileId.toString());
    formData.append('profileUrlKey', this.profileUrlKey);
    formData.append('domainSequence', this.domainSequence.toString());
    formData.append('areaTypeId', this.areaTypeId.toString());
    formData.append('groupId', this.groupId.toString());
    formData.append('groupingPlusNames', JSON.stringify(this.groupingPlusNames));
    formData.append('groupingSubheadings', JSON.stringify(this.groupingSubheadings));

    this.profileService.saveReorderedIndicators(formData)
      .subscribe(
        (response) => {
          // Saved successfully, go back to the previous page
          this.goToParentPage();
        },
        (error) => {
          this.errorMessage = 'Unable to save the changes, please try again.<br>' +
            'If the issue persists then please contact the administrator.';
        }
      );
  }

  // Cancel button event
  cancel() {
    // Go back to the previous page
    this.goToParentPage();
  }

  // Function to redirect to the previous page
  goToParentPage() {
    window.location.href = 'profile/indicators/specific?ProfileKey=' +
      this.profileUrlKey +
      '&DomainSequence=' +
      this.domainSequence +
      '&SelectedAreaTypeId=' +
      this.areaTypeId;
  }

  // Add subheading button event, to show light box for add subheading confirmation
  addSubHeadingConfirmation() {
    // Get grouping subheadings associated with other area types
    const filteredGroupingSubheadings: GroupingSubheading[] = this.getGroupingSubheadingsOfOtherAreaTypes();

    // Show light box
    const config = new LightBoxIndicatorReorderConfig();
    config.Title = 'Add subheading';
    config.OkButtonText = 'Add';
    config.CancelButtonText = 'Cancel';
    config.InputBoxLabelName = 'Name';
    config.ActionType = 'ADD';
    config.Height = 400;

    if (filteredGroupingSubheadings.length > 0) {
      config.GroupingSubheadings = this.sortGroupingSubheadings(filteredGroupingSubheadings);
    }

    this.lightBoxIndicatorReorderConfig = config;
  }

  // Add subheading
  addSubheading() {
    let groupingSubheading: GroupingSubheading;

    groupingSubheading = {
      AreaTypeId: this.areaTypeId,
      GroupId: this.groupId,
      Sequence: 1,
      SubheadingId: -1,
      Subheading: this.subheading,
    }

    this.groupingSubheadings.push(groupingSubheading);
    this.addGroupingSubheadingToReorderIndicatorList(groupingSubheading, true);
  }

  // Show light box for edit subheading confirmation
  editSubheadingConfirmation(subheadingId: number) {
    this.subheadingId = subheadingId;

    const subheading = this.groupingSubheadings.find(x => x.SubheadingId === subheadingId).Subheading;
    this.subheading = subheading;
    this.subheadingToEdit = subheading;

    // Get grouping subheadings associated with other area types
    const filteredGroupingSubheadings: GroupingSubheading[] = this.getGroupingSubheadingsOfOtherAreaTypes();

    // Show light box
    const config = new LightBoxIndicatorReorderConfig();
    config.Title = 'Edit subheading';
    config.OkButtonText = 'Save';
    config.CancelButtonText = 'Cancel';
    config.InputBoxLabelName = 'Name';
    config.Height = 400;
    config.InputBoxText = this.subheading;
    config.ActionType = 'EDIT';

    if (filteredGroupingSubheadings.length > 0) {
      config.GroupingSubheadings = this.sortGroupingSubheadings(filteredGroupingSubheadings);
    }

    this.lightBoxIndicatorReorderConfig = config;
  }

  // Edit subheading
  editSubheading() {
    this.groupingSubheadings.find(x => x.SubheadingId === this.subheadingId).Subheading = this.subheading;
    this.reorderIndicators.find(x => x.IndicatorId === this.subheadingId && x.IsSubheading === true).IndicatorName = this.subheading;
    this.ref.detectChanges();
  }

  // Show light box for delete subheading confirmation
  deleteSubheadingConfirmation(subheadingId: number) {
    this.subheadingId = subheadingId;
    const subheading = this.groupingSubheadings.find(x => x.SubheadingId === subheadingId).Subheading;

    // Show light box
    const config = new LightBoxConfig();
    config.Type = LightBoxTypes.OkCancel;
    config.Title = 'Delete subheading';
    config.Html = 'Are you sure you want to delete the subheading <b>' + subheading + '</b>?';
    config.OkButtonText = 'Delete';
    config.CancelButtonText = 'Cancel';
    config.ActionType = 'DELETE';
    this.lightBoxConfig = config;
  }

  // Delete subheading
  deleteSubheading() {
    const groupingSubheadingToDeleteIndex = this.groupingSubheadings.findIndex(x => x.SubheadingId === this.subheadingId);
    this.groupingSubheadings.splice(groupingSubheadingToDeleteIndex, 1);

    const reorderIndicatorToDeleteIndex = this.reorderIndicators.findIndex(x => x.IndicatorId === this.subheadingId);
    this.reorderIndicators.splice(reorderIndicatorToDeleteIndex, 1);

    this.ref.detectChanges();
  }

  // Sort grouping subheadings
  sortGroupingSubheadings(groupingSubheadings: GroupingSubheading[]): GroupingSubheading[] {
    let sortedGroupingSubheadings: GroupingSubheading[] = [];
    sortedGroupingSubheadings = groupingSubheadings.slice(0);

    sortedGroupingSubheadings.sort((a, b): number => {
      if (a.Subheading < b.Subheading) {
        return -1;
      } else if (a.Subheading > b.Subheading) {
        return 1;
      } else {
        return 0;
      }
    });

    return sortedGroupingSubheadings;
  }

  getGroupingSubheadingsOfOtherAreaTypes(): GroupingSubheading[] {
    const groupingSubheadingsOfOtherAreaTypes = this.allgroupingSubheadings.filter(x => x.AreaTypeId !== this.areaTypeId);
    const filteredGroupingSubheadings: GroupingSubheading[] = [];

    groupingSubheadingsOfOtherAreaTypes.forEach(groupingSubheadingsOfOtherAreaType => {
      if (this.groupingSubheadings.findIndex(x => x.Subheading.toLowerCase() === groupingSubheadingsOfOtherAreaType.Subheading.toLowerCase()) === -1) {
        if (filteredGroupingSubheadings.findIndex(x => x.Subheading.toLowerCase() === groupingSubheadingsOfOtherAreaType.Subheading.toLowerCase()) === -1) {
          filteredGroupingSubheadings.push(groupingSubheadingsOfOtherAreaType);
        }
      }
    });

    return filteredGroupingSubheadings;
  }

  // Event emitter functions
  updateLightBoxInputText(subheading: string) {
    this.subheading = subheading;
  }

  updateLightBoxActionConfirmed(actionConfirmed: boolean) {
    if (actionConfirmed) {
      switch (this.lightBoxConfig.ActionType) {
        case 'DELETE':
          this.deleteSubheading();
          break;
      }
    }
  }

  updateLightBoxIndicatorReorderActionConfirmed(actionConfirmed: boolean) {
    if (actionConfirmed) {
      switch (this.lightBoxIndicatorReorderConfig.ActionType) {
        case 'ADD':
          this.addSubheading();
          break;
        case 'EDIT':
          this.editSubheading();
          break;
      }
    }
  }
}

$(function () {
  // JQuery UI methods should be cast to <any> to avoid compile time errors
  (<any>$('tbody')).sortable({
    items: '> tr',
    stop: function (event, ui) {
      // Define an array by reading all the rows displayed
      const rows = Array.from($('.tablesorter-dataRow'));

      // Update the sequence for indicators
      let sequenceCounter = 0;
      rows.forEach(row => {
        const isSubheading = row.children[0].textContent;
        if (isSubheading === 'false') {
          sequenceCounter++;
          row.children[1].textContent = sequenceCounter.toString();
        }
      });

      // Update the sequence for subheadings
      let rowCounter = 0;
      rows.forEach(row => {
        const isSubheading = row.children[0].textContent;
        if (isSubheading === 'true') {
          let td = row.children[1];
          $(td).addClass('white');
          if (rows[rowCounter + 1].children[0].textContent === 'true') {
            td.textContent = "1";
          } else {
            td.textContent = rows[rowCounter + 1].children[1].textContent;
          }
        }

        rowCounter++;
      });
    }
  });
});
