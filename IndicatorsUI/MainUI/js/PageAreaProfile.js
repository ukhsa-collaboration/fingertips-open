'use strict';

function goToAreaProfilePage() {

    setPageMode(PAGE_MODES.AREA_SPINE);

    if (!areIndicatorsInDomain()) {
        displayNoData();
        callAngularEvent('NoDataDisplayed', { 'isEnglandAreaType': isEnglandAreaType() });
    } else {
        showSpinner();
        callAngularEvent('AreaProfileSelected', { 'isEnglandAreaType': isEnglandAreaType() });
    }
}

pages.add(PAGE_MODES.AREA_SPINE, {
    id: 'area-profile',
    title: 'Area<br>profiles',
    goto: goToAreaProfilePage,
    gotoName: 'goToAreaProfilePage',
    needsContainer: false,
    jqIds: ['area-profile-container','spine-range-key', 'spineKey', '.geo-menu',
        'spineRangeKeyContainer', 'value-note-legend', 'nearest-neighbour-link', 'trend-marker-legend',
        'area-list-wrapper', 'filter-indicator-wrapper'],
    jqIdsNotInitiallyShown: ['data-quality-key', 'target-benchmark-box', 'key-spine-chart']
});
