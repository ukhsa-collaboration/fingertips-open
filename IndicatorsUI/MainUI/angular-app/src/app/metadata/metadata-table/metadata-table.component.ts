import {
  Component, HostListener, Input, ViewChild, ElementRef, Output,
  EventEmitter
} from '@angular/core';
import { Observable } from 'rxjs/Observable';
import {
  FTModel, FTRoot, GroupRoot, IndicatorMetadata, TooltipManager, IndicatorMetadataTextProperty,
  KeyValuePair, ComparatorMethod
} from '../../typings/FT.d';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { TooltipHelper } from '../../shared/shared';
import { IndicatorService } from '../../shared/service/api/indicator.service';
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
  private metadataProperties: IndicatorMetadataTextProperty[]

  constructor(private ftHelperService: FTHelperService, private indicatorService: IndicatorService) {
    this.showDataQuality = ftHelperService.getFTConfig().showDataQuality;
  }

  public showInLightbox() {
    this.ftHelperService.showIndicatorMetadataInLightbox(this.table.nativeElement);
  }

  public displayMetadataForGroupRoot(root: GroupRoot): void {

    this.isReady.emit(false);

    // Get data by AJAX
    let metadataPropertiesObservable = this.indicatorService.getIndicatorMetadataProperties();
    let metadataObservable = this.indicatorService.getIndicatorMetadata(root.Grouping[0].GroupId);

    Observable.forkJoin([metadataPropertiesObservable, metadataObservable]).subscribe(results => {

      this.metadataProperties = results[0];
      let metadataHash: Map<number, IndicatorMetadata> = results[1];

      var indicatorMetadata = metadataHash[root.IID];
      this.displayMetadata(indicatorMetadata, root);

      this.ftHelperService.showAndHidePageElements();
      this.ftHelperService.unlock();
      this.isReady.emit(true);
    });
  }

  public displayMetadataForIndicator(indicatorId: number, restrictToProfileIds: number[]): void {

    this.isReady.emit(false);

    // Get data by AJAX
    let metadataPropertiesObservable = this.indicatorService.getIndicatorMetadataProperties();
    let metadataObservable = this.indicatorService.getIndicatorMetadataByIndicatorId(indicatorId, restrictToProfileIds);

    Observable.forkJoin([metadataPropertiesObservable, metadataObservable]).subscribe(results => {

      this.metadataProperties = results[0];
      let metadataHash: Map<number, IndicatorMetadata> = results[1];

      var indicatorMetadata = metadataHash[indicatorId];
      this.displayMetadata(indicatorMetadata);
      this.isReady.emit(true);
    });
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

   if(root){
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
      var property = this.metadataProperties[propertyIndex];
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
    var unit = indicatorMetadata.Unit.Label;
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
    var ciMethod = indicatorMetadata.ConfidenceIntervalMethod;
    if (ciMethod) {

      this.addMetadataRow('Confidence interval method', ciMethod.Name);

      // Display CI method description
      var cimDescription = ciMethod.Description;
      if (cimDescription) {
        this.addMetadataRow('Confidence interval methodology', cimDescription);
      }
    }

    // Confidence level
    var confidenceLevel = indicatorMetadata.ConfidenceLevel;
    if (confidenceLevel > -1) {
      this.addMetadataRow('Confidence level', confidenceLevel + '%');
    }

    // Display all remaining properties
    while (propertyIndex < this.metadataProperties.length) {
      this.addMetadataRowByProperty(descriptive, this.metadataProperties[propertyIndex]);
      propertyIndex++;
    }

    // Trigger view refresh
    this.rows = this.tempRows;
  }

  private addMetadataRow(header: string, text: string | number): void {
    this.tempRows.push(new MetadataRow(header, text));
  }

  private addMetadataRowByProperty(textMetadata: KeyValuePair<string, string>[], property: IndicatorMetadataTextProperty): void {

    var columnName = property.ColumnName;

    if (textMetadata !== null && textMetadata.hasOwnProperty(columnName)) {
      var text = textMetadata[columnName];

      if (!_.isUndefined(text) && text !== '') {

        if ((columnName === 'DataQuality') && this.showDataQuality) {
          // Add data quality flags instead of number
          var dataQualityCount = parseInt(text);
          var displayText = this.ftHelperService.getIndicatorDataQualityHtml(text) + ' ' +
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
