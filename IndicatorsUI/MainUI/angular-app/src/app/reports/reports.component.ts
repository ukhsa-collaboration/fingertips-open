import { Component, HostListener } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { SsrsReportService } from '../shared/service/api/ssrs-report.service';
import { ContentService } from '../shared/service/api/content.service';
import { SSRSReport, FTModel } from '../typings/FT.d'

@Component({
  selector: 'ft-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css']
})
export class ReportsComponent {

  public isInitialised = false;
  public hasReports = false;
  public reports: SSRSReport[] = [];
  public extraNotes: string;

  private model: FTModel;

  constructor(private ftHelperService: FTHelperService, private ssrsReportService: SsrsReportService,
    private contentService: ContentService) { }

  @HostListener('window:ReportsSelected', ['$event'])
  public onOutsideEvent(event) {

    this.model = this.ftHelperService.getFTModel();
    const profileId = this.model.profileId;

    // Get data
    let reportsObservable = this.ssrsReportService.getSsrsReports(profileId);
    let contentObservable = this.contentService.getContent(profileId, 'ssrs-report-extra-notes');

    Observable.forkJoin([reportsObservable, contentObservable]).subscribe(results => {
      this.reports = results[0];
      this.extraNotes = results[1];

      this.hasReports = this.reports.length > 0;

      this.isInitialised = true;
      this.ftHelperService.showAndHidePageElements();
      this.ftHelperService.unlock();
    });

  }

  openReport(report: SSRSReport, format: string) {

    let reportUrl = this.getReportUrl(report, format);
    window.open(reportUrl, '_blank');

    this.ftHelperService.logEvent('SsrsReportView', format, report.Name);
  }

  getReportUrl(report: SSRSReport, format: string) {

    var parameters = report.Parameters;

    var model = this.model;

    let url = ["/reports/ssrs/?reportName=", encodeURIComponent(report.File)];

    if (parameters.includes('areaCode')) {
      url.push("&areaCode=" + model.areaCode);
    }

    if (parameters.includes('areaTypeId')) {
      url.push("&areaTypeId=" + model.areaTypeId);
    }

    if (parameters.includes('parentCode')) {
      url.push("&parentCode=" + model.parentCode);
    }

    if (parameters.includes('parentTypeId')) {
      url.push("&parentTypeId=" + model.parentTypeId);
    }

    if (parameters.includes('groupId')) {
      url.push("&groupId=" + model.groupId);
    }

    url.push("&format=" + format);

    return url.join('');
  }

}
