import { Component } from '@angular/core';
import { ProfileService } from '../../shared/service/api/profile.service';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service'
import { FTModel, AreaType, FTConfig, FTUrls, Area } from '../../typings/FT';
import { ProfileIds, ParameterBuilder, AreaCodes, AreaTypeIds, ProfileUrlKeys } from '../../shared/shared';
import { StaticReportsService } from '../../shared/service/api/static-reports.service';
import * as _ from 'underscore';
import { LightBoxTypes, LightBoxConfig } from '../../shared/component/light-box/light-box.component';
import { LightBoxService } from '../../shared/service/helper/light-box.service';

@Component({
  selector: 'ft-download-report',
  templateUrl: './download-report.component.html',
  styleUrls: ['./download-report.component.css']
})
export class DownloadReportComponent {

  public showMentalHealthSurvey = true;
  public showTimePeriodsMenu = true;
  public isChildHealthProfile = false;
  public arePdfsForCurrentAreaType = true;
  public areAnyPdfsForProfile = false;
  public areAtAGlanceReports = false;
  public profileId: number;

  public title;
  public areaName;
  public reportsLabel;
  public fileName;
  public imagesUrl;
  public noPdfsMessage;
  public childHealthBehavioursFileName;
  public timePeriods = [];
  public selectedTimePeriod: string = null;
  public selectedTimePeriodForDentalHealth = '2017';
  public areasWithPdfs: Area[];

  // Instance data
  private model: FTModel;
  private config: FTConfig;
  private urls: FTUrls;
  private areaTypesWithPdfs: AreaType[];

  constructor(private profileService: ProfileService, private ftHelperService: FTHelperService,
    private staticReportsService: StaticReportsService, private lightBoxService: LightBoxService) { }

  public refresh() {

    this.model = this.ftHelperService.getFTModel();
    this.config = this.ftHelperService.getFTConfig();
    this.urls = this.ftHelperService.getURL();

    this.profileService.areaTypesWithPdfs(this.model.profileId).subscribe((areaTypesWithPdfs) => {

      this.areaTypesWithPdfs = areaTypesWithPdfs;

      this.setPdfHtml();
    });
  }

  setPdfHtml() {

    this.areAnyPdfsForProfile = this.config.areAnyPdfsForProfile
    this.imagesUrl = this.urls.img;
    this.profileId = this.model.profileId;

    if (this.profileId === ProfileIds.SearchResults) {
      this.noPdfsMessage = 'search results';
    } else if (!this.isPdfAvailableForCurrentAreaType()) {
      this.noPdfsMessage = this.ftHelperService.getAreaTypeName();
    } else {
      this.noPdfsMessage = null;
    }
    this.arePdfsForCurrentAreaType = this.noPdfsMessage === null;

    // Time periods
    this.timePeriods = this.config.staticReportsFolders;
    this.showTimePeriodsMenu = this.timePeriods.length > 1;
    if (this.showTimePeriodsMenu) {
      this.selectedTimePeriod = this.timePeriods[0];
    }
    this.reportsLabel = this.config.staticReportsLabel;

    this.setShowMentalHealthSurvey();
    this.setTitle();
    this.fileName = this.getPdfFileName(this.profileId);
    this.areaName = this.ftHelperService.getAreaName(this.model.areaCode);

    // At a glance
    this.areAtAGlanceReports =
      this.profileId === ProfileIds.Phof || this.profileId === ProfileIds.Tobacco;
    if (this.areAtAGlanceReports) {
      this.displayAtAGlanceReports();
    }

    // Child Health behaviours
    this.isChildHealthProfile = this.profileId === ProfileIds.ChildHealth;
    if (this.isChildHealthProfile) {
      this.childHealthBehavioursFileName = this.getPdfFileName(ProfileIds.ChildHealthBehaviours);
    }
  }

  displayAtAGlanceReports() {
    let areaList = [];

    // Add England
    areaList.push({ Name: "England", Code: AreaCodes.England });

    // Add parent area 
    let parentArea = this.ftHelperService.getParentArea();
    let parentAreaTypesWithReports = [AreaTypeIds.Region, AreaTypeIds.CombinedAuthorities, AreaTypeIds.County];
    if (_.contains(parentAreaTypesWithReports, parentArea.AreaTypeId)) {
      areaList.push({ Name: parentArea.Short, Code: parentArea.Code });
    }

    // Add current area
    areaList.push(this.ftHelperService.getArea(this.model.areaCode));

    this.areasWithPdfs = areaList;
  }

  setTitle() {
    // Set PDF section title
    this.title = 'Area profile';
    if (this.model.profileId === ProfileIds.ChildHealth) {
      this.title = 'Child health profile';
    }
  }

  /* Returns true if PDFs are available for the user to download.*/
  isPdfAvailableForCurrentAreaType() {
    let areaTypeId = this.model.areaTypeId;
    return _.some(this.areaTypesWithPdfs, function (areaType) {
      return areaType.Id === areaTypeId;
    });
  }

  getPdfFileName(profileId) {
    return 'pdf' + profileId + '.png';
  }

  setShowMentalHealthSurvey() {
    this.showMentalHealthSurvey = _.contains([
      ProfileIds.Dementia,
      ProfileIds.ChildrenYoungPeoplesWellBeing,
      ProfileIds.CommonMentalHealthDisorders,
      ProfileIds.MentalHealthJsna,
      ProfileIds.SevereMentalIllness,
      ProfileIds.SuicidePrevention],
      this.model.profileId);
  }

  public exportChildHealthBehavioursPdf() {
    this.downloadStaticReport(this.model.areaCode, ProfileUrlKeys.ChildHealthBehaviours);
  }

  public exportChildHealthDentalReport() {
    this.downloadStaticReport(this.model.areaCode, ProfileUrlKeys.DentalHealth);
  }

  public exportPdf() {
    this.downloadCachedPdf(this.model.areaCode);
    this.logReportDownload(this.ftHelperService.getProfileUrlKey());
  }

  /**
  * Downloads a cached PDF. This function is only used on the live site.
  */
  downloadCachedPdf(areaCode) {

    const profileId = this.model.profileId;
    let url;

    if (this.config.hasStaticReports) {
      this.downloadStaticReport(areaCode, this.ftHelperService.getProfileUrlKey());
      return;
    } else if (profileId === ProfileIds.Liver) {
      // Liver profiles
      url = 'http://www.endoflifecare-intelligence.org.uk/profiles/liver-disease/' + areaCode + '.pdf';
    } else {
      url = this.getPdfUrl(areaCode);
    }

    this.openFile(url);
  }

  getPdfUrl(areaCode) {
    return this.urls.pdf + this.ftHelperService.getProfileUrlKey() +
      '/' + areaCode + '.pdf';
  }

  /**
  * Downloads a static document after first checking whether it is available.
  */
  downloadStaticReport(areaCode, profileKey) {
    this.checkStaticReportExistsThenDownload(areaCode, profileKey);
  }

  checkStaticReportExistsThenDownload(areaCode, profileKey) {

    const fileName = areaCode + '.pdf';

    // Select time period
    let timePeriod;
    if (profileKey === ProfileUrlKeys.DentalHealth) {
      timePeriod = this.selectedTimePeriodForDentalHealth;
    } else if (profileKey === ProfileUrlKeys.ChildHealthBehaviours) {
      // Time period menu for main report not applicable for this report
      timePeriod = null;
    } else {
      timePeriod = this.selectedTimePeriod;
    }

    this.staticReportsService.doesStaticReportExist(profileKey, fileName, timePeriod).subscribe((doesReportExist) => {
      if (doesReportExist) {
        this.openStaticReport(profileKey, fileName, timePeriod);
      } else {

        const top = profileKey === ProfileUrlKeys.DentalHealth ? 1200 : 600;
        this.showDocumentNotExistMessage(top);
      }
    });
  }

  downloadStaticReportAtaGlance(areaCode) {
    let url = this.urls.bridge + 'static-reports/' +
      this.ftHelperService.getProfileUrlKey() + '/at-a-glance/' + areaCode + '.html';

    this.openFile(url);
    this.logReportDownload('PhofAtAGlance');
  }

  logReportDownload(report) {
    this.ftHelperService.logEvent('Download-Report', report, this.areaName);
  }

  public timePeriodChange(timePeriod: string) {
    this.selectedTimePeriod = timePeriod;
  }

  public timePeriodChangeForDentalHealth(timePeriod: string) {
    this.selectedTimePeriodForDentalHealth = timePeriod;
  }

  showDocumentNotExistMessage(top: number) {
    const config = new LightBoxConfig();
    config.Type = LightBoxTypes.OkCancel;
    config.Title = 'Sorry, this document is not available';
    config.Html = '';
    config.Top = top;
    this.lightBoxService.display(config);
  }

  openStaticReport(profileKey: string, fileName: string, timePeriod: string): void {

    // Define URL parameters
    let params = new ParameterBuilder();
    params.add('profile_key', profileKey);
    params.add('file_name', fileName);
    if (timePeriod) {
      params.add('time_period', timePeriod);
    }

    // Download file
    let url = this.urls.corews + 'static-reports?' + params.build();
    this.openFile(url);
    this.logReportDownload(profileKey);
  }

  openFile(url: string) {
    window.open(url.toLowerCase(), '_blank');
  }
}
