'use strict';

/**
* England namespace
* @module england
*/

/**
* Entry point to displaying england page
* @class goToEnglandPage
*/
function goToEnglandPage() {
    if (!groupRoots.length) {
        noDataForAreaType();
    } else {
        setPageMode(PAGE_MODES.ENGLAND);

        ajaxMonitor.setCalls(0);
        getArePdfsAvailable();
        ajaxMonitor.monitor(displayEngland);
    }
}


/**
* Creates and displays the HTML for the england page.
* @class displayEngland
*/
function displayEngland() {

   
    showAndHidePageElements();

    unlock();
}

pages.add(PAGE_MODES.ENGLAND, {
    id: 'england',
    title: 'England',
    goto: goToEnglandPage,
    gotoName: 'goToEnglandPage',
    needsContainer: true,
    jqIds: ['areaMenuBox', 'parentTypeBox', 'areaTypeBox', 'region-menu-box', 'nearest-neighbour-link']
});
