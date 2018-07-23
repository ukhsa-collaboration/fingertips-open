'use strict';

/**
* Download namespace
* @module download
*/

var download = {};

/**
* Entry point to displaying download page
* @class goToDownloadPage
*/
function goToDownloadPage() {

    setPageMode(PAGE_MODES.DOWNLOAD);

    if (!areIndicatorsInDomain()) {
        displayNoData();
    } else {

        ajaxMonitor.setCalls(1);
        getArePdfsAvailable();
        ajaxMonitor.monitor(displayDownload);
    }
}

/**
* AJAX call to determine whether or not PDFs are available for the current profile and area type.
* @class getArePdfsAvailable
*/
function getArePdfsAvailable() {
    var model = FT.model;
    var profileId = model.profileId;

    if (!loaded.areaTypesWithPdfs[profileId]) {
        var parameters = new ParameterBuilder(
        ).add('profile_id', profileId);

        ajaxGet('api/profile/area_types_with_pdfs',
            parameters.build(),
            function (obj) {
                loaded.areaTypesWithPdfs[profileId] = obj;
                ajaxMonitor.callCompleted();
            });
    } else {
        ajaxMonitor.callCompleted();
    }
}




/**
* Returns true if PDFs are available for the user to download.
* @class isPdfAvailableForCurrentAreaType
*/
function isPdfAvailableForCurrentAreaType() {
    var areaTypes = loaded.areaTypesWithPdfs[FT.model.profileId];
    var areaTypeId = FT.model.areaTypeId;
    return _.some(areaTypes, function (areaType) {
        return areaType.Id === areaTypeId;
    });
}
/**
* The filename of the PDF preview image.
* @class PdfFileNamer
*/
function PdfFileNamer(profileId) {
    this.name = 'pdf' + profileId + '.png';
}

/**
* Creates and displays the HTML for the download page.
* @class displayDownload
*/
function displayDownload() {

    pages.getContainerJq().html('<div class="row">' +
        getCsvDownloadHtml() + '<div class="col-md-6">' +
         '<div class="row col-md-12">' + getPdfHtml() + '</div>' +
          '<div class="row col-md-12">&nbsp;</div>' +
         '<div class="row col-md-12">' +
          (areAtAGlanceReports() ? getAtAGlanceHtml() : "") +
         '</div>' +
         '</div>' +
         '</div>');

    showAndHidePageElements();

    unlock();

    // Bind time period menu
    $('#report-time-period').change(function (e) {
        download.timePeriod = $(this).find('option:selected').text();
    });
}

function areAtAGlanceReports() {
    var profileId = FT.model.profileId;
    return profileId === ProfileIds.Phof || profileId === ProfileIds.Tobacco;
}


function getCsvDownloadHtml() {
    var model = FT.model,
    menus = FT.menus;

    var areaTypeName = menus.areaType.getName();

    var showSubnational = !isParentCountry() && !FT.model.isNearestNeighbours();
    var groupName = getCurrentDomainName();

    var showAllIndicatorsInProfile = isInSearchMode();
    var showProfile = !showAllIndicatorsInProfile;
    var showGroup = !String.isNullOrEmpty(groupName) && showProfile;
    var showPopulation = false;

    // Too much data to download for all of practice profiles
    if (FT.model.profileId === ProfileIds.PracticeProfiles) {
        showProfile = false;
        showPopulation = true;
    }

    var viewModel = {
        profileName: FT.config.profileName,
        groupName: groupName,
        showGroup: showGroup,
        showProfile: showProfile,
        showAllIndicators: showAllIndicatorsInProfile,
        showPopulation: showPopulation,
        indicatorName: getIndicatorName(getGroupRoot().IID),
        allLabel: 'Data for ' + areaTypeName + ' in England',
        parentLabel: FT.model.isNearestNeighbours() ? ''
            : 'Data for ' + areaTypeName + ' in ' + getParentArea().Name,
        nationalCode: NATIONAL_CODE,
        parentCode: model.parentCode,
        showSubnational: showSubnational,
        showAddresses: model.areaTypeId === AreaTypeIds.Practice,
        excelExportText: download.excelExportText,
        apiUrl: FT.url.bridge + 'api'
    };

    return templates.render('excel', viewModel);
}

function getPdfHtml() {

    var config = FT.config;

    if (!config.areAnyPdfsForProfile) {
        return '';
    }

    var model = FT.model;
    var imgUrl = FT.url.img;

    var profileId = model.profileId;

    var noPdfsMessage = null;
    if (profileId === ProfileIds.SearchResults) {
        noPdfsMessage = 'search results';
    } else if (!isPdfAvailableForCurrentAreaType(model)) {
        noPdfsMessage = FT.menus.areaType.getName();;
    }

    var areaName = areaHash[FT.model.areaCode].Name;

    // Time periods
    var timePeriods = config.staticReportsFolders;
    if (timePeriods.length > 0) {
        download.timePeriod = timePeriods[0];
    }

    // Mental health survey
    var showMentalHealthSurvey = _.contains([ProfileIds.Dementia, ProfileIds.ChildrenYoungPeoplesWellBeing,
    ProfileIds.CommonMentalHealthDisorders, ProfileIds.MentalHealthJsna, ProfileIds.SevereMentalIllness,
    ProfileIds.SuicidePrevention], model.profileId);

    var viewModel = {
        showMentalHealthSurvey: showMentalHealthSurvey,
        noPdfsMessage: noPdfsMessage,
        images: imgUrl,
        fileName: new PdfFileNamer(profileId).name,
        areaName: areaName,
        areaCode: model.areaCode,
        profileId: profileId,
        reportsLabel: config.staticReportsLabel,
        showTimePeriodsMenu: timePeriods.length > 1,
        timePeriods: timePeriods
    };

    return templates.render('pdf', viewModel);
}

function getAtAGlanceHtml() {

    var config = FT.config;
    var model = FT.model;

    var areaName = areaHash[model.areaCode].Name;

    // Time periods
    var timePeriods = config.staticReportsFolders;
    if (timePeriods.length > 0) {
        download.timePeriod = timePeriods[0];
    }

    var parentArea = getParentArea();
    var areaList = [];
    areaList.push({ Name: "England", Code: NATIONAL_CODE });

    // Not all parent areas have reports 
    var parentAreaTypesWithReports = [AreaTypeIds.Region, AreaTypeIds.CombinedAuthorities, AreaTypeIds.County];
    if (_.contains(parentAreaTypesWithReports, parentArea.AreaTypeId)) {
        areaList.push({ Name: parentArea.Short, Code: parentArea.Code });
    }

    areaList.push({ Name: areaName, Code: model.areaCode });

    var viewModel = {
        images: FT.url.img,
        profileId: model.profileId,
        reportsLabel: config.staticReportsLabel,
        showTimePeriodsMenu: timePeriods.length > 1,
        timePeriods: timePeriods,
        areaList: areaList
    };

    return templates.render('atAGlance', viewModel);
}

/**
* Event handler that downloads a PDF (overriden on test site).
* @class exportPdf
*/
function exportPdf(areaCode, area/*optional*/) {
    downloadCachedPdf(areaCode);

    // areaHash will not be defined from area search result but area will be 
    var areaName = isDefined(area) ? area.Name : areaHash[areaCode].Name;

    logEvent('Download', 'PDF', areaName);
}

/**
* Downloads a cached PDF. This function is only used on the live site.
* @class downloadCachedPdf
*/
function downloadCachedPdf(areaCode) {

    var profileId = FT.model.profileId;
    var url;

    if (FT.config.hasStaticReports) {
        downloadStaticReport(areaCode);
        return;
    } else if (profileId === ProfileIds.Liver) {
        // Liver profiles
        url = 'http://www.endoflifecare-intelligence.org.uk/profiles/liver-disease/' + areaCode + '.pdf';
    } else if (profileId === ProfileIds.PracticeProfiles) {
        url = FT.url.practiceProfilePdf + '/gpp/index.php?' + 'CCG=' + FT.model.parentCode + '&PracCode=' + areaCode;
    }
    else {
        url = getPdfUrl(areaCode);
    }

    download.openFile(url);
}

download.openFile = function (url) {
    window.open(url.toLowerCase(), '_blank');
}

function checkStaticReportExistsThenDownload(parametersString) {

    // Check report exists
    ajaxGet('api/static-reports/exists',
        parametersString,
        function (doesReportExist) {
            if (doesReportExist) {
                // Download report
                var url = FT.url.corews + 'static-reports?' + parametersString;
                window.open(url.toLowerCase(), '_blank');
            } else {
                var html = '<div style="padding:15px;"><h3>Sorry, this document is not available</h3></div>';
                var popupWidth = 800;
                var left = ($(window).width() - popupWidth) / 2;
                var top = 500;
                lightbox.show(html, top, left, popupWidth);
            }
        });
}

/**
* Downloads a static document after first checking whether it is available.
* @class downloadStaticReport
*/
function downloadStaticReport(areaCode) {
    var parameters = new ParameterBuilder(
    ).add('profile_key', profileUrlKey
    ).add('file_name', areaCode + '.pdf');

    // Time period (if required)
    if (download.timePeriod) {
        parameters.add('time_period', download.timePeriod);
    }

    var parametersString = parameters.build();
    checkStaticReportExistsThenDownload(parametersString);
}

function downloadStaticReportAtaGlance(areaCode) {
    var url = FT.url.bridge + 'static-reports/' + profileUrlKey + '/at-a-glance/' + areaCode + '.html';

    download.openFile(url);
    logEvent('Download', 'PhofAtAGlance', areaCode);
}


download.addProfileIdParameter = function (parameters) {
    if (!isInSearchMode()) {
        parameters.add('profile_id', FT.model.profileId);
    }
}

/**
* Gets the URL for a PDF.
* @class getPdfUrl
*/
function getPdfUrl(areaCode) {
    return FT.url.pdf + profileUrlKey /*global set elsewhere*/ +
    '/' + areaCode + '.pdf';
}

/**
* Export all indicator metadata for specific profile.
* @class download.exportProfileMetadata
*/
download.exportProfileMetadata = function () {
    var parameters = new ParameterBuilder();
    download.addProfileIdParameter(parameters);
    download.downloadCsvMetadata('by_profile_id', parameters);
}

/**
* Export indicator metadata for profile group
* @class download.exportGroupMetadata
*/
download.exportGroupMetadata = function () {
    var parameters = new ParameterBuilder(
        ).add('group_id', FT.model.groupId);
    download.downloadCsvMetadata('by_group_id', parameters);
}

/**
* Export indicator metadata for single indicator.
* @class download.exportIndicatorMetadata
*/
download.exportIndicatorMetadata = function () {
    var parameters = new ParameterBuilder();
    parameters.add('indicator_ids', getGroupRoot().IID);
    download.addProfileIdParameter(parameters);
    download.downloadCsvMetadata('by_indicator_id', parameters);
}

/**
* Export all indicator metadata for search results.
* @class download.exportAllIndicatorMetadata
*/
download.exportAllIndicatorMetadata = function () {
    var parameters = new ParameterBuilder();
    parameters.add('indicator_ids', getIndicatorIdsParameter());
    download.addProfileIdParameter(parameters);
    download.downloadCsvMetadata('by_indicator_id', parameters);
}

download.downloadCsvData = function (byTerm, parameters) {
    var url = FT.url.corews + 'api/all_data/csv/' + byTerm + '?' + parameters.build();
    download.openFile(url);
}

download.downloadCsvMetadata = function (byTerm, parameters) {
    var url = FT.url.corews + 'api/indicator_metadata/csv/' + byTerm + '?' + parameters.build();
    download.openFile(url);
}

/**
* Export all indicator data for specific profile.
* @class download.exportProfileData
*/
download.exportProfileData = function (parentCode) {
    var parameters = getExportParameters(parentCode);
    download.addProfileIdParameter(parameters);
    download.downloadCsvData('by_profile_id', parameters);
    logEvent('Download', 'DataForProfile', parameters.build());
}

/**
* Export indicator data for profile group
* @class download.exportGroupData
*/
download.exportGroupData = function (parentCode) {
    var parameters = getExportParameters(parentCode);
    parameters.add('group_id', FT.model.groupId);
    download.downloadCsvData('by_group_id', parameters);
    logEvent('Download', 'DataForGroup', parameters.build());
}

/**
* Export indicator data for single indicator.
* @class download.exportIndicatorData
*/
download.exportIndicatorData = function (parentCode) {
    var parameters = getExportParameters(parentCode);
    download.addProfileIdParameter(parameters);
    parameters.add('indicator_ids', getGroupRoot().IID);
    download.downloadCsvData('by_indicator_id', parameters);
    logEvent('Download', 'DataForIndicator', parameters.build());
}

/**
* Export all indicator data for search results.
* @class download.exportAllIndicatorData
*/
download.exportAllIndicatorData = function (parentCode) {
    var parameters = getExportParameters(parentCode);
    parameters.add('indicator_ids', getIndicatorIdsParameter());
    download.addProfileIdParameter(parameters);
    download.downloadCsvData('by_indicator_id', parameters);
    logEvent('Download', 'DataForAllSearchIndicators', parameters.build());
}

/**
* Export addresses for current area type (e.g. GP practices).
* @class download.exportAddresses
*/
download.exportAddresses = function (parentCode) {
    var parameters = new ParameterBuilder()
    .add('parent_area_code', parentCode)
    .add('area_type_id', FT.model.areaTypeId).build();
    var url = FT.url.corews + 'api/area_addresses/csv/by_parent_area_code?' + parameters;
    download.openFile(url);
    logEvent('Download', 'Addresses', parameters);
}

/**
* Export population data
* @class download.exportPopulation
*/
download.exportPopulation = function (parentCode) {
    var parameters = new ParameterBuilder()
        .add('are', parentCode)
        .add('gid', GroupIds.PracticeProfiles.Population)
        .add('ati', FT.model.parentTypeId).build();
    var url = FT.url.corews + 'GetData.ashx?s=db&pro=qp&' + parameters;
    download.openFile(url);
    logEvent('Download', 'Population', parameters);
}

function getExportParameters(parentCode) {
    var model = FT.model;
    var parameters = new ParameterBuilder()
    .add('parent_area_code', parentCode)
    .add('parent_area_type_id', model.parentTypeId)
    .add('child_area_type_id', model.areaTypeId);
    return parameters;
}

pages.add(PAGE_MODES.DOWNLOAD, {
    id: 'download',
    title: 'Download',
    goto: goToDownloadPage,
    gotoName: 'goToDownloadPage',
    needsContainer: true,
    jqIds: ['indicator-menu-div', 'areaMenuBox', 'parentTypeBox', 'areaTypeBox',
        'region-menu-box', 'nearest-neighbour-link']
});

templates.add('pdf',
    '<div id="pdf-download-text" class="text col-md-6"><h2>Area profile</h2>\
{{^noPdfsMessage}}\
<p>Download a detailed report of the data for</p>\
{{#showTimePeriodsMenu}}<p>{{reportsLabel}} <select id="report-time-period">{{#timePeriods}}<option>{{.}}</option>{{/timePeriods}}</select></p>{{/showTimePeriodsMenu}}\
<a class="pdf" href="javascript:exportPdf(\'{{areaCode}}\')">{{areaName}}</a>\
{{/noPdfsMessage}}\
{{#noPdfsMessage}}<p>PDF profiles are not currently available for {{{noPdfsMessage}}}</p>{{/noPdfsMessage}}\
{{#showMentalHealthSurvey}}<br><a href="https://surveys.phe.org.uk/TakeSurvey.aspx?PageNumber=1&SurveyID=82428nlK#" target="_blank">Click here to provide feedback on the PDF</a>{{/showMentalHealthSurvey}}\
</div>\
<div class="col-md-6"><img src="{{images}}download/{{fileName}}"{{#isAvailable}} class="pdf" onclick="exportPdf(\'{{areaCode}}\')"{{/isAvailable}}/></div>');

templates.add('excel',
    '<div id="excel-box" class="text col-md-6"><h2>Get the data</h2>\
<p>Download indicator data and definitions</p>\
{{#showProfile}}<h3>Profile: {{{profileName}}}</h3>\
<a class="excel" href="javascript:download.exportProfileData(\'{{nationalCode}}\')">{{allLabel}}</a>\
{{#showSubnational}}<a class="excel" href="javascript:download.exportProfileData(\'{{parentCode}}\')">{{parentLabel}}</a>{{/showSubnational}}\
<a class="excel" href="javascript:download.exportProfileMetadata()">Indicator definitions</a>{{/showProfile}}\
{{#showGroup}}<h3>Domain: {{{groupName}}}</h3>\
<a class="excel" href="javascript:download.exportGroupData(\'{{nationalCode}}\')">{{allLabel}}</a>\
{{#showSubnational}}<a class="excel" href="javascript:download.exportGroupData(\'{{parentCode}}\')">{{parentLabel}}</a>{{/showSubnational}}\
<a class="excel" href="javascript:download.exportGroupMetadata()">Indicator definitions</a>{{/showGroup}}\
{{#showAllIndicators}}<h3>All indicators</h3>\
<a class="excel" href="javascript:download.exportAllIndicatorData(\'{{nationalCode}}\')">{{allLabel}}</a>\
{{#showSubnational}}<a class="excel" href="javascript:download.exportAllIndicatorData(\'{{parentCode}}\')">{{parentLabel}}</a>{{/showSubnational}}\
<a class="excel" href="javascript:download.exportAllIndicatorMetadata()">Indicator definitions</a>{{/showAllIndicators}}\
<h3>Indicator: {{{indicatorName}}}</h3>\
<a class="excel" href="javascript:download.exportIndicatorData(\'{{nationalCode}}\')">{{allLabel}}</a>\
{{#showSubnational}}<a class="excel" href="javascript:download.exportIndicatorData(\'{{parentCode}}\')">{{parentLabel}}</a>{{/showSubnational}}\
<a class="excel" href="javascript:download.exportIndicatorMetadata()">Indicator definition</a>\
{{#showAddresses}}<h3>GP practice addresses</h3>\
<a class="excel" href="javascript:download.exportAddresses(\'{{nationalCode}}\')">{{allLabel}}</a>\
{{#showSubnational}}<a class="excel" href="javascript:download.exportAddresses(\'{{parentCode}}\')">{{parentLabel}}</a>{{/showSubnational}}{{/showAddresses}}\
{{#showPopulation}}<h3>Population age distribution</h3>\
<a class="excel" href="javascript:download.exportPopulation(\'{{nationalCode}}\')">{{allLabel}}</a>\
{{#showSubnational}}<a class="excel" href="javascript:download.exportPopulation(\'{{parentCode}}\')">{{parentLabel}}</a>{{/showSubnational}}{{/showPopulation}}\
<br><p>{{{excelExportText}}}</p>\
<h2>Get the data with R</h2>\
<p>The <a style="display:inline;" href="https://cran.r-project.org/web/packages/fingertipsR/index.html" target="_blank">fingertipsR</a> package allows you to download public health data using R</p>\
<br/><h2>Public Health Data API</h2>\
<p>The <a style="display:inline;" href="{{apiUrl}}" target="_blank">Fingertips API</a> (Chrome only) allows public health data to be retrieved in either JSON or CSV formats</p>\
</div>'
);

templates.add('atAGlance',
    '<div id="phof-html-download-text"  class="text col-md-6">' +
    '<h2>At a Glance</h2>' +
    '<p>View a web summary report of the data for</p>' +
     '{{#showTimePeriodsMenu}}<p>{{reportsLabel}} <select id="report-time-period">{{#timePeriods}}<option>{{.}}</option>{{/timePeriods}}</select></p>{{/showTimePeriodsMenu}}' +
    '{{#areaList}}' +
    '<a class="pdf" href="javascript:downloadStaticReportAtaGlance(\'{{Code}}\', false)">{{Name}}</a>' +
    '{{/areaList}}' +
    '</div>' +
    '<div class="col-md-6"><img src="{{images}}download/html{{profileId}}-at-a-glance.png"{{#isAvailable}} class="pdf" {{/isAvailable}}/></div>'
);

/**
* Stores results of previous AJAX requests for information on what profiles PDFs are available for.
* @class loaded.areaTypesWithPdfs
*/
loaded.areaTypesWithPdfs = {};

// Text for extra message to be displayed on test site
download.excelExportText = '';

// Optional if a time period needs to be specified for a PDF
download.timePeriod = null;
