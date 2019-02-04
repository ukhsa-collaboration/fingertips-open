'use strict';

/**
* Population namespace
* @module population
*/

/**
* Entry point to displaying population page
* @class goToPopulationPage
*/
function goToPopulationPage() {
    setPageMode(PAGE_MODES.POPULATION);
    callAngularEvent('PopulationSelected');
}

pages.add(PAGE_MODES.POPULATION, {
    id: 'population',
    title: 'Population',
    goto: goToPopulationPage,
    gotoName: 'goToPopulationPage',
    needsContainer: false,
    jqIds: ['population-container', 'areaMenuBox', 'parentTypeBox', 'areaTypeBox',
        'region-menu-box', 'nearest-neighbour-link', 'area-list-wrapper']
});

function showIndicatorMetadataInLightbox(element) {
    var html = '<div class="definition-table">' + $(element).html() + '</div>';
    var popupWidth = 900;
    var left = lightbox.getLeftForCenteredPopup(popupWidth);
    var top = 200;
    lightbox.show(html, top, left, popupWidth);
}

