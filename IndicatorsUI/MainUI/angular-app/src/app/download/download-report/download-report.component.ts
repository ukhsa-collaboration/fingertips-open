import { Component } from '@angular/core';
import { ProfileService } from '../../shared/service/api/profile.service';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service'
import { FTModel, AreaType, FTConfig, FTUrls, Area } from '../../typings/FT';
import { ParameterBuilder } from '../../shared/shared';
import { ProfileIds, AreaTypeIds, ProfileUrlKeys } from '../../shared/constants';
import { AreaCodes } from '../../shared/constants';
import { StaticReportsService } from '../../shared/service/api/static-reports.service';
import * as _ from 'underscore';
import { LightBoxConfig, LightBoxTypes } from '../../shared/component/light-box/light-box';
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
    public showSexualHealthLaserReport = false;
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
    public dentalHeathTimePeriods = [2015, 2012];
    public selectedTimePeriodForDentalHealth;
    public areasWithPdfs: Area[];

    // Instance data
    private model: FTModel;
    private config: FTConfig;
    private urls: FTUrls;
    private areaTypesWithPdfs: AreaType[];

    constructor(private profileService: ProfileService, private ftHelperService: FTHelperService,
        private staticReportsService: StaticReportsService, private lightBoxService: LightBoxService) {

        // Set default value
        this.selectedTimePeriodForDentalHealth = this.dentalHeathTimePeriods[0];
    }

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

        this.noPdfsMessage = this.getNoPdfsMessage();
        this.arePdfsForCurrentAreaType = this.noPdfsMessage === null;

        // Time periods
        this.timePeriods = this.config.staticReportsFolders;
        this.showTimePeriodsMenu = this.timePeriods.length > 1;
        if (this.showTimePeriodsMenu) {
            this.selectedTimePeriod = this.timePeriods[0];
        }
        this.reportsLabel = this.config.staticReportsLabel;

        this.setShowMentalHealthSurvey();

        // FIN-2392 Commented out as Paul Crook request to not show these yet
        this.showSexualHealthLaserReport = this.profileId === ProfileIds.SexualHealth &&
            _.indexOf([AreaTypeIds.CountyUA, AreaTypeIds.CountyUAPostApr2019], this.model.areaTypeId) > -1;

        this.setTitle();
        this.fileName = this.getPdfFileName(this.profileId);
        this.areaName = this.ftHelperService.getAreaName(this.model.areaCode);

        // At a glance
        this.setAreAtAGlanceReports();
        if (this.areAtAGlanceReports) {
            this.displayAtAGlanceReports();
        }

        // Child Health behaviours
        this.isChildHealthProfile = this.profileId === ProfileIds.ChildHealth;
        if (this.isChildHealthProfile) {
            this.childHealthBehavioursFileName = this.getPdfFileName(ProfileIds.ChildHealthBehaviours);
            this.setUpHIAReports();
        }
    }

    getNoPdfsMessage(): string {
        if (this.model.profileId === ProfileIds.PracticeProfile) {
            return 'Downloadable PDFs for these profiles are currently under review. Unless decided otherwise we intend to make them available again by February 2020.';
        }

        if (this.profileId === ProfileIds.SearchResults) {
            return 'PDF profiles are not currently available for search results';
        }

        if (!this.isPdfAvailableForCurrentAreaType()) {
            return 'PDF profiles are not currently available for ' + this.ftHelperService.getAreaTypeName();
        }

        return null;
    }

    setAreAtAGlanceReports() {
        this.areAtAGlanceReports =
            this.profileId === ProfileIds.Phof
            || this.profileId === ProfileIds.Tobacco
            || (this.profileId === ProfileIds.LAPE && this.ftHelperService.isFeatureEnabled('lapeAtAGlanceReports'));
    }

    displayAtAGlanceReports() {
        this.setAreasWithPdfs([AreaTypeIds.Region, AreaTypeIds.CombinedAuthorities, AreaTypeIds.County]);
    }

    setUpHIAReports() {
        this.setAreasWithPdfs([AreaTypeIds.Region, AreaTypeIds.PheCentres2015]);
    }

    setAreasWithPdfs(parentAreaIdsWithReports: number[]) {
        const areaList = [];

        // Add England
        areaList.push({ Name: 'England', Code: AreaCodes.England });

        // Add parent area
        const parentArea = this.ftHelperService.getParentArea();
        if (_.contains(parentAreaIdsWithReports, this.model.parentTypeId)) {
            // Use short name instead of full name
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
        const areaTypeId = this.model.areaTypeId;
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

    public downloadChildHealthBehavioursPdf() {
        this.downloadStaticReport(this.model.areaCode, ProfileUrlKeys.ChildHealthBehaviours, null,
            900);
    }

    public downloadChildHealthDentalReport() {
        this.downloadStaticReport(this.model.areaCode, ProfileUrlKeys.DentalHealth,
            this.selectedTimePeriodForDentalHealth, 1200);
    }

    public openChildHealthEarlyYearHIAReport(areaCode) {
        this.openHtmlReport('child-health-early-years-high-impact', 1500, areaCode);
    }

    public openChildHealthYoungPeoplesHIAReport(areaCode) {
        this.openHtmlReport('child-health-young-people-high-impact', 1800, areaCode);
    }

    public downloadReport() {
        this.downloadCachedReport(this.model.areaCode);
        this.logReportDownload(this.ftHelperService.getProfileUrlKey());
    }

    /**
    * Downloads a cached PDF. This function is only used on the live site.
    */
    downloadCachedReport(areaCode) {

        const profileId = this.model.profileId;

        if (this.config.hasStaticReports) {
            this.downloadStaticReport(areaCode, this.ftHelperService.getProfileUrlKey(), this.selectedTimePeriod);
        } else if (profileId === ProfileIds.Liver) {
            // Liver profiles
            const url = 'http://www.endoflifecare-intelligence.org.uk/profiles/liver-disease/' + areaCode + '.pdf';
            this.openUrl(url);
        }
    }

    /**
    * Downloads a static document after first checking whether it is available.
    */
    downloadStaticReport(areaCode, profileKey, timePeriod, noDocMessageTop = 600) {

        const fileName = areaCode + '.' + this.getReportExtension(timePeriod);

        this.staticReportsService.doesStaticReportExist(profileKey, fileName, timePeriod).subscribe((doesReportExist) => {
            if (doesReportExist) {
                this.openStaticReport(profileKey, fileName, timePeriod);
            } else {
                this.showDocumentNotExistMessage(noDocMessageTop);
            }
        });
    }

    getReportExtension(timePeriod) {

        let isHtml = false;

        // Health Profiles switched from PDF to HTML reports from 2019
        if (this.profileId === ProfileIds.HealthProfiles) {
            const year = parseInt(timePeriod, 10);
            if (year > 2018) {
                isHtml = true;
            }
        } else if (this.profileId === ProfileIds.HealthProtection) {
            isHtml = true;
        }

        return isHtml ? 'html' : 'pdf';
    }

    openAtAGlanceHtmlReport(areaCode) {

        const urlKey = this.ftHelperService.getProfileUrlKey();
        this.checkHtmlReportExistsThenOpen(areaCode, urlKey + '/at-a-glance');
        this.logReportDownload('AtAGlance-' + urlKey);
    }

    openSplashReport() {
        const urlKey = this.ftHelperService.getProfileUrlKey();
        this.openHtmlReport(urlKey);
    }

    openHtmlReport(urlKey, noDocMessageTop = 600, areaCode = null) {

        areaCode = areaCode !== null ? areaCode : this.model.areaCode;
        this.checkHtmlReportExistsThenOpen(areaCode, urlKey, noDocMessageTop);
        this.logReportDownload(urlKey);
    }

    logReportDownload(report) {
        this.ftHelperService.logEvent('Download-Report', report, this.areaName);
    }

    public setTimePeriod(timePeriod: string) {
        this.selectedTimePeriod = timePeriod;
    }

    public setTimePeriodForDentalHealth(timePeriod: string) {
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

    checkHtmlReportExistsThenOpen(areaCode, profileKey, noDocMessageTop = 600) {

        const fileName = areaCode + '.html';

        this.staticReportsService.doesStaticReportExist(profileKey, fileName, null).subscribe((doesReportExist) => {
            if (doesReportExist) {
                this.openFile(fileName, profileKey);
            } else {
                this.showDocumentNotExistMessage(noDocMessageTop);
            }
        });
    }

    openStaticReport(profileKey: string, fileName: string, timePeriod: string): void {
        this.openFile(fileName, profileKey, timePeriod);
        this.logReportDownload(profileKey);
    }

    openFile(fileName, profileKey, timePeriod = null) {
        let url;

        if (fileName.indexOf('.html') > -1) {
            // HTML
            url = this.urls.bridge + 'static-reports/' + profileKey + '/';
            if (timePeriod) {
                url += timePeriod + '/';
            }

            // Area name is appended to help track in GA
            url += fileName + '?area-name=' + this.areaName;
        } else {
            // PDF

            // Define URL parameters
            const params = new ParameterBuilder();
            params.add('profile_key', profileKey);
            params.add('file_name', fileName);
            if (timePeriod) {
                params.add('time_period', timePeriod);
            }

            // Download file
            url = this.urls.corews + 'static-reports?' + params.build();
        }

        this.openUrl(url);
    }

    openUrl(url: string) {
        window.open(url.toLowerCase(), '_blank');
    }
}
