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
    return _.some(areaTypes, function(areaType) {
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
        getCsvDownloadHtml() + getPdfHtml() + '</div>');

    showAndHidePageElements();

    unlock();

    // Bind time period menu
    $('#report-time-period').change(function (e) {
        download.timePeriod = $(this).find('option:selected').text();
    });
}

function getCsvDownloadHtml() {
    var model = FT.model,
    menus = FT.menus;

    var areaTypeName = menus.areaType.getName();

    var showSubnational = !isParentCountry() && !FT.model.isNearestNeighbours();
    var groupName = getCurrentDomainName();

    var showAllIndicators = isInSearchMode();
    var showProfile = !showAllIndicators;
    var showGroup = !String.isNullOrEmpty(groupName) && showProfile;

    var viewModel = {
        profileName: FT.config.profileName,
        groupName: groupName,
        showGroup: showGroup,
        showProfile: showProfile,
        showAllIndicators: showAllIndicators,
        indicatorName: getIndicatorName(getGroupRoot().IID),
        allLabel: 'Data for ' + areaTypeName + ' in England',
        parentLabel: FT.model.isNearestNeighbours() ? '' : 'Data for ' + areaTypeName + ' in ' + getParentArea().Name,
        nationalCode: NATIONAL_CODE,
        parentCode: model.parentCode,
        showSubnational: showSubnational,
        showAddresses: model.areaTypeId === AreaTypeIds.Practice,
        excelExportText: download.excelExportText
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

    var viewModel = {
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
* Download a child health PDF from the Chimat site.
* @class downloadChildHealthPdf
*/
function downloadChildHealthPdf(areaCode, profileId) {
    ajaxGet('api/area/chimat_resource_id', 'area_code=' + areaCode + '&profile_id=' + profileId,
        function (resourceId) {
            window.open('http://www.chimat.org.uk/resource/view.aspx?RID=' + resourceId, '_blank');
        });
}

/**
* Downloads a cached PDF. This function is only used on the live site.
* @class downloadCachedPdf
*/
function downloadCachedPdf(areaCode) {

    var profileId = FT.model.profileId;
    var url;

    // What about youth profiles are hosted on Chimat site
    if (profileId === ProfileIds.ChiMatWAY || profileId === ProfileIds.ChildHealthBehaviours) {
        downloadChildHealthPdf(areaCode, ProfileIds.ChiMatWAY);
        return;
    } else if (FT.config.hasStaticReports) {
        downloadStaticReport(areaCode);
        return;
    } else if (profileId === ProfileIds.Liver) {
        // Liver profiles
        url = 'http://www.endoflifecare-intelligence.org.uk/profiles/liver-disease/' + areaCode + '.pdf';
    } else if (profileId === ProfileIds.HealthProfiles) {
        // Health profiles
        url = FT.url.pdf + profileUrlKey /*global set elsewhere*/ + '/' + download.timePeriod + '/' + areaCode + '.pdf';
    } else {
        url = getPdfUrl(areaCode);
    }

    download.openFile(url);
}

download.openFile = function (url) {
    window.open(url.toLowerCase(), '_blank');
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
                var top = 200;
                lightbox.show(html, top, left, popupWidth);
            }
        });
}

download.addProfileIdParameter = function (parameters) {
    if (isInSearchMode()) {
        if (isDefined(restrictSearchProfileId)) {
            parameters.add('profile_id', restrictSearchProfileId);
        }
    } else {
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
download.exportProfileData = function(parentCode) {
    var parameters = getExportParameters(parentCode);
    download.addProfileIdParameter(parameters);
    download.downloadCsvData('by_profile_id', parameters);
    logEvent('Download', 'DataForProfile', parameters.build());
}

/**
* Export indicator data for profile group
* @class download.exportGroupData
*/
download.exportGroupData = function(parentCode) {
    var parameters = getExportParameters(parentCode);
    parameters.add('group_id', FT.model.groupId);
    download.downloadCsvData('by_group_id', parameters);
    logEvent('Download', 'DataForGroup', parameters.build());
}

/**
* Export indicator data for single indicator.
* @class download.exportIndicatorData
*/
download.exportIndicatorData = function(parentCode) {
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
    .add('area_type_id',FT. model.areaTypeId).build();
    var url = FT.url.corews + 'api/area_addresses/csv/by_parent_area_code?' + parameters;
    download.openFile(url);
    logEvent('Download', 'Addresses', parameters);
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
    '<div id="pdf-download-text" class="text col-md-3"><h2>Area profile</h2>\
{{^noPdfsMessage}}\
<p>Download a detailed report of the data for</p>\
{{#showTimePeriodsMenu}}<p>{{reportsLabel}} <select id="report-time-period">{{#timePeriods}}<option>{{.}}</option>{{/timePeriods}}</select></p>{{/showTimePeriodsMenu}}\
<a class="pdf" href="javascript:exportPdf(\'{{areaCode}}\')">{{areaName}}</a>\
{{/noPdfsMessage}}\
{{#noPdfsMessage}}<p>PDF profiles are not currently available for {{{noPdfsMessage}}}</p>{{/noPdfsMessage}}</div>\
<div class="col-md-3"><img src="{{images}}download/{{fileName}}"{{#isAvailable}} class="pdf" onclick="exportPdf(\'{{areaCode}}\')"{{/isAvailable}}/></div>');

templates.add('excel',
    '<div id="excel-box" class="text col-md-6"><h2>Get the data</h2>\
<p>Download indicator data and definitions</p>\
{{#showProfile}}<h3>Profile: {{profileName}}</h3>\
<a class="excel" href="javascript:download.exportProfileData(\'{{nationalCode}}\')">{{allLabel}}</a>\
{{#showSubnational}}<a class="excel" href="javascript:download.exportProfileData(\'{{parentCode}}\')">{{parentLabel}}</a>{{/showSubnational}}\
<a class="excel" href="javascript:download.exportProfileMetadata()">Indicator definitions</a>{{/showProfile}}\
{{#showGroup}}<h3>Domain: {{groupName}}</h3>\
<a class="excel" href="javascript:download.exportGroupData(\'{{nationalCode}}\')">{{allLabel}}</a>\
{{#showSubnational}}<a class="excel" href="javascript:download.exportGroupData(\'{{parentCode}}\')">{{parentLabel}}</a>{{/showSubnational}}\
<a class="excel" href="javascript:download.exportGroupMetadata()">Indicator definitions</a>{{/showGroup}}\
{{#showAllIndicators}}<h3>All indicators</h3>\
<a class="excel" href="javascript:download.exportAllIndicatorData(\'{{nationalCode}}\')">{{allLabel}}</a>\
{{#showSubnational}}<a class="excel" href="javascript:download.exportAllIndicatorData(\'{{parentCode}}\')">{{parentLabel}}</a>{{/showSubnational}}\
<a class="excel" href="javascript:download.exportAllIndicatorMetadata()">Indicator definitions</a>{{/showAllIndicators}}\
<h3>Indicator: {{indicatorName}}</h3>\
<a class="excel" href="javascript:download.exportIndicatorData(\'{{nationalCode}}\')">{{allLabel}}</a>\
{{#showSubnational}}<a class="excel" href="javascript:download.exportIndicatorData(\'{{parentCode}}\')">{{parentLabel}}</a>{{/showSubnational}}\
<a class="excel" href="javascript:download.exportIndicatorMetadata()">Indicator definition</a>\
{{#showAddresses}}<h3>GP practice addresses</h3>\
<a class="excel" href="javascript:download.exportAddresses(\'{{nationalCode}}\')">{{allLabel}}</a>\
{{#showSubnational}}<a class="excel" href="javascript:download.exportAddresses(\'{{parentCode}}\')">{{parentLabel}}</a>{{/showSubnational}}{{/showAddresses}}\
<br><p>{{{excelExportText}}}</p></div>');

/**
* Stores results of previous AJAX requests for information on what profiles PDFs are available for.
* @class loaded.areaTypesWithPdfs
*/
loaded.areaTypesWithPdfs = {};

// Text for extra message to be displayed on test site
download.excelExportText = '';

// Optional if a time period needs to be specified for a PDF
download.timePeriod = null;
