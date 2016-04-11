/**
* Download namespace
* @module download
*/

/**
* Entry point to displaying download page
* @class goToDownloadPage
*/
download = {};

/**
* Entry point to displaying download page
* @class goToDownloadPage
*/
function goToDownloadPage() {
    if (!groupRoots.length) {
        noDataForAreaType();
    } else {
        setPageMode(PAGE_MODES.DOWNLOAD);

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
    var key = getArePdfsKey();

    if (!loaded.arePdfs[key]) {
        var parameters = new ParameterBuilder(
        ).add('profile_id', model.profileId
        ).add('area_type_id', model.areaTypeId);

        ajaxGet('data/are_pdfs',
            parameters.build(),
            function (obj) {
                loaded.arePdfs[key] = obj;
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
    return loaded.arePdfs[getArePdfsKey()];
}

/**
* Returns a unique string key that captures the current context as relevant to PDFs.
* @class getArePdfsKey
*/
function getArePdfsKey() {
    var model = FT.model;
    return getKey(model.profileId, model.areaTypeId);
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

    var model = FT.model,
    menus = FT.menus,
    imgUrl = FT.url.img,
    profileId = isDefined(restrictSearchProfileId) ?
        restrictSearchProfileId :
        model.profileId;

    var areaTypeName = menus.areaType.getName(),
    parentCount = menus.parent.count(),
    isMidlandsAndEast = parentCount === 3,
    nationalCode = isMidlandsAndEast ?
        'all'/* Midlands & East */ :
        NATIONAL_CODE,
    allLabel = isMidlandsAndEast ?
        areaTypeName + ' in Midlands and East of England' :
        areaTypeName + ' in England';

    var noPdfsMessage = null;
    if (model.profileId === ProfileIds.SearchResults) {
        noPdfsMessage = 'search results';
    } else if (!isPdfAvailableForCurrentAreaType(model)) {
        noPdfsMessage = areaTypeName;
    }

    var areaName = areaHash[FT.model.areaCode].Name;
    var pdfHtml = areAnyPdfsForProfile ?
        templates.render('pdf', {
            noPdfsMessage: noPdfsMessage,
            images: imgUrl,
            fileName: new PdfFileNamer(profileId).name,
            areaName: areaName,
            areaCode: model.areaCode,
            profileId: model.profileId
        }) :
        '';

    var excelHtml = templates.render('excel', {
        images: imgUrl,
        allLabel: allLabel,
        parentLabel: FT.model.isNearestNeighbours() ? '' : areaTypeName + ' in ' + getParentArea().Name,
        nationalCode: nationalCode,
        parentCode: model.parentCode,
        showSubnational: !isParentCountry(),
        isNotNN: !FT.model.isNearestNeighbours(),
        excelExportText: download.excelExportText
    });

    pages.getContainerJq().html(pdfHtml + excelHtml);

    showAndHidePageElements();

    unlock();
}

/**
* Event handler that downloads a PDF (overriden on test site).
* @class exportPdf
*/
function exportPdf(areaCode) {
    downloadCachedPdf(areaCode);

    // areaHash will not be defined for health profiles area search
    if (areaHash) {
        logEvent('Download', 'PDF', areaHash[areaCode].Name);
    }
}

/**
* Download a child health PDF from the Chimat site.
* @class downloadChildHealthPdf
*/
function downloadChildHealthPdf(areaCode) {
    ajaxGet('data/chimat_resource_id', 'area_code=' + areaCode,
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

    // Child Health profiles are hosted on Chimat site
    if (profileId === ProfileIds.ChildHealth) {
        downloadChildHealthPdf(areaCode);
        return;
    }

    if (FT.isProfileWithOnlyStaticReports) {
        // Static reports
        url = FT.url.corews + 'static-reports?profile_key=' + profileUrlKey + '&file_name=' + areaCode + '.pdf';

        if (download.timePeriod) {
            // Add time period parameter to url
            url = url + '&time_period=' + download.timePeriod;

            // Reset time period
            download.timePeriod = null;
        }
    }
    else if (profileId === ProfileIds.Liver) {
        // Liver profiles
        url = 'http://www.endoflifecare-intelligence.org.uk/profiles/liver-disease/' + areaCode + '.pdf';
    } else {
        url = getPdfUrl(areaCode);
    }
    window.open(url.toLowerCase(), '_blank');
}

/**
* Gets the URL for a PDF.
* @class getPdfUrl
*/
function getPdfUrl(areaCode) {
    return FT.url.pdf + profileUrlKey /*global set elsewhere*/ +
    '/' + areaCode + '.pdf';
}

pages.add(PAGE_MODES.DOWNLOAD, {
    id: 'download',
    title: 'Download',
    goto: goToDownloadPage,
    gotoName: 'goToDownloadPage',
    needsContainer: true,
    jqIds: ['areaMenuBox', 'parentTypeBox', 'areaTypeBox', 'region-menu-box', 'nearest-neighbour-link']
});

templates.add('pdf',
    '<div><img src="{{images}}download/{{fileName}}"{{#isAvailable}} class="pdf" onclick="exportPdf(\'{{areaCode}}\', {{profileId}})"{{/isAvailable}}/></div>\
<div id="pdf-download-text" class="text"><h2>Area profile</h2>\
{{^noPdfsMessage}}<p>Download a detailed profile of the data for</p><a class="pdf" href="javascript:exportPdf(\'{{areaCode}}\', {{profileId}})">{{areaName}}</a>{{/noPdfsMessage}}\
{{#noPdfsMessage}}<p>PDF profiles are not currently available for {{{noPdfsMessage}}}</p>{{/noPdfsMessage}}</div>');

templates.add('excel',
    '<div><img src="{{images}}download/excel.png"/></div>\
<div class="text"><h2>Get the data</h2><p>Download the data as an Excel spreadsheet for</p>\
<a class="excel" href="javascript:exportExcel(\'{{nationalCode}}\')">{{allLabel}}</a>\
{{#showSubnational}}{{#isNotNN}}<a class="excel" href="javascript:exportExcel(\'{{parentCode}}\')">{{parentLabel}}</a>{{/isNotNN}}{{/showSubnational}}' +
    '<br><p>{{excelExportText}}</p></div>');

/**
* Stores results of previous AJAX requests for information on what profiles PDFs are available for.
* @class loaded.arePdfs
*/
loaded.arePdfs = {};

// Text for extra message to be displayed on test site
download.excelExportText = '';

// Optional if a time period needs to be specified for a PDF
download.timePeriod = null;
