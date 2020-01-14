'use strict';

function goToAreaTrendsPage() {

    setPageMode(PAGE_MODES.AREA_TRENDS);

    if (!areIndicatorsInDomain()) {
        displayNoData();
    } else {
        showSpinner();
        callAngularEvent('AreaTrendSelected');
        adjustScrollTop();
    }
}

pages.add(PAGE_MODES.AREA_TRENDS, {
    id: 'trends',
    title: 'Trends',
    goto: goToAreaTrendsPage,
    gotoName: 'goToAreaTrendsPage',
    needsContainer: false,
    jqIds: [
        'trends-container', 'indicator-select-all-td', 'indicator-menu-div', '.geo-menu', 'trends-chart-sorter-az',
        'key-spine-chart', 'value-note-legend', 'nearest-neighbour-link', 'trend-marker-legend', 'area-list-wrapper',
        'filter-indicator-wrapper'],
    jqIdsNotInitiallyShown: ['data-quality-key', 'key-spine-chart']
});
