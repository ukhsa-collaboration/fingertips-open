'use strict';

function goToAreaProfilePage(areaCode) {

    FT.menus.area.setCode(areaCode);

    setPageMode(PAGE_MODES.AREA_SPINE);

    if (!areIndicatorsInDomain()) {
        displayNoData();
    } else {
        showSpinner();
        callAngularEvent('AreaProfileSelected');
    }
}

pages.add(PAGE_MODES.AREA_SPINE, {
    id: 'areas',
    title: 'Area<br>profiles',
    goto: goToAreaProfilePage,
    gotoName: 'goToAreaProfilePage',
    needsContainer: false,
    jqIds: ['area-profile-container','spine-range-key', 'spineKey', '.geo-menu',
        'spineRangeKeyContainer', 'value-note-legend', 'nearest-neighbour-link', 'trend-marker-legend',
        'area-list-wrapper', 'filter-indicator-wrapper'],
    jqIdsNotInitiallyShown: ['data-quality-key', 'target-benchmark-box', 'key-spine-chart']
});
