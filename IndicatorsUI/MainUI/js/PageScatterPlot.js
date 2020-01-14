'use strict';

function goToScatterPlotPage() {

    setPageMode(PAGE_MODES.SCATTER_PLOT);

    if (!areIndicatorsInDomain()) {
        displayNoData();
        callAngularEvent('NoDataDisplayed', { 'isEnglandAreaType': isEnglandAreaType() });
    } else {
        showSpinner();
        callAngularEvent('CompareIndicatorSelected');
    }
}

pages.add(PAGE_MODES.SCATTER_PLOT, {
    id: 'compare-indicators',
    title: 'Compare<br>indicators',
    goto: goToScatterPlotPage,
    gotoName: 'goToScatterPlotPage',
    needsContainer: false,
    jqIds: ['compare-indicators-container', '.geo-menu', 'indicator-menu-div', 'nearest-neighbour-link', 'area-list-wrapper',
        'filter-indicator-wrapper'],
    jqIdsNotInitiallyShown: []
});
