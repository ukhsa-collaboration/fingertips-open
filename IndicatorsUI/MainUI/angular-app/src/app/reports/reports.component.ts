import { Component, HostListener } from '@angular/core';
import { forkJoin } from 'rxjs';
import * as _ from 'underscore';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { SsrsReportService } from '../shared/service/api/ssrs-report.service';
import { ContentService } from '../shared/service/api/content.service';
import { SSRSReport, FTModel } from '../typings/FT'

@Component({
  selector: 'ft-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css']
})
export class ReportsComponent {

  public isInitialised = false;
  public hasReports = false;
  public reports: SSRSReport[] = [];
  public reportsToDisplay: ReportToDisplay[] = [];
  public extraNotes: string;

  private model: FTModel;

  constructor(private ftHelperService: FTHelperService, private ssrsReportService: SsrsReportService,
    private contentService: ContentService) { }

  @HostListener('window:ReportsSelected', ['$event'])
  public onOutsideEvent(event) {

    this.model = this.ftHelperService.getFTModel();
    const profileId = this.model.profileId;

    // Get data
    const reportsObservable = this.ssrsReportService.getSsrsReports(profileId);
    const contentObservable = this.contentService.getContent(profileId, 'ssrs-report-extra-notes');

    forkJoin([reportsObservable, contentObservable]).subscribe(results => {
      this.reports = results[0];
      this.extraNotes = results[1];

      this.hasReports = this.reports.length > 0;

      this.loadReportsToDisplay();

      this.isInitialised = true;
      this.ftHelperService.showAndHidePageElements();
      this.ftHelperService.unlock();
    });
  }

  loadReportsToDisplay(): void {
    const areaTypeId = this.model.areaTypeId;

    let reportsToDisplay = [];

    this.reports.forEach(report => {
      const reportToDisplay = new ReportToDisplay();
      reportToDisplay.id = report.Id;
      reportToDisplay.name = report.Name;
      reportToDisplay.notes = report.Notes;
      reportToDisplay.canBeDisplayed = false;

      if (report.AreaTypeIds !== null) {
        const areaTypeIds = report.AreaTypeIds.split(',');
        const areaTypeIdIndex = areaTypeIds.findIndex(x => x === areaTypeId.toString());
        if (areaTypeIdIndex > -1) {
          reportToDisplay.canBeDisplayed = true;
        }
      }

      reportsToDisplay.push(reportToDisplay);
    });

    // Sort reports
    reportsToDisplay = _.sortBy(reportsToDisplay,
      function (report) { return report.name; });

    this.reportsToDisplay = reportsToDisplay;
  }

  openReport(reportId: number, format: string): void {
    // Find the report based on report id
    const report = this.reports.find(x => x.Id === reportId);

    // Get report url and load it in new browser window
    const reportUrl = this.getReportUrl(report, format);
    window.open(reportUrl, '_blank');

    // Log the event
    this.ftHelperService.logEvent('SsrsReportView', format, report.Name);
  }

  getReportUrl(report: SSRSReport, format: string): string {

    const parameters = report.Parameters;
    const model = this.model;
    const url = ['/reports/ssrs/?reportName=', encodeURIComponent(report.File)];

    if (parameters.includes('areaCode')) {
      url.push('&areaCode=' + model.areaCode);
    }

    if (parameters.includes('areaTypeId')) {
      url.push('&areaTypeId=' + model.areaTypeId);
    }

    if (parameters.includes('parentCode')) {
      url.push('&parentCode=' + model.parentCode);
    }

    if (parameters.includes('parentTypeId')) {
      url.push('&parentTypeId=' + model.parentTypeId);
    }

    if (parameters.includes('groupId')) {
      url.push('&groupId=' + model.groupId);
    }

    url.push('&format=' + format);

    return url.join('');
  }
}

export class ReportToDisplay {
  id: number;
  name: string;
  notes: string;
  canBeDisplayed: boolean
}
