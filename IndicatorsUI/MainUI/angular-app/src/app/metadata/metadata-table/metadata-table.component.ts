import {
  Component, ViewChild, ElementRef, Output,
  EventEmitter
} from '@angular/core';
import { forkJoin } from 'rxjs';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { SexAndAgeLabelHelper } from '../../shared/shared';
import { IndicatorService } from '../../shared/service/api/indicator.service';
import { ProfileService } from '../../shared/service/api/profile.service';
import { DatePipe } from '@angular/common';
import {
  IndicatorMetadataTextProperty, ProfilePerIndicator, IndicatorMetadata,
  GroupRoot, KeyValuePair, ComparatorMethod, FTModel
} from '../../typings/FT';
import { isDefined } from '@angular/compiler/src/util';
import { ProfileIds } from '../../shared/constants';

@Component({
  selector: 'ft-metadata-table',
  templateUrl: './metadata-table.component.html',
  styleUrls: ['./metadata-table.component.css']
})
export class MetadataTableComponent {

  @Output() isReady = new EventEmitter<boolean>();
  @ViewChild('table', { static: true }) table: ElementRef;

  public rows: MetadataRow[];
  private tempRows: MetadataRow[];
  private showDataQuality: boolean;
  private includeInternalMetadata: boolean;
  private readonly NotApplicable: string = 'n/a';
  private metadataProperties: IndicatorMetadataTextProperty[];
  private indicatorProfiles: Map<number, ProfilePerIndicator[]>;
  private model: FTModel;

  constructor(private ftHelperService: FTHelperService,
    private indicatorService: IndicatorService,
    private profileService: ProfileService) {

    this.showDataQuality = ftHelperService.getFTConfig().showDataQuality;
    this.model = ftHelperService.getFTModel();

    // Display internal metadata for 'Indicators for review' profile
    this.includeInternalMetadata = this.model.profileId === ProfileIds.IndicatorsForReview ? true : false;
  }

  public showInLightbox() {
    this.ftHelperService.showIndicatorMetadataInLightbox(this.table.nativeElement);
  }

  /**
   * For displaying metadata on the Definitions tab
   */
  public displayMetadataForGroupRoot(root: GroupRoot): void {

    this.isReady.emit(false);

    // Get data by AJAX
    const metadataPropertiesObservable = this.indicatorService.getIndicatorMetadataProperties(this.includeInternalMetadata);
    const metadataObservable = this.indicatorService.getIndicatorMetadata(root.Grouping[0].GroupId);
    const indicatorProfilesObservable = this.profileService.getIndicatorProfiles([root.IID]);

    forkJoin([metadataPropertiesObservable, metadataObservable, indicatorProfilesObservable]).subscribe(results => {

      this.metadataProperties = results[0];
      const metadataHash: Map<number, IndicatorMetadata> = results[1];
      this.indicatorProfiles = results[2];

      const indicatorMetadata = metadataHash[root.IID];

      this.displayMetadata(indicatorMetadata, root);

      this.ftHelperService.showAndHidePageElements();
      this.ftHelperService.unlock();
      this.isReady.emit(true);
    });
  }

  /**
   * For displaying metadata in a pop up
   */
  public displayMetadataForIndicator(indicatorId: number, restrictToProfileIds: number[]): void {

    this.isReady.emit(false);

    // Get data by AJAX
    const metadataPropertiesObservable = this.indicatorService.getIndicatorMetadataProperties(this.includeInternalMetadata);
    const metadataObservable = this.indicatorService.getIndicatorMetadataByIndicatorId(indicatorId, restrictToProfileIds);

    forkJoin([metadataPropertiesObservable, metadataObservable]).subscribe(results => {

      this.metadataProperties = results[0];
      const metadataHash: Map<number, IndicatorMetadata> = results[1];
      this.indicatorProfiles = null;

      const indicatorMetadata = metadataHash[indicatorId];
      this.displayMetadata(indicatorMetadata);
      this.isReady.emit(true);
    });
  }

  private displayMetadata(indicatorMetadata: IndicatorMetadata, root?: GroupRoot): void {

    this.tempRows = new Array<MetadataRow>();

    // Assign key variables
    const descriptive = indicatorMetadata.Descriptive;

    // Define IDs
    let propertyIndex, benchmarkingMethodId, benchmarkingSigLevel;
    if (root) {
      benchmarkingMethodId = root.ComparatorMethodId;
      benchmarkingSigLevel = root.Grouping[0].SigLevel;
    } else {
      benchmarkingMethodId = benchmarkingSigLevel = null;
    }

    this.addMetadataRow('Indicator ID', indicatorMetadata.IID);

    if (root) {
      if (root.DateChanges && root.DateChanges.DateOfLastChange && root.DateChanges.DateOfLastChange !== '') {
        let dateOfLastChange = new DatePipe('en-GB').transform(root.DateChanges.DateOfLastChange, 'dd MMM yyyy')
        if (root.DateChanges.HasDataChangedRecently) {
          dateOfLastChange = dateOfLastChange + ' <span class="badge badge-success">New data</span>';
        }
        this.addMetadataRow('Date updated', dateOfLastChange);
      }
    }

    // Initial indicator text properties
    for (propertyIndex = 0; propertyIndex < this.metadataProperties.length; propertyIndex++) {
      const property = this.metadataProperties[propertyIndex];
      if (property.Order > 59) {
        break;
      }

      // Do not include internal metadata here
      if (property.Order > 0) {
        this.addMetadataRowByProperty(descriptive, property);
      }
    }

    // Value type
    this.addMetadataRow('Value type', indicatorMetadata.ValueType.Name);

    // Text - Methodology
    this.addMetadataRowByProperty(descriptive, this.metadataProperties[propertyIndex++]);

    // Unit
    const unit = indicatorMetadata.Unit.Label;
    if (unit) {
      this.addMetadataRow('Unit', indicatorMetadata.Unit.Label);
    }

    // Text - Standard population
    this.addMetadataRowByProperty(descriptive, this.metadataProperties[propertyIndex++]);

    if (root) {
      // Age
      this.addMetadataRow('Age', root.Age.Name);

      // Sex
      this.addMetadataRow('Sex', root.Sex.Name);
    }

    // Year type
    this.addMetadataRow('Year type', indicatorMetadata.YearType.Name);

    // Frequency
    this.addMetadataRowByProperty(descriptive, this.metadataProperties[propertyIndex++]);

    // Benchmarking method
    if (benchmarkingMethodId) {
      const row = new MetadataRow('Benchmarking method', '');
      this.tempRows.push(row);
      this.assignBenchmarkingMethod(row, benchmarkingMethodId);
    }

    // Benchmarking significance level
    if (benchmarkingSigLevel) {
      const text = (benchmarkingSigLevel <= 0) ?
        this.NotApplicable :
        benchmarkingSigLevel + '%';
      this.addMetadataRow('Benchmarking significance level', text);
    }

    // Confidence interval method
    const ciMethod = indicatorMetadata.ConfidenceIntervalMethod;
    if (ciMethod) {

      this.addMetadataRow('Confidence interval method', ciMethod.Name);

      // Display CI method description
      const cimDescription = ciMethod.Description;
      if (cimDescription) {
        this.addMetadataRow('Confidence interval methodology', cimDescription);
      }
    }

    // Confidence level
    const confidenceLevel = indicatorMetadata.ConfidenceLevel;
    if (confidenceLevel > -1) {
      this.addMetadataRow('Confidence level', confidenceLevel + '%');
    }

    // Display all remaining properties
    while (propertyIndex < this.metadataProperties.length) {
      this.addMetadataRowByProperty(descriptive, this.metadataProperties[propertyIndex]);
      propertyIndex++;
    }

    if (this.includeInternalMetadata) {
      const internalMetadata = this.metadataProperties.filter(x => x.Order === 0);

      for (propertyIndex = 0; propertyIndex < internalMetadata.length; propertyIndex++) {
        this.addMetadataRowByProperty(descriptive, this.metadataProperties[propertyIndex]);
      }
    }

    this.addIndicatorProfiles(indicatorMetadata.IID);

    // Trigger view refresh
    this.rows = this.tempRows;
  }

  private addIndicatorProfiles(indicatorId: number) {

    const indicatorProfiles = this.indicatorProfiles;

    if (indicatorProfiles) {
      const profiles: string[] = [];
      for (let i = 0; i < indicatorProfiles[indicatorId].length; i++) {
        const profilePerIndicator: ProfilePerIndicator = indicatorProfiles[indicatorId][i];
        profiles.push('<a href="' + profilePerIndicator.Url + '" target="_blank">' + profilePerIndicator.ProfileName + '</a>')
      }

      this.addMetadataRow('Profiles included in', profiles.join(', '));
    }
  }

  private addMetadataRow(header: string, text: string | number): void {
    this.tempRows.push(new MetadataRow(header, text));
  }

  private addMetadataRowByProperty(textMetadata: KeyValuePair<string, string>[], property: IndicatorMetadataTextProperty): void {

    const columnName = property.ColumnName;

    if (textMetadata !== null && textMetadata.hasOwnProperty(columnName)) {
      const text = textMetadata[columnName];

      if (isDefined(text) && text !== '') {

        let displayText: string;
        if (columnName === 'Name') {
          const groupRoot = this.ftHelperService.getCurrentGroupRoot();
          const sexAndAgeLabel = new SexAndAgeLabelHelper(groupRoot).getSexAndAgeLabel();
          displayText = text + sexAndAgeLabel;
        } else if (columnName === 'DataQuality' && this.showDataQuality) {
          displayText = this.getDataQualityHtml(text, displayText);
        } else if (columnName === 'Rationale' || columnName === 'SpecificRationale') {

          if (columnName === 'SpecificRationale') {
            if (this.existRationaleData(textMetadata)) {
              return;
            } else {
              property.DisplayName = 'Rationale';
            }
          }
          displayText = this.getRationaleSpecificRationaleHtml(textMetadata);
        } else {
          displayText = text;
        }

        const row = new MetadataRow(property.DisplayName, displayText);
        this.tempRows.push(row);
      } else {
        if (this.model.profileId === ProfileIds.IndicatorsForReview) {
          const row = new MetadataRow(property.DisplayName, '');
          this.tempRows.push(row);
        }
      }
    }
  }

  private existRationaleData(textMetadata: KeyValuePair<string, string>[]): boolean {
    return textMetadata['Rationale'] !== undefined;
  }

  private existSpecificRationaleData(textMetadata: KeyValuePair<string, string>[]): boolean {
    return textMetadata['SpecificRationale'] !== undefined;
  }

  private getRationaleSpecificRationaleHtml(textMetadata: KeyValuePair<string, string>[]) {

    let displayText = '';
    if (this.existRationaleData(textMetadata)) {
      displayText = textMetadata['Rationale'];
    }

    if (this.existSpecificRationaleData(textMetadata)) {
      if (displayText !== '') {
        displayText = displayText.concat('<br>');
      }
      displayText = displayText.concat(textMetadata['SpecificRationale']);
    }
    return displayText;
  }

  private getDataQualityHtml(text: any, displayText: string) {
    // Add data quality flags instead of number
    const dataQualityCount = parseInt(text, 10);
    displayText = this.ftHelperService.getIndicatorDataQualityHtml(text) + ' ' +
      this.ftHelperService.getIndicatorDataQualityTooltipText(dataQualityCount);
    return displayText;
  }

  private assignBenchmarkingMethod(row: MetadataRow, benchmarkingMethodId: number) {
    const metadataObservable = this.indicatorService.getBenchmarkingMethod(benchmarkingMethodId).subscribe(
      data => {
        const method = <ComparatorMethod>data;
        row.text = method.Name;
      },
      error => { });
  }
}

class MetadataRow {
  constructor(public header: string, public text: string | number) { }
}
