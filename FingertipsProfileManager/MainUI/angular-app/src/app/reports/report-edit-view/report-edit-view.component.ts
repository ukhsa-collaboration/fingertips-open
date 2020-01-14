import { Component, OnChanges, Output, EventEmitter, Input, SimpleChanges } from '@angular/core';
import { Report, ReportListView } from '../../model/report';
import { Profile, AreaType } from '../../model/profile';
import { ReportsService } from '../../services/reports.service';
import { ReportStatus } from '../reports';
import { LightBoxConfig, LightBoxTypes } from '../../shared/component/light-box/light-box';
import { Select2OptionData } from 'ng-select2';
import { Options } from 'select2';

@Component({
  selector: 'app-report-edit-view',
  templateUrl: './report-edit-view.component.html',
  styleUrls: ['./report-edit-view.component.css']
})

export class ReportEditViewComponent implements OnChanges {
  @Input() status: any;
  @Input() profiles: Profile[];
  @Input() userProfiles: Profile[];
  @Input() areaTypes: AreaType[];
  @Input() selectedReport: ReportListView;
  @Output() getReportViewStatus: EventEmitter<number> = new EventEmitter<number>();

  reportId: number;
  validationError: string;
  lightBoxConfig: LightBoxConfig;

  profileOptionData: Select2OptionData[] = [];
  parameterOptionData: Select2OptionData[] = [];
  areaTypeOptionData: Select2OptionData[] = [];
  options: Options = { width: '600', multiple: true, tags: true };
  selectedProfiles: any[] = [];
  selectedParameters: string[] = [];
  selectedAreaTypes: any[] = [];

  constructor(private service: ReportsService) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['profiles']) {
      if (this.profiles) {
        this.initialiseProfilesDropdown();
        this.initialiseParametersDropdown();
      }
    }
    if (changes['areaTypes']) {
      if (this.areaTypes) {
        this.initialiseAreaTypesDropdown();
      }
    }
    if (changes['selectedReport']) {
      if (this.selectedReport !== undefined) {
        this.loadDropdownDefaultValues(this.selectedReport);
      }
    }
  }

  loadDropdownDefaultValues(report: ReportListView): void {
    if (report.profiles.length > 0) {
      report.profiles.forEach(profile => {
        this.selectedProfiles.push(profile.Id.toString());
      });
    }

    if (report.parameters.length > 0) {
      report.parameters.forEach(parameter => {
        this.selectedParameters.push(parameter);
      });
    }

    if (report.areaTypes !== undefined && report.areaTypes.length > 0) {
      report.areaTypes.forEach(areaType => {
        this.selectedAreaTypes.push(areaType.Id.toString());
      });
    }
  }

  initialiseProfilesDropdown(): void {
    if (this.profiles !== undefined) {
      if (this.profiles.length > 0) {
        this.profiles.forEach(profile => {
          this.profileOptionData.push({ id: profile.Id.toString(), text: profile.Name });
        });
      }
    }
  }

  initialiseParametersDropdown(): void {
    const parameters: string[] = ['areaCode', 'areaTypeId', 'groupId',
      'parentCode', 'parentTypeId'];

    parameters.forEach(parameter => {
      this.parameterOptionData.push({ id: parameter, text: parameter });
    });
  }

  initialiseAreaTypesDropdown(): void {
    if (this.areaTypes !== undefined) {
      if (this.areaTypes.length > 0) {
        this.areaTypes.forEach(areaType => {
          this.areaTypeOptionData.push({ id: areaType.Id.toString(), text: areaType.ShortName });
        });
      }
    }
  }

  saveReport(): void {
    if (this.isFormValid()) {
      const report = new Report();
      report.name = this.selectedReport.name;
      report.file = this.selectedReport.file;
      report.notes = this.selectedReport.notes;
      report.isLive = this.selectedReport.isLive;

      const profiles: number[] = [];
      this.selectedReport.profiles.forEach(profile => {
        profiles.push(profile.Id);
      });
      report.profiles = profiles;

      const parameters: string[] = [];
      this.selectedReport.parameters.forEach(paramater => {
        parameters.push(paramater);
      });
      report.parameters = parameters;

      const areaTypeIds: number[] = [];
      this.selectedReport.areaTypes.forEach(areaType => {
        areaTypeIds.push(areaType.Id);
      });
      report.areaTypeIds = areaTypeIds;

      if (this.selectedReport.id != null) {
        report.id = this.selectedReport.id;

        this.service.updateReport(report).subscribe(
          (response) => {
            this.getReportViewStatus.emit(ReportStatus.List);
          },
          (error) => {
            console.log('Unable to update the report');
          }
        );
      } else {
        this.service.saveReport(report).subscribe(
          (response) => {
            this.getReportViewStatus.emit(ReportStatus.List);
          },
          (error) => {
            console.log('Unable to save the report');
          }
        );
      }
    }
  }

  cancelReport(): void {
    this.getReportViewStatus.emit(ReportStatus.List);
  }

  // Show light box for delete report confirmation
  deleteReportConfirmation(): void {
    // Show light box
    const config = new LightBoxConfig();
    config.Type = LightBoxTypes.OkCancel;
    config.Title = 'Delete report';
    config.Html = 'Are you sure you want to delete the report <b>' + this.selectedReport.name + '</b>?';
    config.OkButtonText = 'Delete';
    config.CancelButtonText = 'Cancel';
    config.ActionType = 'DELETE';
    this.lightBoxConfig = config;
  }

  // Delete report action confirmed
  // i.e., Ok button clicked on the light box
  updateLightBoxActionConfirmed(actionConfirmed: boolean): void {
    if (actionConfirmed) {
      switch (this.lightBoxConfig.ActionType) {
        case 'DELETE':
          this.deleteReport();
          break;
      }
    }
  }

  // Delete report
  deleteReport(): void {
    this.service.deleteReportById(this.selectedReport.id)
      .subscribe(
        (response) => {
          this.getReportViewStatus.emit(ReportStatus.List);
        },
        (error) => {
          console.log('Unable to delete the report');
        }
      );
  }

  // Validate the form entries before saving
  isFormValid(): boolean {
    let isValid = false;

    if (!this.selectedReport.name) {
      this.validationError = 'Report name is required';
    } else {
      isValid = true;
    }

    return isValid;
  }

  // Profile dropdown value changed
  onProfileChanged(event: any): void {
    // Reset the profiles array
    this.selectedReport.profiles.length = 0;

    // Add new values to the profiles array
    event.forEach(profileId => {
      const profile = this.profiles.find(x => x.Id === Number(profileId));
      this.selectedReport.profiles.push(profile);
    });
  }

  // Parameter dropdown value changed
  onParameterChanged(event: any): void {
    // Reset the parameters array
    this.selectedReport.parameters.length = 0;

    // Add new values to the parameters array
    event.forEach(parameter => {
      this.selectedReport.parameters.push(parameter);
    });
  }

  // Area type dropdown value changed
  onAreaTypeChanged(event: any): void {
    // Reset the area types array
    if (this.selectedReport.areaTypes !== undefined) {
      this.selectedReport.areaTypes.length = 0;
    }

    // Add new values to the area types array
    event.forEach(areaTypeId => {
      const areaType = this.areaTypes.find(x => x.Id === Number(areaTypeId));
      this.selectedReport.areaTypes.push(areaType);
    });
  }
}
