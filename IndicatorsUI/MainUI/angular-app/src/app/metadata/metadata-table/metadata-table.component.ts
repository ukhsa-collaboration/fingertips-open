import {
  Component, HostListener, Input, ViewChild, ElementRef, Output,
  EventEmitter
} from '@angular/core';
import { Observable } from 'rxjs/Observable';
import {
  FTModel, FTRoot, GroupRoot, IndicatorMetadata, TooltipManager, IndicatorMetadataTextProperty,
  KeyValuePair, ComparatorMethod, ProfilePerIndicator
} from '../../typings/FT.d';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { TooltipHelper } from '../../shared/shared';
import { IndicatorService } from '../../shared/service/api/indicator.service';
import { ProfileService } from '../../shared/service/api/profile.service';
import * as _ from 'underscore';
import { DatePipe } from '@angular/common';


@Component({
  selector: 'ft-metadata-table',
  templateUrl: './metadata-table.component.html',
  styleUrls: ['./metadata-table.component.css']
})
export class MetadataTableComponent {

  @Output() isReady = new EventEmitter<boolean>();
  @ViewChild('table') table: ElementRef;

  public rows: MetadataRow[];
  private tempRows: MetadataRow[];
  private showDataQuality: boolean;
  private readonly NotApplicable: string = 'n/a';
  private metadataProperties: IndicatorMetadataTextProperty[];
  private indicatorProfiles: Map<number, ProfilePerIndicator[]>;

  constructor(private ftHelperService: FTHelperService,
    private indicatorService: IndicatorService,
    private profileService: ProfileService) {
    this.showDataQuality = ftHelperService.getFTConfig().showDataQuality;
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
    let metadataPropertiesObservable = this.indicatorService.getIndicatorMetadataProperties();
    let metadataObservable = this.indicatorService.getIndicatorMetadata(root.Grouping[0].GroupId);
    let indicatorProfilesObservable = this.profileService.getIndicatorProfiles([root.IID]);

    Observable.forkJoin([metadataPropertiesObservable, metadataObservable, indicatorProfilesObservable]).subscribe(results => {

      this.metadataProperties = results[0];
      let metadataHash: Map<number, IndicatorMetadata> = results[1];
      this.indicatorProfiles = results[2];

      let indicatorMetadata = metadataHash[root.IID];
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
    let metadataPropertiesObservable = this.indicatorService.getIndicatorMetadataProperties();
    let metadataObservable = this.indicatorService.getIndicatorMetadataByIndicatorId(indicatorId, restrictToProfileIds);

    Observable.forkJoin([metadataPropertiesObservable, metadataObservable]).subscribe(results => {

      this.metadataProperties = results[0];
      let metadataHash: Map<number, IndicatorMetadata> = results[1];
      this.indicatorProfiles = null;

      let indicatorMetadata = metadataHash[indicatorId];
      this.displayMetadata(indicatorMetadata);
      this.isReady.emit(true);
    });
  }

  private displayMetadataForAjaxResults() {

  }

  private displayMetadata(indicatorMetadata: IndicatorMetadata, root?: GroupRoot): void {

    this.tempRows = new Array<MetadataRow>();

    // Assign key variables
    let descriptive = indicatorMetadata.Descriptive;

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
      let property = this.metadataProperties[propertyIndex];
      if (property.Order > 59) {
        break;
      }

      // Do not dislay name as full name is displayed
      if ((property.ColumnName !== 'Name')) {
        this.addMetadataRowByProperty(descriptive, property);
      }
    }

    // Value type
    this.addMetadataRow('Value type', indicatorMetadata.ValueType.Name);

    // Text - Methodology
    this.addMetadataRowByProperty(descriptive, this.metadataProperties[propertyIndex++]);

    // Unit
    let unit = indicatorMetadata.Unit.Label;
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
      let row = new MetadataRow('Benchmarking method', '');
      this.tempRows.push(row);
      this.assignBenchmarkingMethod(row, benchmarkingMethodId);
    }

    // Benchmarking significance level
    if (benchmarkingSigLevel) {
      let text = (benchmarkingSigLevel <= 0) ?
        this.NotApplicable :
        benchmarkingSigLevel + '%';
      this.addMetadataRow('Benchmarking significance level', text);
    }

    // Confidence interval method
    let ciMethod = indicatorMetadata.ConfidenceIntervalMethod;
    if (ciMethod) {

      this.addMetadataRow('Confidence interval method', ciMethod.Name);

      // Display CI method description
      let cimDescription = ciMethod.Description;
      if (cimDescription) {
        this.addMetadataRow('Confidence interval methodology', cimDescription);
      }
    }

    // Confidence level
    let confidenceLevel = indicatorMetadata.ConfidenceLevel;
    if (confidenceLevel > -1) {
      this.addMetadataRow('Confidence level', confidenceLevel + '%');
    }

    // Display all remaining properties
    while (propertyIndex < this.metadataProperties.length) {
      this.addMetadataRowByProperty(descriptive, this.metadataProperties[propertyIndex]);
      propertyIndex++;
    }

    this.addIndicatorProfiles(indicatorMetadata.IID);

    // Trigger view refresh
    this.rows = this.tempRows;
  }

  private addIndicatorProfiles(indicatorId: number) {

    let indicatorProfiles = this.indicatorProfiles;

    if (indicatorProfiles) {
      let profiles: string[] = [];
      for (let i = 0; i < indicatorProfiles[indicatorId].length; i++) {
        let profilePerIndicator: ProfilePerIndicator = indicatorProfiles[indicatorId][i];
        profiles.push('<a href="' + profilePerIndicator.Url + '" target="_blank">' + profilePerIndicator.ProfileName + '</a>')
      }

      this.addMetadataRow('Profiles included in', profiles.join(', '));
    }
  }

  private addMetadataRow(header: string, text: string | number): void {
    this.tempRows.push(new MetadataRow(header, text));
  }

  private addMetadataRowByProperty(textMetadata: KeyValuePair<string, string>[], property: IndicatorMetadataTextProperty): void {

    let columnName = property.ColumnName;

    if (textMetadata !== null && textMetadata.hasOwnProperty(columnName)) {
      let text = textMetadata[columnName];

      if (!_.isUndefined(text) && text !== '') {

        let displayText: string;
        if ((columnName === 'DataQuality') && this.showDataQuality) {
          // Add data quality flags instead of number
          let dataQualityCount = parseInt(text);
          displayText = this.ftHelperService.getIndicatorDataQualityHtml(text) + ' ' +
            this.ftHelperService.getIndicatorDataQualityTooltipText(dataQualityCount);
        } else {
          displayText = text;
        }

        let row = new MetadataRow(property.DisplayName, displayText);
        this.tempRows.push(row);
      }
    }
  }

  private assignBenchmarkingMethod(row: MetadataRow, benchmarkingMethodId: number) {
    let metadataObservable = this.indicatorService.getBenchmarkingMethod(benchmarkingMethodId).subscribe(
      data => {
        let method = <ComparatorMethod>data;
        row.text = method.Name;
      },
      error => { });
  }
}

class MetadataRow {
  constructor(public header: string, public text: string | number) { }
}
