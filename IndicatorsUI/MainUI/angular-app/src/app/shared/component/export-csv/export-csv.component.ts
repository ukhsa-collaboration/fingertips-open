import { Component, OnInit, OnChanges, SimpleChanges, Input } from '@angular/core';
import { AngularCsv } from 'angular7-csv';
import { Tabs } from '../../constants';
import { CsvConfig, CsvData } from './export-csv';

@Component({
  selector: 'ft-export-csv',
  templateUrl: './export-csv.component.html',
  styleUrls: ['./export-csv.component.css']
})
export class ExportCsvComponent implements OnChanges {

  @Input() csvConfig: CsvConfig = null;

  ngOnChanges(changes: SimpleChanges) {
    if (changes['csvConfig']) {
      if (this.csvConfig) {
        this.generateCsv();
      }
    }
  }

  generateCsv(): void {
    // Add header if the data is not empty
    const data = this.csvConfig.csvData;
    if (data.length > 0) {
      const headerRow = this.getHeaderRow();
      data.unshift(headerRow);
    }

    // Get the file name
    const fileName = this.getFileName();

    // Download csv
    new AngularCsv(data, fileName);
  }

  getHeaderRow(): CsvData {
    const data = new CsvData();
    data.indicatorId = 'Indicator ID';
    data.indicatorName = 'Indicator Name';
    data.parentCode = 'Parent Code';
    data.parentName = 'Parent Name';
    data.areaCode = 'Area Code';
    data.areaName = 'AreaName';
    data.areaType = 'Area Type';
    data.sex = 'Sex';
    data.age = 'Age';
    data.categoryType = 'Category Type';
    data.category = 'Category';
    data.timePeriod = 'Time period';
    data.value = 'Value';
    data.lowerCiLimit95 = 'Lower CI 95.0 limit';
    data.upperCiLimit95 = 'Upper CI 95.0 limit';
    data.lowerCiLimit99_8 = 'Lower CI 99.8 limit';
    data.upperCiLimit99_8 = 'Upper CI 99.8 limit';
    data.count = 'Count';
    data.denominator = 'Denominator';
    data.valueNote = 'Value note';
    data.recentTrend = 'Recent Trend';
    data.comparedToEnglandValueOrPercentiles = 'Compared to England value or percentiles';
    data.comparedToRegionValueOrPercentiles = 'Compared to parent value or percentiles';
    data.timePeriodSortable = 'Time period Sortable';
    data.newData = 'New data';
    data.comparedToGoal = 'Compared to goal';

    return data;
  }

  getFileName(): string {
    switch (this.csvConfig.tab) {
      case Tabs.AreaProfiles:
        return 'Area profiles';
      case Tabs.BoxPlots:
        return 'Box plots';
      case Tabs.CompareAreas:
        return 'Compare areas';
      case Tabs.CompareIndicators:
        return 'Compare indicators';
      case Tabs.Definitions:
        return 'Definitions';
      case Tabs.Download:
        return 'Download';
      case Tabs.England:
        return 'England';
      case Tabs.Inequalities:
        return 'Inequalities';
      case Tabs.Map:
        return 'Map';
      case Tabs.Overview:
        return 'Overview';
      case Tabs.Population:
        return 'Population';
      case Tabs.Trends:
        return 'Trends';
    }
  }
}
