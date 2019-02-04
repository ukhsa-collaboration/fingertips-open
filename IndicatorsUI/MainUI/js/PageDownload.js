'use strict';

/**
* Entry point to displaying download page
* @class goToDownloadPage
*/
function goToDownloadPage() {

    setPageMode(PAGE_MODES.DOWNLOAD);

    if (!areIndicatorsInDomain()) {
        displayNoData();
    } else {
        callAngularEvent('DownloadSelected');
    }
}

pages.add(PAGE_MODES.DOWNLOAD, {
    id: 'download',
    title: 'Download',
    goto: goToDownloadPage,
    gotoName: 'goToDownloadPage',
    needsContainer: false,
    jqIds: ['download-container', 'indicator-menu-div', 'areaMenuBox', 'parentTypeBox', 'areaTypeBox',
        'region-menu-box', 'nearest-neighbour-link', 'area-list-wrapper', 'filter-indicator-wrapper']
});
