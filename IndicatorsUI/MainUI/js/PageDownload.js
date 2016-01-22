/**
* Download namespace
* @module download
*/

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
* @class isPdfAvailable
*/
function isPdfAvailable() {
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
    } else if (!isPdfAvailable(model)) {
        noPdfsMessage = areaTypeName;
    }

    var areaName = areaHash[FT.model.areaCode].Name;
    var pdfHtml = arePdfs ?
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
        excelExportText: excelExportText === null ? '' : excelExportText
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
    logEvent('Download', 'PDF', areaHash[areaCode].Name);
}

/**
* Downloads a cached PDF. This function is only used on the live site.
* @class downloadCachedPdf
*/
function downloadCachedPdf(areaCode) {

    var profileId = FT.model.profileId;

    if (profileId === ProfileIds.Liver) {
        // Liver profiles
        var url = 'http://www.endoflifecare-intelligence.org.uk/profiles/liver-disease/' + areaCode + '.pdf';
    } else {
        url = FT.url.pdf + profileUrlKey /*global set elsewhere*/ +
            '/' + areaCode + '.pdf';
    }

    window.open(url.toLowerCase(), '_blank');
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
<div id="download-text" class="text"><h2>Area profile</h2>\
{{^noPdfsMessage}}<p>Download a detailed profile of the data for</p><a class="pdf" href="javascript:exportPdf(\'{{areaCode}}\', {{profileId}})">{{areaName}}</a>{{/noPdfsMessage}}\
{{#noPdfsMessage}}PDF profiles are not<br>currently available<br>for {{{noPdfsMessage}}}{{/noPdfsMessage}}</div>');

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

excelExportText = null;
